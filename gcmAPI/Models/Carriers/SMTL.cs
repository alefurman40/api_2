#region Using

using gcmAPI.Models.LTL;
using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

#endregion

namespace gcmAPI.Models.Carriers
{
    public class SMTL
    {
        QuoteData quoteData;

        #region Constructor
        // Constructor
        public SMTL(ref QuoteData quoteData)
        {

            this.quoteData = quoteData;

        }

        #endregion

        #region Get_rates
        public void Get_rates(out GCMRateQuote SMTL_Quote_Genera)
        {
            Random rnd = new Random();
            int sess_1 = rnd.Next(10000000, 99999999);
            int sess_2 = rnd.Next(10000000, 99999999);
            try
            {
                string data = string.Concat("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>",
                "<SMTLRateRequest>",
                "<SMTLIam>Genera_api</SMTLIam>",
                "<Token>", AppCodeConstants.smtl_genera_token, "</Token>",
                "<Option>Rate</Option>",
                "<SessionId>", sess_1, sess_2, "</SessionId>",

                "<FromCity>", quoteData.origCity, "</FromCity>",
                "<FromState>", quoteData.origState, "</FromState>",
                "<FromZip>", quoteData.origZip, "</FromZip>",

                "<ToCity>", quoteData.destCity, "</ToCity>",
                "<ToState>", quoteData.destState, "</ToState>",
                "<ToZip>", quoteData.destZip, "</ToZip>",

                //"<FromCity>San Antonio</FromCity>",
                //"<FromState>TX</FromState>",
                //"<FromZip>78218</FromZip>",

                //"<ToCity>Tulsa</ToCity>",
                //"<ToState>OK</ToState>",
                //"<ToZip>74115</ToZip>",

                //"<ShipDate>11/01/2019</ShipDate>",
                "<ShipDate>", quoteData.puDate.ToShortDateString(), "</ShipDate>",
                "<Terms>P</Terms>",

                Get_items(),

                //"<Class01>70</Class01><Piece01>3</Piece01>",
                //"<Packaging01>SK</Packaging01>",
                //"<Weight01>500</Weight01>",
                //"<Class02>100</Class02><Piece02>5</Piece02>",
                //"<Packaging02>SK</Packaging02><Weight02>1000</Weight02>",
                //"<Class03>65</Class03><Piece03>15</Piece03><Packaging03>SK</Packaging03><Weight03>750</Weight03>",
                //"<Class04>125</Class04><Piece04>6</Piece04><Packaging04>SK</Packaging04><Weight04>600</Weight04>",
                //"<Class05>200</Class05><Piece05>8</Piece05><Packaging05>SK</Packaging05><Weight05>1500</Weight05>",

                //"<Class01>70</Class01><Piece01>3</Piece01>",
                //"<Packaging01>SK</Packaging01>",
                //"<Weight01>500</Weight01><Class02>100</Class02><Piece02>5</Piece02>",
                //"<Packaging02>SK</Packaging02><Weight02>1000</Weight02>",
                //"<Class03>65</Class03><Piece03>15</Piece03><Packaging03>SK</Packaging03><Weight03>750</Weight03>",
                //"<Class04>125</Class04><Piece04>6</Piece04><Packaging04>SK</Packaging04><Weight04>600</Weight04>",
                //"<Class05>200</Class05><Piece05>8</Piece05><Packaging05>SK</Packaging05><Weight05>1500</Weight05>",
                "<LiabilityCoverage>25000</LiabilityCoverage>",
                "<WingExpress>N</WingExpress>",
                Get_accessorials(),
                //"<Accessorial01></Accessorial01>",
                //"<Accessorial02></Accessorial02>",
                //"<Accessorial03></Accessorial03>",
                //"<Accessorial04></Accessorial04>",
                //"<Accessorial05></Accessorial05>",
                //"<Accessorial01>Haz</Accessorial01>",
                //"<Accessorial02>Const</Accessorial02>",
                //"<Accessorial03>Liftp</Accessorial03>",
                //"<Accessorial04>Liftd</Accessorial04>",
                //"<Accessorial05>Ib</Accessorial05>",
                "<ShipperName>ORouke</ShipperName><ShipperAddress>1875 Water Ridge</ShipperAddress>",
                "<ReadyTime>1400</ReadyTime><CloseTime>1700</CloseTime><Phone>2106623272</Phone><Contact>Randy</Contact>",
                "<PUInstruction1></PUInstruction1><PUInstruction2></PUInstruction2>",
                "</SMTLRateRequest>");

                //DB.Log("SMTL data", data);

                Web_client http = new Web_client();


                http.url = string.Concat("http://www2.smtl.com/rpgsp/SMTLRATES.pgm?Request=", data,
                    "&SessionId=", sess_1, sess_2);


                http.content_type = "text/xml,application/xml";
                http.method = "GET";

                http.accept = "text/xml,application/xml";

                string doc = http.Make_http_request();

                //DB.Log("SMTL doc", doc);

                #region Parse result

                string[] tokens = new string[3];
                tokens[0] = "<FreightCharges>";
                tokens[1] = ">";
                tokens[2] = "<";

                string FreightCharges_str = HelperFuncs.scrapeFromPage(tokens, doc);

                tokens[0] = "<DiscountAmount>";
                string DiscountAmount_str = HelperFuncs.scrapeFromPage(tokens, doc);

                tokens[0] = "<FuelSurchargeAmount>";
                string FuelSurchargeAmount_str = HelperFuncs.scrapeFromPage(tokens, doc);

                decimal TotalCharges=0.0M, FreightCharges, DiscountAmount, FuelSurchargeAmount;

                if (decimal.TryParse(FreightCharges_str, out FreightCharges) &&
                    decimal.TryParse(DiscountAmount_str, out DiscountAmount) &&
                    decimal.TryParse(FuelSurchargeAmount_str, out FuelSurchargeAmount))
                {
                    if (FreightCharges > 0M && DiscountAmount > 0M && FuelSurchargeAmount > 0M)
                    {
                        TotalCharges = FreightCharges - DiscountAmount + FuelSurchargeAmount;
                    }
                }

                tokens[0] = "<PickupDate>";
                string PickupDate_str = HelperFuncs.scrapeFromPage(tokens, doc);

                tokens[0] = "<DeliveryDate>";
                string DeliveryDate_str = HelperFuncs.scrapeFromPage(tokens, doc);

                DateTime PickupDate, DeliveryDate;

                byte ServiceDays = 0;
                if (DateTime.TryParse(PickupDate_str, out PickupDate) &&
                    DateTime.TryParse(DeliveryDate_str, out DeliveryDate))
                {
                    for (byte i = 0; i < 15; i++)
                    {
                        if (PickupDate == DeliveryDate)
                        {
                            break;
                        }
                        if (PickupDate.DayOfWeek == DayOfWeek.Saturday || PickupDate.DayOfWeek == DayOfWeek.Sunday)
                        {

                        }
                        else
                        {
                            ServiceDays++;
                        }

                        PickupDate = PickupDate.AddDays(1);
                    }
                }

                #endregion

                #region Set result

                SMTL_Quote_Genera = new GCMRateQuote();

                if (TotalCharges > 0.0M)
                {
                    SMTL_Quote_Genera.TotalPrice = (double)TotalCharges;
                    if(ServiceDays>0)
                    {
                        SMTL_Quote_Genera.DeliveryDays = ServiceDays;
                    }
                    else
                    {
                        SMTL_Quote_Genera.DeliveryDays = 10;
                    }
                    
                    SMTL_Quote_Genera.DisplayName = "SMTL - Genera";
                    SMTL_Quote_Genera.CarrierKey = "UPS";
                    SMTL_Quote_Genera.BookingKey = "#1#";
                    SMTL_Quote_Genera.Scac = "SMTL";
                }

                #endregion
            }
            catch(Exception e)
            {
                SMTL_Quote_Genera = new GCMRateQuote();
                DB.LogGenera("SMTL", "exception", e.ToString());
            }
        }

