#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using gcmAPI.Models.Utilities;

#endregion

public class LOUP : SharedRail.ICarrier
{
    #region Fields

    public string username, password, origZip, destZip, origCity, destCity;
    CookieContainer container;
    public bool isHazMat;
    IntermodalRater.railResult railResultObj;
    List<SharedRail.Result> streamlineResults;
    List<string> originRamps, destinationRamps;
   
    #endregion

    #region Constructor

    public LOUP(SharedRail.Parameters parameters)
    {
        username = parameters.username;
        password = parameters.password;
        origZip = parameters.origZip;
        destZip = parameters.destZip;
        origCity = parameters.origCity;
        destCity = parameters.destCity;
        isHazMat = parameters.isHazMat;
    }

    #endregion

    #region Interface implementation

    #region getRate

    public IntermodalRater.railResult getRate()
    {
        try
        {
            HelperFuncs.writeToSiteErrors("LOUP new function", "LOUP new function");

            Random random = new Random();
            int randInt = random.Next(999); // Max value
            string randStr = "1543" + randInt.ToString();
            randInt = random.Next(999999); // Max value
            randStr += randInt.ToString();

            Web_client http = new Web_client();

            http.method = "GET";
            http.url = string.Concat("http://www.shipstreamline.com/");
            http.referrer = "";
            http.accept = "text/html, application/xhtml+xml, */*";

            string doc = http.Make_http_request();

            // Set headers
            http.header_names = new string[1];
            http.header_names[0] = "X-Requested-With";
            http.header_values = new string[1];
            http.header_values[0] = "XMLHttpRequest";

            http.referrer = "https://www.shipstreamline.com/";
            http.url = string.Concat(
                "http://www.shipstreamline.com/customers/apps/news/list-json.cfm?_=", randStr);
            http.method = "GET";
            http.accept = "text/plain, */*; q=0.01";
            http.content_type = "";
            doc = http.Make_http_request();

            // Empty headers
            http.header_names = new string[0];
            http.header_values = new string[0];

            http.referrer = "https://www.shipstreamline.com/";
            http.url = "https://www.shipstreamline.com/admin/login.fcc";
            http.accept = "text/html, application/xhtml+xml, */*";
            http.method = "POST";
            http.content_type = "application/x-www-form-urlencoded";
            http.post_data = "target=%2Fstm%2Fresponsive%2Fsecure%2Flogin.shtml&USER=" + username + "&PASSWORD=" + password +
                    "&submitBtn=Login";
           
            //http http = new http(info);
            doc = http.Make_http_request();

            // Get_LOUP_random
            Get_LOUP_random(ref random, ref randInt, ref randStr);

            // Set headers
            http.header_names = new string[1];
            http.header_names[0] = "X-Requested-With";
            http.header_values = new string[1];
            http.header_values[0] = "XMLHttpRequest";

            http.referrer = "https://www.shipstreamline.com/";
            http.url = string.Concat(
                "https://www.shipstreamline.com/admin/tools/clear-count.shtml?_=", randStr);
            http.method = "GET";
            http.accept = "*/*";
            http.content_type = "";
            doc = http.Make_http_request();

            //

            // Empty headers
            http.header_names = new string[0];
            http.header_values = new string[0];

            http.url =
                "https://www.shipstreamline.com/stm/redirect/quoteline.shtml?track=mini-app-more";
            http.accept = "text/html, application/xhtml+xml, */*";
            doc = http.Make_http_request();

            http.referrer = http.url;
            http.url =
                "https://www.shipstreamline.com/customers/quoteline/secure/index.shtml?track=mini-app-more";

            doc = http.Make_http_request();

            // Get_LOUP_random
            Get_LOUP_random(ref random, ref randInt, ref randStr);

            //

            // Set headers
            http.header_names = new string[1];
            http.header_names[0] = "X-Requested-With";
            http.header_values = new string[1];
            http.header_values[0] = "XMLHttpRequest";

            http.referrer = http.url;
            http.url = string.Concat(
                "https://www.shipstreamline.com/customers/quoteline/secure/i18n/locale_en-US.json?cb=", randStr);
            http.accept = "application/json, text/javascript, */*; q=0.01";
            doc = http.Make_http_request();

            // Get_LOUP_random
            Get_LOUP_random(ref random, ref randInt, ref randStr);

            // Empty headers
            http.header_names = new string[0];
            http.header_values = new string[0];

            http.url = string.Concat(
                "https://www.shipstreamline.com/pricing-services/secure/jas/api/user?cb=", randStr);
            http.accept = "application/json, text/plain, */*";
            doc = http.Make_http_request();

            //
            // Make missing requests

            http.url = "https://www.shipstreamline.com/customers/quoteline/secure/assets/spinner/spinner_32.gif";
            http.accept = "image/webp,image/apng,image/*,*/*;q=0.8";
            doc = http.Make_http_request();

            // Set headers
            http.header_names = new string[1];
            http.header_names[0] = "X-Requested-With";
            http.header_values = new string[1];
            http.header_values[0] = "XMLHttpRequest";

            http.url = string.Concat("https://www.shipstreamline.com/stm/ssi/ue-apps/components/mapping/location-data/can.min.json?d=2018-11-28");
            http.accept = "application/json, text/javascript, */*; q=0.01";
            doc = http.Make_http_request();

            http.url = string.Concat("https://www.shipstreamline.com/stm/ssi/ue-apps/components/mapping/location-data/usa.min.json?d=2018-11-28");
            doc = http.Make_http_request();

            http.url = string.Concat("https://www.shipstreamline.com/stm/ssi/ue-apps/components/mapping/location-data/mex.min.json?d=2018-11-28");
            doc = http.Make_http_request();

            http.url = string.Concat("https://www.shipstreamline.com/stm/ssi/ue-apps/components/mapping/location-data/usa-state-lines.min.json?d=2018-11-28");
            doc = http.Make_http_request();

            http.url = string.Concat("https://www.shipstreamline.com/stm/ssi/ue-apps/components/mapping/location-data/base-ramps.json?d=2018-11-28");
            doc = http.Make_http_request();


            //

            // Get_LOUP_random
            Get_LOUP_random(ref random, ref randInt, ref randStr);

            // Set headers
            http.header_names = new string[1];
            http.header_names[0] = "X-Requested-With";
            http.header_values = new string[1];
            http.header_values[0] = "XMLHttpRequest";

            http.url = string.Concat(
                "https://www.shipstreamline.com/customer-services/secure/jas/capacity-freight?_=", randStr);
            http.accept = "text/plain, */*; q=0.01";
            doc = http.Make_http_request();

            // Get_LOUP_random
            Get_LOUP_random(ref random, ref randInt, ref randStr);

            // Empty headers
            http.header_names = new string[0];
            http.header_values = new string[0];

            http.url =
                "https://www.shipstreamline.com/pricing-services/secure/jas/api/quote/specials";
            http.accept = "application/json, text/plain, */*";
            doc = http.Make_http_request();

            #region Locations requests

            // Set headers
            http.header_names = new string[1];
            http.header_names[0] = "X-Requested-With";
            http.header_values = new string[1];
            http.header_values[0] = "XMLHttpRequest";

            // Get locations
            http.url = string.Concat(
                "https://www.shipstreamline.com/pricing-services/secure/jas/api/location/search?term=1521");

            http.accept = "*/*";
            doc = http.Make_http_request();

            http.url = string.Concat(
                "https://www.shipstreamline.com/pricing-services/secure/jas/api/location/search?term=15214");
            doc = http.Make_http_request();

            http.url = string.Concat(
                "https://www.shipstreamline.com/pricing-services/secure/jas/api/location/search?term=9814");
            doc = http.Make_http_request();

            http.url = string.Concat(
                "https://www.shipstreamline.com/pricing-services/secure/jas/api/location/search?term=98144");
            doc = http.Make_http_request();

            #endregion

            // Empty headers
            http.header_names = new string[0];
            http.header_values = new string[0];

            // Get_LOUP_random
            Get_LOUP_random(ref random, ref randInt, ref randStr);

            HelperFuncs.writeToSiteErrors("LOUP before summary", "LOUP before summary");

            http.url = string.Concat(
               "https://www.shipstreamline.com/pricing-services/secure/jas/api/pricing/summary?cb=", randStr);
            http.accept = "application/json, text/plain, */*";
            http.method = "POST";
            http.content_type = "application/json;charset=utf-8";
            http.post_data = "{\"hazmatShipment\":false,\"beneficialOwner\":{\"name\":\"BCO_UNKNOWN\"},\"pickupDate\":\"20181128\",\"intermodalMoveType\":\"DOOR_TO_DOOR\",\"shipFromLocation\":{\"spotLiveType\":\"STAY\",\"locationId\":\"018413\",\"city\":\"PITTSBURGH\",\"state\":\"PA\",\"zipcode\":\"15214\",\"country\":\"US\",\"area\":\"PIT\",\"latitude\":40.481828,\"longitude\":-80.016002,\"timeZone\":\"ED\",\"services\":[]},\"shipToLocation\":{\"spotLiveType\":\"STAY\",\"locationId\":\"130145\",\"city\":\"SEATTLE\",\"state\":\"WA\",\"zipcode\":\"98144\",\"country\":\"US\",\"area\":\"SEA\",\"latitude\":47.58564,\"longitude\":-122.298,\"timeZone\":\"PD\",\"services\":[]},\"extraPickups\":[],\"extraDeliveries\":[]}";

            doc = http.Make_http_request();

            HelperFuncs.writeToSiteErrors("LOUP doc", doc);

            railResultObj = new IntermodalRater.railResult();
            railResultObj.success = "";
            return railResultObj;

        }
        catch (Exception e)
        {
            #region Catch

            HelperFuncs.writeToSiteErrors("LOUP", e.ToString());

            railResultObj = new IntermodalRater.railResult();
            railResultObj.success = "";
            return railResultObj;

            #endregion
        }

    }

