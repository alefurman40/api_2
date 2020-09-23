#region Using

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
//using System.Data.Odbc;
using System.Data.SqlClient;
using System.Text;
using System.Xml;
using System.Threading;
using System.Net;

#endregion

// API on ALEX

public class IntermodalRater
{

    #region Variables

    public static string aesmain_dataConnectionStringSS = AppCodeConstants.connStringAesData;
  
    public static string destZipGlobal = "";
    public static string originZipGlobal = "";
    public static string originCity, originState, originCountry, destCity, destState, destCountry;

    public static string puDateGlobalStr = "";
    public static string globalError = "";
    public static string usernameGlobal = "";
    public static int resultID = 0;

    public static DateTime puDateGlobal = new DateTime();
    public static DateTime validThrough = new DateTime();

    public static List<string[]> results = new List<string[]>();
    public static List<string[]> sortedResults = new List<string[]>();

    // New objects
    public static List<IntermodalResult> resultsList = new List<IntermodalResult>();

    public static string[] csxiResultArray;
  
    #endregion

    #region GetIntermodalRate
    
    public List<string[]> GetIntermodalRate(string username, string password, DateTime pickupDate, string originZip, string destinationZip, 
        string[] additionalServices, ref int QuoteID)
    {
        try
        {
            usernameGlobal = username;
            string user = GetGCMUserNameByAPILogins(username, password);
            int[] info = GetMarkupPercent(user);
            int IntermodalMarkupPercent = info[0];
            int IntermodalMinimum = info[1];

            destZipGlobal = destinationZip;
            originZipGlobal = originZip;

            //cityByZip();
            string[] cityStateCountryOrig = new string[3];
            string[] cityStateCountryDest = new string[3];

            SharedRail.cityStateCountryByZip(ref cityStateCountryOrig, originZip);
            SharedRail.cityStateCountryByZip(ref cityStateCountryDest, destinationZip);

            //originState = dr[0].ToString();
            //originCity = dr[1].ToString();
            //originCountry = dr[2].ToString();

            //destState = dr[0].ToString();
            //destCity = dr[1].ToString();
            //destCountry = dr[2].ToString();

            originCity = cityStateCountryOrig[0];
            destCity = cityStateCountryDest[0];
            originState = cityStateCountryOrig[1];
            destState = cityStateCountryDest[1];
            originCountry = cityStateCountryOrig[2];
            destCountry = cityStateCountryDest[2];


            #region puDate

            puDateGlobal = pickupDate;

            DateTime tomorrow = DateTime.Today.AddDays(1);

            int comparison;
            comparison = puDateGlobal.CompareTo(tomorrow);

            if (comparison == -1) // date is earlier than tomorrow, set the date to tomorrow
            {
                puDateGlobal = tomorrow;
            }
            if (puDateGlobal.DayOfWeek.ToString() == "Saturday")
            {
                puDateGlobal = puDateGlobal.AddDays(2);
            }
            else if (puDateGlobal.DayOfWeek.ToString() == "Sunday")
            {
                puDateGlobal = puDateGlobal.AddDays(1);
            }
            #endregion

            HelperFuncs.writeToSiteErrors("GetIntermodalRate Demo", string.Concat(originZip, " ", destinationZip, " ", originCity, " ", destCity));

            //int QuoteID = 0;
            // Insert into database
            SharedRail.saveResults(false, ref originCity, ref destCity, ref originState, ref destState, ref usernameGlobal, ref results, ref puDateGlobal,
                ref QuoteID);

            #region Threads

            List<Thread> threads = new List<Thread>();
            results.Clear();
            threads.Clear();

            SharedRail.streamlineResultArray = new string[13];
            SharedRail.csxiResultArray = new string[13];
            SharedRail.modalXResultArray = new string[13];
            SharedRail.integraResultArray = new string[13];
            SharedRail.integraResultArray40FT = new string[13];
            SharedRail.integraResultArray45FT = new string[13];
            SharedRail.mgOtrResultArray = new string[13];

            //SharedRail.MG_OTR_Result mgOtrRes = new SharedRail.MG_OTR_Result();

            SharedRail.synchronet20FT_DryResultArray = new string[13];
            SharedRail.synchronet40FT_High_CubeResultArray = new string[13];
            SharedRail.synchronet45FT_High_CubeResultArray = new string[13];

            

            // New objects
            //SharedRail.streamlineResults = new List<IntermodalResult>();

            // Initialize the notification field to avoid null reference exception when checking for function success
            SharedRail.streamlineResultArray[0] = "";
            SharedRail.csxiResultArray[0] = "";
            SharedRail.modalXResultArray[0] = "";
            SharedRail.integraResultArray[0] = "";
            SharedRail.mgOtrResultArray[0] = "";

            SharedRail.synchronet20FT_DryResultArray[0] = "";
            SharedRail.synchronet40FT_High_CubeResultArray[0] = "";
            SharedRail.synchronet45FT_High_CubeResultArray[0] = "";

            for (int i = 0; i < 8; i++)
            {
                Thread thread;
                //thread = new Thread(new ThreadStart(GetStreamlineInfo));
                if (i == 0)
                {
                    thread = new Thread(new ThreadStart(getRate_Streamline));
                    //continue;
                }
                else if (i == 1)
                {
                    //thread = new Thread(new ThreadStart(GetIntegraInfo));
                    //thread = new Thread(() => GetIntegraInfo(SharedRail.FiftyThreeFt));
                    continue;
                }
                else if (i == 2)
                {
                    thread = new Thread(new ThreadStart(GetModalX_Info));
                }
                else if (i.Equals(3))
                {
                    //thread = new Thread(new ThreadStart(getSynchronetInfo20));
                    continue;
                }
                else if (i.Equals(4))
                {
                    //thread = new Thread(new ThreadStart(getSynchronetInfo40));
                    continue;
                }
                else if (i.Equals(5))
                {
                    //thread = new Thread(new ThreadStart(getSynchronetInfo45));
                    continue;
                }
                else if (i.Equals(6))
                {
                    //thread = new Thread(new ThreadStart(getOTR_RateDLS));
                    continue;
                }
                //else if (i == 2)
                //{
                //    thread = new Thread(new ThreadStart(GetIDIInfo));
                //}

                else
                {
                    //thread = new Thread(new ThreadStart(GetCSXIInfo));
                    continue;
                }
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Name = i.ToString();
                threads.Add(thread);
            }


            foreach (Thread t in threads)
            {
                if (!t.Join(TimeSpan.FromSeconds(25)))
                {
                    t.Abort();
                }
            }

            GetIntegraInfo(SharedRail.FourtyFtHighCube);

            #endregion

            #region Gather all results to one results object

            decimal integra40cost = 0M, synchro40cost = 0M, integra45cost = 0M, synchro45cost = 0M;

            #region Get Synchro costs

            if (SharedRail.synchronet40FT_High_CubeResultArray[0] != null && SharedRail.synchronet40FT_High_CubeResultArray[0].Equals(SharedRail.success))
            {
                decimal.TryParse(SharedRail.synchronet40FT_High_CubeResultArray[2].Replace("$", "").Trim(), out synchro40cost);
                HelperFuncs.writeToSiteErrors("WS synchronet40FT_High_CubeResultArray", SharedRail.synchronet40FT_High_CubeResultArray[2]);
            }
            else
            {
                HelperFuncs.writeToSiteErrors("WS synchronet40FT_High_CubeResultArray", "null");
            }

            if (SharedRail.synchronet45FT_High_CubeResultArray[0] != null && SharedRail.synchronet45FT_High_CubeResultArray[0].Equals(SharedRail.success))
            {
                decimal.TryParse(SharedRail.synchronet45FT_High_CubeResultArray[2].Replace("$", "").Trim(), out synchro45cost);
                HelperFuncs.writeToSiteErrors("WS synchronet45FT_High_CubeResultArray", SharedRail.synchronet45FT_High_CubeResultArray[2]);
            }
            else
            {
                HelperFuncs.writeToSiteErrors("WS synchronet45FT_High_CubeResultArray", "null");
            }

            #endregion

            #region Get Integra costs

            if (SharedRail.integraResultArray40FT[0] != null && SharedRail.integraResultArray40FT[0].Equals(SharedRail.success))
            {
                if (SharedRail.integraResultArray40FT[0] != null && SharedRail.integraResultArray40FT[0].Equals(SharedRail.success))
                {
                    decimal.TryParse(SharedRail.integraResultArray40FT[2].Replace("$", "").Trim(), out integra40cost);
                    HelperFuncs.writeToSiteErrors("WS integraResultArray40FT", SharedRail.integraResultArray40FT[2]);
                }
                else
                {
                    HelperFuncs.writeToSiteErrors("WS integraResultArray40FT", "null");
                }

                if (SharedRail.integraResultArray45FT[0] != null && SharedRail.integraResultArray45FT[0].Equals(SharedRail.success))
                {
                    decimal.TryParse(SharedRail.integraResultArray45FT[2].Replace("$", "").Trim(), out integra45cost);
                    HelperFuncs.writeToSiteErrors("WS integraResultArray45FT", SharedRail.integraResultArray45FT[2]);
                }
                else
                {
                    HelperFuncs.writeToSiteErrors("WS integraResultArray45FT", "null");
                }
            }

            #endregion

            HelperFuncs.writeToSiteErrors("WS integra vs synchro", string.Concat(integra40cost, " ", integra45cost, " ", synchro40cost, " ", synchro45cost));

            // Gather all results to one results object
            if (SharedRail.streamlineResultArray[0].Equals(SharedRail.success))
            {
                results.Add(SharedRail.streamlineResultArray);

                //

                //IntermodalResult res = new IntermodalResult();
                //res.TotalCharge = streamRes.totalPrice;

                //if (transitDays.Equals(-1))
                //{
                //    res.EstimatedTransitTime = "10";
                //    res.EstimatedDeliveryDate = DateTime.Today.AddDays(10).ToShortDateString();
                //}
                //else
                //{
                //    res.EstimatedTransitTime = transitDays.ToString();
                //    res.EstimatedDeliveryDate = DateTime.Today.AddDays(transitDays).ToShortDateString();
                //}
                //res.Carrier = Streamline;
            }
            if (SharedRail.csxiResultArray[0].Equals(SharedRail.success))
            {
                results.Add(SharedRail.csxiResultArray);
            }
            if (SharedRail.modalXResultArray[0].Equals(SharedRail.success))
            {
                results.Add(SharedRail.modalXResultArray);
            }
            if (SharedRail.integraResultArray != null && SharedRail.integraResultArray[0] != null && SharedRail.integraResultArray[0].Equals(SharedRail.success))
            {
                results.Add(SharedRail.integraResultArray);
            }

            if (integra40cost > 0 && (integra40cost < synchro40cost || synchro40cost.Equals(0M)))
            {
                if (SharedRail.integraResultArray40FT != null && SharedRail.integraResultArray40FT[0] != null && SharedRail.integraResultArray40FT[0].Equals(SharedRail.success))
                {
                    results.Add(SharedRail.integraResultArray40FT);
                }
            }
            else if (synchro40cost > 0 && (synchro40cost < integra40cost || integra40cost.Equals(0M)))
            {
                if (SharedRail.synchronet40FT_High_CubeResultArray != null && !string.IsNullOrEmpty(SharedRail.synchronet40FT_High_CubeResultArray[0]) &&
                    SharedRail.synchronet40FT_High_CubeResultArray[0].Equals(SharedRail.success))
                {
                    results.Add(SharedRail.synchronet40FT_High_CubeResultArray);
                }
            }

            if (integra45cost > 0 && (integra45cost < synchro45cost || synchro45cost.Equals(0M)))
            {
                if (SharedRail.integraResultArray45FT != null && SharedRail.integraResultArray45FT[0] != null && SharedRail.integraResultArray45FT[0].Equals(SharedRail.success))
                {
                    results.Add(SharedRail.integraResultArray45FT);
                }
            }
            else if (synchro45cost > 0 && (synchro45cost < integra45cost || integra45cost.Equals(0M)))
            {
                if (SharedRail.synchronet45FT_High_CubeResultArray != null && !string.IsNullOrEmpty(SharedRail.synchronet45FT_High_CubeResultArray[0]) &&
                    SharedRail.synchronet45FT_High_CubeResultArray[0].Equals(SharedRail.success))
                {
                    results.Add(SharedRail.synchronet45FT_High_CubeResultArray);
                }
            }

            //if (SharedRail.synchronet20FT_DryResultArray != null && !string.IsNullOrEmpty(SharedRail.synchronet20FT_DryResultArray[0]) &&  
            //    SharedRail.synchronet20FT_DryResultArray[0].Equals(SharedRail.success))
            //{
            //    results.Add(SharedRail.synchronet20FT_DryResultArray);
            //}

            //if (SharedRail.synchronet40FT_High_CubeResultArray != null && !string.IsNullOrEmpty(SharedRail.synchronet40FT_High_CubeResultArray[0]) &&
            //    SharedRail.synchronet40FT_High_CubeResultArray[0].Equals(SharedRail.success))
            //{
            //    results.Add(SharedRail.synchronet40FT_High_CubeResultArray);
            //}

            //if (SharedRail.synchronet45FT_High_CubeResultArray != null && !string.IsNullOrEmpty(SharedRail.synchronet45FT_High_CubeResultArray[0]) &&
            //    SharedRail.synchronet45FT_High_CubeResultArray[0].Equals(SharedRail.success))
            //{
            //    results.Add(SharedRail.synchronet45FT_High_CubeResultArray);
            //}

            if (SharedRail.mgOtrResultArray != null && !string.IsNullOrEmpty(SharedRail.mgOtrResultArray[0]) && 
                SharedRail.mgOtrResultArray[0].Equals(SharedRail.success))
            {
                results.Add(SharedRail.mgOtrResultArray);
            }
            
            //for (byte i = 0; i < SharedRail.streamlineResults.Count; i++)
            //{
            //    resultsList.Add(SharedRail.streamlineResults[i]);
            //}

            #endregion

            SharedRail.saveResults(true, ref originCity, ref destCity, ref originState, ref destState, ref usernameGlobal, ref results, ref puDateGlobal,
                ref QuoteID);

            #region Add percentage

            Double rate, decPercent = (Double)IntermodalMarkupPercent / (Double)100, percentSum = 0;
            //Int32 transit;

            for (int i = 0; i < results.Count; i++)
            {
                //HelperFuncs.writeToSiteErrors("GetIntermodalRate user: " + user , results[i][2].Replace("$", "").Replace(",", "") + " " +
                //    decPercent.ToString() + " " + IntermodalMinimum.ToString());
                if (!Double.TryParse(results[i][2].Replace("$", "").Replace(",", ""), out rate))
                {
                    continue;
                }
                percentSum = rate * decPercent;
                if (percentSum < IntermodalMinimum)
                {
                    rate += IntermodalMinimum;
                    results[i][2] = "$" + rate;
                }
                else
                {
                    rate += percentSum;
                    results[i][2] = "$" + rate;
                }
                //throw new Exception(IntermodalMarkupPercent + " " + decPercent + " " + percentSum + " " + IntermodalMinimum);
            }
            
            #endregion

            #region Valid through

            DateTime today = new DateTime();
            today = DateTime.Today;
            int weekendDays = 0, daysToAdd = 0;
            for (int i = 1; i < 6; i++)
            {
                if (today.AddDays(i).DayOfWeek.ToString() == "Saturday" || today.AddDays(i).DayOfWeek.ToString() == "Sunday")
                {
                    weekendDays++;
                }
            }
            if (weekendDays == 1)
            {
                if (today.DayOfWeek.ToString() == "Monday")
                {
                    daysToAdd = 2;
                }
                else if (today.DayOfWeek.ToString() == "Saturday")
                {
                    daysToAdd = 1;
                }
            }
            else if (weekendDays == 2)
            {
                daysToAdd = 2;
            }
            else //weekend days = 0
            {
                daysToAdd = 0;
            }

            validThrough = DateTime.Today.AddDays(5 + daysToAdd);
            string vThru = validThrough.ToShortDateString();

            #endregion

            #region Compare rates

            //int indOfMinRate = -1;
            //int indOfMinTransit = -1;
            if (results.Count == 0)   // No results
            {
                return results;
            }
           
            for (int i = 0; i < results.Count; i++)
            {
                if (!Double.TryParse(results[i][2].Replace("$", "").Replace(",", ""), out rate))
                {
                    continue;
                }

                results[i][6] = validThrough.ToShortDateString();
                results[i][7] = originCity + ", " + originState;
                results[i][8] = originCountry;
                results[i][9] = destCity + ", " + destState;
                results[i][10] = destCountry;

                sortedResults.Add(results[i]);

                #region Not used
                //if (i == 0)
                //{
                //    minRate = rate;
                //    indOfMinRate = i;
                //}
                //if (minRate > rate)
                //{
                //    minRate = rate;
                //    indOfMinRate = i;
                //}
                #endregion

                //IntermodalResult res = new IntermodalResult();
                //res.TotalCharge = streamRes.totalPrice;

                //if (transitDays.Equals(-1))
                //{
                //    res.EstimatedTransitTime = "10";
                //    res.EstimatedDeliveryDate = DateTime.Today.AddDays(10).ToShortDateString();
                //}
                //else
                //{
                //    res.EstimatedTransitTime = transitDays.ToString();
                //    res.EstimatedDeliveryDate = DateTime.Today.AddDays(transitDays).ToShortDateString();
                //}
                //res.Carrier = Streamline;
            }

            #region Not used
            //find index of lowest transit
            //minTransit = Int32.MaxValue;
            //for (int j = 0; j < results.Count; j++)
            //{

            //    if (!Int32.TryParse(results[j][3], out transit))
            //    {

            //        if (results[j][1].Contains("IDI<(") == false)
            //        {
            //            try
            //            {
            //                //writeBugReport("could not parse transit " + results[j][3]);
            //                HelperFuncs.writeToSiteErrors("GetIntermodalRate", "could not parse transit " + results[j][3]);
            //            }
            //            catch { }
            //        }
            //        continue;
            //    }

            //    if (minTransit > transit)
            //    {
            //        minTransit = transit;
            //        indOfMinTransit = j;
            //    }
            //}



            //if (indOfMinRate != -1)
            //{
            //    if (indOfMinTransit > -1)
            //    {
            //        results[indOfMinTransit][2] = SharedRail.normalizeTrailNumbs(results[indOfMinTransit][2]);
            //    }
            //    results[indOfMinRate][2] = SharedRail.normalizeTrailNumbs(results[indOfMinRate][2]);
            //}



            

            //if (indOfMinRate != -1)
            //{
            //    if (results[indOfMinRate][1].Contains("Integra")) // in case of Integra
            //    {
            //        results[indOfMinRate][6] = results[indOfMinRate][4]; //doing this way to keep the function similar to Web Page function
            //    }
            //    else
            //    {
            //        results[indOfMinRate][6] = vThru;
            //    }
            //    results[indOfMinRate][7] = originCity + ", " + originState;
            //    results[indOfMinRate][8] = originCountry;
            //    results[indOfMinRate][9] = destCity + ", " + destState;
            //    results[indOfMinRate][10] = destCountry;

            //    if (results[indOfMinRate][1].Contains("IDI<(")) // in case of IDI
            //    {
            //        results[indOfMinRate][3] = "";
            //        results[indOfMinRate][4] = "";
            //        results[indOfMinRate][5] = "";
            //        sortedResults.Add(results[indOfMinRate]);
            //    }
            //    else
            //    {
            //        sortedResults.Add(results[indOfMinRate]);
            //    }
            //}

            //if (indOfMinTransit != -1 && (indOfMinTransit != indOfMinRate) && (results[indOfMinTransit][3] != results[indOfMinRate][3])) // transit time shorter but rate higher
            //{
            //    if (results[indOfMinTransit][1].Contains("Integra")) // in case of Integra
            //    {
            //        results[indOfMinTransit][6] = results[indOfMinTransit][4]; //doing this way to keep the function similar to Web Page function
            //    }
            //    else
            //    {
            //        results[indOfMinTransit][6] = vThru;
            //    }
            //    results[indOfMinTransit][7] = originCity + ", " + originState;
            //    results[indOfMinTransit][8] = originCountry;
            //    results[indOfMinTransit][9] = destCity + ", " + destState;
            //    results[indOfMinTransit][10] = destCountry;

            //    sortedResults.Add(results[indOfMinTransit]);
            //}
            #endregion

            #endregion

            return sortedResults;
        }
        catch (Exception exep)
        {
            HelperFuncs.writeToSiteErrors("GetIntermodalRate", exep.ToString());
            return results;
        }
    }