        #endregion

        #region Get_items
        private string Get_items()
        {

            StringBuilder sb = new StringBuilder();

            string one_based_i = "";
            int pieces = 1;
            for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
            {
                if (quoteData.m_lPiece[i].Units > 0)
                {
                    pieces = quoteData.m_lPiece[i].Units;
                }
                else if (quoteData.m_lPiece[i].Pieces > 0)
                {
                    pieces = quoteData.m_lPiece[i].Pieces;
                }
                else
                {
                    pieces = 1;
                }
                one_based_i = (i + 1).ToString();
                //sb.Append(string.Concat("<Class0", one_based_i, ">", quoteData.m_lPiece[i].FreightClass,
                //    "</Class0", one_based_i, "><Piece0", one_based_i, ">", pieces, "</Piece0", one_based_i, ">",
                //"<Packaging0", one_based_i, ">SK</Packaging0", one_based_i, ">",
                //"<Weight0", one_based_i, ">", quoteData.m_lPiece[i].Weight, "</Weight0", one_based_i, ">"));

                sb.Append(string.Concat("<Class0", one_based_i, ">", "60",
                      "</Class0", one_based_i, "><Piece0", one_based_i, ">", pieces, "</Piece0", one_based_i, ">",
                  "<Packaging0", one_based_i, ">SK</Packaging0", one_based_i, ">",
                  "<Weight0", one_based_i, ">", quoteData.m_lPiece[i].Weight, "</Weight0", one_based_i, ">"));

            }

            return sb.ToString();

        }

