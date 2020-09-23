#region Using

using gcmAPI.Models.LTL;
using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

#endregion

namespace gcmAPI.Models.Carriers.UPS
{
    public class UPS_FREIGHT
    {
        string AccessLicenseNumber;
        //Account Number
        CarrierAcctInfo acctInfo;
        QuoteData quoteData;

        public struct UPS_FREIGHT_RESULT
        {
            public string CarrierName, Scac;
            public decimal TotalCharges;
            public byte ServiceDays;
            ///public double rate;
        }

        // Constructor
        public UPS_FREIGHT(CarrierAcctInfo acctInfo, ref QuoteData quoteData, ref string AccessLicenseNumber)
        {
            this.acctInfo = acctInfo;
            this.quoteData = quoteData;
            this.AccessLicenseNumber = AccessLicenseNumber;
        }


        #region Rating

        #region Get_UPS_Freight_rate

        public void Get_UPS_Freight_rate(out GCMRateQuote UPS_FREIGHT_Quote)
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Web_client http = new Web_client();

            http.header_names = new string[3];
            http.header_values = new string[3];

            http.header_names[0] = "Username";
            http.header_names[1] = "Password";
            http.header_names[2] = "AccessLicenseNumber";

            http.header_values[0] = AppCodeConstants.ups_freight_genera_un;
            http.header_values[1] = AppCodeConstants.ups_freight_genera_pwd;
            http.header_values[2] = AppCodeConstants.ups_freight_genera_license_num;
            http.url = "https://onlinetools.ups.com/ship/v1801/freight/rating/ground";

            #region Not used
            //http.url = "https://onlinetools.ups.com/ship/v1801/freight/rating/ground";
            //http.url = "https://onlinetools.ups.com/ship/%7Bversion%7D/freight/rating/%7Brequestoption%7D";
            //http.url = "https://wwwcie.ups.com/ship/v1801/freight/rating/ground";
            #endregion
            
            http.method = "POST";

            #region Post data

            string orig_country = "US", dest_country = "US";
            if (quoteData.origCountry == "CANADA")
            {
                orig_country = "CA";
            }
            if (quoteData.destCountry == "CANADA")
            {
                dest_country = "CA";
            }

            string month, day;
            month = quoteData.puDate.Month.ToString();
            day = quoteData.puDate.Day.ToString();
            if(month.Length == 1)
            {
                month = "0" + month;
            }
            if (day.Length == 1)
            {
                day = "0" + day;
            }

