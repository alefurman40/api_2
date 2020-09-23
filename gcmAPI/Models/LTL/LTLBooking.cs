#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http.Formatting;
using System.Text;
using gcmAPI.Models.Public.LTL;
using gcmAPI.Models.Carriers.DLS;
using System.Data.SqlClient;
using gcmAPI.Models.Utilities;
using gcmAPI.Models.Public.LTL.JSON;

#endregion

public class LTLBooking
{
    #region setParameters

    #region MakeBookingViaSOAP

    public string MakeBookingViaSOAP(ref LTLBookRequest ltl_book_request, ref string username, ref string password)
    {
        //gcmAPI.gcmWebService.LTLBookingReply lbr;

        try
        {
            //return "{ \"Notification\" : \"temporily stopped service\" }";

            #region Testing

            StringBuilder logger = new StringBuilder();

            //ltl_book_request.lineItems.Length;
            logger.Append(string.Concat("lineItems.Length: ", ltl_book_request.items.Count, ", "));
            logger.Append(string.Concat("shipmentDate: ", ltl_book_request.pickupDate, ", "));
            logger.Append(string.Concat("bookingKey: ", ltl_book_request.bookingKey, ", "));
            logger.Append(string.Concat("customerType: ", ltl_book_request.customerType, ", "));

            logger.Append(string.Concat("origName: ", ltl_book_request.originName, ", "));
            logger.Append(string.Concat("origEmail: ", ltl_book_request.originEmail, ", "));
            logger.Append(string.Concat("origCompany: ", ltl_book_request.originCompany, ", "));
            logger.Append(string.Concat("origPhone: ", ltl_book_request.originPhone, ", "));
            logger.Append(string.Concat("origFax: ", ltl_book_request.originFax, ", "));
            logger.Append(string.Concat("origAddress1: ", ltl_book_request.originAddress1, ", "));
            logger.Append(string.Concat("origAddress2: ", ltl_book_request.originAddress2, ", "));
            logger.Append(string.Concat("origCity: ", ltl_book_request.originCity, ", "));
            logger.Append(string.Concat("origState: ", ltl_book_request.originState, ", "));
            logger.Append(string.Concat("origZip: ", ltl_book_request.originZip, ", "));

            logger.Append(string.Concat("destName: ", ltl_book_request.destinationName, ", "));
            logger.Append(string.Concat("destEmail: ", ltl_book_request.destinationEmail, ", "));
            logger.Append(string.Concat("destCompany: ", ltl_book_request.destinationCompany, ", "));
            logger.Append(string.Concat("destPhone: ", ltl_book_request.destinationPhone, ", "));
            logger.Append(string.Concat("destFax: ", ltl_book_request.destinationFax, ", "));
            logger.Append(string.Concat("destAddress1: ", ltl_book_request.destinationAddress1, ", "));
            logger.Append(string.Concat("destAddress2: ", ltl_book_request.destinationAddress2, ", "));
            logger.Append(string.Concat("destCity: ", ltl_book_request.destinationCity, ", "));
            logger.Append(string.Concat("destState: ", ltl_book_request.destinationState, ", "));
            logger.Append(string.Concat("destZip: ", ltl_book_request.destinationZip, ", "));


            HelperFuncs.writeToSiteErrors("LTL Booking Restful", logger.ToString());

            #endregion

            #region Not used
            //logger.Append(string.Concat("shipmentDate: ", ltl_book_request.shipmentDate, ", "));

            //if(j

            //if (!DateTime.TryParse(ltl_book_request.shipmentDate, out shipmentDate))
            //{
            //    shipmentDate = DateTime.Today.AddDays(1);
            //}




            // Authenticate to the web service/API
            //string sessionId = rs.Authenticate(apiUserName, apiKey);

            //// Initialize SOAP header for authentication
            //rs.AuthHeaderValue = new MyWebReference.AuthHeader();

            //// Set session id to the SOAP header
            //rs.AuthHeaderValue.SessionID = sessionId;
            #endregion

            DateTime shipmentDate = ltl_book_request.pickupDate;

            // Initialize web service/API object
            gcmAPI.gcmWebService.RateService2 rs = new gcmAPI.gcmWebService.RateService2();

            string bookingKey = ltl_book_request.bookingKey;

            string customerType = ltl_book_request.customerType; //Set customer type

            gcmAPI.gcmWebService.BookingThirdPartyBilling thirdPartyBilling = null;
            //DateTime shipmentDate = DateTime.Now.Date; //Set shipment date

            string readyTime = "02:15 PM"; //Set ready time of the shipment
            string closeTime = "06:15 PM"; //Set close time of the shipment

            //string readyTime = "10"; //Set ready time of the shipment
            //string closeTime = "3"; //Set close time of the shipment

            string bolSendTo = "EML"; //Set BOL sending option
            //string poNumber = "45455"; //Set PO Number
            string poNumber = null; //Set PO Number

            #region Origin and Destination

            // Set pickup location
            gcmAPI.gcmWebService.BookingPickupLocation pickupLocation = new gcmAPI.gcmWebService.BookingPickupLocation();
            pickupLocation.Name = ltl_book_request.originName;
            pickupLocation.Email = ltl_book_request.originEmail;
            pickupLocation.Company = ltl_book_request.originCompany;
            pickupLocation.Phone = ltl_book_request.originPhone;
            pickupLocation.Fax = ltl_book_request.originFax;
            pickupLocation.Address1 = ltl_book_request.originAddress1;
            pickupLocation.Address2 = ltl_book_request.originAddress2;
            pickupLocation.City = ltl_book_request.originCity;
            pickupLocation.State = ltl_book_request.originState;
            pickupLocation.Zip = ltl_book_request.originZip;
            pickupLocation.DispatchAddressesId = 1;

            // Set destination location
            gcmAPI.gcmWebService.BookingDestinationLocation destinationLocation = new gcmAPI.gcmWebService.BookingDestinationLocation();
            destinationLocation.Name = ltl_book_request.destinationName;
            destinationLocation.Email = ltl_book_request.destinationEmail;
            destinationLocation.Company = ltl_book_request.destinationCompany;
            destinationLocation.Phone = ltl_book_request.destinationPhone;
            destinationLocation.Fax = ltl_book_request.destinationFax;
            destinationLocation.Address1 = ltl_book_request.destinationAddress1;
            destinationLocation.Address2 = ltl_book_request.destinationAddress2;
            destinationLocation.City = ltl_book_request.destinationCity;
            destinationLocation.State = ltl_book_request.destinationState;
            destinationLocation.Zip = ltl_book_request.destinationZip;

            #endregion

            //string SessionID = ltl_book_request.SessionID;
            //logger.Append(string.Concat("SessionID: ", ltl_book_request.SessionID, ", "));

            //int testInt;
            List<gcmAPI.gcmWebService.LTLBookingPiece> bookingLineItems = new List<gcmAPI.gcmWebService.LTLBookingPiece>();

            for (byte i = 0; i < ltl_book_request.items.Count; i++)
            {
                gcmAPI.gcmWebService.LTLBookingPiece lineItem = new gcmAPI.gcmWebService.LTLBookingPiece();
                lineItem.Tag = ltl_book_request.items[i].tag;
                lineItem.Description = ltl_book_request.items[i].description;
                if (ltl_book_request.items[i].units > 0)
                {
                    lineItem.NumberOfPallet = ltl_book_request.items[i].units;
                }
                else if (ltl_book_request.items[i].pieces > 0)
                {
                    lineItem.NumberOfPallet = ltl_book_request.items[i].pieces;
                }
                else
                {
                    lineItem.NumberOfPallet = 1;
                }


                // Testing
                logger.Append(string.Concat("Tag", i.ToString(), ": ", lineItem.Tag, ", "));
                logger.Append(string.Concat("Description", i.ToString(), ": ", lineItem.Description, ", "));
                logger.Append(string.Concat("NumberOfPallet", i.ToString(), ": ", lineItem.NumberOfPallet, ", "));

                bookingLineItems.Add(lineItem);
            }


            HelperFuncs.writeToSiteErrors("LTL Booking Restful", logger.ToString());

            #region Not used
            //Set other line items
            //gcmWebService.LTLBookingPiece[] bookingLineItems = new gcmWebService.LTLBookingPiece[1];

            //Set first line item for booking
            //bookingLineItems[0] = new gcmWebService.LTLBookingPiece();

            //// This tag is related to the object(LTLPiece) in lineItems array which is used in GetLTLBookingRate function call
            //bookingLineItems[0].Tag = form.Get("Tag");

            ////bookingLineItems[0].NMFC = 50;
            //bookingLineItems[0].NumberOfPallet = 1;
            //bookingLineItems[0].Description = form.Get("Description");

            //Set second line item for booking
            //bookingLineItems[1] = new MyWebReference.LTLBookingPiece();

            ////// This tag is related to the object(LTLPiece) in lineItems array which is used in GetLTLBookingRate function call
            //bookingLineItems[1].Tag = "2";

            //bookingLineItems[1].NMFC = 60;
            //bookingLineItems[1].NumberOfPallet = 10;
            //bookingLineItems[1].Description = "TEST";
            #endregion

            string comments = "TEST";
            //string comments = null;
            //bool insuranceRequired = true;
            bool insuranceRequired = false;
            double declaredValue = 100;

            // 
            //Book an LTL rate and getting the BOL and insurance certificate PDF url 
            gcmAPI.gcmWebService.LTLBookingReply lbr = rs.CreateLTLBooking_3(username, password, bookingKey, customerType, thirdPartyBilling, shipmentDate, readyTime, closeTime, bolSendTo,
                poNumber, pickupLocation, destinationLocation, bookingLineItems.ToArray(), comments, insuranceRequired, declaredValue);

            HelperFuncs.writeToSiteErrors("LTL Booking Restful result", string.Concat("notification=", lbr.Notification,
                "&shipmentID=", lbr.ShipmentId, "&=BOLURL", lbr.BOLURL));

            //
            DB.Log("before DLS Shipment Import", "before DLS Shipment Import");
            #region DLS Shipment Import

            try
            {
                var repo = new gcmAPI.Models.Public.LTL.Repository();
                AES_API_info api_info;
                repo.Get_booking_info_by_booking_key(ltl_book_request.bookingKey, out api_info);

                HelperFuncs.dlsShipInfo dlsInfo = new HelperFuncs.dlsShipInfo();
                HelperFuncs.DispatchInfo dispatchInfo = new HelperFuncs.DispatchInfo();
                HelperFuncs.AccessorialsObj AccessorialsObj = new HelperFuncs.AccessorialsObj();

                DLS_ShipmentImport shipment_import = new DLS_ShipmentImport(ref dlsInfo, ref dispatchInfo, ref api_info, ref AccessorialsObj,
                    ref username, lbr.ShipmentId);
                shipment_import.Set_DLS_ShipmentImport_objects(ref ltl_book_request, ref api_info);

                //DB.Log("dispatchInfo.username", dispatchInfo.username);

                string DLS_PrimaryReferencePNW = "";

                DB.Log("before ShipmentImportDLS", "before ShipmentImportDLS");

                //shipment_import.ShipmentImportDLS(ref DLS_PrimaryReferencePNW);

                DB.Log("after ShipmentImportDLS", "after ShipmentImportDLS");
            }
            catch(Exception e)
            {
                DB.Log("MakeBookingViaSOAP DLS Shipment Import", e.ToString());
            }
            DB.Log("after DLS Shipment Import", "after DLS Shipment Import");
            #endregion

            // Insert DLS_PrimaryReferencePNW into DB


            return lbr.ToJSON();

            //return string.Concat("{ \"notification\" : \"", lbr.Notification, "\", \"shipmentID\" : ", lbr.ShipmentId, ", \"BOLURL", lbr.BOLURL, " }");

            //return "test test";
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("LTL Booking Restful", e.ToString());
            Json_helper json_helper = new Json_helper();
            return json_helper.Build_error_response("1", "An error occurred while processing the request");

            //return "{ \"Notification\" : \"error\" }";
            //return lbr.ToJSON;
        }

    }

