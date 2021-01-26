#region Using

using gcmAPI.Models.LTL;
using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static gcmAPI.Models.Carriers.DLS.DLS;

#endregion

namespace gcmAPI.Models.Carriers.DLS
{
    public class Add_DLS_results
    {
        #region Variables

        QuoteData quoteData;
        public int transitAddition, spcMaxMarkup, elapsed_milliseconds_DLS;
        public double addition, dupreRRTS_Buy, aafesRRTS_Buy;
        public bool got_Estes_HHG_Under_500_rate;
        public string serviceRRD, UserName;

        #endregion

        #region Constructor

        public Add_DLS_results(ref QuoteData quoteData)
        {
            this.quoteData = quoteData;
        }

        #endregion

        #region AddDLS_ResultToArray

        public void AddDLS_ResultToArray(ref List<dlsPricesheet> dlsPricesheets, ref GCMRateQuote[] totalQuotes, ref string UserName)
        {
          
            HelperFuncs.dlsMarkup dlsMarkup = new HelperFuncs.dlsMarkup();
            dlsMarkup.DLSMU = 0;
            dlsMarkup.DLSMinDollar = 0;

            if (quoteData.subdomain.Equals("spc"))
            {
                dlsMarkup.DLSMU = 33;
                dlsMarkup.DLSMinDollar = 35;
            }
            //else if (quoteData.mode.Equals("ws"))
            //{
            //    dlsMarkup.DLSMU = 33;
            //    dlsMarkup.DLSMinDollar = 40;
            //}
            else
            {
                HelperFuncs.GetDLS_Markup(quoteData.username, ref dlsMarkup);
            }

            decimal dlsPercentSum = 0, dlsPercent;
            dlsPercent = Convert.ToDecimal(dlsMarkup.DLSMU) / 100;

            //DB.Log("GetDLS_Markup dlsPercent, minDollar", dlsPercent.ToString() + " " + dlsMarkup.DLSMinDollar.ToString());
            
            // Get Overlenth Fee
            int overlengthFee = 0;
            HelperFuncs.GetOverlengthFee(ref quoteData.m_lPiece, ref overlengthFee, 143, 216, 288, 100, 150, 200);
            
            double maxDim = 0;
            HelperFuncs.GetMaxDimension(ref quoteData.m_lPiece, ref maxDim);
            
            bool foundDLS_CTII = false, add_overlength_success = false;
           
            string dlsCarrierKey = "";
            //CarsOnTime carOnTime;
            string onTimeCarName = "";
            //bool found_ups_dls_spc = false;

            bool can_add_carrier_result = false;

            foreach (dlsPricesheet objCarrier in dlsPricesheets)
            {
                //DB.LogGenera("XPO test", "XPO test", objCarrier.CarrierName + " " + objCarrier.ContractName + " " +
                //    objCarrier.Total.ToString());

                can_add_carrier_result = Can_add_carrier_result(objCarrier, ref foundDLS_CTII);

                if(can_add_carrier_result==true)
                {
                    // Do nothing
                }
                else
                {
                    continue;
                }

                try
                {
                  
                    GCMRateQuote objQuote = new GCMRateQuote();

                    objQuote.base_rate = objCarrier.base_rate;

                    objQuote.Cost_breakdown = objCarrier.Cost_breakdown;

                    objQuote.BookingKey = "#1#";

                    objQuote.Scac = objCarrier.Scac; // Scac

                    objQuote.Elapsed_milliseconds = elapsed_milliseconds_DLS;

                    // Switch the carrier SCAC to get the CarrierKey
                    onTimeCarName = ""; // Reset
                    dlsCarrierKey = ScacToCarrierKey_DLS(objCarrier.Scac, objCarrier.CarrierName, ref onTimeCarName);

                    //DB.Log("CarrierName dlsCarrierKey", string.Concat(objCarrier.CarrierName, " ", objCarrier.Scac, " ", dlsCarrierKey));

                    if (!dlsCarrierKey.Equals("not found"))
                    {
                        objQuote.CarrierKey = dlsCarrierKey;
                    }
                    else
                    {
                        // For now set the default to UPS
                        objQuote.CarrierKey = "UPS";
                    }

                    objQuote.DeliveryDays = objCarrier.ServiceDays;
                    objQuote.DeliveryDays += transitAddition;

                    objQuote.DisplayName = string.Concat(objCarrier.CarrierName.Replace(",", "%2C"), " RRD");

                    if (UserName == "Ben Franklin Crafts - Macon" && objCarrier.Scac == "OAKH") //(objCarrier.Scac == "UPGF" || 
                    {
                        //
                        //continue;
                        objQuote.DisplayName = string.Concat(objQuote.DisplayName, " DLS SPC");
                        ////objCarrier.Total = objCarrier.Total * 0.7519M;
                    }
                    else if(UserName == "Ben Franklin Crafts - Macon")
                    {
                        continue;
                    }
                    else
                    {
                        // Do nothing
                    }
                  

                    if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
                    {
                        if (objCarrier.ContractName.Contains(" ASP"))
                        {
                            objQuote.DisplayName = string.Concat(objQuote.DisplayName, " Genera");
                        }
                    }

                    if (objCarrier.CarrierName.StartsWith("Central Transport"))
                    {
                        foundDLS_CTII = true;
                        //dlsCTII_Addition = objCarrier.Total * 0.1M;
                    }
                    //else
                    //{
                    //    dlsCTII_Addition = 0M;
                    //}

                    if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
                        objQuote.OurRate = Convert.ToDouble(objCarrier.Total / 1.16M);
                    else
                        objQuote.OurRate = Convert.ToDouble(objCarrier.Total);

                    if (quoteData.mode.Equals("NetNet"))
                    {
                        objQuote.TotalPrice = Convert.ToDouble(objCarrier.Total / 1.16M);

                        if (quoteData.is_Genera_quote == true)
                        {
                            // Do nothing
                        }
                        else
                        {
                            objQuote.TotalPrice = Convert.ToDouble(objCarrier.Total);
                        }                            
                    }
                    else
                    {
                        #region Add markup

                        if (!quoteData.isAssociationID_5.Equals(true) && (dlsMarkup.DLSMU > 0 || dlsMarkup.DLSMinDollar > 0))
                        {
                            dlsPercentSum = (objCarrier.Total + Convert.ToDecimal(addition)) * dlsPercent;
                            if (quoteData.subdomain.Equals("spc") && dlsPercentSum > spcMaxMarkup)
                            {
                                objQuote.TotalPrice = Convert.ToDouble(objCarrier.Total + spcMaxMarkup);
                                //DB.Log("GetDLS_Markup spc: ", spcMaxMarkup.ToString());

                            }
                            else if (dlsPercentSum > dlsMarkup.DLSMinDollar)
                            {
                                //DB.Log("objQuote.TotalPrice",
                                //    objCarrier.Total.ToString());

                                if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
                                {
                                    if (objCarrier.Scac == "CNWY")
                                    {
                                        dlsPercentSum = (objCarrier.Total + Convert.ToDecimal(addition)) * 0.2M;
                                        objQuote.TotalPrice = Convert.ToDouble(objCarrier.Total + dlsPercentSum);
                                    }
                                    else
                                    {
                                        objQuote.TotalPrice = Convert.ToDouble(objCarrier.Total);
                                    }
                                    
                                }
                                else
                                {
                                    objQuote.TotalPrice = Convert.ToDouble(objCarrier.Total + dlsPercentSum);
                                }
                              
                                //DB.Log("adding markup", dlsPercentSum.ToString());
                                //DB.Log("objQuote.TotalPrice", objQuote.TotalPrice.ToString());
                                //objQuote.TotalPrice += Convert.ToDouble(dlsCTII_Addition);

                                //DB.Log("GetDLS_Markup percentSum larger: ",
                                //dlsPercentSum.ToString() + " " + dlsMarkup.DLSMinDollar.ToString());
                            }
                            else
                            {
                                if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
                                {
                                    if (objCarrier.Scac == "CNWY")
                                    {
                                        dlsPercentSum = (objCarrier.Total + Convert.ToDecimal(addition)) * 0.2M;
                                        objQuote.TotalPrice = Convert.ToDouble(objCarrier.Total + dlsPercentSum);
                                    }
                                    else
                                    {
                                        objQuote.TotalPrice = Convert.ToDouble(objCarrier.Total);
                                    }
                                    
                                }
                                else
                                {
                                    objQuote.TotalPrice = Convert.ToDouble(objCarrier.Total + dlsMarkup.DLSMinDollar);
                                }
                                    

                                //objQuote.TotalPrice += Convert.ToDouble(dlsCTII_Addition);

                                //DB.Log("GetDLS_Markup DLSMinDollar larger: ",
                                //    dlsPercentSum.ToString() + " " + dlsMarkup.DLSMinDollar.ToString());
                            }
                        }
                        else
                        {
                            objQuote.TotalPrice = Convert.ToDouble(objCarrier.Total);

                            //objQuote.TotalPrice += Convert.ToDouble(dlsCTII_Addition);

                            //DB.Log("GetDLS_Markup all 0: ",
                            //        dlsPercentSum.ToString() + " " + dlsMarkup.DLSMinDollar.ToString());
                        }

                        #endregion
                    }

                    objQuote.TotalPrice += addition;
                    objQuote.OurRate += addition;

                    if(objQuote.TotalPrice > 0)
                    {
                        // Do nothing
                    }
                    else
                    {
                        continue;
                    }

                    Add_overlength_fee(ref objQuote, ref quoteData, ref maxDim, ref overlengthFee, ref add_overlength_success);

                    if (add_overlength_success == true)
                    {
                        // Do nothing
                    }
                    else
                    {
                        continue;
                    }

                    if (quoteData.username.ToLower().Equals("dupraafesbuy"))
                    {
                        double DuprMinusCost = dupreRRTS_Buy - objQuote.TotalPrice;
                        
                        if (!(DuprMinusCost >= 30)) // Cost needs to be at least $30 more than dupreRRTS_Buy
                        {
                            continue;
                        }
                        else if (aafesRRTS_Buy - dupreRRTS_Buy < 25)
                        {
                            continue;
                        }
                    }

                    objQuote.Documentation = null;


                    //if (Session["onTimeDict"] != null && ((Dictionary<string, CarsOnTime>)Session["onTimeDict"]).TryGetValue(onTimeCarName, out carOnTime))
                    //{
                    //    objQuote.OnTimePercent = carOnTime.onTimePercent + '%';
                    //    objQuote.ShipsBetweenStates = carOnTime.delivOnTime + carOnTime.delivLate;
                    //}


                    if (serviceRRD.Equals("regularServiceRRD"))
                    {
                        objQuote.RateType = "REGULAR";
                        totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objQuote);
                    }
                    else
                    {
                        objQuote.GuaranteedRatePM = 1;
                        objQuote.RateType = "GUARANTEEDPM";
                        totalQuotes = AddDLS_GuaranteedItemsToQuoteArray(totalQuotes, objQuote);
                    }

                }
                catch (Exception upsE)
                {
                    DB.Log("DLS", upsE.ToString());
                }

            }
        }

