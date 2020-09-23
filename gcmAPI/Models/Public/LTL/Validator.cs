#region Using

using gcmAPI.Models.LTL;
using gcmAPI.Models.Public.LTL.JSON;
using gcmAPI.Models.Public.LTL.XML;
using gcmAPI.Models.Utilities;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml;

#endregion

namespace gcmAPI.Models.Public.LTL
{
    public class Validator
    {
        #region Variables

        string iam = "gcmAPI.Models.Public.LTLValidator";

        QuoteData quoteData;
        LTLPiece[] m_lPiece;
        HelperFuncs.AccessorialsObj AccessorialsObj;

        //RateServiceNotification res;

        #endregion

        #region Constructors

        public Validator(ref QuoteData quoteData)
        {
            this.quoteData = quoteData;
            this.m_lPiece = quoteData.m_lPiece;
            this.AccessorialsObj = quoteData.AccessorialsObj;
        }

        public Validator()
        {
        }

        #endregion

        #region Validate_rating_request
        public RateServiceNotification Validate_rating_request()
        {

            RateServiceNotification res = new RateServiceNotification();

            DB.LogGenera(iam, "m_lPiece.Length", m_lPiece.Length.ToString());

            if (m_lPiece.Length == 0)
            {
                res.Code = "307";
                res.Message = "Please give at least one line item";
                return res;
            }

            bool first_line_has_class = false;

            for (byte i = 0; i < m_lPiece.Length; i++)
            {
                #region Check Class and dimensions

                if (m_lPiece[i].Weight > 0 && m_lPiece[i].FreightClass == "")
                {
                    //if (m_lPiece[i].Length < 1 || m_lPiece[i].Width < 1 || m_lPiece[i].Height < 1)
                    //{
                    //    //throw new RateServiceException("338", "Please enter dimensions for each item to get No NMFC No Freight Class Shipping LTL rates");
                    //    res.Code = "338";
                    //    res.Message = "Please enter dimensions for each item to get No NMFC No Freight Class Shipping LTL rates";
                    //    return res;
                    //}
                    //if (count == 0)
                    //{
                    //    hasFreightClass = false;
                    //}
                    //else if (m_lPiece[i].FreightClass == "" && hasFreightClass == true)
                    //{
                    //    // Not the first lineitem is "no class", but first was with a class
                    //    throw new RateServiceException("337", "Freight Class fields must be either filled in for each line item or all left blank");
                    //}
                    m_lPiece[i].FreightClass = "-1";
                }

                #endregion

                if (Is_valid_freight_class(m_lPiece[i].FreightClass))
                {
                    quoteData.hasFreightClass = true;
                    if (i == 0)
                    {
                        first_line_has_class = true;

                    }
                    else
                    {
                        if (first_line_has_class == false)
                        {
                            res.Code = "337";
                            res.Message = "Freight Class fields must be either filled in for each line item or all left blank";
                            return res;
                        }
                    }
                }
                else if (string.IsNullOrEmpty(m_lPiece[i].FreightClass)) // Empty Freight class 
                {
                    if (i == 0)
                    {
                        first_line_has_class = false;
                    }
                    else
                    {
                        if (first_line_has_class == true)
                        {
                            res.Code = "337";
                            res.Message = "Freight Class fields must be either filled in for each line item or all left blank";
                            return res;
                        }
                    }
                }
                else // Freight class is not valid
                {
                    //res.Code = "337";
                    //res.Message = string.Concat("Freight Class ", m_lPiece[i].FreightClass, " is not valid");
                    //return res;
                }


                if (m_lPiece[i].Commodity.Equals("HHG"))
                {
                    m_lPiece[i].FreightClass = "150";
                }

                if (m_lPiece[i].Length > 0 && m_lPiece[i].Width > 0 || m_lPiece[i].Height > 0)
                {
                    quoteData.hasDimensions = true;
                }
                else
                {
                    // Do nothing
                }

                if (m_lPiece[i].Length > 94 || m_lPiece[i].Width > 94 || m_lPiece[i].Height > 94)
                {
                    //throw new RateServiceException("308", "Your item\'s dimension is 95\" or more. Please contact us at cs@aeslogistics.com or 877-890-2295 x 2 to get a quote.");
                    res.Code = "308";
                    res.Message = "Your item\'s dimension is 95 inches or more. Please contact us at cs@aeslogistics.com or 877-890-2295 x 2 to get a quote.";
                    return res;
                }
                else if (m_lPiece[i].Length > 50 || m_lPiece[i].Width > 48)
                {
                    if (quoteData.AccessorialsObj.LGPU == true || quoteData.AccessorialsObj.LGDEL == true)
                    {
                        //throw new RateServiceException("310", "Items over 50” in Length or 48” Width will not fit on a Liftgate Trailer, Email cs@aeslogistics.com for a rate quote or call 206-214-0341 ext 3");
                        res.Code = "310";
                        res.Message = "Items over 50” in Length or 48” Width will not fit on a Liftgate Trailer, Email cs@aeslogistics.com for a rate quote or call 206-214-0341 ext 3";
                        return res;
                    }
                }
                //else if (m_lPiece[i].Length > 0 || m_lPiece[i].Width > 0 || m_lPiece[i].Height > 0)
                //{
                //    hasDimensions = true;
                //}
                else
                {
                    // Do nothing
                }

                //


            }

            #region Ensure total weight as 500lb for HHG

            // Ensure total weight as 500lb for HHG
            int firstIndexHHG = -1;
            double totalWeight = 0;
            for (int i = 0; i < m_lPiece.Length; i++)
            {
                if (m_lPiece[i].Commodity.Equals("HHG") && firstIndexHHG == -1)
                {
                    firstIndexHHG = i;
                }
                totalWeight += m_lPiece[i].Weight;
            }
            if (firstIndexHHG > -1 && totalWeight < 500)
            {
                m_lPiece[firstIndexHHG].Weight += 500 - totalWeight;
            }
            // End

            #endregion

            #region Alaska

            if (quoteData.destState.Equals("AK") && AccessorialsObj.RESDEL.Equals("RSD"))
            {
                res.Code = "391";
                res.Message =
                    string.Concat("Please contact AES customer service 877-890-2295 for this quote");
                return res;
                // Alaska Residential Delivery
                //throw new RateServiceException("391", "Please contact AES customer service 877-890-2295 for this quote");
            }

            #endregion

            #region Hawaii

            bool isHawaiiRate = false;
            if (quoteData.origState == "HI" || quoteData.destState == "HI")
            {
                isHawaiiRate = true;
                foreach (LTLPiece lpiece in m_lPiece)
                {
                    if (lpiece.Length == 0 || lpiece.Length == 0 || lpiece.Width == 0 || lpiece.Width == 0 ||
                      lpiece.Height == 0 || lpiece.Height == 0)
                    {
                        res.Code = "340";
                        res.Message =
                            string.Concat("Please enter dimensions for each item to get Hawaii rates");
                        return res;
                        //throw new RateServiceException("340", "Please enter dimensions for each item to get Hawaii rates");
                    }
                }
            }

            #endregion

            #region Origin and Destination

            quoteData.origCity = quoteData.origCity.Replace("'", "").Replace("`", "");
            quoteData.destCity = quoteData.destCity.Replace("'", "").Replace("`", "");

            if (String.IsNullOrEmpty(quoteData.origZip))
            {
                //throw new RateServiceException("301", "Please give origin zip code");
                res.Code = "301";
                res.Message =
                    string.Concat("Please give origin zip code");
                return res;
            }
            if (String.IsNullOrEmpty(quoteData.origCity))
            {
                //throw new RateServiceException("302", "Please give origin city");
                res.Code = "302";
                res.Message =
                    string.Concat("Please give origin city");
                return res;
            }
            if (String.IsNullOrEmpty(quoteData.origState))
            {
                //throw new RateServiceException("303", "Please give origin state");
                res.Code = "303";
                res.Message =
                    string.Concat("Please give origin state");
                return res;
            }
            if (String.IsNullOrEmpty(quoteData.destZip))
            {
                //throw new RateServiceException("304", "Please give destination zip code");
                res.Code = "304";
                res.Message =
                    string.Concat("Please give destination zip code");
                return res;
            }
            if (String.IsNullOrEmpty(quoteData.destCity))
            {
                //throw new RateServiceException("305", "Please give destination city");
                res.Code = "305";
                res.Message =
                    string.Concat("Please give destination city");
                return res;
            }
            if (String.IsNullOrEmpty(quoteData.destState))
            {
                //throw new RateServiceException("306", "Please give destination state");
                res.Code = "306";
                res.Message =
                    string.Concat("Please give destination state");
                return res;
            }

            #endregion



            res.Code = "0";
            res.Message = "Success";

            return res;
        }

