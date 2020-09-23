#region Using

using System;
using gcmAPI.Models.LTL;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using gcmAPI.Estes_RateWebServ;
using gcmAPI.Models.Utilities;

#endregion

namespace gcmAPI.Models.Carriers
{
    public class Estes
    {
        CarrierAcctInfo acctInfo;
        QuoteData quoteData;

        // Constructor
        public Estes(ref CarrierAcctInfo acctInfo, ref QuoteData quoteData)
        {
            this.acctInfo = acctInfo;
            this.quoteData = quoteData;
        }

        public Estes()
        {

        }

        #region Volume

        #region Get_ESTES_volume_rates_xml

        // This function is an attempt to consume an SOAP API, using XML instead. This is because the SOAP API
        // Is not giving the correct rate for some reason. XML is easier to debug.

        public Volume_result Get_ESTES_volume_rates_xml(ref Estes.Volume_result estes_volume_economy_result,
            ref Estes.Volume_result estes_volume_basic_result)
        {
            try
            {
              
                #region Build Items string

                int total_units = 0;

                StringBuilder items = new StringBuilder();

                for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
                {
                    // Guard
                    if (quoteData.m_lPiece[i].Length > 48 || quoteData.m_lPiece[i].Width > 48)
                    {
                        throw new Exception("Overlength for volume Estes");
                        //return;
                    }

                    //items.Append(string.Concat(
                    //    "{ \"total_weight\":", m_lPiece[i].Weight,
                    //    ", \"length\":", m_lPiece[i].Length, ", \"width\":", m_lPiece[i].Width, ", \"height\":", m_lPiece[i].Height,
                    //    ", \"units\":", m_lPiece[i].Units, " }"));

                    items.Append(string.Concat(
                        "{ \"total_weight\":", quoteData.m_lPiece[i].Weight,
                            ", \"length\":48, \"width\":48, \"height\":70, \"units\":", quoteData.m_lPiece[i].Units,
                            ", \"freight_class\":", quoteData.m_lPiece[i].FreightClass, " }"));

                    //DB.Log("P44 i", i.ToString());
                    //DB.Log("P44 Length - 1", (m_lPiece.Length - 1).ToString());

                    if (i == quoteData.m_lPiece.Length - 1) // Last iteration
                    {
                        // Do nothing
                    }
                    else
                    {
                        //DB.Log("P44 ", "i not equal to length - 1");
                        items.Append(",");
                    }

                    //

                    total_units += quoteData.m_lPiece[i].Units;
                }

                DB.Log("P44 items", items.ToString());

                #endregion


                // Guard
                if (total_units < 4)
                {
                    throw new Exception("Less than 4 units for volume Estes");
                    //return;
                }

                #region Set pickup date variables

                DateTime puDate = quoteData.puDate;
                string puDateDay = puDate.Day.ToString(), puDateMonth = puDate.Month.ToString();

                if (puDateDay.Length == 1)
                    puDateDay = "0" + puDateDay;
                if (puDateMonth.Length == 1)
                    puDateMonth = "0" + puDateMonth;

                #endregion

                #region Build Items string

                StringBuilder sb_items = new StringBuilder();
                for(byte i=0;i<quoteData.m_lPiece.Length;i++)
                {
                    sb_items.Append(string.Concat("<commodity>"));

                    sb_items.Append(string.Concat("<baseCommodity>"));
                    sb_items.Append(string.Concat("<class>", quoteData.m_lPiece[i].FreightClass,
                        "</class><weight>", quoteData.m_lPiece[i].Weight, "</weight>"));
                    sb_items.Append(string.Concat("</baseCommodity>"));

                    sb_items.Append(string.Concat("<pieces>", quoteData.m_lPiece[i].Quantity, "</pieces><pieceType>PT</pieceType>"));
                    sb_items.Append(string.Concat("<dimensions><length>48</length><width>48</width><height>70</height></dimensions>"));

                    sb_items.Append(string.Concat("</commodity>"));
                }

                #endregion

                #region Accessorials

                string hazmat = "N";
                if (quoteData.isHazmat == true)
                    hazmat = "Y";

                StringBuilder accessorials = new StringBuilder();

                #region Accessorials

                // Add APT by default
                accessorials.Append("<accessorialCode>APT</accessorialCode>");

                if (quoteData.AccessorialsObj.LGPU)
                {
                    accessorials.Append("<accessorialCode>LGATEP</accessorialCode>");
                }
                if (quoteData.AccessorialsObj.LGDEL)
                {
                    accessorials.Append("<accessorialCode>LGATE</accessorialCode>");
                }
                if (quoteData.AccessorialsObj.RESPU)
                {
                    accessorials.Append("<accessorialCode>HPUP</accessorialCode>");
                }
                if (quoteData.AccessorialsObj.RESDEL)
                {
                    accessorials.Append("<accessorialCode>HD</accessorialCode>");
                }
                if (quoteData.AccessorialsObj.CONPU)
                {
                    accessorials.Append("<accessorialCode>LAPU</accessorialCode>");
                }
                if (quoteData.AccessorialsObj.CONDEL)
                {
                    accessorials.Append("<accessorialCode>CONST</accessorialCode>");
                }
                //if (AccessorialsObj.APTPU || AccessorialsObj.APTDEL)
                //{
                //    accessorsList.Add("APT");
                //}
                //if (AccessorialsObj.APTDEL)
                //{
                //    accessorsList.Add("APTDEL");
                //}
                if (quoteData.AccessorialsObj.TRADEPU)
                {
                    accessorials.Append("<accessorialCode>FAIRPU</accessorialCode>");
                }
                if (quoteData.AccessorialsObj.TRADEDEL)
                {
                    accessorials.Append("<accessorialCode>FAIRDL</accessorialCode>");
                }
                if (quoteData.AccessorialsObj.INSDEL)
                {
                    accessorials.Append("<accessorialCode>INS</accessorialCode>");
                }

                #endregion


                #endregion

                Logins.Login_info login_info;
                Logins logins = new Logins();
                logins.Get_login_info(111, out login_info);

                string data = string.Concat(
                    "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><soap:Header><auth xmlns=\"http://ws.estesexpress.com/ratequote\">",
                    "<user>", login_info.username, "</user><password>", login_info.password, "</password></auth></soap:Header><soap:Body><rateRequest xmlns=\"http://ws.estesexpress.com/schema/2017/07/ratequote\">",
                    "<requestID>123</requestID><account>", login_info.account, "</account>",
                    "<originPoint><countryCode>US</countryCode><postalCode>", quoteData.origZip, "</postalCode>",
                    "<city>", quoteData.origCity, "</city><stateProvince>", quoteData.origState, "</stateProvince></originPoint>",
                    "<destinationPoint><countryCode>US</countryCode><postalCode>", quoteData.destZip, "</postalCode>",
                    "<city>", quoteData.destCity, "</city><stateProvince>", quoteData.destState, "</stateProvince></destinationPoint>",
                    "<payor>T</payor><terms>PPD</terms><pickup><date>", puDate.Year, "-", puDateMonth , "-", puDateDay, "</date></pickup>",
                    "<hazmat>", hazmat, "</hazmat><equipmentType>TRAILER</equipmentType>",

                    "<fullCommodities>",

                    //"<commodity>",
                    //"<baseCommodity>",
                    //"<class>125</class><weight>6000</weight></baseCommodity>",

                    //"<pieces>8</pieces><pieceType>PT</pieceType>",
                    //"<dimensions><length>48</length><width>48</width><height>70</height></dimensions>",
                    //"</commodity>",

                    sb_items,

                    "</fullCommodities>",
                    
                    "<accessorials>",

                    //"<accessorialCode>APT</accessorialCode>",

                    accessorials,

                    "</accessorials>",

                    "</rateRequest></soap:Body></soap:Envelope>");

                //DB.Log("Estes Volume request", data);

                Web_client http = new Web_client
                {
                    url = "https://www.estes-express.com/tools/rating/ratequote/v3.0/services/RateQuoteService",
                    content_type = "text/xml; charset=utf-8",
                    //accept = "*/*",
                    post_data = data,
                    method = "POST"
                };

                http.header_names = new string[1];
                http.header_names[0] = "SOAPAction";
                http.header_values = new string[1];
                http.header_values[0] = "\"http://ws.estesexpress.com/ratequote/getQuote\"";

                //DB.Log("gcmAPI_Get_LOUP_Rates before send request", "before send request");

                string doc = http.Make_http_request();


                //DB.Log("Estes Volume result", doc);

                string[] tokens = new string[3];
                tokens[0] = "<rat:quoteNumber>";
                tokens[1] = ">";
                tokens[2] = "<";

                string quoteNumber = HelperFuncs.scrapeFromPage(tokens, doc);

                List<Estes_price_res> list = new List<Estes_price_res>();
                int ind;
                while (doc.IndexOf("<rat:price>") != -1)
                {
                    ind = doc.IndexOf("<rat:price>");
                    doc = doc.Substring(ind + 1);

                    ind = doc.IndexOf("</rat:price>");
                    if (ind != -1)
                    {
                        Parse_one_estes_result(ref list, doc.Remove(ind), ref quoteNumber);
                    }
                    else
                    {
                        // Do nothing
                    }
                }

                double cost=0.0, cost_economy = 0.0, cost_basic = 0.0;
                //int transit_days;
                DateTime delDate=DateTime.MinValue, delDate_economy = DateTime.MinValue, 
                    delDate_basic = DateTime.MinValue;
                DateTime pickupDate = quoteData.puDate;

                for (byte i = 0;i<list.Count;i++)
                {
                    if (list[i].serviceLevel.Equals("Volume and Truckload Guaranteed Standard"))
                    {
                        if (list[i].standardPrice > 0)
                        {
                            cost = list[i].standardPrice;
                            delDate = list[i].deliveryDate;
                        }
                        else if (list[i].guaranteedPrice > 0)
                        {
                            cost = list[i].guaranteedPrice;
                            delDate = list[i].deliveryDate;
                        }
                        else
                        {
                            // Do nothing
                        }
                        //break;
                    }
                    else if (list[i].serviceLevel.Equals("Volume and Truckload Guaranteed Economy"))
                    {
                        if (list[i].standardPrice > 0)
                        {
                            cost_economy = list[i].standardPrice;
                            delDate_economy = list[i].deliveryDate;
                        }
                        else if (list[i].guaranteedPrice > 0)
                        {
                            cost_economy = list[i].guaranteedPrice;
                            delDate_economy = list[i].deliveryDate;
                        }
                        else
                        {
                            // Do nothing
                        }
                        //break;
                    }
                    else if (list[i].serviceLevel.Equals("Volume and Truckload Basic"))
                    {
                        if (list[i].standardPrice > 0)
                        {
                            cost_basic = list[i].standardPrice;
                            delDate_basic = list[i].deliveryDate;
                        }
                        else if (list[i].guaranteedPrice > 0)
                        {
                            cost_basic = list[i].guaranteedPrice;
                            delDate_basic = list[i].deliveryDate;
                        }
                        else
                        {
                            // Do nothing
                        }
                        //break;
                    }
                    else
                    {
                        // Do nothing
                    }
                }

                Volume_result volume_result = new Volume_result
                {
                    cost = cost,
                    scac = "EXLA",
                    carrier_name = "Estes Standard",
                    quote_number = quoteNumber,
                    transit_days = Convert.ToInt32((delDate - pickupDate).TotalDays)
                };

                estes_volume_economy_result.cost = cost_economy;
                estes_volume_economy_result.scac = "EXLA";
                estes_volume_economy_result.carrier_name = "Estes Economy";
                estes_volume_economy_result.quote_number = quoteNumber;
                estes_volume_economy_result.transit_days = Convert.ToInt32((delDate_economy - pickupDate).TotalDays);

                estes_volume_basic_result.cost = cost_basic;
                estes_volume_basic_result.scac = "EXLA";
                estes_volume_basic_result.carrier_name = "Estes Basic";
                estes_volume_basic_result.quote_number = quoteNumber;
                estes_volume_basic_result.transit_days = Convert.ToInt32((delDate_economy - pickupDate).TotalDays);

                if(delDate_economy == DateTime.MinValue)
                {
                    estes_volume_basic_result.transit_days = 10;
                }

                //DB.Log("Get_ESTES_volume_rates_xml", estes_volume_basic_result.transit_days.ToString());

                //DB.Log("Get_ESTES_volume_rates_xml delDate_economy", delDate_economy.ToShortDateString());

                //DB.Log("Get_ESTES_volume_rates_xml pickupDate", pickupDate.ToShortDateString());

                return volume_result;
            }
            catch (Exception e)
            {
                DB.Log("Get_ESTES_volume_rates_xml", e.ToString());
                Volume_result volume_result = new Volume_result();
                return volume_result;
            }
        }