        #endregion

        #region Can_add_carrier_result

        private bool Can_add_carrier_result(dlsPricesheet objCarrier, ref bool foundDLS_CTII)
        {
            if (objCarrier.CarrierName.StartsWith("TForce"))
            {
                return false;
            }


            if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
            {
                if (objCarrier.CarrierName=="Fedex LTL Economy")
                {
                    return false;
                }

                if (quoteData.destZip == "11570" && objCarrier.Scac == "CTII")
                    return false;

                if (objCarrier.Scac == "EXLA" && objCarrier.ContractName.Contains("Genera Corp"))
                {
                    // Do nothing
                }
                else if (objCarrier.Scac != "EXLA" )
                {
                    // Do nothing
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (objCarrier.Scac == "CTII")
                    return false;
            }

            if (objCarrier.Scac == "EXLA" && got_Estes_HHG_Under_500_rate && (serviceRRD.Equals("regularServiceRRD") ||
                  serviceRRD.Equals("GLTL_ServiceRRD"))) // && serviceRRD.Equals("regularServiceRRD")
            {
                // Do not add Estes with extra weight, it is being added with actual weight
                return false;
            }

            if (objCarrier.Scac == "CSEQ" && quoteData.origCity == "BLUFFTON" && quoteData.origState == "IN")
            {
                return false;
            }

            if (objCarrier.Scac == "CSEQ" && quoteData.destCity == "BLUFFTON" && quoteData.destState == "IN")
            {
                return false;
            }

            if (UserName == "Ben Franklin Crafts - Macon" && (objCarrier.Scac == "UPGF" || objCarrier.Scac == "OAKH"))
            {
                if (objCarrier.Scac == "UPGF" && objCarrier.ContractName == "1PSI Cost UPGF Sierra Pacific Crafts 3PL ASP")
                {
                    //DB.Log("objCarrier.ContractName", objCarrier.ContractName);
                    //found_ups_dls_spc = true;
                    // Do nothing
                }
                else if (objCarrier.Scac == "UPGF")
                {
                    return false;
                }
                else if (objCarrier.Scac == "OAKH")
                {
                    // Do nothing
                }
                else
                {
                    return false;
                }

                //

                //if (objCarrier.Scac == "UPGF")
                //{
                //    found_ups_dls_spc = true;
                //}
            }
            else if (UserName == "Ben Franklin Crafts - Macon")
            {
                return false;
            }
            else
            {
                // Do nothing
            }
            
            if (quoteData.is_AAFES_quote.Equals(true) && objCarrier.Scac == "RDFS")
            {
                return false;
            }
            
            if (objCarrier.CarrierName.StartsWith("Central Transport") && foundDLS_CTII.Equals(true)) // Don't add CTII twice
            {
                return false;
            }

            if (quoteData.mode.Equals("ws") && quoteData.api_username == "inxpressncti" && objCarrier.Scac.Equals("CTII"))
            {
                return false;
            }
            if (objCarrier.CarrierName.StartsWith("Crosscountry"))
            {
                return false;
            }

            double maxLengthDim = 0;
            HelperFuncs.GetMaxLengthDimension(ref quoteData.m_lPiece, ref maxLengthDim);

            if (!quoteData.mode.Equals("NetNet"))
            {
                if ((quoteData.AccessorialsObj.LGPU.Equals(true) ||
                    quoteData.AccessorialsObj.LGDEL.Equals(true))
                    && maxLengthDim > 70 &&
                    (objCarrier.Scac.Equals("CLNI") ||
                    objCarrier.Scac.Equals("RDFS") ||
                    objCarrier.Scac.Equals("PITD") ||
                    objCarrier.Scac.Equals("FCSY") ||
                    objCarrier.Scac.Equals("CTII")))
                {
                    return false;
                }
            }

            if (quoteData.mode.Equals("NetNet") && quoteData.numOfUnitsPieces > 5 &&
                objCarrier.Scac.Equals("CLNI"))
            {
                return false;
                //DB.Log("NetNet 5 clearlane", "NetNet 5 clearlane");
            }

            // Do not show roadrunner or fedex for household goods
            if (quoteData.isHHG && (objCarrier.CarrierName.Contains("ROADRUNNER") || objCarrier.CarrierName.ToLower().Contains("fedex")))
            {
                return false;
            }

            if (quoteData.AccessorialsObj.TRADEPU.Equals(true) &&
                !(objCarrier.CarrierName.Contains("UPS") || objCarrier.CarrierName.Contains("YRC")))
            {
                return false;
            }

            if (quoteData.username.Equals("near200n") && objCarrier.Scac.Equals("AACT"))
            {
                return false;
            }


            #region Check conditions for CTII

            if (objCarrier.CarrierName.StartsWith("Central Transport") &&
               (quoteData.isHHG || quoteData.isUSED || quoteData.AccessorialsObj.RESPU || quoteData.AccessorialsObj.RESDEL || quoteData.AccessorialsObj.LGPU || quoteData.AccessorialsObj.LGDEL ||
                quoteData.AccessorialsObj.TRADEPU || quoteData.AccessorialsObj.TRADEDEL || quoteData.AccessorialsObj.CONPU || quoteData.AccessorialsObj.CONDEL))
            {
                return false;
            }

            #endregion


            return true;
        }

        #endregion

        #region AddDLS_ResultToArray_HHG_Under_500

        public void AddDLS_ResultToArray_HHG_Under_500(ref List<dlsPricesheet> dlsPricesheets, ref GCMRateQuote[] totalQuotes)
        {

            HelperFuncs.dlsMarkup dlsMarkup = new HelperFuncs.dlsMarkup();
            dlsMarkup.DLSMU = 0;
            dlsMarkup.DLSMinDollar = 0;

            if (quoteData.subdomain.Equals("spc"))
            {
                dlsMarkup.DLSMU = 33;
                dlsMarkup.DLSMinDollar = 35;
            }
            //else if (quoteData.mode.Equals("ws"))
            //{
            //    dlsMarkup.DLSMU = 33;
            //    dlsMarkup.DLSMinDollar = 40;
            //}
            else
            {
                HelperFuncs.GetDLS_Markup(quoteData.username, ref dlsMarkup);
            }

            decimal dlsPercentSum = 0, dlsPercent;
            dlsPercent = Convert.ToDecimal(dlsMarkup.DLSMU) / 100;

            // Get Overlenth Fee
            int overlengthFee = 0;
            HelperFuncs.GetOverlengthFee(ref quoteData.m_lPiece, ref overlengthFee, 143, 216, 288, 100, 150, 200);

            double maxDim = 0;
            HelperFuncs.GetMaxDimension(ref quoteData.m_lPiece, ref maxDim);

            double maxLengthDim = 0;
            HelperFuncs.GetMaxLengthDimension(ref quoteData.m_lPiece, ref maxLengthDim);

            bool add_overlength_success = false;

            string dlsCarrierKey = "";

            string onTimeCarName = "";

            foreach (dlsPricesheet objCarrier in dlsPricesheets)
            {
                try
                {
                    if (objCarrier.Scac == "EXLA")
                    {
                        // Do nothing
                    }
                    else
                    {
                        continue;
                    }

                    if (quoteData.AccessorialsObj.TRADEPU.Equals(true) &&
                        !(objCarrier.CarrierName.Contains("UPS") || objCarrier.CarrierName.Contains("YRC")))
                    {
                        continue;
                    }

                    GCMRateQuote objQuote = new GCMRateQuote();

                    objQuote.base_rate = objCarrier.base_rate;

                    objQuote.Cost_breakdown = objCarrier.Cost_breakdown;

                    objQuote.BookingKey = "#1#";

                    objQuote.Scac = objCarrier.Scac; // Scac

                    objQuote.Elapsed_milliseconds = elapsed_milliseconds_DLS;

                    // Switch the carrier SCAC to get the CarrierKey
                    onTimeCarName = ""; // Reset
                    dlsCarrierKey = ScacToCarrierKey_DLS(objCarrier.Scac, objCarrier.CarrierName, ref onTimeCarName);

                    //DB.Log("CarrierName dlsCarrierKey", string.Concat(objCarrier.CarrierName, " ", objCarrier.Scac, " ", dlsCarrierKey));

                    if (!dlsCarrierKey.Equals("not found"))
                    {
                        objQuote.CarrierKey = dlsCarrierKey;
                    }
                    else
                    {
                        // For now set the default to UPS
                        objQuote.CarrierKey = "UPS";
                    }

                    objQuote.DeliveryDays = objCarrier.ServiceDays;
                    objQuote.DeliveryDays += transitAddition;

                    objQuote.DisplayName =
                        string.Concat(objCarrier.CarrierName.Replace(",", "%2C"), " RRD"); //+ " Estes HHG test"

                    if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
                    {
                        if (objCarrier.ContractName.Contains(" ASP"))
                        {
                            objQuote.DisplayName = string.Concat(objQuote.DisplayName, " Genera");
                        }
                    }

                    objQuote.OurRate = Convert.ToDouble(objCarrier.Total);

                    if (quoteData.mode.Equals("NetNet"))
                    //if (true)
                    {
                        // Do Nothing
                        objQuote.TotalPrice = Convert.ToDouble(objCarrier.Total);
                    }
                    else
                    {
                        #region Add markup

                        if (!quoteData.isAssociationID_5.Equals(true) && (dlsMarkup.DLSMU > 0 || dlsMarkup.DLSMinDollar > 0))
                        {
                            dlsPercentSum = (objCarrier.Total + Convert.ToDecimal(addition)) * dlsPercent;
                            if (quoteData.subdomain.Equals("spc") && dlsPercentSum > spcMaxMarkup)
                            {
                                objQuote.TotalPrice = Convert.ToDouble(objCarrier.Total + spcMaxMarkup);
                                //DB.Log("GetDLS_Markup spc: ", spcMaxMarkup.ToString());

                            }
                            else if (dlsPercentSum > dlsMarkup.DLSMinDollar)
                            {

                                objQuote.TotalPrice = Convert.ToDouble(objCarrier.Total + dlsPercentSum);

                            }
                            else
                            {
                                objQuote.TotalPrice = Convert.ToDouble(objCarrier.Total + dlsMarkup.DLSMinDollar);

                            }
                        }
                        else
                        {
                            objQuote.TotalPrice = Convert.ToDouble(objCarrier.Total);

                        }

                        #endregion
                    }

                    //DB.Log("objQuote.TotalPrice", objQuote.TotalPrice.ToString());

                    objQuote.TotalPrice += addition;
                    objQuote.OurRate += addition;

                    Add_overlength_fee(ref objQuote, ref quoteData, ref maxDim, ref overlengthFee, ref add_overlength_success);

                    if (add_overlength_success == true)
                    {
                        // Do nothing
                    }
                    else
                    {
                        continue;
                    }

                    objQuote.Documentation = null;

                    if (serviceRRD.Equals("regularServiceRRD"))
                    {
                        objQuote.RateType = "REGULAR";
                        totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objQuote);
                    }
                    else
                    {
                        objQuote.GuaranteedRatePM = 1;
                        objQuote.RateType = "GUARANTEEDPM";
                        totalQuotes = AddDLS_GuaranteedItemsToQuoteArray(totalQuotes, objQuote);
                    }
                }
                catch (Exception upsE)
                {
                    DB.Log("DLS", upsE.ToString());
                }
            }
        }

