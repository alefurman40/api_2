#region Using

using System;
using gcmAPI.Models.LTL;
using System.Text;
using System.Collections.Generic;
//using gcmAPI.ODFL_RateService;
//using SampleRatingWebServiceClient.RateServiceReference;
using System.Xml;
using gcmAPI.Models.Utilities;

#endregion

namespace gcmAPI.Models.Carriers
{
    public class ODFL
    {
        CarrierAcctInfo acctInfo;
        QuoteData quoteData;

        // Constructor
        public ODFL(ref CarrierAcctInfo acctInfo, ref QuoteData quoteData)
        {
            this.acctInfo = acctInfo;
            this.quoteData = quoteData;
        }

        #region GetResultObjectFromOldDominion_XML

        public void GetResultObjectFromOldDominion_XML(ref GCMRateQuote gcmRateQuote)
        {

            try
            {
                DB.Log("GetResultObjectFromOldDominion zips, acct",
                    string.Concat(quoteData.origZip, ", ", quoteData.destZip, ", ", acctInfo.acctNum));

                if (quoteData.AccessorialsObj.TRADEPU.Equals(true) || quoteData.AccessorialsObj.TRADEDEL.Equals(true))
                {
                    return;
                }

                #region Variables

                string url = "", referrer, contentType, accept, method, doc = "", data = "";

                url = "https://www.odfl.com/wsRate_v4/RateService";
                referrer = "";
                contentType = "text/xml; charset=utf-8";
                method = "POST";
                accept = "*/*";

                #endregion

                #region Build Freight Items

                //StringBuilder freightItems = new StringBuilder();
                string freightItems = string.Empty;
                string freightClass = string.Empty;
                for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
                {
                    if (quoteData.m_lPiece[i].FreightClass.Equals("77.5"))
                    {
                        freightClass = "77";
                    }
                    else if (quoteData.m_lPiece[i].FreightClass.Equals("92.5"))
                    {
                        freightClass = "92";
                    }
                    else
                    {
                        freightClass = quoteData.m_lPiece[i].FreightClass;
                    }
                    freightItems += string.Concat(
                        "<freightItems><ratedClass>", freightClass,
                        "</ratedClass><weight>", quoteData.m_lPiece[i].Weight,
                        "</weight></freightItems>");
                }

                #endregion

                #region Accessorials

                /*
              
                    ARN Arrival Notification
                    HYD Lift Gate Service
                COD COD - requires COD amount
                    CA Appointment
                IND Insurance - requires insurance amount
                    IDC Inside Pickup/Delivery
                    OVL Overlength Article - 12' but less than 20'
                    RDC Residential/Non-Commercial Pickup/Delivery
                    CSD Construction Site Pickup/Delivery
                CDC Schools, Colleges, Churches Pickup/Delivery
                LDC Secured or Limited Access Pickup/Delivery
                SWD Self Storage Delivery
                PFF Protect From Freezing
                    HAZ Hazardous Material
                    OV2 Overlength Article - 20' to 28'
                DSM Delivery Service to Mines
                HSO – scale tickets per military move (Home Moves only)
                INH – home move insurance (Home Moves only)
                 
                 */

                StringBuilder sbAccessorials = new StringBuilder();

                if (quoteData.AccessorialsObj.RESPU || quoteData.AccessorialsObj.RESDEL)
                {
                    sbAccessorials.Append("<accessorials>RDC</accessorials>");
                }

                if (quoteData.AccessorialsObj.CONPU || quoteData.AccessorialsObj.CONDEL)
                {
                    sbAccessorials.Append("<accessorials>CSD</accessorials>");
                }

                if (quoteData.AccessorialsObj.INSDEL)
                {
                    sbAccessorials.Append("<accessorials>IDC</accessorials>");
                }

                if (quoteData.AccessorialsObj.LGPU || quoteData.AccessorialsObj.LGDEL)
                {
                    sbAccessorials.Append("<accessorials>HYD</accessorials>");
                }

                if (quoteData.AccessorialsObj.APTDEL)
                {
                    sbAccessorials.Append("<accessorials>ARN</accessorials>");
                }

                if (quoteData.isHazmat.Equals(true))
                {
                    sbAccessorials.Append("<accessorials>HAZ</accessorials>");
                }

                double maxDim = 0;
                HelperFuncs.GetMaxDimension(ref quoteData.m_lPiece, ref maxDim);

                if (maxDim >= 240)
                {
                    sbAccessorials.Append("<accessorials>OVL</accessorials>");
                }
                else if (maxDim >= 144)
                {
                    sbAccessorials.Append("<accessorials>OV2</accessorials>");
                }
                else
                {
                    // Do nothing
                }

                #endregion

                #region Country

                string originCountry = "usa";
                string destinationCountry = "usa";

                if (quoteData.origCountry.Equals("CANADA"))
                {
                    originCountry = "CAN";
                }

                if (quoteData.destCountry.Equals("CANADA"))
                {
                    destinationCountry = "CAN";
                }

                #endregion

                #region Post Data

                data = string.Concat("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">",
                    "<getLTLRateEstimate xmlns=\"http://myRate.ws.odfl.com/\"><arg0 xmlns=\"\">",

                    sbAccessorials,
                    "<destinationCountry>", destinationCountry, "</destinationCountry><destinationPostalCode>", quoteData.destZip, "</destinationPostalCode>",
                    freightItems,
                    //"<freightItems><ratedClass>55</ratedClass><weight>2012</weight></freightItems>",
                    "<odfl4MePassword>", acctInfo.password, "</odfl4MePassword><odfl4MeUser>", acctInfo.username, "</odfl4MeUser>",
                    "<odflCustomerAccount>", acctInfo.acctNum, "</odflCustomerAccount>",
                    "<originCountry>", originCountry, "</originCountry><originPostalCode>", quoteData.origZip, "</originPostalCode>",
                    "<requestReferenceNumber>false</requestReferenceNumber><shipType>LTL</shipType></arg0></getLTLRateEstimate></s:Body></s:Envelope>");

                DB.Log("odfl request", data);

                #endregion

                #region Headers

                string[] headerNames = new string[1];
                string[] headerValues = new string[1];

                headerNames[0] = "SOAPAction";

                headerValues[0] = "http://myRate.ws.odfl.com/RateDelegate/getLTLRateEstimateRequest";

                #endregion

                doc = (string)HelperFuncs.generic_http_request_addHeaders("string", null, url, referrer, contentType, accept, method,
                   data, false, headerNames, headerValues);

                DB.Log("odfl response", doc);

                #region Gather results into an object

                // Gather results into an object
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(doc);

                XmlNodeList nodeList = xmlDoc.GetElementsByTagName("netFreightCharge");

                if (nodeList.Count > 0)
                {
                    string netFreightCharge = nodeList[0].InnerText.Trim();
                    DB.Log("odfl", netFreightCharge);

                    nodeList = xmlDoc.GetElementsByTagName("serviceDays");

                    string serviceDays = string.Empty;
                    if (nodeList.Count > 0)
                    {
                        serviceDays = nodeList[0].InnerText.Trim();
                        DB.Log("odfl serviceDays", serviceDays);
                    }

                    if (double.TryParse(netFreightCharge, out double TotalPrice))
                    {
                        DB.Log("odfl parsed to double", TotalPrice.ToString());

                        #region Add manual Accessorial charges

                        if (quoteData.AccessorialsObj.RESPU && quoteData.AccessorialsObj.RESDEL)
                        {
                            TotalPrice += 70;
                        }

                        if (quoteData.AccessorialsObj.CONPU && quoteData.AccessorialsObj.CONDEL)
                        {
                            TotalPrice += 70;
                        }

                        if (quoteData.AccessorialsObj.LGPU && quoteData.AccessorialsObj.LGDEL)
                        {
                            TotalPrice += 70;
                        }

                        #endregion


                        gcmRateQuote = new GCMRateQuote
                        {
                            TotalPrice = TotalPrice,
                            DisplayName = acctInfo.displayName,
                            Documentation = "http://www.odfl.com/service/Expedited/ODFL_660.pdf",
                            BookingKey = acctInfo.bookingKey,
                            CarrierKey = acctInfo.carrierKey
                        };

                        if (int.TryParse(serviceDays, out int DeliveryDays))
                        {
                            gcmRateQuote.DeliveryDays = DeliveryDays;
                        }
                        //DB.Log("odfl ", "after service days");
                    }
                }

                #endregion
            }
            catch (Exception e)
            {
                DB.Log("odfl", e.ToString());
            }
        }

