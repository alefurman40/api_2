using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gcmAPI.Models.Public.LTL
{
    public class AES_API_info
    {
        public string BusinessDays, CarrierDisplayName, Rate, RequestId, CarrierKey, BuyRate, QuoteId,SCAC, CarrierQuoteID;
        public string proNumber;
        string orig_zip, orig_city, orig_state, dest_zip, dest_city, dest_state;

        public LTLPiece[] m_lPiece;
        HelperFuncs.AccessorialsObj accessorialsObj;

        public double total_cube,total_density;
        public int total_weight, total_units;
    }
}