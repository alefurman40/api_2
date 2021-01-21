#region Using

using gcmAPI.Models.Customers;
using gcmAPI.Models.LTL;
using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using static gcmAPI.Models.Carriers.DLS.DLS;

#endregion

namespace gcmAPI.Models.Carriers.DLS
{
    public class Parser
    {
        #region Variables

        QuoteData quoteData;
        bool guaranteedService, is_Estes_HHG_Under_500;
        string UserName; //, APIKey

        #endregion

        #region Constructor

        public Parser(ref QuoteData quoteData, ref string UserName, ref bool guaranteedService, ref bool is_Estes_HHG_Under_500)
        {
            this.quoteData = quoteData;
            this.UserName = UserName;
            this.guaranteedService = guaranteedService;
            this.is_Estes_HHG_Under_500 = is_Estes_HHG_Under_500;
        }

        #endregion

        #region Parse_Results

        public void Parse_results(ref List<dlsPricesheet> dlsPricesheets, ref string doc, ref bool isOverlengthUPS)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(doc);

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("PricesheetViewModel");

            decimal Total = 0M;

            bool gotDLS_Worldwide = false, gotGLTL_DLS_Worldwide = false;
            //dlsPricesheets = new List<dlsPricesheet>();

            string breakdown_description = "", breakdown_cost = "";
            decimal breakd_cost = 0M;
            //
            // Markup

            //
            decimal dlsPercentSum;
            HelperFuncs.dlsMarkup dlsMarkup = new HelperFuncs.dlsMarkup();
            dlsMarkup.DLSMU = 0;
            dlsMarkup.DLSMinDollar = 0;
            
            if (quoteData.mode == "NetNet")
            {
                // Do nothing
            }
            else if (quoteData.subdomain.Equals("spc"))
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

            //DB.Log("markup username " + quoteData.username, dlsMarkup.DLSMU.ToString());