        #endregion

        #region AddDLS_GuaranteedItemsToQuoteArray

        private GCMRateQuote[] AddDLS_GuaranteedItemsToQuoteArray(GCMRateQuote[] objQuoteArray, GCMRateQuote newQuoteItem)
        {
            if (objQuoteArray != null && newQuoteItem != null)
            {
                List<GCMRateQuote> sortedList = new List<GCMRateQuote>();

                for (byte i = 0; i < objQuoteArray.Length; i++)
                {
                    sortedList.Add(objQuoteArray[i]);

                    //DB.Log("objQuoteArray[i].DisplayName", objQuoteArray[i].DisplayName);

                    //DB.Log("newQuoteItem.DisplayName",
                    //    newQuoteItem.DisplayName.Replace("<span style='color:Green;'> Guaranteed by 5PM</span>", ""));

                    //DB.Log("objQuoteArray[i].DisplayName", objQuoteArray[i].DisplayName);

                    if (newQuoteItem.DisplayName.Replace("<span style='color:Green;'> Guaranteed by 5PM</span>", "").Equals(objQuoteArray[i].DisplayName))
                    {
                        sortedList.Add(newQuoteItem);
                        //DB.Log("sorted names added guaranteed", newQuoteItem.DisplayName);
                    }
                    //DB.Log("sorted names", newQuoteItem.DisplayName.Replace("<span style='color:Green;'> Guaranteed by 5PM</span>", "") + " - " + objQuoteArray[i].DisplayName);
                }
                return sortedList.ToArray();
            }
            else
            {
                return objQuoteArray;
            }

        }

