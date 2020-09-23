using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace gcmAPI.Models.AAFES
{
    public static class AAFES
    {
        #region AAFES

        #region isAAFES_Shipment
        public static bool IsAAFES_Shipment(int shipID)
        {
            bool isAAFES_Shipment = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
                {
                    string sql = string.Concat("SELECT CompID_CUST FROM tbl_SHIPMENTS ",
                                               "WHERE ShipmentID=", shipID);

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["CompID_CUST"] != DBNull.Value)
                                {
                                    if (AppCodeConstants.aafesIDs.Contains((int)reader["CompID_CUST"]))
                                    {
                                        isAAFES_Shipment = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToAAFES_Logs("IsAAFES_Shipment", e.ToString());
            }

            return isAAFES_Shipment;
        }
        #endregion

        #region IsAAFES_Quote
        public static bool IsAAFES_Quote(int quoteID)
        {
            bool IsAAFES_Quote = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAAFES))
                {
                    string sql = string.Concat("SELECT QuoteID FROM PO ",
                                               "WHERE QuoteID=", quoteID);

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                IsAAFES_Quote = true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToAAFES_Logs("IsAAFES_Quote", e.ToString());
            }

            return IsAAFES_Quote;
        }
        #endregion

        #region isAAFES_ShipmentBooked
        public static bool isAAFES_ShipmentBooked(int poID, ref int shipID)
        {
            bool ShipmentBooked = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAAFES))
                {
                    string sql = string.Concat("SELECT GCMShipmentID FROM PO ",
                                               "WHERE ID=", poID);

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["GCMShipmentID"] != DBNull.Value)
                                {
                                    //AAFES.writeToSiteErrors("shipid", reader["GCMShipmentID"].ToString());
                                    if ((int)reader["GCMShipmentID"] > 0)
                                    {
                                        shipID = (int)reader["GCMShipmentID"];
                                        ShipmentBooked = true;
                                        //AAFES.writeToSiteErrors("shipid", "was booked");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("shipid", e.ToString());
            }

            return ShipmentBooked;
        }
        #endregion

        #region GetAAFES_LocationsQuoteInfo
        public static void GetAAFES_LocationsQuoteInfo(ref AAFES.AAFES_ShipInfo shipInfo, ref int QuoteID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAAFES))
                {
                    #region SQL
                    string sql = string.Concat("SELECT PO.ID, ",
                        "origLocation.Address1 AS origLocationAddress1, origLocation.Address2 AS origLocationAddress2, origLocation.Address3 AS origLocationAddress3, ",
                        "origLocation.City AS origLocationCity, origLocation.State AS origLocationState, origLocation.ZipCode AS origLocationZipCode, ",
                        "origLocation.CompanyName AS origCompanyName, ",
                        "destLocation.Address1 AS destLocationAddress1, destLocation.Address2 AS destLocationAddress2, destLocation.Address3 AS destLocationAddress3, ",
                        "destLocation.City AS destLocationCity, destLocation.State AS destLocationState, destLocation.ZipCode AS destLocationZipCode, ",
                        "destLocation.CompanyName AS destCompanyName ",

                        "FROM PO ",

                        "LEFT JOIN Locations as origLocation ON PO.OrigLocationID = origLocation.ID ",
                        "LEFT JOIN Locations as destLocation ON PO.DestLocationID = destLocation.ID ",
                        "WHERE PO.QuoteID=", QuoteID);
                    #endregion

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["ID"] != DBNull.Value)
                                {
                                    shipInfo.PO_ID = (int)reader["ID"];
                                }

                                //-----------------------------------------------------------------------------

                                if (reader["origLocationAddress1"] != DBNull.Value)
                                {
                                    shipInfo.spAddress1 = reader["origLocationAddress1"].ToString();
                                }
                                if (reader["origLocationAddress2"] != DBNull.Value)
                                {
                                    shipInfo.spAddress2 = reader["origLocationAddress2"].ToString();
                                }
                                if (reader["origLocationAddress3"] != DBNull.Value)
                                {
                                    shipInfo.spAddress3 = reader["origLocationAddress3"].ToString();
                                }

                                //--

                                if (reader["destLocationAddress1"] != DBNull.Value)
                                {
                                    shipInfo.ccAddress1 = reader["destLocationAddress1"].ToString();
                                }
                                if (reader["destLocationAddress2"] != DBNull.Value)
                                {
                                    shipInfo.ccAddress2 = reader["destLocationAddress2"].ToString();
                                }
                                if (reader["destLocationAddress3"] != DBNull.Value)
                                {
                                    shipInfo.ccAddress3 = reader["destLocationAddress3"].ToString();
                                }

                                //----------------------------------------------------------------------------


                                if (reader["origLocationCity"] != DBNull.Value)
                                {
                                    shipInfo.spCity = reader["origLocationCity"].ToString();
                                }
                                if (reader["origLocationState"] != DBNull.Value)
                                {
                                    shipInfo.spState = reader["origLocationState"].ToString();
                                }
                                if (reader["origLocationZipCode"] != DBNull.Value)
                                {
                                    shipInfo.spZip = reader["origLocationZipCode"].ToString();
                                }

                                if (reader["destLocationCity"] != DBNull.Value)
                                {
                                    shipInfo.ccCity = reader["destLocationCity"].ToString();
                                }
                                if (reader["destLocationState"] != DBNull.Value)
                                {
                                    shipInfo.ccState = reader["destLocationState"].ToString();
                                }
                                if (reader["destLocationZipCode"] != DBNull.Value)
                                {
                                    shipInfo.ccZip = reader["destLocationZipCode"].ToString();
                                }


                                //----------------------------------------------------------------------------

                                if (reader["origCompanyName"] != DBNull.Value)
                                {
                                    shipInfo.spName = reader["origCompanyName"].ToString();
                                }

                                //--

                                if (reader["destCompanyName"] != DBNull.Value)
                                {
                                    shipInfo.ccName = reader["destCompanyName"].ToString();
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("demo aafes", e.ToString());
            }
        }
        #endregion

        #region GetAAFES_CustomerPOs_QuoteInfo
        public static void GetAAFES_CustomerPOs_QuoteInfo(ref AAFES.AAFES_ShipInfo shipInfo)
        {
            try
            {
                shipInfo.customerPO = new List<string>();
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAAFES))
                {
                    #region SQL
                    string sql = string.Concat("SELECT RefNumber  ",

                        "FROM CustomerRefNumbers ",

                        "WHERE PO_ID=", shipInfo.PO_ID);
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

                                if (reader["RefNumber"] != DBNull.Value)
                                {
                                    shipInfo.customerPO.Add(reader["RefNumber"].ToString());
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("demo aafes", e.ToString());
            }
        }
        #endregion

        #region GetAAFES_ShipperPOs_QuoteInfo
        public static void GetAAFES_ShipperPOs_QuoteInfo(ref AAFES.AAFES_ShipInfo shipInfo)
        {
            try
            {
                shipInfo.shipperRefNumbers = new List<string>();
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAAFES))
                {
                    #region SQL
                    string sql = string.Concat("SELECT RefNumber  ",

                        "FROM ShipperRefNumbers ",

                        "WHERE PO_ID=", shipInfo.PO_ID);
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

                                if (reader["RefNumber"] != DBNull.Value)
                                {
                                    shipInfo.shipperRefNumbers.Add(reader["RefNumber"].ToString());
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("demo aafes", e.ToString());
            }
        }
        #endregion

        #region GetAAFES_Customer_QuoteInfo
        public static void GetAAFES_Customer_QuoteInfo(ref AAFES.AAFES_ShipInfo shipInfo)
        {
            try
            {

                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAAFES))
                {
                    #region SQL
                    string sql = string.Concat("SELECT ContactName, ContactPhone  ",

                        "FROM PO ",

                        "WHERE ID=", shipInfo.PO_ID);
                    #endregion

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                if (reader["ContactName"] != DBNull.Value)
                                {
                                    shipInfo.spContactName = reader["ContactName"].ToString();
                                }

                                if (reader["ContactPhone"] != DBNull.Value)
                                {
                                    shipInfo.spContactPhone = reader["ContactPhone"].ToString();
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("aafes", e.ToString());
            }
        }
        #endregion

        #region updateAAFES_PO_TableShipID
        public static void updateAAFES_PO_TableShipID(ref int shipID, ref int AAFES_QuoteID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAAFES))
                {
                    string sql = string.Concat("UPDATE PO SET GCMShipmentID=", shipID, " ",
                                               "WHERE QuoteID=", AAFES_QuoteID);

                    writeToAAFES_Logs("updateAAFES_PO_TableShipID", sql);

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                writeToAAFES_Logs("updateAAFES_PO_TableShipID", e.ToString());
            }
        }
        #endregion

        #region Not used, assignSFproToShipID
        //public static void assignSFproToShipID(ref int ShipmentID)
        //{

        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAAFES))
        //        {

        //            // This will pick the next available pro number (pro number that does not have a shipment id assigned to it yet), the MIN - next row number in line
        //            // Is selected, and newly created shipment id from the dispatch page goes in there.
        //            string sql = string.Concat("UPDATE SFProDemo SET GCMShipmentID=", ShipmentID, ", ",
        //                                       "DateAssigned='", DateTime.Today.ToShortDateString(), "'",
        //                                       " WHERE GCMShipmentID IS NULL AND SFProID=",
        //                                       "(SELECT MIN(SFProID) FROM SFProDemo WHERE GCMShipmentID is NULL)");

        //            using (SqlCommand command = new SqlCommand())
        //            {
        //                command.Connection = conn;
        //                command.CommandText = sql;
        //                conn.Open();
        //                command.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        SharedFunctions.writeToAAFES_Logs("AAFES", e.ToString());
        //    }
        //}
        #endregion

        #region GetCarrierPhoneByCarrierName
        public static void GetCarrierPhoneByCarrierName(ref string carrierPhone, string CarrierName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringLTLRater))
                {
                    #region SQL

                    string sql = string.Concat("SELECT Phone ",

                        "FROM SQL_LTLCARRIERS ",

                        "WHERE CarrierName='", CarrierName, "'");

                    #endregion

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["Phone"] != DBNull.Value)
                                {
                                    carrierPhone = reader["Phone"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("GetCarrierPhoneByCarrierName", e.ToString());
            }
        }
        #endregion

        #region GetCarrierPhoneByCarrierID
        public static void GetCarrierPhoneByCarrierID(ref string carrierPhone, int carrierID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringLTLRater))
                {
                    #region SQL

                    string sql = string.Concat("SELECT Phone ",

                        "FROM SQL_LTLCARRIERS ",

                        "WHERE ID=", carrierID);

                    #endregion

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["Phone"] != DBNull.Value)
                                {
                                    carrierPhone = reader["Phone"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("GetCarrierPhoneByCarrierID", e.ToString());
            }
        }
        #endregion

        #region GetAAFES_ShipmentInfoByCustPoNum
        public static void GetAAFES_ShipmentInfoByCustPoNum(ref AAFES.AAFES_ShipInfo shipInfo, ref string custPoNum)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAAFES))
                {
                    #region SQL

                    string sql = string.Concat("SELECT custRefNumbsTbl.PO_ID as poID, poTbl.ContactName AS ContactName, poTbl.ContactPhone AS ContactPhone, ",
                        "poTbl.ContactEmail AS ContactEmail, poTbl.QuoteID AS QuoteID, poTbl.AcceptOrderAES AS acceptOrderAES, poTbl.IndexOfCarrierInQuote ",
                        "AS indexOfCarrierInQuote, poTbl.GCMShipmentID AS shipID, ",
                        "statsGcmLtl.Class AS freightClass, statsGcmLtl.Weight AS weight, ",
                        "origLocation.Address1 AS origLocationAddress1, origLocation.Address2 AS origLocationAddress2, origLocation.Address3 AS origLocationAddress3, ",
                        "origLocation.City AS origLocationCity, origLocation.State AS origLocationState, origLocation.ZipCode AS origLocationZipCode, ",
                        "origLocation.CompanyName AS origCompanyName, origLocation.GCMCompID AS origGCMCompID, ",
                        "destLocation.Address1 AS destLocationAddress1, destLocation.Address2 AS destLocationAddress2, destLocation.Address3 AS destLocationAddress3, ",
                        "destLocation.City AS destLocationCity, destLocation.State AS destLocationState, destLocation.ZipCode AS destLocationZipCode, ",
                        "destLocation.CompanyName AS destCompanyName, destLocation.GCMCompID AS destGCMCompID, ",
                        "itemDetailsTbl.TotalWeight AS TotalWeight, itemDetailsTbl.TotalCube AS TotalCube, itemDetailsTbl.Pcs AS Pcs, ",
                        "itemDetailsTbl.ReqPuDateStart AS ReqPuDateStart, segmentsTbl.SegmentID AS SegmentID ",

                        "FROM CustomerRefNumbers AS custRefNumbsTbl ",
                        //
                        "LEFT JOIN PO as poTbl ON poTbl.ID = custRefNumbsTbl.PO_ID ",
                        "LEFT JOIN Locations as origLocation ON poTbl.OrigLocationID = origLocation.ID ",
                        "LEFT JOIN Locations as destLocation ON poTbl.DestLocationID = destLocation.ID ",
                        "LEFT JOIN [AESRater2009].[dbo].[SQL_STATS_GCM_LTL] AS statsGcmLtl ON poTbl.QuoteID = statsGcmLtl.ID ",
                        "LEFT JOIN ItemDetails AS itemDetailsTbl ON poTbl.ID = itemDetailsTbl.PO_ID ",
                        "LEFT JOIN [AESData].[dbo].[tbl_SEGMENTS] AS segmentsTbl ON segmentsTbl.ShipmentID = poTbl.GCMShipmentID ",
                        "WHERE custRefNumbsTbl.RefNumber='", custPoNum, "'");

                    #endregion

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["poID"] != DBNull.Value)
                                {
                                    shipInfo.PO_ID = (int)reader["poID"];
                                }

                                if (reader["QuoteID"] != DBNull.Value)
                                {
                                    shipInfo.QuoteID = (int)reader["QuoteID"];
                                }

                                if (reader["SegmentID"] != DBNull.Value)
                                {
                                    shipInfo.SegmentID = (int)reader["SegmentID"];
                                }

                                if (reader["shipID"] != DBNull.Value)
                                {
                                    shipInfo.wasBooked = true;
                                    shipInfo.ShipmentID = (int)reader["shipID"];

                                    //
                                    if (reader["poID"] != DBNull.Value)
                                    {
                                        shipInfo.wasBooked = isAAFES_ShipmentBooked(shipInfo.PO_ID, ref shipInfo.ShipmentID);
                                    }
                                    //
                                }
                                else
                                {
                                    shipInfo.wasBooked = false;
                                }

                                //--

                                if (reader["acceptOrderAES"] != DBNull.Value)
                                {
                                    shipInfo.AcceptOrderAES = (bool)reader["acceptOrderAES"];
                                }

                                if (reader["indexOfCarrierInQuote"] != DBNull.Value)
                                {
                                    shipInfo.IndexOfCarrierInQuote = (byte)reader["indexOfCarrierInQuote"];
                                }

                                //--

                                if (reader["origGCMCompID"] != DBNull.Value)
                                {
                                    shipInfo.spGCMCompID = (int)reader["origGCMCompID"];
                                }

                                if (reader["destGCMCompID"] != DBNull.Value)
                                {
                                    shipInfo.ccGCMCompID = (int)reader["destGCMCompID"];
                                }

                                if (reader["TotalWeight"] != DBNull.Value)
                                {
                                    shipInfo.totalWeight = (decimal)reader["TotalWeight"];
                                }

                                if (reader["freightClass"] != DBNull.Value)
                                {
                                    shipInfo.freightClass = (double)reader["freightClass"];
                                }

                                if (reader["TotalCube"] != DBNull.Value)
                                {
                                    shipInfo.totalCube = (decimal)reader["TotalCube"];
                                }

                                if (reader["Pcs"] != DBNull.Value)
                                {
                                    shipInfo.Pcs = (int)reader["Pcs"];
                                }

                                if (reader["ReqPuDateStart"] != DBNull.Value)
                                {
                                    shipInfo.requestedPuDateStart = (DateTime)reader["ReqPuDateStart"];
                                }

                                if (reader["ContactName"] != DBNull.Value)
                                {
                                    shipInfo.spContactName = reader["ContactName"].ToString();
                                }
                                if (reader["ContactPhone"] != DBNull.Value)
                                {
                                    shipInfo.spContactPhone = reader["ContactPhone"].ToString();
                                }
                                if (reader["ContactEmail"] != DBNull.Value)
                                {
                                    shipInfo.spContactEmail = reader["ContactEmail"].ToString();
                                }

                                //-----------------------------------------------------------------------------

                                if (reader["origLocationAddress1"] != DBNull.Value)
                                {
                                    shipInfo.spAddress1 = reader["origLocationAddress1"].ToString();
                                }
                                if (reader["origLocationAddress2"] != DBNull.Value)
                                {
                                    shipInfo.spAddress2 = reader["origLocationAddress2"].ToString();
                                }
                                if (reader["origLocationAddress3"] != DBNull.Value)
                                {
                                    shipInfo.spAddress3 = reader["origLocationAddress3"].ToString();
                                }

                                //--

                                if (reader["destLocationAddress1"] != DBNull.Value)
                                {
                                    shipInfo.ccAddress1 = reader["destLocationAddress1"].ToString();
                                }
                                if (reader["destLocationAddress2"] != DBNull.Value)
                                {
                                    shipInfo.ccAddress2 = reader["destLocationAddress2"].ToString();
                                }
                                if (reader["destLocationAddress3"] != DBNull.Value)
                                {
                                    shipInfo.ccAddress3 = reader["destLocationAddress3"].ToString();
                                }

                                //----------------------------------------------------------------------------


                                if (reader["origLocationCity"] != DBNull.Value)
                                {
                                    shipInfo.spCity = reader["origLocationCity"].ToString();
                                }
                                if (reader["origLocationState"] != DBNull.Value)
                                {
                                    shipInfo.spState = reader["origLocationState"].ToString();
                                }
                                if (reader["origLocationZipCode"] != DBNull.Value)
                                {
                                    shipInfo.spZip = reader["origLocationZipCode"].ToString();
                                }

                                if (reader["destLocationCity"] != DBNull.Value)
                                {
                                    shipInfo.ccCity = reader["destLocationCity"].ToString();
                                }
                                if (reader["destLocationState"] != DBNull.Value)
                                {
                                    shipInfo.ccState = reader["destLocationState"].ToString();
                                }
                                if (reader["destLocationZipCode"] != DBNull.Value)
                                {
                                    shipInfo.ccZip = reader["destLocationZipCode"].ToString();
                                }


                                //----------------------------------------------------------------------------

                                if (reader["origCompanyName"] != DBNull.Value)
                                {
                                    shipInfo.spName = reader["origCompanyName"].ToString();
                                }

                                //--

                                if (reader["destCompanyName"] != DBNull.Value)
                                {
                                    shipInfo.ccName = reader["destCompanyName"].ToString();
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("demo aafes", e.ToString());
            }
        }
        #endregion

        #region GetCarrierPhoneByCarrierKey
        public static void GetCarrierPhoneByCarrierKey(ref string carrierPhone, string carrierKey)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringLTLRater))
                {
                    #region SQL

                    string sql = string.Concat("SELECT Phone ",

                        "FROM SQL_LTLCARRIERS ",

                        "WHERE CarrierKey='", carrierKey, "' ");

                    #endregion

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["Phone"] != DBNull.Value)
                                {
                                    carrierPhone = reader["Phone"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("aafes", e.ToString());
            }
        }
        #endregion

        #region GetAAFES_QuoteAccessorialsByQuoteID
        public static void GetAAFES_QuoteAccessorialsByQuoteID(ref AAFES.LTL_Quote ltlQuote, ref int QuoteID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringRater2009))
                {
                    #region SQL

                    string sql = string.Concat("SELECT * ",

                        "FROM SQL_STATS_GCM_LTL_ACCESSORIALS ",

                        "WHERE ID=", QuoteID);

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
                                if (reader["AccessorialCode"] != DBNull.Value)
                                {
                                    ltlQuote.Accessorials.Add(reader["AccessorialCode"].ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("demo aafes", e.ToString());
            }
        }
        #endregion

        #region GetAAFES_SelectedQuoteInfoByQuoteID_
        public static void GetAAFES_SelectedQuoteInfoByQuoteID(ref bool found, ref AAFES.LTL_Quote ltlQuote, ref int QuoteID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringRater2009))
                {
                    #region SQL

                    string sql = string.Concat("SELECT * ",

                        "FROM SQL_STATS_GCM_LTL_QUOTES ",

                        "WHERE ID=", QuoteID, " ",

                        "AND IsBestQuoteAAFES=1 ",

                        "ORDER BY TotalPrice ASC");

                    #endregion

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                found = true;

                                if (reader["DeliveryDays"] != DBNull.Value)
                                {
                                    ltlQuote.DeliveryDays = (int)reader["DeliveryDays"];
                                }

                                if (reader["TotalPrice"] != DBNull.Value)
                                {
                                    ltlQuote.TotalPrice = Convert.ToDecimal((double)reader["TotalPrice"]);
                                }

                                if (reader["Liability"] != DBNull.Value)
                                {
                                    ltlQuote.Liability = Convert.ToDecimal((double)reader["Liability"]);
                                }

                                if (reader["OurRate"] != DBNull.Value)
                                {
                                    ltlQuote.OurRate = Convert.ToDecimal((double)reader["OurRate"]);
                                }

                                if (reader["CarrierDisplayName"] != DBNull.Value)
                                {
                                    ltlQuote.CarrierDisplayName = reader["CarrierDisplayName"].ToString();
                                }

                                if (reader["CarrierKey"] != DBNull.Value)
                                {
                                    ltlQuote.CarrierKey = reader["CarrierKey"].ToString();
                                }

                                if (reader["BookingKey"] != DBNull.Value)
                                {
                                    ltlQuote.BookingKey = reader["BookingKey"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("demo aafes", e.ToString());
            }
        }
        #endregion

        #region GetAAFES_QuoteInfoByQuoteID_AndChooseCarrier
        public static void GetAAFES_QuoteInfoByQuoteID_AndChooseCarrier(ref AAFES.LTL_Quote ltlQuote, ref int QuoteID)
        {
            decimal gcmLowestRate = decimal.MaxValue, dhlLowestRate = decimal.MaxValue, standardForwardingRate = decimal.MaxValue;
            List<LTL_Quote> ltlQuotesList = new List<LTL_Quote>();
            //LTL_Quote myQuote = new LTL_Quote();
            byte indexOfGCM_LowestRate = 0, indexOfOtherDHL_LowestRate = 0;
            byte counter = 0, lowestRatesCounter = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringRater2009))
                {
                    #region SQL

                    string sql = string.Concat("SELECT * ",

                        "FROM SQL_STATS_GCM_LTL_QUOTES ",

                        "WHERE ID=", QuoteID, " ",

                        "ORDER BY OurRate ASC");

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
                                #region Build quotes list

                                LTL_Quote myQuote = new LTL_Quote();

                                if (reader["DeliveryDays"] != DBNull.Value)
                                {
                                    myQuote.DeliveryDays = (int)reader["DeliveryDays"];
                                }

                                if (reader["TotalPrice"] != DBNull.Value)
                                {
                                    myQuote.TotalPrice = Convert.ToDecimal((double)reader["TotalPrice"]);
                                }

                                if (reader["Liability"] != DBNull.Value)
                                {
                                    myQuote.Liability = Convert.ToDecimal((double)reader["Liability"]);
                                }

                                //if (reader["OurRate"] != DBNull.Value)
                                //{
                                //    myQuote.OurRate = Convert.ToDecimal((double)reader["OurRate"]);
                                //}

                                if (reader["CarrierDisplayName"] != DBNull.Value && reader["OurRate"] != DBNull.Value)
                                {
                                    myQuote.CarrierDisplayName = reader["CarrierDisplayName"].ToString();
                                    myQuote.OurRate = Convert.ToDecimal((double)reader["OurRate"]);


                                    if (lowestRatesCounter < 3)
                                    {
                                        if (!myQuote.CarrierDisplayName.Contains("DHL") && gcmLowestRate.Equals(decimal.MaxValue))
                                        {
                                            gcmLowestRate = myQuote.OurRate;
                                            indexOfGCM_LowestRate = counter;
                                            lowestRatesCounter++;
                                        }
                                        else if (!myQuote.CarrierDisplayName.Contains("FreightCenter") && gcmLowestRate.Equals(decimal.MaxValue))
                                        {
                                            gcmLowestRate = myQuote.OurRate;
                                            indexOfGCM_LowestRate = counter;
                                            lowestRatesCounter++;
                                        }
                                        else if (myQuote.CarrierDisplayName.Contains("DHL") &&
                                            !myQuote.CarrierDisplayName.Contains("Standard Forwarding – DHL standard") &&
                                            !myQuote.CarrierDisplayName.Contains("ABF") &&
                                            dhlLowestRate.Equals(decimal.MaxValue))
                                        {
                                            dhlLowestRate = myQuote.OurRate;
                                            indexOfOtherDHL_LowestRate = counter;
                                            lowestRatesCounter++;
                                        }
                                        else if (myQuote.CarrierDisplayName.Contains("FreightCenter") &&
                                            !myQuote.CarrierDisplayName.Contains("STDF") &&
                                            !myQuote.CarrierDisplayName.Contains("ABF") &&
                                            dhlLowestRate.Equals(decimal.MaxValue))
                                        {
                                            dhlLowestRate = myQuote.OurRate;
                                            indexOfOtherDHL_LowestRate = counter;
                                            lowestRatesCounter++;
                                        }
                                        else if (myQuote.CarrierDisplayName.Contains("DHL") && myQuote.CarrierDisplayName.Contains("Standard Forwarding – DHL standard") &&
                                            standardForwardingRate.Equals(decimal.MaxValue))
                                        {
                                            standardForwardingRate = myQuote.OurRate;
                                            lowestRatesCounter++;
                                        }
                                        else if (myQuote.CarrierDisplayName.Contains("FreightCenter") && myQuote.CarrierDisplayName.Contains("STDF") &&
                                            standardForwardingRate.Equals(decimal.MaxValue))
                                        {
                                            standardForwardingRate = myQuote.OurRate;
                                            lowestRatesCounter++;
                                        }
                                    }

                                }

                                if (reader["CarrierKey"] != DBNull.Value)
                                {
                                    myQuote.CarrierKey = reader["CarrierKey"].ToString();
                                }

                                if (reader["BookingKey"] != DBNull.Value)
                                {
                                    myQuote.BookingKey = reader["BookingKey"].ToString();
                                }

                                ltlQuotesList.Add(myQuote);


                                counter++;
                                #endregion
                            }
                        }
                    }
                }

                if (!standardForwardingRate.Equals(decimal.MaxValue) && !gcmLowestRate.Equals(decimal.MaxValue))
                {
                    if ((gcmLowestRate + 25) <= standardForwardingRate)
                    {
                        ltlQuote = ltlQuotesList[indexOfGCM_LowestRate];
                        AAFES.writeToAAFES_Logs("comparisons", " AES accepting quote id was: " + QuoteID.ToString());
                    }
                    //else if (!dhlLowestRate.Equals(decimal.MaxValue))
                    //{
                    //    ltlQuote = ltlQuotesList[indexOfOtherDHL_LowestRate];
                    //    AAFES.writeToAAFES_Logs("comparisons", " FreightCenter accepting quote id was: " + QuoteID.ToString());
                    //}
                }
                //else if (!standardForwardingRate.Equals(decimal.MaxValue) && !dhlLowestRate.Equals(decimal.MaxValue))
                //{
                //    ltlQuote = ltlQuotesList[indexOfOtherDHL_LowestRate];
                //    AAFES.writeToAAFES_Logs("comparisons", " FreightCenter accepting quote id was: " + QuoteID.ToString());
                //}
                else
                {
                    AAFES.writeToAAFES_Logs("comparisons issue", "quote id was: " + QuoteID.ToString());
                    ltlQuote = ltlQuotesList[0]; // Fall back on default
                }

                AAFES.writeToAAFES_Logs("comparisons", "accepting quote id was: " + QuoteID.ToString() + " accepted carrier was: " +
                    ltlQuote.CarrierDisplayName + " accepted our rate was: " + ltlQuote.OurRate.ToString());

            }
            catch (Exception e)
            {
                AAFES.writeToAAFES_Logs("comparisons issue", e.ToString());
            }
        }
        #endregion

        #region GetAAFES_QuoteInfoByQuoteID
        public static void GetAAFES_QuoteInfoByQuoteID(ref AAFES.LTL_Quote ltlQuote, ref int QuoteID, ref byte AAFES_IndexOfCarrierInQuote)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringRater2009))
                {
                    #region SQL

                    string sql = string.Concat("SELECT * ",

                        "FROM SQL_STATS_GCM_LTL_QUOTES ",

                        "WHERE ID=", QuoteID, " ",

                        "ORDER BY TotalPrice ASC");

                    #endregion

                    byte counter = 0;

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (counter.Equals(AAFES_IndexOfCarrierInQuote))
                                {
                                    if (reader["DeliveryDays"] != DBNull.Value)
                                    {
                                        ltlQuote.DeliveryDays = (int)reader["DeliveryDays"];
                                    }

                                    if (reader["TotalPrice"] != DBNull.Value)
                                    {
                                        ltlQuote.TotalPrice = Convert.ToDecimal((double)reader["TotalPrice"]);
                                    }

                                    if (reader["Liability"] != DBNull.Value)
                                    {
                                        ltlQuote.Liability = Convert.ToDecimal((double)reader["Liability"]);
                                    }

                                    if (reader["OurRate"] != DBNull.Value)
                                    {
                                        ltlQuote.OurRate = Convert.ToDecimal((double)reader["OurRate"]);
                                    }

                                    if (reader["CarrierDisplayName"] != DBNull.Value)
                                    {
                                        ltlQuote.CarrierDisplayName = reader["CarrierDisplayName"].ToString();
                                    }

                                    if (reader["CarrierKey"] != DBNull.Value)
                                    {
                                        ltlQuote.CarrierKey = reader["CarrierKey"].ToString();
                                    }

                                    if (reader["BookingKey"] != DBNull.Value)
                                    {
                                        ltlQuote.BookingKey = reader["BookingKey"].ToString();
                                    }

                                    break; // Done
                                }
                                counter++; // Increment counter
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("demo aafes", e.ToString());
            }
        }
        #endregion

        #region GetAAFES_FullQuoteInfoByQuoteID
        public static void GetAAFES_FullQuoteInfoByQuoteID(ref List<AAFES.LTL_Quote> ltlQuotes, ref int QuoteID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringRater2009))
                {
                    #region SQL

                    string sql = string.Concat("SELECT TotalPrice, OurRate, CarrierDisplayName ",

                        "FROM SQL_STATS_GCM_LTL_QUOTES ",

                        "WHERE ID=", QuoteID, " ",

                        "ORDER BY TotalPrice ASC");

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
                                LTL_Quote ltlQuote = new LTL_Quote();

                                if (reader["TotalPrice"] != DBNull.Value)
                                {
                                    ltlQuote.TotalPrice = Convert.ToDecimal((double)reader["TotalPrice"]);
                                }

                                if (reader["OurRate"] != DBNull.Value)
                                {
                                    ltlQuote.OurRate = Convert.ToDecimal((double)reader["OurRate"]);
                                }

                                if (reader["CarrierDisplayName"] != DBNull.Value)
                                {
                                    ltlQuote.CarrierDisplayName = reader["CarrierDisplayName"].ToString();
                                }

                                ltlQuotes.Add(ltlQuote);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("demo aafes", e.ToString());
            }
        }
        #endregion

        #region GetAAFES_FullQuoteInfoByQuoteID_2
        public static void GetAAFES_FullQuoteInfoByQuoteID_2(ref int QuoteID, ref string origZipOfQuote,
            ref string destZipOfQuote, ref DateTime DateGathered, ref int weightOfQuote, ref double classOfQuote, ref string refNumberOfQuote)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringRater2009))
                {
                    #region SQL

                    string sql = string.Concat("SELECT Weight, Class, Origin, Destination, Day, RefNumber ",

                        "FROM SQL_STATS_GCM AS statsGCM_Tbl ",

                        "LEFT JOIN SQL_STATS_GCM_LTL AS statsGCM_LTL_Tbl ON statsGCM_LTL_Tbl.ID = statsGCM_Tbl.ID ",

                        "LEFT JOIN [AAFES].[dbo].[PO] AS poTbl ON poTbl.QuoteID = statsGCM_Tbl.ID ",

                        "LEFT JOIN [AAFES].[dbo].[CustomerRefNumbers] AS refTbl ON refTbl.PO_ID = poTbl.ID ",

                        "WHERE statsGCM_Tbl.ID=", QuoteID);

                    #endregion

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                if (reader["Weight"] != DBNull.Value)
                                {
                                    weightOfQuote = (int)reader["Weight"];
                                }

                                if (reader["Class"] != DBNull.Value)
                                {
                                    classOfQuote = (double)reader["Class"];
                                }

                                //

                                if (reader["Origin"] != DBNull.Value)
                                {
                                    origZipOfQuote = reader["Origin"].ToString();
                                }

                                if (reader["Destination"] != DBNull.Value)
                                {
                                    destZipOfQuote = reader["Destination"].ToString();
                                }

                                if (reader["RefNumber"] != DBNull.Value)
                                {
                                    refNumberOfQuote = reader["RefNumber"].ToString();
                                }

                                if (reader["Day"] != DBNull.Value)
                                {
                                    DateTime.TryParse(reader["Day"].ToString(), out DateGathered);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("demo aafes", e.ToString());
            }
        }
        #endregion

        #region GetAAFES_QuoteInfoForAES_Email
        public static void GetAAFES_QuoteInfoForAES_Email(ref List<AAFES.LTL_Quote> ltlQuotes, ref StringBuilder emailText,
            ref int AAFES_QuoteID, ref int intShipmentID, ref string origZipOfQuote,
            ref string destZipOfQuote, ref DateTime DateGathered, ref int weightOfQuote, ref double classOfQuote, ref string refNumberOfQuote)
        {

            string tdStyle = "";

            try
            {
                emailText.Append(string.Concat("<table style='border:1px solid black; border-collapse:collapse; margin-top:20px; margin-bottom:20px;'>",
                 "<tr>",

                 //"<th style='border:1px solid black; color: Black; background-color: CCCCFF;'>Accept Order</th>",
                 "<th style='border:1px solid black; color: Black; background-color: CCCCFF;'>RefNumber</th>",
                 "<th style='border:1px solid black; color: Black; background-color: CCCCFF;'>GCMShipmentID</th>",
                 "<th style='border:1px solid black; color: Black; background-color: CCCCFF;'>DateGathered</th>",
                 "<th style='border:1px solid black; color: Black; background-color: CCCCFF;'>Quote ID</th>",

                 "<th style='border:1px solid black; color: Black; background-color: CCCCFF;'>Origin</th>",
                 "<th style='border:1px solid black; color: Black; background-color: CCCCFF;'>Destination</th>",

                 "<th style='border:1px solid black; color: Black; background-color: CCCCFF;'>Weight</th>",
                 "<th style='border:1px solid black; color: Black; background-color: CCCCFF;'>Class</th>",


                 "</tr>"));

                tdStyle = string.Concat("style='border:1px solid black; background-color:", "rgb(255,255,255); ");

                emailText.Append(string.Concat("<tr>",

                    //"<td ", tdStyle + "';>", ltlQuotes[i].CarrierDisplayName, "</td>",

                    "<td ", tdStyle, "padding-left: 20px;'>", refNumberOfQuote, "</td>",

                    "<td ", tdStyle, "padding: 20px;'>", intShipmentID, "</td>",

                    "<td ", tdStyle, "';>", DateGathered.ToShortDateString(), "</td>",

                    "<td ", tdStyle, "padding-left: 20px;'>", AAFES_QuoteID, "</td>",

                    "<td ", tdStyle, "padding: 20px;'>", origZipOfQuote, "</td>",

                    "<td ", tdStyle, "';>", destZipOfQuote, "</td>",

                    "<td ", tdStyle, "padding-left: 20px;'>", weightOfQuote, "</td>",

                    "<td ", tdStyle, "padding: 20px;'>", classOfQuote, "</td>",

                    "</tr>"));

                emailText.Append("</table>");


                #region Append table with info from sql_stats_ltl_quotes

                emailText.Append(string.Concat("<table style='border:1px solid black; border-collapse:collapse; margin-top:20px; margin-bottom:20px;'>",
                "<tr>",

                "<th style='border:1px solid black; color: Black; background-color: CCCCFF;'>Carrier Display Name</th>",
                "<th style='border:1px solid black; color: Black; background-color: CCCCFF;'>Total Price</th>",
                "<th style='border:1px solid black; color: Black; background-color: CCCCFF;'>Our Rate</th>",

                "</tr>"));

                for (byte i = 0; i < ltlQuotes.Count; i++)
                {

                    tdStyle = string.Concat("style='border:1px solid black; background-color:", "rgb(255,255,255); ");

                    emailText.Append(string.Concat("<tr>",

                        "<td ", tdStyle + "';>", ltlQuotes[i].CarrierDisplayName,
                        "</td><td ", tdStyle, "padding-left: 20px;'>", ltlQuotes[i].TotalPrice, "</td>",

                        "<td ", tdStyle, "padding: 20px;'>", ltlQuotes[i].OurRate, "</td>",

                        "</tr>"));
                }

                emailText.Append("</table>");

                #endregion

            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("demo aafes", e.ToString());
            }
        }
        #endregion

        #region GetRRTS_AAFES_ClassByQuoteID
        public static void GetRRTS_AAFES_ClassByQuoteID(ref int QuoteID, ref double ClassRRTS_AAFES)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringRater2009))
                {
                    #region SQL

                    string sql = string.Concat("SELECT ClassRRTS_AAFES ",

                        "FROM SQL_STATS_GCM_LTL_QUOTES_LTL ",

                        "WHERE ID=", QuoteID);

                    #endregion

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                if (reader["OurRate"] != DBNull.Value)
                                {
                                    ClassRRTS_AAFES = (double)reader["ClassRRTS_AAFES"];
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("GetRRTS_AAFES_ClassByQuoteID", e.ToString());
            }
        }
        #endregion

        #region insertIntoItems_AAFES
        public static void insertIntoItems_AAFES(ref List<AAFES.AAFES_ItemsInfo> aafesItemsInfoList, ref int ShipmentID, ref double ClassRRTS_AAFES,
            ref bool isCarrierRRTS_AAFES)
        {
            try
            {
                try
                {
                    string sql = "", cubeText = "";
                    using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
                    {
                        conn.Open();
                        for (byte i = 0; i < aafesItemsInfoList.Count; i++)
                        {
                            if (aafesItemsInfoList[i].Cube > 0)
                            {
                                cubeText = string.Concat(aafesItemsInfoList[i].Cube.ToString(), " Cube");
                            }
                            else
                            {
                                cubeText = "";
                            }

                            if (isCarrierRRTS_AAFES)
                            {
                                sql = string.Concat("INSERT INTO tbl_ITEMS(ShipmentID,Class,Descr,Kind,Nmfc,Pcs,Units,WtLBS,VolCF)",
                                                           " VALUES(", ShipmentID, ",", Math.Round(ClassRRTS_AAFES, 1), ",'",
                                aafesItemsInfoList[i].commodity, " ", cubeText, "',",
                                "'PALLETS',",
                                Math.Round(aafesItemsInfoList[i].NMFC, 2), ",",
                                aafesItemsInfoList[i].Pcs, ",",
                                aafesItemsInfoList[i].Pallets, ",",
                                aafesItemsInfoList[i].Weight, ",",
                                aafesItemsInfoList[i].Cube,

                                ")");
                            }
                            else
                            {
                                sql = string.Concat("INSERT INTO tbl_ITEMS(ShipmentID,Class,Descr,Kind,Nmfc,Pcs,Units,WtLBS,VolCF)",
                                                           " VALUES(", ShipmentID, ",", Math.Round(aafesItemsInfoList[i].FreightClass, 1), ",'",
                                aafesItemsInfoList[i].commodity, " ", cubeText, "',",
                                "'PALLETS',",
                                Math.Round(aafesItemsInfoList[i].NMFC, 2), ",",
                                aafesItemsInfoList[i].Pcs, ",",
                                aafesItemsInfoList[i].Pallets, ",",
                                aafesItemsInfoList[i].Weight, ",",
                                aafesItemsInfoList[i].Cube,

                                ")");
                            }

                            using (SqlCommand command = new SqlCommand())
                            {
                                command.Connection = conn;
                                command.CommandText = sql;

                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception e1)
                {
                    writeToAAFES_Logs("AAFES", e1.ToString());
                }
            }
            catch (Exception e)
            {
                writeToAAFES_Logs("AAFES", e.ToString());
            }
        }
        #endregion

        #region assignSFproToShipID and insert pro into AESData.PO

        #region assignSFproToShipID
        /// <summary>
        /// Dispatch page will call this function with a newly created shipment id
        /// </summary>
        /// <param name="ShipmentID"></param>
        public static void assignSFproToShipID(ref int ShipmentID)
        {

            try
            {
                string pro = "";
                int SFproID = -1;
                // GetAAFES_NextAvailableSFpro
                // Finds next available pro number in SFpro table and returns - pro number and SFproID
                GetAAFES_NextAvailableSFpro(ref pro, ref SFproID);

                // Test
                writeToSiteErrors("GetAAFES_NextAvailableSFpro", "shipmentID was: " + ShipmentID + " pro was: " + pro + " SFproID: " + SFproID.ToString());

                // This will pick the next available pro number (pro number that does not have a shipment id assigned to it yet), the MIN - next row number in line
                // Is selected, and newly created shipment id from the dispatch page goes in there.
                string sql = string.Concat("UPDATE SFPro SET GCMShipmentID=", ShipmentID, ", ",
                                           "DateAssigned='", DateTime.Today.ToShortDateString(), "'",
                                           " WHERE GCMShipmentID IS NULL AND SFProID=",
                                           "(SELECT MIN(SFProID) FROM SFPro WHERE GCMShipmentID is NULL)");

                // If next available pro is found, go to next step and update the ShipmentID for this row from NULL to the shipment ID from the dispatch page
                if (!string.IsNullOrEmpty(pro) && SFproID > 0)
                {
                    try
                    {
                        int updateResult;

                        using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAAFES))
                        {
                            using (SqlCommand command = new SqlCommand())
                            {
                                command.Connection = conn;
                                command.CommandText = sql;
                                conn.Open();
                                updateResult = command.ExecuteNonQuery();
                            }
                        }

                        if (updateResult.Equals(1))
                        {
                            // Insert SFpro to AESData.PO table
                            insertSFproInToAESData_PO_Table(ref pro, ref ShipmentID);
                        }
                        else
                        {
                            throw new Exception("update SFPro.GCMShipmentID failed, sql was: " + sql);
                        }
                    }
                    catch (Exception e1)
                    {
                        writeToAAFES_Logs("AAFES", e1.ToString());
                    }
                }
                else
                {
                    throw new Exception(string.Concat("invalid pro or SFPro.ID, sql was: ", sql, " pro was:", pro, " SFproID was: " + SFproID));
                }

            }
            catch (Exception e)
            {
                writeToAAFES_Logs("AAFES", e.ToString());
            }
        }
        #endregion

        #region insertSFproInToAESData_PO_Table
        public static void insertSFproInToAESData_PO_Table(ref string po, ref int ShipmentID)
        {
            try
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
                    {
                        string sql = string.Concat("INSERT INTO tbl_PO(ShipmentID,PONumber)",
                                                   " VALUES(", ShipmentID, ",", po, ")");

                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = conn;
                            command.CommandText = sql;
                            conn.Open();
                            command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception e1)
                {
                    writeToAAFES_Logs("AAFES", e1.ToString());
                }
            }
            catch (Exception e)
            {
                writeToAAFES_Logs("AAFES", e.ToString());
            }
        }
        #endregion

        #region GetAAFES_NextAvailableSFpro
        public static void GetAAFES_NextAvailableSFpro(ref string pro, ref int ID)
        {
            try
            {

                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAAFES))
                {
                    #region SQL

                    string sql = string.Concat("SELECT SFProID, ProNumber ",
                                               "FROM SFPro ",
                                               "WHERE GCMShipmentID IS NULL AND SFProID=",
                                               "(SELECT MIN(SFProID) FROM SFPro WHERE GCMShipmentID is NULL)");

                    #endregion

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                if (reader["SFProID"] != DBNull.Value)
                                {
                                    ID = (int)reader["SFProID"];
                                }

                                if (reader["ProNumber"] != DBNull.Value)
                                {
                                    pro = reader["ProNumber"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AAFES.writeToSiteErrors("demo aafes", e.ToString());
            }
        }
        #endregion

        #endregion

        #region AAFES Structs
        public struct AAFES_ShipInfo
        {

            public string fCharges, spZip, ccZip;
            public string spContactName, spContactPhone, spContactEmail, spName, spAddress1, spCity, spState, spCountry, ccName, ccAddress1, ccAddress3, ccCity,
                ccState, ccCountry, spAddress2, spAddress3, ccAddress2;

            public DateTime requestedDelDateStart, requestedDelDateEnd, requestedPuDateStart, requestedPuDateEnd, requestedDelTimeStart, requestedDelTimeEnd, requestedPuTimeStart, requestedPuTimeEnd;
            public List<string> customerPO, shipperRefNumbers;

            public decimal totalCube, totalWeight;
            public bool accept, hazMat, AcceptOrderAES, wasBooked;
            public string status;
            public int Pcs, PO_ID, QuoteID, ShipmentID, SegmentID, spGCMCompID, ccGCMCompID, weight;
            public byte IndexOfCarrierInQuote;
            public double freightClass;

        }

        public struct AAFES_ConvertedShipInfo
        {

            //public bool accept;

            //public int PO_ID;

            // Calculated values
            public decimal freightClass, totalWeight, totalDensity, length, width, height;
            public decimal ctiiFreightClass;
            public int pallets;

            public AAFES_ShipInfo shipInfo;
        }

        public struct AAFES_ItemsInfo
        {

            public string commodity;

            public double Length, Width, Height, NMFC, FreightClass, Cube, Weight;
            public int Pcs, Pallets;

        }

        public struct LTL_Quote
        {
            public string CarrierDisplayName, CarrierKey, BookingKey;
            public decimal TotalPrice, Liability, OurRate;
            public int DeliveryDays;
            public List<string> Accessorials;
        }

        #endregion

        #endregion

        public static void writeToSiteErrors(string carrier, string exception) //overloaded, this one writes to LTL Rater database
        {
            SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aes_daylightSS"].ConnectionString);
            SqlCommand command = new SqlCommand();
            string sql, location = "LTLRater.aspx.cs";
            try
            {
                conn.Open();

                sql = string.Concat("INSERT INTO AF_ERRORS(Exception, Page, Location, ErrorDate, ErrorTime) ",
                    "VALUES('", exception.Replace("'", ""), "', '", location, "', '", carrier.Replace("'", ""), "', '",
                    DateTime.Today.ToShortDateString(), "', '", DateTime.Now.ToShortTimeString(), "')");

                command.Connection = conn;
                command.CommandText = sql;
                command.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
                command.Dispose();
            }
            catch (Exception e)
            {
                try
                {
                    writeToSiteErrors("writeToSiteErrors", e.ToString());
                    conn.Close();
                    conn.Dispose();
                    command.Dispose();
                }
                catch
                {
                }
            }
        }


        public static void writeToAAFES_Logs(string carrier, string exception)
        {
            SqlConnection conn = new SqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["aes_daylightSS"].ConnectionString);
            SqlCommand command = new SqlCommand();
            string sql, location = "LTLRater.aspx.cs";
            try
            {
                conn.Open();

                sql = "INSERT INTO AAFES_Logs(Exception, Page, Location, ErrorDate, ErrorTime) " +
                    "VALUES('" + exception.Replace("'", "") + "', '" + location + "', '" + carrier.Replace("'", "") + "', '" +
                    DateTime.Today.ToShortDateString() + "', '" + DateTime.Now.ToShortTimeString() + "')";

                command.Connection = conn;
                command.CommandText = sql;
                command.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
                command.Dispose();
            }
            catch (Exception e)
            {
                string str = e.ToString();
                try
                {
                    conn.Close();
                    conn.Dispose();
                    command.Dispose();
                }
                catch
                {
                }
                //writeToSiteErrors("EmailPastDueInvoices", e.ToString(), "");
            }
        }

    }
}