        #endregion

        #region Parse_one_estes_result

        private void Parse_one_estes_result(ref List<Estes_price_res> list, string doc, ref string quoteNumber)
        {
            Estes_price_res res = new Estes_price_res();

            string[] tokens = new string[3];
            tokens[0] = "<rat:standardPrice>";
            tokens[1] = ">";
            tokens[2] = "<";

            double.TryParse(HelperFuncs.scrapeFromPage(tokens, doc), out res.standardPrice);

            tokens[0] = "<rat:guaranteedPrice>";
            double.TryParse(HelperFuncs.scrapeFromPage(tokens, doc), out res.guaranteedPrice);

            tokens[0] = "<rat:deliveryDate>";
            res.deliveryDate = DateTime.MinValue;
            DateTime.TryParse(HelperFuncs.scrapeFromPage(tokens, doc), out res.deliveryDate);

            tokens[0] = "<rat:serviceLevel>";
            res.serviceLevel = HelperFuncs.scrapeFromPage(tokens, doc);

            //tokens[0] = "<rat:code>";
            //res.code = HelperFuncs.scrapeFromPage(tokens, doc);

            res.quoteNumber = quoteNumber;

            list.Add(res);
        }

        #endregion

        public struct Estes_price_res
        {
            public double standardPrice, guaranteedPrice;
            public DateTime deliveryDate;
            public string serviceLevel, quoteNumber;//code, 
        }

