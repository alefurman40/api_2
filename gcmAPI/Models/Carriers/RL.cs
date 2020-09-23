#region Using

using System;
using System.Collections.Generic;
using gcmAPI.Models.LTL;
using System.Text;
using System.Net;
using gcmAPI.Models.Utilities;

#endregion

namespace gcmAPI.Models.Carriers
{
    public class RL
    {
        
        string strRLDisplay = "R&L Carrier", API_KeyRL_Durachem;

        CarrierAcctInfo acctInfo;
        QuoteData quoteData;

        // Constructor
        public RL(CarrierAcctInfo acctInfo, ref QuoteData quoteData, ref string API_KeyRL_Durachem)
        {
            this.acctInfo = acctInfo;
            this.quoteData = quoteData;
            this.API_KeyRL_Durachem = API_KeyRL_Durachem;
        }

        #region GetRateFromRL

        public GCMRateQuote GetRateFromRL(ref GCMRateQuote rl_quote_guaranteed)
        {
            GCMRateQuote gcmRateQuote = new GCMRateQuote();

            //GetResultObjectFromConWayFreight(ref gcmRateQuote);
            GetResultObjectFromRAndL(ref gcmRateQuote, ref rl_quote_guaranteed);

            return gcmRateQuote;
        }

        #endregion

        #region getXML_RateQuote_RL

        // getRateQuote_RL
        private string[] getXML_RateQuote_RL(string[] accessorials, string multItems, ref string res_doc)
        {
            #region Variables

            string url = "", referrer, contentType, accept, method, doc = "", data = "";

            url = "http://api.rlcarriers.com/1.0.2/RateQuoteService.asmx";
            referrer = "";
            contentType = "text/xml; charset=utf-8";
            method = "POST";
            accept = "text/xml";
            //SOAPAction: "http://www.rlcarriers.com/GetRateQuote"
            #endregion

            string orig_country = "USA", dest_country = "USA";
            if(quoteData.origCountry=="CANADA")
            {
                orig_country = "CAN";
            }
            if (quoteData.destCountry == "CANADA")
            {
                dest_country = "CAN";
            }

            StringBuilder items = new StringBuilder();

            for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
            {
                items.Append("<Item>");
                if (quoteData.m_lPiece[i].FreightClass.Contains("."))
                {

                    items.Append(string.Concat("<Class>", quoteData.m_lPiece[i].FreightClass, "</Class>"));
                }
                else
                {
                    items.Append(string.Concat("<Class>", quoteData.m_lPiece[i].FreightClass, ".0", "</Class>"));
                }


                items.Append(string.Concat("<Weight>", quoteData.m_lPiece[i].Weight, "</Weight><Width>", quoteData.m_lPiece[i].Width, "</Width><Height>", quoteData.m_lPiece[i].Height,
                    "</Height><Length>", quoteData.m_lPiece[i].Length, "</Length>"));

                items.Append("</Item>");
            }

            StringBuilder sbAccessorials = new StringBuilder();

            #region Accessorials

            byte countAcc = 0;

            if (quoteData.AccessorialsObj.RESPU || quoteData.AccessorialsObj.CONPU)
            {
                sbAccessorials.Append("<Accessorial>ResidentialPickup</Accessorial>");
                ////accessorials.Add("RESPU");
                //gcmAPI.RL_CarriersAPI.Accessorial acc = new gcmAPI.RL_CarriersAPI.Accessorial();
                //acc = gcmAPI.RL_CarriersAPI.Accessorial.ResidentialPickup;
                //accessorials2[countAcc] = acc;
                //countAcc++;
            }

            if (quoteData.AccessorialsObj.RESDEL || quoteData.AccessorialsObj.CONDEL)
            {
                sbAccessorials.Append("<Accessorial>ResidentialDelivery</Accessorial>");
                ////accessorials2[countAcc] = "ResidentialDelivery";
                //gcmAPI.RL_CarriersAPI.Accessorial acc = new gcmAPI.RL_CarriersAPI.Accessorial();
                //acc = gcmAPI.RL_CarriersAPI.Accessorial.ResidentialDelivery;
                //accessorials2[countAcc] = acc;
                //countAcc++;
            }
            //accessorials.Add("RESDEL");

            if (quoteData.AccessorialsObj.INSDEL)
            {
                sbAccessorials.Append("<Accessorial>InsideDelivery</Accessorial>");
                ////accessorials2[countAcc] = "InsideDelivery";
                //gcmAPI.RL_CarriersAPI.Accessorial acc = new gcmAPI.RL_CarriersAPI.Accessorial();
                //acc = gcmAPI.RL_CarriersAPI.Accessorial.InsideDelivery;
                //accessorials2[countAcc] = acc;
                //countAcc++;
            }
            //accessorials.Add("INSDEL");

            if (quoteData.AccessorialsObj.LGPU)
            {
                sbAccessorials.Append("<Accessorial>OriginLiftgate</Accessorial>");
                //gcmAPI.RL_CarriersAPI.Accessorial acc = new gcmAPI.RL_CarriersAPI.Accessorial();
                //acc = gcmAPI.RL_CarriersAPI.Accessorial.OriginLiftgate;
                //accessorials2[countAcc] = acc;
                ////accessorials2[countAcc] = "OriginLiftgate";
                //countAcc++;
            }
            //accessorials.Add("LGPU");

            if (quoteData.AccessorialsObj.LGDEL)
            {
                sbAccessorials.Append("<Accessorial>DestinationLiftgate</Accessorial>");
                ////accessorials.Add("LGDEL");
                ////accessorials2[countAcc] = "DestinationLiftgate";
                //gcmAPI.RL_CarriersAPI.Accessorial acc = new gcmAPI.RL_CarriersAPI.Accessorial();
                //acc = gcmAPI.RL_CarriersAPI.Accessorial.DestinationLiftgate;
                //accessorials2[countAcc] = acc;
                //countAcc++;
            }

            if (quoteData.AccessorialsObj.APTDEL)
            {
                sbAccessorials.Append("<Accessorial>DeliveryNotification</Accessorial>");
                ////accessorials2[countAcc] = "DeliveryNotification";
                //gcmAPI.RL_CarriersAPI.Accessorial acc = new gcmAPI.RL_CarriersAPI.Accessorial();
                //acc = gcmAPI.RL_CarriersAPI.Accessorial.DeliveryNotification;
                //accessorials2[countAcc] = acc;
                //countAcc++;
            }
            //accessorials.Add("APT");

            if (quoteData.isHazmat.Equals(true))
            {
                sbAccessorials.Append("<Accessorial>Hazmat</Accessorial>");

            }

            #endregion

            #region Post Data

            data = string.Concat("<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">",

    "<soap:Body><GetRateQuote xmlns=\"http://www.rlcarriers.com/\"><APIKey>", API_KeyRL_Durachem, "</APIKey><request><CustomerData />",

    "<QuoteType>Domestic</QuoteType><CODAmount>0</CODAmount>",

            "<Origin><City>", quoteData.origCity, "</City><StateOrProvince>", quoteData.origState, "</StateOrProvince><ZipOrPostalCode>", quoteData.origZip, "</ZipOrPostalCode>",
            "<CountryCode>", orig_country, "</CountryCode></Origin>",


    "<Destination><City>", quoteData.destCity, "</City><StateOrProvince>", quoteData.destState, "</StateOrProvince><ZipOrPostalCode>", quoteData.destZip, "</ZipOrPostalCode>",
            "<CountryCode>", dest_country, "</CountryCode></Destination>",

            "<Items>",

            items,
    //        "<Item><Class>50.0</Class>",
    //"<Weight>500</Weight><Width>45</Width><Height>45</Height><Length>45</Length></Item>",

    "</Items>",

            "<DeclaredValue>0</DeclaredValue>",
            "<Accessorials>", sbAccessorials,
            //"<Accessorial>OriginLiftgate</Accessorial>",
            "</Accessorials>",

    "<OverDimensionPcs>0</OverDimensionPcs></request></GetRateQuote></soap:Body></soap:Envelope>");

            #endregion

            DB.Log("RL data " + acctInfo.password, data);

            string[] headerNames = new string[1];
            headerNames[0] = "SOAPAction";
            string[] headerValues = new string[1];
            headerValues[0] = "http://www.rlcarriers.com/GetRateQuote";

            doc = (string)HelperFuncs.generic_http_request_addHeaders("string", null, url, referrer, contentType, accept, method, data, false, headerNames, headerValues);
            res_doc = doc;

            string[] tokens = new string[4];
            tokens[0] = ">Net Charge<";
            tokens[1] = "<Amount>";
            tokens[2] = "$";
            tokens[3] = "<";

            string cost = HelperFuncs.scrapeFromPage(tokens, doc);

            DB.Log("RL xml cost", cost);

            //DB.Log("RL costAmount ", costAmount);

            int transitTime = getSOAP_TransitTimes_RL();

            string[] returnInfo = new string[5];
            returnInfo[0] = "";
            returnInfo[1] = "";
            returnInfo[2] = "";
            //returnInfo[3] = cost.Replace("$", "");
            returnInfo[3] = cost;
            returnInfo[4] = transitTime.ToString();

            return returnInfo;

        }

