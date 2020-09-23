#region Using

using gcmAPI.Models.Public.LTL;
using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using static gcmAPI.Models.Utilities.Mail;

#endregion

namespace gcmAPI.Models.Carriers.DLS
{
    public class DLS_ShipmentImport
    {
        #region Variables

        HelperFuncs.dlsShipInfo dlsInfo;
        HelperFuncs.DispatchInfo dispatchInfo;
        AES_API_info api_info;
        HelperFuncs.AccessorialsObj AccessorialsObj;
        string username;
        string ShipmentId;
        string iam = "DLS_ShipmentImport";
        string shipmentReference;

        #endregion

        #region Constructor

        public DLS_ShipmentImport(ref HelperFuncs.dlsShipInfo dlsInfo, ref HelperFuncs.DispatchInfo dispatchInfo, ref AES_API_info api_info,
            ref HelperFuncs.AccessorialsObj AccessorialsObj, ref string username, string ShipmentId)
        {
            this.dlsInfo = dlsInfo;
            this.dispatchInfo = dispatchInfo;
            this.api_info = api_info;
            this.AccessorialsObj = AccessorialsObj;
            this.username = username;
        }

        #endregion

        #region shipmentImportDLS

        // ShipmentImportDLS
        // Used by Genera/Restful
        public void ShipmentImportDLS(ref string DLS_PrimaryReferencePNW, ref List<AES_API_info> quote_carriers, ref StringBuilder sb_carrier_rates)
        {

            #region Get Billing Comp Info

            HelperFuncs.CompInfo billingCompInfo = new HelperFuncs.CompInfo();
            DB.Log("dispatchInfo.username", dispatchInfo.username);
            DB.Log("username", username);

            SharedLTL.GetCompInfoByUserName(username, ref billingCompInfo);

            #endregion

            #region Dates

            string puMonth = dispatchInfo.puDate.Month.ToString();
            string puDay = dispatchInfo.puDate.Day.ToString();
            string puYear = dispatchInfo.puDate.Year.ToString();

            if (puMonth.Length.Equals(1))
            {
                puMonth = string.Concat("0", puMonth);
            }

            if (puDay.Length.Equals(1))
            {
                puDay = string.Concat("0", puDay);
            }

            //--

            //DateTime delDate = DateTime.Today.AddDays(5);
            string delMonth = dispatchInfo.delDate.Month.ToString();
            string delDay = dispatchInfo.delDate.Day.ToString();
            string delYear = dispatchInfo.delDate.Year.ToString();

            if (delMonth.Length.Equals(1))
            {
                delMonth = string.Concat("0", delMonth);
            }

            if (delDay.Length.Equals(1))
            {
                delDay = string.Concat("0", delDay);
            }

            #endregion

            #region Set Ready/Close time

            byte byte_ready_hour = 15, byte_close_hour = 17;
            string ready_minute = "00", close_minute="00", ready_hour="15",close_hour="17";

            try
            {
                if (string.IsNullOrEmpty(dispatchInfo.ddlRHour) ||
                string.IsNullOrEmpty(dispatchInfo.ddlRMinute) ||
                string.IsNullOrEmpty(dispatchInfo.ddlRAMPM)
                )
                {
                    // Do nothing
                    DB.LogGenera(iam, "ready", "was null or empty");
                }
                else
                {
                    if (dispatchInfo.ddlRAMPM == "AM")
                    {                        
                        ready_hour = dispatchInfo.ddlRHour;
                    }
                    else
                    {
                        // PM
                        byte_ready_hour = Convert.ToByte(dispatchInfo.ddlRHour);
                        byte_ready_hour += 12;
                        ready_hour = byte_ready_hour.ToString();
                    }

                    ready_minute = dispatchInfo.ddlRMinute;
                }

                //

                if (string.IsNullOrEmpty(dispatchInfo.ddlCHour) ||
                string.IsNullOrEmpty(dispatchInfo.ddlCMinute) ||
                string.IsNullOrEmpty(dispatchInfo.ddlCAMPM)
                )
                {
                    // Do nothing
                    DB.LogGenera(iam, "close", "was null or empty");
                }
                else
                {
                    if (dispatchInfo.ddlCAMPM == "AM")
                    {
                        close_hour = dispatchInfo.ddlCHour;
                    }
                    else
                    {
                        // PM
                        byte_close_hour = Convert.ToByte(dispatchInfo.ddlCHour);
                        byte_close_hour += 12;
                        close_hour = byte_close_hour.ToString();
                    }

                    close_minute = dispatchInfo.ddlCMinute;
                }
            }
            catch(Exception ready_close_e)
            {
                DB.LogGenera(iam, "ready_close_e", ready_close_e.ToString());
            }
            

            #endregion

            //DB.LogGenera("DLS", "before items", "before items");

            #region Items

            int Units = 1;

            StringBuilder infoItems = new StringBuilder();
            for (byte i = 0; i < dlsInfo.Items.Length; i++)
            {
                if(api_info.m_lPiece != null && api_info.m_lPiece[i] != null && api_info.m_lPiece[i].Units > 0)
                {
                    Units = api_info.m_lPiece[i].Units;
                    DB.LogGenera("DLS Shipment Import", "api_info.m_lPiece[i].Units", Units.ToString());
                }
                else
                {
                    Units = 1;
                    DB.LogGenera("DLS Shipment Import", "api_info.m_lPiece[i].Units", "was null");
                }

                infoItems.Append(string.Concat("<ShipmentImportItemViewModel>", "<Commodity>Auto Body Parts</Commodity>"));

                #region Add Item Description

                if (i.Equals(0))
                {
                    infoItems.Append(string.Concat("<Description>", dispatchInfo.txtDesc1, "</Description>"));
                }
                else if (i.Equals(1))
                {
                    infoItems.Append(string.Concat("<Description>", dispatchInfo.txtDesc2, "</Description>"));
                }
                else if (i.Equals(2))
                {
                    infoItems.Append(string.Concat("<Description>", dispatchInfo.txtDesc3, "</Description>"));
                }
                else if (i.Equals(3))
                {
                    infoItems.Append(string.Concat("<Description>", dispatchInfo.txtDesc4, "</Description>"));
                }
                else
                {
                    infoItems.Append(string.Concat("<Description>-</Description>"));
                }

                #endregion

                infoItems.Append(string.Concat("<Dimensions>",
                                    "<Height>", dlsInfo.Items[i].height, "</Height>",
                                    "<Length>", dlsInfo.Items[i].length, "</Length>",
                                    "<Uom>In</Uom>",
                                    "<Width>", dlsInfo.Items[i].width, "</Width>",
                                "</Dimensions>",
                                "<FreightClasses>",
                                    "<FreightClass>", dlsInfo.Items[i].fClass, "</FreightClass>",
                                    "<Type />",
                                "</FreightClasses>",
                                "<HazardousMaterial>false</HazardousMaterial>",
                                "<Id>Item", (i + 1), "</Id>",
                                "<IsHandlingUnit>false</IsHandlingUnit>",
                                "<IsShipUnit>false</IsShipUnit>",
                                "<Quantities>",
                                    "<Actual>", Units, "</Actual>",
                                    //"<Delivered>1</Delivered>",
                                    //"<Ordered>1</Ordered>",
                                    //"<Planned>1</Planned>",
                                    "<Uom>Skid</Uom>",
                                "</Quantities>",
                                "<Stackability>false</Stackability>",
                                "<Weights>",
                                    "<Actual>", dlsInfo.Items[i].weight, "</Actual>",
                                    "<Uom>lbs</Uom>",
                                "</Weights>", "</ShipmentImportItemViewModel>"));
            }

            #endregion


            #region Scac and CarrierQuoteID
            // Get scac by carrier name
            string SCAC = string.Empty;

            SCAC = api_info.SCAC;
            //SharedLTL.GetRRD_ScacByCarrierName(dispatchInfo.carrier.Trim(), ref SCAC);

            DB.LogGenera(iam, "api_info.SCAC", string.Concat("carrier: ",
                dispatchInfo.carrier.Trim(), " scac: ", SCAC));
            //DB.LogGenera(iam, "api_info.SCAC", string.Concat("carrier: ",
            //  dispatchInfo.carrier.Trim(), " scac: ", SCAC));
            //
            //CarrierQuoteID
            string CarrierQuoteID = string.Empty;

            if (string.IsNullOrEmpty(api_info.CarrierQuoteID))
            {
                DB.LogGenera(iam, "api_info.CarrierQuoteID", string.Concat("carrier: ",
                dispatchInfo.carrier.Trim(), " CarrierQuoteID: Was null or empty "));
            }
            else
            {
                CarrierQuoteID = api_info.CarrierQuoteID;
                DB.LogGenera(iam, "api_info.CarrierQuoteID", string.Concat("carrier: ",
                dispatchInfo.carrier.Trim(), " CarrierQuoteID: ", CarrierQuoteID));
            }

            //SharedLTL.GetRRD_ScacByCarrierName(dispatchInfo.carrier.Trim(), ref SCAC);


            //
            #endregion


            #region PO Numbers

            string bol_po = "";

            StringBuilder poNums = new StringBuilder();
            //bool IsPrimary = true;
            //string poType = "PONumber";

            if(!string.IsNullOrEmpty(dispatchInfo.txtPONumber))
            {
                bol_po = dispatchInfo.txtPONumber;

                string[] forSplit = dispatchInfo.txtPONumber.Trim().Split(',');
                /*
                poNums.Append(string.Concat("<ShipmentImportReferenceNumberViewModel><IsPrimary>"));

                //if (i.Equals(0))
                //{
                //    poNums.Append("true");
                //}
                //else
                //{
                //    poNums.Append("false");
                //}

                //poNums.Append("false");

                poNums.Append("true");

                poNums.Append(string.Concat("</IsPrimary><ReferenceNumber>", dispatchInfo.txtPONumber, "</ReferenceNumber><Type>"));

                poNums.Append("PONumber");
                
                poNums.Append("</Type></ShipmentImportReferenceNumberViewModel>");
                */
                for (byte i = 0; i < forSplit.Length; i++)
                {
                    poNums.Append(string.Concat("<ShipmentImportReferenceNumberViewModel><IsPrimary>"));

                    if (i.Equals(0))
                    {
                        poNums.Append("true");
                    }
                    else
                    {
                        poNums.Append("false");
                    }

                    poNums.Append(string.Concat("</IsPrimary><ReferenceNumber>", forSplit[i].Trim(), "</ReferenceNumber><Type>"));

                    poNums.Append("PO Number");

                    #region Not used
                    //if (i.Equals(0))
                    //{
                    //    poNums.Append("PONumber");
                    //}
                    //else
                    //{
                    //    poNums.Append("ShipmentReference");
                    //}
                    #endregion

                    poNums.Append("</Type></ShipmentImportReferenceNumberViewModel>");

                }

            }

            // Add Shipment ID as PO/REF number
            //poNums.Append(string.Concat("<ShipmentImportReferenceNumberViewModel><IsPrimary>false</IsPrimary><ReferenceNumber>",
            //    dispatchInfo.ShipmentID,
            //    "</ReferenceNumber><Type>ShipmentReference</Type></ShipmentImportReferenceNumberViewModel>"));

            //foreach (string strPONo in txtPONumber.Text.Trim().Split(','))
            //{

            //}

            #endregion

            if (!string.IsNullOrEmpty(CarrierQuoteID))
            {
                poNums.Append(string.Concat("<ShipmentImportReferenceNumberViewModel><IsPrimary>false</IsPrimary><ReferenceNumber>",
                CarrierQuoteID,
                "</ReferenceNumber><Type>QuoteNumber</Type></ShipmentImportReferenceNumberViewModel>"));
            }
            // ShipmentReference primaryReferenceBol
            if (!string.IsNullOrEmpty(shipmentReference))
            {
                poNums.Append(string.Concat("<ShipmentImportReferenceNumberViewModel><IsPrimary>false</IsPrimary><ReferenceNumber>",
                shipmentReference,
                "</ReferenceNumber><Type>ShipmentReference</Type></ShipmentImportReferenceNumberViewModel>"));
            }

            //

            if (!string.IsNullOrEmpty(api_info.proNumber))
            {
                poNums.Append(string.Concat("<ShipmentImportReferenceNumberViewModel><IsPrimary>false</IsPrimary><ReferenceNumber>",
                api_info.proNumber,
                "</ReferenceNumber><Type>PRO</Type></ShipmentImportReferenceNumberViewModel>"));
            }

            //"Type": "PO Number"
            #region Accessorials

            //bool blnResult;
            StringBuilder accessorials = new StringBuilder();

            if (!string.IsNullOrEmpty(dispatchInfo.rateType) && dispatchInfo.rateType.Equals("GUARANTEEDPM"))
            {
                accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>GLTL</ServiceCode></ServiceFlagViewModel>"));
            }

            if (dispatchInfo.isHazmat.Equals(true))
            {
                accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>HAZM</ServiceCode></ServiceFlagViewModel>"));
            }

            if (AccessorialsObj.RESPU.Equals(true))
            {
                accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>RES2</ServiceCode></ServiceFlagViewModel>"));
            }

            if (AccessorialsObj.RESDEL.Equals(true))
            {
                accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>RES1</ServiceCode></ServiceFlagViewModel>"));
            }

            //if ((requestedValues["q_ConstPick"] != null && bool.TryParse(requestedValues["q_ConstPick"], out blnResult) && blnResult.Equals(true)) ||
            //    (requestedValues["q_ConstDel"] != null && bool.TryParse(requestedValues["q_ConstDel"], out blnResult) && blnResult.Equals(true)))
            //{
            //    accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>CONSL</ServiceCode></ServiceFlagViewModel>")); 
            //}

            //if (AccessorialsObj.CONPU.Equals(true))
            //{
            //    accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>LMAC1</ServiceCode></ServiceFlagViewModel>"));
            //}

            //if (AccessorialsObj.CONDEL.Equals(true))
            //{
            //    accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>LMAC2</ServiceCode></ServiceFlagViewModel>"));
            //}


            if (AccessorialsObj.CONPU.Equals(true))
            {
                accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>LAPCONS</ServiceCode></ServiceFlagViewModel>"));
            }

            if (AccessorialsObj.CONDEL.Equals(true))
            {
                accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>LADCONS</ServiceCode></ServiceFlagViewModel>"));
            }



            //if (requestedValues["q_ConstDel"] != null && bool.TryParse(requestedValues["q_ConstDel"], out blnResult))
            //{
            //    accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>ID1</ServiceCode></ServiceFlagViewModel>")); 
            //}

            if (AccessorialsObj.TRADEPU.Equals(true))
            {
                accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>CONV2</ServiceCode></ServiceFlagViewModel>"));
            }

            if (AccessorialsObj.TRADEDEL.Equals(true))
            {
                accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>CONV1</ServiceCode></ServiceFlagViewModel>"));
            }

            if (AccessorialsObj.LGPU.Equals(true))
            {
                accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>LG2</ServiceCode></ServiceFlagViewModel>"));
            }

            if (AccessorialsObj.LGDEL.Equals(true))
            {
                accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>LG1</ServiceCode></ServiceFlagViewModel>"));
            }

            if (AccessorialsObj.APTPU.Equals(true) || AccessorialsObj.APTDEL.Equals(true))
            {
                accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>APPT</ServiceCode></ServiceFlagViewModel>"));
            }

            //if (requestedValues["q_AppDel"] != null && bool.TryParse(requestedValues["q_AppDel"], out blnResult))
            //{
            //    accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>APPT</ServiceCode></ServiceFlagViewModel>")); 
            //}

            //if (requestedValues["q_InsPick"] != null && bool.TryParse(requestedValues["q_InsPick"], out blnResult))
            //{
            //    accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>ID1</ServiceCode></ServiceFlagViewModel>")); 
            //}

            if (AccessorialsObj.INSDEL.Equals(true))
            {
                accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>IDL</ServiceCode></ServiceFlagViewModel>"));
            }

            #endregion

            #region Post data

            string Mode = "LTL";

            if(SCAC == "DRRQ")
            {
                Mode = "TL";
            }
            else if (!string.IsNullOrEmpty(api_info.CarrierQuoteID))
            {
                //service = "Volume";
                // Volume
                DB.LogGenera("DLS_ShipmentImport", "volume", "volume");
                if (SCAC == "FXNL" || SCAC == "FXFE")
                {
                    DB.LogGenera("DLS_ShipmentImport", "fedex scac set tl", "fedex scac set tl");
                    Mode = "TL";
                }
                
            }

            DB.LogGenera("DLS_ShipmentImport","Mode", Mode);

            string data = string.Concat("<?xml version=\"1.0\" encoding=\"utf-8\"?>",
                "<ShipmentImportViewModel xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.datacontract.org/2004/07/Rrdl.Dls.Proxy\">",

                    "<Consignee>",
            #region Consignee
 "<AddressLine1>", dispatchInfo.txtDAddress1, "</AddressLine1>",
 "<AddressLine2>", dispatchInfo.txtDAddress2, "</AddressLine2>",
                            //"<AddressLine2 />",
                            "<City>", dlsInfo.toCity, "</City>",
                            "<Comments />",
                            "<Contact>",
                                "<ContactType /><Email />",
                                "<Fax />",
                                //"<Name />",
                                "<Name>", HelperFuncs.XmlEscape(dispatchInfo.txtDName), "</Name>",
                                //"<Phone />",
                                "<Phone>", dlsInfo.toPhone, "</Phone>",
                            "</Contact>",
                            "<CountryCode>", dlsInfo.toCountryCode, "</CountryCode>", //USA
                                                                                      //"<EarliestAppointmentTime>", puYear, "-", puMonth, "-", puDay, "T13:53:04.0179088</EarliestAppointmentTime>",
                            "<IsResidential>false</IsResidential>", // IsResidential
                                                                    //"<LatestAppointmentTime>", delYear, "-", delMonth, "-", delDay, "T13:53:04.0179088</LatestAppointmentTime>",
                            "<LocationCode />",
                            //"<Name>", dispatchInfo.txtDCompany, "</Name>",
                            "<Name>", HelperFuncs.XmlEscape(dlsInfo.toName), " </Name>",
                            //dlsInfo.toName
                            "<PostalCode>", dlsInfo.toPostalCode, "</PostalCode>",
                            "<StateProvince>", dlsInfo.toState, "</StateProvince>",
                            "<Type>Consignee</Type>",
            #endregion
 "</Consignee>",
                    "<Dates>",
            #region Dates
 "<EarliestDropDate>", delYear, "-", delMonth, "-", delDay, "T13:00:00.0000000</EarliestDropDate>",
                        //"<EarliestPickupDate>", puYear, "-", puMonth, "-", puDay, "T09:00:00.0000000</EarliestPickupDate>",
                        //"<LatestDropDate>", delYear, "-", delMonth, "-", delDay, "T17:00:00.0000000</LatestDropDate>",
                        //"<LatestPickupDate>", puYear, "-", puMonth, "-", puDay, "T10:00:00.0000000</LatestPickupDate>",


            //"<EarliestPickupDate>", puYear, "-", puMonth, "-", puDay, "T15:00:00.0000000</EarliestPickupDate>", // Set dynamic time

            "<EarliestPickupDate>", puYear, "-", puMonth, "-", puDay, "T", ready_hour, ":", ready_minute, ":00.0000000</EarliestPickupDate>",
                        "<LatestDropDate>", delYear, "-", delMonth, "-", delDay, "T17:00:00.0000000</LatestDropDate>",

            //"<LatestPickupDate>", puYear, "-", puMonth, "-", puDay, "T17:00:00.0000000</LatestPickupDate>", // Set dynamic time

            "<LatestPickupDate>", puYear, "-", puMonth, "-", puDay, "T", close_hour, ":", close_minute, ":00.0000000</LatestPickupDate>",

            #endregion
 "</Dates>",
                    "<Items>", infoItems,
                    "</Items>",
                    "<Payment>",

                    "<Address>",
            #region Address
 "<AddressLine1>", HelperFuncs.XmlEscape(billingCompInfo.Addr1), "</AddressLine1>",
                                    //"<AddressLine2 />",
                                    "<AddressLine2>", HelperFuncs.XmlEscape(billingCompInfo.Addr2), "</AddressLine2>",
                                    "<City>", HelperFuncs.XmlEscape(billingCompInfo.City), "</City>",
                                    "<Comments />",
                                    "<Contact>",
                                        //"<Contact>", billingCompInfo.Contact, "</Contact>",
                                        "<ContactType />",
                                        //"<Email />",
                                        "<Email>", HelperFuncs.XmlEscape(billingCompInfo.EMail), "</Email>",
                                        //"<Fax />",
                                        "<Fax>", HelperFuncs.XmlEscape(billingCompInfo.Fax), "</Fax>",
                                        "<Name>", HelperFuncs.XmlEscape(billingCompInfo.Contact), "</Name>",
                                        //"<Name />",
                                        //"<Phone />",
                                        "<Phone>", HelperFuncs.XmlEscape(billingCompInfo.Phone), "</Phone>",
                                    "</Contact>",
                                    "<CountryCode>USA</CountryCode>",
                                    "<IsResidential>false</IsResidential>", //IsResidential
                                    "<LocationCode />",
                                    "<Name>", HelperFuncs.XmlEscape(billingCompInfo.CompName), "</Name>",
                                    "<PostalCode>", HelperFuncs.XmlEscape(billingCompInfo.Zip), "</PostalCode>",
                                    "<StateProvince>", HelperFuncs.XmlEscape(billingCompInfo.State), "</StateProvince>",
            #endregion
 "</Address>",
                        "<IsThirdParty />",
                        "<Method>Third Party</Method>",
                    "</Payment>",
                    "<Pricesheets>",
                    "<ShipmentImportPricesheetViewModel>",
                    "<ContractId />",
                    "<IsSelected>true</IsSelected>",
                    "<Mode>", Mode, "</Mode>",
                    "<Scac>", SCAC.Trim(), "</Scac>", // Scac
                    "<Service>2nd Day</Service><Type>Carrier</Type>", // ?
                    "</ShipmentImportPricesheetViewModel>",
                    "</Pricesheets>",
                    "<ReferenceNumbers>", poNums,
            #region ReferenceNumbers
                //"<ShipmentImportReferenceNumberViewModel>",
                //        "<IsPrimary>true</IsPrimary>",
                //        "<ReferenceNumber>12345</ReferenceNumber>",
                //        "<Type>PONumber</Type>",
                // "</ShipmentImportReferenceNumberViewModel>",
                // "<ShipmentImportReferenceNumberViewModel>",
                //        "<IsPrimary>false</IsPrimary>",
                //        "<ReferenceNumber>987987</ReferenceNumber>",
                //        "<Type>ShipmentReference</Type>",
                //"</ShipmentImportReferenceNumberViewModel>",
            #endregion
                "</ReferenceNumbers>",
                    "<ServiceFlags>", accessorials,
                    //"<ServiceFlagViewModel>",
                    //"<ServiceCode>ID1</ServiceCode>", //ServiceCode - Accessorials
                    //"</ServiceFlagViewModel>",
                    "</ServiceFlags>",

                    "<Shipper>",
            #region Shipper
 "<AddressLine1>", dispatchInfo.txtPAddress1, "</AddressLine1>",
                        "<AddressLine2 />",
                        "<City>", dlsInfo.fromCity, "</City>",
                        "<Comments />",
                        "<Contact>",
                            "<ContactType />",
                            "<Email />",
                            "<Fax />",
                            //"<Name />",
                            "<Name>", HelperFuncs.XmlEscape(dispatchInfo.txtPName), "</Name>",
                            //"<Phone />",
                            "<Phone>", dlsInfo.fromPhone, "</Phone>",
                        "</Contact>",
                        "<CountryCode>", dlsInfo.fromCountryCode, "</CountryCode>",
                        //"<EarliestAppointmentTime>", puYear, "-", puMonth, "-", puDay, "T13:53:04.0179088</EarliestAppointmentTime>",
                        "<IsResidential>false</IsResidential>", // IsResidential
                                                                //"<LatestAppointmentTime>", puYear, "-", puMonth, "-", puDay, "T13:53:04.0179088</LatestAppointmentTime>",
                        "<LocationCode />",
                        "<Name>", HelperFuncs.XmlEscape(dispatchInfo.txtPCompany), "</Name>",
                        "<PostalCode>", dlsInfo.fromPostalCode, "</PostalCode>",
                        "<StateProvince>", dlsInfo.fromState, "</StateProvince>",
                        "<Type>shipper</Type>",
            #endregion
 "</Shipper>",
                    //"<Status>Pending</Status>", // Status
                    "<Status>Pending</Status>", // Status
                "</ShipmentImportViewModel>");

            DB.LogGenera(iam, "RRD booking post data GCM API", data);

            #endregion

            #region Authentication Headers

            Logins.Login_info login_info;
            Logins logins = new Logins();
            logins.Get_login_info(109, out login_info);
            
            string UserName = login_info.username, APIKey = login_info.API_Key;

            string[] headerNames = new string[2];
            string[] headerValues = new string[2];

            headerNames[0] = "UserName";
            headerNames[1] = "APIKey";
            // headerNames[2] = "CustomerAccountNumber";

            headerValues[0] = UserName;
            headerValues[1] = APIKey; 
                
            #endregion

            //string url = "https://dlsworldwideproxy-stage.rrd.com/services/api/v1/Shipment/Import";
            string url = "https://dlsworldwideproxy.rrd.com/services/api/v1/Shipment/Import";

            string doc = (string)HelperFuncs.generic_http_request_addHeaders("string", null, url, "", "application/xml", "text/xml", "POST",
               data, false, headerNames, headerValues);

            DB.LogGenera(iam, "RRD booking response data GCM API", doc);

            #region Get PrimaryReference Number

            // Gather results into an object
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(doc);

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("PrimaryReference");

            //string PrimaryReference = string.Empty; // PrimaryReference

            if (nodeList != null && nodeList.Count > 0)
            {
                //PrimaryReference = nodeList[0].InnerText;
                DLS_PrimaryReferencePNW = nodeList[0].InnerText;
            }

            DB.Log("RRD booking PrimaryReference", DLS_PrimaryReferencePNW);

            string service = "LTL";

            if (SCAC == "DRRQ")
            {
                service = "TL";
            }
            else if (!string.IsNullOrEmpty(api_info.CarrierQuoteID))
            {
                service = "Volume";
            }

            if (!DLS_PrimaryReferencePNW.Equals(string.Empty))
            {
                //
                try
                {
                    string BuyRate = api_info.BuyRate;

                    if (service == "TL")
                    {
                        BuyRate = "";
                    }

                    //
                    string buy_rate_line = "";
                    if (service == "Volume")
                    {
                        buy_rate_line = string.Concat("<span style='font-weight: bold;'>Buy Rate</span>: $", BuyRate, "<br><br>");
                    }
                    //
                    
                    //string.Format("{0:0.00}", api_info.Rate)
                    EmailInfo email_info = new EmailInfo();
                    email_info.subject = DLS_PrimaryReferencePNW + " has been booked by Genera";

                    if(AppCodeConstants.mode=="demo")
                        email_info.to = AppCodeConstants.Alex_email;                    
                    else
                        email_info.to = string.Concat(AppCodeConstants.BobsEmail, " ", AppCodeConstants.AnnesEmail, " ",
                            "HilaryH", AppCodeConstants.email_domain);

                    email_info.fromAddress = AppCodeConstants.Alex_email;
                    email_info.body = string.Concat("Genera has booked ", DLS_PrimaryReferencePNW, " shipment<br><br>",
                        //shipmentReference
                        "<span style='font-weight: bold;'>BOL/PO #</span>: ", bol_po, "<br><br>",
                        "<span style='font-weight: bold;'>Carrier</span>: ", api_info.CarrierDisplayName.Replace("%2C", ","), "<br><br>",
                "<span style='font-weight: bold;'>Pickup Date</span>: ", dispatchInfo.puDate.ToShortDateString(), "<br><br>",
                "<span style='font-weight: bold;'>Service</span>: ", service, "<br><br>",
                "<span style='font-weight: bold;'>Pallets</span>: ", api_info.total_units, "<br><br>",
                "<span style='font-weight: bold;'>Weight</span>: ", api_info.total_weight, "<br><br>",
                //"<span style='font-weight: bold;'>Buy Rate</span>: $", BuyRate, "<br><br>",//"(Truckload will not have a buy rate - LTL and Volume will)",
                buy_rate_line,
                "<span style='font-weight: bold;'>Sell Rate</span>: $", api_info.Rate, "<br><br>",
                "<span style='font-weight: bold;'>Origin DC State</span>: ", dlsInfo.fromState, "<br><br>",
                "<span style='font-weight: bold;'>Consignee zip code</span>: ", dlsInfo.toPostalCode, "<br><br>",
                "<span style='font-weight: bold;'>Consignee Company</span>: ", dlsInfo.toName, "<br><br>",
                 sb_carrier_rates

                );
                    email_info.fromName = "Genera Booking";
                    //email_info.
                    Mail mail = new Mail(ref email_info);
                    mail.SendEmail();
                }
                catch (Exception ex)
                {
                    DB.LogGenera("SendEmail", "SendEmail", ex.ToString());
                }
                //
            }

            //if (!DLS_PrimaryReferencePNW.Equals(string.Empty))
            //{
            // Get the BOL
            //getBOL_DLS(ref DLS_PrimaryReferencePNW, ref CustomerAccountNumber, ref UserName, ref APIKey);
            //}

            #endregion

        }