        #region Get_ESTES_volume_rates

        // Not used, This SOAP function is not giving the correct rates, for an unknown reason..
        public Volume_result Get_ESTES_volume_rates()
        {
            #region Variables

            Logins.Login_info login_info;
            Logins logins = new Logins();
            logins.Get_login_info(111, out login_info);

            string username = login_info.username, password = login_info.password, account = login_info.account, payor = "T", terms = "PPD";
          
            //string origZip = "55317", origCity = "CHANHASSEN",
            //     origState = "MN", destZip = "22102", destCity = "MC LEAN", destState = "VA";

            //string destCountry = "US", origCountry = "US";

            //DateTime pickupDate = quoteData.puDate.AddDays(1);
            DateTime pickupDate = quoteData.puDate;
            bool hazmat = false;

            //List<string> fClasses = new List<string>();
            //List<string> weights = new List<string>();

            List<string> accessorials = new List<string>();

            #endregion

            try
            {

                RateQuoteService service = new RateQuoteService();

                string[] res = new string[4];

                // Define rateService object, makes the request to Estes API, accepts rateRequest object as parameter
                RateQuoteService rateServ = new RateQuoteService();

                //estesRatingService.RateQuoteType abc = new estesRatingService.RateQuoteType();
                //abc.

                rateQuote rateQuote = new rateQuote();  //define rateQuote object, response rate quote 

                rateRequest rateRequest = new rateRequest();  //define rateRequest object, stores iformation about the shipment

                PricingInfoType[] pricing = new PricingInfoType[1]; //define pricing object stores cost and delivery date and other fields 

                PickupType pickupType = new PickupType
                {
                    date = pickupDate
                };

                #region Not used LTL class and weight
                //estesRatingService.BaseCommoditiesType baseCommodities = new estesRatingService.BaseCommoditiesType();

                //estesRatingService.BaseCommodityType[] commodTypeArray = new estesRatingService.BaseCommodityType[weights.Count];
                //for (int i = 0; i < weights.Count; i++)
                //{
                //    estesRatingService.BaseCommodityType commodType = new estesRatingService.BaseCommodityType(); //object that holds one class/weight pair
                //    commodType.@class = Convert.ToDecimal(fClasses[i]);
                //    commodType.weight = weights[i];
                //    commodTypeArray[i] = commodType; //set class/weight to an item in array or classes/weights
                //}

                //baseCommodities.commodity = commodTypeArray; //set the classes/weights array to the commodity object
                //rateRequest.Item = baseCommodities;
                #endregion

                //

                FullCommoditiesType fullCommodities = new FullCommoditiesType();
                FullCommodityType[] commodTypeArray = new FullCommodityType[quoteData.m_lPiece.Length];
                for (int i = 0; i < quoteData.m_lPiece.Length; i++)
                {
                    // Object that holds one class/weight pair
                    FullCommodityType commodType = new FullCommodityType();
                    BaseCommodityType base_commodity = new BaseCommodityType
                    {
                        weight = quoteData.m_lPiece[i].Weight.ToString(),
                        @class = Convert.ToDecimal(quoteData.m_lPiece[i].FreightClass)
                    };
                    commodType.baseCommodity = base_commodity;

                    //commodType.@class = Convert.ToDecimal(fClasses[i]);
                    //commodType.weight = weights[i];
                    commodType.dimensions = new DimensionsType
                    {
                        length = "48",
                        width = "48",
                        height = "70"
                    };
                    DB.Log("Get_ESTES_volume_rates quoteData.m_lPiece[i].Quantity", quoteData.m_lPiece[i].Quantity.ToString());

                    commodType.pieces = quoteData.m_lPiece[i].Quantity.ToString();
                    commodType.pieceType = new PackagingType();
                    //commodType.pieceType = estesRatingService.PackagingType.CR;
                    //commodType.pieceType = estesRatingService.PackagingType.CT;
                    commodType.pieceType = PackagingType.PT;
                    commodTypeArray[i] = commodType; //set class/weight to an item in array or classes/weights
                }

                fullCommodities.commodity = commodTypeArray; //set the classes/weights array to the commodity object
                rateRequest.Item = fullCommodities;

                rateRequest.equipmentType = "TRAILER";

                #region Origin and destination

                DB.Log("quoteData.origCountry", quoteData.origCountry);
                DB.Log("quoteData.destCountry", quoteData.destCountry);

                rateRequest.originPoint = new PointType
                {
                    countryCode = quoteData.origCountry,
                    postalCode = quoteData.origZip,
                    city = quoteData.origCity,
                    stateProvince = quoteData.origState
                };

                rateRequest.destinationPoint = new PointType
                {
                    countryCode = quoteData.destCountry,
                    postalCode = quoteData.destZip,
                    city = quoteData.destCity,
                    stateProvince = quoteData.destState
                };

                #endregion

                #region Accessorials

                //accessorials.Add("LGATE");
                accessorials.Add("APT");

                if (accessorials.Count == 0)
                    rateRequest.accessorials = null;
                else
                {
                    rateRequest.accessorials = new string[accessorials.Count];
                    for (int i = 0; i < accessorials.Count; i++)
                    {
                        rateRequest.accessorials[i] = accessorials[i];
                        //DB.Log("Estes acc live", accessorials[i], "");
                    }
                }

                #endregion

                rateRequest.requestID = " ";
                rateRequest.account = account;

                rateRequest.payor = payor;
                rateRequest.terms = terms;
                rateRequest.pickup = pickupType;
                //rateRequest.liability = "1";
                //rateRequest.declaredValue = 100;
                //rateRequest.declaredValueSpecified = true;

                #region Hazmat and stackable
                //DB.Log("Estes", hazmat.ToString(), "");
                rateRequest.hazmatSpecified = true;
                if (hazmat)
                    rateRequest.hazmat = YesNoBlankType.Y;
                else
                    rateRequest.hazmat = YesNoBlankType.N;

                //rateRequest.stackableSpecified = true; //to do ask Bob about this
                //rateRequest.stackable = estesRatingService.YesNoBlankType.Y;
                #endregion

                rateRequest.requestID = "123";

                rateServ.auth = new AuthenticationType
                {
                    user = username,
                    password = password
                };

                // Make the API call
                rateQuote = rateServ.getQuote(rateRequest);

                pricing = rateQuote.quote.pricing;
                res[1] = pricing[0].standardPrice.ToString();

                DB.Log("Get_ESTES_volume_rates", pricing[0].guaranteedPrice.ToString());


                //pricing[0].
                //double totalCharges;
                //if (!double.TryParse(res[1], out totalCharges))
                //{
                //    res[1] = rateQuote.
                //}

                DateTime delDate = pricing[0].deliveryDate;

                res[2] = (delDate - pickupDate).TotalDays.ToString();

                res[0] = "success";

                Volume_result volume_result = new Volume_result
                {
                    cost = (double)pricing[0].guaranteedPrice,
                    carrier_name = "Estes",
                    quote_number = "",
                    transit_days = Convert.ToInt32((delDate - pickupDate).TotalDays)
                };

                return volume_result;

            }
            catch (Exception e)
            {
                DB.Log("Get_ESTES_volume_rates", e.ToString());
                Volume_result volume_result = new Volume_result();
                return volume_result;
            }
        }

