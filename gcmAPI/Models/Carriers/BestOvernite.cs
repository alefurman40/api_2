#region Using

using gcmAPI.Models.LTL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

#endregion

namespace gcmAPI.Models.Carriers
{
    public class BestOvernite
    {
        QuoteData quoteData;

        #region Constructor
        // Constructor
        public BestOvernite(ref QuoteData quoteData)
        {

            this.quoteData = quoteData;

        }

        #endregion

        #region Get_rates

        public void Get_rates(out GCMRateQuote BestOvernite_Quote_Genera)
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var security = new Best_Overnite_RateService.SECURITYINFODS();
            security.USERNAME = AppCodeConstants.best_overnite_genera_un;
            security.PASSWORD = AppCodeConstants.best_overnite_genera_pwd;

            var quote = new Best_Overnite_RateService.QUOTEDS();

            //quote.PPDCOL = "P";

            quote.IAM = "D";

            //quote.SHIPPER = new Best_Overnite_RateService.CUSTINFO();
            //quote.SHIPPER.ADDRESS1 = "test adddress1";
            //quote.SHIPPER.ADDRESS2 = "";
            //quote.SHIPPER.CITY = "Fontana";
            //quote.SHIPPER.STATE = "CA";
            //quote.SHIPPER.ZIP = "92335";
            //quote.SHIPPER.NAME = "";

            //quote.CONSIGNEE = new Best_Overnite_RateService.CUSTINFO();
            //quote.CONSIGNEE.ADDRESS1 = "test adddress1";
            //quote.CONSIGNEE.ADDRESS2 = "";
            //quote.CONSIGNEE.CITY = "San Jose";
            //quote.CONSIGNEE.STATE = "CA";
            //quote.CONSIGNEE.ZIP = "95119";
            //quote.CONSIGNEE.NAME = "";

            quote.SHIPPER = new Best_Overnite_RateService.CUSTINFO();
            quote.SHIPPER.ADDRESS1 = "test adddress1";
            quote.SHIPPER.ADDRESS2 = "";
            quote.SHIPPER.CITY = quoteData.origCity;
            quote.SHIPPER.STATE = quoteData.origState;
            quote.SHIPPER.ZIP = quoteData.origZip;
            quote.SHIPPER.NAME = "";

            quote.CONSIGNEE = new Best_Overnite_RateService.CUSTINFO();
            quote.CONSIGNEE.ADDRESS1 = "test adddress1";
            quote.CONSIGNEE.ADDRESS2 = "";
            quote.CONSIGNEE.CITY = quoteData.destCity;
            quote.CONSIGNEE.STATE = quoteData.destState;
            quote.CONSIGNEE.ZIP = quoteData.destZip;
            quote.CONSIGNEE.NAME = "";

            #region Accessorials

            byte acc_count = 0;

            if (quoteData.AccessorialsObj.RESPU)
            {
                acc_count++;
            }
            if (quoteData.AccessorialsObj.RESDEL)
            {
                acc_count++;
            }
            if (quoteData.AccessorialsObj.LGPU)
            {
                acc_count++;
            }
            if (quoteData.AccessorialsObj.LGDEL)
            {
                acc_count++;
            }
            if (quoteData.AccessorialsObj.APTPU || quoteData.AccessorialsObj.APTDEL)
            {
                acc_count++;
            }
            if (quoteData.AccessorialsObj.INSDEL)
            {
                acc_count++;
            }

            quote.ACCESSORIAL = new Best_Overnite_RateService.ACCESSORIALDS[acc_count];

            // Reset
            acc_count = 0;

