#region Using

using gcmAPI.Models.Public.LTL;
using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;

#endregion

namespace gcmAPI.Models.LTL
{
    public class Helper
    {

        #region Set_parameters
        // Genera
        public void Set_parameters(ref LTLQuoteRequest ltl_quote_request, ref QuoteData quoteData)
        {
            if (ltl_quote_request.totalCube == null)
            {
                // Do nothing
            }
            else
            {
                quoteData.totalCube = (double)ltl_quote_request.totalCube;
            }

            if (ltl_quote_request.linealFeet == null)
            {
                // Do nothing
            }
            else
            {
                quoteData.linealFeet = (double)ltl_quote_request.linealFeet;
            }
            

            #region pickup date

            quoteData.puDate = ltl_quote_request.pickupDate;

            #endregion

            #region Boolean flags

            for(byte i=0;i<ltl_quote_request.items.Count;i++)
            {
                if(ltl_quote_request.items[i].hazmat == true)
                {
                    quoteData.isHazmat = true;
                    break;
                }
                else
                {
                    // Do nothing
                }
            }

            if (ltl_quote_request.items[0].length > 0)
            {
                quoteData.hasDimensions = true;
            }

            if (ltl_quote_request.items[0].freightClass > 0)
            {
                quoteData.hasFreightClass = true;
            }

            for (byte i = 0; i < ltl_quote_request.items.Count; i++)
            {
                if (ltl_quote_request.items[i].commodity == "HHG")
                {
                    quoteData.isHHG = true;
                    break;
                }
                else
                {
                    // Do nothing
                }
            }

            for (byte i = 0; i < ltl_quote_request.items.Count; i++)
            {
                if (ltl_quote_request.items[i].commodity == "USED")
                {
                    quoteData.isUSED = true;
                    break;
                }
                else
                {
                    // Do nothing
                }
            }

            double total_weight = 0.0;
            for (byte i = 0; i < ltl_quote_request.items.Count; i++)
            {
                if (ltl_quote_request.items[i].weight > 0)
                {
                    total_weight += (double)ltl_quote_request.items[i].weight;
                }
                else
                {
                    // Do nothing
                }
            }

            if(total_weight < 500 && quoteData.isHHG == true)
            {
                quoteData.isHHG_AndUnder500 = true;
            }

            #endregion

            //quoteData.username = username;

            //DB.Log("username", quoteData.username.ToString());

            quoteData.is_like_NetNet = Is_login_like_NetNet(quoteData.username);

            quoteData.is_good_USF_Holland_orig_state = Is_good_USF_Holland_orig_state(ref quoteData);

            #region Origin and Destination

            string[] split_arr;

            split_arr = ltl_quote_request.originZip.Trim().Split(' ');

            if(split_arr.Length == 2)
            {
                quoteData.origZip = split_arr[0] + split_arr[1];
            }
            else
            {
                quoteData.origZip = ltl_quote_request.originZip;
            }

            //if (ltl_quote_request.originZip)

            split_arr = ltl_quote_request.destinationZip.Trim().Split(' ');

            if (split_arr.Length == 2)
            {
                quoteData.destZip = split_arr[0] + split_arr[1];
            }
            else
            {
                quoteData.destZip = ltl_quote_request.destinationZip;
            }


            //quoteData.destZip = ltl_quote_request.destinationZip;
            quoteData.origCity = ltl_quote_request.originCity;
            quoteData.destCity = ltl_quote_request.destinationCity;
            quoteData.origState = ltl_quote_request.originState;
            quoteData.destState = ltl_quote_request.destinationState;

            #endregion

            quoteData.mode = "ws";

            int numOfUnits;

            SetLTLPiecesObject(out quoteData.numOfUnitsPieces, out numOfUnits, ref ltl_quote_request, ref quoteData);

            quoteData.totalUnits = numOfUnits;

            #region Accessorials

            quoteData.AccessorialsObj = new HelperFuncs.AccessorialsObj();

            HelperFuncs.setAccessorialsObject(ref quoteData, ref quoteData.AccessorialsObj,
                ltl_quote_request.additionalServices.RSP,
                ltl_quote_request.additionalServices.RSD,
                ltl_quote_request.additionalServices.CSP,
                ltl_quote_request.additionalServices.CSD,
                ltl_quote_request.additionalServices.ISD,
                ltl_quote_request.additionalServices.AMP,
                ltl_quote_request.additionalServices.AMD,
                ltl_quote_request.additionalServices.TSP,
                ltl_quote_request.additionalServices.TSD,
                ltl_quote_request.additionalServices.TGP,
                ltl_quote_request.additionalServices.TGD,
                false,
                false,
                false,
                false
                );

            #endregion
        }

