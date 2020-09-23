#region Using

using gcmAPI.Models.LTL;
using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

#endregion

namespace gcmAPI.Models.Carriers
{
    public class CTII
    {
        #region Variables

        CarrierAcctInfo acctInfo;
        QuoteData quoteData;

        #endregion

        #region Constructors

        public CTII(ref CarrierAcctInfo acctInfo, ref QuoteData quoteData)
        {
            this.quoteData = quoteData;
            this.acctInfo = acctInfo;
        }

        public CTII()
        {          
        }

        #endregion

        #region Get_CTII_rate

        //public void Get_CTII_rate(out GCMRateQuote ctiiQuote)
        //{
        //    ctiiQuote = new GCMRateQuote();

        //    Web_client http = new Web_client();
        //    try
        //    {
        //        #region Login

        //        http.url = "https://mycentral.goctii.com/";
        //        http.referrer = "";
        //        http.method = "GET";
        //        http.accept =
        //            "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
        //        string doc = http.Make_http_request();

        //        string[] viewEvent = HelperFuncs.getViewAndEvent(doc);

        //        string[] toks = new string[4];
        //        toks[0] = "VIEWSTATEGENERATOR";
        //        toks[1] = "value=";
        //        toks[2] = "\"";
        //        toks[3] = "\"";
        //        string VIEWSTATEGENERATOR = HelperFuncs.scrapeFromPage(toks, doc);

        //        //--

        //        http.referrer = http.url;
        //        http.method = "POST";
        //        http.url = "https://mycentral.goctii.com/login.aspx";

        //        http.post_data = string.Concat("__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE=", viewEvent[0],
        //            //"%2FwEPDwULLTE2MjUwMTMwOTYPZBYCZg9kFgICAw9kFgQCAw9kFgQCBw9kFgICBw8PFgIeB0VuYWJsZWRoZGQCCQ9kFgICCw8QZGQWAWZkAgUPDxYCHgRUZXh0BQQyMDIwZGRkSGRcK3GjMgnjsU6jcifDuZm0Sc8%3D",
        //            "&__VIEWSTATEGENERATOR=", VIEWSTATEGENERATOR,
        //            //"C2EE9ABB",
        //            "&__EVENTVALIDATION=", viewEvent[1],
        //            //"%2FwEWBAKdl578DgL8vovzAgLxp5vLAgLXx47%2BBk1fZ4b%2BFgb5hO9O09e7OJ6cpqSw",
        //            "&ctl00%24MainContentPlaceHolder%24txtUserID=", acctInfo.username.Replace("@", "%40"),
        //            //"",
        //            "&ctl00%24MainContentPlaceHolder%24txtPassword=", acctInfo.password,
        //            //"",
        //            "&ctl00%24MainContentPlaceHolder%24btLogin=Login");
        //        doc = http.Make_http_request();

        //        //--

        //        http.method = "GET";
        //        http.url = "https://mycentral.goctii.com/Default.aspx";
        //        doc = http.Make_http_request();

        //        viewEvent = HelperFuncs.getViewAndEvent(doc);
        //        VIEWSTATEGENERATOR = HelperFuncs.scrapeFromPage(toks, doc);

        //        #endregion

        //        #region Make sure account is still active(in select)

        //        toks[0] = "<option value=\"";
        //        toks[1] = "value=";
        //        toks[2] = "\"";
        //        toks[3] = "\"";
        //        string account_number = HelperFuncs.scrapeFromPage(toks, doc);

        //        if (account_number == "")
        //        {
        //            // Do nothing
        //        }
        //        else
        //        {
        //            return;
        //        }

        //        #endregion

        //        #region Switch to the DLS Genera Account

