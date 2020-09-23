using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http.Formatting;

namespace gcmAPI.Models.UPS_Package
{
    public class Rater
    {
        #region getPackageAPI_XML_Rate

        public static string getPackageAPI_XML_Rate(ref FormDataCollection form)
        {

            #region Variables

            string url = "", referrer, contentType, accept, method, doc = "";

            //url = "https://wwwcie.ups.com/ups.app/xml/ShipAccept";
            //url = "https://onlinetools.ups.com/ups.app/xml/ShipAccept";

            //url = "https://wwwcie.ups.com/ups.app/xml/Rate";
            url = "https://onlinetools.ups.com/ups.app/xml/Rate";

            referrer = "";
            //contentType = "application/xml";
            contentType = "text/xml; charset=utf-8";
            method = "POST";
            //accept = "text/xml";
            accept = "*/*";

            #endregion

            #region Post Data

  //          string data = string.Concat("<?xml version=\"1.0\"?>",
  //"<AccessRequest xml:lang=\"en-US\">",
  //    "<AccessLicenseNumber></AccessLicenseNumber>",
  //    "<UserId></UserId>",
  //    "<Password></Password>",
  //"</AccessRequest>",
  //"<?xml version=\"1.0\"?>",
  //"<RatingServiceSelectionRequest xml:lang=\"en-US\">",
  //  "<Request>",
  //    "<TransactionReference>",
  //      "<CustomerContext>Rating and Service</CustomerContext>",
  //      "<XpciVersion>1.0</XpciVersion>",
  //    "</TransactionReference>",
  //    "<RequestAction>Rate</RequestAction>",
  //    "<RequestOption>Rate</RequestOption>",
  //    "<ShipmentRatingOptions><NegotiatedRatesIndicator /></ShipmentRatingOptions>",
  //  "</Request>",
  //              //"<PickupType>",
  //              //"<Code>07</Code>",
  //              //"<Description>Rate</Description>",
  //              //"</PickupType>",
  //  "<Shipment>",
  //        "<Description>Rate Description</Description>",
  //    "<Shipper>",
  //      "<Name>Name</Name>",
  //      "<PhoneNumber>1234567890</PhoneNumber>",
  //      "<ShipperNumber></ShipperNumber>",
  //      "<Address>",
  //        "<AddressLine1>Address Line</AddressLine1>",
  //        "<City>SANTA FE SPRINGS</City>",
  //        "<StateProvinceCode>CA</StateProvinceCode>",
  //        "<PostalCode>90670</PostalCode>",
  //        "<CountryCode>US</CountryCode>",
  //      "</Address>",
  //    "</Shipper>",
  //    "<ShipTo>",
  //      "<CompanyName>Company Name</CompanyName>",
  //      "<PhoneNumber>1234567890</PhoneNumber>",
  //      "<Address>",
  //        "<AddressLine1>Address Line</AddressLine1>",
  //        "<City>HIGH POINT</City>",
  //        "<StateProvinceCode>NC</StateProvinceCode>",
  //        "<PostalCode>27260</PostalCode>",
  //        "<CountryCode>US</CountryCode>",
  //      "</Address>",
  //    "</ShipTo>",

  //    "<ShipFrom>",
  //      "<CompanyName>Company Name</CompanyName>",
  //      "<AttentionName>Attention Name</AttentionName>",
  //      "<PhoneNumber>1234567890</PhoneNumber>",
  //      "<FaxNumber>1234567890</FaxNumber>",
  //      "<Address>",
  //        "<AddressLine1>Address Line</AddressLine1>",
  //        "<City>SANTA FE SPRINGS</City>",
  //        "<StateProvinceCode>CA</StateProvinceCode>",
  //        "<PostalCode>90670</PostalCode>",
  //        "<CountryCode>US</CountryCode>",
  //      "</Address>",
  //    "</ShipFrom>",
  //    "<Service>",
  //            "<Code>03</Code>",
            //    "</Service>",
  //    "<PaymentInformation>",
  //            "<Prepaid>",
  //                "<BillShipper>",
  //                    "<AccountNumber></AccountNumber>",
  //                "</BillShipper>",
  //            "</Prepaid>",
  //    "</PaymentInformation>",
  //    "<Package>",
  //            "<PackagingType>",
  //                "<Code>02</Code>",
  //                "<Description>Customer Supplied</Description>",
  //            "</PackagingType>",
  //            "<Description>Rate</Description>",
  //            "<PackageWeight>",
  //                "<UnitOfMeasurement>",
  //                  "<Code>LBS</Code>",
  //                "</UnitOfMeasurement>",
  //                "<Weight>65</Weight>",
  //            "</PackageWeight>",
  //            "<Dimensions>",
  //                "<UnitOfMeasurement>",
  //                  "<Code>IN</Code>",
  //                "</UnitOfMeasurement>",
  //                "<Length>29</Length>",
  //                "<Width>29</Width>",
  //                "<Height>42</Height>",
  //            "</Dimensions>",
  //    "</Package>",
  //              //"<ShipmentServiceOptions>",
  //              //  "<OnCallAir>",
  //              //    "<Schedule>", 
  //              //        "<PickupDay>02</PickupDay>",
  //              //        "<Method>02</Method>",
  //              //    "</Schedule>",
  //              //  "</OnCallAir>",
  //              //"</ShipmentServiceOptions>",

  //    "<RateInformation><NegotiatedRatesIndicator /></RateInformation>",

  //  "</Shipment>",

  //"</RatingServiceSelectionRequest>");

            #endregion

            HelperFuncs.writeToSiteErrors("getPackageAPI_XML_Rate request xml", form.Get("data"));

            doc = (string)HelperFuncs.generic_http_request("string", null, url, referrer, contentType, accept, method,
               form.Get("data"), false);

            HelperFuncs.writeToSiteErrors("getPackageAPI_XML_Rate response xml", doc);

            string[] tokens = new string[4];
            tokens[0] = "<NegotiatedRates>";
            tokens[1] = "<MonetaryValue>";
            tokens[2] = ">";
            tokens[3] = "<";
            string strNegotiatedRate = HelperFuncs.scrapeFromPage(tokens, doc);

            HelperFuncs.writeToSiteErrors("getPackageAPI_XML_Rate strNegotiatedRate", strNegotiatedRate);

            double negotiatedRate = 0.0;
            if (!double.TryParse(strNegotiatedRate, out negotiatedRate))
            {
                HelperFuncs.writeToSiteErrors("getPackageAPI_XML_Rate", string.Concat("negotiated rate not parsed to double, value was: ", strNegotiatedRate));
                return "-1";
            }



            return strNegotiatedRate;


        }