        #endregion

        #region setParameters
        // Genera
        public void setParameters(ref FormDataCollection form, ref QuoteData quoteData)
        {
           

            if (string.IsNullOrEmpty(form.Get("linealFeet")))
            {
                // Do nothing
            }
            else
            {
                double.TryParse(form.Get("linealFeet"), out quoteData.linealFeet);
            }

            #region pickup date

            DateTime pickupDate;
            if (!DateTime.TryParse(form.Get("pickupDate"), out pickupDate))
            {
                pickupDate = DateTime.Today;
            }
            quoteData.puDate = pickupDate;

            #endregion

            #region Boolean flags

            try
            {
                if (!bool.TryParse(form.Get("is_AAFES_quote"), out quoteData.is_AAFES_quote))
                {
                    quoteData.is_AAFES_quote = false;
                }
            }
            catch (Exception e)
            {
                DB.LogException("bool.TryParse(form.Get(is_AAFES_quote", e.ToString());
            }
            if (!bool.TryParse(form.Get("is_Genera_quote"), out quoteData.is_Genera_quote))
            {
                quoteData.is_Genera_quote = false;
            }
            if (!bool.TryParse(form.Get("isHazMat"), out quoteData.isHazmat))
            {
                quoteData.isHazmat = false;
            }
            if (!bool.TryParse(form.Get("hasDimensions"), out quoteData.hasDimensions))
            {
                quoteData.hasDimensions = false;
            }
            if (!bool.TryParse(form.Get("hasFreightClass"), out quoteData.hasFreightClass))
            {
                quoteData.hasFreightClass = false;
            }
            if (!bool.TryParse(form.Get("showDLSRates"), out quoteData.showDLSRates))
            {
                quoteData.showDLSRates = false;
            }
            if (!bool.TryParse(form.Get("isDUR"), out quoteData.isDUR))
            {
                quoteData.isDUR = false;
            }
            if (!bool.TryParse(form.Get("isAssociationID_5"), out quoteData.isAssociationID_5))
            {
                quoteData.isAssociationID_5 = false;
            }
            if (!bool.TryParse(form.Get("isCommodity"), out quoteData.isCommodity))
            {
                quoteData.isCommodity = false;
            }
            if (!bool.TryParse(form.Get("isCommodityLkupHHG"), out quoteData.isCommodityLkupHHG))
            {
                if (form.Get("isCommodityLkupHHG") != null && form.Get("isCommodityLkupHHG").Equals("1"))
                {
                    quoteData.isCommodityLkupHHG = true;
                }
                else
                {
                    quoteData.isCommodityLkupHHG = false;
                }
            }
            if (!bool.TryParse(form.Get("isHHG"), out quoteData.isHHG))
            {
                quoteData.isHHG = false;
            }
            if (!bool.TryParse(form.Get("isUSED"), out quoteData.isUSED))
            {
                quoteData.isUSED = false;
            }
            if (!bool.TryParse(form.Get("q_isHHG_AndUnder500"), out quoteData.isHHG_AndUnder500))
            {
                quoteData.isHHG_AndUnder500 = false;
            }
            if (!bool.TryParse(form.Get("isUserVanguard"), out quoteData.isUserVanguard))
            {
                quoteData.isUserVanguard = false;
            }

            #endregion

            quoteData.username = form.Get("username").ToLower();

            DB.Log("username", quoteData.username.ToString());

            if (string.IsNullOrEmpty(form.Get("api_username")))
            {
                quoteData.api_username = "";
            }
            else
            {
                quoteData.api_username = form.Get("api_username").ToLower();
                DB.Log("quoteData.api_username", quoteData.api_username.ToString());
            }

            quoteData.is_like_NetNet = Is_login_like_NetNet(quoteData.username);

            quoteData.is_good_USF_Holland_orig_state = Is_good_USF_Holland_orig_state(ref quoteData);

            if (quoteData.username.Equals("durachem") || quoteData.username.Equals("jcma512h"))
            {
                quoteData.is_dura_logic = true;
            }

            DB.Log("is_dura_logic", quoteData.is_dura_logic.ToString());

            quoteData.txtCommodityCubicFeet = form.Get("txtCommodityCubicFeet");
            quoteData.txtAAFES_Quote_PO = form.Get("txtAAFES_Quote_PO");

            #region Origin and Destination

            quoteData.origZip = form.Get("q_OPCode");
            quoteData.destZip = form.Get("q_DPCode");
            quoteData.origCity = form.Get("origCity");
            quoteData.destCity = form.Get("destCity");
            quoteData.origState = form.Get("origState");
            quoteData.destState = form.Get("destState");

            #endregion

            quoteData.totalCube = 0.0;
            double.TryParse(form.Get("q_CubicFeet"), out quoteData.totalCube);
            DB.Log("quoteData.totalCube", quoteData.totalCube.ToString());

            if (string.IsNullOrEmpty(form.Get("totalCube")))
            {
                // Do nothing
                DB.Log("total cube", "null or empty");
            }
            else
            {
                double.TryParse(form.Get("totalCube"), out quoteData.totalCube);
                DB.Log("total cube", quoteData.totalCube.ToString());
            }

            quoteData.subdomain = form.Get("subdomain");

            quoteData.mode = form.Get("mode");

            quoteData.hasFreightClass = Convert.ToBoolean(form.Get("hasFreightClass"));

            DB.Log("quoteData.hasFreightClass", quoteData.hasFreightClass.ToString());

            int numOfUnits;

            SetLTLPiecesObject(out quoteData.numOfUnitsPieces, out numOfUnits, ref form, ref quoteData);

            quoteData.totalUnits = numOfUnits;

            #region Accessorials

            quoteData.AccessorialsObj = new HelperFuncs.AccessorialsObj();

            HelperFuncs.setAccessorialsObject(ref quoteData, ref quoteData.AccessorialsObj,
                Convert.ToBoolean(form.Get("q_ResPick")),
                Convert.ToBoolean(form.Get("q_ResDel")),
                Convert.ToBoolean(form.Get("q_ConstPick")),
                Convert.ToBoolean(form.Get("q_ConstDel")),
                Convert.ToBoolean(form.Get("q_InsDel")),
                Convert.ToBoolean(form.Get("q_AppPick")),
                Convert.ToBoolean(form.Get("q_AppDel")),
                Convert.ToBoolean(form.Get("q_TradePick")),
                Convert.ToBoolean(form.Get("q_TradeDel")),
                Convert.ToBoolean(form.Get("q_TailPick")),
                Convert.ToBoolean(form.Get("q_TailDel")),
                Convert.ToBoolean(form.Get("q_MiliPick")),
                Convert.ToBoolean(form.Get("q_MiliDel")),
                Convert.ToBoolean(form.Get("q_GovPick")),
                Convert.ToBoolean(form.Get("q_GovDel"))
                );

            #endregion
        }

