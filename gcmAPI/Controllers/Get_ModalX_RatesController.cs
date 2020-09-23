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
    public class Get_ModalX_RatesController : ApiController
    {
        // GET api/get_modalx_rates
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/get_modalx_rates/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/get_modalx_rates
        public string Post(FormDataCollection form)
        {
            try
            {
                HelperFuncs.writeToSiteErrors("get_ModalX_rates new ", "get_ModalX_rates new ");
                int CarrierCompID = 78573; // Wrong id
                SharedRail.Parameters parameters = new SharedRail.Parameters();
                SharedRail.setParameters(ref form, ref parameters, ref CarrierCompID);

                SharedRail.ICarrier carrier = new ModalX(parameters);

                IntermodalRater.railResult railResult = new IntermodalRater.railResult();
                railResult = carrier.getRate();

                return SharedRail.getResultString(ref railResult);
            }
            catch (Exception e)
            {
                HelperFuncs.writeToSiteErrors("get_ModalX_rates", e.ToString());
                return "0";
            }
        }

        // PUT api/get_modalx_rates/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/get_modalx_rates/5
        public void Delete(int id)
        {
        }
    }
}