        #endregion

        #region Validate_request_json()
        public void Validate_request_json(ref string data, ref string url_string, ref string error_response,
            ref string request_format, ref LTLQuoteRequest ltl_quote_request)
        {
            //StringBuilder log = new StringBuilder();

            Parser parser = new Parser();

            bool is_request_valid_json = false;

            Xml_helper xml_helper = new Xml_helper();
            Json_helper json_helper = new Json_helper();

            if ((data.StartsWith("{") && data.EndsWith("}")) || // For object
                (data.StartsWith("[") && data.EndsWith("]"))) // For array
            {
                #region JSON

                //is_json = true; // Request is JSON
                request_format = "JSON";
                //DB.LogGenera(iam,"Public API request_format", request_format);

                is_request_valid_json = json_helper.Is_request_valid_json(ref data);
                if (is_request_valid_json == true)
                {

                    ltl_quote_request = new JavaScriptSerializer().Deserialize<LTLQuoteRequest>(data);
                    
                    //DB.LogGenera(iam,"ltl_quote_request.originZip", ltl_quote_request.originZip);
                    //DB.LogGenera(iam,"ltl_quote_request.originCity", ltl_quote_request.originCity);
                    //DB.LogGenera(iam,"ltl_quote_request.originState", ltl_quote_request.originState);

                    //DB.LogGenera(iam,"ltl_quote_request.originZip", ltl_quote_request.destinationZip);
                    //DB.LogGenera(iam,"ltl_quote_request.destinationCity", ltl_quote_request.destinationCity);
                    //DB.LogGenera(iam,"ltl_quote_request.destinationState", ltl_quote_request.destinationState);

                    //DB.LogGenera(iam,"ltl_quote_request.pickupDate", ltl_quote_request.pickupDate.ToShortDateString());

                    //for (byte i = 0; i < ltl_quote_request.items.Count; i++)
                    //{
                    //    DB.LogGenera(iam,"ltl_quote_request.items[i].freightClass", ltl_quote_request.items[i].freightClass + " " +
                    //        ltl_quote_request.items[i].weight);
                    //}

                    //DB.LogGenera(iam,"additionalServices.TSD", ltl_quote_request.additionalServices.TSD.ToString());

                    //DB.LogGenera(iam,"ltl_quote_request.linealFeet", ltl_quote_request.linealFeet.ToString());

                    //DB.LogGenera(iam,"ltl_quote_request.totalCube", ltl_quote_request.totalCube.ToString());

                }
                else
                {
                    error_response = json_helper.Build_error_response("400", "JSON Request was not properly formatted");
                }

                #endregion
            }
            else
            {
                #region Request format was not valid

                DB.LogGenera(iam, "WS Request was not valid JSON or XML", data);

                error_response = xml_helper.Build_error_response("400", "Request was not properly formatted",
                    ref request_format);

                #endregion
            }
        }