        public struct Volume_result
        {
            public double cost;
            public int transit_days;
            public string carrier_name, quote_number, scac;
        }

        #endregion

        #endregion

        #region GetResultObjectFromEstesExpress

        public void GetResultObjectFromEstesExpress(out GCMRateQuote estesQuote)
        {
            estesQuote = new GCMRateQuote();

            #region Variables

            List<string> accessorsList = new List<string>();

            string origCountry = "US";
            string destCountry = "US";
            bool long20 = false, long28 = false, long53 = false;

            #endregion

            try
            {

                #region Check if Canada
                
                if (quoteData.origCountry.Equals("CANADA"))
                {
                    origCountry = "CN";
                }

                if (quoteData.destCountry.Equals("CANADA"))
                {
                    destCountry = "CN";
                }
                
                #endregion

                #region Overlength

                for (int i = 0; i < quoteData.m_lPiece.Length; i++)
                {
                    //-----------------------------------------------
                    // Length
                    if (quoteData.m_lPiece[i].Length > 336)
                    {
                        long53 = true;
                        break;
                    }
                    else if (quoteData.m_lPiece[i].Length > 239)
                    {
                        long28 = true;
                    }
                    else if (quoteData.m_lPiece[i].Length > 144)
                    {
                        long20 = true;
                    }

                    // Width
                    if (quoteData.m_lPiece[i].Width > 336)
                    {
                        long53 = true;
                        break;
                    }
                    else if (quoteData.m_lPiece[i].Width > 239)
                    {
                        long28 = true;
                    }
                    else if (quoteData.m_lPiece[i].Width > 144)
                    {
                        long20 = true;
                    }

                    // Height
                    if (quoteData.m_lPiece[i].Height > 336)
                    {
                        long53 = true;
                        break;
                    }
                    else if (quoteData.m_lPiece[i].Height > 239)
                    {
                        long28 = true;
                    }
                    else if (quoteData.m_lPiece[i].Height > 144)
                    {
                        long20 = true;
                    }
                    //-----------------------------------------------
                }

                if (long53)
                {
                    accessorsList.Add("LONG53");
                }
                else if (long28)
                {
                    accessorsList.Add("LONG28");
                }
                else if (long20)
                {
                    accessorsList.Add("LONG20");
                }

                #endregion

                #region Accessorials

                if (quoteData.AccessorialsObj.LGPU)
                {
                    accessorsList.Add("LGATEP");
                }
                if (quoteData.AccessorialsObj.LGDEL)
                {
                    accessorsList.Add("LGATE");
                }
                if (quoteData.AccessorialsObj.RESPU)
                {
                    accessorsList.Add("HD");
                }
                if (quoteData.AccessorialsObj.RESDEL)
                {
                    accessorsList.Add("HD");
                }
                if (quoteData.AccessorialsObj.CONPU)
                {
                    accessorsList.Add("LADPU");
                }
                if (quoteData.AccessorialsObj.CONDEL)
                {
                    accessorsList.Add("CONST");
                }
                if (quoteData.AccessorialsObj.APTPU || quoteData.AccessorialsObj.APTDEL)
                {
                    accessorsList.Add("APT");
                }
                //if (AccessorialsObj.APTDEL)
                //{
                //    accessorsList.Add("APTDEL");
                //}
                if (quoteData.AccessorialsObj.TRADEPU)
                {
                    accessorsList.Add("FAIRPU");
                }
                if (quoteData.AccessorialsObj.TRADEDEL)
                {
                    accessorsList.Add("FAIRDL");
                }
                if (quoteData.AccessorialsObj.INSDEL)
                {
                    accessorsList.Add("INS");
                }

                #endregion

                getEstesAPI_Rate(origCountry, destCountry, accessorsList, ref estesQuote);

                #region Subdomains additions
                //if (quoteData.subdomain.Equals("spc") || isCostPlus)
                //{
                //    totalCharges = HelperFuncs.addSPC_Addition(totalCharges);
                //}

                //if (quoteData.subdomain.Equals(HelperFuncs.Subdomains.clipper))
                //{
                //    totalCharges = HelperFuncs.addClipperSubdomain_Addition(totalCharges);
                //}
                #endregion

            }
            catch (Exception e)
            {
                DB.LogGenera("Estes", "Exception", e.ToString());
            }
        }