    public struct csxResult
    {
        public string success, transitTime, rate, containerSize;
        public bool hasCapacity;
        public DateTime firstCapacityDate, eta;
    }

    public struct railResult
    {
        public string success, transitTime, rate, containerSize;
        public bool hasCapacity;
        public DateTime firstCapacityDate, eta;
    }

    #endregion

    #region Carrier functions

    // These are entry function names only, need to have them here as required for start of threads, need to be in same file.

    #region Synchronet

    #region getSynchronetInfo

    //public static void getSynchronetInfo20() 
    //{
    //    string containerType = SharedRail.synchronetContainer20FT_Dry;
    //    SharedRail.getRateFrom_SynchronetViaAlex2015(containerType, originZipGlobal, destZipGlobal);
    //}

    //public static void getSynchronetInfo40() 
    //{
    //    string containerType = SharedRail.synchronetContainer40FT_High_Cube;
    //    SharedRail.getRateFrom_SynchronetViaAlex2015(containerType, originZipGlobal, destZipGlobal);
    //}

    //public static void getSynchronetInfo45()
    //{
    //    string containerType = SharedRail.synchronetContainer45FT_High_Cube;
    //    SharedRail.getRateFrom_SynchronetViaAlex2015(containerType, originZipGlobal, destZipGlobal);
    //}

