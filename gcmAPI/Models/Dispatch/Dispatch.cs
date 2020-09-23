#region Using

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using gcmAPI.Models.Misc;
using gcmAPI.Models.Utilities;

#endregion

namespace gcmAPI.Models.Dispatch
{
    public class Dispatch
    {
        #region Variables

        DispatchData dispatch_data;
        SqlCommand comm = new SqlCommand();
        private static string mode = "demo";
        private static string demo_table_name_addition = "_DEMO";

        #endregion

        #region Constructor
        public Dispatch(ref DispatchData dispatch_data)
        {
            this.dispatch_data = dispatch_data;
        }
        #endregion

        #region SaveDispatchInformation

        public void SaveDispatchInformation()
        {
            string strSQL;
            object objData;
            int companyLoginID = -1;
            int intTPAddressID = -1, intPAddressID = -1, intDAddressID = -1, intDispatchID = -1;

            object objID;

            //int nmfcValue;
            //int palletValue;

            //Store Dispatch Basic Info

            using (SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aesmain_dataConnectionStringSS"].ConnectionString))
            {
                conn.Open();
                comm.Connection = conn;
                //SqlDataReader dr;// = comm.ExecuteReader();

                /////////////////////Save Dispatch Information for BOL Report/////////////
                if (String.IsNullOrEmpty(Helper.GetSpecialState(ref dispatch_data)))
                {
                    SaveDispatchInformationForBOL();
                }
                else
                {
                    SaveDispatchInformationForSpecialBOL();
                }
                //////////////////////////////////////////////////////////////////////////

                strSQL = "select CompanyLoginId from tbl_LOGIN where UserName='" + dispatch_data.username.ToString() + "'";
                comm.CommandText = strSQL;
                objData = comm.ExecuteScalar();

                if (objData != null)
                {
                    companyLoginID = Convert.ToInt32(objData);


                    object objTempID;

                    if (dispatch_data.ddlPickupAddress != "-1")
                    {
                        intPAddressID = Convert.ToInt32(dispatch_data.ddlPickupAddress);
                        // Update the table with the new info
                        strSQL = "UPDATE tbl_DispatchAddresses SET "
                        + " CompanyName=" + DB.GetSingleQuoteValue(dispatch_data.txtPCompany.Trim())
                        + ", Address1=" + DB.GetSingleQuoteValue(dispatch_data.txtPAddress1.Trim())
                        + ", Address2=" + DB.GetSingleQuoteValue(dispatch_data.txtPAddress2.Trim())
                        + ", City=" + DB.GetSingleQuoteValue(dispatch_data.txtPCity.Trim())
                        + ", State=" + DB.GetSingleQuoteValue(dispatch_data.txtPST.Trim())
                        + ", Zip=" + DB.GetSingleQuoteValue(dispatch_data.txtPZip.Trim())
                        + ", Phone=" + DB.GetSingleQuoteValue(dispatch_data.txtPPhone.Trim())
                        + ", Fax=" + DB.GetSingleQuoteValue(dispatch_data.txtPFax.Trim())
                        + ", Email=" + DB.GetSingleQuoteValue(dispatch_data.txtPEmail.Trim())
                        + ", ContactName=" + DB.GetSingleQuoteValue(dispatch_data.txtPName.Trim()) +
                        " WHERE DispatchAddressesId = " + intPAddressID;

                        comm.CommandText = strSQL;
                        comm.ExecuteNonQuery();
                    }
                    else
                    {
                        //

                        // Insert And get Pickup Location Addres ID
                        strSQL = "insert into tbl_DispatchAddresses(CompanyLoginId,IsGCM,CompanyName,Address1,Address2,City,State,Country,Zip,Phone,Fax,Email,ContactName)"
                         + " values("
                         + companyLoginID + ",1," + DB.GetSingleQuoteValue(dispatch_data.txtPCompany) +
                         DB.GetCommaSingleQuoteValue(dispatch_data.txtPAddress1)
                         + DB.GetCommaSingleQuoteValue(dispatch_data.txtPAddress2) +
                         DB.GetCommaSingleQuoteValue(dispatch_data.txtPCity) +
                         DB.GetCommaSingleQuoteValue(dispatch_data.txtPST)
                         + DB.GetCommaSingleQuoteValue("") + DB.GetCommaSingleQuoteValue(dispatch_data.txtPZip)
                         + DB.GetCommaSingleQuoteValue(dispatch_data.txtPPhone) +
                         DB.GetCommaSingleQuoteValue(dispatch_data.txtPFax) +
                         DB.GetCommaSingleQuoteValue(dispatch_data.txtPEmail)
                         + DB.GetCommaSingleQuoteValue(dispatch_data.txtPName)
                         + ")";

                        comm.CommandText = strSQL;
                        comm.ExecuteNonQuery();

                        intPAddressID = HelperFuncs.GetLastAutogeneratedID(comm);
                    }

                    if (dispatch_data.ddlDeliveryAddress != "-1")
                    {
                        intDAddressID = Convert.ToInt32(dispatch_data.ddlDeliveryAddress);
                        //Update the table with the new info
                        strSQL = "UPDATE tbl_DispatchAddresses SET "
                        + " CompanyName=" + DB.GetSingleQuoteValue(dispatch_data.txtDCompany.Trim())
                        + ", Address1=" + DB.GetSingleQuoteValue(dispatch_data.txtDAddress1.Trim())
                        + ", Address2=" + DB.GetSingleQuoteValue(dispatch_data.txtDAddress2.Trim())
                        + ", City=" + DB.GetSingleQuoteValue(dispatch_data.txtDCity.Trim())
                        + ", State=" + DB.GetSingleQuoteValue(dispatch_data.txtDST.Trim())
                        + ", Zip=" + DB.GetSingleQuoteValue(dispatch_data.txtDZip.Trim())
                        + ", Phone=" + DB.GetSingleQuoteValue(dispatch_data.txtDPhone.Trim())
                        + ", Fax=" + DB.GetSingleQuoteValue(dispatch_data.txtDFax.Trim())
                        + ", Email=" + DB.GetSingleQuoteValue(dispatch_data.txtDEmail.Trim())
                        + ", ContactName=" + DB.GetSingleQuoteValue(dispatch_data.txtDName.Trim()) +
                        " WHERE DispatchAddressesId = " + intDAddressID;

                        comm.CommandText = strSQL;
                        comm.ExecuteNonQuery();
                    }
                    else
                    {
                        // Insert And get Delivery Location Addres ID
                        strSQL = "insert into tbl_DispatchAddresses(CompanyLoginId,IsGCM,CompanyName,Address1,Address2,City,State,Country,Zip,Phone,Fax,Email,ContactName)"
                         + " values("
                         + companyLoginID + ",1," + DB.GetSingleQuoteValue(dispatch_data.txtDCompany) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDAddress1)
                         + DB.GetCommaSingleQuoteValue(dispatch_data.txtDAddress2) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDCity) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDST)
                         + DB.GetCommaSingleQuoteValue("") + DB.GetCommaSingleQuoteValue(dispatch_data.txtDZip)
                         + DB.GetCommaSingleQuoteValue(dispatch_data.txtDPhone) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDFax) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDEmail)
                         + DB.GetCommaSingleQuoteValue(dispatch_data.txtDName)
                         + ")";

                        comm.CommandText = strSQL;
                        comm.ExecuteNonQuery();

                        comm.CommandText = "select @@Identity";
                        objID = comm.ExecuteScalar();

                        if (objID != null)
                            intDAddressID = Convert.ToInt32(objID);
                        //////////////////////////////////////////////
                    }
                    // Insert And get Third Party Location Addres ID
                    if (dispatch_data.rdblClientType == "3")
                    {

                        strSQL = "select top 1 DispatchAddressesId from tbl_DispatchAddresses where CompanyLoginId=" + companyLoginID
                           + " and IsGCM=1"
                           + " and Address1=" + DB.GetSingleQuoteValue(dispatch_data.txtTPAddress1.Trim());

                        comm.CommandText = strSQL;
                        objTempID = comm.ExecuteScalar();
                        if (objTempID != null)
                        {
                            intTPAddressID = Convert.ToInt32(objTempID);
                            // Update the table with the new info
                            strSQL = "UPDATE tbl_DispatchAddresses SET "
                            + " CompanyName=" + DB.GetSingleQuoteValue(dispatch_data.txtTPCompany.Trim())
                            + ", Address1=" + DB.GetSingleQuoteValue(dispatch_data.txtTPAddress1.Trim())
                            + ", Address2=" + DB.GetSingleQuoteValue(dispatch_data.txtTPAddress2.Trim())
                            + ", City=" + DB.GetSingleQuoteValue(dispatch_data.txtTPCity.Trim())
                            + ", State=" + DB.GetSingleQuoteValue(dispatch_data.txtTPST.Trim())
                            + ", Zip=" + DB.GetSingleQuoteValue(dispatch_data.txtTPZip.Trim())
                            + ", Phone=" + DB.GetSingleQuoteValue(dispatch_data.txtTPPhone.Trim())
                            + ", Fax=" + DB.GetSingleQuoteValue(dispatch_data.txtTPFax.Trim())
                            + ", Email=" + DB.GetSingleQuoteValue(dispatch_data.txtTPEmail.Trim())
                            + ", ContactName=" + DB.GetSingleQuoteValue(dispatch_data.txtTPName.Trim()) +
                            " WHERE DispatchAddressesId = " + intTPAddressID;

                            comm.CommandText = strSQL;
                            comm.ExecuteNonQuery();
                        }
                        else
                        {

                            strSQL = "insert into tbl_DispatchAddresses(CompanyLoginId,IsGCM,CompanyName,Address1,Address2,City,State,Country,Zip,Phone,Fax,Email,ContactName)"
                             + " values("
                             + companyLoginID + ",1," + DB.GetSingleQuoteValue(dispatch_data.txtTPCompany) +
                             DB.GetCommaSingleQuoteValue(dispatch_data.txtTPAddress1)
                             + DB.GetCommaSingleQuoteValue(dispatch_data.txtTPAddress2) +
                             DB.GetCommaSingleQuoteValue(dispatch_data.txtTPCity) +
                             DB.GetCommaSingleQuoteValue(dispatch_data.txtTPST)
                             + DB.GetCommaSingleQuoteValue("") +
                             DB.GetCommaSingleQuoteValue(dispatch_data.txtTPZip)
                             + DB.GetCommaSingleQuoteValue(dispatch_data.txtTPPhone) +
                             DB.GetCommaSingleQuoteValue(dispatch_data.txtTPFax) +
                             DB.GetCommaSingleQuoteValue(dispatch_data.txtTPEmail)
                             + DB.GetCommaSingleQuoteValue(dispatch_data.txtTPName)
                            + ")";

                            comm.CommandText = strSQL;
                            comm.ExecuteNonQuery();

                            intTPAddressID = HelperFuncs.GetLastAutogeneratedID(comm);
                        }
                    }

                    //Insert Basic Dispatch Information

                    //int dispatchAddressID;
                    int useFax;


                    if (dispatch_data.rdblBill == "FAX")
                        useFax = 1;
                    else
                        useFax = 0;

                    string contactEmail = "";
                    if (dispatch_data.rdblClientType == "1")
                        contactEmail = dispatch_data.txtPEmail;
                    else if (dispatch_data.rdblClientType == "2")
                        contactEmail = dispatch_data.txtDEmail;
                    else if (dispatch_data.rdblClientType == "3")
                        contactEmail = dispatch_data.txtTPEmail;

