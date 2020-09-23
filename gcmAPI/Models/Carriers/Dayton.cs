#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using gcmAPI.Models;
using gcmAPI.Models.LTL;
using System.Xml;
using System.Text;
using gcmAPI.Models.Utilities;

#endregion

namespace gcmAPI.Models.Carriers
{
    public class Dayton
    {
        //CarrierAcctInfo acctInfo;
        QuoteData quoteData;
        gcmAPI.com.daytonfreight.www.RateShipmentInformation[] objRSIArray;

        // Constructor
        public Dayton(ref QuoteData quoteData)
        {
            //this.acctInfo = acctInfo;
            this.quoteData = quoteData;
        }

        #region GetResultObjectFromDaytonFreight

        public GCMRateQuote GetResultObjectFromDaytonFreight()
        {

            //DB.Log("GetResultObjectFromDaytonFreight", "");
            try
            {
                if (quoteData.AccessorialsObj.TRADEPU.Equals(true) || quoteData.AccessorialsObj.TRADEDEL.Equals(true))
                {
                    throw new Exception("Tradeshow not supported");
                }

                LoadFreightArray();

                Logins.Login_info login_info;
                Logins logins = new Logins();
                logins.Get_login_info(69, out login_info);

                string strCustomerNumber = login_info.account;
                gcmAPI.com.daytonfreight.www.RateResult objDFResult;
                //gcmAPI.com.daytonfreight.www.RateResult objGRResultAM;
                //gcmAPI.com.daytonfreight.www.RateResult objGRResultPM;

                gcmAPI.com.daytonfreight.www.Credentials objCredentials = new gcmAPI.com.daytonfreight.www.Credentials();
                objCredentials.UserName = login_info.username;
                objCredentials.Password = login_info.password;
                gcmAPI.com.daytonfreight.www.ShippingService objSS = new gcmAPI.com.daytonfreight.www.ShippingService();
                objSS.CredentialsValue = objCredentials;
                /////////////////////////

                string[] arrAccCode = GetAccessorialCodesForDaytonFreight();
                objDFResult = objSS.Rate(strCustomerNumber, com.daytonfreight.www.Terms.ThirdParty,
                    quoteData.origZip, quoteData.destZip, "", objRSIArray, arrAccCode);
                //SC

                //objGRResultAM = objSS.Rate(strCustomerNumber, com.daytonfreight.www.Terms.ThirdParty, midOrigZip, midDestZip, "AM", objRSIArray, arrAccCode);
                //objGRResultPM = objSS.Rate(strCustomerNumber, com.daytonfreight.www.Terms.ThirdParty, midOrigZip, midDestZip, "PM", objRSIArray, arrAccCode);

                //objDFResult.
                //objGUResult = objSS.GetGuaranteedServiceInformation(midOrigZip, midDestZip, Convert.ToDateTime("2008-04-17 13:58:06.000"));

                //SC
                ///////////////////////
                if (objDFResult != null)
                {
                    double dblAdditionalAccessorialCharge = 0;

                    foreach (gcmAPI.com.daytonfreight.www.AccessorialDetailInformation objAccessDetail in objDFResult.Accessorials)
                    {
                        if (
                            objAccessDetail.Code.Trim().ToUpper().Equals("LIFT") && quoteData.AccessorialsObj.LGDEL
                            && quoteData.AccessorialsObj.LGPU
                            )
                            dblAdditionalAccessorialCharge += objAccessDetail.Amount;
                        else if (
                            objAccessDetail.Code.Trim().ToUpper().Equals("HMF") && quoteData.AccessorialsObj.APTPU
                            && quoteData.AccessorialsObj.APTDEL
                            )
                            dblAdditionalAccessorialCharge += objAccessDetail.Amount;
                        else if (objAccessDetail.Code.Trim().ToUpper().Equals("RESID"))
                        {
                            int intCount = 0;
                            if (quoteData.AccessorialsObj.RESDEL)
                                intCount += 1;
                            if (quoteData.AccessorialsObj.RESPU)
                                intCount += 1;
                            if (quoteData.AccessorialsObj.CONDEL)
                                intCount += 1;
                            if (quoteData.AccessorialsObj.CONPU)
                                intCount += 1;
                            /////////////
                            dblAdditionalAccessorialCharge += objAccessDetail.Amount * (intCount - 1);
                        }
                    }
                    dblAdditionalAccessorialCharge += GetInsidePickupDeliveryCharge();

                    double totalCharges = objDFResult.Total + dblAdditionalAccessorialCharge;

                    #region Overlength
                    if (totalCharges > 0)
                    {
                        int overlengthFee = 0;

                        // Get Overlenth Fee
                        HelperFuncs.GetOverlengthFee(ref quoteData.m_lPiece, ref overlengthFee, 180, 180, 180, 110, 110, 110);
                        totalCharges += overlengthFee;
                    }
                    #endregion

                    #region Cost Additions
                    //if (quoteData.subdomain.Equals("spc") || isCostPlus)
                    //{
                    //    totalCharges = HelperFuncs.addSPC_Addition(totalCharges);
                    //}

                    //if (quoteData.subdomain.Equals(HelperFuncs.Subdomains.clipper))
                    //{
                    //    totalCharges = HelperFuncs.addClipperSubdomain_Addition(totalCharges);
                    //}
                    #endregion

                    gcmAPI.com.daytonfreight.www.TransitTimeResult objTransitTime;
                    objTransitTime = objSS.GetTransitTime(quoteData.origZip, quoteData.destZip);

                    Int16 transitTime;

                    if (objTransitTime != null)
                    {
                        transitTime = Convert.ToInt16(objTransitTime.TotalServiceDays);
                    }
                    else
                    {
                        transitTime = -1;
                    }

                    //objDaytonFreightResult = SetInfoToObjectQuote(ref totalCharges, "Dayton Freight", "#1#", "Dayton",
                    //"http://www.globalcargomanager.com/Documents/DaytonFreight_Guaranteed.pdf", transitTime, "Dayton");

                    GCMRateQuote gcmRateQuote = new GCMRateQuote
                    {
                        TotalPrice = totalCharges,
                        DisplayName = "Dayton Freight",
                        Documentation = "http://www.globalcargomanager.com/Documents/DaytonFreight_Guaranteed.pdf",
                        DeliveryDays = transitTime,
                        BookingKey = "#1#",
                        CarrierKey = "Dayton"
                    };

                    return gcmRateQuote;

                }
                else
                {
                    return null;
                }
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {

                #region Catch

                //gcmRateQuote = null;
                WebServiceException[] webServiceExceptions = ParseSoapException(ex);
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < webServiceExceptions.Length; i++)
                    stringBuilder.Append(webServiceExceptions[i].Message + "\r\n");
                //throw new Exception(stringBuilder.ToString());

                DB.Log("Dayton (Live) Soap Exception", stringBuilder.ToString());

                return null;

                #endregion
                //DB.Log("Dayton (Live) Soap Exception", ex.ToString());

            }
            catch (Exception ex)
            {
                #region Catch

                //gcmRateQuote = null;
                DB.Log("Dayton (Live)", ex.ToString());

                return null;

                #endregion
            }
        }

