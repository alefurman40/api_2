#region Using

using gcmAPI.Models.LTL;
using gcmAPI.Models.Public.LTL;
using gcmAPI.Models.Repository;
using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using static gcmAPI.Models.Utilities.Mail;

#endregion

namespace gcmAPI.Models.Customers
{
    public class Genera
    {
        #region Get_active_carriers

        public string[] Get_active_carriers(string service_type)
        {
            StringBuilder sb = new StringBuilder();
            string search_column = "is_active";

            if(service_type == "Volume")
            {
                search_column = service_type;
            }
            //DB.LogGenera("", "service_type", service_type);
            //DB.LogGenera("", "search_column", search_column);

            List<string> list = new List<string>();
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.conn_string_Genera))
                {
                    #region SQL

                    //string sql = string.Concat("SELECT SCAC ",

                    //    "FROM Carriers ",

                    //    "WHERE ", search_column, "='True'");

                    string sql = string.Concat("SELECT SCAC ",

                        "FROM Carriers ",

                        "WHERE ", search_column, "='True'");

                    #endregion

                    //DB.LogGenera("","Get_active_carriers sql", sql);

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = sql;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader["SCAC"] != DBNull.Value)
                                {

                                    //DB.Log("Get_Genera_Truckload rate", rate.ToString());
                                    list.Add(reader["SCAC"].ToString());
                                    //if(service_type == "Volume")
                                    //{
                                        //DB.LogGenera("get active carriers", "SCAC " + service_type,
                                        //reader["SCAC"].ToString());
                                    //}
                                    
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DB.Log("Get_active_carriers", e.ToString());
            }
            return list.ToArray();
        }

        #endregion

        #region Get_Genera_bill_to

        public string Get_Genera_bill_to(string SCAC)
        {
            string bill_to = "";

            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.conn_string_Genera))
                {
                    #region SQL

                    string sql = string.Concat("SELECT Bill_to_label ",

                        "FROM BillTo ",

                        "WHERE SCAC='", SCAC, "'");

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
                                if (reader["Bill_to_label"] != DBNull.Value)
                                {
                                    bill_to = reader["Bill_to_label"].ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DB.LogGenera("LTL_Carriers", "Get_Genera_bill_to", e.ToString());
            }

            if (bill_to == "")
            {
                DB.LogGenera("LTL_Carriers", "bill_to was empty", SCAC);
            }

            return bill_to;
        }

        #endregion

        #region Send_email_over_9_pallets

        public void Send_email_over_9_pallets(ref QuoteData quoteData, ref int newLogId)
        {
            try
            {
                if (quoteData.totalUnits > 9)
                {
                    #region Accessorials

                    string accessorials_email = "";
                    if (quoteData.AccessorialsObj.RESPU)
                    {
                        accessorials_email += "Residential Pickup,";
                    }
                    if (quoteData.AccessorialsObj.RESDEL)
                    {
                        accessorials_email += "Residential Delivery,";
                    }
                    if (quoteData.AccessorialsObj.LGPU)
                    {
                        accessorials_email += "Liftgate Pickup,";
                    }
                    if (quoteData.AccessorialsObj.LGDEL)
                    {
                        accessorials_email += "Liftgate Delivery,";
                    }
                    if (quoteData.AccessorialsObj.CONPU)
                    {
                        accessorials_email += "Construction Pickup,";
                    }
                    if (quoteData.AccessorialsObj.CONDEL)
                    {
                        accessorials_email += "Construction Delivery,";
                    }
                    if (quoteData.AccessorialsObj.INSDEL)
                    {
                        accessorials_email += "Inside Delivery,";
                    }
                    if (quoteData.AccessorialsObj.APTPU)
                    {
                        accessorials_email += "Appointment Pickup,";
                    }
                    if (quoteData.AccessorialsObj.APTDEL)
                    {
                        accessorials_email += "Appointment Delivery";
                    }

                    #endregion

                    var repo = new QuotesRepository();
                    var quote_carriers = new List<AES_API_info>();
                    repo.Get_carriers_by_QuoteID(newLogId.ToString(), out quote_carriers);

                    StringBuilder sb_carriers = new StringBuilder();
                    GetEmailText(ref sb_carriers, ref quote_carriers);

                    StringBuilder sb_ship_info = new StringBuilder();
                    Get_shipment_info_EmailText(ref sb_ship_info, ref quoteData, ref accessorials_email);

                    // Send email
                    EmailInfo email_info = new EmailInfo();
                    email_info.subject = string.Concat("Genera Volume quote ", quoteData.origZip, " to ", quoteData.destZip);
                   
                    email_info.to = AppCodeConstants.BobsEmail;
                    email_info.bcc = AppCodeConstants.Alex_email;
                    email_info.fromAddress = AppCodeConstants.Alex_email;

                    email_info.body =
                        string.Concat(
                            //quoteData.origCity, ", ", quoteData.origState, "/",
                            //quoteData.destCity, ", ", quoteData.destState, 
                            sb_ship_info,
                            //" total units: ", quoteData.totalUnits,
                            //", total weight: ", quoteData.totalWeight, " ", accessorials_email, 
                            
                            "<br><br><br>", sb_carriers);
                    email_info.fromName = "Genera Booking";

                    Mail mail = new Mail(ref email_info);
                    mail.SendEmail();
                }

            }
            catch (Exception e)
            {
                DB.LogGenera("LTL_Carriers", "Send_email_over_9_pallets", e.ToString());
            }
        }

        #endregion

        #region Get_shipment_info_EmailText

        private void Get_shipment_info_EmailText(ref StringBuilder emailText, ref QuoteData quoteData, ref string accessorials_email)
        {

            string bgColor = "";

            bool is_zebra = true;

            string tdStyle = "style='text-align: left;padding: 8px;'";//
            //string thStyle = "style='text-align: left;padding: 8px;background-color: #ffff80;color: black;'";
            string thStyle = "style='text-align: left;padding: 8px;background-color: #b3b300;color: white;'";
            //b3b300  font-weight: bold;
            emailText.Append(string.Concat("<table style='border-collapse: collapse;width: 100%;'>",
            "<tr>",


            "<th ", thStyle, ">Origin</th>",

            "<th ", thStyle, ">Destination</th>",

            "<th ", thStyle, ">Total units</th>",

            "<th ", thStyle, ">Total weight</th>",

            "<th ", thStyle, ">Accessorials</th>"

            ));

            if (is_zebra == true)
            {
                bgColor = "";
            }
            else
            {
                bgColor = "style='background-color: #f2f2f2'";
            }

            is_zebra = !is_zebra;

            emailText.Append(string.Concat("<tr ", bgColor, ">",


                "<td ", tdStyle, ">", quoteData.origCity, ", ", quoteData.origState, " ", quoteData.origZip,
                "</td>",

                 "<td ", tdStyle, ">", quoteData.destCity, ", ", quoteData.destState, " ", quoteData.destZip,
                "</td>",

                 "<td ", tdStyle, ">", quoteData.totalUnits,
                "</td>",

                 "<td ", tdStyle, ">", quoteData.totalWeight,
                "</td>",

                 "<td ", tdStyle, ">", accessorials_email,
                "</td>"

                ));

            emailText.Append(string.Concat(
            //"<td ", tdStyle, "'>", shipments[i].shipID, 
            //"</td>",
            //  "<td ", tdStyle, "'>", shipments[i].LoadNumber,
            //"</td>",
            "</tr>"
            ));

            //emailText.Append(string.Concat("<th ", thStyle, ">LOAD#</th>"));

            //for (int i = 0; i < quote_carriers.Count; i++)
            //{          
            //}

            emailText.Append("</table>");

        }

        #endregion

        #region GetEmailText

        private void GetEmailText(ref StringBuilder emailText, ref List<AES_API_info> quote_carriers)
        {

            string bgColor = "";

            bool is_zebra = true;

            string tdStyle = "style='text-align: left;padding: 8px;'";//
            //string thStyle = "style='text-align: left;padding: 8px;background-color: #ffff80;color: black;'";
            string thStyle = "style='text-align: left;padding: 8px;background-color: #b3b300;color: white;'";
            //b3b300  font-weight: bold;
            emailText.Append(string.Concat("<table style='border-collapse: collapse;width: 100%;'>",
            "<tr>",


            "<th ", thStyle, ">Carrier</th>",

            "<th ", thStyle, ">Cost $</th>"

            ));

            //emailText.Append(string.Concat("<th ", thStyle, ">LOAD#</th>"));

            for (int i = 0; i < quote_carriers.Count; i++)
            {
                //tdStyle = string.Concat("style='border:1px solid black; background-color:", "rgb(255,255,255); padding: 3px;");

                if (is_zebra == true)
                {
                    bgColor = "";
                }
                else
                {
                    bgColor = "style='background-color: #f2f2f2'";
                }

                is_zebra = !is_zebra;

                emailText.Append(string.Concat("<tr ", bgColor, ">",


                    "<td ", tdStyle, ">", quote_carriers[i].CarrierDisplayName,
                    "</td>",

                     "<td ", tdStyle, ">", quote_carriers[i].Rate,
                    "</td>"

                    ));

                emailText.Append(string.Concat(
                //"<td ", tdStyle, "'>", shipments[i].shipID, 
                //"</td>",
                //  "<td ", tdStyle, "'>", shipments[i].LoadNumber,
                //"</td>",
                "</tr>"
                ));

            }

            emailText.Append("</table>");

        }

        #endregion
    }
}