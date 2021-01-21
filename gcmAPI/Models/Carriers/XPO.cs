#region Using

using System;
using System.Collections.Generic;
using gcmAPI.Models.LTL;
using System.Xml;
using System.Text;
using System.Net;
using System.IO;
using gcmAPI.Models.Utilities;

#endregion

public class XPO
{
    CarrierAcctInfo acctInfo;
    QuoteData quoteData;

    // Constructor
    public XPO()
    {      
    }

    // Constructor
    public XPO(CarrierAcctInfo acctInfo, ref QuoteData quoteData)
    {
        this.acctInfo = acctInfo;
        this.quoteData = quoteData;
    }

    #region GetRateFromXPO

    public GCMRateQuote GetRateFromXPO()
    {
        GCMRateQuote gcmRateQuote = new GCMRateQuote();

        GetResultObjectFromConWayFreight(ref gcmRateQuote);

        return gcmRateQuote;
    }

    #endregion

    #region GetResultObjectFromConWayFreight

    private void GetResultObjectFromConWayFreight(ref GCMRateQuote gcmRateQuote)
    {
        Logins.Login_info login_info;
        Logins logins = new Logins();
        logins.Get_login_info(46, out login_info);

      
        String userId = login_info.username; String passWd = login_info.password;
       

        try
        {
            if (quoteData.AccessorialsObj.TRADEPU.Equals(true) || quoteData.AccessorialsObj.TRADEDEL.Equals(true))
            {
                throw new Exception("Tradeshow not supported");
            }

            double totalCharges = 0;
            int standardDays = -1, overlengthFee = 0;
            string multPieces = "", accessorials = "";

            CookieContainer container = new CookieContainer();

            for (int i = 0; i < quoteData.m_lPiece.Length; i++)
            {
                multPieces += string.Concat("<Item>",
                        "<CmdtyClass>", quoteData.m_lPiece[i].FreightClass.Replace(".", ""),
                        "</CmdtyClass>",
                        "<Weight unit=\"lbs\">", quoteData.m_lPiece[i].Weight,
                        "</Weight>",
                        "</Item>");
            }

            // Get Overlenth Fee
            HelperFuncs.GetOverlengthFee(ref quoteData.m_lPiece, ref overlengthFee, 168, 168, 168, 89, 89, 89); // $88.25 per shipment

            #region Accessorials

            if (quoteData.AccessorialsObj.INSDEL)
            {
                accessorials += "<Accessorial>DID</Accessorial>";
            }
            if (quoteData.AccessorialsObj.RESDEL)
            {
                accessorials += "<Accessorial>RSD</Accessorial>";
            }
            if (quoteData.AccessorialsObj.RESPU)
            {
                accessorials += "<Accessorial>RSO</Accessorial>";
            }
            if (quoteData.AccessorialsObj.CONDEL)
            {
                accessorials += "<Accessorial>CSD</Accessorial>";
            }
            if (quoteData.AccessorialsObj.CONPU)
            {
                accessorials += "<Accessorial>OCS</Accessorial>";
            }
            /*if (AccessorialsObj.TRADEDEL)
            {
               request.Append("<AccessorialItem>");
                request.Append("<Code>InsideDelivery</Code>");
                request.Append("</AccessorialItem>");
            }
            if (AccessorialsObj.TRADEPU)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>InsideDelivery</Code>");
                request.Append("</AccessorialItem>");
            }*/
            if (quoteData.AccessorialsObj.LGPU)
            {
                accessorials += "<Accessorial>OLG</Accessorial>";
            }
            if (quoteData.AccessorialsObj.LGDEL)
            {
                accessorials += "<Accessorial>DLG</Accessorial>";
            }
            if (quoteData.AccessorialsObj.APTDEL)
            {
                accessorials += "<Accessorial>DNC</Accessorial>";
            }
            if (quoteData.isHazmat)
            {
                accessorials += "<Accessorial>ZHM</Accessorial>";
            }

            #endregion

            // Format Today's Date into mm/dd/yy format 
            string today = DateTime.Today.Month.ToString() + "/" + DateTime.Today.Day.ToString() +
            "/" + DateTime.Today.Year.ToString().Remove(0, 2);

            #region Not used
            //string ChargeCode = "C";

            //if (acctInfo.acctNum.Equals("561452031"))
            //{
            //    ChargeCode = "P";
            //}
            //else if (quoteData.origZip.Equals("08854") || quoteData.origZip.Equals("08901") || quoteData.origZip.Equals("40109") ||
            //    quoteData.origZip.Equals("77029") || quoteData.origZip.Equals("91708"))
            //{
            //    ChargeCode = "P";
            //}
            #endregion

            string ChargeCode = acctInfo.chargeType;

            if (quoteData.origZip.Equals("08854") || quoteData.origZip.Equals("08901") || quoteData.origZip.Equals("40109") ||
                quoteData.origZip.Equals("77029") || quoteData.origZip.Equals("91708"))
            {
                ChargeCode = "P";
            }

            string rateRequest = string.Concat("<RateRequest>", "<OriginZip country=\"us\">", quoteData.origZip, "</OriginZip>",
                "<DestinationZip country=\"us\">", quoteData.destZip, "</DestinationZip>",
                "<CustNmbr shipcode=\"", acctInfo.terms, "\">", acctInfo.acctNum, "</CustNmbr>", // make dynamic
                "<ChargeCode>", ChargeCode, "</ChargeCode>",
                "<EffectiveDate>", today, "</EffectiveDate>", multPieces, accessorials,
                //"<Item>" + "<CmdtyClass>775</CmdtyClass>" + "<Weight unit=\"lbs\">667</Weight>" + "</Item>" + 
                //"<Item>" +
                //"<CmdtyClass>100</CmdtyClass>" + "<Weight unit=\"lbs\">555</Weight>" + "</Item>" +
                //"<Accessorial>SSC</Accessorial>" + "<Accessorial>DNC</Accessorial>" + "<Accessorial>GUR</Accessorial>" +
                "</RateRequest>");

            //DB.Log("Con Way (Live)", rateRequest, "");

            string[] res = getConWayFreightRate(ref userId, ref passWd, ref rateRequest);

            totalCharges = Convert.ToDouble(res[3]);
            //DB.Log("Con Way (Live)", totalCharges.ToString());

            if (!int.TryParse(res[2], out standardDays))
            {
                standardDays = -3;
            }

            if (totalCharges > 0)
            {

                gcmRateQuote.TotalPrice = totalCharges + overlengthFee;
                gcmRateQuote.DisplayName = acctInfo.displayName;
                gcmRateQuote.Documentation = "";
                gcmRateQuote.DeliveryDays = standardDays;
                gcmRateQuote.BookingKey = "#1#";
                gcmRateQuote.CarrierKey = "Con-way";

                //CarsOnTime carOnTime;
                //if (Session["onTimeDict"] != null && ((Dictionary<string, CarsOnTime>)Session["onTimeDict"]).TryGetValue("Frontline", out carOnTime))
                //{
                //    objQuote.OnTimePercent = carOnTime.onTimePercent + '%';
                //    objQuote.ShipsBetweenStates = carOnTime.delivOnTime + carOnTime.delivLate;
                //}
               
            }
            else
            {            
            }
        }
        catch (Exception exp)
        {
            DB.Log("Con Way (Live)", exp.ToString());
        }
    }