        #endregion

        #region AddDLS_ResultToArray_cust_rates

        public void AddDLS_ResultToArray_cust_rates(ref List<dlsPricesheet> dlsPricesheets, ref GCMRateQuote[] totalQuotes)
        {
            //DB.Log("cust rates AddDLS_ResultToArray_cust_rates", "AddDLS_ResultToArray_cust_rates");
            foreach (dlsPricesheet objCarrier in dlsPricesheets)
            {
                //DB.Log("cust rates foreach", "cust rates foreach");
                GCMRateQuote objQuote = new GCMRateQuote();

                objQuote.base_rate = objCarrier.base_rate;

                objQuote.BookingKey = "#1#";

                objQuote.Scac = objCarrier.Scac; // Scac
                objQuote.TotalPrice = Convert.ToDouble(objCarrier.Total);
                objQuote.OurRate = Convert.ToDouble(objCarrier.Total);
                objQuote.RateType = "cust_rates";
                objQuote.Documentation = null;
                string onTimeCarName = "";
                string dlsCarrierKey = ScacToCarrierKey_DLS(objCarrier.Scac, objCarrier.CarrierName, ref onTimeCarName);

                //DB.Log("CarrierName dlsCarrierKey", string.Concat(objCarrier.CarrierName, " ", objCarrier.Scac, " ", dlsCarrierKey));

                if (!dlsCarrierKey.Equals("not found"))
                {
                    objQuote.CarrierKey = dlsCarrierKey;
                }
                else
                {
                    // For now set the default to UPS
                    objQuote.CarrierKey = "UPS";
                }

                objQuote.DeliveryDays = objCarrier.ServiceDays;
                objQuote.DeliveryDays += transitAddition;

                objQuote.DisplayName = string.Concat(objCarrier.CarrierName.Replace(",", "%2C"), " RRD");
                //DB.Log("cust rates Display name", objQuote.DisplayName);

                totalQuotes = SharedLTL.AddItemsToQuoteArray(totalQuotes, objQuote);
            }
        }

