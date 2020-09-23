#region Using

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
//using System.IO;
//using System.Data;
//using System.Data.Sql;
using System.Data.SqlClient;
//using gcmAPI.UPS_PackagePickupService;
//using System.Linq;
//using System.Xml;
//using gcmAPI.AAACooperRateService;
//using gcmAPI.gcmWebService;
//using System.Net.Http.Formatting;
using System.Web.Script.Serialization;
using gcmAPI.Models.LTL;

#endregion

/// <summary>
/// Summary description for SharedLTL
/// </summary>
/// 
// WS Live

public static class SharedLTL
{
    #region Variables

    public struct LTLBookingInfo
    {
        public string origName, origEmail, origCompany, origPhone, origFax, origAddress1, origAddress2, origCity, origState, origZip;


        public string destName, destEmail, destCompany, destPhone, destFax, destAddress1, destAddress2, destCity, destState, destZip;



        //public string origName, origEmail, origCompany, origPhone, origFax, origAddress1, origAddress2, origCity, origState, origZip, 
        //    origDispatchAddressesId;

        //public string destName, destEmail, destCompany, destPhone, destFax, destAddress1, destAddress2, destCity, destState, destZip,
        //    destDispatchAddressesId;

        public string SessionID, bookingKey, customerType, thirdPartyBilling, readyTime, closeTime, bolSendTo, poNumber, comments;

        public DateTime shipmentDate;
        public bool insuranceRequired;
        public double declaredValue;

        public bookingLineItems[] lineItems;
    }

    public struct bookingLineItems
    {
        public string Tag, Description;
        public int NumberOfPallet;
    }

    public static bool isInDefaultLoginTable, isDefaultLoginDensityBased;

    public struct CarriersResult
    {
        public HelperFuncs.dlsShipInfo packInfo;
        public HelperFuncs.upsPackageShipInfo upsPackageShipInfo;
        public GCMRateQuote[] totalQuotes;
        public int elapsed_milliseconds_DLS_Genera, elapsed_milliseconds_P44;
    }

    public struct DispatchInfo
    {
        public string origCompName, destCompName, origCity, origState, destCity, destState, origZip, destZip, origAddress1, origAddress2, destAddress1, destAddress2;
        public string poNum;
        public DateTime puDate;
        public int shipmentID, totalPieces, totalPallets;
        public double totalWeight;
        public DateTime puReadyTime, dockCloseTime;
        public bool hazMat;
    }

    public struct freightCenterRate
    {
        public string rateId, carrierName, carrierCode, carrierScac, mode, modeId, serviceDays, serviceName, serviceCode,
            totalCharge;
    }

    public struct BolParams
    {

        // Shipper
        public string ShipperCompName, ShipperAddr1, ShipperAddr2, ShipperAddr3, ShipperPhone, ShipperFAX;//ShipperCity, ShipperState, ShipperZip

        // Consignee
        public string ConsCompName, ConsAddr1, ConsAddr2, ConsAddr3, ConsPhone, ConsFAX;//ConsCity, ConsState, ConsZip

        // Carrier       
        public string CarrierCompName, CarrierPhone, CarrierFAX;

        // Items
        public List<bolItem> bolItems;

        // Various
        public string MsgBillTo, MsgBillToCaption, PUDate, PONumber, ProNbr, ShipmentID, AdditionalServices, UltimateConsignee, ItemNotes, SegmentComments;

        // reportPath
        public string reportPath;

        // Multi segment
        public bool isMultiSegment;

        // Saia
        public bool isSaia;
    }

    public struct bolItem
    {
        public string fClass, Descr, HR, Kind, Nmfc, Pcs, Units, WtLBS;
    }
    
    #endregion

    #region AddItemsToQuoteArray