        #endregion

        #region getSOAP_RateQuote_RL

        // getRateQuote_RL
        private string[] getSOAP_RateQuote_RL(string[] accessorials, string multItems)
        {
            gcmAPI.RL_CarriersAPI.RateQuoteRequest request = new gcmAPI.RL_CarriersAPI.RateQuoteRequest();

            gcmAPI.RL_CarriersAPI.Item[] items = new gcmAPI.RL_CarriersAPI.Item[quoteData.m_lPiece.Length];

            for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
            {

                items[i] = new gcmAPI.RL_CarriersAPI.Item();

                if (quoteData.m_lPiece[i].FreightClass.Contains("."))
                {
                    items[i].Class = quoteData.m_lPiece[i].FreightClass;
                }
                else
                {
                    items[i].Class = string.Concat(quoteData.m_lPiece[i].FreightClass, ".0");
                }

                items[i].Weight = (float)quoteData.m_lPiece[i].Weight;
                items[i].Length = (float)quoteData.m_lPiece[i].Length;
                items[i].Width = (float)quoteData.m_lPiece[i].Width;
                items[i].Height = (float)quoteData.m_lPiece[i].Height;

            }

            request.Items = items;

            // InsideDelivery or ResidentialPickup or ResidentialDelivery or OriginLiftgate or DestinationLiftgate or DeliveryNotification or 
            // Freezable or Hazmat or InsidePickup or LimitedAccessPickup or DockPickup or DockDelivery or AirportPickup or AirportDelivery or 
            // LimitedAccessDelivery or quoteData.totalCube or KeepFromFreezing or DoorToDoor or COD or FZ or OverDimension

            gcmAPI.RL_CarriersAPI.Accessorial[] accessorials2 = new gcmAPI.RL_CarriersAPI.Accessorial[accessorials.Length];
            #region Accessorials

            byte countAcc = 0;

            if (quoteData.AccessorialsObj.RESPU || quoteData.AccessorialsObj.CONPU)
            {
                //accessorials.Add("RESPU");
                gcmAPI.RL_CarriersAPI.Accessorial acc = new gcmAPI.RL_CarriersAPI.Accessorial();
                acc = gcmAPI.RL_CarriersAPI.Accessorial.ResidentialPickup;
                accessorials2[countAcc] = acc;
                countAcc++;
            }

            if (quoteData.AccessorialsObj.RESDEL || quoteData.AccessorialsObj.CONDEL)
            {
                //accessorials2[countAcc] = "ResidentialDelivery";
                gcmAPI.RL_CarriersAPI.Accessorial acc = new gcmAPI.RL_CarriersAPI.Accessorial();
                acc = gcmAPI.RL_CarriersAPI.Accessorial.ResidentialDelivery;
                accessorials2[countAcc] = acc;
                countAcc++;
            }
            //accessorials.Add("RESDEL");

            if (quoteData.AccessorialsObj.INSDEL)
            {
                //accessorials2[countAcc] = "InsideDelivery";
                gcmAPI.RL_CarriersAPI.Accessorial acc = new gcmAPI.RL_CarriersAPI.Accessorial();
                acc = gcmAPI.RL_CarriersAPI.Accessorial.InsideDelivery;
                accessorials2[countAcc] = acc;
                countAcc++;
            }
            //accessorials.Add("INSDEL");

            if (quoteData.AccessorialsObj.LGPU)
            {
                gcmAPI.RL_CarriersAPI.Accessorial acc = new gcmAPI.RL_CarriersAPI.Accessorial();
                acc = gcmAPI.RL_CarriersAPI.Accessorial.OriginLiftgate;
                accessorials2[countAcc] = acc;
                //accessorials2[countAcc] = "OriginLiftgate";
                countAcc++;
            }
            //accessorials.Add("LGPU");

            if (quoteData.AccessorialsObj.LGDEL)
            {
                //accessorials.Add("LGDEL");
                //accessorials2[countAcc] = "DestinationLiftgate";
                gcmAPI.RL_CarriersAPI.Accessorial acc = new gcmAPI.RL_CarriersAPI.Accessorial();
                acc = gcmAPI.RL_CarriersAPI.Accessorial.DestinationLiftgate;
                accessorials2[countAcc] = acc;
                countAcc++;
            }

            if (quoteData.AccessorialsObj.APTDEL)
            {
                //accessorials2[countAcc] = "DeliveryNotification";
                gcmAPI.RL_CarriersAPI.Accessorial acc = new gcmAPI.RL_CarriersAPI.Accessorial();
                acc = gcmAPI.RL_CarriersAPI.Accessorial.DeliveryNotification;
                accessorials2[countAcc] = acc;
                countAcc++;
            }
            //accessorials.Add("APT");

            if (quoteData.isHazmat.Equals(true))
            {
                gcmAPI.RL_CarriersAPI.Accessorial acc = new gcmAPI.RL_CarriersAPI.Accessorial();
                acc = gcmAPI.RL_CarriersAPI.Accessorial.Hazmat;
                accessorials2[countAcc] = acc;
                //accessorials2[countAcc] = "Hazmat";
                //countAcc++;
            }

            #endregion

            request.Accessorials = accessorials2;

            request.CODAmount = 0;
            request.DeclaredValue = 0;

            gcmAPI.RL_CarriersAPI.QuoteDetails details = new gcmAPI.RL_CarriersAPI.QuoteDetails();

            details.Origin = new gcmAPI.RL_CarriersAPI.ServicePoint();
            details.Origin.ZipOrPostalCode = quoteData.origZip;
            details.Origin.City = quoteData.origCity;
            details.Origin.StateOrProvince = quoteData.origState;
            details.Origin.CountryCode = "USA";

            details.Destination = new gcmAPI.RL_CarriersAPI.ServicePoint();
            details.Destination.ZipOrPostalCode = quoteData.destZip;
            details.Destination.City = quoteData.destCity;
            details.Destination.StateOrProvince = quoteData.destState;
            details.Destination.CountryCode = "USA";

            gcmAPI.RL_CarriersAPI.QuoteType quoteType = new gcmAPI.RL_CarriersAPI.QuoteType();
            quoteType = gcmAPI.RL_CarriersAPI.QuoteType.Domestic;

            request.QuoteType = quoteType;

            request.CustomerData = "";

            request.Origin = details.Origin;
            request.Destination = details.Destination;

            gcmAPI.RL_CarriersAPI.RateQuoteReply reply = new gcmAPI.RL_CarriersAPI.RateQuoteReply();

            gcmAPI.RL_CarriersAPI.RateQuoteService service = new gcmAPI.RL_CarriersAPI.RateQuoteService();

            reply = service.GetRateQuote(API_KeyRL_Durachem, request);

            //string costAmount  = reply.Result.Charges[0].Amount;

            string costAmount = string.Empty;
            if (quoteData.m_lPiece.Length.Equals(1))
            {
                costAmount = reply.Result.Charges[5].Amount;
            }
            else if (quoteData.m_lPiece.Length.Equals(2))
            {
                costAmount = reply.Result.Charges[6].Amount;
            }
            else if (quoteData.m_lPiece.Length.Equals(3))
            {
                costAmount = reply.Result.Charges[7].Amount;
            }
            else if (quoteData.m_lPiece.Length.Equals(4))
            {
                costAmount = reply.Result.Charges[8].Amount;
            }

            DB.Log("RL costAmount ", costAmount);

            string[] returnInfo = new string[5];
            returnInfo[0] = "";
            returnInfo[1] = "";
            returnInfo[2] = "";
            returnInfo[3] = costAmount.Replace("$", "");
            returnInfo[4] = "5";

            return returnInfo;

        }