            http.post_data = string.Concat(
                "{",
"\"FreightRateRequest\": {",
                // FreightRateRequest start
                // ShipFrom start
                "\"ShipFrom\": {",
                    "\"Name\": \"Test US Shipper\",",
                    "\"Address\": {",
                        "\"AddressLine\": \"123 Lane\",",
                        "\"City\": \"", quoteData.origCity, "\",",
                        "\"StateProvinceCode\": \"", quoteData.origState, "\",",
                        "\"PostalCode\": \"", quoteData.origZip, "\",",
                        "\"CountryCode\": \"", orig_country, "\",",
                        "\"ResidentialAddressIndicator\": \"\"",
                    "},",
                    "\"AttentionName\": \"Test Shipper\",",
                    "\"Phone\": {",
                        "\"Number\": \"4444444444\",",
                        "\"Extension\": \"4444\"",
                    "},",
                    "\"EMailAddress\": \"gcc0htq@ups.com\"",
                "},",
                // ShipFrom end
                "\"ShipperNumber\": \"54A9A6\",",

                // ShipTo end
                "\"ShipTo\": {",
                    "\"Name\": \"Test US Consignee\",",
                    "\"Address\": {",
                    "\"AddressLine\": \"555 Main St\",",
                    "\"City\": \"", quoteData.destCity, "\",\"StateProvinceCode\": \"", quoteData.destState, "\",",
                    "\"PostalCode\": \"", quoteData.destZip, "\",",
                    "\"CountryCode\": \"", dest_country, "\"",
                    "},",
                "\"AttentionName\": \"Dilbert\",",
                "\"Phone\": {",
                        "\"Number\": \"8459865555\"",
                    "}",
                "},",
                // ShipTo end

                // PaymentInformation start
                "\"PaymentInformation\": {",
                    "\"Payer\": {",
                            "\"Name\": \"Test US Shipper\",",
                            "\"Address\": {",
                            "\"AddressLine\": \"123 Lane\",",
                            "\"City\": \"LUTHERVILLE TIMONIUM\",",
                            "\"StateProvinceCode\": \"MD\",",
                            "\"PostalCode\": \"21093\",",
                            "\"CountryCode\": \"US\"",
                        "},",
                        "\"ShipperNumber\": \"54A9A6\",",
                        "\"AccountType\": \"1\",",
                        "\"AttentionName\": \"Test Shipper\",",
                        "\"Phone\": {",
                                "\"Number\": \"4444444444\",",
                                "\"Extension\": \"4444\"",
                        "},",
                        "\"EMailAddress\": \"gcc0htq@ups.com\"",
                    "},",

                    "\"ShipmentBillingOption\": {",
                        "\"Code\": \"10\"",
                        "}",
                "},",
                // PaymentInformation end
                "\"Service\": {",
                                    "\"Code\": \"308\"",
                "},",

                    // Commodity start
                    Get_items_json(),
                   
                    // Commodity end

                    "\"DensityEligibleIndicator\": \"\",",
                    "\"AlternateRateOptions\": {",
                                        "\"Code\": \"3\"",
                    "},",
                    "\"PickupRequest\": {",
                                        //"\"PickupDate\": \"20191021\"",
                                        "\"PickupDate\": \"", quoteData.puDate.Year, month, day, "\"",
                    "},",
                    "\"GFPOptions\": {",
                                        "\"GPFAccesorialRateIndicator\": \"\"",
                    "},",
                    Get_accessorials_json(),
                    "\"TimeInTransitIndicator\": \"\"",
                "}",
                // FreightRateRequest end
        "}"
        );

            #endregion

            http.accept = "application/json";
            http.content_type = "application/json";

            DB.Log("UPS_FREIGHT post data", http.post_data);
            UPS_FREIGHT_Quote = new GCMRateQuote();
            // Test
            //return;

            string doc = "";
            try
            {
                doc = http.Make_http_request();
            }
            catch(Exception e)
            {
                DB.Log("UPS_FREIGHT", e.ToString());
            }
            

            DB.Log("UPS_FREIGHT result", doc);

            #region Parse result

            string[] tokens = new string[5];
            tokens[0] = "TotalShipmentCharge";
            tokens[1] = "MonetaryValue";
            tokens[2] = ":";
            tokens[3] = "\"";
            tokens[4] = "\"";

            string TotalShipmentCharge_str = HelperFuncs.scrapeFromPage(tokens, doc);
            double TotalShipmentCharge;

            tokens[0] = "TimeInTransit";
            tokens[1] = "DaysInTransit";

            string DaysInTransit_str = HelperFuncs.scrapeFromPage(tokens, doc);
            int DaysInTransit;

            #endregion

            //UPS_FREIGHT_Quote = new GCMRateQuote();

            if (double.TryParse(TotalShipmentCharge_str, out TotalShipmentCharge))
            {
                UPS_FREIGHT_Quote.TotalPrice = TotalShipmentCharge;

                UPS_FREIGHT_Quote.DisplayName = acctInfo.displayName;
                UPS_FREIGHT_Quote.CarrierKey = acctInfo.carrierKey;
                UPS_FREIGHT_Quote.BookingKey = acctInfo.bookingKey;
                UPS_FREIGHT_Quote.Scac = "UPGF";

                if (int.TryParse(DaysInTransit_str, out DaysInTransit))
                {
                    UPS_FREIGHT_Quote.DeliveryDays = DaysInTransit;
                }
            }
            else
            {
                UPS_FREIGHT_Quote = null;
            }
        }

        #endregion

        #region Get_items_json
        private string Get_items_json()
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("\"Commodity\": [");

