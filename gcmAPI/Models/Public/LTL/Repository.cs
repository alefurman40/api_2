#region Using

using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

#endregion

namespace gcmAPI.Models.Public.LTL
{
    public class Repository
    {
        string iam = "gcmAPI.Models.Public.LTL.Repository";

        #region Get_booking_info_by_booking_key

        public void Get_booking_info_by_booking_key(string booking_key, out AES_API_info api_info)
        {
            api_info = new AES_API_info();
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesAPI))
                {
                    #region SQL

                    string sql = string.Concat("SELECT BusinessDays, CarrierDisplayName, Rate, RequestId, CarrierKey, BuyRate, QuoteId,SCAC,CarrierQuoteID ",

                        "FROM LTL_RATE_RESULTS ",

                        "WHERE BookingKey='", booking_key, "'");

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
                                if (reader["BusinessDays"] != DBNull.Value)
                                {
                                    api_info.BusinessDays = reader["BusinessDays"].ToString();
                                }
                                if (reader["CarrierDisplayName"] != DBNull.Value)
                                {
                                    api_info.CarrierDisplayName = reader["CarrierDisplayName"].ToString();
                                }
                                if (reader["Rate"] != DBNull.Value)
                                {
                                    api_info.Rate = string.Format("{0:0.00}", (double)reader["Rate"]); 
                                }
                                if (reader["RequestId"] != DBNull.Value)
                                {
                                    api_info.RequestId = reader["RequestId"].ToString();
                                }
                                if (reader["CarrierKey"] != DBNull.Value)
                                {
                                    api_info.CarrierKey = reader["CarrierKey"].ToString();
                                }
                                if (reader["BuyRate"] != DBNull.Value)
                                {
                                    api_info.BuyRate = string.Format("{0:0.00}", (double)reader["BuyRate"]);
                                }
                                if (reader["QuoteId"] != DBNull.Value)
                                {
                                    api_info.QuoteId = reader["QuoteId"].ToString();
                                }
                                if (reader["SCAC"] != DBNull.Value)
                                {
                                    api_info.SCAC = reader["SCAC"].ToString();
                                }
                                if (reader["CarrierQuoteID"] != DBNull.Value)
                                {
                                    api_info.CarrierQuoteID = reader["CarrierQuoteID"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DB.LogGenera(iam,"Get_booking_info_by_booking_key", e.ToString());
            }

            //return api_info;
        }

        #endregion

        #region Get_carriers_by_QuoteID

        public void Get_carriers_by_QuoteID(string QuoteID, out List<AES_API_info> quote_carriers)
        {
            quote_carriers = new List<AES_API_info>();
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesAPI))
                {
                    #region SQL

                    string sql = string.Concat("SELECT BusinessDays, CarrierDisplayName, Rate, RequestId, CarrierKey, BuyRate, QuoteId,SCAC,CarrierQuoteID ",

                        "FROM LTL_RATE_RESULTS ",

                        "WHERE QuoteId=", QuoteID, " ",
                        "ORDER BY Rate");

                    #endregion

                    DB.LogGenera("Get_carriers_by_QuoteID", "sql", sql);

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AES_API_info api_info = new AES_API_info();


                                if (reader["BusinessDays"] != DBNull.Value)
                                {
                                    api_info.BusinessDays = reader["BusinessDays"].ToString();
                                }
                                if (reader["CarrierDisplayName"] != DBNull.Value)
                                {
                                    api_info.CarrierDisplayName = reader["CarrierDisplayName"].ToString();
                                }
                                if (reader["Rate"] != DBNull.Value)
                                {
                                    api_info.Rate = string.Format("{0:0.00}", (double)reader["Rate"]);
                                }
                                if (reader["RequestId"] != DBNull.Value)
                                {
                                    api_info.RequestId = reader["RequestId"].ToString();
                                }
                                if (reader["CarrierKey"] != DBNull.Value)
                                {
                                    api_info.CarrierKey = reader["CarrierKey"].ToString();
                                }
                                if (reader["BuyRate"] != DBNull.Value)
                                {
                                    api_info.BuyRate = string.Format("{0:0.00}", (double)reader["BuyRate"]);
                                }
                                if (reader["QuoteId"] != DBNull.Value)
                                {
                                    api_info.QuoteId = reader["QuoteId"].ToString();
                                }
                                if (reader["SCAC"] != DBNull.Value)
                                {
                                    api_info.SCAC = reader["SCAC"].ToString();
                                }
                                if (reader["CarrierQuoteID"] != DBNull.Value)
                                {
                                    api_info.CarrierQuoteID = reader["CarrierQuoteID"].ToString();
                                }

                                quote_carriers.Add(api_info);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DB.LogGenera(iam, "Get_booking_info_by_booking_key", e.ToString());
            }