        #endregion

        #region shipmentImportDLS

        // ShipmentImportDLS
        // Used by Genera/Restful
        public void test_email(ref string DLS_PrimaryReferencePNW, ref List<AES_API_info> quote_carriers, ref StringBuilder sb_carrier_rates)
        {
            //
            try
            {
                string BuyRate = api_info.BuyRate;

                //if (service == "TL")
                //{
                //    BuyRate = "";
                //}
                //string.Format("{0:0.00}", api_info.Rate)
                EmailInfo email_info = new EmailInfo();
                email_info.subject = DLS_PrimaryReferencePNW + " has been booked by Genera";
                
                email_info.to = AppCodeConstants.Alex_email;
                email_info.bcc = AppCodeConstants.Alex_email;
                email_info.fromAddress = AppCodeConstants.Alex_email;

                email_info.body = string.Concat("Genera has booked ", DLS_PrimaryReferencePNW, " shipment<br><br>",
                    "<span style='font-weight: bold;'>Carrier</span>: ", api_info.CarrierDisplayName.Replace("%2C", ","), "<br><br>",
            "<span style='font-weight: bold;'>Pickup Date</span>: ", dispatchInfo.puDate.ToShortDateString(), "<br><br>",
            "<span style='font-weight: bold;'>Service</span>: ", "service", "<br><br>",
            "<span style='font-weight: bold;'>Pallets</span>: ", api_info.total_units, "<br><br>",
            "<span style='font-weight: bold;'>Weight</span>: ", api_info.total_weight, "<br><br>",
            "<span style='font-weight: bold;'>Buy Rate</span>: $", BuyRate, "<br><br>",//"(Truckload will not have a buy rate - LTL and Volume will)",
            "<span style='font-weight: bold;'>Sell Rate</span>: $", api_info.Rate, "<br><br>",
            "<span style='font-weight: bold;'>Origin DC State</span>: ", dlsInfo.fromState, "<br><br>",
            "<span style='font-weight: bold;'>Consignee zip code</span>: ", dlsInfo.toPostalCode, "<br><br>",
            "<span style='font-weight: bold;'>Consignee Company</span>: ", dlsInfo.toName, "<br><br>",
            sb_carrier_rates

            );
                email_info.fromName = "Genera Booking";
                //email_info.
                Mail mail = new Mail(ref email_info);
                mail.SendEmail();
            }
            catch (Exception ex)
            {
                DB.LogGenera("SendEmail", "SendEmail", ex.ToString());
            }
            //
        }