        //        http.method = "POST";
        //        http.post_data = string.Concat("__EVENTTARGET=ctl00%24MainContentPlaceHolder%24dCustomers&__EVENTARGUMENT=",
        //            "&__LASTFOCUS=&__VIEWSTATE=", viewEvent[0],
        //            //"%2FwEPDwUKLTMwOTIyOTk4Ng9kFgJmD2QWAgIDD2QWBAIFD2QWAgIDD2QWBgIPDw8WAh4EVGV4dAUObHRsQGRscy13dy5jb21kZAITDw8WAh8ABQ1ETFMgV09STERXSURFZGQCFQ9kFgICAQ8QDxYGHg1EYXRhVGV4dEZpZWxkBQxDdXN0b21lck5hbWUeDkRhdGFWYWx1ZUZpZWxkBQtDdXN0b21lcktleR4LXyFEYXRhQm91bmRnZBAVAg1ETFMgV09STERXSURFFkdFTkVSQSAlIERMUyBXT1JMRFdJREUVAggxMDE5MzE4NAgxMDI4MTMyMRQrAwJnZ2RkAgcPDxYCHwAFBDIwMjBkZGQXgS3bpAU8COtWGiIVqq2%2FJ2XmDg%3D%3D",
        //            "&__VIEWSTATEGENERATOR=CA0B0334",
        //            "&__EVENTVALIDATION=", viewEvent[1],
        //            //"%2FwEWBQLM%2BO2bCQK8z6uaDwL%2F%2B7njCALm1bfNAgKrro%2B3Duk24Rn06k6N7igtqA6UYLLGKklw",
        //            "&ctl00%24MainContentPlaceHolder%24dCustomers=",
        //            "10281321",
        //            "&ctl00%24MainContentPlaceHolder%24TextBox1=");
        //        doc = http.Make_http_request();

        //        #endregion

        //        #region Go to rating page

        //        http.url = "https://mycentral.goctii.com/RateQuote/RateQuote.aspx";
        //        http.method = "GET";
        //        doc = http.Make_http_request();

        //        viewEvent = HelperFuncs.getViewAndEvent(doc);
        //        //VIEWSTATEGENERATOR = HelperFuncs.scrapeFromPage(toks, doc);

        //        #endregion

        //        #region Class and weight

        //        int ind;
        //        string key = "", value = "";

        //        List<KeyValuePair<string, string>> classes = new List<KeyValuePair<string, string>>();
        //        ind = doc.IndexOf("id=\"ctl00_MainContentPlaceHolder_cboClass2");
        //        if (ind == -1)
        //            throw new Exception("couldn't scrape classes drop down");
        //        doc = doc.Substring(ind);

        //        ind = doc.IndexOf("</select>");
        //        if (ind == -1)
        //            throw new Exception("couldn't scrape classes drop down");
        //        doc = doc.Remove(ind);
        //        while (doc.IndexOf("value=") != -1)
        //        {
        //            ind = doc.IndexOf("value=");
        //            doc = doc.Substring(ind);
        //            ind = doc.IndexOf("\"");
        //            doc = doc.Substring(ind + 1);
        //            ind = doc.IndexOf("\"");
        //            key = doc.Remove(ind);

        //            ind = doc.IndexOf(">");
        //            doc = doc.Substring(ind + 1);

        //            ind = doc.IndexOf("<");
        //            value = doc.Remove(ind);
        //            KeyValuePair<string, string> pair = new KeyValuePair<string, string>(key, value);
        //            classes.Add(pair);
        //        }

        //        string[] classesForPost = new string[4];
        //        bool found = false;

        //        for (int i = 0; i < quoteData.m_lPiece.Length; i++)
        //        {
        //            found = false;
        //            foreach (KeyValuePair<string, string> pair in classes)
        //            {
        //                if (pair.Value == quoteData.m_lPiece[i].FreightClass)
        //                {
        //                    classesForPost[i] = pair.Key;
        //                    found = true;
        //                    break;
        //                }
        //            }
        //            if (!found)
        //                throw new Exception("couldn't match class: " + quoteData.m_lPiece[i].FreightClass + " to ctii classes");
        //        }