    #endregion

    #region login

    public void login()
    {
    }

    #endregion

    #region getRate_Old

    public IntermodalRater.railResult getRate_Old()
    {
        try
        {
            //HelperFuncs.writeToSiteErrors("Streamline", "hit func");

            #region Variables

            //List<HelperFuncs.Credentials> crds = new List<HelperFuncs.Credentials>();
            //string username = "", password = "";
            //crds = HelperFuncs.GetLoginsByCarID(78573);
            //username = crds[0].username;
            //password = crds[0].password;

            container = new CookieContainer();
            login();
            string url = "", referrer, contentType, accept, method, doc = "", data = "";
            DateTime puDate = DateTime.Today.AddDays(1);
            string puDateDay = puDate.Day.ToString(), puDateMonth = puDate.Month.ToString();

            if (puDateDay.Length == 1)
                puDateDay = "0" + puDateDay;
            if (puDateMonth.Length == 1)
                puDateMonth = "0" + puDateMonth;

            #endregion

            referrer = "https://www.shipstreamline.com/";
            url = "https://www.shipstreamline.com/stm/redirect/quoteline.shtml";
            accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            contentType = "";
            method = "GET";
            doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");

            //--------------------------------------------------------------------------------------------------------------------

            referrer = url;
            url = "https://www.shipstreamline.com/customers/quoteline/secure/index.shtml";
            doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");

            //--------------------------------------------------------------------------------------------------------------------
            SharedRail.Location origLocation = new SharedRail.Location();
            getLocationInfo(ref origLocation, ref origZip, ref origCity, ref container, ref doc, referrer, url, accept, contentType);

            SharedRail.Location destLocation = new SharedRail.Location();
            getLocationInfo(ref destLocation, ref destZip, ref destCity, ref container, ref doc, referrer, url, accept, contentType);
            //--------------------------------------------------------------------------------------------------------------------

            // getLocationInfo_Streamline

            //

            string hazMat = "false";
            if (isHazMat.Equals(true))
            {
                hazMat = "true";
            }

            //

            Random random = new Random();
            int randInt = random.Next(999); // Max value
            string randStr = "1403" + randInt.ToString();
            randInt = random.Next(999999); // Max value
            randStr += randInt.ToString();

            referrer = url;
            url = "https://www.shipstreamline.com/pricing-services/secure/jas/api/pricing/summary?cb=" + randStr;
            accept = "application/json, text/plain, */*";
            contentType = "application/json;charset=utf-8";
            method = "POST";

            data = string.Concat("{\"hazmatShipment\":", hazMat, ",\"beneficialOwner\":{\"name\":\"AES\"},\"pickupDate\":\"", puDate.Year.ToString(),
                puDateMonth, puDateDay, "\",\"intermodalMoveType\":\"DOOR_TO_DOOR\",\"shipFromLocation\":{\"spotLiveType\":\"STAY\",",
                "\"locationId\":\"", origLocation.locationId, "\",\"city\":\"", origLocation.city, "\",\"state\":\"", origLocation.state,
                "\",\"zipcode\":\"", origLocation.zipCode, "\",\"country\":\"", origLocation.country, "\",\"area\":\"", origLocation.area, "\",",
                "\"latitude\":", origLocation.latitude, ",\"longitude\":", origLocation.longitude, ",\"timeZone\":\"", origLocation.timeZone,
                "\",\"services\":[]},\"shipToLocation\":{\"spotLiveType\":\"STAY\",",
                "\"locationId\":\"", destLocation.locationId, "\",\"city\":\"", destLocation.city, "\",\"state\":\"", destLocation.state,
                "\",\"zipcode\":\"", destLocation.zipCode, "\",\"country\":\"", destLocation.country, "\",\"area\":\"", destLocation.area, "\",",
                "\"latitude\":", destLocation.latitude, ",\"longitude\":", destLocation.longitude,
                ",\"timeZone\":\"", destLocation.timeZone, "\",\"services\":[]},\"extraPickups\":[],\"extraDeliveries\":[]}");

            string ratesDoc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, data, false, false, "", "");

            // Test
            HelperFuncs.writeToSiteErrors("Streamline ratesDoc", ratesDoc);

            //do not have capacity
            bool resHasNoCapacity = false;
            if (ratesDoc.Contains("do not have capacity"))
            {
                resHasNoCapacity = true;
            }

            streamlineResults = new List<SharedRail.Result>();
            //List<string> originRamps = new List<string>();
            //List<string> destinationRamps = new List<string>();
            int transitDays = -1;

            railResultObj = new IntermodalRater.railResult();

            if (resHasNoCapacity.Equals(false))
            {
                #region Has Capacity

                HelperFuncs.writeToSiteErrors("Streamline", "has capacity");
                originRamps = new List<string>();
                destinationRamps = new List<string>();

                scrapeResult(ref ratesDoc);

                HelperFuncs.writeToSiteErrors("Streamline", "after scrapeResult");

                // Find index of first result that has canBook = true
                int indexOfFirstCanBookResult = -1;
                for (int i = 0; i < streamlineResults.Count; i++)
                {
                    //HelperFuncs.writeToSiteErrors("Streamline", streamlineResults[i].totalPrice.ToString());
                    if (streamlineResults[i].canBook == true)
                    {
                        indexOfFirstCanBookResult = i;
                        break;
                    }
                }

                if (indexOfFirstCanBookResult == -1)
                {
                    //throw new Exception("no rate was found");
                }
                else
                {
                    #region Get Transit Time

                    // Need to make 2 or 3 more requests here to get the transit time
                    //--------------------------------------------------------------------------------------------------------------------
                    url = "https://www.shipstreamline.com/pricing-services/secure/jas/api/capacity";
                    data = "{\"quoteLineTransactionId\":" + streamlineResults[indexOfFirstCanBookResult].quoteLineTransactionId.Trim() + "}";
                    doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, data, false, false, "", "");
                    //--------------------------------------------------------------------------------------------------------------------

                    randInt = random.Next(999); // Max value
                    randStr = "1403" + randInt.ToString();
                    randInt = random.Next(999999); // Max value
                    randStr += randInt.ToString();

                    url = "https://www.shipstreamline.com/pricing-services/secure/jas/api/pricing/route?cb=" + randStr;
                    data = string.Concat("{\"hazmatShipment\":false,\"lane\":{\"originRamp\":\"", originRamps[indexOfFirstCanBookResult], "\",\"destinationRamp\":\"",
                        destinationRamps[indexOfFirstCanBookResult],
                        "\"},\"beneficialOwner\":{\"name\":\"aes\"},\"pickupDate\":\""
                        , puDate.Year.ToString(), puDateMonth, puDateDay,

                    "\",\"intermodalMoveType\":\"DOOR_TO_DOOR\",\"shipFromLocation\":{\"spotLiveType\":\"STAY\",\"locationId\":\"", origLocation.locationId,
                    "\",\"city\":\"", origLocation.city, "\",\"state\":\"", origLocation.state, "\"",
                    ",\"zipcode\":\"", origLocation.zipCode, "\",\"country\":\"", origLocation.country, "\",\"area\":\"", origLocation.area,
                    "\",\"latitude\":\"", origLocation.latitude, "\",\"longitude\":\"", origLocation.longitude, "\",\"timeZone\":\"", origLocation.timeZone,
                    "\",\"services\":[]},\"shipToLocation\"",

                    ":{\"spotLiveType\":\"STAY\",\"locationId\":\"", destLocation.locationId, "\",\"city\":\"", destLocation.city,
                    "\",\"state\":\"", destLocation.state, "\",\"zipcode\":\"", destLocation.zipCode, "\",\"country\":\"", destLocation.country,
                    "\",\"area\":\"", destLocation.area, "\"",
                    ",\"latitude\":\"", destLocation.latitude, "\",\"longitude\":\"", destLocation.longitude, "\",\"timeZone\":\"", destLocation.timeZone,
                    "\",\"services\":[]},\"extraPickups\":[],\"extraDeliveries\":[],\"",
                    "quoteLineTransactionId\":\"", streamlineResults[indexOfFirstCanBookResult].quoteLineTransactionId.Trim(),
                    "\",\"appointmentType\":\"APPOINTMENT_BETWEEN\",\"appointmentTimeStart\":\"0800\",\"appointmentTimeEnd\":\"1600\",\"",
                    "crossingMethodType\":\"NOT_APPLICABLE\"}");

                    doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, data, false, false, "", "");

                    //HelperFuncs.writeToSiteErrors("Streamline res", doc);

                    string[] tokens = new string[3];
                    tokens[0] = "transitDays";
                    tokens[1] = ":";
                    tokens[2] = ",";

                    doc = HelperFuncs.scrapeFromPage(tokens, doc);
                    //int transitDays;
                    if (!int.TryParse(doc, out transitDays))
                        transitDays = -1;

                    #endregion
                }

