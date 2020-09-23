#region Using

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

#endregion

namespace gcmAPI.Models.Utilities
{
    public class Logins
    {
        #region Get_login_info

        public void Get_login_info(int id, out Login_info login_info)
        {
            login_info = new Login_info();
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
                {
                    #region SQL

                    string sql = string.Concat("SELECT Username,Password,AccountNum,API_Key ",

                        "FROM tbl_CARRIER_ACCOUNTS ",

                        "WHERE ID=", id);

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
                                if (reader["Username"] != DBNull.Value)
                                {
                                    login_info.username = reader["Username"].ToString();
                                }
                                else
                                {
                                    login_info.username = "";
                                }

                                if (reader["Password"] != DBNull.Value)
                                {
                                    login_info.password = reader["Password"].ToString();
                                }
                                else
                                {
                                    login_info.password = "";
                                }

                                if (reader["AccountNum"] != DBNull.Value)
                                {
                                    login_info.account = reader["AccountNum"].ToString();
                                }
                                else
                                {
                                    login_info.account = "";
                                }

                                if (reader["API_Key"] != DBNull.Value)
                                {
                                    login_info.API_Key = reader["API_Key"].ToString();
                                }
                                else
                                {
                                    login_info.API_Key = "";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HelperFuncs.writeToSiteErrors("Get_login_info", e.ToString());
            }
        }

        #endregion

        public struct Login_info
        {
            public string username,password,account,API_Key;
        }
    }
}