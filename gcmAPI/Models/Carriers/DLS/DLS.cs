#region Using

using System;
using System.Collections.Generic;
//using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using gcmAPI.Models.Customers;
using gcmAPI.Models.LTL;
using gcmAPI.Models.Utilities;

#endregion

namespace gcmAPI.Models.Carriers.DLS
{
    public class DLS
    {
        #region Variables

        QuoteData quoteData;
        bool guaranteedService;//, is_cust_rates;
        bool is_Estes_HHG_Under_500, is_Estes_HHG_Under_500_GLTL, is_xpo;
        string UserName, APIKey;
        //bool need_log = false;

        public struct dlsPricesheet
        {
            public string CarrierName, Scac, ContractName, Cost_breakdown;
            public decimal Total;
            public byte ServiceDays;
            public double base_rate;
        }

        #endregion

        #region Constructor

        // Constructor
        public DLS(ref List<dlsPricesheet> dlsPricesheets, ref QuoteData quoteData, bool guaranteedService, 
            ref string UserName, ref string APIKey, ref bool is_Estes_HHG_Under_500, ref bool is_Estes_HHG_Under_500_GLTL, ref bool is_xpo)
        {
            //Tester tester = new Tester();
            //need_log = tester.Is_in_log_mode();

            this.quoteData = quoteData;
            this.guaranteedService = guaranteedService;
            //this.is_cust_rates = is_cust_rates;

            this.UserName = UserName;
            this.APIKey = APIKey;
            this.is_Estes_HHG_Under_500 = is_Estes_HHG_Under_500;
            this.is_Estes_HHG_Under_500_GLTL = is_Estes_HHG_Under_500_GLTL;
            this.is_xpo = is_xpo;

            getRateFromDLS(ref dlsPricesheets, ref guaranteedService);

        }

        #endregion

        #region getRateFromDLS

        // This list is named dlsPricesheets, but works for either regular or AM or PM Guaranteed rates 
        // which ever is passed as the ref parameter

