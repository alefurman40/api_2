#region Using

using gcmAPI.Models.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

#endregion

namespace gcmAPI.Models.Dispatch
{
    public static class Helper
    {
        #region Set Item descriptions

        public static void SetItemDescriptions(ref DispatchData dispatch_data, ref string Desc1, ref string Desc2, ref string Desc3, ref string Desc4)
        {
            #region Set Item descriptions
            // If session is null here fall back on txtDesc.Text


            if (dispatch_data.DimsCubeDesc1 != null && dispatch_data.DimsCubeDesc1.ToString().Trim().Length > 0)
            {
                // Make sure dims are in format: customer description // dims
                Desc1 = Desc1.Replace(dispatch_data.DimsCubeDesc1.ToString().Trim(), " ") +
                    dispatch_data.DimsCubeDesc1.ToString();
            }
            //else
            //{
            //    HelperFuncs.writeToSiteErrors("Dispatch", "Session Desc1 was null");
            //}
            if (dispatch_data.DimsCubeDesc2 != null && dispatch_data.DimsCubeDesc2.ToString().Trim().Length > 0)
            {
                Desc2 = Desc2.Replace(dispatch_data.DimsCubeDesc2.ToString().Trim(), " ") +
                    dispatch_data.DimsCubeDesc2.ToString();
            }
            //else
            //{
            //    HelperFuncs.writeToSiteErrors("Dispatch", "Session Desc2 was null");
            //}
            if (dispatch_data.DimsCubeDesc3 != null && dispatch_data.DimsCubeDesc3.ToString().Trim().Length > 0)
            {
                Desc3 = Desc3.Replace(dispatch_data.DimsCubeDesc3.ToString().Trim(), " ") +
                    dispatch_data.DimsCubeDesc3.ToString();
            }
            //else
            //{
            //    HelperFuncs.writeToSiteErrors("Dispatch", "Session Desc3 was null");
            //}
            if (dispatch_data.DimsCubeDesc4 != null && dispatch_data.DimsCubeDesc4.ToString().Trim().Length > 0)
            {
                Desc4 = Desc4.Replace(dispatch_data.DimsCubeDesc4.ToString().Trim(), " ") +
                    dispatch_data.DimsCubeDesc4.ToString();
            }
            //else
            //{
            //    HelperFuncs.writeToSiteErrors("Dispatch", "Session Desc4 was null");
            //}
            #endregion
        }

        #endregion

        #region GetDeliveryDate

        public static DateTime GetDeliveryDate(DateTime dtPickupDate, int intNoOfDeliveryDays)
        {
            DateTime dtDeliveryDate;
            string strDay;
            dtDeliveryDate = dtPickupDate;

            int intBusinessDay = 0;

            while (intBusinessDay < intNoOfDeliveryDays)
            {
                dtDeliveryDate = dtDeliveryDate.AddDays(1);
                strDay = dtDeliveryDate.DayOfWeek.ToString().ToUpper();
                if (strDay.Equals("MONDAY") || strDay.Equals("TUESDAY") || strDay.Equals("WEDNESDAY") || strDay.Equals("THURSDAY") || strDay.Equals("FRIDAY"))
                    intBusinessDay += 1;
            }

            return dtDeliveryDate;
        }

        #endregion

        #region GetUnitDescFromUnitKey

        public static string GetUnitDescFromUnitKey(string strUnitID)
        {
            return strUnitID;
            /*
            if (strUnitID.ToUpper().Trim().Equals("TYPE_BOXXX"))
                return "Box";
            else if (strUnitID.ToUpper().Trim().Equals("BUNDLESXXX"))
                return "Bundles";
            else if (strUnitID.ToUpper().Trim().Equals("TYPE_CARTN"))
                return "Carton";
            else if (strUnitID.ToUpper().Trim().Equals("TYPE_COILX"))
                return "Coil";
            else if (strUnitID.ToUpper().Trim().Equals("TYPE_CRATE"))
                return "Crate";
            else if (strUnitID.ToUpper().Trim().Equals("TYPE_DRUM"))
                return "Drum";
            else if (strUnitID.ToUpper().Trim().Equals("TYPE_OTHER"))
                return "Other";
            else if (strUnitID.ToUpper().Trim().Equals("TYPE_PAILX"))
                return "Pail";
            else if (strUnitID.ToUpper().Trim().Equals("PALLET_XXX"))
                return "Pallet";
            else if (strUnitID.ToUpper().Trim().Equals("TYPE_PIECE"))
                return "Pieces";
            else if (strUnitID.ToUpper().Trim().Equals("TYPE_ROLLS"))
                return "Rolls";
            else if (strUnitID.ToUpper().Trim().Equals("SLIPSHEETX"))
                return "Slipsheet";
            else if (strUnitID.ToUpper().Trim().Equals("TYPE_TOTES"))
                return "Totes";
            else if (strUnitID.ToUpper().Trim().Equals("GAYLORD_XX"))
                return "Gaylord";
            else if (strUnitID.ToUpper().Trim().Equals("TYPE_BAGS"))
                return "Bags";

            else
                return "";
            */
        }

        #endregion

        #region GetSpecialState

        //public static string GetSpecialState()
        //{
        //    string originState = "";
        //    string destState = "";

