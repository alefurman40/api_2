using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gcmAPI;
//using Tests.Intermodal;
using gcmAPI.Models.Carriers;
using gcmAPI.Models.LTL;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            //LOUP_test loup = new LOUP_test();
            //loup.Get_LOUP_rates();

            //Console.WriteLine("Hello World");
            //Console.ReadLine();

            CarrierAcctInfo acctInfo = new CarrierAcctInfo();
            acctInfo.acctNum = "";
            acctInfo.chargeType = "P";
            acctInfo.terms = "3";
            acctInfo.username = "";
            acctInfo.password = "";

            acctInfo.bookingKey = "#1#";
            acctInfo.displayName = "Estes Genera";
            acctInfo.carrierKey = "Estes";

            //Repository repo = new Repository();

            QuoteData quoteData = new QuoteData();
            quoteData.origCity = "Atlanta";
            quoteData.origZip = "30303";
            quoteData.origState = "GA";

            quoteData.destCity = "Seattle";
            quoteData.destZip = "98144";
            quoteData.destState = "WA";

            GCMRateQuote estesQuote_DLS_account=new GCMRateQuote();

            Estes estes = new Estes(ref acctInfo, ref quoteData);
            estes.getEstesAPI_Rate("US", "US", null, ref estesQuote_DLS_account);
        }
    }
}
