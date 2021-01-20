#region Using

using System;
using System.Data;
//using System.Collections;
using System.Collections.Generic;
//using System.Data.SqlClient;
using System.Net;
//using System.IO;
using System.Text;
using System.Xml;
using System.Threading;
using System.Web.Services.Protocols;
using System.Linq;
//using UPS_PackageRateService;
using gcmAPI.Models.LTL;
using gcmAPI.Models.Carriers;
using gcmAPI.Models.Utilities;
using gcmAPI.Models.Carriers.DLS;
using gcmAPI.Models.Carriers.UPS;
using System.Diagnostics;
using gcmAPI.Models.Customers;
//using static gcmAPI.Models.Utilities.Mail;

#endregion
    
public class LTL_Carriers
{

    #region Variables

    #region Threads declaration

    List<Thread> ltl_threads;

    Thread oThreadEstes_DLS_account, oThreadEstes_Genera_account;

    // Volume threads
    Thread oThread_P44, oThread_USF_volume, oThread_ABF_volume, oThread_Estes_volume, oThread_XPO_volume,
        oThread_YRC_volume, oThread_Sunset_volume;

    Thread oThreadDLS, oThreadDLS_GLTL, oThreadDLS_cust_rates, oThreadDLS_BenFranklinCraftsMacon, oThreadDLS_Genera,
        oThreadDLS_HHG_Under_500, oThreadDLS_GLTL_HHG_Under_500,
        oThreadRR, oThreadRR_Class50, 
        oThreadPittOhio_API, oThreadPittOhio_API_SPC, oThreadPittOhio_API_Durachem,
        oThreadYRC, oThreadYRC_SPC, oThreadRL, oThreadRL_Genera, oThreadSAIASPC,
        oThreadRRSPC, oThreadDayton;
    //, oThreadNEMF;oThreadSEFL,oThreadRLGriots,  oThreadCentralFreight,

    Thread oThreadUSFREDDEPP, oThreadUSFREDPP, oThreadUSFREDPP_Packwest, oThreadUSFREDSPCPP, oThreadUSFREDSPCC,
        oThreadUSFHOLSPCPP, oThreadUSFHOLSPCC, oThreadUSFREDMWIPP, oThreadUSFHOLMWIPP;

    Thread oThreadODFL, oThreadConWayFreight, oThreadXPO_DLS,
        oThreadXPO_Shug, oThreadXPO_Durachem, oThreadAAACooper;

    Thread oThreadRR_AAFES;

    Thread oThreadUPS_FREIGHT_Genera, oThreadBestOvernite_Genera, oThreadSMTL_Genera, oThreadNewPenn_Genera,
        oThreadTruckload_Genera, oThreadFrontier_Genera, oThreadPyle_Genera, oThreadAveritt_Genera, oThreadDaylight_Genera;

    Thread oThreadUPSPackage_Ground, oThreadUPSPackage_NextDayAir, oThreadUPSPackage_SecondDayAir,
        oThreadUPSPackage_3DaySelect, oThreadUPSPackage_NextDayAirSaver, oThreadUPSPackage_NextDayAirEarlyAM,
        oThreadUPSPackage_2ndDayAirAM;

    Thread oThreadRLMWI, oThreadSAIAMWI;

    #endregion

    SharedLTL.CarriersResult carriersResult = new SharedLTL.CarriersResult();

    public struct LTLDefaultnmfc
    {
        public string CommodityName, NMFC, FreightClass, MinDensity;
    }

    private static int spcMaxMarkup = 175;

    #region Carrier Display Names

    string strRoadRunnerDisplay = "Roadrunner Transportation Services";
    string strSEFLDisplay = "Southeastern Freight Lines";
    string strNEMFDisplay = "New England Motor Freight";

    #endregion

    #region Fields

    bool isAlaskaOrig = false;
    bool isHawaiiOrig = false;
    bool isPuertoRicoOrig = false;
    bool isAlaskaDest = false;
    bool isHawaiiDest = false;
    bool isPuertoRicoDest = false;
    string trueOrig = "";
    string trueDest = "";
    string trueOrigZip = "";
    string trueDestZip = "";
    string midOrig = "";
    string midDest = "";
    static string midOrigZip = "";
    static string midDestZip = "";

    public static int intTimeOut = 12000;

    #endregion

    GCMRateQuote[] totalQuotes;

    //QuoteResults objResult;

    #region GCMRateQuote declaration

    gcmAPI.RRTS_WebService.QuoteResponse objRoadRunnerResult;
    gcmAPI.RRTS_WebService.QuoteResponse objRoadRunnerResultClass50;
    gcmAPI.RRTS_WebService.QuoteResponse objRoadRunnerResultAAFES;
    //GCMRateQuote centralTransDensityQuote;
    GCMRateQuote usfReddawayQuoteDEPrepaid;
    GCMRateQuote yrcQuote;
    GCMRateQuote seflQuote;
    GCMRateQuote rlQuote, rlQuote_Genera;
    GCMRateQuote rl_quote_guaranteed, rl_quote_guaranteed_Genera;
    GCMRateQuote rlQuoteGriots;

    GCMRateQuote conWayFreightQuote;
    GCMRateQuote xpoDLS_Quote;
    GCMRateQuote xpoShugQuote;
    //GCMRateQuote nemfQuote;
    GCMRateQuote saiaQuote;
    GCMRateQuote odflQuote;
    //GCMRateQuote odflMWIQuote; 
    GCMRateQuote aaaCooperQuote;

    gcmAPI.RRTS_WebService.QuoteResponse objRoadRunnerResultSPC;

    GCMRateQuote objDaytonFreightResult;

    GCMRateQuote estesQuote_DLS_account, estesQuote_Genera_account;
    //GCMRateQuote objEstesExpressResultSPC;
    //GCMRateQuote objEstesExpressResultAllmodes;
    //GCMRateQuote objEstesExpressResultICAT;
    //GCMRateQuote objEstesExpressResultOnline;

    GCMRateQuote yrcQuoteSPC;
    GCMRateQuote usfReddawayQuoteSPCPrepaid;
    GCMRateQuote usfReddawayQuotePrepaid;
    GCMRateQuote usfReddawayQuotePrepaid_Packwest;
    GCMRateQuote usfHollandQuoteSPCPrepaid;
    GCMRateQuote usfReddawayQuoteSPCCollect;
    GCMRateQuote usfHollandQuoteSPCCollect;
    GCMRateQuote saiaQuoteSPC;
    //GCMRateQuote centralFreightQuote;

    GCMRateQuote pittOhioQuoteAPI;
    GCMRateQuote pittOhioQuoteAPI_SPC;
    GCMRateQuote pittOhioQuoteAPI_Durachem;

    ///////////MWI Pricing/////////////// 
    GCMRateQuote saiaQuoteMWI;
    GCMRateQuote rlQuoteMWI;
    //GCMRateQuote nemfQuoteMWI;

    //GCMRateQuote rlQuotePuertoRico;
    //GCMRateQuote objEstesExpressResultMWI;
    GCMRateQuote usfReddawayQuoteMWIPrepaid;
    GCMRateQuote usfHollandQuoteMWIPrepaid;

    GCMRateQuote UPS_FREIGHT_Quote_Genera;
    GCMRateQuote BestOvernite_Quote_Genera, SMTL_Quote_Genera, NewPenn_Quote_Genera, Truckload_Quote_Genera,
        Frontier_Quote_Genera, Pyle_Quote_Genera, Averitt_Quote_Genera, Daylight_Quote_Genera;

    #endregion

    #region Custom results objects

    UPS_Package.upsPackageResRow resUPSGround;
    UPS_Package.upsPackageResRow resUPSNextDayAir;
    UPS_Package.upsPackageResRow resSecondDayAir;
    UPS_Package.upsPackageResRow res3DaySelect;
    UPS_Package.upsPackageResRow resNextDayAirSaver;
    UPS_Package.upsPackageResRow resNextDayAirEarlyAM;
    UPS_Package.upsPackageResRow res2ndDayAirAM;

    List<DLS.dlsPricesheet> dlsPricesheets;
    List<DLS.dlsPricesheet> dlsPricesheetsGLTL;
    List<DLS.dlsPricesheet> dls_cust_rates_Pricesheets;
    List<DLS.dlsPricesheet> dlsPricesheets_BenFranklinCraftsMacon;
    List<DLS.dlsPricesheet> dlsPricesheets_Genera;
    List<DLS.dlsPricesheet> dlsPricesheets_HHG_Under_500;
    List<DLS.dlsPricesheet> dlsPricesheetsGLTL_HHG_Under_500;

    List<P44.result> P44_results;

    USF.Volume_result usf_volume_result;
    Abf_volume_result abf_volume_result;
    YRC.Volume_result yrc_volume_result;
    XPO.Volume_result xpo_volume_result;
    Sunset_Pacific.Sunset_P_Res sunset_volume_result;

    Estes.Volume_result estes_volume_result;
    Estes.Volume_result estes_volume_economy_result;
    Estes.Volume_result estes_volume_basic_result;

    #endregion

    #region Structs

    public struct CarsOnTime
    {
        public int delivOnTime, delivLate;
        public string onTimePercent;
        public string carName, origState, destState;
    }

    #endregion

    private static LTLPiece[] m_lPiece;

    QuoteData quoteData;
    HelperFuncs.AccessorialsObj AccessorialsObj;

    static double dupreRRTS_Buy = 0.0, aafesRRTS_Buy = 0.0;

    //private string TopTransit = "";
    //private string TopOnTime = "";
    //private string TopCarrier = "";
    //private double TopRate = -1;

    private int elapsed_milliseconds_DLS;

    private bool got_Estes_HHG_Under_500_rate = false;
    //, got_Estes_GLTL_HHG_Under_500_rate = false

    bool found_yrc_volume = false;

    #endregion

    #region Constructor

    public LTL_Carriers(QuoteData quoteData)
    {
        this.quoteData = quoteData;
        AccessorialsObj = quoteData.AccessorialsObj;
        m_lPiece = quoteData.m_lPieceList.ToArray();
    }

    #endregion

    #region GetRates

    public SharedLTL.CarriersResult GetRates()
    {
        // Test abc

        // Test 2 abc

        // Test 3 abc

        #region Testing log request data

        DB.Log("test", string.Concat(" subdomain:", quoteData.subdomain,
            " mode:", quoteData.mode,
            " puDate:", quoteData.puDate, " isHazmat:", quoteData.isHazmat,
            " quoteData.hasDimensions:", quoteData.hasDimensions, " showDLSRates:",
            quoteData.showDLSRates, " isDUR:", quoteData.isDUR, " isAssociationID_5:", quoteData.isAssociationID_5, " isCommodity:",
            quoteData.isCommodity, " isCommodityLkupHHG:", quoteData.isCommodityLkupHHG, " isHHG:", quoteData.isHHG, " isUSED:",
            quoteData.isUSED, " isHHG_AndUnder500:",
            quoteData.isHHG_AndUnder500, " isUserVanguard:", quoteData.isUserVanguard, " username:", quoteData.username, " origZip:",
            quoteData.origZip, " origCity:", quoteData.origCity, " origState:", quoteData.origState, " destZip:", quoteData.destZip, " destCity:",
            quoteData.destCity, " destState:", quoteData.destState

            )
            );

        //for (int i = 0; i < m_lPiece.Length; i++)
        //{
        //    DB.Log("test m_lPiece", string.Concat(" Weight:", m_lPiece[i].Weight,

        //        " FreightClass:", m_lPiece[i].FreightClass,
        //        " Quantity:", m_lPiece[i].Quantity,
        //        " Pieces:", m_lPiece[i].Pieces,
        //        " Units:", m_lPiece[i].Units,
        //        " ItemUnit:", m_lPiece[i].ItemUnit,
        //        " HazMat:", m_lPiece[i].HazMat,
        //        " Tag:", m_lPiece[i].Tag,
        //        " Length:", m_lPiece[i].Length,
        //        " Width:", m_lPiece[i].Width,
        //        " Height:", m_lPiece[i].Height,
        //        " Commodity:", m_lPiece[i].Commodity

        //        ));
        //}

        //DB.Log("Accessors Test", " RESPU " + AccessorialsObj.RESPU.ToString() +
        //" RESDEL " + AccessorialsObj.RESDEL.ToString() +
        //" CONPU " + AccessorialsObj.CONPU.ToString() +
        //" CONDEL " + AccessorialsObj.CONDEL.ToString() +
        //" INSDEL " + AccessorialsObj.INSDEL.ToString() +
        //" APTPU " + AccessorialsObj.APTPU.ToString() +
        //" APTDEL " + AccessorialsObj.APTDEL.ToString() +
        //" TRADEPU " + AccessorialsObj.TRADEPU.ToString() +
        //" TRADEDEL " + AccessorialsObj.TRADEDEL.ToString() +
        //" LGPU " + AccessorialsObj.LGPU.ToString() +
        //" LGDEL " + AccessorialsObj.LGDEL.ToString() +
        //" MILIPU " + AccessorialsObj.MILIPU.ToString() +
        //" MILIDEL " + AccessorialsObj.MILIDEL.ToString() +
        //" GOVPU " + AccessorialsObj.GOVPU.ToString() +
        //" GOVDEL " + AccessorialsObj.GOVDEL.ToString()

        //);

        #endregion

        #region True zip Mid zip

        trueOrig = quoteData.origCity;
        trueOrigZip = quoteData.origZip;
        trueDest = quoteData.destCity;
        trueDestZip = quoteData.destZip;

        midOrig = quoteData.origCity;
        midOrigZip = quoteData.origZip;
        midDest = quoteData.destCity;
        midDestZip = quoteData.destZip;

        #endregion

        double origAddition = 0;
        double destAddition = 0;
        int transitAddition = 0;

        #region Get countries

        quoteData.origCountry = HelperFuncs.GetCountryByZip(trueOrigZip, true, quoteData.origState, quoteData.destState);
        quoteData.destCountry = HelperFuncs.GetCountryByZip(trueDestZip, false, quoteData.origState, quoteData.destState);
        //DB.Log("origCountry, destCountry", quoteData.origCountry + ", " + quoteData.destCountry);

        //orig_zip_Canada_no_space
        if (quoteData.origCountry == "CANADA")
        {
            string[] for_split = quoteData.origZip.Split(' ');
            if (for_split.Length == 2)
            {
                quoteData.orig_zip_Canada_no_space = for_split[0] + for_split[1];
            }
            else
            {
                quoteData.orig_zip_Canada_no_space = quoteData.origZip;
            }
        }

        if (quoteData.destCountry == "CANADA")
        {
            string[] for_split = quoteData.destZip.Split(' ');
            if (for_split.Length == 2)
            {
                quoteData.dest_zip_Canada_no_space = for_split[0] + for_split[1];
            }
            else
            {
                quoteData.dest_zip_Canada_no_space = quoteData.destZip;
            }
        }

        #endregion

        #region Hawaii, Alaska, Puerto Rico

        //23601 S WILMINGTON AVE CARSON CA 90745
        if (quoteData.origState == "HI")
        {
            isHawaiiOrig = true;
            //midOrig = "GARDENA, CA";
            //midOrigZip = "90248";

            midOrig = "CARSON, CA";
            midOrigZip = "90745";

            // Fix quoteData obj
            HelperFuncs.fixQuoteDataOrig(ref quoteData, ref midOrig, ref midOrigZip);
        }
        else
        {
            isHawaiiOrig = false;
        }

        if (quoteData.origState == "AK")
        {
            isAlaskaOrig = true;
            midOrig = "TACOMA, WA";
            midOrigZip = "98424";
            // Fix quoteData obj
            HelperFuncs.fixQuoteDataOrig(ref quoteData, ref midOrig, ref midOrigZip);
        }
        else
        {
            isAlaskaOrig = false;
        }

        if (quoteData.origState == "PR")
        {
            isPuertoRicoOrig = true;
            midOrig = "JACKSONVILLE, FL";
            midOrigZip = "32220";
            // Fix quoteData obj
            HelperFuncs.fixQuoteDataOrig(ref quoteData, ref midOrig, ref midOrigZip);
        }
        else
        {
            isPuertoRicoOrig = false;
        }

        if (quoteData.destState == "HI")
        {
            isHawaiiDest = true;
            //midDest = "GARDENA, CA";
            //midDestZip = "90248";

            midDest = "CARSON, CA";
            midDestZip = "90745";

            // Fix quoteData obj
            HelperFuncs.fixQuoteDataDest(ref quoteData, ref midDest, ref midDestZip);
        }
        else
        {
            isHawaiiDest = false;
        }

        if (quoteData.destState == "AK")
        {
            isAlaskaDest = true;
            midDest = "TACOMA, WA";
            midDestZip = "98424";
            // Fix quoteData obj
            HelperFuncs.fixQuoteDataDest(ref quoteData, ref midDest, ref midDestZip);
        }
        else
        {
            isAlaskaDest = false;
        }

        if (quoteData.destState == "PR")
        {
            isPuertoRicoDest = true;
            midDest = "JACKSONVILLE, FL";
            midDestZip = "32220";
            // Fix quoteData obj
            HelperFuncs.fixQuoteDataDest(ref quoteData, ref midDest, ref midDestZip);
        }
        else
        {
            isPuertoRicoDest = false;
        }

        #endregion

        var repo = new gcmAPI.Models.LTL.Repository(ref quoteData);

        #region Hawaii, Alaska, Puerto Rico
        // Not multithreading HI, AK, PR for the time being
        if (isHawaiiOrig == true)
        {
            origAddition = repo.GetHawaiiOrigRate(trueOrigZip);
            transitAddition += 8;
        }
        else if (isAlaskaOrig == true)
        {
            origAddition = repo.GetAlaskaOrigRate(trueOrigZip);
            origAddition += origAddition * .10;    //markup Per Bob's request
            origAddition += origAddition * .35;    //fuel
            transitAddition += 8;
        }
        else if (isPuertoRicoOrig == true)
        {
            //origAddition = GetPuertoRicoOrigRate(trueOrigZip);
            origAddition = 99999; // GetPuertoRicoOrigRate is not working
            transitAddition += 8;
        }

        if (isHawaiiDest == true)
        {
            destAddition = repo.GetHawaiiDestRate(trueDestZip);
            transitAddition += 8;
        }
        else if (isAlaskaDest == true)
        {
            destAddition = repo.GetAlaskaDestRate(trueDestZip);
            destAddition += destAddition * .10;    //markup Per Bob's request
            destAddition += destAddition * .35;    //fuel
            transitAddition += 8;
        }
        else if (isPuertoRicoDest == true)
        {
            //destAddition = GetPuertoRicoDestRate(trueDestZip);
            destAddition = 99999; // GetPuertoRicoDestRate is not working
            transitAddition += 8;
        }
        #endregion

        #region Cost additions

        if (quoteData.username.ToLower() != "gcm")
        {
            origAddition += origAddition * .15;
            destAddition += destAddition * .15;
        }

        //if (midOrigZip == "60666" || midOrigZip == "90045")
        //{
        //    origAddition += 190;
        //}

        //if (midDestZip == "60666" || midDestZip == "90045")
        //{
        //    destAddition += 190;
        //}

        double addition = origAddition + destAddition;

        #endregion

        #region SaveLog

        int newLogId = 0;

        if (quoteData.mode.Equals("ws") && quoteData.username == AppCodeConstants.un_genera) // Genera
        {
            // Do nothing
        }
        else
        {
            repo.LTLSaveLog(trueDestZip, quoteData.username, trueOrigZip, ref newLogId);
        }


        if (quoteData.is_AAFES_quote == true)
        {
            // Insert into AAFES Quotes
            repo.Insert_into_AAFES_quotes(ref newLogId);
        }

        #endregion

        ltl_threads = new List<Thread>();


        //if (quoteData.mode.Equals("durachem_bidding"))
        //{
        //    Durachem_Bidding_StartAndJoinThreads();
        //}
        if (quoteData.mode.Equals("NetNet"))
        {
            if (quoteData.is_Genera_quote == true)
            {
                string sql = string.Concat("INSERT INTO Genera_Rating(QuoteID,Request_Data,Response_Data,TotalCube) VALUES(",
                            newLogId, ",'','',0.0",
                            ")");
                HelperFuncs.ExecuteNonQuery(AppCodeConstants.connStringAesAPI, ref sql, "Get_LTL_ratesController");
            }
            NetNetStartAndJoinThreads();
        }
        else if (quoteData.mode.Equals("ws") && quoteData.username == AppCodeConstants.un_genera) // Genera if 
        {
            //if (quoteData.totalUnits > 9)
            //{
            //    Genera genera = new Genera();
            //    genera.Send_email_over_9_pallets(ref quoteData, ref newLogId);
            //}

            if (quoteData.totalUnits > 30)
            {
                return carriersResult;
            }

            #region Calculate class by density

            // Get cube and weight and units
            //DB.Log("genera cube", quoteData.totalCube.ToString());

            //DB.Log("genera total weight", quoteData.totalWeight.ToString());

            //DB.Log("genera totalUnits", quoteData.totalUnits.ToString());
            quoteData.totalCube *= 1.5;

            // Calculate density
            quoteData.totalDensity = quoteData.totalWeight / quoteData.totalCube;

            // Get freight class by density
            SharedLTL.Get_class_by_density(ref quoteData);

            quoteData.hasFreightClass = true;

            quoteData.linealFeet = quoteData.totalUnits * 2;

            for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
            {
                quoteData.m_lPiece[i].FreightClass = quoteData.calculated_freight_class.ToString();
                //DB.Log("genera calculated freight class " + i.ToString(), quoteData.m_lPiece[i].FreightClass);
            }

            repo.LTLSaveLog(trueDestZip, quoteData.username, trueOrigZip, ref newLogId);

            #endregion

            #region Start_Genera_volume_threads

            if (quoteData.totalUnits > 26)
            {
                quoteData.totalUnits = 26;
                quoteData.linealFeet = quoteData.totalUnits * 2;
                Start_Genera_volume_threads();
            }
            else
            {
                Start_Genera_volume_threads();
            }

            #endregion

            #region Start_LTL_threads

            

            if (quoteData.totalUnits == 6 || quoteData.totalUnits == 7 || quoteData.totalUnits == 8 || quoteData.totalUnits == 9)
            {
                // Start DLS threads, to get only XPO rate
                Start_join_LTL_threads("start");
            }
            else if (quoteData.totalUnits < 6)
            {
                Start_join_LTL_threads("start");
            }
            else
            {
                // Do nothing
            }

            #endregion

            //

            #region Join_Genera_volume_threads

            Join_Genera_volume_threads();

            //if (quoteData.totalUnits > 26)
            //{
            //    //quoteData.totalUnits = 26;
            //    //quoteData.linealFeet = quoteData.totalUnits * 2;
            //    Join_Genera_volume_threads();
            //}
            //else
            //{
            //    Join_Genera_volume_threads();
            //}

            #endregion

            #region Join_LTL_threads

            if (quoteData.totalUnits == 6 || quoteData.totalUnits == 7 || quoteData.totalUnits == 8 || quoteData.totalUnits == 9)
            {
                // Join DLS threads, to get only XPO rate
                Start_join_LTL_threads_if_density_not_low("join");
            }
            else if (quoteData.totalUnits < 6)
            {
                Start_join_LTL_threads_if_density_not_low("join");
            }
            else
            {
                // Do nothing
            }

            #endregion
        }
        else
        {
            // Live or WS
            // If mode = WS with Lineal Feet WS_with_lineal_feet mode == "WS" && lineal_feet > 0.0
            if (quoteData.mode.Equals("ws") && quoteData.linealFeet > 0.0)
            {
                #region This case is not currently needed
                // This case is not currently needed Add logic here

                #region Start_volume_threads

                if (quoteData.linealFeet >= 20.0)
                {
                    // Do nothing
                }
                else
                {
                    Start_volume_threads();
                }

                #endregion

                Start_join_LTL_threads_if_density_not_low("start");

                #endregion
            }
            else
            {
                // Live GCM
                Start_threads();
                Start_volume_threads();
            }

            if (quoteData.mode.Equals("ws") && quoteData.linealFeet > 0.0)
            {
                #region This case is not currently needed

                // Add logic here

                #region Join_volume_threads

                if (quoteData.linealFeet >= 20.0)
                {
                    // Do nothing
                }
                else
                {
                    Join_volume_threads();
                }

                #endregion

                Start_join_LTL_threads_if_density_not_low("join");

                #endregion
            }
            else
            {
                // Live GCM
                Join_threads();
                Join_volume_threads();
            }
        }


        // Checks each carrier result object and adds to array of results
        AddCarrierResultsToArray(ref transitAddition, ref addition, quoteData.isHazmat, ref newLogId);

        #region Variables for markup calculation

        int intLTLMarkup;
        double dblLTLMarkupRatio; //, dblParcelMarkup

        if (quoteData.subdomain.Equals("spc"))
        {
            intLTLMarkup = 33;
        }
        //else if (quoteData.mode.Equals("ws"))
        //{
        //    intLTLMarkup = 33;
        //}
        else
        {
            //gcmAPI.Models.LTL.Repository repo = new gcmAPI.Models.LTL.Repository(ref quoteData);
            intLTLMarkup = repo.GetLTLMarkup();
        }
        //

        //dblParcelMarkup = GetParcelMarkup();
        //dblParcelMarkup = Convert.ToDouble(Convert.ToDouble(dblParcelMarkup) / 100.00);
        dblLTLMarkupRatio = Convert.ToDouble(Convert.ToDouble(intLTLMarkup) / 100.00);
        double finalMarkup = 0;
        //SC
        //double finalMarkupGAM = 0, finalMarkupGPM = 0;
        double rateAfterAddinLTLMarkup = 0.0;
        //SC
        //double rateAfterAddinLTLMarkupGAM, rateAfterAddinLTLMarkupGPM;
        //string strRatedClass = "";

        //int resultCounter = 0;

        bool isUPS_Package = false;

        #endregion

        if (totalQuotes == null)
        {
            return carriersResult;
        }

        for (byte i = 0; i < totalQuotes.Length; i++)
        {
            if (totalQuotes[i].DisplayName.Contains("YRC Volume RRD"))
            {
                if (quoteData.mode == "NetNet")
                {
                    // Do nothing
                }
                else
                {
                    AddMarkup(ref isUPS_Package, ref totalQuotes[i], ref finalMarkup, ref rateAfterAddinLTLMarkup, ref dblLTLMarkupRatio);
                }
            }
            else if (quoteData.mode.Equals("NetNet") || totalQuotes[i].DisplayName.Contains(" RRD"))
            {
                // Do nothing
            }
            else
            {
                AddMarkup(ref isUPS_Package, ref totalQuotes[i], ref finalMarkup, ref rateAfterAddinLTLMarkup, ref dblLTLMarkupRatio);
            }

            repo.CalculateCoverageCost(i, ref totalQuotes, ref isHawaiiDest);
            totalQuotes[i].TotalLineHaul = totalQuotes[i].TotalPrice * 4;

            //gcmAPI.Models.LTL.Repository repo = new gcmAPI.Models.LTL.Repository(ref quoteData);
            totalQuotes[i].RulesTarrif = repo.QueryCarrierNotes(totalQuotes[i].CarrierKey);

            totalQuotes[i].RateId = Guid.NewGuid().ToString();
            totalQuotes[i].NewLogId = newLogId;

            //

            totalQuotes[i].TotalPrice = Convert.ToDouble(string.Format("{0:0.00}", totalQuotes[i].TotalPrice));

            //totalQuotes[i].RateType = "GUARANTEEDPM";

            //if (totalQuotes[i].DisplayName.Contains("Guaranteed by 5PM"))
            //{
            //    totalQuotes[i].RateType = "GUARANTEEDPM";
            //}
            //else
            //{
            //    totalQuotes[i].RateType = "REGULAR";
            //}

            if (quoteData.mode.Equals("NetNet"))
            {
                // Do nothing
            }
            else
            {
                if (totalQuotes[i].DisplayName.Contains("SPC Pricing"))
                {
                    totalQuotes[i].DisplayName = totalQuotes[i].DisplayName.Replace("SPC Pricing", " ");
                }
                else if (totalQuotes[i].DisplayName.Contains("SPC Collect Pricing") ||
                    totalQuotes[i].DisplayName.Contains("SPC Prepaid Pricing"))
                {
                    totalQuotes[i].DisplayName = totalQuotes[i].DisplayName.Replace(" SPC ", " ");
                }
            }

            #region OnTimePercent

            //if (quoteData.mode.Equals("LTLRaterLive") && totalQuotes[i].DisplayName.Contains(" RRD"))
            //{
            //    // On time
            //    On_time_info on_time_info =
            //        gcmAPI.Models.LTL.Repository.Get_on_time_info_by_scac(
            //            quoteData.origState, quoteData.destState, totalQuotes[i].Scac);

            //    totalQuotes[i].OnTimePercent = on_time_info.onTimePercent;
            //}
            //else
            //{
            //    // Do nothing
            //}

            #endregion

        }

        for (byte i = 0; i < totalQuotes.Length; i++)
        {
            if (totalQuotes[i].DisplayName.Contains("Genera"))
            {
                if (totalQuotes[i].DisplayName.Contains("RRD"))
                {
                    totalQuotes[i].DisplayName = totalQuotes[i].DisplayName.Replace("Genera", "");
                }
                else
                {
                    //totalQuotes[i].DisplayName = "Genera";
                }

            }

            #region For Genera, set BillTo

            if (quoteData.mode.Equals("ws") && quoteData.username == AppCodeConstants.un_genera)
            {
                if (totalQuotes[i].DisplayName.Contains("Volume") || totalQuotes[i].DisplayName == "RRD Truckload")
                {
                    totalQuotes[i].BillTo = "TForce Worldwide 1000 Wyndham Pkwy Bolingbrook, IL 60490";
                }
                else if (totalQuotes[i].DisplayName.Contains("Genera"))
                {
                    totalQuotes[i].BillTo = "2800 Saturn Street, Brea, CA 92821";
                }
                else
                {
                    Genera genera = new Genera();
                    totalQuotes[i].BillTo = genera.Get_Genera_bill_to(totalQuotes[i].Scac);
                }

            }
            else
            {
                // Do nothing
            }

            #endregion

        }

        carriersResult.totalQuotes = totalQuotes;

        #region UpdateSqlStatsAtEndOfRateQuote

        try
        {
            repo.UpdateSqlStatsAtEndOfRateQuote(ref trueDestZip, ref trueOrigZip, ref totalQuotes);
        }
        catch (Exception updateStatsEx)
        {
            DB.Log("UpdateSqlStatsAtEndOfRateQuote", updateStatsEx.ToString());
        }

        #endregion

        // If Genera, insert into API tables, to get the booking key
        // Enable this logic, to use later for any booking

        //DB.LogGenera("quoteData.mode", "quoteData.mode", quoteData.mode);

        if (quoteData.username == AppCodeConstants.un_genera && quoteData.mode == "LTLRaterLive")
        {
            //DB.LogGenera("ADDING BOOKING KEY", "ADDING BOOKING KEY", "ADDING BOOKING KEY");
            // Genera Live GCM
            string requestId;
            string data = "LiveGCM", response_string = "";
            var public_helper = new gcmAPI.Models.Public.LTL.Helper();
            public_helper.StoreLTLRequestsSql(
                ref quoteData, ref totalQuotes, totalQuotes[0].NewLogId, data, response_string, out requestId);


            //for (byte i = 0; i < totalQuotes.Length; i++)
            //{
            //    DB.LogGenera("BookingKey", "BookingKey", totalQuotes[i].BookingKey);
            //}
        }

        if (quoteData.mode.Equals("ws") && quoteData.username == AppCodeConstants.un_genera)
        {
            if (quoteData.totalUnits > 9)
            {
                Genera genera = new Genera();
                genera.Send_email_over_9_pallets(ref quoteData, ref newLogId);
            }
        }

        return carriersResult;
    }

    #endregion

    #region CARRIERS

    #region DLS

    #region GetResultObjectFromDLS

    private void GetResultObjectFromDLS()
    {
        if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera) //quoteData.is_Genera_quote == true
        {
            return;
        }
        else
        {
            // Do nothing
        }

        //--
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        //--

