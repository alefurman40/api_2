#region Using 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Data.SqlClient;
using gcmAPI.Models.Utilities;
using gcmAPI.Models.LTL;
using System.Web.Script.Serialization;

#endregion

namespace gcmAPI.Models.Public.LTL
{
    public class Helper
    {
        #region StoreLTLRequestsSql
        // Overloaded, used by Genera
        public void StoreLTLRequestsSql(ref QuoteData quoteData, ref GCMRateQuote[] lResult,
            int newLogId, string XML, string response_XML, out string requestId)
        {
            requestId = Guid.NewGuid().ToString();

            try
            {
                

                SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesAPI);
                conn.Open();
                SqlCommand comm2 = new SqlCommand();
                comm2.Connection = conn;

                comm2.CommandText = "insert into LTL_RATE_REQUESTS (RequestId,XML,PickupDate,OriginZip,OriginCity,OriginState,DestinationZip,DestinationCity,DestinationState,CreatedAt,UserName) " +
                    "values ('" + requestId + "','" + XML + "','" + quoteData.puDate.ToString() + "','" + quoteData.origZip + "','" + quoteData.origCity + "','" + quoteData.origState + "','" + quoteData.destZip + "','" + quoteData.destCity + "','" + quoteData.destState + "','" + DateTime.Now.ToString() + "','" + quoteData.username + "')";
                comm2.ExecuteNonQuery();

                int count = 0;

                #region Insert into LTL_RATE_REQUESTS_ITEMS

                foreach (LTLPiece lp in quoteData.m_lPiece)
                {
                    if ((lp.FreightClass == "-1" || lp.FreightClass == "" || lp.FreightClass == null || lp.FreightClass == "0") 
                        && ("dimWeight" != "-1"))
                    {
                        if (count == 0)
                            lp.FreightClass = "dimWeight";// no class rate
                        else lp.FreightClass = "-1";
                    }
                    count++;

                    string tempHazMat = "NO";
                    if (lp.HazMat)
                    {
                        tempHazMat = "YES";
                    }
                    /*comm2.CommandText = "insert into LTL_RATE_REQUESTS_ITEMS (RequestId,ClassCode,Weight,Piece,Unit,HazMat,Tag) values "+
                       "('"+requestId+"','"+lp.FreightClass+"',"+lp.Weight.ToString()+","+lp.Quantity.ToString()+",'"+lp.ItemUnit+"','"+tempHazMat+"','"+lp.Tag+"')";*/
                    comm2.CommandText = "insert into LTL_RATE_REQUESTS_ITEMS (RequestId,ClassCode,Weight,Piece,Unit,HazMat,Tag,Length,Width,Height) values " +
                        "('" + requestId + "','" + lp.FreightClass + "'," + lp.Weight.ToString() + "," + lp.Quantity.ToString() + ",'" + lp.ItemUnit + "','" + tempHazMat + "','" + lp.Tag + "'," + lp.Length.ToString() + "," + lp.Width.ToString() + "," + lp.Height.ToString() + ")";
                    comm2.ExecuteNonQuery();
                }

                #endregion

                #region Accessorials

                string sql = "",ServiceCode="",Type="";

                if(quoteData.AccessorialsObj.RESDEL==true)
                {
                    ServiceCode = "RSD";
                    Type = "DS";
                    Store_one_service_info(ref requestId, ref ServiceCode, ref Type);
                    //sql = string.Concat("insert into LTL_RATE_REQUESTS_SERVICE (RequestId,ServiceCode,Type) values ",
                    //   "('", requestId, "','RSD','DS')");
                    //HelperFuncs.ExecuteNonQuery(AppCodeConstants.connStringAesAPI, ref sql, "StoreLTLRequestsSql Genera");
                }

                if (quoteData.AccessorialsObj.RESPU == true)
                {
                    ServiceCode = "RSP";
                    Type = "PS";
                    Store_one_service_info(ref requestId, ref ServiceCode, ref Type);
                    //sql = string.Concat("insert into LTL_RATE_REQUESTS_SERVICE (RequestId,ServiceCode,Type) values ",
                    //   "('", requestId, "','RSP','PS')");
                    //HelperFuncs.ExecuteNonQuery(AppCodeConstants.connStringAesAPI, ref sql, "StoreLTLRequestsSql Genera");
                }

                //

                if (quoteData.AccessorialsObj.CONDEL == true)
                {
                    ServiceCode = "CSD";
                    Type = "DS";
                    Store_one_service_info(ref requestId, ref ServiceCode, ref Type);                 
                }

                if (quoteData.AccessorialsObj.CONPU == true)
                {
                    ServiceCode = "CSP";
                    Type = "PS";
                    Store_one_service_info(ref requestId, ref ServiceCode, ref Type);              
                }

                //

                if (quoteData.AccessorialsObj.LGDEL == true)
                {
                    ServiceCode = "TGD";
                    Type = "DS";
                    Store_one_service_info(ref requestId, ref ServiceCode, ref Type);
                }

                if (quoteData.AccessorialsObj.LGPU == true)
                {
                    ServiceCode = "TGP";
                    Type = "PS";
                    Store_one_service_info(ref requestId, ref ServiceCode, ref Type);
                }

                //

                if (quoteData.AccessorialsObj.APTDEL == true)
                {
                    ServiceCode = "AMD";
                    Type = "DS";
                    Store_one_service_info(ref requestId, ref ServiceCode, ref Type);
                }

                if (quoteData.AccessorialsObj.APTPU == true)
                {
                    ServiceCode = "AMP";
                    Type = "PS";
                    Store_one_service_info(ref requestId, ref ServiceCode, ref Type);
                }

                //

                if (quoteData.AccessorialsObj.TRADEDEL == true)
                {
                    ServiceCode = "TSD";
                    Type = "DS";
                    Store_one_service_info(ref requestId, ref ServiceCode, ref Type);
                }

                if (quoteData.AccessorialsObj.TRADEPU == true)
                {
                    ServiceCode = "TSP";
                    Type = "PS";
                    Store_one_service_info(ref requestId, ref ServiceCode, ref Type);
                }

                //

                if (quoteData.AccessorialsObj.INSDEL == true)
                {
                    ServiceCode = "ISD";
                    Type = "DS";
                    Store_one_service_info(ref requestId, ref ServiceCode, ref Type);
                }

                //comm2.CommandText = "insert into LTL_RATE_REQUESTS_SERVICE (RequestId,ServiceCode,Type) values " +
                //    "('" + requestId + "','" + pickupService + "','PS')";
                //comm2.ExecuteNonQuery();

                //comm2.CommandText = "insert into LTL_RATE_REQUESTS_SERVICE (RequestId,ServiceCode,Type) values " +
                //    "('" + requestId + "','" + deliveryService + "','DS')";
                //comm2.ExecuteNonQuery();

                //foreach (string aService in additionalServices)
                //{
                //    comm2.CommandText = "insert into LTL_RATE_REQUESTS_SERVICE (RequestId,ServiceCode,Type) values " +
                //    "('" + requestId + "','" + aService + "','AS')";
                //    comm2.ExecuteNonQuery();
                //}
                #endregion

                int addlMarkup = 0;
                int addlMin = 0;

                #region Not used
                //try
                //{
                //    if (!_UserName.Equals(string.Empty))
                //    {
                //        SqlConnection conn2 = new SqlConnection(_ConnStr);
                //        conn2.Open();
                //        SqlCommand comm = new SqlCommand("SELECT AdditionalLTLMarkup, AdditionalLTLMinimum FROM tbl_LOGIN_ACCTMANAGER WHERE UserName = '" + _UserName + "'", conn2);
                //        SqlDataReader dr = comm.ExecuteReader();
                //        bool isAuth = dr.HasRows;
                //        if (isAuth)
                //        {
                //            addlMarkup = Convert.ToInt32(dr["AdditionalLTLMarkup"]);
                //            addlMin = Convert.ToInt32(dr["AdditionalLTLMinimum"]);
                //        }
                //        dr.Close();
                //        conn2.Close();
                //        //Response.Redirect("index.aspx");
                //    }
                //}
                //catch (Exception exp)
                //{

                //}
                #endregion

                int counter = -1;

                string SCAC = "";

                DB.LogGenera(
                    "Setting API_BookingKey", "quoteData.username,quoteData.api_username,request", 
                    quoteData.username + " " + quoteData.api_username + " " + XML);

                foreach (GCMRateQuote lr in lResult)
                {
                    counter++;
                    double parentRate = 0;

                    parentRate = lr.TotalPrice / (1 + (double)(addlMarkup / 100.0));

                    if (lr.TotalPrice - parentRate < addlMin)
                    {
                        parentRate = lr.TotalPrice - addlMin;
                    }
                    //genera
                    string tempBookingKey = Guid.NewGuid().ToString();
                    if (quoteData.username == AppCodeConstants.un_genera && XML == "LiveGCM")
                    {
                        lr.API_BookingKey = tempBookingKey;
                    }
                    else
                    {
                        lr.BookingKey = tempBookingKey;
                    }

                    

                    if (counter > 0)
                    {
                        response_XML = "";
                    }
                    else
                    {
                        // Do nothing
                    }

                    if (lr.DisplayName == "RRD Truckload")
                    {
                        SCAC = "DRRQ";
                    }
                    else
                    {
                        SCAC = lr.Scac;
                    }

                    if (quoteData.username == AppCodeConstants.un_genera && XML == "LiveGCM")
                    {
                        comm2.CommandText = string.Concat("insert into LTL_RATE_RESULTS (BookingKey,XML,BusinessDays,CarrierQuoteID,CarrierDisplayName,SCAC,Rate,RequestId,CarrierKey, BuyRate, QuoteId) values ",
                            "('", lr.API_BookingKey, "','", response_XML, "','", lr.DeliveryDays, "','", lr.CarrierQuoteID, "','",
                            lr.DisplayName, "','", SCAC, "',",
                            parentRate, ",'", requestId, "','", lr.CarrierKey, "', ",
                            lr.OurRate, ",",
                            newLogId, ")");
                    }
                    else
                    {
                        comm2.CommandText = string.Concat("insert into LTL_RATE_RESULTS (BookingKey,XML,BusinessDays,CarrierQuoteID,CarrierDisplayName,SCAC,Rate,RequestId,CarrierKey, BuyRate, QuoteId) values ",
                            "('", lr.BookingKey, "','", response_XML, "','", lr.DeliveryDays, "','", lr.CarrierQuoteID, "','",
                            lr.DisplayName, "','", SCAC, "',",
                            parentRate, ",'", requestId, "','", lr.CarrierKey, "', ",
                            lr.OurRate, ",",
                            newLogId, ")");
                    }

                    comm2.ExecuteNonQuery();
                    
                }
                conn.Close();
            }
            catch(Exception e)
            {
                DB.LogGenera("StoreLTLRequestsSql", "StoreLTLRequestsSql", e.ToString());
            }
       
        }

