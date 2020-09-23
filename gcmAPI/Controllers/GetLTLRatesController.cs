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
using System.Text;
using gcmAPI.Models.LTL;
using gcmAPI.Models.Utilities;

#endregion

namespace gcmAPI.Controllers
{
    public class GetLTLRatesController : ApiController
    {
       
        // POST api/getltlrates
        public string Post(FormDataCollection form)
        {
            try
            {
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HelperFuncs.writeToSiteErrors("get_ltl_rates new ", "get_ltl_rates new ");

                //HelperFuncs.QuoteData quoteData = new HelperFuncs.QuoteData();
                QuoteData quoteData = new QuoteData();

                Models.LTL.Helper helper = new Models.LTL.Helper();
                helper.setParameters(ref form, ref quoteData);

                LTL_Carriers carriers = new LTL_Carriers(quoteData);
                SharedLTL.CarriersResult result = carriers.GetRates();
                //string res = carriers.GetRates();
                //HelperFuncs.writeToSiteErrors("get_ltl_rates result", res);
                StringBuilder res = new StringBuilder();

                //res.Append("myObj = {\"carrierRates\": ");
                //res.Append("\"carrierRates\": ");
                res.Append(result.totalQuotes.ToJSON());
                //SharedLTL.getCarriersResultJSON(ref result.totalQuotes, ref res);

                //res.Append("}");
                //res.Append("]");

                return res.ToString();
            }
            catch (Exception e)
            {
                HelperFuncs.writeToSiteErrors("get_ltl_rates", e.ToString());
                return "0";
            }
        }

     
    }
}