        #endregion

        #region Validate_booking_request_json

        public void Validate_booking_request_json(ref string data, ref string error_response,
            ref string request_format, ref LTLBookRequest ltl_book_request)
        {
            Models.Public.LTL.Helper public_helper = new Models.Public.LTL.Helper();
            Parser parser = new Parser();

            bool is_request_valid_json = false;

            //Xml_helper xml_helper = new Xml_helper();
            Json_helper json_helper = new Json_helper();

            StringBuilder logger = new StringBuilder();

            if ((data.StartsWith("{") && data.EndsWith("}")) || // For object
                (data.StartsWith("[") && data.EndsWith("]"))) // For array
            {
                #region JSON


                request_format = "JSON";
                DB.LogGenera(iam, "Public API request_format", request_format);

                is_request_valid_json = json_helper.Is_request_valid_json(ref data);
                if (is_request_valid_json == true)
                {
                    try
                    {
                        ltl_book_request = new JavaScriptSerializer().Deserialize<LTLBookRequest>(data);
                        if (string.IsNullOrEmpty(ltl_book_request.bookingKey))
                        {
                            error_response = json_helper.Build_error_response("401", "Please give bookingKey");
                            return;
                        }

                        int units = 0;

                        for (byte i = 0; i < ltl_book_request.items.Count; i++)
                        {
                            //units += ltl_book_request.items[i].units;
                        }

                        #region Log info

                        #region Not used
                        /*
                        DB.LogGenera(iam, "ltl_book_request.bookingKey", ltl_book_request.bookingKey);
                        DB.LogGenera(iam,"ltl_book_request.originZip", ltl_book_request.originZip);
                        DB.LogGenera(iam,"ltl_book_request.originCity", ltl_book_request.originCity);
                        DB.LogGenera(iam,"ltl_book_request.originState", ltl_book_request.originState);

                        DB.LogGenera(iam,"ltl_book_request.destinationZip", ltl_book_request.destinationZip);
                        DB.LogGenera(iam,"ltl_book_request.destinationCity", ltl_book_request.destinationCity);
                        DB.LogGenera(iam,"ltl_book_request.destinationState", ltl_book_request.destinationState);

                        DB.LogGenera(iam,"ltl_book_request.pickupDate", ltl_book_request.pickupDate.ToShortDateString());

                        DB.LogGenera(iam,"ltl_book_request.originAddress1", ltl_book_request.originAddress1);
                        DB.LogGenera(iam,"ltl_book_request.originAddress2", ltl_book_request.originAddress2);
                        DB.LogGenera(iam,"ltl_book_request.originState", ltl_book_request.originState);
                          //DB.LogGenera(iam,"additionalServices.TSD", ltl_book_request.additionalServices.TSD.ToString());

                        //DB.LogGenera(iam,"ltl_book_request.linealFeet", ltl_book_request.linealFeet.ToString());

                        //DB.LogGenera(iam,"ltl_book_request.totalCube", ltl_book_request.totalCube.ToString());
                        */
                        #endregion

                        logger.Append(
                        string.Concat(
                            "bookingKey: ", ltl_book_request.bookingKey,
                            "linealFeet: ", ltl_book_request.linealFeet.ToString(),
                            "totalCube: ", ltl_book_request.totalCube.ToString(),
                            "originZip: ", ltl_book_request.originZip,
                            "originCity: ", ltl_book_request.originCity,
                            "originState: ", ltl_book_request.originState,
                            "destinationZip: ", ltl_book_request.destinationZip,
                            "destinationCity: ", ltl_book_request.destinationCity,
                            "destinationState: ", ltl_book_request.destinationState,
                            "pickupDate: ", ltl_book_request.pickupDate.ToShortDateString(),
                            "originAddress1: ", ltl_book_request.originAddress1,
                            "originAddress2: ", ltl_book_request.originAddress2,
                            "originState: ", ltl_book_request.originState

                            )
                        );

                        #region Add freight items to logger

                        for (byte i = 0; i < ltl_book_request.items.Count; i++)
                        {

                            logger.Append(
                            string.Concat(
                               "items[", i, "].freightClass ",
                                ltl_book_request.items[i].freightClass, ", ",
                                "items[", i, "].weight",
                                ltl_book_request.items[i].weight, ", ",

                                "items[", i, "].commodity ",
                                ltl_book_request.items[i].commodity, ", ",
                                "items[", i, "].type ",
                                ltl_book_request.items[i].type, ", ",
                                "items[", i, "].description ",
                                ltl_book_request.items[i].description, ", ",

                                 "items[", i, "].hazmat ",
                                ltl_book_request.items[i].hazmat, ", ",
                                 "items[", i, "].length ",
                                ltl_book_request.items[i].length, ", ",
                                 "items[", i, "].width ",
                                ltl_book_request.items[i].width, ", ",
                                 "items[", i, "].height ",
                                ltl_book_request.items[i].height, ", ",
                                 "items[", i, "].units ",
                                ltl_book_request.items[i].units, ", ",
                                 "items[", i, "].pieces ",
                                ltl_book_request.items[i].pieces, ", ",

                                "items[", i, "].tag ",
                                ltl_book_request.items[i].tag
                                )
                            );

                            #region Not used
                            /*
                            DB.LogGenera(iam,"Item", 
                                string.Concat(
                                    "items[", i, "].freightClass ", 
                                ltl_book_request.items[i].freightClass, ", ",
                                "items[", i, "].weight",
                                ltl_book_request.items[i].weight, ", ",

                                "items[", i, "].commodity ",
                                ltl_book_request.items[i].commodity, ", ",
                                "items[", i, "].type ",
                                ltl_book_request.items[i].type, ", ",
                                "items[", i, "].description ",
                                ltl_book_request.items[i].description, ", ",

                                 "items[", i, "].hazmat ",
                                ltl_book_request.items[i].hazmat, ", ",
                                 "items[", i, "].length ",
                                ltl_book_request.items[i].length, ", ",
                                 "items[", i, "].width ",
                                ltl_book_request.items[i].width, ", ",
                                 "items[", i, "].height ",
                                ltl_book_request.items[i].height, ", ",
                                 "items[", i, "].units ",
                                ltl_book_request.items[i].units, ", ",
                                 "items[", i, "].pieces ",
                                ltl_book_request.items[i].pieces, ", ",

                                "items[", i, "].tag ",
                                ltl_book_request.items[i].tag

                                )
                                

                                );
                                */
                            #endregion

                            #endregion

                        }

                        DB.LogGenera(iam, "Logger", logger.ToString());

                        #endregion
                    }
                    catch (Exception e)
                    {
                        DB.LogGenera(iam, "Deserialize", e.ToString());
                        error_response = json_helper.Build_error_response("400", "");
                    }
                }
                else
                {
                    error_response = json_helper.Build_booking_error_response("400", "JSON Request was not properly formatted");
                }

                #endregion
            }
            else
            {
                #region Request format was not valid

                DB.LogGenera(iam, "WS Request was not valid JSON", data);
                //Xml_helper xml_helper = new Xml_helper();
                //error_response = xml_helper.Build_error_response("400", "Request was not properly formatted",
                //    ref request_format);

                error_response = json_helper.Build_error_response("400", "Request was not properly formatted");


                #endregion
            }
        }