    #endregion

    #endregion

    #region Streamline
    
    private static void getRate_Streamline()
    {
        //SharedRail.getRate_Streamline(ref originZipGlobal, ref destZipGlobal, ref originCity, ref destCity, ref usernameGlobal, ref bool chkHazmat);
    }
    
    #endregion

    #region CSXI

    public static List<string[]> GetCSXI_Rate(string username, string password, DateTime pickupDate, string originZip, string destinationZip, string[] additionalServices)
    {
        List<string[]> list = new List<string[]>();
        //railResult csxResultObj = new railResult();
        //GetCSXIInfo(ref originZip, ref destinationZip, ref pickupDate, ref list, ref csxResultObj);
        return list;
    }

    #region GetCSXIInfo

    //public static void GetCSXIInfo(ref string originZipGlobal, ref string destZipGlobal, ref DateTime puDateGlobal, ref List<string[]> list,
    //    ref railResult csxResultObj)
    //{


    //    int timeOut = 25000;
    //    try
    //    {
    //        List<HelperFuncs.Credentials> crds = new List<HelperFuncs.Credentials>();
    //        string username = "", password = "";

    //        try
    //        {
    //            crds = HelperFuncs.GetLoginsByCarID(90199);
    //            username = crds[0].username;
    //            password = crds[0].password;
    //        }
    //        catch (Exception ex)
    //        {
    //            HelperFuncs.writeToSiteErrors("CSXI", ex.ToString());
    //        }

