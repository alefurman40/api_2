#region Using

using System;
using gcmAPI.Models.LTL;
using System.Text;
using gcmAPI.Models.Utilities;

#endregion

namespace gcmAPI.Models.Carriers
{
    public class PittOhio
    {
        CarrierAcctInfo acctInfo;
        QuoteData quoteData;

        // Constructor
        public PittOhio(ref CarrierAcctInfo acctInfo, ref QuoteData quoteData)
        {
            this.acctInfo = acctInfo;
            this.quoteData = quoteData;
        }

        #region GetResultObjectFromPittOhio_API

        public void GetResultObjectFromPittOhio_API(ref GCMRateQuote gcmRateQuote)
        {

            //DB.Log("GetResultObjectFromPittOhio_API", "");
            //DB.Log("GetResultObjectFromPittOhio_API", quoteData.pickupDate);

            try
            {
                double maxLengthDim = 0;
                HelperFuncs.GetMaxLengthDimension(ref quoteData.m_lPiece, ref maxLengthDim);

                if (!quoteData.mode.Equals("NetNet"))
                {
                    if ((quoteData.AccessorialsObj.LGPU.Equals(true) ||
                            quoteData.AccessorialsObj.LGDEL.Equals(true)) && maxLengthDim > 54)
                    {
                        throw new Exception("Pitt Ohio over 54 dim");
                    }
                }

                if (quoteData.AccessorialsObj.TRADEPU.Equals(true) || quoteData.AccessorialsObj.TRADEDEL.Equals(true))
                {
                    throw new Exception("Tradeshow not supported");
                }

                #region Variables

                int overlengthFee = 0;
                double cost;
                Int16 days;
                string multPieces = "", overlength = "";
                StringBuilder sbAccessorials = new StringBuilder();

                #endregion

                #region Weight/class/overlength

                for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
                {

                    multPieces += string.Concat("&Class", (i + 1), "=", quoteData.m_lPiece[i].FreightClass, "&Wgt", (i + 1), "=", quoteData.m_lPiece[i].Weight);

                    if (quoteData.m_lPiece[i].Length > 143)
                    {
                        overlength = "&Acc_OVR=Y";
                    }
                    if (quoteData.m_lPiece[i].Width > 143)
                    {
                        overlength = "&Acc_OVR=Y";
                    }
                    if (quoteData.m_lPiece[i].Height > 143)
                    {
                        overlength = "&Acc_OVR=Y";
                    }
                }

                #endregion

                #region Accessorials

                sbAccessorials.Append(overlength);

                if (quoteData.AccessorialsObj.INSDEL)
                {
                    //insdel = "&Acc_IDL=Y";
                    sbAccessorials.Append("&Acc_IDL=Y");
                }
                //
                if (quoteData.AccessorialsObj.TRADEDEL)
                {
                    //tradedel = "&Acc_CNV=Y";
                    sbAccessorials.Append("&Acc_CNV=Y");
                    //throw new Exception("accessorial not supported");
                }
                if (quoteData.AccessorialsObj.TRADEPU)
                {
                    throw new Exception("accessorial not supported");
                }

                //if (quoteData.AccessorialsObj.LGDEL)
                //{
                //    sbAccessorials.Append("&Acc_LGD=Y");
                //    //lgdel = "&Acc_LGD=Y";
                //}

                if (quoteData.AccessorialsObj.LGPU && quoteData.AccessorialsObj.LGDEL)
                {
                    gcmRateQuote = null;
                    return;
                }

                if (quoteData.AccessorialsObj.LGPU || quoteData.AccessorialsObj.LGDEL)
                {
                    sbAccessorials.Append("&Acc_LGD=Y"); // Sets LGDEL, instead of LGPU           
                }

                if (quoteData.AccessorialsObj.APTDEL || quoteData.AccessorialsObj.APTPU)
                {
                    sbAccessorials.Append("&Acc_MNC=Y");
                    //apt = "&Acc_MNC=Y";
                }

                //if (quoteData.AccessorialsObj.APTPU)
                //    throw new Exception("apt pickup not supported");

                if (quoteData.AccessorialsObj.RESPU)
                {
                    sbAccessorials.Append("&Acc_REP=Y");
                    //respu = "&Acc_REP=Y";
                }
                if (quoteData.AccessorialsObj.RESDEL)
                {
                    sbAccessorials.Append("&Acc_RES=Y");
                    //resdel = "&Acc_RES=Y";
                }
                if (quoteData.AccessorialsObj.CONPU)
                {
                    sbAccessorials.Append("&Acc_LAP=Y");
                    //throw new Exception("construction pickup accessorial not supported");
                    //conspu = "&Acc_LAP=Y";
                }
                if (quoteData.AccessorialsObj.CONDEL)
                {
                    sbAccessorials.Append("&Acc_CSD=Y");
                    //consdel = "&Acc_CSD=Y";
                }

                if (quoteData.isHazmat)
                {
                    sbAccessorials.Append("&Acc_HAZ=Y");
                    //hazmat = "&Acc_HAZ=Y";
                }

                #endregion
                
                string TotalChargesString = string.Empty, daysString = string.Empty;

                getRatesFromPittOhioAPI(ref multPieces, ref sbAccessorials, ref TotalChargesString,
                    ref daysString);

                if (double.TryParse(TotalChargesString, out cost))
                {
                    //GCMRateQuote objQuote = new GCMRateQuote();
                    //objQuote.TotalPrice = cost + overlengthFee;

                    //DB.Log("Pitt Ohio (Live) ", cost.ToString());

                    cost += overlengthFee;
                    
                    if (!Int16.TryParse(daysString, out days))
                    {
                        days = -3;
                    }

                    gcmRateQuote = new GCMRateQuote();
                    gcmRateQuote.TotalPrice = cost;
                    gcmRateQuote.DisplayName = acctInfo.displayName;
                    gcmRateQuote.Documentation = "";
                    gcmRateQuote.DeliveryDays = days;
                    gcmRateQuote.BookingKey = acctInfo.bookingKey;
                    gcmRateQuote.CarrierKey = acctInfo.carrierKey;

                    //pittOhioQuoteAPI = SetInfoToObjectQuote(ref cost, "Pitt Ohio", "#1#", carrierKey, "", days, "Pitt Ohio");

                }
                else
                {
                    gcmRateQuote = null;

                    DB.Log("Pitt Ohio error (Live) " + acctInfo.username, "");

                }
            }
            catch (Exception ecf)
            {
                #region Catch

                gcmRateQuote = null;
                DB.Log("Pitt Ohio API (Live) " + acctInfo.username, ecf.ToString());
                //if (ecf.Message != "accessorial not supported")
                //{
                //    DB.Log("Pitt Ohio (Live) " + user, ecf.ToString(), "");
                //}

                #endregion
            }
        }