    #endregion

    #region getConWayFreightRate

    private string[] getConWayFreightRate(ref string userId, ref string passWd, ref string rateRequest)
    {
        //DB.Log("getConWayFreightRate", "getConWayFreightRate");
        //DB.Log("rateRequest", rateRequest);
        string[] res = new string[4];

        String authType = "basic";
        // The target URI for the request is SSL-secured. 
        Uri conwayUri = new Uri("https://www.Con-way.com/XMLj/X-Rate");

        /* Build Rate Request XML String. * In actual use, you would probably populate the Rating Request * parameters (Weights, Classes, Zip Codes, etc.) from data submitted * via an on-line order form or database. * You could use the .NET XmlDocument class to build the XML, and then * use the .InnerXml property to turn it into a String for the POST. * For this sample we will just hard code some dummy data. */

        // Encode the Request String and set up the POST data 
        rateRequest = System.Web.HttpUtility.UrlEncode(rateRequest);
        String postData = "RateRequest=" + rateRequest;
        ASCIIEncoding encoding = new ASCIIEncoding();
        byte[] postBuffer = encoding.GetBytes(postData);

        // Set up the HTTP Request 
        HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(conwayUri);
        wReq.ContentType = "application/x-www-form-urlencoded";
        wReq.ContentLength = postBuffer.Length;
        wReq.Method = "POST";
        wReq.Timeout = 20000;
        wReq.KeepAlive = false;
        NetworkCredential myCred = new NetworkCredential(userId, passWd);
        CredentialCache myCache = new CredentialCache();
        myCache.Add(conwayUri, authType, myCred);
        wReq.Credentials = myCache;
        wReq.PreAuthenticate = true;
        Stream reqStream = wReq.GetRequestStream();
        reqStream.Write(postBuffer, 0, postBuffer.Length);
        HttpWebResponse wResp = (HttpWebResponse)wReq.GetResponse();
        Stream respStream = wResp.GetResponseStream();
        XmlTextReader xmlReader = new XmlTextReader(respStream);
        XmlDocument xmlRateQuote = new XmlDocument();
        xmlRateQuote.Load(xmlReader);
        // The entire XML Response String 
        String respString = xmlRateQuote.InnerXml;
        //DB.Log("Con Way (Live) respString", respString);

        // Here is how to get a value out of a specific XML element: 
        String netCharge = xmlRateQuote.GetElementsByTagName("NetCharge").Item(0).InnerText;
        String transitTime = xmlRateQuote.GetElementsByTagName("TransitTime").Item(0).InnerText;
        res[0] = "success";
        res[2] = transitTime;
        res[3] = netCharge;
        //DB.Log("Con Way (Live) netCharge", netCharge);
        return res;
    }