    //        string url, referrer, contentType, accept, method, data, doc, originCity = "", destCity = "", originState = "", destState = "";

    //        CookieContainer container = new CookieContainer();
    //        CookieCollection collection = new CookieCollection();

    //        #region Login and go to rate page

    //        #region Login request

    //        referrer = "https://shipcsx.com/pub_sx_mainpagepublic_jct/sx.shipcsxpublic/PublicNavbar";
    //        url = "https://shipcsx.com/pkmslogin.form";
    //        contentType = "application/x-www-form-urlencoded";
    //        accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
    //        method = "POST";
    //        data = "login-form-type=pwd&username=" + username + "&password=" + password + "&LoginGoButton.x=7&LoginGoButton.y=8"; //user password
    //        collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, data, true, timeOut);

    //        #endregion

    //        #region Some required requests
    //        //--------------------------------------------------------------------------------------------------------------
    //        url = "https://shipcsx.com/sx_mainpage_jct/sx.shipcsx/Main";
    //        referrer = "";
    //        contentType = "";
    //        method = "GET";
    //        collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", true, timeOut);

    //        //--------------------------------------------------------------------------------------------------------------
    //        referrer = url;
    //        url = "https://shipcsx.com/sx_mainpage_jct/sx.shipcsx/Navbar";
    //        collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", true, timeOut);

