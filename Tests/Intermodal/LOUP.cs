using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using gcmAPI;

namespace Tests.Intermodal
{
    class LOUP_test
    {
        public void Get_LOUP_rates()
        {
            try
            {
                //return "0"; // LOUP had changed their website, need to rewrite scraper

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //HelperFuncs.writeToSiteErrors("get_loup_rates new ", "get_loup_rates new ");
                int CarrierCompID = 78573;
                SharedRail.Parameters parameters = new SharedRail.Parameters();

                //username=gcm&password=gcm1&originZip=30303&destinationZip=98177&originCity=ATLANTA
                //&destinationCity=SEATTLE&isHazMat=False&pickupDate=6/12/2019

                var data = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("username", ""),
                new KeyValuePair<string, string>("password", ""),
                new KeyValuePair<string, string>("originZip", "30303"),
                new KeyValuePair<string, string>("destinationZip", "98177"),
                new KeyValuePair<string, string>("originCity", "ATLANTA"),
                new KeyValuePair<string, string>("destinationCity", "SEATTLE"),
                new KeyValuePair<string, string>("isHazMat", "False"),
                new KeyValuePair<string, string>("pickupDate", "6/12/2019")
            };


                FormDataCollection form = new FormDataCollection(data);
                SharedRail.setParameters(ref form, ref parameters, ref CarrierCompID);

                SharedRail.ICarrier carrier = new LOUP(parameters);

                IntermodalRater.railResult railResult = new IntermodalRater.railResult();
                railResult = carrier.getRate();

                //return SharedRail.getResultString(ref railResult);

            }
            catch (Exception e)
            {
                string str = e.ToString();
                //HelperFuncs.writeToSiteErrors("get_loup_rates", e.ToString());
                //return "0";
            }
        }
    }
}
