using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace gcmAPI.Models.Utilities
{
    public static class DB
    {
        #region Log

        public static void Log(string Location, object Exception)
        {
            try
            {
                string sql = string.Concat("INSERT INTO AF_ERRORS(Exception, Page, Location, ErrorDate, ErrorTime) ",
                           "VALUES('", Exception.ToString().Replace("'", ""), "', '", "test", "', '", Location.Replace("'", ""), "', '",
                           DateTime.Today.ToShortDateString(), "', '", DateTime.Now.ToShortTimeString(), "')");

                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringLTLRater))
                {
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
                //HelperFuncs.MailUser("", e.ToString(), "LOUP controller", "",
                //                  "test subject 2", "");

                //Mail.EmailInfo email_info = new Mail.EmailInfo
                //{
                //    to = "",
                //    fromAddress = "",
                //    fromName = "",
                //    subject = "exception in Log function",
                //    body = e.ToString()
                //};

                //Mail mail = new Mail(ref email_info);
                //mail.SendEmail();
            }
        }

        #endregion

        #region Log

        public static void Log(string Location, string Exception)
        {
            try
            {
                Tester tester = new Tester();
                bool need_log = tester.Is_in_log_mode();
                if(need_log == true)
                {
                    // Do nothing
                }
                else
                {
                    return;
                }

                string sql = string.Concat("INSERT INTO AF_ERRORS(Exception, Page, Location, ErrorDate, ErrorTime) ",
                           "VALUES('", Exception.Replace("'", ""), "', '", "test", "', '", Location.Replace("'", ""), "', '",
                           DateTime.Today.ToShortDateString(), "', '", DateTime.Now.ToShortTimeString(), "')");

                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringLTLRater))
                {
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
                string str = e.ToString();
                
            }
        }

        #endregion

        #region Log

        public static void LogException(string Location, string Exception)
        {
            try
            {
                string sql = string.Concat("INSERT INTO AF_ERRORS(Exception, Page, Location, ErrorDate, ErrorTime) ",
                           "VALUES('", Exception.Replace("'", ""), "', '", "test", "', '", Location.Replace("'", ""), "', '",
                           DateTime.Today.ToShortDateString(), "', '", DateTime.Now.ToShortTimeString(), "')");

                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringLTLRater))
                {
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
             
            }
        }

        #endregion

        #region Log

        public static void Log(string Location, string Exception, ref string connection_string)
        {

            //HelperFuncs.MailUser("", test, "LOUP controller", "",
            //            "test subject 2", "");

            try
            {
                string sql = string.Concat("INSERT INTO AF_ERRORS(Exception, Page, Location, ErrorDate, ErrorTime) ",
                           "VALUES('" + Exception.Replace("'", ""), "', '", "test", "', '", Location.Replace("'", ""), "', '",
                           DateTime.Today.ToShortDateString(), "', '", DateTime.Now.ToShortTimeString(), "')");

                using (SqlConnection conn = new SqlConnection(connection_string))
                {
                    using (SqlCommand command = new SqlCommand())
                    {

                        conn.Open();

                        command.Connection = conn;
                        command.CommandText = sql;
                        command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception e)
            {
                
            }
        }

        #endregion

        #region LogGenera

        public static void LogGenera(string Sender, string Code, string Message)
        {
            try
            {
                string sql = string.Concat("INSERT INTO Genera_Logs(Sender, Code, Message) ",
                           "VALUES('", Sender, "','", Code, "','", Message.Replace("'", "''"), "')");

                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringLTLRater))
                {
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
               
            }
        }

        #endregion

        public static string GetSingleQuoteValue(string str)
        {
            return "'" + str.Trim().Replace("'", "''") + "'";
        }

        public static string GetCommaSingleQuoteValue(string str)
        {
            return ",'" + str.Trim().Replace("'", "''") + "'";
        }
    }
}