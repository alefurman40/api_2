#region Using

using gcmAPI.Models.LTL;
using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

#endregion

namespace gcmAPI.Models.Carriers
{
    public class Frontier
    {
        QuoteData quoteData;

        #region Constructor
        // Constructor
        public Frontier(ref QuoteData quoteData)
        {

            this.quoteData = quoteData;

        }

        #endregion

        #region Get_rates

        public void Get_rates(out GCMRateQuote Frontier_Quote_Genera)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Web_client http = new Web_client();

                #region Basic Authentication

                
                http.header_names = new string[1];
                http.header_values = new string[1];

                http.header_names[0] = "Authorization";

                http.header_values[0] = "Basic " + AppCodeConstants.frontier_genera_basic_auth;


                //http.url = "https://apitest.frontierscs.com/ShippingAPI/rates/";

                #endregion

                http.url = "https://apiprod.frontierscs.com/ShippingAPI/rates/";

                http.method = "POST";

                string items = "";
                for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
                {
                    items += string.Concat("{",
                    "\"weight\":", quoteData.m_lPiece[i].Weight, ", ",
                    "\"length\":40,",
                    "\"width\":48,",
                    "\"height\":40",
                    "}");

                    if (i == quoteData.m_lPiece.Length - 1)
                    {
                        // Do nothing
                    }
                    else
                    {
                        items += ",";
                    }
                }

                //DB.LogGenera("items", "items before", items);

                //items.Remove(items.Length - 1);

                //DB.LogGenera("items", "items after", items);

                #region Post data

                http.post_data = string.Concat("{",
    "\"originCode\":\"", quoteData.origZip, "\",",
    //"\"destinationCode\":\"T1X 0A2\",",
    "\"destinationCode\":\"", quoteData.destZip, "\",",
    "\"pieceCollection\":{",
                    "\"pieces\":[",
                    items,
    //"{",
    //"\"weight\":60,",
    //"\"length\":10,",
    //"\"width\":10,",
    //"\"height\":10",
    //"}",
    "]",
    "}",
    "}");

                DB.LogGenera("Frontier Get_rates", "http.post_data", http.post_data);

                #endregion

                http.accept = "application/json";
                http.content_type = "application/json";

                string doc = "";
                doc = http.Make_http_request();

                DB.LogGenera("Frontier Get_rates", "doc", doc);

                #region Parse result

                //"{\"cost\":{\"estimateCharge\":52.38,\"oversizeCharge\":0.0,\"fuelSurcharge\":8.38,\"additionalCharges\":0.0,\"tax\":0.0,
                //\"crossBorder\":0.0,\"totalCharge\":60.76,\"valid\":true,\"currency\":\"CAD\",\"accessorials\":[]},
                //\"carrier\":\"LOOMIS\",\"estimatedDeliveryDate\":\"2020-01-02\"}"


                string[] tokens = new string[3];
                tokens[0] = "totalCharge";
                tokens[1] = ":";
                tokens[2] = ",";


                string totalCharge_str = HelperFuncs.scrapeFromPage(tokens, doc);
                decimal totalCharge = 0M;

                DateTime delivery_date = DateTime.MinValue;
                string estimatedDeliveryDate_str = "";

                if (decimal.TryParse(totalCharge_str, out totalCharge))
                {
                    tokens = new string[4];
                    tokens[0] = "estimatedDeliveryDate";
                    tokens[1] = ":";
                    tokens[2] = "\"";
                    tokens[3] = "\"";

                    estimatedDeliveryDate_str = HelperFuncs.scrapeFromPage(tokens, doc);

                }

                #endregion

                #region Set result

                Frontier_Quote_Genera = new GCMRateQuote();

                if (totalCharge > 0.0M)
                {
                    Frontier_Quote_Genera.TotalPrice = (double)totalCharge;

                    if (DateTime.TryParse(estimatedDeliveryDate_str, out delivery_date))
                    {
                        Utilities.Helper helper = new Utilities.Helper();
                        Frontier_Quote_Genera.DeliveryDays = helper.Get_business_days_between_2_dates(quoteData.puDate, delivery_date);
                        if (Frontier_Quote_Genera.DeliveryDays == 0)
                        {
                            Frontier_Quote_Genera.DeliveryDays = 10;
                        }
                    }
                    else
                    {
                        Frontier_Quote_Genera.DeliveryDays = 10;
                    }
                    //if (ServiceDays > 0)
                    //{
                    //    Frontier_Quote_Genera.DeliveryDays = ServiceDays;
                    //}
                    //else
                    //{
                    //    Frontier_Quote_Genera.DeliveryDays = 10;
                    //}

                    Frontier_Quote_Genera.DisplayName = "Frontier - Genera";
                    Frontier_Quote_Genera.CarrierKey = "UPS";
                    Frontier_Quote_Genera.BookingKey = "#1#";
                    Frontier_Quote_Genera.Scac = "FXPC";
                }

                #endregion
            }
            catch (Exception e)
            {
                Frontier_Quote_Genera = new GCMRateQuote();
                DB.LogGenera("Frontier", "Get_rates", e.ToString());
            }

        }

        #endregion
    }
}