        #endregion

        #region getSOAP_TransitTimes_RL

        // getRateQuote_RL
        private int getSOAP_TransitTimes_RL()
        {
            try
            {
                gcmAPI.RL_CarriersTransitTimes_API.TransitTimesRequest request = new gcmAPI.RL_CarriersTransitTimes_API.TransitTimesRequest();
                gcmAPI.RL_CarriersTransitTimes_API.TransitTimesReply reply = new gcmAPI.RL_CarriersTransitTimes_API.TransitTimesReply();

                request.DateOfPickup = DateTime.Today.ToShortDateString();
                gcmAPI.RL_CarriersTransitTimes_API.Origin orig = new gcmAPI.RL_CarriersTransitTimes_API.Origin();

                gcmAPI.RL_CarriersTransitTimes_API.ServicePoint servicePoint = new gcmAPI.RL_CarriersTransitTimes_API.ServicePoint();
                servicePoint.City = quoteData.origCity;
                servicePoint.ZipOrPostalCode = quoteData.origZip;
                servicePoint.StateOrProvince = quoteData.origState;
                servicePoint.CountryCode = "USA";

                request.OriginPoint = servicePoint;

                gcmAPI.RL_CarriersTransitTimes_API.Destination destination = new gcmAPI.RL_CarriersTransitTimes_API.Destination();
                gcmAPI.RL_CarriersTransitTimes_API.Destinations destinations = new gcmAPI.RL_CarriersTransitTimes_API.Destinations();
                gcmAPI.RL_CarriersTransitTimes_API.ServicePoint servicePointDest = new gcmAPI.RL_CarriersTransitTimes_API.ServicePoint();
                servicePointDest.City = quoteData.destCity;
                servicePointDest.ZipOrPostalCode = quoteData.destZip;
                servicePointDest.StateOrProvince = quoteData.origState;
                servicePointDest.CountryCode = "USA";

                gcmAPI.RL_CarriersTransitTimes_API.ServicePoint[] servicePointsDest = new gcmAPI.RL_CarriersTransitTimes_API.ServicePoint[1];
                servicePointsDest[0] = new gcmAPI.RL_CarriersTransitTimes_API.ServicePoint();

                servicePointsDest[0] = servicePointDest;

                destination.DestinationPoint = servicePointDest;

                destinations.DestinationPoint = servicePointsDest;

                request.Destinations = destinations;

                gcmAPI.RL_CarriersTransitTimes_API.TransitTimesService service = new gcmAPI.RL_CarriersTransitTimes_API.TransitTimesService();

                reply = service.GetTransitTimes(API_KeyRL_Durachem, request);


                int transitDays = 5;

                if (reply.Result.Destinations[0].ServiceDays > 0)
                {
                    transitDays = reply.Result.Destinations[0].ServiceDays;
                }

                return transitDays;
            }
            catch (Exception e)
            {
                DB.Log("RL", e.ToString());
                return 10;
            }


        }

        #endregion

        #region GetRLInfo