            for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
            {
                sb.Append(string.Concat("{\"CommodityID\": \"", i+1, "\",",
                                        "\"Description\": \"FRS-Freight\",",
                            "\"Weight\": {",
                                                "\"UnitOfMeasurement\": {",
                                                    "\"Code\": \"LBS\"",
                                                "},\"Value\": \"", quoteData.m_lPiece[0].Weight, "\"",
                                "},",


                        "\"Dimensions\": {",
                                                "\"UnitOfMeasurement\": {",
                                                    "\"Code\": \"IN\",",
                                                    "\"Description\": \" \"",
                                                "},",
                                                "\"Length\": \"", quoteData.m_lPiece[0].Length, "\",",
                                                "\"Width\": \"", quoteData.m_lPiece[0].Width, "\",",
                                                "\"Height\": \"", quoteData.m_lPiece[0].Height, "\"",
                        "},",

                        "\"NumberOfPieces\": \"1\",",
                        "\"PackagingType\": {",
                                                "\"Code\": \"PLT\"",
                        "},",
                        "\"FreightClass\": \"", quoteData.m_lPiece[0].FreightClass, "\"",
                    "}"));

                if(i < quoteData.m_lPiece.Length - 1)
                {
                    sb.Append(",");
                }
            }

            sb.Append("], ");

            return sb.ToString();

            #region Not used

            //sb.Append(string.Concat(

            //    "\"Commodity\": [{",

            //    //"\"Commodity\": {",
            //    "\"CommodityID\": \"1\",",
            //                            "\"Description\": \"FRS-Freight\",",
            //                "\"Weight\": {",
            //                                    "\"UnitOfMeasurement\": {",
            //                                        "\"Code\": \"LBS\"",
            //                                    "},\"Value\": \"", quoteData.m_lPiece[0].Weight, "\"",
            //                    "},",


            //            "\"Dimensions\": {",
            //                                    "\"UnitOfMeasurement\": {",
            //                                        "\"Code\": \"IN\",",
            //                                        "\"Description\": \" \"",
            //                                    "},",
            //                                    "\"Length\": \"", quoteData.m_lPiece[0].Length, "\",",
            //                                    "\"Width\": \"", quoteData.m_lPiece[0].Width, "\",",
            //                                    "\"Height\": \"", quoteData.m_lPiece[0].Height, "\"",
            //            "},",

            //            "\"NumberOfPieces\": \"1\",",
            //            "\"PackagingType\": {",
            //                                    "\"Code\": \"PLT\"",
            //            "},",
            //            "\"FreightClass\": \"", quoteData.m_lPiece[0].FreightClass, "\"",
            //        "},",

            //        "], "

            //

            //"{\"CommodityID\": \"2\",",
            //                    "\"Description\": \"FRS-Freight\",",
            //        "\"Weight\": {",
            //                            "\"UnitOfMeasurement\": {",
            //                                "\"Code\": \"LBS\"",
            //                            "},\"Value\": \"", "500", "\"",
            //            "},",


            //    "\"Dimensions\": {",
            //                            "\"UnitOfMeasurement\": {",
            //                                "\"Code\": \"IN\",",
            //                                "\"Description\": \" \"",
            //                            "},",
            //                            "\"Length\": \"", quoteData.m_lPiece[0].Length, "\",",
            //                            "\"Width\": \"", quoteData.m_lPiece[0].Width, "\",",
            //                            "\"Height\": \"", quoteData.m_lPiece[0].Height, "\"",
            //    "},",