        #endregion

        #region Set_DLS_ShipmentImport_objects

        public void Set_DLS_ShipmentImport_objects(ref LTLBookRequest ltl_book_request, ref AES_API_info api_info)
        {
            //string origCountry = HelperFuncs.GetCountryByZip(ltl_book_request.originZip, true, ltl_book_request.originState, 
            //    ltl_book_request.destinationState);
            //string destCountry = HelperFuncs.GetCountryByZip(trueDestZip, false, quoteData.origState, quoteData.destState);

            gcmAPI.Models.Utilities.Repository repo = new gcmAPI.Models.Utilities.Repository();
            string orig_country = repo.Get_country(ltl_book_request.originZip, ltl_book_request.originState);
            string dest_country = repo.Get_country(ltl_book_request.destinationZip, ltl_book_request.destinationState);



            DB.LogGenera("Set_DLS_ShipmentImport_objects", "orig_country", orig_country);
            DB.LogGenera("Set_DLS_ShipmentImport_objects", "dest_country", dest_country);

            //if (!orig_country.Contains("US"))
            //{
            //    originCountry = quoteData.origCountry;
            //}

            //if (!quoteData.destCountry.Contains("US"))
            //{
            //    destinationCountry = quoteData.destCountry;
            //}

            //if(orig_country == "US")

            shipmentReference = "";
            if (!string.IsNullOrEmpty(ltl_book_request.shipmentReference))
            {
                shipmentReference = ltl_book_request.shipmentReference;
            }

            dlsInfo.fromAddressLine1 = ltl_book_request.originAddress1;
            dlsInfo.fromAttentionName = ltl_book_request.originName;
            dlsInfo.fromName = ltl_book_request.originName;
            dlsInfo.fromCity = ltl_book_request.originCity;
            dlsInfo.fromState = ltl_book_request.originState;
            dlsInfo.fromPostalCode = ltl_book_request.originZip;
            dlsInfo.fromCountryCode = "USA";
            if (orig_country == "CANADA")
            {
                dlsInfo.fromCountryCode = "CANADA";
            }

            dlsInfo.fromPhone = ltl_book_request.originPhone;

            dlsInfo.toAddressLine1 = ltl_book_request.destinationAddress1;
            //dlsInfo.toAddressLine2 = ltl_book_request.destinationAddress2;
            dlsInfo.toAttentionName = ltl_book_request.destinationName;
            dlsInfo.toName = ltl_book_request.destinationCompany;
            dlsInfo.toCity = ltl_book_request.destinationCity;
            dlsInfo.toState = ltl_book_request.destinationState;
            dlsInfo.toPostalCode = ltl_book_request.destinationZip;
            dlsInfo.toCountryCode = "USA";
            if (dest_country == "CANADA")
            {
                dlsInfo.toCountryCode = "CANADA";
            }

            dlsInfo.toPhone = ltl_book_request.destinationPhone;

            //

            dispatchInfo.txtDesc1 = "";
            dispatchInfo.txtDesc2 = "";
            dispatchInfo.txtDesc3 = "";
            dispatchInfo.txtDesc4 = "";
            bool isHazmat = false;

            dlsInfo.Items = new HelperFuncs.dlsItem[ltl_book_request.items.Count];

            for (byte i = 0; i < ltl_book_request.items.Count; i++)
            {
                #region Get freight class

                //dlsInfo.Items[i].fClass = ltl_book_request.items[i].freightClass.ToString();
                try
                {
                    string quote_id = "";
                    if (string.IsNullOrEmpty(api_info.QuoteId))
                    {
                        // Do nothing
                    }
                    else
                    {
                        quote_id = api_info.QuoteId;
                    }

                    if (api_info.m_lPiece == null)
                    {
                        DB.LogGenera("Set_DLS_ShipmentImport_objects",
                            "FreightClass from quotes quote_id " + quote_id, "api_info.m_lPiece = null");

                    }
                    else
                    {
                        DB.LogGenera("Set_DLS_ShipmentImport_objects",
                           "api_info.m_lPiece.Length", api_info.m_lPiece.Length.ToString());

                        DB.LogGenera("Set_DLS_ShipmentImport_objects",
                           "ltl_book_request.items.Count", ltl_book_request.items.Count.ToString());

                        if (!string.IsNullOrEmpty(api_info.m_lPiece[i].FreightClass))
                        {
                            dlsInfo.Items[i].fClass = api_info.m_lPiece[i].FreightClass;
                            DB.LogGenera("Set_DLS_ShipmentImport_objects",
                                "FreightClass from quotes quote_id " + quote_id, dlsInfo.Items[i].fClass);
                        }
                        else
                        {
                            dlsInfo.Items[i].fClass = "50";
                            DB.LogGenera("Set_DLS_ShipmentImport_objects",
                                "FreightClass from quotes quote_id " + quote_id, "Not found, defaulting to 50");
                        }
                    }

                }
                catch (Exception freight_class_e)
                {
                    dlsInfo.Items[i].fClass = "50";
                    DB.LogGenera("Set_DLS_ShipmentImport_objects", "FreightClass from quotes", freight_class_e.ToString());
                }

                #endregion

                dlsInfo.Items[i].weight = ltl_book_request.items[i].weight.ToString();

                //dlsInfo.Items[i].length = ltl_book_request.items[i].length.ToString();
                //dlsInfo.Items[i].width = ltl_book_request.items[i].width.ToString();
                //dlsInfo.Items[i].height = ltl_book_request.items[i].height.ToString();

                dlsInfo.Items[i].length = "42";
                dlsInfo.Items[i].width = "48";
                dlsInfo.Items[i].height = "72";

                //

                #region Set item descriptions 

                string total_cube = "",remainder="";
                string[] for_split;

                try
                {
                    api_info.total_cube *= 1.5;

                    DB.LogGenera("DLS Shipment Import", "total cube", 
                        String.Format("{0:.##}", Math.Round(api_info.total_cube, 2)));

                    for_split = String.Format("{0:.##}", Math.Round(api_info.total_cube, 2)).Split('.');

                    if (for_split.Length == 2)
                    {
                        DB.LogGenera("DLS Shipment Import", "for_split length", "2");
                        remainder = for_split[1];
                        if (remainder.Length == 1)
                        {
                            remainder = remainder + "0";
                        }

                        total_cube = for_split[0] + "." + remainder;
                    }
                    else
                    {
                        DB.LogGenera("DLS Shipment Import", "for_split length", for_split.Length.ToString());
                    }
                }
                catch(Exception e_1)
                {
                    DB.LogGenera("DLS Shipment Import", "descriptions", e_1.ToString());
                }

                
                
                string desc_text = string.Concat("Auto Parts // Cube ", total_cube);


                if (i == 0)
                {
                    //dispatchInfo.txtDesc1 = ltl_book_request.items[i].description;
                    dispatchInfo.txtDesc1 = desc_text;

                }
                else if (i == 1)
                {
                    //dispatchInfo.txtDesc2 = ltl_book_request.items[i].description;
                    dispatchInfo.txtDesc2 = desc_text;
                }
                else if (i == 2)
                {
                    //dispatchInfo.txtDesc3 = ltl_book_request.items[i].description;
                    dispatchInfo.txtDesc3 = desc_text;
                }
                else if (i == 3)
                {
                    //dispatchInfo.txtDesc4 = ltl_book_request.items[i].description;
                    dispatchInfo.txtDesc4 = desc_text;
                }

                if(ltl_book_request.items[i].hazmat == true)
                {
                    isHazmat = true;
                }

                #endregion
            }

            //

            dispatchInfo.username = username;
            dispatchInfo.puDate = ltl_book_request.pickupDate;
            dispatchInfo.delDate = ltl_book_request.pickupDate;

            dispatchInfo.ddlRHour = ltl_book_request.ddlRHour;
            dispatchInfo.ddlRMinute = ltl_book_request.ddlRMinute;
            dispatchInfo.ddlRAMPM = ltl_book_request.ddlRAMPM;
            dispatchInfo.ddlCHour = ltl_book_request.ddlCHour;
            dispatchInfo.ddlCMinute = ltl_book_request.ddlCMinute;
            dispatchInfo.ddlCAMPM = ltl_book_request.ddlCAMPM;
            // This is empty
            //dispatchInfo.txtPONumber = ltl_book_request.poNumber;

            dispatchInfo.txtPONumber = "-";

            if (!string.IsNullOrEmpty(ltl_book_request.poNumber))
            {
                dispatchInfo.txtPONumber = ltl_book_request.poNumber;
            }

            if (!string.IsNullOrEmpty(ltl_book_request.proNumber))
            {
                api_info.proNumber = ltl_book_request.proNumber;
            }
            else
            {
                api_info.proNumber = "";
            }

            if (!int.TryParse(ShipmentId, out dispatchInfo.ShipmentID))
                dispatchInfo.ShipmentID = 0;
            
            dispatchInfo.rateType = ltl_book_request.rateType;
            dispatchInfo.isHazmat = isHazmat;
            dispatchInfo.carrier = api_info.CarrierDisplayName;

            dispatchInfo.txtDAddress1 = ltl_book_request.destinationAddress1;
            dispatchInfo.txtDAddress2 = ltl_book_request.destinationAddress2;

            DB.LogGenera("Set_DLS_ShipmentImport_objects", "txtDAddress2", ltl_book_request.destinationAddress2);

            dispatchInfo.txtDName = ltl_book_request.destinationName;
            dispatchInfo.txtDCompany = ltl_book_request.destinationName;

            dispatchInfo.txtPAddress1 = ltl_book_request.originAddress1;
            dispatchInfo.txtPName = ltl_book_request.originName;

            try
            {
                //DB.LogGenera("Set_DLS_ShipmentImport_objects username", username);
                if (ltl_book_request.originCompany != null)
                {
                    dispatchInfo.txtPCompany = ltl_book_request.originCompany;
                    DB.LogGenera("Set_DLS_ShipmentImport_objects " + username, "ltl_book_request.originCompany",
                        ltl_book_request.originCompany + " " + ltl_book_request.originName);
                }
                else
                {
                    dispatchInfo.txtPCompany = ltl_book_request.originName;
                    DB.LogGenera("Set_DLS_ShipmentImport_objects " + username, "ltl_book_request.originName, comp was null",
                        ltl_book_request.originName);
                }
            }
            catch(Exception e)
            {
                DB.LogGenera("Set_DLS_ShipmentImport_objects", "e", e.ToString());
            }

        }

        #endregion
    }
}