    //        url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/TruckingMain";
    //        referrer = "https://shipcsx.com/sx_mainpage_jct/sx.shipcsx/Navbar";
    //        contentType = "";
    //        method = "GET";
    //        collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", true, timeOut);

    //        //--------------------------------------------------------------------------------------------------------------

    //        referrer = url;
    //        url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/jsp/sx_base_classes/blank.jsp";
    //        collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", true, timeOut);

    //        //--------------------------------------------------------------------------------------------------------------

    //        //referrer = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/TruckingMain";
    //        url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/SpotQuoteBegin?";
    //        collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", true, timeOut);

    //        //--------------------------------------------------------------------------------------------------------------

    //        referrer = url;
    //        url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/jsp/sx_base_classes/blank.jsp";
    //        collection = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", true, timeOut);

    //        //--------------------------------------------------------------------------------------------------------------
    //        #endregion

    //        #region Get City and State

    //        // Get City and State
    //        referrer = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/SpotQuoteBegin?";
    //        url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/GetCityStateData?zip=" + originZipGlobal;
    //        doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true, timeOut);

    //        // Scrape origin city and state
    //        string[] tokens = new string[3];
    //        tokens[0] = "<city>";
    //        tokens[1] = ">";
    //        tokens[2] = "<";

    //        HelperFuncs.writeToSiteErrors("CSXI originZipGlobal", originZipGlobal);

    //        originCity = HelperFuncs.scrapeFromPage(tokens, doc);
    //        if (originCity == "" || originCity == "not found")
    //            throw new Exception("No matching City/State for zipcode " + originZipGlobal);

    //        tokens[0] = "<state>";
    //        originState = HelperFuncs.scrapeFromPage(tokens, doc);

    //        url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/GetUnservicableLocations?zip=" + originZipGlobal;
    //        doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true, timeOut);

    //        url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/GetUnservicableLocations?zip=" + destZipGlobal;
    //        doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true, timeOut);

    //        url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/GetCityStateData?zip=" + destZipGlobal;
    //        doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true, timeOut);

    //        // Scrape destination city and state
    //        tokens[0] = "<city>";
    //        destCity = HelperFuncs.scrapeFromPage(tokens, doc);
    //        if (destCity == "" || destCity == "not found")
    //            throw new Exception("No matching City/State for zipcode " + destZipGlobal);