        #endregion

        #region Overloaded getPackageAPI_XML_Rate

  //      public static string getPackageAPI_XML_Rate(ref string xml)
  //      {

  //          #region Variables

  //          string url = "", referrer, contentType, accept, method, doc = "";

  //          //url = "https://wwwcie.ups.com/ups.app/xml/ShipAccept";
  //          //url = "https://onlinetools.ups.com/ups.app/xml/ShipAccept";

  //          //url = "https://wwwcie.ups.com/ups.app/xml/Rate";
  //          url = "https://onlinetools.ups.com/ups.app/xml/Rate";

  //          referrer = "";
  //          //contentType = "application/xml";
  //          contentType = "text/xml; charset=utf-8";
  //          method = "POST";
  //          //accept = "text/xml";
  //          accept = "*/*";

  //          #endregion

  //          #region Post Data

  //          string data = string.Concat("<?xml version=\"1.0\"?>",
  //"<AccessRequest xml:lang=\"en-US\">",
  //    "<AccessLicenseNumber></AccessLicenseNumber>",
  //    "<UserId></UserId>",
  //    "<Password></Password>",
  //"</AccessRequest>",
  //"<?xml version=\"1.0\"?>",
  //"<RatingServiceSelectionRequest xml:lang=\"en-US\">",
  //  "<Request>",
  //    "<TransactionReference>",
  //      "<CustomerContext>Rating and Service</CustomerContext>",
  //      "<XpciVersion>1.0</XpciVersion>",
  //    "</TransactionReference>",
  //    "<RequestAction>Rate</RequestAction>",
  //    "<RequestOption>Rate</RequestOption>",
  //    "<ShipmentRatingOptions><NegotiatedRatesIndicator /></ShipmentRatingOptions>",
  //  "</Request>",
  //              //"<PickupType>",
  //              //"<Code>07</Code>",
  //              //"<Description>Rate</Description>",
  //              //"</PickupType>",
  //  "<Shipment>",
  //        "<Description>Rate Description</Description>",
  //    "<Shipper>",
  //      "<Name>Name</Name>",
  //      "<PhoneNumber>1234567890</PhoneNumber>",
  //      "<ShipperNumber></ShipperNumber>",
  //      "<Address>",
  //        "<AddressLine1>Address Line</AddressLine1>",
  //        "<City>SANTA FE SPRINGS</City>",
  //        "<StateProvinceCode>CA</StateProvinceCode>",
  //        "<PostalCode>90670</PostalCode>",
  //        "<CountryCode>US</CountryCode>",
  //      "</Address>",
  //    "</Shipper>",
  //    "<ShipTo>",
  //      "<CompanyName>Company Name</CompanyName>",
  //      "<PhoneNumber>1234567890</PhoneNumber>",
  //      "<Address>",
  //        "<AddressLine1>Address Line</AddressLine1>",
  //        "<City>HIGH POINT</City>",
  //        "<StateProvinceCode>NC</StateProvinceCode>",
  //        "<PostalCode>27260</PostalCode>",
  //        "<CountryCode>US</CountryCode>",
  //      "</Address>",
  //    "</ShipTo>",

