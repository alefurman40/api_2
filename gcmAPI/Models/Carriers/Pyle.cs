#region Using

using gcmAPI.Models.LTL;
using gcmAPI.Models.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

#endregion

namespace gcmAPI.Models.Carriers
{
    public class Pyle
    {
        QuoteData quoteData;

        #region Constructor
        // Constructor
        public Pyle(ref QuoteData quoteData)
        {

            this.quoteData = quoteData;

        }

        #endregion

        #region Get_rates

        public void Get_rates(out GCMRateQuote Pyle_Quote_Genera)
        {
            try
            {
                #region Build accessorials string

                string accessorials = "";

                if (quoteData.isHazmat == true)
                {
                    accessorials += "&hazard=1";
                }

                if (quoteData.AccessorialsObj.LGPU)
                {
                    accessorials += "&nplift=1";
                }
                if (quoteData.AccessorialsObj.LGDEL)
                {
                    accessorials += "&nlift=1";
                }
                if (quoteData.AccessorialsObj.RESPU)
                {
                    accessorials += "LGATEP";
                }
                if (quoteData.AccessorialsObj.RESDEL)
                {
                    accessorials += "&nresid=1";
                }
                if (quoteData.AccessorialsObj.CONPU)
                {
                    accessorials += "LGATEP";
                }
                if (quoteData.AccessorialsObj.CONDEL)
                {
                    accessorials += "&ncons=1";
                }
                if (quoteData.AccessorialsObj.APTPU)
                {

                }
                if (quoteData.AccessorialsObj.APTDEL)
                {
                    accessorials += "&ncall=1";
                }
                if (quoteData.AccessorialsObj.TRADEPU)
                {
                    accessorials += "LGATEP";
                }
                if (quoteData.AccessorialsObj.TRADEDEL)
                {
                    accessorials += "LGATEP";
                }
                if (quoteData.AccessorialsObj.INSDEL)
                {
                    accessorials += "&nid=1";
                }

                #endregion

                #region Build items string

                //StringBuilder items = new StringBuilder();

                string items = "";

                //StringBuilder weights = new StringBuilder();
                //StringBuilder classes = new StringBuilder();
                //StringBuilder counts = new StringBuilder();
                //StringBuilder dims = new StringBuilder();

                string weights = "", classes = "", counts = "", dims = "";

                for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
                {
                    weights += string.Concat((int)quoteData.m_lPiece[i].Weight, ",");

                    classes += string.Concat(quoteData.m_lPiece[i].FreightClass, ",");

                    counts += string.Concat(quoteData.m_lPiece[i].Quantity, ",");

                    if (quoteData.m_lPiece[i].Length > 0 && quoteData.m_lPiece[i].Width > 0 && quoteData.m_lPiece[i].Height > 0)
                    {
                        dims += string.Concat(quoteData.m_lPiece[i].Length, "x", quoteData.m_lPiece[i].Width, "x", quoteData.m_lPiece[i].Height, ",");
                    }
                }

                items = string.Concat("&weights=", weights.Remove(weights.Length - 1),
                        "&classes=", classes.Remove(classes.Length - 1),
                        "&count=", counts.Remove(counts.Length - 1)
                        );

                if (dims.Length > 0)
                {
                    items += string.Concat("&dim=", dims.Remove(dims.Length - 1));
                }

                //DB.LogGenera("Pyle", "items", items);

                #endregion
                //11550
                string URL =
                string.Concat(
                    "https://www.aduiepyle.com/publicdocs/RateQuoteAPI4?MyPyleID=", AppCodeConstants.pyle_genera_id, 
                    "&account=", AppCodeConstants.pyle_genera_acct,
                    "&terms=P&oZip=08831&dZip=", quoteData.destZip,

                    items,
                    //"&weights=5000&classes=55&count=4",
                    //"&dim=48x40x48",

                    "&ctrtype=pallet&isfull=empty",

                    accessorials,
                    //"&hazard=1",
                    //"&ins=10000",
                    "&json=1");

                //DB.LogGenera("Pyle", "Request URL", URL);

                #region Get result from server

                Web_client http = new Web_client();

                http.url = URL;
                http.method = "GET";
                http.accept = "application/json";
                string doc = http.Make_http_request();

                //DB.LogGenera("Pyle", "Response", doc);

                dynamic dyn = JsonConvert.DeserializeObject(doc);

                #endregion

                #region Parse result

                string RateQuoteNumber = "", ShippingDays = "", TotalCharge = "";

                if (dyn.RateQuoteNumber != null)
                {
                    RateQuoteNumber = dyn.RateQuoteNumber;
                }

                if (dyn.ShippingDays != null)
                {
                    ShippingDays = dyn.ShippingDays;
                }

                if (dyn.TotalCharge != null)
                {
                    TotalCharge = dyn.TotalCharge;
                }

                #region Not used, get data from dynamic array

                //string Charge = "";
                //foreach (var obj in dyn.ShipmentCharges)
                //{
                //    if(obj.Charge != null)
                //    {
                //        Charge = obj.Charge;

                //    }
                //}

                #endregion

                Pyle_Quote_Genera = new GCMRateQuote();

                double TotalPrice;
                int DeliveryDays;
                if (double.TryParse(TotalCharge, out TotalPrice))
                {
                    Pyle_Quote_Genera.TotalPrice = TotalPrice;
                    if (int.TryParse(ShippingDays, out DeliveryDays))
                    {
                        Pyle_Quote_Genera.DeliveryDays = DeliveryDays;
                    }
                    else
                    {
                        Pyle_Quote_Genera.DeliveryDays = 5;
                    }

                    Pyle_Quote_Genera.DisplayName = "A.Duie Pyle - Genera";
                    Pyle_Quote_Genera.CarrierKey = "UPS";
                    Pyle_Quote_Genera.BookingKey = "#1#";
                    Pyle_Quote_Genera.Scac = "PYLE";
                    Pyle_Quote_Genera.CarrierQuoteID = RateQuoteNumber;
                }

                #endregion

            }
            catch (Exception e)
            {
                Pyle_Quote_Genera = new GCMRateQuote();
                DB.LogGenera("", "e", e.ToString());
            }
        }

        #endregion
    }
}