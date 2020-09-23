#region Using

using System;
using System.Collections.Generic;
using gcmAPI.Models.LTL;
using System.Text;
using System.Xml;
using gcmAPI.Models.Utilities;
using System.Net;

#endregion

namespace gcmAPI.Models.Carriers
{
    public class YRC
    {
        #region Structs

        public struct RateAndQuoteNum
        {
            public string rate, quote;
        }

        public struct DateDayOfWeek
        {
            public string dayOfWeek;
            public DateTime date;
        }

        public struct YRC_Result
        {
            public double standardLTL, guaranteedByNoon, guaranteedBy5PM;
            public List<RateAndQuoteNum> rowByNoon;
            public List<RateAndQuoteNum> rowBy5PM;
            public List<RateAndQuoteNum> rowBy10AM;
            public List<DateDayOfWeek> rowDatesDaysOfWeek;
            public int days;
        }

        #endregion

        private YRC_Result YRC_Res;

        QuoteData quoteData;
        CarrierAcctInfo acctInfo;

        // Constructor
        public YRC(ref QuoteData quoteData, ref CarrierAcctInfo acctInfo)
        {
            this.quoteData = quoteData;
            this.acctInfo = acctInfo;
        }

        #region Volume, Not used, in favor of net_core

        #region Get_YRC_API_Spot_Quote_Volume

        // Not used, in favor of net_core
        public void Get_YRC_API_Spot_Quote_Volume(ref Volume_result result)
        {

            #region Not used
            /*
            string url = string.Concat("https://my.yrc.com/myyrc-api/national/servlet?CONTROLLER=com.rdwy.ec.rexcommon.proxy.http.controller.ProxyApiController&redir=/tfq561",
                "&LOGIN_USER=&LOGIN_PASSWORD=&BusId=",
                "&BusRole=Third Party&PaymentTerms=Prepaid",

                "&OrigCityName=", orig_city, "&OrigStateCode=", orig_state, "&OrigZipCode=", orig_zip, "&OrigNationCode=USA",
                "&DestCityName=", dest_city, "&DestStateCode=", dest_state, "&DestZipCode=", dest_zip, "&DestNationCode=USA",
                "&ServiceClass=SPOT&PickupDate=20190102",
                
                "&LineItemWeight1=13500&LineItemCount=1",
                "&LineItemPackageLength1=144&LineItemPackageWidth1=80&LineItemPackageHeight1=80",

                "&AcceptTerms=Y&LineItemHandlingUnits1=1&AccOption1=NTFY&AccOptionCount=1");
            */
            #endregion

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Web_client http = new Web_client
                {
                    url = "",
                    content_type = "",
                    accept = "*/*",
                    method = "GET"
                };

                //DB.Log("gcmAPI_Get_LOUP_Rates before send request", "before send request");

                //string doc = http.Make_http_request();

                Logins.Login_info login_info;
                Logins logins = new Logins();
                logins.Get_login_info(37, out login_info);

                http.method = "POST";
                //http.referrer = http.url;
                http.url = "https://my.yrc.com/dynamic/national/servlet";
                http.post_data = string.Concat("CNTR=&AccOptionCount=1&AccOption1=NTFY&AcceptTerms=Y",
                    "&DestZipCode=", quoteData.destZip, "&OrigNationCode=USA",
                    "&LineItemPackageWidth1=80",
                    "&LOGIN_USER=", login_info.username, "&LineItemPackageHeight1=80",
                    "&DestCityName=", quoteData.destCity,
                    "&LineItemPackageLength1=144",
                    "&BusId=", login_info.account, "&redir=%2Ftfq561",
                    "&LineItemHandlingUnits1=1&LineItemWeight1=13500",
                    "&OrigZipCode=", quoteData.origZip, "&LineItemCount=1&BusRole=Third Party&OrigCityName=",
                    quoteData.origCity,
                    "&DestStateCode=", quoteData.destState,
                    "&PickupDate=20190102&CONTROLLER=com.rdwy.ec.rexcommon.proxy.http.controller.ProxyApiController",
                    "&ServiceClass=SPOT&PaymentTerms=Prepaid&OrigStateCode=", quoteData.origState,
                    "&DestNationCode=USA",
                    "&LOGIN_USERID=", login_info.username, "&LOGIN_PASSWORD=", login_info.password);

                //DB.Log("YRC post_data volume", http.post_data);

                string doc = http.Make_http_request();

                //DB.Log("YRC response volume", doc);

                #region Parse result

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(doc);
                XmlNodeList nodeList = xmlDoc.GetElementsByTagName("TotalCharges");
                double TotalCharges = 0;
                if (nodeList.Count > 0)
                {
                    TotalCharges = Convert.ToDouble(nodeList[0].InnerText);
                }

                TotalCharges = TotalCharges / 100;

                

                nodeList = xmlDoc.GetElementsByTagName("StandardDate");
                DateTime StandardDate = DateTime.MaxValue;

                string delivery_date = nodeList[0].InnerText;

                string delivery_year = "";
                string delivery_month = "";
                string delivery_day = "";

                try
                {
                    delivery_year = delivery_date.Remove(4);

                    delivery_date = delivery_date.Substring(4);

                    delivery_month = delivery_date.Remove(2);

                    delivery_day = delivery_date.Substring(2);
                }
                catch
                {
                    // Do nothing
                }

                int transit_days = 10;
                if (DateTime.TryParse(string.Concat(delivery_month, "/", delivery_day, "/", delivery_year), out StandardDate))
                {
                    transit_days = Convert.ToInt32((StandardDate - DateTime.Today).TotalDays);
                }

                nodeList = xmlDoc.GetElementsByTagName("QuoteId");
                string QuoteId = string.Empty;
                if (nodeList.Count > 0)
                {
                    QuoteId = nodeList[0].InnerText;
                }

                nodeList = xmlDoc.GetElementsByTagName("ReferenceId");
                string ReferenceId = string.Empty;
                if (nodeList.Count > 0)
                {
                    ReferenceId = nodeList[0].InnerText;
                }

                #endregion

                result.cost = TotalCharges;
                result.quote_number = QuoteId;
                result.transit_days = transit_days;
                result.carrier_name = "YRC Spot Quote";

            }
            catch (Exception e)
            {
                DB.Log("YRC exception volume", e.ToString());
            }
        }

