#region Using

using System;
using System.Collections.Generic;
using gcmAPI.Models.LTL;
using System.Text;
using System.Net;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using gcmAPI.Models.Utilities;
//using System.Threading;

#endregion

namespace gcmAPI.Models.Carriers
{
    public class UPS_Package
    {
        //public struct upsPackageResults

        public struct upsPackageResRow
        {
            public string service;
            public double cost;
            public int days;
            public DateTime latestPickupDateTime, scheduleBy, deliveredBy;
        }

        QuoteData quoteData;

        #region Constructors

        // Constructor
        public UPS_Package(ref QuoteData quoteData)
        {

            this.quoteData = quoteData;
        }

        public UPS_Package()
        {

        }

        #endregion

        #region GetRateFromUPS

        //public upsPackageResRow GetRateFromUPS_Package()
        //{
        //    upsPackageResRow gcmRateQuote = new upsPackageResRow();

        //    //GetResultObjectFromRAndLScrape(ref gcmRateQuote);

        //    return gcmRateQuote;
        //}

        #endregion

        #region getUPS_PackageInfoViaAPI_XML

        public void getUPS_PackageInfoViaAPI_XML(
                ref string username, ref string password, ref List<string> length, ref List<string> width,
                ref List<string> height, ref List<string> weight, ref List<string> packageType, ref string serviceType,
                ref upsPackageResRow resUPSGround,
            ref upsPackageResRow resUPSNextDayAir,

            ref upsPackageResRow resSecondDayAir,
            ref upsPackageResRow res3DaySelect,
            ref upsPackageResRow resNextDayAirSaver,
            ref upsPackageResRow resNextDayAirEarlyAM,
            ref upsPackageResRow res2ndDayAirAM,ref QuoteData quoteData)
        {
            //HelperFuncs.writeToSiteErrors("getUPS_PackageInfoViaAPI_XML UPS", "start of func");

            #region Items

            StringBuilder sbPackages = new StringBuilder();

            for (int i = 0; i < weight.Count; i++)
            {
                sbPackages.Append(string.Concat("<Package>",
                 "<PackagingType>",
                     "<Code>02</Code>",
                     "<Description>Customer Supplied</Description>",
                 "</PackagingType>",
                 "<Description>Rate</Description>",
                 "<PackageWeight>",
                     "<UnitOfMeasurement>",
                       "<Code>LBS</Code>",
                     "</UnitOfMeasurement>",
                     "<Weight>", weight[i], "</Weight>",
                 "</PackageWeight>"));

                if (!string.IsNullOrEmpty(length[i]))
                {
                    sbPackages.Append(string.Concat("<Dimensions>",
                        "<UnitOfMeasurement>",
                          "<Code>IN</Code>",
                        "</UnitOfMeasurement>",
                        "<Length>", length[i], "</Length>",
                        "<Width>", width[i], "</Width>",
                        "<Height>", height[i], "</Height>",
                    "</Dimensions>"));
                }

                sbPackages.Append("</Package>");
            }

            #endregion

            #region Post data

            Logins.Login_info login_info;
            Logins logins = new Logins();
            logins.Get_login_info(85, out login_info);

            //HelperFuncs.writeToSiteErrors("origCityUPS", quoteData.origCity);

            string data = string.Concat("data=<?xml version=\"1.0\"?>",
    "<AccessRequest xml:lang=\"en-US\">",
        "<AccessLicenseNumber>", login_info.API_Key, "</AccessLicenseNumber>",
        "<UserId>", login_info.username, "</UserId>",
        "<Password>", login_info.password, "</Password>",
    "</AccessRequest>",
    "<?xml version=\"1.0\"?>",
    "<RatingServiceSelectionRequest xml:lang=\"en-US\">",
      "<Request>",
        "<TransactionReference>",
          "<CustomerContext>Rating and Service</CustomerContext>",
          "<XpciVersion>1.0</XpciVersion>",
        "</TransactionReference>",
        "<RequestAction>Rate</RequestAction>",
        "<RequestOption>Rate</RequestOption>",

      "</Request>",
      //"<PickupType>",
      //"<Code>07</Code>",
      //"<Description>Rate</Description>",
      //"</PickupType>",
      "<Shipment>",
            "<Description>Rate Description</Description>",
        "<Shipper>",
          "<Name>Name</Name>",
          "<PhoneNumber>1234567890</PhoneNumber>",
          "<ShipperNumber>", login_info.account, "</ShipperNumber>",
          "<Address>",
            "<AddressLine1>Address Line</AddressLine1>",
            "<City>", quoteData.origCity, "</City>",
            "<StateProvinceCode>", quoteData.origState, "</StateProvinceCode>",
            "<PostalCode>", quoteData.origZip, "</PostalCode>",
            "<CountryCode>US</CountryCode>",
          "</Address>",
        "</Shipper>",
        "<ShipTo>",
          "<CompanyName>Company Name</CompanyName>",
          "<PhoneNumber>1234567890</PhoneNumber>",
          "<Address>",
            "<AddressLine1>Address Line</AddressLine1>",
            "<City>", quoteData.destCity, "</City>",
            "<StateProvinceCode>", quoteData.destState, "</StateProvinceCode>",
            "<PostalCode>", quoteData.destZip, "</PostalCode>",
            "<CountryCode>US</CountryCode>",
          "</Address>",
        "</ShipTo>",

        "<ShipFrom>",
          "<CompanyName>Company Name</CompanyName>",
          "<AttentionName>Attention Name</AttentionName>",
          "<PhoneNumber>1234567890</PhoneNumber>",
          "<FaxNumber>1234567890</FaxNumber>",
          "<Address>",
            "<AddressLine1>Address Line</AddressLine1>",
            "<City>", quoteData.origCity, "</City>",
            "<StateProvinceCode>", quoteData.origState, "</StateProvinceCode>",
            "<PostalCode>", quoteData.origZip, "</PostalCode>",
            "<CountryCode>US</CountryCode>",
          "</Address>",
        "</ShipFrom>",
        "<Service>",
                //"<Code>03</Code>",
                "<Code>", serviceType, "</Code>",
        "</Service>",
        "<PaymentInformation>",
                "<Prepaid>",
                    "<BillShipper>",
                        "<AccountNumber>", login_info.account, "</AccountNumber>",
                    "</BillShipper>",
                "</Prepaid>",
        "</PaymentInformation>",
        sbPackages,

            #region Not used
        //"<Package>",
        //        "<PackagingType>",
        //            "<Code>02</Code>",
        //            "<Description>Customer Supplied</Description>",
        //        "</PackagingType>",
        //        "<Description>Rate</Description>",
        //        "<PackageWeight>",
        //            "<UnitOfMeasurement>",
        //              "<Code>LBS</Code>",
        //            "</UnitOfMeasurement>",
        //            "<Weight>65</Weight>",
        //        "</PackageWeight>",
        //        "<Dimensions>",
        //            "<UnitOfMeasurement>",
        //              "<Code>IN</Code>",
        //            "</UnitOfMeasurement>",
        //            "<Length>29</Length>",
        //            "<Width>29</Width>",
        //            "<Height>42</Height>",
        //        "</Dimensions>",
        //"</Package>",


        //"<ShipmentServiceOptions>",
        //  "<OnCallAir>",
        //    "<Schedule>", 
        //        "<PickupDay>02</PickupDay>",
        //        "<Method>02</Method>",
        //    "</Schedule>",
        //  "</OnCallAir>",
        //"</ShipmentServiceOptions>",
            #endregion

        "<RateInformation><NegotiatedRatesIndicator /></RateInformation>",

      "</Shipment>",

    "</RatingServiceSelectionRequest>");

            #endregion

            //string url = "http://api.globalcargomanager.com/GetUPS_PackageRates";
            string url = "http://apiTest.globalcargomanager.com/GetUPS_PackageRates";

            //HelperFuncs.writeToSiteErrors("getPackageAPI_XML_Rate data", data);
            //string doc = "test";
            string doc = (string)HelperFuncs.generic_http_request_3("string", null, url, "", "application/x-www-form-urlencoded",
                "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8", "POST", data, false, false, "", "");

            //HelperFuncs.writeToSiteErrors("getPackageAPI_XML_Rate doc", doc);

            #region Not used
            //string[] tokens = new string[4];
            //tokens[0] = "<NegotiatedRates>";
            //tokens[1] = "<MonetaryValue>";
            //tokens[2] = ">";
            //tokens[3] = "<";
            //string strNegotiatedRate = HelperFuncs.scrapeFromPage(tokens, doc);
            #endregion

            string[] tokens = new string[2];
            tokens[0] = ">";
            tokens[1] = "<";

            string strNegotiatedRate = HelperFuncs.scrapeFromPage(tokens, doc);

            //HelperFuncs.writeToSiteErrors("getPackageAPI_XML_Rate strNegotiatedRate", strNegotiatedRate);

            double negotiatedRate = 0.0;
            if (!double.TryParse(strNegotiatedRate, out negotiatedRate))
            {
                HelperFuncs.writeToSiteErrors("getPackageAPI_XML_Rate", string.Concat("negotiated rate not parsed to double, value was: ", strNegotiatedRate));
            }

            #region Switch serviceType

            switch (serviceType)
            {
                case ("01"):
                    {
                        resUPSNextDayAir.service = "Next Day Air";
                        double.TryParse(strNegotiatedRate, out resUPSNextDayAir.cost);
                        resUPSNextDayAir.days = 5;
                        break;
                    }
                case ("02"):
                    {
                        resSecondDayAir.service = "Second Day Air";
                        double.TryParse(strNegotiatedRate, out resSecondDayAir.cost);
                        resSecondDayAir.days = 5;
                        break;
                    }
                case ("03"):
                    {
                        resUPSGround.service = "UPS Ground";
                        double.TryParse(strNegotiatedRate, out resUPSGround.cost);
                        resUPSGround.days = 5;
                        break;
                    }

                case ("12"):
                    {
                        res3DaySelect.service = "3 Day Select";
                        double.TryParse(strNegotiatedRate, out res3DaySelect.cost);
                        res3DaySelect.days = 5;
                        break;
                    }
                case ("13"):
                    {
                        resNextDayAirSaver.service = "Next Day Air Saver";
                        double.TryParse(strNegotiatedRate, out resNextDayAirSaver.cost);
                        resNextDayAirSaver.days = 5;
                        break;
                    }
                case ("14"):
                    {
                        resNextDayAirEarlyAM.service = "Next Day Air Early AM";
                        double.TryParse(strNegotiatedRate, out resNextDayAirEarlyAM.cost);
                        resNextDayAirEarlyAM.days = 5;
                        break;
                    }
                case ("59"):
                    {
                        res2ndDayAirAM.service = "2nd Day Air AM";
                        double.TryParse(strNegotiatedRate, out res2ndDayAirAM.cost);
                        res2ndDayAirAM.days = 5;
                        break;
                    }
                default:
                    {
                        //row.service = "UPS Ground";
                        break;
                    }
            }

            #endregion

        }

