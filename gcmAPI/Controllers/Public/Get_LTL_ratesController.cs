#region Using

using gcmAPI.Models.LTL;
using gcmAPI.Models.Public.LTL;
using gcmAPI.Models.Public.LTL.JSON;
using gcmAPI.Models.Public.LTL.XML;
using gcmAPI.Models.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
//using System.Data.Json;

#endregion

namespace gcmAPI.Controllers
{
    public class Get_LTL_ratesController : ApiController
    {
        string iam = "Get_LTL_ratesController " + AppCodeConstants.mode;
        
        #region POST

        public HttpResponseMessage Post()
        {
            try
            {
                //--
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                //--

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

                string data = this.Request.Content.ReadAsStringAsync().Result;
                
                DB.LogGenera("Get_LTL_ratesController " + AppCodeConstants.mode, "Public API data", data);

                string url_string = "", request_format = "", error_response="", response_string = "",
                    content_type= "application/json";

                LTLQuoteRequest ltl_quote_request=new LTLQuoteRequest();
                QuoteData quoteData = new QuoteData();

                #region Parse/Deserialize request, make sure JSON is valid

                // Parse/Deserialize request, make sure JSON is valid
                Validator vld = new Validator();
                vld.Validate_request_json(ref data, ref url_string, ref error_response,ref request_format,
                    ref ltl_quote_request);

                #endregion

                // Xml_helper xml_helper = new Xml_helper();
                Json_helper json_helper = new Json_helper();

                Models.Public.LTL.Helper public_helper = new Models.Public.LTL.Helper();

                //DB.LogGenera(iam, "Public API url_string", url_string);
                //DB.LogGenera(iam, "Public API error_response", error_response);

                //DB.LogGenera(iam, "Public API url_string", url_string);

                string sql = "";
                if (error_response == "")
                {
                    // Do nothing
                }
                else
                {
                    
                    //return error_response;
                    response_string = error_response;
                    var response_message = public_helper.Get_response_message(ref response_string, ref content_type);
                    sql = string.Concat("INSERT INTO Genera_Rating(QuoteID,Request_Data,Response_Data,TotalCube) VALUES(",
                        "0,'", data, "','", response_string, "',", ltl_quote_request.totalCube,
                        ")");
                    HelperFuncs.ExecuteNonQuery(AppCodeConstants.connStringAesAPI, ref sql, "Get_LTL_ratesController");

                    return response_message;
                  
                }


                // If got to here, request is valid JSON or XML
                // Convert JSON or XML to URL Encoded string and get GCM rates

                url_string = string.Concat("username=", username, "&password=", password,
                    "&mode=ws&subdomain=www&showDLSRates=True",
                    "&isHHG=False&isUSED=False",
                    url_string);
               
                //DB.LogGenera(iam, "Public API url_string", url_string);

                

                Models.LTL.Helper helper = new Models.LTL.Helper();

                helper.Set_parameters(ref ltl_quote_request, ref quoteData);
                quoteData.username = username;
                quoteData.showDLSRates = true;
                quoteData.subdomain = "www";

                #region Check for > 4 line items

                if (quoteData.m_lPiece.Length > 4)
                {
                    DB.LogGenera(iam, "m_lPiece.Length > 4", "true");
                    response_string = json_helper.Build_error_response("35",
                          "Too many freight item nodes, maximum number of item nodes is 4");

                    return public_helper.Get_response_message(ref response_string, ref content_type);
                }
                else
                {
                    DB.LogGenera(iam, "m_lPiece.Length > 4", "false");
                }

                #endregion

                Validator validator = new Validator(ref quoteData);
                RateServiceNotification validation_result = validator.Validate_rating_request();

                #region If validation was not successful return error result

                if (validation_result.Code == "0")
                {
                    // Do nothing
                    DB.LogGenera(iam, "validation_result.Code", validation_result.Code);
                }
                else
                {
                    DB.LogGenera(iam, "validation_result.Code", validation_result.Code);

                    response_string = json_helper.Build_error_response(validation_result.Code, validation_result.Message);

                    var response_message = public_helper.Get_response_message(ref response_string, ref content_type);
                    
                    sql = string.Concat("INSERT INTO Genera_Rating(QuoteID,Request_Data,Response_Data,TotalCube) VALUES(",
                        "0,'", data, "','", response_string, "',", ltl_quote_request.totalCube,
                        ")");
                    HelperFuncs.ExecuteNonQuery(AppCodeConstants.connStringAesAPI, ref sql, "Get_LTL_ratesController");

                    return response_message;

                }

                #endregion

                quoteData.destCity = quoteData.destCity.Replace(" Twp", " Township");
                quoteData.origCity = quoteData.origCity.Replace(" Twp", " Township");

                LTL_Carriers carriers = new LTL_Carriers(quoteData);
                SharedLTL.CarriersResult result = carriers.GetRates();

                #region If any rates were found return rates, if not give error result

                if (result.totalQuotes == null || result.totalQuotes.Length == 0)
                {
                    #region If did not find any rates return error result

                    // If did not find any rates return error result

                    response_string = json_helper.Build_error_response("2",
                            string.Concat("Could not find any rates. Please try again."));

                    var response_message = public_helper.Get_response_message(ref response_string, ref content_type);
                    sql = string.Concat("INSERT INTO Genera_Rating(QuoteID,Request_Data,Response_Data,TotalCube) VALUES(",
                        "0,'", data, "','", response_string, "',", ltl_quote_request.totalCube,
                        ")");
                    HelperFuncs.ExecuteNonQuery(AppCodeConstants.connStringAesAPI, ref sql, "Get_LTL_ratesController");

                    return response_message;

                    #endregion
                }
                else
                {
                    string[] additionalServices = new string[0];

                    string requestId;

                    public_helper.StoreLTLRequestsSql(
                        ref quoteData, ref result.totalQuotes, result.totalQuotes[0].NewLogId, data, response_string, 
                        out requestId);
                    
                    response_string = json_helper.Build_response(ref result.totalQuotes);

                    var response_message = public_helper.Get_response_message(ref response_string, ref content_type);

                    #region INSERT INTO Genera_Rating

                    sql = string.Concat("INSERT INTO Genera_Rating(QuoteID,Request_Data,Response_Data,TotalCube) VALUES(",
                        result.totalQuotes[0].NewLogId, ",'", data, "','", response_string, "',", ltl_quote_request.totalCube,
                        ")");
                    HelperFuncs.ExecuteNonQuery(AppCodeConstants.connStringAesAPI, ref sql, "Get_LTL_ratesController");

                    #endregion

                    #region INSERT INTO ResponseTimes
                    // INSERT INTO ResponseTimes

                    int UPS_freight_milliseconds=0,SMTL_milliseconds=0, BestOvernite_milliseconds = 0, NewPenn_milliseconds = 0,
                        RL_milliseconds = 0,RRD_Truckload_milliseconds=0;

                    foreach(GCMRateQuote quote in result.totalQuotes)
                    {
                        //DB.LogGenera("quote.DisplayName", "quote.DisplayName", quote.DisplayName);
                        if(quote.DisplayName == "UPS - Genera")
                        {
                            UPS_freight_milliseconds = quote.Elapsed_milliseconds;
                        }
                        else if (quote.DisplayName == "SMTL - Genera")
                        {
                            SMTL_milliseconds = quote.Elapsed_milliseconds;
                        }
                        else if (quote.DisplayName == "Best Overnite - Genera")
                        {
                            BestOvernite_milliseconds = quote.Elapsed_milliseconds;
                        }
                        else if (quote.DisplayName == "New Penn - Genera")
                        {
                            NewPenn_milliseconds = quote.Elapsed_milliseconds;
                        }
                        else if (quote.DisplayName == "R&L Carrier - Genera")
                        {
                            RL_milliseconds = quote.Elapsed_milliseconds;
                        }
                        else if (quote.DisplayName == "RRD Truckload")
                        {
                            RRD_Truckload_milliseconds = quote.Elapsed_milliseconds;
                        }
                    }

                    stopwatch.Stop();
                    int elapsedMilliseconds = (int)stopwatch.ElapsedMilliseconds;

                    //int seconds = elapsedMilliseconds / 1000;

                    //int gcm_time = 0;
                    sql = string.Concat("INSERT INTO Rate_response_times(",
                        "QuoteID,RequestID,GCM_API,DLS,UPS_freight,SMTL,BestOvernite,NewPenn,RL,P44,Truckload) VALUES(",
                        result.totalQuotes[0].NewLogId, ",'", requestId, "',", elapsedMilliseconds, ",", result.elapsed_milliseconds_DLS_Genera,
                        ",", UPS_freight_milliseconds, ",", SMTL_milliseconds, ",", BestOvernite_milliseconds, ",", NewPenn_milliseconds,
                        ",", RL_milliseconds, ",", result.elapsed_milliseconds_P44, ",", RRD_Truckload_milliseconds,
                        ")");

                    HelperFuncs.ExecuteNonQuery(AppCodeConstants.conn_string_Genera, ref sql, "Get_LTL_ratesController");

                    #endregion

                    return response_message;
                }

                #endregion
            }
            catch (Exception e)
            {
                #region Unknown error

                DB.LogGenera(iam, "get_ltl_rates_test", e.ToString());
               
                Json_helper json_helper = new Json_helper();
               
                string response_string = "";

                response_string = json_helper.Build_error_response("2",
                           string.Concat("Could not find any rates. Please try again."));

                Models.Public.LTL.Helper public_helper = new Models.Public.LTL.Helper();
                string content_type = "application/json";
                return public_helper.Get_response_message(ref response_string, ref content_type);
              
                #endregion
            }
        }