        #endregion

        #region Get_accessorials
        private string Get_accessorials()
        {
            if (quoteData.hasAccessorials == false && quoteData.isHazmat == false)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            byte acc_num = 0;
            if (quoteData.AccessorialsObj.RESPU)
            {
                acc_num++;
                sb.Append(string.Concat("<Accessorial0", acc_num, ">RESIP</Accessorial0", acc_num, ">"));
            }
            if (quoteData.AccessorialsObj.RESDEL)
            {
                acc_num++;
                sb.Append(string.Concat("<Accessorial0", acc_num, ">RESID</Accessorial0", acc_num, ">"));
            }
            if (quoteData.AccessorialsObj.LGPU)
            {
                acc_num++;
                sb.Append(string.Concat("<Accessorial0", acc_num, ">LIFTP</Accessorial0", acc_num, ">"));
            }
            if (quoteData.AccessorialsObj.LGDEL)
            {
                acc_num++;
                sb.Append(string.Concat("<Accessorial0", acc_num, ">LIFTD</Accessorial0", acc_num, ">"));
            }
            if (quoteData.AccessorialsObj.CONPU)
            {
                acc_num++;
                sb.Append(string.Concat("<Accessorial0", acc_num, ">LIMITP</Accessorial0", acc_num, ">"));
            }
            if (quoteData.AccessorialsObj.CONDEL)
            {
                acc_num++;
                if (acc_num > 5)
                {
                    throw new Exception("SMTL Too many accessorials");
                }
                sb.Append(string.Concat("<Accessorial0", acc_num, ">LIMITD</Accessorial0", acc_num, ">"));
            }
            if (quoteData.AccessorialsObj.INSDEL)
            {
                acc_num++;
                if (acc_num > 5)
                {
                    throw new Exception("SMTL Too many accessorials");
                }
                sb.Append(string.Concat("<Accessorial0", acc_num, ">INSD</Accessorial0", acc_num, ">"));
            }
            if (quoteData.isHazmat)
            {
                acc_num++;
                if (acc_num > 5)
                {
                    throw new Exception("SMTL Too many accessorials");
                }
                sb.Append(string.Concat("<Accessorial0", acc_num, ">HAZ</Accessorial0", acc_num, ">"));
            }
            if (quoteData.AccessorialsObj.APTPU || quoteData.AccessorialsObj.APTDEL)
            {
                acc_num++;
                if (acc_num > 5)
                {
                    throw new Exception("SMTL Too many accessorials");
                }
                sb.Append(string.Concat("<Accessorial0", acc_num, ">NOT</Accessorial0", acc_num, ">"));
            }

            return sb.ToString();

            /*
            "<Accessorial01></Accessorial01>",
                "<Accessorial02></Accessorial02>",
                "<Accessorial03></Accessorial03>",
                "<Accessorial04></Accessorial04>",
                "<Accessorial05></Accessorial05>",
            */
        }

        #endregion

        //