        //        string[] weights = new string[4];

        //        for (int i = 0; i < quoteData.m_lPiece.Length; i++)
        //        {
        //            weights[i] = quoteData.m_lPiece[i].Weight.ToString();
        //        }

        //        for (int i = quoteData.m_lPiece.Length; i < 4; i++)
        //        {
        //            weights[i] = "";
        //            classesForPost[i] = "";
        //        }

        //        #endregion

        //        #region Accessorials

        //        string accessor_str = "";
        //        if (quoteData.AccessorialsObj.LGPU || quoteData.AccessorialsObj.LGDEL)   //Liftgate Pickup
        //        {
        //            accessor_str += "%7E10018";
        //        }
        //        else if (quoteData.AccessorialsObj.RESPU || quoteData.AccessorialsObj.RESDEL) //Residential Pickup
        //        {
        //            accessor_str += "%7E10007";
        //        }
        //        else if (quoteData.AccessorialsObj.INSDEL)  //Inside Delivery
        //        {
        //            accessor_str += "%7E10002";
        //        }
        //        else if (quoteData.AccessorialsObj.APTDEL || quoteData.AccessorialsObj.APTPU)  //Delivery Notification
        //        {
        //            accessor_str += "%7E10009";
        //        }
        //        else if (quoteData.isHazmat)  //Hazerdous Materials    
        //        {
        //            accessor_str += "%7E10005";
        //        }
        //        //else if (accessorial == "CONPU")
        //        //{
        //        //    throw new Exception("Rate Estimator does not accept the Construction Pickup accessorial");
        //        //}
        //        //else if (accessorial == "CONDEL")
        //        //{
        //        //    throw new Exception("Rate Estimator does not accept the Construction Delivery accessorial");
        //        //}
        //        //            <option value="10009">APPOINTMENT</option>
        //        //<option value="10148">FARM DELIVERY</option>
        //        //<option value="10005">HAZARDOUS MATERIALS</option>
        //        //<option value="10002">INSIDE DELIVERY</option>
        //        //<option value="10018">LIFT GATE</option>
        //        //<option value="10024">LIMITED ACCESS</option>
        //        //<option value="10001">NOTIFY</option>
        //        //<option value="10007">RESIDENTIAL PICKUP/DELIVERY</option>
        //        //<option value="10004">SINGLE SHIPMENT</option>
        //        //<option value="10016">SORT and SEG</option>

        //        #endregion

        //        #region Get a rate

        //        DateTime today = DateTime.Today;