    //        tokens[0] = "<state>";
    //        destState = HelperFuncs.scrapeFromPage(tokens, doc);

    //        #endregion

    //        #region Variables

    //        DateTime dt = puDateGlobal;
    //        DateTime dtLast = new DateTime();
    //        dtLast = DateTime.Today.AddDays(6);
    //        TimeSpan span = dtLast - dt;

    //        bool hasCapacity = true;
    //        string hazmat = "";
    //        double parseDbl;
    //        bool successBool = false;
    //        string price = "";
    //        string transitTime = "";

    //        bool isLastDateTry = false;

    //        #endregion

    //        for (int i = 0; i <= span.TotalDays; i++)
    //        {
    //            HelperFuncs.writeToSiteErrors("CSXI i", i.ToString());
    //            HelperFuncs.writeToSiteErrors("CSXI span.TotalDays", span.TotalDays.ToString());

    //            if (i.Equals((int)span.TotalDays))
    //            {
    //                HelperFuncs.writeToSiteErrors("CSXI i=span", "CSXI i=span");
    //                isLastDateTry = true;
    //            }

    //            dt = DateTime.Today.AddDays(i + 1);

    //            if (dt.DayOfWeek.ToString() == "Saturday" || dt.DayOfWeek.ToString() == "Sunday")
    //            {
    //                continue;
    //            }

    //            #region Fix date

    //            string day = dt.Day.ToString(), month = dt.Month.ToString(), year = dt.Year.ToString();
    //            if (day.Length.Equals(1))
    //            {
    //                day = "0" + day;
    //            }

    //            if (month.Length.Equals(1))
    //            {
    //                month = "0" + month;
    //            }

    //            #endregion

    //            hasCapacity = true;

    //            tryDateCSXI_WithCapacity(ref container, ref originZipGlobal, ref originCity,
    //                ref originState, ref destZipGlobal, ref destCity, ref destState, ref month, ref day, ref year,
    //                ref hasCapacity, ref transitTime, ref price, ref successBool, ref isLastDateTry);

    //            if (successBool.Equals(true) && hasCapacity.Equals(true))
    //            {
    //                break;
    //            }             
    //        }

    //        #endregion


    //        #region Not used
    //        //for (int i = 0; i <= span.TotalDays; i++)
    //        //{
    //        //    dt = DateTime.Today.AddDays(i + 1);
    //        //    //dt = dt.AddDays(i);
    //        //    if (dt.DayOfWeek.ToString() == "Saturday" || dt.DayOfWeek.ToString() == "Sunday")
    //        //    {
    //        //        continue;
    //        //    }
    //        //    doc = tryDateCSXI(container, originZipGlobal, destZipGlobal, originCity, destCity, originState, destState, hazmat, i + 1);
    //        //    //if (doc.Contains("No pricing is available"))
    //        //    //{
    //        //    //    throw new Exception("No pricing is available");
    //        //    //}

    //        //    tokens[0] = "<price>";
    //        //    tokens[1] = ">";
    //        //    tokens[2] = "<";

    //        //    price = HelperFuncs.scrapeFromPage(tokens, doc).Replace("$", "");
    //        //    if (!double.TryParse(price, out parseDbl))
    //        //    {
    //        //        continue;
    //        //    }
    //        //    else
    //        //    {
    //        //        successBool = true;
    //        //    }

    //        //    tokens[0] = "<transitTime>";
    //        //    tokens[1] = ">";
    //        //    tokens[2] = "<";
    //        //    transitTime = HelperFuncs.scrapeFromPage(tokens, doc).Replace("days", "").Trim();

    //        //    if (successBool.Equals(true))
    //        //    {
    //        //        break;
    //        //    }
    //        //}
    //        #endregion

    //        if (successBool == true)
    //        {
    //            #region Set the result object

    //            HelperFuncs.writeToSiteErrors("CSXI success", "CSXI success");
    //            string[] csxiResultArray = new string[7];
    //            //csxiResultArray[0] = SharedRail.success;
    //            csxiResultArray[0] = "success";
    //            csxiResultArray[1] = "CSXI";

    //            csxiResultArray[3] = transitTime;

    //            csxiResultArray[2] = price;
    //            //insertIntoRailLogs("CSXI", 90199, "1", "", "", Convert.ToDouble(rate));
    //            HelperFuncs.writeToSiteErrors("CSXI live rate", price);

    //            csxiResultArray[4] = dt.ToShortDateString();

    //            Int32 transit;
    //            if (!Int32.TryParse(csxiResultArray[3], out transit))
    //            {
    //                HelperFuncs.writeToSiteErrors("CSXI", "could not parse transit " + csxiResultArray[3]);
    //            }
    //            csxiResultArray[5] = dt.AddDays(transit).ToShortDateString();
    //            csxiResultArray[6] = "FiftyThreeFt";
    //            list.Add(csxiResultArray);


    //            csxResultObj.success = "success";
    //            csxResultObj.transitTime = transitTime;
    //            csxResultObj.rate = price;
    //            csxResultObj.firstCapacityDate = dt;
    //            if (int.TryParse(transitTime, out transit))
    //            {
    //                csxResultObj.eta = dt.AddDays(transit);
    //            }

    //            csxResultObj.hasCapacity = hasCapacity;
    //            csxResultObj.containerSize = "FiftyThreeFt";