            //    "\"NumberOfPieces\": \"1\",",
            //    "\"PackagingType\": {",
            //                            "\"Code\": \"PLT\"",
            //    "},",
            //    "\"FreightClass\": \"", quoteData.m_lPiece[0].FreightClass, "\"",
            //"}",
            #endregion
        }

        #endregion

        #region Not used
        //#region Get_items_json
        //private string Get_items_json()
        //{
        //    //CommodityID
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(string.Concat(



        //        "\"Commodity\": {",
        //        "\"CommodityID\": \"1\",",
        //                                "\"Description\": \"FRS-Freight\",",
        //                    "\"Weight\": {",
        //                                        "\"UnitOfMeasurement\": {",
        //                                            "\"Code\": \"LBS\"",
        //                                        "},\"Value\": \"", quoteData.m_lPiece[0].Weight, "\"",
        //                        "},",


        //                "\"Dimensions\": {",
        //                                        "\"UnitOfMeasurement\": {",
        //                                            "\"Code\": \"IN\",",
        //                                            "\"Description\": \" \"",
        //                                        "},",
        //                                        "\"Length\": \"", quoteData.m_lPiece[0].Length, "\",",
        //                                        "\"Width\": \"", quoteData.m_lPiece[0].Width, "\",",
        //                                        "\"Height\": \"", quoteData.m_lPiece[0].Height, "\"",
        //                "},",

        //                "\"NumberOfPieces\": \"1\",",
        //                "\"PackagingType\": {",
        //                                        "\"Code\": \"PLT\"",
        //                "},",
        //                "\"FreightClass\": \"", quoteData.m_lPiece[0].FreightClass, "\"",
        //            "},"
        //            //,

        //        //

        //        //"\"Commodity\": {",
        //        //"\"CommodityID\": \"2\",",
        //        //                        "\"Description\": \"FRS-Freight\",",
        //        //            "\"Weight\": {",
        //        //                                "\"UnitOfMeasurement\": {",
        //        //                                    "\"Code\": \"LBS\"",
        //        //                                "},\"Value\": \"400\"",
        //        //                "},",

        //        //        "\"Dimensions\": {",
        //        //                                "\"UnitOfMeasurement\": {",
        //        //                                    "\"Code\": \"IN\",",
        //        //                                    "\"Description\": \" \"",
        //        //                                "},",
        //        //                                "\"Length\": \"45\",",
        //        //                                "\"Width\": \"45\",",
        //        //                                "\"Height\": \"45\"",
        //        //        "},",

        //        //        "\"NumberOfPieces\": \"2\",",
        //        //        "\"PackagingType\": {",
        //        //                                "\"Code\": \"PLT\"",
        //        //        "},",
        //        //        "\"FreightClass\": \"60\"",
        //        //    "},"

        //            )


        //        );
        //    return sb.ToString();
        //}

        //#endregion
        #endregion

        #region Get_accessorials_json
        private string Get_accessorials_json()
        {
            if(quoteData.hasAccessorials == false)
            {
                return "";
            }

            bool has_pu = false, has_del = false;

            if(quoteData.AccessorialsObj.RESPU || quoteData.AccessorialsObj.CONPU || 
                quoteData.AccessorialsObj.LGPU)
            {
                has_pu = true;
            }

            if (quoteData.AccessorialsObj.RESDEL || quoteData.AccessorialsObj.APTDEL || quoteData.AccessorialsObj.CONDEL ||
                quoteData.AccessorialsObj.LGDEL || quoteData.AccessorialsObj.INSDEL)
            {
                has_del = true;
            }

            if(has_pu == false && has_del == false)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(string.Concat("\"ShipmentServiceOptions\" : {"));

            if (has_pu == true)
            {
                sb.Append(string.Concat("\"PickupOptions\" : {",
                    Get_pu_accessorials_json(),
                    //"\"ResidentialPickupIndicator\" : \"\",\"LiftGateRequiredIndicator\" : \"\"",
                    "}")); ;
            }

            if (has_pu == true && has_del == true)
            {
                sb.Append(",");
            }
                

            if (has_del == true)
            {
                sb.Append(string.Concat("\"DeliveryOptions\" : {",
                    Get_del_accessorials_json(),
                    //"\"ResidentialDeliveryIndicator\" : \"\",\"LiftGateRequiredIndicator\" : \"\"",
                    "}"));
            }


            sb.Append(string.Concat("},"));
            return sb.ToString();
        }

        #endregion

        #region Get_pu_accessorials_json
        private string Get_pu_accessorials_json()
        {
            bool found = false;
            StringBuilder sb = new StringBuilder();

            if(quoteData.AccessorialsObj.RESPU == true)
            {
                sb.Append("\"ResidentialPickupIndicator\" : \"\"");
                found = true;
            }

            if (quoteData.AccessorialsObj.LGPU == true)
            {
                if(found == true)
                {
                    sb.Append(",");
                }
                else
                {
                    found = true;
                }
                sb.Append("\"LiftGateRequiredIndicator\" : \"\"");
            }

            if (quoteData.AccessorialsObj.CONPU == true)
            {
                if (found == true)
                {
                    sb.Append(",");
                }
                sb.Append("\"LimitedAccessPickupIndicator\" : \"\"");
            }

            return sb.ToString();
        }

        #endregion

        #region Get_del_accessorials_json
        private string Get_del_accessorials_json()
        {
            bool found = false;
            StringBuilder sb = new StringBuilder();

            if (quoteData.AccessorialsObj.RESDEL == true)
            {
                sb.Append("\"ResidentialDeliveryIndicator\" : \"\"");
                found = true;
            }

            if (quoteData.AccessorialsObj.LGDEL == true)
            {
                if (found == true)
                {
                    sb.Append(",");
                }
                else
                {
                    found = true;
                }
                sb.Append("\"LiftGateRequiredIndicator\" : \"\"");
            }

            if (quoteData.AccessorialsObj.CONDEL == true)
            {
                if (found == true)
                {
                    sb.Append(",");
                }
                else
                {
                    found = true;
                }
                sb.Append("\"LimitedAccessDeliveryIndicator\" : \"\"");
            }

            if (quoteData.AccessorialsObj.INSDEL == true)
            {
                if (found == true)
                {
                    sb.Append(",");
                }
                else
                {
                    found = true;
                }
                sb.Append("\"InsideDeliveryIndicator\" : \"\"");
            }

            if (quoteData.AccessorialsObj.APTDEL == true)
            {
                if (found == true)
                {
                    sb.Append(",");
                }
                sb.Append("\"CallBeforeDeliveryIndicator\" : \"\"");
            }

            return sb.ToString();
        }

        #endregion

        #endregion

        //--

        #region Create_UPS_Freight_pickup_request
        public void Create_UPS_Freight_pickup_request(out string PickupRequestConfirmationNumber)
        {

            #region Date breakdown

            string month, day;
            month = DateTime.Today.Month.ToString();
            day = DateTime.Today.Day.ToString();
            if (month.Length == 1)
            {
                month = "0" + month;
            }
            if (day.Length == 1)
            {
                day = "0" + day;
            }

            #endregion

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Web_client http = new Web_client();

            //http.header_names = new string[3];
            //http.header_values = new string[3];

            //http.header_names[0] = "Username";
            //http.header_names[1] = "Password";
            //http.header_names[2] = "AccessLicenseNumber";

            string username = AppCodeConstants.ups_freight_genera_un, password = AppCodeConstants.ups_freight_genera_pwd, 
                AccessLicenseNumber = AppCodeConstants.ups_freight_genera_license_num;

            //http.url = "https://onlinetools.ups.com/rest/FreightPickup";
            http.url = "https://wwwcie.ups.com/rest/FreightPickup";
            http.method = "POST";
            http.post_data = string.Concat(
                "{ \"Security\": { \"UsernameToken\": { \"Username\": \"", username, "\", \"Password\": \"", password, "\" },",
                "\"UPSServiceAccessToken\": { \"AccessLicenseNumber\": \"", AccessLicenseNumber, "\" } }, ",
                "\"FreightPickupRequest\": ",
                "{ \"Request\": { \"TransactionReference\": { \"CustomerContext\": \"\" } }, ",
                "\"AdditionalComments\": \"AdditionalComments\", \"DestinationPostalCode\": \"98144\", ",
                "\"DestinationCountryCode\": \"US\", ",

                "\"Requester\": ",
                "{ \"AttentionName\": \"Mr. ABC\", \"EMailAddress\": \"", AppCodeConstants.Alex_email, "\", \"Name\": \"ABC Associates\", ",
                "\"Phone\": { \"Number\": \"123456789\" } }, ",

                "\"ShipFrom\": { \"AttentionName\": \"Mr. ABC\", \"Name\": \"Mr. ABC\", ",
                "\"Address\": { \"AddressLine\": \"1712 Shadowood PKWY SE\", \"City\": \"Atlanta\", ",
                "\"StateProvinceCode\": \"GA\", \"PostalCode\": \"30303\", ",
                "\"CountryCode\": \"US\" }, \"Phone\": { \"Number\": \"123456789\" } }, ",

                "\"ShipmentDetail\": { \"HazMatIndicator\": \"\", ",
                "\"PackagingType\": { \"Code\": \"PLT\", \"Description\": \"Pallet/Skid\" }, ", // 06 is code for Pallet
                "\"NumberOfPieces\": \"1\", \"DescriptionOfCommodity\":\"Description\", ",
                "\"Weight\": {\"UnitOfMeasurement\": { \"Code\": \"LBS\", \"Description\": \"Pounds\" }, ",
                "\"Value\": \"500\" } }, ",
                "\"PickupDate\": \"20191202\", \"EarliestTimeReady\": \"0800\", ",
                "\"LatestTimeReady\": \"1800\" } }");
            /*
             Appendix I- Ground Freight Handling Unit Codes
Code Valid for Handling Valid for Handling
Unit One Unit Two
Description
SKD X SKID
CBY X CARBOY
PLT X PALLET
TOT X TOTES
LOO X LOOSE
OTH X OTHER
             */

            http.accept = "application/json";
            http.content_type = "application/json";

            string doc = http.Make_http_request();

            #region Parse result

            string[] tokens = new string[4];
            tokens[0] = "PickupRequestConfirmationNumber";
            tokens[1] = ":";
            tokens[2] = "\"";
            tokens[3] = "\"";

            PickupRequestConfirmationNumber = HelperFuncs.scrapeFromPage(tokens, doc);

            #endregion

        }

        #endregion

        #region Cancel_UPS_Freight_pickup_request
        public void Cancel_UPS_Freight_pickup_request(ref string PickupRequestConfirmationNumber)
        {
            string month, day;
            month = DateTime.Today.Month.ToString();
            day = DateTime.Today.Day.ToString();
            if (month.Length == 1)
            {
                month = "0" + month;
            }
            if (day.Length == 1)
            {
                day = "0" + day;
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Web_client http = new Web_client();

            string username = AppCodeConstants.ups_freight_genera_un, password = AppCodeConstants.ups_freight_genera_pwd,
                 AccessLicenseNumber = AppCodeConstants.ups_freight_genera_license_num;

            //http.url = "https://onlinetools.ups.com/rest/FreightPickup";
            http.url = "https://wwwcie.ups.com/rest/FreightPickup";
            http.method = "POST";
            http.post_data = string.Concat(
                "{ \"Security\": { \"UsernameToken\": { \"Username\": \"", username, "\", \"Password\": \"", password, "\" }, ",
                "\"UPSServiceAccessToken\": { \"AccessLicenseNumber\": \"", AccessLicenseNumber, "\" } }, ",
                "\"LumberJack\": \"\", \"pNg911jan06\": \"\", ",
                "\"FreightCancelPickupRequest\": { \"Request\": { \"RequestOption\": \"\", ",
                "\"TransactionReference\": { \"CustomerContext\": \"\" } }, ",
                "\"PickupRequestConfirmationNumber\": \"", PickupRequestConfirmationNumber, "\" } }");

            http.accept = "application/json";
            http.content_type = "application/json";

            string doc = http.Make_http_request();

            #region Parse result

            string[] tokens = new string[5];
            tokens[0] = "FreightCancelStatus";
            tokens[1] = "Code";
            tokens[2] = ":";
            tokens[3] = "\"";
            tokens[4] = "\"";

            string Code = HelperFuncs.scrapeFromPage(tokens, doc);

            //tokens[0] = "TimeInTransit";
            tokens[1] = "Description";

            string Description = HelperFuncs.scrapeFromPage(tokens, doc);

            #endregion

        }

        #endregion

    }
}