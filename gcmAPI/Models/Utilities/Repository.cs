using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace gcmAPI.Models.Utilities
{
    public class Repository
    {
        string iam = "Utilities/Repository";

        #region Get_country

        public string Get_country(string zip, string state)
        {
            #region SQL

            string sql = string.Concat("select Country from SQL_ZIPS where Zip='", zip, "' ", "and State='", state.Trim(), "' ");

            #endregion

            string country = "US";

            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringRater2009))
                {
                   

                    DB.LogGenera(iam, "Get_country sql", sql);

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                               
                                if (reader["Country"] != DBNull.Value)
                                {
                                    country = reader["Country"].ToString();

                                    DB.LogGenera(iam, "Get_country, found", country);

                                    return country;
                                }
                            }
                        }
                    }
                    

                }
            }
            catch (Exception e)
            {
                DB.LogGenera(iam, "Get_country", e.ToString());
            }
            DB.LogGenera(iam, "Get_country, did not find", country);
            return country;
        }

        #endregion
    }
}