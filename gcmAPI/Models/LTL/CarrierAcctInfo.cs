using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gcmAPI.Models.LTL
{
    public class CarrierAcctInfo
    {
        public string acctNum, displayName, bookingKey, carrierKey;
        public string chargeType, terms, username, password;
        public GCMRateQuote quote;
    }
}