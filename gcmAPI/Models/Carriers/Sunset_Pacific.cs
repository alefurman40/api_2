using gcmAPI.Models.LTL;
using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace gcmAPI.Models.Carriers
{
    public class Sunset_Pacific
    {
        CarrierAcctInfo acctInfo;
        QuoteData quoteData;

        // Constructor
        public Sunset_Pacific(ref CarrierAcctInfo acctInfo, ref QuoteData quoteData)
        {
            this.acctInfo = acctInfo;
            this.quoteData = quoteData;
        }

        #region Get_Sunset_Pacific_access_token

        public void Get_Sunset_Pacific_access_token(ref string access_token)
        {
            try
            {
                string data = string.Concat("grant_type=password&username=", AppCodeConstants.sunset_un,
                    "&password=", AppCodeConstants.sunset_pwd
                    );

                Web_client http = new Web_client
                {
                    url = "https://api.sunsetpacific.com/token",
                    content_type = "application/x-www-form-urlencoded",
                    accept = "application/json",
                    post_data = data,
                    method = "POST"
                };

                string doc = http.Make_http_request();

                #region Parse result

                string[] tokens = new string[4];
                tokens[0] = "access_token";
                tokens[1] = ":";
                tokens[2] = "\"";
                tokens[3] = "\"";

                access_token = HelperFuncs.scrapeFromPage(tokens, doc);

                #endregion

            }
            catch (Exception e)
            {
                string str = e.ToString();
                DB.Log("Get_Sunset_Pacific_access_token", str);
            }
        }

        #endregion

        #region Get_Sunset_Pacific_rates

        public void Get_Sunset_Pacific_rates(ref string access_token, ref Sunset_P_Res sunset_volume_result)
        {
            try
            {
                if (quoteData.AccessorialsObj.TRADEPU.Equals(true) || quoteData.AccessorialsObj.TRADEDEL.Equals(true)
                    
                    || quoteData.AccessorialsObj.APTPU.Equals(true) 
                    || quoteData.AccessorialsObj.APTDEL.Equals(true)

                    || quoteData.AccessorialsObj.LGPU.Equals(true)
                    || quoteData.AccessorialsObj.LGDEL.Equals(true)

                    || quoteData.AccessorialsObj.RESPU.Equals(true)
                    || quoteData.AccessorialsObj.RESDEL.Equals(true)

                    || quoteData.AccessorialsObj.CONPU.Equals(true)
                    || quoteData.AccessorialsObj.CONDEL.Equals(true)

                    || quoteData.AccessorialsObj.INSDEL.Equals(true)

                    || quoteData.isHazmat == true
                    //|| quoteData.AccessorialsObj..Equals(true)

                    )
                {
                    throw new Exception("Accessorials not supported");
                }

                #region Build Items string

                int total_units = 0;

                StringBuilder items = new StringBuilder();

                for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
                {
                    // Guard
                    if (quoteData.m_lPiece[i].Length > 48 || quoteData.m_lPiece[i].Width > 48)
                    {
                        throw new Exception("Overlength for volume Sunset Pacific");
                        //return;
                    }

                    //items.Append(string.Concat(
                    //    "{ \"total_weight\":", quoteData.m_lPiece[i].Weight,
                    //        ", \"length\":48, \"width\":48, \"height\":70, \"units\":", quoteData.m_lPiece[i].Units,
                    //        ", \"freight_class\":", quoteData.m_lPiece[i].FreightClass, " }"));

                    //if (i == quoteData.m_lPiece.Length - 1) // Last iteration
                    //{
                    //    // Do nothing
                    //}
                    //else
                    //{
                    //    //DB.Log("P44 ", "i not equal to length - 1");
                    //    items.Append(",");
                    //}

                    //

                    total_units += quoteData.m_lPiece[i].Units;
                }

                DB.Log("Sunset Pacific items", items.ToString());

                #endregion


                // Guard
                if (total_units < 4)
                {
                    throw new Exception("Less than 4 units for volume Sunset Pacific");
                    //return;
                }

                int Total_lineal_feet = total_units * 2;

                if(quoteData.linealFeet > 0.0) // Requested by XML GCM API
                {
                    Total_lineal_feet = Convert.ToInt32(quoteData.linealFeet);
                }

                string month = quoteData.puDate.Month.ToString();
                if (month.Length == 1)
                {
                    month = "0" + month;
                }

                string day = quoteData.puDate.Day.ToString();
                if (day.Length == 1)
                {
                    day = "0" + day;
                }

                int orig_zip_code = 0;

                int.TryParse(quoteData.origZip, out orig_zip_code);

                DB.Log("Get_Sunset_Pacific_rates dest_zip_code", orig_zip_code.ToString());

                string Origin_code = "1"; //1 s. California, 2 n. California

                if(orig_zip_code > 93099)
                {
                    Origin_code = "2";
                }

                string data = string.Concat("Origin=", Origin_code, "&PickUpDate=", month, "-", day, "-", quoteData.puDate.Year.ToString(), 
                    "&Destination=", quoteData.destZip, "&LinealFootageKnown=", Total_lineal_feet,
                    "&Weight=", quoteData.totalWeight, "&IsFloorLoaded=null&ShippingRemarks=&MeasurementLFT=0",
                    "&NonFloorLoadeds=[]&FloorLoadeds=[]");

                DB.Log("Get_Sunset_Pacific_rates data", data);

                Web_client http = new Web_client
                {
                    url = "https://api.sunsetpacific.com/api/Quotes",
                    content_type = "application/x-www-form-urlencoded",
                    accept = "application/json",
                    post_data = data,
                    method = "POST"
                };

                http.header_names = new string[1];
                http.header_names[0] = "Authorization";
                http.header_values = new string[1];
                http.header_values[0] = string.Concat("Bearer ", access_token);

                string doc = http.Make_http_request();

                DB.Log("Get_Sunset_Pacific_rates result", doc);

                #region Parse result

                string[] tokens = new string[3];
                tokens[0] = "Total\":";
                tokens[1] = ":";
                tokens[2] = ",";


                //string cost_string = HelperFuncs.scrapeFromPage(tokens, doc);

                if (double.TryParse(HelperFuncs.scrapeFromPage(tokens, doc), out double total_cost))
                {
                    sunset_volume_result.cost = total_cost;
                }


                tokens[0] = "EstimatedDays\":";

                if (int.TryParse(HelperFuncs.scrapeFromPage(tokens, doc), out int days))
                {
                    sunset_volume_result.days = days;
                }

                tokens[0] = "QuoteId\":";
                sunset_volume_result.quote_id = HelperFuncs.scrapeFromPage(tokens, doc);

                #endregion

            }
            catch (Exception e)
            {
                //string str = e.ToString();
                DB.Log("Get_Sunset_Pacific_rates", e.ToString());
            }
        }

        #endregion

        public struct Sunset_P_Res
        {
            public double cost;
            public int days;
            public string quote_id;
        }
    }
}