        #endregion

        #region getEstesAPI_Rate

        public void getEstesAPI_Rate(string origCountry, string destCountry,
                List<string> accessorials, ref GCMRateQuote estesQuote)
        {

            //string[] res = new string[4];

            //define rateService object, makes the request to Estes API, accepts rateRequest object as parameter
            gcmAPI.Estes_RateWebServ.RateQuoteService rateServ = new gcmAPI.Estes_RateWebServ.RateQuoteService();

            gcmAPI.Estes_RateWebServ.rateQuote rateQuote = new gcmAPI.Estes_RateWebServ.rateQuote();  //define rateQuote object, response rate quote 

            gcmAPI.Estes_RateWebServ.rateRequest rateRequest = new gcmAPI.Estes_RateWebServ.rateRequest();  //define rateRequest object, stores iformation about the shipment

            gcmAPI.Estes_RateWebServ.PricingInfoType[] pricing = new gcmAPI.Estes_RateWebServ.PricingInfoType[1]; //define pricing object stores cost and delivery date and other fields 

            gcmAPI.Estes_RateWebServ.PickupType pickupType = new gcmAPI.Estes_RateWebServ.PickupType(); //pickup date
            pickupType.date = quoteData.puDate;

            StringBuilder sb = new StringBuilder();

            #region Class and weight

            gcmAPI.Estes_RateWebServ.BaseCommoditiesType baseCommodities = new gcmAPI.Estes_RateWebServ.BaseCommoditiesType();

            gcmAPI.Estes_RateWebServ.BaseCommodityType[] commodTypeArray = new gcmAPI.Estes_RateWebServ.BaseCommodityType[quoteData.m_lPiece.Length];
            //for (int i = 0; i < weights.Count; i++)
            //{
            //    gcmAPI.Estes_RateWebServ.BaseCommodityType commodType = new gcmAPI.Estes_RateWebServ.BaseCommodityType(); //object that holds one class/weight pair
            //    commodType.@class = Convert.ToDecimal(fClasses[i]);
            //    commodType.weight = weights[i];
            //    commodTypeArray[i] = commodType; //set class/weight to an item in array or classes/weights
            //}

            for (int i = 0; i < quoteData.m_lPiece.Length; i++)
            {
                //DB.LogGenera("ESTES", "class", quoteData.m_lPiece[i].FreightClass);
                //DB.LogGenera("ESTES", "weight", quoteData.m_lPiece[i].Weight.ToString());

                gcmAPI.Estes_RateWebServ.BaseCommodityType commodType = new gcmAPI.Estes_RateWebServ.BaseCommodityType(); //object that holds one class/weight pair
                commodType.@class = Convert.ToDecimal(quoteData.m_lPiece[i].FreightClass);
                //commodType.weight = quoteData.m_lPiece[i].Weight.ToString();
                commodType.weight = Convert.ToInt32(quoteData.m_lPiece[i].Weight).ToString();
                commodTypeArray[i] = commodType; //set class/weight to an item in array or classes/weights

                //DB.LogGenera("ESTES", "weight fixed", commodType.weight);
            }

            baseCommodities.commodity = commodTypeArray; //set the classes/weights array to the commodity object
            rateRequest.Item = baseCommodities;

            #endregion

            #region Origin and destination

            rateRequest.originPoint = new gcmAPI.Estes_RateWebServ.PointType();
            rateRequest.originPoint.countryCode = origCountry;
            rateRequest.originPoint.postalCode = quoteData.origZip;
            rateRequest.originPoint.city = quoteData.origCity;
            rateRequest.originPoint.stateProvince = quoteData.origState;

            rateRequest.destinationPoint = new gcmAPI.Estes_RateWebServ.PointType();
            rateRequest.destinationPoint.countryCode = destCountry;
            rateRequest.destinationPoint.postalCode = quoteData.destZip;
            rateRequest.destinationPoint.city = quoteData.destCity;
            rateRequest.destinationPoint.stateProvince = quoteData.destState;

            if(quoteData.origCountry == "CANADA")
            {
                rateRequest.originPoint.postalCode = quoteData.orig_zip_Canada_no_space;
            }
            else
            {
                //rateRequest.destinationPoint.countryCode = destCountry;
            }

            if (quoteData.destCountry == "CANADA")
            {
                rateRequest.destinationPoint.postalCode = quoteData.dest_zip_Canada_no_space;
            }
            else
            {
                rateRequest.destinationPoint.countryCode = destCountry;
            }

            sb.Append(
                string.Concat(
                    rateRequest.originPoint.city, ", ",
                    rateRequest.originPoint.stateProvince, " ",
                    rateRequest.originPoint.postalCode, " ",
                    rateRequest.originPoint.countryCode, " "
                    )
                    );

            sb.Append(
                string.Concat(
                    rateRequest.destinationPoint.city, ", ",
                    rateRequest.destinationPoint.stateProvince, " ",
                    rateRequest.destinationPoint.postalCode, " ",
                    rateRequest.destinationPoint.countryCode
                    )
                    );

            #endregion

            //DB.LogGenera("Estes", "request info", sb.ToString());

            #region Accessorials

            if (accessorials == null || accessorials.Count == 0)
                rateRequest.accessorials = null;
            else
            {
                rateRequest.accessorials = new string[accessorials.Count];
                for (int i = 0; i < accessorials.Count; i++)
                {
                    rateRequest.accessorials[i] = accessorials[i];
                    //HelperFuncs.writeToSiteErrors("Estes acc live", accessorials[i], "");
                }
            }

            #endregion

            rateRequest.requestID = " ";
            rateRequest.account = acctInfo.acctNum;

            rateRequest.payor = acctInfo.chargeType;
            rateRequest.terms = acctInfo.terms;
            rateRequest.pickup = pickupType;
            rateRequest.liability = "1";
            rateRequest.declaredValue = 100;
            rateRequest.declaredValueSpecified = true;

            #region Hazmat and stackable

            //HelperFuncs.writeToSiteErrors("Estes", hazmat.ToString(), "");
            rateRequest.hazmatSpecified = true;
            if (quoteData.isHazmat)
                rateRequest.hazmat = gcmAPI.Estes_RateWebServ.YesNoBlankType.Y;
            else
                rateRequest.hazmat = gcmAPI.Estes_RateWebServ.YesNoBlankType.N;

            rateRequest.stackableSpecified = true; //to do ask Bob about this
            rateRequest.stackable = gcmAPI.Estes_RateWebServ.YesNoBlankType.Y;

            #endregion

            rateServ.auth = new gcmAPI.Estes_RateWebServ.AuthenticationType();
            rateServ.auth.user = acctInfo.username;
            rateServ.auth.password = acctInfo.password;

            // Make the API call
            rateQuote = rateServ.getQuote(rateRequest);

            pricing = rateQuote.quote.pricing;
            
            DateTime delDate = pricing[0].deliveryDate;
            
            decimal discount = pricing[0].totalDiscount;
            //decimal fuel = pricing[0].accessorialInfo[0].charge;

            decimal accessorial_charges = 0m;
            for (byte i = 0; i < pricing[0].accessorialInfo.Length; i++)
            {
                accessorial_charges += pricing[0].accessorialInfo[i].charge;
            }

            Estes_RateWebServ.CommodityInfoType[] commodity =
                new Estes_RateWebServ.CommodityInfoType[rateQuote.quote.commodityInfo.Length];
            commodity = rateQuote.quote.commodityInfo;

            //decimal charge = commodity[0].charge;
            decimal charge = 0m;
            for (byte i = 0; i < commodity.Length; i++)
            {
                charge += commodity[i].charge;
            }


            decimal rate = commodity[0].rate;

            DB.LogGenera("Estes", "charge", charge.ToString());

            decimal buy_rate = charge - discount + accessorial_charges;
            DB.LogGenera("Estes", "discount", discount.ToString());
            //DB.LogGenera("Estes", "fuel", fuel.ToString());
            //DB.LogGenera("Estes", "charge", buy_rate.ToString());

            //DB.LogGenera("Estes", "buy rate", buy_rate.ToString());

            decimal TotalPrice = charge + accessorial_charges - discount;
            //rate +
            if (buy_rate > 0)
            {
                //DB.LogGenera("Estes", "got price", pricing[0].standardPrice.ToString());

                //estesQuote.TotalPrice = (double)buy_rate;

                estesQuote.TotalPrice = (double)TotalPrice;
                estesQuote.Scac = "EXLA";
                estesQuote.DeliveryDays = Convert.ToInt32((delDate - quoteData.puDate).TotalDays);
                estesQuote.DisplayName = "Estes Express - Genera";
                estesQuote.CarrierKey = "Estes";
                estesQuote.BookingKey = "#1#";

                //

                // Check for hazmat
                if (quoteData.isHazmat)
                {
                    estesQuote.TotalPrice += 23;
                }

                // liftgate pickup and delivery together addition (when both are present only one charge is added by Estes for some reason)
                if (quoteData.AccessorialsObj.LGPU && quoteData.AccessorialsObj.LGDEL)
                {
                    estesQuote.TotalPrice += 10;
                }
            }
            else
            {
                DB.LogGenera("Estes", "NO price", "");
            }
        }

