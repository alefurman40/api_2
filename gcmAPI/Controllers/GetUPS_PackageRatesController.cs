using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using gcmAPI.Models;
using gcmAPI.Models.UPS_Package;
using System.Net.Http.Formatting;

namespace gcmAPI.Controllers
{
    public class GetUPS_PackageRatesController : ApiController
    {
        // GET api/getups_packagerates
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/getups_packagerates/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/getups_packagerates
        //public void Post([FromBody]string value)
        //{
        //}

        public string Post(FormDataCollection form)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HelperFuncs.writeToSiteErrors("GetUPS_PackageRatesController", "GetUPS_PackageRatesController");
                //HelperFuncs.writeToSiteErrors("GetUPS_PackageRatesController post string", form.ToString());

                #region Get form data

                //HelperFuncs.writeToSiteErrors("data", form.Get("data"));

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

                #endregion

                //string doc = Rater.getPackageAPI_XML_Rate(ref form);
                return Rater.getPackageAPI_XML_Rate(ref form);
            }
            catch (Exception e)
            {
                HelperFuncs.writeToSiteErrors("GetUPS_PackageRatesController", e.ToString());
                return "-1";
            }
        }


        // PUT api/getups_packagerates/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/getups_packagerates/5
        public void Delete(int id)
        {
        }
    }
}
