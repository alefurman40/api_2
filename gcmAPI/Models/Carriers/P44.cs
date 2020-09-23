using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gcmAPI.Models.Carriers
{
    public class P44
    {
        public struct result
        {
            public double cost;
            public int transit_days;
            public string carrier_name, quote_number, scac, description;
        }
    }
}