    public static GCMRateQuote[] AddItemsToQuoteArray(GCMRateQuote[] objQuoteArray, GCMRateQuote newQuoteItem)
    {
        int intIndex;
        GCMRateQuote tempQuote;
        bool blnIsAdded = false;
        string strMFWDisplayName;
        GCMRateQuote[] objResultArray;


        if (objQuoteArray != null && newQuoteItem != null)
        {
            string strYRCIndexesofMFW = "";

            for (intIndex = 0; intIndex < objQuoteArray.Length; intIndex++)
            {
                tempQuote = objQuoteArray[intIndex];
                if (tempQuote != null)
                {
                    strMFWDisplayName = tempQuote.DisplayName;//.Replace(strMFWIdentification,"");

                    if ((tempQuote.CarrierKey == "MFW" && tempQuote.BookingKey.Trim().Equals("#1#")) &&
                        (strMFWDisplayName.Trim().ToLower().Contains(newQuoteItem.DisplayName.Trim().ToLower()) || newQuoteItem.DisplayName.Trim().ToLower().Equals(strMFWDisplayName.Trim().ToLower()))
                        && !strMFWDisplayName.Trim().ToLower().Contains("max liability"))
                    {
                        if (IsOurRateBetter(tempQuote, newQuoteItem))
                        {
                            //newQuoteItem.DisplayName += strAESIdentification;
                            objQuoteArray[intIndex] = newQuoteItem;
                            return objQuoteArray;
                        }
                        else
                            return objQuoteArray;
                    }
                }
            }

            // If the carrier of new quote does not exist in MFW;
            int length;

            if (objQuoteArray != null)
                length = objQuoteArray.Length;
            else
                length = 0;

            length = length + 1;

            objResultArray = new GCMRateQuote[length];

            int index = 0;
            foreach (GCMRateQuote quote in objQuoteArray)
            {
                objResultArray[index] = quote;
                index = index + 1;
            }
            //////////
            //newQuoteItem.DisplayName += strAESIdentification;
            objResultArray[index] = newQuoteItem;

        }
        else
        {
            objResultArray = new GCMRateQuote[1];
            //newQuoteItem.DisplayName += strAESIdentification;
            objResultArray[0] = newQuoteItem;
        }

        return objResultArray;


    }

    #endregion

    #region IsOurRateBetter

    private static bool IsOurRateBetter(GCMRateQuote tempQuote, GCMRateQuote newQuoteItem)
    {
        double dblRate;
        dblRate = Convert.ToDouble(Convert.ToDouble(tempQuote.TotalPrice * 105) / 100.00);
        if (newQuoteItem.TotalPrice <= dblRate || (newQuoteItem.TotalPrice - tempQuote.TotalPrice) <= 10)
            return true;
        else
            return false;
    }

    #endregion

    #region CTII
    
    #region getClassForCTII

    public static void getClassForCTII(ref decimal ctiiFreightClass, ref decimal totalDensity)
    {
        #region Get ctiiFreightClass by density
        if (totalDensity < 0)
        {
            //SharedFunctions.writeToAAFES_Logs("AAFES", string.Concat("Density was less than zero po id: ", shipInfo.PO_ID.ToString()));
            //continue;
            HelperFuncs.writeToSiteErrors("getClassForCTII", "Density was less than zero");
            ctiiFreightClass = -3;
        }
        else if (totalDensity >= 0 && totalDensity < 1)
        {
            ctiiFreightClass = 500;
        }
        else if (totalDensity >= 1 && totalDensity < 2)
        {
            ctiiFreightClass = 400;
        }
        else if (totalDensity >= 2 && totalDensity < 3)
        {
            ctiiFreightClass = 300;
        }
        else if (totalDensity >= 3 && totalDensity < 4)
        {
            ctiiFreightClass = 250;
        }
        else if (totalDensity >= 4 && totalDensity < 5)
        {
            ctiiFreightClass = 200;
        }
        else if (totalDensity >= 5 && totalDensity < 6)
        {
            ctiiFreightClass = 175;
        }
        else if (totalDensity >= 6 && totalDensity < 7)
        {
            ctiiFreightClass = 150;
        }
        else if (totalDensity >= 7 && totalDensity < 8)
        {
            ctiiFreightClass = 125;
        }
        else if (totalDensity >= 8 && totalDensity < 9)
        {
            ctiiFreightClass = 110;
        }
        else if (totalDensity >= 9 && totalDensity < 10.5M)
        {
            ctiiFreightClass = 100;
        }
        else if (totalDensity >= 10.5M && totalDensity < 12)
        {
            ctiiFreightClass = 92.5M;
        }
        else if (totalDensity >= 12 && totalDensity < 13.5M)
        {
            ctiiFreightClass = 85;
        }
        else if (totalDensity >= 13.5M && totalDensity < 15)
        {
            ctiiFreightClass = 77.5M;
        }
        else if (totalDensity >= 15 && totalDensity < 22.5M)
        {
            ctiiFreightClass = 70;
        }
        else if (totalDensity >= 22.5M && totalDensity < 30)
        {
            ctiiFreightClass = 65;
        }
        else if (totalDensity >= 30 && totalDensity < 35)
        {
            ctiiFreightClass = 60;
        }
        else if (totalDensity >= 35 && totalDensity < 50)
        {
            ctiiFreightClass = 55;
        }
        else
        {
            ctiiFreightClass = 50;
        }


        #endregion
    }