        #endregion

        #region Not used

        // GET api/get_ltl_rates
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/get_ltl_rates/5
        public string Get(int id)
        {
            return "value";
        }

        // PUT api/get_ltl_rates/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/get_ltl_rates/5
        public void Delete(int id)
        {
        }

        #region Not used
        //public string Post(LTLQuoteRequest quote_request)
        //{
        //    string data = this.Request.Content.ReadAsStringAsync().Result;

        //    DB.LogGenera("Public API data", data);
        //    //string result = await Request.Content.ReadAsStringAsync();
        //    //LTLBooking booking = new LTLBooking();
        //    //HelperFuncs.writeToSiteErrors("test SearchObject", string.Concat(json.origName, " ", json.origEmail));

        //    //LTLBooking booking = new LTLBooking();
        //    //booking.makeBookingViaSOAP(ref quote_request);

        //    //return booking.makeBookingViaSOAP(ref quote_request);
        //    string test = "";
        //    try
        //    {
        //        test = string.Concat("test ", quote_request.originCity);
        //        DB.LogGenera("Public API test", test);
        //    }
        //    catch(Exception e)
        //    {
        //        DB.LogGenera("Public API", e.ToString());
        //    }
        //    return string.Concat("test ");

        //}
        #endregion


        #endregion
    }
}