                    int rateQuoteAmt = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(dispatch_data.rate)));

                    strSQL = "insert into tbl_DispatchInfo(PickupDispatchAddressesId,DeliveryDispatchAddressesId,ThirdDispatchAddressesId,UseFax,PoNo,Readytime,Readydate,CarrierSelected,RateQuoteAmt,DispatchDate,DispatchTime,ContactEmailAddress)"
                     + " values("
                     + intPAddressID + "," + intDAddressID + "," + intTPAddressID + "," + useFax +
                     DB.GetCommaSingleQuoteValue(dispatch_data.txtPONumber) +
                     DB.GetCommaSingleQuoteValue(dispatch_data.ddlRHour + ":" +
                     dispatch_data.ddlRMinute + " " + dispatch_data.ddlRAMPM)
                     + DB.GetCommaSingleQuoteValue(dispatch_data.txtShipmentDate) +
                     DB.GetCommaSingleQuoteValue(dispatch_data.carrier) +
                     DB.GetCommaSingleQuoteValue(rateQuoteAmt.ToString())
                     + DB.GetCommaSingleQuoteValue(dispatch_data.txtShipmentDate) +
                     DB.GetCommaSingleQuoteValue(dispatch_data.ddlCHour + ":" +
                     dispatch_data.ddlCMinute + " " +
                     dispatch_data.ddlCAMPM)
                     + DB.GetCommaSingleQuoteValue(contactEmail)
                     + ")";

                    comm.CommandText = strSQL;
                    comm.ExecuteNonQuery();

                    comm.CommandText = "select @@Identity";
                    objID = comm.ExecuteScalar();

                    if (objID != null)
                        intDispatchID = Convert.ToInt32(objID);

                    InsertAllFreightInformation(ref strSQL, ref intDispatchID);

                }
            }
        }

        #endregion

        #region SaveDispatchInformationForBOL

        private void SaveDispatchInformationForBOL()
        {
            string strSQL;
            int intDelCompID = -1;
            int intPUCompID = -1;

            string strPUDate = "";
            int intDeliveryDays = 0;
            int intDays;
            if (dispatch_data.deliveryDay != null && dispatch_data.deliveryDay.Length > 0 && 
                int.TryParse(dispatch_data.deliveryDay, out intDays))
            {
                intDeliveryDays = intDays;
            }

            DateTime dtShipment;
            DateTime myTestDate;
            //string strPUDate = "";
            if (dispatch_data.isAAFES_Shipment.Equals(false))
            {
                if (dispatch_data.txtShipmentDate != null && dispatch_data.txtShipmentDate.Trim().Length > 0 && 
                    DateTime.TryParse(dispatch_data.txtShipmentDate.Trim(), out dtShipment))
                {
                    strPUDate = dtShipment.ToString("MM/dd/yyyy");
                }
                else
                {
                    strPUDate = DateTime.Now.ToString("MM/dd/yyyy");
                }
            }
            else if (dispatch_data.q_ShipmentReadyDate != null && 
                DateTime.TryParse(dispatch_data.q_ShipmentReadyDate, out myTestDate))
            {
                strPUDate = dispatch_data.q_ShipmentReadyDate;
            }
            else
            {
                strPUDate = DateTime.Now.ToString("MM/dd/yyyy");
            }


            DateTime dtDeliveryDate = Helper.GetDeliveryDate(Convert.ToDateTime(strPUDate), intDeliveryDays);
            dispatch_data.glbDeliveryDate = dtDeliveryDate;

            #region Get the logged in Customer Company ID
            // Here I have to pick AES Company ID Later.

            strSQL = "select AESCompID from tbl_LOGIN where UserName='" + dispatch_data.username + "'";

            comm.CommandText = strSQL;
            SqlDataReader dr = comm.ExecuteReader();
            bool hasRows = dr.HasRows;
            dr.Read();
            if (hasRows)
            {
                if (!DBNull.Value.Equals(dr["AESCompID"]))
                {
                    dispatch_data.intCustCompanyID = Convert.ToInt32(dr["AESCompID"]);
                }
            }
            dr.Close();
            dr.Dispose();
            #endregion

            SaveCompanyTableDispatchInfo(ref intDelCompID, ref intPUCompID, ref strSQL);

            #region Insert into Shipment table

            // Insert into Shipment table

            #region For RRD, Guaranteed
            //dispatch_data.carrier.Contains(" RRD") && 
            if (dispatch_data.rateType.Equals("GUARANTEEDPM"))
            {
                HelperFuncs.writeToSiteErrors("dispatch", "GUARANTEEDPM");
                dispatch_data.txtComment += string.Concat(" Guaranteed by 5PM RRD for delivery by 5 PM on ",
                    dispatch_data.glbDeliveryDate.ToShortDateString());

            }
            else
            {
                HelperFuncs.writeToSiteErrors("dispatch", "not GUARANTEEDPM");
            }

            #endregion

            if (dispatch_data.repName != null)
            {
                // AES person booking on GCM customer account
                dispatch_data.Initials = dispatch_data.repName.ToString();
                dispatch_data.Status = "TODISP";

            }
            else if (dispatch_data.username != null && dispatch_data.username.Equals("AESW140S"))
            {
                dispatch_data.Initials = "GCM DISPATCHER";
                dispatch_data.Status = "Worldwide";
            }
            else
            {
                dispatch_data.Initials = "GCM DISPATCHER";
                dispatch_data.Status = "TODISP";
            }

            strSQL = string.Concat("insert into tbl_Shipments", demo_table_name_addition, "(CompID_CUST,Initials,ItemNotes,ShipStatus)"
                              , " values("
                              , dispatch_data.intCustCompanyID, DB.GetCommaSingleQuoteValue(dispatch_data.Initials), 
                              DB.GetCommaSingleQuoteValue(dispatch_data.txtComment.Trim()), DB.GetCommaSingleQuoteValue(dispatch_data.Status)
                              , ")");

            comm.CommandText = strSQL;
            comm.ExecuteNonQuery();

            dispatch_data.intShipmentID = HelperFuncs.GetLastAutogeneratedID(comm);
            //HelperFuncs.writeToSiteErrors("SaveTheReportAsPDF", "Created shipID: " + intShipmentID.ToString());

            #endregion

            #region If UPS Package Save Labels

            // If UPS Package Save Labels
            /*
            if (dispatch_data.carrier.ToUpper().Contains("UPS") &&
                        (dispatch_data.carrier.ToUpper().Contains("GROUND") || dispatch_data.carrier.ToUpper().Contains("DAY")))
            {
                SaveLabels_UPS_Package(); // ref labelImgStrings
            }
            */

            #endregion

            #region Insert into rate comparison table

            // Insert into rate comparison table
            try
            {
                strSQL = "INSERT INTO tbl_RATE_DIFFERENCE(ShipmentID, SelectedTransitTime, SelectedOnTimePercent, SelectedCarrier, SelectedRate, " +
                            "TopTransitTime, TopOnTimePercent, TopCarrier, TopRate)" +
                            "VALUES(" + dispatch_data.intShipmentID + ", " + dispatch_data.selectedTransit.ToString() + ", " + dispatch_data.selectedOnTime.ToString() + ", '" +
                            dispatch_data.selectedCarrier + "', " + dispatch_data.selectedRate.ToString() + ", " + dispatch_data.topTransit.ToString() +
                            ", " + dispatch_data.topOnTime.ToString() + ", '" + dispatch_data.topCarrier.ToString() + "', " + dispatch_data.topRate.ToString() + ")";

                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                HelperFuncs.writeToSiteErrors("Rate difference insertion", dispatch_data.intShipmentID + " - " + e.ToString());
            }

            #endregion

            #region Get intCarrierCompID
            dispatch_data.intCarrierCompID = -1;
            if (dispatch_data.carrier != null)
            {
                dispatch_data.intCarrierCompID = GetCarrierCompanyID(dispatch_data.carrier.Trim());
            }
            #endregion

            #region Insert into Segment table
            // Insert into Segment table

            double dblCarrierQuoteAmount = 0;
            double dblRate = 0;
            if (dispatch_data.ourRate != null && dispatch_data.ourRate.Length > 0 && 
                double.TryParse(dispatch_data.ourRate, out dblRate))
            {
                dblCarrierQuoteAmount = dblRate;
            }

            string strInsurance = "";
            double dblShipmentValue = 0;
            if (dispatch_data.hasInsurance != null && dispatch_data.hasInsurance.Equals("yes") && 
                double.TryParse(dispatch_data.shipmentValue, out dblShipmentValue))
            {
                strInsurance = "Sold Insurance of $" + string.Format("{0:#,###.00}", dblShipmentValue) + 
                    " sell rate of $" + 
                    System.Web.Configuration.WebConfigurationManager.AppSettings["InsuranceSellRate"];
            }

            string CarrierQuoteNum = "";

            if (dispatch_data.carrier.Equals("Central Transport A/N"))
            {
                CarrierQuoteNum = "Central Transport A/N";
            }

            if (dispatch_data.carrier.EndsWith(" RRD"))
            {

                if (!dispatch_data.DLS_PrimaryReferencePNW.Equals(string.Empty))
                {
                    if (dispatch_data.isDUR == true)
                    {
                        CarrierQuoteNum = string.Concat("CC", dispatch_data.DLS_PrimaryReferencePNW);
                    }
                    else
                    {
                        CarrierQuoteNum = dispatch_data.DLS_PrimaryReferencePNW;
                    }
                }
                else
                {
                    CarrierQuoteNum = "RRD";
                }

                HelperFuncs.writeToSiteErrors("CarrierQuoteNum RRD shipID, carrierQuoteNum", 
                    dispatch_data.intShipmentID.ToString() + ", " + dispatch_data.DLS_PrimaryReferencePNW);
                HelperFuncs.writeToSiteErrors("CarrierQuoteNum RRD", CarrierQuoteNum);
            }
            //if (Session["showDLSRates"] != null && ((bool)Session["showDLSRates"]).Equals(true) &&
            //    !dispatch_data.carrierKey.Equals("Central Transport"))
            //{
            //    CarrierQuoteNum = "RRD";
            //    HelperFuncs.writeToSiteErrors("CarrierQuoteNum RRD shipID", intShipmentID.ToString());
            //}
            else
            {
                HelperFuncs.writeToSiteErrors("CarrierQuoteNum RRD shipID, carrierKey", dispatch_data.intShipmentID.ToString() + ", " + dispatch_data.carrierKey);
            }

            string mySegStatus = "TODISP"; // Default
            if (dispatch_data.username != null && dispatch_data.username.ToString().Equals("AESW140S"))
            {
                mySegStatus = "Worldwide";
            }

            strSQL = string.Concat("insert into tbl_Segments", demo_table_name_addition, "(ShipmentID,CompID_CAR,CompID_DELIV,CompID_PU,CarrierQuoteAmt,CarrierQuoteNum,PUDate,RPUDate,DelivDate,DispatchDate,TransMode,DI,SegRmk1,SegmentNbr,ATA, SegStatus)"
                         , " values("
                         , dispatch_data.intShipmentID, ",", dispatch_data.intCarrierCompID, ",", intDelCompID, ",", intPUCompID, ",", dblCarrierQuoteAmount, ",'", CarrierQuoteNum, "'"
                         , DB.GetCommaSingleQuoteValue(strPUDate.Trim()), DB.GetCommaSingleQuoteValue(strPUDate.Trim()), DB.GetCommaSingleQuoteValue(dtDeliveryDate.ToString("MM/dd/yyyy"))
                         , DB.GetCommaSingleQuoteValue(DateTime.Now.ToString("MM/dd/yyyy")), DB.GetCommaSingleQuoteValue("G"), ",1", DB.GetCommaSingleQuoteValue(strInsurance)
                         , ",1", DB.GetCommaSingleQuoteValue(dtDeliveryDate.ToString("MM/dd/yyyy"))
                         , ",'", mySegStatus, "')");

            HelperFuncs.writeToSiteErrors("Dispatch into tbl_Segments sql", strSQL);

            comm.CommandText = strSQL;
            comm.ExecuteNonQuery();

            dispatch_data.intSegmentID = HelperFuncs.GetLastAutogeneratedID(comm);

            #endregion

            #region Set Session Report paths

            //strReportFileName = dispatch_data.username.ToString() + "_" + intShipmentID.ToString() + "_" + 
            //    dispatch_data.intSegmentID.ToString() + ".pdf";
            //Session["BOLReportName"] = strReportFileName;

            //strReportFileName2 = dispatch_data.username.ToString() + "_seg2_" + intShipmentID.ToString() + "_" +
            //    dispatch_data.intSegmentID.ToString() + ".pdf";
            //Session["BOLReportName2"] = strReportFileName2;

            //exportPath = Server.MapPath("BOLReports\\" + Session["BOLReportName"].ToString());
            //Session["BOLReportPath"] = exportPath;

            // Here generate the new report path for the fedEx with new prefix fedEx
            //strFedexFileName = dispatch_data.username.ToString() + "_FedEx_" + intShipmentID.ToString() + "_" +
            //    dispatch_data.intSegmentID.ToString() + ".pdf";
            //Session["FedExReportName"] = strFedexFileName;

            //exportPathFedex = Server.MapPath("BOLReports\\" + Session["FedExReportName"].ToString());
            //Session["FedExReportPath"] = exportPathFedex;

            //exportPath2 = Server.MapPath("BOLReports\\" + Session["BOLReportName2"].ToString());
            //Session["BOLReportPath2"] = exportPath2;

            #endregion

            InsertIntoRateQuotesTable(ref dblRate, ref dblShipmentValue, ref strSQL);

            #region Insert Insurance cost

            if (dispatch_data.hasInsurance != null && dispatch_data.hasInsurance.Equals("yes"))
            {
                Repository.InsertInsuranceCost(ref dispatch_data, dblShipmentValue, dispatch_data.intRQID);
            }

            #endregion

            ArrayList services = new ArrayList();
            // Insert all the services into tbl_AccDetails
            Repository.InsertAccessorialsIntoAccDetailsTable(ref dispatch_data, ref services, ref strSQL);


            string Desc1 = dispatch_data.txtDesc1, Desc2 = dispatch_data.txtDesc2, Desc3 = dispatch_data.txtDesc3, Desc4 = dispatch_data.txtDesc4;

            Helper.SetItemDescriptions(ref dispatch_data,ref Desc1, ref Desc2, ref Desc3, ref Desc4);

            bool isHazMatItem = false;
            int itemCount = 0;

            // Insert into Items table
            InsertIntoItemsTable(ref isHazMatItem, ref itemCount, ref Desc1, ref Desc2, ref Desc3, ref Desc4, ref strSQL);

            #region Insert PONumber into PO table
            // Insert PONumber into PO table
            foreach (string strPONo in dispatch_data.txtPONumber.Trim().Split(','))
            {
                strSQL = "insert into tbl_PO(ShipmentID,PONumber)"
                     + " values("
                     + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(strPONo.Trim())
                     + ")";
                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();
            }
            #endregion

        }

        #endregion

        #region SaveDispatchInformationForSpecialBOL

        private void SaveDispatchInformationForSpecialBOL()
        {
            string strSQL;
            ////////////////////////////////////////////////Get the logged in Customer Company ID///////////////
            //Here I have to pick AES Company ID Later.
            strSQL = "select AESCompID from tbl_LOGIN where UserName='" + dispatch_data.username + "'";
            comm.CommandText = strSQL;
            SqlDataReader dr = comm.ExecuteReader();
            bool hasRows = dr.HasRows;
            dr.Read();
            if (hasRows)
            {
                if (!DBNull.Value.Equals(dr["AESCompID"]))
                    dispatch_data.intCustCompanyID = Convert.ToInt32(dr["AESCompID"]);
            }
            dr.Close();
            dr.Dispose();

            //////////////////////////////////////////////////////////////////////////////////
            object objTempID;
            //Insert Pickup Company Adress in Company table
            strSQL = "select top 1 CompID from tbl_Company where CompName=" + DB.GetSingleQuoteValue(dispatch_data.txtPCompany.Trim())
                    + "and PUContact=" + DB.GetSingleQuoteValue(dispatch_data.txtPName.Trim())
                    + "and Addr1=" + DB.GetSingleQuoteValue(dispatch_data.txtPAddress1.Trim())
                    + "and Addr2=" + DB.GetSingleQuoteValue(dispatch_data.txtPAddress2.Trim())
                    + "and City=" + DB.GetSingleQuoteValue(dispatch_data.txtPCity.Trim())
                    + "and State=" + DB.GetSingleQuoteValue(dispatch_data.txtPST.Trim())
                    + "and Zip=" + DB.GetSingleQuoteValue(dispatch_data.txtPZip.Trim())
                    + "and PUEmail=" + DB.GetSingleQuoteValue(dispatch_data.txtPEmail.Trim())
                    + "and Phone=" + DB.GetSingleQuoteValue(dispatch_data.txtPPhone.Trim())
                    + "and Fax=" + DB.GetSingleQuoteValue(dispatch_data.txtPFax.Trim());

            comm.CommandText = strSQL;
            objTempID = comm.ExecuteScalar();
            //
            int intPUCompID;
            //
            if (objTempID != null)
                intPUCompID = Convert.ToInt32(objTempID);
            else
            {

                strSQL = "insert into tbl_Company(CompName,PUContact,Addr1,Addr2,City,State,Zip,PUEmail,Phone,Fax)"
                             + " values("
                             + DB.GetSingleQuoteValue(dispatch_data.txtPCompany.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtPName.Trim())
                             + DB.GetCommaSingleQuoteValue(dispatch_data.txtPAddress1.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtPAddress2.Trim())
                             + DB.GetCommaSingleQuoteValue(dispatch_data.txtPCity.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtPST.Trim())
                             + DB.GetCommaSingleQuoteValue(dispatch_data.txtPZip.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtPEmail.Trim())
                             + DB.GetCommaSingleQuoteValue(dispatch_data.txtPPhone.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtPFax.Trim())
                             + ")";
                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();

                dispatch_data.intPUCompID = HelperFuncs.GetLastAutogeneratedID(comm);
            }
            //////////////////////////////////////////////////////////////////////////////////


            //////////////////////////////////////////////////////////////////////////////////

            //Get Interim Shipper ID
            //string interimShipperCompName = GetSpecialCarrierName();
            //string interimCarrierCompName = GetSpecialCarrierName();
            //int intInterimShipperCompID=-1;
            int intInterimCarrierCompID = -1;
            string specialState = Helper.GetSpecialState(ref dispatch_data);

            intInterimCarrierCompID = Repository.GetInterimCarrierCompanyID(ref dispatch_data);

            Company interimCompany = Helper.GetInterimCompanyDetails(ref dispatch_data);

            //Insert Interim Consignee
            strSQL = "select top 1 CompID from tbl_Company where CompName=" + DB.GetSingleQuoteValue(interimCompany.CompName)
                    + "and DeliveryContact=" + DB.GetSingleQuoteValue(dispatch_data.txtDCompany.Trim())
                    + "and Addr1=" + DB.GetSingleQuoteValue(interimCompany.Addr1)
                    + "and Addr2=" + DB.GetSingleQuoteValue(interimCompany.Addr2)
                    + "and City=" + DB.GetSingleQuoteValue(interimCompany.City)
                    + "and State=" + DB.GetSingleQuoteValue(interimCompany.State)
                    + "and Zip=" + DB.GetSingleQuoteValue(interimCompany.Zip)
                    + "and DeliveryEMail=" + DB.GetSingleQuoteValue(dispatch_data.txtDEmail.Trim())
                    + "and Phone=" + DB.GetSingleQuoteValue(interimCompany.Phone)
                    + "and Fax=" + DB.GetSingleQuoteValue("");

            comm.CommandText = strSQL;
            objTempID = comm.ExecuteScalar();
            //
            int intInterimConsigneeCompID;
            //
            if (objTempID != null)
                intInterimConsigneeCompID = Convert.ToInt32(objTempID);
            else
            {
                strSQL = "insert into tbl_Company(CompName,DeliveryContact,Addr1,Addr2,City,State,Zip,DeliveryEMail,Phone,Fax)"
                            + " values("
                            + DB.GetSingleQuoteValue(interimCompany.CompName) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDCompany.Trim())
                            + DB.GetCommaSingleQuoteValue(interimCompany.Addr1) + DB.GetCommaSingleQuoteValue(interimCompany.Addr2)
                            + DB.GetCommaSingleQuoteValue(interimCompany.City) + DB.GetCommaSingleQuoteValue(interimCompany.State)
                            + DB.GetCommaSingleQuoteValue(interimCompany.Zip) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDEmail.Trim())
                            + DB.GetCommaSingleQuoteValue(interimCompany.Phone) + DB.GetCommaSingleQuoteValue("")
                            + ")";
                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();

                intInterimConsigneeCompID = HelperFuncs.GetLastAutogeneratedID(comm);
            }
            ////////////////////

            //Insert Interim Shipper
            strSQL = "select top 1 CompID from tbl_Company where CompName=" + DB.GetSingleQuoteValue(interimCompany.CompName)
                    + "and PUContact=" + DB.GetSingleQuoteValue(dispatch_data.txtDCompany.Trim())
                    + "and Addr1=" + DB.GetSingleQuoteValue(interimCompany.Addr1)
                    + "and Addr2=" + DB.GetSingleQuoteValue(interimCompany.Addr2)
                    + "and City=" + DB.GetSingleQuoteValue(interimCompany.City)
                    + "and State=" + DB.GetSingleQuoteValue(interimCompany.State)
                    + "and Zip=" + DB.GetSingleQuoteValue(interimCompany.Zip)
                    + "and PUEmail=" + DB.GetSingleQuoteValue(dispatch_data.txtDEmail.Trim())
                    + "and Phone=" + DB.GetSingleQuoteValue(interimCompany.Phone)
                    + "and Fax=" + DB.GetSingleQuoteValue("");

            comm.CommandText = strSQL;
            objTempID = comm.ExecuteScalar();
            //
            int intInterimShipperCompID;
            //
            if (objTempID != null)
                intInterimShipperCompID = Convert.ToInt32(objTempID);
            else
            {
                strSQL = "insert into tbl_Company(CompName,PUContact,Addr1,Addr2,City,State,Zip,PUEmail,Phone,Fax)"
                            + " values("
                            + DB.GetSingleQuoteValue(interimCompany.CompName) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDCompany.Trim())
                            + DB.GetCommaSingleQuoteValue(interimCompany.Addr1) + DB.GetCommaSingleQuoteValue(interimCompany.Addr2)
                            + DB.GetCommaSingleQuoteValue(interimCompany.City) + DB.GetCommaSingleQuoteValue(interimCompany.State)
                            + DB.GetCommaSingleQuoteValue(interimCompany.Zip) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDEmail.Trim())
                            + DB.GetCommaSingleQuoteValue(interimCompany.Phone) + DB.GetCommaSingleQuoteValue("")
                            + ")";
                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();

                intInterimShipperCompID = HelperFuncs.GetLastAutogeneratedID(comm);
            }

            // Insert Delivery Company Adress in Company table
            strSQL = "select top 1 CompID from tbl_Company where CompName=" + DB.GetSingleQuoteValue(dispatch_data.txtDCompany.Trim())
                    + "and DeliveryContact=" + DB.GetSingleQuoteValue(dispatch_data.txtDName.Trim())
                    + "and Addr1=" + DB.GetSingleQuoteValue(dispatch_data.txtDAddress1.Trim())
                    + "and Addr2=" + DB.GetSingleQuoteValue(dispatch_data.txtDAddress2.Trim())
                    + "and City=" + DB.GetSingleQuoteValue(dispatch_data.txtDCity.Trim())
                    + "and State=" + DB.GetSingleQuoteValue(dispatch_data.txtDST.Trim())
                    + "and Zip=" + DB.GetSingleQuoteValue(dispatch_data.txtDZip.Trim())
                    + "and DeliveryEMail=" + DB.GetSingleQuoteValue(dispatch_data.txtDEmail.Trim())
                    + "and Phone=" + DB.GetSingleQuoteValue(dispatch_data.txtDPhone.Trim())
                    + "and Fax=" + DB.GetSingleQuoteValue(dispatch_data.txtDFax.Trim());

            comm.CommandText = strSQL;
            objTempID = comm.ExecuteScalar();
            //
            int intDelCompID;
            //
            if (objTempID != null)
                intDelCompID = Convert.ToInt32(objTempID);
            else
            {
                strSQL = "insert into tbl_Company(CompName,DeliveryContact,Addr1,Addr2,City,State,Zip,DeliveryEMail,Phone,Fax)"
                            + " values("
                            + DB.GetSingleQuoteValue(dispatch_data.txtDCompany.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDName.Trim())
                            + DB.GetCommaSingleQuoteValue(dispatch_data.txtDAddress1.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDAddress2.Trim())
                            + DB.GetCommaSingleQuoteValue(dispatch_data.txtDCity.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDST.Trim())
                            + DB.GetCommaSingleQuoteValue(dispatch_data.txtDZip.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDEmail.Trim())
                            + DB.GetCommaSingleQuoteValue(dispatch_data.txtDPhone.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDFax.Trim())
                            + ")";
                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();

                intDelCompID = HelperFuncs.GetLastAutogeneratedID(comm);
            }

            //Insert into Shipment table
            //SC 
            string rateTypeG1 = dispatch_data.rateType.ToString(), AMPMG1 = "", MsgG1 = "";
            if (rateTypeG1 == "GUARANTEEDAM")
            {
                AMPMG1 = "Noon";
            }
            else if (rateTypeG1 == "GUARANTEEDPM")
            {
                AMPMG1 = "5 PM";
            }
            DateTime deliveryDate = Helper.GetDeliveryDate(Convert.ToDateTime(dispatch_data.txtShipmentDate),
                Convert.ToInt32(dispatch_data.deliveryDay));
            string career = dispatch_data.carrier.ToString();
            if (rateTypeG1 == "GUARANTEEDAM" || rateTypeG1 == "GUARANTEEDPM")
            {
                //MsgG1 = "This shipment is guaranteed with " + career + " for delivery by  " + AMPMG1 + " on " + deliveryDate.Date.DayOfWeek.ToString() + "";
                //MsgG1 = string.Concat("<span style='color:Red;'>This shipment is guaranteed with ", career, " for delivery by  ", AMPMG1, " on ",
                //    deliveryDate.Date.ToShortDateString(), "</span>");
                MsgG1 = string.Concat("This shipment is guaranteed with ", career, " for delivery by  ", AMPMG1, " on ",
                    deliveryDate.Date.ToShortDateString());
            }
            //
            if (rateTypeG1 == "GUARANTEEDAM" || rateTypeG1 == "GUARANTEEDPM")
            {
                if (dispatch_data.repName != null)
                {
                    // AES person booking on GCM customer account  
                    dispatch_data.Initials = dispatch_data.repName.ToString();
                    //strSQL = "insert into tbl_Shipments(CompID_CUST,Initials,ItemNotes,ShipStatus)"
                    //            + " values("
                    //            + intCustCompanyID + DB.GetCommaSingleQuoteValue(dispatch_data.repName.ToString()) + DB.GetCommaSingleQuoteValue(MsgG1.Trim()) + DB.GetCommaSingleQuoteValue("TODISP")
                    //            + ")";
                }
                else
                {
                    dispatch_data.Initials = "GCM DISPATCHER";

                }
                strSQL = string.Concat("insert into tbl_Shipments", demo_table_name_addition, 
                    "(CompID_CUST,Initials,ItemNotes,ShipStatus)",
                                 " values(",
                                 dispatch_data.intCustCompanyID,
                                 DB.GetCommaSingleQuoteValue(dispatch_data.Initials),
                                 DB.GetCommaSingleQuoteValue(MsgG1.Trim()),
                                 DB.GetCommaSingleQuoteValue("TODISP"),
                                 ")");
            }
            else
            {
                if (dispatch_data.rateType.Equals("GUARANTEEDPM"))
                {
                    if (!dispatch_data.txtComment.Contains("Guaranteed by 5PM") && !dispatch_data.glbDeliveryDate.Equals(DateTime.MinValue))
                    {
                        dispatch_data.txtComment += string.Concat(" Guaranteed by 5PM RRD for delivery by 5 PM on ", dispatch_data.glbDeliveryDate.ToShortDateString());
                    }
                    else
                    {
                        HelperFuncs.writeToSiteErrors("Segment2 comment by 5pm",
                            string.Concat(dispatch_data.txtComment, " ", dispatch_data.glbDeliveryDate.ToShortDateString()));
                    }
                }

                if (dispatch_data.repName != null)
                {
                    // AES person booking on GCM customer account   
                    dispatch_data.Initials = dispatch_data.repName.ToString();
                    dispatch_data.Status = "TODISP";

                }
                else if (dispatch_data.username != null && dispatch_data.username.ToString().Equals("AESW140S"))
                {
                    dispatch_data.Initials = "GCM DISPATCHER";
                    dispatch_data.Status = "Worldwide";

                }
                else
                {
                    dispatch_data.Initials = "GCM DISPATCHER";
                    dispatch_data.Status = "TODISP";

                }

                strSQL = string.Concat("insert into tbl_Shipments", demo_table_name_addition, "(CompID_CUST,Initials,ItemNotes,ShipStatus)"
                                 , " values("
                                 , dispatch_data.intCustCompanyID,
                                 DB.GetCommaSingleQuoteValue(dispatch_data.Initials),
                                 DB.GetCommaSingleQuoteValue(dispatch_data.txtComment.Trim()),
                                 DB.GetCommaSingleQuoteValue(dispatch_data.Status)
                                 , ")");


            }
            comm.CommandText = strSQL;
            comm.ExecuteNonQuery();

            dispatch_data.intShipmentID = HelperFuncs.GetLastAutogeneratedID(comm);
            //HelperFuncs.writeToSiteErrors("SaveTheReportAsPDF", "Created shipID: " + intShipmentID.ToString());

            //if (dispatch_data.isAAFES_Quote != null && dispatch_data.isAAFES_Quote == true)
            //{
            //    HelperFuncs.assignSFproToShipID(ref dispatch_data.intShipmentID);
            //}

            //HelperFuncs.writeToSiteErrors("Labels ship id seg 2", "test for seg 2");
            //////////////////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////////////////////
            //Insert into rate comparison table
            try
            {
                strSQL = "INSERT INTO tbl_RATE_DIFFERENCE(ShipmentID, SelectedTransitTime, SelectedOnTimePercent, SelectedCarrier, SelectedRate, " +
                            "TopTransitTime, TopOnTimePercent, TopCarrier, TopRate)" +
                            "VALUES(" + dispatch_data.intShipmentID + ", " + dispatch_data.selectedTransit.ToString() + ", " + dispatch_data.selectedOnTime.ToString() + ", '" +
                            dispatch_data.selectedCarrier.ToString() + "', " + dispatch_data.selectedRate.ToString() + ", " + dispatch_data.topTransit.ToString() +
                            ", " + dispatch_data.topOnTime.ToString() + ", '" + dispatch_data.topCarrier.ToString() + "', " + dispatch_data.topRate.ToString() + ")";

                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                strSQL = "INSERT INTO [CarrierAuditInfo].[dbo].ERRORS (Description, Date) " +
                    "VALUES ('rate difference insertion - " + e.ToString().Replace("'", "") + "', '" + DateTime.Now.ToShortDateString() + "');";

                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();
            }
            // Getting Carrier Company ID
            dispatch_data.intCarrierCompID = -1;
            if (dispatch_data.carrier != null)
            {
                dispatch_data.intCarrierCompID = GetCarrierCompanyID(dispatch_data.carrier.ToString().Trim());
            }
            ////////////////////////////////////////////////////////////////////////////////////


            ////////////////////////////////////////////////////////////////////////////////////
            // Insert into Segment table
            DateTime dtShipment;
            string strPUDate = "";
            //if (requestedValues["q_pickupDate"] != null && requestedValues["q_pickupDate"].Length > 0 && DateTime.TryParse(requestedValues["q_pickupDate"], out dtShipment))
            if (dispatch_data.txtShipmentDate != null && dispatch_data.txtShipmentDate.Trim().Length > 0 && DateTime.TryParse(dispatch_data.txtShipmentDate.Trim(), out dtShipment))
                strPUDate = dtShipment.ToString("MM/dd/yyyy");
            else
                strPUDate = DateTime.Now.ToString("MM/dd/yyyy");

            double dblCarrierQuoteAmount = 0;
            double dblRate;
            if (dispatch_data.ourRate != null && dispatch_data.ourRate.Length > 0 && double.TryParse(dispatch_data.ourRate, out dblRate))
                dblCarrierQuoteAmount = dblRate;

            int intDeliveryDays = 0;
            int intDays;
            if (dispatch_data.deliveryDay != null && dispatch_data.deliveryDay.Length > 0 && int.TryParse(dispatch_data.deliveryDay, out intDays))
                intDeliveryDays = intDays;

            DateTime dtDeliveryDate = Helper.GetDeliveryDate(Convert.ToDateTime(strPUDate), intDeliveryDays);


            int intDeliveryDays2 = 7;
            DateTime dtDeliveryDate2 = Helper.GetDeliveryDate(dtDeliveryDate, intDeliveryDays2);


            string strInsurance = "";
            double dblShipmentValue = 0;
            if (dispatch_data.hasInsurance.Equals("yes") && double.TryParse(dispatch_data.shipmentValue, out dblShipmentValue))
            {
                strInsurance = "Sold Insurance of $" + string.Format("{0:#,###.00}", dblShipmentValue) + " sell rate of $" + System.Web.Configuration.WebConfigurationManager.AppSettings["InsuranceSellRate"]; ;

            }

            // If it is outbound from alaska or hawaii to the US, switch the carriers
            string[] arrPCityState = dispatch_data.q_OCity.Split(',');
            if (arrPCityState[1].Trim() == "HI" || arrPCityState[1].Trim() == "AK")
            {
                int tempCarIDHolder = dispatch_data.intCarrierCompID;
                dispatch_data.intCarrierCompID = intInterimCarrierCompID;
                intInterimCarrierCompID = tempCarIDHolder;
            }

            //Insert Segment1
            //--------------------------------------------------------------------------------------------------------------------------------------------
            //NITF
            string CarrierQuoteNum = "";

            if (dispatch_data.carrier.Equals("Central Transport A/N"))
            {
                CarrierQuoteNum = "Central Transport A/N";
            }

            if (dispatch_data.carrier.EndsWith(" RRD"))
            {
                //CarrierQuoteNum = "RRD";

                if (!dispatch_data.DLS_PrimaryReferencePNW.Equals(string.Empty))
                {
                    if (dispatch_data.isDUR == true)
                    {
                        CarrierQuoteNum = string.Concat("CC", dispatch_data.DLS_PrimaryReferencePNW);
                    }
                    else
                    {
                        CarrierQuoteNum = dispatch_data.DLS_PrimaryReferencePNW;
                    }
                }
                else
                {
                    CarrierQuoteNum = "RRD";
                }


                //CarrierQuoteNum = dispatch_data.DLS_PrimaryReferencePNW;
                HelperFuncs.writeToSiteErrors("CarrierQuoteNum RRD shipID, carrierQuoteNum", dispatch_data.intShipmentID.ToString() + ", " + dispatch_data.DLS_PrimaryReferencePNW);
                HelperFuncs.writeToSiteErrors("CarrierQuoteNum RRD", CarrierQuoteNum);
            }
            //if (Session["showDLSRates"] != null && ((bool)Session["showDLSRates"]).Equals(true) &&
            //           !dispatch_data.carrierKey.Equals("Central Transport"))
            //{
            //    CarrierQuoteNum = "RRD";
            //    HelperFuncs.writeToSiteErrors("CarrierQuoteNum RRD shipID", intShipmentID.ToString());
            //}
            else
            {
                HelperFuncs.writeToSiteErrors("CarrierQuoteNum RRD shipID, carrierKey", dispatch_data.intShipmentID.ToString() + ", " + dispatch_data.carrierKey);
            }

            string mySegStatus = "TODISP"; // Default
            if (dispatch_data.username != null && dispatch_data.username.ToString().Equals("AESW140S"))
            {
                mySegStatus = "Worldwide";
            }

            strSQL = string.Concat("insert into tbl_Segments", demo_table_name_addition, 
                "(ShipmentID,CompID_CAR,CompID_DELIV,CompID_PU,CarrierQuoteAmt,CarrierQuoteNum,PUDate,RPUDate,DelivDate,DispatchDate,TransMode,DI,SegRmk1,SegmentNbr,ATA, SegStatus)",
                         " values(",
                         dispatch_data.intShipmentID, ",", dispatch_data.intCarrierCompID, ",", intInterimConsigneeCompID, ",", dispatch_data.intPUCompID, ",", dblCarrierQuoteAmount, ",'", CarrierQuoteNum, "'"
                         ,DB.GetCommaSingleQuoteValue(strPUDate.Trim()), DB.GetCommaSingleQuoteValue(strPUDate.Trim()), DB.GetCommaSingleQuoteValue(dtDeliveryDate.ToString("MM/dd/yyyy"))
                         ,DB.GetCommaSingleQuoteValue(DateTime.Now.ToString("MM/dd/yyyy")), DB.GetCommaSingleQuoteValue("G"), ",1", DB.GetCommaSingleQuoteValue(strInsurance)
                         ,",1", DB.GetCommaSingleQuoteValue(dtDeliveryDate.ToString("MM/dd/yyyy"))
                         ,",'", mySegStatus, "')");


            HelperFuncs.writeToSiteErrors("Dispatch into tbl_Segments sql", strSQL);

            comm.CommandText = strSQL;
            comm.ExecuteNonQuery();

            dispatch_data.intSegmentID = HelperFuncs.GetLastAutogeneratedID(comm);

            dispatch_data.strReportFileName = dispatch_data.username.ToString() + "_" + dispatch_data.intShipmentID.ToString() + "_" + dispatch_data.intSegmentID.ToString() + ".pdf";
            //Session["BOLReportName"] = strReportFileName;

            dispatch_data.strReportFileName2 = dispatch_data.username.ToString() + "_seg2_" + dispatch_data.intShipmentID.ToString() + "_" + dispatch_data.intSegmentID.ToString() + ".pdf";
            //Session["BOLReportName2"] = strReportFileName2;

            //dispatch_data.exportPath = Server.MapPath("BOLReports\\" + Session["BOLReportName"].ToString());
            //Session["BOLReportPath"] = exportPath;

            //here generate the new report path for the fedEx with new prefix fedEx
            //dispatch_data.strFedexFileName = dispatch_data.username.ToString() + "_FedEx_" + dispatch_data.intShipmentID.ToString() + "_" + dispatch_data.intSegmentID.ToString() + ".pdf";
            //Session["FedExReportName"] = strFedexFileName;

            //dispatch_data.exportPathFedex = Server.MapPath("BOLReports\\" + dispatch_data.FedExReportName);
            //Session["FedExReportPath"] = exportPathFedex;

            //dispatch_data.exportPath2 = Server.MapPath("BOLReports\\" + dispatch_data.BOLReportName2);
            //Session["BOLReportPath2"] = exportPath2;

            if (dispatch_data.carrier.EndsWith(" RRD"))
            {
                //CarrierQuoteNum = "RRD";

                if (!dispatch_data.DLS_PrimaryReferencePNW.Equals(string.Empty))
                {
                    if (dispatch_data.isDUR == true)
                    {
                        CarrierQuoteNum = string.Concat("CC", dispatch_data.DLS_PrimaryReferencePNW);
                    }
                    else
                    {
                        CarrierQuoteNum = dispatch_data.DLS_PrimaryReferencePNW;
                    }
                }
                else
                {
                    CarrierQuoteNum = "RRD";
                }

                //CarrierQuoteNum = dispatch_data.DLS_PrimaryReferencePNW;
                HelperFuncs.writeToSiteErrors("CarrierQuoteNum RRD shipID, carrierQuoteNum", dispatch_data.intShipmentID.ToString() + ", " + dispatch_data.DLS_PrimaryReferencePNW);
                HelperFuncs.writeToSiteErrors("CarrierQuoteNum RRD", CarrierQuoteNum);
            }
            //if (Session["showDLSRates"] != null && ((bool)Session["showDLSRates"]).Equals(true) &&
            //           !dispatch_data.carrierKey.Equals("Central Transport"))
            //{
            //    CarrierQuoteNum = "RRD";
            //    HelperFuncs.writeToSiteErrors("CarrierQuoteNum RRD shipID", intShipmentID.ToString());
            //}
            else
            {
                HelperFuncs.writeToSiteErrors("CarrierQuoteNum RRD shipID, carrierKey", dispatch_data.intShipmentID.ToString() + ", " + dispatch_data.carrierKey);
            }

            //Insert Segment2
            strSQL = string.Concat("insert into tbl_Segments", demo_table_name_addition, "(ShipmentID,CompID_CAR,CompID_DELIV,CompID_PU,CarrierQuoteAmt,CarrierQuoteNum,PUDate,DelivDate,DispatchDate,TransMode,DI,SegRmk1,SegmentNbr,ATA, SegStatus)"
                         ," values("
                         ,dispatch_data.intShipmentID, ",", intInterimCarrierCompID, ",", intDelCompID, ",", intInterimShipperCompID, ",", dblCarrierQuoteAmount, ",'", CarrierQuoteNum, "'"
                         ,DB.GetCommaSingleQuoteValue(dtDeliveryDate.ToString("MM/dd/yyyy")), DB.GetCommaSingleQuoteValue(dtDeliveryDate2.ToString("MM/dd/yyyy"))
                         ,DB.GetCommaSingleQuoteValue(DateTime.Now.ToString("MM/dd/yyyy")), DB.GetCommaSingleQuoteValue("G"), ",1", DB.GetCommaSingleQuoteValue(strInsurance)
                         ,",2", DB.GetCommaSingleQuoteValue(dtDeliveryDate2.ToString("MM/dd/yyyy"))
                         ,",'", mySegStatus, "')");


            HelperFuncs.writeToSiteErrors("Dispatch into tbl_Segments segment2 sql", strSQL);

            comm.CommandText = strSQL;
            comm.ExecuteNonQuery();

            dispatch_data.intSegmentID2 = HelperFuncs.GetLastAutogeneratedID(comm);

            //strReportFileName2 = dispatch_data.username.ToString() + "_seg2_" + intShipmentID.ToString() + "_" + dispatch_data.intSegmentID2.ToString() + ".pdf";
            //Session["BOLReportName2"] = strReportFileName2;

            //Insert into RATE QUOTES table
            double dblSellRate = 0;
            

            if (dispatch_data.rate != null && dispatch_data.rate.Length > 0 && double.TryParse(dispatch_data.rate, out dblRate))
                dblSellRate = dblRate;


            if (dispatch_data.txtSellRate == "")
            {
                dispatch_data.txtSellRate = "0";
            }

            double tempAesQuote = 0;

            Double.TryParse(dispatch_data.txtSellRate, out tempAesQuote);

            if (tempAesQuote == 0)
            {
                tempAesQuote = dblSellRate;
            }

            if (dispatch_data.hasInsurance.Equals("yes"))
            {
                dispatch_data.insuranceCost = dblShipmentValue * Convert.ToDouble(System.Web.Configuration.WebConfigurationManager.AppSettings["InsuranceSellRate"]);

                if (dispatch_data.insuranceCost < dispatch_data.minInsuranceCost)
                {
                    dispatch_data.insuranceCost = dispatch_data.minInsuranceCost;
                }

                Repository.SetInsuranceMinimum(ref dispatch_data, ref dispatch_data.insuranceCost, ref dblShipmentValue);
            }

            //double dblCarrierQuoteAmount = 0;      
            double DURRRDsell = 0;

            if (dispatch_data.isDUR == true || (dispatch_data.username != null && dispatch_data.username.ToString().Equals("AESW140S")))
            {
                DURRRDsell = Math.Round(dblCarrierQuoteAmount * 1.25, 2);

                if (dispatch_data.repName != null)
                {
                    // AES person booking on GCM customer account  
                    dispatch_data.Initials = dispatch_data.repName.ToString();

                }
                else
                {
                    dispatch_data.Initials = "GCM DISPATCHER";

                }

                strSQL = string.Concat("insert into tbl_RATEQUOTES", demo_table_name_addition, "(SEGMENTID,NetCharge,Initials, AESQuote, DURRRDsell)",
                             " values(",
                             dispatch_data.intSegmentID, ",", Math.Round(tempAesQuote, 2), DB.GetCommaSingleQuoteValue(dispatch_data.Initials),
                             ",", Math.Round((tempAesQuote + dispatch_data.insuranceCost), 2), ",", DURRRDsell, ")");

                HelperFuncs.writeToSiteErrors("Dispatch updating DURRRDsell", strSQL);
            }
            else
            {
                if (dispatch_data.repName != null)
                {
                    // AES person booking on GCM customer account    
                    dispatch_data.Initials = dispatch_data.repName.ToString();

                }
                else
                {
                    dispatch_data.Initials = "GCM DISPATCHER";

                }
                strSQL = string.Concat("insert into tbl_RATEQUOTES", demo_table_name_addition, 
                                "(SEGMENTID,NetCharge,Initials, AESQuote)"
                                 ," values("
                                 ,dispatch_data.intSegmentID, ",", tempAesQuote.ToString(),
                                 DB.GetCommaSingleQuoteValue(dispatch_data.Initials)
                                 ,", ", (tempAesQuote + dispatch_data.insuranceCost).ToString(), ")");
            }


            comm.CommandText = strSQL;
            comm.ExecuteNonQuery();

            dispatch_data.intRQID = HelperFuncs.GetLastAutogeneratedID(comm);


            if (dispatch_data.isDUR == true || dispatch_data.username.Equals("AESW140S"))
            {

                DURRRDsell = Math.Round(dblCarrierQuoteAmount * 1.25, 2);

                if (dispatch_data.repName != null)
                {
                    // AES person booking on GCM customer account   
                    dispatch_data.Initials = dispatch_data.repName.ToString();

                }
                else
                {
                    dispatch_data.Initials = "GCM DISPATCHER";

                }

                strSQL = string.Concat("insert into tbl_RATEQUOTES", demo_table_name_addition, 
                            "(SEGMENTID,NetCharge,Initials, AESQuote, DURRRDsell)",
                             " values(",
                             dispatch_data.intSegmentID2, ",", Math.Round(tempAesQuote, 2),
                             DB.GetCommaSingleQuoteValue(dispatch_data.Initials),
                             ",", Math.Round(tempAesQuote, 2), ",", DURRRDsell, ")");

                HelperFuncs.writeToSiteErrors("Dispatch updating DURRRDsell seg 2", strSQL);
            }
            else
            {
                if (dispatch_data.repName != null)
                {
                    // AES person booking on GCM customer account 
                    dispatch_data.Initials = dispatch_data.repName.ToString();

                }
                else
                {
                    dispatch_data.Initials = "GCM DISPATCHER";

                }
                strSQL = string.Concat("insert into tbl_RATEQUOTES", demo_table_name_addition, 
                                "(SEGMENTID,NetCharge,Initials, AESQuote)"
                                 ," values("
                                 ,dispatch_data.intSegmentID2, ",", tempAesQuote.ToString(),
                                 DB.GetCommaSingleQuoteValue(dispatch_data.Initials)
                                 ,", ", tempAesQuote.ToString(), ")");
            }


            comm.CommandText = strSQL;

            //////////// Insert Insurance cost////////
            if (dispatch_data.hasInsurance.Equals("yes"))
            {
                Repository.InsertInsuranceCost(ref dispatch_data, dblShipmentValue, dispatch_data.intRQID);
            }
            if (dispatch_data.hasInsurance.Equals("yes"))
            {
                //InsertInsuranceCost(dispatch_data.intSegmentID2, dblShipmentValue, dispatch_data.intRQID2, comm);
            }

            ArrayList services = new ArrayList();

            Repository.InsertAccessorialsIntoAccDetailsTable(ref dispatch_data, ref services, ref strSQL);

            // If session is null here fall back on txtDesc.Text
            string Desc1 = dispatch_data.txtDesc1, Desc2 = dispatch_data.txtDesc2, Desc3 = dispatch_data.txtDesc3, Desc4 = dispatch_data.txtDesc4;

            if (dispatch_data.DimsCubeDesc1 != null && dispatch_data.DimsCubeDesc1.ToString().Trim().Length > 0)
            {
                // Make sure dims are in format: customer description // dims
                Desc1 = Desc1.Replace(dispatch_data.DimsCubeDesc1.ToString().Trim(), " ") +
                    dispatch_data.DimsCubeDesc1.ToString();
            }
            //else
            //{
            //    HelperFuncs.writeToSiteErrors("Dispatch", "Session Desc1 was null");
            //}
            if (dispatch_data.DimsCubeDesc2 != null && dispatch_data.DimsCubeDesc2.ToString().Trim().Length > 0)
            {
                Desc2 = Desc2.Replace(dispatch_data.DimsCubeDesc2.ToString().Trim(), " ") +
                    dispatch_data.DimsCubeDesc2.ToString();
            }
            //else
            //{
            //    HelperFuncs.writeToSiteErrors("Dispatch", "Session Desc2 was null");
            //}
            if (dispatch_data.DimsCubeDesc3 != null && dispatch_data.DimsCubeDesc3.ToString().Trim().Length > 0)
            {
                Desc3 = Desc3.Replace(dispatch_data.DimsCubeDesc3.ToString().Trim(), " ") +
                    dispatch_data.DimsCubeDesc3.ToString();
            }
            //else
            //{
            //    HelperFuncs.writeToSiteErrors("Dispatch", "Session Desc3 was null");
            //}
            if (dispatch_data.DimsCubeDesc4 != null && dispatch_data.DimsCubeDesc4.ToString().Trim().Length > 0)
            {
                Desc4 = Desc4.Replace(dispatch_data.DimsCubeDesc4.ToString().Trim(), " ") +
                    dispatch_data.DimsCubeDesc4.ToString();
            }
            //else
            //{
            //    HelperFuncs.writeToSiteErrors("Dispatch", "Session Desc4 was null");
            //}

            //Insert all the item information
            string hazmat;
            string strPiece;
            bool isHazMatItem = false;
            int itemCount = 0;

            if (dispatch_data.q_Weight1 != null && !dispatch_data.q_Weight1.Equals(string.Empty))
            {
                itemCount++;
                if (dispatch_data.chkHazMat1)
                {
                    hazmat = "1";
                    if (isHazMatItem == false)
                    {
                        isHazMatItem = true;
                    }
                }
                else
                {
                    hazmat = "0";
                }

                if (dispatch_data.lblPiece1.Trim().Length > 0)
                    strPiece = dispatch_data.lblPiece1.Trim();
                else
                    strPiece = "0";

                if (dispatch_data.txtPallet1.Trim().Length > 0)
                {
                    strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,Units,WtLBS)"
                         + " values("
                         + dispatch_data.intShipmentID +
                         DB.GetCommaSingleQuoteValue(dispatch_data.lblClass1.Trim())
                         + DB.GetCommaSingleQuoteValue(Desc1.Trim()) + "," + hazmat
                         + DB.GetCommaSingleQuoteValue(dispatch_data.lblType1.Trim()) +
                         DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC1.Trim())
                         + "," + strPiece + DB.GetCommaSingleQuoteValue(dispatch_data.txtPallet1.Trim())
                         + "," + dispatch_data.lblWeight1.Trim()
                         + ")";
                }
                else
                {
                    strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,WtLBS)"
                         + " values("
                         + dispatch_data.intShipmentID +
                         DB.GetCommaSingleQuoteValue(dispatch_data.lblClass1.Trim())
                         + DB.GetCommaSingleQuoteValue(Desc1.Trim()) + "," + hazmat
                         + DB.GetCommaSingleQuoteValue(dispatch_data.lblType1.Trim()) +
                         DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC1.Trim())
                         + "," + strPiece
                         + "," + dispatch_data.lblWeight1.Trim()
                         + ")";
                }
                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();
            }
            if (dispatch_data.q_Weight2 != null && !dispatch_data.q_Weight2.Equals(string.Empty))
            {
                itemCount++;
                if (dispatch_data.chkHazMat2)
                {
                    hazmat = "1";
                    if (isHazMatItem == false)
                    {
                        isHazMatItem = true;
                    }
                }
                else
                    hazmat = "0";

                if (dispatch_data.lblPiece2.Trim().Length > 0)
                    strPiece = dispatch_data.lblPiece2.Trim();
                else
                    strPiece = "0";

                if (dispatch_data.txtPallet2.Trim().Length > 0)
                {
                    strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,Units,WtLBS)"
                         + " values("
                         + dispatch_data.intShipmentID +
                         DB.GetCommaSingleQuoteValue(dispatch_data.lblClass2.Trim())
                         + DB.GetCommaSingleQuoteValue(Desc2.Trim()) + "," + hazmat
                         + DB.GetCommaSingleQuoteValue(dispatch_data.lblType2.Trim()) +
                         DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC2.Trim())
                         + "," + strPiece + DB.GetCommaSingleQuoteValue(dispatch_data.txtPallet2.Trim())
                         + "," + dispatch_data.lblWeight2.Trim()
                         + ")";
                }
                else
                {
                    strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,WtLBS)"
                         + " values("
                         + dispatch_data.intShipmentID +
                         DB.GetCommaSingleQuoteValue(dispatch_data.lblClass2.Trim())
                         + DB.GetCommaSingleQuoteValue(Desc2.Trim()) + "," + hazmat
                         + DB.GetCommaSingleQuoteValue(dispatch_data.lblType2.Trim()) +
                         DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC2.Trim())
                         + "," + strPiece
                         + "," + dispatch_data.lblWeight2.Trim()
                         + ")";
                }
                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();
            }
            if (dispatch_data.q_Weight3 != null && !dispatch_data.q_Weight3.Equals(string.Empty))
            {
                itemCount++;
                if (dispatch_data.chkHazMat3)
                {
                    hazmat = "1";
                    if (isHazMatItem == false)
                    {
                        isHazMatItem = true;
                    }
                }
                else
                    hazmat = "0";

                if (dispatch_data.lblPiece3.Trim().Length > 0)
                    strPiece = dispatch_data.lblPiece3.Trim();
                else
                    strPiece = "0";

                if (dispatch_data.txtPallet3.Trim().Length > 0)
                {
                    strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,Units,WtLBS)"
                         + " values("
                         + dispatch_data.intShipmentID +
                         DB.GetCommaSingleQuoteValue(dispatch_data.lblClass3.Trim())
                         + DB.GetCommaSingleQuoteValue(Desc3.Trim()) + "," + hazmat
                         + DB.GetCommaSingleQuoteValue(dispatch_data.lblType3.Trim()) +
                         DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC3.Trim())
                         + "," + strPiece + DB.GetCommaSingleQuoteValue(dispatch_data.txtPallet3.Trim())
                         + "," + dispatch_data.lblWeight3.Trim()
                         + ")";
                }
                else
                {
                    strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,WtLBS)"
                         + " values("
                         + dispatch_data.intShipmentID +
                         DB.GetCommaSingleQuoteValue(dispatch_data.lblClass3.Trim())
                         + DB.GetCommaSingleQuoteValue(Desc3.Trim()) + "," + hazmat
                         + DB.GetCommaSingleQuoteValue(dispatch_data.lblType3.Trim()) +
                         DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC3.Trim())
                         + "," + strPiece
                         + "," + dispatch_data.lblWeight3.Trim()
                         + ")";
                }
                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();
            }
            if (dispatch_data.q_Weight4 != null && !dispatch_data.q_Weight4.Equals(string.Empty))
            {
                itemCount++;
                if (dispatch_data.chkHazMat4)
                {
                    hazmat = "1";
                    if (isHazMatItem == false)
                    {
                        isHazMatItem = true;
                    }
                }
                else
                    hazmat = "0";

                if (dispatch_data.lblPiece4.Trim().Length > 0)
                    strPiece = dispatch_data.lblPiece4.Trim();
                else
                    strPiece = "0";

                if (dispatch_data.txtPallet4.Trim().Length > 0)
                {
                    strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,Units,WtLBS)"
                         + " values("
                         + dispatch_data.intShipmentID +
                         DB.GetCommaSingleQuoteValue(dispatch_data.lblClass4.Trim())
                         + DB.GetCommaSingleQuoteValue(Desc4.Trim()) + "," + hazmat
                         + DB.GetCommaSingleQuoteValue(dispatch_data.lblType4.Trim()) +
                         DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC4.Trim())
                         + "," + strPiece + DB.GetCommaSingleQuoteValue(dispatch_data.txtPallet4.Trim())
                         + "," + dispatch_data.lblWeight4.Trim()
                         + ")";
                }
                else
                {
                    strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,WtLBS)"
                         + " values("
                         + dispatch_data.intShipmentID +
                         DB.GetCommaSingleQuoteValue(dispatch_data.lblClass4.Trim())
                         + DB.GetCommaSingleQuoteValue(Desc4.Trim()) + "," + hazmat
                         + DB.GetCommaSingleQuoteValue(dispatch_data.lblType4.Trim()) +
                         DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC4.Trim())
                         + "," + strPiece
                         + "," + dispatch_data.lblWeight4.Trim()
                         + ")";
                }
                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();
            }
            ////////////////////////////////////////////////////////////////////////////////

            //Insert PONumber into PO table
            foreach (string strPONo in dispatch_data.txtPONumber.Trim().Split(','))
            {
                strSQL = "insert into tbl_PO(ShipmentID,PONumber)"
                     + " values("
                     + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(strPONo.Trim())
                     + ")";
                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();
            }

        }

        #endregion

        #region SaveCompanyTableDispatchInfo

        private void SaveCompanyTableDispatchInfo(ref int intDelCompID, ref int intPUCompID, ref string strSQL)
        {
            #region Company Table
            object objTempID;
            // Insert Pickup Company Adress in Company table
            strSQL = "select top 1 CompID from tbl_Company where CompName=" + 
                DB.GetSingleQuoteValue(dispatch_data.txtPCompany.Trim())
                    + "and PUContact=" + DB.GetSingleQuoteValue(dispatch_data.txtPName.Trim())
                    + "and Addr1=" + DB.GetSingleQuoteValue(dispatch_data.txtPAddress1.Trim())
                    + "and Addr2=" + DB.GetSingleQuoteValue(dispatch_data.txtPAddress2.Trim())
                    + "and City=" + DB.GetSingleQuoteValue(dispatch_data.txtPCity.Trim())
                    + "and State=" + DB.GetSingleQuoteValue(dispatch_data.txtPST.Trim())
                    + "and Zip=" + DB.GetSingleQuoteValue(dispatch_data.txtPZip.Trim())
                    + "and PUEmail=" + DB.GetSingleQuoteValue(dispatch_data.txtPEmail.Trim())
                    + "and Phone=" + DB.GetSingleQuoteValue(dispatch_data.txtPPhone.Trim())
                    + "and Fax=" + DB.GetSingleQuoteValue(dispatch_data.txtPFax.Trim());

            comm.CommandText = strSQL;
            objTempID = comm.ExecuteScalar();

            if (objTempID != null)
            {
                intPUCompID = Convert.ToInt32(objTempID);
            }
            else
            {

                strSQL = "insert into tbl_Company(CompName,PUContact,Addr1,Addr2,City,State,Zip,PUEmail,Phone,Fax)"
                             + " values("
                             + DB.GetSingleQuoteValue(dispatch_data.txtPCompany.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtPName.Trim())
                             + DB.GetCommaSingleQuoteValue(dispatch_data.txtPAddress1.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtPAddress2.Trim())
                             + DB.GetCommaSingleQuoteValue(dispatch_data.txtPCity.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtPST.Trim())
                             + DB.GetCommaSingleQuoteValue(dispatch_data.txtPZip.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtPEmail.Trim())
                             + DB.GetCommaSingleQuoteValue(dispatch_data.txtPPhone.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtPFax.Trim())
                             + ")";
                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();

                intPUCompID = HelperFuncs.GetLastAutogeneratedID(comm);
            }
            //////////////////////////////////////////////////////////////////////////////////

            //Insert Delivery Company Adress in Company table
            strSQL = "select top 1 CompID from tbl_Company where CompName=" + DB.GetSingleQuoteValue(dispatch_data.txtDCompany.Trim())
                    + "and DeliveryContact=" + DB.GetSingleQuoteValue(dispatch_data.txtDName.Trim())
                    + "and Addr1=" + DB.GetSingleQuoteValue(dispatch_data.txtDAddress1.Trim())
                    + "and Addr2=" + DB.GetSingleQuoteValue(dispatch_data.txtDAddress2.Trim())
                    + "and City=" + DB.GetSingleQuoteValue(dispatch_data.txtDCity.Trim())
                    + "and State=" + DB.GetSingleQuoteValue(dispatch_data.txtDST.Trim())
                    + "and Zip=" + DB.GetSingleQuoteValue(dispatch_data.txtDZip.Trim())
                    + "and DeliveryEMail=" + DB.GetSingleQuoteValue(dispatch_data.txtDEmail.Trim())
                    + "and Phone=" + DB.GetSingleQuoteValue(dispatch_data.txtDPhone.Trim())
                    + "and Fax=" + DB.GetSingleQuoteValue(dispatch_data.txtDFax.Trim());

            comm.CommandText = strSQL;
            objTempID = comm.ExecuteScalar();

            if (objTempID != null)
            {
                intDelCompID = Convert.ToInt32(objTempID);
            }
            else
            {
                strSQL = "insert into tbl_Company(CompName,DeliveryContact,Addr1,Addr2,City,State,Zip,DeliveryEMail,Phone,Fax)"
                            + " values("
                            + DB.GetSingleQuoteValue(dispatch_data.txtDCompany.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDName.Trim())
                            + DB.GetCommaSingleQuoteValue(dispatch_data.txtDAddress1.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDAddress2.Trim())
                            + DB.GetCommaSingleQuoteValue(dispatch_data.txtDCity.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDST.Trim())
                            + DB.GetCommaSingleQuoteValue(dispatch_data.txtDZip.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDEmail.Trim())
                            + DB.GetCommaSingleQuoteValue(dispatch_data.txtDPhone.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtDFax.Trim())
                            + ")";
                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();

                intDelCompID = HelperFuncs.GetLastAutogeneratedID(comm);
            }
            #endregion
        }

        #endregion

        #region InsertIntoRateQuotesTable

        private void InsertIntoRateQuotesTable(ref double dblRate, ref double dblShipmentValue, ref string strSQL)
        {
            #region Insert into RATE QUOTES table
            // Insert into RATE QUOTES table

            double dblSellRate = 0;
            if (dispatch_data.rate != null && dispatch_data.rate.Length > 0 && double.TryParse(dispatch_data.rate, out dblRate))
            {
                dblSellRate = dblRate;
            }

            if (dispatch_data.txtSellRate == "")
            {
                dispatch_data.txtSellRate = "0";
            }

            //double insuranceCost = 0;

            Double.TryParse(dispatch_data.txtSellRate, out double tempAesQuote);

            if (tempAesQuote == 0)
            {
                tempAesQuote = dblSellRate;
            }

            #region Has insurance
            if (dispatch_data.hasInsurance != null && 
                dispatch_data.hasInsurance.Equals("yes"))
            {
                dispatch_data.insuranceCost = dblShipmentValue * Convert.ToDouble(System.Web.Configuration.WebConfigurationManager.AppSettings["InsuranceSellRate"]);

                if (dispatch_data.insuranceCost < dispatch_data.minInsuranceCost)
                {
                    dispatch_data.insuranceCost = dispatch_data.minInsuranceCost;
                }

                Repository.SetInsuranceMinimum(ref dispatch_data, ref dispatch_data.insuranceCost, ref dblShipmentValue);

                //tempAesQuote += insuranceCost;
            }
            #endregion

            //--
            double ourRate = 0.0;
            if (dispatch_data.ourRate != null)
            {
                double.TryParse(dispatch_data.ourRate, out ourRate);
            }

            if (dispatch_data.isAssociationID_5.Equals(true))
            {
                double assoc5SellRate = 0.0;
                // GetAssociationID_5_SellRate
                HelperFuncs.GetAssociationID_5_SellRate(dispatch_data.ourRate, ref assoc5SellRate);

                if (dispatch_data.repName != null)
                {
                    // AES person booking on GCM customer account             
                    HelperFuncs.InsertIntoRateQuotes(ref dispatch_data.intRQID, ref dispatch_data.intSegmentID, Math.Round(assoc5SellRate, 2).ToString(), dispatch_data.repName.ToString(),
                       Math.Round((assoc5SellRate + dispatch_data.insuranceCost), 2), ref ourRate, "LiveGCM");
                }
                else
                {
                    HelperFuncs.InsertIntoRateQuotes(ref dispatch_data.intRQID, ref dispatch_data.intSegmentID, Math.Round(assoc5SellRate, 2).ToString(), "GCM DISPATCHER",
                        Math.Round((assoc5SellRate + dispatch_data.insuranceCost), 2), ref ourRate, "LiveGCM");
                }
            }
            else if (dispatch_data.isDUR == true || dispatch_data.username.Equals("AESW140S"))
            {
                double dblCarrierQuoteAmount = 0;
                //double dblRate = 0;
                double DURRRDsell = 0;
                if (dispatch_data.ourRate != null && dispatch_data.ourRate.Length > 0 && double.TryParse(dispatch_data.ourRate, out dblCarrierQuoteAmount))
                {
                    //dblCarrierQuoteAmount = dblRate;
                    DURRRDsell = Math.Round(dblCarrierQuoteAmount * 1.25, 2);
                }

                //HelperFuncs.writeToSiteErrors("Dispatch updating DURRRDsell", strSQL);
                if (dispatch_data.repName != null)
                {
                    // AES person booking on GCM customer account             
                    HelperFuncs.InsertIntoRateQuotes(ref dispatch_data.intRQID, ref dispatch_data.intSegmentID, Math.Round(tempAesQuote, 2).ToString(), dispatch_data.repName.ToString(),
                       Math.Round((tempAesQuote + dispatch_data.insuranceCost), 2), ref ourRate, "LiveGCM");
                }
                else
                {
                    HelperFuncs.InsertIntoRateQuotes(ref dispatch_data.intRQID, ref dispatch_data.intSegmentID, Math.Round(tempAesQuote, 2).ToString(), "GCM DISPATCHER",
                        Math.Round((tempAesQuote + dispatch_data.insuranceCost), 2), ref ourRate, "LiveGCM");
                }
            }
            else
            {
                if (dispatch_data.repName != null)
                {
                    // AES person booking on GCM customer account  
                    dispatch_data.Initials = dispatch_data.repName.ToString();

                }
                else
                {
                    dispatch_data.Initials = "GCM DISPATCHER";

                }

                strSQL = string.Concat("insert into tbl_RATEQUOTES", demo_table_name_addition, 
                                "(SEGMENTID,NetCharge,Initials, AESQuote)"
                                 ," values("
                                 ,dispatch_data.intSegmentID, ",", tempAesQuote.ToString(), 
                                 DB.GetCommaSingleQuoteValue(dispatch_data.Initials)
                                 ,", ", (tempAesQuote + dispatch_data.insuranceCost).ToString(), ")");

                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();

                dispatch_data.intRQID = HelperFuncs.GetLastAutogeneratedID(comm);
            }


            #endregion
        }

        #endregion

        #region InsertIntoItemsTable

        private void InsertIntoItemsTable(ref bool isHazMatItem, ref int itemCount, ref string Desc1, ref string Desc2, ref string Desc3, ref string Desc4,
            ref string strSQL)
        {
            #region Check if dimensions are valid, if yes insert into Items table, if not - insert without dimensions
            // Insert all the item information
            // Check if dimensions are valid, if yes insert into Items table, if not - insert without dimensions
            string hazmat;
            string strPiece;

            if (dispatch_data.q_Weight1 != null && !dispatch_data.q_Weight1.Equals(string.Empty))
            {
                itemCount++;
                if (dispatch_data.chkHazMat1)
                {
                    hazmat = "1";
                    if (isHazMatItem == false)
                    {
                        isHazMatItem = true;
                    }
                }
                else
                {
                    hazmat = "0";
                }

                if (dispatch_data.lblPiece1.Trim().Length > 0)
                    strPiece = dispatch_data.lblPiece1.Trim();
                else
                    strPiece = "0";


                if (dispatch_data.txtDesc1.Length > 80)
                {
                    dispatch_data.txtDesc1 = dispatch_data.txtDesc1.Replace(",", "").Replace(" lbs", "#").Replace("lbs", "#").Replace("  ", " ").Trim();
                    if (dispatch_data.txtDesc1.Length > 80)
                    {
                        dispatch_data.txtDesc1 = dispatch_data.txtDesc1.Remove(80); //truncate description to 80 chars to fit into db, otherwise sql exception
                        Desc1 = Desc1.Remove(80);
                    }
                }
                //-----------------------------------------------------------------------------------------------------------------------------
                //check if dimensions are valid, if yes insert into Items table, if not - insert without dimensions
                double testDbl;
                if (!double.TryParse(dispatch_data.q_Length1, out testDbl) || dispatch_data.q_Length1 == "-1" ||
                                   dispatch_data.q_Length1 == "NaN" ||
                                   !double.TryParse(dispatch_data.q_Width1, out testDbl) || dispatch_data.q_Width1 == "-1" ||
                                   dispatch_data.q_Width1 == "NaN" ||
                                   !double.TryParse(dispatch_data.q_Height1, out testDbl) || dispatch_data.q_Height1 == "-1" ||
                                   dispatch_data.q_Height1 == "NaN")
                {
                    //dimensions not valid
                    if (dispatch_data.txtPallet1.Trim().Length > 0)
                    {
                        strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,Units,WtLBS)"
                             + " values("
                             + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(dispatch_data.lblClass1.Trim())
                                 + DB.GetCommaSingleQuoteValue(Desc1.Trim()) + "," + hazmat
                             + DB.GetCommaSingleQuoteValue(dispatch_data.lblType1.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC1.Trim())
                             + "," + strPiece + DB.GetCommaSingleQuoteValue(dispatch_data.txtPallet1.Trim())
                             + "," + dispatch_data.lblWeight1.Trim()
                             + ")";
                    }
                    else
                    {
                        strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,WtLBS)"
                             + " values("
                             + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(dispatch_data.lblClass1.Trim())
                                 + DB.GetCommaSingleQuoteValue(Desc1.Trim()) + "," + hazmat
                             + DB.GetCommaSingleQuoteValue(dispatch_data.lblType1.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC1.Trim())
                             + "," + strPiece
                             + "," + dispatch_data.lblWeight1.Trim()
                             + ")";
                    }
                }
                else  //dimensions are valid, insert them into DB
                {
                    if (dispatch_data.txtPallet1.Trim().Length > 0)
                    {
                        strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,Units,WtLBS,DimsHt,DimsL,DimsW)"
                             + " values("
                             + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(dispatch_data.lblClass1.Trim())
                             + DB.GetCommaSingleQuoteValue(Desc1) + "," + hazmat
                             + DB.GetCommaSingleQuoteValue(dispatch_data.lblType1.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC1.Trim())
                             + "," + strPiece + DB.GetCommaSingleQuoteValue(dispatch_data.txtPallet1.Trim())
                             + "," + dispatch_data.lblWeight1.Trim()
                             + "," + dispatch_data.q_Height1 + "," + dispatch_data.q_Length1   // add the dimensions
                             + "," + dispatch_data.q_Width1 + ")";
                    }
                    else
                    {
                        strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,WtLBS,DimsHt,DimsL,DimsW)"
                             + " values("
                             + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(dispatch_data.lblClass1.Trim())
                             + DB.GetCommaSingleQuoteValue(Desc1) + "," + hazmat
                             + DB.GetCommaSingleQuoteValue(dispatch_data.lblType1.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC1.Trim())
                             + "," + strPiece
                             + "," + dispatch_data.lblWeight1.Trim()
                             + "," + dispatch_data.q_Height1 + "," + dispatch_data.q_Length1   // add the dimensions
                             + "," + dispatch_data.q_Width1 + ")";
                    }
                }
                //-----------------------------------------------------------------------------------------------------------------------------
                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();
            }
            if (dispatch_data.q_Weight2 != null && !dispatch_data.q_Weight2.Equals(string.Empty))
            {
                itemCount++;
                if (dispatch_data.chkHazMat2)
                {
                    hazmat = "1";
                    if (isHazMatItem == false)
                    {
                        isHazMatItem = true;
                    }
                }
                else
                {
                    hazmat = "0";
                }

                if (dispatch_data.lblPiece2.Trim().Length > 0)
                    strPiece = dispatch_data.lblPiece2.Trim();
                else
                    strPiece = "0";

                if (dispatch_data.txtDesc2.Length > 80)
                {
                    dispatch_data.txtDesc2 = dispatch_data.txtDesc2.Replace(",", "").Replace(" lbs", "#").Replace("lbs", "#").Replace("  ", " ").Trim();
                    if (dispatch_data.txtDesc2.Length > 80)
                    {
                        dispatch_data.txtDesc2 = dispatch_data.txtDesc2.Remove(80); //truncate description to 80 chars to fit into db, otherwise sql exception
                        Desc2 = Desc2.Remove(80);
                    }
                }

                //-----------------------------------------------------------------------------------------------------------------------------
                //check if dimensions are valid, if yes insert into Items table, if not - insert without dimensions
                double testDbl;
                if (!double.TryParse(dispatch_data.q_Length2, out testDbl) || dispatch_data.q_Length2 == "-1" ||
                                   dispatch_data.q_Length2 == "NaN" ||
                                   !double.TryParse(dispatch_data.q_Width2, out testDbl) || dispatch_data.q_Width2 == "-1" ||
                                   dispatch_data.q_Width2 == "NaN" ||
                                   !double.TryParse(dispatch_data.q_Height2, out testDbl) || dispatch_data.q_Height2 == "-1" ||
                                   dispatch_data.q_Height2 == "NaN")
                {
                    //dimensions not valid
                    if (dispatch_data.txtPallet2.Trim().Length > 0)
                    {
                        strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,Units,WtLBS)"
                             + " values("
                             + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(dispatch_data.lblClass2.Trim())
                             + DB.GetCommaSingleQuoteValue(Desc2.Trim()) + "," + hazmat
                             + DB.GetCommaSingleQuoteValue(dispatch_data.lblType2.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC2.Trim())
                             + "," + strPiece + DB.GetCommaSingleQuoteValue(dispatch_data.txtPallet2.Trim())
                             + "," + dispatch_data.lblWeight2.Trim()
                             + ")";
                    }
                    else
                    {
                        strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,WtLBS)"
                             + " values("
                             + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(dispatch_data.lblClass2.Trim())
                             + DB.GetCommaSingleQuoteValue(Desc2.Trim()) + "," + hazmat
                             + DB.GetCommaSingleQuoteValue(dispatch_data.lblType2.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC2.Trim())
                             + "," + strPiece
                             + "," + dispatch_data.lblWeight2.Trim()
                             + ")";
                    }
                }
                else  //dimensions are valid, insert them into DB
                {
                    if (dispatch_data.txtPallet2.Trim().Length > 0)
                    {
                        strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,Units,WtLBS,DimsHt,DimsL,DimsW)"
                             + " values("
                             + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(dispatch_data.lblClass2.Trim())
                             + DB.GetCommaSingleQuoteValue(Desc2) + "," + hazmat
                             + DB.GetCommaSingleQuoteValue(dispatch_data.lblType2.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC2.Trim())
                             + "," + strPiece + DB.GetCommaSingleQuoteValue(dispatch_data.txtPallet2.Trim())
                             + "," + dispatch_data.lblWeight2.Trim()
                             + "," + dispatch_data.q_Height2 + "," + dispatch_data.q_Length2   // add the dimensions
                             + "," + dispatch_data.q_Width2 + ")";
                    }
                    else
                    {
                        strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,WtLBS,DimsHt,DimsL,DimsW)"
                             + " values("
                             + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(dispatch_data.lblClass2.Trim())
                             + DB.GetCommaSingleQuoteValue(Desc2) + "," + hazmat
                             + DB.GetCommaSingleQuoteValue(dispatch_data.lblType2.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC2.Trim())
                             + "," + strPiece
                             + "," + dispatch_data.lblWeight2.Trim()
                             + "," + dispatch_data.q_Height2 + "," + dispatch_data.q_Length2   // add the dimensions
                             + "," + dispatch_data.q_Width2 + ")";
                    }
                }
                //-----------------------------------------------------------------------------------------------------------------------------
                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();
            }
            if (dispatch_data.q_Weight3 != null && !dispatch_data.q_Weight3.Equals(string.Empty))
            {
                itemCount++;
                if (dispatch_data.chkHazMat3)
                {
                    hazmat = "1";
                    if (isHazMatItem == false)
                    {
                        isHazMatItem = true;
                    }
                }
                else
                {
                    hazmat = "0";
                }

                if (dispatch_data.lblPiece3.Trim().Length > 0)
                    strPiece = dispatch_data.lblPiece3.Trim();
                else
                    strPiece = "0";

                if (dispatch_data.txtDesc3.Length > 80)
                {
                    dispatch_data.txtDesc3 = dispatch_data.txtDesc3.Replace(",", "").Replace(" lbs", "#").Replace("lbs", "#").Replace("  ", " ").Trim();
                    if (dispatch_data.txtDesc3.Length > 80)
                    {
                        dispatch_data.txtDesc3 = dispatch_data.txtDesc3.Remove(80); //truncate description to 80 chars to fit into db, otherwise sql exception
                        Desc3 = Desc3.Remove(80);
                    }
                }

                //-----------------------------------------------------------------------------------------------------------------------------
                //check if dimensions are valid, if yes insert into Items table, if not - insert without dimensions
                double testDbl;
                if (!double.TryParse(dispatch_data.q_Length3, out testDbl) || dispatch_data.q_Length3 == "-1" ||
                                   dispatch_data.q_Length3 == "NaN" ||
                                   !double.TryParse(dispatch_data.q_Width3, out testDbl) || dispatch_data.q_Width3 == "-1" ||
                                   dispatch_data.q_Width3 == "NaN" ||
                                   !double.TryParse(dispatch_data.q_Height3, out testDbl) || dispatch_data.q_Height3 == "-1" ||
                                   dispatch_data.q_Height3 == "NaN")
                {
                    //dimensions not valid
                    if (dispatch_data.txtPallet3.Trim().Length > 0)
                    {
                        strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,Units,WtLBS)"
                             + " values("
                             + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(dispatch_data.lblClass3.Trim())
                             + DB.GetCommaSingleQuoteValue(Desc3.Trim()) + "," + hazmat
                             + DB.GetCommaSingleQuoteValue(dispatch_data.lblType3.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC3.Trim())
                             + "," + strPiece + DB.GetCommaSingleQuoteValue(dispatch_data.txtPallet3.Trim())
                             + "," + dispatch_data.lblWeight3.Trim()
                             + ")";
                    }
                    else
                    {
                        strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,WtLBS)"
                             + " values("
                             + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(dispatch_data.lblClass3.Trim())
                             + DB.GetCommaSingleQuoteValue(Desc3.Trim()) + "," + hazmat
                             + DB.GetCommaSingleQuoteValue(dispatch_data.lblType3.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC3.Trim())
                             + "," + strPiece
                             + "," + dispatch_data.lblWeight3.Trim()
                             + ")";
                    }
                }
                else  //dimensions are valid, insert them into DB
                {
                    if (dispatch_data.txtPallet3.Trim().Length > 0)
                    {
                        strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,Units,WtLBS,DimsHt,DimsL,DimsW)"
                             + " values("
                             + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(dispatch_data.lblClass3.Trim())
                             + DB.GetCommaSingleQuoteValue(Desc3) + "," + hazmat
                             + DB.GetCommaSingleQuoteValue(dispatch_data.lblType3.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC3.Trim())
                             + "," + strPiece + DB.GetCommaSingleQuoteValue(dispatch_data.txtPallet3.Trim())
                             + "," + dispatch_data.lblWeight3.Trim()
                             + "," + dispatch_data.q_Height3 + "," + dispatch_data.q_Length3   // add the dimensions
                             + "," + dispatch_data.q_Width3 + ")";
                    }
                    else
                    {
                        strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,WtLBS,DimsHt,DimsL,DimsW)"
                             + " values("
                             + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(dispatch_data.lblClass3.Trim())
                             + DB.GetCommaSingleQuoteValue(Desc3) + "," + hazmat
                             + DB.GetCommaSingleQuoteValue(dispatch_data.lblType3.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC3.Trim())
                             + "," + strPiece
                             + "," + dispatch_data.lblWeight3.Trim()
                             + "," + dispatch_data.q_Height3 + "," + dispatch_data.q_Length3   // add the dimensions
                             + "," + dispatch_data.q_Width3 + ")";
                    }
                }
                //-----------------------------------------------------------------------------------------------------------------------------
                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();
            }
            if (dispatch_data.q_Weight4 != null && !dispatch_data.q_Weight4.Equals(string.Empty))
            {
                itemCount++;
                if (dispatch_data.chkHazMat4)
                {
                    hazmat = "1";
                    if (isHazMatItem == false)
                    {
                        isHazMatItem = true;
                    }
                }
                else
                {
                    hazmat = "0";
                }

                if (dispatch_data.lblPiece4.Trim().Length > 0)
                    strPiece = dispatch_data.lblPiece4.Trim();
                else
                    strPiece = "0";

                if (dispatch_data.txtDesc4.Length > 80)
                {
                    dispatch_data.txtDesc4 = dispatch_data.txtDesc4.Replace(",", "").Replace(" lbs", "#").Replace("lbs", "#").Replace("  ", " ").Trim();
                    if (dispatch_data.txtDesc4.Length > 80)
                    {
                        dispatch_data.txtDesc4 = dispatch_data.txtDesc4.Remove(80); //truncate description to 80 chars to fit into db, otherwise sql exception
                        Desc4 = Desc4.Remove(80);
                    }
                }

                //-----------------------------------------------------------------------------------------------------------------------------
                //check if dimensions are valid, if yes insert into Items table, if not - insert without dimensions
                double testDbl;
                if (!double.TryParse(dispatch_data.q_Length4, out testDbl) || dispatch_data.q_Length4 == "-1" ||
                                   dispatch_data.q_Length4 == "NaN" ||
                                   !double.TryParse(dispatch_data.q_Width4, out testDbl) || dispatch_data.q_Width4 == "-1" ||
                                   dispatch_data.q_Width4 == "NaN" ||
                                   !double.TryParse(dispatch_data.q_Height4, out testDbl) || dispatch_data.q_Height4 == "-1" ||
                                   dispatch_data.q_Height4 == "NaN")
                {
                    //dimensions not valid
                    if (dispatch_data.txtPallet4.Trim().Length > 0)
                    {
                        strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,Units,WtLBS)"
                             + " values("
                             + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(dispatch_data.lblClass4.Trim())
                             + DB.GetCommaSingleQuoteValue(Desc4.Trim()) + "," + hazmat
                             + DB.GetCommaSingleQuoteValue(dispatch_data.lblType4.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC4.Trim())
                             + "," + strPiece + DB.GetCommaSingleQuoteValue(dispatch_data.txtPallet4.Trim())
                             + "," + dispatch_data.lblWeight4.Trim()
                             + ")";
                    }
                    else
                    {
                        strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,WtLBS)"
                             + " values("
                             + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(dispatch_data.lblClass4.Trim())
                             + DB.GetCommaSingleQuoteValue(Desc4.Trim()) + "," + hazmat
                             + DB.GetCommaSingleQuoteValue(dispatch_data.lblType4.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC4.Trim())
                             + "," + strPiece
                             + "," + dispatch_data.lblWeight4.Trim()
                             + ")";
                    }
                }
                else  //dimensions are valid, insert them into DB
                {
                    if (dispatch_data.txtPallet4.Trim().Length > 0)
                    {
                        strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,Units,WtLBS,DimsHt,DimsL,DimsW)"
                             + " values("
                             + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(dispatch_data.lblClass4.Trim())
                             + DB.GetCommaSingleQuoteValue(Desc4) + "," + hazmat
                             + DB.GetCommaSingleQuoteValue(dispatch_data.lblType4.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC4.Trim())
                             + "," + strPiece + DB.GetCommaSingleQuoteValue(dispatch_data.txtPallet4.Trim())
                             + "," + dispatch_data.lblWeight4.Trim()
                             + "," + dispatch_data.q_Height4 + "," + dispatch_data.q_Length4   // add the dimensions
                             + "," + dispatch_data.q_Width4 + ")";
                    }
                    else
                    {
                        strSQL = "insert into tbl_Items(SHIPMENTID,Class,Descr,HR,Kind,Nmfc,Pcs,WtLBS,DimsHt,DimsL,DimsW)"
                             + " values("
                             + dispatch_data.intShipmentID + DB.GetCommaSingleQuoteValue(dispatch_data.lblClass4.Trim())
                             + DB.GetCommaSingleQuoteValue(Desc4) + "," + hazmat
                             + DB.GetCommaSingleQuoteValue(dispatch_data.lblType4.Trim()) + DB.GetCommaSingleQuoteValue(dispatch_data.txtNMFC4.Trim())
                             + "," + strPiece
                             + "," + dispatch_data.lblWeight4.Trim()
                             + "," + dispatch_data.q_Height4 + "," + dispatch_data.q_Length4   // add the dimensions
                             + "," + dispatch_data.q_Width4 + ")";
                    }
                }
                //-----------------------------------------------------------------------------------------------------------------------------
                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();
            }
            #endregion
        }

        #endregion

        #region InsertAllFreightInformation

        private void InsertAllFreightInformation(ref string strSQL, ref int intDispatchID)
        {
            #region Variables

            double dblResult;
            bool blnResult;
            int hazMat;
            int intResult;
            int pieceValue;

            string class1 = dispatch_data.q_Class1;
            string class2 = dispatch_data.q_Class2;
            string class3 = dispatch_data.q_Class3;
            string class4 = dispatch_data.q_Class4;
            if (class1 == "")
            {
                class1 = "-1";
                class2 = "-1";
                class3 = "-1";
                class4 = "-1";
            }

            #endregion

            // Insert information of Item 1/////////////////////////////
            if (dispatch_data.q_Weight1 != null && dispatch_data.q_Weight1.Length > 0 && double.TryParse(dispatch_data.q_Weight1, out dblResult))
            {
                if (dispatch_data.q_HazMat1 != null && bool.TryParse(dispatch_data.q_HazMat1, out blnResult))
                {
                    if (blnResult)
                        hazMat = 1;
                    else
                        hazMat = 0;
                }
                else
                    hazMat = 0;

                if (dispatch_data.q_Piece1 != null && dispatch_data.q_Piece1.Length > 0 && 
                    int.TryParse(dispatch_data.q_Piece1, out intResult))
                    pieceValue = intResult;
                else
                    pieceValue = -1;

                if (pieceValue >= 0)
                    strSQL = "insert into tbl_DispatchPieces(DispatchId,Pieces,Weight,FreightClass,UnitType,isHaz)"
                     + " values("
                     + intDispatchID + "," + pieceValue + "," + dispatch_data.q_Weight1 + ","
                     + class1 + DB.GetCommaSingleQuoteValue(Helper.GetUnitDescFromUnitKey(dispatch_data.q_Unit1)) + 
                     "," + hazMat
                    + ")";
                else
                    strSQL = "insert into tbl_DispatchPieces(DispatchId,Weight,FreightClass,UnitType,isHaz)"
                     + " values("
                     + intDispatchID + "," + dispatch_data.q_Weight1 + ","
                     + class1 + DB.GetCommaSingleQuoteValue(Helper.GetUnitDescFromUnitKey(dispatch_data.q_Unit1)) + 
                     "," + hazMat
                    + ")";

                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();

            }
            //////////////////////////////////////////////
            // Insert information of Item 2/////////////////////////////
            if (dispatch_data.q_Weight2 != null && dispatch_data.q_Weight2.Length > 0 && double.TryParse(dispatch_data.q_Weight2, out dblResult))
            {
                if (dispatch_data.q_HazMat2 != null && bool.TryParse(dispatch_data.q_HazMat2, out blnResult))
                {
                    if (blnResult)
                        hazMat = 1;
                    else
                        hazMat = 0;
                }
                else
                    hazMat = 0;

                if (dispatch_data.q_Piece2 != null && dispatch_data.q_Piece2.Length > 0 && 
                    int.TryParse(dispatch_data.q_Piece2, out intResult))
                    pieceValue = intResult;
                else
                    pieceValue = -1;

                if (pieceValue >= 0)
                    strSQL = "insert into tbl_DispatchPieces(DispatchId,Pieces,Weight,FreightClass,UnitType,isHaz)"
                     + " values("
                     + intDispatchID + "," + pieceValue + "," + dispatch_data.q_Weight2 + ","
                     + class2 + DB.GetCommaSingleQuoteValue(Helper.GetUnitDescFromUnitKey(dispatch_data.q_Unit2)) + 
                     "," + hazMat
                    + ")";
                else
                    strSQL = "insert into tbl_DispatchPieces(DispatchId,Weight,FreightClass,UnitType,isHaz)"
                     + " values("
                     + intDispatchID + "," + dispatch_data.q_Weight2 + ","
                     + class2 + DB.GetCommaSingleQuoteValue(Helper.GetUnitDescFromUnitKey(dispatch_data.q_Unit2)) + 
                     "," + hazMat
                    + ")";

                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();
            }
            //////////////////////////////////////////////
            // Insert information of Item 3/////////////////////////////
            if (dispatch_data.q_Weight3 != null && dispatch_data.q_Weight3.Length > 0 && double.TryParse(dispatch_data.q_Weight3, out dblResult))
            {
                if (dispatch_data.q_HazMat3 != null && bool.TryParse(dispatch_data.q_HazMat3, out blnResult))
                {
                    if (blnResult)
                        hazMat = 1;
                    else
                        hazMat = 0;
                }
                else
                    hazMat = 0;

                if (dispatch_data.q_Piece3 != null && dispatch_data.q_Piece3.Length > 0 && 
                    int.TryParse(dispatch_data.q_Piece3, out intResult))
                    pieceValue = intResult;
                else
                    pieceValue = -1;

                if (pieceValue >= 0)
                    strSQL = "insert into tbl_DispatchPieces(DispatchId,Pieces,Weight,FreightClass,UnitType,isHaz)"
                     + " values("
                     + intDispatchID + "," + pieceValue + "," + dispatch_data.q_Weight3 + ","
                     + class3 + DB.GetCommaSingleQuoteValue(Helper.GetUnitDescFromUnitKey(dispatch_data.q_Unit3)) + "," + hazMat
                    + ")";
                else
                    strSQL = "insert into tbl_DispatchPieces(DispatchId,Weight,FreightClass,UnitType,isHaz)"
                     + " values("
                     + intDispatchID + "," + dispatch_data.q_Weight3 + ","
                     + class3 + DB.GetCommaSingleQuoteValue(Helper.GetUnitDescFromUnitKey(dispatch_data.q_Unit3)) + "," + hazMat
                    + ")";

                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();
            }
            //////////////////////////////////////////////
            // Insert information of Item 4/////////////////////////////
            if (dispatch_data.q_Weight4 != null && dispatch_data.q_Weight4.Length > 0 && double.TryParse(dispatch_data.q_Weight4, out dblResult))
            {
                if (dispatch_data.q_HazMat4 != null && bool.TryParse(dispatch_data.q_HazMat4, out blnResult))
                {
                    if (blnResult)
                        hazMat = 1;
                    else
                        hazMat = 0;
                }
                else
                    hazMat = 0;

                if (dispatch_data.q_Piece4 != null && dispatch_data.q_Piece4.Length > 0 && 
                    int.TryParse(dispatch_data.q_Piece4, out intResult))
                    pieceValue = intResult;
                else
                    pieceValue = -1;

                if (pieceValue >= 0)
                    strSQL = "insert into tbl_DispatchPieces(DispatchId,Pieces,Weight,FreightClass,UnitType,isHaz)"
                     + " values("
                     + intDispatchID + "," + pieceValue + "," + dispatch_data.q_Weight4 + ","
                     + class4 + DB.GetCommaSingleQuoteValue(Helper.GetUnitDescFromUnitKey(dispatch_data.q_Unit4)) + "," + hazMat
                    + ")";
                else
                    strSQL = "insert into tbl_DispatchPieces(DispatchId,Weight,FreightClass,UnitType,isHaz)"
                     + " values("
                     + intDispatchID + "," + dispatch_data.q_Weight4 + ","
                     + class4 + DB.GetCommaSingleQuoteValue(Helper.GetUnitDescFromUnitKey(dispatch_data.q_Unit4)) + "," + hazMat
                    + ")";

                comm.CommandText = strSQL;
                comm.ExecuteNonQuery();
            }
            //////////////////////////////////////////////

        }

        #endregion

        #region GetCarrierCompanyID

        private int GetCarrierCompanyID(string strCarrierName)
        {
            //Get Carrier Company ID
            string strSQL = "";
            //string strCarrierName = "";
            if (string.IsNullOrEmpty(strCarrierName) && dispatch_data.carrier != null)
            {
                strCarrierName = dispatch_data.carrier.ToString().Trim();
            }

            if (strCarrierName.ToLower().Contains(" rrd"))
            {
                HelperFuncs.writeToSiteErrors("carNameTest", "RRD");
                strSQL = SharedLTL.GetDLS_CarrierCompanyID(ref strCarrierName);
            }
            else
            {
                HelperFuncs.writeToSiteErrors("carNameTest", "NOT RRD");
            }

            if (strSQL.Equals(string.Empty)) // If it's RRD, it may have not caught the CompName, try the regular way
            {
                HelperFuncs.getSQL_ForSelectCarrierCompID_ByCompName(ref strSQL, strCarrierName.ToLower());
            }

            comm.CommandText = strSQL;

            // Test
            string errorLoggingStr = string.Concat("CarrierName: ", strCarrierName, " sql: ", strSQL);

            HelperFuncs.writeToSiteErrors("carNameTest", errorLoggingStr);

            HelperFuncs.MailUser(AppCodeConstants.Alex_email, errorLoggingStr, "", "", "cs" + AppCodeConstants.email_domain, "dispatch logs carrier and sql", "");

            dispatch_data.intCarrierCompID = -1;
            object objCarrierID;

            objCarrierID = comm.ExecuteScalar();
            if (objCarrierID != null)
            {
                dispatch_data.intCarrierCompID = Convert.ToInt32(objCarrierID);
            }

            HelperFuncs.writeToSiteErrors("carNameTest intCarrierCompID", dispatch_data.intCarrierCompID.ToString());

            return dispatch_data.intCarrierCompID;

        }

        #endregion

    }
}