        #endregion

        #region Store_one_service_info

        private void Store_one_service_info(ref string requestId, ref string ServiceCode, ref string Type)
        {
            string sql = string.Concat("insert into LTL_RATE_REQUESTS_SERVICE (RequestId,ServiceCode,Type) values ",
                       "('", requestId, "','", ServiceCode, "','", Type, "')");
            HelperFuncs.ExecuteNonQuery(AppCodeConstants.connStringAesAPI, ref sql, "StoreLTLRequestsSql Genera");
        }

        #endregion

        #region StoreLTLRequestsSql
        // Overloaded
        public LTLResult[] StoreLTLRequestsSql(string _UserName, DateTime pickupDate, string originZip, string originCity, 
            string originState, string destinationZip, string destinationCity,
            string destinationState, LTLPiece[] lineItems, string pickupService, 
            string deliveryService, string[] additionalServices, string dimWeight, LTLResult[] lResult,
            bool setBuyRateToZero, int newLogId, string XML, string response_XML)
        {
            string requestId = Guid.NewGuid().ToString();
            SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesAPI);
            conn.Open();
            SqlCommand comm2 = new SqlCommand();
            comm2.Connection = conn;
            comm2.CommandText = "insert into LTL_RATE_REQUESTS (RequestId,XML,PickupDate,OriginZip,OriginCity,OriginState,DestinationZip,DestinationCity,DestinationState,CreatedAt,UserName) " +
                "values ('" + requestId + "','" + XML + "','" + pickupDate.ToString() + "','" + originZip + "','" + originCity + "','" + originState + "','" + destinationZip + "','" + destinationCity + "','" + destinationState + "','" + DateTime.Now.ToString() + "','" + _UserName + "')";
            comm2.ExecuteNonQuery();
            int count = 0;
            foreach (LTLPiece lp in lineItems)
            {
                if ((lp.FreightClass == "-1" || lp.FreightClass == "" || lp.FreightClass == null || lp.FreightClass == "0") && (dimWeight != "-1"))
                {
                    if (count == 0)
                        lp.FreightClass = dimWeight;// no class rate
                    else lp.FreightClass = "-1";
                }
                count++;

                string tempHazMat = "NO";
                if (lp.HazMat)
                {
                    tempHazMat = "YES";
                }
                /*comm2.CommandText = "insert into LTL_RATE_REQUESTS_ITEMS (RequestId,ClassCode,Weight,Piece,Unit,HazMat,Tag) values "+
                   "('"+requestId+"','"+lp.FreightClass+"',"+lp.Weight.ToString()+","+lp.Quantity.ToString()+",'"+lp.ItemUnit+"','"+tempHazMat+"','"+lp.Tag+"')";*/
                comm2.CommandText = "insert into LTL_RATE_REQUESTS_ITEMS (RequestId,ClassCode,Weight,Piece,Unit,HazMat,Tag,Length,Width,Height) values " +
                    "('" + requestId + "','" + lp.FreightClass + "'," + lp.Weight.ToString() + "," + lp.Quantity.ToString() + ",'" + lp.ItemUnit + "','" + tempHazMat + "','" + lp.Tag + "'," + lp.Length.ToString() + "," + lp.Width.ToString() + "," + lp.Height.ToString() + ")";
                comm2.ExecuteNonQuery();
            }
            //comm2.CommandText = "insert into LTL_RATE_REQUESTS_SERVICE (RequestId,ServiceCode,Type) values " +
            //    "('" + requestId + "','" + pickupService + "','PS')";
            //comm2.ExecuteNonQuery();