        #endregion

        #region AddUPS_PackageResultToArray

        public void AddUPS_PackageResultToArray(string mode, upsPackageResRow objCarrier, double dblParcelMarkupPercent,
            ref int transitAddition, ref double addition, ref GCMRateQuote[] totalQuotes)
        {
            if (!(objCarrier.cost > 0))
            {
                return;
            }

            double dblParcelMarkup = 0.0;

            try
            {
                GCMRateQuote objQuote = new GCMRateQuote();
                objQuote.BookingKey = "#1#";
                objQuote.CarrierKey = "UPS";
                objQuote.DeliveryDays = objCarrier.days;
                objQuote.DeliveryDays += transitAddition;

                objQuote.DisplayName = objCarrier.service;
                objQuote.TotalPrice = Math.Round(objCarrier.cost, 2);
                objQuote.OurRate = objQuote.TotalPrice;
                dblParcelMarkup = Convert.ToDouble(Convert.ToDouble(dblParcelMarkupPercent) / 100.00) * objQuote.TotalPrice;

                // Add markup
                #region Add markup

                #region Not used
                //if (Request.QueryString["q_Unit1"] != null && Request.QueryString["q_Unit1"] == "LETTER")
                //{
                //    if (dblParcelMarkup < 6)
                //        objQuote.TotalPrice += 6;
                //    else
                //        objQuote.TotalPrice += dblParcelMarkup;
                //}
                //else
                //{
                //    if (objQuote.TotalPrice < 20)
                //    {
                //        if (dblParcelMarkup < 15)
                //            objQuote.TotalPrice += 15;
                //        else
                //            objQuote.TotalPrice += dblParcelMarkup;
                //    }
                //    else if (objQuote.TotalPrice < 100)
                //    {
                //        if (dblParcelMarkup < 25)
                //            objQuote.TotalPrice += 25;
                //        else
                //            objQuote.TotalPrice += dblParcelMarkup;
                //    }
                //    else
                //    {
                //        if (dblParcelMarkup < 30)
                //            objQuote.TotalPrice += 30;
                //        else
                //            objQuote.TotalPrice += dblParcelMarkup;
                //    }
                //}
                #endregion

                if (mode.Equals("NetNet"))
                {
                    // Do Nothing
                }
                else
                {
                    if (objQuote.TotalPrice < 20)
                    {
                        if (dblParcelMarkup < 15)
                            objQuote.TotalPrice += 15;
                        else
                            objQuote.TotalPrice += dblParcelMarkup;
                    }
                    else if (objQuote.TotalPrice < 100)
                    {
                        if (dblParcelMarkup < 25)
                            objQuote.TotalPrice += 25;
                        else
                            objQuote.TotalPrice += dblParcelMarkup;
                    }
                    else
                    {
                        if (dblParcelMarkup < 30)
                            objQuote.TotalPrice += 30;
                        else
                            objQuote.TotalPrice += dblParcelMarkup;
                    }
                }

                #endregion

                objQuote.TotalPrice += addition;
                objQuote.TotalPrice += 3; //dispatch fee

                objQuote.Documentation = null;
                totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objQuote);
            }
            catch (Exception upsE)
            {
                HelperFuncs.writeToSiteErrors("UPS Package", upsE.ToString());
            }
        }

        #endregion

        #region AddCarrierResultsToArray_UPS_Package

        public void AddCarrierResultsToArray_UPS_Package(string mode, string username, ref int transitAddition, ref double addition, 
            ref GCMRateQuote[] totalQuotes,

                ref upsPackageResRow resUPSGround,
            ref upsPackageResRow resUPSNextDayAir,

            ref upsPackageResRow resSecondDayAir,
            ref upsPackageResRow res3DaySelect,
            ref upsPackageResRow resNextDayAirSaver,
            ref upsPackageResRow resNextDayAirEarlyAM,
            ref upsPackageResRow res2ndDayAirAM)
            
        {
           
            /*
              HelperFuncs.upsPackageResRow resSecondDayAir;
        HelperFuncs.upsPackageResRow res3DaySelect;
        HelperFuncs.upsPackageResRow resNextDayAirSaver;
        HelperFuncs.upsPackageResRow resNextDayAirEarlyAM;
        HelperFuncs.upsPackageResRow res2ndDayAirAM;
             */
            double dblParcelMarkupPercent = GetParcelMarkup(ref username);

            if (resUPSGround.cost > 0)
            {               
                AddUPS_PackageResultToArray(mode, resUPSGround, dblParcelMarkupPercent, ref transitAddition, ref addition, ref totalQuotes);
            }
            if (resUPSNextDayAir.cost > 0)
            {               
                AddUPS_PackageResultToArray(mode, resUPSNextDayAir, dblParcelMarkupPercent, ref transitAddition, ref addition, ref totalQuotes);
            }
            if (resSecondDayAir.cost > 0)
            {                
                AddUPS_PackageResultToArray(mode, resSecondDayAir, dblParcelMarkupPercent, ref transitAddition, ref addition, ref totalQuotes);
            }
            if (res3DaySelect.cost > 0)
            {               
                AddUPS_PackageResultToArray(mode, res3DaySelect, dblParcelMarkupPercent, ref transitAddition, ref addition, ref totalQuotes);
            }
            if (resNextDayAirSaver.cost > 0)
            {                
                AddUPS_PackageResultToArray(mode, resNextDayAirSaver, dblParcelMarkupPercent, ref transitAddition, ref addition, ref totalQuotes);
            }
            if (resNextDayAirEarlyAM.cost > 0)
            {                
                AddUPS_PackageResultToArray(mode, resNextDayAirEarlyAM, dblParcelMarkupPercent, ref transitAddition, ref addition, ref totalQuotes);
            }
            if (res2ndDayAirAM.cost > 0)
            {               
                AddUPS_PackageResultToArray(mode, res2ndDayAirAM, dblParcelMarkupPercent, ref transitAddition, ref addition, ref totalQuotes);
            }
        }