        #endregion

        #region Book_request_vs_BookingKey

        public void Book_request_vs_BookingKey(ref string error_response, ref LTLBookRequest ltl_book_request,
            ref AES_API_info api_info)
        {
            Json_helper json_helper = new Json_helper();

            try
            {
                if(ltl_book_request.items.Count == api_info.m_lPiece.Length)
                {
                    // Do nothing
                    DB.LogGenera(iam, "Book_request_vs_BookingKey, number of items MATCHING", ltl_book_request.bookingKey);
                }
                else
                {
                    DB.LogGenera(iam, "Book_request_vs_BookingKey, number of items NOT matching", 
                        string.Concat("ltl_book_request.items.Count: ", ltl_book_request.items.Count, 
                        " api_info.m_lPiece.Length: ", 
                        api_info.m_lPiece.Length,
                        " ltl_book_request.bookingKey: ", ltl_book_request.bookingKey
                        )
                        );

                    //error_response = json_helper.Build_error_response("400", "");
                }
                //int units = 0;

                //for (byte i = 0; i < ltl_book_request.items.Count; i++)
                //{
                //    //units += ltl_book_request.items[i].units;
                //}
            }
            catch (Exception e)
            {
                DB.LogGenera(iam, "Validate_book_request_info_matches_BookingKey_info", e.ToString());
                //error_response = json_helper.Build_error_response("400", "");
            }
        }