        #endregion

        #region SetLTLPiecesObject
        /// <summary>
        /// Intention here is to work with the inputs on this page same as the objects on WS, this way adding a new carrier to both files is easier
        /// </summary>
        private void SetLTLPiecesObject(out int numOfUnitsPieces, out int numOfUnits, ref FormDataCollection form,
            ref QuoteData quoteData)
        {
            numOfUnitsPieces = 0;
            numOfUnits = 0;

            int Quantity = 0, intResult, pieces = 0;
            double parsedDouble;

            //
            quoteData.m_lPieceList = new List<LTLPiece>();
            quoteData.m_lPiece_actual_List = new List<LTLPiece>();
            //

            //
            int Units = 0, Pieces = 0;

            try
            {
                for (byte i = 1; i <= 4; i++)
                {
                    if (!string.IsNullOrEmpty(form.Get("q_Weight" + i.ToString())))
                    {
                        if (form.Get("q_Piece" + i.ToString()) != null)
                        {
                            int.TryParse(form.Get("q_Piece" + i.ToString()), out pieces);
                        }

                        Units = 0;
                        Pieces = 0;
                        if (form.Get("commodity_unit" + i.ToString()) != null && form.Get("commodity_unit" + i.ToString()).Length > 0 &&
                            int.TryParse(form.Get("commodity_unit" + i.ToString()), out intResult))
                        {
                            numOfUnitsPieces += intResult;
                            numOfUnits += intResult;
                            Quantity = intResult;

                            //
                            Units = intResult;

                            Pieces = pieces;
                        }
                        else if (form.Get("q_Piece" + i.ToString()) != null && form.Get("q_Piece" + i.ToString()).Length > 0 &&
                            int.TryParse(form.Get("q_Piece" + i.ToString()), out intResult))
                        {
                            numOfUnitsPieces += intResult;
                            Quantity = intResult;

                            //
                            Pieces = intResult;
                        }
                        else
                        {
                            numOfUnitsPieces += 1;
                            Quantity = 1;

                            Pieces = pieces;
                        }

                        LTLPiece ltlPiece = new LTLPiece();
                        LTLPiece ltlPiece_actual = new LTLPiece();

                        // Set class weight and quantity
                        //---------------------------------------------------------------------------------

                        if (quoteData.isHHG.Equals(true))
                        {
                            ltlPiece.FreightClass = "150";
                            ltlPiece_actual.FreightClass = "150";
                        }
                        else
                        {
                            ltlPiece.FreightClass = form.Get("q_Class" + i.ToString());
                            ltlPiece_actual.FreightClass = form.Get("q_Class" + i.ToString());
                        }

                        //ltlPiece.FreightClass = Request.QueryString["q_Class" + i.ToString()];

                        // Check if this rate has freight class
                        if (i.Equals(1) && (string.IsNullOrEmpty(ltlPiece.FreightClass) || ltlPiece.FreightClass.Equals("-1")))
                        {
                            quoteData.hasFreightClass = false;
                        }

                        if (i.Equals(1) && form.Get("q_isHHG_AndUnder500") != null && form.Get("q_isHHG_AndUnder500") == "true" &&
                            form.Get("q_ExtraWeight") != null && double.TryParse(form.Get("q_ExtraWeight"), out quoteData.extraWeight) &&
                            quoteData.extraWeight > 0)
                        {
                            // Extra weight
                            ltlPiece.Weight = Convert.ToDouble(form.Get("q_Weight" + i.ToString())) + quoteData.extraWeight;
                            quoteData.totalWeight += ltlPiece.Weight;
                            ltlPiece_actual.Weight = Convert.ToDouble(form.Get("q_Weight" + i.ToString()));
                        }
                        else
                        {
                            // Regular case
                            ltlPiece.Weight = Convert.ToDouble(form.Get("q_Weight" + i.ToString()));
                            quoteData.totalWeight += ltlPiece.Weight;
                            ltlPiece_actual.Weight = Convert.ToDouble(form.Get("q_Weight" + i.ToString()));
                        }

                        ltlPiece.Quantity = Quantity;
                        ltlPiece.Units = Units;
                        ltlPiece.Pieces = Pieces;

                        ltlPiece_actual.Quantity = Quantity;
                        ltlPiece_actual.Units = Units;
                        ltlPiece_actual.Pieces = Pieces;

                        //---------------------------------------------------------------------------------

                        #region Set dimensions

                        // If there are no dims they are set to 0 by default, otherwise set dims
                        if (form.Get("q_Length" + i.ToString()) != null && form.Get("q_Length" + i.ToString()) != "NaN"
                        && double.TryParse(form.Get("q_Length" + i.ToString()), out parsedDouble))
                        {
                            ltlPiece.Length = parsedDouble;
                            ltlPiece_actual.Length = parsedDouble;
                        }
                        if (form.Get("q_Width" + i.ToString()) != null && form.Get("q_Width" + i.ToString()) != "NaN"
                            && double.TryParse(form.Get("q_Width" + i.ToString()), out parsedDouble))
                        {
                            ltlPiece.Width = parsedDouble;
                            ltlPiece_actual.Width = parsedDouble;
                        }
                        if (form.Get("q_Height" + i.ToString()) != null && form.Get("q_Height" + i.ToString()) != "NaN"
                            && double.TryParse(form.Get("q_Height" + i.ToString()), out parsedDouble))
                        {
                            ltlPiece.Height = parsedDouble;
                            ltlPiece_actual.Height = parsedDouble;
                        }

                        #endregion

                        ltlPiece.HazMat = Convert.ToBoolean(form.Get("q_HazMat" + i.ToString()));
                        ltlPiece_actual.HazMat = Convert.ToBoolean(form.Get("q_HazMat" + i.ToString()));

                        // Commodity
                        if (form.Get("q_Commodity" + i.ToString()) != null)
                        {
                            ltlPiece.Commodity = form.Get("q_Commodity" + i.ToString());
                            ltlPiece_actual.Commodity = form.Get("q_Commodity" + i.ToString());
                        }

                        quoteData.m_lPieceList.Add(ltlPiece);
                        quoteData.m_lPiece_actual_List.Add(ltlPiece_actual);

                    }
                }

                quoteData.m_lPiece = quoteData.m_lPieceList.ToArray();
                quoteData.m_lPiece_actual = quoteData.m_lPiece_actual_List.ToArray();

                #region For testing
                //for (int i = 0; i < m_lPieceList.Count; i++)
                //{
                //    DB.Log("myTest", m_lPieceList[i].FreightClass + " " + m_lPieceList[i].Weight.ToString() +
                //    " quantity: " + m_lPieceList[i].Quantity.ToString() + " length: " + m_lPieceList[i].Length.ToString()
                //    + " width: " + m_lPieceList[i].Width.ToString() + " height: " + m_lPieceList[i].Height.ToString(), "");
                //}
                #endregion
            }
            catch (Exception e)
            {
                DB.LogException("SetLTLPiecesObject", e.ToString());
            }
        }

