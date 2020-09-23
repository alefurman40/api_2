#region Using

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Xml;
using System.Net.Http.Formatting;
//using gcmAPI.gcmWebService;

#endregion

/// <summary>
/// Summary description for SharedRail
/// </summary>
/// 
// Live

public class SharedRail
{
    #region Variables

    public interface ICarrier
    {
        void login();
        IntermodalRater.railResult getRate();
        void scrapeResult(ref string doc);
    }

    public struct Location
    {
        public string zipCode, locationId, city, state, country, latitude, longitude, area, timeZone;
    }

    public struct Result
    {
        public DateTime pickupDate;
        public double totalPrice;
        public string quoteLineTransactionId;
        public bool canBook;
    }

    public struct Parameters
    {
        public string username, password, origZip, destZip, origCity, destCity;
        public bool isHazMat;
        public DateTime puDate;
    }

    public delegate string RaterDelegate(ref FormDataCollection form);

    public static string[] streamlineResultArray;
    public static string[] csxiResultArray;
    public static string[] integraResultArray;
    public static string[] integraResultArray40FT;
    public static string[] integraResultArray45FT;

    public static string[] modalXResultArray;
    public static string[] mgOtrResultArray;
    public static MG_OTR_Result mgOtrRes;
    public static string[] synchronet20FT_DryResultArray;
    public static string[] synchronet40FT_High_CubeResultArray;
    public static string[] synchronet45FT_High_CubeResultArray;

    public static List<IntermodalResult> streamlineResults;

    //public static List<IntermodalResult> allResults;


    #region String constants

    public static string synchronetContainer20FT_Dry = "20FT%20Dry";
    public static string synchronetContainer40FT_High_Cube = "40FT%20High%20Cube";
    public static string synchronetContainer45FT_High_Cube = "45FT%20High%20Cube";
    public static string Synchronet = "Synchronet";
    public static string Streamline = "Streamline";
    public static readonly string CSXI = "CSXI";
    public static readonly string Integra = "Integra";
    public static string success = "success";
    //public static readonly string FiftyThreeFt = "FiftyThreeFt";
    
    // Container names for db
    public static readonly string FourtyFt = "FourtyFt";
    public static readonly string FourtyFiveFt = "FourtyFiveFt";
    public static readonly string TwentyFtDry = "TwentyFtDry";
    public static readonly string FourtyFtHighCube = "FourtyFtHighCube";
    public static readonly string FourtyFiveFtHighCube = "FourtyFiveFtHighCube";
    public static readonly string FiftyThreeFt = "FiftyThreeFt";

    #endregion

    // For quotes
    private static int resultID = 0;

    #region Structs

    public struct RailResult
    {
        public string notification, quote;
        public double rate;
        public int transitTime;
        public DateTime quoteExpiration;
    }

    public struct IntegraResult
    {
        public string notification, quote;
        public double rate;
        public int transitTime;
        public DateTime quoteExpiration;
    }

    public struct ModalX_Lane
    {
        public string id, rateRequestId, lane, origAreaId, origAreaZip, destAreaId, destAreaZip;
        public ModalX_Date modalX_Date;
        //ModalX_Area origArea;
        //ModalX_Area destArea;
    }

    public struct ModalX_Date
    {
        public string id, quote, status, date, day, laneProvider;
        public bool isAvailable, isBestLane;
    }

    public struct ModalX_PurchasedBy
    {
        public string email, phone, name, company;
    }

    public struct MG_OTR_Result
    {
        public string notification, confidenceGroup;
        public decimal rate, confidenceRate;
        //public int transitTime;
        //public DateTime quoteExpiration;

    }

    #endregion

    #endregion

    #region HTML Building functions
    
    #region AppendTableCells
    
    public static void AppendTableCells(ref string testStrCarrierName, ref List<string[]> results, ref int indOfMinRateOrTransit, ref StringBuilder dataRows,
        ref string vThru, ref byte resultNumber, ref string minRateOrTransitCompCode, ref string originZip, ref string destZip, ref string origCity,
        ref string destCity, ref string origState, ref string destState)
    {
        bool gotSynchronetRate = false;
        if (SharedRail.synchronet20FT_DryResultArray[0].Equals(SharedRail.success) || SharedRail.synchronet40FT_High_CubeResultArray[0].Equals(SharedRail.success) ||
            SharedRail.synchronet45FT_High_CubeResultArray[0].Equals(SharedRail.success))
        {
            gotSynchronetRate = true;
        }

        string rateButtonFirstPart = string.Concat("<td style=\"text-align: center;\">", "<button id=\"requestBookingBtn\" type=\"button\" ",
                    "class=\"btn btn-primary rateButtons\" onClick=\"parent.location='mailto:intermodal", AppCodeConstants.email_domain, "?subject=Request Booking ",
                    originZip, "-", destZip, " quoted ");


        string RowStyle = "style=\"background-color: #E8E8E8;\"";

        if (resultNumber.Equals(1))
        {
            RowStyle = "style=\"background-color: white;\"";
        }
        dataRows.Append(string.Concat("<table class=\"table table-bordered table-condensed\">", testStrCarrierName,

                "<thead><tr><th>Container Size</th><th style=\"text-align: center;\">20'</th><th style=\"text-align: center;\">40'HC</th>",
                "<th style=\"text-align: center;\">45'</th><th style=\"text-align: center;\">53'</th></tr></thead>"));
        //

        #region Add Cost
        // Add cost
        dataRows.Append("<tr " + RowStyle + ">");
        dataRows.Append("<td>Click to request booking</td>");

        #region Add 20 Dry, 40', 45' containers rates (Synchronet only)
        // For Synchronet
        if (resultNumber.Equals(1) && gotSynchronetRate)
        {
            string synchronetCompCode = "(6)";
            SynchronetAddContainersInfoForCost(ref dataRows, ref rateButtonFirstPart, ref synchronetCompCode, 2); // 2 is Index of data (cost)

        }
        else // Not Synchronet
        {
            dataRows.Append("<td></td><td></td><td></td>");
        }
        #endregion

        #region Add the 53' container rate (Not Synchronet)

        dataRows.Append(string.Concat(rateButtonFirstPart, results[indOfMinRateOrTransit][2], " ", minRateOrTransitCompCode, "'\">", results[indOfMinRateOrTransit][2],
            "</button>", "</td>"));

        #endregion

        dataRows.Append("</tr>");
        #endregion

        //

        #region Add ETA
        string dataType = "eta";
        dataRows.Append("<tr " + RowStyle + ">");
        dataRows.Append("<td>Estimated Transit</td>");

        if (resultNumber.Equals(1) && gotSynchronetRate) // For Synchronet
        {
            double miles = -1;
            HelperFuncs.UseGoogleAPI_ToGetMilesBetweenCities(ref origCity, ref origState, ref destCity, ref destState, ref miles);

            if (miles.Equals(-1)) // Error getting miles
            {
                dataRows.Append("<td></td><td></td><td></td><td></td>");
            }
            else // Got miles
            {
                // Calculate transit time
                int etaFromMiles = (int)Math.Ceiling(miles / 450); // Round the double up to nearest int

                SynchronetAddContainersInfo(ref dataRows, string.Concat("<td style=\"text-align: center;\">", etaFromMiles, "</td>"), ref dataType, etaFromMiles);

            }
        }
        else // All other carriers
        {
            dataRows.Append("<td></td><td></td><td></td>");

        }

        dataRows.Append(string.Concat("<td style=\"text-align: center;\">", results[indOfMinRateOrTransit][3], "</td>"));

        dataRows.Append("</tr>");

        #endregion

        //

        #region Add Rate Expiration

        dataType = "expiration";
        dataRows.Append("<tr " + RowStyle + ">");
        dataRows.Append("<td>Rate Expiration</td>");

        if (resultNumber.Equals(1) && gotSynchronetRate) // For Synchronet
        {

            SynchronetAddContainersInfo(ref dataRows, string.Concat("<td style=\"text-align: center;\">", vThru, "</td>"), ref dataType, -1);

        }
        else // All other carriers
        {
            dataRows.Append("<td></td><td></td><td></td>");

        }

        //

        dataRows.Append("<td style=\"text-align: center;\">" + vThru + "</td>"); // Valid through

        //if (results[indOfMinRateOrTransit][1].Contains("Integra")) // In case of Integra
        //{
        //    dataRows.Append(string.Concat("<td style=\"text-align: center;\">", results[indOfMinRateOrTransit][4], "</td>")); // Pickup date
        //}
        //else
        //{
        //    dataRows.Append("<td style=\"text-align: center;\">" + vThru + "</td>"); // Valid through
        //}


        dataRows.Append("</tr>");

        #endregion

        //

        #region Add "Door to Door"

        dataType = "service";
        dataRows.Append("<tr " + RowStyle + ">");
        dataRows.Append("<td>Service</td>");

        if (resultNumber.Equals(1) && gotSynchronetRate) // For Synchronet
        {
            SynchronetAddContainersInfo(ref dataRows, string.Concat("<td style=\"text-align: center;\">Door to Door</td>", ""), ref dataType, -1);

        }
        else // All other carriers
        {
            dataRows.Append("<td></td><td></td><td></td>");

        }

        dataRows.Append("<td style=\"text-align: center;\">Door to Door</td>");

        dataRows.Append("</tr>");

        #endregion

        dataRows.Append("</table>");

    }
   
    #endregion

    #region SynchronetAddContainersInfo
    
    private static void SynchronetAddContainersInfo(ref StringBuilder dataRows, string htmlToAdd, ref string dataType, int etaFromMiles)
    {
        #region For Synchronet
        if (SharedRail.synchronet20FT_DryResultArray[0].Equals(SharedRail.success))
        {
            if (dataType.Equals("eta"))
            {
                // Add 3 extra days for 20', example 5-8 days
                dataRows.Append(htmlToAdd.Replace(etaFromMiles.ToString(), string.Concat(etaFromMiles.ToString(), "-", (etaFromMiles + 3).ToString())));
            }
            else
            {
                dataRows.Append(htmlToAdd);
            }
        }
        else
        {
            dataRows.Append("<td></td>");
        }

        //

        if (SharedRail.synchronet40FT_High_CubeResultArray[0].Equals(SharedRail.success))
        {
            dataRows.Append(htmlToAdd);
        }
        else
        {
            dataRows.Append("<td></td>");
        }

        //

        if (SharedRail.synchronet45FT_High_CubeResultArray[0].Equals(SharedRail.success))
        {
            dataRows.Append(htmlToAdd);
        }
        else
        {
            dataRows.Append("<td></td>");
        }
        //dataRows.Append("<td></td>");
        #endregion
    }
    
    #endregion

    #region SynchronetAddContainersInfoForCost
    
    private static void SynchronetAddContainersInfoForCost(ref StringBuilder dataRows, ref string rateButtonFirstPart, ref string minRateOrTransitCompCode, byte indexOfData)
    {
        #region For Synchronet
        if (SharedRail.synchronet20FT_DryResultArray[0].Equals(SharedRail.success))
        {
            dataRows.Append(string.Concat(rateButtonFirstPart, SharedRail.synchronet20FT_DryResultArray[indexOfData], " ", minRateOrTransitCompCode, "'\">", SharedRail.synchronet20FT_DryResultArray[indexOfData],
                "</button>", "</td>"));
        }
        else
        {
            dataRows.Append("<td></td>");
        }

        //

        if (SharedRail.synchronet40FT_High_CubeResultArray[0].Equals(SharedRail.success))
        {
            dataRows.Append(string.Concat(rateButtonFirstPart, SharedRail.synchronet40FT_High_CubeResultArray[indexOfData], " ", minRateOrTransitCompCode, "'\">", SharedRail.synchronet40FT_High_CubeResultArray[indexOfData],
                "</button>", "</td>"));
        }
        else
        {
            dataRows.Append("<td></td>");
        }

        //

        if (SharedRail.synchronet45FT_High_CubeResultArray[0].Equals(SharedRail.success))
        {
            dataRows.Append(string.Concat(rateButtonFirstPart, SharedRail.synchronet45FT_High_CubeResultArray[indexOfData], " ", minRateOrTransitCompCode, "'\">", SharedRail.synchronet45FT_High_CubeResultArray[indexOfData],
                "</button>", "</td>"));
        }
        else
        {
            dataRows.Append("<td></td>");
        }
        #endregion
    }
    
    #endregion

    #endregion

    #region Carrier functions

    #region Synchronet

    #region getRateFrom_SynchronetViaAlex2015

    //public static void getRateFrom_SynchronetViaAlex2015(string containerType, string origZip, string destZip)
    //{

    //    try
    //    {
    //        //Initialize web service/API object
    //        gcmAPI.gcmWebService.RateService2 rs = new gcmAPI.gcmWebService.RateService2();

    //        #region Gather results

    //        if (containerType.Equals(synchronetContainer20FT_Dry))
    //        {
    //            string[] synchronet20FT_DryResultArrayTemp = rs.getRateFrom_Synchronet(synchronetContainer20FT_Dry, origZip, destZip);
                