        #endregion

        #region Add_result_to_array

        public void Add_result_to_array(ref GCMRateQuote estesQuote, ref GCMRateQuote[] totalQuotes)
        {
            totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, estesQuote);
        }

        #endregion

        #region Not used getEstesAPI_Rate


        #region Accounts switch 1

        //objEstesExpressResultOnline = SetInfoToObjectQuote(ref totalCharges, DisplayName, "#1#", CarrierKey, "", transitTime, "Estes");

        //if (quoteData.subdomain.Equals("spc")) // Account specific login
        //{
        //    //objEstesExpressResultSPC = objQuote;
        //    //HelperFuncs.writeToSiteErrors("Estes " + quoteData.username.ToLower(), "spc", "");

        //    objEstesExpressResultSPC = SetInfoToObjectQuote(ref totalCharges, DisplayName, "#1#", CarrierKey, "", transitTime, "Estes");

        //}
        //else if (quoteData.subdomain.Equals(HelperFuncs.Subdomains.mwi)) //account specific login
        //{
        //    //objEstesExpressResultMWI = objQuote;
        //    objEstesExpressResultMWI = SetInfoToObjectQuote(ref totalCharges, DisplayName, "#1#", CarrierKey, "", transitTime, "Estes");
        //}
        //else if (quoteData.subdomain.Equals(HelperFuncs.Subdomains.allmodes)) //account specific login
        //{
        //    //objEstesExpressResultAllmodes = objQuote;
        //    objEstesExpressResultAllmodes = SetInfoToObjectQuote(ref totalCharges, DisplayName, "#1#", CarrierKey, "", transitTime, "Estes");
        //}
        //else
        //{
        //    //objEstesExpressResultOnline = objQuote;
        //    objEstesExpressResultOnline = SetInfoToObjectQuote(ref totalCharges, DisplayName, "#1#", CarrierKey, "", transitTime, "Estes");
        //    //HelperFuncs.writeToSiteErrors("Estes " + quoteData.username.ToLower(), "regular", "");
        //}
        #endregion