        public struct Volume_result
        {
            public double cost;
            public int transit_days;
            public string carrier_name, quote_number, scac;
        }

        #endregion

        #endregion

        #region GetYRCInfo_API

        private void GetYRCInfo_API(ref bool isDurachem, ref string[] weight, ref string[] fclass,
           ref string[] nmfc, ref string[] pieces, ref int numLineItems, ref bool dimsProvided,
           ref string[] cube, ref string[] density, ref StringBuilder sbAccessorials, int AccOptionCount, ref string[] length,
           ref string[] width, ref string[] height)
        {
            //DB.Log("YRC Live", "GetYRCInfo");

            YRC_Res = new YRC_Result();
            try
            {
                #region Variables

                //DateTime today = DateTime.Today;
                DateTime today = quoteData.puDate;
                string day = today.Day.ToString();
                string month = today.Month.ToString();
                string year = today.Year.ToString();
                string dayOfWeek = today.DayOfWeek.ToString();
                dayOfWeek = dayOfWeek.Remove(3);
                if (day.Length == 1)
                    day = "0" + day;
                if (month.Length == 1)
                    month = "0" + month;

                string doc = "";
                string url, referrer, contentType, accept, method;

                #endregion

                #region Country

                string origCountry = "USA";
                string destCountry = "USA";
                if (HelperFuncs.GetCountryByZip(quoteData.origZip, true, quoteData.origZip, quoteData.destZip).Equals("CANADA"))
                {
                    origCountry = "CAN";
                }
                if (HelperFuncs.GetCountryByZip(quoteData.destZip, false, quoteData.origZip, quoteData.destZip).Equals("CANADA"))
                {
                    destCountry = "CAN";
                }

                #endregion

                #region 10 units limit

                if (fclass.Length > 10)
                {
                    throw new Exception("YRC Freight: Shipments of up to 10 units are handled, you have entered " + fclass.Length.ToString() + " units.");
                }

                #endregion

                #region Items

                StringBuilder items = new StringBuilder();

                string itemNum = string.Empty;

                for (int i = 0; i < fclass.Length; i++)
                {

                    itemNum = (i + 1).ToString();
                    items.Append(string.Concat("&LineItemWeight", itemNum, "=", weight[i]));

                    if (fclass[i].Equals(string.Empty))
                    {
                        // Use NMFC instead of class
                        string[] forSplit = nmfc[i].Split('-');
                        if (forSplit.Length.Equals(2))
                        {
                            items.Append(string.Concat("&LineItemNmfcPrefix", itemNum, "=", forSplit[0], "&LineItemNmfcSuffix", itemNum, "=0", forSplit[1]));
                        }
                        else
                        {
                            items.Append(string.Concat("&LineItemNmfcPrefix", itemNum, "=", nmfc[i], "&LineItemNmfcSuffix", itemNum, "="));
                        }
                    }
                    else
                    {
                        items.Append(string.Concat("&LineItemNmfcClass", itemNum, "="));
                        items.Append(fclass[i]);
                    }

                    items.Append(string.Concat("&LineItemCount", itemNum, "=", pieces[i]));

                    if (dimsProvided.Equals(true))
                    {
                        items.Append(
                            string.Concat(
                            "&LineItemPackageLength", itemNum, "=", length[i],
                            "&LineItemPackageWidth", itemNum, "=", width[i],
                            "&LineItemPackageHeight", itemNum, "=", height[i])
                            );
                    }
                }

                #endregion

                #region Make http requests

                referrer = "";
                url = string.Concat("http://my.yrc.com/myyrc-api/national/servlet?CONTROLLER=com.rdwy.ec.rexcommon.proxy.http.controller.ProxyApiController&redir=/tfq561",
                    "&LOGIN_USERID=", acctInfo.username, "&LOGIN_PASSWORD=", acctInfo.password, "&BusId=", acctInfo.acctNum, "&BusRole=Third%20Party&PaymentTerms=Prepaid",
                    "&OrigCityName=", quoteData.origCity.Replace(" ", "%20"), "&OrigStateCode=", quoteData.origState, "&OrigZipCode=", quoteData.origZip, "&OrigNationCode=", origCountry,
                    "&DestCityName=", quoteData.destCity.Replace(" ", "%20"), "&DestStateCode=", quoteData.destState, "&DestZipCode=", quoteData.destZip, "&DestNationCode=", destCountry,
                    "&ServiceClass=STD&PickupDate=", year, month, day, "&TypeQuery=QUOTE",
                    items, "&LineItemCount=", fclass.Length,

                    sbAccessorials, "&AccOptionCount=", AccOptionCount);

                //"&AccOption1=HOMD&AccOptionCount=1");
                contentType = "";
                method = "GET";
                accept = "*/*";
                doc = (string)HelperFuncs.generic_http_request("string", null, url, referrer, contentType, accept, method, "", false);

                //DB.Log("YRC Live request", url);
                //DB.Log("YRC_Live response", doc);


                #endregion

                #region Get regular rates

                //string tmp = doc;
                bool isIntraCanada = false;
                if (origCountry == "CAN" && destCountry == "CAN")
                {
                    if (!quoteData.username.ToLower().Equals("durachem"))
                    {
                        throw new Exception("Intra Canada");  // Intra Canada enabled only for durachem for now
                    }
                    isIntraCanada = true;
                }

                #region Get cost and transit days from the result

                string[] tokens = new string[3];
                tokens[0] = "<TotalCharges>";
                tokens[1] = ">";
                tokens[2] = "<";

                string costStr = HelperFuncs.scrapeFromPage(tokens, doc);

                DB.Log("YRC_Live costStr", costStr);

                int costInt = -1;
                int costCents = 0;

                if (int.TryParse(costStr, out int testInt))
                {
                    costInt = testInt / 100;
                    costCents = testInt % 100;

                    DB.Log("YRC_Live costInt costCents", costInt + " " + costCents);
                }

                if (double.TryParse(string.Concat(costInt, ".", costCents.ToString().PadLeft(2, '0')), out double costDouble))
                {
                    YRC_Res.standardLTL = costDouble;
                    DB.Log("YRC_Live costDouble", costDouble.ToString());
                }

                YRC_Res.days = 5;
                tokens[0] = "<StandardDays>";

                int.TryParse(HelperFuncs.scrapeFromPage(tokens, doc), out YRC_Res.days);

                #endregion

                #endregion

            }

            catch (Exception e)
            {
                #region Exception code

                DB.Log("YRC", e.ToString());

                YRC_Res.standardLTL = -1;

                #endregion
            }
        }