  //    "<ShipFrom>",
  //      "<CompanyName>Company Name</CompanyName>",
  //      "<AttentionName>Attention Name</AttentionName>",
  //      "<PhoneNumber>1234567890</PhoneNumber>",
  //      "<FaxNumber>1234567890</FaxNumber>",
  //      "<Address>",
  //        "<AddressLine1>Address Line</AddressLine1>",
  //        "<City>SANTA FE SPRINGS</City>",
  //        "<StateProvinceCode>CA</StateProvinceCode>",
  //        "<PostalCode>90670</PostalCode>",
  //        "<CountryCode>US</CountryCode>",
  //      "</Address>",
  //    "</ShipFrom>",
  //    "<Service>",
  //            "<Code>03</Code>",
  //    "</Service>",
  //    "<PaymentInformation>",
  //            "<Prepaid>",
  //                "<BillShipper>",
  //                    "<AccountNumber></AccountNumber>",
  //                "</BillShipper>",
  //            "</Prepaid>",
  //    "</PaymentInformation>",
  //    "<Package>",
  //            "<PackagingType>",
  //                "<Code>02</Code>",
  //                "<Description>Customer Supplied</Description>",
  //            "</PackagingType>",
  //            "<Description>Rate</Description>",
  //            "<PackageWeight>",
  //                "<UnitOfMeasurement>",
  //                  "<Code>LBS</Code>",
  //                "</UnitOfMeasurement>",
  //                "<Weight>65</Weight>",
  //            "</PackageWeight>",
  //            "<Dimensions>",
  //                "<UnitOfMeasurement>",
  //                  "<Code>IN</Code>",
  //                "</UnitOfMeasurement>",
  //                "<Length>29</Length>",
  //                "<Width>29</Width>",
  //                "<Height>42</Height>",
  //            "</Dimensions>",
  //    "</Package>",
  //              //"<ShipmentServiceOptions>",
  //              //  "<OnCallAir>",
  //              //    "<Schedule>", 
  //              //        "<PickupDay>02</PickupDay>",
  //              //        "<Method>02</Method>",
  //              //    "</Schedule>",
  //              //  "</OnCallAir>",
  //              //"</ShipmentServiceOptions>",

  //    "<RateInformation><NegotiatedRatesIndicator /></RateInformation>",

  //  "</Shipment>",

  //"</RatingServiceSelectionRequest>");

  //          #endregion

  //          data = xml;

  //          HelperFuncs.writeToSiteErrors("getPackageAPI_XML_Rate request xml", data);

  //          doc = (string)HelperFuncs.generic_http_request("string", null, url, referrer, contentType, accept, method,
  //             data, false);

  //          HelperFuncs.writeToSiteErrors("getPackageAPI_XML_Rate response xml", doc);

        //          //string[] tokens = new string[4];
        //          //tokens[0] = "<NegotiatedRates>";
        //          //tokens[1] = "<MonetaryValue>";
        //          //tokens[2] = ">";
        //          //tokens[3] = "<";
        //          //string strNegotiatedRate = HelperFuncs.scrapeFromPage(tokens, doc);

        //          //HelperFuncs.writeToSiteErrors("getPackageAPI_XML_Rate strNegotiatedRate", strNegotiatedRate);

        //          //double negotiatedRate = 0.0;
        //          //if (!double.TryParse(strNegotiatedRate, out negotiatedRate))
        //          //{
        //          //    HelperFuncs.writeToSiteErrors("getPackageAPI_XML_Rate", string.Concat("negotiated rate not parsed to double, value was: ", strNegotiatedRate));
        //          //}

        //          return doc;


        //      }

        #endregion
    }
}