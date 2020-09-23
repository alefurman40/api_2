#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using gcmAPI.Models;
using gcmAPI.Models.LTL;
using System.Xml;
using gcmAPI.Models.Utilities;
using static gcmAPI.Models.Utilities.Mail;

#endregion

public class USF
{
    CarrierAcctInfo acctInfo;
    QuoteData quoteData;

    // Constructor
    public USF(CarrierAcctInfo acctInfo, ref QuoteData quoteData)
    {
        this.acctInfo = acctInfo;
        this.quoteData = quoteData;
    }

    #region GetRateFromUSF

    public GCMRateQuote GetRateFromUSF()
    {
        GCMRateQuote gcmRateQuote = new GCMRateQuote();

        GetResultObjectFromUSFReddaway(ref gcmRateQuote);
          
        bool isSpc = false;
        if (acctInfo.acctNum.Equals("0875353"))
        {
            isSpc = true;
        }

        double overlengthFee = 0.0;

        overlengthFee = HelperFuncs.getUSF_OverlengthFee(ref quoteData.m_lPiece);

        gcmRateQuote.TotalPrice += overlengthFee;

        DB.Log("USF overlength fee", overlengthFee.ToString());

        // Get Overlenth Fee
        //HelperFuncs.GetOverlengthFee(ref quoteData.m_lPiece, ref overlengthFee, 96, 216, 288, 100, 150, 200);

        /*
           8-12 feet   96 to  144         $120
           12-16 feet  144 to 192      $300
           16-20 feet   192 to 240        $600
           20 feet plus   240+     $1200
           */

        #region Accessorials additions

        if (gcmRateQuote != null && gcmRateQuote.TotalPrice > 0)
        {
            if (quoteData.AccessorialsObj.LGPU)
            {
                if (isSpc.Equals(true))
                {
                    gcmRateQuote.TotalPrice += 50;
                }
                else
                {
                    gcmRateQuote.TotalPrice += 65;
                }
            }

            if (quoteData.AccessorialsObj.LGDEL)
            {
                if (isSpc.Equals(true))
                {
                    gcmRateQuote.TotalPrice += 50;
                }
                else
                {
                    gcmRateQuote.TotalPrice += 65;
                }
            }

            if (quoteData.AccessorialsObj.INSDEL)
            {
                gcmRateQuote.TotalPrice += 55;
            }

            if (quoteData.AccessorialsObj.APTPU)
            {
                if (isSpc.Equals(false))
                {
                    gcmRateQuote.TotalPrice += 15;
                }
            }

            if (quoteData.AccessorialsObj.APTDEL)
            {
                if (isSpc.Equals(false))
                {
                    gcmRateQuote.TotalPrice += 15;
                }
            }
        }

        //DB.Log("USFReddawayPrepaid gcmRateQuote.TotalPrice after", gcmRateQuote.TotalPrice.ToString());

        double totalShipWeight = quoteData.totalWeight;

        DB.Log("USFReddawayPrepaid totalShipWeight", quoteData.totalWeight.ToString());

        //for (int i = 0; i < lineItems.Length; i++)
        //{
        //    totalShipWeight += lineItems[i].Weight;
        //}


        if (quoteData.AccessorialsObj.RESDEL || quoteData.AccessorialsObj.CONDEL)
        {
            double resDelCharge = (totalShipWeight / 100) * 7.2;
            if (resDelCharge < 130)
            {
                gcmRateQuote.TotalPrice += 130;
            }
            else
            {
                gcmRateQuote.TotalPrice += resDelCharge;
            }

        }
        if (quoteData.AccessorialsObj.RESPU || quoteData.AccessorialsObj.CONPU)
        {
            double resPickCharge = (totalShipWeight / 100) * 7.2;
            if (resPickCharge < 130)
            {
                gcmRateQuote.TotalPrice += 130;
            }
            else
            {
                gcmRateQuote.TotalPrice += resPickCharge;
            }
        }

        if (quoteData.isHazmat.Equals(true))
        {
            gcmRateQuote.TotalPrice += 20.5;
        }

        #endregion

        return gcmRateQuote;
    }

    #endregion

    #region GetResultObjectFromUSFReddaway