        #endregion

        #region GetResultObjectFromYRC_API

        public void GetResultObjectFromYRC_API(ref GCMRateQuote gcmRateQuote)
        {
            try
            {
                //DB.Log("GetResultObjectFromYRC_API", "hit func");

                //if (quoteData.AccessorialsObj.TRADEPU.Equals(true) || quoteData.AccessorialsObj.TRADEDEL.Equals(true))
                //{
                //    throw new Exception("Tradeshow not supported");
                //}

                if (quoteData.AccessorialsObj.APTPU)
                {
                    throw new Exception("appointment pickup not supported");
                }

                #region Variables

                int count = 0;

                bool isDurachem = false;

                if (acctInfo.acctNum.Equals("13203032805"))
                {
                    isDurachem = true;
                    //If class 55 pass nmfc 101720-3
                    //If class 125 pass nmfc 41027
                }

                string[] weight = new string[quoteData.m_lPiece.Length];
                string[] fclass = new string[quoteData.m_lPiece.Length];
                string[] nmfc = new string[quoteData.m_lPiece.Length];
                string[] length = new string[quoteData.m_lPiece.Length];
                string[] width = new string[quoteData.m_lPiece.Length];
                string[] height = new string[quoteData.m_lPiece.Length];
                string[] pieces = new string[quoteData.m_lPiece.Length];
                string[] cube = new string[quoteData.m_lPiece.Length];
                string[] density = new string[quoteData.m_lPiece.Length];

                double[] lengthDbl = new double[quoteData.m_lPiece.Length];
                double[] widthDbl = new double[quoteData.m_lPiece.Length];
                double[] heightDbl = new double[quoteData.m_lPiece.Length];

                #endregion

                #region Get inputs

                for (int i = 0; i < quoteData.m_lPiece.Length; i++)
                {
                    addOneYRC_Item(ref i, ref count, ref isDurachem, ref weight, ref fclass, ref nmfc, ref length, ref width, ref height, ref cube, ref density, ref pieces);
                }

                #endregion

                #region Accessorials

                byte accCounter = 1;

                StringBuilder sbAccessorials = new StringBuilder();

                if (quoteData.AccessorialsObj.LGPU)
                {
                    sbAccessorials.Append(string.Concat("&AccOption", accCounter, "=LFTO"));
                    //LGPU = "&rq.service.LFTO=true";
                    accCounter++;
                }

                if (quoteData.AccessorialsObj.CONPU)
                {
                    sbAccessorials.Append(string.Concat("&AccOption", accCounter, "=LTDO"));
                    //LIMACCPU = "&rq.service.LTDO=true";
                    accCounter++;
                }

                if (quoteData.AccessorialsObj.CONDEL)
                {
                    sbAccessorials.Append(string.Concat("&AccOption", accCounter, "=LTDD"));
                    //LIMACCDEL = "&rq.service.LTDD=true";
                    accCounter++;
                }

                if (quoteData.AccessorialsObj.RESPU) // || quoteData.AccessorialsObj.CONPU
                {
                    sbAccessorials.Append(string.Concat("&AccOption", accCounter, "=HOMP"));
                    //RESPU = "&rq.service.HOMP=true";
                    accCounter++;
                }

                if (quoteData.AccessorialsObj.LGDEL)
                {
                    sbAccessorials.Append(string.Concat("&AccOption", accCounter, "=LFTD"));
                    //LGDEL = "&rq.service.LFTD=true";
                    accCounter++;
                }

                if (quoteData.AccessorialsObj.RESDEL) // || quoteData.AccessorialsObj.CONDEL
                {
                    //RESDEL = string.Concat("&AccOption", accCounter, "=HOMD");
                    sbAccessorials.Append(string.Concat("&AccOption", accCounter, "=HOMD"));
                    accCounter++;
                }
                if (quoteData.AccessorialsObj.INSDEL)
                {
                    sbAccessorials.Append(string.Concat("&AccOption", accCounter, "=ID"));
                    //INSDEL = "&rq.service.ID=true";
                    accCounter++;
                }

                if (quoteData.AccessorialsObj.APTDEL)
                {
                    sbAccessorials.Append(string.Concat("&AccOption", accCounter, "=NTFY"));
                    //APT = "&rq.service.NTFY=true";
                    accCounter++;
                }

                if (quoteData.isHazmat)
                {
                    sbAccessorials.Append(string.Concat("&AccOption", accCounter, "=HAZM"));
                    //HAZ = "&rq.service.HAZM=true";
                    accCounter++;
                }

                //if (Request.QueryString["q_InsPick"].Equals("true"))
                //{
                //    sbAccessorials.Append(string.Concat("&AccOption", accCounter, "=IP"));
                //    //INSPU = "&rq.service.IP=true";
                //    accCounter++;
                //}

                if (quoteData.AccessorialsObj.TRADEPU)
                {
                    sbAccessorials.Append(string.Concat("&AccOption", accCounter, "=SHWO"));
                    //TRADEPU = "&rq.service.SHWO=true";
                    accCounter++;
                }

                if (quoteData.AccessorialsObj.TRADEDEL)
                {
                    sbAccessorials.Append(string.Concat("&AccOption", accCounter, "=SHWD"));
                    //TRADEDEL = "&rq.service.SHWD=true";
                    accCounter++;
                }


                #endregion

                //------------------------------

                GetYRCInfo_API(ref isDurachem, ref weight, ref fclass, ref nmfc, ref pieces, ref quoteData.numOfUnitsPieces,
                    ref quoteData.hasDimensions, ref cube, ref density,
                    ref sbAccessorials, --accCounter, ref length, ref width, ref height);

                //------------------------------

                if (YRC_Res.standardLTL == -1) // Error
                {
                    throw new Exception("logged error");
                }

                double totalCharges = 0;
                totalCharges = YRC_Res.standardLTL;

                gcmRateQuote.TotalPrice = totalCharges;
                gcmRateQuote.DisplayName = acctInfo.displayName;
                gcmRateQuote.BookingKey = acctInfo.bookingKey;
                gcmRateQuote.CarrierKey = acctInfo.carrierKey;

                gcmRateQuote.DeliveryDays = YRC_Res.days;
                gcmRateQuote.Documentation = "https://my.yrc.com/dynamic/national/servlet?CONTROLLER=com.rdwy.ec.rexcommon.proxy.http.controller.PublicProxyController&redir=/TFD617&TAG=1TFA99088214BA750628&DATE=01/01/2011";
            }
            catch (Exception exp)
            {
                #region Catch

                gcmRateQuote = null;

                DB.Log("YRC Live", exp.ToString());

                //DB.Log("YRC Live", exp.ToString());
                //if (exp.Message != "logged error" && exp.Message != "appointment pickup not supported" && !exp.Message.Contains("there were not three rate results"))
                //{
                //    DB.Log("YRC Live", exp.ToString(), "");
                //}

                #endregion
            }
        }