            //comm2.CommandText = "insert into LTL_RATE_REQUESTS_SERVICE (RequestId,ServiceCode,Type) values " +
            //    "('" + requestId + "','" + deliveryService + "','DS')";
            //comm2.ExecuteNonQuery();

            //foreach (string aService in additionalServices)
            //{
            //    comm2.CommandText = "insert into LTL_RATE_REQUESTS_SERVICE (RequestId,ServiceCode,Type) values " +
            //    "('" + requestId + "','" + aService + "','AS')";
            //    comm2.ExecuteNonQuery();
            //}

            int addlMarkup = 0;
            int addlMin = 0;

            #region Not used
            //try
            //{
            //    if (!_UserName.Equals(string.Empty))
            //    {
            //        SqlConnection conn2 = new SqlConnection(_ConnStr);
            //        conn2.Open();
            //        SqlCommand comm = new SqlCommand("SELECT AdditionalLTLMarkup, AdditionalLTLMinimum FROM tbl_LOGIN_ACCTMANAGER WHERE UserName = '" + _UserName + "'", conn2);
            //        SqlDataReader dr = comm.ExecuteReader();
            //        bool isAuth = dr.HasRows;
            //        if (isAuth)
            //        {
            //            addlMarkup = Convert.ToInt32(dr["AdditionalLTLMarkup"]);
            //            addlMin = Convert.ToInt32(dr["AdditionalLTLMinimum"]);
            //        }
            //        dr.Close();
            //        conn2.Close();
            //        //Response.Redirect("index.aspx");
            //    }
            //}
            //catch (Exception exp)
            //{