        #region Accounts switch 2
        //if (quoteData.subdomain.Equals("spc")) //account specific login
        //{
        //    objEstesExpressResultSPC = null;
        //}
        //else if (quoteData.subdomain.Equals(HelperFuncs.Subdomains.mwi)) //account specific login
        //{
        //    objEstesExpressResultMWI = null;
        //}
        //else if (quoteData.subdomain.Equals(HelperFuncs.Subdomains.allmodes)) //account specific login
        //{
        //    objEstesExpressResultAllmodes = null;
        //}
        //else
        //    objEstesExpressResultOnline = null;
        #endregion

        
        //GCMRateQuote objQuote = new GCMRateQuote();
        //string DisplayName = "Estes Express";
        //objQuote.BookingKey = "#1#";
        //string CarrierKey = "EstesOnline";
        
        //public static string[] getEstesAPI_Rate(string username, string password, string account, string origZip, string origCity, string origState, string origCountry,
        //        string destZip, string destCity, string destState, string destCountry, string payor, string terms, DateTime pickupDate, bool hazmat, List<string> fClasses,
        //        List<string> weights, List<string> accessorials)
        //{
        //    //HelperFuncs.writeToSiteErrors("Estes", "origZip: " + origZip + " origCity: " + origCity + " origState: " + origState + " origCountry: " + origCountry + 
        //    //    " destZip: " + destZip + " destCity: " + destCity + " destState: " + destState + " destCountry: " + destCountry + " puDate: " + puDate.ToString() + " class: " + fClasses[0] +
        //    //    " weight: " + weight[0], "");