        // Set quote info to Session for DLS booking (on DLS)
        #region Set quote info to Session for DLS booking (on DLS)

        HelperFuncs.dlsShipInfo packInfo = new HelperFuncs.dlsShipInfo();

        #region Add items

        List<HelperFuncs.dlsItem> items = new List<HelperFuncs.dlsItem>();

        for (byte i = 0; i < m_lPiece.Length; i++)
        {
            HelperFuncs.dlsItem item = new HelperFuncs.dlsItem();
            item.weight = m_lPiece[i].Weight.ToString();
            item.fClass = m_lPiece[i].FreightClass;
            item.length = m_lPiece[i].Length.ToString();
            item.width = m_lPiece[i].Width.ToString();
            item.height = m_lPiece[i].Height.ToString();

            items.Add(item);
        }

        packInfo.Items = items.ToArray();

        #endregion

        #region Add from and to info
        //packInfo.fromAddressLine1 = "Ship From Street";
        packInfo.fromCity = quoteData.origCity;
        packInfo.fromState = quoteData.origState;
        packInfo.fromPostalCode = midOrigZip;
        packInfo.fromCountryCode = "US"; //to do check if Canada
        //packInfo.fromName = "ABC Associates";
        //packInfo.fromAttentionName = "Mr.ABC";

        //packInfo.toAddressLine1 = "Some Street";
        packInfo.toCity = quoteData.destCity;
        packInfo.toState = quoteData.destState;
        packInfo.toPostalCode = midDestZip;
        packInfo.toCountryCode = "US";
        //packInfo.toName = "DEF Associates";
        //packInfo.toAttentionName = "DEF";
        //packInfo.toPhone = "1234567890";
        #endregion

        //Session["dlsShipInfo"] = packInfo;

        // To do SetToSession
        carriersResult.packInfo = packInfo;

        #endregion

        bool guaranteedService = false;//, is_cust_rates = false

        Logins.Login_info login_info;
        Logins logins = new Logins();
        logins.Get_login_info(104, out login_info);

        string UserName = login_info.username, APIKey = login_info.API_Key;
        
        //DB.LogGenera("credentials test", "UserName,APIKey", UserName + " " + APIKey);

        bool is_Estes_HHG_Under_500 = false, is_Estes_HHG_Under_500_GLTL = false;
        dlsPricesheets = new List<DLS.dlsPricesheet>();
        DLS dls = new DLS(ref dlsPricesheets, ref quoteData, guaranteedService,
            ref UserName, ref APIKey, ref is_Estes_HHG_Under_500, ref is_Estes_HHG_Under_500_GLTL);


        //string GS = "";
        //SharedLTL.getRateFromDLS(ref dlsPricesheets, quoteData.puDate.ToShortDateString(), ref m_lPiece, ref quoteData, ref AccessorialsObj, ref quoteData.isHazmat,
        //    ref GS, ref quoteData.origCountry, ref quoteData.destCountry, ref quoteData.isHHG, ref quoteData.isUSED);

        //elapsed_milliseconds_DLS

