#region Using

using gcmAPI.Models.Misc;
using gcmAPI.Models.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

#endregion

namespace gcmAPI.Models.Dispatch
{
    public static class Repository
    {
        #region InsertAccessorialsIntoAccDetailsTable

        public static void InsertAccessorialsIntoAccDetailsTable(ref DispatchData dispatch_data, ref ArrayList services, ref string strSQL)
        {
            bool blnResult;

            #region Not used
            //string rateType = dispatch_data.rateType.ToString(), AMPM = "";
            //if (rateType == "GUARANTEEDAM")
            //{
            //    AMPM = "Noon";
            //}
            // else if (rateType == "GUARANTEEDPM")
            #endregion

            #region Logging
            if (dispatch_data.carrier != null)
            {
                HelperFuncs.writeToSiteErrors("carrier", dispatch_data.carrier);
            }
            else
            {
                HelperFuncs.writeToSiteErrors("carrier", "was null");
            }
            #endregion

            #region Insert all accessorials
            if (dispatch_data.rateType.Equals("GUARANTEEDPM")) //dispatch_data.carrier.Contains("R+L GDD QT:") || 
            {
                HelperFuncs.writeToSiteErrors("adding guaranteed", "adding guaranteed");

                Get_Accessorial_ID_and_insert(ref dispatch_data, ref services, "GSD", "GUARANTEED SERVICE");
            }

            if (dispatch_data.q_ResPick != null && bool.TryParse(dispatch_data.q_ResPick, out blnResult))
            {
                //Get the lookup ID for Residential Pickup.
                if (blnResult)
                {
                    Get_Accessorial_ID_and_insert(ref dispatch_data, ref services, "RSP", "RESIDENTIAL PICKUP");
                }
            }
            if (dispatch_data.q_ResDel != null && bool.TryParse(dispatch_data.q_ResDel, out blnResult))
            {
                //Get the lookup ID for Residential Delivery.
                if (blnResult)
                {
                    Get_Accessorial_ID_and_insert(ref dispatch_data, ref services, "RSD", "RESIDENTIAL DELIVERY");
                }
            }
            if (dispatch_data.q_ConstPick != null && bool.TryParse(dispatch_data.q_ConstPick, out blnResult))
            {
                //Get the lookup ID for NON COMMERCIAL Pickup.
                if (blnResult)
                {
                    Get_Accessorial_ID_and_insert(ref dispatch_data, ref services, "CSP", "NON COMMERCIAL PICKUP");
                }
            }
            if (dispatch_data.q_ConstDel != null && bool.TryParse(dispatch_data.q_ConstDel, out blnResult))
            {
                //Get the lookup ID for NON COMMERCIAL Delivery.
                if (blnResult)
                {
                    Get_Accessorial_ID_and_insert(ref dispatch_data, ref services, "CSD", "NON COMMERCIAL DELIVERY");
                }
            }
            if (dispatch_data.q_TradePick != null && bool.TryParse(dispatch_data.q_TradePick, out blnResult))
            {
                //Get the lookup ID for Tradeshow Pickup.
                if (blnResult)
                {
                    Get_Accessorial_ID_and_insert(ref dispatch_data, ref services, "TSP", "TRADESHOW PICKUP");
                }
            }

            if (dispatch_data.q_TradeDel != null && bool.TryParse(dispatch_data.q_TradeDel, out blnResult))
            {
                //Get the lookup ID for Residential Delivery.
                if (blnResult)
                {
                    Get_Accessorial_ID_and_insert(ref dispatch_data, ref services, "TSD", "TRADESHOW DELIVERY");
                }
            }

            if (dispatch_data.q_TailPick != null && bool.TryParse(dispatch_data.q_TailPick, out blnResult))
            {
                //Get the lookup ID for LIFTGATE PICKUP.
                if (blnResult)
                {
                    Get_Accessorial_ID_and_insert(ref dispatch_data, ref services, "TGP", "LIFTGATE PICKUP");
                }
            }

            if (dispatch_data.q_TailDel != null && bool.TryParse(dispatch_data.q_TailDel, out blnResult))
            {
                //Get the lookup ID for LIFTGATE DELIVERY.
                if (blnResult)
                {
                    Get_Accessorial_ID_and_insert(ref dispatch_data, ref services, "TGD", "LIFTGATE DELIVERY");
                }
            }

            if (dispatch_data.q_AppPick != null && bool.TryParse(dispatch_data.q_AppPick, out blnResult))
            {
                //Get the lookup ID for NOTIFICATION PICKUP.
                if (blnResult)
                {
                    Get_Accessorial_ID_and_insert(ref dispatch_data, ref services, "AMP", "DELIVERY APPT.");
                }
            }
            if (dispatch_data.q_AppDel != null && bool.TryParse(dispatch_data.q_AppDel, out blnResult))
            {
                //Get the lookup ID for NOTIFICATION DELIVERY.
                if (blnResult)
                {
                    Get_Accessorial_ID_and_insert(ref dispatch_data, ref services, "AMD", "DELIVERY APPT.");

                }
            }
            if (dispatch_data.q_InsDel != null && bool.TryParse(dispatch_data.q_InsDel, out blnResult))
            {
                //Get the lookup ID for INSIDE DELIVERY.
                if (blnResult)
                {
                    Get_Accessorial_ID_and_insert(ref dispatch_data, ref services, "ISD", "INSIDE DELIVERY");
                }
            }
            #endregion

        }