        private string[] GetRLInfo(string originZip,
            string originCity, string originState, string destZip,
            string destCity, string destState, string[] accessorials, string multItems)
        {
            try
            {

                originZip = originZip.ToUpper(); //for Canadian zipcodes
                destZip = destZip.ToUpper();
                string url, referrer, contentType, accept, method, doc;
                CookieContainer container = new CookieContainer();
                CookieCollection collection = new CookieCollection();
                #region Accessorials
                string accessor_str = "";
                foreach (string accessorial in accessorials)
                {
                    if (accessorial == "LGPU")   //Liftgate Pickup
                    {
                        accessor_str += "&origin-liftgate=on";
                    }
                    else if (accessorial == "LGDEL")  //Liftgate Delivery
                    {
                        accessor_str += "&destination-liftgate=on";

                    }
                    //else if (accessorial == "LIMACCPU")
                    //{
                    //    //throw new CarrierException("R&L: Rate Estimator does not accept Limited Access option at this time.");
                    //}
                    //else if (accessorial == "LIMACCDEL")
                    //{
                    //    //throw new CarrierException("R&L: Rate Estimator does not accept Limited Access option at this time.");
                    //}
                    //else if (accessorial == "TRADEPU" || accessorial == "TRADEDEL")
                    //{                  
                    //}
                    //else if ()
                    //{
                    //    //throw new CarrierException("R&L: Rate Estimator does not accept Tradeshow Delivery option at this time.");
                    //}

                    else if (accessorial == "RESPU") //Residential Pickup
                    {
                        accessor_str += "&residential-pickup=on";
                    }

                    else if (accessorial == "RESDEL") //Residential Delivery
                    {
                        accessor_str += "&residential-delivery=on";
                    }
                    else if (accessorial == "INSDEL")  //Inside Delivery
                    {
                        accessor_str += "&inside-delivery=on";
                    }
                    else if (accessorial == "APT")  //Delivery Notification
                    {
                        accessor_str += "&delivery-notification=on";
                    }
                    else if (accessorial == "HAZMAT")  //Hazerdous Materials    
                    {
                        accessor_str += "&hazmat=on";
                    }
                }
                #endregion


                #region new reworked

                url = "http://www2.rlcarriers.com/";
                referrer = "";
                contentType = "";
                accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                method = "GET";
                doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", false);
                string[] info = HelperFuncs.getViewAndEvent(doc);
                string viewstate = info[0];
                string eventvalidation = info[1];

                url = "http://www2.rlcarriers.com/default.aspx?AspxAutoDetectCookieSupport=1";
                referrer = "";
                //contentType = "";
                //accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                //method = "GET";
                doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", false);
                info = HelperFuncs.getViewAndEvent(doc);
                viewstate = info[0];
                eventvalidation = info[1];

                //

                url = "http://www2.rlcarriers.com/default.aspx?AspxAutoDetectCookieSupport=1";
                referrer = url;
                contentType = "application/x-www-form-urlencoded";
                accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                method = "POST";
                string DataParameters = string.Concat("__EVENTTARGET=ctl00%24MainMenu%24btnLogon&__EVENTARGUMENT=&__VIEWSTATE=",
                    viewstate,

                    //"%2FwEPDwULLTExOTU0MjM2MDIPZBYCZg9kFggCAQ8WAh4EVGV4dAWSATxsaW5rIHJlbD0ic3R5bGVzaGVldCIgaHJlZj0iL2ZpbGVzL3N0eWxlcy9zdHlsZS5jc3MiIHR5cGU9InRleHQvY3NzIiBtZWRpYT0ic2NyZWVuIiAvPg0KPGxpbmsgcmVsPSJzaG9ydGN1dCBpY29uIiBocmVmPSIvaW1hZ2VzL2Zhdmljb24uaWNvIiAvPg0KZAIDDxYCHwAFwAE8c2NyaXB0IHR5cGU9J3RleHQvamF2YXNjcmlwdCcgc3JjPScvL3d3dzIucmxjYXJyaWVycy5jb20vZmlsZXMvc2NyaXB0cy9jb3JlLmpzJz48L3NjcmlwdD48c2NyaXB0IHR5cGU9J3RleHQvamF2YXNjcmlwdCcgc3JjPScvL3d3dzIucmxjYXJyaWVycy5jb20vZmlsZXMvc2NyaXB0cy9hbmFseXRpY3MuanM%2Fdj0xODA4Jz48L3NjcmlwdD5kAg0PFgIfAAVKPGlucHV0IHR5cGU9J2hpZGRlbicgaWQ9J01vYmlsZVNpdGVVUkwnIHZhbHVlPSdodHRwOi8vbS5ybGNhcnJpZXJzLmNvbScgLz5kAg8PZBYGAgEPZBYGZg9kFgICAQ8WAh8AZWQCAg8WAh8ABasCPGRpdiBjbGFzcz0ibG9ja2xpbmstY29udGFpbmVyIj48YSBocmVmPSIvZnJlaWdodC9zaGlwcGluZy1yZXNvdXJjZXMvYWN0aXZpdHktaGlzdG9yeSIgY2xhc3M9Im15cmxjLWxvY2sgcmVzb3VyY2VzLWxvY2siIHRpdGxlPSJWaWV3IHlvdXIgYWNjb3VudCBoaXN0b3J5LiI%2BPHNwYW4%2BPGltZyBzcmM9Ii9JbWFnZXMvbXlybGMtbG9jay5wbmciIGFsdD0iTXlSTEMgdG9vbCIgY2xhc3M9Im15cmxjLWltYWdlIi8%2BPC9zcGFuPiA8c3BhbiBjbGFzcz0idW5kZXJsaW5lIj5BY3Rpdml0eSBIaXN0b3J5PC9zcGFuPjwvYT48L2Rpdj5kAgMPFgIfAGVkAgMPZBYEAgEPZBYEZg8PFgIeB1Zpc2libGVoZGQCAg8PFgIfAWhkFgICBQ8QZA8WE2YCAQICAgMCBAIFAgYCBwIIAgkCCgILAgwCDQIOAg8CEAIRAhIWExBlZWcQBQQ1MC4wBQQ1MC4wZxAFBDU1LjAFBDU1LjBnEAUENjAuMAUENjAuMGcQBQQ2NS4wBQQ2NS4wZxAFBDcwLjAFBDcwLjBnEAUENzcuNQUENzcuNWcQBQQ4NS4wBQQ4NS4wZxAFBDkyLjUFBDkyLjVnEAUFMTAwLjAFBTEwMC4wZxAFBTExMC4wBQUxMTAuMGcQBQUxMjUuMAUFMTI1LjBnEAUFMTUwLjAFBTE1MC4wZxAFBTE3NS4wBQUxNzUuMGcQBQUyMDAuMAUFMjAwLjBnEAUFMjUwLjAFBTI1MC4wZxAFBTMwMC4wBQUzMDAuMGcQBQU0MDAuMAUFNDAwLjBnEAUFNTAwLjAFBTUwMC4wZxYBZmQCAw8QDxYCHgtEaXNwbGF5TW9kZQsqMVN5c3RlbS5XZWIuVUkuV2ViQ29udHJvbHMuQnVsbGV0ZWRMaXN0RGlzcGxheU1vZGUBZA8WBWYCAQICAgMCBBYFEAVRR2xvYmFsIFRyYW5zcG9ydGF0aW9uIFByb3ZpZGVyIFIrTCBDYXJyaWVycyBFeHBhbmRzIEV4cGVkaXRlZCBTZXJ2aWNlIGludG8gQ2FuYWRhBWUvbmV3cy8yMDE1LTA4LTI1L0dsb2JhbC1UcmFuc3BvcnRhdGlvbi1Qcm92aWRlci1STC1DYXJyaWVycy1FeHBhbmRzLUV4cGVkaXRlZC1TZXJ2aWNlLWludG8tQ2FuYWRhLzM2MWcQBUVSK0wgQ2FycmllcnMgVHJhbnNwb3J0cyBMaW1hIENvbXBhbnkgTWVtb3JpYWwgdG8gQmlrZSBXZWVrIGluIFN0dXJnaXMFWS9uZXdzLzIwMTUtMDgtMTgvUkwtQ2FycmllcnMtVHJhbnNwb3J0cy1MaW1hLUNvbXBhbnktTWVtb3JpYWwtdG8tQmlrZS1XZWVrLWluLVN0dXJnaXMvMzUxZxAFP1IrTCBDYXJyaWVycyBBbm5vdW5jZXMgTmV3IFNlcnZpY2UgQ2VudGVyIGluIE1ldHJvIEF0bGFudGEgQXJlYQVTL25ld3MvMjAxNS0wNy0xMy9STC1DYXJyaWVycy1Bbm5vdW5jZXMtTmV3LVNlcnZpY2UtQ2VudGVyLWluLU1ldHJvLUF0bGFudGEtQXJlYS8zNDFnEAUxUitMIENBUlJJRVJTIE5FVyBPUkxFQU5TIEJPV0wgQU5OT1VOQ0VTIDIwMTUgREFURQVFL25ld3MvMjAxNS0wNi0wNC9STC1DQVJSSUVSUy1ORVctT1JMRUFOUy1CT1dMLUFOTk9VTkNFUy0yMDE1LURBVEUvMzMyZxAFS1IrTCBDYXJyaWVycyAtIENhbmFkaWFuIEdhdGV3YXkgU2VydmljZSBDZW50ZXIgUmVsb2NhdGlvbiBFeHBhbmRzIENhcGFjaXR5IAVfL25ld3MvMjAxNS0wMy0wOS9STC1DYXJyaWVycy0tLUNhbmFkaWFuLUdhdGV3YXktU2VydmljZS1DZW50ZXItUmVsb2NhdGlvbi1FeHBhbmRzLUNhcGFjaXR5LS8zMTFnFgBkAgcPZBYEAgEPFgIfAAUeJmNvcHk7IDIwMTUgUitMIENhcnJpZXJzLCBJbmMuZAIDDxYCHwAFLDxhIGhyZWY9J2h0dHA6Ly9tLnJsY2FycmllcnMuY29tJz5Nb2JpbGU8L2E%2BZBgBBR5fX0NvbnRyb2xzUmVxdWlyZVBvc3RCYWNrS2V5X18WAgUaY3RsMDAkTWFpbk1lbnUkY2hrUmVtZW1iZXIFJ2N0bDAwJGNwaEJvZHkkVG9vbHNNZW51JGNoa1JlbWVtYmVyUGFzc%2FW%2BnXlJaDgw9BqOJdBVpJudXPHo",

                    "&__EVENTVALIDATION=", eventvalidation,
                //"%2FwEWDQKDqMz4CQKR2qWYAQK0usD6AgKZscWRDgLrwIX5BwLX58aGDQKmxbiMAwLh0afRAQK0%2Ft2xAgLMmKmYCQLx8tDJBwKKj9LtBgLNoIzbApjUrr8JImy7THUYlHW5%2FiEYvpd%2F",
                
                "&ctl00%24MainMenu%24txtUserName=", acctInfo.username, "&ctl00%24MainMenu%24txtPassword=", acctInfo.password, "&ctl00%24MainMenu%24txtsearch=Search+Site&ctl00%24cphBody%24ToolsMenu%24txtLoginUserName=&ctl00%24cphBody%24ToolsMenu%24txtLoginPassword=&ctl00%24cphBody%24ToolsMenu%24searchBy=PRO&ctl00%24cphBody%24ToolsMenu%24txtPro=");

                collection = (CookieCollection)HelperFuncs.generic_http_request_3("collection", container, url, referrer,
                     contentType, accept, method, DataParameters, false, false, "", "");

                url = "http://www2.rlcarriers.com/default.aspx?AspxAutoDetectCookieSupport=1";

                referrer = "";
                contentType = "";
                accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                method = "GET";
                doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", false);

                //--------------------------------------------------------------------------
                contentType = "application/x-www-form-urlencoded";
                method = "POST";

                referrer = "http://www2.rlcarriers.com/";
                url = "http://www2.rlcarriers.com/freight/shipping/rate-quote";
                doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, "", false);



                info = HelperFuncs.getViewAndEvent(doc);
                viewstate = info[0];
                eventvalidation = info[1];
                //--------------------------------------------------------------------------

                referrer = url;
                contentType = "application/x-www-form-urlencoded";
                method = "POST";

                //DB.Log("r&l accessor", accessor_str);

                string data = string.Concat("__EVENTTARGET=&__EVENTARGUMENT=&__VIEWSTATE=", viewstate,

                    //"%2FwEPDwUKMTgzODIzMDQ5OGQYAQUeX19Db250cm9sc1JlcXVpcmVQb3N0QmFja0tleV9fFgEFIGN0bDAwJGNwaEJvZHkkYnRuU3VibWl0UmF0ZVF1b3Rl",
                    "&__EVENTVALIDATION=", eventvalidation,

                    //"%2FwEWHgKe8pnpCwK9ju3fCgK0n6PGBQLX58aGDQK4rZzVDwKos%2FCHBQLq6NqUBALa9tfABgL6hurZAQLl5o6yCAL1p8%2BuAwLx8tDJBwKSv82JBgLItv6IAQLMvL2RDQL%2B%2Beu8DwL%2Bs7JhAsGOqbkPAqH4yuQMArPssvIEAsTJ3NcHApX65KsHArHUgOkIAoqP0u0GAs2gjNsCAvHmzJkOAoC9o%2F4PApOyisoOAvjCjc0HAvKG89IB",
                    "&ctl00%24MainMenu%24txtsearch=Search+Site&ctl00%24cphBody%24txtOriginZipCode=", originZip,
                    "&ddlOriginCity=", originCity.Replace(" ", "+"), "%2C+", originState, "&ctl00%24cphBody%24txtDestinationZipCode=", destZip,
                    "&ddlDestinationCity=", destCity.Replace(" ", "+"), "%2C+" + destState, multItems,

                    //"&class-field-1=50.0&weight-field-1=1200",
                    //&origin-liftgate=on&destination-liftgate=on
                    //&origin-liftgate=on&destination-liftgate=on
                    "&ctl00%24cphBody%24txtCODAmount=",
                    "&ctl00%24cphBody%24txtCustomerDiscount=", accessor_str, "&pieces=",
                    "&ctl00%24cphBody%24GlobalTSRQLink=http%3A%2F%2Fwww.rlglobal.com%2Ftrade-show-rate-quote.aspx",
                    "&ctl00%24cphBody%24site=beta&ctl00%24cphBody%24btnSubmitRateQuote.x=49",
                    "&ctl00%24cphBody%24btnSubmitRateQuote.y=15&hidClassWeightCount=1&hidValidOrigin=0",
                    "&hidValidDestination=0&hidValidPointToPoint=0&ctl00%24cphBody%24ToolsMenu%24searchBy=PRO",
                    "&ctl00%24cphBody%24ToolsMenu%24txtPro=&ctl00%24cphBody%24hfRequest=&ctl00%24cphBody%24hfOriginCity=",
                    "&ctl00%24cphBody%24hfDestinationCity=&ctl00%24cphBody%24hdnFreezableStart=10%2F01",
                    "&ctl00%24cphBody%24hdnFreezableEnd=05%2F01");


                doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, data, false);
                #endregion
                //--------------------------------------------------------------------------