    private void GetResultObjectFromUSFReddaway(ref GCMRateQuote gcmRateQuote)
    {
        try
        {

            Logins.Login_info login_info;
            Logins logins = new Logins();
            logins.Get_login_info(120, out login_info);

            string url = string.Concat("https://api.reddawayregional.com/api/RateQuote/doRateQuote?accessKey=",
                login_info.API_Key,
                
                "&accountId=", acctInfo.acctNum,
                "&originZipCode=", quoteData.origZip, "&destZipCode=", quoteData.destZip, "&direction=3rdParty&chargeType=", acctInfo.chargeType, 
                "&AspxAutoDetectCookieSupport=1");
            
            for (byte i = 1; i <= quoteData.m_lPiece.Length; i++)
            {

                url += string.Concat("&shipmentClass", i, "=", quoteData.m_lPiece[i - 1].FreightClass,
                    "&shipmentWeight", i, "=", quoteData.m_lPiece[i - 1].Weight);
            }

            string responseFromServer = (string)HelperFuncs.generic_http_request_3("string", null, url, "", "",
               "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8", "GET", "", false, false, "", "");

            //DB.Log("USFReddawayPrepaid responseFromServer", responseFromServer);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(responseFromServer);
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("TOTAL_COST");
            double totalCharges = 0;
            if (nodeList.Count > 0)
            {
                totalCharges = Convert.ToDouble(nodeList[0].InnerText);
            }

            if (quoteData.isHazmat.Equals(true))
            {
                totalCharges += 20.5;
            }
            
            nodeList = xmlDoc.GetElementsByTagName("SERVICEDAYS");
            int standardDays = 0;
            if (nodeList.Count > 0)
            {
                standardDays = Convert.ToInt32(nodeList[0].InnerText);
            }

            nodeList = xmlDoc.GetElementsByTagName("ASCLASS");

            int transitDays;
            transitDays = standardDays;
          
            gcmRateQuote.TotalPrice = totalCharges;
            
            gcmRateQuote.DisplayName = acctInfo.displayName;
            gcmRateQuote.BookingKey = acctInfo.bookingKey;
            gcmRateQuote.CarrierKey = acctInfo.carrierKey;
            
            gcmRateQuote.DeliveryDays = transitDays;

        }
        catch (Exception exp)
        {
            gcmRateQuote = null;
            DB.Log("USFReddawayPrepaid", exp.ToString());
        }
    }

    #endregion

    // For Volume quoting
    #region Get_USF_API_Volume_Quote

    public Volume_result Get_USF_API_Volume_Quote(ref int total_units)
    {
        try
        {
            //quoteData.totalWeight

            //DB.Log("quoteData.totalWeight", quoteData.totalWeight.ToString());

            int Total_lineal_feet = total_units * 2;

            if (quoteData.linealFeet > 0.0) // Requested by XML GCM API
            {
                Total_lineal_feet = Convert.ToInt32(quoteData.linealFeet);
            }

            Logins.Login_info login_info;
            Logins logins = new Logins();
            logins.Get_login_info(120, out login_info);

            string url = string.Concat("https://api.reddawayregional.com/api/SpotQuote/doSpotQuote?accessKey=", login_info.API_Key,
                
                //"&originZip=29154&destinationZip=30303",
                "&originZip=", quoteData.origZip, "&destinationZip=", quoteData.destZip,
                "&weight=", quoteData.totalWeight , "&handlingUnits=", total_units, 
                "&palletized=Y&stackable=N&lengthInFeet=", Total_lineal_feet);

            //DB.Log("Get_USF_API_Volume_Quote request", url);

            Web_client http = new Web_client
            {
                url = url,
                content_type = "",
                accept = "*/*",
                method = "GET"
            };

            //DB.Log("gcmAPI_Get_LOUP_Rates before send request", "before send request");

            string doc = http.Make_http_request();

            //DB.Log("Get_USF_API_Volume_Quote response", doc);

            Volume_result volume_result = new Volume_result();

            #region Parse result

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(doc);
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("CHARGES");
            double CHARGES = 0;
            if (nodeList.Count > 0)
            {
                CHARGES = Convert.ToDouble(nodeList[0].InnerText);
                volume_result.cost = CHARGES;
            }

            nodeList = xmlDoc.GetElementsByTagName("FREIGHTCHARGES");
            double FREIGHTCHARGES = 0;
            if (nodeList.Count > 0)
            {
                FREIGHTCHARGES = Convert.ToDouble(nodeList[0].InnerText);
            }

            if (FREIGHTCHARGES != CHARGES)
            {
                #region Send email

                EmailInfo info = new EmailInfo
                {
                    to = AppCodeConstants.Alex_email,
                    fromAddress = AppCodeConstants.Alex_email,
                    fromName = "Alex",
                    subject = "FREIGHTCHARGES != CHARGES"
                };
                Mail mail = new Mail(ref info);
                mail.SendEmail();

                #endregion
            }

            //if (quoteData.isHazmat.Equals(true))
            //{
            //    totalCharges += 20.5;
            //}

            nodeList = xmlDoc.GetElementsByTagName("SERVICEDAYS");
            int SERVICEDAYS = 0;
            if (nodeList.Count > 0)
            {
                SERVICEDAYS = Convert.ToInt32(nodeList[0].InnerText);
                volume_result.transit_days = SERVICEDAYS;
            }

            nodeList = xmlDoc.GetElementsByTagName("SPOTQUOTEID");
            string SPOTQUOTEID = string.Empty;
            if (nodeList.Count > 0)
            {
                SPOTQUOTEID = nodeList[0].InnerText;
                volume_result.quote_number = SPOTQUOTEID;
            }

            volume_result.carrier_name = "USF Holland";

            #endregion

            return volume_result;
        }
        catch(Exception e)
        {
            DB.Log("Get_USF_API_Volume_Quote", e.ToString());
            Volume_result volume_result = new Volume_result();
            return volume_result;
        }
    }

    #endregion

    public struct Volume_result
    {
        public double cost;
        public int transit_days;
        public string carrier_name, quote_number, scac;
    }
}