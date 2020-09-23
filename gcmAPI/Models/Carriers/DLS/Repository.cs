using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace gcmAPI.Models.Carriers.DLS
{
    public class Repository
    {
        #region Repository

        #region GetDestCompInfoByCompID
        /*
        public static void GetCompInfoByUserName(string UserName, ref HelperFuncs.CompInfo info)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
                {
                    #region SQL

                    string sql = string.Concat("SELECT City, State, Zip, Phone, Addr1, Addr2, CompName, Contact, EMail, Fax ",

                        "FROM tbl_COMPANY ",

                        "WHERE AccountNbr='", UserName, "'");

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
                                if (reader["City"] != DBNull.Value)
                                {
                                    info.City = reader["City"].ToString();
                                }

                                if (reader["State"] != DBNull.Value)
                                {
                                    info.State = reader["State"].ToString();
                                }

                                if (reader["Zip"] != DBNull.Value)
                                {
                                    info.Zip = reader["Zip"].ToString();
                                }

                                if (reader["CompName"] != DBNull.Value)
                                {
                                    info.CompName = reader["CompName"].ToString();
                                }

                                if (reader["Contact"] != DBNull.Value)
                                {
                                    info.Contact = reader["Contact"].ToString();
                                }

                                if (reader["Phone"] != DBNull.Value)
                                {
                                    info.Phone = reader["Phone"].ToString();
                                }

                                if (reader["Addr1"] != DBNull.Value)
                                {
                                    info.Addr1 = reader["Addr1"].ToString();
                                }

                                if (reader["Addr2"] != DBNull.Value)
                                {
                                    info.Addr2 = reader["Addr2"].ToString();
                                }

                                if (reader["EMail"] != DBNull.Value)
                                {
                                    info.EMail = reader["EMail"].ToString();
                                }

                                if (reader["Fax"] != DBNull.Value)
                                {
                                    info.Fax = reader["Fax"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HelperFuncs.writeToSiteErrors("poSystemFuncs", e.ToString());
            }
        }
        */
        #endregion

        #region GetRRD_ScacByCarrierName
        /*
        public static void GetRRD_ScacByCarrierName(string CarrierName, ref string SCAC)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringLTLRater))
                {
                    #region SQL

                    string sql = string.Concat("SELECT SCAC ",

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
                                if (reader["SCAC"] != DBNull.Value)
                                {
                                    SCAC = reader["SCAC"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HelperFuncs.writeToSiteErrors("GetRRD_ScacByCarrierName", e.ToString());
            }
        }
        */
        #endregion

        #region GetDLS_strSQL_ForCompID
        /*
        private static string GetDLS_strSQL_ForCompID(string CompName)
        {
            string sql = string.Concat("select CompID from tbl_Company where Carrier = 1 AND CompName='", CompName, "'");
            HelperFuncs.writeToDispatchLogs("carNameTest RRD", sql);
            return sql;
        }
        */
        #endregion

        #region GetDLS_CarrierCompanyID
        /*
        public static string GetDLS_CarrierCompanyID(ref string strCarrierName)
        {
            string rrd = " RRD", strSQL = "";

            HelperFuncs.writeToDispatchLogs("RRD carrier WS", strCarrierName);

            switch (strCarrierName)
            {
                case "Crosscountry Courier Inc RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Cross Country Courier");
                        break;
                    }
                case "Con-Way RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Con-Way Transportation Services");
                        break;
                    }
                case "XPO Logistics Freight, Inc. RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Con-Way Transportation Services");
                        break;
                    }
                case "Fedex LTL Priority RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("FEDEX LTL PRIORITY");
                        break;
                    }
                case "FedEx Freight (R) Priority RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("FEDEX LTL PRIORITY");
                        break;
                    }
                case "Fedex LTL Economy RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("FEDEX LTL ECONOMY");
                        break;
                    }
                case "FedEx Freight (R) Economy RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("FEDEX LTL ECONOMY");
                        break;
                    }
                case "Central Transport RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("CENTRAL TRANSPORT");
                        break;
                    }
                case "YRC Freight RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("YRC");
                        break;
                    }
                case "UPS Freight RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("UPS FREIGHT");
                        break;
                    }
                case "SAIA RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Saia Motor Freight");
                        break;
                    }
                case "Estes Express Lines RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("ESTES EXPRESS LINES");
                        break;
                    }
                case "R & L Carriers RRD":
                    {
                        //strSQL = GetDLS_strSQL_ForCompID("R+L Carriers, inc.");
                        strSQL = string.Concat("select CompID from tbl_Company where Carrier=1 AND CompID=158276");
                        break;
                    }
                case "R $ L Carriers RRD": //R $ L Carriers RRD
                    {
                        //strSQL = GetDLS_strSQL_ForCompID("R+L Carriers, inc.");
                        strSQL = string.Concat("select CompID from tbl_Company where Carrier=1 AND CompID=158276");
                        break;
                    }
                case "ROADRUNNER TRANSPORTATION SERVICES INC RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("ROADRUNNER TRANSPORTATION - RRTS");
                        break;
                    }
                case "New Penn Motor Express, Inc. RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("NEW PENN MOTOR EXPRESS");
                        break;
                    }
                case "Clear Lane Freight Systems LLC RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("CLEAR LANE FREIGHT SYSTEMS");
                        break;
                    }
                case "Pitt Ohio Express RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("PITT OHIO EXPRESS");
                        break;
                    }
                case "Dayton Freight Lines Inc RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("DAYTON FREIGHT LINES");
                        break;
                    }
                case "AAA Cooper RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("AAA COOPER");
                        break;
                    }
                case "AAA Cooper Transportation RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("AAA COOPER");
                        break;
                    }
                case "A&B Freight Line RRD":
                    {
                        //strSQL = GetDLS_strSQL_ForCompID("Saia Motor Freight");
                        break;
                    }
                case "ABERDEEN EXPRESS INC. RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("ABERDEEN EXPRESS");
                        break;
                    }
                case "ABERDEEN EXPRESS INC RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("ABERDEEN EXPRESS");
                        break;
                    }
                case "Averitt Express Express RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Averitt Express");
                        break;
                    }
                case "Benjamin Best Freight Inc RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Benjamin Best freight");
                        break;
                    }
                case "Beaver Express RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("BEAVER EXPRESS");
                        break;
                    }
                case "Benton Express RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("BENTON EXPRESS");
                        break;
                    }
                case "Central Freight Lines RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("CENTRAL FREIGHT LINES");
                        break;
                    }
                case "Central Transport Intl RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("CENTRAL TRANSPORT");
                        break;
                    }
                case "Clear Lane Freight RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("CLEAR LANE FREIGHT SYSTEMS");
                        break;
                    }
                case "Dayton Freight RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("DAYTON FREIGHT LINES");
                        break;
                    }
                case "DHE RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("DEPENDABLE HIGHWAY EXPRESS");
                        break;
                    }
                case "Dependable Highway Express RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("DEPENDABLE HIGHWAY EXPRESS");
                        break;
                    }
                case "Dorhn Transfer RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Dohrn Transfer Company");
                        break;
                    }
                case "Dohrn Transfer RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Dohrn Transfer Company");
                        break;
                    }
                case "Dugan RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Dugan Truck Line");
                        break;
                    }
                case "Expedited Freight Systems RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Expedited Freight Systems");
                        break;
                    }
                case "Expedited Freight Systems, Llc RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Expedited Freight Systems");
                        break;
                    }
                case "Frontline Freight RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Frontline Freight Inc");
                        break;
                    }
                case "LME Inc RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("LAKEVILLE MOTOR EXPRESS");
                        break;
                    }
                case "lme inc rrd":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("LAKEVILLE MOTOR EXPRESS");
                        break;
                    }
                case "Lakeville Motor RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("LAKEVILLE MOTOR EXPRESS");
                        break;
                    }
                case "Lakeville Motor Express, Inc. RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("LAKEVILLE MOTOR EXPRESS");
                        break;
                    }
                case "Land Air Express RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("LAND AIR EXPRESS");
                        break;
                    }
                case "Midwest Motor Express RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("MIDWEST MOTOR EXPRESS");
                        break;
                    }
                case "New England Motor RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("NEW ENGLAND MOTOR FREIGHT");
                        break;
                    }
                case "New England Motor Freight RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("NEW ENGLAND MOTOR FREIGHT");
                        break;
                    }
                case "Oak Harbor RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("OAK HARBOR");
                        break;
                    }
                case "Southeastern Freight Lines RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("SOUTHEASTERN FREIGHT LINES");
                        break;
                    }
                case "Southwestern Motor RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("SOUTHWESTERN MOTOR TRANSPORT");
                        break;
                    }
                case "SOUTHWESTERN MOTOR TRANSPORT INC RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("SOUTHWESTERN MOTOR TRANSPORT");
                        break;
                    }
                case "Standard Forwarding RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Standard Forwarding Company");
                        break;
                    }
                case "STANDARD FORWARDING LLC RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Standard Forwarding Company");
                        break;
                    }
                case "USF Holland RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("USF Holland");
                        break;
                    }
                case "USF Reddaway RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("USF Reddaway");
                        break;
                    }
                case "Valley Cartage Company RRD":
                    {
                        //strSQL = GetDLS_strSQL_ForCompID("Saia Motor Freight");
                        break;
                    }
                case "Vision Express RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Vision Express");
                        break;
                    }
                case "Wrag-Time Trans RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Vision Express");
                        break;
                    }
                case "Vision Express/Wrag-Time Trans RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Vision Express");
                        break;
                    }
                case "Vision Express/Wrag-TIme Transportation RRD": //Vision Express/Wrag-TIme Transportation RRD
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Vision Express");
                        break;
                    }
                case "Wilson Trucking Company RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("WILSON TRUCKING");
                        break;
                    }
                case "Forward Air RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("FORWARD AIR");
                        break;
                    }
                case "FORWARD AIR, INC. RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("FORWARD AIR");
                        break;
                    }
                case "Tax Airfreight, Inc. RRD":
                    {
                        strSQL = GetDLS_strSQL_ForCompID("Tax Airfreight Inc");
                        break;
                    }
                default:
                    {
                        HelperFuncs.writeToDispatchLogs("RRD carrier not caught LIVE", strCarrierName);
                        //HelperFuncs.writeToSiteErrors("RRD carrier not caught", strCarrierName);
                        break;
                    }
            }

            return strSQL;
        }
        */
        #endregion

        #region IsDLS_Book
        /*
        public static bool IsDLS_Book(string Username)
        {
            bool myBool = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesData))
                {
                    string sql = string.Concat("SELECT DLSBook FROM tbl_LOGIN WHERE UserName='", Username, "'");

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader["DLSBook"] != DBNull.Value)
                                {
                                    myBool = (bool)reader["DLSBook"];
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HelperFuncs.writeToSiteErrors("IsDLS_Book", e.ToString());
            }

            return myBool;
        }
        */
        #endregion

        #endregion
    }
}