    #endregion

    #region makeBookingViaSOAP

    public string makeBookingViaSOAP(ref SharedLTL.LTLBookingInfo json)
    {
        //gcmAPI.gcmWebService.LTLBookingReply lbr;

        try
        {
            //return "{ \"Notification\" : \"temporarily stopped service\" }";
         
            #region Testing

            StringBuilder logger = new StringBuilder();
            logger.Append(string.Concat("shipmentDate: ", json.shipmentDate, ", "));
            logger.Append(string.Concat("bookingKey: ", json.bookingKey, ", "));
            logger.Append(string.Concat("customerType: ", json.customerType, ", "));

            logger.Append(string.Concat("origName: ", json.origName, ", "));
            logger.Append(string.Concat("origEmail: ", json.origEmail, ", "));
            logger.Append(string.Concat("origCompany: ", json.origCompany, ", "));
            logger.Append(string.Concat("origPhone: ", json.origPhone, ", "));
            logger.Append(string.Concat("origFax: ", json.origFax, ", "));
            logger.Append(string.Concat("origAddress1: ", json.origAddress1, ", "));
            logger.Append(string.Concat("origAddress2: ", json.origAddress2, ", "));
            logger.Append(string.Concat("origCity: ", json.origCity, ", "));
            logger.Append(string.Concat("origState: ", json.origState, ", "));
            logger.Append(string.Concat("origZip: ", json.origZip, ", "));

            logger.Append(string.Concat("destName: ", json.destName, ", "));
            logger.Append(string.Concat("destEmail: ", json.destEmail, ", "));
            logger.Append(string.Concat("destCompany: ", json.destCompany, ", "));
            logger.Append(string.Concat("destPhone: ", json.destPhone, ", "));
            logger.Append(string.Concat("destFax: ", json.destFax, ", "));
            logger.Append(string.Concat("destAddress1: ", json.destAddress1, ", "));
            logger.Append(string.Concat("destAddress2: ", json.destAddress2, ", "));
            logger.Append(string.Concat("destCity: ", json.destCity, ", "));
            logger.Append(string.Concat("destState: ", json.destState, ", "));
            logger.Append(string.Concat("destZip: ", json.destZip, ", "));

            #endregion

            #region Not used
            //logger.Append(string.Concat("shipmentDate: ", json.shipmentDate, ", "));

            //if(j

            //if (!DateTime.TryParse(json.shipmentDate, out shipmentDate))
            //{
            //    shipmentDate = DateTime.Today.AddDays(1);
            //}




            // Authenticate to the web service/API
            //string sessionId = rs.Authenticate(apiUserName, apiKey);

            //// Initialize SOAP header for authentication
            //rs.AuthHeaderValue = new MyWebReference.AuthHeader();

            //// Set session id to the SOAP header
            //rs.AuthHeaderValue.SessionID = sessionId;
            #endregion

            DateTime shipmentDate = json.shipmentDate;

            // Initialize web service/API object
            gcmAPI.gcmWebService.RateService2 rs = new gcmAPI.gcmWebService.RateService2();

            string bookingKey = json.bookingKey;

            string customerType = json.customerType; //Set customer type

            gcmAPI.gcmWebService.BookingThirdPartyBilling thirdPartyBilling = null;
            //DateTime shipmentDate = DateTime.Now.Date; //Set shipment date

            string readyTime = "02:15 PM"; //Set ready time of the shipment
            string closeTime = "06:15 PM"; //Set close time of the shipment

            //string readyTime = "10"; //Set ready time of the shipment
            //string closeTime = "3"; //Set close time of the shipment

            string bolSendTo = "EML"; //Set BOL sending option
            //string poNumber = "45455"; //Set PO Number
            string poNumber = null; //Set PO Number

            #region Origin and Destination

            // Set pickup location
            gcmAPI.gcmWebService.BookingPickupLocation pickupLocation = new gcmAPI.gcmWebService.BookingPickupLocation();
            pickupLocation.Name = json.origName;
            pickupLocation.Email = json.origEmail;
            pickupLocation.Company = json.origCompany;
            pickupLocation.Phone = json.origPhone;
            pickupLocation.Fax = json.origFax;
            pickupLocation.Address1 = json.origAddress1;
            pickupLocation.Address2 = json.origAddress2;
            pickupLocation.City = json.origCity;
            pickupLocation.State = json.origState;
            pickupLocation.Zip = json.origZip;
            pickupLocation.DispatchAddressesId = 1;

            // Set destination location
            gcmAPI.gcmWebService.BookingDestinationLocation destinationLocation = new gcmAPI.gcmWebService.BookingDestinationLocation();
            destinationLocation.Name = json.destName;
            destinationLocation.Email = json.destEmail;
            destinationLocation.Company = json.destCompany;
            destinationLocation.Phone = json.destPhone;
            destinationLocation.Fax = json.destFax;
            destinationLocation.Address1 = json.destAddress1;
            destinationLocation.Address2 = json.destAddress2;
            destinationLocation.City = json.destCity;
            destinationLocation.State = json.destState;
            destinationLocation.Zip = json.destZip;

            #endregion

            string SessionID = json.SessionID;
            logger.Append(string.Concat("SessionID: ", json.SessionID, ", "));

            //int testInt;
            List<gcmAPI.gcmWebService.LTLBookingPiece> bookingLineItems = new List<gcmAPI.gcmWebService.LTLBookingPiece>();

            for (byte i = 0; i < json.lineItems.Length; i++)
            {
                gcmAPI.gcmWebService.LTLBookingPiece lineItem = new gcmAPI.gcmWebService.LTLBookingPiece();
                lineItem.Tag = json.lineItems[i].Tag;
                lineItem.Description = json.lineItems[i].Description;
                lineItem.NumberOfPallet = json.lineItems[i].NumberOfPallet;

                // Testing
                logger.Append(string.Concat("Tag", i.ToString(), ": ", lineItem.Tag, ", "));
                logger.Append(string.Concat("Description", i.ToString(), ": ", lineItem.Description, ", "));
                logger.Append(string.Concat("NumberOfPallet", i.ToString(), ": ", lineItem.NumberOfPallet, ", "));

                bookingLineItems.Add(lineItem);
            }


            HelperFuncs.writeToSiteErrors("LTL Booking Restful", logger.ToString());

            #region Not used
            //Set other line items
            //gcmWebService.LTLBookingPiece[] bookingLineItems = new gcmWebService.LTLBookingPiece[1];

            //Set first line item for booking
            //bookingLineItems[0] = new gcmWebService.LTLBookingPiece();

            //// This tag is related to the object(LTLPiece) in lineItems array which is used in GetLTLBookingRate function call
            //bookingLineItems[0].Tag = form.Get("Tag");

            ////bookingLineItems[0].NMFC = 50;
            //bookingLineItems[0].NumberOfPallet = 1;
            //bookingLineItems[0].Description = form.Get("Description");

            //Set second line item for booking
            //bookingLineItems[1] = new MyWebReference.LTLBookingPiece();

            ////// This tag is related to the object(LTLPiece) in lineItems array which is used in GetLTLBookingRate function call
            //bookingLineItems[1].Tag = "2";

            //bookingLineItems[1].NMFC = 60;
            //bookingLineItems[1].NumberOfPallet = 10;
            //bookingLineItems[1].Description = "TEST";
            #endregion

            string comments = "TEST";
            //string comments = null;
            //bool insuranceRequired = true;
            bool insuranceRequired = false;
            double declaredValue = 100;

            // 
            //Book an LTL rate and getting the BOL and insurance certificate PDF url 
            gcmAPI.gcmWebService.LTLBookingReply lbr = rs.CreateLTLBooking2(SessionID, bookingKey, customerType, thirdPartyBilling, shipmentDate, readyTime, closeTime, bolSendTo,
                poNumber, pickupLocation, destinationLocation, bookingLineItems.ToArray(), comments, insuranceRequired, declaredValue);

            HelperFuncs.writeToSiteErrors("LTL Booking Restful result", string.Concat("notification=", lbr.Notification, "&shipmentID=", lbr.ShipmentId, "&=BOLURL", lbr.BOLURL));

            return lbr.ToJSON();

            //return string.Concat("{ \"notification\" : \"", lbr.Notification, "\", \"shipmentID\" : ", lbr.ShipmentId, ", \"BOLURL", lbr.BOLURL, " }");

            //return "test test";
        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("LTL Booking Restful", e.ToString());
            return "{ \"Notification\" : \"error\" }";
            //return lbr.ToJSON;
        }

    }