        #region Request_pickup
        public void Request_pickup()
        {
            Random rnd = new Random();
            int sess_1 = rnd.Next(10000000, 99999999);
            int sess_2 = rnd.Next(10000000, 99999999);
            try
            {
                string data = string.Concat("<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>",
                "<SMTLRateRequest>",
                "<SMTLIam>Genera_api</SMTLIam>",
                "<Token>", AppCodeConstants.smtl_genera_token, "</Token>",
                "<Option>Rate</Option>",
                "<SessionId>", sess_1, sess_2, "</SessionId>",

                "<FromCity>", quoteData.origCity, "</FromCity>",
                "<FromState>", quoteData.origState, "</FromState>",
                "<FromZip>", quoteData.origZip, "</FromZip>",

                "<ToCity>", quoteData.destCity, "</ToCity>",
                "<ToState>", quoteData.destState, "</ToState>",
                "<ToZip>", quoteData.destZip, "</ToZip>",

                "<ShipDate>", quoteData.puDate.ToShortDateString(), "</ShipDate>",
                "<Terms>P</Terms>",

                "<LiabilityCoverage>25000</LiabilityCoverage>",
                "<WingExpress>N</WingExpress>",
             
                "<ShipperName>ORouke</ShipperName><ShipperAddress>1875 Water Ridge</ShipperAddress>",
                "<ReadyTime>1400</ReadyTime><CloseTime>1700</CloseTime><Phone>2106623272</Phone><Contact>Randy</Contact>",
                "<PUInstruction1>long beam</PUInstruction1><PUInstruction2>2 pallets</PUInstruction2>",
                "</SMTLRateRequest>");

                data = string.Concat(
                    "<?xml version=\"1.0\" encoding=\"ISO-8859-1\" ?>",
                "<SMTLPickUpRequest>",
                "<SMTLIam>Genera_api</SMTLIam>",
                "<Token>Si2wKXpsU!</Token>",
              
                "<SessionId>", sess_1, sess_2, "</SessionId>",

  "<ShipperName> ORouke </ShipperName >",
  "<ShipperAddress> 1875 Water Ridge </ShipperAddress>",

  "<FromCity>", quoteData.origCity, "</FromCity>",
                "<FromState>", quoteData.origState, "</FromState>",
                "<FromZip>", quoteData.origZip, "</FromZip>",

  "<ShipDate>", quoteData.puDate.ToShortDateString(), "</ShipDate>",
  "<ReadyTime> 1400 </ReadyTime>",
  "<CloseTime> 1700 </CloseTime>",
  "<Phone> 2106623272 </Phone>",// ***no edits just number
  "<Contact> Randy </Contact>",

  "<OneCity> Tulsa </OneCity>",
  "<OneState> OK </OneState>",
  "<OneZip> 74115 </OneZip>",
  "<OnePUComment1> long beam </OnePUComment1>",
  "<OnePUComment2> 2 pallets </OnePUComment2>",
  "<OnePieces> 3 </OnePieces>",
  "<OneWeight> 500 </OneWeight>", //***whole number no decimals
  "<OnePckgCode> SK </OnePckgCode>",
  "<OneHazMat> Y </OneHazMat>",
  "<OneFrtIdNbr> 357357</OneFrtIdNbr>",
  
  "<TwoCity> McAllen </TwoCity>",
  "<TwoState> TX </TwoState>",
  "<TwoZip> 78501 </TwoZip>",
  "<TwoPUComment1> flexible conduit </TwoPUComment1>",
  "<TwoPUComment2> 2 pallets </TwoPUComment2>",
  "<TwoPieces> 2 </TwoPieces>",
  "<TwoWeight> 514 </TwoWeight>",
  "<TwoPckgCode> PT </TwoPckgCode>",
  "<TwoHazMat> N </TwoHazMat>",
  "<TwoFrtIdNbr> 357358 </TwoFrtIdNbr>",
  "<ThrCity> PortLand </ThrCity>",
  "<ThrState> OR </ThrState>",
  "<ThrZip> 97216 </ThrZip>",
  "<ThrPUComment1> 38 boxes per pallet </ThrPUComment1> ",
  "<ThrPUComment2>do not double stack</ThrPUComment2>",
  "<ThrPieces> 2 </ThrPieces>",
  "<ThrWeight> 1298 </ThrWeight>",
  "<ThrPckgCode> PT </ThrPckgCode>",
  "<ThrHazMat> N </ThrHazMat>",
  "<ThrFrtIdNbr> 357357 </ThrFrtIdNbr>",
"</SMTLPickUpRequest> ");

                //DB.Log("SMTL Pickup Request data", data);

                Web_client http = new Web_client();


                http.url = string.Concat("http://www2.smtl.com/rpgsp/SMTLPCKUP.pgm?Request=", data,
                    "&SessionId=", sess_1, sess_2);


                http.content_type = "text/xml,application/xml";
                http.method = "GET";

                http.accept = "text/xml,application/xml";

                string doc = http.Make_http_request();

                //DB.Log("SMTL Pickup Request doc", doc);

                #region Parse result

                //string[] tokens = new string[3];
                //tokens[0] = "<FreightCharges>";
                //tokens[1] = ">";
                //tokens[2] = "<";

                //string FreightCharges_str = HelperFuncs.scrapeFromPage(tokens, doc);

                #endregion

            }
            catch (Exception e)
            {
               
                DB.Log("SMTL", e.ToString());
            }
        }

        #endregion

    }
}