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
    public class Get_LOUP_RatesController : ApiController
    {
        // GET api/get_loup_rates
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/get_loup_rates/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/get_loup_rates
        public string Post(FormDataCollection form)
        {
            try
            {
                //return "0"; // LOUP had changed their website, need to rewrite scraper
                
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HelperFuncs.writeToSiteErrors("get_loup_rates new ", "get_loup_rates new ");
                int CarrierCompID = 78573;
                SharedRail.Parameters parameters = new SharedRail.Parameters();
                SharedRail.setParameters(ref form, ref parameters, ref CarrierCompID);

                SharedRail.ICarrier carrier = new LOUP(parameters);

                IntermodalRater.railResult railResult = new IntermodalRater.railResult();
                railResult = carrier.getRate();

                return SharedRail.getResultString(ref railResult);   
                
            }
            catch (Exception e)
            {
                HelperFuncs.writeToSiteErrors("get_loup_rates", e.ToString());
                return "0";
            }
        }

        // PUT api/get_loup_rates/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/get_loup_rates/5
        public void Delete(int id)
        {
        }
    }
}