        //        http.referrer = http.url;
        //        http.method = "POST";
        //        http.post_data = string.Concat("__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE=", viewEvent[0],
        //        //"%2FwEPDwULLTE0MTg3MTcxODQPFgYeEUlzQWdncmVnYXRlUGFsbGV0aB4PRm9yY2VkUmF0ZVF1b3RlaB4IaXNQYWxsZXRoFgJmD2QWAgIDD2QWCAIDDw8WAh4EVGV4dAUObHRsQGRscy13dy5jb21kZAIFDw8WAh8DBRZHRU5FUkEgJSBETFMgV09STERXSURFZGQCBg9kFj4CAQ8PFgIfA2VkZAIJDxAPZBYCHghvbkNoYW5nZQUMR2V0TWVzc2FnZSgpZGRkAgsPD2QWAh8EBQxHZXRNZXNzYWdlKClkAg8PDxYCHg5WYWx1ZVRvQ29tcGFyZQUJMi8xMi8yMDIwZGQCEQ8WAh4Hb25jbGljawU7T3BlbkNhbGVuZGFyKCdjdGwwMF9NYWluQ29udGVudFBsYWNlSG9sZGVyX3Vzcl9QaWNrdXBEYXRlJylkAhgPDxYCHwMFVlBsZWFzZSBjYWxsIDx1PlZvbHVtZSBRdW90ZXM8L3U%2BIGZvciBzaGlwbWVudHMgZXhjZWVkaW5nIDE5LDk5OSBsYnMgYXQgKDU4NikgNDY3LTE5MDAuZGQCGg8PZBYCHgdvbktleVVwBQxHZXRNZXNzYWdlKClkAicPD2QWAh8HBQxHZXRNZXNzYWdlKClkAjQPDxYCHwMFBUNsYXNzZGQCNg8PFgIfAwUQUmF0ZSBwZXIgMTAwIGxic2RkAjoPEA8WBh4NRGF0YVRleHRGaWVsZAUETmFtZR4ORGF0YVZhbHVlRmllbGQFBXZhbHVlHgtfIURhdGFCb3VuZGcWAh8EBQxHZXRNZXNzYWdlKCkQFRMAAjUwAjU1AjYwAjY1AjcwBDc3LjUCODUEOTIuNQMxMDADMTEwAzEyNQMxNTADMTc1AzIwMAMyNTADMzAwAzQwMAM1MDAVEwABMQEyATMBNAE1ATYBNwE4ATkCMTACMTECMTICMTMCMTQCMTUCMTYCMTcCMTgUKwMTZ2dnZ2dnZ2dnZ2dnZ2dnZ2dnZ2RkAjwPD2QWBB8EBQ1TaG93RGV0YWlscygpHwcFDEdldE1lc3NhZ2UoKWQCQA8PFgIeDEVycm9yTWVzc2FnZQUOQ2xhc3MgUmVxdWlyZWRkZAJGDw9kFgIfBwUMR2V0TWVzc2FnZSgpZAJIDw8WBB8LBTBXZWlnaHQgMSBjYW5ub3QgYmUgZ3JlYXRlciB0aGFuIDE5LDk5OSBsYnM8YnIgLz4eDE1heGltdW1WYWx1ZQUFMTk5OTlkZAJSDxAPFgYfCAUETmFtZR8JBQV2YWx1ZR8KZxYCHwQFDEdldE1lc3NhZ2UoKRAVEwACNTACNTUCNjACNjUCNzAENzcuNQI4NQQ5Mi41AzEwMAMxMTADMTI1AzE1MAMxNzUDMjAwAzI1MAMzMDADNDAwAzUwMBUTAAExATIBMwE0ATUBNgE3ATgBOQIxMAIxMQIxMgIxMwIxNAIxNQIxNgIxNwIxOBQrAxNnZ2dnZ2dnZ2dnZ2dnZ2dnZ2dnZGQCVA8PZBYCHwcFDEdldE1lc3NhZ2UoKWQCVg8PFgQfCwUwV2VpZ2h0IDIgY2Fubm90IGJlIGdyZWF0ZXIgdGhhbiAxOSw5OTkgbGJzPGJyIC8%2BHwwFBTE5OTk5ZGQCXg8QDxYGHwgFBE5hbWUfCQUFdmFsdWUfCmcWAh8EBQxHZXRNZXNzYWdlKCkQFRMAAjUwAjU1AjYwAjY1AjcwBDc3LjUCODUEOTIuNQMxMDADMTEwAzEyNQMxNTADMTc1AzIwMAMyNTADMzAwAzQwMAM1MDAVEwABMQEyATMBNAE1ATYBNwE4ATkCMTACMTECMTICMTMCMTQCMTUCMTYCMTcCMTgUKwMTZ2dnZ2dnZ2dnZ2dnZ2dnZ2dnZ2RkAmAPD2QWAh8HBQxHZXRNZXNzYWdlKClkAmIPDxYEHwsFMFdlaWdodCAzIGNhbm5vdCBiZSBncmVhdGVyIHRoYW4gMTksOTk5IGxiczxiciAvPh8MBQUxOTk5OWRkAmoPEA8WBh8IBQROYW1lHwkFBXZhbHVlHwpnFgIfBAUMR2V0TWVzc2FnZSgpEBUTAAI1MAI1NQI2MAI2NQI3MAQ3Ny41Ajg1BDkyLjUDMTAwAzExMAMxMjUDMTUwAzE3NQMyMDADMjUwAzMwMAM0MDADNTAwFRMAATEBMgEzATQBNQE2ATcBOAE5AjEwAjExAjEyAjEzAjE0AjE1AjE2AjE3AjE4FCsDE2dnZ2dnZ2dnZ2dnZ2dnZ2dnZ2dkZAJsDw9kFgIfBwUMR2V0TWVzc2FnZSgpZAJuDw8WBB8LBTBXZWlnaHQgNCBjYW5ub3QgYmUgZ3JlYXRlciB0aGFuIDE5LDk5OSBsYnM8YnIgLz4fDAUFMTk5OTlkZAKmAQ8WAh8GBRNPcGVuQWNjZXNzb3JpZXMoJycpZALmAQ9kFgICAw8QD2QWAh4Hb25DbGljawVGZ2V0UmFkaW9CdXR0b25MaXN0U2VsZWN0aW9uKCdjdGwwMF9NYWluQ29udGVudFBsYWNlSG9sZGVyX3JkUXVlc3Rpb24nKWQWAQIBZAKAAg8QDxYGHwgFD0N1c3RvbWVyTG9jTmFtZR8JBQ5DdXN0b21lckxvY0tleR8KZ2QQFQYPR0VORVJBIERDIC1FQVNUFUdFTkVSQSBEQyAtIFNPVVRIRUFTVBZHRU5FUkEgJSBETFMgV09STERXSURFE0dFTkVSQSBEQyAtIE1JRFdFU1QTR0VORVJBIERDIC0gQ0VOVFJBTBBHRU5FUkEgREMgLSBXRVNUFQYIMTA5NzgzNjIIMTE4MjM2MDgIMTUwMzE2MTMIMTA5NzA4MjYIMTI4NDEyMDMIMTQ1MzI3MzEUKwMGZ2dnZ2dnFgFmZAKCAg8PFgIeB1Zpc2libGVoZGQChAIPDxYCHw5oZGQChgIPDxYCHw5oZGQCiAIPDxYCHwNlZGQCBw8PFgIfAwUEMjAyMGRkZPJJXw81M%2Ba68TiF6cJmFXX61VoI",
        //        "&__VIEWSTATEGENERATOR=", VIEWSTATEGENERATOR,
        //        //59F1FB91
        //        "&__EVENTVALIDATION=", viewEvent[1],
        //        //"%2FwEWXQK2qZaCDQKN6PGiDQKS6PGiDQKT6PGiDQL%2FtcP7AwKLw6TOBgLGpqnsAwKO3fCsBgKaiv7NDgKV5dSjAgKU5dSjAgKX5dSjAgKW5dSjAgKR5dSjAgKQ5dSjAgKT5dSjAgKC5dSjAgKN5dSjAgKV5ZSgAgKV5ZigAgKV5ZygAgKV5aCgAgKV5aSgAgKV5aigAgKV5aygAgKV5bCgAgKV5fSjAgLQp8r9DwKaiorODgKV5aCgAgKU5aCgAgKX5aCgAgKW5aCgAgKR5aCgAgKQ5aCgAgKT5aCgAgKC5aCgAgKN5aCgAgKV5eCjAgKV5eyjAgKV5eijAgKV5dSjAgKV5dCjAgKV5dyjAgKV5dijAgKV5cSjAgKV5YCgAgLrkOiSCgKaiobODgKV5aygAgKU5aygAgKX5aygAgKW5aygAgKR5aygAgKQ5aygAgKT5aygAgKC5aygAgKN5aygAgKV5eyjAgKV5eCjAgKV5eSjAgKV5dijAgKV5dyjAgKV5dCjAgKV5dSjAgKV5cijAgKV5YygAgKG%2BoWoBAKaivLNDgKV5dijAgKU5dijAgKX5dijAgKW5dijAgKR5dijAgKQ5dijAgKT5dijAgKC5dijAgKN5dijAgKV5ZigAgKV5ZSgAgKV5ZCgAgKV5aygAgKV5aigAgKV5aSgAgKV5aCgAgKV5bygAgKV5fijAgLJmbWTDQKBi6lLAvTlh5MPAseJrcUIAte4v88IAqOdoJIIJ4oB46n5NZxdYKBIQadF7qpOySc%3D",
        //        "&ctl00%24MainContentPlaceHolder%24dlCustomerType=1",
        //        "&ctl00%24MainContentPlaceHolder%24usr_PickupDate=", today.Month, "%2F", today.Day, "%2F", today.Year,
        //        //2%2F12%2F2020",
        //        "&ctl00%24MainContentPlaceHolder%24txtOrigin=", quoteData.origZip,
        //        "&ctl00%24MainContentPlaceHolder%24txtDestination=", quoteData.destZip,
        //        "&ctl00%24MainContentPlaceHolder%24cboClass1=", classesForPost[0],
        //        "&ctl00%24MainContentPlaceHolder%24txtWeight1=", weights[0],
        //        "&ctl00%24MainContentPlaceHolder%24cboClass2=", classesForPost[1],
        //        "&ctl00%24MainContentPlaceHolder%24txtWeight2=", weights[1],
        //        "&ctl00%24MainContentPlaceHolder%24cboClass3=", classesForPost[2],
        //        "&ctl00%24MainContentPlaceHolder%24txtWeight3=", weights[2],
        //        "&ctl00%24MainContentPlaceHolder%24cboClass4=", classesForPost[3],
        //        "&ctl00%24MainContentPlaceHolder%24txtWeight4=", weights[3],

