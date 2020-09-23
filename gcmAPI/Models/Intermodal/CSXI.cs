#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

#endregion

public class CSXI : SharedRail.ICarrier
{
    #region fields

    public string username, password, origZip, destZip, origCity, destCity;
    //CookieContainer container;
    public bool isHazMat;
    IntermodalRater.railResult railResultObj;
    DateTime puDate;
    //List<SharedRail.Result> streamlineResults;
    //List<string> originRamps, destinationRamps;

    #endregion

    #region Constructor

    public CSXI(SharedRail.Parameters parameters)
    {
        username = parameters.username;
        password = parameters.password;
        origZip = parameters.origZip;
        destZip = parameters.destZip;
        //origCity = parameters.origCity;
        //destCity = parameters.destCity;
        //isHazMat = parameters.isHazMat;
        puDate = parameters.puDate;
    }

    #endregion

    #region interface implementation

    #region login

    public void login()
    {

        throw new Exception("not implemented");

    }

    #endregion

    #region getRate

    public IntermodalRater.railResult getRate()
    {
        // CSXI are blocking our IP bc of scraping
        railResultObj.success = "";
        return railResultObj;

        int timeOut = 25000;
        try
        {
            HelperFuncs.writeToSiteErrors("CSXI", "oop");
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

            #region Login and go to rate page

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
            url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/SpotQuoteBegin?";
            collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", true, timeOut);

            //--------------------------------------------------------------------------------------------------------------

            referrer = url;
            url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/jsp/sx_base_classes/blank.jsp";
            collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", true, timeOut);

            //--------------------------------------------------------------------------------------------------------------
            #endregion

            #region Get City and State

            // Get City and State
            referrer = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/SpotQuoteBegin?";
            url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/GetCityStateData?zip=" + origZip;
            doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true, timeOut);

            // Scrape origin city and state
            string[] tokens = new string[3];
            tokens[0] = "<city>";
            tokens[1] = ">";
            tokens[2] = "<";

            HelperFuncs.writeToSiteErrors("CSXI origZip", origZip);

            originCity = HelperFuncs.scrapeFromPage(tokens, doc);
            if (originCity == "" || originCity == "not found")
                throw new Exception("No matching City/State for zipcode " + origZip);

            tokens[0] = "<state>";
            originState = HelperFuncs.scrapeFromPage(tokens, doc);

            url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/GetUnservicableLocations?zip=" + origZip;
            doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true, timeOut);

