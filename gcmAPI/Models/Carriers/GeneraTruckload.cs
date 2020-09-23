#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using gcmAPI.Models.LTL;
using gcmAPI.Models.Utilities;

#endregion

namespace gcmAPI.Models.Carriers
{

    public class GeneraTruckload
    {
        QuoteData quoteData;
        public GeneraTruckload(ref QuoteData quoteData)
        {
            this.quoteData = quoteData;
        }

        #region Get_rate_from_TruckloadRatesCFI

        public GCMRateQuote Get_rate_from_TruckloadRatesCFI()
        {
            decimal rate = 0m;
            GCMRateQuote quote = new GCMRateQuote();
            try
            {
                using (SqlConnection conn = new SqlConnection(AppCodeConstants.connStringAesAPI))
                {
                    #region SQL

                    string sql = string.Concat("SELECT rate,transit,quote_id ",

                        "FROM TruckloadRatesCFI ",

                        "WHERE orig_zip='", quoteData.origZip, "' AND dest_zip='", quoteData.destZip, "'");

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
                                if (reader["rate"] != DBNull.Value)
                                {
                                    rate = (decimal)reader["rate"];
                                    quote.TotalPrice = Convert.ToDouble(rate);
                                    quote.DisplayName = "RRD Truckload";
                                    quote.CarrierKey = "NA";
                                    quote.BookingKey = "#1#";
                                    quote.OurRate = quote.TotalPrice;
                                    quote.RateType = "Truckload Quote";
                                    if(reader["transit"] != DBNull.Value)
                                    {
                                        quote.DeliveryDays = (int)reader["transit"];
                                    }
                                    else
                                    {
                                        quote.DeliveryDays = 10;
                                    }
                                    quote.Scac = "DRRQ";

                                    if (reader["quote_id"] != DBNull.Value)
                                    {
                                        quote.CarrierQuoteID = reader["quote_id"].ToString();
                                    }
                                    
                                    //quote.CoverageCost = 100000;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DB.Log("Get_rate_from_TruckloadRatesCFI", e.ToString());
            }
            return quote;
        }
        
        #endregion
    }
}