                #region scrape
                Int32 y;
                string temp, rate, transit;
                temp = doc;
                y = doc.IndexOf("Standard Service Net Charge");
                temp = temp.Substring(y + 27);
                y = temp.IndexOf("charge");
                temp = temp.Substring(y + 9);

                y = temp.IndexOf("</td>");
                temp = temp.Remove(y);
                rate = temp;              // got rate

                if (rate.Length > 15) rate = "";

                temp = doc;
                y = temp.IndexOf("Service Days:");
                if (y == -1)
                {
                    transit = "Not Listed";
                }
                else
                {
                    temp = temp.Substring(y + 14);
                    y = temp.IndexOf("</div>");
                    temp = temp.Remove(y);
                    transit = temp;
                }

                temp = doc;
                string guaranteedChargesPM = "", guaranteedChargesAM = "";
                y = temp.IndexOf("id=\"gsdsAmount");
                if (y == -1)
                {
                    guaranteedChargesPM = "n/a";
                }
                else
                {
                    temp = temp.Substring(y);
                    y = temp.IndexOf(">");
                    temp = temp.Substring(y + 1);
                    y = temp.IndexOf("<");
                    guaranteedChargesPM = temp.Remove(y).Replace("Add", "").Replace("$", "").Trim();
                }

                y = temp.IndexOf("id=\"gsamAmount");
                if (y == -1)
                {
                    guaranteedChargesAM = "n/a";
                }
                else
                {
                    temp = temp.Substring(y);
                    y = temp.IndexOf(">");
                    temp = temp.Substring(y + 1);
                    y = temp.IndexOf("<");
                    guaranteedChargesAM = temp.Remove(y).Replace("Add", "").Replace("$", "").Trim();
                }
                #endregion

                double totalCharges;
                int standardDays;
                if (!double.TryParse(rate, out totalCharges) || !int.TryParse(transit, out standardDays))
                {
                    throw new Exception("charges: " + rate + " transit: " + transit);
                }

