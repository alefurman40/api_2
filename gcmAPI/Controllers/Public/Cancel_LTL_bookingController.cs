#region Using

using gcmAPI.Models.Public.LTL.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using gcmAPI.Models.Utilities;
using static gcmAPI.Models.Utilities.Mail;
using gcmAPI.Models.Public.LTL;
using System.Web.Script.Serialization;

#endregion

namespace gcmAPI.Controllers.Public
{
    public class Cancel_LTL_bookingController : ApiController
    {
        string iam = "Cancel_LTL_bookingController";
     
        #region POST
        public HttpResponseMessage Post()
        {
            try
            {
                #region Authentication

                var re = Request;
                var headers = re.Headers;

                string username = "", password = "";

                if (headers.Contains("username"))
                {
                    username = headers.GetValues("username").First();
                    DB.LogGenera(iam,"username", username);
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

                string data = this.Request.Content.ReadAsStringAsync().Result;

                DB.LogGenera(iam, "Public API cancel_ltl_book_test data", data);

                LTLCancelBookRequest ltl_cancel_book_request = new LTLCancelBookRequest();
                ltl_cancel_book_request = new JavaScriptSerializer().Deserialize<LTLCancelBookRequest>(data);

                Models.Public.LTL.Helper public_helper = new Models.Public.LTL.Helper();
                string content_type = "application/json";
                string response_string = "success";

                if(ltl_cancel_book_request== null || ltl_cancel_book_request.PNW == null)
                {
                    response_string = "To cancel booking please give a valid PNW number, you gave: null";
                    return public_helper.Get_response_message(ref response_string, ref content_type);
                }


                if (ltl_cancel_book_request.PNW.Contains("PNW"))
                {
                    // Do nothing
                }
                else
                {
                    response_string = "To cancel booking please give a valid PNW number, you gave: " + ltl_cancel_book_request.PNW;
                    return public_helper.Get_response_message(ref response_string, ref content_type);
                }

                string po = "", our_first_carrier = "", genera_first_carrier = "", 
                    our_first_cost_str = "", genera_first_cost_str = "";
                decimal our_first_cost = 0m, genera_first_cost = 0m;

                public_helper.Get_cancelled_pnw_info(ltl_cancel_book_request.PNW, ref po, 
                    ref our_first_carrier, ref genera_first_carrier, ref our_first_cost, ref genera_first_cost);

                //
                if(genera_first_cost > 0m)
                {
                    genera_first_cost_str = genera_first_cost.ToString();
                }
                //

                //StringBuilder email_body = new StringBuilder();
                //email_body.Append(string.Concat("Please cancel Pickup for ", ltl_cancel_book_request.PNW));

                #region Send email to CS
                // Send email to CS
                EmailInfo info = new EmailInfo();
                //info.body = string.Concat("Please cancel Pickup for ", ltl_cancel_book_request.PNW);
                info.body = string.Concat("Please cancel Pickup for ", ltl_cancel_book_request.PNW, "<br><br>",
                    "<span style='font-weight: bold;'>BOL/PO#</span>: ", po, "<br><br>",
            "<span style='font-weight: bold;'>Our carrier</span>: ", our_first_carrier, "<br><br>",
            "<span style='font-weight: bold;'>Rate</span>: ", our_first_cost, "<br><br>",
            "<span style='font-weight: bold;'>Genera carrier</span>: ", genera_first_carrier, "<br><br>",
            "<span style='font-weight: bold;'>Rate</span>: ", genera_first_cost_str, "<br><br>"
            
            );
                info.fromAddress = AppCodeConstants.Alex_email;
                info.to = string.Concat("cs" + AppCodeConstants.email_domain, " ", AppCodeConstants.BobsEmail, " ", AppCodeConstants.AnnesEmail);
                info.fromName = "Genera";
                //info.bcc = AppCodeConstants.Alex_email;
                info.subject = "Cancellation # " + ltl_cancel_book_request.PNW;

                Mail mail = new Mail(ref info);
                mail.SendEmail();

                #endregion

                #region Update DB tables

                string sql = string.Concat("INSERT INTO Genera_Cancel_Booking(PNW) VALUES(",
                  "'", ltl_cancel_book_request.PNW, "')");

                HelperFuncs.ExecuteNonQuery(AppCodeConstants.connStringAesAPI, ref sql, "Cancel_LTL_bookingController");

                DB.LogGenera(iam, "cancel_ltl_book_test", response_string);

                //

                //sql = string.Concat("UPDATE SQL_STATS_GCM SET PNW='Cancelled' ",
                //    "WHERE PNW='", ltl_cancel_book_request.PNW, "'");
                //HelperFuncs.ExecuteNonQuery(AppCodeConstants.connStringRater2009, ref sql, "Cancel_LTL_bookingController");

                sql = string.Concat("UPDATE SQL_STATS_GCM SET PNW_Cancelled='True' ",
                    "WHERE PNW='", ltl_cancel_book_request.PNW, "'");
                HelperFuncs.ExecuteNonQuery(AppCodeConstants.connStringRater2009, ref sql, "Cancel_LTL_bookingController");


                //

                //response_string = "test done";

                #endregion

                return public_helper.Get_response_message(ref response_string, ref content_type);

            }
            catch (Exception e)
            {
                #region Unknown error

                DB.LogGenera(iam, "cancel_ltl_book_test", e.ToString());
                //Xml_helper xml_helper = new Xml_helper();
                Json_helper json_helper = new Json_helper();
                // string request_format = "XML";
                //return xml_helper.Build_error_response("2",
                //            string.Concat("Could not find any rates. Please try again."), ref request_format);
                string response_string = json_helper.Build_error_response("2",
                    string.Concat("Could not cancel booking. Please try again."));

                Models.Public.LTL.Helper public_helper = new Models.Public.LTL.Helper();
                string content_type = "application/json";
                return public_helper.Get_response_message(ref response_string, ref content_type);
                //return new HttpResponseMessage()
                //{
                //    Content = new StringContent(response_string, Encoding.UTF8, "application/json")
                //};
                #endregion
            }
        }

        #endregion

        #region Not used
        // GET api/cancel_ltl_booking
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/cancel_ltl_booking/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/cancel_ltl_booking
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/cancel_ltl_booking/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/cancel_ltl_booking/5
        //public void Delete(int id)
        //{
        //}
        #endregion
    }
}