        #endregion

        #region SetLTLPiecesObject
        /// <summary>
        /// Genera
        /// </summary>
        private void SetLTLPiecesObject(out int numOfUnitsPieces, out int numOfUnits, ref LTLQuoteRequest ltl_quote_request,
            ref QuoteData quoteData)
        {
            //

            numOfUnitsPieces = 0;
            numOfUnits = 0;

            int Quantity = 0,pieces = 0;// intResult, 
            //double parsedDouble;

            //
            quoteData.m_lPieceList = new List<LTLPiece>();
            quoteData.m_lPiece_actual_List = new List<LTLPiece>();
            //

            //
            int Units = 0, Pieces = 0;

            try
            {
                for (byte i = 1; i <= ltl_quote_request.items.Count; i++)
                {
                    if (ltl_quote_request.items[i-1].weight > 0)
                    {
                        if (ltl_quote_request.items[i - 1].pieces > 0)
                        {
                            pieces= ltl_quote_request.items[i - 1].pieces;
                        }

                        Units = 0;
                        Pieces = 0;
                        if (ltl_quote_request.items[i - 1].units > 0)
                        {
                            numOfUnitsPieces += ltl_quote_request.items[i - 1].units;
                            numOfUnits += ltl_quote_request.items[i - 1].units;
                            Quantity = ltl_quote_request.items[i - 1].units;

                            //
                            Units = ltl_quote_request.items[i - 1].units;

                            Pieces = pieces;
                        }
                        else if (ltl_quote_request.items[i - 1].pieces > 0)
                        {
                            numOfUnitsPieces += ltl_quote_request.items[i - 1].pieces;
                            Quantity = ltl_quote_request.items[i - 1].pieces;

                            //
                            Pieces = ltl_quote_request.items[i - 1].pieces;
                        }
                        else
                        {
                            numOfUnitsPieces += 1;
                            Quantity = 1;

                            Pieces = pieces;
                        }

                        LTLPiece ltlPiece = new LTLPiece();
                        LTLPiece ltlPiece_actual = new LTLPiece();

                        // Set class weight and quantity
                        //---------------------------------------------------------------------------------

                        if (quoteData.isHHG.Equals(true))
                        {
                            ltlPiece.FreightClass = "150";
                            ltlPiece_actual.FreightClass = "150";
                            //DB.Log("hhg", "");
                        }
                        else
                        {
                            ltlPiece.FreightClass = ltl_quote_request.items[i - 1].freightClass.ToString();
                            ltlPiece_actual.FreightClass = ltl_quote_request.items[i - 1].freightClass.ToString();
                            //DB.Log("not hhg", "");
                        }

                        //ltlPiece.FreightClass = Request.QueryString["q_Class" + i.ToString()];

                        // Check if this rate has freight class
                        if (i.Equals(1) && (string.IsNullOrEmpty(ltlPiece.FreightClass) || ltlPiece.FreightClass.Equals("-1")))
                        {
                            quoteData.hasFreightClass = false;
                        }

                        if (i.Equals(1) && quoteData.isHHG_AndUnder500 == true && quoteData.extraWeight > 0)
                        {
                            // Extra weight
                            ltlPiece.Weight = (double)ltl_quote_request.items[i - 1].weight + quoteData.extraWeight;
                            quoteData.totalWeight += ltlPiece.Weight;

                            ltlPiece_actual.Weight = (double)ltl_quote_request.items[i - 1].weight;
                            //ltlPiece_actual.Weight = (int)ltlPiece_actual.Weight;
                        }
                        else
                        {
                            // Regular case
                            ltlPiece.Weight = (double)ltl_quote_request.items[i - 1].weight;
                            quoteData.totalWeight += ltlPiece.Weight;

                            ltlPiece_actual.Weight = (double)ltl_quote_request.items[i - 1].weight;
                            //ltlPiece_actual.Weight = (int)ltlPiece_actual.Weight;
                        }

                        ltlPiece.Quantity = Quantity;
                        ltlPiece.Units = Units;
                        ltlPiece.Pieces = Pieces;

                        ltlPiece_actual.Quantity = Quantity;
                        ltlPiece_actual.Units = Units;
                        ltlPiece_actual.Pieces = Pieces;

                        //---------------------------------------------------------------------------------

                        #region Set dimensions

                        // If there are no dims they are set to 0 by default, otherwise set dims
                        if (ltl_quote_request.items[i - 1].length > 0)
                        {
                            ltlPiece.Length = (double)ltl_quote_request.items[i - 1].length;
                            ltlPiece_actual.Length = (double)ltl_quote_request.items[i - 1].length;
                        }
                        if (ltl_quote_request.items[i - 1].width > 0)
                        {
                            ltlPiece.Width = (double)ltl_quote_request.items[i - 1].width;
                            ltlPiece_actual.Width = (double)ltl_quote_request.items[i - 1].width;
                        }
                        if (ltl_quote_request.items[i - 1].height > 0)
                        {
                            ltlPiece.Height = (double)ltl_quote_request.items[i - 1].height;
                            ltlPiece_actual.Height = (double)ltl_quote_request.items[i - 1].height;
                        }

                        #endregion

                        ltlPiece.HazMat = quoteData.isHazmat;
                        ltlPiece_actual.HazMat = quoteData.isHazmat;

                        // Commodity
                        //if (form.Get("q_Commodity" + i.ToString()) != null)
                        //{
                        //    ltlPiece.Commodity = form.Get("q_Commodity" + i.ToString());
                        //}

                        ltlPiece.Commodity = ltl_quote_request.items[i - 1].commodity;
                        ltlPiece_actual.Commodity = ltl_quote_request.items[i - 1].commodity;

                        quoteData.m_lPieceList.Add(ltlPiece);
                        quoteData.m_lPiece_actual_List.Add(ltlPiece_actual);
                    }
                }

                quoteData.m_lPiece = quoteData.m_lPieceList.ToArray();
                quoteData.m_lPiece_actual = quoteData.m_lPiece_actual_List.ToArray();

                #region For testing
                //for (int i = 0; i < m_lPieceList.Count; i++)
                //{
                //    DB.Log("myTest", m_lPieceList[i].FreightClass + " " + m_lPieceList[i].Weight.ToString() +
                //    " quantity: " + m_lPieceList[i].Quantity.ToString() + " length: " + m_lPieceList[i].Length.ToString()
                //    + " width: " + m_lPieceList[i].Width.ToString() + " height: " + m_lPieceList[i].Height.ToString(), "");
                //}
                #endregion
            }
            catch (Exception e)
            {
                DB.LogException("SetLTLPiecesObject", e.ToString());
            }
        }

