#region Using

using gcmAPI.Models.Carriers.DLS;
using gcmAPI.Models.LTL;
using gcmAPI.Models.Public.LTL;
using gcmAPI.Models.Public.LTL.JSON;
using gcmAPI.Models.Public.LTL.XML;
using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Http;

#endregion

namespace gcmAPI.Controllers.Public
{
    public class Create_LTL_bookingController : ApiController
    {
        string iam = "Create_LTL_bookingController";

        #region POST

        public HttpResponseMessage Post()
        {
            string data = "", DLS_PrimaryReferencePNW="", response_string = "";
            try
            {
                #region Authentication

                var re = Request;
                var headers = re.Headers;

                string username = "", password = "";

                if (headers.Contains("username"))
                {
                    username = headers.GetValues("username").First();
                    DB.LogGenera(iam, "username", username);
                }
                else
                {
                    DB.LogGenera(iam, "username", "none found");
                }

                if (headers.Contains("password"))
                {
                    password = headers.GetValues("password").First();
                    DB.LogGenera(iam, "password", password);
                }
                else
                {
                    DB.LogGenera(iam, "password", "none found");
                }

                #endregion

                data = this.Request.Content.ReadAsStringAsync().Result;

                DB.LogGenera(iam, "Public API booking data", data);

                string request_format = "", error_response = "",
                    content_type = "application/json";

                LTLBookRequest ltl_book_request = new LTLBookRequest();
                QuoteData quoteData = new QuoteData();

                //

                Validator vld = new Validator();

                DB.LogGenera(iam, "before", "Validate_booking_request_xml_json");

                // Validate_booking_request_json
                vld.Validate_booking_request_json(ref data, ref error_response, ref request_format,
                    ref ltl_book_request);

                DB.LogGenera(iam, "after", "Validate_booking_request_xml_json");

                Models.Public.LTL.Helper public_helper = new Models.Public.LTL.Helper();

                #region If got an error response from Validator - return error message

                DB.LogGenera(iam, "Public API error_response", error_response);

                if (error_response == "")
                {
                    // Do nothing
                }
                else
                {               
                    response_string = error_response;
                    return public_helper.Get_response_message(ref response_string, ref content_type);                   
                }

                #endregion

                #region Not used, MakeBookingViaSOAP
                //return new HttpResponseMessage()
                //{
                //    Content = new StringContent(response_string, Encoding.UTF8, "application/json")
                //};

                //Models.Public.LTL.Helper public_helper = new Models.Public.LTL.Helper();


                // If got to here, request is valid JSON or XML

                // Make booking via SOAP
                //LTLBooking booking = new LTLBooking();
                //response_string = booking.MakeBookingViaSOAP(ref ltl_book_request, ref username, ref password);
                #endregion

                // Get Calculated freight_class, carrier Mode
                var repo = new gcmAPI.Models.Public.LTL.Repository();
                AES_API_info api_info;
                repo.Get_booking_info_by_booking_key(ltl_book_request.bookingKey, out api_info);

                // Get_carriers_by_QuoteID 
                List<AES_API_info> quote_carriers;
                // Get_carriers_by_QuoteID
                repo.Get_carriers_by_QuoteID(api_info.QuoteId, out quote_carriers);

                //
                // Build email text
                StringBuilder sb = new StringBuilder();

                public_helper.getEmailText(ref sb, ref quote_carriers);


                DB.LogGenera("create ltl booking", "sb carriers count", quote_carriers.Count.ToString());
                DB.LogGenera("create ltl booking", "sb carriers", sb.ToString());

                // Get_items_by_QuoteID
                repo.Get_items_by_QuoteID(api_info.QuoteId, ref api_info);

                // Get_total_cube_by_QuoteID 
                repo.Get_total_cube_by_QuoteID(api_info.QuoteId, ref api_info);

                #region Calculate density 

                if(api_info.total_cube > 0 && api_info.total_weight > 0)
                {
                    api_info.total_density = api_info.total_weight / api_info.total_cube;
                    DB.LogGenera(iam, "api_info.total_density", api_info.total_density.ToString());
                }
                else
                {
                    DB.LogGenera(iam, "could not set density", "total cube or total weight 0");
                }

                #endregion

                string sql = "";

                #region Check for Genera carrier, if true return before getting PNW

                DB.LogGenera(iam, "before DLS Shipment Import", "before DLS Shipment Import");
                DB.LogGenera(iam, "api_info.CarrierDisplayName", api_info.CarrierDisplayName);

                if (api_info.CarrierDisplayName.Contains("Genera"))
                {
                    response_string = "Genera carrier, could not get PNW";
                    // Return error
                    return public_helper.Get_response_message(ref response_string, ref content_type);
                }
                else
                {
                    // Do nothing
                }

                #endregion

                //

                // Book_request_vs_BookingKey
                vld.Book_request_vs_BookingKey(ref error_response, ref ltl_book_request, ref api_info);

                //

                #region DLS Shipment Import

                HelperFuncs.dlsShipInfo dlsInfo = new HelperFuncs.dlsShipInfo();
                HelperFuncs.DispatchInfo dispatchInfo = new HelperFuncs.DispatchInfo();
                HelperFuncs.AccessorialsObj AccessorialsObj = new HelperFuncs.AccessorialsObj();

                // Get_accessorials_by_RequestID
                repo.Get_accessorials_by_RequestID(ref AccessorialsObj, ref api_info);

                DLS_ShipmentImport shipment_import = 
                    new DLS_ShipmentImport(ref dlsInfo, ref dispatchInfo, ref api_info, ref AccessorialsObj,
                    ref username, "0");

                shipment_import.Set_DLS_ShipmentImport_objects(ref ltl_book_request, ref api_info);

                //DB.LogGenera("dispatchInfo.username", dispatchInfo.username);

                DB.LogGenera(iam, "before ShipmentImportDLS, mode:", AppCodeConstants.mode);

                if(AppCodeConstants.mode == AppCodeConstants.prod)
                {
                    DB.LogGenera(iam, "ShipmentImportDLS, mode:", "is prod");
                    // Prod ShipmentImportDLS               
                    shipment_import.ShipmentImportDLS(ref DLS_PrimaryReferencePNW, ref quote_carriers, ref sb);

                }
                else if (AppCodeConstants.mode == AppCodeConstants.demo)
                {
                    DB.LogGenera(iam, "ShipmentImportDLS, mode:", "is demo");
                    // Demo ShipmentImportDLS 
                    shipment_import.test_email(ref DLS_PrimaryReferencePNW, ref quote_carriers, ref sb);

                    Random rnd = new Random();
                    int rand = rnd.Next();

                    DLS_PrimaryReferencePNW = string.Concat("PNW", rand);
                }
                else
                {
                    DB.LogGenera(iam, "ShipmentImportDLS, mode:", "is different");
                    // A different mode
                }  

                //

                DB.LogGenera(iam, "after ShipmentImportDLS", "after ShipmentImportDLS");

                #endregion

                if(DLS_PrimaryReferencePNW=="")
                {
                    throw new Exception("Did not get PNW");
                }

                #region INSERT INTO Segments.CarrierQuoteNum

                // Only for Live GCM bookings
                string[] tokens = new string[4];
                tokens[0] = "shipmentID";
                tokens[1] = ":";
                tokens[2] = "\"";
                tokens[3] = "\"";

                int shipmentID = 0;
                // Scrape the shipment id from the request string
                string shipment_id = HelperFuncs.scrapeFromPage(tokens, data);

                DB.LogGenera(iam, "shipment_id", shipment_id);

                if (int.TryParse(shipment_id, out shipmentID) && shipmentID > 0)
                {
                    // This happens only when Genera user booked in Live GCM
                    // Update CarrierQuoteNum
                    HelperFuncs.updateCarrierQuoteNum(ref shipmentID, ref DLS_PrimaryReferencePNW);
                }
                else
                {
                    // Regular booking, from the API
                    // Do nothing
                }

                //dynamic dyn = JsonConvert.DeserializeObject(doc);

                #endregion

                //response_string = DLS_PrimaryReferencePNW;

                DB.LogGenera(iam, "get_ltl_book_test", DLS_PrimaryReferencePNW);

                //
                repo.UpdateStatsQuoteWasBooked(api_info.QuoteId, DLS_PrimaryReferencePNW, 
                    ltl_book_request.destinationCompany.Replace("'", "''"));
                //

                Json_helper json_helper = new Json_helper();
                response_string = json_helper.Build_booking_response("0", "success", ref DLS_PrimaryReferencePNW, ref api_info);

                #region INSERT INTO Genera_Booking

                //
                string PO = "";
                if (!string.IsNullOrEmpty(ltl_book_request.poNumber))
                {
                    PO = ltl_book_request.poNumber;
                }
                //

                data = data.Replace("'", "''");

                sql = string.Concat("INSERT INTO Genera_Booking_2(Booking_request,Booking_request_escaped,Booking_response,PNW,PO) VALUES(",
                   "'", data, "','",
                   data.Replace("\"", "\\\""), "','",
                   response_string.Replace("'", "''"), "','", 
                   DLS_PrimaryReferencePNW, "','",
                   PO.Replace("'", "''"),
                   "')");

                HelperFuncs.ExecuteNonQuery(AppCodeConstants.connStringAesAPI, ref sql, "Get_LTL_ratesController");

                #endregion

                return public_helper.Get_response_message(ref response_string, ref content_type);

            }
            catch (Exception e)
            {
                #region Unknown error

                DB.LogGenera(iam, "get_ltl_book_test", e.ToString());              
                Json_helper json_helper = new Json_helper();            
                response_string = json_helper.Build_booking_error_response("2", 
                    string.Concat("Could not create booking. Please try again."));

                Models.Public.LTL.Helper public_helper = new Models.Public.LTL.Helper();
                string content_type = "application/json";

                string sql = string.Concat("INSERT INTO Genera_Booking_2(Booking_request,Booking_Response,PNW) VALUES(",
                   "'", data, "','", response_string, "','", DLS_PrimaryReferencePNW, "')");

                HelperFuncs.ExecuteNonQuery(AppCodeConstants.connStringAesAPI, ref sql, "Get_LTL_ratesController");

                return public_helper.Get_response_message(ref response_string, ref content_type);
             
                #endregion
            }
        }

        #endregion

        #region Not used
        // GET api/create_ltl_booking
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/create_ltl_booking/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/create_ltl_booking
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/create_ltl_booking/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/create_ltl_booking/5
        //public void Delete(int id)
        //{
        //}
        #endregion
    }
}