        private void getRateFromDLS(ref List<dlsPricesheet> dlsPricesheets, ref bool guaranteedService)
        {
            //DB.Log("getRateFromDLS isUSED", isUSED.ToString());

            #region Variables

            //string url = "", referrer, contentType, accept, method;

            string doc = "", data = "";
            //CookieContainer container = new CookieContainer();

            //url = "https://dlsworldwideproxy-stage.rrd.com/services";
            //url = "https://dlsworldwideproxy.rrd.com/services/api/v1/RateShop/RateRequest";

            //url = "https://dlsworldwideproxy.rrd.com/services/api/v1/RateShop/RateRequest";
            ////url = "https://dlsworldwideproxy-stage.rrd.com/services/api/v1/RateShop/RateRequest";
            //referrer = "";
            //contentType = "application/xml";
            //method = "POST";
            //accept = "text/xml";

            bool isOverlength = HelperFuncs.IsOverlength(ref quoteData.m_lPiece, 192, 192, 192);
            bool isOverlengthUPS = HelperFuncs.IsOverlength(ref quoteData.m_lPiece, 92, 92, 92);

            #endregion

            try
            {
                StringBuilder accessorials = new StringBuilder();

                Get_accessorials_request_xml(guaranteedService, isOverlength, accessorials);

                #region Post Data

                #region Date

                DateTime today = quoteData.puDate;
                string day = today.Day.ToString();
                if (day.Length.Equals(1))
                {
                    day = string.Concat("0", day);
                }

                string month = today.Month.ToString();
                if (month.Length.Equals(1))
                {
                    month = string.Concat("0", month);
                }
                string year = today.Year.ToString();

                #endregion

                #region Weight/class/overlength

                Int16 length, width, height;

                StringBuilder sbiItems = new StringBuilder();
                string fClass = "";
                double totalCube;

                for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
                {
                    sbiItems.Append("<RateItemViewModel>");

                    //totalCube = 0.0;
                    length = Convert.ToInt16(quoteData.m_lPiece[i].Length);
                    width = Convert.ToInt16(quoteData.m_lPiece[i].Width);
                    height = Convert.ToInt16(quoteData.m_lPiece[i].Height);

                    if (length > 0)
                    {
                        totalCube = (length * width * height) / 1728;
                    }
                    else
                    {
                        totalCube = 0;
                    }

                    sbiItems.Append(string.Concat("<Cube>", totalCube, "</Cube>"));

                    sbiItems.Append(string.Concat("<DimensionUnits>in</DimensionUnits>"));

                    if (quoteData.m_lPiece[i].FreightClass.Contains("."))
                    {
                        fClass = quoteData.m_lPiece[i].FreightClass;
                    }
                    else
                    {
                        fClass = string.Concat(quoteData.m_lPiece[i].FreightClass, ".0");
                    }

                    sbiItems.Append(string.Concat("<FreightClass>", fClass, "</FreightClass>"));

                    if(is_xpo==true)
                    {
                        sbiItems.Append(string.Concat("<Height>0.0</Height>"));
                        sbiItems.Append(string.Concat("<Length>0.0</Length>"));
                        sbiItems.Append(string.Concat("<Name>ItemName</Name>"));
                        sbiItems.Append(string.Concat("<Quantity>0</Quantity>"));
                    }
                    else
                    {
                        sbiItems.Append(string.Concat("<Height>", (int)quoteData.m_lPiece[i].Height, ".0</Height>"));
                        sbiItems.Append(string.Concat("<Length>", (int)quoteData.m_lPiece[i].Length, ".0</Length>"));
                        sbiItems.Append(string.Concat("<Name>ItemName</Name>"));
                        sbiItems.Append(string.Concat("<Quantity>", quoteData.m_lPiece[i].Quantity, "</Quantity>"));
                    }
                    


                    sbiItems.Append(string.Concat("<QuantityUnits>Unit</QuantityUnits>"));

                    if(is_Estes_HHG_Under_500 == true || is_Estes_HHG_Under_500_GLTL == true)
                    {
                        double actual_weight = quoteData.m_lPiece[i].Weight - quoteData.extraWeight;
                        sbiItems.Append(string.Concat("<Weight>", (int)actual_weight, ".0</Weight>"));
                    }
                    else
                    {
                    sbiItems.Append(string.Concat("<Weight>", (int)quoteData.m_lPiece[i].Weight, ".0</Weight>"));//800.0
                    }
                    sbiItems.Append(string.Concat("<WeightUnits>lb</WeightUnits>"));
                    sbiItems.Append(string.Concat("<Width>", (int)quoteData.m_lPiece[i].Width, ".0</Width>"));

                    sbiItems.Append("</RateItemViewModel>");

                }
                //DB.Log("getRateFromDLS sbiItems", sbiItems.ToString());

                #endregion

                #region Country

                string originCountry = "USA", destinationCountry = "USA";

                if (!quoteData.origCountry.Contains("US"))
                {
                    originCountry = quoteData.origCountry;
                }

                if (!quoteData.destCountry.Contains("US"))
                {
                    destinationCountry = quoteData.destCountry;
                }

                #endregion

                #region Request Data XML string

                data = string.Concat("<RateRequestViewModel xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.datacontract.org/2004/07/Rrdl.Dls.Proxy\">",
                    //"<Constraints>",
                    //"<ServiceFlags>",
                    //"<ServiceFlagViewModel>",
                    //"<ServiceCode>RES1</ServiceCode>",
                    //"</ServiceFlagViewModel>",
                    //"</ServiceFlags>",
                    //"</Constraints>",
                    accessorials,
                    "<DropEvent>",
                        "<City>", quoteData.destCity, "</City>",
                        "<Country>", destinationCountry, "</Country>",
                        "<Date>", year, "-", month, "-", day, "T22:00:00.000000</Date>",
                        "<State>", quoteData.destState, "</State>",
                        "<Zip>", quoteData.destZip, "</Zip>",
                    "</DropEvent>",
                    "<IsCompanyAccountNumber>true</IsCompanyAccountNumber>",
                    "<Items>",
                        sbiItems,
                #region Not used
                //"<RateItemViewModel>",
                //    "<Cube>100</Cube>",
                //    "<DimensionUnits>in</DimensionUnits>",
                //    "<FreightClass>70.0</FreightClass>",
                //    "<Height>48.0</Height>",
                //    "<Length>42.0</Length>",
                //    "<Name>ItemName</Name>",
                //    "<Quantity>610</Quantity>",
                //    "<QuantityUnits>Unit</QuantityUnits>",
                //    "<Weight>800.0</Weight>",
                //    "<WeightUnits>lb</WeightUnits>",
                //    "<Width>42.0</Width>",
                //"</RateItemViewModel>",
                #endregion
                "</Items>",
                    "<LinearFeet>4</LinearFeet>",
                    "<MaxPriceSheet>6</MaxPriceSheet>",
                    "<PickupEvent>",
                        "<City>", quoteData.origCity, "</City>",
                        "<Country>", originCountry, "</Country>",
                        "<Date>", year, "-", month, "-", day, "T10:00:00.000000</Date>",
                        "<State>", quoteData.origState, "</State>",
                        "<Zip>", quoteData.origZip, "</Zip>",
                    "</PickupEvent>",
                    "<ReturnAssociatedCarrierPricesheet>true</ReturnAssociatedCarrierPricesheet>",
                    "<ShowInsurance>true</ShowInsurance>",
                    "</RateRequestViewModel>");

                if (guaranteedService == false)
                {
                    //DB.Log("getRateFromDLS data", data);
                }

                #endregion

                #endregion
                
                doc = getResponseFromDLS(data);

                string res_part_1="", res_part_2="";
                if(doc.Length > 1000)
                {
                    res_part_1 = doc.Remove(doc.Length / 2);
                    res_part_2 = doc.Substring(doc.Length / 2);
                }
                
                if (guaranteedService == false)
                {
                    //DB.Log("getRateFromDLS doc", doc);
                    if (is_Estes_HHG_Under_500 == true)
                    {
                    }
                    else if (UserName == "The Exchange")
                    {
                        DB.Log("getRateFromDLS_cust_rates res_part_1", res_part_1);
                        DB.Log("getRateFromDLS_cust_rates res_part_2", res_part_2);
                    }
                    else if (UserName == "Ben Franklin Crafts - Macon")
                    {
                        DB.Log("getRateFromDLS Ben Franklin Crafts - Macon res_part_1", res_part_1);
                        DB.Log("getRateFromDLS Ben Franklin Crafts - Macon res_part_2", res_part_2);
                    }
                    else if (UserName == "Genera Corp")
                    {

                        DB.Log("getRateFromDLS Genera Corp res_part_1", res_part_1);
                        DB.Log("getRateFromDLS Genera Corp res_part_2", res_part_2);

                    }
                    else
                    {
                        // PNW - Burien WA
                        DB.Log("getRateFromDLS res_part_1", res_part_1);
                        DB.Log("getRateFromDLS res_part_2", res_part_2);
                    }
                }

                //doc = (string)HelperFuncs.generic_http_request_addHeaders("string", container, url, referrer, contentType, accept, method,
                //    data, false, headerNames, headerValues);

                // Gather results into an object
                Parser parser = new Parser(ref quoteData, ref UserName, ref guaranteedService, ref is_Estes_HHG_Under_500);

                if (UserName == "The Exchange")
                {
                    // Do nothing
                    parser.Parse_results_cust_rates(ref dlsPricesheets, ref doc);
                }
                else
                {
                    parser.Parse_results(ref dlsPricesheets, ref doc, ref isOverlengthUPS, ref is_xpo);
                }

                dlsPricesheets = dlsPricesheets.Distinct().ToList();

                //DB.Log("getRateFromDLS", doc);

            }
            catch (Exception e)
            {
                DB.Log("getRateFromDLS", e.ToString());
            }
        }