    #endregion

    #endregion

    #region getClassForRRTS_AAFES

    public static void getClassForRRTS_AAFES(ref decimal rrtsFreightClass, ref decimal totalDensity)
    {
        #region Get rrtsFreightClass by density
        if (totalDensity <= 0)
        {
            //SharedFunctions.writeToAAFES_Logs("AAFES", string.Concat("Density was less than zero po id: ", shipInfo.PO_ID.ToString()));
            //continue;
            HelperFuncs.writeToSiteErrors("rrtsFreightClass", "Density was less than zero");
            rrtsFreightClass = -1;
        }
        else if (totalDensity > 0 && totalDensity < 1)
        {
            rrtsFreightClass = 400;
        }
        else if (totalDensity >= 1 && totalDensity < 2)
        {
            rrtsFreightClass = 300;
        }
        else if (totalDensity >= 2 && totalDensity < 4)
        {
            rrtsFreightClass = 250;
        }
        else if (totalDensity >= 4 && totalDensity < 6)
        {
            rrtsFreightClass = 175;
        }
        else if (totalDensity >= 6 && totalDensity < 8)
        {
            rrtsFreightClass = 125;
        }
        else if (totalDensity >= 8 && totalDensity < 10)
        {
            rrtsFreightClass = 100;
        }
        else if (totalDensity >= 10 && totalDensity < 12)
        {
            rrtsFreightClass = 92.5M;
        }
        else if (totalDensity >= 12 && totalDensity < 15)
        {
            rrtsFreightClass = 85;
        }
        else if (totalDensity >= 15 && totalDensity < 22.5M)
        {
            rrtsFreightClass = 70;
        }
        else if (totalDensity >= 22.5M && totalDensity < 30)
        {
            rrtsFreightClass = 65;
        }
        else
        {
            rrtsFreightClass = 60;
        }


        #endregion
    }

    #endregion