            if (quoteData.AccessorialsObj.RESPU)
            {
                quote.ACCESSORIAL[acc_count] = new Best_Overnite_RateService.ACCESSORIALDS
                {
                    CODE = "RESPIC"
                };
                //var accessorial = new Best_Overnite_RateService.ACCESSORIALDS();
                //accessorial.CODE = "RESPIC";
                //quote.ACCESSORIAL[acc_count] = accessorial;
                acc_count++;
            }
            if (quoteData.AccessorialsObj.RESDEL)
            {
                quote.ACCESSORIAL[acc_count] = new Best_Overnite_RateService.ACCESSORIALDS
                {
                    CODE = "RESDEL"
                };
                //var accessorial = new Best_Overnite_RateService.ACCESSORIALDS();
                //accessorial.CODE = "RESDEL";
                //quote.ACCESSORIAL[acc_count] = accessorial;
                acc_count++;
            }
            if (quoteData.AccessorialsObj.LGPU)
            {
                quote.ACCESSORIAL[acc_count] = new Best_Overnite_RateService.ACCESSORIALDS
                {
                    CODE = "LIFPIC"
                };
                //var accessorial = new Best_Overnite_RateService.ACCESSORIALDS();
                //accessorial.CODE = "LIFPIC";
                //quote.ACCESSORIAL[acc_count] = accessorial;
                acc_count++;
            }
            if (quoteData.AccessorialsObj.LGDEL)
            {
                quote.ACCESSORIAL[acc_count] = new Best_Overnite_RateService.ACCESSORIALDS
                {
                    CODE = "LIFDEL"
                };
                //var accessorial = new Best_Overnite_RateService.ACCESSORIALDS();
                //accessorial.CODE = "LIFDEL";
                //quote.ACCESSORIAL[acc_count] = accessorial;
                acc_count++;
            }
            if (quoteData.AccessorialsObj.APTPU || quoteData.AccessorialsObj.APTDEL)
            {
                //var accessorial = new Best_Overnite_RateService.ACCESSORIALDS();               
                //accessorial.CODE = "NOTFEE";

                quote.ACCESSORIAL[acc_count] = new Best_Overnite_RateService.ACCESSORIALDS
                {
                    CODE = "NOTFEE"
                };
                //quote.ACCESSORIAL[acc_count] = accessorial;
                acc_count++;
            }
            if (quoteData.AccessorialsObj.INSDEL)
            {
                quote.ACCESSORIAL[acc_count] = new Best_Overnite_RateService.ACCESSORIALDS
                {
                    CODE = "INSDEL"
                };
                //var accessorial = new Best_Overnite_RateService.ACCESSORIALDS();
                //accessorial.CODE = "INSDEL";
                //quote.ACCESSORIAL[acc_count] = accessorial;
                acc_count++;
            }

            quote.ACCESSORIALCOUNT = acc_count;
           
            #endregion

            var quote_input = new Best_Overnite_RateService.GETQUOTEInput();

            var quote_result = new Best_Overnite_RateService.GETQUOTEResult();

            var items = new Best_Overnite_RateService.ITEMDS();

            quote.ITEMCOUNT = quoteData.m_lPiece.Length;

            quote.ITEM = new Best_Overnite_RateService.ITEMDS[quoteData.m_lPiece.Length];

            for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
            {
                quote.ITEM[i] = new Best_Overnite_RateService.ITEMDS();
                quote.ITEM[i].WEIGHT = (decimal)quoteData.m_lPiece[i].Weight;


                if (quoteData.m_lPiece[i].Units > 0 && quoteData.m_lPiece[i].Pieces > 0)
                {
                    quote.ITEM[i].PIECES = quoteData.m_lPiece[i].Pieces;
                    quote.ITEM[i].PALLETS = quoteData.m_lPiece[i].Units; ;
                }
                else
                {
                    if (quoteData.m_lPiece[i].Units > 0)
                    {
                        quote.ITEM[i].PIECES = quoteData.m_lPiece[i].Units;
                        quote.ITEM[i].PALLETS = quoteData.m_lPiece[i].Units; ;
                    }
                    else if (quoteData.m_lPiece[i].Pieces > 0)
                    {
                        quote.ITEM[i].PIECES = quoteData.m_lPiece[i].Pieces;
                        quote.ITEM[i].PALLETS = quoteData.m_lPiece[i].Pieces;
                    }
                    else
                    {
                        quote.ITEM[i].PIECES = 1;
                        quote.ITEM[i].PALLETS = 1;
                    }
                }
                

                //if (quoteData.m_lPiece[i].Units > 0)
                //{
                //    quote.ITEM[i].PIECES = quoteData.m_lPiece[i].Units;
                //}
                //else if (quoteData.m_lPiece[i].Pieces > 0)
                //{
                //    quote.ITEM[i].PIECES = quoteData.m_lPiece[i].Pieces;
                //}
                //else
                //{
                //    quote.ITEM[i].PIECES = 1;
                //}
                //quote.ITEM[i].PIECES = quoteData.m_lPiece[i].Pieces;

                //quote.ITEM[i]._CLASS = Convert.ToDecimal(quoteData.m_lPiece[i].FreightClass);
                quote.ITEM[i]._CLASS = 70M;
            }


            quote_input.QUOTE = quote;
            quote_input.SECURITYINFO = security;

            using (var quote_api = new Best_Overnite_RateService.TQUOTEAPI())
            {
                //quote_api.Url = "http://tgif1.bestovernite.com:10010/web/services/TQUOTEAPI";
                quote_result = quote_api.getquote(quote_input);
            }

            BestOvernite_Quote_Genera = new GCMRateQuote();

            if (quote_result != null && quote_result.RATING.AMOUNT > 0.0M)
            {
                BestOvernite_Quote_Genera.TotalPrice = (double)quote_result.RATING.AMOUNT;
                BestOvernite_Quote_Genera.DeliveryDays = (int)quote_result.SERVICE.DAYS;
                BestOvernite_Quote_Genera.DisplayName = "Best Overnite - Genera";
                BestOvernite_Quote_Genera.CarrierKey = "UPS";
                BestOvernite_Quote_Genera.BookingKey = "#1#";
                BestOvernite_Quote_Genera.Scac = "BTVP";
            }

        }

        #endregion
    }
}