            url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/GetUnservicableLocations?zip=" + destZip;
            doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true, timeOut);

            url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/GetCityStateData?zip=" + destZip;
            doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true, timeOut);

            // Scrape destination city and state
            tokens[0] = "<city>";
            destCity = HelperFuncs.scrapeFromPage(tokens, doc);
            if (destCity == "" || destCity == "not found")
                throw new Exception("No matching City/State for zipcode " + destZip);

            tokens[0] = "<state>";
            destState = HelperFuncs.scrapeFromPage(tokens, doc);

            #endregion

            #region Variables

            HelperFuncs.writeToSiteErrors("CSXI TEST TEST TEST", "CSXI TEST TEST TEST");

            HelperFuncs.writeToSiteErrors("CSXI puDate", puDate.ToShortDateString());
            DateTime dt = puDate;
            DateTime dtLast = new DateTime();
            dtLast = DateTime.Today.AddDays(6);
            TimeSpan span = dtLast - dt;

            HelperFuncs.writeToSiteErrors("CSXI span.TotalDays", span.TotalDays.ToString());

            bool hasCapacity = true;
            //string hazmat = "";
            //double parseDbl;
            bool successBool = false;
            string price = "";
            string transitTime = "";

            bool isLastDateTry = false;

            #endregion

            for (int i = 0; i <= span.TotalDays; i++)
            {
                //HelperFuncs.writeToSiteErrors("CSXI i", i.ToString());
                //HelperFuncs.writeToSiteErrors("CSXI span.TotalDays", span.TotalDays.ToString());

                if (i.Equals((int)span.TotalDays))
                {
                    HelperFuncs.writeToSiteErrors("CSXI i=span", "CSXI i=span");
                    isLastDateTry = true;
                }

                dt = DateTime.Today.AddDays(i + 1);

                if (dt.DayOfWeek.ToString() == "Saturday" || dt.DayOfWeek.ToString() == "Sunday")
                {
                    continue;
                }

                #region Fix date

                string day = dt.Day.ToString(), month = dt.Month.ToString(), year = dt.Year.ToString();
                if (day.Length.Equals(1))
                {
                    day = "0" + day;
                }

                if (month.Length.Equals(1))
                {
                    month = "0" + month;
                }

                #endregion

                hasCapacity = true;

                tryDateCSXI_WithCapacity(ref container, ref origZip, ref originCity,
                    ref originState, ref destZip, ref destCity, ref destState, ref month, ref day, ref year,
                    ref hasCapacity, ref transitTime, ref price, ref successBool, ref isLastDateTry);

                if (successBool.Equals(true) && hasCapacity.Equals(true))
                {
                    break;
                }
            }

            #endregion

            #region Not used
            //for (int i = 0; i <= span.TotalDays; i++)
            //{
            //    dt = DateTime.Today.AddDays(i + 1);
            //    //dt = dt.AddDays(i);
            //    if (dt.DayOfWeek.ToString() == "Saturday" || dt.DayOfWeek.ToString() == "Sunday")
            //    {
            //        continue;
            //    }
            //    doc = tryDateCSXI(container, originZipGlobal, destZipGlobal, originCity, destCity, originState, destState, hazmat, i + 1);
            //    //if (doc.Contains("No pricing is available"))
            //    //{
            //    //    throw new Exception("No pricing is available");
            //    //}

            //    tokens[0] = "<price>";
            //    tokens[1] = ">";
            //    tokens[2] = "<";

            //    price = HelperFuncs.scrapeFromPage(tokens, doc).Replace("$", "");
            //    if (!double.TryParse(price, out parseDbl))
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        successBool = true;
            //    }

            //    tokens[0] = "<transitTime>";
            //    tokens[1] = ">";
            //    tokens[2] = "<";
            //    transitTime = HelperFuncs.scrapeFromPage(tokens, doc).Replace("days", "").Trim();

            //    if (successBool.Equals(true))
            //    {
            //        break;
            //    }
            //}
            #endregion

            if (successBool == true)
            {
                #region Set the result object

                HelperFuncs.writeToSiteErrors("CSXI success", "CSXI success");
                //string[] csxiResultArray = new string[7];
                ////csxiResultArray[0] = SharedRail.success;
                //csxiResultArray[0] = "success";
                //csxiResultArray[1] = "CSXI";

                //csxiResultArray[3] = transitTime;

                //csxiResultArray[2] = price;
                //insertIntoRailLogs("CSXI", 90199, "1", "", "", Convert.ToDouble(rate));
                HelperFuncs.writeToSiteErrors("CSXI live rate", price);

                //csxiResultArray[4] = dt.ToShortDateString();

                Int32 transit;
                //if (!Int32.TryParse(csxiResultArray[3], out transit))
                //{
                //    HelperFuncs.writeToSiteErrors("CSXI", "could not parse transit " + csxiResultArray[3]);
                //}
                //csxiResultArray[5] = dt.AddDays(transit).ToShortDateString();
                //csxiResultArray[6] = "FiftyThreeFt";

                //List<string[]> list
                //list.Add(csxiResultArray);


                railResultObj.success = "success";
                railResultObj.transitTime = transitTime;
                railResultObj.rate = price;
                railResultObj.firstCapacityDate = dt;
                if (int.TryParse(transitTime, out transit))
                {
                    railResultObj.eta = dt.AddDays(transit);
                }

                railResultObj.hasCapacity = hasCapacity;
                railResultObj.containerSize = "FiftyThreeFt";

                return railResultObj;

                #endregion
            }
            else
            {
                //throw new Exception("No Capacity for all days, or no rate found");
                HelperFuncs.writeToSiteErrors("CSXI", "No Capacity for all days, or no rate found");
                railResultObj.success = "";
                return railResultObj;
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("CSXI", e.ToString());
            railResultObj.success = "";
            return railResultObj;
        }
    }

    #endregion

    #region scrapeResult

    public void scrapeResult(ref string doc)
    {
        throw new Exception("not implemented");
    }

    #endregion

    #endregion

    #region Special methods

    #region tryDateCSXI_WithCapacity

    public static void tryDateCSXI_WithCapacity(ref CookieContainer container, ref string originZipGlobal, ref string originCity,
        ref string originState, ref string destZipGlobal, ref string destCity, ref string destState, ref string month, ref string day,
        ref string year, ref bool hasCapacity, ref string transitTime, ref string price, ref bool successBool, ref bool isLastDateTry)
    {
        #region Variables

        HelperFuncs.writeToSiteErrors("CSXI date", month + " " + day);
        //HelperFuncs.writeToSiteErrors("CSXI isLastDateTry", isLastDateTry.ToString());

        string url, referrer, contentType, accept, method, data, doc;

        referrer = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/SpotQuoteBegin";
        url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/SpotQuoteSelectPrice";
        contentType = "application/x-www-form-urlencoded";
        accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        method = "POST";

        data = string.Concat("addFavoriteLaneName=&originZipCode=", originZipGlobal, "&originCityStateDisplay=", originCity.Replace(" ", "+"), "%2C+", originState,
            "&originCity=", originCity.Replace(" ", "+"), "&originState=", originState, "&destinationZipCode=", destZipGlobal,
            "&destinationCityStateDisplay=", destCity.Replace(" ", "+"), "%2C+", destState, "&destinationCity=", destCity.Replace(" ", "+"),
            "&destinationState=", destState, "&pickupDate=", month, "%2F", day, "%2F", year, "&selectedTimeOfDay=5&begEquipment=53",
            "&numberOfLoads=1&extraPickup0ZipCode=&extraPickup0CityStateDisplay=&extraPickup0City=",
            "&extraPickup0State=&extraDelivery0ZipCode=&extraDelivery0CityStateDisplay=&extraDelivery0City=",
            "&extraDelivery0State=&doResetQuote=&reuseAdapter=true&requote=false&quoteType=&templateId=");

        #endregion

        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, data, false, false, "", "");

        //HelperFuncs.writeToSiteErrors("CSXI", doc);

        // Get cost/costs from this page
        // Scrape html table 

        string[] tokens2 = new string[3];
        tokens2[0] = "<table border=\"0\" cellpadding=\"4\" cellspacing=\"0\" width=\"100%";
        tokens2[1] = ">";
        tokens2[2] = "</table>";
        string tblHtml = HelperFuncs.scrapeFromPage(tokens2, doc);
        //HelperFuncs.writeToSiteErrors("CSXI tblHtml", tblHtml);

        #region Scrape result row

        // Scrape result row 

        tokens2[0] = "<tr class=\"columnBasedResultsRow";
        tokens2[1] = ">";
        tokens2[2] = "</tr>";
        string columnBasedResultsRow = HelperFuncs.scrapeFromPage(tokens2, tblHtml);
        //HelperFuncs.writeToSiteErrors("CSXI columnBasedResultsRow", columnBasedResultsRow);

        if (columnBasedResultsRow.Contains("No Capacity"))
        {
            HelperFuncs.writeToSiteErrors("CSXI no capacity", "CSXI no capacity");
            hasCapacity = false;
            if (!isLastDateTry)
            {
                //HelperFuncs.writeToSiteErrors("CSXI not LastDateTry", "CSXI not LastDateTry");
                return;
            }
            else
            {
                //HelperFuncs.writeToSiteErrors("CSXI LastDateTry", "CSXI LastDateTry");
            }
        }

        tokens2[0] = "<span";  // style=\\\"vertical-align: top;
        tokens2[1] = ">";
        tokens2[2] = "</span>";
        price = HelperFuncs.scrapeFromPage(tokens2, columnBasedResultsRow)
            .Replace("&nbsp;", "").Replace("$", "").Replace(",", "").Trim();
        //HelperFuncs.writeToSiteErrors("CSXI costStr", price);

        decimal testDecimal;
        if (decimal.TryParse(price, out testDecimal))
        {
            successBool = true;
        }

        // Get transit time

        string[] tokens3 = new string[5];
        tokens3[0] = "<td";
        tokens3[1] = "<td";
        tokens3[2] = "<td";
        tokens3[3] = ">";
        tokens3[4] = "</td>";
        transitTime = HelperFuncs.scrapeFromPage(tokens3, columnBasedResultsRow).Replace(",", "").Replace("days", "")
            .Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();
        //HelperFuncs.writeToSiteErrors("CSXI transitTime", transitTime);

        #endregion

        #region Scrape alt result row

        // Scrape alt result row 

        //tokens2[0] = "<tr class=\"columnBasedResultsRowAlt";
        //tokens2[1] = ">";
        //tokens2[2] = "</tr>";
        //string columnBasedResultsRowAlt = HelperFuncs.scrapeFromPage(tokens2, tblHtml);
        //HelperFuncs.writeToSiteErrors("CSXI columnBasedResultsRowAlt", columnBasedResultsRowAlt);

        #endregion

    }

    #endregion
   
    #endregion
}