    #endregion

    #region makeBookingViaSOAP

    public string makeBookingViaSOAP(ref FormDataCollection form)
    {
        //LTLBookingInfo info = new LTLBookingInfo();
        try
        {

            #region Testing

            StringBuilder logger = new StringBuilder();
            logger.Append(string.Concat("shipmentDate: ", form.Get("shipmentDate"), ", "));
            logger.Append(string.Concat("bookingKey: ", form.Get("bookingKey"), ", "));
            logger.Append(string.Concat("customerType: ", form.Get("customerType"), ", "));

            logger.Append(string.Concat("origName: ", form.Get("origName"), ", "));
            logger.Append(string.Concat("origEmail: ", form.Get("origEmail"), ", "));
            logger.Append(string.Concat("origCompany: ", form.Get("origCompany"), ", "));
            logger.Append(string.Concat("origPhone: ", form.Get("origPhone"), ", "));
            logger.Append(string.Concat("origFax: ", form.Get("origFax"), ", "));
            logger.Append(string.Concat("origAddress1: ", form.Get("origAddress1"), ", "));
            logger.Append(string.Concat("origAddress2: ", form.Get("origAddress2"), ", "));
            logger.Append(string.Concat("origCity: ", form.Get("origCity"), ", "));
            logger.Append(string.Concat("origState: ", form.Get("origState"), ", "));
            logger.Append(string.Concat("origZip: ", form.Get("origZip"), ", "));

            logger.Append(string.Concat("destName: ", form.Get("destName"), ", "));
            logger.Append(string.Concat("destEmail: ", form.Get("destEmail"), ", "));
            logger.Append(string.Concat("destCompany: ", form.Get("destCompany"), ", "));
            logger.Append(string.Concat("destPhone: ", form.Get("destPhone"), ", "));
            logger.Append(string.Concat("destFax: ", form.Get("destFax"), ", "));
            logger.Append(string.Concat("destAddress1: ", form.Get("destAddress1"), ", "));
            logger.Append(string.Concat("destAddress2: ", form.Get("destAddress2"), ", "));
            logger.Append(string.Concat("destCity: ", form.Get("destCity"), ", "));
            logger.Append(string.Concat("destState: ", form.Get("destState"), ", "));
            logger.Append(string.Concat("destZip: ", form.Get("destZip"), ", "));

            #endregion

            DateTime shipmentDate;

            if (!DateTime.TryParse(form.Get("shipmentDate"), out shipmentDate))
            {
                shipmentDate = DateTime.Today.AddDays(1);
            }

            // Initialize web service/API object
            gcmAPI.gcmWebService.RateService2 rs = new gcmAPI.gcmWebService.RateService2();

            #region Not used
            // Authenticate to the web service/API
            //string sessionId = rs.Authenticate(apiUserName, apiKey);

            //// Initialize SOAP header for authentication
            //rs.AuthHeaderValue = new MyWebReference.AuthHeader();

            //// Set session id to the SOAP header
            //rs.AuthHeaderValue.SessionID = sessionId;
            #endregion

            string bookingKey = form.Get("bookingKey");

            string customerType = form.Get("customerType"); //Set customer type

            gcmAPI.gcmWebService.BookingThirdPartyBilling thirdPartyBilling = null;
            //DateTime shipmentDate = DateTime.Now.Date; //Set shipment date

            string readyTime = "02:15 PM"; //Set ready time of the shipment
            string closeTime = "06:15 PM"; //Set close time of the shipment

            //string readyTime = "10"; //Set ready time of the shipment
            //string closeTime = "3"; //Set close time of the shipment

            string bolSendTo = "EML"; //Set BOL sending option
            //string poNumber = "45455"; //Set PO Number
            string poNumber = null; //Set PO Number

            #region Origin and Destination

            // Set pickup location
            gcmAPI.gcmWebService.BookingPickupLocation pickupLocation = new gcmAPI.gcmWebService.BookingPickupLocation();
            pickupLocation.Name = form.Get("origName");
            pickupLocation.Email = form.Get("origEmail");
            pickupLocation.Company = form.Get("origCompany");
            pickupLocation.Phone = form.Get("origPhone");
            pickupLocation.Fax = form.Get("origFax");
            pickupLocation.Address1 = form.Get("origAddress1");
            pickupLocation.Address2 = form.Get("origAddress2");
            pickupLocation.City = form.Get("origCity");
            pickupLocation.State = form.Get("origState");
            pickupLocation.Zip = form.Get("origZip");
            pickupLocation.DispatchAddressesId = 1;

            // Set destination location
            gcmAPI.gcmWebService.BookingDestinationLocation destinationLocation = new gcmAPI.gcmWebService.BookingDestinationLocation();
            destinationLocation.Name = form.Get("destName");
            destinationLocation.Email = form.Get("destEmail");
            destinationLocation.Company = form.Get("destCompany");
            destinationLocation.Phone = form.Get("destPhone");
            destinationLocation.Fax = form.Get("destFax");
            destinationLocation.Address1 = form.Get("destAddress1");
            destinationLocation.Address2 = form.Get("destAddress2");
            destinationLocation.City = form.Get("destCity");
            destinationLocation.State = form.Get("destState");
            destinationLocation.Zip = form.Get("destZip");

            #endregion

            string SessionID = form.Get("SessionID");
            logger.Append(string.Concat("SessionID: ", form.Get("SessionID"), ", "));
            //SessionID
            int testInt;
            List<gcmAPI.gcmWebService.LTLBookingPiece> bookingLineItems = new List<gcmAPI.gcmWebService.LTLBookingPiece>();
            for (byte i = 1; i <= 20; i++)
            {
                if (form.Get("Tag" + i.ToString()) != null)
                {
                    //int.TryParse(form.Get("q_Piece" + i.ToString()), out pieces);
                    gcmAPI.gcmWebService.LTLBookingPiece lineItem = new gcmAPI.gcmWebService.LTLBookingPiece();
                    lineItem.Tag = form.Get("Tag" + i.ToString());

                    // Testing
                    logger.Append(string.Concat("Tag", i.ToString(), ": ", form.Get("Tag" + i.ToString()), ", "));

                    if (form.Get("NumberOfPallet" + i.ToString()) != null)
                    {
                        if (int.TryParse(form.Get("NumberOfPallet" + i.ToString()), out testInt))
                        {
                            lineItem.NumberOfPallet = testInt;
                        }

                        // Testing
                        logger.Append(string.Concat("NumberOfPallet", i.ToString(), ": ", form.Get("NumberOfPallet" + i.ToString()), ", "));
                    }

                    if (form.Get("Description" + i.ToString()) != null)
                    {
                        lineItem.Description = form.Get("Description" + i.ToString());

                        // Testing
                        logger.Append(string.Concat("Description", i.ToString(), ": ", form.Get("Description" + i.ToString()), ", "));
                    }
                    bookingLineItems.Add(lineItem);
                }
                else
                {
                    break;
                }
            }

            HelperFuncs.writeToSiteErrors("LTL Booking Restful", logger.ToString());

            #region Not used
            //Set other line items
            //gcmWebService.LTLBookingPiece[] bookingLineItems = new gcmWebService.LTLBookingPiece[1];

            //Set first line item for booking
            //bookingLineItems[0] = new gcmWebService.LTLBookingPiece();

            //// This tag is related to the object(LTLPiece) in lineItems array which is used in GetLTLBookingRate function call
            //bookingLineItems[0].Tag = form.Get("Tag");

            ////bookingLineItems[0].NMFC = 50;
            //bookingLineItems[0].NumberOfPallet = 1;
            //bookingLineItems[0].Description = form.Get("Description");

            //Set second line item for booking
            //bookingLineItems[1] = new MyWebReference.LTLBookingPiece();

            ////// This tag is related to the object(LTLPiece) in lineItems array which is used in GetLTLBookingRate function call
            //bookingLineItems[1].Tag = "2";

            //bookingLineItems[1].NMFC = 60;
            //bookingLineItems[1].NumberOfPallet = 10;
            //bookingLineItems[1].Description = "TEST";
            #endregion

            string comments = "TEST";
            //string comments = null;
            //bool insuranceRequired = true;
            bool insuranceRequired = false;
            double declaredValue = 100;

            //Book an LTL rate and getting the BOL and insurance certificate PDF url 
            gcmAPI.gcmWebService.LTLBookingReply lbr = rs.CreateLTLBooking2(SessionID, bookingKey, customerType, thirdPartyBilling, shipmentDate, readyTime, closeTime, bolSendTo,
                poNumber, pickupLocation, destinationLocation, bookingLineItems.ToArray(), comments, insuranceRequired, declaredValue);

            HelperFuncs.writeToSiteErrors("LTL Booking Restful result", string.Concat("notification=", lbr.Notification, "&shipmentID=", lbr.ShipmentId, "&=BOLURL", lbr.BOLURL));
            return string.Concat("notification=", lbr.Notification, "&shipmentID=", lbr.ShipmentId, "&=BOLURL", lbr.BOLURL);

        }
        catch (Exception e)
        {
            HelperFuncs.writeToSiteErrors("LTL Booking Restful", e.ToString());
            return "error";
        }

    }

    #endregion

    #endregion
}