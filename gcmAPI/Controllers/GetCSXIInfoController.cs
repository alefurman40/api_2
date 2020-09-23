#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using gcmAPI.Models;
using System.Net.Http.Formatting;
using System.IO;

#endregion

namespace gcmAPI.Controllers
{
    public class GetCSXIInfoController : ApiController
    {
        // GET api/getcsxiinfo
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/getcsxiinfo/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/getcsxiinfo
        public string Post(FormDataCollection form)
        {
            #region Not used
            //#region Get form data

            ////HelperFuncs.writeToSiteErrors("test", "test1");

            //string username = form.Get("username");
            //string password = form.Get("password");
            
            //string originZip = form.Get("originZip");
            //string destinationZip = form.Get("destinationZip");
      
            //string[] additionalServices = new string[1];
            //DateTime pickupDate;

            //if (!DateTime.TryParse(form.Get("pickupDate"), out pickupDate))
            //{
            //    pickupDate = DateTime.Today.AddDays(1);
            //}

            //#endregion

            //List<string[]> accessorials = new List<string[]>();
            //IntermodalRater.railResult railResult = new IntermodalRater.railResult();

            //SharedRail.GetCSXIInfo(ref originZip, ref destinationZip, ref pickupDate, ref accessorials, ref railResult);

            //if (string.IsNullOrEmpty(railResult.success) || !railResult.success.Equals("success"))
            //{
            //    return "0";
            //}
            //else
            //{
            //    return string.Concat("success=", railResult.success, "&rate=", railResult.rate, "&transitTime=", railResult.transitTime,
            //        "&hasCapacity=", railResult.hasCapacity, "&firstCapacityDate=", railResult.firstCapacityDate.ToShortDateString(),
            //        "&eta=", railResult.eta.ToShortDateString(), "&containerSize=", railResult.containerSize);


            //}
            #endregion

            try
            {
                HelperFuncs.writeToSiteErrors("get_CSXI_rates new ", "get_CSXI_rates new ");
                int CarrierCompID = 90199;
                SharedRail.Parameters parameters = new SharedRail.Parameters();
                SharedRail.setParameters(ref form, ref parameters, ref CarrierCompID);

                SharedRail.ICarrier carrier = new CSXI(parameters);

                IntermodalRater.railResult railResult = new IntermodalRater.railResult();
                railResult = carrier.getRate();

                return SharedRail.getResultString(ref railResult);
            }
            catch (Exception e)
            {
                HelperFuncs.writeToSiteErrors("get_CSXI_rates", e.ToString());
                return "0";
            }

        }

        // PUT api/getcsxiinfo/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/getcsxiinfo/5
        public void Delete(int id)
        {
        }
    }
}
