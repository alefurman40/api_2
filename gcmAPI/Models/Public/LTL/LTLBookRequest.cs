#region Using

using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Web;

#endregion

namespace gcmAPI.Models.Public.LTL
{
    public class LTLBookRequest
    {
        #region Same as LTLQuoteRequest

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

        #endregion

        public string rateType { get; set; }
        public string poNumber { get; set; }
        public string proNumber { get; set; }
        //ShipmentReference
        public string shipmentReference { get; set; }
        public string bookingKey { get; set; }
        public string customerType { get; set; }
        //public DateTime shipmentDate { get; set; }
        //public string SCAC { get; set; }

        public string originAddress1 { get; set; }
        public string originAddress2 { get; set; }

        public string originEmail { get; set; }
        public string originPhone { get; set; }
        public string originFax { get; set; }

        public string originCompany { get; set; }
        public string originName { get; set; }

        public string destinationAddress1 { get; set; }
        public string destinationAddress2 { get; set; }

        public string destinationEmail { get; set; }
        public string destinationPhone { get; set; }
        public string destinationFax { get; set; }

        public string destinationCompany { get; set; }
        public string destinationName { get; set; }

        //
        //public string destinationName { get; set; }

        public string ddlRHour { get; set; }
        public string ddlRMinute { get; set; }
        public string ddlRAMPM { get; set; }
        public string ddlCHour { get; set; }
        public string ddlCMinute { get; set; }
        public string ddlCAMPM { get; set; }
    }
}