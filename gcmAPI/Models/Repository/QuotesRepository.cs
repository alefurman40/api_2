using gcmAPI.Models.Public.LTL;
using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace gcmAPI.Models.Repository
{
    public class QuotesRepository
    {
        #region GetQuoteInfoByQuoteID

        public static void GetQuoteInfoByQuoteID(ref int QuoteID, ref string oZip, ref string dZip, ref DateTime Day, ref DateTime Time)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringRater2009))
                {
                    #region SQL

                    string sql = string.Concat("SELECT Origin, Destination, Day, Time ",

                        "FROM SQL_STATS_GCM ",

                        "WHERE ID=", QuoteID);

                    #endregion

                    byte counter = 0;

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["Origin"] != DBNull.Value)
                                {
                                    oZip = reader["Origin"].ToString();
                                }

                                if (reader["Destination"] != DBNull.Value)
                                {
                                    dZip = reader["Destination"].ToString();
                                }

                                if (reader["Day"] != DBNull.Value)
                                {
                                    Day = (DateTime)reader["Day"];
                                }

                                if (reader["Time"] != DBNull.Value)
                                {
                                    Time = (DateTime)reader["Time"];
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HelperFuncs.writeToSiteErrors("GetQuoteInfoByQuoteID", e.ToString());
            }
        }

        #endregion


        #region Get_carriers_by_QuoteID

        public void Get_carriers_by_QuoteID(string QuoteID, out List<AES_API_info> quote_carriers)
        {
            quote_carriers = new List<AES_API_info>();
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringRater2009))
                {
                    #region SQL

                    string sql = string.Concat("SELECT CarrierDisplayName, TotalPrice ",

                        "FROM SQL_STATS_GCM_LTL_QUOTES ",

                        "WHERE ID=", QuoteID, " ",
                        "ORDER BY TotalPrice");

                    #endregion

                    //DB.LogGenera("Get_carriers_by_QuoteID", "sql", sql);

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

                                if (reader["CarrierDisplayName"] != DBNull.Value)
                                {
                                    api_info.CarrierDisplayName = reader["CarrierDisplayName"].ToString();
                                }
                                if (reader["TotalPrice"] != DBNull.Value)
                                {
                                    api_info.Rate = string.Format("{0:0.00}", (double)reader["TotalPrice"]);
                                }

                                quote_carriers.Add(api_info);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DB.LogGenera("get from sql_ltl_quotes", "Get_booking_info_by_booking_key", e.ToString());
            }
        }

        #endregion
    }
}