            //return api_info;
        }

        #endregion

        #region Get_items_by_QuoteID

        public void Get_items_by_QuoteID(string QuoteID, ref AES_API_info api_info)
        {
            List<LTLPiece> pieces = new List<LTLPiece>();
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringRater2009))
                {
                    #region SQL

                    string sql = string.Concat("SELECT Class,Weight,Units ",

                        "FROM SQL_STATS_GCM_LTL ",

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
                                LTLPiece piece = new LTLPiece();
                                if (reader["Class"] != DBNull.Value)
                                {
                                    piece.FreightClass = reader["Class"].ToString();
                                    
                                    DB.LogGenera(iam, "Get_items_by_QuoteID piece.FreightClass", piece.FreightClass);
                                }
                                if (reader["Weight"] != DBNull.Value)
                                {
                                    piece.Weight = (int)reader["Weight"];
                                    api_info.total_weight += (int)piece.Weight;

                                    DB.LogGenera(iam, "Get_items_by_QuoteID piece.Weight", piece.Weight.ToString());
                                }
                                if (reader["Units"] != DBNull.Value)
                                {
                                    piece.Units = (int)reader["Units"];
                                    api_info.total_units += (int)piece.Units;

                                    DB.LogGenera(iam, "Get_items_by_QuoteID piece.Units", piece.Units.ToString());
                                }
                                pieces.Add(piece);
                            }
                        }
                    }
                }

                api_info.m_lPiece = pieces.ToArray();
            }
            catch (Exception e)
            {
                DB.LogGenera(iam,"Get_items_by_QuoteID", e.ToString());
            }

            //return api_info;
        }

        #endregion

        #region Get_accessorials_by_RequestID

        struct Acc
        {
            public string ServiceCode, Type;
        }

        public void Get_accessorials_by_RequestID(ref HelperFuncs.AccessorialsObj AccessorialsObj, ref AES_API_info api_info)
        {
            List<Acc> list = new List<Acc>();
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesAPI))
                {
                    #region SQL

                    string sql = string.Concat("SELECT ServiceCode,Type ",

                        "FROM LTL_RATE_REQUESTS_SERVICE ",

                        "WHERE RequestId='", api_info.RequestId, "'");

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
                                DB.LogGenera("Get_accessorials_by_RequestID", "found accessorial", "found accessorial");
                                Acc a = new Acc();

                                if (reader["ServiceCode"] != DBNull.Value)
                                {
                                    a.ServiceCode = reader["ServiceCode"].ToString();

                                    DB.LogGenera(iam, "Get_accessorials_by_RequestID ServiceCode", a.ServiceCode);
                                }

                                if (reader["Type"] != DBNull.Value)
                                {
                                    a.Type = reader["Type"].ToString();

                                    DB.LogGenera(iam, "Get_accessorials_by_RequestID Type", a.Type);
                                }

                                list.Add(a);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DB.LogGenera(iam, "Get_accessorials_by_RequestID", e.ToString());
            }

            //

            for(byte i=0;i<list.Count;i++)
            {
                if(list[i].ServiceCode == "RSD")
                {
                    AccessorialsObj.RESDEL = true;
                }
                else if (list[i].ServiceCode == "RSP")
                {
                    AccessorialsObj.RESPU = true;
                }
                else if (list[i].ServiceCode == "CSD")
                {
                    AccessorialsObj.CONDEL = true;
                }
                else if (list[i].ServiceCode == "CSP")
                {
                    AccessorialsObj.CONPU = true;
                }
                else if (list[i].ServiceCode == "TGD")
                {
                    DB.LogGenera("Get_accessorials_by_RequestID", "found LGDEL", "found LGDEL");
                    AccessorialsObj.LGDEL = true;
                }
                else if (list[i].ServiceCode == "TGP")
                {
                    AccessorialsObj.LGPU = true;
                }
                else if (list[i].ServiceCode == "TSD")
                {
                    AccessorialsObj.TRADEDEL = true;
                }
                else if (list[i].ServiceCode == "TSP")
                {
                    AccessorialsObj.TRADEPU = true;
                }
                else if (list[i].ServiceCode == "AMD")
                {
                    AccessorialsObj.APTDEL = true;
                }
                else if (list[i].ServiceCode == "AMP")
                {
                    AccessorialsObj.APTPU = true;
                }
                else if (list[i].ServiceCode == "ISD")
                {
                    AccessorialsObj.INSDEL = true;
                }
            }
        }

        #endregion

        #region Get_total_cube_by_QuoteID

        public void Get_total_cube_by_QuoteID(string QuoteID, ref AES_API_info api_info)
        {
            
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesAPI))
                {
                    #region SQL

                    string sql = string.Concat("SELECT TotalCube ",

                        "FROM Genera_Rating ",

                        "WHERE QuoteID=", QuoteID);

                    #endregion

                    DB.LogGenera(iam, "Get_total_cube_by_QuoteID sql", sql);

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                LTLPiece piece = new LTLPiece();
                                if (reader["TotalCube"] != DBNull.Value)
                                {
                                    api_info.total_cube = (double)reader["TotalCube"];

                                    DB.LogGenera(iam, "Get_total_cube_by_QuoteID api_info.total_cube", api_info.total_cube.ToString());
                                }                           
                            }
                        }
                    }
                    DB.LogGenera(iam, "Get_total_cube_by_QuoteID api_info.total_cube", api_info.total_cube.ToString());

                }
            }
            catch (Exception e)
            {
                DB.LogGenera(iam, "Get_total_cube_by_QuoteID", e.ToString());
            }

            //return api_info;
        }

        #endregion

        #region Get_sub_nmfc_by_density

        public void Get_sub_nmfc_by_density(ref AES_API_info api_info, out int sub_nmfc)
        {
            sub_nmfc = 0;
            if (api_info.total_density <= 0)
            {

                DB.LogGenera("Get_sub_nmfc_by_density", "sub_nmfc", "total_density was less than zero");
                sub_nmfc = -1;
            }
            else if (api_info.total_density > 0 && api_info.total_density < 1)
            {
                sub_nmfc = 1;
            }
            else if (api_info.total_density >= 1 && api_info.total_density < 2)
            {
                sub_nmfc = 2;
            }
            else if (api_info.total_density >= 2 && api_info.total_density < 4)
            {
                sub_nmfc = 3;
            }
            else if (api_info.total_density >= 4 && api_info.total_density < 6)
            {
                sub_nmfc = 4;
            }
            else if (api_info.total_density >= 6 && api_info.total_density < 8)
            {
                sub_nmfc = 5;
            }
            else if (api_info.total_density >= 8 && api_info.total_density < 10)
            {
                sub_nmfc = 6;
            }
            else if (api_info.total_density >= 10 && api_info.total_density < 12)
            {
                sub_nmfc = 7;
            }
            else if (api_info.total_density >= 12 && api_info.total_density < 15)
            {
                sub_nmfc = 8;
            }
            else if (api_info.total_density >= 15 && api_info.total_density < 22.5)
            {
                sub_nmfc = 9;
            }
            else if (api_info.total_density >= 22.5 && api_info.total_density < 30)
            {
                sub_nmfc = 10;
            }
            else
            {
                sub_nmfc = 10;
            }           
        }

        #endregion

        #region UpdateStatsQuoteWasBooked

        public void UpdateStatsQuoteWasBooked(string newLogId, string PNW, string destinationCompany)
        {
            try
            {
                DB.LogGenera("Public Repository", "updateStatsQuoteWasBooked newLogId, PNW", 
                    string.Concat(newLogId, " ", PNW));

                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringRater2009))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        string sql = "UPDATE SQL_STATS_GCM SET WasBooked='True', DateBooked='" + DateTime.Today.ToShortDateString() +
                            "', TimeBooked='" + DateTime.Now.ToShortTimeString() + 
                            "', PNW='" + PNW + "',Consignee_comp='" + destinationCompany.Replace("'","''") + "'" +
                            " WHERE ID=" + newLogId;

                        DB.LogGenera("Public Repository", "updateStatsQuoteWasBooked sql", sql);

                        command.CommandText = sql;
                        conn.Open();
                        command.Connection = conn;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                DB.LogGenera("Public Repository", "exception", e.ToString());
            }
        }

        #endregion

        //

        #region Get_pnw_info

        public Booking_PNW_info Get_pnw_info(string PNW)
        {
            Booking_PNW_info info = new Booking_PNW_info();

            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesAPI))
                {
                    #region SQL

                    string sql = string.Concat("SELECT * ",

                        "FROM Genera_Booking_2 ",//,                    
                        "WHERE PNW='", PNW, "'"
                        );

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
                                if (reader["PNW"] != DBNull.Value)
                                {
                                    info.PNW = reader["PNW"].ToString();

                                    info.ID = (int)reader["ID"];

                                    info.TimeStamp = (DateTime)reader["TimeStamp"];

                                    info.Booking_request = reader["Booking_request"].ToString();
                                }
                            }
                        }
                    }
                }

                return info;
            }
            catch (Exception e)
            {
                DB.LogGenera(iam, "Get_pnw_info", e.ToString());
                return info;
            }
        }

        #endregion

        #region Get_booking_info_by_booking_key

        //public void Get_booking_info_by_booking_key(string booking_key, out AES_API_info api_info)
        //{
        //    api_info = new AES_API_info();
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesAPI))
        //        {
        //            #region SQL

        //            string sql = string.Concat("SELECT BusinessDays, CarrierDisplayName, Rate, RequestId, CarrierKey, BuyRate, QuoteId,SCAC,CarrierQuoteID ",

        //                "FROM LTL_RATE_RESULTS ",

        //                "WHERE BookingKey='", booking_key, "'");

        //            #endregion

        //            using (SqlCommand command = new SqlCommand())
        //            {
        //                command.Connection = conn;
        //                command.CommandText = sql;
        //                conn.Open();
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    if (reader.Read())
        //                    {
        //                        if (reader["BusinessDays"] != DBNull.Value)
        //                        {
        //                            api_info.BusinessDays = reader["BusinessDays"].ToString();
        //                        }
        //                        if (reader["CarrierDisplayName"] != DBNull.Value)
        //                        {
        //                            api_info.CarrierDisplayName = reader["CarrierDisplayName"].ToString();
        //                        }
        //                        if (reader["Rate"] != DBNull.Value)
        //                        {
        //                            api_info.Rate = string.Format("{0:0.00}", (double)reader["Rate"]);
        //                        }
        //                        if (reader["RequestId"] != DBNull.Value)
        //                        {
        //                            api_info.RequestId = reader["RequestId"].ToString();
        //                        }
        //                        if (reader["CarrierKey"] != DBNull.Value)
        //                        {
        //                            api_info.CarrierKey = reader["CarrierKey"].ToString();
        //                        }
        //                        if (reader["BuyRate"] != DBNull.Value)
        //                        {
        //                            api_info.BuyRate = string.Format("{0:0.00}", (double)reader["BuyRate"]);
        //                        }
        //                        if (reader["QuoteId"] != DBNull.Value)
        //                        {
        //                            api_info.QuoteId = reader["QuoteId"].ToString();
        //                        }
        //                        if (reader["SCAC"] != DBNull.Value)
        //                        {
        //                            api_info.SCAC = reader["SCAC"].ToString();
        //                        }
        //                        if (reader["CarrierQuoteID"] != DBNull.Value)
        //                        {
        //                            api_info.CarrierQuoteID = reader["CarrierQuoteID"].ToString();
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        DB.LogGenera(iam, "Get_booking_info_by_booking_key", e.ToString());
        //    }

        //    //return api_info;
        //}

        #endregion

        #region Get_carriers_by_QuoteID

        //public void Get_carriers_by_QuoteID(string QuoteID, out List<AES_API_info> quote_carriers)
        //{
        //    quote_carriers = new List<AES_API_info>();
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesAPI))
        //        {
        //            #region SQL

        //            string sql = string.Concat("SELECT BusinessDays, CarrierDisplayName, Rate, RequestId, CarrierKey, BuyRate, QuoteId,SCAC,CarrierQuoteID ",

        //                "FROM LTL_RATE_RESULTS ",

        //                "WHERE QuoteId=", QuoteID);

        //            #endregion

        //            DB.LogGenera("Get_carriers_by_QuoteID", "sql", sql);

        //            using (SqlCommand command = new SqlCommand())
        //            {
        //                command.Connection = conn;
        //                command.CommandText = sql;
        //                conn.Open();
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        AES_API_info api_info = new AES_API_info();


        //                        if (reader["BusinessDays"] != DBNull.Value)
        //                        {
        //                            api_info.BusinessDays = reader["BusinessDays"].ToString();
        //                        }
        //                        if (reader["CarrierDisplayName"] != DBNull.Value)
        //                        {
        //                            api_info.CarrierDisplayName = reader["CarrierDisplayName"].ToString();
        //                        }
        //                        if (reader["Rate"] != DBNull.Value)
        //                        {
        //                            api_info.Rate = string.Format("{0:0.00}", (double)reader["Rate"]);
        //                        }
        //                        if (reader["RequestId"] != DBNull.Value)
        //                        {
        //                            api_info.RequestId = reader["RequestId"].ToString();
        //                        }
        //                        if (reader["CarrierKey"] != DBNull.Value)
        //                        {
        //                            api_info.CarrierKey = reader["CarrierKey"].ToString();
        //                        }
        //                        if (reader["BuyRate"] != DBNull.Value)
        //                        {
        //                            api_info.BuyRate = string.Format("{0:0.00}", (double)reader["BuyRate"]);
        //                        }
        //                        if (reader["QuoteId"] != DBNull.Value)
        //                        {
        //                            api_info.QuoteId = reader["QuoteId"].ToString();
        //                        }
        //                        if (reader["SCAC"] != DBNull.Value)
        //                        {
        //                            api_info.SCAC = reader["SCAC"].ToString();
        //                        }
        //                        if (reader["CarrierQuoteID"] != DBNull.Value)
        //                        {
        //                            api_info.CarrierQuoteID = reader["CarrierQuoteID"].ToString();
        //                        }

        //                        quote_carriers.Add(api_info);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        DB.LogGenera(iam, "Get_booking_info_by_booking_key", e.ToString());
        //    }

        //    //return api_info;
        //}

        #endregion

        #region Strucs

        public struct Cancelled_PNW_info
        {
            public int ID;
            public DateTime TimeStamp;
            public string PNW;
        }

        public struct Booking_PNW_info
        {
            public int ID;
            public DateTime TimeStamp;
            public string PNW, Booking_request, poNumber;
            public List<AES_API_info> quote_carriers;
        }

        #endregion

    }
}