        #endregion

        #region getRatesFromPittOhioAPI

        private void getRatesFromPittOhioAPI(ref string multPieces, ref StringBuilder sbAccessorials,
            ref string TotalChargesString, ref string daysString)
        {

            string url = string.Concat("http://works.pittohio.com/mypittohio/b2bratecalc.asp?acctnum=", 
                acctInfo.username, "&password=", acctInfo.password,
                "&ShipCity=", quoteData.origCity,
                "&ShipState=", quoteData.origState,
                "&ShipZIP=", quoteData.origZip,

                "&ConsCity=", quoteData.destCity,
                "&ConsState=", quoteData.destState,
                "&ConsZIP=", quoteData.destZip,

                "&Terms=", acctInfo.terms, // Options P,I,3 - Prepaid, Inbound Collect, Third Party
                "&ShipDate=", quoteData.puDate.ToShortDateString(), //mm/dd/yyyy

                // ShipType=PAL // Options P or PAL for Pallets, N for not pallets (default), M for Mixed
                //"&TestMode=Y",
                //"&Class1=50&Wgt1=1482",
                multPieces,
                sbAccessorials
               //"&Acc_RES=Y&Acc_LGD=Y"
               );

            //HelperFuncs.writeToSiteErrors("Pitt Ohio API (Live) request", url);
            //string doc = "";
            string doc = (string)HelperFuncs.generic_http_request_3(
                "string", null, url, "", "text/xml", "*/*", "GET", "", false, false, "", "");

            //HelperFuncs.writeToSiteErrors("Pitt Ohio API (Live) response", doc);


            string[] tokens = new string[3];
            tokens[0] = "TotalCharges";
            tokens[1] = ">";
            tokens[2] = "<";
            TotalChargesString = HelperFuncs.scrapeFromPage(tokens, doc);

            //DB.Log("Pitt Ohio API (Live) TotalChargesStr", TotalChargesString);

            tokens[0] = "AdvertisedTransit";
            daysString = HelperFuncs.scrapeFromPage(tokens, doc);

        }

        #endregion

    }
}