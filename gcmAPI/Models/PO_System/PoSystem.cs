#region Using

using System;
using System.Data;
using System.Configuration;
using System.Collections;

using System.Collections.Specialized;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Net;
using System.IO;
using System.Text;
using System.Xml;
using System.Threading;
//using System.Web.Services.Protocols;

using System.Linq;
//using DocumentFormat.OpenXml;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing;


#endregion

// Live

public static class PoSystem
{

    #region Variables

    public struct Info
    {
        public string origZip, destZip, origCity, destCity, origState, destState, origCompName, destCompName, origAddr1, destAddr1, origAddr2, destAddr2,
            destAddr3, BookedOnShipType, combinedPOs;
        public string origContact, origPhone, destContact, destPhone, carrierToBidWith, CarrierShippingWith, CarrierFromUser, TrackingNumber, notesFromVendorToCustomer, carrierToBidWithPhone, Ponumber, Routing;

        public decimal freightBidAmount, savedAmount;

        public int totalBoxes, totalWeight, totalCube, totalPieces, totalPallets, totalPiecesDims, spGCMCompID, ccGCMCompID, poDataID, QuoteID, UPSParcelQuoteID, carrierToBidWithID;
        public DateTime shipmentReadyDate, shipByDate, shipAfterDate;

        public List<double> fClass;
        public List<double> weight;
        public List<double> nmfc;
        public List<int> pieces;
        public List<int> palletCount;
        public List<int> cube;
        public List<int> length, width, height, piecesDims, weightDims;

        public List<string> commodity, combinedPOsList, combinedPO_IDsList;

        public bool routedByVendor;

        public double totalWeightFromDimsPopup;
        public double cubicFeetFromDimsPopup;

        public int CompIDbyAcctNum;

    }



    #region Podata

    public class Podata
    {

        public string Ponumber;

        public DateTime DateEntered;

        public DateTime ShipbyDate, ShipAfterDate;

        public string EnteredBy;

        public decimal POvalue;

        public int VendorID;

        public int ReceivingID;

        public int Pieces;

        public int CategoryID;

        public string VendorNotes, ShipperNotes, Category, VendorCarrier;

        public bool FreeFreight;

    }

    #endregion

    static Info infoGlobal;
    static string pickupServiceGlobal = "";
    static string deliveryServiceGlobal = "";
    static string[] additionalServicesGlobal;
    static gcmAPI.gcmWebService.RateService2 rsGlobal;
    static DateTime pickupDateGlobal;
    static gcmAPI.gcmWebService.LTLRateReply lReplyGlobal;
    //static gcmAPI.gcmWebService.PackageBookingReply packageBookingReplyGlobal;
    static gcmAPI.gcmWebService.PackageRateReply packageReplyGlobal;

    #endregion

    #region GetAPI_AndPackageRates

    //public static void GetAPI_AndPackageRates(ref PoSystem.Info info, ref gcmAPI.gcmWebService.RateService2 rs,
    //    ref DateTime pickupDate, ref string pickupService, ref string deliveryService, ref string[] additionalServices, ref gcmAPI.gcmWebService.LTLRateReply lReply,
    //    ref gcmAPI.gcmWebService.PackageBookingReply packageBookingReply, ref gcmAPI.gcmWebService.PackageRateReply packageReply)
    //{
    //    // Set the global variables to parameters
    //    infoGlobal = info;
    //    pickupServiceGlobal = pickupService;
    //    deliveryServiceGlobal = deliveryService;
    //    additionalServicesGlobal = additionalServices;
    //    rsGlobal = rs;
    //    pickupDateGlobal = pickupDate;

    //    //lReplyGlobal = lReply;
    //    //packageBookingReplyGlobal = packageBookingReply;
    //    //packageReplyGlobal = packageReply;


    //    //--

    //    Thread oThreadLTL = new Thread(new ThreadStart(GetGCM_API_Rates_2));
    //    Thread oThreadUPS_Package = new Thread(new ThreadStart(getPackageRate_2));

    //    oThreadLTL.Start();
    //    oThreadUPS_Package.Start();

    //    if (!oThreadLTL.Join(TimeSpan.FromSeconds(50)))
    //    {
    //        oThreadLTL.Abort();
    //    }

    //    if (!oThreadUPS_Package.Join(TimeSpan.FromSeconds(20)))
    //    {
    //        oThreadUPS_Package.Abort();
    //    }

    //    //--

    //    // Set the thread results to the parameters
    //    lReply = lReplyGlobal;

    //    //HelperFuncs.writeToSiteErrors("lReply.LTLRates threads done", lReply.LTLRates.Length.ToString());

    //    packageReply = packageReplyGlobal;
    //    packageBookingReply = packageBookingReplyGlobal;

    //}

    #region Get rate functions using global variables instead of parameters

    #region GetGCM_API_Rates_2