        #endregion

        #region GetAccessorialCodesForDaytonFreight

        private string[] GetAccessorialCodesForDaytonFreight()
        {
            //Hazardous Materials Fee|HMF|0.0
            //Liftgate Delivery Charge|LIFT|0.0
            //Notification Fee|NOT|0.0
            //Residential Delivery Fee|RESID|0.0

            int size = 0;

            if (quoteData.AccessorialsObj.LGDEL || quoteData.AccessorialsObj.LGPU)
            {
                size = size + 1;
            }
            if (quoteData.AccessorialsObj.RESDEL || quoteData.AccessorialsObj.RESPU || quoteData.AccessorialsObj.CONDEL || quoteData.AccessorialsObj.CONPU)
            {
                size = size + 1;
            }
            if (quoteData.AccessorialsObj.APTPU || quoteData.AccessorialsObj.APTDEL) // Notification Charger
            {
                size = size + 1;
            }
            if (quoteData.isHazmat)
            {
                size = size + 1;
            }

            if (size > 0)
            {
                string[] arrAccCodes = new string[size];

                int index = 0;

                if (quoteData.AccessorialsObj.LGDEL || quoteData.AccessorialsObj.LGPU)
                {
                    arrAccCodes[index] = "LIFT";
                    index = index + 1;
                }
                if (quoteData.AccessorialsObj.RESDEL || quoteData.AccessorialsObj.RESPU || quoteData.AccessorialsObj.CONDEL || quoteData.AccessorialsObj.CONPU)
                {
                    arrAccCodes[index] = "RESID";
                    index = index + 1;
                }
                if (quoteData.AccessorialsObj.APTPU || quoteData.AccessorialsObj.APTDEL)//Notification Charger
                {
                    arrAccCodes[index] = "NOT";
                    index = index + 1;
                }
                if (quoteData.isHazmat)
                {
                    arrAccCodes[index] = "HMF";
                    index = index + 1;
                }
                return arrAccCodes;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region GetInsidePickupDeliveryCharge

        //Dayton Freight helper function
        private double GetInsidePickupDeliveryCharge()
        {
            //q_InsDel
            double dblInsideCharge = 0;
            if (quoteData.AccessorialsObj.INSDEL)
            {
                //double dblTotalWeight = 0;
                //double dblResult;
                //if (
                //    Request.QueryString["q_Weight1"] != null && Request.QueryString["q_Weight1"].Length > 0 && double.TryParse(Request.QueryString["q_Weight1"], out dblResult)
                //    )
                //    dblTotalWeight += dblResult;
                //if (
                //    Request.QueryString["q_Weight2"] != null && Request.QueryString["q_Weight2"].Length > 0 && double.TryParse(Request.QueryString["q_Weight2"], out dblResult)
                //    )
                //    dblTotalWeight += dblResult;
                //if (
                //    Request.QueryString["q_Weight3"] != null && Request.QueryString["q_Weight3"].Length > 0 && double.TryParse(Request.QueryString["q_Weight3"], out dblResult)
                //    )
                //    dblTotalWeight += dblResult;
                //if (
                //    Request.QueryString["q_Weight4"] != null && Request.QueryString["q_Weight4"].Length > 0 && double.TryParse(Request.QueryString["q_Weight4"], out dblResult)
                //    )
                //    dblTotalWeight += dblResult;
                ////////////////
                double dblCharge;
                dblCharge = (double)(.042 * quoteData.totalWeight);
                if (dblCharge < 65)
                    dblCharge = 65.0;
                ////////////
                int intCount = 0;
                //if (Convert.ToBoolean(Request.QueryString["q_InsPick"]))
                //    intCount += 1;
                if (quoteData.AccessorialsObj.INSDEL)
                    intCount += 1;

                dblInsideCharge = dblCharge * intCount;
            }
            return dblInsideCharge;
        }

        #endregion

        #region LoadFreightArray

        private void LoadFreightArray()
        {


            /////For Dayton Freight
            objRSIArray = new gcmAPI.com.daytonfreight.www.RateShipmentInformation[quoteData.m_lPiece.Length];
            gcmAPI.com.daytonfreight.www.RateShipmentInformation objRSI;

            //com.myfreightworld.webapi.Freight objFreight;
            //RRTS_WebService.ShipmentDetail objSD;

            /* Add Freight 1 Information in the Freight Array*/
            int index = 0;

            for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
            {

                ////////////////////For Dayton Freight////////////////////////
                objRSI = new gcmAPI.com.daytonfreight.www.RateShipmentInformation();
                objRSI.Class = quoteData.m_lPiece[i].FreightClass;
                objRSI.Weight = Convert.ToInt32(quoteData.m_lPiece[i].Weight);

                objRSI.HandlingUnits = quoteData.m_lPiece[i].Quantity;

                objRSIArray[index] = objRSI;

                index = index + 1;
            }
        }

        #endregion

        #region ParseSoapException

        public static WebServiceException[] ParseSoapException(System.Web.Services.Protocols.SoapException soapException)
        {
            WebServiceException[] exceptions;
            try
            {
                exceptions = new WebServiceException[soapException.Detail.ChildNodes.Count];
                for (int i = 0; i < exceptions.Length; i++)
                {
                    exceptions[i] = new WebServiceException
                        (Convert.ToInt32(soapException.Detail.ChildNodes[i].Attributes["e:number"].Value)
                            , soapException.Detail.ChildNodes[i].Attributes["e:type"].Value
                            , soapException.Detail.ChildNodes[i].Attributes["e:message"].Value);
                }
                return exceptions;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                exceptions = null;
            }
        }

        #endregion

        #region Web service exception class

        public class WebServiceException : System.ApplicationException
        {
            int number;
            public int Number
            {
                get { return this.number; }
                set { this.number = value; }
            }
            string type;
            public string Type
            {
                get { return this.type; }
                set { this.type = value; }
            }
            string message;
            public new string Message
            {
                get { return this.message; }
                set { this.message = value; }
            }
            public WebServiceException()
            {
            }
            public WebServiceException(int number, string type, string message)
            {
                this.number = number;
                this.type = type;
                this.message = message;
            }
        }

        #endregion

        #region Not used, GetInsidePickupDeliveryCharge

        //Dayton Freight helper function
        //private double GetInsidePickupDeliveryCharge()
        //{
        //    //q_InsDel
        //    double dblInsideCharge = 0;
        //    if (quoteData.AccessorialsObj.INSDEL)
        //    {
        //        double dblTotalWeight = 0;
        //        double dblResult;
        //        if (
        //            Request.QueryString["q_Weight1"] != null && Request.QueryString["q_Weight1"].Length > 0 && double.TryParse(Request.QueryString["q_Weight1"], out dblResult)
        //            )
        //            dblTotalWeight += dblResult;
        //        if (
        //            Request.QueryString["q_Weight2"] != null && Request.QueryString["q_Weight2"].Length > 0 && double.TryParse(Request.QueryString["q_Weight2"], out dblResult)
        //            )
        //            dblTotalWeight += dblResult;
        //        if (
        //            Request.QueryString["q_Weight3"] != null && Request.QueryString["q_Weight3"].Length > 0 && double.TryParse(Request.QueryString["q_Weight3"], out dblResult)
        //            )
        //            dblTotalWeight += dblResult;
        //        if (
        //            Request.QueryString["q_Weight4"] != null && Request.QueryString["q_Weight4"].Length > 0 && double.TryParse(Request.QueryString["q_Weight4"], out dblResult)
        //            )
        //            dblTotalWeight += dblResult;
        //        ////////////////
        //        double dblCharge;
        //        dblCharge = (double)(.042 * dblTotalWeight);
        //        if (dblCharge < 65)
        //            dblCharge = 65.0;
        //        ////////////
        //        int intCount = 0;
        //        //if (Convert.ToBoolean(Request.QueryString["q_InsPick"]))
        //        //    intCount += 1;
        //        if (quoteData.AccessorialsObj.INSDEL)
        //            intCount += 1;

        //        dblInsideCharge = dblCharge * intCount;
        //    }
        //    return dblInsideCharge;
        //}

        #endregion

    }
}