        #endregion

        #region Is_login_like_NetNet

        private bool Is_login_like_NetNet(string username)
        {
            bool is_like_NetNet = false;
            if (username == "aes25" || username == "aes30" || username == "aes33" || username == "aes35" ||
                username == "aes40" || username == "aes45")
            {
                is_like_NetNet = true;
            }
            else
            {
                // Do nothing
            }
            return is_like_NetNet;
        }

        #endregion

        #region Is_good_USF_Holland_orig_state
        private bool Is_good_USF_Holland_orig_state(ref QuoteData quoteData)
        {
            if (quoteData.origState == "AL" || quoteData.origState == "GA" || quoteData.origState == "SC" || quoteData.origState == "NC" || quoteData.origState == "WV" || quoteData.origState == "MI" ||
                        quoteData.origState == "KY" || quoteData.origState == "TN" || quoteData.origState == "MS" || quoteData.origState == "MO" || quoteData.origState == "IA" || quoteData.origState == "MN" ||
                        quoteData.origState == "WI" || quoteData.origState == "IL" || quoteData.origState == "IN")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Not used
        /*
        // GET api/getltlrates
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/getltlrates/5
        public string Get(int id)
        {
            return "value";
        }

        // PUT api/getltlrates/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/getltlrates/5
        public void Delete(int id)
        {
        }
        */
        #endregion
    }
}