    public static void GetGCM_API_Rates_2()
    {
        try
        {
            #region Set origin and destination locations
            string originZip = infoGlobal.origZip;    	// Origin ZIP code 
            string originCity = infoGlobal.origCity; 	// Origin city
            string originState = infoGlobal.origState;		// Origin State

            string destinationZip = infoGlobal.destZip;	// Destination ZIP code
            string destinationCity = infoGlobal.destCity; // Destination city
            string destinationState = infoGlobal.destState;	  //Destination state
            #endregion

            #region Line Items

            gcmAPI.gcmWebService.LTLPiece[] lineItems = new gcmAPI.gcmWebService.LTLPiece[infoGlobal.weight.Count];

            for (byte i = 0; i < infoGlobal.weight.Count; i++)
            {
                lineItems[i] = new gcmAPI.gcmWebService.LTLPiece();

                lineItems[i].FreightClass = infoGlobal.fClass[i].ToString();

                lineItems[i].Weight = Convert.ToInt32(infoGlobal.weight[i]);

                lineItems[i].Quantity = infoGlobal.pieces[i];

                lineItems[i].ItemUnit = "PALLET_XXX";

                //lineItems[i].HazMat = info.hazMat; // Hazmat is not implemented yet

                lineItems[i].HazMat = false; // Not implemented

                lineItems[i].Tag = "1";

                lineItems[i].Commodity = "NEW";

                //HelperFuncs.writeToSiteErrors("fClass", infoGlobal.fClass[i].ToString());

                //HelperFuncs.writeToSiteErrors("weight", infoGlobal.weight[i].ToString());

                //HelperFuncs.writeToSiteErrors("palletCount", infoGlobal.palletCount[i].ToString());

                if (infoGlobal.length.Count == infoGlobal.weight.Count)
                {
                    lineItems[i].Length = Convert.ToDouble(infoGlobal.length[i]);
                }
                //else
                //{
                //    lineItems[i].Length = Convert.ToDouble(infoGlobal.length[i]);
                //}
                //else
                //{
                //    lineItems[i].Length = 
                //}

                if (infoGlobal.width.Count == infoGlobal.weight.Count)
                {
                    lineItems[i].Width = Convert.ToDouble(infoGlobal.width[i]);
                }
                //else
                //{
                //    lineItems[i].Width = Convert.ToDouble(infoGlobal.width[i]);
                //}

                if (infoGlobal.height.Count == infoGlobal.weight.Count)
                {
                    lineItems[i].Height = Convert.ToDouble(infoGlobal.height[i]);
                }
                //else
                //{
                //    lineItems[i].Height = Convert.ToDouble(infoGlobal.height[i]);
                //}
            }
            #endregion

            //gcmWebService.LTLRateReply lReply = rs.GetLTLBookingRate(pickupDate, originZip, originCity, originState, destinationZip, destinationCity, destinationState, lineItems, pickupService,
            //    deliveryService, additionalServices);

            string UserName = "";

            // If CompIDbyAcctNum is not empty then the account (username) was entered by vendor (on the fly) and need to apply markup according to this username
            if (!infoGlobal.CompIDbyAcctNum.Equals(-1))
            {
                UserName = HelperFuncs.getUsernameByCompID(infoGlobal.CompIDbyAcctNum);
            }

            //lReplyGlobal = rsGlobal.GetLTLBookingRatePoSystem(pickupDateGlobal, originZip, originCity, originState, destinationZip, destinationCity, destinationState, lineItems, pickupServiceGlobal,
            //   deliveryServiceGlobal, additionalServicesGlobal, UserName);

            //--

            int QuoteID;
            if (lReplyGlobal.Notification.Message.Contains("Success -"))
            {
                int.TryParse(lReplyGlobal.Notification.Message.Replace("Success - Quote ID: ", "").Trim(), out QuoteID);
                //HelperFuncs.writeToSiteErrors("updatePoDataTableAfterBooking GetGCM_API_Rates", QuoteID.ToString());
                // Add markup
                //gcmWebService.LTLResult[] lResults = null;

                //if (lReplyGlobal.LTLRates != null)
                //{
                //    lResults = lReplyGlobal.LTLRates;
                //}


            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("GetGCM_API_Rates_2", e.ToString());
        }
    }

    #region Not used

    #region GetLTLMarkup

    private static int GetLTLMarkup(ref string m_username, ref bool isSPCSubdomain)
    {
        try
        {

            int intLTLMarkup = 0;
            if (!m_username.Equals(string.Empty))
            {
                OdbcConnection conn = new OdbcConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aesmain_dataConnectionString"].ConnectionString);
                conn.Open();
                OdbcCommand comm = new OdbcCommand("select LTLMarkup, Subdomain from tbl_LOGIN where UserName='" + m_username + "'", conn);
                OdbcDataReader dr = comm.ExecuteReader();
                bool isAuth = dr.HasRows;
                if (isAuth)
                {
                    intLTLMarkup = Convert.ToInt16(dr["LTLMarkup"]);
                    HelperFuncs.writeToSiteErrors("dr[Subdomain]", dr["Subdomain"].ToString());

                    if (dr["Subdomain"].ToString().ToLower().Equals("spc"))
                    {
                        isSPCSubdomain = true;
                        HelperFuncs.writeToSiteErrors("isSPCSubdomain", "true");
                    }
                }
                dr.Close();
                conn.Close();
                //Response.Redirect("index.aspx");
            }
            return intLTLMarkup;
        }
        catch (Exception exp)
        {
            return 0;
        }
    }
    
    #endregion

    #region GetAddlLTLMarkup
    
    private static int GetAddlLTLMarkup(ref string m_username)
    {
        try
        {
            int addlLTLMarkup = 0;
            if (!m_username.Equals(string.Empty))
            {
                OdbcConnection conn = new OdbcConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aesmain_dataConnectionString"].ConnectionString);
                conn.Open();
                OdbcCommand comm = new OdbcCommand("SELECT AdditionalLTLMarkup FROM tbl_LOGIN_ACCTMANAGER WHERE UserName = '" + m_username + "'", conn);
                OdbcDataReader dr = comm.ExecuteReader();
                bool isAuth = dr.HasRows;
                if (isAuth)
                {
                    addlLTLMarkup = Convert.ToInt16(dr["AdditionalLTLMarkup"]);
                }
                dr.Close();
                conn.Close();
                //Response.Redirect("index.aspx");
            }
            return addlLTLMarkup;
        }
        catch (Exception exp)
        {
            string str = exp.ToString();
            return 0;
        }
    }
    
    #endregion

    #region addOnTheFlyLogicMarkup
    /// <summary>
    /// Not used
    /// </summary>
    public static void addOnTheFlyLogicMarkup()
    {
        // Get username by comp id
        //info.CompIDbyAcctNum
        string UserName = "";

        // If CompIDbyAcctNum is not empty then the account (username) was entered by vendor (on the fly) and need to apply markup according to this username
        if (!infoGlobal.CompIDbyAcctNum.Equals(-1))
        {
            UserName = HelperFuncs.getUsernameByCompID(infoGlobal.CompIDbyAcctNum);
        }

        if (!UserName.Equals(string.Empty))
        {
            int intLTLMarkup;
            int addlLTLMarkup;
            int addlLTLMinimum;
            double dblLTLMarkupRatio;
            double dblAddlLTLMarkupRatio;
            //double[] res;

            bool isSPCSubdomain = false;

            intLTLMarkup = GetLTLMarkup(ref UserName, ref isSPCSubdomain);
            HelperFuncs.writeToSiteErrors("intLTLMarkup", intLTLMarkup.ToString());

            addlLTLMarkup = GetAddlLTLMarkup(ref UserName);

            HelperFuncs.writeToSiteErrors("addlLTLMarkup", addlLTLMarkup.ToString());

            addlLTLMinimum = GetAddlLTLMinimum(ref UserName);

            HelperFuncs.writeToSiteErrors("addlLTLMinimum", addlLTLMinimum.ToString());

            dblLTLMarkupRatio = Convert.ToDouble(Convert.ToDouble(intLTLMarkup) / 100.00);
            dblAddlLTLMarkupRatio = Convert.ToDouble(Convert.ToDouble(addlLTLMarkup) / 100.00);
            double finalMarkup = 0;

            double rateAfterAddinLTLMarkup;

            for (byte i = 0; i < lReplyGlobal.LTLRates.Length; i++)
            {
                //LTLResult
                finalMarkup = lReplyGlobal.LTLRates[i].Rate * dblLTLMarkupRatio;

                HelperFuncs.writeToSiteErrors("lReplyGlobal.LTLRates[i].Rate:", lReplyGlobal.LTLRates[i].Rate.ToString());

                HelperFuncs.writeToSiteErrors("final Markup: ", finalMarkup.ToString() + " ratio: " + dblLTLMarkupRatio.ToString());

                if (finalMarkup > 0 && finalMarkup < 35) //  && isSPCSubdomain == false
                {
                    finalMarkup = 35;
                }

                rateAfterAddinLTLMarkup = lReplyGlobal.LTLRates[i].Rate;

                rateAfterAddinLTLMarkup = rateAfterAddinLTLMarkup + finalMarkup;

                HelperFuncs.writeToSiteErrors("rateAfterAddinLTLMarkup", rateAfterAddinLTLMarkup.ToString());

                //
                if (isSPCSubdomain.Equals(true))
                {
                    rateAfterAddinLTLMarkup = HelperFuncs.addSPC_Addition(rateAfterAddinLTLMarkup);
                }

                HelperFuncs.writeToSiteErrors("rateAfterAddinLTLMarkup after spc addition", rateAfterAddinLTLMarkup.ToString());

                // For account manager created logins
                if (addlLTLMarkup > 0 || addlLTLMinimum > 0)
                {

                    if (finalMarkup < addlLTLMinimum)
                    {
                        finalMarkup = addlLTLMinimum;
                    }

                    if (!(lReplyGlobal.LTLRates[i].CarrierKey == "R+L")) // No markup for R&L, Standard
                    {
                        rateAfterAddinLTLMarkup = rateAfterAddinLTLMarkup + finalMarkup;
                    }
                }

                lReplyGlobal.LTLRates[i].Rate = rateAfterAddinLTLMarkup;
            }
        }
    }

    #endregion

    #region GetAddlLTLMinimum

    private static int GetAddlLTLMinimum(ref string m_username)
    {
        try
        {

            int intLTLMinimum = 0;
            if (!m_username.Equals(string.Empty))
            {
                OdbcConnection conn = new OdbcConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aesmain_dataConnectionString"].ConnectionString);
                conn.Open();
                OdbcCommand comm = new OdbcCommand("SELECT AdditionalLTLMinimum FROM tbl_LOGIN_ACCTMANAGER WHERE UserName = '" + m_username + "'", conn);
                OdbcDataReader dr = comm.ExecuteReader();
                bool isAuth = dr.HasRows;
                if (isAuth)
                {
                    intLTLMinimum = Convert.ToInt16(dr["AdditionalLTLMinimum"]);
                }
                dr.Close();
                conn.Close();
                //Response.Redirect("index.aspx");
            }
            return intLTLMinimum;
        }
        catch (Exception exp)
        {
            return 0;
        }
    }
    
    #endregion

    #endregion

    #endregion

    #region getPackageRate_2

    //public static void getPackageRate_2()
    //{
    //    try
    //    {
    //        #region Authentication

    //        // infoGlobal.Ponumber
    //        // Get dynamic username according to POData.Ponumber

    //        string apiUserName = "";
    //        string apiKey = "";

    //        //Initialize web service/API object
    //        gcmWebService.RateService2 rs = new gcmWebService.RateService2();

    //        //Authenticate to the web service/API
    //        string sessionId = rs.Authenticate(apiUserName, apiKey);

    //        //Initialize SOAP header for authentication
    //        rs.AuthHeaderValue = new gcmWebService.AuthHeader();

    //        //Set session id to the SOAP header
    //        rs.AuthHeaderValue.SessionID = sessionId;
    //        #endregion

    //        #region Set pickup date
    //        //DateTime pickupDate = DateTime.Now.Date.AddDays(2);

    //        DateTime pickupDate = infoGlobal.shipmentReadyDate;
    //        if (pickupDate.DayOfWeek.Equals(DayOfWeek.Saturday))
    //        {
    //            pickupDate = pickupDate.AddDays(2);
    //        }
    //        else if (pickupDate.DayOfWeek.Equals(DayOfWeek.Sunday))
    //        {
    //            pickupDate = pickupDate.AddDays(1);
    //        }
    //        #endregion

    //        #region Origin, Destination

    //        string originZip = infoGlobal.origZip;    	//Origin ZIP code 
    //        string originCity = infoGlobal.origCity; 	//Origin city
    //        string originState = infoGlobal.origState;		//Origin State

    //        string destinationZip = infoGlobal.destZip;	//Destination ZIP code
    //        string destinationCity = infoGlobal.destCity; //Destination city
    //        string destinationState = infoGlobal.destState;	  //Destination state

    //        #endregion

    //        int totalPieces = 0;
    //        foreach (int piecesNum in infoGlobal.pieces)
    //        {
    //            totalPieces += piecesNum;
    //        }

    //        #region Set items

    //        gcmWebService.PackagePiece[] lineItems = new gcmWebService.PackagePiece[infoGlobal.piecesDims.Count];

    //        //int pieceIndex = 0;
    //        for (int i = 0; i < infoGlobal.piecesDims.Count; i++)
    //        {
    //            lineItems[i] = new gcmWebService.PackagePiece();
    //            //lineItems[0] = new gcmWebService.PackagePiece();
    //            lineItems[i].FreightClass = "50";
    //            lineItems[i].Weight = infoGlobal.weightDims[i];
    //            lineItems[i].Quantity = infoGlobal.piecesDims[i];
    //            lineItems[i].ItemUnit = "TYPE_PIECE";
    //            lineItems[i].HazMat = false;
    //            lineItems[i].Tag = (i + 1).ToString();
    //            lineItems[i].Commodity = "NEW";

    //            lineItems[i].Width = infoGlobal.width[i];
    //            lineItems[i].Length = infoGlobal.length[i];
    //            lineItems[i].Height = infoGlobal.height[i];

    //            //pieceIndex++;

    //            HelperFuncs.writeToSiteErrors("poSystemFuncs", string.Concat("index: ", i, " weight: ", lineItems[i].Weight, " quantity: ", lineItems[i].Quantity, " width: ",
    //                lineItems[i].Width, " length: ", lineItems[i].Length, " height: ", lineItems[i].Height));
    //        }

    //        HelperFuncs.writeToSiteErrors("poSystemFuncs", string.Concat("origZip: /r/n", originZip, " destZip: ", destinationZip, " origCity: ", originCity, " destCity: ",
    //            destinationCity, " origState: ", originState, " destState: ", destinationState, "totalWeight: ", infoGlobal.totalWeight, " ", infoGlobal.totalBoxes, " ", infoGlobal.destState, " addr1: ",
    //            infoGlobal.destAddr1, " addr2: ", infoGlobal.destAddr2, " addr3: ", infoGlobal.destAddr3, " compName: ", infoGlobal.destCompName, " contact: ", infoGlobal.destContact,
    //            " phone: ", infoGlobal.destPhone, " "));

    //        #endregion

    //        #region Accessorials

    //        #region Set pickup and delivery service

    //        string pickupService = ""; //Set pickup service
    //        string deliveryService = ""; //Set delivery service
    //        #endregion

    //        #region Set additional services
    //        // Set additional services

    //        string[] additionalServices = new string[0];

    //        //Getting the rates for booking
    //        #endregion

    //        #endregion

    //        #region Call the package web service
    //        // Get rates from package web service
    //        //gcmWebService.PackageRateReply lReply = rs.GetPackageBookingRate(pickupDate, originZip, originCity, originState, destinationZip, destinationCity, destinationState, lineItems, pickupService,
    //        //    deliveryService, additionalServices);

    //        packageReplyGlobal = rs.GetPackageBookingRate(pickupDate, originZip, originCity, originState, destinationZip, destinationCity, destinationState, lineItems, pickupService,
    //            deliveryService, additionalServices, infoGlobal.Ponumber);

    //        //gcmWebService.PackageResult[] lResults = lReply.PackageRates;

    //        //lResults = lReply.PackageRates;

    //        //gcmWebService.PackageBookingInfo BookingInfo = lReply.BookingInfo;
    //        #endregion
    //    }
    //    catch (Exception e)
    //    {
    //        HelperFuncs.writeToSiteErrors("getPackageRate_2", e.ToString());
    //    }
    //}

    #endregion

    #endregion

    #endregion

    #region makeBookingOnAPI
    
    //public static void makeBookingOnAPI(ref gcmAPI.gcmWebService.RateService2 rs, ref gcmAPI.gcmWebService.LTLResult[] lResults, ref byte indexToBookWith,
    //    ref gcmAPI.gcmWebService.LTLBookingReply lbr, ref Info info)
    //{
    //    //Set a booking key to book a rate
    //    string bookingKey = lResults[0].BookingKey;

    //    string customerType = "SP"; //Set customer type
    //    gcmAPI.gcmWebService.BookingThirdPartyBilling thirdPartyBilling = null;
    //    DateTime shipmentDate = DateTime.Now.Date; //Set shipment date
    //    string readyTime = "02:15 PM"; //Set ready time of the shipment
    //    string closeTime = "06:15 PM"; //Set close time of the shipment

    //    //string readyTime = "10"; //Set ready time of the shipment
    //    //string closeTime = "3"; //Set close time of the shipment

    //    string bolSendTo = "EML"; //Set BOL sending option
    //    //string poNumber = "45455"; //Set PO Number
    //    string poNumber = info.Ponumber; //Set PO Number

    //    #region Set pickup and delivery locations

    //    //Set pickup location
    //    gcmAPI.gcmWebService.BookingPickupLocation pickupLocation = new gcmAPI.gcmWebService.BookingPickupLocation();
    //    pickupLocation.Name = info.origContact;
    //    pickupLocation.Email = ""; // Add this later
    //    pickupLocation.Company = info.origCompName;
    //    pickupLocation.Phone = info.origPhone;
    //    pickupLocation.Fax = "";
    //    pickupLocation.Address1 = info.origAddr1;
    //    pickupLocation.Address2 = info.origAddr2;
    //    pickupLocation.City = info.origCity;
    //    pickupLocation.State = info.origState;
    //    pickupLocation.Zip = info.origZip;

    //    //Set destination location
    //    gcmWebService.BookingDestinationLocation destinationLocation = new gcmWebService.BookingDestinationLocation();
    //    destinationLocation.Name = info.destContact;
    //    destinationLocation.Email = ""; // Add this later
    //    destinationLocation.Company = info.destCompName;
    //    destinationLocation.Phone = info.destPhone;
    //    destinationLocation.Fax = "";
    //    destinationLocation.Address1 = info.destAddr1;
    //    destinationLocation.Address2 = info.destAddr2;
    //    destinationLocation.City = info.destCity;
    //    destinationLocation.State = info.destState;
    //    destinationLocation.Zip = info.destZip;

    //    #endregion

    //    #region Set line items

    //    //Set line items
    //    gcmWebService.LTLBookingPiece[] bookingLineItems = new gcmWebService.LTLBookingPiece[info.weight.Count];

    //    for (byte i = 0; i < bookingLineItems.Length; i++)
    //    {
    //        //Set first line item for booking
    //        bookingLineItems[i] = new gcmWebService.LTLBookingPiece();

    //        // This tag is related to the object(LTLPiece) in lineItems array which is used in GetLTLBookingRate function call
    //        bookingLineItems[i].Tag = (i + 1).ToString();

    //        if (info.nmfc.Count > i)
    //        {
    //            bookingLineItems[i].NMFC = info.nmfc[i];
    //        }
    //        else
    //        {
    //            bookingLineItems[i].NMFC = 0;
    //        }

    //        if (info.palletCount.Count > i)
    //        {
    //            bookingLineItems[i].NumberOfPallet = info.palletCount[i];
    //        }
    //        else
    //        {
    //            bookingLineItems[i].NumberOfPallet = 0;
    //        }
    //        bookingLineItems[i].Description = info.commodity[i];
    //    }

    //    #endregion

    //    string comments = "";

    //    bool insuranceRequired = false;
    //    double declaredValue = 100;

    //    HelperFuncs.writeToSiteErrors("CreateLTLBookingPoSystem", "start of function");

    //    //Book an LTL rate and getting the BOL and insurance certificate PDF url 
    //    lbr = rs.CreateLTLBookingPoSystem(bookingKey, customerType, thirdPartyBilling, shipmentDate, readyTime, closeTime, bolSendTo, //gcmWebService.LTLBookingReply 
    //        poNumber, pickupLocation, destinationLocation, bookingLineItems, comments, insuranceRequired, declaredValue, info.spGCMCompID, info.ccGCMCompID,
    //        info.notesFromVendorToCustomer, info.freightBidAmount, info.QuoteID, info.UPSParcelQuoteID, info.poDataID, info.routedByVendor, info.carrierToBidWith, info.totalPieces,
    //        info.totalCube, info.shipByDate, info.shipAfterDate, info.savedAmount, info.BookedOnShipType, info.Routing, info.TrackingNumber, info.CarrierShippingWith,
    //        info.combinedPOs, info.combinedPO_IDsList.ToArray(), info.combinedPOsList.ToArray(), info.CompIDbyAcctNum, info.CarrierFromUser);

    //}
    
    #endregion

    //--

    #region GetDestCompInfoByCompID
  
    private static void GetDestCompInfoByCompID(ref int compID, ref Info info)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {
                #region SQL

                string sql = string.Concat("SELECT City, State, Zip, Phone, Addr1, Addr2, Addr3, CompName, Contact ",

                    "FROM tbl_COMPANY ",

                    "WHERE CompID=", compID);

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
                                info.destCity = reader["City"].ToString();
                            }

                            if (reader["State"] != DBNull.Value)
                            {
                                info.destState = reader["State"].ToString();
                            }

                            if (reader["Zip"] != DBNull.Value)
                            {
                                info.destZip = reader["Zip"].ToString();
                            }

                            if (reader["CompName"] != DBNull.Value)
                            {
                                info.destCompName = reader["CompName"].ToString();
                            }

                            if (reader["Contact"] != DBNull.Value)
                            {
                                info.destContact = reader["Contact"].ToString();
                            }

                            if (reader["Phone"] != DBNull.Value)
                            {
                                info.destPhone = reader["Phone"].ToString();
                            }

                            if (reader["Addr1"] != DBNull.Value)
                            {
                                info.destAddr1 = reader["Addr1"].ToString();
                            }

                            if (reader["Addr2"] != DBNull.Value)
                            {
                                info.destAddr2 = reader["Addr2"].ToString();
                            }

                            if (reader["Addr3"] != DBNull.Value)
                            {
                                info.destAddr3 = reader["Addr3"].ToString();
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

    //--

    #region Make booking UPS

    #region makeBooking
   
    //public static void makeBooking(ref gcmAPI.gcmWebService.RateService2 rs, ref gcmAPI.gcmWebService.PackageResult[] lResults,
    //    ref gcmAPI.gcmWebService.PackageBookingInfo BookingInfo, ref gcmAPI.gcmWebService.PackageBookingReply lbr, ref int upsGroundIndex, ref Info info)
    //{
    //    // MAKE BOOKING
    //    string bookingKey = "";
    //    // Set a booking key to book a rate
    //    bookingKey = lResults[upsGroundIndex].BookingKey;

    //    string customerType = "SP"; //Set customer type
    //    gcmAPI.gcmWebService.BookingThirdPartyBilling thirdPartyBilling = null;
    //    //DateTime shipmentDate = DateTime.Now.Date; //Set shipment date
    //    string readyTime = "12:00 PM"; //Set ready time of the shipment
    //    string closeTime = "05:00 PM"; //Set close time of the shipment
    //    string bolSendTo = "EML"; //Set BOL sending option
    //    string poNumber = info.Ponumber; //Set PO Number

    //    #region Set Pickup and Destination locations
    //    // Set pickup location
    //    gcmWebService.BookingPickupLocation pickupLocation = new gcmWebService.BookingPickupLocation();
    //    pickupLocation.Name = info.origContact;
    //    pickupLocation.Email = "";
    //    pickupLocation.Company = info.origCompName;
    //    pickupLocation.Phone = info.origPhone;
    //    pickupLocation.Fax = "";
    //    pickupLocation.Address1 = info.origAddr1;
    //    pickupLocation.Address2 = info.origAddr2;
    //    pickupLocation.City = info.origCity;
    //    pickupLocation.State = info.origState;
    //    pickupLocation.Zip = info.origZip;

    //    // Set destination location
    //    gcmWebService.BookingDestinationLocation destinationLocation = new gcmWebService.BookingDestinationLocation();
    //    destinationLocation.Name = info.destContact;
    //    destinationLocation.Email = "";
    //    destinationLocation.Company = info.destCompName;
    //    destinationLocation.Phone = info.destPhone;
    //    destinationLocation.Fax = "";
    //    destinationLocation.Address1 = info.destAddr1;
    //    destinationLocation.Address2 = info.destAddr2;
    //    destinationLocation.City = info.destCity;
    //    destinationLocation.State = info.destState;
    //    destinationLocation.Zip = info.destZip;
    //    #endregion

    //    // Set other line items
    //    gcmWebService.PackageBookingPiece[] bookingLineItems = new gcmWebService.PackageBookingPiece[info.piecesDims.Count];

    //    for (int i = 0; i < info.piecesDims.Count; i++)
    //    {
    //        // Set first line item for booking
    //        bookingLineItems[i] = new gcmWebService.PackageBookingPiece();

    //        // This tag is related to the object(LTLPiece) in lineItems array which is used in GetLTLBookingRate function call
    //        bookingLineItems[i].Tag = (i + 1).ToString();

    //        //bookingLineItems[0].NMFC = 50;
    //        //bookingLineItems[0].NumberOfPallet = 1;

    //        // Make sure there is not index out of range exception
    //        try
    //        {
    //            bookingLineItems[i].Description = info.commodity[i];
    //        }
    //        catch (Exception e)
    //        {
    //            HelperFuncs.writeToSiteErrors("PoSystem", e.ToString());
    //            bookingLineItems[i].Description = "";
    //        }


    //    }

    //    ////Set second line item for booking
    //    //bookingLineItems[1] = new gcmWebService.LTLBookingPiece();

    //    ////// This tag is related to the object(LTLPiece) in lineItems array which is used in GetLTLBookingRate function call
    //    //bookingLineItems[1].Tag = "2";

    //    //bookingLineItems[1].NMFC = 60;
    //    //bookingLineItems[1].NumberOfPallet = 10;
    //    //bookingLineItems[1].Description = "TEST";

    //    string comments = "";
    //    bool insuranceRequired = true;
    //    double declaredValue = 100;

    //    // Book an LTL rate and getting the BOL and insurance certificate PDF url 
    //    lbr = rs.CreatePackageBooking(bookingKey, BookingInfo, customerType, thirdPartyBilling, info.shipmentReadyDate, readyTime, closeTime, bolSendTo,
    //        poNumber, pickupLocation, destinationLocation, bookingLineItems, comments, insuranceRequired, declaredValue, info.spGCMCompID, info.ccGCMCompID,
    //        info.notesFromVendorToCustomer, info.freightBidAmount, info.QuoteID, info.UPSParcelQuoteID, info.poDataID, info.routedByVendor, info.carrierToBidWith, info.totalPieces,
    //        info.totalCube, info.shipByDate, info.shipAfterDate, info.savedAmount, info.BookedOnShipType, info.Routing, info.TrackingNumber, info.CarrierShippingWith,
    //        info.combinedPOs, info.combinedPO_IDsList.ToArray(), info.combinedPOsList.ToArray(), info.CompIDbyAcctNum, info.CarrierFromUser);


    //    //else
    //    //{
    //    //    if (!lReply.Notification.Code.Equals("0"))
    //    //    {
    //    //        //Print error message
    //    //        Response.Write(lReply.Notification.Message);
    //    //    }
    //    //}
    //}
    
    #endregion

    #endregion

    #region Not used

    #region GetGCM_API_Rates

    //public static void GetGCM_API_Rates(ref PoSystem.Info info, ref gcmAPI.gcmWebService.RateService2 rs,
    //    ref DateTime pickupDate, ref string pickupService, ref string deliveryService, ref string[] additionalServices, ref gcmAPI.gcmWebService.LTLRateReply lReply)
    //{


    //    #region Set origin and destination locations
    //    string originZip = info.origZip;    	// Origin ZIP code 
    //    string originCity = info.origCity; 	// Origin city
    //    string originState = info.origState;		// Origin State

    //    string destinationZip = info.destZip;	// Destination ZIP code
    //    string destinationCity = info.destCity; // Destination city
    //    string destinationState = info.destState;	  //Destination state
    //    #endregion

    //    #region Line Items

    //    gcmWebService.LTLPiece[] lineItems = new gcmWebService.LTLPiece[info.weight.Count];

    //    for (byte i = 0; i < info.weight.Count; i++)
    //    {
    //        lineItems[i] = new gcmWebService.LTLPiece();

    //        lineItems[i].FreightClass = info.fClass[i].ToString();

    //        lineItems[i].Weight = Convert.ToInt32(info.weight[i]);

    //        lineItems[i].Quantity = info.palletCount[i];

    //        lineItems[i].ItemUnit = "PALLET_XXX";

    //        //lineItems[i].HazMat = info.hazMat; // Hazmat is not implemented yet

    //        lineItems[i].HazMat = false; // Not implemented

    //        lineItems[i].Tag = "1";

    //        lineItems[i].Commodity = "NEW";

    //        //lineItems[i].Length = Convert.ToDouble(info.length[i]);

    //        //lineItems[i].Width = Convert.ToDouble(info.width[i]);

    //        //lineItems[i].Height = Convert.ToDouble(info.height[i]);
    //    }
    //    #endregion

    //    //gcmWebService.LTLRateReply lReply = rs.GetLTLBookingRate(pickupDate, originZip, originCity, originState, destinationZip, destinationCity, destinationState, lineItems, pickupService,
    //    //    deliveryService, additionalServices);

    //    lReply = rs.GetLTLBookingRate(pickupDate, originZip, originCity, originState, destinationZip, destinationCity, destinationState, lineItems, pickupService,
    //       deliveryService, additionalServices);

    //    //--

    //    int QuoteID;
    //    if (lReply.Notification.Message.Contains("Success -"))
    //    {
    //        int.TryParse(lReply.Notification.Message.Replace("Success - Quote ID: ", "").Trim(), out QuoteID);
    //        HelperFuncs.writeToSiteErrors("updatePoDataTableAfterBooking GetGCM_API_Rates", QuoteID.ToString());
    //    }

    //    //--


    //}
    
    #endregion

    #region getRate
   
    //public static void getPackageRate(ref Info info, ref gcmAPI.gcmWebService.PackageBookingReply lbr, ref gcmAPI.gcmWebService.PackageRateReply lReply)
    //{
    //    #region Authentication
    //    string apiUserName = "";
    //    string apiKey = "";

    //    //Initialize web service/API object
    //    gcmAPI.gcmWebService.RateService2 rs = new gcmAPI.gcmWebService.RateService2();

    //    //Authenticate to the web service/API
    //    string sessionId = rs.Authenticate(apiUserName, apiKey);

    //    //Initialize SOAP header for authentication
    //    rs.AuthHeaderValue = new gcmAPI.gcmWebService.AuthHeader();

    //    //Set session id to the SOAP header
    //    rs.AuthHeaderValue.SessionID = sessionId;
    //    #endregion

    //    #region Set pickup date
    //    //DateTime pickupDate = DateTime.Now.Date.AddDays(2);

    //    DateTime pickupDate = info.shipmentReadyDate;
    //    if (pickupDate.DayOfWeek.Equals(DayOfWeek.Saturday))
    //    {
    //        pickupDate = pickupDate.AddDays(2);
    //    }
    //    else if (pickupDate.DayOfWeek.Equals(DayOfWeek.Sunday))
    //    {
    //        pickupDate = pickupDate.AddDays(1);
    //    }
    //    #endregion

    //    #region Origin, Destination
    //    //string originZip = "30303";    	//Origin ZIP code 
    //    //string originCity = "ATLANTA"; 	//Origin city
    //    //string originState = "GA";		//Origin State

    //    //string destinationZip = "90040";	//Destination ZIP code
    //    //string destinationCity = "LOS ANGELES"; //Destination city
    //    //string destinationState = "CA";	  //Destination state

    //    string originZip = info.origZip;    	//Origin ZIP code 
    //    string originCity = info.origCity; 	//Origin city
    //    string originState = info.origState;		//Origin State

    //    string destinationZip = info.destZip;	//Destination ZIP code
    //    string destinationCity = info.destCity; //Destination city
    //    string destinationState = info.destState;	  //Destination state
    //    #endregion

    //    int totalPieces = 0;
    //    foreach (int piecesNum in info.pieces)
    //    {
    //        totalPieces += piecesNum;
    //    }

    //    #region Set items
    //    gcmWebService.PackagePiece[] lineItems = new gcmWebService.PackagePiece[info.piecesDims.Count];

    //    //int pieceIndex = 0;
    //    for (int i = 0; i < info.piecesDims.Count; i++)
    //    {
    //        lineItems[i] = new gcmWebService.PackagePiece();
    //        //lineItems[0] = new gcmWebService.PackagePiece();
    //        lineItems[i].FreightClass = "50";
    //        lineItems[i].Weight = info.weightDims[i];
    //        lineItems[i].Quantity = info.piecesDims[i];
    //        lineItems[i].ItemUnit = "TYPE_PIECE";
    //        lineItems[i].HazMat = false;
    //        lineItems[i].Tag = (i + 1).ToString();
    //        lineItems[i].Commodity = "NEW";

    //        lineItems[i].Width = info.width[i];
    //        lineItems[i].Length = info.length[i];
    //        lineItems[i].Height = info.height[i];

    //        //pieceIndex++;

    //        HelperFuncs.writeToSiteErrors("poSystemFuncs", string.Concat("index: ", i, " weight: ", lineItems[i].Weight, " quantity: ", lineItems[i].Quantity, " width: ",
    //            lineItems[i].Width, " length: ", lineItems[i].Length, " height: ", lineItems[i].Height));
    //    }


    //    HelperFuncs.writeToSiteErrors("poSystemFuncs", string.Concat("origZip: /r/n", originZip, " destZip: ", destinationZip, " origCity: ", originCity, " destCity: ",
    //        destinationCity, " origState: ", originState, " destState: ", destinationState, "totalWeight: ", info.totalWeight, " ", info.totalBoxes, " ", info.destState, " addr1: ",
    //        info.destAddr1, " addr2: ", info.destAddr2, " addr3: ", info.destAddr3, " compName: ", info.destCompName, " contact: ", info.destContact,
    //        " phone: ", info.destPhone, " "));
    //    #endregion

    //    #region Accessorials

    //    #region Set pickup and delivery service
    //    //string pickupService = "CSP"; //Set pickup service
    //    //string deliveryService = "CSD"; //Set delivery service
    //    //string pickupService = "RSP"; //Set pickup service
    //    //string deliveryService = "RSD"; //Set delivery service

    //    string pickupService = ""; //Set pickup service
    //    string deliveryService = ""; //Set delivery service
    //    #endregion

    //    #region Set additional services
    //    // Set additional services

    //    string[] additionalServices = new string[0];
    //    //additionalServices[0] = "";

    //    //string[] additionalServices = new string[0];
    //    //additionalServices[0] = "ISP";
    //    //additionalServices[0] = "TGD";
    //    //additionalServices[0] = "TGP";
    //    //additionalServices[1] = "";

    //    //additionalServices[0] = "ISD";
    //    //additionalServices[0] = "TGD";
    //    //additionalServices[0] = "TGP";
    //    //additionalServices[0] = "ISD";
    //    //additionalServices[0] = "TGD";
    //    //additionalServices[0] = "TGP";
    //    //Getting the rates for booking
    //    #endregion

    //    #endregion

    //    #region Call the package web service
    //    // Get rates from package web service
    //    //gcmWebService.PackageRateReply lReply = rs.GetPackageBookingRate(pickupDate, originZip, originCity, originState, destinationZip, destinationCity, destinationState, lineItems, pickupService,
    //    //    deliveryService, additionalServices);

    //    lReply = rs.GetPackageBookingRate(pickupDate, originZip, originCity, originState, destinationZip, destinationCity, destinationState, lineItems, pickupService,
    //        deliveryService, additionalServices, "");

    //    //gcmWebService.PackageResult[] lResults = lReply.PackageRates;

    //    //lResults = lReply.PackageRates;

    //    gcmWebService.PackageBookingInfo BookingInfo = lReply.BookingInfo;
    //    #endregion


    //}
   
    #endregion

    #endregion

    //--

    #region downloadPackageLabels

    //public static void downloadPackageLabels(ref string file, ref string poNumber, ref string carrier, ref string consComp, ref string consAddr1, ref string consAddr2,
    //            ref string consCity, ref string consState, ref string consZip, ref int totalPieces, ref int totalPallets)
    //{
    //    int numOfLabels = 0;
    //    if (totalPallets > 0)
    //    {
    //        numOfLabels = totalPallets;
    //    }
    //    else if (totalPieces > 0)
    //    {
    //        numOfLabels = totalPieces;
    //    }

    //    byte[] byteArray = File.ReadAllBytes(@"testLabels1.docx");
        //using (MemoryStream stream = new MemoryStream())
        //{
        //    stream.Write(byteArray, 0, (int)byteArray.Length);
        //    using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, true))
        //    {
        //        // Do work here
        //        // Add a main document part. 
        //        //MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();

        //        MainDocumentPart mainPart = wordDoc.MainDocumentPart;

        //        // Create the document structure and add some text.
        //        mainPart.Document = new Document();
        //        Body body = mainPart.Document.AppendChild(new Body());
        //        Paragraph para = body.AppendChild(new Paragraph());

        //        Run run = para.AppendChild(new Run());



        //        for (int i = 1; i <= numOfLabels; i++)
        //        {
        //            // Add new text
        //            //if (i > 1)
        //            //{
        //            //    Paragraph para10 = body.AppendChild(new Paragraph());
        //            //    run = para10.AppendChild(new Run());
        //            //    //run.AppendChild(new Text(string.Concat("Test next page")));
        //            //}

        //            Paragraph para10 = body.AppendChild(new Paragraph());
        //            run = para10.AppendChild(new Run());
        //            run.AppendChild(new Text(string.Concat("PO Number: ", poNumber)));

        //            //--

        //            // Add new text
        //            Paragraph para2 = body.AppendChild(new Paragraph());
        //            run = para2.AppendChild(new Run());
        //            run.AppendChild(new Text("Deliver to:"));

        //            // Add new text
        //            Paragraph para3 = body.AppendChild(new Paragraph());
        //            run = para3.AppendChild(new Run());
        //            run.AppendChild(new Text(consComp));

        //            // Add new text
        //            Paragraph para4 = body.AppendChild(new Paragraph());
        //            run = para4.AppendChild(new Run());
        //            run.AppendChild(new Text(consAddr1));

        //            // Add new text
        //            Paragraph para5 = body.AppendChild(new Paragraph());
        //            run = para5.AppendChild(new Run());
        //            run.AppendChild(new Text(consAddr2));

        //            // Add new text
        //            Paragraph para6 = body.AppendChild(new Paragraph());
        //            run = para6.AppendChild(new Run());
        //            run.AppendChild(new Text(string.Concat(consCity, ", ", consState, " ", consZip)));

        //            //--

        //            // Add new text
        //            Paragraph para7 = body.AppendChild(new Paragraph());
        //            run = para7.AppendChild(new Run());
        //            run.AppendChild(new Text(string.Concat("Total shipping units: ", totalPallets))); // Add logic to check if units or pieces

        //            // Add new text X of Y
        //            Paragraph para8 = body.AppendChild(new Paragraph());
        //            run = para8.AppendChild(new Run());
        //            run.AppendChild(new Text(string.Concat("Piece ", i, " of ", numOfLabels))); // Add logic to check if units or pieces

        //            // Add new text
        //            Paragraph para9 = body.AppendChild(new Paragraph());

        //            //--

        //            // Start on a new page
        //            ParagraphProperties paragraphProperties220 = new ParagraphProperties();

        //            SectionProperties sectionProperties1 = new SectionProperties();
        //            SectionType sectionType1 = new SectionType() { Val = SectionMarkValues.NextPage };

        //            sectionProperties1.Append(sectionType1);

        //            paragraphProperties220.Append(sectionProperties1);

        //            para9.Append(paragraphProperties220);
        //            //--

        //            run = para9.AppendChild(new Run());
        //            run.AppendChild(new Text(string.Concat("Carrier shipping on: ", carrier)));

        //            //

        //            ApplyStyleToParagraph(wordDoc, "style1", "style 1", para);
        //            ApplyStyleToParagraph(wordDoc, "style1", "style 1", para2);
        //            ApplyStyleToParagraph(wordDoc, "style1", "style 1", para3);
        //            ApplyStyleToParagraph(wordDoc, "style1", "style 1", para4);

        //            ApplyStyleToParagraph(wordDoc, "style1", "style 1", para5);
        //            ApplyStyleToParagraph(wordDoc, "style1", "style 1", para6);
        //            ApplyStyleToParagraph(wordDoc, "style1", "style 1", para7);
        //            ApplyStyleToParagraph(wordDoc, "style1", "style 1", para8);
        //            ApplyStyleToParagraph(wordDoc, "style1", "style 1", para9);

        //            ApplyStyleToParagraph(wordDoc, "style1", "style 1", para10);
        //        }


        //    }

        //    //string file = @"testLabels2.docx";
        //    // Save the file with the new name
        //    File.WriteAllBytes(file, stream.ToArray());

    //    }
    //}

    #endregion

    #region Apply Styles to Paragraph

    #region ApplyStyleToParagraph

    // Apply a style to a paragraph.
    //public static void ApplyStyleToParagraph(WordprocessingDocument doc,
    //    string styleid, string stylename, Paragraph p)
    //{
    //    // If the paragraph has no ParagraphProperties object, create one.
    //    if (p.Elements<ParagraphProperties>().Count() == 0)
    //    {
    //        p.PrependChild<ParagraphProperties>(new ParagraphProperties());
    //    }

    //    // Get the paragraph properties element of the paragraph.
    //    ParagraphProperties pPr = p.Elements<ParagraphProperties>().First();

    //    // Get the Styles part for this document.
    //    StyleDefinitionsPart part =
    //        doc.MainDocumentPart.StyleDefinitionsPart;

    //    // If the Styles part does not exist, add it and then add the style.
    //    if (part == null)
    //    {
    //        part = AddStylesPartToPackage(doc);
    //        AddNewStyle(part, styleid, stylename);
    //    }
    //    else
    //    {
    //        // If the style is not in the document, add it.
    //        if (IsStyleIdInDocument(doc, styleid) != true)
    //        {
    //            // No match on styleid, so let's try style name.
    //            string styleidFromName = GetStyleIdFromStyleName(doc, stylename);
    //            if (styleidFromName == null)
    //            {
    //                AddNewStyle(part, styleid, stylename);
    //            }
    //            else
    //                styleid = styleidFromName;
    //        }
    //    }

    //    // Set the style of the paragraph.
    //    pPr.ParagraphStyleId = new ParagraphStyleId() { Val = styleid };
    //}

    #endregion

    #region IsStyleIdInDocument

    // Return true if the style id is in the document, false otherwise.
    //public static bool IsStyleIdInDocument(WordprocessingDocument doc,

    //    string styleid)
    //{
    //    // Get access to the Styles element for this document.
    //    DocumentFormat.OpenXml.Wordprocessing.Styles s = doc.MainDocumentPart.StyleDefinitionsPart.Styles;

    //    // Check that there are styles and how many.
    //    int n = s.Elements<DocumentFormat.OpenXml.Wordprocessing.Style>().Count();
    //    if (n == 0)
    //        return false;

    //    // Look for a match on styleid.
    //    DocumentFormat.OpenXml.Wordprocessing.Style style = s.Elements<DocumentFormat.OpenXml.Wordprocessing.Style>()
    //        .Where(st => (st.StyleId == styleid) && (st.Type == StyleValues.Paragraph))
    //        .FirstOrDefault();
    //    if (style == null)
    //        return false;

    //    return true;
    //}
    
    #endregion

    #region GetStyleIdFromStyleName

    // Return styleid that matches the styleName, or null when there's no match.
    //public static string GetStyleIdFromStyleName(WordprocessingDocument doc, string styleName)
    //{
    //    StyleDefinitionsPart stylePart = doc.MainDocumentPart.StyleDefinitionsPart;
    //    string styleId = stylePart.Styles.Descendants<StyleName>()
    //        .Where(s => s.Val.Value.Equals(styleName) &&
    //            (((DocumentFormat.OpenXml.Wordprocessing.Style)s.Parent).Type == StyleValues.Paragraph))
    //        .Select(n => ((DocumentFormat.OpenXml.Wordprocessing.Style)n.Parent).StyleId).FirstOrDefault();
    //    return styleId;
    //}

    #endregion

    #region AddNewStyle

    // Create a new style with the specified styleid and stylename and add it to the specified
    // style definitions part.
    //private static void AddNewStyle(StyleDefinitionsPart styleDefinitionsPart,
    //    string styleid, string stylename)
    //{
    //    // Get access to the root element of the styles part.
    //    DocumentFormat.OpenXml.Wordprocessing.Styles styles = styleDefinitionsPart.Styles;

    //    // Create a new paragraph style and specify some of the properties.
    //    DocumentFormat.OpenXml.Wordprocessing.Style style = new DocumentFormat.OpenXml.Wordprocessing.Style()
    //    {
    //        Type = StyleValues.Paragraph,
    //        StyleId = styleid,
    //        CustomStyle = true
    //    };
    //    StyleName styleName1 = new StyleName() { Val = stylename };
    //    BasedOn basedOn1 = new BasedOn() { Val = "Normal" };
    //    NextParagraphStyle nextParagraphStyle1 = new NextParagraphStyle() { Val = "Normal" };
    //    style.Append(styleName1);
    //    style.Append(basedOn1);
    //    style.Append(nextParagraphStyle1);

    //    // Create the StyleRunProperties object and specify some of the run properties.
    //    StyleRunProperties styleRunProperties1 = new StyleRunProperties();
    //    Bold bold1 = new Bold();
    //    //DocumentFormat.OpenXml.Wordprocessing.Color color1 = new DocumentFormat.OpenXml.Wordprocessing.Color() { ThemeColor = ThemeColorValues.Accent2 };

    //    RunFonts font1 = new RunFonts() { Ascii = "Lucida Console" };
    //    //Italic italic1 = new Italic();
    //    // Specify a 12 point size.
    //    DocumentFormat.OpenXml.Wordprocessing.FontSize fontSize1 = new DocumentFormat.OpenXml.Wordprocessing.FontSize() { Val = "40" };
    //    styleRunProperties1.Append(bold1);
    //    //styleRunProperties1.Append(color1);
    //    styleRunProperties1.Append(font1);
    //    styleRunProperties1.Append(fontSize1);

    //    //styleRunProperties1.Append(italic1);

    //    // Add the run properties to the style.
    //    style.Append(styleRunProperties1);

    //    // Add the style to the styles part.
    //    styles.Append(style);
    //}

    #endregion

    #region StyleDefinitionsPart

    // Add a StylesDefinitionsPart to the document.  Returns a reference to it.
    //public static StyleDefinitionsPart AddStylesPartToPackage(WordprocessingDocument doc)
    //{
    //    StyleDefinitionsPart part;
    //    part = doc.MainDocumentPart.AddNewPart<StyleDefinitionsPart>();
    //    DocumentFormat.OpenXml.Wordprocessing.Styles root = new DocumentFormat.OpenXml.Wordprocessing.Styles();
    //    root.Save(part);
    //    return part;
    //}

    #endregion

    #endregion

    #region GetParcelThreshold()
    //public static int GetParcelThreshold(string username, ref int parcelThreshold)
    //{
    //    int compID = 0;
    //    List<int> compIDs = new List<int>();

    //    //HelperFuncs.writeToSiteErrors("aesComp", strAESCompID);

    //    try
    //    {
    //        string sql = string.Concat("SELECT AESCompID, ParcelThreshold ",
    //                "FROM tbl_LOGIN ",
    //                "LEFT JOIN tbl_COMPANY AS comp ON comp.CompID = AESCompID ",
    //                "WHERE UserName='", username, "' ORDER BY AESCompID");

    //        //using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
    //        //{
    //        //    using (SqlCommand command = new SqlCommand())
    //        //    {
    //        //        command.CommandText = sql;
    //        //        conn.Open();
    //        //        command.Connection = conn;
    //        //        using (SqlDataReader reader = command.ExecuteReader())
    //        //        {
    //        //            //reader = command.ExecuteReader();

    //        //            while (reader.Read())
    //        //            {
    //        //                if (reader["AESCompID"] != null && reader["AESCompID"] != DBNull.Value)
    //        //                {
    //        //                    if (reader["ParcelThreshold"] != null && reader["ParcelThreshold"] != DBNull.Value)
    //        //                    {
    //        //                        parcelThreshold = (int)reader["ParcelThreshold"];
    //        //                    }
    //        //                    else
    //        //                    {
    //        //                        parcelThreshold = 0;
    //        //                    }
    //        //                    compID = (int)reader["AESCompID"]; //getting the last (highest number id of a group of id's) seems to work..
    //        //                    compIDs.Add((int)reader["AESCompID"]);
    //        //                    //break;
    //        //                }
    //        //            }
    //        //        }
    //        //    }
    //        //}

    //        //HelperFuncs.writeToSiteErrors("compid", compID.ToString());
    //        return compID;
    //    }
    //    catch (Exception e)
    //    {

    //        HelperFuncs.writeToSiteErrors("getCompID2", e.ToString());
    //        return compID;
    //    }

    //}
    #endregion

    #region GetAES_CompIdByLocationId()
    //public static void GetAES_CompIdByLocationId(int locationId, ref int compID)
    //{
    //    try
    //    {
    //        using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
    //        {
    //            #region SQL

    //            string sql = string.Concat("SELECT CustCompID ",

    //                "FROM Locations ",

    //                "WHERE ID=", locationId);

    //            #endregion

    //            using (SqlCommand command = new SqlCommand())
    //            {
    //                command.Connection = conn;
    //                command.CommandText = sql;
    //                conn.Open();
    //                using (SqlDataReader reader = command.ExecuteReader())
    //                {
    //                    if (reader.Read())
    //                    {
    //                        if (reader["CustCompID"] != DBNull.Value)
    //                        {
    //                            compID = (int)reader["CustCompID"];
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        HelperFuncs.writeToSiteErrors("GetAES_CompIdByLocationId", e.ToString());
    //    }
    //}
    #endregion

    // Not used
    #region GetTotalWeightFromItemsTable()
    //public static void GetTotalWeightFromItemsTable(int shipID, ref int totalWeight)
    //{
    //    try
    //    {
    //        using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
    //        {
    //            #region SQL

    //            string sql = string.Concat("SELECT SUM(WtLBS) ",

    //                "FROM tbl_ITEMS ",

    //                "WHERE SHIPMENTID=", shipID);

    //            #endregion

    //            using (SqlCommand command = new SqlCommand())
    //            {
    //                command.Connection = conn;
    //                command.CommandText = sql;
    //                conn.Open();
    //                totalWeight = Convert.ToInt32((double)command.ExecuteScalar());                   
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        HelperFuncs.writeToSiteErrors("GetAES_CompIdByLocationId", e.ToString());
    //    }
    //}
    #endregion

    #region IsCompVendor

    //public static void IsCompVendor(ref int compID, ref bool isVendor)
    //{
    //    try
    //    {
    //        using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
    //        {
    //            #region SQL

    //            string sql = string.Concat("SELECT IsVendor ",

    //                "FROM Locations ",

    //                "WHERE CustCompID=", compID);

    //            #endregion

    //            using (SqlCommand command = new SqlCommand())
    //            {
    //                command.Connection = conn;
    //                command.CommandText = sql;
    //                conn.Open();
    //                using (SqlDataReader reader = command.ExecuteReader())
    //                {
    //                    if (reader.Read())
    //                    {
    //                        if (reader["IsVendor"] != DBNull.Value)
    //                        {
    //                            isVendor = (bool)reader["IsVendor"];
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        HelperFuncs.writeToSiteErrors("IsCompVendor", e.ToString());
    //    }
    //}

    #endregion

    #region IsCompClearviewPO
   
//public static void IsCompClearviewPO(ref int compID, ref bool isCompClearviewPO)
//    {
//        try
//        {
//            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
//            {
//                #region SQL

//                string sql = string.Concat("SELECT ClearviewPO ",

//                    "FROM tbl_LOGIN ",

//                    "WHERE AESCompID=", compID);

//                #endregion

//                using (SqlCommand command = new SqlCommand())
//                {
//                    command.Connection = conn;
//                    command.CommandText = sql;
//                    conn.Open();
//                    using (SqlDataReader reader = command.ExecuteReader())
//                    {
//                        if (reader.Read())
//                        {
//                            if (reader["ClearviewPO"] != DBNull.Value)
//                            {
//                                isCompClearviewPO = (bool)reader["ClearviewPO"];
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        catch (Exception e)
//        {
//            HelperFuncs.writeToSiteErrors("IsCompVendor", e.ToString());
//        }
//    }

    #endregion

    #region GetVendorInfoByCompID

    //public static void GetVendorInfoByCompID(ref int compID, ref List<PoSystem.Vendor> vendors)
    //{

    //    try
    //    {
    //        using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
    //        {
    //            #region SQL

    //            string sql = string.Concat("SELECT CompName, Addr1, Addr2, City, State, Zip, Contact, Phone, EMail, locations.ID as VendorID ",

    //             "FROM CompaniesVendors ",

    //             "LEFT JOIN Locations AS locations ON locations.ID=CompaniesVendors.VendorID ",

    //             "LEFT JOIN [AESData].[dbo].[tbl_COMPANY] AS compTable ON compTable.CompID=locations.CustCompID ",

    //             "WHERE CompaniesVendors.AESCompID=", compID, //, " ",

    //             "AND Locations.IsVendor='true'");

    //            #endregion

    //            //HelperFuncs.writeToSiteErrors("GetVendorInfoByCompID", sql);

    //            using (SqlCommand command = new SqlCommand())
    //            {
    //                command.Connection = conn;
    //                command.CommandText = sql;
    //                conn.Open();
    //                using (SqlDataReader reader = command.ExecuteReader())
    //                {
    //                    while (reader.Read())
    //                    {
    //                        PoSystem.Vendor vendor = new PoSystem.Vendor();

    //                        if (reader["CompName"] != DBNull.Value)
    //                        {
    //                            vendor.CompName = reader["CompName"].ToString();
    //                        }
    //                        if (reader["Addr1"] != DBNull.Value)
    //                        {
    //                            vendor.Addr1 = reader["Addr1"].ToString();
    //                        }
    //                        if (reader["Addr2"] != DBNull.Value)
    //                        {
    //                            vendor.Addr2 = reader["Addr2"].ToString();
    //                        }
    //                        if (reader["City"] != DBNull.Value)
    //                        {
    //                            vendor.City = reader["City"].ToString();
    //                        }
    //                        if (reader["State"] != DBNull.Value)
    //                        {
    //                            vendor.State = reader["State"].ToString();
    //                        }
    //                        if (reader["Zip"] != DBNull.Value)
    //                        {
    //                            vendor.Zip = reader["Zip"].ToString();
    //                        }
    //                        if (reader["Contact"] != DBNull.Value)
    //                        {
    //                            vendor.Contact = reader["Contact"].ToString();
    //                        }
    //                        if (reader["Phone"] != DBNull.Value)
    //                        {
    //                            vendor.Phone = reader["Phone"].ToString();
    //                        }
    //                        if (reader["EMail"] != DBNull.Value)
    //                        {
    //                            vendor.Email = reader["EMail"].ToString();
    //                        }

    //                        //--

    //                        if (reader["VendorID"] != DBNull.Value)
    //                        {
    //                            vendor.VendorID = (int)reader["VendorID"];
    //                        }

    //                        vendors.Add(vendor);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        HelperFuncs.writeToSiteErrors("GetVendorInfoByCompID", e.ToString());
    //    }
    //}

    #endregion

    #region GetRecurringVendorInfoByCompID

    //public static void GetRecurringVendorInfoByCompID(ref int compID, ref List<PoSystem.Vendor> vendors, ref string CompName, ref Int16 timesRecurring)
    //{

    //    try
    //    {
    //        using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
    //        {
    //            #region SQL

    //            string sql = string.Concat("SELECT CompName, Addr1, Addr2, City, State, Zip, Contact, Phone, EMail, locations.ID as VendorID ",

    //             "FROM CompaniesVendors ",

    //             "LEFT JOIN Locations AS locations ON locations.ID=CompaniesVendors.VendorID ",

    //             "LEFT JOIN [AESData].[dbo].[tbl_COMPANY] AS compTable ON compTable.CompID=locations.CustCompID ",

    //             "WHERE CompaniesVendors.AESCompID=", compID, //, " ",

    //             "AND Locations.IsVendor='true' AND CompName='", CompName, "'");

    //            #endregion

    //            //HelperFuncs.writeToSiteErrors("GetVendorInfoByCompID", sql);

    //            using (SqlCommand command = new SqlCommand())
    //            {
    //                command.Connection = conn;
    //                command.CommandText = sql;
    //                conn.Open();
    //                using (SqlDataReader reader = command.ExecuteReader())
    //                {
    //                    while (reader.Read())
    //                    {
    //                        PoSystem.Vendor vendor = new PoSystem.Vendor();

    //                        if (reader["CompName"] != DBNull.Value)
    //                        {
    //                            vendor.CompName = reader["CompName"].ToString();
    //                        }
    //                        if (reader["Addr1"] != DBNull.Value)
    //                        {
    //                            vendor.Addr1 = reader["Addr1"].ToString();
    //                        }
    //                        if (reader["Addr2"] != DBNull.Value)
    //                        {
    //                            vendor.Addr2 = reader["Addr2"].ToString();
    //                        }
    //                        if (reader["City"] != DBNull.Value)
    //                        {
    //                            vendor.City = reader["City"].ToString();
    //                        }
    //                        if (reader["State"] != DBNull.Value)
    //                        {
    //                            vendor.State = reader["State"].ToString();
    //                        }
    //                        if (reader["Zip"] != DBNull.Value)
    //                        {
    //                            vendor.Zip = reader["Zip"].ToString();
    //                        }
    //                        if (reader["Contact"] != DBNull.Value)
    //                        {
    //                            vendor.Contact = reader["Contact"].ToString();
    //                        }
    //                        if (reader["Phone"] != DBNull.Value)
    //                        {
    //                            vendor.Phone = reader["Phone"].ToString();
    //                        }
    //                        if (reader["EMail"] != DBNull.Value)
    //                        {
    //                            vendor.Email = reader["EMail"].ToString();
    //                        }

    //                        //--

    //                        if (reader["VendorID"] != DBNull.Value)
    //                        {
    //                            vendor.VendorID = (int)reader["VendorID"];
    //                        }

    //                        for (Int16 i = 0; i < timesRecurring; i++)
    //                        {
    //                            vendors.Add(vendor);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        HelperFuncs.writeToSiteErrors("GetVendorInfoByCompID", e.ToString());
    //    }
    //}

    #endregion

    #region For downloading Excel snapshot of each table on web page

    #region InsertIntoDownloadQueriesTable

    //public static void InsertIntoDownloadQueriesTable(ref string Query, ref int insertedID) // ref string Guid, 
    //{
    //    string sql = "";
    //    try
    //    {
    //        //Replace("%", "percentCharacter")
    //        using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
    //        {
    //            #region SQL

    //            sql = string.Concat("INSERT INTO DownloadQueries(Query) OUTPUT INSERTED.ID VALUES('", //,Guid 
    //                Query.Replace("'", "''"), "')"); //,'", Guid, "'

    //            #endregion

    //            using (SqlCommand command = new SqlCommand())
    //            {
    //                command.Connection = conn;
    //                command.CommandText = sql;
    //                conn.Open();
    //                insertedID = (int)command.ExecuteScalar();
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        HelperFuncs.writeToSiteErrors("InsertIntoDownloadQueriesTable", string.Concat("sql was: ", sql, " exception was: ", e.ToString()));
    //    }
    //}

    #endregion

    #region GetDownloadQueryByID
   
//public static void GetDownloadQueryByID(ref string Query, int insertedID)
//    {
//        try
//        {
//            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
//            {
//                #region SQL

//                string sql = string.Concat("SELECT Query ",

//                    "FROM DownloadQueries ",

//                    "WHERE ID=", insertedID);

//                #endregion
//                //.Replace("percentCharacter", "%")
//                using (SqlCommand command = new SqlCommand())
//                {
//                    command.Connection = conn;
//                    command.CommandText = sql;
//                    conn.Open();
//                    using (SqlDataReader reader = command.ExecuteReader())
//                    {
//                        if (reader.Read())
//                        {
//                            if (reader["Query"] != DBNull.Value)
//                            {
//                                Query = reader["Query"].ToString();
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        catch (Exception e)
//        {
//            HelperFuncs.writeToSiteErrors("GetDownloadQueryByID", e.ToString());
//        }
//    }

    #endregion

    #endregion

    #region GetPoInfoByPoId

    //public static void GetPoInfoByPoId(ref int poID, ref string poNum)
    //{
    //    try
    //    {
    //        using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
    //        {
    //            #region SQL

    //            string sql = string.Concat("SELECT Ponumber ",

    //                                         "FROM Podata ",

    //                                         //"LEFT JOIN Locations AS locations ON locations.ID=CompaniesVendors.VendorID ",

    //                                         //"LEFT JOIN [AESData].[dbo].[tbl_COMPANY] AS compTable ON compTable.CompID=locations.CustCompID ",

    //                                         "WHERE id=", poID);

    //            //HelperFuncs.writeToSiteErrors("sql", sql);
    //            #endregion

    //            using (SqlCommand command = new SqlCommand())
    //            {
    //                command.Connection = conn;
    //                command.CommandText = sql;
    //                conn.Open();
    //                using (SqlDataReader reader = command.ExecuteReader())
    //                {
    //                    if (reader.Read())
    //                    {

    //                        if (reader["Ponumber"] != DBNull.Value)
    //                        {
    //                            poNum = reader["Ponumber"].ToString();
    //                        }

    //                    }
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        HelperFuncs.writeToSiteErrors("GetPoInfoByPoId", e.ToString());
    //    }
    //}

    #endregion

    #region GetPoInfoByPoId

    //public static void GetPoInfoByPoId_2(ref int poID, ref string poNum, ref string receivingCompName, ref string vendorCompName, ref string status,
    //    ref string enteredBy, ref string ETA, ref string routing)
    //{
    //    try
    //    {
    //        using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
    //        {

    //            #region SQL
    //            string sql = string.Concat("SELECT Podata.id AS PodataId, Podata.Compid AS PodataCompId, Podata.ShipmentID, Ponumber, ",

    //                "Routing, EnteredBy, vendorCustComp.CompName as vendorCompName, receivingCustComp.CompName as receivingCompName, ",
    //                "shipments.ShipStatus AS shipStatus, segments.ATA AS ETA ",
    //                "FROM Podata ",
    //                "LEFT JOIN Locations AS vendorLocation ON vendorLocation.id = Podata.VendorID ",
    //                "LEFT JOIN Locations AS receivingLocation ON receivingLocation.id = Podata.ReceivingID ",
    //                "LEFT JOIN [AESData].[dbo].[tbl_COMPANY] AS vendorCustComp ON vendorCustComp.CompID = vendorLocation.CustCompID ",
    //                "LEFT JOIN [AESData].[dbo].[tbl_COMPANY] AS receivingCustComp ON receivingCustComp.CompID = receivingLocation.CustCompID ",
    //                "LEFT JOIN PO_Categories AS categories ON categories.ID = Podata.CategoryID ",

    //                "LEFT JOIN [AESData].[dbo].[tbl_SHIPMENTS] AS shipments ON shipments.ShipmentID = Podata.ShipmentID ",
    //                "LEFT JOIN [AESData].[dbo].[tbl_SEGMENTS] AS segments ON segments.ShipmentID = Podata.ShipmentID ",

    //                "WHERE Podata.id=", poID
    //              );
    //            #endregion


    //            using (SqlCommand command = new SqlCommand())
    //            {
    //                command.Connection = conn;
    //                command.CommandText = sql;
    //                conn.Open();
    //                using (SqlDataReader reader = command.ExecuteReader())
    //                {
    //                    if (reader.Read())
    //                    {

    //                        if (reader["Ponumber"] != DBNull.Value)
    //                        {
    //                            poNum = reader["Ponumber"].ToString();
    //                        }
    //                        if (reader["receivingCompName"] != DBNull.Value)
    //                        {
    //                            receivingCompName = reader["receivingCompName"].ToString();
    //                        }
    //                        if (reader["vendorCompName"] != DBNull.Value)
    //                        {
    //                            vendorCompName = reader["vendorCompName"].ToString();
    //                        }
    //                        if (reader["shipStatus"] != DBNull.Value)
    //                        {
    //                            status = reader["shipStatus"].ToString();
    //                        }
    //                        if (reader["EnteredBy"] != DBNull.Value)
    //                        {
    //                            enteredBy = reader["EnteredBy"].ToString();
    //                        }
    //                        if (reader["ETA"] != DBNull.Value)
    //                        {
    //                            ETA = ((DateTime)reader["ETA"]).ToShortDateString();
    //                        }
    //                        if (reader["Routing"] != DBNull.Value)
    //                        {
    //                            routing = reader["Routing"].ToString();
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        HelperFuncs.writeToSiteErrors("GetPoInfoByPoId_2", e.ToString());
    //    }
    //}

    #endregion

    #region GetEmailsByPoId
    //public static void GetEmailsByPoId(ref int poID, ref StringBuilder emails)
    //{
    //    try
    //    {

    //        using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
    //        {
    //            #region SQL

    //            string sql = string.Concat("SELECT DISTINCT Email ",

    //                                         "FROM MsgSysEmails ",

    //                                         "WHERE PO_ID=", poID);

    //            //HelperFuncs.writeToSiteErrors("sql", sql);
    //            #endregion

    //            using (SqlCommand command = new SqlCommand())
    //            {
    //                command.Connection = conn;
    //                command.CommandText = sql;
    //                conn.Open();
    //                using (SqlDataReader reader = command.ExecuteReader())
    //                {
    //                    while (reader.Read())
    //                    {
    //                        if (reader["Email"] != DBNull.Value)
    //                        {
    //                            emails.Append(string.Concat(reader["Email"].ToString(), " , "));
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        HelperFuncs.writeToSiteErrors("GetEmailsByPoId", e.ToString());
    //    }
    //}
    #endregion

    #region InsertNewMessage

    public static void InsertNewMessage(ref int poID, string Message, string Name)
    {
        try
        {

            #region InsertNewMessage

            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
            {

                string sql = string.Concat("INSERT INTO Messages(PO_ID,Msg,Name,MessageDate,MessageTime)",
                                                   " VALUES(", poID, ",'",
                                                                Message.Replace("'", "''"), "','",
                                                                Name.Replace("'", "''"), "','",
                                                                DateTime.Today.ToShortDateString(), "', '", DateTime.Now.ToShortTimeString(), "')");


                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    command.ExecuteNonQuery();
                }
            }

            #endregion
        }
        catch (Exception cE)
        {

            //HelperFuncs.writeToSiteErrors("dataTable", "sql was: " + sql + " exception was: " + cE.ToString());
            HelperFuncs.writeToSiteErrors("InsertNewMessage", cE.ToString());

        }
    }

    #endregion

    #region insertRowIntoPO_Data

    public static void insertRowIntoPO_Data(ref Podata poData, ref int compID, ref int newPoId)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
            {
                string sql = "";

                if (poData.FreeFreight.Equals(true))
                {
                    sql = string.Concat("INSERT INTO Podata(Compid, Ponumber, DateEntered, ShipAfterDate, ShipbyDate, EnteredBy, POvalue, VendorID,",
                        "ReceivingID, Pieces, Category, VendorNotes, ShipperNotes, FreeFreight, Routing) ",

                    "OUTPUT INSERTED.id ",

                    "VALUES(", compID, ",'", poData.Ponumber, "', '", poData.DateEntered, "', '", poData.ShipAfterDate, "', '", poData.ShipbyDate, "', '",
                    poData.EnteredBy, "', ", poData.POvalue, ",", poData.VendorID, ",", poData.ReceivingID, ",", poData.Pieces, ",'",
                    poData.Category, "','", poData.VendorNotes, "','", poData.ShipperNotes, "','", poData.FreeFreight, "','DFA')");
                }
                else
                {
                    sql = string.Concat("INSERT INTO Podata(Compid, Ponumber, DateEntered, ShipAfterDate, ShipbyDate, EnteredBy, POvalue, VendorID,",
                        "ReceivingID, Pieces, Category, VendorNotes, ShipperNotes, FreeFreight) ",

                    "OUTPUT INSERTED.id ",

                    "VALUES(", compID, ",'", poData.Ponumber, "', '", poData.DateEntered, "', '", poData.ShipAfterDate, "', '", poData.ShipbyDate, "', '",
                    poData.EnteredBy, "', ", poData.POvalue, ",", poData.VendorID, ",", poData.ReceivingID, ",", poData.Pieces, ",'",
                    poData.Category, "','", poData.VendorNotes, "','", poData.ShipperNotes, "','", poData.FreeFreight, "')");
                }

                //HelperFuncs.writeToSiteErrors("sql", sql);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    //command.ExecuteNonQuery();

                    newPoId = (int)command.ExecuteScalar();
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("insertRowIntoPO_Data", e.ToString());
        }
    }