        #endregion

        #region Get_Accessorial_ID_and_insert
        private static void Get_Accessorial_ID_and_insert(ref DispatchData dispatch_data, ref ArrayList services, 
            string service_name, string LkUpValue)
        {
            services.Add(service_name);
            string strSQL = string.Concat("select LkUpID from tbl_LKUP where LkUpValue='", LkUpValue, "'");

            using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringLTLRater))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = strSQL;
                    object objID;
                    objID = command.ExecuteScalar();
                    if (objID != null)
                    {
                        Repository.InsertAdditionalService(dispatch_data.intSegmentID, Convert.ToInt32(objID));
                    }
                }
            }   
        }
        #endregion

        #region InsertAdditionalService

        public static void InsertAdditionalService(int intSegmentID, int intServiceID)
        {
            string strSQL =
                "insert into tbl_ACCDetail(SEGMENTID,LKUPID_ACC,ACCType)"
                 + " values("
                 + intSegmentID + "," + intServiceID + DB.GetCommaSingleQuoteValue("SEG")
                 + ")";
           
            HelperFuncs.ExecuteNonQuery(AppCodeConstants.connStringAesData, ref strSQL, 
                "dispatch InsertAdditionalService");
        }

        #endregion

        #region InsertInsuranceCost

        public static void InsertInsuranceCost(ref DispatchData dispatch_data, double shipmentValue, int intRQID)
        {
            string strSQL;
            int intServiceID = 291;
            double insuranceCost = shipmentValue * AppCodeConstants.InsuranceSellRate;
            if (insuranceCost < dispatch_data.minInsuranceCost)
            {
                insuranceCost = dispatch_data.minInsuranceCost;
            }

            SetInsuranceMinimum(ref dispatch_data, ref insuranceCost, ref shipmentValue);

            strSQL = "insert into tbl_ACCDetail(SEGMENTID,LKUPID_ACC,ACCType,ChargeAmt,RQID)"
                 + " values("
                 + dispatch_data.intSegmentID + "," + intServiceID + DB.GetCommaSingleQuoteValue("SEG") + "," + 
                 insuranceCost + "," + intRQID
                 + ")";
           
            HelperFuncs.ExecuteNonQuery(AppCodeConstants.connStringAesData, ref strSQL,
                "dispatch InsertInsuranceCost");
        }

        #endregion

        #region SetInsuranceMinimum

        public static void SetInsuranceMinimum(ref DispatchData dispatch_data, ref double insuranceCost, ref double dblShipmentValue)
        {
            if (dispatch_data.commodityName != null)
            {
                HelperFuncs.writeToSiteErrors("requestedValues commodityName",
                    dispatch_data.commodityName);

                if (dispatch_data.commodityName.Contains("Antiques, Artwork, Sculpture"))
                {
                    HelperFuncs.writeToSiteErrors("requestedValues commodityName Contains Antiques",
                        dispatch_data.commodityName);
                    insuranceCost = dblShipmentValue * .0125;
                    if (insuranceCost < 65.00)
                    {
                        insuranceCost = 65.00;
                    }
                }
            }

            if (dispatch_data.Commodity1 != null)
            {
                HelperFuncs.writeToSiteErrors("Session Commodity1", dispatch_data.Commodity1.ToString());
                if ((dispatch_data.Commodity1 != null && dispatch_data.Commodity1 != "" && dispatch_data.Commodity1.ToString().Contains("Antiques, Artwork, Sculpture")) ||
                   (dispatch_data.Commodity2 != null && dispatch_data.Commodity2 != "" && dispatch_data.Commodity2.ToString().Contains("Antiques, Artwork, Sculpture")) ||
                   (dispatch_data.Commodity3 != null && dispatch_data.Commodity3 != "" && dispatch_data.Commodity3.ToString().Contains("Antiques, Artwork, Sculpture")) ||
                   (dispatch_data.Commodity4 != null && dispatch_data.Commodity4 != "" && dispatch_data.Commodity4.ToString().Contains("Antiques, Artwork, Sculpture")))
                {
                    HelperFuncs.writeToSiteErrors("Session[Commodity1].ToString()", dispatch_data.Commodity1.ToString());
                    insuranceCost = dblShipmentValue * .0125;
                    if (insuranceCost < 65.00)
                    {
                        insuranceCost = 65.00;
                    }
                }
            }


            #region For HHG, set insurance cost

            // If household goods
            if ((dispatch_data.Commodity1 != null && dispatch_data.Commodity1 != "" && dispatch_data.Commodity1.ToString() == "Household Goods") ||
                (dispatch_data.Commodity2 != null && dispatch_data.Commodity2 != "" && dispatch_data.Commodity2.ToString() == "Household Goods") ||
                (dispatch_data.Commodity3 != null && dispatch_data.Commodity3 != "" && dispatch_data.Commodity3.ToString() == "Household Goods") ||
                (dispatch_data.Commodity4 != null && dispatch_data.Commodity4 != "" && dispatch_data.Commodity4.ToString() == "Household Goods"))
            {
                insuranceCost = dblShipmentValue * .0125;
                if (insuranceCost < 65.00)
                {
                    insuranceCost = 65.00;
                }
            }

            #endregion

            //bool.TryParse(dispatch_data.isUSED, out isUSED);

            //HelperFuncs.writeToSiteErrors("dispatch IsUSED", isUSED.ToString());

            //

            bool isRESDEL = false;
            if (dispatch_data.q_ResDel != null)
            {
                bool.TryParse(dispatch_data.q_ResDel, out isRESDEL);
            }

            HelperFuncs.writeToSiteErrors("dispatch isRESDEL", isRESDEL.ToString());

            if (dispatch_data.isUSED.Equals(true) && isRESDEL.Equals(true))
            {
                if (insuranceCost < 65.00)
                {
                    insuranceCost = 65.00;
                }
            }

            HelperFuncs.writeToSiteErrors("dispatch insuranceCost", insuranceCost.ToString());
        }

        #endregion

        #region GetInterimCarrierCompanyID

        public static int GetInterimCarrierCompanyID(ref DispatchData dispatch_data)
        {
            object objTempID;
            int intInterimCarrierCompID;
            string strSQL = "";
            Company interimCompany = Helper.GetInterimCompanyDetails(ref dispatch_data);

            strSQL = "select top 1 CompID from tbl_Company where CompName=" + DB.GetSingleQuoteValue(interimCompany.CompName)
                    + "and Carrier=1 "
                    + "and Addr1=" + DB.GetSingleQuoteValue(interimCompany.Addr1)
                    + "and Addr2=" + DB.GetSingleQuoteValue(interimCompany.Addr2)
                    + "and City=" + DB.GetSingleQuoteValue(interimCompany.City)
                    + "and State=" + DB.GetSingleQuoteValue(interimCompany.State)
                    + "and Zip=" + DB.GetSingleQuoteValue(interimCompany.Zip)
                    + "and Phone=" + DB.GetSingleQuoteValue(interimCompany.Phone)
                    + "and Fax=" + DB.GetSingleQuoteValue("");

            //comm.CommandText = strSQL;
            objTempID = HelperFuncs.ExecuteScalar(AppCodeConstants.connStringAesData, ref strSQL, 
                "Dispatch GetInterimCarrierCompanyID");
            //

            //
            if (objTempID != null)
                intInterimCarrierCompID = Convert.ToInt32(objTempID);
            else
            {
                strSQL = "insert into tbl_Company(CompName,Carrier,Addr1,Addr2,City,State,Zip,Phone,Fax)"
                            + " values("
                            + DB.GetSingleQuoteValue(interimCompany.CompName) + ",1"
                            + DB.GetCommaSingleQuoteValue(interimCompany.Addr1) + DB.GetCommaSingleQuoteValue(interimCompany.Addr2)
                            + DB.GetCommaSingleQuoteValue(interimCompany.City) + DB.GetCommaSingleQuoteValue(interimCompany.State)
                            + DB.GetCommaSingleQuoteValue(interimCompany.Zip) + DB.GetCommaSingleQuoteValue(interimCompany.Phone)
                            + DB.GetCommaSingleQuoteValue("")
                            + ")";
                //comm.CommandText = strSQL;
                //comm.ExecuteNonQuery();

                //intInterimCarrierCompID = HelperFuncs.GetLastAutogeneratedID(comm);

                intInterimCarrierCompID = HelperFuncs.ExecuteNonQuery_GetLastAutogeneratedID(
                    AppCodeConstants.connStringAesData, ref strSQL, "Dispatch GetInterimCarrierCompanyID");

            }

            return intInterimCarrierCompID;
        }

        #endregion
    }
}