        #endregion

        #region GetParcelMarkup

        private int GetParcelMarkup(ref string username)
        {

            int intParcelMarkup = 0;
            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aesmain_dataConnectionStringSS"].ConnectionString))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(string.Concat("select FedExMarkup from tbl_LOGIN where UserName='", username, "'"), conn))
                {
                    using (SqlDataReader dr = comm.ExecuteReader())
                    {
                        //bool isAuth = dr.HasRows;
                        //if (isAuth)
                        //{
                        //    intParcelMarkup = Convert.ToInt32(dr["FedExMarkup"]);
                        //}
                        if (dr.Read())
                        {
                            if (dr["FedExMarkup"] != DBNull.Value)
                            {
                                intParcelMarkup = Convert.ToInt32(dr["FedExMarkup"]);
                            }
                        }
                        //dr.Close();
                        //comm.Dispose();
                        //conn.Close();
                        //conn.Dispose();
                    }
                }
            }

            return intParcelMarkup;

        }

        #endregion
        
        #region Not used getUPS_PackageInfoViaAPI
        // Not used in favor of XML API
        //    private string[] getUPS_PackageInfoViaAPI(
        //            ref string username, ref string password, ref string originZip, ref string originCity, ref string originState,
        //            ref string destZip, ref string destCity, ref string destState, ref List<string> length, ref List<string> width,
        //            ref List<string> height, ref List<string> weight, ref List<string> packageType, ref double totWeight, ref string serviceType)
        //    {

        //        #region Variables

        //        string[] res = new string[3];

        //        #endregion

        //        StringBuilder logger = new StringBuilder();

        //        try
        //        {
        //            #region Variables
        //            gcmAPI.UPS_PackageRateService1.RateService rate = new gcmAPI.UPS_PackageRateService1.RateService();
        //            gcmAPI.UPS_PackageRateService1.RateRequest rateRequest = new gcmAPI.UPS_PackageRateService1.RateRequest();
        //            gcmAPI.UPS_PackageRateService1.UPSSecurity upss = new gcmAPI.UPS_PackageRateService1.UPSSecurity();

        //            gcmAPI.UPS_PackageRateService1.UPSSecurityServiceAccessToken upssSvcAccessToken = new gcmAPI.UPS_PackageRateService1.UPSSecurityServiceAccessToken();
        //            upssSvcAccessToken.AccessLicenseNumber = "";
        //            upss.ServiceAccessToken = upssSvcAccessToken;
        //            gcmAPI.UPS_PackageRateService1.UPSSecurityUsernameToken upssUsrNameToken = new gcmAPI.UPS_PackageRateService1.UPSSecurityUsernameToken();
        //            upssUsrNameToken.Username = "";
        //            upssUsrNameToken.Password = "";
        //            upss.UsernameToken = upssUsrNameToken;


        //            rate.UPSSecurityValue = upss;

        //            gcmAPI.UPS_PackageRateService1.RequestType request = new gcmAPI.UPS_PackageRateService1.RequestType();
        //            String[] requestOption = { "Rate" };
        //            request.RequestOption = requestOption;
        //            rateRequest.Request = request;
        //            gcmAPI.UPS_PackageRateService1.ShipmentType shipment = new gcmAPI.UPS_PackageRateService1.ShipmentType();
        //            gcmAPI.UPS_PackageRateService1.ShipperType shipper = new gcmAPI.UPS_PackageRateService1.ShipperType();
        //            shipper.ShipperNumber = "";
        //            #endregion

        //            #region Origin
        //            gcmAPI.UPS_PackageRateService1.AddressType shipperAddress = new gcmAPI.UPS_PackageRateService1.AddressType();
        //            String[] addressLine = { "", "", "" };
        //            shipperAddress.AddressLine = addressLine;
        //            shipperAddress.City = originCity;
        //            shipperAddress.PostalCode = originZip;
        //            shipperAddress.StateProvinceCode = originState;
        //            shipperAddress.CountryCode = "US";
        //            shipperAddress.AddressLine = addressLine;
        //            shipper.Address = shipperAddress;
        //            shipment.Shipper = shipper;
        //            gcmAPI.UPS_PackageRateService1.ShipFromType shipFrom = new gcmAPI.UPS_PackageRateService1.ShipFromType();

        //            gcmAPI.UPS_PackageRateService1.AddressType shipFromAddress = new gcmAPI.UPS_PackageRateService1.AddressType();
        //            shipFromAddress.AddressLine = addressLine;
        //            shipFromAddress.City = originCity;
        //            shipFromAddress.PostalCode = originZip;
        //            shipFromAddress.StateProvinceCode = originState;
        //            shipFromAddress.CountryCode = "US";
        //            shipFrom.Address = shipFromAddress;
        //            shipment.ShipFrom = shipFrom;
        //            gcmAPI.UPS_PackageRateService1.ShipToType shipTo = new gcmAPI.UPS_PackageRateService1.ShipToType();
        //            #endregion

        //            #region Destination
        //            gcmAPI.UPS_PackageRateService1.ShipToAddressType shipToAddress = new gcmAPI.UPS_PackageRateService1.ShipToAddressType();
        //            String[] addressLine1 = { "", "", "" };
        //            shipToAddress.AddressLine = addressLine1;
        //            shipToAddress.City = destCity;
        //            shipToAddress.PostalCode = destZip;
        //            shipToAddress.StateProvinceCode = destState;
        //            shipToAddress.CountryCode = "US";
        //            shipTo.Address = shipToAddress;
        //            shipment.ShipTo = shipTo;
        //            #endregion

        //            gcmAPI.UPS_PackageRateService1.CodeDescriptionType service = new gcmAPI.UPS_PackageRateService1.CodeDescriptionType();


        //            // Below code uses dummy date for reference. Please udpate as required.
        //            service.Code = serviceType; // UPS Ground
        //            shipment.Service = service;

        //            List<gcmAPI.UPS_PackageRateService1.PackageType> pkgList = new List<gcmAPI.UPS_PackageRateService1.PackageType>();
        //            for (int i = 1; i <= weight.Count; i++)
        //            {
        //                gcmAPI.UPS_PackageRateService1.PackageType package = new gcmAPI.UPS_PackageRateService1.PackageType();
        //                gcmAPI.UPS_PackageRateService1.PackageWeightType packageWeight = new gcmAPI.UPS_PackageRateService1.PackageWeightType();

        //                packageWeight.Weight = weight[i - 1];
        //                gcmAPI.UPS_PackageRateService1.CodeDescriptionType uom = new gcmAPI.UPS_PackageRateService1.CodeDescriptionType();
        //                uom.Code = "LBS";
        //                uom.Description = "pounds";
        //                packageWeight.UnitOfMeasurement = uom;
        //                package.PackageWeight = packageWeight;
        //                gcmAPI.UPS_PackageRateService1.CodeDescriptionType packType = new gcmAPI.UPS_PackageRateService1.CodeDescriptionType();
        //                packType.Code = "02"; // Your packaging
        //                package.PackagingType = packType;

        //                if (!string.IsNullOrEmpty(length[i - 1]))
        //                {
        //                    package.Dimensions = new gcmAPI.UPS_PackageRateService1.DimensionsType();
        //                    gcmAPI.UPS_PackageRateService1.CodeDescriptionType uomDims = new gcmAPI.UPS_PackageRateService1.CodeDescriptionType();
        //                    uomDims.Code = "IN";
        //                    package.Dimensions.UnitOfMeasurement = uomDims;
        //                    package.Dimensions.Length = length[i - 1];
        //                    package.Dimensions.Width = width[i - 1];
        //                    package.Dimensions.Height = height[i - 1];
        //                }

        //                pkgList.Add(package);
        //            }

        //            shipment.Package = pkgList.ToArray();

        //            rateRequest.Shipment = shipment;

        //            logger.Append(rateRequest);

        //            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //            gcmAPI.UPS_PackageRateService1.RateResponse rateResponse = rate.ProcessRate(rateRequest);
        //            logger.Append("The transaction was a " + rateResponse.Response.ResponseStatus.Description);
        //            logger.Append("Total Shipment Charges " + rateResponse.RatedShipment[0].TotalCharges.MonetaryValue + rateResponse.RatedShipment[0].TotalCharges.CurrencyCode);

        //            // Get all services - ground, 2 day, air early..
        //            //upsParcelResList = new List<HelperFuncs.upsPackageResRow>();

        //            //HelperFuncs.upsPackageResRow row = new HelperFuncs.upsPackageResRow();
        //            //double.TryParse(rateResponse.RatedShipment[0].TotalCharges.MonetaryValue, out row.cost);
        //            //int.TryParse(rateResponse.RatedShipment[0].GuaranteedDelivery.BusinessDaysInTransit, out row.days);
        //            //row.days = 5;

        //            /*
        //            * 01 = Next Day Air
        //02 = 2nd Day Air
        //03 = Ground
        //12 = 3 Day Select
        //13 = Next Day Air Saver
        //14 = Next Day Air Early AM
        //59 = 2nd Day Air AM

        //            */

        //            /*
        //      HelperFuncs.upsPackageResRow resSecondDayAir;
        //HelperFuncs.upsPackageResRow res3DaySelect;
        //HelperFuncs.upsPackageResRow resNextDayAirSaver;
        //HelperFuncs.upsPackageResRow resNextDayAirEarlyAM;
        //HelperFuncs.upsPackageResRow res2ndDayAirAM;
        //     */

        //            switch (serviceType)
        //            {
        //                case ("01"):
        //                    {
        //                        resUPSNextDayAir.service = "Next Day Air";
        //                        double.TryParse(rateResponse.RatedShipment[0].TotalCharges.MonetaryValue, out resUPSNextDayAir.cost);
        //                        resUPSNextDayAir.days = 5;
        //                        break;
        //                    }
        //                case ("02"):
        //                    {
        //                        resSecondDayAir.service = "Second Day Air";
        //                        double.TryParse(rateResponse.RatedShipment[0].TotalCharges.MonetaryValue, out resSecondDayAir.cost);
        //                        resSecondDayAir.days = 5;
        //                        break;
        //                    }
        //                case ("03"):
        //                    {
        //                        resUPSGround.service = "UPS Ground";
        //                        double.TryParse(rateResponse.RatedShipment[0].TotalCharges.MonetaryValue, out resUPSGround.cost);
        //                        resUPSGround.days = 5;
        //                        break;
        //                    }

        //                case ("12"):
        //                    {
        //                        res3DaySelect.service = "3 Day Select";
        //                        double.TryParse(rateResponse.RatedShipment[0].TotalCharges.MonetaryValue, out res3DaySelect.cost);
        //                        res3DaySelect.days = 5;
        //                        break;
        //                    }
        //                case ("13"):
        //                    {
        //                        resNextDayAirSaver.service = "Next Day Air Saver";
        //                        double.TryParse(rateResponse.RatedShipment[0].TotalCharges.MonetaryValue, out resNextDayAirSaver.cost);
        //                        resNextDayAirSaver.days = 5;
        //                        break;
        //                    }
        //                case ("14"):
        //                    {
        //                        resNextDayAirEarlyAM.service = "Next Day Air Early AM";
        //                        double.TryParse(rateResponse.RatedShipment[0].TotalCharges.MonetaryValue, out resNextDayAirEarlyAM.cost);
        //                        resNextDayAirEarlyAM.days = 5;
        //                        break;
        //                    }
        //                case ("59"):
        //                    {
        //                        res2ndDayAirAM.service = "2nd Day Air AM";
        //                        double.TryParse(rateResponse.RatedShipment[0].TotalCharges.MonetaryValue, out res2ndDayAirAM.cost);
        //                        res2ndDayAirAM.days = 5;
        //                        break;
        //                    }
        //                default:
        //                    {
        //                        //row.service = "UPS Ground";
        //                        break;
        //                    }
        //            }

        //            //upsParcelResList.Add(row);

        //            //HelperFuncs.writeToSiteErrors("UPSPackageAPI", row.cost.ToString() + " " + row.days.ToString());

        //            res[0] = "success";
        //            res[1] = "";
        //            res[2] = "";

        //        }
        //        catch (System.Web.Services.Protocols.SoapException ex)
        //        {
        //            logger.Append("");
        //            logger.Append("---------Rate Web Service returns error----------------");
        //            logger.Append("---------\"Hard\" is user error \"Transient\" is system error----------------");
        //            logger.Append("SoapException Message= " + ex.Message);
        //            logger.Append("");
        //            logger.Append("SoapException Category:Code:Message= " + ex.Detail.LastChild.InnerText);
        //            logger.Append("");
        //            logger.Append("SoapException XML String for all= " + ex.Detail.LastChild.OuterXml);
        //            logger.Append("");
        //            logger.Append("SoapException StackTrace= " + ex.StackTrace);

        //        }
        //        //catch (System.ServiceModel.CommunicationException ex)
        //        //{
        //        //    logger.Append("");
        //        //    logger.Append("--------------------");
        //        //    logger.Append("CommunicationException= " + ex.Message);
        //        //    logger.Append("CommunicationException-StackTrace= " + ex.StackTrace);
        //        //    logger.Append("-------------------------");

        //        //}
        //        catch (Exception ex)
        //        {
        //            logger.Append("");
        //            logger.Append("-------------------------");
        //            logger.Append(" General Exception= " + ex.Message);
        //            logger.Append(" General Exception-StackTrace= " + ex.StackTrace);

        //        }
        //        finally
        //        {

        //            HelperFuncs.writeToSiteErrors("UPSPackageAPI", logger.ToString());
        //        }

        //        return res;

        //    }

        #endregion

        #region Regular UPS

        #region GetResultObjectFromUPS

        private void GetResultObjectFromUPS(ref ArrayList alUPSResult)
        {

            try
            {
                if (quoteData.AccessorialsObj.TRADEPU.Equals(true) || quoteData.AccessorialsObj.TRADEDEL.Equals(true))
                {
                    throw new Exception("Tradeshow not supported");
                }

                ArrayList alAllItems = new ArrayList();
                ArrayList alItem;

                for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
                {
                    if (quoteData.m_lPiece[i].Weight > 0)
                    {
                        alItem = new ArrayList();

                        alItem.Add(Convert.ToInt32(quoteData.m_lPiece[i].Weight));
                        if (quoteData.AccessorialsObj.TRADEDEL || quoteData.AccessorialsObj.TRADEPU)
                        {
                            alItem.Add(125);
                        }
                        else
                        {
                            alItem.Add(quoteData.m_lPiece[i].FreightClass);
                        }
                        alItem.Add(quoteData.isHazmat);

                        alAllItems.Add(alItem);
                    }
                }

                //For Additional Services//////////////
                bool[] services = new bool[10];

                services[0] = (quoteData.AccessorialsObj.RESPU || quoteData.AccessorialsObj.CONPU);
                services[1] = (quoteData.AccessorialsObj.RESDEL || quoteData.AccessorialsObj.CONDEL);
                services[2] = quoteData.AccessorialsObj.TRADEPU;
                services[3] = quoteData.AccessorialsObj.TRADEDEL;
                services[4] = quoteData.AccessorialsObj.LGPU;
                services[5] = quoteData.AccessorialsObj.LGDEL;
                services[6] = quoteData.AccessorialsObj.APTPU;
                services[7] = quoteData.AccessorialsObj.APTDEL;
                services[8] = false;
                services[9] = quoteData.AccessorialsObj.INSDEL;

                //ArrayList alResult=new ArrayList();
                ArrayList alResult = getRatesUPS(quoteData.origZip.Trim(), quoteData.destZip.Trim(), alAllItems, services);

                alUPSResult = alResult;
            }
            catch (Exception ex)
            {
                alUPSResult = null;
                HelperFuncs.writeToSiteErrors("GetResultObjectFromUPS", ex.ToString());
            }
        }

        #endregion

        #region getRatesUPS

        private ArrayList getRatesUPS(string oZip, string dZip, ArrayList pieces, bool[] info)
        {

            #region Variables

            ArrayList rate = new ArrayList();
            ArrayList classes = new ArrayList();
            double absMin = 0;
            double discount = 0;
            double laneMin = 0;
            double totalWeight = 0;
            double extraFlat = 0;
            double extraCWT = 0;
            double deficitRate = 999999;
            double costActual = 0;
            double costDeficit = 0;
            double bestCost = 0;
            double BAF = 0;
            int transitTime = 0;
            int originZip = 0;
            int destZip = 0;
            double priceIncrease = 13.9;  //Price increase that was enacted in January 2010, adding 13.9% to all rates pre accessorials

            //variables used in finding rates
            int originZipPrefix = 0, destZipPrefix = 0, rateBasis = 0, classFactor = 0;
            double baseRate = 0, minCharge = 0, deficitBaseRate = 0, adjustment = 0, minAdjustment = 0, adjustmentDeficit = 0;
            string adjustmentNums = "";

            #endregion

            #region Accessorials and Info

            try
            {
                originZip = Int32.Parse(oZip);
                destZip = Int32.Parse(dZip);
            }
            catch
            {
                return rate;
            }

            string connString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aes_daylightSS"].ConnectionString;
            SqlConnection dbConn = new SqlConnection(connString);
            dbConn.Open();

            string sql = "SELECT BAF FROM SQL_INFO";

            DataSet ds1 = new DataSet();
            SqlDataAdapter da1 = new SqlDataAdapter(sql, dbConn);
            ds1.Clear();
            da1.Fill(ds1, "rate");

            if (ds1.Tables["rate"].Rows.Count == 1)
            {
                BAF = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(0).ToString());
            }
            else
            {
                return rate;
            }

            foreach (ArrayList a in pieces)
            {
                totalWeight += Double.Parse(a[0].ToString());
                classes.Add(a[1].ToString());
            }

            sql = "SELECT Discount, Minimum, ResidentialPickup, ResidentialDelivery, LiftgatePickupMin, LiftgateDeliveryMin, LiftgatePickupRate, " +
                "LiftgateDeliveryRate, TradeshowPickup, TradeshowDelivery, InsidePickupMin, InsideDeliveryMin, InsidePickupRate, InsideDeliveryRate, " +
                "InsidePickupMinNY, InsideDeliveryMinNY, InsidePickupRateNY, InsideDeliveryRateNY FROM SQL_UPS_INFO";

            da1 = new SqlDataAdapter(sql, dbConn);
            ds1 = new DataSet();
            da1.Fill(ds1, "rate");

            discount = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(0).ToString());
            absMin = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(1).ToString());

            if (info[0] == true) //res PU
            {
                extraFlat += Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(2).ToString());
            }
            if (info[1] == true) //res del
            {
                extraFlat += Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(3).ToString());
            }
            if (info[2] == true) //Trade PU
            {
                extraFlat += Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(8).ToString());
                transitTime += 1;
            }
            if (info[3] == true) //Trade Del
            {
                extraFlat += Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(9).ToString());
                transitTime += 1;
            }
            if (info[4] == true) //lift PU
            {
                double cwt = 0, min = 0;
                cwt = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(6).ToString());
                min = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(4).ToString());
                if ((totalWeight / 100) * cwt > min)
                {
                    extraCWT += cwt;
                }
                else
                {
                    extraFlat += min;
                }
            }
            if (info[5] == true) //lift del
            {
                double cwt = 0, min = 0;
                cwt = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(7).ToString());
                min = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(5).ToString());
                if ((totalWeight / 100) * cwt > min)
                {
                    extraCWT += cwt;
                }
                else
                {
                    extraFlat += min;
                }
            }
            if (info[6] == true) //Note PU
            {
                extraFlat += 32;
            }
            if (info[7] == true) //Note Del
            {
                extraFlat += 32;
            }
            if (info[8] == true) //inside PU
            {
                if (originZip < 10100 && originZip > 10001)
                {
                    double cwt = 0, min = 0;
                    cwt = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(16).ToString());
                    min = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(14).ToString());
                    if ((totalWeight / 100) * cwt > min)
                    {
                        extraCWT += cwt;
                    }
                    else
                    {
                        extraFlat += min;
                    }
                }
                else
                {
                    double cwt = 0, min = 0;
                    cwt = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(12).ToString());
                    min = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(10).ToString());
                    if ((totalWeight / 100) * cwt > min)
                    {
                        extraCWT += cwt;
                    }
                    else
                    {
                        extraFlat += min;
                    }
                }
            }
            if (info[9] == true) //inside Del
            {
                if (destZip < 10100 && destZip > 10001)
                {
                    double cwt = 0, min = 0;
                    cwt = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(17).ToString());
                    min = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(15).ToString());
                    if ((totalWeight / 100) * cwt > min)
                    {
                        extraCWT += cwt;
                    }
                    else
                    {
                        extraFlat += min;
                    }
                }
                else
                {
                    double cwt = 0, min = 0;
                    cwt = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(13).ToString());
                    min = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(11).ToString());
                    if ((totalWeight / 100) * cwt > min)
                    {
                        extraCWT += cwt;
                    }
                    else
                    {
                        extraFlat += min;
                    }
                }
            }

            #endregion

            #region Step 1

            originZipPrefix = originZip / 100;
            destZipPrefix = destZip / 100;

            sql = "SELECT RateBasis FROM SQL_UPS_RBN WHERE OriginZipPrefix = " + originZipPrefix + " AND DestZipPrefix >= " + destZipPrefix;

            da1 = new SqlDataAdapter(sql, dbConn);
            ds1 = new DataSet();
            da1.Fill(ds1, "rate");

            try
            {
                rateBasis = Int32.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(0).ToString());
            }
            catch
            {
                return rate;
            }

            #endregion

            #region Step 2

            sql = "SELECT ClassFactor, MinCharge";

            if (totalWeight < 500)
            {
                sql += ", LT500Rate, LT1000Rate";
            }
            else if (totalWeight < 1000)
            {
                sql += ", LT1000Rate, LT2000Rate";
            }
            else if (totalWeight < 2000)
            {
                sql += ", LT2000Rate, LT5000Rate";
            }
            else if (totalWeight < 5000)
            {
                sql += ", LT5000Rate, LT10000Rate";
            }
            else
            {
                sql += ", LT10000Rate, LT10000Rate";
            }

            sql += " FROM SQL_UPS_RTS WHERE RateBasis = " + rateBasis;

            da1 = new SqlDataAdapter(sql, dbConn);
            ds1 = new DataSet();
            da1.Fill(ds1, "rate");

            try
            {
                classFactor = Int32.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(0).ToString());
                minCharge = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(1).ToString());
                baseRate = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(2).ToString());
                deficitBaseRate = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(3).ToString());
            }
            catch
            {
                return rate;
            }

            #endregion

            #region Step 3

            foreach (ArrayList a in pieces)
            {
                if (totalWeight < 500)
                {
                    sql = "SELECT LT500Factor, LT1000Factor";
                }
                else if (totalWeight < 1000)
                {
                    sql = "SELECT LT1000Factor, LT2000Factor";
                }
                else if (totalWeight < 2000)
                {
                    sql = "SELECT LT2000Factor, LT5000Factor";
                }
                else if (totalWeight < 5000)
                {
                    sql = "SELECT LT5000Factor, LT10000Factor";
                }
                else
                {
                    sql = "SELECT LT10000Factor, LT10000Factor";
                }

                sql += " FROM SQL_UPS_FAC WHERE ClassFactor = " + classFactor + " AND Class = " + a[1].ToString();

                da1 = new SqlDataAdapter(sql, dbConn);
                ds1 = new DataSet();
                da1.Fill(ds1, "rate");

                try
                {
                    double temp = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(0).ToString());
                    a.Add(temp);

                    double temp2 = Double.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(1).ToString());
                    a.Add(temp2);
                }
                catch
                {
                    return rate;
                }
            }

            #endregion

            #region Step 4

            sql = "SELECT AdjustmentNum FROM SQL_UPS_LANE WHERE OriginZipLow <= " + originZip + " AND OriginZipHigh >= " + originZip + " AND " +
                "DestZipLow <= " + destZip + " AND DestZipHigh >= " + destZip;

            da1 = new SqlDataAdapter(sql, dbConn);
            ds1 = new DataSet();
            da1.Fill(ds1, "rate");

            try
            {
                if (ds1.Tables["rate"].Rows.Count > 0)
                {
                    foreach (DataRow a in ds1.Tables["rate"].Rows)
                    {
                        adjustmentNums += a.ItemArray.GetValue(0).ToString() + ", ";
                    }
                    adjustmentNums = adjustmentNums.Remove(adjustmentNums.Length - 2);
                }
            }
            catch
            {
                return rate;
            }

            bool firstPiece = true;

            if (adjustmentNums.Length > 0)
            {

                foreach (ArrayList a in pieces)
                {
                    sql = "SELECT MinChargeAdj";

                    if (totalWeight < 500)
                    {
                        sql += ", LT500Adj, LT1000Adj";
                    }
                    else if (totalWeight < 1000)
                    {
                        sql += ", LT1000Adj, LT2000Adj";
                    }
                    else if (totalWeight < 2000)
                    {
                        sql += ", LT2000Adj, LT5000Adj";
                    }
                    else if (totalWeight < 5000)
                    {
                        sql += ", LT5000Adj, LT10000Adj";
                    }
                    else
                    {
                        sql += ", LT10000Adj, LT10000Adj";
                    }

                    sql += ", AdjustmentNum FROM SQL_UPS_ADJ WHERE AdjustmentNum IN (" + adjustmentNums + ") AND Class = " + a[1].ToString();

                    da1 = new SqlDataAdapter(sql, dbConn);
                    ds1 = new DataSet();
                    da1.Fill(ds1, "rate");

                    string[] temp1 = adjustmentNums.Split(',');

                    try
                    {
                        foreach (DataRow b in ds1.Tables["rate"].Rows)
                        {
                            int count = 0;
                            foreach (string c in temp1)
                            {
                                if (c.Trim() == b.ItemArray.GetValue(3).ToString())
                                {
                                    count++;
                                }
                            }

                            if (firstPiece)
                            {
                                minAdjustment += count * (Double.Parse(b.ItemArray.GetValue(0).ToString()) - 1);
                            }

                            adjustment += count * (Double.Parse(b.ItemArray.GetValue(1).ToString()) - 1);
                            adjustmentDeficit += count * (Double.Parse(b.ItemArray.GetValue(2).ToString()) - 1);
                        }
                    }
                    catch
                    {
                        return rate;
                    }

                    minAdjustment += 1;
                    a.Add(adjustment + 1);
                    a.Add(adjustmentDeficit + 1);
                    firstPiece = false;
                    adjustment = 0;
                    adjustmentDeficit = 0;
                }
            }
            else
            {
                minAdjustment = 1;

                foreach (ArrayList a in pieces)
                {
                    a.Add(1);
                    a.Add(1);
                }
            }

            #endregion

            #region Step 5

            foreach (ArrayList a in pieces)
            {
                costActual += (baseRate * Double.Parse(a[3].ToString()) * (Double.Parse(a[5].ToString()))) * (Double.Parse(a[0].ToString()) / 100);
                costDeficit += (deficitBaseRate * Double.Parse(a[4].ToString()) * (Double.Parse(a[6].ToString()))) * (Double.Parse(a[0].ToString()) / 100);

                if ((baseRate * Double.Parse(a[4].ToString()) * (Double.Parse(a[6].ToString()))) < deficitRate)
                {
                    deficitRate = (deficitBaseRate * Double.Parse(a[4].ToString()) * (Double.Parse(a[6].ToString())));
                }
            }

            if (totalWeight < 500)
            {
                costDeficit += ((500 - totalWeight) / 100) * deficitRate;
            }
            else if (totalWeight < 1000)
            {
                costDeficit += ((1000 - totalWeight) / 100) * deficitRate;
            }
            else if (totalWeight < 2000)
            {
                costDeficit += ((2000 - totalWeight) / 100) * deficitRate;
            }
            else if (totalWeight < 5000)
            {
                costDeficit += ((5000 - totalWeight) / 100) * deficitRate;
            }
            else  //if (totalWeight < 10000)
            {
                costDeficit += ((10000 - totalWeight) / 100) * deficitRate;
            }

            if (costActual <= costDeficit)
            {
                bestCost = costActual;
            }
            else
            {
                bestCost = costDeficit;
            }

            bestCost = bestCost * ((100 - discount) / 100);
            laneMin = (minCharge * (minAdjustment)) * ((100 - discount) / 100);

            if (bestCost < laneMin)
            {
                bestCost = laneMin;
            }

            if (bestCost < absMin)
            {
                bestCost = absMin;
            }

            bestCost += bestCost * (priceIncrease / 100);
            bestCost += bestCost * (BAF / 100);
            bestCost += extraFlat;
            bestCost += extraCWT * (totalWeight / 100);

            //if (quoteData.subdomain.Equals("spc") || isCostPlus)
            //{
            //    bestCost = HelperFuncs.addSPC_Addition(bestCost);
            //}


            //if (quoteData.subdomain.Equals(HelperFuncs.Subdomains.clipper))
            //{
            //    bestCost = HelperFuncs.addClipperSubdomain_Addition(bestCost);
            //}

            bestCost = Math.Round(bestCost, 2);

            #endregion

            #region Transit Times

            rate.Add(bestCost);
            rate.Add("UPS Freight");
            rate.Add(classes);

            string originZone = "";
            string destZone = "";

            sql = "SELECT DeliveryZone FROM SQL_UPS_ZONES WHERE BeginZip <= " + originZip + " AND EndZip >= " + originZip;

            ds1 = new DataSet();
            da1 = new SqlDataAdapter(sql, dbConn);
            ds1.Clear();
            da1.Fill(ds1, "rate");

            if (ds1.Tables["rate"].Rows.Count == 1)
            {
                originZone = ds1.Tables["rate"].Rows[0].ItemArray.GetValue(0).ToString();
            }
            else
            {
                rate.Add(7);
                return rate;
            }

            sql = "SELECT DeliveryZone FROM SQL_UPS_ZONES WHERE BeginZip <= " + destZip + " AND EndZip >= " + destZip;

            ds1 = new DataSet();
            da1 = new SqlDataAdapter(sql, dbConn);
            ds1.Clear();
            da1.Fill(ds1, "rate");

            if (ds1.Tables["rate"].Rows.Count == 1)
            {
                destZone = ds1.Tables["rate"].Rows[0].ItemArray.GetValue(0).ToString();
            }
            else
            {
                rate.Add(7);
                return rate;
            }

            sql = "SELECT TransitTime FROM SQL_UPS_DAYS WHERE OriginZone = \'" + originZone + "\' AND DestZone = \'" + destZone + "\'";

            ds1 = new DataSet();
            da1 = new SqlDataAdapter(sql, dbConn);
            ds1.Clear();
            da1.Fill(ds1, "rate");

            if (ds1.Tables["rate"].Rows.Count == 1)
            {
                transitTime += Int32.Parse(ds1.Tables["rate"].Rows[0].ItemArray.GetValue(0).ToString());
            }
            else
            {
                rate.Add(7);
                return rate;
            }

            rate.Add(transitTime);

            #endregion

            dbConn.Close();

            return rate;
        }

        #endregion

        #region Not used, GetResultObjectFromUPSMWIScrape

        //private void GetResultObjectFromUPSMWIScrape()
        //{
        //    if (quoteData.AccessorialsObj.TRADEPU.Equals(true) || quoteData.AccessorialsObj.TRADEDEL.Equals(true))
        //    {
        //        throw new Exception("Tradeshow not supported");
        //    }

        //    HelperFuncs.writeToSiteErrors("GetResultObjectFromUPSMWIScrape", "");
        //    try
        //    {
        //        double dbl;
        //        Int16 transitDays;

        //        string[] res = GetLtlUPSFreightInfo("", "", midOrigZip, quoteData.origCity, quoteData.origZip, midDestZip, quoteData.destCity, quoteData.destZip);

        //        if (!Int16.TryParse(res[2], out transitDays))
        //            transitDays = -3;

        //        if (double.TryParse(res[1], out dbl))
        //        {
        //            GCMRateQuote objQuote = new GCMRateQuote();
        //            objQuote.TotalPrice = dbl;
        //            if (double.TryParse(res[3], out dbl)) //overlength Fee
        //                objQuote.TotalPrice += dbl;

        //            objQuote.DisplayName = "UPS Freight - MWI";
        //            objQuote.BookingKey = "#1#";
        //            objQuote.CarrierKey = "UPSMWI";
        //            //SC-1
        //            objQuote.GuaranteedRateAM = -1;
        //            objQuote.Documentation = "";
        //            //SC-1

        //            //transitDays = standardDays;
        //            objQuote.DeliveryDays = transitDays;
        //            upsQuoteMWI = objQuote;
        //        }
        //        else
        //            upsQuoteMWI = null;
        //    }
        //    catch (Exception e)
        //    {
        //        //if (e.Message == "Thread was being aborted.")
        //        //{
        //        //    //clean up              
        //        //}
        //        HelperFuncs.writeToSiteErrors("GetLtlUPSFreightInfo (Live)", e.ToString(), "");
        //    }


        //}

        #endregion

        #region Not used, GetLtlUPSFreightInfo

        //public string[] GetLtlUPSFreightInfo(string username, string password, string originZip, string originCity, string originState, string destZip, string destCity, string destState)
        //{
        //    string url, referrer, contentType, accept, method, doc, data;
        //    CookieContainer container = new CookieContainer();
        //    int overlengthFee = 0;

        //    try
        //    {
        //        #region weight/class/overlength

        //        List<string> weight = new List<string>();
        //        List<string> fclass = new List<string>();

        //        for (int i = 0; i < quoteData.m_lPiece.Length; i++)
        //        {
        //            fclass.Add(quoteData.m_lPiece[i].FreightClass);
        //            weight.Add(quoteData.m_lPiece[i].Weight.ToString());
        //        }

        //        // Get Overlenth Fee
        //        HelperFuncs.GetOverlengthFee(ref quoteData.m_lPiece, ref overlengthFee, 143, 216, 288, 100, 150, 200);

        //        #endregion

        //        #region Accessorials
        //        List<string> accessorials = new List<string>();
        //        if (quoteData.AccessorialsObj.RESPU)
        //        {
        //            accessorials.Add("RESPU");
        //        }
        //        if (quoteData.AccessorialsObj.RESDEL)
        //            accessorials.Add("RESDEL");

        //        if (quoteData.AccessorialsObj.INSDEL)
        //            accessorials.Add("INSDEL");

        //        if (quoteData.AccessorialsObj.LGPU)
        //            accessorials.Add("LGPU");

        //        if (quoteData.AccessorialsObj.LGDEL)
        //        {
        //            accessorials.Add("LGDEL");
        //        }

        //        if (quoteData.AccessorialsObj.CONPU)
        //            throw new Exception("UPS: Rate Estimator does not provide the option Construction Pickup, please use Limited Access Pickup instead.");

        //        if (quoteData.AccessorialsObj.CONDEL)
        //        {
        //            throw new Exception("UPS: Rate Estimator does not provide the option Construction Delivery, please use Limited Access Pickup instead.");
        //        }
        //        if (quoteData.AccessorialsObj.APTPU)
        //            throw new Exception("UPS: Rate Estimator does not provide the option Appointment on pickup.");

        //        if (quoteData.AccessorialsObj.APTDEL)
        //        {
        //            accessorials.Add("APT");
        //        }
        //        if (quoteData.isHazmat)
        //        {
        //            accessorials.Add("HAZMAT");
        //        }

        //        #endregion

        //        #region Build mult units str
        //        string multUnits = "&j=";
        //        for (int i = 0; i < fclass.Count; i++)
        //        {
        //            multUnits += fclass[i] + '!';
        //        }
        //        for (int i = fclass.Count; i < 5; i++)
        //        {
        //            multUnits += '!';
        //        }
        //        multUnits += "@";
        //        for (int i = 0; i < weight.Count; i++)
        //        {
        //            multUnits += weight[i] + '!';
        //        }
        //        for (int i = weight.Count; i < 5; i++)
        //        {
        //            multUnits += '!';
        //        }


        //        #endregion

        //        #region Login and go to index page

        //        accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        //        contentType = "";
        //        method = "GET";
        //        referrer = "https://www.ltlservices.com/user_services/non-auth/userlogin.do";
        //        url = "https://www.ups.com/one-to-one/login?method=login&uid=" + username + "&sysid=LB&langc=US&lang=en&returnto=https%3A%2F%2Fwww.ltlservices.com%2Fuser_services%2Fnon-auth%2Fuserlogin.do";
        //        doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", false);

        //        #region get cpf
        //        string[] tokens = new string[4];
        //        tokens[0] = "name=\"ims.cpf";
        //        tokens[1] = "value=";
        //        tokens[2] = "\"";
        //        tokens[3] = "\"";
        //        string cpf = HelperFuncs.scrapeFromPage(tokens, doc);
        //        #endregion

        //        referrer = url;
        //        url = "https://www.ups.com/one-to-one/login";
        //        contentType = "application/x-www-form-urlencoded";
        //        method = "POST";
        //        data = "sysid=LB&appid=null&lang=en&langc=US&method=login&returnto=https%3A%2F%2Fwww.ltlservices.com%2Fuser_services%2Fnon-auth%2Fuserlogin.do&loc=en_US&uid=" + username + "&password=" + password + "&next=Log+In&ims.cpf=" + cpf;
        //        doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, data, false);


        //        referrer = "https://www.ups.com/one-to-one/finishlogin?returnto=https%3A%2F%2Fwww.ltlservices.com%2Fuser_services%2Fnon-auth%2Fuserlogin.do&loc=en_US&returnto=https%3A%2F%2Fwww.ltlservices.com%2Fuser_services%2Fnon-auth%2Fuserlogin.do&lang=en&sysid=LB&langc=US";
        //        url = "https://www.ups.com/sso/SSO?TARGET=https%3A%2F%2Fwww.ltlservices.com%2Fuser_services%2Fnon-auth%2Fuserlogin.do&SPEntityID=upgf.sp";
        //        doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", false);

        //        tokens[0] = "name=\"SAMLResponse";
        //        string samlResponse = HelperFuncs.scrapeFromPage(tokens, doc);

        //        referrer = url;
        //        url = "https://fim.ltlservices.com/sso/ACS";
        //        contentType = "application/x-www-form-urlencoded";
        //        method = "POST";
        //        data = "SAMLResponse=" + samlResponse.Replace("+", "%2B") + "&RelayState=https%3A%2F%2Fwww.ltlservices.com%2Fuser_services%2Fnon-auth%2Fuserlogin.do";
        //        doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, data, false);



        //        #endregion


        //        //go to rating page
        //        contentType = "";
        //        method = "GET";
        //        //referrer = "https://www.ltlservices.com/apps/rating/Rating.aspx";
        //        //url = "https://www.ltlservices.com/ratingRest/non-auth/html/rating.html?source=secured";
        //        //doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", false);

        //        #region dateTime
        //        DateTime TodayDate = new DateTime(); // used in Data Params string for the time stamp
        //        TodayDate = DateTime.Today;
        //        string day = TodayDate.Day.ToString();

        //        if (Convert.ToInt32(day) < 10)
        //        {
        //            day = '0' + day;
        //        }
        //        #endregion

        //        #region Accessorials
        //        char LGPU = '0', RESPU = '0', LGDEL = '0', RESDEL = '0', INSDEL = '0',
        //            APT = '0', HAZ = '0', INSPU = '0', LIMACCDEL = '0', LIMACCPU = '0', TRADEPU = '0', TRADEDEL = '0';

        //        bool has_Resdel = false, has_Apt = false;
        //        foreach (string accessorial in accessorials)
        //        {
        //            if (accessorial == "LGPU")   //Liftgate Pickup
        //            {
        //                LGPU = '1';
        //            }

        //            else if (accessorial == "CONSPU")
        //            {
        //                throw new Exception("UPS: Rate Estimator does not provide the option Construction Pickup, please use Limited Access Pickup instead.");
        //            }
        //            else if (accessorial == "CONSDEL")
        //            {
        //                throw new Exception("UPS: Rate Estimator does not provide the option Construction Delivery, please use Limited Access Delivery instead.");
        //            }

        //            else if (accessorial == "RESPU") //Residential Pickup
        //            {
        //                RESPU = '1';
        //            }

        //            else if (accessorial == "LGDEL")  //Liftgate Delivery
        //            {
        //                LGDEL = '1';
        //            }

        //            else if (accessorial == "RESDEL") //Residential Delivery
        //            {
        //                RESDEL = '1';
        //            }
        //            else if (accessorial == "INSDEL")  //Inside Delivery
        //            {
        //                INSDEL = '1';
        //            }

        //            else if (accessorial == "APT")  //Delivery Notification
        //            {
        //                APT = '1';
        //            }

        //            else if (accessorial == "HAZMAT")  //Hazerdous Materials    
        //            {
        //                HAZ = '1';
        //            }
        //            else if (accessorial == "LIMACCDEL")
        //            {
        //                LIMACCDEL = '1';
        //            }
        //            else if (accessorial == "LIMACCPU")
        //            {
        //                LIMACCPU = '1';
        //            }
        //        }
        //        string accessorsStr = INSPU.ToString() + LGPU + RESPU + '0' + TRADEPU + '0' + LIMACCPU + "00"
        //            + INSDEL + LGDEL + RESDEL + '0' + TRADEDEL + LIMACCDEL + APT + "00" + HAZ;

        //        #endregion

        //        //build parameters string and get rate
        //        Random RndNum = new Random();
        //        int RnNum = RndNum.Next(1000000, 9999999);
        //        string rn = RndNum.ToString();
        //        referrer = "https://www.ltlservices.com/ratingRest/non-auth/html/rating.html?source=secured";
        //        //referrer = url;
        //        url = string.Concat("https://www.ltlservices.com/ratingRest/secured/ltlratingservice/getrates?i=US!", originZip, "!US!", destZip, "!", TodayDate.DayOfWeek.ToString(),
        //            ",%20", HelperFuncs.getMonth(TodayDate.Month), "%20", day, ",%20", TodayDate.Year.ToString(),
        //            "!40!10!!!!US!98166!!!!", //this makes it third party with zipcode 98166
        //            accessorsStr, //accessorials
        //            multUnits, "@!!!!!@!!!!!", // multiple units
        //            "&_=136313", Convert.ToString(RnNum));

        //        doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", false);

        //        string[] returnInfo;
        //        returnInfo = new string[4];
        //        //grab info
        //        //scrape total charge
        //        string[] toks = new string[3];
        //        toks[0] = "totalAmount";
        //        toks[1] = ":";
        //        toks[2] = "}";
        //        returnInfo[1] = HelperFuncs.scrapeFromPage(toks, doc); //rate

        //        toks[0] = "\"days\":";
        //        toks[2] = ",";
        //        returnInfo[2] = HelperFuncs.scrapeFromPage(toks, doc); //transit


        //        returnInfo[0] = "UPS Freight";
        //        returnInfo[3] = overlengthFee.ToString();
        //        return returnInfo;
        //    }
        //    catch (Exception e)
        //    {
        //        #region Catch
        //        string[] returnInfo;
        //        returnInfo = new string[4];
        //        string s = e.ToString();
        //        returnInfo[0] = "UPS Freight";
        //        returnInfo[1] = "issue";
        //        returnInfo[2] = "N/A";
        //        returnInfo[3] = "N/A";
        //        return returnInfo;
        //        #endregion
        //    }

        //}

        #endregion

        #endregion

        #region Not used, getAllUPS_PackageServices

        //private static void getAllUPS_PackageServices(string doc, ref List<HelperFuncs.upsPackageResRow> resRow)
        //{
        //    string tmp = doc, days = "", rate = "", service = "", tmpDoc = doc;
        //    int ind, i = 1;
        //    //HelperFuncs.writeToSiteErrors("GetUPSPackageNew Live get services", doc, "");
        //    while (doc.IndexOf("id=\"servicerow" + i) != -1)
        //    {
        //        //HelperFuncs.writeToSiteErrors("GetUPSPackageNew (Live)", "service row", "");
        //        ind = doc.IndexOf("id=\"servicerow" + i);
        //        doc = doc.Substring(ind);

        //        try
        //        {
        //            ind = doc.IndexOf("</tr>");
        //            if (ind != -1)
        //            {
        //                doc = doc.Remove(ind);

        //                //-------------------------------------

        //                ind = doc.IndexOf("class=\"btnlnkR");
        //                if (ind != -1)
        //                {
        //                    doc = doc.Substring(ind);
        //                    ind = doc.IndexOf(">");
        //                    doc = doc.Substring(ind + 1);
        //                    ind = doc.IndexOf("<");
        //                    service = doc.Remove(ind).Replace("&reg;", "");
        //                    //HelperFuncs.writeToSiteErrors("GetUPSPackageNew (Live)", service, "");
        //                }

        //                //-------------------------------------
        //                ind = doc.IndexOf("Days In Transit:");
        //                if (ind != -1)
        //                {
        //                    doc = doc.Substring(ind);
        //                    ind = doc.IndexOf("<strong>");
        //                    doc = doc.Substring(ind);
        //                    ind = doc.IndexOf(">");
        //                    doc = doc.Substring(ind + 1);
        //                    ind = doc.IndexOf("<");
        //                    days = doc.Remove(ind);

        //                    ind = doc.IndexOf("Begin Cost");
        //                    if (ind != -1)
        //                    {
        //                        doc = doc.Substring(ind);
        //                        ind = doc.IndexOf("<strong>");
        //                        doc = doc.Substring(ind);
        //                        ind = doc.IndexOf(">");
        //                        doc = doc.Substring(ind + 1);
        //                        ind = doc.IndexOf("<");
        //                        rate = doc.Remove(ind);
        //                        //HelperFuncs.writeToSiteErrors("GetUPSPackageNew (Live)", rate, "");
        //                    }
        //                }
        //                else
        //                {
        //                    ind = doc.IndexOf("Begin Cost");
        //                    if (ind != -1)
        //                    {
        //                        doc = doc.Substring(ind);
        //                        ind = doc.IndexOf("<strong>");
        //                        doc = doc.Substring(ind);
        //                        ind = doc.IndexOf(">");
        //                        doc = doc.Substring(ind + 1);
        //                        ind = doc.IndexOf("<");
        //                        rate = doc.Remove(ind);
        //                        //HelperFuncs.writeToSiteErrors("GetUPSPackageNew (Live)", rate, "");
        //                    }
        //                }
        //            }
        //            HelperFuncs.upsPackageResRow row = new HelperFuncs.upsPackageResRow();
        //            double.TryParse(rate, out row.cost);
        //            int.TryParse(days, out row.days);

        //            row.service = service;
        //            resRow.Add(row);
        //            doc = tmpDoc;
        //            i++;
        //        }
        //        catch (Exception e)
        //        {
        //            //HelperFuncs.writeToSiteErrors("GetUPSPackageNew (Live)", e.ToString(), "");
        //        }
        //    }

        //}

        #endregion

    }
}