        //    string[] res = new string[4];

        //    //define rateService object, makes the request to Estes API, accepts rateRequest object as parameter
        //    gcmAPI.Estes_RateWebServ.RateQuoteService rateServ = new gcmAPI.Estes_RateWebServ.RateQuoteService();

        //    gcmAPI.Estes_RateWebServ.rateQuote rateQuote = new gcmAPI.Estes_RateWebServ.rateQuote();  //define rateQuote object, response rate quote 

        //    gcmAPI.Estes_RateWebServ.rateRequest rateRequest = new gcmAPI.Estes_RateWebServ.rateRequest();  //define rateRequest object, stores iformation about the shipment

        //    gcmAPI.Estes_RateWebServ.PricingInfoType[] pricing = new gcmAPI.Estes_RateWebServ.PricingInfoType[1]; //define pricing object stores cost and delivery date and other fields 

        //    gcmAPI.Estes_RateWebServ.PickupType pickupType = new gcmAPI.Estes_RateWebServ.PickupType(); //pickup date
        //    pickupType.date = pickupDate;

        //    #region class and weight
        //    gcmAPI.Estes_RateWebServ.BaseCommoditiesType baseCommodities = new gcmAPI.Estes_RateWebServ.BaseCommoditiesType();

        //    gcmAPI.Estes_RateWebServ.BaseCommodityType[] commodTypeArray = new gcmAPI.Estes_RateWebServ.BaseCommodityType[weights.Count];
        //    for (int i = 0; i < weights.Count; i++)
        //    {
        //        gcmAPI.Estes_RateWebServ.BaseCommodityType commodType = new gcmAPI.Estes_RateWebServ.BaseCommodityType(); //object that holds one class/weight pair
        //        commodType.@class = Convert.ToDecimal(fClasses[i]);
        //        commodType.weight = weights[i];
        //        commodTypeArray[i] = commodType; //set class/weight to an item in array or classes/weights
        //    }

        //    baseCommodities.commodity = commodTypeArray; //set the classes/weights array to the commodity object
        //    rateRequest.Item = baseCommodities;
        //    #endregion

        //    #region Origin and destination

        //    rateRequest.originPoint = new gcmAPI.Estes_RateWebServ.PointType();
        //    rateRequest.originPoint.countryCode = origCountry;
        //    rateRequest.originPoint.postalCode = origZip;
        //    rateRequest.originPoint.city = origCity;
        //    rateRequest.originPoint.stateProvince = origState;

        //    rateRequest.destinationPoint = new gcmAPI.Estes_RateWebServ.PointType();
        //    rateRequest.destinationPoint.countryCode = destCountry;
        //    rateRequest.destinationPoint.postalCode = destZip;
        //    rateRequest.destinationPoint.city = destCity;
        //    rateRequest.destinationPoint.stateProvince = destState;

        //    #endregion

        //    #region Accessorials

        //    if (accessorials.Count == 0)
        //        rateRequest.accessorials = null;
        //    else
        //    {
        //        rateRequest.accessorials = new string[accessorials.Count];
        //        for (int i = 0; i < accessorials.Count; i++)
        //        {
        //            rateRequest.accessorials[i] = accessorials[i];
        //            //HelperFuncs.writeToSiteErrors("Estes acc live", accessorials[i], "");
        //        }
        //    }

        //    #endregion

        //    rateRequest.requestID = " ";
        //    rateRequest.account = account;

        //    rateRequest.payor = payor;
        //    rateRequest.terms = terms;
        //    rateRequest.pickup = pickupType;
        //    rateRequest.liability = "1";
        //    rateRequest.declaredValue = 100;
        //    rateRequest.declaredValueSpecified = true;

        //    #region Hazmat and stackable
        //    //HelperFuncs.writeToSiteErrors("Estes", hazmat.ToString(), "");
        //    rateRequest.hazmatSpecified = true;
        //    if (hazmat)
        //        rateRequest.hazmat = gcmAPI.Estes_RateWebServ.YesNoBlankType.Y;
        //    else
        //        rateRequest.hazmat = gcmAPI.Estes_RateWebServ.YesNoBlankType.N;

        //    rateRequest.stackableSpecified = true; //to do ask Bob about this
        //    rateRequest.stackable = gcmAPI.Estes_RateWebServ.YesNoBlankType.Y;
        //    #endregion

        //    rateServ.auth = new gcmAPI.Estes_RateWebServ.AuthenticationType();
        //    rateServ.auth.user = username;
        //    rateServ.auth.password = password;

        //    // Make the API call
        //    rateQuote = rateServ.getQuote(rateRequest);

        //    pricing = rateQuote.quote.pricing;
        //    res[1] = pricing[0].standardPrice.ToString();

        //    //double totalCharges;
        //    //if (!double.TryParse(res[1], out totalCharges))
        //    //{
        //    //    res[1] = rateQuote.
        //    //}

        //    DateTime delDate = pricing[0].deliveryDate;

        //    res[2] = (delDate - pickupDate).TotalDays.ToString();

        //    res[0] = "success";

        //    return res;
        //}

        #endregion

    }
}