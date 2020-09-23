using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gcmAPI.Models
{
    public class Intermodal
    {
        public void testFunc(ref string testStr)
        {
            testStr += "it works";
        }

        public static string testStaticFunc(ref string testStr)
        {
            testStr += "static it works";
            return testStr;
        }

        //public void getRatesFromSoap(ref string testStr)
        //{
        //    //http://wstest.globalcargomanager.com/RateService2.asmx?WSDL
          
        //    string apiUserName = "";
        //    string apiKey = "";

        //    // Initialize web service/API object
        //    wstest.RateService2 rs = new wstest.RateService2();

        //    // Authenticate to the web service/API
        //    string sessionId = rs.Authenticate(apiUserName, apiKey);

        //    // Initialize SOAP header for authentication
        //    rs.AuthHeaderValue = new wstest.AuthHeader();

        //    // Set session id to the SOAP header
        //    rs.AuthHeaderValue.SessionID = sessionId;

        //    // Initialize pickup date, origin zip code, destination zip code
        //    DateTime pickupDate = DateTime.Today.AddDays(2);
        //    string originZip = "30303";    	//Origin ZIP code      
        //    string destinationZip = "98177";	//Destination ZIP code

        //    // Set additional services(currently not applicable)
        //    string[] additionalServices = new string[2];
        //    additionalServices[0] = "";
        //    additionalServices[1] = "";

        //    // Getting Intermodal rate
        //    wstest.IntermodalRateReply iReply = rs.GetIntermodalRate(pickupDate, originZip, destinationZip, additionalServices);
        //    wstest.IntermodalResult[] iResults = iReply.IntermodalRates;
        //}
    }
}