        //--
        stopwatch.Stop();
        elapsed_milliseconds_DLS = (int)stopwatch.ElapsedMilliseconds;
        //--
    }

    #endregion

    #region GetResultObjectFromDLS_GLTL

    private void GetResultObjectFromDLS_GLTL()
    {
        bool validForNetNet = false, validForLive = false;

        if (quoteData.mode.Equals("NetNet") && AccessorialsObj.RESDEL.Equals(false) && AccessorialsObj.LGDEL.Equals(false))
        {
            validForNetNet = true;
        }

        if (AccessorialsObj.TRADEPU.Equals(false))
        {
            validForLive = true;
        }

        if ((quoteData.mode.Equals("NetNet") && validForNetNet.Equals(true)) || validForLive.Equals(true))
        {
            //DB.Log("GetResultObjectFromDLS_GLTL", "GetResultObjectFromDLS_GLTL");

            bool guaranteedService = true;

            Logins.Login_info login_info;
            Logins logins = new Logins();
            logins.Get_login_info(104, out login_info);

            string UserName = login_info.username, APIKey = login_info.API_Key;
           
            bool is_Estes_HHG_Under_500 = false, is_Estes_HHG_Under_500_GLTL = false;

            dlsPricesheetsGLTL = new List<DLS.dlsPricesheet>();
            DLS dls = new DLS(ref dlsPricesheetsGLTL, ref quoteData, guaranteedService,
                ref UserName, ref APIKey, ref is_Estes_HHG_Under_500, ref is_Estes_HHG_Under_500_GLTL);
            
        }
        else
        {
            // Do nothing
        }
    }

    #endregion

    #region GetResultObjectFromDLS_cust_rates

    private void GetResultObjectFromDLS_cust_rates()
    {
        if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera) //quoteData.is_Genera_quote == true
        {
            return;
        }
        else
        {
            // Do nothing
        }

        bool guaranteedService = false;

        Logins.Login_info login_info;
        Logins logins = new Logins();
        logins.Get_login_info(107, out login_info);

        string UserName = login_info.username, APIKey = login_info.API_Key;
       
        bool is_Estes_HHG_Under_500 = false, is_Estes_HHG_Under_500_GLTL = false;

        dls_cust_rates_Pricesheets = new List<DLS.dlsPricesheet>();
        DLS dls = new DLS(ref dls_cust_rates_Pricesheets, ref quoteData, guaranteedService,
            ref UserName, ref APIKey, ref is_Estes_HHG_Under_500, ref is_Estes_HHG_Under_500_GLTL);

    }

    #endregion

    #region GetResultObjectFromDLS_BenFranklinCraftsMacon

    private void GetResultObjectFromDLS_BenFranklinCraftsMacon()
    {
        if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera) //quoteData.is_Genera_quote == true
        {
            return;
        }
        else
        {
            // Do nothing
        }

        bool guaranteedService = false;

        Logins.Login_info login_info;
        Logins logins = new Logins();
        logins.Get_login_info(108, out login_info);

        string UserName = login_info.username, APIKey = login_info.API_Key;
       
        bool is_Estes_HHG_Under_500 = false, is_Estes_HHG_Under_500_GLTL = false;

        dlsPricesheets_BenFranklinCraftsMacon = new List<DLS.dlsPricesheet>();
        DLS dls = new DLS(ref dlsPricesheets_BenFranklinCraftsMacon, ref quoteData, guaranteedService,
            ref UserName, ref APIKey, ref is_Estes_HHG_Under_500, ref is_Estes_HHG_Under_500_GLTL);

    }

    #endregion

    #region GetResultObjectFromDLS_Genera

    private void GetResultObjectFromDLS_Genera()
    {
        //--
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        //--

        if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera) //quoteData.is_Genera_quote == true || 
        {
            // Do nothing
            // quoteData.username == AppCodeConstants.un_genera for LiveGCM and API
        }
        else
        {
            return;
        }

        bool guaranteedService = false;

        Logins.Login_info login_info;
        Logins logins = new Logins();
        logins.Get_login_info(109, out login_info);

        string UserName = login_info.username, APIKey = login_info.API_Key;
        
        bool is_Estes_HHG_Under_500 = false, is_Estes_HHG_Under_500_GLTL = false;

        DB.Log("STARTING GENERA DLS", "START");

        dlsPricesheets_Genera = new List<DLS.dlsPricesheet>();
        DLS dls = new DLS(ref dlsPricesheets_Genera, ref quoteData, guaranteedService,
            ref UserName, ref APIKey, ref is_Estes_HHG_Under_500, ref is_Estes_HHG_Under_500_GLTL);

        DB.Log("STARTING GENERA DLS", "END");

        if(dlsPricesheets_Genera != null)
        {
            DB.Log("dlsPricesheets_Genera count", dlsPricesheets_Genera.Count.ToString());
        }
        else
        {
            DB.Log("dlsPricesheets_Genera count", "was null");
        }

        //--
        stopwatch.Stop();
        carriersResult.elapsed_milliseconds_DLS_Genera = (int)stopwatch.ElapsedMilliseconds;
        //--

    }

    #endregion

    #region GetResultObjectFromDLS_HHG_Under_500

    private void GetResultObjectFromDLS_HHG_Under_500()
    {
        if (quoteData.isHHG_AndUnder500 == true)
        {
            // Do nothing
            //DB.Log("GetResultObjectFromDLS_HHG_Under_500", "isHHG_AndUnder500 true");
        }
        else
        {
            //DB.Log("GetResultObjectFromDLS_HHG_Under_500", "isHHG_AndUnder500 false");
            return;
        }

        bool guaranteedService = false;

        Logins.Login_info login_info;
        Logins logins = new Logins();
        
        string UserName = "", APIKey = "";

        if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
        {
            logins.Get_login_info(109, out login_info);
            UserName = login_info.username;
            APIKey = login_info.API_Key;        
        }
        else
        {
            logins.Get_login_info(104, out login_info);
            UserName = login_info.username;
            APIKey = login_info.API_Key;          
        }

        bool is_Estes_HHG_Under_500 = true, is_Estes_HHG_Under_500_GLTL = false;
        dlsPricesheets_HHG_Under_500 = new List<DLS.dlsPricesheet>();
        DLS dls = new DLS(ref dlsPricesheets_HHG_Under_500, ref quoteData, guaranteedService,
            ref UserName, ref APIKey, ref is_Estes_HHG_Under_500, ref is_Estes_HHG_Under_500_GLTL);

        if (dlsPricesheets_HHG_Under_500 != null && dlsPricesheets_HHG_Under_500.Count > 0)
        {
            got_Estes_HHG_Under_500_rate = true;
        }
    }

    #endregion

    #region GetResultObjectFromDLS_GLTL_HHG_Under_500

    private void GetResultObjectFromDLS_GLTL_HHG_Under_500()
    {
        if (quoteData.isHHG_AndUnder500 == true)
        {
            // Do nothing
            //DB.Log("GetResultObjectFromDLS_HHG_Under_500", "isHHG_AndUnder500 true");
        }
        else
        {
            //DB.Log("GetResultObjectFromDLS_HHG_Under_500", "isHHG_AndUnder500 false");
            return;
        }


        bool validForNetNet = false, validForLive = false;

        if (quoteData.mode.Equals("NetNet") && AccessorialsObj.RESDEL.Equals(false) && AccessorialsObj.LGDEL.Equals(false))
        {
            validForNetNet = true;
        }

        if (AccessorialsObj.TRADEPU.Equals(false))
        {
            validForLive = true;
        }

        if ((quoteData.mode.Equals("NetNet") && validForNetNet.Equals(true)) || validForLive.Equals(true))
        {
            //DB.Log("GetResultObjectFromDLS_GLTL", "GetResultObjectFromDLS_GLTL");

            bool guaranteedService = true;

            Logins.Login_info login_info;
            Logins logins = new Logins();
            logins.Get_login_info(104, out login_info);

            string UserName = login_info.username, APIKey = login_info.API_Key;
           
            bool is_Estes_HHG_Under_500 = false, is_Estes_HHG_Under_500_GLTL = true;

            dlsPricesheetsGLTL_HHG_Under_500 = new List<DLS.dlsPricesheet>();
            DLS dls = new DLS(ref dlsPricesheetsGLTL_HHG_Under_500, ref quoteData, guaranteedService,
                ref UserName, ref APIKey, ref is_Estes_HHG_Under_500, ref is_Estes_HHG_Under_500_GLTL);

        }
        else
        {
            // Do nothing
        }
    }

    #endregion

    #endregion

    #region Estes DLS account

    private void GetResultObjectFromEstesDLS_account()
    {
        // Testing
        return;

        try
        {
            //DB.LogGenera("LTL_Carriers", "GetResultObjectFromEstesDLS_account", "hit function");

            if (AccessorialsObj.TRADEDEL || AccessorialsObj.TRADEPU) //AccessorialsObj.APTPU || 
            {
                return;
            }

            if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
            {
                // Do nothing
                // quoteData.username == AppCodeConstants.un_genera for LiveGCM and API
            }
            else
            {
                return;
            }

            //--
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Logins.Login_info login_info;
            Logins logins = new Logins();
            logins.Get_login_info(111, out login_info);

            CarrierAcctInfo acctInfo = new CarrierAcctInfo();
            acctInfo.acctNum = login_info.account;
            acctInfo.chargeType = "C";
            acctInfo.terms = "PPD";
            acctInfo.username = login_info.username;
            acctInfo.password = login_info.password;

            acctInfo.bookingKey = "#1#";
            acctInfo.displayName = "Estes Genera";
            acctInfo.carrierKey = "Estes";

            Estes estes = new Estes(ref acctInfo, ref quoteData);

            estes.GetResultObjectFromEstesExpress(out estesQuote_DLS_account);

            //--
            stopwatch.Stop();

            //--
            //DB.LogGenera("LTL_Carriers", "GetResultObjectFromEstesDLS_account", "end function");
        }
        catch (Exception e)
        {
            DB.LogGenera("LTL_Carriers", "GetResultObjectFromEstesDLS_account", e.ToString());
        }

    }

    #endregion

    #region Estes DLS account

    private void GetResultObjectFromEstesGenera_account()
    {
        // Testing
        //return;

        try
        {
            //DB.LogGenera("LTL_Carriers", "GetResultObjectFromEstesDLS_account", "hit function");

            if (AccessorialsObj.TRADEDEL || AccessorialsObj.TRADEPU) //AccessorialsObj.APTPU || 
            {
                return;
            }

            if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
            {
                // Do nothing
                // quoteData.username == AppCodeConstants.un_genera for LiveGCM and API
            }
            else
            {
                return;
            }

            //--
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Logins.Login_info login_info;
            Logins logins = new Logins();
            logins.Get_login_info(119, out login_info);

            CarrierAcctInfo acctInfo = new CarrierAcctInfo();
            acctInfo.acctNum = login_info.account;
            acctInfo.chargeType = "S"; // S T C
            acctInfo.terms = "PPD";
            acctInfo.username = login_info.username;
            acctInfo.password = login_info.password;

            acctInfo.bookingKey = "#1#";
            acctInfo.displayName = "Estes Genera";
            acctInfo.carrierKey = "Estes";

            Estes estes = new Estes(ref acctInfo, ref quoteData);

            estes.GetResultObjectFromEstesExpress(out estesQuote_Genera_account);

            //--
            stopwatch.Stop();

            //--
            //DB.LogGenera("LTL_Carriers", "GetResultObjectFromEstesDLS_account", "end function");
        }
        catch (Exception e)
        {
            DB.LogGenera("LTL_Carriers", "GetResultObjectFromEstesDLS_account", e.ToString());
        }

    }

    #endregion

    #region ODFL

    private void GetResultObjectFromOldDominion()
    {
        // The dura account only uses orig zip 08854, a different zip gives error result from ODFL
        if (quoteData.origZip == "08854")
        {
            // Do nothing
        }
        else
        {
            return;
        }

        Logins.Login_info login_info;
        Logins logins = new Logins();
        logins.Get_login_info(112, out login_info);

        CarrierAcctInfo acctInfo = new CarrierAcctInfo
        {
            username = login_info.username,
            password = login_info.password,
            acctNum = login_info.account,
            terms = "S", // Default terms
            //terms = "C",
            bookingKey = "#1#",
            displayName = "Old Dominion Freight Line",
            carrierKey = "ODFL"

        };

        ODFL odfl = new ODFL(ref acctInfo, ref quoteData);
        odfl.SetOldDominionAccountNumber();
        odfl.GetResultObjectFromOldDominion_XML(ref odflQuote);

    }

    #endregion

    #region SEFL

    #region GetResultObjectFromSEFL_AnyAccount
    // Account is not active
    private void GetResultObjectFromSEFL_AnyAccount()
    {
        //DB.Log("GetResultObjectFromSEFL_AnyAccount", "");

        Logins.Login_info login_info;
        Logins logins = new Logins();
        logins.Get_login_info(24, out login_info);

        if (!AccessorialsObj.RESPU && !AccessorialsObj.RESDEL
                && !AccessorialsObj.CONPU && !AccessorialsObj.CONDEL) //&& !AccessorialsObj.APTPU
        {
            GetResultObjectFromSEFL(login_info.username, login_info.password, login_info.account);

        }
        else
        {
            seflQuote = null;
        }
    }

    #endregion

    #region GetResultObjectFromSEFL
    // Account is not active
    private void GetResultObjectFromSEFL(string user, string pass, string account)
    {
        //DB.Log("GetResultObjectFromSEFL", "");

        if (AccessorialsObj.TRADEPU.Equals(true) || AccessorialsObj.TRADEDEL.Equals(true))
        {
            seflQuote = null;
            return;
        }

        string doc = "";
        try
        {
            #region Variables
            int overlengthFee = 0;
            string origCountry = "U";
            string destCountry = "U";

            #region Fix country in case of Canada
            if (HelperFuncs.GetCountryByZip(midOrigZip, true, quoteData.origZip, quoteData.destZip).Equals("CANADA"))
            {
                origCountry = "C";
            }
            if (HelperFuncs.GetCountryByZip(midDestZip, false, quoteData.origZip, quoteData.destZip).Equals("CANADA"))
            {
                destCountry = "C";
            }
            #endregion

            string temp = quoteData.puDate.ToShortDateString();
            string[] dateArray = temp.Split('/');

            string url = "https://www.sefl.com/webconnect/ratequotes";

            int lineItemCount = 0;
            //int accOptionCount = 0;

            string strLineItem = "";
            string strAccOptions = "";

            #endregion

            for (int i = 1; i <= m_lPiece.Length; i++)
            {
                string itemClass = m_lPiece[i - 1].FreightClass;

                #region class fix
                if (itemClass == "92.5")
                {
                    itemClass = "925";
                }
                else if (itemClass == "77.5")
                {
                    itemClass = "775";
                }
                #endregion

                strLineItem += "&Class" + i.ToString() + "=" + itemClass + "&Weight" + i.ToString() + "=" + m_lPiece[i - 1].Weight;
                //+ "&Description" + i.ToString() + "="

            }

            // Get Overlenth Fee
            HelperFuncs.GetOverlengthFee(ref m_lPiece, ref overlengthFee, 143, 216, 288, 100, 150, 200);

            #region Accessorials
            if (AccessorialsObj.INSDEL)
            {
                strAccOptions += "&chkID=on";
            }
            //if (InsPick"].Equals("true"))
            //{
            //    strAccOptions += "&chkIP=on";
            //}
            if (AccessorialsObj.LGPU)
            {
                strAccOptions += "&chkLGP=on";
            }
            if (AccessorialsObj.LGDEL)
            {
                strAccOptions += "&chkLGD=on";
            }
            if (AccessorialsObj.APTDEL)
            {
                strAccOptions += "&chkAN=on";
            }
            if (quoteData.isHazmat)
            {
                strAccOptions += "&chkHM=on";
            }
            #endregion

            //--------------------------------------------------------------------------------------------------
            CookieContainer container = new CookieContainer();
            string referrer, contentType, accept, method;

            url = string.Concat("https://www.sefl.com/webconnect/ratequotes?Username=", user, "&Password=", pass, "&CustomerAccount=", account,
                "&returnX=Y&rateXML=Y", //return in xml format
                "&PickupDateMM=", dateArray[0], "&PickupDateDD=", dateArray[1], "&PickupDateYYYY=", dateArray[2],
                "&Terms=P",
                "&OriginZip=", midOrigZip, "&OrigCountry=", origCountry,
                "&DestinationZip=", midDestZip, "&DestCountry=", destCountry,
                "&CustomerCity=Burien&CustomerName=AES Logistics&CustomerState=WA&CustomerStreet=140 S.W. 153rd Street",
                "&CustomerZip=98166",
                strLineItem, strAccOptions);
            referrer = "";
            contentType = "";
            accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            method = "GET";
            doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", true);

            //--------------------------------------------------------------------------------------------------

            #region Get cost and transit from response
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(doc);

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("rateQuote");
            double totalCharges = 0;
            double temp1 = 0;
            if (nodeList.Count > 0)
            {
                if (double.TryParse(nodeList[0].InnerText.Trim(), out temp1))
                {
                    totalCharges = Convert.ToDouble(nodeList[0].InnerText.Trim());
                }
            }

            nodeList = xmlDoc.GetElementsByTagName("transitTime");
            int standardDays = 0;
            int temp2 = 0;
            if (nodeList.Count > 0)
            {
                if (int.TryParse(nodeList[0].InnerText.Trim(), out temp2))
                {
                    standardDays = Convert.ToInt32(nodeList[0].InnerText.Trim());
                }
            }

            if (totalCharges > 0)
            {
                totalCharges += overlengthFee;
            }

            #endregion

            if (totalCharges > 0 && AccessorialsObj.APTPU)
            {
                totalCharges += 13;
            }

            if (totalCharges > 0)
            {
                #region Set info to objQuote

                seflQuote = SetInfoToObjectQuote(ref totalCharges, strSEFLDisplay, "#1#", "SEFL", "", standardDays, "SEFL");

                //DB.Log("SEFL (Live)", "charges: " + totalCharges.ToString(), "");
                #endregion
            }
            else
            {
                seflQuote = null;
            }

            //DB.Log("SEFL (Live)", "response: " + doc);
        }
        catch (Exception exp)
        {
            seflQuote = null;
            //DB.Log("SEFL (Live)", exp.ToString());
        }
    }

    #endregion

    #endregion

    #region YRC

    #region GetResultObjectFrom_YRC_Spot_Quote

    private void GetResultObjectFrom_YRC_Spot_Quote()
    {
        try
        {
            //DB.Log("YRC Volume  ", "start function");

            #region Build Accessorials string

            StringBuilder accessorials = new StringBuilder();

            if (AccessorialsObj.LGPU || AccessorialsObj.LGDEL ||
                AccessorialsObj.CONPU || AccessorialsObj.CONDEL ||
                AccessorialsObj.RESPU || AccessorialsObj.RESDEL ||
                AccessorialsObj.INSDEL || AccessorialsObj.APTPU)
            {
                return;
            }

            accessorials.Append("\"APPTDEL\":\"true\""); // Adding by default

            #endregion

            #region Build Items string

            int total_units = 0;

            StringBuilder items = new StringBuilder();

            for (byte i = 0; i < m_lPiece.Length; i++)
            {
                // Guard
                if (m_lPiece[i].Length > 48 || m_lPiece[i].Width > 48)
                {
                    return;
                }

                total_units += m_lPiece[i].Units;
            }

            //DB.Log("YRC items", items.ToString());

            #endregion

            // Guard
            if (total_units < 4)
            {
                return;
            }

            //DB.Log("YRC total_units", total_units.ToString());
            //int Total_lineal_feet = 2;
            int Total_lineal_feet = total_units * 2;

            if (quoteData.linealFeet > 0.0) // Requested by XML GCM API
            {
                Total_lineal_feet = Convert.ToInt32(quoteData.linealFeet);
            }

            yrc_volume_result = new YRC.Volume_result();
            Get_YRC_API_Spot_Quote_Volume(ref yrc_volume_result);

            //DB.Log("From gcmAPI_Demo YRC result cost", yrc_volume_result.cost.ToString());
        }
        catch (Exception e)
        {
            DB.Log("GetResultObjectFrom_YRC_Volume", e.ToString());
        }
    }

    #endregion

    #region Get_YRC_API_Spot_Quote_Volume

    public string Get_YRC_API_Spot_Quote_Volume(ref YRC.Volume_result result)
    {
        try
        {
            Web_client http = new Web_client
            {
                url = "",
                content_type = "",
                accept = "*/*",
                method = "GET"
            };

            //HelperFuncs.writeToSiteErrors("gcmAPI_Get_LOUP_Rates before send request", "before send request");

            //string doc = http.Make_http_request();

            #region Set pickup date variables

            DateTime puDate = DateTime.Today;
            string puDateDay = puDate.Day.ToString(), puDateMonth = puDate.Month.ToString();

            if (puDateDay.Length == 1)
                puDateDay = "0" + puDateDay;
            if (puDateMonth.Length == 1)
                puDateMonth = "0" + puDateMonth;

            #endregion

            #region Build Accessorials string

            StringBuilder accessorials = new StringBuilder();

            byte accessorials_count = 1;

            accessorials.Append("&AccOption1=NTFY"); // Adding by default

            if (AccessorialsObj.APTDEL)
            {
                accessorials_count++;
                accessorials.Append(string.Concat("&AccOption", accessorials_count, "=APPT"));
            }

            if (AccessorialsObj.TRADEPU)
            {
                accessorials_count++;
                accessorials.Append(string.Concat("&AccOption", accessorials_count, "=SHWO"));
            }

            if (AccessorialsObj.TRADEDEL)
            {
                accessorials_count++;
                accessorials.Append(string.Concat("&AccOption", accessorials_count, "=SHWD"));
            }

            if (quoteData.isHazmat)
            {
                accessorials_count++;
                accessorials.Append(string.Concat("&AccOption", accessorials_count, "=HAZM"));
            }

            //DB.Log("YRC accessorials", accessorials.ToString());

            #endregion

            #region Items

            StringBuilder items = new StringBuilder();

            int line_number;

            for (int i = 0; i < m_lPiece.Length; i++)
            {
                line_number = i + 1;

                items.Append(string.Concat(

                    "&LineItemPackageLength", line_number, "=48",
                    "&LineItemPackageWidth", line_number, "=48",
                    "&LineItemPackageHeight", line_number, "=70",
                    "&LineItemHandlingUnits", line_number, "=", m_lPiece[i].Units,
                    "&LineItemWeight", line_number, "=", (int)m_lPiece[i].Weight

                ));
            }

            #endregion

            string Orig_country = "USA", Dest_country = "USA";

            if (quoteData.origCountry.Equals("CANADA"))
            {
                Orig_country = "CAN";
            }

            if (quoteData.destCountry.Equals("CANADA"))
            {
                Dest_country = "CAN";
            }

            Logins.Login_info login_info;
            Logins logins = new Logins();
            logins.Get_login_info(113, out login_info);
            
            string username = login_info.username, password = login_info.password, account_number = login_info.account;

            //

            // Service types: STD/TCS/ALL/TCSA/TCSP/TCSW/DEGP/DEGX/GDEL/FAF/DEG/DEGN/SPOT/ACEL
            // Result types: QUOTE/MATRX/TABLE/M/D
            //DB.Log("YRC Items", items.ToString(), ref connection_string);

            http.method = "POST";
            //http.referrer = http.url;
            http.url = "https://my.yrc.com/dynamic/national/servlet";

            http.post_data = string.Concat(
                "CNTR=&AccOptionCount=", accessorials_count,
                accessorials,
                //"&AccOption1=NTFY",
                "&AcceptTerms=Y",
                "&DestZipCode=", quoteData.destZip, "&OrigNationCode=", Orig_country,

                items,

                "&LineItemCount=", quoteData.m_lPiece.Length,
                //"&LOGIN_USER=",
                "&LOGIN_USER=", username,
                "&DestCityName=", quoteData.destCity,
                //"&BusId=",
                "&BusId=", account_number,
                "&redir=%2Ftfq561",
                "&OrigZipCode=", quoteData.origZip,
                "&BusRole=Third Party&OrigCityName=", quoteData.origCity,
                "&DestStateCode=", quoteData.destState,
                "&PickupDate=", puDate.Year, puDateMonth, puDateDay,
                "&CONTROLLER=com.rdwy.ec.rexcommon.proxy.http.controller.ProxyApiController",
                "&ServiceClass=SPOT",
                //"&ServiceClass=ALL",
                //"&ResultType=QUOTE",
                "&PaymentTerms=Prepaid&OrigStateCode=", quoteData.origState,
                "&DestNationCode=", Dest_country,
                //"&LOGIN_USERID=&LOGIN_PASSWORD="
                "&LOGIN_USERID=", username, "&LOGIN_PASSWORD=", password
                );

            //DB.Log("YRC post_data volume", http.post_data);

            string doc = http.Make_http_request();

            //DB.Log("YRC response volume", doc);

            #region Parse result

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(doc);
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("TotalCharges");
            double TotalCharges = 0;
            if (nodeList.Count > 0)
            {
                TotalCharges = Convert.ToDouble(nodeList[0].InnerText);
            }

            TotalCharges = TotalCharges / 100;

            nodeList = xmlDoc.GetElementsByTagName("StandardDate");
            DateTime StandardDate = DateTime.MaxValue;

            string delivery_date = nodeList[0].InnerText;

            string delivery_year = "";
            string delivery_month = "";
            string delivery_day = "";

            try
            {
                delivery_year = delivery_date.Remove(4);

                delivery_date = delivery_date.Substring(4);

                delivery_month = delivery_date.Remove(2);

                delivery_day = delivery_date.Substring(2);
            }
            catch
            {
                // Do nothing
            }

            int transit_days = 10;
            if (DateTime.TryParse(string.Concat(delivery_month, "/", delivery_day, "/", delivery_year), out StandardDate))
            {
                transit_days = Convert.ToInt32((StandardDate - DateTime.Today).TotalDays);
            }

            nodeList = xmlDoc.GetElementsByTagName("QuoteId");
            string QuoteId = string.Empty;
            if (nodeList.Count > 0)
            {
                QuoteId = nodeList[0].InnerText;
            }

            nodeList = xmlDoc.GetElementsByTagName("ReferenceId");
            string ReferenceId = string.Empty;
            if (nodeList.Count > 0)
            {
                ReferenceId = nodeList[0].InnerText;
            }

            #endregion

            result.cost = TotalCharges;
            result.quote_number = QuoteId;
            result.transit_days = transit_days;
            result.carrier_name = "YRC Spot Quote";

            //return "test yrc";

            //DB.Log("YRC response volume", string.Concat("cost=", result.cost, "&quote_number=", result.quote_number,
            //"&transit_days=", result.transit_days, "&carrier_name=", result.carrier_name));

            return string.Concat("cost=", result.cost, "&quote_number=", result.quote_number,
                "&transit_days=", result.transit_days, "&carrier_name=", result.carrier_name);


            //return doc;
        }
        catch (Exception e)
        {
            DB.Log("YRC exception volume", e.ToString());
            return string.Concat("test yrc exception ", e.ToString());
        }
    }

    #endregion
    
    #region GetResultObjectFromYRC_API

    private void GetResultObjectFromYRC_API()
    {
        yrcQuote = new GCMRateQuote();

        Logins.Login_info login_info;
        Logins logins = new Logins();
        logins.Get_login_info(56, out login_info);

        CarrierAcctInfo acctInfo = new CarrierAcctInfo
        {
            acctNum = login_info.account,
            chargeType = "Prepaid",
            bookingKey = "#1#",
            displayName = "YRC Freight",
            carrierKey = "YRC",
            username = login_info.username,
            password = login_info.password
        };

        if (quoteData.username.ToLower().Contains("vanguard"))
        {
            acctInfo.acctNum = "05269132309";
        }

        if (quoteData.is_dura_logic == true) //account specific login
        {
            acctInfo.acctNum = "13203032805"; //13203032805 //71084617679

            //If class 55 pass nmfc 101720-3
            //If class 125 pass nmfc 41027
        }

        YRC yrc = new YRC(ref quoteData, ref acctInfo);
        yrc.GetResultObjectFromYRC_API(ref yrcQuote);
    }

    #endregion

    #region GetResultObjectFromYRC_API_SPC

    private void GetResultObjectFromYRC_API_SPC()
    {
        yrcQuoteSPC = new GCMRateQuote();

        Logins.Login_info login_info;
        Logins logins = new Logins();
        logins.Get_login_info(60, out login_info);

        CarrierAcctInfo acctInfo = new CarrierAcctInfo
        {
            acctNum = login_info.account,

            chargeType = "Prepaid",
            bookingKey = "---2---",
            displayName = "YRC SPC Pricing",
            carrierKey = "YRCSPC",
            username = login_info.username,
            password = login_info.password
        };

        YRC yrc = new YRC(ref quoteData, ref acctInfo);
        yrc.GetResultObjectFromYRC_API(ref yrcQuoteSPC);
    }

    #endregion

    #endregion

    #region R&L

    #region GetResultObjectFromRAndL

    private void GetResultObjectFromRAndL()
    {

        //DB.Log("GetResultObjectFromRAndL", "");

        if (quoteData.is_dura_logic == true) // Account specific login
        {
            Logins.Login_info login_info;
            Logins logins = new Logins();
            logins.Get_login_info(15, out login_info);

            CarrierAcctInfo acctInfo = new CarrierAcctInfo();
            acctInfo.username = login_info.username;
            acctInfo.password = login_info.password;
            acctInfo.carrierKey = "R+L";
            acctInfo.bookingKey = "#1#";
            acctInfo.displayName = "R&L Carrier";

            string API_KeyRL_Durachem = login_info.API_Key;
            RL rl = new RL(acctInfo, ref quoteData, ref API_KeyRL_Durachem);
            rl_quote_guaranteed = new GCMRateQuote();
            rlQuote = rl.GetRateFromRL(ref rl_quote_guaranteed);

        }
    }

    #endregion

    #region GetResultObjectFromRAndLGriots
    // Not active
    //private void GetResultObjectFromRAndLGriots()
    //{
    //    Logins.Login_info login_info;
    //    Logins logins = new Logins();
    //    logins.Get_login_info(16, out login_info);

    //    CarrierAcctInfo acctInfo = new CarrierAcctInfo();
    //    acctInfo.username = login_info.username;
    //    acctInfo.password = login_info.password;
    //    acctInfo.carrierKey = "R+LGriots";
    //    acctInfo.bookingKey = "#1#";
    //    acctInfo.displayName = "R&L Carrier";

    //    string API_KeyRL_Durachem = login_info.API_Key;
    //    RL rl = new RL(acctInfo, ref quoteData, ref API_KeyRL_Durachem);
    //    rlQuoteGriots = rl.GetRateFromRL(ref rl_quote_guaranteed);

    
    //}

    #endregion

    #region GetResultObjectFromRAndL_Genera

    private void GetResultObjectFromRAndL_Genera()
    {
        if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
        {
            // Do nothing
            // quoteData.username == AppCodeConstants.un_genera for LiveGCM and API
        }
        else
        {
            return;
        }

        //--
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        //--

        Logins.Login_info login_info;
        Logins logins = new Logins();
        logins.Get_login_info(114, out login_info);

        CarrierAcctInfo acctInfo = new CarrierAcctInfo();
        acctInfo.username = login_info.username;
        acctInfo.password = login_info.password;
        acctInfo.carrierKey = "R+L";
        acctInfo.bookingKey = "#1#";
        acctInfo.displayName = "R&L Carrier - Genera";

        string api_key = login_info.API_Key;
        
        if (quoteData.origState == "TX" || quoteData.origState == "CA")
        {
            logins.Get_login_info(116, out login_info);
            api_key = login_info.API_Key;
            //DB.LogGenera("RL API Key", "RL API Key", api_key);
           
        }
        else
        {
        }

        RL rl = new RL(acctInfo, ref quoteData, ref api_key);
        rl_quote_guaranteed_Genera = new GCMRateQuote();
        rlQuote_Genera = rl.GetRateFromRL(ref rl_quote_guaranteed_Genera);

        //--
        stopwatch.Stop();
        rlQuote_Genera.Elapsed_milliseconds = (int)stopwatch.ElapsedMilliseconds;
        //DB.LogGenera("GetResultObjectFromBestOvernite_Genera", "SMTL_Quote_Genera.Elapsed_milliseconds", rlQuote_Genera.Elapsed_milliseconds.ToString());
        //--
    }

    #endregion

    #region GetResultObjectFromRAndLMWI

    private void GetResultObjectFromRAndLMWI()
    {
        //DB.Log("GetResultObjectFromRAndLMWI", "");

        Logins logins = new Logins();
        logins.Get_login_info(17, out Logins.Login_info login_info);

        CarrierAcctInfo acctInfo = new CarrierAcctInfo();
        acctInfo.username = login_info.username;
        acctInfo.password = login_info.password;
        acctInfo.carrierKey = "R+LMWI";
        acctInfo.bookingKey = "#1#";
        acctInfo.displayName = "R&L Carrier MWI Pricing";

        string API_KeyRL_Durachem = login_info.API_Key;
        RL rl = new RL(acctInfo, ref quoteData, ref API_KeyRL_Durachem);
        rlQuoteMWI = rl.GetRateFromRL(ref rl_quote_guaranteed);

    }

    #endregion

    #endregion

    #region RRTS

    #region GetResultObjectFromRoadRunner

    private void GetResultObjectFromRoadRunner()
    {
        //DB.Log("GetResultObjectFromRoadRunner", "");
        try
        {

            //DB.Log("regular", "regular");
            if (AccessorialsObj.TRADEPU || AccessorialsObj.TRADEDEL)
            {
                throw new Exception("tradeshow");
                //LoadFreightArray();
            }

            bool rateAsClass50 = false;
            int accountNumber = -1;
            bool isAAFES = false;

            if (quoteData.username.ToLower().Equals("standard") ||
                quoteData.username.ToLower().Equals("dupraafesbuy")) //account specific login
            {
                Logins logins = new Logins();
                logins.Get_login_info(92, out Logins.Login_info login_info);

                accountNumber = Convert.ToInt32(login_info.account);
                RRTS rrts = new RRTS(accountNumber, rateAsClass50, isAAFES, ref quoteData);
                objRoadRunnerResult = rrts.GetResultObjectFromRoadRunnerByAccount();

            }
            else
            {
                RRTS rrts = new RRTS(accountNumber, rateAsClass50, isAAFES, ref quoteData);
                objRoadRunnerResult = rrts.GetResultObjectFromRoadRunner();
            }
        }
        catch (SoapException ex)
        {
            string str = ex.ToString();
            objRoadRunnerResult = null;
            //if (!ex.Message.Contains("no standard service") && !ex.Message.Contains("not in the standard"))
            //    DB.Log("Roadrunner (Live)", ex.ToString());
        }
        catch (Exception ex)
        {
            objRoadRunnerResult = null;
            if (!ex.Message.Contains("no standard service") && !ex.Message.Contains("not in the standard") && !ex.Message.Contains("must be today") && !ex.Message.Contains("timed out"))
            {
                DB.Log("Roadrunner (Live)", ex.ToString());
            }
        }
    }

    #endregion

    #region GetResultObjectFromRoadRunnerClass50

    private void GetResultObjectFromRoadRunnerClass50()
    {
        //DB.Log("GetResultObjectFromRoadRunnerClass50", "");
        bool rateAsClass50 = true;

        Logins logins = new Logins();
        logins.Get_login_info(92, out Logins.Login_info login_info);

        int accountNumber = Convert.ToInt32(login_info.account);
        bool isAAFES = false;

        RRTS rrts = new RRTS(accountNumber, rateAsClass50, isAAFES, ref quoteData);
        objRoadRunnerResultClass50 = rrts.GetResultObjectFromRoadRunnerByAccount();
        //GetResultObjectFromRoadRunnerByAccount(ref accountNumber, ref rateAsClass50, ref isAAFES);
        //return;
    }

    #endregion

    #region GetResultObjectFromRRTS_ByAccountSPC

    private void GetResultObjectFromRRTS_ByAccountSPC()
    {
        Logins logins = new Logins();
        logins.Get_login_info(23, out Logins.Login_info login_info);

        int accountNumber = Convert.ToInt32(login_info.account);
        bool rateAsClass50 = false;
        bool isAAFES = false;

        RRTS rrts = new RRTS(accountNumber, rateAsClass50, isAAFES, ref quoteData);
        objRoadRunnerResultSPC = rrts.GetResultObjectFromRoadRunnerByAccount();

        //GetResultObjectFromRoadRunnerByAccount(ref accountNumber, ref rateAsClass50, ref isAAFES);
    }

    #endregion

    #region GetResultObjectFromRRTS_ByAccountAAFES

    private void GetResultObjectFromRRTS_ByAccountAAFES()
    {

        Logins logins = new Logins();
        logins.Get_login_info(92, out Logins.Login_info login_info);

        int accountNumber = Convert.ToInt32(login_info.account);
        bool rateAsClass50 = false;
        bool isAAFES = true;
        RRTS rrts = new RRTS(accountNumber, rateAsClass50, isAAFES, ref quoteData);
        objRoadRunnerResultAAFES = rrts.GetResultObjectFromRoadRunnerByAccount();
        //GetResultObjectFromRoadRunnerByAccount(ref accountNumber, ref rateAsClass50, ref isAAFES);
    }

    #endregion

    #endregion

    #region Pitt Ohio API

    #region GetResultObjectFromPittOhio_API

    private void GetResultObjectFromPittOhio_API()
    {
        Logins logins = new Logins();
        logins.Get_login_info(64, out Logins.Login_info login_info);

        CarrierAcctInfo acctInfo = new CarrierAcctInfo
        {
            username = login_info.username,
            password = login_info.password,
            terms = "3",
            bookingKey = "#1#",
            displayName = "Pitt Ohio",
            carrierKey = "Pitt Ohio"

        };

        PittOhio pittOhio = new PittOhio(ref acctInfo, ref quoteData);
        pittOhio.GetResultObjectFromPittOhio_API(ref pittOhioQuoteAPI);
    }

    #endregion

    #region GetResultObjectFromPittOhio_API_SPC

    private void GetResultObjectFromPittOhio_API_SPC()
    {
        Logins logins = new Logins();
        logins.Get_login_info(67, out Logins.Login_info login_info);

        CarrierAcctInfo acctInfo = new CarrierAcctInfo
        {
            username = login_info.username,
            password = login_info.password,
            terms = "P",
            bookingKey = "#1#",
            displayName = "Pitt Ohio",
            carrierKey = "Pitt OhioSPC"

        };

        PittOhio pittOhio = new PittOhio(ref acctInfo, ref quoteData);
        pittOhio.GetResultObjectFromPittOhio_API(ref pittOhioQuoteAPI_SPC);
    }

    #endregion

    #region GetResultObjectFromPittOhio_API_Durachem

    private void GetResultObjectFromPittOhio_API_Durachem()
    {
        Logins logins = new Logins();
        logins.Get_login_info(66, out Logins.Login_info login_info);

        CarrierAcctInfo acctInfo = new CarrierAcctInfo
        {
            username = login_info.username,
            password = login_info.password,
            //terms = "3",
            terms = "P",
            bookingKey = "#1#",
            displayName = "Pitt Ohio",
            carrierKey = "Pitt OhioDura"

        };

        PittOhio pittOhio = new PittOhio(ref acctInfo, ref quoteData);
        pittOhio.GetResultObjectFromPittOhio_API(ref pittOhioQuoteAPI_Durachem);
    }

    #endregion

    #endregion

    #region Genera Carriers

    #region GetResultObjectFromSMTL_Genera

    private void GetResultObjectFromSMTL_Genera()
    {

        if (AccessorialsObj.TRADEDEL || AccessorialsObj.TRADEPU)
        {
            return;
        }

        if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
        {
            // Do nothing
            // quoteData.username == AppCodeConstants.un_genera for LiveGCM and API
        }
        else
        {
            return;
        }

        //--
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        //--

        SMTL smtl = new SMTL(ref quoteData);

        smtl.Get_rates(out SMTL_Quote_Genera);

        //--
        stopwatch.Stop();
        SMTL_Quote_Genera.Elapsed_milliseconds = (int)stopwatch.ElapsedMilliseconds;
        //DB.LogGenera("GetResultObjectFromBestOvernite_Genera", "SMTL_Quote_Genera.Elapsed_milliseconds", SMTL_Quote_Genera.Elapsed_milliseconds.ToString());
        //--

    }

    #endregion

    #region GetResultObjectFromBestOvernite_Genera

    private void GetResultObjectFromBestOvernite_Genera()
    {
        try
        {
            if (AccessorialsObj.CONPU || AccessorialsObj.CONDEL || AccessorialsObj.TRADEDEL || AccessorialsObj.TRADEPU)
            {
                throw new Exception("Accessorial not supported");
            }

            if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
            {
                // Do nothing
                // quoteData.username == AppCodeConstants.un_genera for LiveGCM and API
            }
            else
            {
                return;
            }

            //--
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //--

            BestOvernite best_overnite = new BestOvernite(ref quoteData);

            best_overnite.Get_rates(out BestOvernite_Quote_Genera);

            //--
            stopwatch.Stop();
            BestOvernite_Quote_Genera.Elapsed_milliseconds = (int)stopwatch.ElapsedMilliseconds;
            //DB.LogGenera("GetResultObjectFromBestOvernite_Genera", "BestOvernite_Quote_Genera.Elapsed_milliseconds", BestOvernite_Quote_Genera.Elapsed_milliseconds.ToString());
            //--
        }
        catch (Exception e)
        {
            DB.LogGenera("GetResultObjectFromBestOvernite_Genera", "exception", e.ToString());
        }
    }

    #endregion

    #region GetResultObjectFromPyle_Genera

    private void GetResultObjectFromPyle_Genera()
    {
        try
        {
            //if (AccessorialsObj.CONPU || AccessorialsObj.CONDEL || AccessorialsObj.TRADEDEL || AccessorialsObj.TRADEPU)
            //{
            //    throw new Exception("Accessorial not supported");
            //}

            if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
            {
                // Do nothing
                // quoteData.username == AppCodeConstants.un_genera for LiveGCM and API
            }
            else
            {
                return;
            }

            if (quoteData.origZip == "08831")
            {
                // Do nothing
            }
            else
            {
                return;
            }

            #region Accessorials 

            if (quoteData.AccessorialsObj.RESPU)
            {
                return;
            }

            if (quoteData.AccessorialsObj.CONPU)
            {
                return;
            }

            if (quoteData.AccessorialsObj.APTPU)
            {
                return;
            }

            if (quoteData.AccessorialsObj.TRADEPU)
            {
                return;
            }
            if (quoteData.AccessorialsObj.TRADEDEL)
            {
                return;
            }

            #endregion


            //--
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //--

            Pyle pyle = new Pyle(ref quoteData);

            pyle.Get_rates(out Pyle_Quote_Genera);

            //--
            stopwatch.Stop();
            Pyle_Quote_Genera.Elapsed_milliseconds = (int)stopwatch.ElapsedMilliseconds;
            //DB.LogGenera("GetResultObjectFromBestOvernite_Genera", "BestOvernite_Quote_Genera.Elapsed_milliseconds", BestOvernite_Quote_Genera.Elapsed_milliseconds.ToString());
            //--
        }
        catch (Exception e)
        {
            DB.LogGenera("GetResultObjectFromBestAveritt", "exception", e.ToString());
        }
    }

    #endregion

    #region GetResultObjectFromAveritt_Genera

    private void GetResultObjectFromAveritt_Genera()
    {
        try
        {
            //DB.LogGenera("GetResultObjectFromAveritt_Genera", "GetResultObjectFromAveritt_Genera",
            //    "GetResultObjectFromAveritt_Genera");
            //if (AccessorialsObj.CONPU || AccessorialsObj.CONDEL || AccessorialsObj.TRADEDEL || AccessorialsObj.TRADEPU)
            //{
            //    throw new Exception("Accessorial not supported");
            //}

            if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
            {
                // Do nothing
                // quoteData.username == AppCodeConstants.un_genera for LiveGCM and API
            }
            else
            {
                return;
            }

            if (quoteData.origZip == "75019")
            {
                // Do nothing
            }
            else
            {
                return;
            }

            #region Accessorials 

            if (quoteData.AccessorialsObj.RESPU)
            {
                return;
            }

            if (quoteData.AccessorialsObj.APTPU)
            {
                return;
            }

            if (quoteData.AccessorialsObj.TRADEPU)
            {
                return;
            }

            #endregion


            //--
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //--

            string encrypted_password = AppCodeConstants.averitt_genera_pwd;
            Averitt averitt = new Averitt(ref quoteData);

            //DB.LogGenera("averitt.Get_rates", "averitt.Get_rates", "averitt.Get_rates");

            averitt.Get_rates(encrypted_password, out Averitt_Quote_Genera);

            //--
            stopwatch.Stop();
            Averitt_Quote_Genera.Elapsed_milliseconds = (int)stopwatch.ElapsedMilliseconds;
            //DB.LogGenera("GetResultObjectFromBestOvernite_Genera", "BestOvernite_Quote_Genera.Elapsed_milliseconds", BestOvernite_Quote_Genera.Elapsed_milliseconds.ToString());
            //--
        }
        catch (Exception e)
        {
            DB.LogGenera("GetResultObjectFromAveritt_Genera", "exception", e.ToString());
        }
    }

    #endregion

    #region GetResultObjectFromNewPenn_Genera

    private void GetResultObjectFromNewPenn_Genera()
    {
        try
        {


            if (AccessorialsObj.APTPU || AccessorialsObj.TRADEDEL || AccessorialsObj.TRADEPU)
            {
                throw new Exception("Accessorial not supported");
            }

            if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
            {
                // Do nothing
                // quoteData.username == AppCodeConstants.un_genera for LiveGCM and API
            }
            else
            {
                return;
            }

            //--
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //--

            if (quoteData.origZip == "08831")
            {
                // Do nothing
            }
            else
            {
                return;
            }

            NewPenn new_penn = new NewPenn(ref quoteData);

            new_penn.Get_rates(out NewPenn_Quote_Genera);

            //--
            stopwatch.Stop();
            NewPenn_Quote_Genera.Elapsed_milliseconds = (int)stopwatch.ElapsedMilliseconds;
            //--

        }
        catch (Exception e)
        {
            DB.Log("New Penn", e.ToString());
        }

    }

    #endregion

    #region GetResultObjectFromUPS_FREIGHT_Genera

    private void GetResultObjectFromUPS_FREIGHT_Genera()
    {
        try
        {
            if (AccessorialsObj.APTPU || AccessorialsObj.TRADEDEL || AccessorialsObj.TRADEPU)
            {
                throw new Exception("Accessorial not supported");
            }

            if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
            {
                // Do nothing
                // quoteData.username == AppCodeConstants.un_genera for LiveGCM and API
            }
            else
            {
                return;
            }
            //DB.LogGenera("GetResultObjectFromUPS_FREIGHT_Genera", "start function", "start function");

            //--
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //--

            CarrierAcctInfo acctInfo = new CarrierAcctInfo();
            acctInfo.username = AppCodeConstants.ups_freight_genera_un;
            acctInfo.password = AppCodeConstants.ups_freight_genera_pwd;
            acctInfo.carrierKey = "UPS";
            acctInfo.bookingKey = "#1#";
            acctInfo.displayName = "UPS - Genera";

            string AccessLicenseNumber = AppCodeConstants.ups_freight_genera_license_num;

            UPS_FREIGHT ups_freight = new UPS_FREIGHT(acctInfo, ref quoteData, ref AccessLicenseNumber);

            ups_freight.Get_UPS_Freight_rate(out UPS_FREIGHT_Quote_Genera);

            //--
            stopwatch.Stop();
            UPS_FREIGHT_Quote_Genera.Elapsed_milliseconds = (int)stopwatch.ElapsedMilliseconds;
            //--
            //DB.LogGenera("GetResultObjectFromUPS_FREIGHT_Genera", "UPS_FREIGHT_Quote_Genera.Elapsed_milliseconds", UPS_FREIGHT_Quote_Genera.Elapsed_milliseconds.ToString());
            //UPS_FREIGHT_Quote_Genera.Elapsed_milliseconds = 0;
        }
        catch (Exception e)
        {
            DB.LogGenera("GetResultObjectFromUPS_FREIGHT_Genera", "GetResultObjectFromUPS_FREIGHT_Genera", e.ToString());
        }

    }

    #endregion

    #region GetResultObjectFromFrontier_Genera

    private void GetResultObjectFromFrontier_Genera()
    {

        //if (AccessorialsObj.TRADEDEL || AccessorialsObj.TRADEPU)
        //{
        //    return;
        //}

        if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
        {
            // Do nothing
            // quoteData.username == AppCodeConstants.un_genera for LiveGCM and API
        }
        else
        {
            return;
        }

        if (quoteData.origZip == "60504")
        {
            // Do nothing
        }
        else
        {
            return;
        }

        //--
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        //--

        Frontier frontier = new Frontier(ref quoteData);

        frontier.Get_rates(out Frontier_Quote_Genera);

        //--
        stopwatch.Stop();
        Frontier_Quote_Genera.Elapsed_milliseconds = (int)stopwatch.ElapsedMilliseconds;
        //DB.LogGenera("GetResultObjectFromBestOvernite_Genera", "SMTL_Quote_Genera.Elapsed_milliseconds", SMTL_Quote_Genera.Elapsed_milliseconds.ToString());
        //--

    }

    #endregion

    #region GetResultObjectFromDaylight_Genera

    private void GetResultObjectFromDaylight_Genera()
    {
        if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
        {
            // Do nothing
            // quoteData.username == AppCodeConstants.un_genera for LiveGCM and API
        }
        else
        {
            return;
        }

        if (quoteData.AccessorialsObj.RESPU || quoteData.isHazmat.Equals(true) || 
            quoteData.AccessorialsObj.TRADEPU || quoteData.AccessorialsObj.TRADEDEL)
        {
            return;
        }
        
        //if (quoteData.origZip == "60504")
        //{
        //    // Do nothing
        //}
        //else
        //{
        //    return;
        //}

        //--
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        //--

        Daylight daylight = new Daylight(ref quoteData);
        
        string access_token = "";
      
        daylight.Get_access_token(ref access_token);
        daylight.Get_rate(ref access_token, out Daylight_Quote_Genera);

        //--

        stopwatch.Stop();
        Daylight_Quote_Genera.Elapsed_milliseconds = (int)stopwatch.ElapsedMilliseconds;
        
        //--

    }

    #endregion

    #endregion

    #region UPS

    private void GetResultObjectFromUPSPackageViaAPI(string serviceType)
    {

        //DB.Log("GetResultObjectFromUPSPackageViaAPI", serviceType);
        try
        {

            if (AccessorialsObj.TRADEPU.Equals(true) || AccessorialsObj.TRADEDEL.Equals(true))
            {
                throw new Exception("Tradeshow not supported");
            }
            //DB.Log("GetUPSPackageNew Live", "start of function");

            #region Accessorials

            if (quoteData.isHazmat || AccessorialsObj.LGPU || AccessorialsObj.LGDEL
                || AccessorialsObj.CONPU || AccessorialsObj.CONDEL
                || AccessorialsObj.INSDEL)
            {
                //upsQuotePackage = null;
                return;
            }

            if (AccessorialsObj.APTPU)
            {
                //upsQuotePackage = null;
                return;
            }
            if (AccessorialsObj.APTDEL)
            {
                //upsQuotePackage = null;
                return;
            }

            #endregion

            #region Variables

            List<HelperFuncs.Credentials> crds = new List<HelperFuncs.Credentials>();
          
            string user = "", pass = "";

            crds = HelperFuncs.GetLoginsByCarID(10922);
            user = crds[0].username;
            pass = crds[0].password;

            List<string> length = new List<string>();
            List<string> width = new List<string>();
            List<string> height = new List<string>();
            List<string> weight = new List<string>();
            List<string> packageType = new List<string>();
            //bool unitsGiven = false, piecesGiven = false;

            #endregion

            //
            //Package exceeds the maximum size total constraints of 165 inches (length + girth, where girth is 2 x width plus 2 x height)
            int quantity = 0;

            double lengthGirthSum = 0;

            for (int i = 0; i < m_lPiece.Length; i++)
            {
                //girth = m_lPiece[i].Length + 
                //girth = 2 * m_lPiece[i].Width + 2 * m_lPiece[i].Height;
                lengthGirthSum = m_lPiece[i].Length + (2 * m_lPiece[i].Width + 2 * m_lPiece[i].Height);
                if (lengthGirthSum > 165)
                {
                    //DB.Log("Length + Girth exceeds max, was: ", lengthGirthSum.ToString());
                    return;
                }

                #region Calculate quantity

                quantity = 0;

                if (m_lPiece[i].Units > 0 && m_lPiece[i].Pieces > 0)
                {
                    return; // No UPS Parcel rate if units are specified
                }
                else if (m_lPiece[i].Units > 0)
                {
                    quantity = m_lPiece[i].Units;
                }
                else if (m_lPiece[i].Pieces > 0)
                {
                    quantity = m_lPiece[i].Pieces;
                }

                #endregion

                if (quantity > 1)
                {
                    #region Quantity is > 1

                    for (int j = 0; j < quantity; j++)
                    {
                        if ((m_lPiece[i].Weight / quantity) >= 150) // A piece is equal or greater than 150 lbs
                        {
                            return;
                        }

                        weight.Add(Math.Round(m_lPiece[i].Weight / quantity, 2).ToString());

                        #region Dimensions

                        if (m_lPiece[i].Length > 0)
                        {
                            length.Add(m_lPiece[i].Length.ToString());
                        }
                        else
                        {
                            length.Add("");
                        }

                        if (m_lPiece[i].Width > 0)
                        {
                            width.Add(m_lPiece[i].Width.ToString());
                        }
                        else
                        {
                            width.Add("");
                        }

                        if (m_lPiece[i].Height > 0)
                        {
                            height.Add(m_lPiece[i].Height.ToString());
                        }
                        else
                        {
                            height.Add("");
                        }

                        #endregion

                        #region For Letter
                        /*
                         * if (Request.QueryString["q_Unit" + i.ToString()] != null && Request.QueryString["q_Unit" + i.ToString().Trim()] == "LETTER")
                                packageType.Add("01"); //LETTER
                         */
                        #endregion

                        packageType.Add("02"); // Your packaging
                    }

                    #endregion
                }
                else
                {
                    #region Quantity is 1

                    if (m_lPiece[i].Weight > 150)
                    {
                        // Weight of package is > 150
                        return;
                    }

                    weight.Add(Math.Round(m_lPiece[i].Weight, 2).ToString());

                    #region Dimensions

                    if (m_lPiece[i].Length > 0)
                    {
                        length.Add(m_lPiece[i].Length.ToString());
                    }
                    else
                    {
                        length.Add("");
                    }

                    if (m_lPiece[i].Width > 0)
                    {
                        width.Add(m_lPiece[i].Width.ToString());
                    }
                    else
                    {
                        width.Add("");
                    }

                    if (m_lPiece[i].Height > 0)
                    {
                        height.Add(m_lPiece[i].Height.ToString());
                    }
                    else
                    {
                        height.Add("");
                    }

                    #endregion

                    packageType.Add("02"); // Your packaging

                    #endregion
                }
            }

            //DB.Log("origCityCarriers", quoteData.origCity);

            UPS_Package ups = new UPS_Package(ref quoteData);

            ups.getUPS_PackageInfoViaAPI_XML(ref user, ref pass,
                ref length, ref width, ref height, ref weight, ref packageType, ref serviceType,
                ref resUPSGround,
            ref resUPSNextDayAir,

            ref resSecondDayAir,
            ref res3DaySelect,
            ref resNextDayAirSaver,
            ref resNextDayAirEarlyAM,
            ref res2ndDayAirAM, ref quoteData);



            // Set quote info to Session for UPS Package booking (on UPS)
            #region set quote info to Session for UPS Package booking (on UPS)

            HelperFuncs.upsPackageShipInfo packInfo = new HelperFuncs.upsPackageShipInfo();

            #region Add items

            List<HelperFuncs.upsPackageItem> items = new List<HelperFuncs.upsPackageItem>();

            for (int i = 0; i < weight.Count; i++)
            {
                HelperFuncs.upsPackageItem item = new HelperFuncs.upsPackageItem();
                item.weight = weight[i];
                item.uom = "LBS"; //to do check if this should be dynamic
                //item.packageType = "02"; //your packaging
                item.length = length[i];
                item.width = width[i];
                item.height = height[i];
                item.packageType = packageType[i];
                items.Add(item);
            }

            packInfo.packageItems = items.ToArray();

            #endregion

            #region Add from and to info

            //packInfo.fromAddressLine1 = "Ship From Street";
            packInfo.fromCity = quoteData.origCity;
            packInfo.fromState = quoteData.origState;
            packInfo.fromPostalCode = quoteData.origZip;
            packInfo.fromCountryCode = "US"; //to do check if Canada
            //packInfo.fromName = "ABC Associates";
            //packInfo.fromAttentionName = "Mr.ABC";

            //packInfo.toAddressLine1 = "Some Street";
            packInfo.toCity = quoteData.destCity;
            packInfo.toState = quoteData.destState;
            packInfo.toPostalCode = quoteData.destZip;
            packInfo.toCountryCode = "US";
            //packInfo.toName = "DEF Associates";
            //packInfo.toAttentionName = "DEF";
            //packInfo.toPhone = "1234567890";

            #endregion

            //Session["upsPackageShipInfo"] = packInfo;
            // To Do addToSession
            carriersResult.upsPackageShipInfo = packInfo;

            #endregion

        }
        catch (Exception e)
        {
            DB.Log("GetUPSPackageNew Live", e.ToString());
        }
    }

    #endregion

    #region Dayton

    private void GetResultObjectFromDaytonFreight()
    {

        //DB.Log("GetResultObjectFromDaytonFreight", "");

        Dayton dayton = new Dayton(ref quoteData);

        objDaytonFreightResult = dayton.GetResultObjectFromDaytonFreight();

    }

    #endregion

    #region USF

    #region GetResultObjectFromUSFReddaway

    private void GetResultObjectFromUSFReddaway(ref GCMRateQuote quote, ref CarrierAcctInfo acctInfo)
    {
        try
        {
            if (AccessorialsObj.TRADEPU.Equals(true) || AccessorialsObj.TRADEDEL.Equals(true))
            {
                throw new Exception("Tradeshow not supported");
            }

            //DB.Log("GetResultObjectFromUSFReddaway", string.Concat(acctInfo.acctNum, " ", acctInfo.chargeType));

            USF usf = new USF(acctInfo, ref quoteData);
            quote = usf.GetRateFromUSF();

        }
        catch (Exception exp)
        {
            quote = null;
            DB.Log("USF", exp.ToString());
        }
    }

    #endregion

    #region GetResultObjectFromUSFReddawayPrepaid

    private void GetResultObjectFromUSFReddawayPrepaid()
    {

        Logins logins = new Logins();
        logins.Get_login_info(100, out Logins.Login_info login_info);

        CarrierAcctInfo acctInfo = new CarrierAcctInfo();
        acctInfo.acctNum = login_info.account;
        acctInfo.chargeType = "Prepaid";

        acctInfo.bookingKey = "---7---";
        acctInfo.displayName = "USF Reddaway";
        acctInfo.carrierKey = "USF Reddaway";

        GetResultObjectFromUSFReddaway(ref usfReddawayQuotePrepaid, ref acctInfo);
    }

    #endregion

    #region GetResultObjectFromUSFReddawayPrepaid_Packwest

    private void GetResultObjectFromUSFReddawayPrepaid_Packwest()
    {

        CarrierAcctInfo acctInfo = new CarrierAcctInfo();
        acctInfo.acctNum = "0973164";
        acctInfo.chargeType = "Prepaid";

        acctInfo.bookingKey = "---7---";
        acctInfo.displayName = "USF Reddaway Packwest";
        acctInfo.carrierKey = "USFRED_PAKWEST";

        GetResultObjectFromUSFReddaway(ref usfReddawayQuotePrepaid_Packwest, ref acctInfo);
    }

    #endregion

    #region GetResultObjectFromUSFReddawayDEPrepaid

    private void GetResultObjectFromUSFReddawayDEPrepaid()
    {

        CarrierAcctInfo acctInfo = new CarrierAcctInfo();
        acctInfo.acctNum = "0863711";
        acctInfo.chargeType = "Prepaid";

        acctInfo.bookingKey = "---7---";
        acctInfo.displayName = "Reddaway DE Prepaid Pricing";
        acctInfo.carrierKey = "USFREDDEPP";

        GetResultObjectFromUSFReddaway(ref usfReddawayQuoteDEPrepaid, ref acctInfo);
    }

    #endregion

    #region GetResultObjectFromUSFReddawaySPCPrepaid

    private void GetResultObjectFromUSFReddawaySPCPrepaid()
    {

        CarrierAcctInfo acctInfo = new CarrierAcctInfo();
        acctInfo.acctNum = "0875353";
        acctInfo.chargeType = "Prepaid";

        acctInfo.bookingKey = "---7---";
        acctInfo.displayName = "Reddaway SPC Prepaid Pricing";
        acctInfo.carrierKey = "USFREDSPCPP";

        GetResultObjectFromUSFReddaway(ref usfReddawayQuoteSPCPrepaid, ref acctInfo);
    }

    #endregion

    #region GetResultObjectFromUSFReddawaySPCCollect

    private void GetResultObjectFromUSFReddawaySPCCollect()
    {

        CarrierAcctInfo acctInfo = new CarrierAcctInfo();
        acctInfo.acctNum = "0871497";
        acctInfo.chargeType = "Prepaid";

        acctInfo.bookingKey = "---7---";
        acctInfo.displayName = "Reddaway SPC Collect Pricing";
        acctInfo.carrierKey = "USFREDSPC";

        GetResultObjectFromUSFReddaway(ref usfReddawayQuoteSPCCollect, ref acctInfo);
    }

    #endregion

    #region GetResultObjectFromUSFHollandSPCPrepaid

    private void GetResultObjectFromUSFHollandSPCPrepaid()
    {

        CarrierAcctInfo acctInfo = new CarrierAcctInfo();
        acctInfo.acctNum = "399236";
        acctInfo.chargeType = "Prepaid";

        acctInfo.bookingKey = "---8---";
        acctInfo.displayName = "Holland SPC Prepaid Pricing";
        acctInfo.carrierKey = "USFHOLSPCPP";

        GetResultObjectFromUSFReddaway(ref usfHollandQuoteSPCPrepaid, ref acctInfo);
    }

    #endregion

    #region GetResultObjectFromUSFHollandSPCCollect

    private void GetResultObjectFromUSFHollandSPCCollect()
    {

        CarrierAcctInfo acctInfo = new CarrierAcctInfo();
        acctInfo.acctNum = "399236";
        acctInfo.chargeType = "Collect";

        acctInfo.bookingKey = "---8---";
        acctInfo.displayName = "Holland SPC Collect Pricing";
        acctInfo.carrierKey = "USFHOLSPC";

        GetResultObjectFromUSFReddaway(ref usfHollandQuoteSPCCollect, ref acctInfo);
    }

    #endregion

    #region GetResultObjectFromUSFReddawayMWIPrepaid

    private void GetResultObjectFromUSFReddawayMWIPrepaid()
    {

        CarrierAcctInfo acctInfo = new CarrierAcctInfo();
        acctInfo.acctNum = "0850489";
        acctInfo.chargeType = "Prepaid";
        acctInfo.bookingKey = "---7---";
        acctInfo.displayName = "Reddaway MWI Prepaid Pricing";
        acctInfo.carrierKey = "USFREDMWIPP";

        GetResultObjectFromUSFReddaway(ref usfReddawayQuoteMWIPrepaid, ref acctInfo);
    }

    #endregion

    #region GetResultObjectFromUSFHollandMWIPrepaid

    private void GetResultObjectFromUSFHollandMWIPrepaid()
    {

        CarrierAcctInfo acctInfo = new CarrierAcctInfo();
        acctInfo.acctNum = "388175";

        if (quoteData.username.ToLower().Contains("vanguard")) acctInfo.acctNum = "799158";

        acctInfo.chargeType = "Prepaid";

        acctInfo.bookingKey = "---8---";
        acctInfo.displayName = "Holland MWI Prepaid Pricing";
        acctInfo.carrierKey = "USFHOLMWIPP";

        GetResultObjectFromUSFReddaway(ref usfReddawayQuoteMWIPrepaid, ref acctInfo);
    }

    #endregion

    // Volume

    #region GetResultObjectFrom_USF_volume

    private void GetResultObjectFrom_USF_volume()
    {
        try
        {
            #region Build Items string

            int total_units = 0;

            //StringBuilder items = new StringBuilder();

            for (byte i = 0; i < m_lPiece.Length; i++)
            {
                // Guard
                if (m_lPiece[i].Length > 48 || m_lPiece[i].Width > 48)
                {
                    return;
                }

                total_units += m_lPiece[i].Units;
            }

            #endregion

            // Guard
            if (total_units < 5) // USF Volume minimum is 5                
            {
                return;
            }

            CarrierAcctInfo acctInfo = new CarrierAcctInfo();
            
            acctInfo.carrierKey = "USF Holland";

            USF usf = new USF(acctInfo, ref quoteData);
            usf_volume_result = usf.Get_USF_API_Volume_Quote(ref total_units);
        }
        catch (Exception e)
        {
            DB.Log("GetResultObjectFrom_USF_volume", e.ToString());
        }
    }

    #endregion

    #endregion

    #region Saia

    #region GetResultObjectFromSAIASPC

    private void GetResultObjectFromSAIASPC()
    {
        //DB.Log("GetResultObjectFromSAIASPC", "");
        try
        {
            if (AccessorialsObj.TRADEPU.Equals(true) || AccessorialsObj.TRADEDEL.Equals(true))
            {
                throw new Exception("Tradeshow not supported");
            }
            int overlengthFee = 0;

            string destCity = quoteData.destCity;
            string originCity = quoteData.origCity;
            string destState = quoteData.destState;
            string originState = quoteData.origState;

            string origZip = midOrigZip;
            string destZip = midDestZip;
            //bool hasHazMat = false;
            //double testDouble;

            if (!goodSaiaDest(destState))
            {
                saiaQuote = null;
                return;
            }

            Logins logins = new Logins();
            logins.Get_login_info(19, out Logins.Login_info login_info);

            System.Text.StringBuilder request = new System.Text.StringBuilder();

            request.Append("<Create>");
            request.Append(string.Concat("<UserID>", login_info.username, "</UserID>"));
            request.Append(string.Concat("<Password>", login_info.password, "</Password>"));
            request.Append("<TestMode>N</TestMode>");
            request.Append("<BillingTerms>Prepaid</BillingTerms>");
            request.Append(string.Concat("<AccountNumber>", login_info.account, "</AccountNumber>"));

            // ThirdParty
            request.Append("<Application>ThirdParty</Application>");
            request.Append("<OriginCity>" + originCity + "</OriginCity>");
            request.Append("<OriginState>" + originState + "</OriginState>");
            request.Append("<OriginZipcode>" + origZip + "</OriginZipcode>");
            request.Append("<DestinationCity>" + destCity + "</DestinationCity>");
            request.Append("<DestinationState>" + destState + "</DestinationState>");
            request.Append("<DestinationZipcode>" + destZip + "</DestinationZipcode>");

            request.Append("<Details>");

            for (int i = 0; i < m_lPiece.Length; i++)
            {
                request.Append("<DetailItem>");
                request.Append("<Weight>" + m_lPiece[i].Weight.ToString() + "</Weight>");
                request.Append("<Class>" + m_lPiece[i].FreightClass + "</Class>");
                request.Append("</DetailItem>");

                //if (Request.QueryString["q_HazMat" + i.ToString()].Equals("true"))
                //{
                //    hasHazMat = true;
                //}

                if (m_lPiece[i].Length > 192)
                {
                    request.Append("<AccessorialItem>");
                    request.Append("<Code>ExcessiveLength</Code>");
                    request.Append("</AccessorialItem>");
                    break;
                }
                if (m_lPiece[i].Width > 192)
                {
                    request.Append("<AccessorialItem>");
                    request.Append("<Code>ExcessiveLength</Code>");
                    request.Append("</AccessorialItem>");
                    break;
                }
                if (m_lPiece[i].Height > 192)
                {
                    request.Append("<AccessorialItem>");
                    request.Append("<Code>ExcessiveLength</Code>");
                    request.Append("</AccessorialItem>");
                    break;
                }
            }

            request.Append("</Details>");

            #region Accessorials
            request.Append("<Accessorials>");
            if (AccessorialsObj.INSDEL)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>InsideDelivery</Code>");
                request.Append("</AccessorialItem>");
            }
            if (AccessorialsObj.RESDEL || AccessorialsObj.CONDEL)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>ResidentialDelivery</Code>");
                request.Append("</AccessorialItem>");
            }
            if (AccessorialsObj.RESPU || AccessorialsObj.CONPU)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>ResidentialPickup</Code>");
                request.Append("</AccessorialItem>");
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
            if (AccessorialsObj.LGPU)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>LiftgateService</Code>");
                request.Append("</AccessorialItem>");
            }
            if (AccessorialsObj.LGDEL)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>LiftgateService</Code>");
                request.Append("</AccessorialItem>");
            }
            if (AccessorialsObj.APTDEL)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>ArrivalNotice/Appointment</Code>");
                request.Append("</AccessorialItem>");
            }
            if (quoteData.isHazmat)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>Hazardous</Code>");
                request.Append("</AccessorialItem>");
            }
            request.Append("</Accessorials>");
            #endregion

            request.Append("</Create>");

            string url = "http://www.saiasecure.com/webservice/ratequote/xml.aspx";
            string response = (string)HelperFuncs.generic_http_request_3("string", null, url, "", "text/xml",
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8", "POST", request.ToString(), false, false, "", "");

            System.Xml.XmlDocument xmlResponse = new System.Xml.XmlDocument();
            xmlResponse.LoadXml(response);

            if (xmlResponse.SelectSingleNode("/Response/Code").InnerText != "")
            {
                saiaQuoteSPC = null;
                /*
                this.ResponseCode.Text = xmlResponse.SelectSingleNode("/Response/Code").InnerText;
                this.Element.Text = xmlResponse.SelectSingleNode("/Response/Element").InnerText;
                this.Fault.Text = xmlResponse.SelectSingleNode("/Response/Fault").InnerText;
                this.Message.Text = xmlResponse.SelectSingleNode("/Response/Message").InnerText;
                */
            }
            else
            {
                double totalCharges = Double.Parse(xmlResponse.SelectSingleNode("/Response/TotalInvoice").InnerText);


                if (totalCharges > 0)
                {
                    totalCharges += overlengthFee;
                    //if (Request.QueryString["q_InsPick"].Equals("true"))
                    //{
                    //    totalCharges += 70;
                    //}
                }

                if (totalCharges > 0 && AccessorialsObj.APTPU)
                {
                    totalCharges += 30;
                }

                if (AccessorialsObj.LGPU && AccessorialsObj.LGDEL)
                {
                    totalCharges += 25;
                }

                if (AccessorialsObj.RESDEL)
                {
                    totalCharges += 50;
                }

                //if (quoteData.subdomain.Equals("spc") || isCostPlus)
                //{
                //    totalCharges = HelperFuncs.addSPC_Addition(totalCharges);
                //}

                saiaQuoteSPC = SetInfoToObjectQuote(ref totalCharges, "SAIA SPC Pricing", "---8---", "SAIASPC", "",
                    Int32.Parse(xmlResponse.SelectSingleNode("/Response/StandardServiceDays").InnerText), "SAIA");
            }
        }
        catch (Exception exp)
        {
            string str = exp.ToString();
            saiaQuoteSPC = null;
        }
    }

    #endregion

    #region goodSaiaDest
    private bool goodSaiaDest(string destState)
    {
        bool goodDest = true;

        if (destState == "MT" || destState == "ND" || destState == "SD" || destState == "WY" || destState == "WV" || destState == "PA" || destState == "DE" ||
            destState == "DC" || destState == "NJ" || destState == "ME" || destState == "MA" || destState == "NY" || destState == "NJ" || destState == "CT" ||
            destState == "VT" || destState == "NH" || destState == "RI")
        {
            goodDest = false;
        }

        return goodDest;
    }
    #endregion

    #region GetResultObjectFromSAIA

    private void GetResultObjectFromSAIA()
    {
        //DB.Log("GetResultObjectFromSAIA", "");
        try
        {
            if (AccessorialsObj.TRADEPU.Equals(true) || AccessorialsObj.TRADEDEL.Equals(true))
            {
                throw new Exception("Tradeshow not supported");
            }
            string destCity = quoteData.destCity;
            string originCity = quoteData.origCity;
            string destState = quoteData.destState;
            string originState = quoteData.origState;
            int overlengthFee = 0;
            string origZip = midOrigZip;
            string destZip = midDestZip;
            double testDouble;

            if (!goodSaiaDest(destState))
            {
                saiaQuote = null;
                return;
            }

            Logins logins = new Logins();
            logins.Get_login_info(18, out Logins.Login_info login_info);

            System.Text.StringBuilder request = new System.Text.StringBuilder();

            string accountNumber = login_info.account;
            if (quoteData.username.ToLower().Contains("vanguard"))
            {
                accountNumber = "0957981";
            }

            request.Append("<Create>");

            #region Account info

            //request.Append("<UserID></UserID>");
            //request.Append("<Password></Password>");
            //request.Append("<TestMode>N</TestMode>");
            //request.Append("<BillingTerms>Prepaid</BillingTerms>");
            //request.Append("<AccountNumber>" + accountNumber + "</AccountNumber>");

            request.Append(string.Concat("<UserID>", login_info.username, "</UserID>"));
            request.Append(string.Concat("<Password>", login_info.password, "</Password>"));
            request.Append("<TestMode>N</TestMode>");
            request.Append("<BillingTerms>Prepaid</BillingTerms>");
            request.Append(string.Concat("<AccountNumber>", accountNumber, "</AccountNumber>"));

            // ThirdParty
            request.Append("<Application>ThirdParty</Application>");

            #endregion

            #region Orig, Dest

            request.Append("<OriginCity>" + originCity + "</OriginCity>");
            request.Append("<OriginState>" + originState + "</OriginState>");
            request.Append("<OriginZipcode>" + origZip + "</OriginZipcode>");
            request.Append("<DestinationCity>" + destCity + "</DestinationCity>");
            request.Append("<DestinationState>" + destState + "</DestinationState>");
            request.Append("<DestinationZipcode>" + destZip + "</DestinationZipcode>");

            #endregion

            #region Items details

            request.Append("<Details>");

            for (int i = 0; i < m_lPiece.Length; i++)
            {
                request.Append("<DetailItem>");
                request.Append("<Weight>" + m_lPiece[i].Weight.ToString() + "</Weight>");
                request.Append("<Class>" + m_lPiece[i].FreightClass + "</Class>");
                request.Append("</DetailItem>");
            }

            request.Append("</Details>");

            #endregion

            #region Accessorials

            request.Append("<Accessorials>");

            for (int i = 0; i < m_lPiece.Length; i++)
            {
                if (m_lPiece[i].Length > 192)
                {
                    request.Append("<AccessorialItem>");
                    request.Append("<Code>ExcessiveLength</Code>");
                    request.Append("</AccessorialItem>");
                    break;
                }
                if (m_lPiece[i].Width > 192)
                {
                    request.Append("<AccessorialItem>");
                    request.Append("<Code>ExcessiveLength</Code>");
                    request.Append("</AccessorialItem>");
                    break;
                }
                if (m_lPiece[i].Height > 192)
                {
                    request.Append("<AccessorialItem>");
                    request.Append("<Code>ExcessiveLength</Code>");
                    request.Append("</AccessorialItem>");
                    break;
                }
            }

            if (AccessorialsObj.INSDEL)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>InsideDelivery</Code>");
                request.Append("</AccessorialItem>");
            }
            if (AccessorialsObj.RESDEL || AccessorialsObj.CONDEL)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>ResidentialDelivery</Code>");
                request.Append("</AccessorialItem>");
            }
            if (AccessorialsObj.RESPU || AccessorialsObj.CONPU)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>ResidentialPickup</Code>");
                request.Append("</AccessorialItem>");
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
            if (AccessorialsObj.LGPU)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>LiftgateService</Code>");
                request.Append("</AccessorialItem>");
            }
            if (AccessorialsObj.LGDEL)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>LiftgateService</Code>");
                request.Append("</AccessorialItem>");
            }
            if (AccessorialsObj.APTDEL)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>ArrivalNotice/Appointment</Code>");
                request.Append("</AccessorialItem>");
            }
            if (quoteData.isHazmat)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>Hazardous</Code>");
                request.Append("</AccessorialItem>");
            }
            request.Append("</Accessorials>");

            #endregion

            request.Append("</Create>");

            #region Get XML Response

            string url = "http://www.saiasecure.com/webservice/ratequote/xml.aspx";
            string response = (string)HelperFuncs.generic_http_request_3("string", null, url, "", "text/xml",
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8", "POST", request.ToString(), false, false, "", "");

            System.Xml.XmlDocument xmlResponse = new System.Xml.XmlDocument();
            xmlResponse.LoadXml(response);

            #endregion

            if (xmlResponse.SelectSingleNode("/Response/Code").InnerText != "")
            {
                saiaQuote = null;
                /*
                this.ResponseCode.Text = xmlResponse.SelectSingleNode("/Response/Code").InnerText;
                this.Element.Text = xmlResponse.SelectSingleNode("/Response/Element").InnerText;
                this.Fault.Text = xmlResponse.SelectSingleNode("/Response/Fault").InnerText;
                this.Message.Text = xmlResponse.SelectSingleNode("/Response/Message").InnerText;
                */
                //DB.Log("Saia (Live)", "code: " + xmlResponse.SelectSingleNode("/Response/Code").InnerText +
                //" element: " + xmlResponse.SelectSingleNode("/Response/Element").InnerText +
                //" fault: " + xmlResponse.SelectSingleNode("/Response/Fault").InnerText +
                //" message: " + xmlResponse.SelectSingleNode("/Response/Message").InnerText, "");
            }
            else
            {
                double totalCharges = Double.Parse(xmlResponse.SelectSingleNode("/Response/TotalInvoice").InnerText);

                #region Accessorials additions

                if (totalCharges > 0)
                {
                    totalCharges += overlengthFee;
                    //if (Request.QueryString["q_InsPick"].Equals("true"))
                    //{
                    //    totalCharges += 70;
                    //}
                }

                if (totalCharges > 0 && AccessorialsObj.APTPU)
                {
                    totalCharges += 30;
                }

                if (AccessorialsObj.LGPU && AccessorialsObj.LGDEL)
                {
                    totalCharges += 25;
                }

                if (AccessorialsObj.RESDEL)
                {
                    totalCharges += 50;
                }

                #endregion

                #region unused Guaranteed Rates
                //SC
                /*(double SAIAGuranteedAM = 0, SAIAGuaranteedPM = 0;
                string quoteData.origState = quoteData.origCity;
                string[] Oci = quoteData.origState.Split(',');
                quoteData.origState = Oci[1].Trim();
                string DState = quoteData.destCity;
                string[] Dci = DState.Split(',');
                DState = Dci[1].Trim();
                //
                double fuelCharge = 0, ExcludedFuel = 0;
                XmlDocument xmlDocGS = new XmlDocument();
                xmlDocGS.LoadXml(response);

                fuelCharge = Convert.ToDouble(xmlDocGS.SelectSingleNode("/Response/FuelSurchargeAmount").InnerText);
                ExcludedFuel = (totalCharges - fuelCharge);
                //
                //============================For AM=============================
                if (quoteData.origState == DState)
                {
                    double GurAddAM = ((ExcludedFuel * 25) / 100);
                    if (GurAddAM < 25)
                    {
                        SAIAGuranteedAM = 25;
                    }
                    else
                    {
                        SAIAGuranteedAM = GurAddAM;
                    }
                }
                else
                {
                    double GurAddAM = ((ExcludedFuel * 35) / 100);
                    if (GurAddAM < 35)
                    {
                        SAIAGuranteedAM = 35;
                    }
                    else
                    {
                        SAIAGuranteedAM = GurAddAM;
                    }
                }
                // =================For PM==================================
                if (quoteData.origState == DState)
                {
                    double GurAddPM = ((ExcludedFuel * 10) / 100);
                    if (GurAddPM < 10)
                    {
                        SAIAGuaranteedPM = 10;
                    }
                    else
                    {
                        SAIAGuaranteedPM = GurAddPM;
                    }
                }
                else
                {
                    double GurAddPM = ((ExcludedFuel * 20) / 100);
                    if (GurAddPM < 20)
                    {
                        SAIAGuaranteedPM = 20;
                    }
                    else
                    {
                        SAIAGuaranteedPM = GurAddPM;
                    }
                }
                //
                //double GuaranteedCharge = 0;//Double.Parse(xmlResponse.SelectSingleNode("/Response/GuaranteeAmount").InnerText);
                double guaranteedRateAM = SAIAGuranteedAM + totalCharges;
                double guaranteedRatePM = SAIAGuaranteedPM + totalCharges; */
                //SC
                #endregion

                saiaQuote = SetInfoToObjectQuote(ref totalCharges, "SAIA", "---8---", "SAIA", "http://www.saiasecure.com/rules/b_view.asp?item=1020A",
                Int32.Parse(xmlResponse.SelectSingleNode("/Response/StandardServiceDays").InnerText), "SAIA");

            }
        }
        catch (SoapException ex)
        {
            saiaQuote = null;
            DB.Log("Saia (Live)", ex.ToString());

        }
        catch (Exception exp)
        {
            saiaQuote = null;
            DB.Log("saia", exp.ToString());
        }
    }

    #endregion

    #region GetResultObjectFromSAIAMWI

    private void GetResultObjectFromSAIAMWI()
    {
        DB.Log("GetResultObjectFromSAIAMWI", "");
        try
        {
            if (AccessorialsObj.TRADEPU.Equals(true) || AccessorialsObj.TRADEDEL.Equals(true))
            {
                throw new Exception("Tradeshow not supported");
            }
            int overlengthFee = 0;

            string destCity = quoteData.destCity;
            string originCity = quoteData.origCity;
            string destState = quoteData.destState;
            string originState = quoteData.origState;

            string origZip = midOrigZip;
            string destZip = midDestZip;

            double testDouble;

            if (!goodSaiaDest(destState))
            {
                saiaQuote = null;
                return;
            }

            Logins logins = new Logins();
            logins.Get_login_info(20, out Logins.Login_info login_info);

            System.Text.StringBuilder request = new System.Text.StringBuilder();

            request.Append("<Create>");
            request.Append(string.Concat("<UserID>", login_info.username, "</UserID>"));
            request.Append(string.Concat("<Password>", login_info.password, "</Password>"));
            request.Append("<TestMode>N</TestMode>");
            request.Append("<BillingTerms>Prepaid</BillingTerms>");
            request.Append(string.Concat("<AccountNumber>", login_info.account, "</AccountNumber>"));

            // ThirdParty
            request.Append("<Application>ThirdParty</Application>");
            request.Append("<OriginCity>" + originCity + "</OriginCity>");
            request.Append("<OriginState>" + originState + "</OriginState>");
            request.Append("<OriginZipcode>" + origZip + "</OriginZipcode>");
            request.Append("<DestinationCity>" + destCity + "</DestinationCity>");
            request.Append("<DestinationState>" + destState + "</DestinationState>");
            request.Append("<DestinationZipcode>" + destZip + "</DestinationZipcode>");

            request.Append("<Details>");

            for (int i = 0; i < m_lPiece.Length; i++)
            {

                request.Append("<DetailItem>");
                request.Append("<Weight>" + m_lPiece[i].Weight.ToString() + "</Weight>");
                request.Append("<Class>" + m_lPiece[i].FreightClass + "</Class>");
                request.Append("</DetailItem>");

                if (m_lPiece[i].Length > 192 || m_lPiece[i].Height > 192 || m_lPiece[i].Width > 192)
                {
                    request.Append("<AccessorialItem>");
                    request.Append("<Code>ExcessiveLength</Code>");
                    request.Append("</AccessorialItem>");
                }
            }

            request.Append("</Details>");

            request.Append("<Accessorials>");
            if (AccessorialsObj.INSDEL)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>InsideDelivery</Code>");
                request.Append("</AccessorialItem>");
            }
            if (AccessorialsObj.RESDEL || AccessorialsObj.CONDEL)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>ResidentialDelivery</Code>");
                request.Append("</AccessorialItem>");
            }
            if (AccessorialsObj.RESPU || AccessorialsObj.CONPU)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>ResidentialPickup</Code>");
                request.Append("</AccessorialItem>");
            }
            if (AccessorialsObj.LGPU)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>LiftgateService</Code>");
                request.Append("</AccessorialItem>");
            }
            if (AccessorialsObj.LGDEL)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>LiftgateService</Code>");
                request.Append("</AccessorialItem>");
            }
            if (AccessorialsObj.APTDEL)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>ArrivalNotice/Appointment</Code>");
                request.Append("</AccessorialItem>");
            }
            if (quoteData.isHazmat)
            {
                request.Append("<AccessorialItem>");
                request.Append("<Code>Hazardous</Code>");
                request.Append("</AccessorialItem>");
            }
            request.Append("</Accessorials>");

            request.Append("</Create>");

            string url = "http://www.saiasecure.com/webservice/ratequote/xml.aspx";
            string response = (string)HelperFuncs.generic_http_request_3("string", null, url, "", "text/xml",
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8", "POST", request.ToString(), false, false, "", "");

            System.Xml.XmlDocument xmlResponse = new System.Xml.XmlDocument();
            xmlResponse.LoadXml(response);

            if (xmlResponse.SelectSingleNode("/Response/Code").InnerText != "")
            {
                saiaQuoteMWI = null;
            }
            else
            {
                double totalCharges = Double.Parse(xmlResponse.SelectSingleNode("/Response/TotalInvoice").InnerText);

                if (totalCharges > 0)
                {
                    totalCharges += overlengthFee;
                    //if (AccessorialsObj.["q_InsPick"].Equals("true"))
                    //{
                    //    totalCharges += 70;
                    //}
                }

                if (totalCharges > 0 && AccessorialsObj.APTPU)
                {
                    totalCharges += 30;
                }

                GCMRateQuote objQuote = new GCMRateQuote();
                objQuote.TotalPrice = totalCharges;
                objQuote.DisplayName = "SAIA MWI Pricing";
                objQuote.BookingKey = "---8---";
                objQuote.CarrierKey = "SAIAMWI";
                //SC-1
                objQuote.GuaranteedRateAM = -1;
                objQuote.Documentation = "";
                //SC-1
                objQuote.DeliveryDays = Int32.Parse(xmlResponse.SelectSingleNode("/Response/StandardServiceDays").InnerText);
                //CarsOnTime carOnTime;
                //if (Session["onTimeDict"] != null && ((Dictionary<string, CarsOnTime>)Session["onTimeDict"]).TryGetValue("SAIA", out carOnTime))
                //{
                //    objQuote.OnTimePercent = carOnTime.onTimePercent + '%';
                //    objQuote.ShipsBetweenStates = carOnTime.delivOnTime + carOnTime.delivLate;
                //}
                saiaQuoteMWI = objQuote;
            }
        }
        catch (Exception exp)
        {
            string str = exp.ToString();
            saiaQuoteMWI = null;
        }
    }

    #endregion

    #endregion
    
    #region XPO
    //

    #region GetResultObjectFromXPO_DLS_account

    private void GetResultObjectFromXPO_DLS_account()
    {
        // Testing
        return;

        //DB.Log("GetResultObjectFromXPO_Shug", "");
        
        if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
        {
            // Do nothing
            // quoteData.username == AppCodeConstants.un_genera for LiveGCM and API
        }
        else
        {
            return;
        }

        CarrierAcctInfo acctInfo = new CarrierAcctInfo();
        acctInfo.acctNum = "488269757";
        acctInfo.chargeType = "P";
        acctInfo.terms = "3";

        acctInfo.bookingKey = "#1#";
        acctInfo.displayName = "XPO DLS";
        acctInfo.carrierKey = "Con-way";

        XPO xpo = new XPO(acctInfo, ref quoteData);
        xpoDLS_Quote = xpo.GetRateFromXPO();

    }

    #endregion

    #region GetResultObjectFromXPO_Shug

    private void GetResultObjectFromXPO_Shug()
    {
        //DB.Log("GetResultObjectFromXPO_Shug", "");

        CarrierAcctInfo acctInfo = new CarrierAcctInfo();
        acctInfo.acctNum = "561452031";
        acctInfo.chargeType = "P";
        acctInfo.terms = "3";

        acctInfo.bookingKey = "#1#";
        acctInfo.displayName = "XPO Shug";
        acctInfo.carrierKey = "Con-way";

        XPO xpo = new XPO(acctInfo, ref quoteData);
        xpoShugQuote = xpo.GetRateFromXPO();

    }

    #endregion

    #region GetResultObjectFromXPO_Durachem

    private void GetResultObjectFromXPO_Durachem()
    {
        
        CarrierAcctInfo acctInfo = new CarrierAcctInfo
        {
            acctNum = "715737308",
            chargeType = "C",
            terms = "3",

            bookingKey = "#1#",
            displayName = "XPO Durachem",
            carrierKey = "Con-way"
        };

        //
        if (quoteData.origZip.Equals("44146") || quoteData.origZip.Equals("08854") || quoteData.origZip.Equals("40109") ||
            quoteData.origZip.Equals("91708") || quoteData.origZip.Equals("77029"))
        {
            acctInfo.chargeType = "P";
        }


        //if (quoteData.origZip.Equals("08854") || quoteData.origZip.Equals("08901") || quoteData.origZip.Equals("40109") ||
        //       quoteData.origZip.Equals("77029") || quoteData.origZip.Equals("91708"))
        //{
        //    ChargeCode = "P";
        //}
        //if (quoteData.destZip.Equals("44146") || quoteData.destZip.Equals("08854") || quoteData.destZip.Equals("40109") ||
        //   quoteData.destZip.Equals("91708") || quoteData.destZip.Equals("77029"))
        //{
        //    acctInfo.chargeType = "C";
        //}
        //

        XPO xpo = new XPO(acctInfo, ref quoteData);
        conWayFreightQuote = xpo.GetRateFromXPO();

    }

    #endregion

    #region GetResultObjectFromXPO_AES

    private void GetResultObjectFromXPO_AES()
    {
        //DB.Log("GetResultObjectFromXPO_AES", "");
        
        CarrierAcctInfo acctInfo = new CarrierAcctInfo();
        acctInfo.acctNum = "715737308";
        acctInfo.chargeType = "C";
        acctInfo.terms = "3";

        acctInfo.bookingKey = "#1#";
        acctInfo.displayName = "XPO Logistics";
        acctInfo.carrierKey = "Con-way";

        XPO xpo = new XPO(acctInfo, ref quoteData);
        conWayFreightQuote = xpo.GetRateFromXPO();

    }

    #endregion

    #region Get_rate_from_XPO_spot_quote

    private void Get_rate_from_XPO_spot_quote()
    {

        CarrierAcctInfo acctInfo = new CarrierAcctInfo();
        acctInfo.acctNum = "697924925";
        acctInfo.chargeType = "C";
        acctInfo.terms = "3";

        acctInfo.bookingKey = "#1#";
        acctInfo.displayName = "XPO Logistics";
        acctInfo.carrierKey = "Con-way";

        XPO xpo = new XPO(acctInfo, ref quoteData);

        string access_token;
        xpo.Get_XPO_Access_token(out access_token);
        xpo.Get_XPO_Spot_Quote_rates(ref access_token, ref xpo_volume_result);

    }

    #endregion

    #endregion

    #region AAACooper

    #region GetResultObjectFromAAACooper

    private void GetResultObjectFromAAACooper()
    {
        //DB.Log("GetResultObjectFromAAACooper", "");
        #region Variables

        int overlengthFee = 0;

        #endregion

        try
        {
            if (AccessorialsObj.TRADEPU.Equals(true) || AccessorialsObj.TRADEDEL.Equals(true))
            {
                throw new Exception("Tradeshow not supported");
            }
            // Get Overlenth Fee
            HelperFuncs.GetOverlengthFee(ref m_lPiece, ref overlengthFee, 143, 216, 288, 100, 150, 200);

            double totalCharges = 0.0;

            byte transitDays = 5;

            SharedLTL.getAAACooperRate(ref totalCharges, ref transitDays, ref quoteData, ref m_lPiece, ref AccessorialsObj);
            totalCharges += overlengthFee;

            if (transitDays.Equals(0)) // For some reason AAA Cooper sets some transit times to 0. For example 30303 => 98177
            {
                transitDays = 5;
            }

            aaaCooperQuote = SetInfoToObjectQuote(ref totalCharges, "AAA Cooper", "#1#", "AAA Cooper", "http://www.aaacooper.com/downloads/RulesTariff/RulesTariff.pdf",
                    Convert.ToInt16(transitDays), "AAA Cooper");


        }
        catch (Exception exp)
        {
            aaaCooperQuote = null;
            DB.Log("GetResultObjectFromAAACooper (Demo)", exp.ToString());
        }
    }

    #endregion

    #endregion

    #region GetResultObjectFrom_P44

    private void GetResultObjectFrom_P44()
    {
        try
        {
            //--
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //--

            #region Not used
           
            int total_units = 0;
            
            for (byte i = 0; i < m_lPiece.Length; i++)
            {
                // Guard
                if (m_lPiece[i].Length > 48 || m_lPiece[i].Width > 48)
                {
                    return;
                }

                total_units += m_lPiece[i].Units;
            }

            // Guard
            if (total_units < 4)
            {
                return;
            }
            
            #endregion

            string doc = Get_cloud_44_rates();

            P44_results = new List<P44.result>();

            #region Parse JSON, make list of carrier results

            int ind;
            string single_result_string = string.Empty;
            List<string> single_results = new List<string>();

            while (doc.IndexOf("rateQuote\"") != -1)
            {
                ind = doc.IndexOf("rateQuote\"");
                doc = doc.Substring(ind + 1);

                ind = doc.IndexOf("rateQuote\""); // Look for the next quote, as the end of current quote
                if (ind == -1)
                {
                    single_result_string = doc; // Last result
                }
                else
                {
                    single_result_string = doc.Remove(ind);
                }

                single_results.Add(single_result_string);

                //DB.Log("From gcmAPI_Demo P44 one_result_string", single_result_string);
            }

            #endregion

            #region Parse JSON carrier results into result object

            string[] tokens = new string[3];

            tokens[1] = ":";
            tokens[2] = ",";

            string total, scac, carrierName, quote_number, transitDays, description;

            for (byte i = 0; i < single_results.Count; i++)
            {
                if (single_results[i].Contains("rateQuoteDetail"))
                {
                    //DB.Log("From gcmAPI_Demo P44 has rateQuoteDetail", single_results[i]);

                    tokens[0] = "total";
                    tokens[2] = ",";
                    total = HelperFuncs.scrapeFromPage(tokens, single_results[i]).Replace("}", "");
                    //DB.Log("From gcmAPI_Demo P44 total", total);

                    tokens[0] = "scac";
                    scac = HelperFuncs.scrapeFromPage(tokens, single_results[i]).Replace("\\", "").Replace("\"", "");
                    //DB.Log("From gcmAPI_Demo P44 scac", scac);

                    tokens[0] = "carrierName";
                    carrierName = HelperFuncs.scrapeFromPage(tokens, single_results[i]).Replace("\\", "").Replace("\"", "");
                    //DB.Log("From gcmAPI_Demo P44 carrierName", carrierName);

                    tokens[0] = "capacityProviderQuoteNumber";
                    quote_number = HelperFuncs.scrapeFromPage(tokens, single_results[i]).Replace("\\", "").Replace("\"", "");
                    //DB.Log("From gcmAPI_Demo P44 quote_number", quote_number);

                    tokens[0] = "transitDays";
                    transitDays = HelperFuncs.scrapeFromPage(tokens, single_results[i]);
                    //DB.Log("From gcmAPI_Demo P44 transitDays", transitDays);

                    tokens[0] = "description";
                    tokens[2] = "}";
                    description = HelperFuncs.scrapeFromPage(tokens, single_results[i]).Replace("\\", "").Replace("\"", "");
                    //DB.Log("From gcmAPI_Demo P44 description", description);

                    double test_double;
                    int test_int;
                    //if (!int.TryParse(doc, out transitDays))
                    //    transitDays = -1;
                    if (double.TryParse(total, out test_double))
                    {
                        P44.result result = new P44.result();
                        //result.cost = test_double + 39.00; // Add 39$ for Appointment Delivery
                        result.cost = test_double;

                        if (int.TryParse(transitDays, out test_int))
                        {
                            result.transit_days = test_int;
                        }
                        else
                        {
                            result.transit_days = 10;
                        }

                        result.carrier_name = carrierName;
                        result.scac = scac;
                        result.quote_number = quote_number.Replace("not found", "");
                        result.description = description;

                        if (result.carrier_name.Equals("Holland Motor Express"))
                        {
                            if (result.description.Equals("Standard Rate"))
                            {
                                P44_results.Add(result);
                            }
                            else
                            {
                                // Do nothing
                            }
                        }
                        else
                        {
                            P44_results.Add(result);
                        }
                    }
                    else
                    {
                        // Do nothing
                    }
                }
            }

            #endregion

            //--
            stopwatch.Stop();
            carriersResult.elapsed_milliseconds_P44 = (int)stopwatch.ElapsedMilliseconds;
            //DB.LogGenera("GetResultObjectFromBestOvernite_Genera", "SMTL_Quote_Genera.Elapsed_milliseconds", SMTL_Quote_Genera.Elapsed_milliseconds.ToString());
            //--
        }
        catch (Exception e)
        {
            DB.Log("GetResultObjectFrom_P44", e.ToString());
        }
    }

    #endregion

    #region Get_cloud_44_rates

    public string Get_cloud_44_rates()
    {
        //DB.Log("P44 core Get_p_44_rates", "hit function");

        Logins.Login_info login_info;
        Logins logins = new Logins();
        logins.Get_login_info(101, out login_info);

        Web_client http = new Web_client();

        #region Log in

        http.method = "GET";
        http.url = "https://na12.voc.project44.com/portal/v2/login";
        http.accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
        http.Make_http_request();

        http.method = "POST";
        http.referrer = http.url;
        http.content_type = "application/json";
        http.url = "https://na12.voc.project44.com/api/portal/v2/login";

        http.post_data = string.Concat("{\"username\":\"", login_info.username.Replace("%40", "@"),
            "\",\"password\":\"", login_info.password, "\"}");      
      
        http.Make_http_request();

        http.method = "GET";
        http.referrer = "";
        http.url = "https://na12.voc.project44.com/portal/vltl-ltl-rates";
        http.accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
        http.Make_http_request();

        //http.method = "GET";
        //http.url = "https://cloud.p-44.com/";
        //http.accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
        //http.Make_http_request();

        //http.method = "POST";
        //http.referrer = http.url;
        //http.url = "https://cloud.p-44.com/signin";

        //http.post_data = string.Concat("susername=", login_info.username, "&spassword=", login_info.password);
        //http.Make_http_request();
        ////
        //// Set headers
        //http.header_names = new string[1];
        //http.header_names[0] = "X-Requested-With";
        //http.header_values = new string[1];
        //http.header_values[0] = "XMLHttpRequest";

        //http.method = "GET";
        //http.referrer = "https://cloud.p-44.com/portal/vltl-ltl-rates";
        //http.url = "https://cloud.p-44.com/portal/session/renew";
        //http.accept = "application/json";
        //http.Make_http_request();

        #endregion

        #region Set pickup date variables

        DateTime puDate = DateTime.Today;
        string puDateDay = puDate.Day.ToString(), puDateMonth = puDate.Month.ToString();

        if (puDateDay.Length == 1)
            puDateDay = "0" + puDateDay;
        if (puDateMonth.Length == 1)
            puDateMonth = "0" + puDateMonth;

        #endregion

        #region Build Accessorials string

        StringBuilder accessorials = new StringBuilder();

        //if (InsPick"].Equals("true"))
        //{
        //    strAccOptions += "&chkIP=on";
        //}
        if (AccessorialsObj.LGPU)
        {
            accessorials.Append("{\"code\":\"LGPU\"}");
            accessorials.Append(",");
        }

        if (AccessorialsObj.APTPU)
        {
            accessorials.Append("{\"code\":\"APPTPU\"}");
            accessorials.Append(",");
        }

        if (AccessorialsObj.APTDEL)
        {
            accessorials.Append("{\"code\":\"APPTDEL\"}");
            accessorials.Append(",");
        }

        if (AccessorialsObj.CONPU)
        {
            accessorials.Append("{\"code\":\"CONPU\"}");
            accessorials.Append(",");
        }

        if (AccessorialsObj.TRADEPU)
        {
            accessorials.Append("{\"code\":\"CNVPU\"}");
            accessorials.Append(",");
        }

        if (AccessorialsObj.RESPU)
        {
            accessorials.Append("{\"code\":\"RESPU\"}");
            accessorials.Append(",");
        }

        if (AccessorialsObj.LGDEL)
        {
            accessorials.Append("{\"code\":\"LGDEL\"}");
            accessorials.Append(",");
        }

        if (AccessorialsObj.CONDEL)
        {
            accessorials.Append("{\"code\":\"CONDEL\"}");
            accessorials.Append(",");
        }

        if (AccessorialsObj.TRADEDEL)
        {
            accessorials.Append("{\"code\":\"CNVDEL\"}");
            accessorials.Append(",");
        }

        if (AccessorialsObj.RESDEL)
        {
            accessorials.Append("{\"code\":\"RESDEL\"}");
            accessorials.Append(",");
        }

        if (quoteData.isHazmat)
        {
            accessorials.Append("{\"code\":\"HAZM\"}");
            accessorials.Append(",");
        }

        if (AccessorialsObj.INSDEL)
        {
            accessorials.Append("{\"code\":\"INDEL\"}");
            accessorials.Append(",");
        }

        //
        if (AccessorialsObj.MILIPU)
        {
            accessorials.Append("{\"code\":\"MILPU\"}");
            accessorials.Append(",");
        }

        if (AccessorialsObj.MILIDEL)
        {
            accessorials.Append("{\"code\":\"MILDEL\"}");
            accessorials.Append(",");
        }
        //

        //accessorials.Append("{\"code\":\"APPTDEL\"}"); // Adding by default

        string accessorials_str = accessorials.ToString();

        if (accessorials_str.Length > 0)
        {
            accessorials_str = accessorials_str.Remove(accessorials_str.Length - 1);
        }
        else
        {
            // Do nothing
        }


        //DB.Log("accessorials", accessorials.ToString(), ref connection_string);

        DB.LogGenera("accessorials_str", "accessorials_str", accessorials_str.ToString());

        #endregion

        int total_units = 0;

        #region Build Items string

        //int total_units = 0;

        StringBuilder items = new StringBuilder();

        for (byte i = 0; i < m_lPiece.Length; i++)
        {

            //items.Append(string.Concat(
            //    "{ \"total_weight\":", m_lPiece[i].Weight,
            //    ", \"length\":", m_lPiece[i].Length, ", \"width\":", m_lPiece[i].Width, ", \"height\":", m_lPiece[i].Height,
            //    ", \"units\":", m_lPiece[i].Units, " }"));

            items.Append(string.Concat(
                //"{ \"total_weight\":", model.Items[i].total_weight,
                //", \"length\":48, \"width\":48, \"height\":70, \"units\":", model.Items[i].units, " }"

                "{\"totalWeight\":\"", (int)m_lPiece[i].Weight, "\",",
                "\"packageDimensions\":{\"length\":\"48\",\"width\":\"48\",\"height\":\"70\"},",
                "\"packageType\":\"PLT\",\"totalPackages\":\"", m_lPiece[i].Units,
                "\",\"totalPieces\":\"", m_lPiece[i].Units, "\",\"freightClass\":\"",
                m_lPiece[i].FreightClass, "\"}"



                ));

            //DB.Log("P44 i", i.ToString());
            //DB.Log("P44 Length - 1", (m_lPiece.Length - 1).ToString());

            if (i == m_lPiece.Length - 1) // Last iteration
            {
                // Do nothing
            }
            else
            {
                //DB.Log("P44 ", "i not equal to length - 1");
                items.Append(",");
            }

            //

            //total_units += model.Items[i].units;
            total_units += m_lPiece[i].Units;
        }

        // Guard
        if (total_units < 4)
        {
            return "";
        }

        DB.Log("P44 core items", items.ToString());

        #endregion

        string Orig_country = "US", Dest_country = "US";

        if (quoteData.origCountry.Equals("CANADA"))
        {
            Orig_country = "CA";
        }

        if (quoteData.destCountry.Equals("CANADA"))
        {
            Dest_country = "CA";
        }

        int Total_lineal_feet = total_units * 2;

        if (quoteData.linealFeet > 0.0) // Requested by XML GCM API
        {
            Total_lineal_feet = Convert.ToInt32(quoteData.linealFeet);
        }

        //http.content_type = "application/json";
        //http.method = "POST";
        //http.url = "https://cloud.p-44.com/portal/vltl/quotes/rates/query";

        http.header_names = new string[1];
        http.header_names[0] = "X-Requested-With";
        http.header_values = new string[1];
        http.header_values[0] = "XMLHttpRequest";
        http.content_type = "application/json";
        http.method = "POST";
        http.accept = "application/json";
        http.referrer = http.url;
        http.url = "https://na12.voc.project44.com/portal/vltl/quotes/rates/query";

        http.post_data = string.Concat("{\"originAddress\":{\"postalCode\":\"", quoteData.origZip, "\",",
            "\"addressLines\":[],\"city\":\"", quoteData.origCity.ToUpper(),
            "\",\"state\":\"", quoteData.origState, "\",\"country\":\"", Orig_country, "\"},",
            "\"destinationAddress\":{\"postalCode\":\"", quoteData.destZip, "\",\"addressLines\":[],",
            "\"city\":\"", quoteData.destCity.ToUpper(), "\",\"state\":\"", quoteData.destState, "\",\"country\":\"",
            Dest_country, "\"},",

            "\"lineItems\":[",

            items,
            //"{\"totalWeight\":\"6500\",",
            //"\"packageDimensions\":{\"length\":\"48\",\"width\":\"48\",\"height\":\"70\"},",
            //"\"packageType\":\"PLT\",\"totalPackages\":\"5\",\"totalPieces\":\"5\"}",

            "],",

            //                "\"lineItems\":[{",
            //                "\"totalWeight\":\"4500\",\"packageDimensions\":{ \"length\":\"48\",\"width\":\"48\",\"height\":\"70\"},",
            //"\"packageType\":\"PLT\",\"totalPackages\":1,\"totalPieces\":1},",
            //"{",
            //                "\"totalWeight\":\"2000\",\"packageDimensions\":{ \"length\":\"45\",\"width\":\"45\",\"height\":\"70\"},",
            //"\"packageType\":\"PLT\",\"totalPackages\":1,\"totalPieces\":1}],",


            "\"capacityProviderAccountGroup\":{\"code\":\"Default\",\"accounts\":[]},",

            "\"accessorialServices\":[",

            //"{\"code\":\"APPTDEL\"}", 

            //accessorials,
            accessorials_str,


            "],",


            "\"pickupWindow\":{\"date\":\"", puDate.Year, "-", puDateMonth, "-", puDateDay,
            "\",\"startTime\":\"12:55\"},",
            "\"totalLinearFeet\":\"", Total_lineal_feet, "\",\"linearFeetVisualizationIdentifier\":null,\"weightUnit\":\"LB\",",
            "\"lengthUnit\":\"IN\",\"apiConfiguration\":{\"enableUnitConversion\":true,",
            "\"accessorialServiceConfiguration\":{\"fetchAllServiceLevels\":true,\"allowUnacceptedAccessorials\":false}}}");


        DB.Log("P_44 post data", http.post_data);

        string doc = string.Empty;
        //doc = "44 test";
        doc = http.Make_http_request();

        DB.Log("P_44 result", doc);

        return doc;
    }

    #endregion

    #region Get_ABF_volume_rates

    private void Get_ABF_volume_rates()
    {
        // Guard
        if (quoteData.totalWeight < 2500)
        {
            return;
        }

        #region Variables

        string month = quoteData.puDate.Month.ToString();
        string day = quoteData.puDate.Day.ToString();

        if (month.Length == 1)
            month = "0" + month;

        if (day.Length == 1)
            day = "0" + day;

        string orig_country = "US", dest_country = "US";

        if (quoteData.origCountry.Equals("CANADA"))
        {
            orig_country = "CA";
        }

        if (quoteData.destCountry.Equals("CANADA"))
        {
            dest_country = "CA";
        }

        #endregion

        try
        {

            #region Accessorials

            StringBuilder accessorials = new StringBuilder();

            //Acc_GRD_PU LGPU
            //Acc_IPU Inside Pickup
            //Acc_LAP Limited Access pickup
            //LAPType
            //Acc_RPU respu
            //Acc_TRDSHWO 

            //Acc_CSD Construction Delivery
            // Acc_GRD_DEL lgpu
            //Acc_IDEL inside delivery
            //Acc_LAD limited access delivery
            //LADType
            //Acc_RDEL resdel
            //Acc_TRDSHWD tradeshow delivery

            //Acc_HAZ

            if (AccessorialsObj.LGPU)
            {
                accessorials.Append("&Acc_GRD_PU=Y");
            }

            if (AccessorialsObj.APTPU || AccessorialsObj.APTDEL)
            {
                //accessorials.Append("&Acc_GRD_PU=Y");  
                throw new Exception("ABF APTPU");
            }

            if (AccessorialsObj.CONPU)
            {
                accessorials.Append("&Acc_LAP=Y");
                accessorials.Append("&LAPType=O");
            }

            if (AccessorialsObj.TRADEPU)
            {
                accessorials.Append("&Acc_TRDSHWO=Y");
            }

            if (AccessorialsObj.RESPU)
            {
                accessorials.Append("&Acc_RPU=Y");
            }

            if (AccessorialsObj.LGDEL)
            {
                accessorials.Append("&Acc_GRD_DEL=Y");
            }

            if (AccessorialsObj.CONDEL)
            {
                accessorials.Append("&Acc_CSD=Y");
            }

            if (AccessorialsObj.TRADEDEL)
            {
                accessorials.Append("&Acc_TRDSHWD=Y");
            }

            if (AccessorialsObj.RESDEL)
            {
                accessorials.Append("&Acc_RDEL=Y");
            }

            if (quoteData.isHazmat)
            {
                accessorials.Append("&Acc_HAZ=Y");
            }

            if (AccessorialsObj.INSDEL)
            {
                accessorials.Append("&Acc_IDEL=Y");
            }

            #endregion

            #region Build Items string

            string freight_class = "50.0";

            int total_units = 0;

            StringBuilder items = new StringBuilder();

            int one_based_ind = 1;

            for (int i = 0; i < m_lPiece.Length; i++)
            {
                // Guard
                if (m_lPiece[i].Length > 48 || m_lPiece[i].Width > 48)
                {
                    return;
                }

                if (m_lPiece[i].FreightClass == "92.5" || m_lPiece[i].FreightClass == "77.5")
                {
                    // Do nothing
                    freight_class = m_lPiece[i].FreightClass;
                }
                else
                {
                    freight_class = m_lPiece[i].FreightClass + ".0";
                }

                //items.Append(string.Concat(
                //    "{ \"total_weight\":", m_lPiece[i].Weight,
                //    ", \"length\":48, \"width\":48, \"height\":70, \"units\":", m_lPiece[i].Units,
                //    ", \"freight_class\":", m_lPiece[i].FreightClass, " }"));

                //one_based_ind = i + Convert.ToByte(1);
                one_based_ind = i + 1;

                items.Append(string.Concat(
                    "&UnitNo", one_based_ind, "=", m_lPiece[i].Units, "&UnitType", one_based_ind, "=PLT",
                    "&Wgt", one_based_ind, "=", m_lPiece[i].Weight,
                    "&Class", one_based_ind, "=", freight_class
                    ));


                //DB.Log("P44 i", i.ToString());
                //DB.Log("P44 Length - 1", (m_lPiece.Length - 1).ToString());

                if (i == m_lPiece.Length - 1) // Last iteration
                {
                    // Do nothing
                }
                else
                {
                    //DB.Log("P44 ", "i not equal to length - 1");
                    items.Append(",");
                }

                //

                total_units += m_lPiece[i].Units;
            }

            // Guard
            if (total_units < 4)
            {
                return;
            }

            DB.Log("ABF_volume items", items.ToString());

            int lineal_feet = total_units * 2;

            if (quoteData.linealFeet > 0.0) // Requested by XML GCM API
            {
                lineal_feet = Convert.ToInt32(quoteData.linealFeet);
            }

            #endregion

            Logins logins = new Logins();
            logins.Get_login_info(74, out Logins.Login_info login_info);
            
            string url = string.Concat("https://api.arcb.com/quotes/volume/xml/?ID=", login_info.API_Key,
               "&RequesterName=Alex+Furman&RequesterEmail=", AppCodeConstants.Alex_email,

               "&TPBAff=Y",
               "&TPBName=AES Logistics, Inc",
               "&TPBZip=98003",
               "&TPBAddr=2505 South 320th Street %23625",
               "&TPBCity=Federal Way",
               "&TPBState=WA",
               "&TPBCountry=US",
               //"&TPBPay=Y",

               "&ShipCity=", quoteData.origCity, "&ShipState=", quoteData.origState, "&ShipZip=", quoteData.origZip,
               "&ShipCountry=", orig_country,
               "&ConsCity=", quoteData.destCity, "&ConsState=", quoteData.destState, "&ConsZip=", quoteData.destZip,
               "&ConsCountry=", dest_country,


               "&FrtLng=", lineal_feet,

               "&FrtWdth=8&FrtHght=8",
               "&FrtLWHType=FT",
               //"&FrtLWHType=IN",

               items,

               //"&UnitNo1=10&UnitType1=PLT&Wgt1=6000&Class1=50.0",

               accessorials,

               "&ShipMonth=", month, "&ShipDay=", day, "&ShipYear=", quoteData.puDate.Year);

            Web_client http = new Web_client
            {
                url = url,
                content_type = "",
                accept = "*/*",
                method = "GET"
            };

            //DB.Log("gcmAPI_Get_LOUP_Rates before send request", "before send request");

            string doc = http.Make_http_request();

            //DB.Log("ABF request", url);
            //DB.Log("ABF result", doc);

            #region Parse result

            abf_volume_result = new Abf_volume_result();

            string[] tokens = new string[3];
            tokens[0] = "<CHARGE>";
            tokens[1] = ">";
            tokens[2] = "<";

            //DB.Log("ABF CHARGE", HelperFuncs.scrapeFromPage(tokens, doc));
            abf_volume_result.CHARGE = Convert.ToDouble(HelperFuncs.scrapeFromPage(tokens, doc));

            tokens[0] = "<ESTIMATEDTRANSIT>";

            //int ESTIMATEDTRANSIT = 0;
            abf_volume_result.ESTIMATEDTRANSIT = Convert.ToInt32(HelperFuncs.scrapeFromPage(tokens, doc).
                    Replace("Days", "").Replace("Day", ""));

            tokens[0] = "<QUOTEID>";
            abf_volume_result.QUOTEID = HelperFuncs.scrapeFromPage(tokens, doc);

            #endregion

        }
        catch (Exception e)
        {
            //string str = e.ToString();
            DB.Log("Get_ABF_volume_rates", e.ToString());
        }
    }

    private struct Abf_volume_result
    {
        public string QUOTEID;
        public int ESTIMATEDTRANSIT;
        public double CHARGE;
    }

    #endregion

    #region Get_ESTES_volume_rates

    private void Get_ESTES_volume_rates()
    {

        CarrierAcctInfo acctInfo = new CarrierAcctInfo();
       
        Estes estes = new Estes(ref acctInfo, ref quoteData);
        estes_volume_result = estes.Get_ESTES_volume_rates_xml(ref estes_volume_economy_result,
            ref estes_volume_basic_result);

    }

    #endregion

    //

    #region Get_Sunset_Pacific_volume_rates

    private void Get_Sunset_Pacific_volume_rates()
    {

        CarrierAcctInfo acctInfo = new CarrierAcctInfo();

        if (quoteData.origState.Equals("CA"))
        {

            Sunset_Pacific sunset_p = new Sunset_Pacific(ref acctInfo, ref quoteData);

            string access_token = string.Empty;

            sunset_p.Get_Sunset_Pacific_access_token(ref access_token);

            sunset_p.Get_Sunset_Pacific_rates(ref access_token, ref sunset_volume_result);

        }
        else
        {
            // Do nothing 
        }
    }

    #endregion

    #region Get_Genera_Truckload_rates

    private void Get_Genera_Truckload_rates()
    {
        //DB.Log("Get_Genera_Truckload_rates", "start function");

        //--
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        //--

        GeneraTruckload genera_truckload = new GeneraTruckload(ref quoteData);
        Truckload_Quote_Genera = genera_truckload.Get_rate_from_TruckloadRatesCFI();

        //--
        stopwatch.Stop();
        Truckload_Quote_Genera.Elapsed_milliseconds = (int)stopwatch.ElapsedMilliseconds;
        //--
    }

    #endregion
    
    #endregion

    #region Threads

    #region Start_threads

    private void Start_threads()
    {
        #region Check for Inter Canada

        if (quoteData.origCountry == "CANADA" && quoteData.destCountry == "CANADA")
        {

            //Response.Write("We cannot rate Inter Canada. We apologize.");
            return;
        }

        #endregion

        if (quoteData.hasFreightClass == false)
        {
            return;
        }

        #region Thread declarations

        oThreadDLS = new Thread(new ThreadStart(GetResultObjectFromDLS));
        oThreadDLS_GLTL = new Thread(new ThreadStart(GetResultObjectFromDLS_GLTL));
        //Thread oThreadDLS_cust_rates = new Thread(new ThreadStart(GetResultObjectFromDLS_cust_rates));
        oThreadDLS_Genera = new Thread(new ThreadStart(GetResultObjectFromDLS_Genera));

        oThreadDLS_HHG_Under_500 = new Thread(new ThreadStart(GetResultObjectFromDLS_HHG_Under_500));

        oThreadDLS_GLTL_HHG_Under_500 = new Thread(new ThreadStart(GetResultObjectFromDLS_GLTL_HHG_Under_500));

        oThreadRR = new Thread(new ThreadStart(GetResultObjectFromRoadRunner));
        oThreadRR_Class50 = new Thread(new ThreadStart(GetResultObjectFromRoadRunnerClass50));

        //Thread oThreadUPS = new Thread(new ThreadStart(GetResultObjectFromUPS));

        oThreadUPS_FREIGHT_Genera = new Thread(new ThreadStart(GetResultObjectFromUPS_FREIGHT_Genera));

        oThreadBestOvernite_Genera = new Thread(new ThreadStart(GetResultObjectFromBestOvernite_Genera));

        oThreadPyle_Genera = new Thread(new ThreadStart(GetResultObjectFromPyle_Genera));

        oThreadAveritt_Genera = new Thread(new ThreadStart(GetResultObjectFromAveritt_Genera));

        oThreadNewPenn_Genera = new Thread(new ThreadStart(GetResultObjectFromNewPenn_Genera));

        oThreadSMTL_Genera = new Thread(new ThreadStart(GetResultObjectFromSMTL_Genera));
        //
        oThreadFrontier_Genera = new Thread(new ThreadStart(GetResultObjectFromFrontier_Genera));

        //oThreadDaylight_Genera
        oThreadDaylight_Genera = new Thread(new ThreadStart(GetResultObjectFromDaylight_Genera));

        #region UPS Package API threads

        /*
            * 01 = Next Day Air
02 = 2nd Day Air
03 = Ground
12 = 3 Day Select
13 = Next Day Air Saver
14 = Next Day Air Early AM
59 = 2nd Day Air AM
             
            */
        /*
    Thread oThreadUPSPackage_Ground = new Thread(() => GetResultObjectFromUPSPackageViaAPI("03"));

    Thread oThreadUPSPackage_NextDayAir = new Thread(() => GetResultObjectFromUPSPackageViaAPI("01"));

    Thread oThreadUPSPackage_SecondDayAir = new Thread(() => GetResultObjectFromUPSPackageViaAPI("02"));

    Thread oThreadUPSPackage_3DaySelect = new Thread(() => GetResultObjectFromUPSPackageViaAPI("12"));

    Thread oThreadUPSPackage_NextDayAirSaver = new Thread(() => GetResultObjectFromUPSPackageViaAPI("13"));

    Thread oThreadUPSPackage_NextDayAirEarlyAM = new Thread(() => GetResultObjectFromUPSPackageViaAPI("14"));

    Thread oThreadUPSPackage_2ndDayAirAM = new Thread(() => GetResultObjectFromUPSPackageViaAPI("59"));
*/

        #endregion

        //DB.Log("after lamda", "after lamda");

        //Get CentralFreight Quote
        //oThreadCentralFreight = new Thread(new ThreadStart(GetResultObjectFromCentralFreight));

        oThreadPittOhio_API = new Thread(new ThreadStart(GetResultObjectFromPittOhio_API));
        oThreadPittOhio_API_SPC = new Thread(new ThreadStart(GetResultObjectFromPittOhio_API_SPC));
        oThreadPittOhio_API_Durachem = new Thread(new ThreadStart(GetResultObjectFromPittOhio_API_Durachem));
        oThreadYRC = new Thread(new ThreadStart(GetResultObjectFromYRC_API));
        oThreadYRC_SPC = new Thread(new ThreadStart(GetResultObjectFromYRC_API_SPC));
        //oThreadSEFL = new Thread(new ThreadStart(GetResultObjectFromSEFL_AnyAccount));

        oThreadRL = new Thread(new ThreadStart(GetResultObjectFromRAndL));

        //oThreadRLGriots = new Thread(new ThreadStart(GetResultObjectFromRAndLGriots));

        oThreadRL_Genera = new Thread(new ThreadStart(GetResultObjectFromRAndL_Genera));

        oThreadRRSPC = new Thread(new ThreadStart(GetResultObjectFromRRTS_ByAccountSPC));

        oThreadDayton = new Thread(new ThreadStart(GetResultObjectFromDaytonFreight));

        #region USF Reddaway threads

        //Get USF Reddaway Quote for DE Pricing for Prepaid shipments
        oThreadUSFREDDEPP = new Thread(new ThreadStart(GetResultObjectFromUSFReddawayDEPrepaid));

        //Get USF Reddaway Quote for Prepaid shipments
        oThreadUSFREDPP = new Thread(new ThreadStart(GetResultObjectFromUSFReddawayPrepaid));

        //Get USF Reddaway _Packwest Quote for Prepaid shipments
        oThreadUSFREDPP_Packwest = new Thread(new ThreadStart(GetResultObjectFromUSFReddawayPrepaid_Packwest));

        //Get USF Reddaway Qoute for SPC Pricing for Prepaid shipments
        oThreadUSFREDSPCPP = new Thread(new ThreadStart(GetResultObjectFromUSFReddawaySPCPrepaid));

        //Get USF Reddaway Qoute for SPC Pricing for Collect shipments
        oThreadUSFREDSPCC = new Thread(new ThreadStart(GetResultObjectFromUSFReddawaySPCCollect));

        //Get USF Holland Qoute for SPC Pricing for Prepaid shipments
        oThreadUSFHOLSPCPP = new Thread(new ThreadStart(GetResultObjectFromUSFHollandSPCPrepaid));

        //Get USF Holland Qoute for SPC Pricing for Collect shipments
        oThreadUSFHOLSPCC = new Thread(new ThreadStart(GetResultObjectFromUSFHollandSPCCollect));

        //Get USF Reddaway Qoute for MWI Pricing for Prepaid shipments
        oThreadUSFREDMWIPP = new Thread(new ThreadStart(GetResultObjectFromUSFReddawayMWIPrepaid));

        //Get USF Holland Qoute for MWI Pricing for Prepaid shipments
        oThreadUSFHOLMWIPP = new Thread(new ThreadStart(GetResultObjectFromUSFHollandMWIPrepaid));

        #endregion

        //Get SAIA Qoute for SPC Pricing
        oThreadSAIASPC = new Thread(new ThreadStart(GetResultObjectFromSAIASPC));

        //Get Estes Qoute for Allmodes Pricing
        //Thread oThreadEstesAllmodes = new Thread(new ThreadStart(GetResultObjectFromEstesExpress_1));

        #region MWI threads

        //Get R+L Rates for MWI Pricing
        oThreadRLMWI = new Thread(new ThreadStart(GetResultObjectFromRAndLMWI));

        //Get SAIA Quote for MWI Pricing
        oThreadSAIAMWI = new Thread(new ThreadStart(GetResultObjectFromSAIAMWI));

        #endregion

        //Get ODFL Quote
        oThreadODFL = new Thread(new ThreadStart(GetResultObjectFromOldDominion));

        // oThreadConWayFreight 
        oThreadConWayFreight = new Thread(new ThreadStart(GetResultObjectFromXPO_AES));

        // oThreadConWayFreight Durachem Account
        oThreadXPO_Shug = new Thread(new ThreadStart(GetResultObjectFromXPO_Shug));

        oThreadXPO_DLS = new Thread(new ThreadStart(GetResultObjectFromXPO_DLS_account));

        oThreadEstes_DLS_account = new Thread(new ThreadStart(GetResultObjectFromEstesDLS_account));

        oThreadEstes_Genera_account = new Thread(new ThreadStart(GetResultObjectFromEstesGenera_account));

        oThreadXPO_Durachem = new Thread(new ThreadStart(GetResultObjectFromXPO_Durachem));

        // AAACooper       
        oThreadAAACooper = new Thread(new ThreadStart(GetResultObjectFromAAACooper));

        #region Net Net Threads

        oThreadRR_AAFES = new Thread(new ThreadStart(GetResultObjectFromRRTS_ByAccountAAFES));

        #endregion

        #endregion

        if (quoteData.username == AppCodeConstants.un_genera && 
            (quoteData.totalUnits == 6 || quoteData.totalUnits == 7 || quoteData.totalUnits == 8 || quoteData.totalUnits == 9))
        {
            oThreadDLS_Genera.Start();
            ltl_threads.Add(oThreadDLS_Genera);

            return;
        }
        //else if (quoteData.username == AppCodeConstants.un_genera && quoteData.totalUnits == 6)
        //{
        //    return;
        //}
        else if (quoteData.username == AppCodeConstants.un_genera && quoteData.totalUnits > 7)
        {         
            return;
        }
        else
        {
            // Do nothing
        }

        if (quoteData.username == AppCodeConstants.un_genera && quoteData.totalUnits > 5)
        {
            //oThreadDLS_Genera.Start();
            //ltl_threads.Add(oThreadDLS_Genera);

            return;
        }


        #region For username gwwshipus

        if (quoteData.mode.Equals("ws") && quoteData.username.ToLower() == "gwwshipus")
        {


            // Start threads

            if (quoteData.origState == "WA" || quoteData.origState == "OR" || quoteData.origState == "CA" || quoteData.origState == "NV" || quoteData.origState == "ID" || quoteData.origState == "MT" ||
                quoteData.origState == "AZ" || quoteData.origState == "UT" || quoteData.origState == "WY" || quoteData.origState == "CO")
            {
                oThreadUSFREDPP.Start();
            }

            // Join threads

            if (quoteData.origState == "WA" || quoteData.origState == "OR" || quoteData.origState == "CA" || quoteData.origState == "NV" || quoteData.origState == "ID" || quoteData.origState == "MT" ||
                quoteData.origState == "AZ" || quoteData.origState == "UT" || quoteData.origState == "WY" || quoteData.origState == "CO")
            {
                if (!oThreadUSFREDPP.Join(TimeSpan.FromSeconds(10)))
                {
                    oThreadUSFREDPP.Abort();
                }
            }

            return;

        }

        #endregion

        #region US to US or Canada Start threads

        #region Not used
        //bool isInxpressEmail = false;

        //int inxpressIndex = quoteData.username.ToLower().IndexOf("inxpress");
        //if (inxpressIndex == -1)
        //{
        //    isInxpressEmail = false;
        //}
        //else
        //{
        //    isInxpressEmail = true;
        //}
        #endregion

        bool isDirectPittOhioZone = false;

        #region Start threads

        //Start_volume_threads();

        if (quoteData.showDLSRates == true)
        {
            //DB.Log("ShowDLSRates starting thread", "test");
            //oThreadDLS.Start();
            //ltl_threads.Add(oThreadDLS);

            Start_thread(ref oThreadDLS);

            //oThreadDLS_cust_rates.Start();
            //oThreadDLS_Genera.Start();
            //ltl_threads.Add(oThreadDLS_Genera);

            Start_thread(ref oThreadDLS_Genera);

            //oThreadDLS_HHG_Under_500.Start();
            //ltl_threads.Add(oThreadDLS_HHG_Under_500);

            Start_thread(ref oThreadDLS_HHG_Under_500);

            //oThreadDLS_GLTL_HHG_Under_500.Start();
            //ltl_threads.Add(oThreadDLS_GLTL_HHG_Under_500);

            Start_thread(ref oThreadDLS_GLTL_HHG_Under_500);

            //oThreadXPO_DLS.Start();

            //oThreadEstes_DLS_account.Start();

            oThreadEstes_Genera_account.Start();

            //DB.Log("quoteData.mode", quoteData.mode);
            if (!quoteData.mode.Equals("ws"))
            {
                if (quoteData.hasAccessorials.Equals(false) && quoteData.isHazmat.Equals(false))
                {
                    //DB.Log("oThreadDLS_GLTL", "start");
                    //oThreadDLS_GLTL.Start();
                    //ltl_threads.Add(oThreadDLS_GLTL);

                    Start_thread(ref oThreadDLS_GLTL);
                }
            }

            //
            if (quoteData.username.Equals("jcma512h"))
            {
                //oThreadRL.Start();
                //ltl_threads.Add(oThreadRL);

                Start_thread(ref oThreadRL);
            }
        }

        //oThreadUPS_FREIGHT_Genera.Start();
        //ltl_threads.Add(oThreadUPS_FREIGHT_Genera);

        Start_thread(ref oThreadUPS_FREIGHT_Genera);

        //oThreadBestOvernite_Genera.Start();
        //ltl_threads.Add(oThreadBestOvernite_Genera);

        Start_thread(ref oThreadBestOvernite_Genera);

        //oThreadPyle_Genera.Start();
        //ltl_threads.Add(oThreadPyle_Genera);

        Start_thread(ref oThreadPyle_Genera);

        //oThreadAveritt_Genera.Start();
        //ltl_threads.Add(oThreadAveritt_Genera);

        Start_thread(ref oThreadAveritt_Genera);

        //oThreadNewPenn_Genera.Start();
        //ltl_threads.Add(oThreadNewPenn_Genera);

        Start_thread(ref oThreadNewPenn_Genera);

        //oThreadSMTL_Genera.Start();
        //ltl_threads.Add(oThreadSMTL_Genera);

        Start_thread(ref oThreadSMTL_Genera);

        //oThreadFrontier_Genera.Start();
        //ltl_threads.Add(oThreadFrontier_Genera);

        Start_thread(ref oThreadFrontier_Genera);

        Start_thread(ref oThreadDaylight_Genera);

        //oThreadRL_Genera.Start();
        //ltl_threads.Add(oThreadRL_Genera);

        Start_thread(ref oThreadRL_Genera);

        if (quoteData.username.Equals("shug5955"))
        {
            oThreadXPO_Shug.Start();
            ltl_threads.Add(oThreadXPO_Shug);
        }

        if (quoteData.username.Equals("standard") || quoteData.username.Equals("dupraafesbuy"))
        {
            oThreadRR.Start();
            ltl_threads.Add(oThreadRR);

            oThreadRR_Class50.Start();
            ltl_threads.Add(oThreadRR_Class50);

        }

        if ((quoteData.username != "glbolus" && quoteData.isDUR.Equals(false) && quoteData.showDLSRates == false))
        {
          
            #region Start Threads

            if (quoteData.username == "pakwestpaper")
            {
                oThreadUSFREDPP_Packwest.Start();
                ltl_threads.Add(oThreadUSFREDPP_Packwest);
            }

            if (quoteData.is_dura_logic == true)
            {
                HelperFuncs.isDirectPittOhio(ref isDirectPittOhioZone, ref quoteData.origZip, ref quoteData.destZip); // Bool is passed by ref 

                //DB.Log("isDirectPittOhioZone", isDirectPittOhioZone.ToString());

                if (isDirectPittOhioZone)
                {
                    oThreadPittOhio_API_Durachem.Start();
                    ltl_threads.Add(oThreadPittOhio_API_Durachem);
                }

                if (AccessorialsObj.APTPU == false)
                {
                    if (!quoteData.isHHG)
                    {
                        if (quoteData.subdomain.Equals("spc"))
                        {
                            oThreadYRC_SPC.Start();
                            ltl_threads.Add(oThreadYRC_SPC);
                        }
                        else
                        {
                            oThreadYRC.Start();
                            ltl_threads.Add(oThreadYRC);
                        }
                    }
                }
            }
            else
            {
                if (quoteData.subdomain.Equals("spc"))
                {
                    oThreadPittOhio_API_SPC.Start();
                    ltl_threads.Add(oThreadPittOhio_API_SPC);
                }
                else
                {
                    oThreadPittOhio_API.Start();
                    ltl_threads.Add(oThreadPittOhio_API);
                }
            }

            if (quoteData.is_dura_logic == true)
            {
                // Do nothing
            }
            else
            {
                oThreadAAACooper.Start();
                ltl_threads.Add(oThreadAAACooper);
            }

            if (AccessorialsObj.APTPU == false)
            {
                if (!quoteData.subdomain.Equals(HelperFuncs.Subdomains.mwi) && !quoteData.username.ToLower().Equals("pbisupply"))
                {
                    oThreadRL.Start();
                    ltl_threads.Add(oThreadRL);
                }
            }
            if (quoteData.username.Equals("dupraafesbuy") || (quoteData.username.ToLower() != "wdcenter" && quoteData.username.ToLower() != "rbrubber" &&
                quoteData.username.ToLower() != "milanrubber" && !quoteData.isHHG && !quoteData.isCommodityLkupHHG &&
                AccessorialsObj.RESPU == false))
            {
                oThreadRR.Start();
                ltl_threads.Add(oThreadRR);
                oThreadRR_Class50.Start();
                ltl_threads.Add(oThreadRR_Class50);
            }

            //if (AccessorialsObj.RESPU.Equals(false))
            //{
            //    oThreadSEFL.Start();
            //    ltl_threads.Add(oThreadSEFL);
            //}

            oThreadDayton.Start();
            ltl_threads.Add(oThreadDayton);

            if (quoteData.username.Equals("rbrubber") || quoteData.username.Equals("milanrubber") ||
                quoteData.is_dura_logic == true)
            {
                oThreadODFL.Start();
                ltl_threads.Add(oThreadODFL);
            }

            //if (AccessorialsObj.RESPU == false && AccessorialsObj.RESDEL == false)
            //{
            //    if (quoteData.is_dura_logic == true)
            //    {
            //        oThreadCentralFreight.Start();
            //        ltl_threads.Add(oThreadCentralFreight);
            //    }
            //}


            if (quoteData.origState == "WA" || quoteData.origState == "OR" || quoteData.origState == "CA" || quoteData.origState == "NV" || quoteData.origState == "ID" || quoteData.origState == "MT" ||
                quoteData.origState == "AZ" || quoteData.origState == "UT" || quoteData.origState == "WY" || quoteData.origState == "CO")
            {
                if (!quoteData.subdomain.Equals(HelperFuncs.Subdomains.mwi))
                {
                    oThreadUSFREDPP.Start();
                    ltl_threads.Add(oThreadUSFREDPP);
                }
            }

            if (quoteData.is_dura_logic == true)
            {
                //oThreadConWayFreight.Start();
                oThreadXPO_Durachem.Start();
                ltl_threads.Add(oThreadXPO_Durachem);
            }

            if (quoteData.subdomain.Equals("spc") || quoteData.subdomain.Equals(HelperFuncs.Subdomains.mbg))
            {
                #region spc mbg subdomains
                //string originState = quoteData.origState;

                if (quoteData.origState == "WA" || quoteData.origState == "OR" || quoteData.origState == "CA" || quoteData.origState == "NV" || quoteData.origState == "ID" || quoteData.origState == "MT" ||
                    quoteData.origState == "AZ" || quoteData.origState == "UT" || quoteData.origState == "WY" || quoteData.origState == "CO")
                {
                    oThreadUSFREDSPCPP.Start();
                    ltl_threads.Add(oThreadUSFREDSPCPP);

                    if (quoteData.username.ToLower() == "direct")
                    {
                        oThreadUSFREDDEPP.Start();
                        ltl_threads.Add(oThreadUSFREDDEPP);
                    }
                }
                else if (quoteData.origState == "AL" || quoteData.origState == "GA" || quoteData.origState == "SC" || quoteData.origState == "NC" || quoteData.origState == "WV" || quoteData.origState == "MI" ||
                    quoteData.origState == "KY" || quoteData.origState == "TN" || quoteData.origState == "MS" || quoteData.origState == "MO" || quoteData.origState == "IA" || quoteData.origState == "MN" ||
                    quoteData.origState == "WI" || quoteData.origState == "IL" || quoteData.origState == "IN")
                {
                    oThreadUSFHOLSPCPP.Start();
                    ltl_threads.Add(oThreadUSFHOLSPCPP);
                }

                if (!quoteData.isHHG && !quoteData.isCommodityLkupHHG && AccessorialsObj.RESPU == false)
                {
                    oThreadRRSPC.Start();
                    ltl_threads.Add(oThreadRRSPC);
                }

                oThreadUSFREDSPCC.Start();
                ltl_threads.Add(oThreadUSFREDSPCC);
                //oThreadUSFHOLSPCC.Start();
                oThreadSAIASPC.Start();
                ltl_threads.Add(oThreadSAIASPC);
                //oThreadEstesSPC.Start();
                #endregion
            }
            if (quoteData.subdomain.Equals(HelperFuncs.Subdomains.tyson))
            {
                #region tyson subdomain
                //string originState = GetState("origin");

                if (quoteData.origState == "WA" || quoteData.origState == "OR" || quoteData.origState == "CA" || quoteData.origState == "NV" || quoteData.origState == "ID" || quoteData.origState == "MT" ||
                    quoteData.origState == "AZ" || quoteData.origState == "UT" || quoteData.origState == "WY" || quoteData.origState == "CO")
                {
                    oThreadUSFREDSPCPP.Start();
                    ltl_threads.Add(oThreadUSFREDSPCPP);
                }
                else if (quoteData.origState == "AL" || quoteData.origState == "GA" || quoteData.origState == "SC" || quoteData.origState == "NC" || quoteData.origState == "WV" || quoteData.origState == "MI" ||
                    quoteData.origState == "KY" || quoteData.origState == "TN" || quoteData.origState == "MS" || quoteData.origState == "MO" || quoteData.origState == "IA" || quoteData.origState == "MN" ||
                    quoteData.origState == "WI" || quoteData.origState == "IL" || quoteData.origState == "IN")
                {
                    oThreadUSFHOLSPCPP.Start();
                    ltl_threads.Add(oThreadUSFHOLSPCPP);
                }

                if (!quoteData.isHHG && !quoteData.isCommodityLkupHHG && AccessorialsObj.RESPU == false)
                {
                    oThreadRRSPC.Start();
                    ltl_threads.Add(oThreadRRSPC);
                }

                oThreadUSFREDSPCC.Start();
                ltl_threads.Add(oThreadUSFREDSPCC);
                //oThreadUSFHOLSPCC.Start();
                oThreadSAIASPC.Start();
                ltl_threads.Add(oThreadSAIASPC);
                //oThreadEstesSPC.Start();
                #endregion
            }
            //if (quoteData.subdomain.Equals(HelperFuncs.Subdomains.allmodes))
            //{
            //    oThreadEstesAllmodes.Start();
            //}
            if (quoteData.subdomain.Equals(HelperFuncs.Subdomains.mwi))
            {

                #region Mwi subdomain
                //oThreadUPSMWI.Start();

                //if (AccessorialsObj.RESPU.Equals(false))
                //{
                //    oThreadSEFLMWI.Start();
                //}
                //if (isPuertoRicoDest == false)
                //{
                //    oThreadNEMFMWI.Start();
                //}
                if (AccessorialsObj.APTPU == false)
                {
                    oThreadRLMWI.Start();
                    ltl_threads.Add(oThreadRLMWI);
                }
                oThreadSAIAMWI.Start();
                ltl_threads.Add(oThreadSAIAMWI);
                //oThreadEstesMWI.Start();
                //oThreadMMEMWI.Start();
                //oThreadODFLMWI.Start();

                //string originState = GetState("origin");

                if (quoteData.origState == "WA" || quoteData.origState == "OR" || quoteData.origState == "CA" || quoteData.origState == "NV" || quoteData.origState == "ID" || quoteData.origState == "MT" ||
                    quoteData.origState == "AZ" || quoteData.origState == "UT" || quoteData.origState == "WY" || quoteData.origState == "CO")
                {
                    oThreadUSFREDMWIPP.Start();
                    ltl_threads.Add(oThreadUSFREDMWIPP);
                }
                else if (quoteData.origState == "AL" || quoteData.origState == "GA" || quoteData.origState == "SC" || quoteData.origState == "NC" || quoteData.origState == "WV" || quoteData.origState == "MI" ||
                    quoteData.origState == "KY" || quoteData.origState == "TN" || quoteData.origState == "MS" || quoteData.origState == "MO" || quoteData.origState == "IA" || quoteData.origState == "MN" ||
                    quoteData.origState == "WI" || quoteData.origState == "IL" || quoteData.origState == "IN")
                {
                    oThreadUSFHOLMWIPP.Start();
                    ltl_threads.Add(oThreadUSFHOLMWIPP);
                }
                #endregion
            }

            #endregion
        }

        // Join threads

        #endregion

        #endregion

        #region Not used

        // To do add this on the Client


        //------------------------------------------------------------------------
        //else if (origCountry == "PR" && destCountry == "CANADA")
        //{
        //    Response.Write("We cannot rate Puerto Rico to Canada. We apologize.");

        //}
        //else if (origCountry == "CANADA" && destCountry == "PR")
        //{
        //    Response.Write("We cannot rate Canada to Puerto Rico. We apologize.");

        //}
        //else if (origCountry == "PR" && destCountry == "US")
        //{
        //    Response.Write("We cannot rate Puerto Rico to United States. We apologize.");

        //}
        //else if (origCountry == "PR" && destCountry == "PR")
        //{
        //    Response.Write("We cannot rate Inter Puerto Rico. We apologize.");
        //}


        //else if ((quoteData.origCountry == "US" && quoteData.destCountry == "PR") && m_lPiece[0].FreightClass != "")
        //{
        //    oThreadRLPuertoRico.Start();
        //    //oThreadRLPuertoRico.Join();
        //    if (!oThreadRLPuertoRico.Join(TimeSpan.FromSeconds(30)))
        //    {
        //        oThreadRLPuertoRico.Abort();
        //    }
        //}
        //else if (quoteData.subdomain.Equals("spc") && quoteData.username.ToLower() == "expos")
        //{
        //    if (!AccessorialsObj.TRADEPU && !AccessorialsObj.TRADEDEL)
        //    {
        //        oThreadEstesSPC.Start();
        //        oThreadEstesSPC.Join();
        //    }
        //}

        #endregion


        //for (byte i = 0; i < ltl_threads.Count; i++)
        //{
        //    ltl_threads[i].Start();
        //}
    }

    #endregion

    #region Start_thread

    private void Start_thread(ref Thread thread)
    {
        thread.Start();
        ltl_threads.Add(thread);
    }

    #endregion

    #region Join_threads

    private void Join_threads()
    {
        for (byte i = 0; i < ltl_threads.Count; i++)
        {
            if (!ltl_threads[i].Join(TimeSpan.FromSeconds(25)))
            {
                ltl_threads[i].Abort();
            }
        }
    }

    #endregion

    #region NetNet_Start_LTL_threads

    private void NetNet_Start_LTL_threads()
    {
        #region Threads Declaration

        //////////Get Result Object for DLS
        oThreadDLS = new Thread(new ThreadStart(GetResultObjectFromDLS));

        //////////Get Result Object for DLS_GLTL
        oThreadDLS_GLTL = new Thread(new ThreadStart(GetResultObjectFromDLS_GLTL));

        oThreadDLS_cust_rates = new Thread(new ThreadStart(GetResultObjectFromDLS_cust_rates));
        //oThreadDLS_BenFranklinCraftsMacon
        oThreadDLS_BenFranklinCraftsMacon = new Thread(new ThreadStart(GetResultObjectFromDLS_BenFranklinCraftsMacon));

        oThreadDLS_Genera = new Thread(new ThreadStart(GetResultObjectFromDLS_Genera));

        oThreadDLS_HHG_Under_500 = new Thread(new ThreadStart(GetResultObjectFromDLS_HHG_Under_500));

        oThreadDLS_GLTL_HHG_Under_500 = new Thread(new ThreadStart(GetResultObjectFromDLS_GLTL_HHG_Under_500));

        oThreadRR = new Thread(new ThreadStart(GetResultObjectFromRoadRunner));

        oThreadRR_AAFES = new Thread(new ThreadStart(GetResultObjectFromRRTS_ByAccountAAFES));

        oThreadUPS_FREIGHT_Genera = new Thread(new ThreadStart(GetResultObjectFromUPS_FREIGHT_Genera));

        oThreadBestOvernite_Genera = new Thread(new ThreadStart(GetResultObjectFromBestOvernite_Genera));

        oThreadPyle_Genera = new Thread(new ThreadStart(GetResultObjectFromPyle_Genera));

        oThreadAveritt_Genera = new Thread(new ThreadStart(GetResultObjectFromAveritt_Genera));

        oThreadNewPenn_Genera = new Thread(new ThreadStart(GetResultObjectFromNewPenn_Genera));

        oThreadSMTL_Genera = new Thread(new ThreadStart(GetResultObjectFromSMTL_Genera));

        oThreadFrontier_Genera = new Thread(new ThreadStart(GetResultObjectFromFrontier_Genera));

        oThreadDaylight_Genera = new Thread(new ThreadStart(GetResultObjectFromDaylight_Genera));

        oThreadUPSPackage_Ground = new Thread(() => GetResultObjectFromUPSPackageViaAPI("03"));

        oThreadUPSPackage_NextDayAir = new Thread(() => GetResultObjectFromUPSPackageViaAPI("01"));

        oThreadUPSPackage_SecondDayAir = new Thread(() => GetResultObjectFromUPSPackageViaAPI("02"));

        oThreadUPSPackage_3DaySelect = new Thread(() => GetResultObjectFromUPSPackageViaAPI("12"));

        oThreadUPSPackage_NextDayAirSaver = new Thread(() => GetResultObjectFromUPSPackageViaAPI("13"));

        oThreadUPSPackage_NextDayAirEarlyAM = new Thread(() => GetResultObjectFromUPSPackageViaAPI("14"));

        oThreadUPSPackage_2ndDayAirAM = new Thread(() => GetResultObjectFromUPSPackageViaAPI("59"));

        //Get CentralFreight Quote
        //oThreadCentralFreight = new Thread(new ThreadStart(GetResultObjectFromCentralFreight));

        //Get SEFL Qoute
        //oThreadSEFL = new Thread(new ThreadStart(GetResultObjectFromSEFL_AnyAccount));

        //Get R+L Rates
        oThreadRL = new Thread(new ThreadStart(GetResultObjectFromRAndL));

        oThreadRL_Genera = new Thread(new ThreadStart(GetResultObjectFromRAndL_Genera));

        // AAACooper       
        oThreadAAACooper = new Thread(new ThreadStart(GetResultObjectFromAAACooper));

        //Get R+L Rates for Griots Pricing
        //oThreadRLGriots = new Thread(new ThreadStart(GetResultObjectFromRAndLGriots));

        //Get RL quote for Puerto Rico
        //Thread oThreadRLPuertoRico = new Thread(new ThreadStart(GetResultObjectFromRLPuertoRico));

        //Get NEMF Rates
        //oThreadNEMF = new Thread(new ThreadStart(GetResultObjectFromNEMF));

        //Get Daylight Quote
        //Thread oThreadDayLight = new Thread(new ThreadStart(GetResultObjectFromDayLightQuote));

        //Get SAIA Qoute
        //oThreadSAIA = new Thread(new ThreadStart(GetResultObjectFromSAIA));

        //////////Get Result Object for RoadRunner SPC Pricing --- Not used
        //Thread oThreadRRSPC = new Thread(new ThreadStart(GetResultObjectFromRoadRunnerSPC));
        oThreadRRSPC = new Thread(new ThreadStart(GetResultObjectFromRRTS_ByAccountSPC));

        //////////Get Result Object for Dayton Freight SPC Pricing
        oThreadDayton = new Thread(new ThreadStart(GetResultObjectFromDaytonFreight));

        //Get USF Reddaway Qoute for DE Pricing for Prepaid shipments
        oThreadUSFREDDEPP = new Thread(new ThreadStart(GetResultObjectFromUSFReddawayDEPrepaid));

        //Get USF Reddaway Qoute for SPC Pricing for Prepaid shipments
        oThreadUSFREDSPCPP = new Thread(new ThreadStart(GetResultObjectFromUSFReddawaySPCPrepaid));

        //Get USF Reddaway Quote for Prepaid shipments
        oThreadUSFREDPP = new Thread(new ThreadStart(GetResultObjectFromUSFReddawayPrepaid));

        //Get USF Reddaway Qoute for SPC Pricing for Collect shipments
        //Thread oThreadUSFREDSPCC = new Thread(new ThreadStart(GetResultObjectFromUSFReddawaySPCCollect));

        //Get USF Holland Qoute for SPC Pricing for Prepaid shipments
        oThreadUSFHOLSPCPP = new Thread(new ThreadStart(GetResultObjectFromUSFHollandSPCPrepaid));

        //Get USF Holland Qoute for SPC Pricing for Collect shipments
        //Thread oThreadUSFHOLSPCC = new Thread(new ThreadStart(GetResultObjectFromUSFHollandSPCCollect));

        //Get SAIA Qoute for SPC Pricing
        oThreadSAIASPC = new Thread(new ThreadStart(GetResultObjectFromSAIASPC));

        //Get ODFL Quote
        //Thread oThreadODFL = new Thread(new ThreadStart(GetResultObjectFromOldDominion));

        //Get UPS Quote for Griots Pricing
        //Thread oThreadUPSGriots = new Thread(new ThreadStart(GetResultObjectFromUPSGriotsScrape));

        //Companies and markups from login table
        //Thread oThreadGetHTMLForInsurance = new Thread(new ThreadStart(GetHTMLForInsurance));
        //Thread oThreadGetHTMLForInsurance = new Thread(new ThreadStart(GetHTMLForInsuranceCacher));

        // oThreadConWayFreight Durachem Account
        oThreadXPO_Shug = new Thread(new ThreadStart(GetResultObjectFromXPO_Shug));

        oThreadXPO_DLS = new Thread(new ThreadStart(GetResultObjectFromXPO_DLS_account));

        oThreadEstes_DLS_account = new Thread(new ThreadStart(GetResultObjectFromEstesDLS_account));

        oThreadEstes_Genera_account = new Thread(new ThreadStart(GetResultObjectFromEstesGenera_account));

        #endregion

        #region Threads to add

        // oThreadConWayFreight Durachem Account
        oThreadConWayFreight = new Thread(new ThreadStart(GetResultObjectFromXPO_Durachem));

        #endregion

        #region Check countries


        if (quoteData.origCountry == "CANADA" && quoteData.destCountry == "CANADA")
        {


            //Response.Write("We cannot rate Inter Canada. We apologize.");

        }
        //else if (origCountry == "PR" && destCountry == "CANADA")
        //{
        //    Response.Write("We cannot rate Puerto Rico to Canada. We apologize.");

        //}
        //else if (origCountry == "CANADA" && destCountry == "PR")
        //{
        //    Response.Write("We cannot rate Canada to Puerto Rico. We apologize.");

        //}
        //else if (origCountry == "PR" && destCountry == "US")
        //{
        //    Response.Write("We cannot rate Puerto Rico to United States. We apologize.");

        //}
        //else if (origCountry == "PR" && destCountry == "PR")
        //{
        //    Response.Write("We cannot rate Inter Puerto Rico. We apologize.");
        //}

        #endregion

        //else if ((quoteData.origCountry == "US" && quoteData.destCountry == "PR") && m_lPiece[0].FreightClass != "")
        //{
        //    DB.Log("oThreadRLPuertoRico", "start thread");
        //    oThreadRLPuertoRico.Start();
        //    //oThreadRLPuertoRico.Join();
        //    if (!oThreadRLPuertoRico.Join(TimeSpan.FromSeconds(30)))
        //    {
        //        oThreadRLPuertoRico.Abort();
        //    }
        //}
        else // US to US or Canada
        {
            //UPS ups = new UPS();

            if (quoteData.hasFreightClass.Equals(true)) // If has class   
            {
                // Not tradeshow and has class start and join threads
                #region Has class start and join threads

                #region Start Threads

                //Start_volume_threads();

                //oThreadEstes_DLS_account.Start();
                //ltl_threads.Add(oThreadEstes_DLS_account);

                oThreadEstes_Genera_account.Start();
                ltl_threads.Add(oThreadEstes_Genera_account);

                oThreadDLS.Start();
                ltl_threads.Add(oThreadDLS);

                oThreadDLS_GLTL.Start();
                ltl_threads.Add(oThreadDLS_GLTL);

                if (quoteData.is_AAFES_quote.Equals(true))
                {
                    oThreadDLS_cust_rates.Start();
                    ltl_threads.Add(oThreadDLS_cust_rates);
                }

                oThreadDLS_BenFranklinCraftsMacon.Start();
                ltl_threads.Add(oThreadDLS_BenFranklinCraftsMacon);

                oThreadDLS_Genera.Start();
                ltl_threads.Add(oThreadDLS_Genera);

                oThreadDLS_HHG_Under_500.Start();
                ltl_threads.Add(oThreadDLS_HHG_Under_500);

                oThreadDLS_GLTL_HHG_Under_500.Start();
                ltl_threads.Add(oThreadDLS_GLTL_HHG_Under_500);

                oThreadUPS_FREIGHT_Genera.Start();
                ltl_threads.Add(oThreadUPS_FREIGHT_Genera);

                oThreadBestOvernite_Genera.Start();
                ltl_threads.Add(oThreadBestOvernite_Genera);

                oThreadPyle_Genera.Start();
                ltl_threads.Add(oThreadPyle_Genera);

                oThreadAveritt_Genera.Start();
                ltl_threads.Add(oThreadAveritt_Genera);

                oThreadNewPenn_Genera.Start();
                ltl_threads.Add(oThreadNewPenn_Genera);

                oThreadSMTL_Genera.Start();
                ltl_threads.Add(oThreadSMTL_Genera);

                oThreadFrontier_Genera.Start();
                ltl_threads.Add(oThreadFrontier_Genera);

                oThreadDaylight_Genera.Start();
                ltl_threads.Add(oThreadDaylight_Genera);

                //oThreadGetHTMLForInsurance.Start(); //insurance info

                oThreadAAACooper.Start();
                ltl_threads.Add(oThreadAAACooper);

                if (quoteData.isHazmat == false && quoteData.hasDimensions.Equals(true))
                {

                    UPS_PackageStartThreads();

                    #region Not used
                    //oThreadUPSPackage_Ground.Start();
                    //oThreadUPSPackage_NextDayAir.Start();
                    //oThreadUPSPackage_SecondDayAir.Start();
                    //oThreadUPSPackage_3DaySelect.Start();
                    //oThreadUPSPackage_NextDayAirSaver.Start();
                    //oThreadUPSPackage_NextDayAirEarlyAM.Start();
                    //oThreadUPSPackage_2ndDayAirAM.Start();
                    #endregion
                }

                //if (isPuertoRicoDest == false) // Gives a wrong rate for PR
                //{
                //    oThreadNEMF.Start();
                //    ltl_threads.Add(oThreadNEMF);
                //}

                if (AccessorialsObj.APTPU == false)
                {
                    oThreadRL.Start();
                    ltl_threads.Add(oThreadRL);
                }

                oThreadRL_Genera.Start();
                ltl_threads.Add(oThreadRL_Genera);

                if (!quoteData.isHHG && !quoteData.isCommodityLkupHHG && AccessorialsObj.RESPU == false)
                {
                    oThreadRR.Start();
                    ltl_threads.Add(oThreadRR);
                    oThreadRR_AAFES.Start();
                    ltl_threads.Add(oThreadRR_AAFES);
                }

                //if (AccessorialsObj.RESPU.Equals(false))
                //{
                //    oThreadSEFL.Start();
                //    ltl_threads.Add(oThreadSEFL);
                //}

                //oThreadSAIA.Start();

                oThreadDayton.Start();
                ltl_threads.Add(oThreadDayton);

                //oThreadODFL.Start();

                oThreadConWayFreight.Start();
                ltl_threads.Add(oThreadConWayFreight);

                if (quoteData.origZip.Equals("90040"))
                {
                    oThreadXPO_Shug.Start();
                    ltl_threads.Add(oThreadXPO_Shug);
                }

                #region USF Start threads 

                if (quoteData.origState == "WA" || quoteData.origState == "OR" || quoteData.origState == "CA" || quoteData.origState == "NV" || quoteData.origState == "ID" || quoteData.origState == "MT" ||
                quoteData.origState == "AZ" || quoteData.origState == "UT" || quoteData.origState == "WY" || quoteData.origState == "CO")
                {
                    oThreadUSFREDPP.Start();
                    ltl_threads.Add(oThreadUSFREDPP);
                }

                if (quoteData.origState == "WA" || quoteData.origState == "OR" || quoteData.origState == "CA" || quoteData.origState == "NV" || quoteData.origState == "ID" || quoteData.origState == "MT" ||
                    quoteData.origState == "AZ" || quoteData.origState == "UT" || quoteData.origState == "WY" || quoteData.origState == "CO")
                {
                    oThreadUSFREDSPCPP.Start();
                    ltl_threads.Add(oThreadUSFREDSPCPP);
                    oThreadUSFREDDEPP.Start();
                    ltl_threads.Add(oThreadUSFREDDEPP);
                }
                else if (quoteData.is_good_USF_Holland_orig_state == true)
                {
                    oThreadUSFHOLSPCPP.Start();
                    ltl_threads.Add(oThreadUSFHOLSPCPP);
                    //oThread_USF_volume.Start();
                }

                #endregion

                if (!quoteData.isHHG)
                {
                    oThreadRRSPC.Start();
                    ltl_threads.Add(oThreadRRSPC);
                }

                oThreadSAIASPC.Start();
                ltl_threads.Add(oThreadSAIASPC);
                //oThreadEstesSPC.Start();

                #endregion

                #endregion

            }
            else if (m_lPiece[0].Length > 0) // Any accessorial combination 
            {
                #region No Class provided
                // No Class provided
                //oThreadGetHTMLForInsurance.Start(); //insurance info

                if (quoteData.isHazmat == false && quoteData.hasDimensions.Equals(true))
                {
                    UPS_PackageStartThreads();
                    //ltl_threads.Add(UPS_PackageStartThreads);
                }

                #endregion
            }
        }
    }

    #endregion

    #region NetNetStartAndJoinThreads

    private void NetNetStartAndJoinThreads()
    {

        #region Check countries


        if (quoteData.origCountry == "CANADA" && quoteData.destCountry == "CANADA")
        {


            //Response.Write("We cannot rate Inter Canada. We apologize.");

        }
        //else if (origCountry == "PR" && destCountry == "CANADA")
        //{
        //    Response.Write("We cannot rate Puerto Rico to Canada. We apologize.");

        //}
        //else if (origCountry == "CANADA" && destCountry == "PR")
        //{
        //    Response.Write("We cannot rate Canada to Puerto Rico. We apologize.");

        //}
        //else if (origCountry == "PR" && destCountry == "US")
        //{
        //    Response.Write("We cannot rate Puerto Rico to United States. We apologize.");

        //}
        //else if (origCountry == "PR" && destCountry == "PR")
        //{
        //    Response.Write("We cannot rate Inter Puerto Rico. We apologize.");
        //}

        #endregion

        //else if ((quoteData.origCountry == "US" && quoteData.destCountry == "PR") && m_lPiece[0].FreightClass != "")
        //{
        //    DB.Log("oThreadRLPuertoRico", "start thread");
        //    oThreadRLPuertoRico.Start();
        //    //oThreadRLPuertoRico.Join();
        //    if (!oThreadRLPuertoRico.Join(TimeSpan.FromSeconds(30)))
        //    {
        //        oThreadRLPuertoRico.Abort();
        //    }
        //}
        else // US to US or Canada
        {
            //UPS ups = new UPS();

            if (quoteData.hasFreightClass.Equals(true)) // If has class   
            {

                #region Start Threads

                if (quoteData.is_Genera_quote == true)
                {
                    //Start_p44_threads();
                    Start_Genera_volume_threads();
                }
                else
                {
                    Start_volume_threads();
                }

                //if (quoteData.is_Genera_quote == true && quoteData.totalUnits > 6)
                //{
                //    // Do nothing
                //}
                //else
                //{
                //    NetNet_Start_LTL_threads();
                //}

                if (quoteData.is_Genera_quote == true)
                {
                    if(quoteData.totalUnits == 6 || quoteData.totalUnits == 7 || quoteData.totalUnits == 8 || quoteData.totalUnits == 9)
                    {
                        oThreadDLS_Genera = new Thread(new ThreadStart(GetResultObjectFromDLS_Genera));
                        Start_thread(ref oThreadDLS_Genera);
                    }
                    else if (quoteData.totalUnits < 6)
                    {
                        NetNet_Start_LTL_threads();
                    }
                        
                }
                else
                {
                    NetNet_Start_LTL_threads();
                }

                #endregion

                #region Join Threads
                
                if (quoteData.is_Genera_quote == true)
                {
                    //Join_p44_threads();
                    Join_Genera_volume_threads();
                }
                else
                {
                    Join_volume_threads();
                }

                //if (quoteData.is_Genera_quote == true && quoteData.totalUnits > 6)
                //{
                //    // Do nothing
                //}
                //else
                //{
                //    NetNet_Join_LTL_threads();
                //}

                if (quoteData.is_Genera_quote == true)
                {
                    if (quoteData.totalUnits == 6 || quoteData.totalUnits == 7 || quoteData.totalUnits == 8 || quoteData.totalUnits == 9)
                    {
                        Join_threads();
                    }
                    else if (quoteData.totalUnits < 6)
                    {
                        Join_threads();
                    }
                    //if (quoteData.totalUnits < 7)
                    //    Join_threads();
                }
                else
                {
                    Join_threads();
                }


                //NetNet_Join_LTL_threads();

                #endregion

            }
            else if (m_lPiece[0].Length > 0) // Any accessorial combination 
            {
                #region No Class provided

                //oThreadGetHTMLForInsurance.Join(); //insurance info

                #endregion
            }
        }
    }

    #endregion

    #region UPS_PackageStartThreads

    public void UPS_PackageStartThreads()
    {
        oThreadUPSPackage_Ground.Start();
        oThreadUPSPackage_NextDayAir.Start();
        oThreadUPSPackage_SecondDayAir.Start();
        oThreadUPSPackage_3DaySelect.Start();
        oThreadUPSPackage_NextDayAirSaver.Start();
        oThreadUPSPackage_NextDayAirEarlyAM.Start();
        oThreadUPSPackage_2ndDayAirAM.Start();
    }

    #endregion

    #region UPS_PackageJoinThreads

    public void UPS_PackageJoinThreads()
    {
        if (!oThreadUPSPackage_Ground.Join(TimeSpan.FromSeconds(12)))
        {
            oThreadUPSPackage_Ground.Abort();
        }
        if (!oThreadUPSPackage_NextDayAir.Join(TimeSpan.FromSeconds(12)))
        {
            oThreadUPSPackage_NextDayAir.Abort();
        }
        if (!oThreadUPSPackage_SecondDayAir.Join(TimeSpan.FromSeconds(12)))
        {
            oThreadUPSPackage_SecondDayAir.Abort();
        }
        if (!oThreadUPSPackage_3DaySelect.Join(TimeSpan.FromSeconds(12)))
        {
            oThreadUPSPackage_3DaySelect.Abort();
        }
        if (!oThreadUPSPackage_NextDayAirSaver.Join(TimeSpan.FromSeconds(12)))
        {
            oThreadUPSPackage_NextDayAirSaver.Abort();
        }
        if (!oThreadUPSPackage_NextDayAirEarlyAM.Join(TimeSpan.FromSeconds(12)))
        {
            oThreadUPSPackage_NextDayAirEarlyAM.Abort();
        }
        if (!oThreadUPSPackage_2ndDayAirAM.Join(TimeSpan.FromSeconds(12)))
        {
            oThreadUPSPackage_2ndDayAirAM.Abort();
        }
    }

    #endregion

    #region Start_volume_threads

    private void Start_volume_threads()
    {
        if (quoteData.hasFreightClass == false)
        {
            return;
        }

        if (quoteData.origCountry == "CANADA" && quoteData.destCountry == "CANADA")
        {
            //Response.Write("We cannot rate Inter Canada. We apologize.");
        }
        else if (quoteData.mode.Equals("ws") && quoteData.username.ToLower() == "gwwshipus")
        {
            // Do nothing
        }
        else if (quoteData.mode.Equals("NetNet") || quoteData.is_like_NetNet == true || quoteData.linealFeet > 0.0)
        {
            oThread_P44 = new Thread(new ThreadStart(GetResultObjectFrom_P44));

            oThread_ABF_volume = new Thread(new ThreadStart(Get_ABF_volume_rates));

            oThread_Estes_volume = new Thread(new ThreadStart(Get_ESTES_volume_rates));

            oThread_XPO_volume = new Thread(new ThreadStart(Get_rate_from_XPO_spot_quote));

            oThread_YRC_volume = new Thread(new ThreadStart(GetResultObjectFrom_YRC_Spot_Quote));

            oThread_Sunset_volume = new Thread(new ThreadStart(Get_Sunset_Pacific_volume_rates));

            oThreadTruckload_Genera = new Thread(new ThreadStart(Get_Genera_Truckload_rates));

            //

            oThread_Sunset_volume.Start();
            //ltl_threads.Add(oThread_Sunset_volume);

            oThread_XPO_volume.Start();


            oThread_YRC_volume.Start();

            oThread_Estes_volume.Start();

            oThread_P44.Start();

            oThread_ABF_volume.Start();

            oThreadTruckload_Genera.Start();

            if (quoteData.is_good_USF_Holland_orig_state == true)
            {
                oThread_USF_volume = new Thread(new ThreadStart(GetResultObjectFrom_USF_volume));
                oThread_USF_volume.Start();
            }
        }
        else
        {
            // Do nothing
        }
    }

    #endregion

    #region Join_volume_threads

    private void Join_volume_threads()
    {
        if (quoteData.origCountry == "CANADA" && quoteData.destCountry == "CANADA")
        {
            //Response.Write("We cannot rate Inter Canada. We apologize.");
        }
        else if (quoteData.mode.Equals("ws") && quoteData.username.ToLower() == "gwwshipus")
        {
            // Do nothing
        }
        else if (quoteData.mode.Equals("NetNet") || quoteData.is_like_NetNet == true || quoteData.linealFeet > 0.0)
        {
            if (!oThread_Sunset_volume.Join(TimeSpan.FromSeconds(25)))
            {
                oThread_Sunset_volume.Abort();
            }

            if (!oThread_XPO_volume.Join(TimeSpan.FromSeconds(25)))
            {
                oThread_XPO_volume.Abort();
            }

            if (!oThread_YRC_volume.Join(TimeSpan.FromSeconds(25)))
            {
                oThread_YRC_volume.Abort();
            }

            if (!oThread_Estes_volume.Join(TimeSpan.FromSeconds(25)))
            {
                oThread_Estes_volume.Abort();
            }

            if (!oThread_P44.Join(TimeSpan.FromSeconds(60)))
            {
                oThread_P44.Abort();
            }

            if (!oThread_ABF_volume.Join(TimeSpan.FromSeconds(25)))
            {
                oThread_ABF_volume.Abort();
            }

            if (!oThreadTruckload_Genera.Join(TimeSpan.FromSeconds(10)))
            {
                oThreadTruckload_Genera.Abort();
            }


            if (quoteData.is_good_USF_Holland_orig_state == true)
            {
                if (!oThread_USF_volume.Join(TimeSpan.FromSeconds(20)))
                {
                    oThread_USF_volume.Abort();
                }
            }
        }
        else
        {
            // Do nothing
        }
    }

    #endregion

    #region Start_Genera_volume_threads

    private void Start_Genera_volume_threads()
    {
        oThread_P44 = new Thread(new ThreadStart(GetResultObjectFrom_P44));
        oThreadTruckload_Genera = new Thread(new ThreadStart(Get_Genera_Truckload_rates));
        oThread_YRC_volume = new Thread(new ThreadStart(GetResultObjectFrom_YRC_Spot_Quote));

        oThread_P44.Start();
        oThreadTruckload_Genera.Start();
        oThread_YRC_volume.Start();
    }

    #endregion

    #region Join_Genera_volume_threads

    private void Join_Genera_volume_threads()
    {
        if (!oThread_P44.Join(TimeSpan.FromSeconds(60)))
        {
            oThread_P44.Abort();
        }

        if (!oThreadTruckload_Genera.Join(TimeSpan.FromSeconds(10)))
        {
            oThreadTruckload_Genera.Abort();
        }

        if (!oThread_YRC_volume.Join(TimeSpan.FromSeconds(20)))
        {
            oThread_YRC_volume.Abort();
        }
    }

    #endregion

    #region Start_join_LTL_threads_if_density_not_low

    private void Start_join_LTL_threads_if_density_not_low(string mode)
    {
        if (quoteData.linealFeet >= 20.0)
        {
            if (quoteData.totalWeight > 8800)
                if (mode == "start")
                    Start_threads();
                else
                    Join_threads();
        }
        else if (quoteData.linealFeet >= 19.0)
        {
            if (quoteData.totalWeight > 8200)
                if (mode == "start")
                    Start_threads();
                else
                    Join_threads();
        }
        else if (quoteData.linealFeet >= 18.0)
        {
            if (quoteData.totalWeight > 7700)
                if (mode == "start")
                    Start_threads();
                else
                    Join_threads();
        }
        else if (quoteData.linealFeet >= 17.0)
        {
            if (quoteData.totalWeight > 7200)
                if (mode == "start")
                    Start_threads();
                else
                    Join_threads();
        }
        else if (quoteData.linealFeet >= 16.0)
        {
            if (quoteData.totalWeight > 6600)
                if (mode == "start")
                    Start_threads();
                else
                    Join_threads();
        }
        else if (quoteData.linealFeet >= 15.0)
        {
            if (quoteData.totalWeight > 6100)
                if (mode == "start")
                    Start_threads();
                else
                    Join_threads();
        }
        else if (quoteData.linealFeet >= 14.0)
        {
            if (quoteData.totalWeight > 5600)
                if (mode == "start")
                    Start_threads();
                else
                    Join_threads();
        }
        else if (quoteData.linealFeet >= 13.0)
        {
            if (quoteData.totalWeight > 5100)
                if (mode == "start")
                    Start_threads();
                else
                    Join_threads();
        }
        else if (quoteData.linealFeet >= 12.0)
        {
            if (quoteData.totalWeight > 4700)
                if (mode == "start")
                    Start_threads();
                else
                    Join_threads();
        }
        else
        {
            if (mode == "start")
                Start_threads();
            else
                Join_threads();
        }
    }

    #endregion

    #region Start_join_LTL_threads

    private void Start_join_LTL_threads(string mode)
    {
        if (mode == "start")
            Start_threads();
        else
            Join_threads();
    }

    #endregion

    #endregion

    #region Helpers

    #region AddMarkup

    private void AddMarkup(ref bool isUPS_Package, ref GCMRateQuote objQuote, ref double finalMarkup,
        ref double rateAfterAddinLTLMarkup, ref double dblLTLMarkupRatio)
    {
        isUPS_Package = false;

        if (objQuote.DisplayName.ToUpper().Contains("UPS") &&
                (objQuote.DisplayName.ToUpper().Contains("GROUND") || objQuote.DisplayName.ToUpper().Contains("DAY")))
        {
            isUPS_Package = true;
        }

        double tempourRate = objQuote.TotalPrice;

        if (!isUPS_Package) // UPS Package sets it's own buy rate
        {
            objQuote.OurRate = tempourRate;
        }

        tempourRate = objQuote.GuaranteedRateAM;
        objQuote.OurRateGAM = tempourRate;

        tempourRate = objQuote.GuaranteedRatePM;
        objQuote.OurRateGPM = tempourRate;

        // Booking key is not #0#
        #region Booking key is not #0#

        if (!isUPS_Package && objQuote != null && !objQuote.BookingKey.Trim().Equals("#0#"))
        {

            finalMarkup = objQuote.TotalPrice * dblLTLMarkupRatio;

            //DB.Log("objQuote.DisplayName", objQuote.DisplayName);
            //DB.Log("markups", finalMarkup.ToString());

            if (quoteData.subdomain.Equals("spc"))
            {
                #region SPC

                if (quoteData.username.ToLower() == "iptrans" && finalMarkup > 0 && finalMarkup < 35 && !quoteData.subdomain.Equals("spc"))
                {
                    finalMarkup = 35;
                    //DB.Log("markups iptrans", finalMarkup.ToString());
                }
                else if (quoteData.is_dura_logic == true && finalMarkup > 0 && finalMarkup < 35)
                {
                    finalMarkup = 35;
                    //DB.Log("markups durachem", finalMarkup.ToString());
                }
                else if (finalMarkup > 0 && finalMarkup < 35 && !quoteData.subdomain.Equals("spc"))
                {
                    finalMarkup = 35;
                    //DB.Log("markups not iptrans", finalMarkup.ToString());
                }

                if (finalMarkup > spcMaxMarkup)
                {
                    finalMarkup = spcMaxMarkup;
                }

                #endregion
            }
            else
            {
                #region Not SPC

                // Not SPC
                if (quoteData.username.ToLower() == "iptrans" && finalMarkup > 0 && finalMarkup < 35 && !quoteData.subdomain.Equals("spc"))
                {
                    finalMarkup = 50;
                    //DB.Log("markups iptrans", finalMarkup.ToString());
                }
                else if (quoteData.is_dura_logic == true && finalMarkup > 0 && finalMarkup < 45)
                {
                    finalMarkup = 45;
                    //DB.Log("markups durachem", finalMarkup.ToString());
                }
                else if (quoteData.username.ToLower() == "icat" && finalMarkup < 30)
                {
                    finalMarkup = 30;
                    DB.Log("markups icat", finalMarkup.ToString());
                }
                else if (finalMarkup > 0 && finalMarkup < 35 && !quoteData.subdomain.Equals("spc"))
                {
                    finalMarkup = 35;
                    //DB.Log("markups not iptrans", finalMarkup.ToString());
                }

                #endregion
            }

            //If tyson, dont markup the GT rates
            if (quoteData.subdomain.Equals("tyson") && objQuote.CarrierKey != null && objQuote.CarrierKey.EndsWith(" GT"))
            {
                #region For Tyson Subdomain
                rateAfterAddinLTLMarkup = objQuote.TotalPrice;
                //SC
                //rateAfterAddinLTLMarkupGAM = objQuote.GuaranteedRateAM;
                //rateAfterAddinLTLMarkupGPM = objQuote.GuaranteedRatePM;
                #endregion
            }
            else
            {
                //rateAfterAddinLTLMarkup = objQuote.TotalPrice + finalMarkup;
                if (!(objQuote.CarrierKey == "R+LGriots") && !(objQuote.DisplayName == "R&L Pallet") &&
                    !objQuote.DisplayName.Contains("DHL") && !quoteData.isAssociationID_5.Equals(true) &&
                    !objQuote.DisplayName.Contains("Genera") && !objQuote.DisplayName.Contains("Frontier"))
                {
                    //DB.Log("markups", objQuote.DisplayName);
                    if (quoteData.mode.Equals("ws") && quoteData.username == AppCodeConstants.un_genera) // Genera
                    {
                        if (finalMarkup > 150.0)
                        {
                            rateAfterAddinLTLMarkup = objQuote.TotalPrice + 150.0;
                        }
                        else if (finalMarkup < 25.0)
                        {
                            rateAfterAddinLTLMarkup = objQuote.TotalPrice + 25.0;
                        }
                        else
                        {
                            rateAfterAddinLTLMarkup = objQuote.TotalPrice + finalMarkup;
                        }

                    }
                    else
                    {
                        rateAfterAddinLTLMarkup = objQuote.TotalPrice + finalMarkup;
                    }

                }
                else
                {
                    rateAfterAddinLTLMarkup = objQuote.TotalPrice;  // No markup for R&L
                }

            }

            //DB.Log("objQuote.TotalPrice", objQuote.TotalPrice.ToString());

            objQuote.TotalPrice = rateAfterAddinLTLMarkup;

            //DB.Log("rateAfterAddinLTLMarkup", rateAfterAddinLTLMarkup.ToString());

            #region Not used
            //#region For Standard Forwarding login
            //if (Session["svUserID"].ToString().ToLower().Equals("standard") && standardForwardingRate > objQuote.TotalPrice)
            //{
            //    objQuote.TotalPrice = standardForwardingRate;
            //}
            //#endregion

            #endregion
        }
        #endregion
    }

    #endregion

    #region SetInfoToObjectQuote

    private GCMRateQuote SetInfoToObjectQuote(ref double totalCharges, string DisplayName, string BookingKey, string CarrierKey,
        string Documentation, int DeliveryDays, string CarrierOnTimeName)
    {
        GCMRateQuote objQuote = new GCMRateQuote();
        objQuote.TotalPrice = totalCharges;
        objQuote.DisplayName = DisplayName;
        objQuote.BookingKey = BookingKey;
        objQuote.CarrierKey = CarrierKey;

        objQuote.Documentation = Documentation;

        objQuote.DeliveryDays = DeliveryDays;

        //CarsOnTime carOnTime;
        //if (Session["onTimeDict"] != null && ((Dictionary<string, CarsOnTime>)Session["onTimeDict"]).TryGetValue(CarrierOnTimeName, out carOnTime))
        //{
        //    objQuote.OnTimePercent = carOnTime.onTimePercent + '%';
        //    objQuote.ShipsBetweenStates = carOnTime.delivOnTime + carOnTime.delivLate;
        //}
        return objQuote;
    }

    #endregion

    #region AddCarrierResultsToArray

    // Checks each carrier result object and adds to array of results
    private void AddCarrierResultsToArray(ref int transitAddition, ref double addition, bool hasHazmat, ref int newLogId)
    {
        //sunset_volume_result
        DB.Log("AddCarrierResultsToArray", "1");

        #region sunset_volume_result

        if (sunset_volume_result.cost > 0)
        {
            GCMRateQuote objQuote = new GCMRateQuote
            {
                BookingKey = "#1#",
                CarrierKey = "UPS",
                DeliveryDays = sunset_volume_result.days,
                DisplayName = string.Concat("Sunset Pacific Volume ", sunset_volume_result.quote_id), //sunset_volume_result.quote_number
                OurRate = sunset_volume_result.cost,
                CarrierQuoteID = sunset_volume_result.quote_id,
                RateType = "Volume Quote",
                TotalPrice = sunset_volume_result.cost
            };

            objQuote.DeliveryDays += transitAddition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objQuote);
        }

        #endregion

        #region XPO_volume_result

        if (xpo_volume_result.cost > 0)
        {
            GCMRateQuote objQuote = new GCMRateQuote
            {
                BookingKey = "#1#",
                CarrierKey = "Con-way",
                DeliveryDays = xpo_volume_result.transit_days,
                DisplayName = string.Concat("XPO Volume ", xpo_volume_result.quote_number),
                OurRate = xpo_volume_result.cost,
                CarrierQuoteID = xpo_volume_result.quote_number,
                RateType = "Volume Quote",
                TotalPrice = xpo_volume_result.cost
            };

            objQuote.DeliveryDays += transitAddition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objQuote);
        }

        #endregion

        #region YRC_volume_result

        if (yrc_volume_result.cost > 0)
        {
            GCMRateQuote objQuote = new GCMRateQuote
            {
                BookingKey = "#1#",
                CarrierKey = "YRC",
                Scac = "RDWY",
                DeliveryDays = yrc_volume_result.transit_days,
                DisplayName = string.Concat("YRC Volume RRD ", yrc_volume_result.quote_number),
                OurRate = yrc_volume_result.cost,
                CarrierQuoteID = yrc_volume_result.quote_number,
                RateType = "Volume Quote",
                TotalPrice = yrc_volume_result.cost
            };

            objQuote.DeliveryDays += transitAddition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objQuote);
            found_yrc_volume = true;
        }

        #endregion

        #region Estes_volume_result

        if (estes_volume_result.cost > 0)
        {
            GCMRateQuote objQuote = new GCMRateQuote
            {
                BookingKey = "#1#",
                Scac = estes_volume_result.scac,

                CarrierKey = "Estes",

                DeliveryDays = estes_volume_result.transit_days,


                DisplayName = string.Concat(estes_volume_result.carrier_name, " Volume ", estes_volume_result.quote_number),
                CarrierQuoteID = estes_volume_result.quote_number,

                OurRate = estes_volume_result.cost,
                RateType = "Volume Quote",
                TotalPrice = estes_volume_result.cost
            };

            objQuote.DeliveryDays += transitAddition;

            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objQuote);
        }

        //

        if (estes_volume_economy_result.cost > 0)
        {
            GCMRateQuote objQuote = new GCMRateQuote
            {
                BookingKey = "#1#",
                Scac = estes_volume_economy_result.scac,
                CarrierKey = "Estes",
                DeliveryDays = estes_volume_economy_result.transit_days,
                DisplayName = string.Concat(estes_volume_economy_result.carrier_name, " Volume ",
                estes_volume_economy_result.quote_number),
                OurRate = estes_volume_economy_result.cost,
                CarrierQuoteID = estes_volume_economy_result.quote_number,
                RateType = "Volume Quote",
                TotalPrice = estes_volume_economy_result.cost
            };

            objQuote.DeliveryDays += transitAddition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objQuote);
        }

        //

        if (estes_volume_basic_result.cost > 0)
        {
            GCMRateQuote objQuote = new GCMRateQuote
            {
                BookingKey = "#1#",
                Scac = estes_volume_basic_result.scac,
                CarrierKey = "Estes",
                DeliveryDays = estes_volume_basic_result.transit_days,
                DisplayName = string.Concat(estes_volume_basic_result.carrier_name, " Volume ",
                estes_volume_basic_result.quote_number),
                OurRate = estes_volume_basic_result.cost,
                CarrierQuoteID = estes_volume_basic_result.quote_number,
                RateType = "Volume Quote",
                TotalPrice = estes_volume_basic_result.cost
            };

            objQuote.DeliveryDays += transitAddition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objQuote);
        }

        #endregion

        //abf_volume_result
        #region ABF_volume_result

        if (abf_volume_result.CHARGE > 0)
        {
            GCMRateQuote objQuote = new GCMRateQuote
            {
                BookingKey = "#1#",
                CarrierKey = "ABF",
                DeliveryDays = abf_volume_result.ESTIMATEDTRANSIT,
                DisplayName = string.Concat("ABF Volume ", abf_volume_result.QUOTEID),
                OurRate = abf_volume_result.CHARGE,
                CarrierQuoteID = abf_volume_result.QUOTEID,
                RateType = "Volume Quote",
                TotalPrice = abf_volume_result.CHARGE
            };

            objQuote.DeliveryDays += transitAddition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objQuote);
        }

        #endregion

        //Truckload_Quote_Genera
        #region Truckload_Quote_Genera

        if (Truckload_Quote_Genera != null && Truckload_Quote_Genera.TotalPrice > 0)
        {
            Truckload_Quote_Genera.DeliveryDays += transitAddition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, Truckload_Quote_Genera);
        }

        #endregion

        #region USF_volume_result

        if (usf_volume_result.cost > 0)
        {
            GCMRateQuote objQuote = new GCMRateQuote
            {
                BookingKey = "#1#",


                CarrierKey = "USF Reddaway",

                DeliveryDays = usf_volume_result.transit_days,


                DisplayName = string.Concat(usf_volume_result.carrier_name, " Volume ", usf_volume_result.quote_number),
                CarrierQuoteID = usf_volume_result.quote_number,

                OurRate = usf_volume_result.cost,
                RateType = "Volume Quote",
                TotalPrice = usf_volume_result.cost
            };

            objQuote.DeliveryDays += transitAddition;

            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objQuote);
        }

        #endregion

        #region P44_results

        if (P44_results != null)
        {
            //StringBuilder sb = new StringBuilder();
            for (byte i = 0; i < P44_results.Count; i++)
            {
                if(P44_results[i].carrier_name.Contains("YRC") && found_yrc_volume==true)
                {
                    continue;
                }

                if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera) 
                {
                    if (P44_results[i].carrier_name.Contains("FedEx"))
                    {
                        continue;
                    }
                }
                else
                {
                    // Do nothing
                }
                
                GCMRateQuote objQuote = new GCMRateQuote
                {
                    BookingKey = "#1#",

                    Scac = P44_results[i].scac,

                    // For now set the default to UPS
                    CarrierKey = "UPS",

                    DeliveryDays = P44_results[i].transit_days,


                    DisplayName = string.Concat(P44_results[i].carrier_name, " Volume ", P44_results[i].quote_number, " ",
                        P44_results[i].description),
                    CarrierQuoteID = P44_results[i].quote_number + " " + P44_results[i].description,

                    OurRate = P44_results[i].cost,
                    RateType = "Volume Quote",
                    TotalPrice = P44_results[i].cost
                };

                //string[] carriers_allowed = {
                //    "TOTL", "CTII","CNWY","FXFE","FXNL", "RETL", "SAIA", "DPHE", "HMES", "AACT", "EXLA",
                //    "SWFL", "DAFG", "PITD", "WARD", "PYLE", "CENF", "EXDF", "RDWY", "ABFC", "AVRT"
                //};

                //sb.Append(objQuote.Scac + " ");

                //if (objQuote.Scac == "SEFL")
                //{
                //    continue;
                //}

                Genera genera = new Genera();
                string[] genera_carriers_active = genera.Get_active_carriers("Volume");

                if (objQuote.Scac == "RDFS")
                {
                    objQuote.DeliveryDays *= 3;
                }

                objQuote.DeliveryDays += transitAddition;

                if (genera_carriers_active.Contains(objQuote.Scac))
                {
                    totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objQuote);
                }
                else
                {
                    //continue;
                    // Do nothing
                }
            }

            //DB.LogGenera("P44_results", "P44_results", sb.ToString());
        }

        #endregion

        #region UPS_FREIGHT_Quote_Genera

        if (UPS_FREIGHT_Quote_Genera != null && UPS_FREIGHT_Quote_Genera.TotalPrice > 0)
        {
            UPS_FREIGHT_Quote_Genera.DeliveryDays += transitAddition;
            UPS_FREIGHT_Quote_Genera.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, UPS_FREIGHT_Quote_Genera);
        }

        #endregion

        #region BestOvernite_Quote_Genera

        if (BestOvernite_Quote_Genera != null && BestOvernite_Quote_Genera.TotalPrice > 0)
        {
            BestOvernite_Quote_Genera.DeliveryDays += transitAddition;
            BestOvernite_Quote_Genera.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, BestOvernite_Quote_Genera);
        }

        #endregion

        #region Pyle_Quote_Genera

        if (Pyle_Quote_Genera != null && Pyle_Quote_Genera.TotalPrice > 0)
        {
            Pyle_Quote_Genera.DeliveryDays += transitAddition;
            Pyle_Quote_Genera.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, Pyle_Quote_Genera);
        }

        #endregion

        #region Averitt_Quote_Genera

        if (Averitt_Quote_Genera != null && Averitt_Quote_Genera.TotalPrice > 0)
        {
            Averitt_Quote_Genera.DeliveryDays += transitAddition;
            Averitt_Quote_Genera.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, Averitt_Quote_Genera);
        }

        #endregion

        #region NewPenn_Quote_Genera

        if (NewPenn_Quote_Genera != null && NewPenn_Quote_Genera.TotalPrice > 0)
        {
            NewPenn_Quote_Genera.DeliveryDays += transitAddition;
            NewPenn_Quote_Genera.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, NewPenn_Quote_Genera);
        }

        #endregion

        #region SMTL_Quote_Genera

        if (SMTL_Quote_Genera != null && SMTL_Quote_Genera.TotalPrice > 0)
        {
            SMTL_Quote_Genera.DeliveryDays += transitAddition;
            SMTL_Quote_Genera.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, SMTL_Quote_Genera);
        }

        #endregion

        #region Frontier_Quote_Genera

        if (Frontier_Quote_Genera != null && Frontier_Quote_Genera.TotalPrice > 0)
        {
            Frontier_Quote_Genera.DeliveryDays += transitAddition;
            Frontier_Quote_Genera.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, Frontier_Quote_Genera);
        }

        #endregion

        #region Daylight_Quote_Genera

        if (Daylight_Quote_Genera != null && Daylight_Quote_Genera.TotalPrice > 0)
        {
            Daylight_Quote_Genera.DeliveryDays += transitAddition;
            Daylight_Quote_Genera.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, Daylight_Quote_Genera);
        }

        #endregion

        #region UPS

        try
        {
            UPS_Package ups = new UPS_Package();
            ups.AddCarrierResultsToArray_UPS_Package(quoteData.mode, quoteData.username, ref transitAddition, ref addition, ref totalQuotes,
                ref resUPSGround,
                ref resUPSNextDayAir,

                ref resSecondDayAir,
                ref res3DaySelect,
                ref resNextDayAirSaver,
                ref resNextDayAirEarlyAM,
                ref res2ndDayAirAM);
        }
        catch (Exception e)
        {
            DB.Log("AddCarrierResultsToArray_UPS_Package", e.ToString());
        }

        #endregion
        
        #region Con Way

        // conWayFreightQuote
        if (conWayFreightQuote != null && conWayFreightQuote.TotalPrice > 0)
        {
            conWayFreightQuote.DeliveryDays += transitAddition;
            conWayFreightQuote.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, conWayFreightQuote);
        }

        if (xpoShugQuote != null && xpoShugQuote.TotalPrice > 0)
        {
            xpoShugQuote.DeliveryDays += transitAddition;
            xpoShugQuote.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, xpoShugQuote);
        }

        if (xpoDLS_Quote != null && xpoDLS_Quote.TotalPrice > 0)
        {
            xpoDLS_Quote.DeliveryDays += transitAddition;
            xpoDLS_Quote.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, xpoDLS_Quote);
        }

        #endregion

        #region Pitt Ohio API

        // Pitt Ohio API
        if (pittOhioQuoteAPI != null && pittOhioQuoteAPI.TotalPrice > 0)
        {
            pittOhioQuoteAPI.DeliveryDays += transitAddition;
            pittOhioQuoteAPI.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, pittOhioQuoteAPI);
        }

        // Pitt Ohio API_SPC
        if (pittOhioQuoteAPI_SPC != null && pittOhioQuoteAPI_SPC.TotalPrice > 0)
        {
            pittOhioQuoteAPI_SPC.DeliveryDays += transitAddition;
            pittOhioQuoteAPI_SPC.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, pittOhioQuoteAPI_SPC);
        }

        // Pitt Ohio API_Durachem
        if (pittOhioQuoteAPI_Durachem != null && pittOhioQuoteAPI_Durachem.TotalPrice > 0)
        {
            pittOhioQuoteAPI_Durachem.DeliveryDays += transitAddition;
            pittOhioQuoteAPI_Durachem.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, pittOhioQuoteAPI_Durachem);
        }

        #endregion

        #region Dayton

        if (objDaytonFreightResult != null)
        {
            if (objDaytonFreightResult.TotalPrice > 0)
            {
                if (addition > 0)
                {
                    objDaytonFreightResult.GuaranteedRateAM = -1;
                    objDaytonFreightResult.GuaranteedRatePM = -1;
                }

                objDaytonFreightResult.TotalPrice += addition;
                objDaytonFreightResult.DeliveryDays += transitAddition;

                totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objDaytonFreightResult);
            }
        }

        #endregion

        #region Estes

        if (estesQuote_DLS_account != null && estesQuote_DLS_account.TotalPrice > 0)
        {
            Estes estes = new Estes();
            estes.Add_result_to_array(ref estesQuote_DLS_account, ref totalQuotes);
        }

        #endregion

        #region Estes

        if (estesQuote_Genera_account != null && estesQuote_Genera_account.TotalPrice > 0)
        {
            Estes estes = new Estes();
            estes.Add_result_to_array(ref estesQuote_Genera_account, ref totalQuotes);
        }

        #endregion

        #region aaaCooperQuote

        // aaaCooperQuote
        if (aaaCooperQuote != null && aaaCooperQuote.TotalPrice > 0)
        {
            aaaCooperQuote.DeliveryDays += transitAddition;
            aaaCooperQuote.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, aaaCooperQuote);
        }

        #endregion

        #region YRC

        if (yrcQuote != null && !quoteData.isHHG && yrcQuote.DeliveryDays > 0 && yrcQuote.TotalPrice > 0) // No hhg for yrc
        {
            if (addition > 0)
            {
                yrcQuote.GuaranteedRateAM = -1;
                yrcQuote.GuaranteedRatePM = -1;
            }

            yrcQuote.TotalPrice += addition;
            yrcQuote.DeliveryDays += transitAddition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, yrcQuote);
        }

        if (yrcQuoteSPC != null && !quoteData.isHHG && yrcQuoteSPC.DeliveryDays > 0 && yrcQuoteSPC.TotalPrice > 0)
        {
            if (addition > 0)
            {
                yrcQuoteSPC.GuaranteedRateAM = -1;
                yrcQuoteSPC.GuaranteedRatePM = -1;
            }

            yrcQuoteSPC.DeliveryDays += transitAddition;
            yrcQuoteSPC.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, yrcQuoteSPC);
        }

        #endregion
        
        #region R&L

        if (rlQuoteGriots != null && rlQuoteGriots.DeliveryDays > 0 && rlQuoteGriots.TotalPrice > 0)
        {
            if (rlQuoteGriots.TotalPrice > 0)
            {
                if ((rlQuote != null && rlQuote.TotalPrice > 0) && rlQuoteGriots.TotalPrice < rlQuote.TotalPrice)
                {
                    if (addition > 0)
                    {
                        rlQuoteGriots.GuaranteedRateAM = -1;
                        rlQuoteGriots.GuaranteedRatePM = -1;
                    }

                    rlQuoteGriots.TotalPrice += addition;
                    //rlQuoteGriots.TotalPrice += (rlQuoteGriots.TotalPrice * .1);
                    rlQuoteGriots.DeliveryDays += transitAddition;

                    totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, rlQuoteGriots);

                    rlQuote = null;
                }
            }
        }

        if (rlQuote != null && rlQuote.DeliveryDays > 0 && rlQuote.TotalPrice > 0)
        {
            if (addition > 0)
            {
                rlQuote.GuaranteedRateAM = -1;
                rlQuote.GuaranteedRatePM = -1;
            }

            rlQuote.DeliveryDays += transitAddition;

            rlQuote.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, rlQuote);
        }

        if (rlQuote_Genera != null && rlQuote_Genera.DeliveryDays > 0 && rlQuote_Genera.TotalPrice > 0)
        {
            if (addition > 0)
            {
                rlQuote_Genera.GuaranteedRateAM = -1;
                rlQuote_Genera.GuaranteedRatePM = -1;
            }

            rlQuote_Genera.DeliveryDays += transitAddition;

            rlQuote_Genera.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, rlQuote_Genera);
        }

        //rl_quote_guaranteed
        if (rl_quote_guaranteed != null && rl_quote_guaranteed.DeliveryDays > 0 && rl_quote_guaranteed.TotalPrice > 0)
        {
            rl_quote_guaranteed.DeliveryDays += transitAddition;
            rl_quote_guaranteed.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, rl_quote_guaranteed);
        }

        if (rl_quote_guaranteed_Genera != null && rl_quote_guaranteed_Genera.DeliveryDays > 0 && rl_quote_guaranteed_Genera.TotalPrice > 0)
        {
            rl_quote_guaranteed_Genera.DeliveryDays += transitAddition;
            rl_quote_guaranteed_Genera.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, rl_quote_guaranteed_Genera);
        }

        //if (rlQuotePuertoRico != null && rlQuotePuertoRico.TotalPrice > 0)
        //{
        //    totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, rlQuotePuertoRico);
        //}

        if (rlQuoteMWI != null && rlQuoteMWI.DeliveryDays > 0 && rlQuoteMWI.TotalPrice > 0)
        {
            if (addition > 0)
            {
                rlQuoteMWI.GuaranteedRateAM = -1;
                rlQuoteMWI.GuaranteedRatePM = -1;
            }

            rlQuoteMWI.DeliveryDays += transitAddition;
            rlQuoteMWI.TotalPrice += addition;
            //rlQuoteMWI.TotalPrice += (rlQuoteMWI.TotalPrice * .1);
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, rlQuoteMWI);
        }

        #endregion

        #region Saia

        if (saiaQuote != null && saiaQuote.DeliveryDays > 0 && saiaQuote.TotalPrice > 0)
        {
            if (addition > 0)
            {
                saiaQuote.GuaranteedRateAM = -1;
                saiaQuote.GuaranteedRatePM = -1;
            }

            saiaQuote.DeliveryDays += transitAddition;
            saiaQuote.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, saiaQuote);
        }

        if (saiaQuoteSPC != null && saiaQuoteSPC.DeliveryDays > 0 && saiaQuoteSPC.TotalPrice > 0)
        {
            if (addition > 0)
            {
                saiaQuoteSPC.GuaranteedRateAM = -1;
                saiaQuoteSPC.GuaranteedRatePM = -1;
            }

            saiaQuoteSPC.DeliveryDays += transitAddition;
            saiaQuoteSPC.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, saiaQuoteSPC);
        }

        if (saiaQuoteMWI != null && saiaQuoteMWI.DeliveryDays > 0 && saiaQuoteMWI.TotalPrice > 0)
        {
            if (addition > 0)
            {
                saiaQuoteMWI.GuaranteedRateAM = -1;
                saiaQuoteMWI.GuaranteedRatePM = -1;
            }

            saiaQuoteMWI.DeliveryDays += transitAddition;
            saiaQuoteMWI.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, saiaQuoteMWI);
        }

        #endregion

        #region Roadrunner

        try
        {
            if (objRoadRunnerResult != null && objRoadRunnerResult.NetCharge > 0.0)
            {
                GCMRateQuote objQuote = new GCMRateQuote();

                objQuote.TotalPrice = objRoadRunnerResult.NetCharge;

                //DB.Log("rrts NetCharge", objQuote.TotalPrice.ToString());

                //

                double Charge = 0.0;
                if (objRoadRunnerResult.RateDetails.Length > 0)
                {
                    Charge = objRoadRunnerResult.RateDetails[0].Charge;
                    //DB.Log("rrts Charge", Charge.ToString());

                    //dupreRRTS_Buy = (Charge * 0.3) + (Charge * 0.213) + 19; // 0.3 is 70% discount, 21.3 is Fuel, 19 is Notification Charge
                    dupreRRTS_Buy = Charge * 0.3 * 1.215 + 19; // 0.3 is 70% discount, 21.3 is Fuel, 19 is Notification Charge

                    if (AccessorialsObj.LGPU)
                    {
                        dupreRRTS_Buy += 75;
                    }
                    if (AccessorialsObj.LGDEL)
                    {
                        dupreRRTS_Buy += 75;
                    }

                    //DB.Log("rrts dupreRRTS_Buy", dupreRRTS_Buy.ToString());

                }
                //DB.Log("rrts Charge", Charge.ToString());

                //

                #region On Time

                //CarsOnTime carOnTime;
                //if (Session["onTimeDict"] != null && ((Dictionary<string, CarsOnTime>)Session["onTimeDict"]).TryGetValue("RRTS", out carOnTime))
                //{
                //    objQuote.OnTimePercent = carOnTime.onTimePercent + '%';
                //    objQuote.ShipsBetweenStates = carOnTime.delivOnTime + carOnTime.delivLate;
                //}

                #endregion

                #region Accessorials

                if (objQuote.TotalPrice > 0 && AccessorialsObj.APTPU && AccessorialsObj.APTDEL) // Notification Charger
                {
                    objQuote.TotalPrice += 16;
                }
                if (objQuote.TotalPrice > 0 && HelperFuncs.GetCountryByZip(midOrigZip, true, quoteData.origState, quoteData.destState).Equals("CANADA"))
                {
                    objQuote.TotalPrice += 25;
                }
                if (objQuote.TotalPrice > 0 && HelperFuncs.GetCountryByZip(midDestZip, false, quoteData.origState, quoteData.destState).Equals("CANADA"))
                {
                    objQuote.TotalPrice += 25;
                }

                #endregion

                #region Subdomains

                //if (quoteData.subdomain.Equals("spc") || isCostPlus)
                //{
                //    objQuote.TotalPrice = HelperFuncs.addSPC_Addition(objQuote.TotalPrice);

                //}

                //if (quoteData.subdomain.Equals(HelperFuncs.Subdomains.clipper))
                //{
                //    objQuote.TotalPrice = HelperFuncs.addClipperSubdomain_Addition(objQuote.TotalPrice);
                //}

                #endregion

                objQuote.DisplayName = strRoadRunnerDisplay; //"Roadrunner Transportation Services";
                objQuote.BookingKey = "#1#";
                objQuote.CarrierKey = "RRTS";
                //
                //DB.Log("rrts bug", "display name: " + strRoadRunnerDisplay + " car key: RRTS");

                #region Transit Days

                int transitDays;
                transitDays = Convert.ToInt32(objRoadRunnerResult.RoutingInfo.EstimatedTransitDays);
                transitDays += 1;
                if (AccessorialsObj.TRADEPU || AccessorialsObj.TRADEDEL)
                {
                    transitDays += 2;
                }
                objQuote.DeliveryDays = transitDays;

                #endregion

                //DB.Log("Roadrunner (Live)", "before addition");
                if (objQuote.DeliveryDays > 0 || objQuote.TotalPrice > 0)
                {
                    if (addition > 0)
                    {
                        objQuote.GuaranteedRateAM = -1;
                        objQuote.GuaranteedRatePM = -1;
                    }

                    objQuote.DeliveryDays += transitAddition;
                    objQuote.TotalPrice += addition;
                    //DB.Log("Roadrunner (Live)", objQuote.TotalPrice.ToString());

                    totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objQuote);
                }
            }
        }
        catch (Exception e)
        {
            DB.Log("rrts", e.ToString());
        }

        #endregion

        #region RoadrunnerClass50

        try
        {
            if (objRoadRunnerResultClass50 != null && objRoadRunnerResultClass50.NetCharge > 0.0)
            {
                GCMRateQuote objQuote = new GCMRateQuote();

                objQuote.TotalPrice = objRoadRunnerResultClass50.NetCharge;

                //DB.Log("rrts NetChargeClass50", objQuote.TotalPrice.ToString());

                //

                double Charge = 0.0;
                if (objRoadRunnerResultClass50.RateDetails.Length > 0)
                {
                    Charge = objRoadRunnerResultClass50.RateDetails[0].Charge;
                    //DB.Log("rrts Class 50 Charge", Charge.ToString());

                    aafesRRTS_Buy = Charge * 0.35 * 1.215; // 0.3 is 65% discount, 21.3 is Fuel, 19 is Notification Charge
                                                           //DB.Log("rrts aafesRRTS_Buy", aafesRRTS_Buy.ToString());

                }
                //DB.Log("rrts Charge", Charge.ToString());

            }
        }
        catch (Exception e)
        {
            DB.Log("rrts", e.ToString());
        }

        #endregion

        #region objRoadRunnerResultAAFES

        try
        {
            if (objRoadRunnerResultAAFES != null && objRoadRunnerResultAAFES.NetCharge > 0.0)
            {
                GCMRateQuote objQuote = new GCMRateQuote();

                objQuote.TotalPrice = objRoadRunnerResultAAFES.NetCharge;

                //if (isSPCSubdomain)
                //{
                //    objQuote.TotalPrice = HelperFuncs.addSPC_Addition(objQuote.TotalPrice);
                //}

                if (objQuote.TotalPrice > 0 && AccessorialsObj.APTPU && AccessorialsObj.APTDEL)//Notification Charger
                {
                    objQuote.TotalPrice += 16;
                }
                if (objQuote.TotalPrice > 0 && HelperFuncs.GetCountryByZip(midOrigZip, true, quoteData.origState, quoteData.destState).Equals("CANADA"))
                {
                    objQuote.TotalPrice += 25;
                }
                if (objQuote.TotalPrice > 0 && HelperFuncs.GetCountryByZip(midOrigZip, true, quoteData.origState, quoteData.destState).Equals("CANADA"))
                {
                    objQuote.TotalPrice += 25;
                }

                objQuote.DisplayName = "Roadrunner - AAFES"; //"";
                                                             //objQuote.DisplayName = strRoadRunnerDisplay; //"Roadrunner Transportation Services";
                objQuote.BookingKey = "#1#";
                objQuote.CarrierKey = "RRTS";
                int transitDays;
                transitDays = Convert.ToInt32(objRoadRunnerResultAAFES.RoutingInfo.EstimatedTransitDays);
                transitDays += 1;
                if (AccessorialsObj.TRADEPU || AccessorialsObj.TRADEDEL)
                {
                    transitDays += 2;
                }
                objQuote.DeliveryDays = transitDays;

                if (objQuote.DeliveryDays > 0 || objQuote.TotalPrice > 0)
                {
                    if (addition > 0)
                    {
                        objQuote.GuaranteedRateAM = -1;
                        objQuote.GuaranteedRatePM = -1;
                    }
                    objQuote.DeliveryDays += transitAddition;
                    objQuote.TotalPrice += addition;

                    totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objQuote);
                }
            }
        }
        catch (Exception e)
        {
            DB.Log("rrts", e.ToString());
        }

        #endregion

        #region Roadrunner SPC

        try
        {
            if (objRoadRunnerResultSPC != null && objRoadRunnerResultSPC.NetCharge > 0.0)
            {
                GCMRateQuote objQuote = new GCMRateQuote();

                objQuote.TotalPrice = objRoadRunnerResultSPC.NetCharge;
                //DB.Log("Roadrunner SPC (cost)", objQuote.TotalPrice.ToString());

                //CarsOnTime carOnTime;
                //if (Session["onTimeDict"] != null && ((Dictionary<string, CarsOnTime>)Session["onTimeDict"]).TryGetValue("RRTS", out carOnTime))
                //{
                //    objQuote.OnTimePercent = carOnTime.onTimePercent + '%';
                //    objQuote.ShipsBetweenStates = carOnTime.delivOnTime + carOnTime.delivLate;
                //}

                if (objQuote.TotalPrice > 0 && AccessorialsObj.APTPU && AccessorialsObj.APTDEL)//Notification Charger
                {
                    //appointment waived
                    objQuote.TotalPrice += 0;
                }

                //if (quoteData.subdomain.Equals("spc") || isCostPlus)
                //{
                //    objQuote.TotalPrice = HelperFuncs.addSPC_Addition(objQuote.TotalPrice);
                //}

                objQuote.DisplayName = "Roadrunner SPC Pricing"; //"Roadrunner Transportation Services";
                objQuote.BookingKey = "---1---";
                objQuote.CarrierKey = "RRTSSPC";
                int transitDays;
                transitDays = Convert.ToInt32(objRoadRunnerResultSPC.RoutingInfo.EstimatedTransitDays);
                transitDays += 1;
                if (AccessorialsObj.TRADEPU || AccessorialsObj.TRADEDEL)
                {
                    transitDays += 2;
                }
                objQuote.DeliveryDays = transitDays;

                if (objQuote.DeliveryDays > 0 || objQuote.TotalPrice > 0)
                {
                    if (addition > 0)
                    {
                        objQuote.GuaranteedRateAM = -1;
                        objQuote.GuaranteedRatePM = -1;
                    }

                    objQuote.TotalPrice += addition;
                    objQuote.DeliveryDays += transitAddition;

                    totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objQuote);
                }
            }
        }
        catch (Exception e)
        {
            DB.Log("rrts", e.ToString());
        }

        #endregion

        #region USF

        if (usfReddawayQuoteDEPrepaid != null && usfReddawayQuoteDEPrepaid.DeliveryDays > 0 && usfReddawayQuoteDEPrepaid.TotalPrice > 0)
        {
            if (addition > 0)
            {
                usfReddawayQuoteDEPrepaid.GuaranteedRateAM = -1;
                usfReddawayQuoteDEPrepaid.GuaranteedRatePM = -1;
            }

            usfReddawayQuoteDEPrepaid.DeliveryDays += transitAddition;
            usfReddawayQuoteDEPrepaid.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, usfReddawayQuoteDEPrepaid);
        }

        if (usfReddawayQuotePrepaid_Packwest != null && usfReddawayQuotePrepaid_Packwest.DeliveryDays > 0 &&
            usfReddawayQuotePrepaid_Packwest.TotalPrice > 0)
        {
            if (addition > 0)
            {
                usfReddawayQuotePrepaid_Packwest.GuaranteedRateAM = -1;
                usfReddawayQuotePrepaid_Packwest.GuaranteedRatePM = -1;
            }

            usfReddawayQuotePrepaid_Packwest.DeliveryDays += transitAddition;
            usfReddawayQuotePrepaid_Packwest.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, usfReddawayQuotePrepaid_Packwest);
        }

        if (usfReddawayQuoteSPCPrepaid != null && usfReddawayQuoteSPCPrepaid.DeliveryDays > 0 && usfReddawayQuoteSPCPrepaid.TotalPrice > 0)
        {
            if (addition > 0)
            {
                usfReddawayQuoteSPCPrepaid.GuaranteedRateAM = -1;
                usfReddawayQuoteSPCPrepaid.GuaranteedRatePM = -1;
            }

            usfReddawayQuoteSPCPrepaid.DeliveryDays += transitAddition;
            usfReddawayQuoteSPCPrepaid.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, usfReddawayQuoteSPCPrepaid);
        }

        if (usfReddawayQuotePrepaid != null && usfReddawayQuotePrepaid.DeliveryDays > 0 && usfReddawayQuotePrepaid.TotalPrice > 0)
        {
            if (addition > 0)
            {
                usfReddawayQuotePrepaid.GuaranteedRateAM = -1;
                usfReddawayQuotePrepaid.GuaranteedRatePM = -1;
            }

            usfReddawayQuotePrepaid.DeliveryDays += transitAddition;
            usfReddawayQuotePrepaid.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, usfReddawayQuotePrepaid);
        }

        if (usfReddawayQuoteSPCCollect != null && usfReddawayQuoteSPCCollect.DeliveryDays > 0 && usfReddawayQuoteSPCCollect.TotalPrice > 0)
        {
            if (addition > 0)
            {
                usfReddawayQuoteSPCCollect.GuaranteedRateAM = -1;
                usfReddawayQuoteSPCCollect.GuaranteedRatePM = -1;
            }

            usfReddawayQuoteSPCCollect.DeliveryDays += transitAddition;
            usfReddawayQuoteSPCCollect.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, usfReddawayQuoteSPCCollect);
        }

        if (usfHollandQuoteSPCPrepaid != null && usfHollandQuoteSPCPrepaid.DeliveryDays > 0 && usfHollandQuoteSPCPrepaid.TotalPrice > 0)
        {
            if (addition > 0)
            {
                usfHollandQuoteSPCPrepaid.GuaranteedRateAM = -1;
                usfHollandQuoteSPCPrepaid.GuaranteedRatePM = -1;
            }

            usfHollandQuoteSPCPrepaid.DeliveryDays += transitAddition;
            usfHollandQuoteSPCPrepaid.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, usfHollandQuoteSPCPrepaid);
        }

        if (usfHollandQuoteSPCCollect != null && usfHollandQuoteSPCCollect.DeliveryDays > 0 && usfHollandQuoteSPCCollect.TotalPrice > 0)
        {
            if (addition > 0)
            {
                usfHollandQuoteSPCCollect.GuaranteedRateAM = -1;
                usfHollandQuoteSPCCollect.GuaranteedRatePM = -1;
            }

            usfHollandQuoteSPCCollect.DeliveryDays += transitAddition;
            usfHollandQuoteSPCCollect.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, usfHollandQuoteSPCCollect);
        }

        if (usfReddawayQuoteMWIPrepaid != null && usfReddawayQuoteMWIPrepaid.DeliveryDays > 0 && usfReddawayQuoteMWIPrepaid.TotalPrice > 0)
        {
            if (addition > 0)
            {
                usfReddawayQuoteMWIPrepaid.GuaranteedRateAM = -1;
                usfReddawayQuoteMWIPrepaid.GuaranteedRatePM = -1;
            }

            usfReddawayQuoteMWIPrepaid.DeliveryDays += transitAddition;
            usfReddawayQuoteMWIPrepaid.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, usfReddawayQuoteMWIPrepaid);
        }

        if (usfHollandQuoteMWIPrepaid != null && usfHollandQuoteMWIPrepaid.DeliveryDays > 0 && usfHollandQuoteMWIPrepaid.TotalPrice > 0)
        {
            if (addition > 0)
            {
                usfHollandQuoteMWIPrepaid.GuaranteedRateAM = -1;
                usfHollandQuoteMWIPrepaid.GuaranteedRatePM = -1;
            }

            usfHollandQuoteMWIPrepaid.DeliveryDays += transitAddition;
            usfHollandQuoteMWIPrepaid.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, usfHollandQuoteMWIPrepaid);
        }

        #endregion

        #region ODFL

        if (odflQuote != null && odflQuote.DeliveryDays > 0 && odflQuote.TotalPrice > 0)
        {
            //if (addition > 0)
            //{
            //    odflQuote.GuaranteedRateAM = -1;
            //    odflQuote.GuaranteedRatePM = -1;
            //}

            odflQuote.DeliveryDays += transitAddition;
            odflQuote.TotalPrice += addition;
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, odflQuote);
        }

        //if (odflMWIQuote != null && odflMWIQuote.DeliveryDays > 0 && odflMWIQuote.TotalPrice > 0)
        //{
        //    if (addition > 0)
        //    {
        //        odflMWIQuote.GuaranteedRateAM = -1;
        //        odflMWIQuote.GuaranteedRatePM = -1;
        //    }

        //    odflMWIQuote.DeliveryDays += transitAddition;
        //    odflMWIQuote.TotalPrice += addition;
        //    totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, odflMWIQuote);
        //}

        #endregion

        DB.Log("AddCarrierResultsToArray", "before dlsPricesheets");

        #region dlsPricesheets

        if (quoteData.showDLSRates == true)
        {
            DB.Log("AddCarrierResultsToArray", "showDLSRates == true");

            Add_DLS_results add_dls_results = new Add_DLS_results(ref quoteData);
            add_dls_results.transitAddition = transitAddition;
            add_dls_results.addition = addition;
            add_dls_results.serviceRRD = "regularServiceRRD";
            add_dls_results.dupreRRTS_Buy = dupreRRTS_Buy;
            add_dls_results.aafesRRTS_Buy = aafesRRTS_Buy;
            add_dls_results.spcMaxMarkup = spcMaxMarkup;
            //add_dls_results.UserName = UserName;
            add_dls_results.elapsed_milliseconds_DLS = elapsed_milliseconds_DLS;
            add_dls_results.got_Estes_HHG_Under_500_rate = got_Estes_HHG_Under_500_rate;

            // dlsPricesheets
            if (dlsPricesheets != null && dlsPricesheets.Count > 0)
            {
                string UserName = "PNW - Burien WA";
                add_dls_results.AddDLS_ResultToArray(ref dlsPricesheets, ref totalQuotes, ref UserName);
            }

            // dlsPricesheets
            if (dls_cust_rates_Pricesheets != null && dls_cust_rates_Pricesheets.Count > 0)
            {
                //string UserName = "The Exchange";
                add_dls_results.AddDLS_ResultToArray_cust_rates(ref dls_cust_rates_Pricesheets, ref totalQuotes);
            }

            //dlsPricesheets_BenFranklinCraftsMacon
            if (dlsPricesheets_BenFranklinCraftsMacon != null && dlsPricesheets_BenFranklinCraftsMacon.Count > 0)
            {
                string UserName = "Ben Franklin Crafts - Macon";
                //DB.Log("dlsPricesheets_BenFranklinCraftsMacon.Count > 0", "");
                add_dls_results.AddDLS_ResultToArray(ref dlsPricesheets_BenFranklinCraftsMacon, ref totalQuotes, ref UserName);

            }
            else
            {
                //DB.Log("dlsPricesheets_BenFranklinCraftsMacon", " was empty");
            }

            //DB.Log("dlsPricesheets_Genera Count", dlsPricesheets_Genera.Count.ToString());

            if (dlsPricesheets_Genera != null && dlsPricesheets_Genera.Count > 0)
            {
                string UserName = "Genera Corp";
                DB.Log("dlsPricesheets_Genera.Count > 0", "");
                add_dls_results.AddDLS_ResultToArray(ref dlsPricesheets_Genera, ref totalQuotes, ref UserName);

            }
            else
            {
                DB.Log("dlsPricesheets_Genera", " was empty");
            }

            if (dlsPricesheets_HHG_Under_500 != null && dlsPricesheets_HHG_Under_500.Count > 0)
            {
                //string UserName = "Genera Corp";
                //DB.Log("dlsPricesheets_HHG_Under_500.Count > 0", "");
                add_dls_results.AddDLS_ResultToArray_HHG_Under_500(ref dlsPricesheets_HHG_Under_500, ref totalQuotes);

            }
            else
            {
                //DB.Log("dlsPricesheets_HHG_Under_500", " was empty");
            }

            if (totalQuotes != null && totalQuotes.Length > 0)
            {
                IEnumerable<GCMRateQuote> query = totalQuotes.OrderBy(x => x.TotalPrice);

                totalQuotes = query.ToArray();
            }

            #region Add the Guaranteed RRD results

            // Add the Guaranteed RRD results
            if (dlsPricesheetsGLTL != null && dlsPricesheetsGLTL.Count > 0 && (quoteData.hasAccessorials.Equals(false) || AccessorialsObj.TRADEDEL.Equals(true)) &&
                quoteData.isHazmat.Equals(false))
            {
                // GLTL_ServiceRRD
                add_dls_results.serviceRRD = "GLTL_ServiceRRD";

                //DB.Log("DLS GLTL", "DLS GLTL");
                string UserName = "PNW - Burien WA";
                add_dls_results.AddDLS_ResultToArray(ref dlsPricesheetsGLTL, ref totalQuotes, ref UserName);

                //foreach (SharedLTL.dlsPricesheet objCarrier in dlsPricesheetsGLTL)
                //{
                //    DB.Log("DLS GLTL", string.Concat(objCarrier.CarrierName, " - ", objCarrier.Total));
                //}
            }

            #endregion

            #region AddDLS_ResultToArray_HHG_Under_500

            // Add the Guaranteed RRD results
            if (dlsPricesheetsGLTL_HHG_Under_500 != null && dlsPricesheetsGLTL_HHG_Under_500.Count > 0 && (quoteData.hasAccessorials.Equals(false) || AccessorialsObj.TRADEDEL.Equals(true)) &&
                quoteData.isHazmat.Equals(false))
            {

                add_dls_results.serviceRRD = "GLTL_ServiceRRD Estes";

                add_dls_results.AddDLS_ResultToArray_HHG_Under_500(ref dlsPricesheetsGLTL_HHG_Under_500, ref totalQuotes);

                //foreach (SharedLTL.dlsPricesheet objCarrier in dlsPricesheetsGLTL)
                //{
                //    DB.Log("DLS GLTL", string.Concat(objCarrier.CarrierName, " - ", objCarrier.Total));
                //}
            }

            #endregion


        }
        else
        {
            if (totalQuotes != null && totalQuotes.Length > 0)
            {
                IEnumerable<GCMRateQuote> query = totalQuotes.OrderBy(x => x.TotalPrice);

                totalQuotes = query.ToArray();
            }
        }


        #endregion

    }

    #endregion

    #endregion

}