        #endregion

        #region ScacToCarrierKey_DLS

        private string ScacToCarrierKey_DLS(string Scac, string CarrierName, ref string onTimeCarName)
        {
            string rrd = " rrd";
            switch (Scac)
            {
                case ("LMEL"):
                    {
                        // Con Way
                        onTimeCarName = "";
                        return string.Concat("Lakeville Motor", rrd);
                    }
                case ("CNWY"):
                    {
                        // Con Way
                        onTimeCarName = "";
                        return string.Concat("Con-Way", rrd);
                    }
                case ("FXFE"):
                    {
                        // FedEx LTL Priority rrd
                        onTimeCarName = "Fedex";
                        return string.Concat("FedEx LTL Priority", rrd);
                    }
                case ("FXNL"):
                    {
                        // FedEx LTL Economy rrd
                        onTimeCarName = "Fedex";
                        return string.Concat("FedEx LTL Economy", rrd);
                    }
                case ("CTII"):
                    {
                        // Central Transport Intl rrd
                        onTimeCarName = "CTII";
                        return string.Concat("Central Transport Intl", rrd);
                    }
                case ("RDWY"):
                    {
                        // YRC rrd
                        onTimeCarName = "YRC";
                        return string.Concat("YRC", rrd);
                    }
                case ("EXLA"):
                    {
                        // Estes Express Lines rrd
                        onTimeCarName = "Estes";
                        return string.Concat("Estes Express Lines", rrd);
                    }
                case ("UPGF"):
                    {
                        // UPS Freight rrd
                        onTimeCarName = "UPS";
                        return string.Concat("UPS Freight", rrd);
                    }
                case ("SAIA"):
                    {
                        // Con Way
                        onTimeCarName = "SAIA";
                        return string.Concat("Saia", rrd);
                    }
                case ("RNLO"):
                    {
                        // R+L Carriers rrd
                        onTimeCarName = "R&L";
                        return string.Concat("R+L Carriers", rrd);
                    }
                case ("RDFS"):
                    {
                        // Roadrunner rrd
                        onTimeCarName = "RRTS";
                        return string.Concat("Roadrunner", rrd);
                    }
                case ("NEMF"):
                    {
                        // Con Way
                        onTimeCarName = "NEMF";
                        return string.Concat("New England Motor", rrd);
                    }
                case ("NPME"):
                    {
                        // Con Way
                        onTimeCarName = "New Penn Motor Express";
                        return string.Concat("New Penn", rrd);
                    }
                case ("AACT"):
                    {
                        // Con Way
                        onTimeCarName = "AAA Cooper";
                        return string.Concat("AAA Cooper", rrd);
                    }
                case ("DAFG"):
                    {
                        // Con Way
                        onTimeCarName = "Dayton";
                        return string.Concat("Dayton Freight", rrd);
                    }
                case ("LKVL"):
                    {
                        // Con Way
                        onTimeCarName = "Lakeville";
                        return string.Concat("Lakeville Motor", rrd);
                    }
                case ("PITD"):
                    {
                        // Con Way
                        onTimeCarName = "Pitt Ohio";
                        return string.Concat("Pitt Ohio", rrd);
                    }
                case ("ABNE"):
                    {
                        // Con Way
                        onTimeCarName = "";
                        return string.Concat("Aberdeen Express", rrd);
                    }
                case ("AVRT"):
                    {
                        // Con Way
                        onTimeCarName = "Averitt Express";
                        return string.Concat("Averitt Express", rrd);
                    }
                case ("HMES"):
                    {
                        // Con Way
                        onTimeCarName = "USF";
                        return string.Concat("USF Holland", rrd);
                    }
                case ("DUBL"):
                    {
                        // Con Way
                        onTimeCarName = "";
                        return string.Concat("Dugan", rrd);
                    }
                case ("SMTL"):
                    {
                        // Con Way
                        onTimeCarName = "";
                        return string.Concat("Southwestern Motor", rrd);
                    }
                case ("SEFL"):
                    {
                        // Con Way
                        onTimeCarName = "SEFL";
                        return string.Concat("Southeastern Freight Lines", rrd);
                    }
                case ("CENF"):
                    {
                        // Con Way
                        onTimeCarName = "Central Freight";
                        return string.Concat("Central Freight Lines", rrd);
                    }
                case ("STDF"):
                    {
                        // Con Way
                        onTimeCarName = "Standard Forwarding";
                        return string.Concat("Standard Forwarding", rrd);
                    }
                case ("EXDF"):
                    {
                        // Con Way
                        onTimeCarName = "";
                        return string.Concat("Expedited Freight Systems", rrd);
                    }
                //case ("CCYQ"):
                //    {
                //        // Con Way
                //        return string.Concat("Lakeville Motor", rrd);
                //    }
                case ("CLNI"):
                    {
                        // Con Way
                        onTimeCarName = "";
                        return string.Concat("Clear Lane Freight", rrd);
                    }
                case ("VSXP"):
                    {
                        // Con Way
                        onTimeCarName = "";
                        return string.Concat("Vision Express/Wrag-Time Trans", rrd);
                    }
                case ("RETL"):
                    {
                        // Con Way
                        onTimeCarName = "USF";
                        return string.Concat("USF Reddaway", rrd);
                    }
                case ("FCSY"):
                    {
                        // Con Way
                        onTimeCarName = "Frontline";
                        return string.Concat("Frontline Freight", rrd);
                    }
                case ("WTVA"):
                    {
                        // Con Way
                        onTimeCarName = "";
                        return string.Concat("Wilson Trucking Company", rrd);
                    }
                case ("FWDA"):
                    {
                        // Con Way
                        onTimeCarName = "";
                        return string.Concat("Forward Air", rrd);
                    }
                case ("BBFG"):
                    {
                        // Con Way
                        onTimeCarName = "";
                        return string.Concat("Benjamin Best", rrd);
                    }
                case ("DPHE"):
                    {
                        // Con Way
                        onTimeCarName = "";
                        return string.Concat("DHE", rrd);
                    }
                default:
                    {
                        DB.Log("ScacToCarrierKey_DLS Live did not find Carrier Key for Scac:", string.Concat(Scac, " carrier was: ", CarrierName));
                        return "not found";
                    }
            }
        }

