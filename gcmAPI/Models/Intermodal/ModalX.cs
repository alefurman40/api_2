#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

#endregion

public class ModalX : SharedRail.ICarrier
{
    #region fields

    public string username, password, origZip, destZip, origCity, destCity;
    CookieContainer container;
    public bool isHazMat;
    IntermodalRater.railResult railResultObj;

    #endregion

    #region Constructor

    public ModalX(SharedRail.Parameters parameters)
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

    #region interface implementation

    #region login

    public void login()
    {

    }

    #endregion

    #region getRate

    public IntermodalRater.railResult getRate()
    {

        container = new CookieContainer();

        double transitTime = 10;

        username = AppCodeConstants.modalX_un;
        password = AppCodeConstants.modalX_pwd;

        HelperFuncs.ModalX_Result res = new HelperFuncs.ModalX_Result();
        try
        {
            loginTo_ModalX(ref res);

            railResultObj = new IntermodalRater.railResult();

            if (res.success == true)
            {
                #region Not used
                //modalXResultArray[4] = "";

                //modalXResultArray[0] = SharedRail.success;
                //modalXResultArray[1] = SharedRail.ModalX;
                //modalXResultArray[2] = res.cost.ToString();

                //insertIntoRailLogs("ModalX", 92184, "1", "", "", Convert.ToDouble(res.cost));


                //modalXResultArray[3] = transitTime.ToString();
                //modalXResultArray[4] = res.pickupDate.ToShortDateString();
                //modalXResultArray[5] = res.deliveryDate.ToShortDateString();
                //modalXResultArray[6] = SharedRail.FiftyThreeFt;
                #endregion

                HelperFuncs.writeToSiteErrors("ModalX", "success");

                transitTime = (res.deliveryDate - res.pickupDate).TotalDays;


                railResultObj.success = "success";
                railResultObj.transitTime = transitTime.ToString();
                railResultObj.rate = res.cost.ToString();
                railResultObj.firstCapacityDate = res.pickupDate;

                if (transitTime > 0)
                {
                    railResultObj.eta = res.pickupDate.AddDays(transitTime);
                }

                railResultObj.hasCapacity = true;
                railResultObj.containerSize = "FiftyThreeFt";

                return railResultObj;
            }
            else
            {
                railResultObj.success = "";
                return railResultObj;
            }
        }
        catch (Exception e)
        {
            #region Catch

            HelperFuncs.writeToSiteErrors("ModalX", e.ToString());

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

    }

    #endregion

    #endregion

    #region Special methods

    #region loginTo_ModalX

    private void loginTo_ModalX(ref HelperFuncs.ModalX_Result res)
    {
        #region Variables

        DateTime now = DateTime.Now;
        DateTime today = DateTime.Today;
        DateTime tomorrow = DateTime.Today.AddDays(3);

        Int64 timeStampNow = SharedRail.GetTime(ref now);
        Int64 timeStampToday = SharedRail.GetTime(ref today);
        Int64 timeStampTomorrow = SharedRail.GetTime(ref tomorrow);

        string url = "", referrer, contentType, accept, method, doc = "", data = "";

        #endregion

        #region Go to home page

        url = "https://www.modal-x.com/modalx/";
        referrer = "";
        contentType = "";
        accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        method = "GET";
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");

        #endregion

        #region 2 requests with username and password

        referrer = url;
        url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/TDIS/users/" + username + "/login?_dc=" + timeStampNow.ToString();

        //HelperFuncs.writeToSiteErrors("ModalX", url);
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

        //HelperFuncs.writeToSiteErrors("ModalX", url);
        //HelperFuncs.writeToSiteErrors("ModalX", data);
        #endregion

        #region Some required requests
        //--------------------------------------------------------------------------------------------------------------------

        timeStampNow = SharedRail.GetTime(ref now);

        url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/TDIS/users/" + username + "/login?_dc=" + timeStampNow.ToString();
        contentType = "";
        accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        method = "GET";
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");
        // Get user info here
        SharedRail.ModalX_PurchasedBy purchasedBy = new SharedRail.ModalX_PurchasedBy();
        getPurchasedInfo_ModalX(ref purchasedBy, ref doc);
        //--------------------------------------------------------------------------------------------------------------------

        timeStampNow = SharedRail.GetTime(ref now);

        url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/app?_dc=" + timeStampNow.ToString();
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");

        //--------------------------------------------------------------------------------------------------------------------

        timeStampNow = SharedRail.GetTime(ref now);

        url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/app?_dc=" + timeStampNow.ToString();
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, "", false, false, "", "");

        //--------------------------------------------------------------------------------------------------------------------

        timeStampNow = SharedRail.GetTime(ref now);

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

        SharedRail.RailResult railResult = new SharedRail.RailResult();
        tryDate_ModalX(ref railResult, ref tomorrow, ref timeStampTomorrow, ref referrer, ref timeStampNow, ref now,
            ref purchasedBy, ref res);

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
            res.pickupDate = SharedRail.getDateFromJsGetTimeValue(jsDateTime);
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
            res.deliveryDate = SharedRail.getDateFromJsGetTimeValue(jsDateTime);
        }

