#region Using 

using System;
using System.Collections.Generic;

#endregion

namespace gcmAPI.Models.Public.LTL
{
    public class LTLQuoteRequest
    {
        public List<Item> items { get; set; }
        public AdditionalServices additionalServices { get; set; }
        public string originZip { get; set; }
        public string originCity { get; set; }
        public string originState { get; set; }

        public string originCountry { get; set; }
        public string destinationZip { get; set; }
        public string destinationCity { get; set; }
        public string destinationState { get; set; }

        public string destinationCountry { get; set; }

        public DateTime pickupDate { get; set; }

        public double? linealFeet { get; set; }

        public double? totalCube { get; set; }

    }
}