        #endregion

        #region Add_overlength_fee

        private void Add_overlength_fee(ref GCMRateQuote objQuote, ref QuoteData quoteData,
            ref double maxDim, ref int overlengthFee, ref bool success)
        {
            // Add overlength fee
            switch (objQuote.Scac)
            {
                case ("CNWY"):
                    {
                        if (quoteData.AccessorialsObj.CONPU.Equals(true))
                        {
                            objQuote.TotalPrice += 50;
                            objQuote.OurRate += 50;
                        }
                        if (quoteData.AccessorialsObj.CONDEL.Equals(true))
                        {
                            objQuote.TotalPrice += 50;
                            objQuote.OurRate += 50;
                        }

                        break;
                    }
                case ("RDWY"):
                    {
                        if (maxDim > 95 && maxDim < 132)
                        {
                            objQuote.TotalPrice += 85;
                            objQuote.OurRate += 85;
                        }
                        else if (maxDim >= 132 && maxDim < 165)
                        {
                            objQuote.TotalPrice += 150;
                            objQuote.OurRate += 150;
                        }
                        else if (maxDim >= 165 && maxDim < 228) // 145' or more does not give rate on GCM
                        {
                            objQuote.TotalPrice += 225;
                            objQuote.OurRate += 225;
                        }
                        else if (maxDim >= 228 && maxDim < 297)
                        {
                            objQuote.TotalPrice += 385;
                            objQuote.OurRate += 385;
                        }
                        else if (maxDim >= 297)
                        {
                            objQuote.TotalPrice += 1100;
                            objQuote.OurRate += 1100;
                        }
                        else
                        {
                            objQuote.TotalPrice += overlengthFee;
                            objQuote.OurRate += overlengthFee;
                        }
                        if (quoteData.destState.Equals("CA"))
                        {
                            objQuote.TotalPrice += 8;
                            objQuote.OurRate += 8;
                            //DB.Log("CA", "CA");
                        }
                        //else
                        //{
                        //    DB.Log("nt CA", quoteData.destState);
                        //}

                        break;
                    }
                case ("HMES"):
                    {
                        // USF Holland 

                        if (maxDim > 239)
                        {
                            objQuote.TotalPrice += 1200;
                            objQuote.OurRate += 1200;
                        }
                        else if (maxDim > 191)
                        {
                            objQuote.TotalPrice += 600;
                            objQuote.OurRate += 600;
                        }
                        else if (maxDim > 143)
                        {
                            objQuote.TotalPrice += 300;
                            objQuote.OurRate += 300;
                        }
                        else if (maxDim > 95)
                        {
                            objQuote.TotalPrice += 120;
                            objQuote.OurRate += 120;
                        }
                        else
                        {
                            objQuote.TotalPrice += overlengthFee;
                            objQuote.OurRate += overlengthFee;
                        }
                        break;
                    }
                case ("RETL"):
                    {
                        // USF Reddaway 
                        if (maxDim > 239)
                        {
                            objQuote.TotalPrice += 1200;
                            objQuote.OurRate += 1200;
                        }
                        else if (maxDim > 191)
                        {
                            objQuote.TotalPrice += 600;
                            objQuote.OurRate += 600;
                        }
                        else if (maxDim > 143)
                        {
                            objQuote.TotalPrice += 300;
                            objQuote.OurRate += 300;
                        }
                        else if (maxDim > 95)
                        {
                            objQuote.TotalPrice += 120;
                            objQuote.OurRate += 120;
                        }
                        else
                        {
                            objQuote.TotalPrice += overlengthFee;
                            objQuote.OurRate += overlengthFee;
                        }
                        break;
                    }
                case ("SAIA"):
                    {
                        if (maxDim > 144)
                        {
                            objQuote.TotalPrice += 130;
                            objQuote.OurRate += 130;
                        }
                        else
                        {
                            objQuote.TotalPrice += overlengthFee;
                            objQuote.OurRate += overlengthFee;
                        }
                        break;
                    }
                case ("CENF"):
                    {
                        if (maxDim > 95)
                        {
                            objQuote.TotalPrice += 25;
                            objQuote.OurRate += 25;
                        }
                        else
                        {
                            objQuote.TotalPrice += overlengthFee;
                            objQuote.OurRate += overlengthFee;
                        }
                        if (quoteData.destState.Equals("CA"))
                        {
                            objQuote.TotalPrice += 8.25;
                            objQuote.OurRate += 8.25;
                            //DB.Log("CA", "CA");
                        }
                        break;
                    }
                case ("CSEQ"):
                    {
                        objQuote.TotalPrice *= 1.05;  // Add 5%
                        break;
                    }
                case ("UPGF"):
                    {
                        if (maxDim > 179)
                        {
                            objQuote.TotalPrice += 100;
                            objQuote.OurRate += 100;
                        }
                        else
                        {
                            objQuote.TotalPrice += overlengthFee;
                            objQuote.OurRate += overlengthFee;
                        }
                        break;
                    }
                case ("NPME"):
                    {
                        //DB.Log("objQuote.Scac", objQuote.Scac + " " + objQuote.TotalPrice.ToString());
                        if (maxDim > 144)
                        {
                            objQuote.TotalPrice += 35;
                            objQuote.OurRate += 35;
                        }
                        else
                        {
                            objQuote.TotalPrice += overlengthFee;
                            objQuote.OurRate += overlengthFee;
                        }
                        //DB.Log("objQuote.Scac", objQuote.Scac + " " + objQuote.TotalPrice.ToString());

                        break;
                    }
                case ("EXLA"):
                    {
                        //DB.Log("objQuote.Scac ESTES", objQuote.Scac + " " + objQuote.TotalPrice.ToString());
                        //DB.Log("overlengthFee", overlengthFee.ToString());
                        //DB.Log("maxDim", maxDim.ToString());
                        if (maxDim > 95)
                        {
                            objQuote.TotalPrice += 100;
                            objQuote.OurRate += 100;
                        }
                        else if (maxDim > 240)
                        {
                            objQuote.TotalPrice += 200;
                            objQuote.OurRate += 200;
                        }
                        else
                        {
                            objQuote.TotalPrice += overlengthFee;
                            objQuote.OurRate += overlengthFee;
                        }

                        if (quoteData.destState.Equals("CA") || quoteData.origState.Equals("CA"))
                        {
                            objQuote.TotalPrice += 6.9;
                            objQuote.OurRate += 6.9;
                            //DB.Log("CA", "CA");
                        }
                        //DB.Log("objQuote.Scac", objQuote.Scac + " " + objQuote.TotalPrice.ToString());

                        break;
                    }
                case ("AACT"):
                    {
                        if (maxDim > 143 && maxDim < 240)
                        {
                            objQuote.TotalPrice += 75;
                            objQuote.OurRate += 75;
                        }
                        else if (maxDim >= 240 && maxDim < 336)
                        {
                            objQuote.TotalPrice += 200;
                            objQuote.OurRate += 200;
                        }
                        else if (maxDim >= 360)
                        {
                            objQuote.TotalPrice += 600;
                            objQuote.OurRate += 600;
                        }
                        else
                        {
                            objQuote.TotalPrice += overlengthFee;
                            objQuote.OurRate += overlengthFee;
                        }
                        break;
                    }
                case ("LKVL"):
                    {
                        if (maxDim > 143 && maxDim < 192)
                        {
                            objQuote.TotalPrice += 300;
                            objQuote.OurRate += 300;
                        }
                        else if (maxDim >= 192)
                        {
                            objQuote.TotalPrice += 500;
                            objQuote.OurRate += 500;
                        }
                        else
                        {
                            objQuote.TotalPrice += overlengthFee;
                            objQuote.OurRate += overlengthFee;
                        }
                        break;
                    }
                //case ("NEMF"):
                //    {
                //        if (maxDim > 144)
                //        {
                //            objQuote.TotalPrice += 25;
                //            objQuote.OurRate += 25;
                //        }
                //        else
                //        {
                //            objQuote.TotalPrice += overlengthFee;
                //            objQuote.OurRate += overlengthFee;
                //        }
                //        break;
                //    }
                case ("STDF"):
                    {
                        objQuote.TotalPrice += overlengthFee;
                        objQuote.OurRate += overlengthFee;
                        break;
                    }
                case ("DPHE"): // ?
                    {
                        if (maxDim > 143 && maxDim < 240)
                        {
                            objQuote.TotalPrice += 150;
                            objQuote.OurRate += 150;
                        }
                        else if (maxDim >= 240)
                        {
                            objQuote.TotalPrice += 250;
                            objQuote.OurRate += 250;
                        }
                        else
                        {
                            objQuote.TotalPrice += overlengthFee;
                            objQuote.OurRate += overlengthFee;
                        }
                        break;
                    }
                case ("BBFG"): // ?
                    {
                        if (maxDim > 155) // || quoteData.totalWeight > 1599
                        {
                            //continue; // Do not add the quote
                            success = false;
                            return;
                        }
                        else
                        {
                            objQuote.TotalPrice += overlengthFee;
                            objQuote.OurRate += overlengthFee;
                        }
                        break;
                    }
                case ("RDFS"):
                    {
                        objQuote.DeliveryDays += 1;
                        break;
                    }
                case ("CLNI"):
                    {
                        objQuote.DeliveryDays += 2;
                        break;
                    }
                case ("FCSY"):
                    {
                        objQuote.DeliveryDays += 2;
                        break;
                    }
                default:
                    {
                        if (maxDim > 95)
                        {
                            objQuote.TotalPrice += 75;
                            objQuote.OurRate += 75;
                        }
                        objQuote.TotalPrice += overlengthFee;
                        objQuote.OurRate += overlengthFee;
                        break;
                    }
            }

            success = true;
        }

        #endregion
    }
}