        //--

        tokens[0] = "totalCharge";
        tmp = HelperFuncs.scrapeFromPage(tokens, doc);
        decimal.TryParse(tmp, out res.cost);

    }

    #endregion

    #region tryDate_ModalX

    private void tryDate_ModalX(ref SharedRail.RailResult railResult, ref DateTime date, ref Int64 dateStamp, ref string referrer,
        ref Int64 timeStampNow, ref DateTime now, ref SharedRail.ModalX_PurchasedBy purchasedBy,
        ref HelperFuncs.ModalX_Result res)
    {

        string url = "", contentType, accept, method = "POST", doc = "", data = "";

        // This request throws an exception, but.. this is correct. Only there are no rates for these zip codes (on the weekend)
        url = "https://www.modal-x.com/tw-services/modalx/v1.0.0/TDIS/rates/search";
        contentType = "application/json";
        data = "{\"origZip\":\"" + origZip + "\",\"destZip\":\"" + destZip + "\",\"pickupDate\":" + dateStamp.ToString() +
                    ",\"origCountry\":\"USA\",\"destCountry\":\"USA\"}";
        accept = "*/*";
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, data, false, false, "", "");

        SharedRail.ModalX_Lane mXlane = new SharedRail.ModalX_Lane();
        SharedRail.ModalX_Date mXdate = new SharedRail.ModalX_Date();
        getLanes_ModalX(ref mXlane, ref mXdate, ref doc);
        //--------------------------------------------------------------------------------------------------------------------

        timeStampNow = SharedRail.GetTime(ref now);

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

        DateTime scrapedPuDate = SharedRail.getDateFromJsGetTimeValue(Convert.ToInt64(mXlane.modalX_Date.date));
        Int64 scrapedPuDateStamp = SharedRail.GetTime(ref scrapedPuDate);

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
        //HelperFuncs.writeToSiteErrors("ModalX data", data);
        doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, data, false, false, "", "");
        //HelperFuncs.writeToSiteErrors("ModalX doc", doc);
        getResult_ModalX(ref doc, ref res);

    }

    #endregion

    #region getLanes_ModalX

    private static void getLanes_ModalX(ref SharedRail.ModalX_Lane mXlane, ref SharedRail.ModalX_Date mXdate, ref string doc)
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
                    availDate = SharedRail.getDateFromJsGetTimeValue(jsDateStamp);

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

                #region Not used
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
                #endregion
            }
            else
            {
                continue;
            }
        }

    }

    #endregion

    #region getPurchasedInfo_ModalX

    private static void getPurchasedInfo_ModalX(ref SharedRail.ModalX_PurchasedBy purchasedBy, ref string doc)
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

    #endregion
}