    #region Get_class_by_density
    /*
     * function GetClassByDensity(totDensity) {
    var classDensityBased = "-1";

    if (totDensity < 1)
        classDensityBased = "400";
    else if (totDensity >= 1 && totDensity < 2)
        classDensityBased = "300";
    else if (totDensity >= 2 && totDensity < 4)
        classDensityBased = "250";
    else if (totDensity >= 4 && totDensity < 6)
        classDensityBased = "175";
    else if (totDensity >= 6 && totDensity < 8)
        classDensityBased = "125";
    else if (totDensity >= 8 && totDensity < 10)
        classDensityBased = "100";
    else if (totDensity >= 10 && totDensity < 12)
        classDensityBased = "92.5";
    else if (totDensity >= 12 && totDensity < 15)
        classDensityBased = "85";
    else if (totDensity >= 15 && totDensity < 22.5)
        classDensityBased = "70";
    else if (totDensity >= 22.5 && totDensity < 30)
        classDensityBased = "65";
    else //greater than 30
        classDensityBased = "60";

    return classDensityBased;
}
     */
    public static void Get_class_by_density(ref QuoteData quoteData)
    {
        #region Get quoteData.calculated_freight_class by density
        if (quoteData.totalDensity <= 0)
        {
          
            HelperFuncs.writeToSiteErrors("quoteData.calculated_freight_class", "Density was less than zero");
            quoteData.calculated_freight_class = -1;
        }
        else if (quoteData.totalDensity > 0 && quoteData.totalDensity < 1)
        {
            quoteData.calculated_freight_class = 400;
        }
        else if (quoteData.totalDensity >= 1 && quoteData.totalDensity < 2)
        {
            quoteData.calculated_freight_class = 300;
        }
        else if (quoteData.totalDensity >= 2 && quoteData.totalDensity < 4)
        {
            quoteData.calculated_freight_class = 250;
        }
        else if (quoteData.totalDensity >= 4 && quoteData.totalDensity < 6)
        {
            quoteData.calculated_freight_class = 175;
        }
        else if (quoteData.totalDensity >= 6 && quoteData.totalDensity < 8)
        {
            quoteData.calculated_freight_class = 125;
        }
        else if (quoteData.totalDensity >= 8 && quoteData.totalDensity < 10)
        {
            quoteData.calculated_freight_class = 100;
        }
        else if (quoteData.totalDensity >= 10 && quoteData.totalDensity < 12)
        {
            quoteData.calculated_freight_class = 92.5;
        }
        else if (quoteData.totalDensity >= 12 && quoteData.totalDensity < 15)
        {
            quoteData.calculated_freight_class = 85;
        }
        else if (quoteData.totalDensity >= 15 && quoteData.totalDensity < 22.5)
        {
            quoteData.calculated_freight_class = 70;
        }
        else if (quoteData.totalDensity >= 22.5 && quoteData.totalDensity < 30)
        {
            quoteData.calculated_freight_class = 65;
        }
        else
        {
            quoteData.calculated_freight_class = 60;
        }


        #endregion
    }

    #endregion
    
    #region getAAACooperRate