            //}
            #endregion

            int counter = -1;

            foreach (LTLResult lr in lResult)
            {
                counter++;
                double parentRate = 0;

                parentRate = lr.Rate / (1 + (double)(addlMarkup / 100.0));

                if (lr.Rate - parentRate < addlMin)
                {
                    parentRate = lr.Rate - addlMin;
                }

                string tempBookingKey = Guid.NewGuid().ToString();
                lr.BookingKey = tempBookingKey;

                if (counter > 0)
                {
                    response_XML = "";
                }
                else
                {
                    // Do nothing
                }

                comm2.CommandText = string.Concat("insert into LTL_RATE_RESULTS (BookingKey,XML,BusinessDays,CarrierDisplayName,Rate,RequestId,CarrierKey, BuyRate, QuoteId) values ",
                        "('", lr.BookingKey, "','", response_XML, "','", lr.BusinessDays, "','", lr.CarrierDisplayName, "',", parentRate, ",'", requestId, "','", lr.CarrierKey, "', ", lr.BuyRate, ",",
                        newLogId, ")");

                comm2.ExecuteNonQuery();

                if (setBuyRateToZero.Equals(true))
                {
                    lr.BuyRate = 0;
                }
            }
            conn.Close();
            return lResult;
        }

        #endregion

        #region Get_response_message
        public HttpResponseMessage Get_response_message(ref string response_string, ref string content_type)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(response_string, Encoding.UTF8, "application/json")
            };
        }

        #endregion

        #region getEmailText
        public void getEmailText(ref StringBuilder emailText, ref List<AES_API_info> quote_carriers)
        {
            string bgColor = "", compName = "", prevCompName = "", compIdStr = "";

            bool is_zebra = true;

            emailText.Append("<table style='border-collapse: collapse;width: 100%;'>" +
            "<tr><th style='text-align: left;padding: 8px;background-color: #4CAF50;color: white;'>Carrier</th><th style='text-align: left;padding: 8px;background-color: #4CAF50;color: white;'>Rate</th></tr>");

            for (int i = 0; i < quote_carriers.Count; i++)
            {
                //if (SharedFunctions.IsOdd(i))
                //    bgColor = "rgb(255,255,255)";
                //else
                //    bgColor = "rgb(255,204,153)";

                try
                {
                    if (is_zebra == true)
                    {
                        bgColor = "";
                    }
                    else
                    {
                        bgColor = "style='background-color: #f2f2f2'";
                    }

                    is_zebra = !is_zebra;

                    emailText.Append(
                        string.Concat("<tr ", bgColor, "><td style='text-align: left;padding: 8px;'>",
                        quote_carriers[i].CarrierDisplayName.Replace("%2C", ","), "</td>",
                        "<td style='text-align: left;padding: 8px;'>$", quote_carriers[i].Rate, "</td>",
                      "</tr>"));
                }
                catch
                {

                }
                
            }

            emailText.Append("</table>");

        }
        #endregion

        #region Get_cancelled_pnw_info

        public void Get_cancelled_pnw_info(string PNW, ref string po,
                    ref string our_cheapest_carrier, ref string genera_cheapest_carrier,
                    ref decimal our_cheapest_sell, ref decimal genera_cheapest)
        {


            #region Get additional info
            //
            var repo = new Models.Public.LTL.Repository();
            // Gets the Booking_request
            Models.Public.LTL.Repository.Booking_PNW_info booking_info = repo.Get_pnw_info(PNW);

            // PNW exists in Cancel but not in booking
            //if (info.Booking_request == null)
            //    continue;

            // Deserialize booking info, using the Booking_request
            LTLBookRequest ltl_book_request =
                new JavaScriptSerializer().Deserialize<LTLBookRequest>(booking_info.Booking_request); ;

            // Get the poNumber from the Booking_request
            booking_info.poNumber = ltl_book_request.poNumber;

            // 
            po = ltl_book_request.poNumber;

            // bookingKey
            // Get the Booking Key from the Booking_request
            string booking_key = ltl_book_request.bookingKey;

            //
            our_cheapest_carrier = "";
            genera_cheapest_carrier = "";
            our_cheapest_sell = 0m;
            genera_cheapest = 0m;
            bool found_genera = false;
            bool found_our = false;
            //

            // Get QuoteID by bookingKey
            AES_API_info api_info;
            repo.Get_booking_info_by_booking_key(booking_key, out api_info);
            our_cheapest_carrier = api_info.CarrierDisplayName.Replace("%2C", ",");
            //our_cheapest_sell = api_info.Rate;
            decimal.TryParse(api_info.Rate, out our_cheapest_sell);

            // Get Carrier results using the QuoteID
            List<AES_API_info> quote_carriers;
            repo.Get_carriers_by_QuoteID(api_info.QuoteId, out quote_carriers);
            booking_info.quote_carriers = quote_carriers;



            decimal test_decimal = 0m;

            //quote_carriers = quote_carriers.OrderBy(x => x.Rate).ToList();

               
            foreach (var carrier in quote_carriers)
            {
                if (carrier.CarrierDisplayName.Contains("Genera"))
                {
                    if (decimal.TryParse(carrier.Rate, out test_decimal))
                    {
                        genera_cheapest = test_decimal;
                        genera_cheapest_carrier = carrier.CarrierDisplayName.Replace("%2C", ",");
                        break;
                    }
                    //if (found_genera == false)
                    //{

                    //}
                }
                //else
                //{
                //    if (found_our == false)
                //    {
                //        if (decimal.TryParse(carrier.Rate, out test_decimal))
                //        {
                //            our_cheapest_sell = test_decimal;
                //            our_cheapest_carrier = carrier.CarrierDisplayName.Replace("%2C", ",");
                //        }
                //    }
                //}

                //if (found_genera == true) //found_our == true && 
                //    break;
            }

            #endregion

        }

        #endregion

    }
}