    //            #endregion
    //        }
    //        else
    //        {
    //            throw new Exception("No Capacity for all days, or no rate found");
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        HelperFuncs.writeToSiteErrors("CSXI", e.ToString());
    //    }
    //}

   
    #endregion

    #region Not used tryDateCSXI

    //public static string tryDateCSXI(CookieContainer container, string originZip, string destZip, string originCity, string destCity, string originState,
    // string destState, string hazmat, int daysToAdd)
    //{
    //    string url, referrer, contentType, accept, method, doc;
    //    DateTime dt = DateTime.Today.AddDays(daysToAdd);

    //    string day = dt.Day.ToString(), month = dt.Month.ToString(), year = dt.Year.ToString();
    //    if (day.Length.Equals(1))
    //    {
    //        day = "0" + day;
    //    }

    //    if (month.Length.Equals(1))
    //    {
    //        month = "0" + month;
    //    }

    //    Int64 timeStamp = SharedRail.GetTime(ref dt);
    //    url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/GetAlternateLanePriceInfo?date=" + month + "/" + day + "/" + year +
    //        "&timeSlotId=5&timestamp=" + timeStamp.ToString();
    //    referrer = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/SpotQuoteSelectPrice";
    //    contentType = "";
    //    accept = "*/*";
    //    method = "GET";
    //    doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true, 25000);

    //    return doc;
    //}

    #endregion

    #region tryDateCSXI_WithCapacity

    //public static void tryDateCSXI_WithCapacity(ref CookieContainer container, ref string originZipGlobal, ref string originCity,
    //    ref string originState, ref string destZipGlobal, ref string destCity, ref string destState, ref string month, ref string day,
    //    ref string year, ref bool hasCapacity, ref string transitTime, ref string price, ref bool successBool, ref bool isLastDateTry)
    //{
    //    #region Variables

    //    //HelperFuncs.writeToSiteErrors("CSXI date", month + " " + day);
    //    //HelperFuncs.writeToSiteErrors("CSXI isLastDateTry", isLastDateTry.ToString());

    //    string url, referrer, contentType, accept, method, data, doc;

    //    referrer = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/SpotQuoteBegin";
    //    url = "https://shipcsx.com/sx_dealspace_jct/sx.dealspace/SpotQuoteSelectPrice";
    //    contentType = "application/x-www-form-urlencoded";
    //    accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
    //    method = "POST";

    //    data = string.Concat("addFavoriteLaneName=&originZipCode=", originZipGlobal, "&originCityStateDisplay=", originCity.Replace(" ", "+"), "%2C+", originState,
    //        "&originCity=", originCity.Replace(" ", "+"), "&originState=", originState, "&destinationZipCode=", destZipGlobal,
    //        "&destinationCityStateDisplay=", destCity.Replace(" ", "+"), "%2C+", destState, "&destinationCity=", destCity.Replace(" ", "+"),
    //        "&destinationState=", destState, "&pickupDate=", month, "%2F", day, "%2F", year, "&selectedTimeOfDay=5&begEquipment=53",
    //        "&numberOfLoads=1&extraPickup0ZipCode=&extraPickup0CityStateDisplay=&extraPickup0City=",
    //        "&extraPickup0State=&extraDelivery0ZipCode=&extraDelivery0CityStateDisplay=&extraDelivery0City=",
    //        "&extraDelivery0State=&doResetQuote=&reuseAdapter=true&requote=false&quoteType=&templateId=");

    //    #endregion

    //    doc = (string)HelperFuncs.generic_http_request_3("string", container, url, referrer, contentType, accept, method, data, false, false, "", "");

    //    //HelperFuncs.writeToSiteErrors("CSXI", doc);

    //    // Get cost/costs from this page
    //    // Scrape html table 

    //    string[] tokens2 = new string[3];
    //    tokens2[0] = "<table border=\"0\" cellpadding=\"4\" cellspacing=\"0\" width=\"100%";
    //    tokens2[1] = ">";
    //    tokens2[2] = "</table>";
    //    string tblHtml = HelperFuncs.scrapeFromPage(tokens2, doc);
    //    //HelperFuncs.writeToSiteErrors("CSXI tblHtml", tblHtml);

    //    #region Scrape result row

    //    // Scrape result row 

    //    tokens2[0] = "<tr class=\"columnBasedResultsRow";
    //    tokens2[1] = ">";
    //    tokens2[2] = "</tr>";
    //    string columnBasedResultsRow = HelperFuncs.scrapeFromPage(tokens2, tblHtml);
    //    //HelperFuncs.writeToSiteErrors("CSXI columnBasedResultsRow", columnBasedResultsRow);

    //    if (columnBasedResultsRow.Contains("No Capacity"))
    //    {
    //        HelperFuncs.writeToSiteErrors("CSXI no capacity", "CSXI no capacity");
    //        hasCapacity = false;
    //        if (!isLastDateTry)
    //        {
    //            //HelperFuncs.writeToSiteErrors("CSXI not LastDateTry", "CSXI not LastDateTry");
    //            return;
    //        }
    //        else
    //        {
    //            //HelperFuncs.writeToSiteErrors("CSXI LastDateTry", "CSXI LastDateTry");
    //        }
    //    }

    //    tokens2[0] = "<span";  // style=\\\"vertical-align: top;
    //    tokens2[1] = ">";
    //    tokens2[2] = "</span>";
    //    price = HelperFuncs.scrapeFromPage(tokens2, columnBasedResultsRow)
    //        .Replace("&nbsp;", "").Replace("$", "").Replace(",", "").Trim();
    //    HelperFuncs.writeToSiteErrors("CSXI costStr", price);

