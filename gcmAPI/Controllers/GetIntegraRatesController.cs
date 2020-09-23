using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using gcmAPI.Models;
using System.Net.Http.Formatting;

namespace gcmAPI.Controllers
{
    public class GetIntegraRatesController : ApiController
    {
        // GET api/getintegrarates
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/getintegrarates/5
        public string Get(int id)
        {
            return "value";
        }

        //// POST api/getintegrarates
        //public void Post([FromBody]string value)
        //{
        //}

        // POST api/getintegrarates
        public string Post(FormDataCollection form)
        {
            string test2 = form.Get("test2");

            string myString = "";

            //GetIntegraRatesModel model = new GetIntegraRatesModel();
            //model.testFunc(ref myString);

            Intermodal.testStaticFunc(ref myString);

            return "value was " + myString;

            /*IntermodalRater ir = new IntermodalRater();
                List<string[]> results = new List<string[]>();

                int QuoteID = 0;
                results = ir.GetIntermodalRate(username, password, pickupDate, originZip, destinationZip, additionalServices, ref QuoteID);
*/
        }

        // PUT api/getintegrarates/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/getintegrarates/5
        public void Delete(int id)
        {
        }
    }
}