        //    try
        //    {
        //        if (!String.IsNullOrEmpty(requestedValues["oCityState"]))
        //        {
        //            string[] arrPCityState = requestedValues["oCityState"].Split(',');
        //            originState = arrPCityState[1].Trim();
        //        }
        //        else if (Session["oState"] != null)
        //        {
        //            originState = Session["oState"].ToString();
        //        }
        //        else if (!String.IsNullOrEmpty(requestedValues["q_OCity"]))
        //        {
        //            string[] arrPCityState = requestedValues["q_OCity"].Split(',');
        //            originState = arrPCityState[1].Trim();
        //        }

        //        if (!String.IsNullOrEmpty(requestedValues["dCityState"]))
        //        {
        //            string[] arrPCityState = requestedValues["dCityState"].Split(',');
        //            originState = arrPCityState[1].Trim();
        //        }
        //        else if (Session["dState"] != null)
        //        {
        //            destState = Session["dState"].ToString();
        //        }
        //        else if (!String.IsNullOrEmpty(requestedValues["q_DCity"]))
        //        {
        //            string[] arrDCityState = requestedValues["q_DCity"].Split(',');
        //            destState = arrDCityState[1].Trim();
        //        }
        //    }
        //    catch (Exception myException)
        //    {
        //        HelperFuncs.writeToSiteErrors("dispatch getSpecialState", string.Concat("oCityState: ", requestedValues["oCityState"], " q_OCity: ",
        //            requestedValues["q_OCity"], " dCityState: ", requestedValues["dCityState"], " q_DCity: ", requestedValues["q_DCity"]));

        //        throw new Exception(myException.ToString());
        //    }

        //    if (originState.ToUpper().Equals("HI") || originState.ToUpper().Equals("AK"))
        //    {
        //        return originState;
        //    }
        //    if (destState.ToUpper().Equals("HI") || destState.ToUpper().Equals("AK"))
        //    {
        //        return destState;
        //    }
        //    return "";
        //}

        #endregion

        #region GetSpecialState

        public static string GetSpecialState(ref DispatchData dispatch_data)
        {
            string originState = "";
            string destState = "";

            try
            {
                if (!String.IsNullOrEmpty(dispatch_data.oCityState))
                {
                    string[] arrPCityState = dispatch_data.oCityState.Split(',');
                    originState = arrPCityState[1].Trim();
                }
                else if (dispatch_data.oState != null)
                {
                    originState = dispatch_data.oState;
                }
                else if (!String.IsNullOrEmpty(dispatch_data.q_OCity))
                {
                    string[] arrPCityState = dispatch_data.q_OCity.Split(',');
                    originState = arrPCityState[1].Trim();
                }

                if (!String.IsNullOrEmpty(dispatch_data.dCityState))
                {
                    string[] arrPCityState = dispatch_data.dCityState.Split(',');
                    originState = arrPCityState[1].Trim();
                }
                else if (dispatch_data.dState != null)
                {
                    destState = dispatch_data.dState;
                }
                else if (!String.IsNullOrEmpty(dispatch_data.q_DCity))
                {
                    string[] arrDCityState = dispatch_data.q_DCity.Split(',');
                    destState = arrDCityState[1].Trim();
                }
            }
            catch (Exception myException)
            {
                HelperFuncs.writeToSiteErrors("dispatch getSpecialState",
                    string.Concat("oCityState: ", dispatch_data.oCityState, " q_OCity: ",
                    dispatch_data.q_OCity, " dCityState: ", dispatch_data.dCityState, " q_DCity: ",
                    dispatch_data.q_DCity));

                throw new Exception(myException.ToString());
            }

            if (originState.ToUpper().Equals("HI") || originState.ToUpper().Equals("AK"))
            {
                return originState;
            }
            if (destState.ToUpper().Equals("HI") || destState.ToUpper().Equals("AK"))
            {
                return destState;
            }
            return "";
        }

        #endregion

        #region GetSpecialCarrierName

        //public static string GetSpecialCarrierName(ref DispatchData dispatch_data)
        //{
        //    string specialState = Helper.GetSpecialState(ref dispatch_data);
        //    if (specialState.ToUpper().Equals("HI"))
        //    {
        //        return "H2O LOGISTICS";
        //    }
        //    if (specialState.ToUpper().Equals("AK"))
        //    {
        //        return "AMERICAN FAST FREIGHT";
        //    }
        //    return "";
        //}

        #endregion

        #region GetInterimCompanyDetails

        public static Company GetInterimCompanyDetails(ref DispatchData dispatch_data)
        {
            Company comp = new Company();
            string specialState = GetSpecialState(ref dispatch_data);
            if (specialState.ToUpper().Equals("HI"))
            {
                comp.CompName = "H2O LOGISTICS";
                //comp.Addr1 = "16920 South Main Street";
                //comp.Addr2 = "";
                //comp.City = "Gardena";
                //comp.State = "CA";
                //comp.Zip = "90248";
                comp.Phone = "310-324-8955";
                comp.Addr1 = "23601 S WILMINGTON AVE";
                comp.Addr2 = "";
                comp.City = "CARSON";
                comp.State = "CA";
                comp.Zip = "90745";
                // 23601 S WILMINGTON AVE CARSON CA 90745
            }
            if (specialState.ToUpper().Equals("AK"))
            {
                comp.CompName = "American Fast Freight";
                comp.Addr1 = "7400 45th Street Ct East";
                comp.Addr2 = "";
                comp.City = "Tacoma";
                comp.State = "WA";
                comp.Zip = "98424";
                comp.Phone = "800-642-6664";
            }
            return comp;
        }

        #endregion

        //

        
    }
}