    //            //synchronet20FT_DryResultArray = rs.getRateFrom_Synchronet(synchronetContainer20FT_Dry, origZip, destZip);
    //            synchronet20FT_DryResultArray[0] = synchronet20FT_DryResultArrayTemp[0];
    //            synchronet20FT_DryResultArray[1] = synchronet20FT_DryResultArrayTemp[1];
    //            synchronet20FT_DryResultArray[2] = synchronet20FT_DryResultArrayTemp[2];
    //            synchronet20FT_DryResultArray[3] = synchronet20FT_DryResultArrayTemp[3];
    //            synchronet20FT_DryResultArray[4] = synchronet20FT_DryResultArrayTemp[4];
    //            synchronet20FT_DryResultArray[5] = synchronet20FT_DryResultArrayTemp[5];
    //            synchronet20FT_DryResultArray[11] = TwentyFtDry;
    //        }
    //        else if (containerType.Equals(synchronetContainer40FT_High_Cube))
    //        {
    //            string[] synchronet40FT_High_CubeResultArrayTemp = rs.getRateFrom_Synchronet(synchronetContainer40FT_High_Cube, origZip, destZip);

    //            synchronet40FT_High_CubeResultArray[0] = synchronet40FT_High_CubeResultArrayTemp[0];
    //            synchronet40FT_High_CubeResultArray[1] = synchronet40FT_High_CubeResultArrayTemp[1];
    //            synchronet40FT_High_CubeResultArray[2] = synchronet40FT_High_CubeResultArrayTemp[2];
    //            synchronet40FT_High_CubeResultArray[3] = synchronet40FT_High_CubeResultArrayTemp[3];
    //            synchronet40FT_High_CubeResultArray[4] = synchronet40FT_High_CubeResultArrayTemp[4];
    //            synchronet40FT_High_CubeResultArray[5] = synchronet40FT_High_CubeResultArrayTemp[5];
    //            synchronet40FT_High_CubeResultArray[11] = FourtyFtHighCube;
    //            //synchronet40FT_High_CubeResultArray = rs.getRateFrom_Synchronet(synchronetContainer40FT_High_Cube, origZip, destZip);

    //        }
    //        else if (containerType.Equals(synchronetContainer45FT_High_Cube))
    //        {
    //            string[] synchronet45FT_High_CubeResultArrayTemp = rs.getRateFrom_Synchronet(synchronetContainer45FT_High_Cube, origZip, destZip);

    //            synchronet45FT_High_CubeResultArray[0] = synchronet45FT_High_CubeResultArrayTemp[0];
    //            synchronet45FT_High_CubeResultArray[1] = synchronet45FT_High_CubeResultArrayTemp[1];
    //            synchronet45FT_High_CubeResultArray[2] = synchronet45FT_High_CubeResultArrayTemp[2];
    //            synchronet45FT_High_CubeResultArray[3] = synchronet45FT_High_CubeResultArrayTemp[3];
    //            synchronet45FT_High_CubeResultArray[4] = synchronet45FT_High_CubeResultArrayTemp[4];
    //            synchronet45FT_High_CubeResultArray[5] = synchronet45FT_High_CubeResultArrayTemp[5];
    //            synchronet45FT_High_CubeResultArray[11] = FourtyFiveFtHighCube;
    //        }
    //        else
    //        {
    //            throw new Exception("Did not recognize container type");
    //        }

    //        #endregion

    //    }
    //    catch (Exception e)
    //    {
    //        #region Catch code

    //        //HelperFuncs.writeToSiteErrors("Synchronet", e.ToString());
    //        if (!e.Message.Equals("error scraping parameters"))
    //        {
    //            HelperFuncs.writeToSiteErrors("Synchronet", e.ToString());
    //        }

    //        #endregion
    //    }
    //}

    #endregion

    #region loginTo_Synchronet

    public static void loginTo_Synchronet(ref CookieContainer container, string username, string password, ref string[] viewEvent,
        ref string viewstategenerator, ref string[] tokens)
    {

        string url = "", referrer, contentType, accept, method, doc = "", data = "";

        url = "https://webapp.synchronet.co/BoxOnline/LPI_Login.aspx";

        referrer = "";
        contentType = "";
        accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        method = "GET";
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");

        // Scrape viewstate, eventvalidation
        viewEvent = HelperFuncs.getViewAndEvent(doc);

        // Get viewstategenerator           
        viewstategenerator = HelperFuncs.scrapeFromPage(tokens, doc);
        if (viewstategenerator.Equals("not found")) viewstategenerator = "";
        //--------------------------------------------------------------------------------------------------------------------

        referrer = url;
        contentType = "application/x-www-form-urlencoded; charset=UTF-8";
        method = "POST";
        accept = "*/*";

        //data = string.Concat("RadScriptManager=RadAjaxPanel1Panel%7CSubmit&RadScriptManager_TSM=%3B%3BAjaxControlToolkit%2C%20Version%3D4.5.7.429%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3D28f01b0e84b6d53e%3Aen%3Ab20ddc19-01a3-41dd-b1ec-7855917d2ecf%3Aea597d4b%3Ab25378d2%3BTelerik.Web.UI%2C%20Version%3D2013.2.611.45%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3D121fae78165ba3d4%3Aen%3A9711a23a-6cf5-4e6c-87f5-29e6585b3026%3A16e4e7cd%3Aed16cbdc%3Ab7778d6c%3BAjaxControlToolkit%2C%20Version%3D4.5.7.429%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3D28f01b0e84b6d53e%3Aen%3Ab20ddc19-01a3-41dd-b1ec-7855917d2ecf%3A923aa3cc%3A853c2e0b%3A46f97eb1%3A782b16ab%3A50114f04%3A29340eb0%3A13fbf63f%3A1e088fb%3A52c703eb%3Af9ae838a%3Ad754780e%3Af48dface%3A28a7831e&RadStyleSheetManager_TSSM=%3BTelerik.Web.UI%2C%20Version%3D2013.2.611.45%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3D121fae78165ba3d4%3Aen%3A9711a23a-6cf5-4e6c-87f5-29e6585b3026%3Aaac1aeb7",
        //    "&__EVENTTARGET=Submit&__EVENTARGUMENT=&__VIEWSTATE=", viewEvent[0],
        //    "&__VIEWSTATEGENERATOR=", viewstategenerator, "&__EVENTVALIDATION=", viewEvent[1],
        //    "&userfld=", username, "&userfld_ClientState=",
        //"%7B%22enabled%22%3Atrue%2C%22emptyMessage%22%3A%22%22%2C%22validationText%22%3A%22", username,
        //"%22%2C%22valueAsString%22%3A%22", username, "%22%2C%22lastSetTextBoxValue%22%3A%22", username,
        //"%22%7D&userfld_ValidatorCalloutExtender_ClientState=&txtHidData=1280x720&pwfld=", password,
        //"&pwfld_ClientState=%7B%22enabled%22%3Atrue%2C%22emptyMessage%22%3A%22%22%2C%22validationText%22%3A%22%22%2C%22valueAsString%22%3A%22%22%2C%22lastSetTextBoxValue%22%3A%22%22%7D&pwfld_ValidatorCalloutExtender_ClientState=&__ASYNCPOST=true&RadAJAXControlID=RadAjaxPanel1");

        data = string.Concat("RadScriptManager=RadAjaxPanel1Panel%7CSubmit&RadScriptManager_TSM=%3B%3BAjaxControlToolkit%2C%20Version%3D4.5.7.429%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3D28f01b0e84b6d53e%3Aen%3Ab20ddc19-01a3-41dd-b1ec-7855917d2ecf%3Aea597d4b%3Ab25378d2%3BTelerik.Web.UI%2C%20Version%3D2015.2.826.45%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3D121fae78165ba3d4%3Aen%3Aaeb4b824-3293-4ba4-94af-5c6d48e271a3%3A16e4e7cd%3Aed16cbdc%3Ab7778d6c%3BAjaxControlToolkit%2C%20Version%3D4.5.7.429%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3D28f01b0e84b6d53e%3Aen%3Ab20ddc19-01a3-41dd-b1ec-7855917d2ecf%3A923aa3cc%3A853c2e0b%3A46f97eb1%3A782b16ab%3A50114f04%3A29340eb0%3A13fbf63f%3A1e088fb%3A52c703eb%3Af9ae838a%3Ad754780e%3Af48dface%3A28a7831e&RadStyleSheetManager_TSSM=%3BTelerik.Web.UI%2C%20Version%3D2015.2.826.45%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3D121fae78165ba3d4%3Aen%3Aaeb4b824-3293-4ba4-94af-5c6d48e271a3%3Aaac1aeb7",
            "&__EVENTTARGET=Submit&__EVENTARGUMENT=&__VIEWSTATE=", viewEvent[0],
            "&__VIEWSTATEGENERATOR=", viewstategenerator, "&__EVENTVALIDATION=", viewEvent[1],

            "&userfld=", username, "&userfld_ClientState=",
        "%7B%22enabled%22%3Atrue%2C%22emptyMessage%22%3A%22%22%2C%22validationText%22%3A%22", username,
        "%22%2C%22valueAsString%22%3A%22", username, "%22%2C%22lastSetTextBoxValue%22%3A%22", username,
        "%22%7D&userfld_ValidatorCalloutExtender_ClientState=&txtHidData=1280x720&pwfld=", password,
        "&pwfld_ClientState=%7B%22enabled%22%3Atrue%2C%22emptyMessage%22%3A%22%22%2C%22validationText%22%3A%22dlrates%22%2C%22valueAsString%22%3A%22dlrates%22%2C%22lastSetTextBoxValue%22%3A%22dlrates%22%7D&pwfld_ValidatorCalloutExtender_ClientState=&__ASYNCPOST=true&RadAJAXControlID=RadAjaxPanel1");


        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, data, false, false, "", "");

        //--------------------------------------------------------------------------------------------------------------------

        url = "https://webapp.synchronet.co/BoxOnline/Services/SpotQuote/SpotQuote.aspx";

        contentType = "";
        method = "GET";
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");

        viewEvent = HelperFuncs.getViewAndEvent(doc);

