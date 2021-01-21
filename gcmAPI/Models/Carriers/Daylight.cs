#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using gcmAPI.Models.LTL;
using gcmAPI.Models.Utilities;
using Newtonsoft.Json;

#endregion

namespace gcmAPI.Models.Carriers
{
    public class Daylight
    {
        QuoteData quoteData;

        #region Constructor
        // Constructor
        public Daylight(ref QuoteData quoteData)
        {

            this.quoteData = quoteData;

        }

        #endregion

        #region Get_rate

        public void Get_rate(ref string access_token, out GCMRateQuote Daylight_Quote_Genera)
        {
            try
            {
                Web_client http = new Web_client();
                
                #region Pickup date

                string day = DateTime.Today.Day.ToString(), month = DateTime.Today.Month.ToString();
                if (day.Length == 1)
                    day = string.Concat("0", day);
                if (month.Length == 1)
                    month = string.Concat("0", month);

                #endregion

                #region Build items

                StringBuilder sb_items = new StringBuilder();

                if (quoteData.m_lPiece.Length == 1)
                {
                    sb_items.Append(string.Concat("\"item\": {",

                        "\"description\": \"Test\",",
                      "\"nmfcNumber\": \"\",",
                      "\"nmfcSubNumber\": \"\",",
                      "\"pcs\": ", quoteData.m_lPiece[0].Pieces, ",",
                      "\"pallets\": ", quoteData.m_lPiece[0].Quantity, ",",
                      "\"weight\": ", quoteData.m_lPiece[0].Weight, ", ",
                      "\"actualClass\": \"", quoteData.m_lPiece[0].FreightClass, "\"",
                    "}"));
                }
                else
                {
                    sb_items.Append("\"item\": [");

                    string items_nodes = "";
                    for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
                    {
                        items_nodes += string.Concat("{",

                            "\"description\": \"Test\",",
                          "\"nmfcNumber\": \"\",",
                          "\"nmfcSubNumber\": \"\",",
                          "\"pcs\": ", quoteData.m_lPiece[i].Pieces, ",",
                          "\"pallets\": ", quoteData.m_lPiece[i].Quantity, ",",
                          "\"weight\": ", quoteData.m_lPiece[i].Weight, ", ",
                          "\"actualClass\": \"", quoteData.m_lPiece[i].FreightClass, "\"",
                        "},"

                      );
                    }

                    // Remove the comma at the end
                    sb_items.Append(items_nodes.Remove(items_nodes.Length - 1));

                    sb_items.Append("]");

                }

                #endregion

                #region Accessorials

                var sb_accessorials = new StringBuilder();

                byte num_of_acc = 0;
                Get_num_of_acc(ref num_of_acc, ref quoteData);

                //DB.LogGenera("Daylight", "num_of_acc", num_of_acc.ToString());

                if (num_of_acc == 0)
                {
                    // Do nothing
                }
                else if (num_of_acc == 1)
                {
                    string acc = "";
                    Get_acc_nodes(ref acc, ref num_of_acc, ref quoteData);
                    // Remove the comma at the end
                    acc = acc.Remove(acc.Length - 1);

                    sb_accessorials.Append(acc);
                }
                else
                {
                    // More than one
                    sb_accessorials.Append("\"accessorial\": [");

                    string acc = "";
                    Get_acc_nodes(ref acc, ref num_of_acc, ref quoteData);

                    //DB.LogGenera("Daylight", "acc before remove", acc);

                    // Remove the comma at the end
                    acc = acc.Remove(acc.Length - 1);

                    //DB.LogGenera("Daylight", "acc after remove", acc);

                    sb_accessorials.Append(acc);

                    sb_accessorials.Append("]");
                }

                #endregion

                http.url = "https://api.dylt.com/rateQuote";
                http.method = "POST";

                #region post_data

                http.post_data = string.Concat("{",
                    "\"dyltRateQuoteReq\": {",
      "\"accountNumber\": \"", AppCodeConstants.DYLT_Genera_account, "\",",
      "\"userName\": \"", AppCodeConstants.DYLT_Genera_username, "\",",
      "\"password\": \"", AppCodeConstants.DYLT_Genera_password, "\",",
      "\"billTerms\": \"PP\",",
      "\"serviceType\": \"LTL\",",
      "\"pickupDate\": \"", DateTime.Today.Year, "-", month, "-", day, "\",",
      "\"shipperInfo\": {",
                    "\"customerNumber\": \"\",",
        "\"customerName\": \"\",", //Genera Corp
        "\"customerAddress\": {",
                        "\"streetAddress\": \"\",", //600 Freeport Pkwy Ste 250
          "\"aptAddress\": \"\",",
          "\"city\": \"", quoteData.origCity, "\",", //Coppell
          "\"state\": \"", quoteData.origState, "\",", //TX
          "\"zip\": \"", quoteData.origZip, "\"", //75019
        "}",
                "},",
      "\"consigneeInfo\": {",
                    "\"customerNumber\": \"\",",
        "\"customerName\": \"\",", //Simco Automotive Inc
        "\"customerAddress\": {",
                        "\"streetAddress\": \"\",", // 13425 Estelle St
          "\"aptAddress\": \"string\",",
          "\"city\": \"", quoteData.destCity, "\",", //Corona
          "\"state\": \"", quoteData.destState, "\",", //CA
          "\"zip\": \"", quoteData.destZip, "\"", //92879
        "}",
                "},",
      "\"items\": {", sb_items,
                "},",
      "\"accessorials\": {", sb_accessorials,

                //          "\"accessorial\": {",
                //              "\"accName\": \"DELIVERY\",",
                //"\"accId\": \"APPOINTMENT FEE\",",
                //"\"accValue\": 0",
                //          "}",
                "}",
            "}",
            "}"
            );

                //DB.LogGenera("Daylight", "post_data", http.post_data);
                //Daylight_Quote_Genera = new GCMRateQuote();
                //return;

                #endregion

                http.accept = "*/*";
                //http.accept = "application/json";
                http.content_type = "application/json";

                http.header_names = new string[1];
                http.header_names[0] = "Authorization";
                http.header_values = new string[1];
                http.header_values[0] = string.Concat("Bearer ", access_token);

                string doc = http.Make_http_request();

                //DB.LogGenera("Daylight", "response", doc);

                dynamic dyn = JsonConvert.DeserializeObject(doc);

                #region Parse result

                #region Sample Response

                //            {
                //                "dyltRateQuoteResp":{
                //                    "success":"YES",
                //      "errorInformation":{
                //                        "errorMessage":"NULL"
                //      },
                //      "quoteNumber":"CQ20093199",
                //      "quoteDate":"08\/05\/2020",
                //      "pickupDate":"08\/05\/2020",
                //      "earliestDeliveryDate":"08\/07\/2020",
                //      "totalWeight":500,
                //      "origZip":75019,
                //      "destZip":92879,
                //      "rateBaseName":"DYLT507AP 2020-02-03",
                //      "itemCharges":{
                //                        "itemCharge":[
                //                           {
                //               "description":"Test",
                //                              "pcs":1,
                //                              "pallets":1,
                //                              "weight":500,
                //                              "actualClass":50,
                //                              "rateClass":50,
                //                              "rate":215.47,
                //                              "charge":1077.35,
                //                              "discountPct":86.5
                //            }
                //         ]
                //      },
                //      "accessorialCharges":{
                //         "accessorialCharge":[
                //            {
                //               "reqAccessorial":"California Compliance Surcharge",
                //               "accRate":9.95,
                //               "accCharge":9.95
                //            },
                //            {
                //               "reqAccessorial":"Appointment Notification",
                //               "accRate":0.0,
                //               "accCharge":0.0
                //            },
                //            {
                //               "reqAccessorial":"Fuel Surcharge",
                //               "accRate":0.207,
                //               "accCharge":30.11
                //            }
                //         ]
                //      },
                //      "totalCharges":{
                //         "grossCharge":1077.35,
                //         "fuelSurcharge":30.11,
                //         "totalAccessorial":40.06,
                //         "discountAmt":931.91,
                //         "netCharge":185.5
                //      }
                //   }
                //}

                #endregion

                string totalCharges = "", quoteNumber = "", netCharge = "", str_earliestDeliveryDate = "", str_pickupDate = "";


                var dyltRateQuoteResp = new Newtonsoft.Json.Linq.JObject();

                if (dyn.dyltRateQuoteResp != null)
                {
                    if (dyn.dyltRateQuoteResp.totalCharges != null)
                    {
                        if (dyn.dyltRateQuoteResp.totalCharges.netCharge != null)
                        {
                            netCharge = dyn.dyltRateQuoteResp.totalCharges.netCharge;
                        }
                    }

                    if (dyn.dyltRateQuoteResp.quoteNumber != null)
                    {
                        quoteNumber = dyn.dyltRateQuoteResp.quoteNumber;
                    }

                    if (dyn.dyltRateQuoteResp.earliestDeliveryDate != null)
                    {
                        str_earliestDeliveryDate = dyn.dyltRateQuoteResp.earliestDeliveryDate;
                    }

                    if (dyn.dyltRateQuoteResp.pickupDate != null)
                    {
                        str_pickupDate = dyn.dyltRateQuoteResp.pickupDate;
                    }
                }

                int transit_days = 5;
                if (DateTime.TryParse(str_earliestDeliveryDate, out DateTime earliestDeliveryDate) &&
                   DateTime.TryParse(str_pickupDate, out DateTime pickupDate))
                {
                    var h = new Utilities.Helper();
                    transit_days = h.Get_business_days_between_2_dates(pickupDate, earliestDeliveryDate);
                }


                #region Not used, get data from dynamic array

                //string Charge = "";
                //foreach (var obj in dyn.totalCharges)
                //{
                //    if (obj.Charge != null)
                //    {
                //        netCharge = obj.netCharge;

                //    }
                //}

                #endregion

                #endregion


                #region Set result

                Daylight_Quote_Genera = new GCMRateQuote();

                if (decimal.TryParse(netCharge, out decimal totalCharge) && totalCharge > 0.0M)
                {
                    Daylight_Quote_Genera.TotalPrice = (double)totalCharge;

                    Daylight_Quote_Genera.DeliveryDays = transit_days;
                    Daylight_Quote_Genera.CarrierQuoteID = quoteNumber;
                    Daylight_Quote_Genera.DisplayName = "Daylight - Genera";
                    Daylight_Quote_Genera.CarrierKey = "DayLight";
                    Daylight_Quote_Genera.BookingKey = "#1#";
                    Daylight_Quote_Genera.Scac = "DYLT";
                }

                #endregion

            }
            catch (Exception e)
            {
                DB.LogGenera("Daylight", "e", e.ToString());
                Daylight_Quote_Genera = new GCMRateQuote();
            }
        }