        #endregion

        #region SetOldDominionAccountNumber

        public void SetOldDominionAccountNumber()
        {
          
            if (quoteData.username.Equals("rbrubber") || quoteData.username.Equals("milanrubber"))
            {
                acctInfo.acctNum = AppCodeConstants.ODFL_CompanyAccounts.rbrubber;
            }
            else if (quoteData.username.Equals("durachem"))
            {              
                if (quoteData.origZip.Equals(CompanyZips.MainStreet) || quoteData.destZip.Equals(CompanyZips.MainStreet))
                {
                    acctInfo.acctNum = AppCodeConstants.ODFL_CompanyAccounts.MainStreet;
                }
                else if (quoteData.origZip.Equals(CompanyZips.CypresAve) || quoteData.destZip.Equals(CompanyZips.CypresAve))
                {
                    acctInfo.acctNum = AppCodeConstants.ODFL_CompanyAccounts.CypresAve;
                }
                else if (quoteData.origZip.Equals(CompanyZips.PowellStreet) || quoteData.destZip.Equals(CompanyZips.PowellStreet))
                {
                    acctInfo.acctNum = AppCodeConstants.ODFL_CompanyAccounts.PowellStreet;
                }
                else if (quoteData.origZip.Equals(CompanyZips.GriffsRoad) || quoteData.destZip.Equals(CompanyZips.GriffsRoad))
                {
                    acctInfo.acctNum = AppCodeConstants.ODFL_CompanyAccounts.GriffsRoad;
                }
                else if (quoteData.origZip.Equals(CompanyZips.JerseyAve) || quoteData.destZip.Equals(CompanyZips.JerseyAve))
                {
                    acctInfo.acctNum = AppCodeConstants.ODFL_CompanyAccounts.JerseyAve;
                }
                else if (quoteData.origZip.Equals(CompanyZips.SecondStreet) || quoteData.destZip.Equals(CompanyZips.SecondStreet))
                {
                    acctInfo.acctNum = AppCodeConstants.ODFL_CompanyAccounts.SecondStreet;
                }
                else if (quoteData.origZip.Equals(CompanyZips.EmeraldPerformance) || quoteData.destZip.Equals(CompanyZips.EmeraldPerformance))
                {
                    acctInfo.acctNum = AppCodeConstants.ODFL_CompanyAccounts.EmeraldPerformance;
                }
                else if (quoteData.origZip.Equals(CompanyZips.ValtrisChemical) || quoteData.destZip.Equals(CompanyZips.ValtrisChemical))
                {
                    acctInfo.acctNum = AppCodeConstants.ODFL_CompanyAccounts.ValtrisChemical;
                }
                else if (quoteData.origZip.Equals(CompanyZips.SummerLeaRoad) || quoteData.destZip.Equals(CompanyZips.SummerLeaRoad))
                {
                    acctInfo.acctNum = AppCodeConstants.ODFL_CompanyAccounts.SummerLeaRoad;
                }
                else if (quoteData.origZip.Equals(CompanyZips.AdamsWarehouse) || quoteData.destZip.Equals(CompanyZips.AdamsWarehouse))
                {
                    acctInfo.acctNum = AppCodeConstants.ODFL_CompanyAccounts.AdamsWarehouse;
                }
                else
                {
                    // Do nothing
                }
            }
            else
            {
                // Do nothing
            }
        }

        #endregion

        #region Accounts, Locations info

        class CompanyZips
        {
            public static string MainStreet = "V6B3Z7";
            public static string CypresAve = "91708";
            public static string PowellStreet = "94608";
            public static string GriffsRoad = "40109";
            public static string JerseyAve = "08901";
            public static string SecondStreet = "08854";
            public static string EmeraldPerformance = "45237";
            public static string ValtrisChemical = "44112";
            public static string SummerLeaRoad = "L6T4X3";
            public static string AdamsWarehouse = "77029";
        }
        
        #endregion

    }
}