    #endregion

    #region Volume Spot Quote

    #region Get_XPO_Access_token

    public void Get_XPO_Access_token(out string access_token)
    {
        try
        {
            Logins.Login_info login_info;
            Logins logins = new Logins();
            logins.Get_login_info(126, out login_info);

            string access_key = login_info.API_Key;
            
            string data = string.Concat("grant_type=password&username=", login_info.username, 
                "&password=", login_info.password);

            Web_client http = new Web_client
            {

                url = "https://api.ltl.xpo.com/token",
                content_type = "application/x-www-form-urlencoded",
                accept = "*/*",
                post_data = data,
                method = "POST"
            };

            http.header_names = new string[1];
            http.header_names[0] = "Authorization";
            http.header_values = new string[1];          
            http.header_values[0] = string.Concat("Basic ", access_key);

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
            access_token = "not found";
            //string str = e.ToString();
            DB.Log("Get_XPO_Access_token", e.ToString());
        }
    }

    #endregion

    #region Get_XPO_Spot_Quote_rates

    public void Get_XPO_Spot_Quote_rates(ref string access_token, ref Volume_result result)
    {
        try
        {
            #region Build Items string

            int total_units = 0;

            StringBuilder items = new StringBuilder();

            for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
            {
                items.Append(string.Concat("{ \"pieceCnt\": 3, \"packageCd\": \"PLT\", \"stackableInd\": \"N\", ",
                "\"length\": 48, \"width\": 48, \"height\": 72, \"dimensionUOM\": \"IN\", ",
                "\"grossWeight\": ", quoteData.m_lPiece[i].Weight, "\"grossWeightUOM\": \"LBS\", \"nmfcClass\": \"70\" }"));

                //if(i>0 && i < quoteData.m_lPiece.Length-1)
                //{
                //    items.Append(",");
                //} 

                items.Append(",");

                total_units += quoteData.m_lPiece[i].Units;
            }

            string items_str = items.ToString().Remove(items.ToString().Length - 1);
            //strgroupids = strgroupids.Remove(strgroupids.Length - 1);

            //DB.Log("XPO_Spot_Quote items", items_str);

            #endregion


            // Guard
            if (total_units < 4)
            {
                throw new Exception("Less than 4 units for volume XPO_Spot_Quote");
                //return;
            }

            int Total_lineal_feet = total_units * 2;

            if (quoteData.linealFeet > 0.0) // Requested by XML GCM API
            {
                Total_lineal_feet = Convert.ToInt32(quoteData.linealFeet);
            }

            #region Date

            string day = DateTime.Today.Day.ToString(), month = DateTime.Today.Month.ToString();

            if (day.Length == 1)
            {
                day = "0" + day;
            }

            if (month.Length == 1)
            {
                month = "0" + month;
            }

            #endregion

            //string data = string.Concat("{ \"requestDateTime\": \"", DateTime.Today.Year, "-", month, "-", day, "T09:30:47Z\", ",
            //    "\"shipmentValue\": { \"amt\": 1000, \"currencyCd\": \"USD\" } , \"requestedCurrency\": \"USD\", ",
            //    "\"pkupDate\": \"", DateTime.Today.Year, "-", month, "-", day, "\", \"shipperAcctId\": 697924925, ",
            //    "\"shipperPostalCd\": \"80001\", ",
            //    "\"shipperName\": \"Shipper Name\", \"consigneeAcctId\": 0, \"consigneePostalCd\": \"30303\", ",
            //    "\"consigneeName\": \"Consignee Name\", \"shipperBill2AcctId\": 0, \"consigneeBill2AcctId\": 0, ",
            //    "\"thirdBill2AcctId\": 0, \"serviceTypeCd\": \"\", ",
            //    "\"commodityLine\": ",

            //    "[{ \"pieceCnt\": 5, \"packageCd\": \"PLT\", \"stackableInd\": \"N\", ",
            //    "\"length\": 40, \"width\": 48, \"height\": 72, \"dimensionUOM\": \"IN\", ",
            //    "\"grossWeight\": 6500, \"grossWeightUOM\": \"LBS\", \"nmfcClass\": \"70\" } ],",

            //    "\"reference\": [{\"reference\": \"\", \"typeCd\": \"\"}]}");

            string data = string.Concat("{ \"requestDateTime\": \"", DateTime.Today.Year, "-", month, "-", day, "T09:30:47Z\", ",
                "\"shipmentValue\": { \"amt\": 1000, \"currencyCd\": \"USD\" } , \"requestedCurrency\": \"USD\", ",
                "\"pkupDate\": \"", DateTime.Today.Year, "-", month, "-", day, "\", \"shipperAcctId\": 697924925, ",
                "\"shipperPostalCd\": \"", quoteData.origZip, "\", ",
                "\"shipperName\": \"Shipper Name\", \"consigneeAcctId\": 0, \"consigneePostalCd\": \"", quoteData.destZip, "\", ",
                "\"consigneeName\": \"Consignee Name\", \"shipperBill2AcctId\": 0, \"consigneeBill2AcctId\": 0, ",
                "\"thirdBill2AcctId\": 0, \"serviceTypeCd\": \"\", ",
                "\"commodityLine\": ",

                "[",

                items_str,
                //"{ \"pieceCnt\": 3, \"packageCd\": \"PLT\", \"stackableInd\": \"N\", ",
                //"\"length\": 48, \"width\": 48, \"height\": 72, \"dimensionUOM\": \"IN\", ",
                //"\"grossWeight\": 3000, \"grossWeightUOM\": \"LBS\", \"nmfcClass\": \"70\" }, ",

                //"{ \"pieceCnt\": 3, \"packageCd\": \"PLT\", \"stackableInd\": \"N\", ",
                //"\"length\": 48, \"width\": 48, \"height\": 72, \"dimensionUOM\": \"IN\", ",
                //"\"grossWeight\": 3000, \"grossWeightUOM\": \"LBS\", \"nmfcClass\": \"70\" } ",
                "],",

                "\"reference\": [{\"reference\": \"\", \"typeCd\": \"\"}]}");

            Web_client http = new Web_client
            {

                url = "https://api.ltl.xpo.com/rating/1.0/spotquotes",
                content_type = "application/json",
                accept = "*/*",
                post_data = data,
                method = "POST"
            };

            http.header_names = new string[1];
            http.header_names[0] = "Authorization";
            http.header_values = new string[1];
            http.header_values[0] = string.Concat("Bearer ", access_token);

            string doc = http.Make_http_request();

            #region Parse result

            string[] tokens = new string[4];
            tokens[0] = "totChargeAmt\":";
            tokens[1] = "amt\"";
            tokens[2] = ":";
            tokens[3] = ",";

            //string cost_string = HelperFuncs.scrapeFromPage(tokens, doc);

            double.TryParse(HelperFuncs.scrapeFromPage(tokens, doc), out double total_cost);

            //--

            tokens[0] = "spotQuoteNbr\":";
            tokens[1] = ":";
            tokens[2] = "\"";
            tokens[3] = "\"";

            string spotQuoteNbr = HelperFuncs.scrapeFromPage(tokens, doc);

            tokens[0] = "transitDays\":";
            tokens[1] = "\"";
            tokens[2] = ":";
            tokens[3] = ",";

            //string transitDays = HelperFuncs.scrapeFromPage(tokens, doc);

            int.TryParse(HelperFuncs.scrapeFromPage(tokens, doc), out int days);

            #endregion

            result.cost = total_cost;
            result.quote_number = spotQuoteNbr;
            result.transit_days = days;
            result.carrier_name = "XPO Spot Quote";

        }
        catch (Exception e)
        {
            //string str = e.ToString();
            DB.Log("Get_XPO_Spot_Quote_rates", e.ToString());
        }
    }

    #endregion

    public struct Volume_result
    {
        public double cost;
        public int transit_days;
        public string carrier_name, quote_number, scac;
    }

    #endregion

    #region Can_get_XPO_rate

    public bool Can_get_XPO_rate(ref double total_units)
    {
        if (total_units >= 6 && total_units <= 9)
            return true;
        else
            return false;
    }

    #endregion

}