        #endregion

        #region Get_access_token

        public void Get_access_token(ref string access_token)
        {
            try
            {
                string data =
                    string.Concat(
                        "client_secret=", AppCodeConstants.DYLT_Genera_client_secret,
                        "&grant_type=client_credentials&client_id=", AppCodeConstants.DYLT_Genera_client_id);

                Web_client http = new Web_client
                {
                    url = "https://api.dylt.com/oauth/client_credential/accesstoken?grant_type=client_credentials",
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
                HelperFuncs.writeToSiteErrors("Get_access_token", str);
            }
        }

        #endregion

        #region Get_acc_nodes

        private static void Get_acc_nodes(ref string acc, ref byte num_of_acc, ref QuoteData quoteData)
        {
            if (num_of_acc == 1)
                acc += "\"accessorial\": ";

            if (quoteData.AccessorialsObj.RESDEL)
            {
                acc += string.Concat("{",
                    "\"accName\": \"DELIVERY\",",
                      "\"accId\": \"RESIDENTIAL DELIVERY\",",
                      "\"accValue\": 0",
                "},");
            }

            if (quoteData.AccessorialsObj.INSDEL)
            {
                acc += string.Concat("{",
                   "\"accName\": \"DELIVERY\",",
                     "\"accId\": \"INSIDE DELIVERY\",",
                     "\"accValue\": 0",
               "},");
            }

            if (quoteData.AccessorialsObj.CONDEL)
            {
                acc += string.Concat("{",
                  "\"accName\": \"DELIVERY\",",
                    "\"accId\": \"LIMITED ACCESS OR CONSTR SITE DLVRY\",",
                    "\"accValue\": 0",
              "},");
            }

            if (quoteData.AccessorialsObj.CONPU)
            {
                acc += string.Concat("{",
                   "\"accName\": \"PICKUP\",",
                     "\"accId\": \"LIMITED ACCESS OR CONSTR SITE PICKUP\",",
                     "\"accValue\": 0",
               "},");
            }

            if (quoteData.AccessorialsObj.LGDEL)
            {
                acc += string.Concat("{",
                 "\"accName\": \"DELIVERY\",",
                   "\"accId\": \"LIFT GATE DELIVERY\",",
                   "\"accValue\": 0",
             "},");
            }

            if (quoteData.AccessorialsObj.LGPU)
            {
                acc += string.Concat("{",
                 "\"accName\": \"PICKUP\",",
                   "\"accId\": \"LIFT GATE PICKUP\",",
                   "\"accValue\": 0",
             "},");
            }

            if (quoteData.AccessorialsObj.APTDEL)
            {
                acc += string.Concat("{",
                "\"accName\": \"DELIVERY\",",
                  "\"accId\": \"APPOINTMENT FEE\",",
                  "\"accValue\": 0",
            "},");
            }

            if (quoteData.AccessorialsObj.APTPU)
            {
                acc += string.Concat("{",
                "\"accName\": \"PICKUP\",",
                  "\"accId\": \"APPOINTMENT FEE\",",
                  "\"accValue\": 0",
            "},");
            }
        }

        #endregion

        #region Get_num_of_acc

        private static void Get_num_of_acc(ref byte num_of_acc, ref QuoteData quoteData)
        {
            if (quoteData.AccessorialsObj.RESDEL)
            {
                num_of_acc++;
            }

            if (quoteData.AccessorialsObj.INSDEL)
            {
                num_of_acc++;
            }

            if (quoteData.AccessorialsObj.CONDEL)
            {
                num_of_acc++;
            }

            if (quoteData.AccessorialsObj.CONPU)
            {
                num_of_acc++;
            }

            if (quoteData.AccessorialsObj.LGDEL)
            {
                num_of_acc++;
            }

            if (quoteData.AccessorialsObj.LGPU)
            {
                num_of_acc++;
            }

            if (quoteData.AccessorialsObj.APTDEL)
            {
                num_of_acc++;
            }

            if (quoteData.AccessorialsObj.APTPU)
            {
                num_of_acc++;
            }
        }

        #endregion
    }
}