        #endregion

        #region Get_accessorials_request_xml

        private void Get_accessorials_request_xml(bool guaranteedService, bool isOverlength, StringBuilder accessorials)
        {
            #region Accessorials
            //For Additional Services//////////////
            //string services = "";
            //bool hasAccessorials = false;

            if (isOverlength.Equals(true))
            {
                //DB.Log("IsOverlength", "IsOverlength");
                accessorials.Append("<ServiceFlagViewModel><ServiceCode>OVL</ServiceCode></ServiceFlagViewModel>");
            }
            //else
            //{
            //    DB.Log("not IsOverlength", "not IsOverlength");
            //}

            // Test

            /*
            if (GS.Equals("GLTL"))
            {
                accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>", GS, "</ServiceCode></ServiceFlagViewModel>"));
            }
            */

            if (guaranteedService.Equals(true))
            {
                accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>", "GLTL", "</ServiceCode></ServiceFlagViewModel>"));
            }


            //if (GS.Equals("GSAM") || GS.Equals("GSPM"))
            //{
            //    accessorials.Append(string.Concat("<ServiceFlagViewModel><ServiceCode>", GS, "</ServiceCode></ServiceFlagViewModel>"));
            //}
            //else if (GS.Equals("GSPM"))
            //{
            //    accessorials.Append("<ServiceFlagViewModel><ServiceCode>", GS, "</ServiceCode></ServiceFlagViewModel>");
            //}

            if (quoteData.AccessorialsObj.LGPU)
            {
                accessorials.Append("<ServiceFlagViewModel><ServiceCode>LG2</ServiceCode></ServiceFlagViewModel>");
            }

            if (quoteData.AccessorialsObj.LGDEL)
            {
                accessorials.Append("<ServiceFlagViewModel><ServiceCode>LG1</ServiceCode></ServiceFlagViewModel>");
            }
            if (quoteData.AccessorialsObj.APTPU || quoteData.AccessorialsObj.APTDEL)
            {
                accessorials.Append("<ServiceFlagViewModel><ServiceCode>APPT</ServiceCode></ServiceFlagViewModel>");
            }
            if (quoteData.AccessorialsObj.INSDEL)
            {
                accessorials.Append("<ServiceFlagViewModel><ServiceCode>IDL</ServiceCode></ServiceFlagViewModel>");
            }
            if (quoteData.AccessorialsObj.RESPU)
            {
                accessorials.Append("<ServiceFlagViewModel><ServiceCode>RES2</ServiceCode></ServiceFlagViewModel>");
            }
            if (quoteData.AccessorialsObj.RESDEL)
            {
                accessorials.Append("<ServiceFlagViewModel><ServiceCode>RES1</ServiceCode></ServiceFlagViewModel>");
            }

            /*
            if (quoteData.AccessorialsObj.CONPU)
            {
                accessorials.Append("<ServiceFlagViewModel><ServiceCode>LMAC1</ServiceCode></ServiceFlagViewModel>");
            }

            if (quoteData.AccessorialsObj.CONDEL)
            {
                accessorials.Append("<ServiceFlagViewModel><ServiceCode>LMAC2</ServiceCode></ServiceFlagViewModel>");
            }
            */

            if (quoteData.AccessorialsObj.GOVPU)
            {
                accessorials.Append("<ServiceFlagViewModel><ServiceCode>LAPGOVE</ServiceCode></ServiceFlagViewModel>");
            }
            else if (quoteData.AccessorialsObj.MILIPU)
            {
                accessorials.Append("<ServiceFlagViewModel><ServiceCode>LAPMILI</ServiceCode></ServiceFlagViewModel>");
            }
            else if (quoteData.AccessorialsObj.CONPU)
            {
                if (quoteData.is_AAFES_quote == true)
                {
                    accessorials.Append("<ServiceFlagViewModel><ServiceCode>LAPMILI</ServiceCode></ServiceFlagViewModel>");
                }
                else
                {
                    accessorials.Append("<ServiceFlagViewModel><ServiceCode>LAPCONS</ServiceCode></ServiceFlagViewModel>");
                }
            }

            if (quoteData.AccessorialsObj.GOVDEL)
            {
                accessorials.Append("<ServiceFlagViewModel><ServiceCode>LADGOVE</ServiceCode></ServiceFlagViewModel>");
            }
            else if (quoteData.AccessorialsObj.MILIDEL)
            {
                accessorials.Append("<ServiceFlagViewModel><ServiceCode>LADMILI</ServiceCode></ServiceFlagViewModel>");
            }
            else if (quoteData.AccessorialsObj.CONDEL)
            {

                if (quoteData.is_AAFES_quote == true)
                {
                    accessorials.Append("<ServiceFlagViewModel><ServiceCode>LADMILI</ServiceCode></ServiceFlagViewModel>");
                }
                else
                {
                    accessorials.Append("<ServiceFlagViewModel><ServiceCode>LADCONS</ServiceCode></ServiceFlagViewModel>");
                }
            }

            if (quoteData.AccessorialsObj.TRADEPU)
            {
                accessorials.Append("<ServiceFlagViewModel><ServiceCode>CONV2</ServiceCode></ServiceFlagViewModel>");
            }
            if (quoteData.AccessorialsObj.TRADEDEL)
            {
                accessorials.Append("<ServiceFlagViewModel><ServiceCode>CONV1</ServiceCode></ServiceFlagViewModel>");
            }
            if (quoteData.isHazmat.Equals(true))
            {
                accessorials.Append("<ServiceFlagViewModel><ServiceCode>HAZM</ServiceCode></ServiceFlagViewModel>");
            }

            if (accessorials.ToString().Length > 0)
            {
                accessorials.Insert(0, "<Constraints><ServiceFlags>");
                accessorials.Append("</ServiceFlags></Constraints>");
            }

            #endregion
        }