        #endregion

        #region Is_valid_freight_class

        public bool Is_valid_freight_class(string freight_class_str)
        {
            var list = new List<double>
            { 50, 55, 60, 65, 70, 77.5, 85, 92.5, 100, 110, 125, 150, 175, 200, 250, 300, 400, 500 };
            //bool is_valid;

            if (double.TryParse(freight_class_str, out double freight_class))
            {
                //is_valid = list.Contains(freight_class);
                return list.Contains(freight_class);
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Return_error
        //private void Return_error(string code, string message)
        //{

        //}

        #endregion

        #region Not used

        /*
         * else if (data.StartsWith("<") && data.EndsWith(">"))
            {
                #region XML

                is_xml = true; // Request is XML
                request_format = "XML";
                DB.LogGenera(iam,"Public API request_format", request_format);

                //Xml_helper xml_helper = new Xml_helper();
                is_request_valid_xml = xml_helper.Is_request_valid_XML(ref data);
                if (is_request_valid_xml == true)
                {
                    url_string = parser.Get_url_string_from_xml(ref data);
                }
                else
                {
                    error_response = xml_helper.Build_error_response("400", "XML Request was not properly formatted",
                        ref request_format);
                }

                #region Check url_string for errors

                if (url_string == "Could not parse XML")
                {
                    error_response = xml_helper.Build_error_response("1",
                        string.Concat("Could not parse XML data. Please make sure XML is valid. Data was: ",
                        data), ref request_format);

                }
                else if (url_string.Contains("Unrecognized accessorial"))
                {
                    error_response = xml_helper.Build_error_response("350", url_string, ref request_format);
                }
                else
                {
                    // Do nothing
                }

                #endregion

                #endregion
            }
         */


        #region Not used
        //JavaScriptSerializer json_serializer = new JavaScriptSerializer();
        //LTLQuoteRequest quote_request =
        //       (LTLQuoteRequest)json_serializer.DeserializeObject(data);

        //JavaScriptSerializer serializer = new JavaScriptSerializer();

        //var jsonString = serializer.Deserialize<string>(data);

        //HelperFuncs.writeToSiteErrors("GetCarrierRates response jsonString", jsonString);

        // Deserialize
        //LTLQuoteRequest quote_request = 
        //    serializer.Deserialize<LTLQuoteRequest>(jsonString);

        //DB.LogGenera(iam,"quote_request.destinationCity", quote_request.destinationCity);

        //url_string = parser.Get_url_string_from_object(ref data);

        // To convert JSON text contained in string json into an XML node
        //XmlDocument doc = JsonConvert.DeserializeXmlNode(data);
        // XmlDocument doc = JsonConvert.DeserializeXNode(data);
        //XmlDocument doc = (XmlDocument)JsonConvert.DeserializeXmlNode(data);
        //data = doc.ToString();

        //DB.LogGenera(iam,"JSON parsed to XML", data);

        //url_string = parser.Get_url_string_from_xml(ref data);
        //url_string = parser.Get_url_string_from_json(ref data);
        //return url_string;
        #endregion

        #endregion
    }
}