        viewstategenerator = HelperFuncs.scrapeFromPage(tokens, doc);
        if (viewstategenerator.Equals("not found")) viewstategenerator = "";
        //--------------------------------------------------------------------------------------------------------------------

    }

    #endregion

    #region loginTo_Synchronet_Old

    public static void loginTo_Synchronet_Old(ref CookieContainer container, string username, string password, ref string[] viewEvent,
        ref string viewstategenerator, ref string[] tokens)
    {

        string url = "", referrer, contentType, accept, method, doc = "", data = "";

        url = "https://webapp.synchronet.co/BoxOnline/Login.aspx";

        referrer = "";
        contentType = "";
        accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        method = "GET";
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");

        // Scrape viewstate, eventvalidation
        viewEvent = HelperFuncs.getViewAndEvent(doc);

        // Get viewstategenerator           
        viewstategenerator = HelperFuncs.scrapeFromPage(tokens, doc);
        if (viewstategenerator.Equals("not found")) viewstategenerator = "";
        //--------------------------------------------------------------------------------------------------------------------

        referrer = url;
        contentType = "application/x-www-form-urlencoded; charset=UTF-8";
        method = "POST";
        accept = "*/*";

        data = string.Concat("RadScriptManager=RadAjaxPanel1Panel%7CSubmit&RadScriptManager_TSM=%3B%3BAjaxControlToolkit%2C%20Version%3D4.5.7.429%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3D28f01b0e84b6d53e%3Aen%3Ab20ddc19-01a3-41dd-b1ec-7855917d2ecf%3Aea597d4b%3Ab25378d2%3BTelerik.Web.UI%2C%20Version%3D2013.2.611.45%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3D121fae78165ba3d4%3Aen%3A9711a23a-6cf5-4e6c-87f5-29e6585b3026%3A16e4e7cd%3Aed16cbdc%3Ab7778d6c%3BAjaxControlToolkit%2C%20Version%3D4.5.7.429%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3D28f01b0e84b6d53e%3Aen%3Ab20ddc19-01a3-41dd-b1ec-7855917d2ecf%3A923aa3cc%3A853c2e0b%3A46f97eb1%3A782b16ab%3A50114f04%3A29340eb0%3A13fbf63f%3A1e088fb%3A52c703eb%3Af9ae838a%3Ad754780e%3Af48dface%3A28a7831e&RadStyleSheetManager_TSSM=%3BTelerik.Web.UI%2C%20Version%3D2013.2.611.45%2C%20Culture%3Dneutral%2C%20PublicKeyToken%3D121fae78165ba3d4%3Aen%3A9711a23a-6cf5-4e6c-87f5-29e6585b3026%3Aaac1aeb7",
            "&__EVENTTARGET=Submit&__EVENTARGUMENT=&__VIEWSTATE=", viewEvent[0],
            "&__VIEWSTATEGENERATOR=", viewstategenerator, "&__EVENTVALIDATION=", viewEvent[1],
            "&userfld=", username, "&userfld_ClientState=",
        "%7B%22enabled%22%3Atrue%2C%22emptyMessage%22%3A%22%22%2C%22validationText%22%3A%22", username,
        "%22%2C%22valueAsString%22%3A%22", username, "%22%2C%22lastSetTextBoxValue%22%3A%22", username,
        "%22%7D&userfld_ValidatorCalloutExtender_ClientState=&txtHidData=1280x720&pwfld=", password,
        "&pwfld_ClientState=%7B%22enabled%22%3Atrue%2C%22emptyMessage%22%3A%22%22%2C%22validationText%22%3A%22%22%2C%22valueAsString%22%3A%22%22%2C%22lastSetTextBoxValue%22%3A%22%22%7D&pwfld_ValidatorCalloutExtender_ClientState=&__ASYNCPOST=true&RadAJAXControlID=RadAjaxPanel1");

        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, data, false, false, "", "");

        //--------------------------------------------------------------------------------------------------------------------

        url = "https://webapp.synchronet.co/BoxOnline/Services/SpotQuote/SpotQuote.aspx";

        contentType = "";
        method = "GET";
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");

        viewEvent = HelperFuncs.getViewAndEvent(doc);

        viewstategenerator = HelperFuncs.scrapeFromPage(tokens, doc);
        if (viewstategenerator.Equals("not found")) viewstategenerator = "";
        //--------------------------------------------------------------------------------------------------------------------

    }

    #endregion

    #region getLocation_Synchronet

    public static void getLocation_Synchronet(ref CookieContainer container, ref string zipCode, ref string Location, ref string LocationID,
        ref string[] tokens)
    {
        string url, referrer, doc, data;

        url = "https://webapp.synchronet.co/BoxOnline/StackTrainWebServices.asmx/LoadSpotQuoteLocation";
        //url = "https://www.synchronetintermodal.com/BoxOnline/StackTrainWebServices.asmx/LoadSpotQuoteLocation";
        referrer = "https://webapp.synchronet.co/BoxOnline/Services/SpotQuote/SpotQuote.aspx";
        //referrer = "https://www.synchronetintermodal.com/BoxOnline/Services/SpotQuote/SpotQuoteRequest.aspx";

        data = string.Concat("{\"prefixText\":\"", zipCode, "\",\"count\":4}");
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, "application/json; charset=UTF-8", "*/*", "POST", data, false, false, "", "");

        //

        tokens[0] = "First";
        tokens[1] = ":";
        tokens[2] = "\"";
        tokens[3] = "\\";
        Location = HelperFuncs.scrapeFromPage(tokens, doc).Replace(",", "%2C").Replace(" ", "%20");

        tokens[0] = "Second";
        LocationID = HelperFuncs.scrapeFromPage(tokens, doc);

    }

    #endregion

    #endregion

    #region CSXI

    public static void GetCSXIInfo(ref string originZipGlobal, ref string destZipGlobal, ref DateTime puDateGlobal, ref List<string[]> list,
        ref IntermodalRater.railResult csxResultObj)
    {
        List<HelperFuncs.Credentials> crds = new List<HelperFuncs.Credentials>();
        string username = "", password = "";
        crds = HelperFuncs.GetLoginsByCarID(78573);
        username = crds[0].username;
        password = crds[0].password;

        Parameters parameters = new Parameters();
        parameters.username = username;
        parameters.password = password;
        parameters.origZip = originZipGlobal;
        parameters.destZip = destZipGlobal;
        //parameters.origCity = originCity;
        //parameters.destCity = destCity;
        //parameters.isHazMat = chkHazmat;
        parameters.puDate = puDateGlobal;
        ICarrier carrier = new CSXI(parameters);

        csxResultObj = carrier.getRate();
    }

    #region GetCSXIInfoFromAlex2015
    
    //public static void GetCSXIInfoFromAlex2015(ref string originZipGlobal, ref string destZipGlobal, ref DateTime puDateGlobal)
    //{
    //    try
    //    {
    //        //Initialize web service/API object
    //        gcmAPI.gcmWebService.RateService2 rs = new gcmAPI.gcmWebService.RateService2();

    //        #region Not used
    //        /*

    //    //Authenticate to the web service/API
    //    string sessionId = rs.Authenticate(apiUserName, apiKey);

    //    //Initialize SOAP header for authentication
    //    rs.AuthHeaderValue = new gcmWebService2.AuthHeader();

    //    //Set session id to the SOAP header
    //    rs.AuthHeaderValue.SessionID = sessionId;

    //    */

    //        #endregion

    //        #region Set variables

    //        //Initialize pickup date, origin zip code, destination zip code
    //        DateTime pickupDate = puDateGlobal;
    //        string originZip = originZipGlobal;    	//Origin ZIP code      
    //        string destinationZip = destZipGlobal;	//Destination ZIP code

    //        //Set additional services(currently not applicable)
    //        string[] additionalServices = new string[2];
    //        additionalServices[0] = "";
    //        additionalServices[1] = "";

    //        #endregion

    //        //Getting Intermodal rate
    //        gcmAPI.gcmWebService.IntermodalRateReply iReply = rs.GetCSXI_Rate(pickupDate, originZip, destinationZip, additionalServices);

    //        gcmAPI.gcmWebService.IntermodalResult[] iResults = iReply.IntermodalRates;

    //        DateTime dt = puDateGlobal;
    //        if (iResults != null && iResults.Length > 0)
    //        {

    //            csxiResultArray[0] = SharedRail.success;
    //            csxiResultArray[1] = CSXI;

    //            csxiResultArray[3] = iResults[0].EstimatedTransitTime.ToString();

    //            csxiResultArray[2] = iResults[0].TotalCharge.ToString();
    //            //HelperFuncs.writeToSiteErrors("CSXI API", csxiResultArray[2]);

    //            csxiResultArray[4] = dt.ToShortDateString();

    //            Int32 transit;
    //            if (!Int32.TryParse(csxiResultArray[3], out transit))
    //            {
    //                try
    //                {

    //                    HelperFuncs.writeToSiteErrors("CSXI", "could not parse transit " + csxiResultArray[3]);
    //                }
    //                catch { }
    //            }
    //            csxiResultArray[5] = dt.AddDays(transit).ToShortDateString();
    //            csxiResultArray[6] = "";
    //            csxiResultArray[11] = FiftyThreeFt;
    //        }
    //        else
    //        {
    //            throw new Exception("No Capacity for all days, or no rate found");
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        //if (e.Message != "No pricing is available" && e.Message != "No matching City/State" && e.Message != "No Capacity for all days, or no rate found")
    //        //{
    //            //insertIntoRailLogs("CSXI", 90199, "0", "", e.ToString(), 0);
    //            HelperFuncs.writeToSiteErrors("CSXI", e.ToString());
    //        //}
    //    }
    //}
    
    #endregion

    #endregion
   
    #region Integra
    
    public static void GetIntegraInfo(ref string originZipGlobal, ref string destZipGlobal, ref string originCity, ref string destCity, ref string originState, ref string destState,
        ref string originCountry, ref string destCountry, ref DateTime puDateGlobal, ref string containerType)
    {
        IntegraResult res = new IntegraResult();

        #region Country

        string oCountry = originCountry;
        string dCountry = destCountry;

        if (oCountry == "US")
            oCountry = "USA";
        else if (oCountry == "CANADA")
            oCountry = "CAN";

        if (dCountry == "US")
            dCountry = "USA";
        else if (dCountry == "CANADA")
            dCountry = "CAN";

        #endregion

        getIntegraRate(AppCodeConstants.integra_basic_auth, originCity, originState, oCountry, originZipGlobal,
                destCity, destState, dCountry, destZipGlobal, ref res, ref puDateGlobal, ref containerType);

    }

    #region getIntegraRate

    private static void getIntegraRate(string auth, string originCity, string originState, string originCountry, string originZip,
            string destinationCity, string destinationState, string destinationCountry, string destinationZip, ref IntegraResult res,
            ref DateTime puDateGlobal, ref string containerType)
    {
        #region Variables

        CookieContainer container = new CookieContainer();
        string url = "", referrer, contentType, accept, method, doc = "", data = "";
        StringBuilder postData = new StringBuilder();

        #endregion

        try
        {

            #region Log in

            string basicAuth = AppCodeConstants.integra_ratenet_basic_auth;
            //url = "https://ratenet_intlog.gtgs.net/cgi-bin/ver/mainprg.pl";
            url = "https://ratenet-intlog.gtgs.net";
            referrer = "";
            contentType = "";
            accept = "text/html, application/xhtml+xml, image/jxr, */*";
            method = "GET";
            doc = (string)HelperFuncs.generic_http_request_BasicAuth("string", container, url, referrer, contentType, accept, method, data.ToString(),
                false, true, "", basicAuth);

            #endregion

            url = "https://ratenet-intlog.gtgs.net/cgi-bin/ver/mainprg.pl";

            #region Get orig City

            // Get origin city/validate origin zip code
            referrer = url;
            contentType = "application/x-www-form-urlencoded";
            method = "POST";
            accept = "*/*";
            data = string.Concat("rev=cvn5&cgia=lookupcity&f=origval&v=", originZip);

            doc = (string)HelperFuncs.generic_http_request_BasicAuth("string", container, url, referrer, contentType, accept, method, data,
                false, true, "", basicAuth);

            byte resInd = 1;

            string[] tokens = new string[4];
            tokens[0] = string.Concat("id=\"cityval_", resInd);
            tokens[1] = "value=";
            tokens[2] = "\"";
            tokens[3] = "\"";
            string origCity = HelperFuncs.scrapeFromPage(tokens, doc);

            // This fixes the bug on Integra website where it shows first 2 (or so) results as empty strings
            if (origCity.Trim().Equals(string.Empty))
            {
                for (byte i = 2; i < 7; i++)
                {
                    tokens[0] = string.Concat("id=\"cityval_", i);
                    origCity = HelperFuncs.scrapeFromPage(tokens, doc);
                    if (!origCity.Trim().Equals(string.Empty))
                    {
                        resInd = i;
                        break;
                    }
                }
            }
            if (origCity.Trim().Equals(string.Empty) || origCity.Equals("not found"))
            {
                throw new Exception(string.Concat("did not validate zip code ", originZip));
            }
            //

            tokens[0] = string.Concat("id=\"stateval_", resInd);
            string origState = HelperFuncs.scrapeFromPage(tokens, doc);

            tokens[0] = string.Concat("id=\"zipval_", resInd);
            string origZip = HelperFuncs.scrapeFromPage(tokens, doc);

            tokens[0] = string.Concat("id=\"cntryval_", resInd);
            string origCntry = HelperFuncs.scrapeFromPage(tokens, doc);

            #region Make sure origin zip validated

            if (origCity.Equals("not found") || origCity.Trim().Equals(string.Empty))
            {
                HelperFuncs.writeToSiteErrors("integra", string.Concat("did not find city for zip code ", originZip));
                return;
            }
            if (origState.Equals("not found") || origState.Trim().Equals(string.Empty))
            {
                HelperFuncs.writeToSiteErrors("integra", string.Concat("did not find state for zip code ", originZip));
                return;
            }

            if (origZip.Equals("not found") || origZip.Trim().Equals(string.Empty))
            {
                HelperFuncs.writeToSiteErrors("integra", string.Concat("did not find zip for zip code ", originZip));
                return;
            }

            if (origCntry.Equals("not found") || origCntry.Trim().Equals(string.Empty))
            {
                HelperFuncs.writeToSiteErrors("integra", string.Concat("did not find country for zip code ", originZip));
                return;
            }

            #endregion

            #endregion

            #region Get dest City

            data = string.Concat("rev=cvn5&cgia=lookupcity&f=destval&v=", destinationZip);

            doc = (string)HelperFuncs.generic_http_request_BasicAuth("string", container, url, referrer, contentType, accept, method, data,
                false, true, "", basicAuth);

            resInd = 1;

            tokens[0] = string.Concat("id=\"cityval_", resInd);
            string destCity = HelperFuncs.scrapeFromPage(tokens, doc);

            // This fixes the bug on Integra website where it shows first 2 (or so) results as empty strings
            if (destCity.Trim().Equals(string.Empty))
            {
                for (byte i = 2; i < 7; i++)
                {
                    tokens[0] = string.Concat("id=\"cityval_", i);
                    destCity = HelperFuncs.scrapeFromPage(tokens, doc);
                    if (!destCity.Trim().Equals(string.Empty))
                    {
                        resInd = i;
                        break;
                    }
                }
            }
            if (destCity.Trim().Equals(string.Empty) || destCity.Equals("not found"))
            {
                throw new Exception(string.Concat("did not validate zip code ", destinationZip));
            }
            //

            tokens[0] = string.Concat("id=\"stateval_", resInd);
            string destState = HelperFuncs.scrapeFromPage(tokens, doc);

            tokens[0] = string.Concat("id=\"zipval_", resInd);
            string destZip = HelperFuncs.scrapeFromPage(tokens, doc);

            tokens[0] = string.Concat("id=\"cntryval_", resInd);
            string destCntry = HelperFuncs.scrapeFromPage(tokens, doc);

            #region Make sure dest zip validated

            if (destCity.Equals("not found") || destCity.Trim().Equals(string.Empty))
            {
                HelperFuncs.writeToSiteErrors("integra", string.Concat("did not find city for zip code ", destinationZip));
                return;
            }
            if (destState.Equals("not found") || destState.Trim().Equals(string.Empty))
            {
                HelperFuncs.writeToSiteErrors("integra", string.Concat("did not find state for zip code ", destinationZip));
                return;
            }

            if (destZip.Equals("not found") || destZip.Trim().Equals(string.Empty))
            {
                HelperFuncs.writeToSiteErrors("integra", string.Concat("did not find zip for zip code ", destinationZip));
                return;
            }

            if (destCntry.Equals("not found") || destCntry.Trim().Equals(string.Empty))
            {
                HelperFuncs.writeToSiteErrors("integra", string.Concat("did not find country for zip code ", destinationZip));
                return;
            }

            #endregion

            #endregion

            #region Post for rate

            url = string.Concat(url, "?rev=cvn5");
           
            //postData.Append(string.Concat("inp_orig=", origCity, "%2C+", origState, "%2C+", origZip, "%2C+", origCntry,
            //        "&inp_dest=", destCity, "%2C+", destState, "%2C+", destZip, "%2C+", destCntry,

            //        "&sel_drayfuel=17&sel_railfuel=14&sel_milesprg=MileMaker&sel_milestype=Shortest&sel_custquotes=Y",
            //        "&sel_displayrows=10&cbox_mode_1=on&val_mode_1=DOMESTIC&val_mode_2=IMPORT+%2F+EXPORT&cbox_owner_1=on",
            //        "&val_owner_1=RAIL+%2F+STEAMSHIP&val_owner_2=PRIVATE&cbox_type_1=on&val_type_1=CONTAINER&val_type_2=TRAILER",
            //        "&val_type_3=REEFER&cbox_size_1=on&val_size_1=53&cbox_size_2=on&val_size_2=48"));


            //if (containerType.Equals(FiftyThreeFt))
            //{
            //    postData.Append("&val_size_3=45&val_size_4=40&val_size_5=20");
            //}
            //else
            //{
            //    postData.Append("&cbox_size_3=on&val_size_3=45&cbox_size_4=on&val_size_4=40&val_size_5=20");
            //}

            //postData.Append("&filtercol=&filterval=&cgia=mainquery&cgib=formsubmit&cgic=getrates&cgid=remcolfil&rev=cvn5");

            if (containerType.Equals(FiftyThreeFt))
            {
                postData.Append(string.Concat("inp_orig=", origCity, "%2C+", origState, "%2C+", origZip, "%2C+", origCntry,
                    "&inp_dest=", destCity, "%2C+", destState, "%2C+", destZip, "%2C+", destCntry,

                    "&sel_drayfuel=17&sel_railfuel=14&sel_milesprg=MileMaker&sel_milestype=Shortest&sel_custquotes=Y",
                    "&sel_displayrows=10&cbox_mode_1=on&val_mode_1=DOMESTIC&val_mode_2=IMPORT+%2F+EXPORT&cbox_owner_1=on",
                    "&val_owner_1=RAIL+%2F+STEAMSHIP&val_owner_2=PRIVATE&cbox_type_1=on&val_type_1=CONTAINER&val_type_2=TRAILER",
                    "&val_type_3=REEFER&cbox_size_1=on&val_size_1=53&cbox_size_2=on&val_size_2=48"));


                postData.Append("&val_size_3=45&val_size_4=40&val_size_5=20");

                postData.Append("&filtercol=&filterval=&cgia=mainquery&cgib=formsubmit&cgic=getrates&cgid=remcolfil&rev=cvn5");
            }
            else
            {
                //postData.Clear();
                postData.Append(string.Concat("inp_orig=", origCity, "%2C+", origState, "%2C+", origZip, "%2C+", origCntry,
                        "&inp_dest=", destCity, "%2C+", destState, "%2C+", destZip, "%2C+", destCntry,
                    "&sel_drayfuel=17&sel_railfuel=14&sel_milesprg=MileMaker&sel_milestype=Shortest&sel_custquotes=Y&sel_displayrows=10&cbox_mode_1=on&val_mode_1=DOMESTIC&val_mode_2=IMPORT+%2F+EXPORT&cbox_owner_1=on&val_owner_1=RAIL+%2F+STEAMSHIP&val_owner_2=PRIVATE&cbox_type_1=on&val_type_1=CONTAINER&val_type_2=TRAILER&val_type_3=REEFER&val_size_1=53&cbox_size_2=on&val_size_2=48&cbox_size_3=on&val_size_3=45&cbox_size_4=on&val_size_4=40&val_size_5=20&filtercol=&filterval=&cgia=mainquery&cgib=formsubmit&cgic=getrates&cgid=remcolfil&rev=cvn5"));

            }

            doc = (string)HelperFuncs.generic_http_request_BasicAuth("string", container, url, referrer, contentType, accept, method, postData.ToString(),
                false, true, "", basicAuth);

            #endregion

            if (containerType.Equals(FiftyThreeFt))
            {
                scrapeIntegraRateFromResult53(ref tokens, ref doc, ref postData);
            }
            else
            {
                scrapeIntegraRateFromResult40(ref tokens, ref doc, ref postData);
            }
        }
        catch (Exception e)
        {           
            HelperFuncs.writeToSiteErrors("integra", e.ToString());
        }
    }

    #endregion

    #region scrapeIntegraRateFromResult53

    private static void scrapeIntegraRateFromResult53(ref string[] tokens, ref string doc, ref StringBuilder data)
    {
        #region Get third rate from the top

        tokens[0] = "onClick=\"js_mssetfilter('mainprg.pl', 'col_20_3";
        tokens[1] = "col_20_3";
        tokens[2] = ">";
        tokens[3] = "<";
        string costStr = HelperFuncs.scrapeFromPage(tokens, doc);

        HelperFuncs.writeToSiteErrors("integra 53 costStr", costStr);

        decimal cost = 0M;

        if (!decimal.TryParse(costStr, out cost))
        {
            throw new Exception(string.Concat("Integra - cost not parsed to decimal cost was: ", costStr,
                " result was: ", doc,
                " request was: ", data));

            //res.notification = "error";
        }
        else
        {
            //res.notification = SharedRail.success;

            // Fix the buy rate

            decimal percentMarkupSum = cost * 0.05M;
            if (percentMarkupSum < 75M)
            {
                cost += 75M;
            }
            else
            {
                cost += percentMarkupSum;

            }

            HelperFuncs.writeToSiteErrors("integra cost and percent sum ", string.Concat(cost, " ", percentMarkupSum));

            // Set global result

            integraResultArray[0] = SharedRail.success;
            integraResultArray[1] = Integra;
            integraResultArray[2] = string.Format("{0:0.00}", cost);// cost.ToString();           
            integraResultArray[11] = FiftyThreeFt;


            HelperFuncs.writeToSiteErrors("integra cost", integraResultArray[2]);
        }


        #endregion
    }

    #endregion

    #region scrapeIntegraRateFromResult40

    private static void scrapeIntegraRateFromResult40(ref string[] tokens, ref string doc, ref StringBuilder data)
    {
        #region

        //HelperFuncs.writeToSiteErrors("integra 40 data", data.ToString());
        //HelperFuncs.writeToSiteErrors("integra 40 doc", doc);

        #region Get the costs from the results table

        List<string> rawTextRows = new List<string>();
        int ind;

        // Get all tr's
        while (doc.IndexOf("id=\"tr_") != -1)
        {
            ind = doc.IndexOf("id=\"tr_");
            doc = doc.Substring(ind + 1);
            ind = doc.IndexOf("id=\"tr_");

            if (ind != -1)
            {
                rawTextRows.Add(doc.Remove(ind));
            }
            else
            {
                rawTextRows.Add(doc);
            }
        }

        string contSize = "", cost40Str = "", cost45Str = "";
        byte countOfSize40 = 0, countOfSize45 = 0;
        tokens[1] = "<nobr>";
        tokens[2] = ">";
        tokens[3] = "<";

        for (byte i = 0; i < rawTextRows.Count; i++)
        {
            tokens[0] = string.Concat("id=\"td_", i + 1, "_5");

            contSize = HelperFuncs.scrapeFromPage(tokens, rawTextRows[i]);

            if (contSize.Equals("40"))
            {
                countOfSize40++;
                if (countOfSize40.Equals(2))
                {
                    // Get cost                       
                    tokens[0] = string.Concat("id=\"td_", i + 1, "_20");
                    cost40Str = HelperFuncs.scrapeFromPage(tokens, rawTextRows[i]);
                }
            }
            else if (contSize.Equals("45"))
            {
                countOfSize45++;
                if (countOfSize45.Equals(2))
                {
                    tokens[0] = string.Concat("id=\"td_", i + 1, "_20");
                    cost45Str = HelperFuncs.scrapeFromPage(tokens, rawTextRows[i]);
                }
            }

            if (countOfSize40 > 1 && countOfSize45 > 1)
            {
                break;
            }
        }

        #endregion

        HelperFuncs.writeToSiteErrors("integra cost40, cost45", string.Concat(cost40Str, " ", cost45Str));

        //string costStr = "";
        decimal cost40 = 0M, cost45 = 0M, percentMarkupSum = 0M;

        if (decimal.TryParse(cost40Str, out cost40))
        {
            // Fix the buy rate

            cost40 += 250;

            percentMarkupSum = cost40 * 0.05M;
            if (percentMarkupSum < 75M)
            {
                cost40 += 75M;
            }
            else
            {
                cost40 += percentMarkupSum;
            }

            HelperFuncs.writeToSiteErrors("integra cost40 and percent sum ", string.Concat(cost40, " ", percentMarkupSum));

            // Set global result
            integraResultArray40FT[0] = SharedRail.success;
            integraResultArray40FT[1] = Integra;
            integraResultArray40FT[2] = string.Format("{0:0.00}", cost40);
            integraResultArray40FT[11] = FourtyFt;

            //HelperFuncs.writeToSiteErrors("integra cost", integraResultArray[2]);
        }

        if (decimal.TryParse(cost45Str, out cost45))
        {
            // Fix the buy rate

            cost45 += 250;

            percentMarkupSum = cost45 * 0.05M;
            if (percentMarkupSum < 75M)
            {
                cost45 += 75M;
            }
            else
            {
                cost45 += percentMarkupSum;
            }

            HelperFuncs.writeToSiteErrors("integra cost45 and percent sum ", string.Concat(cost45, " ", percentMarkupSum));

            // Set global result
            integraResultArray45FT[0] = SharedRail.success;
            integraResultArray45FT[1] = Integra;
            integraResultArray45FT[2] = string.Format("{0:0.00}", cost45);
            integraResultArray45FT[11] = FourtyFiveFt;

            //HelperFuncs.writeToSiteErrors("integra cost", integraResultArray[2]);
        }

        #region Not used
        //if (!decimal.TryParse(costStr, out cost))
        //{
        //    throw new Exception(string.Concat("Integra - cost not parsed to decimal cost was: ", costStr,
        //        " result was: ", doc,
        //        " request was: ", data));

        //}
        //else
        //{

        //    // Fix the buy rate
        //    decimal percentMarkupSum = cost * 0.05M;
        //    if (percentMarkupSum < 75M)
        //    {
        //        cost += 75M;
        //    }
        //    else
        //    {
        //        cost += percentMarkupSum;

        //    }

        //    HelperFuncs.writeToSiteErrors("integra cost and percent sum ", string.Concat(cost, " ", percentMarkupSum));

        //    // Set global result

        //    //integraResultArray[0] = SharedRail.success;
        //    //integraResultArray[1] = Integra;
        //    //integraResultArray[2] = string.Format("{0:0.00}", cost);// cost.ToString();
        //    //integraResultArray[6] = FiftyThreeFt;

        //    //HelperFuncs.writeToSiteErrors("integra cost", integraResultArray[2]);
        //}
        #endregion

        #endregion
    }

    #endregion

    #endregion

    #region ModalX

    #region GetModalX_Info
    public static void GetModalX_Info(ref string originZipGlobal, ref string destZipGlobal)
    {
        string username = "", password = "";
        CookieContainer container = new CookieContainer();
        //decimal parseDecimal;

        double transitTime = 10;

        username = AppCodeConstants.modalX_un;
        password = AppCodeConstants.modalX_pwd;

        HelperFuncs.ModalX_Result res = new HelperFuncs.ModalX_Result();

        try
        {
            loginTo_ModalX(ref container, ref username, ref password, ref res, ref originZipGlobal, ref destZipGlobal);

            if (res.success == true)
            {
                modalXResultArray[4] = "";

                modalXResultArray[0] = SharedRail.success;
                modalXResultArray[1] = "ModalX";
                modalXResultArray[2] = res.cost.ToString();

                //insertIntoRailLogs("ModalX", 92184, "1", "", "", Convert.ToDouble(res.cost));

                transitTime = (res.deliveryDate - res.pickupDate).TotalDays;
                modalXResultArray[3] = transitTime.ToString();
                modalXResultArray[4] = res.pickupDate.ToShortDateString();
                modalXResultArray[5] = res.deliveryDate.ToShortDateString();

            }
        }
        catch (Exception e)
        {
            if (!e.Message.Contains("400"))
            {
                HelperFuncs.writeToSiteErrors("ModalX", e.ToString(), "");
            }
        }
    }
    #endregion

    #region loginTo_ModalX
    private static void loginTo_ModalX(ref CookieContainer container, ref string username, ref string password, ref HelperFuncs.ModalX_Result res,
        ref string origZip, ref string destZip)
    {
        #region Variables
        DateTime now = DateTime.Now;
        DateTime today = DateTime.Today;
        DateTime tomorrow = DateTime.Today.AddDays(3);

        Int64 timeStampNow = GetTime(ref now);
        Int64 timeStampToday = GetTime(ref today);
        Int64 timeStampTomorrow = GetTime(ref tomorrow);

        string url = "", referrer, contentType, accept, method, doc = "", data = "";
        #endregion

        #region Go to home page
        url = "https://www.modal-x.com/modalx/";
        referrer = "";
        contentType = "";
        accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        method = "GET";
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");

        //--------------------------------------------------------------------------------------------------------------------
        #endregion

        #region 2 requests with username and password
        referrer = url;
        url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/TDIS/users/" + username + "/login?_dc=" + timeStampNow.ToString();

        // Here the request throws a 401 Not Authorized exception, but this is what happens if the ModalX site is used by any Browser as well
        try
        {
            doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");
        }
        catch (Exception e)
        {
            string str = e.ToString();
        }

        //--------------------------------------------------------------------------------------------------------------------

        url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/TDIS/users/" + username + "/login/j_security_check";
        contentType = "application/x-www-form-urlencoded; charset=UTF-8";
        method = "POST";
        data = "j_username=" + username.Replace("@", "%40") + "&j_password=" + password;
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, data, false, false, "", "");
        #endregion

        #region Some required requests
        //--------------------------------------------------------------------------------------------------------------------

        timeStampNow = GetTime(ref now);

        url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/TDIS/users/" + username + "/login?_dc=" + timeStampNow.ToString();
        contentType = "";
        accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        method = "GET";
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");
        // Get user info here
        ModalX_PurchasedBy purchasedBy = new ModalX_PurchasedBy();
        getPurchasedInfo_ModalX(ref purchasedBy, ref doc);
        //--------------------------------------------------------------------------------------------------------------------

        timeStampNow = GetTime(ref now);

        url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/app?_dc=" + timeStampNow.ToString();
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");

        //--------------------------------------------------------------------------------------------------------------------

        timeStampNow = GetTime(ref now);

        url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/app?_dc=" + timeStampNow.ToString();
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");

        //--------------------------------------------------------------------------------------------------------------------

        timeStampNow = GetTime(ref now);

        url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/TDIS/users/" + username + "?_dc=" + timeStampNow.ToString();
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");

        //--------------------------------------------------------------------------------------------------------------------
        #endregion

        #region Not used
        //Cookie cookie = new System.Net.Cookie("modal-x_user", "");
        //container.Add(cookie);
        //Uri uri = new Uri("https://www.modal-x.com/");
        //container.Add(uri, cookie);
        #endregion

        #region This request is the final step of the login process, and goes to landing page with some recent shipments displayed
        //url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/TDIS/requests/search?isSummary=true";
        url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/TDIS/requests/summary/search";
        contentType = "application/json; charset=UTF-8";
        method = "POST";
        data = "{\"minPickup\":" + timeStampToday.ToString() + ",\"maxPickup\":\"\"}";
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, data, false, false, "", "");

        //--------------------------------------------------------------------------------------------------------------------
        #endregion

        //doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, data, false, false, "", "");

        //--------------------------------------------------------------------------------------------------------------------

        RailResult railResult = new RailResult();
        tryDate_ModalX(ref railResult, ref tomorrow, ref timeStampTomorrow, ref referrer, ref container, ref timeStampNow, ref now,
            ref purchasedBy, ref res, ref origZip, ref destZip);



    }
    #endregion

    #region tryDate_ModalX
    private static void tryDate_ModalX(ref RailResult railResult, ref DateTime date, ref Int64 dateStamp, ref string referrer,
        ref CookieContainer container, ref Int64 timeStampNow, ref DateTime now, ref ModalX_PurchasedBy purchasedBy,
        ref HelperFuncs.ModalX_Result res, ref string origZip, ref string destZip)
    {


        string url = "", contentType, accept, method = "POST", doc = "", data = "";

        // This request throws an exception, but.. this is correct. Only there are no rates for these zip codes (on the weekend)
        url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/TDIS/rates/search";
        contentType = "application/json";
        data = "{\"origZip\":\"" + origZip + "\",\"destZip\":\"" + destZip + "\",\"pickupDate\":" + dateStamp.ToString() +
                    ",\"origCountry\":\"USA\",\"destCountry\":\"USA\"}";
        accept = "*/*";
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, data, false, false, "", "");

        ModalX_Lane mXlane = new ModalX_Lane();
        ModalX_Date mXdate = new ModalX_Date();
        getLanes_ModalX(ref mXlane, ref mXdate, ref doc);
        //--------------------------------------------------------------------------------------------------------------------

        timeStampNow = GetTime(ref now);

        url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/TDIS/accessorials?_dc=" + timeStampNow.ToString();
        contentType = "";
        method = "GET";
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");

        //--------------------------------------------------------------------------------------------------------------------

        url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/TDIS/requests/rates/deliverydate";
        contentType = "application/json";
        method = "POST";
        data = "{\"pickupDate\":" + dateStamp.ToString() + ",\"laneIndex\":{\"laneId\":" + mXlane.id +
            ",\"origAreaId\":" + mXlane.origAreaId + ",\"destAreaId\":" + mXlane.destAreaId + "}}";
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, data, false, false, "", "");

        // Scrape delivery date
        #region delivery date
        string[] tokens = new string[3];
        tokens[0] = "payload";
        tokens[1] = "[";
        tokens[2] = "]";

        string deliveryDateStr = HelperFuncs.scrapeFromPage(tokens, doc);
        #endregion


        //--------------------------------------------------------------------------------------------------------------------

        url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/TDIS/requests/rates";



        DateTime scrapedPuDate = getDateFromJsGetTimeValue(Convert.ToInt64(mXlane.modalX_Date.date));
        Int64 scrapedPuDateStamp = GetTime(ref scrapedPuDate);

        //data = "{\"baseRate\":null,\"totalCharge\":null,\"fscPercent\":0,\"miscChargeNote\":\"\",\"railFuelSurcharge\":null" +
        //",\"accessorials\":[{\"charge\":null,\"qualifier\":\"FUEL\"}],\"pickupDate\":" + mXlane.modalX_Date.date +
        //",\"deliveryDate\":" + deliveryDateStr + ",\"laneId\":" + mXlane.id + ",\"laneIndex\":{\"laneId\":\"" + mXlane.id + "\"" +
        //",\"origAreaId\":\"" + mXlane.origAreaId + "\",\"destAreaId\":\"" + mXlane.destAreaId + "\"},\"locations\":[{\"order\":1" +
        //",\"type\":\"ORIG\",\"postalCode\":\"" + origZip + "\",\"country\":\"USA\",\"city\":\"\"" +
        //",\"state\":\"\",\"county\":\"\",\"netTime\":" + scrapedPuDateStamp.ToString() + ",\"nltTime\":" + scrapedPuDateStamp.ToString() + "}" +
        //",{\"order\":1,\"type\":\"DEST\",\"postalCode\":\"" + destZip + "\",\"country\":\"USA\",\"city\"" +
        //":\"\",\"state\":\"\",\"county\":\"\",\"netTime\":null,\"nltTime\":null}],\"hazmat\":false" +
        //",\"scaleLightHeavy\":false,\"status\":null,\"numOfLoads\":1,\"note\":\"\",\"beneficialOwner\":\"\"" +
        //",\"capRequestId\":null,\"reqStatus\":null,\"rateRequestId\":" + mXlane.rateRequestId + ",\"customer\":\"\",\"remainingLoads\":\"\"" +
        //",\"confirmationNumber\":\"\",\"requestDate\":\"\",\"lane\":\"" + mXlane.lane + "\"" +
        //",\"purchasedBy\":\"" + purchasedBy.email + "\",\"purchasedByPhone\":\"" + purchasedBy.phone + "\"" +
        //",\"purchasedByName\":\"" + purchasedBy.name + "\",\"purchasedByCompany\":\"" + purchasedBy.company + "\"}";

        data = string.Concat("{\"baseRate\":null,\"preMarkkupTotal\":null,\"totalCharge\":null,\"fscPercent\":0,\"markupFee\":",
            "{\"percent\":null,\"minAmount\":\"\",\"markupAmount\":null,\"percentAdminOverridden\":false,\"amountAdminOverridden\":false},",
            "\"miscChargeNote\":\"\",\"railFuelSurcharge\":null,\"appliedLineHaul\":null,\"contractor\":null,\"priceAuthority\":null,",
            "\"discountPercentage\":null,\"accessorials\":[{\"charge\":null,\"qualifier\":\"FUEL\"}],\"pickupDate\":", mXlane.modalX_Date.date,
            ",",
            "\"deliveryDate\":", deliveryDateStr, ",\"laneIndex\":{\"laneId\":\"", mXlane.id, "\",\"origAreaId\":\"", mXlane.origAreaId,
            "\",\"destAreaId\":\"",
            mXlane.destAreaId, "\"},",
            "\"equipTypeXref\":\"U53DH\",\"locations\":[{\"order\":1,\"type\":\"ORIG\",\"postalCode\":\"", origZip, "\",\"country\":\"USA\",",
            "\"city\":\"\",\"state\":\"\",\"county\":\"\",\"netTime\":", scrapedPuDateStamp, ",\"nltTime\":", scrapedPuDateStamp, "},",
            "{\"order\":2,\"type\":\"ORIG\",\"postalCode\":\"\",\"country\":\"USA\",\"city\":\"\",\"state\":\"\",\"county\":\"\",",
            "\"netTime\":", scrapedPuDateStamp, ",\"nltTime\":", scrapedPuDateStamp, "},{\"order\":3,\"type\":\"ORIG\",\"postalCode\":\"\",\"country\":\"USA\",",
            "\"city\":\"\",\"state\":\"\",\"county\":\"\",\"netTime\":", scrapedPuDateStamp, ",\"nltTime\":", scrapedPuDateStamp, "},",
            "{\"order\":1,\"type\":\"DEST\",\"postalCode\":\"\",\"country\":\"USA\",\"city\":\"\",\"state\":\"\",",
            "\"county\":\"\",\"netTime\":null,\"nltTime\":null},{\"order\":2,\"type\":\"DEST\",\"postalCode\":\"\",\"country\":\"USA\",",
            "\"city\":\"\",\"state\":\"\",\"county\":\"\",\"netTime\":null,\"nltTime\":null},{\"order\":3,\"type\":\"DEST\",",
            "\"postalCode\":\"", destZip, "\",\"country\":\"USA\",\"city\":\"\",\"state\":\"\",\"county\":\"\",\"netTime\":null,\"nltTime\":null}],",
            "\"hazmat\":false,\"scaleLightHeavy\":false,\"status\":null,\"numOfLoads\":1,\"note\":\"\",\"beneficialOwner\":\"\",",
            "\"capRequestId\":null,\"lineHaul\":null,\"reqStatus\":null,\"rateRequestId\":", mXlane.rateRequestId, ",\"customer\":\"\",\"thirdParty\":\"\",",
            "\"remainingLoads\":\"\",\"confirmationNumber\":\"\",\"requestDate\":\"\",\"lane\":\"", mXlane.lane, "\",\"purchasedBy\":",
            "\"", purchasedBy.email, "\",\"purchasedByPhone\":\"", purchasedBy.phone, "\",\"purchasedByName\":\"", purchasedBy.name, "\",",
            "\"purchasedByCompany\":\"", purchasedBy.company, "\"}");

        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, data, false, false, "", "");


        getResult_ModalX(ref doc, ref res);

    }
    #endregion

    #region getLanes_ModalX

    private static void getLanes_ModalX(ref ModalX_Lane mXlane, ref ModalX_Date mXdate, ref string doc)
    {

        string tmp;
        int ind;
        DateTime today = DateTime.Today;
        DateTime availDate;
        long jsDateStamp;

        ind = doc.IndexOf("rateRequestId");
        doc = doc.Substring(ind);
        ind = doc.IndexOf(":");
        doc = doc.Substring(ind + 1);
        ind = doc.IndexOf(",");
        mXlane.rateRequestId = doc.Remove(ind).Replace("\"", "");

        while (doc.IndexOf("laneId") != -1)
        {
            ind = doc.IndexOf("laneId");
            doc = doc.Substring(ind);
            ind = doc.IndexOf(":");
            doc = doc.Substring(ind + 1);
            ind = doc.IndexOf(",");
            mXlane.id = doc.Remove(ind);

            // Only scrape the "bestLane"
            ind = doc.IndexOf("bestLane");
            tmp = doc.Substring(ind);
            ind = tmp.IndexOf(":");

            tmp = tmp.Substring(ind + 1);
            ind = tmp.IndexOf(",");
            tmp = tmp.Remove(ind);

            if (tmp.Equals("true")) // If best lane
            {
                ind = doc.IndexOf("origAreaId");
                doc = doc.Remove(ind + 20);

                ind = doc.IndexOf("origArea");
                doc = doc.Substring(ind);
                ind = doc.IndexOf("id");
                doc = doc.Substring(ind);
                ind = doc.IndexOf(":");
                doc = doc.Substring(ind + 1);
                ind = doc.IndexOf(",");
                mXlane.origAreaId = doc.Remove(ind);

                ind = doc.IndexOf("zip");
                doc = doc.Substring(ind);
                ind = doc.IndexOf(":");
                doc = doc.Substring(ind + 1);
                ind = doc.IndexOf(",");
                mXlane.origAreaZip = doc.Remove(ind).Replace("\"", "");

                ind = doc.IndexOf("destArea");
                doc = doc.Substring(ind);
                ind = doc.IndexOf("id");
                doc = doc.Substring(ind);
                ind = doc.IndexOf(":");
                doc = doc.Substring(ind + 1);
                ind = doc.IndexOf(",");
                mXlane.destAreaId = doc.Remove(ind);

                ind = doc.IndexOf("zip");
                doc = doc.Substring(ind);
                ind = doc.IndexOf(":");
                doc = doc.Substring(ind + 1);
                ind = doc.IndexOf(",");
                mXlane.destAreaZip = doc.Remove(ind).Replace("\"", "");

                while (doc.IndexOf("status\":\"AVAILABLE") != -1)
                {
                    ind = doc.IndexOf("status\":\"AVAILABLE");
                    doc = doc.Substring(ind);
                    ind = doc.IndexOf("date");
                    doc = doc.Substring(ind);
                    ind = doc.IndexOf(":");
                    doc = doc.Substring(ind + 1);
                    ind = doc.IndexOf(",");
                    mXdate.date = doc.Remove(ind);

                    if (!long.TryParse(mXdate.date, out jsDateStamp))
                        throw new Exception("ModalX Date not parsed");
                    availDate = getDateFromJsGetTimeValue(jsDateStamp);

                    // Make the time of the dates equal for the comparison
                    TimeSpan ts = new TimeSpan(13, 30, 0);
                    today = today.Date + ts;
                    availDate = availDate.Date + ts;

                    int result = DateTime.Compare(today, availDate);
                    if (result >= 0) // Today's date or earlier is too early, get at least tomorrow pickup date
                    {
                        continue;
                    }

                    ind = doc.IndexOf("LaneProvider");
                    doc = doc.Substring(ind);
                    ind = doc.IndexOf(",");
                    doc = doc.Substring(ind + 1);
                    ind = doc.IndexOf("]");
                    mXdate.laneProvider = doc.Remove(ind).Replace("\"", "");
                    mXlane.modalX_Date = mXdate;

                    if (result < 0)
                        break;
                }

                //ind = doc.IndexOf("\"lane\"");
                //doc = doc.Substring(ind);
                //ind = doc.IndexOf(":");
                //doc = doc.Substring(ind + 1);
                //ind = doc.IndexOf(",");
                //mXlane.lane = doc.Remove(ind).Replace("\"", "");

                //while (doc.IndexOf("status\":\"AVAILABLE") != -1)
                //{
                //    ind = doc.IndexOf("status\":\"AVAILABLE");
                //    doc = doc.Substring(ind);
                //}

            }
            else
            {
                continue;
            }
        }

    }

    #endregion
   
    #region getPurchasedInfo_ModalX
    private static void getPurchasedInfo_ModalX(ref ModalX_PurchasedBy purchasedBy, ref string doc)
    {

        string[] tokens = new string[4];
        tokens[0] = "email";
        tokens[1] = ":";
        tokens[2] = "\"";
        tokens[3] = "\"";
        purchasedBy.email = HelperFuncs.scrapeFromPage(tokens, doc);

        tokens[0] = "company";
        purchasedBy.company = HelperFuncs.scrapeFromPage(tokens, doc);

        tokens[0] = "telephone";
        purchasedBy.phone = HelperFuncs.scrapeFromPage(tokens, doc);

        tokens[0] = "fullName";
        purchasedBy.name = HelperFuncs.scrapeFromPage(tokens, doc);

    }
    #endregion

    #region getResult_ModalX
    private static void getResult_ModalX(ref string doc, ref HelperFuncs.ModalX_Result res)
    {
        string[] tokens = new string[3];
        tokens[0] = "success";
        tokens[1] = ":";
        tokens[2] = ",";

        bool.TryParse(HelperFuncs.scrapeFromPage(tokens, doc), out res.success);
        if (res.success != true)
            return;

        tokens[0] = "pickupDate";
        string tmp = HelperFuncs.scrapeFromPage(tokens, doc);
        long jsDateTime;
        long.TryParse(tmp, out jsDateTime);

        if (jsDateTime < 1)
        {
            return;
        }
        else
        {
            res.pickupDate = getDateFromJsGetTimeValue(jsDateTime);
        }

        //--

        tokens[0] = "deliveryDate";
        tmp = HelperFuncs.scrapeFromPage(tokens, doc);
        long.TryParse(tmp, out jsDateTime);

        if (jsDateTime < 1)
        {
            return;
        }
        else
        {
            res.deliveryDate = getDateFromJsGetTimeValue(jsDateTime);
        }

        //--

        tokens[0] = "totalCharge";
        tmp = HelperFuncs.scrapeFromPage(tokens, doc);
        decimal.TryParse(tmp, out res.cost);

    }
    #endregion

    #endregion

    #region getOTR_RateDLS

    // getOTR_RateDLS
    public static void getOTR_RateDLS(ref string originZip, ref string destZip, ref DateTime puDate,
        ref string originCity, ref string originState, ref string originCountry, ref string destCity, ref string destState, ref string destCountry)
    {

        #region Dates

        //DateTime today = DateTime.Today;
        string month = puDate.Month.ToString();
        string day = puDate.Day.ToString();

        if (month.Length.Equals(1))
        {
            month = string.Concat("0", month);
        }

        if (day.Length.Equals(1))
        {
            day = string.Concat("0", day);
        }

        //--

        #endregion

        //#region Variables

        //string url = "", referrer, contentType, accept, method, doc = "";
        //CookieContainer container = new CookieContainer();

        //url = string.Concat("http://elleyment.azurewebsites.net/Api/Quote?OriginZipCode=", originZip,
        //    "&DestinationZipCode=", destZip, "&PickUpDate=", month, "/", day, "/", puDate.Year);

        //referrer = "";
        //contentType = "application/xml";
        //method = "GET";
        //accept = "text/xml";

        //#endregion

        //doc = (string)HelperFuncs.generic_http_request("string", null, url, referrer, contentType, accept, method, "", false);

        #region Variables

        string url = "", doc = "";

        url = string.Concat("http://elleyment-dev.rrd.com/Api/Quote?OriginZipCode=", originZip,
            "&DestinationZipCode=", destZip, "&PickUpDate=", month, "/", day, "/", puDate.Year, "&ClientToken=",
            AppCodeConstants.dls_otr_rrd_token
            );

        //HelperFuncs.writeToSiteErrors("Truckstop url", url);

        #endregion

        doc = (string)HelperFuncs.generic_http_request("string", null, url, "", "application/xml", "text/xml", "GET", "", false);
        
        //HelperFuncs.writeToSiteErrors("MG OTR", doc);
        
        #region Get Rate

        // Gather results into an object
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(doc);

        XmlNodeList nodeList = xmlDoc.GetElementsByTagName("PredictedRate");

        string PredictedRate = string.Empty;

        if (nodeList != null && nodeList.Count > 0)
        {
            PredictedRate = nodeList[0].InnerText;
        }

        string ConfidenceRate = string.Empty;
        nodeList = xmlDoc.GetElementsByTagName("Confidence");

        if (nodeList != null && nodeList.Count > 0)
        {
            ConfidenceRate = nodeList[0].InnerText;
        }

        mgOtrRes.confidenceGroup = string.Empty;
        nodeList = xmlDoc.GetElementsByTagName("ConfidenceGroup");

        if (nodeList != null && nodeList.Count > 0)
        {
            mgOtrRes.confidenceGroup = nodeList[0].InnerText;
        }


        #endregion

        //MG_OTR_Result mgOtrRes = new MG_OTR_Result();
        mgOtrRes.rate = 0M;
        mgOtrRes.confidenceRate = 0M;

        if (decimal.TryParse(PredictedRate, out mgOtrRes.rate))
        {
            if (mgOtrRes.rate.Equals(2208.88M))
            {
                mgOtrRes.rate = 0M;
                //mgOtrRes.confidenceRate = 0M;
            }
            else
            {
                mgOtrResultArray[0] = SharedRail.success;
                //mgOtrResultArray[1] = "MG OTR";
                mgOtrResultArray[1] = "DLSG";

                mgOtrResultArray[3] = "10";

                mgOtrResultArray[2] = mgOtrRes.rate.ToString();
                //HelperFuncs.writeToSiteErrors("CSXI API", csxiResultArray[2]);

                //mgOtrResultArray[4] = dt.ToShortDateString();
                mgOtrResultArray[4] = "";

                //mgOtrResultArray[5] = dt.AddDays(transit).ToShortDateString();
                mgOtrResultArray[5] = "";

                mgOtrResultArray[6] = "";
                mgOtrResultArray[11] = "OTR";
                mgOtrResultArray[12] = mgOtrRes.confidenceGroup;
            }
        }
        
        
        decimal.TryParse(ConfidenceRate, out mgOtrRes.confidenceRate);

        //string test = "";

    }

    #endregion

    #endregion

    #region Helper Functions

    #region getResultString

    public static string getResultString(ref IntermodalRater.railResult railResult)
    {
        if (string.IsNullOrEmpty(railResult.success) || !railResult.success.Equals("success"))
        {
            return "0";
        }
        else
        {
            return string.Concat("success=", railResult.success, "&rate=", railResult.rate, "&transitTime=", railResult.transitTime,
                "&hasCapacity=", railResult.hasCapacity, "&firstCapacityDate=", railResult.firstCapacityDate.ToShortDateString(),
                "&eta=", railResult.eta.ToShortDateString(), "&containerSize=", railResult.containerSize);
        }
    }

    #endregion

    #region setParameters

    public static void setParameters(ref FormDataCollection form, ref SharedRail.Parameters parameters, ref int CarrierCompID)
    {
        DateTime pickupDate;

        if (!DateTime.TryParse(form.Get("pickupDate"), out pickupDate))
        {
            pickupDate = DateTime.Today.AddDays(1);
        }

        string isHazMat = form.Get("isHazMat");
        bool chkHazmat = false;
        if (!bool.TryParse(isHazMat, out chkHazmat))
        {
            chkHazmat = false;
        }

        parameters.username = form.Get("username");
        parameters.password = form.Get("password");
        parameters.origZip = form.Get("originZip");
        parameters.destZip = form.Get("destinationZip");
        parameters.origCity = form.Get("originCity");
        parameters.destCity = form.Get("destinationCity");
        parameters.isHazMat = chkHazmat;

        parameters.puDate = pickupDate;

        List<HelperFuncs.Credentials> crds = new List<HelperFuncs.Credentials>();
        crds = HelperFuncs.GetLoginsByCarID(CarrierCompID);
        parameters.username = crds[0].username;
        parameters.password = crds[0].password;
    }

    #endregion

    #region Not used processRequest

    public static string processRequest(RaterDelegate rater, ref FormDataCollection form, SharedRail.ICarrier carrier)
    {
        try
        {

            #region Get form data

            //HelperFuncs.writeToSiteErrors("test", "test1");

            string username = form.Get("username");
            string password = form.Get("password");

            string originZip = form.Get("originZip");
            string destinationZip = form.Get("destinationZip");

            string originCity = form.Get("originCity");
            string destinationCity = form.Get("destinationCity");

            string isHazMat = form.Get("isHazMat");

            string[] additionalServices = new string[1];
            DateTime pickupDate;

            if (!DateTime.TryParse(form.Get("pickupDate"), out pickupDate))
            {
                pickupDate = DateTime.Today.AddDays(1);
            }

            #endregion

            HelperFuncs.writeToSiteErrors("get_loup_rates", "get_loup_rates");

            List<string[]> accessorials = new List<string[]>();
            IntermodalRater.railResult railResult = new IntermodalRater.railResult();

            bool chkHazmat = false;
            if (!bool.TryParse(isHazMat, out chkHazmat))
            {
                chkHazmat = false;
            }

            SharedRail.Parameters parameters = new SharedRail.Parameters();
            parameters.username = username;
            parameters.password = password;
            parameters.origZip = originZip;
            parameters.destZip = destinationZip;
            parameters.origCity = originCity;
            parameters.destCity = destinationCity;
            parameters.isHazMat = chkHazmat;

            List<HelperFuncs.Credentials> crds = new List<HelperFuncs.Credentials>();
            crds = HelperFuncs.GetLoginsByCarID(78573);
            username = crds[0].username;
            password = crds[0].password;

            //SharedRail.ICarrier carrier = new LOUP(parameters);

            railResult = carrier.getRate();

            if (string.IsNullOrEmpty(railResult.success) || !railResult.success.Equals("success"))
            {
                return "0";
            }
            else
            {
                return string.Concat("success=", railResult.success, "&rate=", railResult.rate, "&transitTime=", railResult.transitTime,
                    "&hasCapacity=", railResult.hasCapacity, "&firstCapacityDate=", railResult.firstCapacityDate.ToShortDateString(),
                    "&eta=", railResult.eta.ToShortDateString(), "&containerSize=", railResult.containerSize);
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("get_loup_rates", e.ToString());
            return "0";
        }
    }

    #endregion

    #region saveResults

    public static void saveResults(bool after, ref string originCity, ref string destCity, ref string originState, ref string destState,
        ref string usernameGlobal, ref List<string[]> results, ref DateTime puDateGlobal, ref int QuoteID)
    {
        Double rate;
        string sql = "";
        DateTime eta;
        SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aesrater_dataConnectionStringSS"].ConnectionString);
        SqlCommand command = new SqlCommand();

        try
        {
            if (after == false)
            {
                string origin = originCity + ", " + originState;
                string dest = destCity + ", " + destState;
                DateTime today = new DateTime();
                today = DateTime.Today;
                int Count = 1;


                //sql = "INSERT INTO SQL_STATS_GCM(Type, Direction, Service, Completed, Count, UserName, Origin, Destination, Day, Source) " +
                //"VALUES(" + "'Rail'" + ", " + "'Domestic'" + ", " +
                //"'D2D'" + ", " + "'NO'" + ", '" + Count + "', '" + usernameGlobal + "', " + "'" + origin + "'" + ", " +
                //"'" + dest + "'" + ", " + "'" + today + "'" + "); SELECT SCOPE_IDENTITY()";

                sql = string.Concat("INSERT INTO SQL_STATS_GCM(Type, Direction, Service, Completed, Count, UserName, Origin, Destination, Day, Source) ",
                "VALUES(", "'Rail'", ", ", "'Domestic'", ", ",
                "'D2D'", ", ", "'NO'", ", '", Count, "', '", usernameGlobal, "', ", "'", origin, "'", ", ",
                "'", dest, "'", ", ", "'", today, "','", "API'); SELECT SCOPE_IDENTITY()");

                command.CommandText = sql;
                command.Connection = conn;

                conn.Open();
                Int32.TryParse(command.ExecuteScalar().ToString(), out resultID);

                QuoteID = resultID;
            }
            else
            {

                command.Connection = conn;
                conn.Open();
                //if list has at least one result, update completed column in main table, add rows for each item in second table
                if (results.Count > 0)
                {
                    sql = "UPDATE SQL_STATS_GCM " +
                           "SET Completed = 'YES' " +
                           "WHERE ID = '" + resultID + "'";


                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
                for (int i = 0; i < results.Count; i++)
                {
                    if (!Double.TryParse(results[i][2].Replace("$", "").Replace(",", ""), out rate))
                    {
                        throw new Exception("Could not parse rate");
                    }
                    if (!DateTime.TryParse(results[i][5], out eta))
                    {
                        eta = DateTime.MinValue;
                    }
                    //puDateGlobal
                    //     sql = "INSERT INTO SQL_STATS_GCM_RAIL(ID, Rate, Carrier, PuDate, Eta) " +
                    //"VALUES('" + resultID + "', " + rate + ", '" + results[i][1] + "', '" + puDateGlobal + "', '" + eta + "')";

                    //sql = "INSERT INTO SQL_STATS_GCM_RAIL(ID, Rate, Carrier, PuDate, Eta, Time) " +
                    //    "VALUES('" + resultID + "', " + rate + ", '" + results[i][1] + "', '" + puDateGlobal + "', '" + eta + "', '" + 
                    //    DateTime.Now.ToShortTimeString() + "')";

                    sql = string.Concat("INSERT INTO SQL_STATS_GCM_RAIL(ID, Rate, Carrier, PuDate, Eta, Time) ",
                        "VALUES('", resultID, "', ", rate, ", '", results[i][1], "', '", puDateGlobal, "', '", eta, "', '",
                        DateTime.Now.ToShortTimeString(), "')");

                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }

            conn.Close();
            conn.Dispose();
        }
        catch
        {
            try
            {
                conn.Close();
                conn.Dispose();
                command.Dispose();
            }
            catch { }
        }
    }

    #endregion

    #region generic_http_request_basic_auth
    public static object generic_http_request_basic_auth(string return_type, CookieContainer container, string req_url, string req_referrer, string req_content_type, string req_accept,
          string req_method, string data_string, bool req_allow_redirect, string auth, int timeOut)
    {

        #region BuildRequest

        StringBuilder sb = new StringBuilder();
        String DataParameters;
        byte[] buf = new byte[8192];

        HttpWebRequest request = (HttpWebRequest)
            WebRequest.Create(req_url);
        request.Referer = req_referrer;

        request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/34.0.1847.116 Safari/537.36";
        request.KeepAlive = true;
        request.Method = req_method;
        request.ContentType = req_content_type;
        request.Accept = req_accept;
        request.CookieContainer = new CookieContainer();
        request.CookieContainer = container;
        request.Timeout = timeOut;
        //if (addHeader == true)
        //{

        //}

        request.Headers.Add("Authorization", "Basic " + auth);

        #endregion

        #region GetResponse

        if (req_method == "POST")
        {
            #region Write to data stream
            DataParameters = data_string;
            byte[] postData;
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            postData = encoding.GetBytes(DataParameters);
            request.ContentLength = postData.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(postData, 0, postData.Length);
            }
            #endregion
        }

        // Execute the request
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            if (return_type == "string")
            {
                // We will read data via the response stream
                #region Read data
                using (Stream resStream = response.GetResponseStream())
                {
                    string tempString = null;
                    int count = 0;

                    do
                    {
                        // Fill the buffer with data
                        count = resStream.Read(buf, 0, buf.Length);

                        // Make sure we read some data
                        if (count != 0)
                        {
                            // Translate from bytes to ASCII text
                            tempString = Encoding.ASCII.GetString(buf, 0, count);

                            // Continue building the string
                            sb.Append(tempString);
                        }
                    }
                    while (count > 0); // Any more data to read?
                }
                #endregion
            }
            request.Abort();

            #region Return result
            if (return_type == "string")
            {
                response.Close();
                return sb.ToString();
            }
            else if (return_type == "collection")
            {
                CookieCollection collection = new CookieCollection();
                collection = response.Cookies;
                response.Close();
                return collection;
            }
            else if (return_type == "location")
            {
                string s = response.Headers["Location"];
                response.Close();
                return s;
            }
            else
            {
                response.Close();
                return null;
            }
            #endregion
        }
        #endregion
    }
    #endregion

    #region GetMarkupPercent
    public static int[] GetMarkupPercent(string username, string password)
    {
        try
        {

            int[] info = new int[2];

            string sql = "";

            sql = "SELECT IntermodalMarkupPercent, IntermodalMinimum " +
                    "FROM tbl_LOGIN WHERE Username=" + "'" + username + "'" + " AND Password=" + "'" + password + "'";

            SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aesmain_dataConnectionStringSS"].ConnectionString);
            SqlCommand command = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader dr = command.ExecuteReader();

            if (dr.Read())
            {
                info[0] = (int)dr[0];
                info[1] = (int)dr[1];
            }
            else
            {
                throw new Exception(username + " " + password);
            }
            dr.Close();
            conn.Close();
            conn.Dispose();
            return info;
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("GetMarkupPercentIntermodal", e.ToString());
            int[] info = new int[2];
            info[0] = -1;
            return info;
        }

    }
    #endregion

    #region normalizeTrailNumbs
    public static string normalizeTrailNumbs(string oldNum)
    {
        string tmp = oldNum;
        if (tmp.Contains(".") == false)
        {
            oldNum = tmp + ".00";
        }
        else // has . 
        {
            int ind = tmp.IndexOf('.');
            tmp = tmp.Substring(ind + 1);
            if (tmp.Length == 1)
            {
                oldNum = oldNum + "0";
            }
            if (tmp.Length == 3)
            {
                oldNum = oldNum.Remove(ind + 1);
                oldNum += tmp.Remove(tmp.Length - 1);
            }
        }
        return oldNum;
    }
    #endregion

    #region Mimic javascript getTime()
    // Mimic javascript getTime()
    public static Int64 GetTime(ref DateTime date)
    {
        Int64 retval = 0;
        DateTime st = new DateTime(1970, 1, 1);
        TimeSpan t = (date.ToUniversalTime() - st);
        retval = (Int64)(t.TotalMilliseconds + 0.5);
        return retval;
    }
    #endregion

    #region getDateFromJsGetTimeValue

    public static DateTime getDateFromJsGetTimeValue(long jsGetTimeValue)
    {
        return new DateTime(1970, 01, 01).AddMilliseconds(jsGetTimeValue);

    }

    #endregion

    // Not used
    #region insertIntoRailLogs
    private static void insertIntoRailLogs(string CarrierName, int CarrierID, string ResultOK, string CarrierResponse, string SystemException, double Rate,
        ref string CodeLocation)
    {
        SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aesmain_dataConnectionStringSS"].ConnectionString);
        SqlCommand command = new SqlCommand();
        try
        {
            conn.Open();
            command.Connection = conn;
            string sql;
            sql = "INSERT INTO ZZ_RAIL_LOGS (CarrierName, CarrierID, ResultOK, CarrierResponse, SystemException, Location, Rate, Date, Time) VALUES('" +
                CarrierName + "'," + CarrierID + ",'" + ResultOK + "','" + CarrierResponse + "','" + SystemException + "','" + CodeLocation + "'," + Rate +
                ",'" + DateTime.Today.ToShortDateString() + "', '" + DateTime.Now.ToShortTimeString() + "')";

            //HelperFuncs.writeToSiteErrors("RailsLogs", sql);
            command.Connection = conn;
            command.CommandText = sql;
            command.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
            command.Dispose();
        }
        catch (Exception e)
        {
            try
            {
                HelperFuncs.writeToSiteErrors("RailsLogs", e.ToString());
                conn.Close();
                conn.Dispose();
                command.Dispose();
            }
            catch { }
        }
    }
    #endregion

    #region cityStateCountryByZip

    public static void cityStateCountryByZip(ref string[] res, string zip)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringRater2009))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;
                    comm.CommandText = string.Concat("SELECT State, City, Country ",
                                                     "FROM SQL_ZIPS ",
                                                     "WHERE Zip ='", zip, "' and Country not in('MX','PR')"); ;

                    using (SqlDataReader dr = comm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            res[0] = dr["City"].ToString();
                            res[1] = dr["State"].ToString();
                            res[2] = dr["Country"].ToString();
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("RailsRater", e.ToString());
        }
    }

    #endregion

    #endregion

    #region Carrier functions not in use

    #region GetCSXIInfo

    public static void GetCSXIInfo(ref string originZipGlobal, ref string destZipGlobal, ref DateTime puDateGlobal)
    {
        int timeOut = 25000;
        try
        {
            List<HelperFuncs.Credentials> crds = new List<HelperFuncs.Credentials>();
            string username = "", password = "";

            try
            {
                crds = HelperFuncs.GetLoginsByCarID(90199);
                username = crds[0].username;
                password = crds[0].password;
            }
            catch (Exception ex)
            {
                HelperFuncs.writeToSiteErrors("CSXI", ex.ToString());
            }

            string url, referrer, contentType, accept, method, data, doc, originCity = "", destCity = "", originState = "", destState = "";

            CookieContainer container = new CookieContainer();
            CookieCollection collection = new CookieCollection();

            double parseDbl;

            #region login and go to rate page

            #region Login request
            referrer = "https://shipcsx.com/pub_sx_mainpagepublic_jct/sx.shipcsxpublic/PublicNavbar";
            url = "https://shipcsx.com/pkmslogin.form";
            contentType = "application/x-www-form-urlencoded";
            accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            method = "POST";
            data = "login-form-type=pwd&username=" + username + "&password=" + password + "&LoginGoButton.x=7&LoginGoButton.y=8"; //user password
            collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, data, true, timeOut);
            #endregion

            #region Some required requests
            //--------------------------------------------------------------------------------------------------------------
            url = "https://shipcsx.com/sx_mainpage_jct/sx.shipcsx/Main";
            referrer = "";
            contentType = "";
            method = "GET";
            collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", true, timeOut);

            //--------------------------------------------------------------------------------------------------------------
            referrer = url;
            url = "https://shipcsx.com/sx_mainpage_jct/sx.shipcsx/Navbar";
            collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", true, timeOut);

            //--------------------------------------------------------------------------------------------------------------

            url = "https://shipcsx.com/secure/ec.mysx/MyShipCSX.evt";
            collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", true, timeOut);

            //--------------------------------------------------------------------------------------------------------------

            referrer = url;
            url = "https://shipcsx.com/secure/ec.mysx/PortalHandler.evt";
            contentType = "application/x-www-form-urlencoded; charset=UTF-8";
            method = "POST";
            data = "AJAX_ACTION=VIEWPORT_LOAD";
            collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, data, true, timeOut);

            //--------------------------------------------------------------------------------------------------------------

            referrer = url;
            url = "https://shipcsx.com/secure/ec.mysx/NewsHandler.evt";
            data = "AJAX_ACTION=NEWS_DATA";
            collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, data, true, timeOut);

            //--------------------------------------------------------------------------------------------------------------

            url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/TruckingMain";
            referrer = "https://shipcsx.com/sx_mainpage_jct/sx.shipcsx/Navbar";
            contentType = "";
            method = "GET";
            collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", true, timeOut);

            //--------------------------------------------------------------------------------------------------------------

            referrer = url;
            url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/jsp/sx_base_classes/blank.jsp";
            collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", true, timeOut);

            //--------------------------------------------------------------------------------------------------------------

            //referrer = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/TruckingMain";
            url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/SpotQuoteBegin";
            collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", true, timeOut);

            //--------------------------------------------------------------------------------------------------------------

            referrer = url;
            url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/jsp/sx_base_classes/blank.jsp";
            collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", true, timeOut);

            //--------------------------------------------------------------------------------------------------------------
            #endregion

            #region Get City and State

            // Get City and State
            referrer = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/SpotQuoteBegin";
            url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/GetCityStateData?zip=" + originZipGlobal;
            doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true, timeOut);

            // Scrape origin city and state
            string[] tokens = new string[3];
            tokens[0] = "<city>";
            tokens[1] = ">";
            tokens[2] = "<";

            originCity = HelperFuncs.scrapeFromPage(tokens, doc);
            if (originCity == "" || originCity == "not found")
                throw new Exception("No matching City/State for zipcode " + originZipGlobal);

            tokens[0] = "<state>";
            originState = HelperFuncs.scrapeFromPage(tokens, doc);

            url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/GetUnservicableLocations?zip=" + originZipGlobal;
            doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true, timeOut);

            url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/GetUnservicableLocations?zip=" + destZipGlobal;
            doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true, timeOut);

            url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/GetCityStateData?zip=" + destZipGlobal;
            doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true, timeOut);

            // Scrape destination city and state
            tokens[0] = "<city>";
            destCity = HelperFuncs.scrapeFromPage(tokens, doc);
            if (destCity == "" || destCity == "not found")
                throw new Exception("No matching City/State for zipcode " + destZipGlobal);

            tokens[0] = "<state>";
            destState = HelperFuncs.scrapeFromPage(tokens, doc);

            #endregion

            string day = puDateGlobal.Day.ToString(), month = puDateGlobal.Month.ToString(), year = puDateGlobal.Year.ToString();
            if (day.Length.Equals(1))
            {
                day = "0" + day;
            }

            if (month.Length.Equals(1))
            {
                month = "0" + month;
            }

            referrer = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/SpotQuoteBegin";
            url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/SpotQuoteSelectPrice";
            contentType = "application/x-www-form-urlencoded";
            accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            method = "POST";

            data = "addFavoriteLaneName=&originZipCode=" + originZipGlobal + "&originCityStateDisplay=" + originCity.Replace(" ", "+") + "%2C+" + originState +
                "&originCity=" + originCity.Replace(" ", "+") + "&originState=" + originState + "&destinationZipCode=" + destZipGlobal +
                "&destinationCityStateDisplay=" + destCity.Replace(" ", "+") + "%2C+" + destState + "&destinationCity=" + destCity.Replace(" ", "+") +
                "&destinationState=" + destState + "&pickupDate=" + month + "%2F" + day + "%2F" + year + "&selectedTimeOfDay=5&begEquipment=53" +
                "&numberOfLoads=1&extraPickup0ZipCode=&extraPickup0CityStateDisplay=&extraPickup0City=" +
                "&extraPickup0State=&extraDelivery0ZipCode=&extraDelivery0CityStateDisplay=&extraDelivery0City=" +
                "&extraDelivery0State=&doResetQuote=&reuseAdapter=true&requote=false&quoteType=&templateId=";
            doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, data, true, timeOut);

            //HelperFuncs.writeToSiteErrors("CSXI", doc);
            //--------------------------------------------------------------------------------------------------------------

            accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            referrer = url;
            url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/jsp/sx_base_classes/blank.jsp";
            contentType = "";
            method = "GET";
            collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", true, timeOut);

            //--------------------------------------------------------------------------------------------------------------

            #endregion


            string hazmat = "";
            //if hazardous
            //hazmat = "&general_services=243"; 

            //try date
            //string tmp2 = "", tmp;
            DateTime dt = puDateGlobal;
            //dt = DateTime.Today.AddDays(1);

            DateTime dtLast = new DateTime();
            dtLast = DateTime.Today.AddDays(6);
            TimeSpan span = dtLast - dt;

            bool successBool = false;
            string price = "";
            string transitTime = "";

            for (int i = 0; i <= span.TotalDays; i++)
            {
                dt = DateTime.Today.AddDays(i + 1);
                //dt = dt.AddDays(i);
                if (dt.DayOfWeek.ToString() == "Saturday" || dt.DayOfWeek.ToString() == "Sunday")
                {
                    continue;
                }
                doc = tryDateCSXI(container, originZipGlobal, destZipGlobal, originCity, destCity, originState, destState, hazmat, i + 1);
                //if (doc.Contains("No pricing is available"))
                //{
                //    throw new Exception("No pricing is available");
                //}

                tokens[0] = "<price>";
                tokens[1] = ">";
                tokens[2] = "<";

                price = HelperFuncs.scrapeFromPage(tokens, doc).Replace("$", "");
                if (!double.TryParse(price, out parseDbl))
                {
                    continue;
                }
                else
                {
                    successBool = true;
                }

                tokens[0] = "<transitTime>";
                tokens[1] = ">";
                tokens[2] = "<";
                transitTime = HelperFuncs.scrapeFromPage(tokens, doc).Replace("days", "").Trim();

                //if (!(tmp.Contains("Please Call") || tmp.Contains("No Capacity")))
                //{
                //    successBool = true;
                //    break;
                //}
                if (successBool.Equals(true))
                {
                    break;
                }
            }

            if (successBool == true)
            {


                csxiResultArray[0] = SharedRail.success;
                csxiResultArray[1] = "CSXI";

                csxiResultArray[3] = transitTime;


                csxiResultArray[2] = price;
                //insertIntoRailLogs("CSXI", 90199, "1", "", "", Convert.ToDouble(rate));
                ///HelperFuncs.writeToSiteErrors("CSXI live rate", price);

                csxiResultArray[4] = dt.ToShortDateString();

                Int32 transit;
                if (!Int32.TryParse(csxiResultArray[3], out transit))
                {
                    try
                    {

                        HelperFuncs.writeToSiteErrors("CSXI", "could not parse transit " + csxiResultArray[3]);
                    }
                    catch { }
                }
                csxiResultArray[5] = dt.AddDays(transit).ToShortDateString();

            }
            else
            {
                throw new Exception("No Capacity for all days, or no rate found");
            }
        }
        catch (Exception e)
        {
            try
            {

                if (e.Message != "No pricing is available" && e.Message != "No matching City/State" && e.Message != "No Capacity for all days, or no rate found")
                {
                    //insertIntoRailLogs("CSXI", 90199, "0", "", e.ToString(), 0);
                    HelperFuncs.writeToSiteErrors("CSXI", e.ToString());
                }
            }
            catch { }
        }
    }

    #endregion

    #region tryDateCSXI
    public static string tryDateCSXI(CookieContainer container, string originZip, string destZip, string originCity, string destCity, string originState,
     string destState, string hazmat, int daysToAdd)
    {
        string url, referrer, contentType, accept, method, doc;
        DateTime dt = DateTime.Today.AddDays(daysToAdd);

        string day = dt.Day.ToString(), month = dt.Month.ToString(), year = dt.Year.ToString();
        if (day.Length.Equals(1))
        {
            day = "0" + day;
        }

        if (month.Length.Equals(1))
        {
            month = "0" + month;
        }

        Int64 timeStamp = GetTime(ref dt);
        url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/GetAlternateLanePriceInfo?date=" + month + "/" + day + "/" + year +
            "&timeSlotId=5&timestamp=" + timeStamp.ToString();
        referrer = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/SpotQuoteSelectPrice";
        contentType = "";
        accept = "*/*";
        method = "GET";
        doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true, 25000);

        return doc;
    }
    #endregion

    //--

    public static void GetIDIInfo()
    {
        try
        {
            //string url, referrer, contentType, accept, method, data;
            //CookieContainer container = new CookieContainer();
            //string doc;

            //url = "http://www.independentdispatch.com/online/login/customer/index.php3";
            //referrer = "http://www.independentdispatch.com/";
            //contentType = "";
            //accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            //method = "GET";

            //doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true, "IDI");

            //url = "http://www.independentdispatch.com/online/login/customer/rates.php3";
            //referrer = "http://www.independentdispatch.com/online/login/customer/index.php3";

            //doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true, "IDI");


            //referrer = url;
            //contentType = "application/x-www-form-urlencoded";
            //method = "POST";
            //data = "ocity=" + originCity.ToLower() + "&ostate=" + originState.ToLower() + "&dcity=" + destCity.ToLower() + "&dstate=" + destState.ToLower();

            //doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, data, true, "IDI");
            //if (doc.Contains("No rate information was found for the origin and destination points entered"))
            //{
            //    throw new Exception("No rate information was found for the origin and destination points entered");
            //}
            ////scrape rate
            //List<string> rates = new List<string>();
            //List<string> companies = new List<string>();
            //string[] res = new string[6];

            //string tmp = doc;
            //int ind = tmp.IndexOf("53ft Container");
            ////throw new Exception(doc);
            //tmp = tmp.Substring(ind);

            //ind = tmp.IndexOf("</table>");
            //tmp = tmp.Remove(ind); //cut off document at end of table tag

            //while (tmp.IndexOf("<div class=\"body1") != -1)
            //{
            //    ind = tmp.IndexOf("<div class=\"body1");
            //    tmp = tmp.Substring(ind + 1);
            //    ind = tmp.IndexOf(">");
            //    tmp = tmp.Substring(ind + 1);
            //    ind = tmp.IndexOf("<");
            //    companies.Add(tmp.Remove(ind).Replace("\n", "").Trim());

            //    for (int i = 0; i < 6; i++)
            //    {
            //        ind = tmp.IndexOf("<div class=\"body1");
            //        tmp = tmp.Substring(ind + 1);
            //    }
            //    ind = tmp.IndexOf(">");
            //    tmp = tmp.Substring(ind + 1);
            //    ind = tmp.IndexOf("<");
            //    rates.Add(tmp.Remove(ind).Replace("\n", "").Trim());
            //}


            ////build td's string
            //string delimiter = "<('')>";
            //Double rate, minRate = 0;
            //int indOfMinRate = -1;

            ////find index of lowest rate
            //if (rates.Count == 0)
            //{
            //    throw new Exception("Rate not found on page");
            //}
            //else if (rates.Count > 1)
            //{
            //    for (int i = 0; i < rates.Count; i++)
            //    {
            //        if (!Double.TryParse(rates[i].Replace("$", "").Replace(",", ""), out rate))
            //        {
            //            throw new Exception("Could not parse rate"); ;
            //        }
            //        if (i == 0)
            //        {
            //            minRate = rate;
            //            indOfMinRate = i;
            //        }
            //        if (minRate > rate)
            //        {
            //            minRate = rate;
            //            indOfMinRate = i;
            //        }
            //    }
            //    res[0] = "success";
            //    res[1] = "IDI" + delimiter + companies[indOfMinRate];
            //    res[2] = "$" + rates[indOfMinRate];



            //    res[3] = "Please Call 877-890-2295";
            //    res[4] = "Please Call 877-890-2295";
            //    res[5] = "Please Call 877-890-2295";



            //}
            //else if (rates.Count == 1)
            //{
            //    res[0] = "success";
            //    res[1] = "IDI" + delimiter + companies[0];
            //    res[2] = "$" + rates[0];



            //    res[3] = "Please Call 877-890-2295";
            //    res[4] = "Please Call 877-890-2295";
            //    res[5] = "Please Call 877-890-2295";

            //}


            //string s = "";
        }
        catch (Exception e)
        {
            string str = e.ToString();
            //try
            //{
            //    HelperFuncs.writeToSiteErrors("RailsRater", e.ToString());
            //}
            //catch { }
        }
    }

    #endregion

}