        #endregion
        
        #region getResponseFromDLS

        // Imported from WS
        private string getResponseFromDLS(string data)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //DB.Log("getRateFromDLS", "start of func");

            #region Variables

            string url = "", referrer, contentType, accept, method, doc = "";
            CookieContainer container = new CookieContainer();

            url = "https://dlsworldwideproxy.rrd.com/services/api/v1/RateShop/RateRequest";
            referrer = "";
            contentType = "application/xml";
            method = "POST";
            accept = "text/xml";

            #endregion

            try
            {

                #region Post Data

                DB.Log("getRateFromDLS Alex2015 data", data);

                #endregion

                #region Authentication Headers

                string[] headerNames = new string[2];
                string[] headerValues = new string[2];

                headerNames[0] = "UserName";
                headerNames[1] = "APIKey";

                headerValues[0] = UserName;
                headerValues[1] = APIKey;
                
                #endregion

                doc = (string)HelperFuncs.generic_http_request_addHeaders("string", container, url, referrer, contentType, accept, method,
                    data, false, headerNames, headerValues);

                return doc;

                #region Not used
                //url = "https://dlsworldwideproxy-stage.rrd.com/services/api/v1/RateShop/RateRequest";
                //url = "https://dlsworldwideproxy-stage.rrd.com/services";
                #endregion

            }
            catch (Exception e)
            {
                DB.Log("getRateFromDLS", e.ToString());
                return "error";
            }
        }

        #endregion

    }
}