            for (byte i = 0; i < nodeList.Count; i++)
            {
                dlsPricesheet pSheet = new dlsPricesheet();

                //if (decimal.TryParse(nodeList[i]["Total"].InnerText.Trim(), out Total))
                //{

                if (!decimal.TryParse(nodeList[i]["Total"].InnerText.Trim(), out Total))
                {
                    continue;
                }

                if(Total > 0M)
                {
                    // Do nothing
                }
                else
                {
                    continue;
                }

                //
                try
                {
                    //
                    //dlsPercentSum = (objCarrier.Total + Convert.ToDecimal(addition)) * dlsPercent;
                    //

                    StringBuilder sb = new StringBuilder();
                    // Get accessorials breakdown
                    XmlDocument accessorials_xmlDoc = new XmlDocument();
                    accessorials_xmlDoc.LoadXml(nodeList[i].OuterXml.ToString());
                    XmlNodeList RateChargeViewModel_nodeList = accessorials_xmlDoc.GetElementsByTagName("RateChargeViewModel");

                    //
                    XmlDocument SubTotal_xmlDoc = new XmlDocument();
                    SubTotal_xmlDoc.LoadXml(nodeList[i].OuterXml.ToString());
                    XmlNodeList SubTotal_nodeList = SubTotal_xmlDoc.GetElementsByTagName("SubTotal");
                    decimal sub_total = 0M;

                    //

                    XmlDocument Total_xmlDoc = new XmlDocument();
                    Total_xmlDoc.LoadXml(nodeList[i].OuterXml.ToString());
                    XmlNodeList Total_nodeList = Total_xmlDoc.GetElementsByTagName("Total");
                    //decimal total = 0M;

                    if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
                    {
                        if (Total_nodeList[0] != null)
                        {
                            decimal.TryParse(Total_nodeList[0].InnerText.Trim(), out sub_total);
                        }
                    }
                    else
                    {
                        if (SubTotal_nodeList[0] != null)
                        {
                            decimal.TryParse(SubTotal_nodeList[0].InnerText.Trim(), out sub_total);
                        }
                    }
                    
                    //
                    for (byte j = 0; j < RateChargeViewModel_nodeList.Count; j++)
                    {
                        //sb.Append("Description " + RateChargeViewModel_nodeList[j]["Description"].InnerText.Trim() + " Amount " +
                        //    RateChargeViewModel_nodeList[j]["Amount"].InnerText.Trim());
                        breakdown_description = RateChargeViewModel_nodeList[j]["Description"].InnerText.Trim();
                        breakdown_cost = RateChargeViewModel_nodeList[j]["Amount"].InnerText.Trim();

                        //DB.Log("Breakdown", breakdown_description + " " + breakdown_cost);

                        if (!decimal.TryParse(breakdown_cost, out breakd_cost))
                        {
                            continue;
                        }
                        dlsPercentSum = breakd_cost * dlsMarkup.DLSMU;
                        //if (breakdown_description.Contains("Fuel") || breakdown_description.Contains("Lift Gate") ||
                        //    breakdown_description.Contains("Residential"))
                        //{
                        //    sb.Append(string.Concat(breakdown_description, "=$", breakdown_cost, "+"));
                        //}
                        //Appointment 10

                        #region Parse breakdown_description

                        Parse_breakdown_description(ref breakdown_description, ref breakd_cost, dlsPercentSum, sb);

                        #endregion
                    }

                    if (sub_total > 0M)
                    {
                        sb.Append(string.Concat("Freight $", sub_total, " - "));

                    }

                    //Limited Access Pickup - Constructions, Job Sites 35
                    //Limited Access Delivery - Constructions, Job Sites 35
                    if (sb.ToString().Length > 2)
                    {
                        //DB.Log("Breakdown", sb.ToString().Remove(sb.ToString().Length - 1));
                        pSheet.Cost_breakdown = sb.ToString().Remove(sb.ToString().Length - 2);
                    }
                    else
                    {
                        //DB.Log("Breakdown", "length was less than 3 chars");
                    }
                }
                catch (Exception m_ex)
                {
                    string str = m_ex.ToString();
                    //DB.Log("Breakdown", m_ex.ToString());
                }
                //Convention Exhibition Site Pickup 40




                if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
                {
                    //pSheet.Total = Total / 1.2M;
                    //pSheet.Total = Total / 1.6M;
                    pSheet.Total = Total;
                }
                else if (UserName == "Ben Franklin Crafts - Macon")
                {
                    pSheet.Total = Total * 0.7519M;
                }
                else
                {
                    pSheet.Total = Total;
                }

                //pSheet.CarrierName = nodeList[i].ChildNodes[3].InnerText.Trim();

                pSheet.CarrierName = nodeList[i]["CarrierName"].InnerText.Trim(); //.Replace("&", "+")
                //DB.Log("CarrierName, total", pSheet.CarrierName + " " + pSheet.Total);

                pSheet.ContractName = nodeList[i]["ContractName"].InnerText.Trim();

                pSheet.Scac = nodeList[i]["Scac"].InnerText.Trim();

                var xpo_helper = new XPO();
                bool can_get_XPO_rate = xpo_helper.Can_get_XPO_rate(ref quoteData.totalUnits);

                if (can_get_XPO_rate==true)
                {
                    //DB.Log("CarrierName, total 6to9", pSheet.CarrierName + " " + pSheet.Total);
                    if (pSheet.Scac == "CNWY")
                    {
                        // Do nothing
                        //DB.Log("CarrierName, total 6to9 CNWY", pSheet.CarrierName + " " + pSheet.Total);
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    //DB.Log("CarrierName, total not 6to9 units " + quoteData.totalUnits.ToString(), pSheet.CarrierName + " " + pSheet.Total);
                    // Do nothing
                }

                if (pSheet.Scac == "XGSI")
                    continue;
                
                Genera genera = new Genera();
                string[] genera_carriers_active = genera.Get_active_carriers("LTL");

                if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
                {
                    if (genera_carriers_active.Contains(pSheet.Scac))
                    {
                        // Do nothing
                    }
                    else
                    {
                        continue;
                    }

                    if (pSheet.Scac == "CTII" && quoteData.destZip == "21076")
                    {
                        continue;
                    }

                    if (pSheet.Scac == "RDFS" || pSheet.Scac == "CLNI" || pSheet.Scac == "FCSY")
                    {
                        continue;
                    }

                    if (pSheet.Scac == "FXFE" || pSheet.Scac == "FXNL")
                    {
                        //double freight_class = 0.0;
                        if (double.TryParse(quoteData.m_lPiece[0].FreightClass, out double freight_class))
                        {
                            if (freight_class > 110.0)
                            {
                                continue;
                            }
                        }
                    }
                }

                if (pSheet.Scac.Equals("VALC") && quoteData.AccessorialsObj.RESDEL == true)
                {
                    continue;
                }

                if (pSheet.Scac.Equals("CENF") &&
                    (quoteData.AccessorialsObj.RESPU == true || quoteData.AccessorialsObj.RESDEL == true || guaranteedService.Equals(true)))
                {
                    continue;
                }
                else
                {
                    // Do Nothing
                }

                //if (quoteData.mode == "NetNet" || quoteData.subdomain.Equals("spc"))
                //{
                //    // Do nothing
                //}
                //else
                //{
                //    if (pSheet.Scac.Equals("UPGF") || pSheet.Scac.Equals("OAKH"))
                //    {
                //        continue;
                //    }
                //    else
                //    {
                //        // Do Nothing
                //    }
                //}

                //
                // Do not show Brown Transfer Company
                if (pSheet.Scac.Equals("BRTC") || pSheet.Scac.Equals("NUMK"))
                {
                    continue;
                }

                if (pSheet.CarrierName.Contains("Numark"))
                {
                    continue;
                }

                if (guaranteedService.Equals(true))
                {
                    
                    //DB.Log("GLTL", pSheet.CarrierName);

                    if (pSheet.Scac.Equals("FXFE") || pSheet.Scac.Equals("FXNL"))
                    {
                        pSheet.Total += 20M;
                    }
                }

                #region Service Days

                pSheet.ServiceDays = 5; // Set default

                //byte.TryParse(nodeList[i].ChildNodes[18].InnerText.Trim(), out pSheet.ServiceDays);
                /*case ("NPME"):*/
                if (byte.TryParse(nodeList[i]["ServiceDays"].InnerText.Trim(), out pSheet.ServiceDays))
                {
                    if (pSheet.Scac.Equals("FCSY"))
                    {
                        pSheet.ServiceDays += 3;
                    }
                    else if (pSheet.Scac.Equals("CLNI"))
                    {
                        pSheet.ServiceDays += 3;
                    }
                    else if (pSheet.Scac.Equals("RDFS"))
                    {
                        pSheet.ServiceDays += 1;
                    }
                    else if (pSheet.Scac.Equals("CTII"))
                    {
                        if (quoteData.is_Genera_quote == true || quoteData.username == AppCodeConstants.un_genera)
                        {
                            // Do nothing
                        }
                        else
                        {
                            pSheet.ServiceDays += 1;
                        }
                    }
                    else if (pSheet.Scac.Equals("NPME"))
                    {
                        pSheet.ServiceDays += 2;
                    }
                }

                #endregion

                #region Accessorials

                // Tradeshow
                if (quoteData.AccessorialsObj.TRADEPU || quoteData.AccessorialsObj.TRADEDEL)
                {
                    // Show only the following 5 carriers //!pSheet.Scac.Equals("RNLO") && 
                    if (!pSheet.Scac.Equals("UPGF") && !pSheet.Scac.Equals("FXFE") &&
                        !pSheet.Scac.Equals("FXNL") && !pSheet.Scac.Equals("RDWY") && !pSheet.Scac.Equals("CNWY"))
                    {
                        continue;
                    }
                }

                if (quoteData.AccessorialsObj.RESPU.Equals(true) && pSheet.Scac.Equals("SEFL"))
                {
                    continue;
                }

                if (quoteData.AccessorialsObj.RESDEL.Equals(true) && pSheet.Scac.Equals("CLNI"))
                {
                    continue;
                }

                // Extreme length
                if (pSheet.Scac.Equals("UPGF") && isOverlengthUPS.Equals(true))
                {
                    continue;
                }

                if (pSheet.Scac.Equals("RPMI") && HelperFuncs.IsOverlength_RPM(ref quoteData.m_lPiece, 48, 48, 48).Equals(true))
                {
                    continue;
                }

                if (pSheet.Scac.Equals("VSXP"))
                {
                    continue;
                }
                if (quoteData.isHHG.Equals(true) || quoteData.isUSED.Equals(true))
                {
                    if (pSheet.Scac.Equals("CTII"))
                    {
                        continue;
                    }
                }

                if (quoteData.isHHG.Equals(true))
                {
                    if (pSheet.Scac.Equals("FCSY") || pSheet.Scac.Equals("CLNI"))
                    {
                        continue;
                    }
                }


                #endregion

                // Do not show RL for username pbisupply
                if (quoteData.username.Equals("pbisupply") && pSheet.Scac.Equals("RNLO"))
                {
                    continue;
                }

                if (pSheet.Scac.Equals("NEMF"))
                {
                    continue;

                }

                #region Add results while adding DLS Worldwide carrier only once
                DB.Log("regular adding not DLS Worldwide",
                       "before add results");
                // Add DLS Worldwide carrier only once
                if (!pSheet.Scac.Equals("DRRQ"))
                {


                    //if (GS.Equals("GLTL"))
                    //{

                    //    DB.Log("added to pricesheets GLTL", pSheet.CarrierName);
                    //}

                    if (pSheet.CarrierName.Contains("DLS Worldwide"))
                    {
                        continue;
                        //DB.Log("regular adding DLS Worldwide", 
                        //    string.Concat("car name: ", pSheet.CarrierName, " scac: ", pSheet.Scac));
                    }
                    //else
                    //{
                    //    DB.Log("regular adding not DLS Worldwide", 
                    //        string.Concat("car name: ", pSheet.CarrierName, " scac: ", pSheet.Scac));
                    //}


                    //DB.Log("regular adding not DLS Worldwide",
                    //    string.Concat("car name: ", pSheet.CarrierName, " scac: ", pSheet.Scac));

                    dlsPricesheets.Add(pSheet);
                }
                else if (guaranteedService.Equals(true) && gotGLTL_DLS_Worldwide.Equals(false))
                {
                    if (pSheet.CarrierName.Contains("DLS Worldwide"))
                    {
                        continue;
                        //DB.Log("regular adding DLS Worldwide", 
                        //    string.Concat("car name: ", pSheet.CarrierName, " scac: ", pSheet.Scac));
                    }
                    //Scac.Equals("DRRQ")
                    dlsPricesheets.Add(pSheet);
                    //DB.Log("GLTL adding GLTL_DLS_Worldwide", pSheet.Scac);
                }
                else if (!guaranteedService.Equals(true) && gotDLS_Worldwide.Equals(false))
                {
                    if (pSheet.CarrierName.Contains("DLS Worldwide"))
                    {
                        continue;
                        //DB.Log("regular adding DLS Worldwide", 
                        //    string.Concat("car name: ", pSheet.CarrierName, " scac: ", pSheet.Scac));
                    }
                    //Scac.Equals("DRRQ")
                    dlsPricesheets.Add(pSheet);
                    //DB.Log("adding DLS_Worldwide", pSheet.Scac);
                }

                
                if (guaranteedService.Equals(true) && pSheet.Scac.Equals("DRRQ"))
                {
                    gotGLTL_DLS_Worldwide = true;
                   
                }
                else if (pSheet.Scac.Equals("DRRQ"))
                {
                    gotDLS_Worldwide = true;
                    
                }

                #endregion

            }
        }

