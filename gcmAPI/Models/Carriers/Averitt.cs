#region Using

using gcmAPI.Models.LTL;
using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

#endregion

namespace gcmAPI.Models.Carriers
{
    public class Averitt
    {
        QuoteData quoteData;

        #region Constructor
        // Constructor
        public Averitt(ref QuoteData quoteData)
        {

            this.quoteData = quoteData;

        }

        #endregion

        #region Get_rates

        public void Get_rates(string enc_password, out GCMRateQuote Averitt_Quote_Genera)
        {
            try
            {
                string[] header_names = new string[1];
                string[] header_values = new string[1];

                header_names[0] = "SOAPAction";

                header_values[0] = "\"\"";

                
                //Host: webservices.averittexpress.com
                
                string password = enc_password;

                #region Date

                string day = DateTime.Today.Day.ToString(), month = DateTime.Today.Month.ToString(), year = DateTime.Today.Year.ToString();

                TimeSpan end = new TimeSpan(17, 0, 0);
                TimeSpan now = DateTime.Now.TimeOfDay;

                if (now > end)
                {
                    //DB.LogGenera("averitt", "after five", now.ToString());
                    // After 5 PM, set pickup date to tomorrow
                    day = DateTime.Today.AddDays(1).Day.ToString();
                    month = DateTime.Today.AddDays(1).Month.ToString();
                    year = DateTime.Today.AddDays(1).Year.ToString();
                }
                else
                {
                    //DB.LogGenera("averitt", "before five", now.ToString());
                }

                #endregion

                #region Items

                StringBuilder items = new StringBuilder();
                for(byte i=0; i<quoteData.m_lPiece.Length; i++)
                {
                    items.Append(
                        string.Concat("<ltl:Items>",
                     "<ltl:ShipmentClass>", quoteData.m_lPiece[i].FreightClass, "</ltl:ShipmentClass>",
                     "<ltl:ShipmentWeight>", (int)quoteData.m_lPiece[i].Weight, "</ltl:ShipmentWeight>",
                     "<ltl:Units></ltl:Units>",
                     "<ltl:Description></ltl:Description>",
                     "<ltl:NmfcNumber></ltl:NmfcNumber>",
                     "<ltl:NmfcSubNumber></ltl:NmfcSubNumber>",
                  "</ltl:Items>"));
                }

                #endregion

                #region Accessorials
                
                string aptdel = "false", condel = "false", tradedel = "false", insdel = "false", liftgate = "false",
                    resdel = "false", non_commercial = "false", hazmat="false";

                if (quoteData.AccessorialsObj.APTDEL == true)
                    aptdel = "true";

                if (quoteData.AccessorialsObj.CONDEL == true)
                    condel = "true";

                if (quoteData.AccessorialsObj.TRADEDEL == true)
                    tradedel = "true";

                if (quoteData.AccessorialsObj.INSDEL == true)
                    insdel = "true";

                if (quoteData.AccessorialsObj.LGDEL == true || quoteData.AccessorialsObj.LGPU == true)
                    liftgate = "true";

                if (quoteData.AccessorialsObj.RESDEL == true)
                    resdel = "true";

                if (quoteData.AccessorialsObj.CONPU == true)
                    non_commercial = "true";

                if (quoteData.isHazmat == true)
                    hazmat = "true";

                #endregion

                string URL = "https://webservices.averittexpress.com/LTLRateQuoteService";

                string XMLRequest =
                    string.Concat("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ltl=\"https://webservices.averittexpress.com/LTLRateQuoteService\">",
      "<soapenv:Header xmlns:soapenc=\"http://schemas.xmlsoap.org/soap/encoding/\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">",
         "<ns:authnHeader soapenv:mustUnderstand=\"0\" xmlns:ns=\"http://webservices.averittexpress.com/authn\" >",
            "<Username>", AppCodeConstants.averitt_genera_un, "</Username>",
            "<Password>", AppCodeConstants.averitt_genera_pwd, "</Password>",
         "</ns:authnHeader>",
      "</soapenv:Header>",
      "<soapenv:Body>",
         "<ltl:getLTLRate>",
           
            "<arg0>",

               "<ltl:AccountNumber>", AppCodeConstants.averitt_genera_acct, "</ltl:AccountNumber>",
              
               "<ltl:OriginCity>Coppell</ltl:OriginCity>",            
               "<ltl:OriginState>TX</ltl:OriginState>",             
               "<ltl:OriginZip>75019</ltl:OriginZip>",

               "<ltl:DestinationCity>", quoteData.destCity, "</ltl:DestinationCity>",
               "<ltl:DestinationState>", quoteData.destState, "</ltl:DestinationState> ",
               "<ltl:DestinationZip>", quoteData.destZip, "</ltl:DestinationZip>",
               "<ltl:ShipDate>", month, "/", day, "/", year, "</ltl:ShipDate>",
               "<ltl:CustomerType>Shipper</ltl:CustomerType>",
               "<ltl:PaymentType>Prepaid</ltl:PaymentType>",

               "<ltl:RequestType></ltl:RequestType>",

               "<ltl:ShipmentInfo>",
                  "<!--Optional:-->",
                  "<ltl:NumPieces></ltl:NumPieces>",
                  "<!--Optional:-->",
                  "<ltl:NumHandlingUnits></ltl:NumHandlingUnits>",
                  "<!--Optional:-->",
                  "<ltl:CubicFeet></ltl:CubicFeet>",
                  "<!--Zero or more repetitions:--> ",

                  items,

                  "<!--Optional:--> ",
                  "<ltl:TotalItem></ltl:TotalItem>",
                  "<!--Optional:-->",
                  "<ltl:TotalWeight></ltl:TotalWeight>",
                  "<!--Optional:-->",

                  "<ltl:OverLength></ltl:OverLength>",
                  "<!--Zero or more repetitions:-->",
                  "<ltl:Accessorial>",

                     "<ltl:ArrivalNotify>", aptdel, "</ltl:ArrivalNotify>",
                     "<ltl:ConventionCenterDelivery>", tradedel, "</ltl:ConventionCenterDelivery>",
                     "<ltl:ConstructionSiteDelivery>", condel, "</ltl:ConstructionSiteDelivery>",
                     "<ltl:InsideDelivery>", insdel, "</ltl:InsideDelivery>",
                     "<ltl:Liftgate>", liftgate, "</ltl:Liftgate>",
                     "<ltl:Hazmat>", hazmat, "</ltl:Hazmat>",
                     "<ltl:ResidentialDelivery>", resdel, "</ltl:ResidentialDelivery>",
                     "<ltl:Non-CommercialPickupDelivery>", non_commercial, "</ltl:Non-CommercialPickupDelivery>",

                     "<ltl:StandardLTLGuarantee>false</ltl:StandardLTLGuarantee>",
                     "<ltl:SecurityInspection>false</ltl:SecurityInspection>",
                  "</ltl:Accessorial>",
               "</ltl:ShipmentInfo>",
            "</arg0>",
         "</ltl:getLTLRate>",
      "</soapenv:Body>",
    "</soapenv:Envelope>");

                //DB.LogGenera("averitt.XMLRequest", "averitt.XMLRequest", XMLRequest);

                Web_client http = new Web_client();

                http.url = URL;
                http.header_names = header_names;
                http.header_values = header_values;
                http.method = "POST";
                http.content_type = "text/xml; charset=utf-8";
                http.post_data = XMLRequest;
                string doc = http.Make_http_request();

                //DB.LogGenera("averitt.response", "averitt.response", doc);

                #region Parse result
                /*
                 * <ns2:RateQuoteNumber>38376982</ns2:RateQuoteNumber>
                 * <ns2:EstimatedServiceDays>2</ns2:EstimatedServiceDays>
                 <ns2:DiscountAmount>$1507.24</ns2:DiscountAmount>
            <ns2:DiscountFactor>86.8</ns2:DiscountFactor>
            <ns2:RateBase>775</ns2:RateBase>
            <ns2:Tariff>ARATE     </ns2:Tariff>
            <ns2:FuelCharge>$40.52</ns2:FuelCharge>
            <ns2:GrossMinimum>98.00</ns2:GrossMinimum>
            <ns2:NetTotal>$269.73</ns2:NetTotal>
            <ns2:TotalCharge>$1736.45</ns2:TotalCharge>
            <ns2:TotalFreightCharge>$1736.45</ns2:TotalFreightCharge>
                 */

                string[] tokens = new string[3];
                tokens[0] = "NetTotal";
                tokens[1] = ">";
                tokens[2] = "<";

                string NetTotal_str = HelperFuncs.scrapeFromPage(tokens, doc).Replace("$", "");
                double NetTotal = 0.0;

                int EstimatedServiceDays = 10;

                Averitt_Quote_Genera = new GCMRateQuote();

                if (double.TryParse(NetTotal_str, out NetTotal))
                {
                    Averitt_Quote_Genera.TotalPrice = NetTotal;

                    tokens[0] = "EstimatedServiceDays";
                    string EstimatedServiceDays_str = HelperFuncs.scrapeFromPage(tokens, doc);
                    if (!int.TryParse(EstimatedServiceDays_str, out EstimatedServiceDays))
                    {
                        Averitt_Quote_Genera.DeliveryDays = 5;
                    }
                    else
                    {
                        Averitt_Quote_Genera.DeliveryDays = EstimatedServiceDays;
                    }

                    tokens[0] = "RateQuoteNumber";
                    string RateQuoteNumber = HelperFuncs.scrapeFromPage(tokens, doc);

                    Averitt_Quote_Genera.DisplayName = "Averitt - Genera";
                    Averitt_Quote_Genera.CarrierKey = "UPS";
                    Averitt_Quote_Genera.BookingKey = "#1#";
                    Averitt_Quote_Genera.Scac = "AVRT";
                    Averitt_Quote_Genera.CarrierQuoteID = RateQuoteNumber;
                }

                #endregion


            }
            catch (Exception e)
            {
                Averitt_Quote_Genera = new GCMRateQuote();
                DB.LogGenera("Averitt", "e", e.ToString());
            }
        }

        #endregion
    }
}