        //        "&ctl00%24MainContentPlaceHolder%24Button1=Get+Rate",
        //        "&ctl00%24MainContentPlaceHolder%24txtJavaScriptReturn=", accessor_str,
        //        "&ctl00%24MainContentPlaceHolder%24txtSubmit=0");
        //        doc = http.Make_http_request();

        //        #endregion

        //        #region Get cost and transit time

        //        double dbl;

        //        toks = new string[4];
        //        toks[0] = "id=\"ctl00_MainContentPlaceHolder_lbService\"";
        //        toks[1] = "\""; //filling one index on purpose, not to declare another array
        //        toks[2] = ">";
        //        toks[3] = "<";

        //        // transit time
        //        string transit_time = HelperFuncs.scrapeFromPage(toks, doc).Replace("&nbsp;", "").Trim();
        //        if (!double.TryParse(transit_time, out dbl))
        //        {
        //            transit_time = "-3";
        //        }

        //        ctiiQuote.DeliveryDays = Convert.ToInt32(dbl);

        //        toks[0] = "<b>Total:</b>";
        //        toks[1] = "<span";
        //        toks[2] = ">";
        //        toks[3] = "<";

        //        // cost
        //        string cost = HelperFuncs.scrapeFromPage(toks, doc).Replace("&nbsp;", "").Replace("$", "").Replace("US", "").Replace(",", "").Trim();
        //        if (!double.TryParse(cost, out dbl))
        //        {
        //            throw new Exception("Cost not parsed to double. Response: " + doc);
        //        }

        //        ctiiQuote.TotalPrice = dbl;

        //        ctiiQuote.DisplayName = acctInfo.displayName;
        //        ctiiQuote.BookingKey = acctInfo.bookingKey;
        //        ctiiQuote.CarrierKey = acctInfo.carrierKey;

        //        #endregion

        //    }
        //    catch (Exception e)
        //    {
        //        string str = e.ToString();
        //    }

        //}

        #endregion

        #region Add_result_to_array

        public void Add_result_to_array(ref GCMRateQuote ctiiQuote, ref GCMRateQuote[] totalQuotes)
        {
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, ctiiQuote);
        }

        #endregion

    }
}