        #endregion

        #region addOneYRC_Item

        private void addOneYRC_Item(ref int i, ref int count, ref bool isDurachem, ref string[] weight, ref string[] fclass, ref string[] nmfc,
            ref string[] length, ref string[] width, ref string[] height, ref string[] cube, ref string[] density, ref string[] pieces)
        {
            if (quoteData.m_lPiece[i].Quantity > 0)
            {
                pieces[count] = quoteData.m_lPiece[i].Quantity.ToString();
            }
            else
            {
                pieces[count] = "1";
            }

            fclass[count] = quoteData.m_lPiece[i].FreightClass;
            if (isDurachem && fclass[count] == "55")
            {
                nmfc[count] = "101720-3";
                fclass[count] = "";
            }
            else if (isDurachem && fclass[count] == "125")
            {
                nmfc[count] = "41027";
                fclass[count] = "";
            }
            else
            {
                nmfc[count] = "";
            }

            weight[count] = quoteData.m_lPiece[i].Weight.ToString();
            //length[count] = "";
            //width[count] = "";
            //height[count] = "";

            if (quoteData.hasDimensions)
            {
                //weight[count] = quoteData.m_lPiece[i].Weight.ToString();
                length[count] = quoteData.m_lPiece[i].Length.ToString();
                width[count] = quoteData.m_lPiece[i].Width.ToString();
                height[count] = quoteData.m_lPiece[i].Height.ToString();

                cube[count] = Math.Round((quoteData.m_lPiece[i].Length * quoteData.m_lPiece[i].Width * quoteData.m_lPiece[i].Height) / 1728, 2).ToString();
                density[count] = Math.Round(quoteData.m_lPiece[i].Weight / Convert.ToDouble(cube[count]), 2).ToString();
            }

            count++;
        }

        #endregion

    }
}