    //    decimal testDecimal;
    //    if (decimal.TryParse(price, out testDecimal))
    //    {
    //        successBool = true;
    //    }

    //    // Get transit time

    //    string[] tokens3 = new string[5];
    //    tokens3[0] = "<td";
    //    tokens3[1] = "<td";
    //    tokens3[2] = "<td";
    //    tokens3[3] = ">";
    //    tokens3[4] = "</td>";
    //    transitTime = HelperFuncs.scrapeFromPage(tokens3, columnBasedResultsRow).Replace(",", "").Replace("days", "")
    //        .Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();
    //    //HelperFuncs.writeToSiteErrors("CSXI transitTime", transitTime);

    //    #endregion

    //    #region Scrape alt result row

    //    // Scrape alt result row 

    //    //tokens2[0] = "<tr class=\"columnBasedResultsRowAlt";
    //    //tokens2[1] = ">";
    //    //tokens2[2] = "</tr>";
    //    //string columnBasedResultsRowAlt = HelperFuncs.scrapeFromPage(tokens2, tblHtml);
    //    //HelperFuncs.writeToSiteErrors("CSXI columnBasedResultsRowAlt", columnBasedResultsRowAlt);

    //    #endregion

    //}

    #endregion


    //public static void GetCSXIInfo()
    //{
    //    //SharedRail.GetCSXIInfo(ref originZipGlobal, ref destZipGlobal, ref puDateGlobal);
    //    SharedRail.GetCSXIInfoFromAlex2015(ref originZipGlobal, ref destZipGlobal, ref puDateGlobal);
    //}

    #endregion

    #region Integra

    private static void GetIntegraInfo(string containerType)
    {
        SharedRail.GetIntegraInfo(ref originZipGlobal, ref destZipGlobal, ref originCity, ref destCity, ref originState, ref destState,
            ref originCountry, ref destCountry, ref puDateGlobal, ref containerType);
    }
    
    #endregion

    #region ModalX
    
    private static void GetModalX_Info()
    {
        SharedRail.GetModalX_Info(ref originZipGlobal, ref destZipGlobal);
    }
    
    #endregion

    #region OTR

    #region getOTR_RateDLS

    public static void getOTR_RateDLS()
    {
        string[] cityStateCountryOrig = new string[3];
        string[] cityStateCountryDest = new string[3];

        string oState, dState, oCity, dCity, oCountry, dCountry;

        try
        {
            SharedRail.cityStateCountryByZip(ref cityStateCountryOrig, originZipGlobal);
            SharedRail.cityStateCountryByZip(ref cityStateCountryDest, destZipGlobal);

            if (!string.IsNullOrEmpty(cityStateCountryOrig[0]))
            {
                oState = cityStateCountryOrig[1];
                oCity = cityStateCountryOrig[0];
                oCountry = cityStateCountryOrig[2];
            }
            else
            {
                return;
            }
            if (!string.IsNullOrEmpty(cityStateCountryDest[0]))
            {
                dState = cityStateCountryDest[1];
                dCity = cityStateCountryDest[0];
                dCountry = cityStateCountryDest[2];
            }
            else
            {
                return;
            }

            SharedRail.getOTR_RateDLS(ref originZipGlobal, ref destZipGlobal, ref puDateGlobal, ref oCity, ref oState, ref oCountry,
                ref dCity, ref dState, ref dCountry);
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("MG OTR", e.ToString());
        }
    }

    #endregion

    #endregion

    #endregion

    #region Not used For WS only

    #region GetGCMUserNameByAPILogins

    private static string GetGCMUserNameByAPILogins(string username, string password)
    {
        string user = "";
        string sql = "";
        sql = string.Concat("SELECT UserName ",
                "FROM tbl_LOGIN_API WHERE APIUserName='", username, "' AND APIKey='", password, "'");
        SqlConnection conn = new SqlConnection(aesmain_dataConnectionStringSS);
        SqlCommand command = new SqlCommand(sql, conn);

        try
        {
            conn.Open();
            SqlDataReader dr = command.ExecuteReader();

            if (dr.Read())
            {
                user = (string)dr[0];
            }
            else
            {
                throw new Exception("");
            }
            dr.Close();
            conn.Close();
            conn.Dispose();
            return user;
        }
        catch
        {
            try
            {
                conn.Close();
                conn.Dispose();
                command.Dispose();
            }
            catch
            {
            }
            return user;
        }
    }
    
    #endregion

    #region GetMarkupPercent
    
    public static int[] GetMarkupPercent(string username)
    {
        int[] info = new int[2];
        string sql = "";
        sql = string.Concat("SELECT IntermodalMarkupPercent, IntermodalMinimum ",
                "FROM tbl_LOGIN WHERE UserName='", username, "'");
        SqlConnection conn = new SqlConnection(aesmain_dataConnectionStringSS);
        SqlCommand command = new SqlCommand(sql, conn);

        try
        {
            conn.Open();
            SqlDataReader dr = command.ExecuteReader();

            if (dr.Read())
            {
                info[0] = (int)dr[0];
                info[1] = (int)dr[1];
            }
            else
            {
                throw new Exception("");
            }
            dr.Close();
            conn.Close();
            conn.Dispose();
            return info;
        }
        catch
        {
            try
            {
                conn.Close();
                conn.Dispose();
                command.Dispose();
            }
            catch
            {
            }
            return info;
        }
    }
    
    #endregion

    #endregion
  
}