        #endregion

        #region Parse_breakdown_description

        private static void Parse_breakdown_description(ref string breakdown_description, ref decimal breakd_cost,
            decimal dlsPercentSum, StringBuilder sb)
        {
            if (breakdown_description.Contains("Fuel") || breakdown_description.Contains("Lift Gate") ||
                breakdown_description.Contains("Residential") || breakdown_description.Contains("Linehaul")
                || breakdown_description.Contains("Limited Access") || breakdown_description.Contains("Convention")
                || breakdown_description.Contains("Inside Delivery") || breakdown_description.Contains("Appointment"))
            {
                if (breakdown_description == "Lift Gate Pickup")
                {
                    breakdown_description = "Liftgate Pickup";
                }
                else if (breakdown_description == "Lift Gate Delivery")
                {
                    breakdown_description = "Liftgate Delivery";
                }
                else if (breakdown_description == "Residential Pickup")
                {
                    breakdown_description = "Residential Pickup";
                }
                else if (breakdown_description == "Residential Delivery")
                {
                    breakdown_description = "Residential Delivery";
                }
                else if (breakdown_description.Contains("Military") && breakdown_description.Contains("Pickup"))
                {
                    breakdown_description = "Military Pickup";
                }
                else if (breakdown_description.Contains("Military") && breakdown_description.Contains("Delivery"))
                {
                    breakdown_description = "Military Delivery";
                }
                else if (breakdown_description.Contains("Government") && breakdown_description.Contains("Pickup"))
                {
                    breakdown_description = "Government Pickup";
                }
                else if (breakdown_description.Contains("Government") && breakdown_description.Contains("Delivery"))
                {
                    breakdown_description = "Government Delivery";
                }
                else if (breakdown_description.Contains("Limited Access Pickup"))
                {
                    breakdown_description = "Limited Access Pickup";
                }
                else if (breakdown_description.Contains("Limited Access Delivery"))
                {
                    breakdown_description = "Limited Access Delivery";
                }
                else if (breakdown_description.Contains("Convention Exhibition Site Pickup"))
                {
                    breakdown_description = "Tradeshow Pickup";
                }
                else if (breakdown_description.Contains("Convention Exhibition Site Delivery"))
                {
                    breakdown_description = "Tradeshow Delivery";
                }
                else if (breakdown_description.Contains("Inside Delivery"))
                {
                    breakdown_description = "Inside Delivery";
                }
                else if (breakdown_description.Contains("Appointment"))
                {
                    breakdown_description = "Appointment";
                }
                else if (breakdown_description == "Total Line Haul")
                {
                    breakdown_description = "Freight";
                }
                else if (breakdown_description == "Linehaul Fee")
                {
                    breakdown_description = "Linehaul Fee";
                }
                else
                {
                    // Do nothing
                    //DB.Log("Breakdown accessorial not found", breakdown_description);
                }
               
                if (dlsPercentSum > 0M)
                {
                    breakd_cost = breakd_cost + dlsPercentSum;
                }
                breakd_cost = Math.Round(breakd_cost, 2);

                sb.Append(string.Concat(breakdown_description, " $", breakd_cost,
                   " - "));
            }
        }

