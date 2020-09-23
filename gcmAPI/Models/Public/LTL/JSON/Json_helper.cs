#region Using 

using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Json;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

#endregion

namespace gcmAPI.Models.Public.LTL.JSON
{
    public class Json_helper
    {
        string iam = "Json_helper";

        #region Is_request_valid_json

        public bool Is_request_valid_json(ref string request)
        {
            JsonValue json_value;
            try
            {
                json_value = JsonValue.Parse(request);
                DB.LogGenera(iam,"valid json", json_value.ToString());
                return true;
            }
            catch (FormatException fex)
            {
                //Invalid json format               
                DB.LogGenera(iam, "not valid json fex", fex.ToString());
            }
            catch (Exception ex) //some other exception
            {
                DB.LogGenera(iam, "not valid json ex", ex.ToString());
            }
            return false;
        }
        
        #endregion

        #region Build_response

        public string Build_response(ref GCMRateQuote[] totalQuotes)
        {
            StringBuilder json = new StringBuilder();
           
            json.Append("{ \"LTLRateReply\":{");
            json.Append("\"RateServiceNotification\":{");
            json.Append("\"Code\":0,");
            json.Append("\"Message\":\"Success\"");
            json.Append("},");
            json.Append("\"LTLResult\":{");

            //byte quote_num = 0;

            for (byte i = 0; i < totalQuotes.Length; i++)
            {
                if (i == 0)
                {               
                    json.Append(string.Concat("\"QuoteId\": \"", totalQuotes[i].NewLogId, "\","));
                }
                else
                {
                    // Do nothing
                }

                //quote_num = i + 1;

                json.Append(string.Concat("\"GCMRateQuote_", (i+1), "\":{"));

                json.Append(string.Concat("\"TotalPrice\": \"", totalQuotes[i].TotalPrice, "\","));

                json.Append(string.Concat("\"DisplayName\": \"", 
                    totalQuotes[i].DisplayName.Replace("%2C", ","), "\","));

                json.Append(string.Concat("\"DeliveryDays\": \"", totalQuotes[i].DeliveryDays, "\","));

                if(totalQuotes[i].DisplayName.Contains("Genera"))
                {
                    json.Append(string.Concat("\"isGeneraCarrier\": \"1\","));
                }
                else
                {
                    json.Append(string.Concat("\"isGeneraCarrier\": \"0\","));
                }

                if (totalQuotes[i].DisplayName.Contains("Genera")) // || totalQuotes[i].DisplayName.Contains("Volume")
                {
                    json.Append(string.Concat("\"pickupAvailable\": \"0\","));
                }
                else
                {
                    json.Append(string.Concat("\"pickupAvailable\": \"1\","));
                }



                json.Append(string.Concat("\"CoverageCost\": \"", totalQuotes[i].CoverageCost, "\","));

                json.Append(string.Concat("\"Documentation\": \"", totalQuotes[i].Documentation, "\","));

                json.Append(string.Concat("\"SCAC\": \"", totalQuotes[i].Scac, "\","));
                json.Append(string.Concat("\"CarrierQuoteID\": \"", totalQuotes[i].CarrierQuoteID, "\","));
                //json.Append(string.Concat("\"RulesTarrif\": \"", totalQuotes[i].RulesTarrif, "\","));
                json.Append(string.Concat("\"RulesTarrif\": \"\","));
                json.Append(string.Concat("\"RateType\": \"", totalQuotes[i].RateType, "\","));
                //json.Append(string.Concat("\"RateId\": \"", totalQuotes[i].RateId, "\","));

                json.Append(string.Concat("\"BillTo\": \"", totalQuotes[i].BillTo, "\","));

                json.Append(string.Concat("\"BookingKey\": \"", totalQuotes[i].BookingKey, "\""));
                //totalQuotes[i].
                json.Append("}");

                if(i< (totalQuotes.Length - 1))
                {
                    json.Append(",");
                }
                
            }

            json.Append("}");

            json.Append("}}");
            DB.LogGenera(iam, "json", json.ToString());
            return json.ToString();
            //.Replace("\\","")
            //return totalQuotes.ToJSON().ToString();

        }

        #endregion

        #region Build_error_response
        public string Build_error_response(string code, string message)
        {
            StringBuilder json = new StringBuilder();
            try
            {
                json.Append("{ \"LTLRateReply\":{");
                json.Append("\"RateServiceNotification\":{");
                //json.Append("\"Code\":0,");
                json.Append(string.Concat("\"Code\":", code, ","));
                json.Append(string.Concat("\"Message\":\"", message, "\"},"));

                json.Append("\"LTLResult\":{}");

                json.Append("} }");
                return json.ToString();
            }
            catch(Exception e)
            {
                DB.LogGenera(iam, "Build_error_response", e.ToString());
                return "error";
            }
           
            
        }

        #endregion

        #region Build_booking_error_response
        public string Build_booking_error_response(string code, string message)
        {
            StringBuilder json = new StringBuilder();
            try
            {
                json.Append("{ \"Notification\":{");
                //json.Append("\"RateServiceNotification\":{");
                //json.Append("\"Code\":0,");
                json.Append(string.Concat("\"Code\":", code, ","));
                json.Append(string.Concat("\"Message\":\"", message, "\"},"));

                json.Append("\"BOLURL\":{},");
                json.Append("\"ICURL\":{},");
                json.Append("\"ShipmentId\":{}");

                json.Append(" }");
                return json.ToString();
            }
            catch (Exception e)
            {
                DB.LogGenera(iam, "Build_error_response", e.ToString());
                return "error";
            }


        }
        #endregion

        #region Build_booking_response
        public string Build_booking_response(string code, string message, ref string PNW, ref AES_API_info api_info)
        {
            string freight_class = "";
            if(api_info.m_lPiece != null && api_info.m_lPiece.Length > 0)
            {
                freight_class = api_info.m_lPiece[0].FreightClass;
                DB.LogGenera("Build_booking_response", "freight_class", freight_class);
            }
            else
            {
                // Do nothing
                DB.LogGenera("Build_booking_response", "freight_class", "api_info.m_lPiece was null or empty");
            }

            // 

            int sub_nmfc;
            var repo = new gcmAPI.Models.Public.LTL.Repository();
            repo.Get_sub_nmfc_by_density(ref api_info, out sub_nmfc);
            DB.LogGenera("Build_booking_response", "sub_nmfc", sub_nmfc.ToString());

            StringBuilder json = new StringBuilder();
            try
            {
                json.Append("{ \"Notification\":{");
                //json.Append("\"RateServiceNotification\":{");
                //json.Append("\"Code\":0,");
                json.Append(string.Concat("\"Code\":", code, ","));
                json.Append(string.Concat("\"Message\":\"", message, "\"},"));

                //json.Append("\"BOLURL\":{},");
                //json.Append("\"ICURL\":{},");
                //json.Append("\"ShipmentId\":{}");
                json.Append(string.Concat("\"PNW\":\"", PNW, "\","));
                json.Append(string.Concat("\"NMFC\":\"18260.", sub_nmfc, "\","));
                json.Append(string.Concat("\"Freight Class\":\"", freight_class, "\""));

                json.Append(" }");
                return json.ToString();
            }
            catch (Exception e)
            {
                DB.LogGenera(iam, "Build_error_response", e.ToString());
                return "error";
            }


        }
        #endregion


    }
}