    public static void getAAACooperRate(ref double totalCharges, ref byte transitDays, ref QuoteData quoteData, ref LTLPiece[] m_lPiece,
        ref HelperFuncs.AccessorialsObj AccessorialsObj)
    {
        #region Bill date

        string day = DateTime.Today.Day.ToString();
        string month = DateTime.Today.Month.ToString();
        string year = DateTime.Today.Year.ToString().Substring(2);

        if (day.Length.Equals(1))
        {
            day = '0' + day;
        }
        if (month.Length.Equals(1))
        {
            month = '0' + month;
        }

        #endregion

        // Define the web service object
        gcmAPI.AAACooperRateService.wsGenRateEstimate ws = new gcmAPI.AAACooperRateService.wsGenRateEstimate();

        #region Accessorials

        // For Additional Services

        var acclinelist = new List<gcmAPI.AAACooperRateService.RateEstimateRequestVOAccLine>();

        #region Liftgate
        if (AccessorialsObj.LGPU)
        {
            var accline = new gcmAPI.AAACooperRateService.RateEstimateRequestVOAccLine()
            {
                AccCode = "LGP"
            };
            acclinelist.Add(accline);
        }
        if (AccessorialsObj.LGDEL)
        {
            var accline = new gcmAPI.AAACooperRateService.RateEstimateRequestVOAccLine()
            {
                AccCode = "LGD"
            };
            acclinelist.Add(accline);
        }
        #endregion

        #region Appointment
        if (AccessorialsObj.APTPU || AccessorialsObj.APTDEL)
        {
            var accline = new gcmAPI.AAACooperRateService.RateEstimateRequestVOAccLine()
            {
                AccCode = "NCM"
            };
            acclinelist.Add(accline);
        }
        #endregion

        #region Inside Delivery
        if (AccessorialsObj.INSDEL)
        {
            var accline = new gcmAPI.AAACooperRateService.RateEstimateRequestVOAccLine()
            {
                AccCode = "ISD"
            };
            acclinelist.Add(accline);
        }
        #endregion

        #region Residential
        if (AccessorialsObj.RESPU)
        {
            var accline = new gcmAPI.AAACooperRateService.RateEstimateRequestVOAccLine()
            {
                AccCode = "RSP"
            };
            acclinelist.Add(accline);
        }
        if (AccessorialsObj.RESDEL)
        {
            var accline = new gcmAPI.AAACooperRateService.RateEstimateRequestVOAccLine()
            {
                AccCode = "RSD"
            };
            acclinelist.Add(accline);
        }
        #endregion

        #region Construction
        if (AccessorialsObj.CONPU)
        {
            var accline = new gcmAPI.AAACooperRateService.RateEstimateRequestVOAccLine()
            {
                AccCode = "CSP"
            };
            acclinelist.Add(accline);
        }
        if (AccessorialsObj.CONDEL)
        {
            var accline = new gcmAPI.AAACooperRateService.RateEstimateRequestVOAccLine()
            {
                AccCode = "CSD"
            };
            acclinelist.Add(accline);
        }
        #endregion

        #region Not used
        /*if (AccessorialsObj.TRADEPU)
        {
            var accline = new gcmAPI.AAACooperRateService.RateEstimateRequestVOAccLine()
            {
                AccCode = "LGD"
            };
            acclinelist.Add(accline);
        }
        if (AccessorialsObj.TRADEDEL)
        {
            var accline = new gcmAPI.AAACooperRateService.RateEstimateRequestVOAccLine()
            {
                AccCode = "LGD"
            };
            acclinelist.Add(accline);
        }*/
        #endregion

        #endregion

        #region Items

        var lineitemslist = new List<gcmAPI.AAACooperRateService.RateEstimateRequestVORateEstimateRequestLine>();

        string hazMatStr = "";
        for (byte i = 0; i < m_lPiece.Length; i++)
        {
            #region Hazmat
            if (m_lPiece[i].HazMat.Equals(true))
            {
                hazMatStr = "X";
            }
            else
            {
                hazMatStr = "";
            }
            #endregion

            var lineitems = new gcmAPI.AAACooperRateService.RateEstimateRequestVORateEstimateRequestLine()
            {
                Class = m_lPiece[i].FreightClass,
                Weight = Convert.ToInt16(m_lPiece[i].Weight).ToString(),
                //HandlingUnitType = "Drums",
                HandlingUnitType = "Pallets",
                HandlingUnits = Convert.ToInt16(m_lPiece[i].Quantity).ToString(),

                Length = Convert.ToInt16(m_lPiece[i].Length).ToString(),
                Width = Convert.ToInt16(m_lPiece[i].Width).ToString(),
                Height = Convert.ToInt16(m_lPiece[i].Height).ToString(),

                Hazmat = hazMatStr
            };

            lineitemslist.Add(lineitems);
        }

        #endregion

        #region Set request info

        var request = new gcmAPI.AAACooperRateService.RateEstimateRequestVO
        {
            Token = AppCodeConstants.aaa_cooper_token,
            CustomerNumber = AppCodeConstants.aaa_cooper_customer_number,
            OriginCity = quoteData.origCity,
            OriginState = quoteData.origState,
            OriginZip = quoteData.origZip,
            OriginCountryCode = "USA",
            DestinationCity = quoteData.destCity,
            DestinationState = quoteData.destState,
            DestinationZip = quoteData.destZip,
            DestinCountryCode = "USA",
            WhoAmI = "T",
            BillDate = string.Concat(month, day, year),
            //BillDate = "050415",
            PrePaidCollect = "P",
            AccLine = acclinelist.ToArray(),
            RateEstimateRequestLine = lineitemslist.ToArray()
        };

        #endregion

        // CallwsGenRateEstimate
        var responseMatches = ws.CallwsGenRateEstimate(request);

        if (!double.TryParse(responseMatches.TotalCharges, out totalCharges) || totalCharges.Equals(0))
        {
            // No rate found          
            throw new Exception(responseMatches.ErrorMessage);
        }

        byte.TryParse(responseMatches.TotalTransit, out transitDays);
        //HelperFuncs.writeToSiteErrors("aaa", responseMatches.TotalTransit);
    }