                //--------------------------------------------------------------------------------------------------------------------

                

                // Set the results
                HelperFuncs.writeToSiteErrors("Streamline", "before for each");
                foreach (SharedRail.Result streamRes in streamlineResults)
                {
                    //HelperFuncs.writeToSiteErrors("Streamline", "one result");
                    if (streamRes.canBook == true)
                    {
                        //HelperFuncs.writeToSiteErrors("Streamline", "can book");

                        //HelperFuncs.writeToSiteErrors("Streamline", "before railResultObj");
                        railResultObj.success = "success";
                        railResultObj.transitTime = transitDays.ToString();
                        railResultObj.rate = streamRes.totalPrice.ToString();
                        railResultObj.firstCapacityDate = streamRes.pickupDate;

                        if (transitDays > 0)
                        {
                            railResultObj.eta = streamRes.pickupDate.AddDays(transitDays);
                        }

                        railResultObj.hasCapacity = true;
                        railResultObj.containerSize = "FiftyThreeFt";
                        HelperFuncs.writeToSiteErrors("Streamline", "after railResultObj");

                        return railResultObj;
                    }
                }

                #endregion
            }

            #region Not used
            //if (!streamlineResultArray[0].Equals(SharedRail.success) && streamlineResults.Count > 0) // resHasNoCapacity.Equals(true) || (
            //{
            //    HelperFuncs.writeToSiteErrors("Streamline", "no capacity");

            //    railResultObj.success = "success";
            //    railResultObj.transitTime = transitDays.ToString();
            //    railResultObj.rate = string.Format("{0:0.00}", streamlineResults[0].totalPrice);
            //    railResultObj.firstCapacityDate = DateTime.MaxValue;

            //    if (transitDays > 0)
            //    {
            //        railResultObj.eta = DateTime.MaxValue;
            //    }

            //    railResultObj.hasCapacity = false;
            //    railResultObj.containerSize = "FiftyThreeFt";

            //    return;
            //}
            #endregion

            if (resHasNoCapacity.Equals(true))
            {
                #region No Capacity

                HelperFuncs.writeToSiteErrors("Streamline", "no capacity");

                //railResultObj.success = "success";
                railResultObj.success = "";
                railResultObj.transitTime = transitDays.ToString();
                //railResultObj.rate = string.Format("{0:0.00}", streamlineResults[0].totalPrice);
                railResultObj.rate = string.Format("{0:0.00}", 0);
                railResultObj.firstCapacityDate = DateTime.MaxValue;

                if (transitDays > 0)
                {
                    railResultObj.eta = DateTime.MaxValue;
                }

                railResultObj.hasCapacity = false;
                railResultObj.containerSize = "FiftyThreeFt";

                return railResultObj;

                #endregion
            }

            railResultObj.success = "";
            return railResultObj;
        }
        catch (Exception e)
        {
            #region Catch

            HelperFuncs.writeToSiteErrors("LOUP", e.ToString());

            railResultObj = new IntermodalRater.railResult();
            railResultObj.success = "";
            return railResultObj;

            #endregion
        }
    }

    #endregion

    #region scrapeResult

    public void scrapeResult(ref string doc)
    {
        string tmp, dateStr = "", costStr, canBookStr, year = "", month = "", day = "";
        int ind;
        SharedRail.Result result = new SharedRail.Result();

        tmp = doc;
        while (tmp.IndexOf("originRamp") != -1)
        {
            ind = tmp.IndexOf("originRamp");
            tmp = tmp.Substring(ind + 1);
            ind = tmp.IndexOf(":");
            tmp = tmp.Substring(ind + 1);
            ind = tmp.IndexOf("\"");
            tmp = tmp.Substring(ind + 1);
            ind = tmp.IndexOf("\"");
            originRamps.Add(tmp.Remove(ind));

            ind = tmp.IndexOf("destinationRamp");
            tmp = tmp.Substring(ind + 1);
            ind = tmp.IndexOf(":");
            tmp = tmp.Substring(ind + 1);
            ind = tmp.IndexOf("\"");
            tmp = tmp.Substring(ind + 1);
            ind = tmp.IndexOf("\"");
            destinationRamps.Add(tmp.Remove(ind));
        }

        while (doc.IndexOf("pickupDate") != -1)
        {
            ind = doc.IndexOf("pickupDate");
            doc = doc.Substring(ind + 1);
            ind = doc.IndexOf("pickupDate");
            if (ind != -1)
                tmp = doc.Remove(ind); // Remove other dates info
            else
                tmp = doc;

            ind = tmp.IndexOf(":");
            tmp = tmp.Substring(ind + 1);
            ind = tmp.IndexOf("\"");
            tmp = tmp.Substring(ind + 1);
            ind = tmp.IndexOf("\"");
            dateStr = tmp.Remove(ind);
            year = dateStr.Remove(4);
            dateStr = dateStr.Substring(4);
            month = dateStr.Remove(2);
            dateStr = dateStr.Substring(2);
            day = dateStr;

            if (!DateTime.TryParse(month + "/" + day + "/" + year, out result.pickupDate))
                continue;

            //--------------------------------------------------------------------------------

            ind = tmp.IndexOf("totalPrice");
            tmp = tmp.Substring(ind + 1);
            ind = tmp.IndexOf(":");
            tmp = tmp.Substring(ind + 1);
            ind = tmp.IndexOf("\"");
            tmp = tmp.Substring(ind + 1);
            ind = tmp.IndexOf("\"");
            costStr = tmp.Remove(ind).Replace("USD", "").Trim();
            if (!double.TryParse(costStr, out result.totalPrice))
                continue;

            //--------------------------------------------------------------------------------

            ind = tmp.IndexOf("canBook");
            tmp = tmp.Substring(ind + 1);
            ind = tmp.IndexOf(":");
            tmp = tmp.Substring(ind + 1);
            ind = tmp.IndexOf(",");
            canBookStr = tmp.Remove(ind).Trim();

            //canBookStr = "true"; // Ignore the can book
            if (!bool.TryParse(canBookStr, out result.canBook))
            {
                continue;
            }

            //--------------------------------------------------------------------------------

            ind = tmp.IndexOf("quoteLineTransactionId");
            tmp = tmp.Substring(ind + 1);
            ind = tmp.IndexOf(":");
            tmp = tmp.Substring(ind + 1);
            ind = tmp.IndexOf(",");
            canBookStr = tmp.Remove(ind).Trim();
            result.quoteLineTransactionId = tmp.Remove(ind);

            //--------------------------------------------------------------------------------

            //HelperFuncs.writeToSiteErrors("Streamline", "before streamlineResults");

            streamlineResults.Add(result);
        }
    }

    #endregion

    #endregion

    #region Special methods

    #region getLocationInfo

    private static void getLocationInfo(ref SharedRail.Location location, ref string zipCode, ref string city,
        ref CookieContainer container, ref string doc,
        string referrer, string url, string accept, string contentType)
    {
        referrer = url;
        accept = "*/*";
        contentType = "";
        url = "https://www.shipstreamline.com/pricing-services/secure/jas/api/location/search?term=" + zipCode;
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, "GET", "", false, false, "", "");

        int ind;
        if (doc.IndexOf(city) != -1)
        {
            ind = doc.IndexOf(city);
            doc = doc.Substring(ind - 50);
        }

        string[] tokens = new string[4];
        tokens[0] = "locationId";
        tokens[1] = "\"";
        tokens[2] = "\"";
        tokens[3] = "\"";

        location.locationId = HelperFuncs.scrapeFromPage(tokens, doc);

        tokens[0] = "city\"";
        location.city = HelperFuncs.scrapeFromPage(tokens, doc);

        tokens[0] = "state\"";
        location.state = HelperFuncs.scrapeFromPage(tokens, doc);

        tokens[0] = "zipcode\"";
        location.zipCode = HelperFuncs.scrapeFromPage(tokens, doc);

        tokens[0] = "country\"";
        location.country = HelperFuncs.scrapeFromPage(tokens, doc);

        tokens[0] = "area";
        location.area = HelperFuncs.scrapeFromPage(tokens, doc);

        tokens[0] = "timeZone";
        location.timeZone = HelperFuncs.scrapeFromPage(tokens, doc);

        tokens[2] = ":";
        tokens[3] = ",";

        tokens[0] = "latitude";
        location.latitude = HelperFuncs.scrapeFromPage(tokens, doc);

        tokens[0] = "longitude";
        location.longitude = HelperFuncs.scrapeFromPage(tokens, doc);
    }

    #endregion

    #region Get_LOUP_random

    private void Get_LOUP_random(ref Random random, ref int randInt, ref string randStr)
    {
        randInt = random.Next(999); // Max value
        randStr = "1543" + randInt.ToString();
        randInt = random.Next(999999); // Max value
        randStr += randInt.ToString();
    }

    #endregion

    #endregion
}