    public static void insertRowIntoPO_Data(ref Podata poData, ref int compID, ref int newPoId, ref string DispatchedBy)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
            {
                string sql = "";

                if (poData.FreeFreight.Equals(true))
                {
                    sql = string.Concat("INSERT INTO Podata(Compid, Ponumber, DateEntered, ShipAfterDate, ShipbyDate, EnteredBy, POvalue, VendorID,",
                        "ReceivingID, Pieces, Category, VendorNotes, ShipperNotes, FreeFreight, Routing, DispatchedBy) ",

                    "OUTPUT INSERTED.id ",

                    "VALUES(", compID, ",'", poData.Ponumber, "', '", poData.DateEntered, "', '", poData.ShipAfterDate, "', '", poData.ShipbyDate, "', '",
                    poData.EnteredBy, "', ", poData.POvalue, ",", poData.VendorID, ",", poData.ReceivingID, ",", poData.Pieces, ",'",
                    poData.Category, "','", poData.VendorNotes, "','", poData.ShipperNotes, "','", poData.FreeFreight, "','DFA','", DispatchedBy, "')");
                }
                else
                {
                    sql = string.Concat("INSERT INTO Podata(Compid, Ponumber, DateEntered, ShipAfterDate, ShipbyDate, EnteredBy, POvalue, VendorID,",
                        "ReceivingID, Pieces, Category, VendorNotes, ShipperNotes, FreeFreight, DispatchedBy) ",

                    "OUTPUT INSERTED.id ",

                    "VALUES(", compID, ",'", poData.Ponumber, "', '", poData.DateEntered, "', '", poData.ShipAfterDate, "', '", poData.ShipbyDate, "', '",
                    poData.EnteredBy, "', ", poData.POvalue, ",", poData.VendorID, ",", poData.ReceivingID, ",", poData.Pieces, ",'",
                    poData.Category, "','", poData.VendorNotes, "','", poData.ShipperNotes, "','", poData.FreeFreight, "','", DispatchedBy, "')");
                }

                //HelperFuncs.writeToSiteErrors("sql", sql);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    //command.ExecuteNonQuery();

                    newPoId = (int)command.ExecuteScalar();
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("insertRowIntoPO_Data", e.ToString());
        }
    }

    #endregion

    #region GetTotalsForCombinedPOs
    public static void GetTotalsForCombinedPOs(ref List<string> combinedPO_IDsList, ref decimal combinedPOsValue, ref int combinedPOsPieces)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
            {
                #region SQL

                string sql = string.Concat("SELECT SUM(POvalue) AS combinedPOsValue, SUM(Pieces) AS combinedPOsPieces ",

                "FROM Podata ",
                "WHERE id IN(", string.Join(",", combinedPO_IDsList.ToArray()), ")");

                HelperFuncs.writeToSiteErrors("GetTotalsForCombinedPOs", sql);

                #endregion

                //byte counter = 0;

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader["combinedPOsValue"] != DBNull.Value)
                            {
                                combinedPOsValue = (decimal)reader["combinedPOsValue"];
                            }

                            if (reader["combinedPOsPieces"] != DBNull.Value)
                            {
                                combinedPOsPieces = (int)reader["combinedPOsPieces"];
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("GetTotalsForCombinedPOs", e.ToString());
        }
    }
    #endregion

    #region GetCombinedSubPOsByMasterPO
    public static void GetCombinedSubPOsByMasterPO(int poID, ref StringBuilder combinedSubPOs)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
            {

                #region SQL

                string sql = string.Concat("SELECT id ",

                    "FROM Podata ",

                    "WHERE Podata.CombinedPoID=", poID, " AND IsMasterPO != 'true'");

                #endregion

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["id"] != DBNull.Value)
                            {
                                combinedSubPOs.Append(string.Concat(reader["id"].ToString(), ","));
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("GetCombinedSubPOsByMasterPO", e.ToString());
        }
    }
    #endregion

    #region insertRowIntoCompanyTable

    public static void insertRowIntoCompanyTable(ref Vendor vendor, ref int newCompID)
    {
        try
        {
            #region Fix input

            if (vendor.City.Trim().Equals(string.Empty))
            {
                vendor.City = null;
            }
            else
            {
                vendor.City = vendor.City.Replace("'", "''");
            }

            if (vendor.State.Trim().Equals(string.Empty))
            {
                vendor.State = null;
            }
            else
            {
                vendor.State = vendor.State.Replace("'", "''");
            }

            if (vendor.Zip.Trim().Equals(string.Empty))
            {
                vendor.Zip = null;
            }
            else
            {
                vendor.Zip = vendor.Zip.Replace("'", "''");
            }

            if (vendor.Contact.Trim().Equals(string.Empty))
            {
                vendor.Contact = null;
            }
            else
            {
                vendor.Contact = vendor.Contact.Replace("'", "''");
            }

            if (vendor.Phone.Trim().Equals(string.Empty))
            {
                vendor.Phone = null;
            }
            else
            {
                vendor.Phone = vendor.Phone.Replace("'", "''");
            }

            if (vendor.Email.Trim().Equals(string.Empty))
            {
                vendor.Email = null;
            }
            else
            {
                vendor.Email = vendor.Email.Replace("'", "''");
            }

            //

            if (vendor.CompName.Trim().Equals(string.Empty))
            {
                vendor.CompName = null;
            }
            else
            {
                vendor.CompName = vendor.CompName.Replace("'", "''");
            }

            if (vendor.Addr1.Trim().Equals(string.Empty))
            {
                vendor.Addr1 = null;
            }
            else
            {
                vendor.Addr1 = vendor.Addr1.Replace("'", "''");
            }

            if (vendor.Addr2.Trim().Equals(string.Empty))
            {
                vendor.Addr2 = null;
            }
            else
            {
                vendor.Addr2 = vendor.Addr2.Replace("'", "''");
            }

            #endregion

            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {

                string sql = string.Concat("INSERT INTO tbl_COMPANY(CompName, Addr1, Addr2, City, State, Zip, Contact, Phone, Email) ",

                "OUTPUT INSERTED.CompID ",

                "VALUES('", vendor.CompName, "','", vendor.Addr1, "', '", vendor.Addr2, "', '", vendor.City, "', '",
                vendor.State, "', '", vendor.Zip, "','", vendor.Contact, "','", vendor.Phone, "','", vendor.Email, "')");

                HelperFuncs.writeToSiteErrors("sql", sql);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();

                    newCompID = (int)command.ExecuteScalar();

                    //HelperFuncs.writeToSiteErrors("newCompID", newCompID.ToString());
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("insertRowIntoCompanyTable", e.ToString());
        }
    }

    #endregion

    #region insertReceiverRowIntoCompanyTable

    public static void insertReceiverRowIntoCompanyTable(ref Receiver receiver, ref int newCompID)
    {
        try
        {
            #region Fix input

            if (receiver.City.Trim().Equals(string.Empty))
            {
                receiver.City = null;
            }
            else
            {
                receiver.City = receiver.City.Replace("'", "''");
            }

            if (receiver.State.Trim().Equals(string.Empty))
            {
                receiver.State = null;
            }
            else
            {
                receiver.State = receiver.State.Replace("'", "''");
            }

            if (receiver.Zip.Trim().Equals(string.Empty))
            {
                receiver.Zip = null;
            }
            else
            {
                receiver.Zip = receiver.Zip.Replace("'", "''");
            }

            if (receiver.Contact.Trim().Equals(string.Empty))
            {
                receiver.Contact = null;
            }
            else
            {
                receiver.Contact = receiver.Contact.Replace("'", "''");
            }

            if (receiver.Phone.Trim().Equals(string.Empty))
            {
                receiver.Phone = null;
            }
            else
            {
                receiver.Phone = receiver.Phone.Replace("'", "''");
            }

            if (receiver.Email.Trim().Equals(string.Empty))
            {
                receiver.Email = null;
            }
            else
            {
                receiver.Email = receiver.Email.Replace("'", "''");
            }

            //

            if (receiver.CompName.Trim().Equals(string.Empty))
            {
                receiver.CompName = null;
            }
            else
            {
                receiver.CompName = receiver.CompName.Replace("'", "''");
            }

            if (receiver.Addr1.Trim().Equals(string.Empty))
            {
                receiver.Addr1 = null;
            }
            else
            {
                receiver.Addr1 = receiver.Addr1.Replace("'", "''");
            }

            if (receiver.Addr2.Trim().Equals(string.Empty))
            {
                receiver.Addr2 = null;
            }
            else
            {
                receiver.Addr2 = receiver.Addr2.Replace("'", "''");
            }

            #endregion

            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
            {

                string sql = string.Concat("INSERT INTO tbl_COMPANY(CompName, Addr1, Addr2, City, State, Zip, Contact, Phone, Email) ",

                "OUTPUT INSERTED.CompID ",

                "VALUES('", receiver.CompName, "','", receiver.Addr1, "', '", receiver.Addr2, "', '", receiver.City, "', '",
                receiver.State, "', '", receiver.Zip, "','", receiver.Contact, "','", receiver.Phone, "','", receiver.Email, "')");

                HelperFuncs.writeToSiteErrors("sql", sql);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();

                    newCompID = (int)command.ExecuteScalar();

                    //HelperFuncs.writeToSiteErrors("newCompID", newCompID.ToString());
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("insertReceiverRowIntoCompanyTable", e.ToString());
        }
    }

    #endregion

    #region insertRowIntoLocationsTable

    public static void insertRowIntoLocationsTable(ref int newCompID, ref int newLocationID)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
            {

                string sql = string.Concat("INSERT INTO Locations(CustCompID, IsVendor) ",

                "OUTPUT INSERTED.ID ",

                "VALUES(", newCompID, ",'true')");

                //HelperFuncs.writeToSiteErrors("sql", sql);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();

                    newLocationID = (int)command.ExecuteScalar();

                    //HelperFuncs.writeToSiteErrors("newLocationID", newLocationID.ToString());
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("insertRowIntoLocationsTable", e.ToString());
        }
    }

    #endregion

    #region insertReceiverRowIntoLocationsTable

    public static void insertReceiverRowIntoLocationsTable(ref int newCompID, ref int newLocationID)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
            {

                string sql = string.Concat("INSERT INTO Locations(CustCompID, IsVendor) ",

                "OUTPUT INSERTED.ID ",

                "VALUES(", newCompID, ",'false')");

                //HelperFuncs.writeToSiteErrors("sql", sql);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();

                    newLocationID = (int)command.ExecuteScalar();

                    //HelperFuncs.writeToSiteErrors("newLocationID", newLocationID.ToString());
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("insertReceiverRowIntoLocationsTable", e.ToString());
        }
    }

    #endregion

    #region insertRowIntoCompaniesVendorsTable

    public static void insertRowIntoCompaniesVendorsTable(ref int newCompID, ref int newLocationID)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
            {

                string sql = string.Concat("INSERT INTO CompaniesVendors(AESCompID, VendorID) ",

                //"OUTPUT INSERTED.ID ",

                "VALUES(", newCompID, ",", newLocationID, ")");

                //HelperFuncs.writeToSiteErrors("sql", sql);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();

                    command.ExecuteNonQuery();
                    //newLocationID = (int)command.ExecuteScalar();

                    //HelperFuncs.writeToSiteErrors("newLocationID", newLocationID.ToString());
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("insertRowIntoLocationsTable", e.ToString());
        }
    }

    #endregion

    #region insertRowIntoCompaniesReceivingLocationsTable

    public static void insertRowIntoCompaniesReceivingLocationsTable(ref int newCompID, ref int newLocationID)
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringPoRouting))
            {

                string sql = string.Concat("INSERT INTO CompaniesReceivingLocations(AESCompID, ReceivingLocationID) ",

                //"OUTPUT INSERTED.ID ",

                "VALUES(", newCompID, ",", newLocationID, ")");

                //HelperFuncs.writeToSiteErrors("sql", sql);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = sql;
                    conn.Open();

                    command.ExecuteNonQuery();
                    //newLocationID = (int)command.ExecuteScalar();

                    //HelperFuncs.writeToSiteErrors("newLocationID", newLocationID.ToString());
                }
            }
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("insertRowIntoCompaniesReceivingLocationsTable", e.ToString());
        }
    }

    #endregion

    #region Structs

    //public struct Vendor
    //{
    //    public string CompName, Addr1, Addr2, City, State, Zip, Contact, Phone, Email;
    //    public int VendorID;
    //}

    public struct Vendor
    {
        public string CompName, Addr1, Addr2, City, State, Zip, Contact, Phone, Email;
        public int VendorID;
    }

    public struct Receiver
    {
        public string CompName, Addr1, Addr2, City, State, Zip, Contact, Phone, Email;
        public int ReceivingLocationID;
    }


    #endregion
}