    #endregion


    #region ToJSON

    // Extension method ToJSON
    public static string ToJSON(this object obj)
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        return serializer.Serialize(obj);
    }

    //public static string ToJSON(this object obj, int recursionDepth)
    //{
    //    JavaScriptSerializer serializer = new JavaScriptSerializer();
    //    serializer.RecursionLimit = recursionDepth;
    //    return serializer.Serialize(obj);
    //}

    #endregion

    #region GetDLS_strSQL_ForCompID

    private static string GetDLS_strSQL_ForCompID(string CompName)
    {
        string sql = string.Concat("select CompID from tbl_Company where Carrier = 1 AND CompName='", CompName, "'");
        HelperFuncs.writeToDispatchLogs("carNameTest RRD", sql);
        return sql;
    }

    #endregion

    #region GetDLS_CarrierCompanyID

    public static string GetDLS_CarrierCompanyID(ref string strCarrierName)
    {
        string strSQL = "";

        strCarrierName = strCarrierName.Replace(" Guaranteed by 5PM", "");

        HelperFuncs.writeToDispatchLogs("RRD carrier LIVE", strCarrierName);

        switch (strCarrierName.Replace("<span style='color:Green;'></span>", "").Replace("%2C", ","))
        {
            case "Crosscountry Courier Inc RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Cross Country Courier");
                    break;
                }
            case "Con-Way RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Con-Way Transportation Services");
                    break;
                }
            case "XPO Logistics Freight, Inc. RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Con-Way Transportation Services");
                    break;
                }
            case "Fedex LTL Priority RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("FEDEX LTL PRIORITY");
                    break;
                }
            case "FedEx Freight (R) Priority RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("FEDEX LTL PRIORITY");
                    break;
                }
            case "Fedex LTL Economy RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("FEDEX LTL ECONOMY");
                    break;
                }
            case "FedEx Freight (R) Economy RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("FEDEX LTL ECONOMY");
                    break;
                }
            case "Central Transport RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("CENTRAL TRANSPORT");
                    break;
                }
            case "YRC Freight RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("YRC");
                    break;
                }
            case "UPS Freight RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("UPS FREIGHT");
                    break;
                }
            case "SAIA RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Saia Motor Freight");
                    break;
                }
            case "Estes Express Lines RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("ESTES EXPRESS LINES");
                    break;
                }
            case "R & L Carriers RRD":
                {
                    //strSQL = GetDLS_strSQL_ForCompID("R+L Carriers, inc.");
                    strSQL = string.Concat("select CompID from tbl_Company where Carrier=1 AND CompID=158276");
                    break;
                }
            case "R $ L Carriers RRD": //R $ L Carriers RRD
                {
                    //strSQL = GetDLS_strSQL_ForCompID("R+L Carriers, inc.");//
                    strSQL = string.Concat("select CompID from tbl_Company where Carrier=1 AND CompID=158276");
                    break;
                }
            case "N & M Transfer Co., Inc. RRD":
                {
                    strSQL = string.Concat("select CompID from tbl_Company where Carrier=1 AND CompID=40554");
                    break;
                }
            case "ROADRUNNER TRANSPORTATION SERVICES INC RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("ROADRUNNER TRANSPORTATION - RRTS");
                    break;
                }
            case "New Penn RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("NEW PENN");
                    break;
                }
            case "Clear Lane Freight Systems LLC RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("CLEAR LANE FREIGHT SYSTEMS");
                    break;
                }
            case "Pitt Ohio Express RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("PITT OHIO EXPRESS");
                    break;
                }
            case "Dayton Freight Lines Inc RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("DAYTON FREIGHT LINES");
                    break;
                }
            case "AAA Cooper RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("AAA COOPER");
                    break;
                }
            case "AAA Cooper Transportation RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("AAA COOPER");
                    break;
                }
            case "A&B Freight Line RRD":
                {
                    //strSQL = GetDLS_strSQL_ForCompID("Saia Motor Freight");
                    break;
                }
            case "ABERDEEN EXPRESS INC. RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("ABERDEEN EXPRESS");
                    break;
                }
            case "ABERDEEN EXPRESS INC RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("ABERDEEN EXPRESS");
                    break;
                }
            case "Averitt Express Express RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Averitt Express");
                    break;
                }
            case "Benjamin Best Freight Inc RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Benjamin Best freight");
                    break;
                }
            case "Beaver Express RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("BEAVER EXPRESS");
                    break;
                }
            case "Benton Express RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("BENTON EXPRESS");
                    break;
                }
            case "Central Freight Lines RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("CENTRAL FREIGHT LINES");
                    break;
                }
            case "Central Transport Intl RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("CENTRAL TRANSPORT");
                    break;
                }
            case "Clear Lane Freight RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("CLEAR LANE FREIGHT SYSTEMS");
                    break;
                }
            case "Dayton Freight RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("DAYTON FREIGHT LINES");
                    break;
                }
            case "DHE RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("DEPENDABLE HIGHWAY EXPRESS");
                    break;
                }
            case "Dependable Highway Express RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("DEPENDABLE HIGHWAY EXPRESS");
                    break;
                }
            case "Dorhn Transfer RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Dohrn Transfer Company");
                    break;
                }
            case "Dohrn Transfer RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Dohrn Transfer Company");
                    break;
                }
            case "Dugan RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Dugan Truck Line");
                    break;
                }
            case "Expedited Freight Systems RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Expedited Freight Systems");
                    break;
                }
            case "Expedited Freight Systems, Llc RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Expedited Freight Systems");
                    break;
                }
            case "Frontline Freight RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Frontline Freight Inc");
                    break;
                }
            case "LME Inc RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("LAKEVILLE MOTOR EXPRESS");
                    break;
                }
            case "lme inc rrd":
                {
                    strSQL = GetDLS_strSQL_ForCompID("LAKEVILLE MOTOR EXPRESS");
                    break;
                }
            case "Lakeville Motor RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("LAKEVILLE MOTOR EXPRESS");
                    break;
                }
            case "Lakeville Motor Express, Inc. RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("LAKEVILLE MOTOR EXPRESS");
                    break;
                }
            case "Land Air Express RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("LAND AIR EXPRESS");
                    break;
                }
            case "Midwest Motor Express RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("MIDWEST MOTOR EXPRESS");
                    break;
                }
            case "New England Motor RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("NEW ENGLAND MOTOR FREIGHT");
                    break;
                }
            case "New England Motor Freight RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("NEW ENGLAND MOTOR FREIGHT");
                    break;
                }
            case "Oak Harbor RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("OAK HARBOR");
                    break;
                }
            case "Southeastern Freight Lines RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("SOUTHEASTERN FREIGHT LINES");
                    break;
                }
            case "Southwestern Motor RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("SOUTHWESTERN MOTOR TRANSPORT");
                    break;
                }
            case "SOUTHWESTERN MOTOR TRANSPORT INC RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("SOUTHWESTERN MOTOR TRANSPORT");
                    break;
                }
            case "Standard Forwarding RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Standard Forwarding Company");
                    break;
                }
            case "STANDARD FORWARDING LLC RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Standard Forwarding Company");
                    break;
                }
            case "USF Holland RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("USF Holland");
                    break;
                }
            case "USF Reddaway RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("USF Reddaway");
                    break;
                }
            case "Valley Cartage Company RRD":
                {
                    //strSQL = GetDLS_strSQL_ForCompID("Saia Motor Freight");
                    break;
                }
            case "Vision Express RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Vision Express");
                    break;
                }
            case "Wrag-Time Trans RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Vision Express");
                    break;
                }
            case "Vision Express/Wrag-Time Trans RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Vision Express");
                    break;
                }
            case "Vision Express/Wrag-TIme Transportation RRD": //Vision Express/Wrag-TIme Transportation RRD
                {
                    strSQL = GetDLS_strSQL_ForCompID("Vision Express");
                    break;
                }
            case "Wilson Trucking Company RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("WILSON TRUCKING");
                    break;
                }
            case "Forward Air RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("FORWARD AIR");
                    break;
                }
            case "FORWARD AIR, INC. RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("FORWARD AIR");
                    break;
                }
            //case "FORWARD AIR%2C INC. RRD":
            //    {
            //        strSQL = GetDLS_strSQL_ForCompID("FORWARD AIR");
            //        break;
            //    }
            case "Tax Airfreight, Inc. RRD":
                {
                    strSQL = GetDLS_strSQL_ForCompID("Tax Airfreight Inc");
                    break;
                }
            default:
                {
                    HelperFuncs.writeToDispatchLogs("RRD carrier not caught LIVE", strCarrierName);
                    //HelperFuncs.writeToSiteErrors("RRD carrier not caught", strCarrierName);
                    break;
                }
        }

        return strSQL;
    }

    #endregion

    #region GetDestCompInfoByCompID

    public static void GetCompInfoByUserName(string UserName, ref HelperFuncs.CompInfo info)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                #region SQL

                string sql = string.Concat("SELECT City, State, Zip, Phone, Addr1, Addr2, CompName, Contact, EMail, Fax ",

                    "FROM tbl_COMPANY ",

                    "WHERE AccountNbr='", UserName, "'");

                #endregion

                byte counter = 0;

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["City"] != DBNull.Value)
                            {
                                info.City = reader["City"].ToString();
                            }

                            if (reader["State"] != DBNull.Value)
                            {
                                info.State = reader["State"].ToString();
                            }

                            if (reader["Zip"] != DBNull.Value)
                            {
                                info.Zip = reader["Zip"].ToString();
                            }

                            if (reader["CompName"] != DBNull.Value)
                            {
                                info.CompName = reader["CompName"].ToString();
                            }

                            if (reader["Contact"] != DBNull.Value)
                            {
                                info.Contact = reader["Contact"].ToString();
                            }

                            if (reader["Phone"] != DBNull.Value)
                            {
                                info.Phone = reader["Phone"].ToString();
                            }

                            if (reader["Addr1"] != DBNull.Value)
                            {
                                info.Addr1 = reader["Addr1"].ToString();
                            }

                            if (reader["Addr2"] != DBNull.Value)
                            {
                                info.Addr2 = reader["Addr2"].ToString();
                            }

                            if (reader["EMail"] != DBNull.Value)
                            {
                                info.EMail = reader["EMail"].ToString();
                            }

                            if (reader["Fax"] != DBNull.Value)
                            {
                                info.Fax = reader["Fax"].ToString();
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("poSystemFuncs", e.ToString());
        }
    }

    #endregion

    #region GetRRD_ScacByCarrierName

    public static void GetRRD_ScacByCarrierName(string CarrierName, ref string SCAC)
    {
        string sql = string.Empty;

        try
        {
            if (CarrierName.ToLower().Contains("dayton"))
            {
                sql = string.Concat("SELECT SCAC ",

                    "FROM SQL_LTLCARRIERS ",

                    "WHERE ID=348");
            }
            else
            {
                sql = string.Concat("SELECT SCAC ",

                    "FROM SQL_LTLCARRIERS ",

                    "WHERE CarrierName='", CarrierName, "'");
            }


            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringLTLRater))
            {
                #region SQL

                //sql = string.Concat("SELECT SCAC ",

                //    "FROM SQL_LTLCARRIERS ",

                //    "WHERE CarrierName='", CarrierName, "'");

                #endregion

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["SCAC"] != DBNull.Value)
                            {
                                SCAC = reader["SCAC"].ToString();
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("GetRRD_ScacByCarrierName", e.ToString());
        }
    }

    #endregion
    
}