        #endregion

        #region Parse_results_cust_rates

        public void Parse_results_cust_rates(ref List<dlsPricesheet> dlsPricesheets, ref string doc)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(doc);

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("PricesheetViewModel");

            decimal temp1 = 0M;

            string breakdown_description = "", breakdown_cost = "";

            for (byte i = 0; i < nodeList.Count; i++)
            {
                //1PSI Cost RDFS The Exchange ASP
                if (nodeList[i]["ContractName"].InnerText.Trim().Equals("1PSI Cust PNW 100100 The Exchange"))
                {
                    
                    if (decimal.TryParse(nodeList[i]["Total"].InnerText.Trim(), out temp1))
                    {
                        dlsPricesheet pSheet = new dlsPricesheet();
                        //
                        try
                        {
                            StringBuilder sb = new StringBuilder();
                            // Get accessorials breakdown
                            XmlDocument accessorials_xmlDoc = new XmlDocument();
                            accessorials_xmlDoc.LoadXml(nodeList[i].OuterXml.ToString());
                            XmlNodeList RateChargeViewModel_nodeList = accessorials_xmlDoc.GetElementsByTagName("RateChargeViewModel");

                            for (byte j = 0; j < RateChargeViewModel_nodeList.Count; j++)
                            {
                                //sb.Append("Description " + RateChargeViewModel_nodeList[j]["Description"].InnerText.Trim() + " Amount " +
                                //    RateChargeViewModel_nodeList[j]["Amount"].InnerText.Trim());
                                breakdown_description = RateChargeViewModel_nodeList[j]["Description"].InnerText.Trim();
                                breakdown_cost = RateChargeViewModel_nodeList[j]["Amount"].InnerText.Trim();

                                //DB.Log("The exchange Breakdown Description", breakdown_description);
                                //DB.Log("The exchange Breakdown Cost", breakdown_cost);

                                if (breakdown_description.Equals("ITEMNAME"))
                                {
                                    //pSheet.base_rate = breakdown_cost;
                                    double.TryParse(breakdown_cost, out pSheet.base_rate);
                                }

                                //if (breakdown_description.Contains("Fuel") || breakdown_description.Contains("Lift Gate") ||
                                //    breakdown_description.Contains("Residential"))
                                //{
                                //    sb.Append(string.Concat(breakdown_description, "=$", breakdown_cost, "+"));
                                //}
                                if (breakdown_description.Contains("Fuel") || breakdown_description.Contains("Lift Gate") ||
                                    breakdown_description.Contains("Residential") || breakdown_description.Contains("Total Line Haul"))
                                {
                                    if (breakdown_description == "Lift Gate Pickup")
                                    {
                                        breakdown_description = "LGPU";
                                    }
                                    if (breakdown_description == "Lift Gate Delivery")
                                    {
                                        breakdown_description = "LGDEL";
                                    }
                                    if (breakdown_description == "Residential Pickup")
                                    {
                                        breakdown_description = "RESPU";
                                    }
                                    if (breakdown_description == "Residential Delivery")
                                    {
                                        breakdown_description = "RESDEL";
                                    }
                                    else if (breakdown_description == "Total Line Haul")
                                    {
                                        breakdown_description = "Freight";
                                    }
                                    else
                                    {
                                        // Do nothing
                                    }
                                   
                                    sb.Append(string.Concat(breakdown_description, " $", breakdown_cost,
                                       " - "));
                                }
                            }

                            //DB.Log("The exchange Breakdown", sb.ToString().Remove(sb.ToString().Length - 1));
                            pSheet.Cost_breakdown = sb.ToString().Remove(sb.ToString().Length - 2);
                        }
                        catch (Exception m_ex)
                        {
                            string str = m_ex.ToString();
                            //DB.Log("The exchange Breakdown", m_ex.ToString());
                        }
                       
                        pSheet.Total = temp1;
                      
                        pSheet.CarrierName = "AAFES Sell";

                        pSheet.ContractName = nodeList[i]["ContractName"].InnerText.Trim();

                        pSheet.Scac = nodeList[i]["Scac"].InnerText.Trim();

                        #region Service Days

                        pSheet.ServiceDays = 5; // Set default

                        if (byte.TryParse(nodeList[i]["ServiceDays"].InnerText.Trim(), out pSheet.ServiceDays))
                        {

                        }

                        #endregion

                        dlsPricesheets.Add(pSheet);

                    }
                }
                else if (nodeList[i]["ContractName"].InnerText.Trim().Equals("1PSI Cost RDFS The Exchange ASP"))
                {
                    if (decimal.TryParse(nodeList[i]["Total"].InnerText.Trim(), out temp1))
                    {
                        dlsPricesheet pSheet = new dlsPricesheet();

                        pSheet.Total = temp1;
                        //DB.Log("The Exchange XPO total", pSheet.Total.ToString());

                        pSheet.CarrierName = "Roadrunner The Exchange";

                        //pSheet.ContractName = nodeList[i]["ContractName"].InnerText.Trim();

                        pSheet.Scac = nodeList[i]["Scac"].InnerText.Trim();

                        #region Service Days

                        pSheet.ServiceDays = 5; // Set default

                        if (byte.TryParse(nodeList[i]["ServiceDays"].InnerText.Trim(), out pSheet.ServiceDays))
                        {

                        }

                        #endregion

                        dlsPricesheets.Add(pSheet);
                    }
                }
                else if (nodeList[i]["CarrierName"].InnerText.Trim().Equals("XPO Logistics Freight, Inc."))
                {
                    if (decimal.TryParse(nodeList[i]["Total"].InnerText.Trim(), out temp1))
                    {
                        dlsPricesheet pSheet = new dlsPricesheet();

                        pSheet.Total = temp1;
                        //DB.Log("The Exchange XPO total", pSheet.Total.ToString());

                        pSheet.CarrierName = "XPO AAFES DC Locations";

                        //pSheet.ContractName = nodeList[i]["ContractName"].InnerText.Trim();

                        pSheet.Scac = nodeList[i]["Scac"].InnerText.Trim();

                        #region Service Days

                        pSheet.ServiceDays = 5; // Set default

                        if (byte.TryParse(nodeList[i]["ServiceDays"].InnerText.Trim(), out pSheet.ServiceDays))
                        {

                        }

                        #endregion

                        dlsPricesheets.Add(pSheet);
                    }
                }
                else
                {
                    //DB.Log("Not The Exchange", "Not The Exchange");
                }
            }
        }

        #endregion
    }
}