                string[] returnInfo = new string[5];
                returnInfo[0] = "";
                returnInfo[1] = guaranteedChargesAM;
                returnInfo[2] = guaranteedChargesPM;
                returnInfo[3] = rate;
                returnInfo[4] = transit;
                //try //temporary "success" logging
                //{
                //    DB.Log("SuccessR&L Freight (Live)", "password: " + acctInfo.password + " " + originZip + " " + originCity + "," + originState +
                //                " to " + destZip + " " + destCity + "," + destState + " " + multItems, "");                
                //}
                //catch { }
                return returnInfo;
            }
            catch (Exception e)
            {
                string[] returnInfo = new string[5];
                returnInfo[0] = "";
                returnInfo[1] = "issue";
                returnInfo[2] = e.ToString();
                returnInfo[3] = "";
                returnInfo[4] = "";
                try
                {
                    string accessors = "";
                    for (int i = 0; i < accessorials.Length; i++)
                    {
                        accessors += accessorials[i] + " ";
                    }
                    DB.Log("R&L Freight (Live)", "password: " + acctInfo.password + " " + originZip + " " + originCity + "," + originState +
                        " to " + destZip + " " + destCity + "," + destState + " " + multItems + " " + accessors + " " + e.ToString());
                    returnInfo[1] = "loggedException"; // so it won't be logged twice
                }
                catch { }
                return returnInfo;
            }
        }

        #endregion

        #region GetResultObjectFromRAndL

        private void GetResultObjectFromRAndL(ref GCMRateQuote gcmRateQuote, ref GCMRateQuote rl_quote_guaranteed)
        {

            DB.Log("GetResultObjectFromRAndL", "");

            //bool loggedException = false;
            try
            {
                if (quoteData.AccessorialsObj.TRADEPU.Equals(true) || quoteData.AccessorialsObj.TRADEDEL.Equals(true))
                {
                    throw new Exception("Tradeshow not supported");
                }

                double totalCharges = 0, accValSPC = 0;
                int standardDays = -1;
                string multPieces = "", str;
                //url, referrer, contentType, accept, method, doc, 
                CookieContainer container = new CookieContainer();
                List<string> accessorials = new List<string>();

                #region Items
                string[] fclassRL = new string[5];

                for (int x = 0; x < quoteData.m_lPiece.Length; x++)
                {
                    str = quoteData.m_lPiece[x].Weight.ToString();
                    if (str != "")
                    {

                        if (quoteData.m_lPiece[x].FreightClass == "50")
                        {
                            fclassRL[x] = "50.0";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "55")
                        {
                            fclassRL[x] = "55.0";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "60")
                        {
                            fclassRL[x] = "60.0";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "65")
                        {
                            fclassRL[x] = "65.0";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "70")
                        {
                            fclassRL[x] = "70.0";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "77.5")
                        {
                            fclassRL[x] = "77.5";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "85")
                        {
                            fclassRL[x] = "85.0";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "92.5")
                        {
                            fclassRL[x] = "92.5";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "100")
                        {
                            fclassRL[x] = "100.0";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "110")
                        {
                            fclassRL[x] = "110.0";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "125")
                        {
                            fclassRL[x] = "125.0";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "150")
                        {
                            fclassRL[x] = "150.0";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "175")
                        {
                            fclassRL[x] = "175.0";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "200")
                        {
                            fclassRL[x] = "200.0";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "250")
                        {
                            fclassRL[x] = "250.0";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "300")
                        {
                            fclassRL[x] = "300.0";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "400")
                        {
                            fclassRL[x] = "400.0";
                        }
                        else if (quoteData.m_lPiece[x].FreightClass == "500")
                        {
                            fclassRL[x] = "500.0";
                        }

                    }

                    multPieces += "&class-field-" + (x + 1).ToString() + "=" + fclassRL[x] +
                            "&weight-field-" + (x + 1).ToString() + "=" + quoteData.m_lPiece[x].Weight;

                    //DB.Log("class rl", fclassRL[x]);
                }

                #endregion

                #region Accessorials

                if (quoteData.AccessorialsObj.RESPU || quoteData.AccessorialsObj.CONPU)
                {
                    accessorials.Add("RESPU");
                    accValSPC += 40;
                }

                if (quoteData.AccessorialsObj.RESDEL || quoteData.AccessorialsObj.CONDEL)
                    accessorials.Add("RESDEL");

                if (quoteData.AccessorialsObj.INSDEL)
                    accessorials.Add("INSDEL");

                if (quoteData.AccessorialsObj.LGPU)
                    accessorials.Add("LGPU");

                if (quoteData.AccessorialsObj.LGDEL)
                {
                    accessorials.Add("LGDEL");
                    accValSPC += 10;
                }

                if (quoteData.AccessorialsObj.APTDEL)
                    accessorials.Add("APT");

                if (quoteData.isHazmat)
                {
                    accessorials.Add("HAZMAT");
                }

                string[] accessors = new string[accessorials.Count];
                for (int i = 0; i < accessorials.Count; i++)
                {
                    accessors[i] = accessorials[i];
                }

                #endregion

                string destCity = quoteData.destCity;
                string originCity = quoteData.origCity;
                string destState = quoteData.destZip;
                string originState = quoteData.origZip;

                //string[] res = GetRLInfo(username, password, midOrigZip, originCity, originState, midDestZip, destCity, destState, accessors, multPieces);


                string res_doc = "";
                string[] res = getXML_RateQuote_RL(accessors, multPieces, ref res_doc);

                string[] tokens = new string[4];
                tokens[0] = ">Guaranteed Service<";
                tokens[1] = "<NetCharge>";              
                tokens[2] = "$";
                tokens[3] = "<";

                string guaranteed_cost_str = HelperFuncs.scrapeFromPage(tokens, res_doc);

                tokens[1] = "<QuoteNumber>";
                tokens[2] = ">";
                tokens[3] = "<";
                string guaranteed_quote_number = HelperFuncs.scrapeFromPage(tokens, res_doc);

                tokens[1] = "<ServiceDays>";            
                string guaranteed_service_days_str = HelperFuncs.scrapeFromPage(tokens, res_doc);

                int guaranteed_service_days = 5;
                int.TryParse(guaranteed_service_days_str, out guaranteed_service_days);

                DB.Log("RL xml cost", guaranteed_cost_str);


                if (res[1] == "loggedException")
                {
                    //loggedException = true;
                    throw new Exception();
                }

                if (!double.TryParse(res[3], out totalCharges))
                {
                    throw new Exception("Total Charges Not Parsed To Double");
                }

                if (!int.TryParse(res[4], out standardDays))
                {
                    standardDays = -3;
                }

                //double guaranteedChargesAM = -1, guaranteedChargesPM = -1;
                //if ((res[1] != "n/a" && !double.TryParse(res[1], out guaranteedChargesAM)) || (res[2] != "n/a" && !double.TryParse(res[2], out guaranteedChargesPM)))
                //{
                //    DB.Log("R&L Carriers (Live)", "Guaranteed Services not parsed", "");
                //}

                #region Clipper subdomain
                
                //if (quoteData.subdomain.Equals(HelperFuncs.Subdomains.clipper))
                //{
                //    totalCharges = HelperFuncs.addClipperSubdomain_Addition(totalCharges);

                //}
                
                
                #endregion


                if (totalCharges > 0)
                {
                    #region Spc subdomain
                    //if (quoteData.subdomain.Equals("spc") || isCostPlus)
                    //{
                    //    totalCharges = HelperFuncs.addSPC_Addition(totalCharges);
                    //}
                    #endregion

                    string Documentation = "http://www.globalcargomanager.com/Documents/R&L_Guaranteed.pdf";

                    gcmRateQuote.TotalPrice = totalCharges;
                    gcmRateQuote.DisplayName = acctInfo.displayName;
                    gcmRateQuote.Documentation = Documentation;
                    gcmRateQuote.DeliveryDays = standardDays;
                    gcmRateQuote.BookingKey = acctInfo.bookingKey;
                    gcmRateQuote.CarrierKey = acctInfo.carrierKey;
                    gcmRateQuote.Scac = "RNLO";

                    string Genera_display = "";
                    if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
                    {
                        Genera_display = "- Genera ";
                    }

                    //gcmRateQuote = SetInfoToObjectQuote(ref totalCharges, strRLDisplay, "#1#", "R+L", Documentation, standardDays, "R&L");
                    if (double.TryParse(guaranteed_cost_str, out double guaranteed_cost))
                    {
                        rl_quote_guaranteed.TotalPrice = guaranteed_cost;
                        rl_quote_guaranteed.DisplayName =
                            string.Concat("R+L GDD ", Genera_display, "QT:", guaranteed_quote_number, " Delivery on ",
                            DateTime.Today.AddDays(guaranteed_service_days).ToShortDateString(),
                            " by 5PM");
                        rl_quote_guaranteed.Documentation = Documentation;
                        rl_quote_guaranteed.DeliveryDays = guaranteed_service_days;
                        rl_quote_guaranteed.BookingKey = "#1#";
                        rl_quote_guaranteed.CarrierKey = "R+L";
                        rl_quote_guaranteed.RateType = "GUARANTEEDPM";
                        rl_quote_guaranteed.Scac = "RNLO";
                    }

                }
                else
                {
                  // Do nothing
                }
            }
            catch (Exception exp)
            {
                #region Catch
               
                DB.Log("R&L Carriers (Live)", exp.ToString());

                #endregion
            }
        }

        #endregion

        #region Not used Puerto Rico function

        #region getRLPuertoRico

        //public string[] getRLPuertoRico(string username, string password, string custCode, string[] weight, string[] fclass, string originZip, string originCity, string originState, string destZip,
        //  string destCity, string destState, string[] accessorials, string[] length, string[] width, string[] height)
        //{
        //    DB.Log("RLPuertoRico hit func", "");
        //    #region Class fix
        //    string[] fclassRL = new string[fclass.Length];
        //    for (int x = 0; x < fclass.Length; x++)
        //    {
        //        if (fclass[x] == "50")
        //        {
        //            fclassRL[x] = "50.0";
        //        }
        //        else if (fclass[x] == "55")
        //        {
        //            fclassRL[x] = "55.0";
        //        }
        //        else if (fclass[x] == "60")
        //        {
        //            fclassRL[x] = "60.0";
        //        }
        //        else if (fclass[x] == "65")
        //        {
        //            fclassRL[x] = "65.0";
        //        }
        //        else if (fclass[x] == "70")
        //        {
        //            fclassRL[x] = "70.0";
        //        }
        //        else if (fclass[x] == "77.5")
        //        {
        //            fclassRL[x] = "77.5";
        //        }
        //        else if (fclass[x] == "85")
        //        {
        //            fclassRL[x] = "85.0";
        //        }
        //        else if (fclass[x] == "92.5")
        //        {
        //            fclassRL[x] = "92.5";
        //        }
        //        else if (fclass[x] == "100")
        //        {
        //            fclassRL[x] = "100.0";
        //        }
        //        else if (fclass[x] == "110")
        //        {
        //            fclassRL[x] = "110.0";
        //        }
        //        else if (fclass[x] == "125")
        //        {
        //            fclassRL[x] = "125.0";
        //        }
        //        else if (fclass[x] == "150")
        //        {
        //            fclassRL[x] = "150.0";
        //        }
        //        else if (fclass[x] == "175")
        //        {
        //            fclassRL[x] = "175.0";
        //        }
        //        else if (fclass[x] == "200")
        //        {
        //            fclassRL[x] = "200.0";
        //        }
        //        else if (fclass[x] == "250")
        //        {
        //            fclassRL[x] = "250.0";
        //        }
        //        else if (fclass[x] == "300")
        //        {
        //            fclassRL[x] = "300.0";
        //        }
        //        else if (fclass[x] == "400")
        //        {
        //            fclassRL[x] = "400.0";
        //        }
        //        else if (fclass[x] == "500")
        //        {
        //            fclassRL[x] = "500.0";
        //        }
        //    }

        //    #endregion

        //    #region Accessorials
        //    string accessor_str = "", hazmat = "", lgpu = "", lgdel = "", respu = "", resdel = "";

        //    foreach (string accessorial in accessorials)
        //    {
        //        if (accessorial == "LGPU")   //Liftgate Pickup
        //        {
        //            lgpu = "&originLiftgate=X";
        //        }

        //        else if (accessorial == "LGDEL")  //Liftgate Delivery
        //        {
        //            lgdel = "&prdesLG=X";
        //        }

        //        else if (accessorial == "RESPU") //Residential Pickup
        //        {
        //            respu = "&resPickup=X";
        //        }

        //        else if (accessorial == "RESDEL") //Residential Delivery
        //        {
        //            resdel = "&prdesRD=X";
        //        }

        //        else if (accessorial == "HAZMAT")  //Hazerdous Materials    
        //        {
        //            hazmat = "&hazmat=X";
        //        }


        //    }
        //    #endregion

        //    #region Cubic feet

        //    decimal[] cubicFeet;
        //    cubicFeet = new decimal[width.Length];
        //    for (int i = 0; i < width.Length; i++)
        //    {
        //        cubicFeet[i] = Math.Round(((Convert.ToDecimal(length[i]) * Convert.ToDecimal(width[i]) * Convert.ToDecimal(height[i])) / 17281) * 10, 1);

        //    }

        //    #endregion

        //    #region Variables
        //    string url, referrer, contentType, accept, method, data, tmp;
        //    int ind;
        //    string[] res = new string[2];

        //    CookieContainer container = new CookieContainer();
        //    CookieCollection coll = new CookieCollection();
        //    //Stopwatch stopwatch = new Stopwatch();
        //    //int timeOut = intTimeOut;
        //    #endregion
        //    DB.Log("RLPuertoRico before login", "");
        //    #region Login
        //    url = "http://www.rlcarriers.com/";
        //    referrer = "";
        //    contentType = "";
        //    accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        //    method = "GET";

        //    //stopwatch.Start();
        //    coll = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", false);
        //    //stopwatch.Stop();
        //    //timeOut -= (int)stopwatch.ElapsedMilliseconds;

        //    url = "http://www.rlcarriers.com/myrlclogin.asp";
        //    referrer = "http://www.rlcarriers.com/";
        //    contentType = "application/x-www-form-urlencoded";
        //    method = "POST";
        //    //data = "login=&password=";
        //    data = "login=&password=";
        //    coll = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, data, false);

        //    //username: 
        //    //password: 

        //    url = "http://www.rlcarriers.com/index.asp";
        //    contentType = "";
        //    accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        //    method = "GET";
        //    coll = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", false);
        //    #endregion

        //    #region Go to Puerto Rico page
        //    referrer = url;
        //    url = "http://www.rlcarriers.com/kp_rate_quote.asp";
        //    coll = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", false);

        //    referrer = url;
        //    url = "http://www.rlcarriers.com/internationalRQ.asp";
        //    coll = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, "", false);
        //    #endregion

        //    #region Set origin/destination and scrape/city state
        //    referrer = url;
        //    url = "http://www.rlcarriers.com/webtools/rate_quote/getCountryFromZip.asp";
        //    accept = "text/html, application/xhtml+xml, */*";
        //    contentType = "application/x-www-form-urlencoded";
        //    method = "POST";
        //    data = "zipcode=" + originZip + "&source=O&countryexpected=USA&which=0&tool=international";
        //    coll = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, data, false);

        //    url = "http://www.rlcarriers.com/webtools/rate_quote/tossedzip.asp";
        //    data = "myzip=" + originZip + "&myparty=shipper";
        //    string doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, data, false);

        //    // Scrape city and state
        //    List<string> cities = new List<string>();
        //    List<string> states = new List<string>();
        //    string actualOriginCity = "";
        //    string actualDestCity = "";
        //    string actualOriginState = "";
        //    string actualDestState = "";

        //    string[] forSplit;
        //    tmp = doc;
        //    ind = tmp.IndexOf("SetCityState(");
        //    tmp = tmp.Substring(ind);
        //    while (tmp.IndexOf("?") != -1)
        //    {
        //        ind = tmp.IndexOf("?");
        //        tmp = tmp.Substring(ind + 1);
        //        ind = tmp.IndexOf("?");
        //        if (ind != -1)
        //        {
        //            forSplit = tmp.Remove(ind).Split(',');
        //            cities.Add(forSplit[0].Trim());
        //            states.Add(forSplit[1].Trim());
        //        }
        //    }
        //    // Try match city
        //    bool found = false;
        //    for (int i = 0; i < cities.Count; i++)
        //    {
        //        if (cities[i] == originCity)
        //        {
        //            actualOriginCity = originCity.Replace(" ", "+");
        //            actualOriginState = states[i];
        //            found = true;
        //        }
        //    }
        //    if (!found)
        //    {
        //        actualOriginCity = cities[0].Replace(" ", "+");
        //        actualOriginState = states[0];
        //    }


        //    url = "http://www.rlcarriers.com/webtools/rate_quote/getCountryFromZip.asp";
        //    data = "zipcode=" + destZip + "&source=D&countryexpected=PRI&which=6&tool=international";
        //    coll = (CookieCollection)HelperFuncs.generic_http_request("collection", container, url, referrer, contentType, accept, method, data, false);

        //    url = "http://www.rlcarriers.com/webtools/rate_quote/tossedzip.asp";
        //    data = "myzip=" + destZip + "&myparty=consignee";
        //    doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, data, false);

        //    tmp = doc;
        //    ind = tmp.IndexOf("SetCityState(");
        //    tmp = tmp.Substring(ind);
        //    cities.Clear();
        //    states.Clear();
        //    while (tmp.IndexOf("?") != -1)
        //    {
        //        ind = tmp.IndexOf("?");
        //        tmp = tmp.Substring(ind + 1);
        //        ind = tmp.IndexOf("?");
        //        if (ind != -1)
        //        {
        //            forSplit = tmp.Remove(ind).Split(',');
        //            cities.Add(forSplit[0].Trim());
        //            states.Add(forSplit[1].Trim());
        //        }
        //    }
        //    // Try match city
        //    found = false;
        //    for (int i = 0; i < cities.Count; i++)
        //    {
        //        if (cities[i] == originCity)
        //        {
        //            actualDestCity = originCity.Replace(" ", "+");
        //            actualDestState = states[i];
        //            found = true;
        //        }
        //    }
        //    if (!found)
        //    {
        //        actualDestCity = cities[0].Replace(" ", "+");
        //        actualDestState = states[0];
        //    }
        //    #endregion

        //    #region Multiple pieces and dimensions strings
        //    int cnt = 0;
        //    StringBuilder multUnits = new StringBuilder();
        //    StringBuilder unitsDimentions = new StringBuilder(); ;
        //    for (int i = 0; i < fclassRL.Length; i++)
        //    {
        //        multUnits.Append(string.Concat("&class", (i + 1).ToString(), "=" + fclassRL[i], "&weight", (i + 1).ToString(), "=", weight[i]));

        //        unitsDimentions.Append(string.Concat("&length", (i + 1).ToString(), "=", length[i], "&width", (i + 1).ToString(), "=" + width[i],
        //            "&height", (i + 1).ToString(), "=", height[i], "&cfeet", (i + 1).ToString(), "=", cubicFeet[i]));
        //        cnt++;
        //    }
        //    for (int i = cnt + 1; i < 8; i++)
        //    {
        //        multUnits.Append(string.Concat("&class", i.ToString(), "=&weight", i.ToString(), "="));

        //        unitsDimentions.Append(string.Concat("&length", i.ToString(), "=&width", i.ToString(), "=&height", i.ToString(), "=&cfeet", i.ToString(), "=0"));
        //    }
        //    #endregion

        //    DB.Log("RLPuertoRico before rate_quote", "before");

        //    #region Rate request
        //    url = "http://www.rlcarriers.com/webtools/rate_quote/internationalVal.asp";
        //    accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        //    data = string.Concat("retries=0&filename2=&orgValues=&dd1Country=USA&txtOriginZip=", originZip,
        //        "&selOriginCity=", actualOriginCity, "%2C+", actualOriginState, lgpu, respu,
        //        "&dd5Country=&fldzip_dest=&selDestCity=",
        //        "&dd6Country=PRI&przip=", destZip, "&prDestCity=", actualDestCity, "%2C+", actualDestState, lgdel, resdel, "&prdesDD=X",
        //        "&dd3Country=&uszip=&usDestCity=", multUnits.ToString(),

        //        "&CODAmt=&declaredVal=1", hazmat, "&txtNumOD=", unitsDimentions.ToString());

        //    doc = (string)HelperFuncs.generic_http_request("string", container, url, referrer, contentType, accept, method, data, false);

        //    DB.Log("RLPuertoRico rate_quote", doc);

        //    #endregion

        //    #region Scrape
        //    tmp = doc;
        //    ind = tmp.IndexOf("Net Charge");
        //    tmp = tmp.Substring(ind);
        //    ind = tmp.IndexOf("$");
        //    tmp = tmp.Substring(ind + 1);
        //    ind = tmp.IndexOf("\"");
        //    res[0] = tmp.Remove(ind).Replace(",", "");

        //    try
        //    {
        //        tmp = doc;
        //        ind = tmp.IndexOf("Quote Number:");
        //        tmp = tmp.Substring(ind);
        //        ind = tmp.IndexOf(", ");
        //        tmp = tmp.Substring(ind);
        //        ind = tmp.IndexOf("|");
        //        tmp = tmp.Substring(ind + 1);
        //        ind = tmp.IndexOf("|");
        //        res[1] = tmp.Remove(ind);
        //    }
        //    catch
        //    {
        //        res[1] = "";
        //    }
        //    #endregion

        //    return res;
        //}

        #endregion

        #region GetResultObjectFromRLPuertoRico

        //private void GetResultObjectFromRLPuertoRico()
        //{
        //    //DB.Log("GetResultObjectFromRLPuertoRico", "");
        //    try
        //    {
        //        if (AccessorialsObj.TRADEPU.Equals(true) || AccessorialsObj.TRADEDEL.Equals(true))
        //        {
        //            throw new Exception("Tradeshow not supported");
        //        }

        //        #region Variables

        //        string originZip = "", originCity = "", originState = "", destZip = "", destCity = "", destState = "PR";
        //        int accessorsCnt = 0;
        //        List<string> accessorsList = new List<string>();
        //        string[] weight = new string[quoteData.numOfUnitsPieces];
        //        string[] fclass = new string[quoteData.numOfUnitsPieces];
        //        string[] length = new string[quoteData.numOfUnitsPieces];
        //        string[] width = new string[quoteData.numOfUnitsPieces];
        //        string[] height = new string[quoteData.numOfUnitsPieces];

        //        int count = 0;

        //        #endregion

        //        #region Get inputs

        //        for (byte i = 0; i < m_lPiece.Length; i++)
        //        {
        //            if (m_lPiece[i].Quantity > 0)
        //            {
        //                string splitWeight = (m_lPiece[i].Weight / m_lPiece[i].Quantity).ToString(); // Divide weight equally
        //                for (byte j = 0; j < m_lPiece[i].Quantity; j++)
        //                {

        //                    fclass[count] = m_lPiece[i].FreightClass;
        //                    weight[count] = splitWeight;
        //                    length[count] = m_lPiece[i].Length.ToString();
        //                    width[count] = m_lPiece[i].Width.ToString();
        //                    height[count] = m_lPiece[i].Height.ToString();
        //                    count++;
        //                }
        //            }
        //            else
        //            {

        //                fclass[count] = m_lPiece[i].FreightClass;
        //                weight[count] = m_lPiece[i].Weight.ToString();
        //                length[count] = m_lPiece[i].Length.ToString();
        //                width[count] = m_lPiece[i].Width.ToString();
        //                height[count] = m_lPiece[i].Height.ToString();
        //                count++;
        //            }
        //        }

        //        #region Accessorials
        //        if (quoteData.isHazmat)
        //        {
        //            accessorsList.Add("HAZMAT");
        //            accessorsCnt++;
        //        }

        //        if (AccessorialsObj.LGPU)
        //        {
        //            accessorsList.Add("LGPU");
        //            accessorsCnt++;
        //        }
        //        if (AccessorialsObj.LGDEL)
        //        {
        //            accessorsList.Add("LGDEL");
        //            accessorsCnt++;
        //        }
        //        if (AccessorialsObj.RESPU)
        //        {
        //            accessorsList.Add("RESPU");
        //            accessorsCnt++;
        //        }
        //        if (AccessorialsObj.RESDEL)
        //        {
        //            accessorsList.Add("RESDEL");
        //            accessorsCnt++;
        //        }

        //        #endregion

        //        #region Accessorials
        //        string[] accessorials;
        //        accessorials = new string[accessorsCnt];
        //        for (int i = 0; i < accessorsList.Count; i++)
        //        {
        //            accessorials[i] = accessorsList[i];
        //        }

        //        if (accessorsCnt == 0)
        //        {
        //            accessorials = new string[1];
        //            accessorials[0] = "";
        //        }
        //        #endregion

        //        originZip = quoteData.origZip;
        //        destZip = quoteData.destZip;
        //        originCity = quoteData.origCity;
        //        destCity = quoteData.destCity;
        //        originState = quoteData.origState;
        //        destState = quoteData.destState;

        //        #endregion


        //        string[] res = getRLPuertoRico("", "", "", weight, fclass, originZip, originCity, originState, destZip, destCity,
        //        destState, accessorials, length, width, height);

        //        Double totalCharges;
        //        int standardDays = -3;
        //        if (!Double.TryParse(res[0], out totalCharges))
        //        {
        //            throw new Exception("Could not parse rate");
        //        }

        //        if (!int.TryParse(res[1], out standardDays))
        //        {
        //            standardDays = -3;
        //        }


        //        if (totalCharges > 0)
        //        {

        //            rlQuotePuertoRico = SetInfoToObjectQuote(ref totalCharges, "R&L Carriers", "#1#", "R+L", "", standardDays, "R&L");

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        #region Catch
        //        rlQuotePuertoRico = null;
        //        DB.Log("RLPuertoRico", e.ToString());
        //        #endregion
        //    }
        //}

        #endregion

        #endregion
    }
}