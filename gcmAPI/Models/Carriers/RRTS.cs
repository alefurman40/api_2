#region Using

using System;
using gcmAPI.Models.LTL;
using gcmAPI.Models.Utilities;

#endregion

namespace gcmAPI.Models.Carriers
{
    public class RRTS
    {
        //CarrierAcctInfo acctInfo;
        QuoteData quoteData;
        int accountNumber;
        bool rateAsClass50, isAAFES;

        // Constructor
        public RRTS(int accountNumber, bool rateAsClass50, bool isAAFES, ref QuoteData quoteData)
        {
            this.accountNumber = accountNumber;
            this.quoteData = quoteData;
            this.rateAsClass50 = rateAsClass50;
            this.isAAFES = isAAFES;
        }

        #region GetResultObjectFromRoadRunnerByAccount

        public RRTS_WebService.QuoteResponse GetResultObjectFromRoadRunnerByAccount()
        {
            gcmAPI.RRTS_WebService.QuoteResponse objRoadRunnerResult = new RRTS_WebService.QuoteResponse();

            GetRateFromRRTS_QuoteByAccount(ref objRoadRunnerResult);

            return objRoadRunnerResult;
        }

        #endregion

        #region GetResultObjectFromRoadRunner

        public RRTS_WebService.QuoteResponse GetResultObjectFromRoadRunner()
        {
            gcmAPI.RRTS_WebService.QuoteResponse objRoadRunnerResult = new RRTS_WebService.QuoteResponse();

            GetRateFromRRTS(ref objRoadRunnerResult);

            return objRoadRunnerResult;
        }

        #endregion

        #region GetRateFromRRTS_QuoteByAccount

        private void GetRateFromRRTS_QuoteByAccount(ref gcmAPI.RRTS_WebService.QuoteResponse objRoadRunnerResult)
        {
            DB.Log("GetRateFromRRTS_QuoteByAccount", "");
            try
            {
                if (quoteData.username.Equals("durachem"))
                {
                    return;
                }

                double maxLengthDim = 0;
                HelperFuncs.GetMaxLengthDimension(ref quoteData.m_lPiece, ref maxLengthDim);

                if (!quoteData.mode.Equals("NetNet"))
                {
                    if ((quoteData.AccessorialsObj.LGPU.Equals(true) ||
                            quoteData.AccessorialsObj.LGDEL.Equals(true)) && maxLengthDim > 54)
                    {
                        throw new Exception("RDFS over 54 dim");
                    }
                }
                if (quoteData.AccessorialsObj.TRADEPU || quoteData.AccessorialsObj.TRADEDEL)
                {
                    throw new Exception("tradeshow");
                }

                gcmAPI.RRTS_WebService.QuoteAccountRequest quoteAccountRequest = new gcmAPI.RRTS_WebService.QuoteAccountRequest();
                quoteAccountRequest.Account = accountNumber;

                gcmAPI.RRTS_WebService.ServiceOptions[] objSOArray;

                quoteAccountRequest.OriginZip = quoteData.origZip;
                quoteAccountRequest.DestinationZip = quoteData.destZip; ;//"N1K1B8";
                quoteAccountRequest.OriginType = "B";//Third Party
                quoteAccountRequest.PaymentType = "P";//PrePaid

                gcmAPI.RRTS_WebService.ShipmentDetail objSD;
                objSD = new gcmAPI.RRTS_WebService.ShipmentDetail();

                quoteAccountRequest.ShipmentDetails = getRRTS_ShipmentDetails(ref rateAsClass50);

                if (quoteAccountRequest.ShipmentDetails == null)
                {
                    throw new Exception("error getting inputs");
                }

                quoteAccountRequest.ShipDate = quoteData.puDate;

                objSOArray = GetShipmentOptionsForRoadRunner();
                quoteAccountRequest.ServiceDeliveryOptions = objSOArray;

                gcmAPI.RRTS_WebService.RateQuote objRQ = new gcmAPI.RRTS_WebService.RateQuote();

                gcmAPI.RRTS_WebService.AuthenticationHeader objAuthentication = new gcmAPI.RRTS_WebService.AuthenticationHeader();

                Logins.Login_info login_info;
                Logins logins = new Logins();
                
                if (quoteData.username.ToLower().Equals("field16")) //account specific login
                {
                    logins.Get_login_info(22, out login_info);
                    objAuthentication.Password = login_info.password;
                    objAuthentication.UserName = login_info.username;
                }
                else
                {
                    logins.Get_login_info(21, out login_info);
                    objAuthentication.Password = login_info.password;
                    objAuthentication.UserName = login_info.username;
                }

                objRQ.Timeout = 20000;
                objRQ.AuthenticationHeaderValue = objAuthentication;

                gcmAPI.RRTS_WebService.QuoteResponse objQResponse = new gcmAPI.RRTS_WebService.QuoteResponse();

                //objQResponse = objRQ.CallRateQuote(quoteAccountRequest);

                objQResponse = objRQ.RateQuoteByAccount(quoteAccountRequest);

                objRoadRunnerResult = objQResponse;

            }
            catch (Exception ex)
            {
                //if (!ex.Message.Contains("no standard service") && !ex.Message.Contains("not in the standard") && !ex.Message.Contains("must be today") && !ex.Message.Contains("timed out"))
                //{
                //    DB.Log("Roadrunner (Live)", ex.ToString(), "");
                //}
                DB.Log("Roadrunner (Live)", ex.ToString());
            }
        }

        #endregion

        #region GetRateFromRRTS

        private void GetRateFromRRTS(ref gcmAPI.RRTS_WebService.QuoteResponse objRoadRunnerResult)
        {
            DB.Log("GetRateFromRRTS", "");
            try
            {
                if (quoteData.username.Equals("durachem"))
                {
                    return;
                }

                double maxLengthDim = 0;
                HelperFuncs.GetMaxLengthDimension(ref quoteData.m_lPiece, ref maxLengthDim);

                if (!quoteData.mode.Equals("NetNet"))
                {
                    if ((quoteData.AccessorialsObj.LGPU.Equals(true) ||
                             quoteData.AccessorialsObj.LGDEL.Equals(true)) && maxLengthDim > 54)
                    {
                        throw new Exception("RDFS over 54 dim");
                    }
                }

                gcmAPI.RRTS_WebService.QuoteRequest objQR = new gcmAPI.RRTS_WebService.QuoteRequest();

                gcmAPI.RRTS_WebService.ServiceOptions[] objSOArray;

                objQR.OriginZip = quoteData.origZip;
                objQR.DestinationZip = quoteData.destZip; ;//"N1K1B8";
                objQR.OriginType = "B";//Third Party
                objQR.PaymentType = "P";//PrePaid

                gcmAPI.RRTS_WebService.ShipmentDetail objSD;
                objSD = new gcmAPI.RRTS_WebService.ShipmentDetail();

                objQR.ShipmentDetails = getRRTS_ShipmentDetails(ref rateAsClass50);

                if (objQR.ShipmentDetails == null)
                {
                    throw new Exception("error getting inputs");
                }

                objQR.ShipDate = quoteData.puDate;

                objSOArray = GetShipmentOptionsForRoadRunner();
                objQR.ServiceDeliveryOptions = objSOArray;

                gcmAPI.RRTS_WebService.RateQuote objRQ = new gcmAPI.RRTS_WebService.RateQuote();

                gcmAPI.RRTS_WebService.AuthenticationHeader objAuthentication = new gcmAPI.RRTS_WebService.AuthenticationHeader();

                Logins.Login_info login_info;
                Logins logins = new Logins();

                if (quoteData.username.ToLower().Equals("field16")) //account specific login
                {
                    logins.Get_login_info(22, out login_info);
                    objAuthentication.Password = login_info.password;
                    objAuthentication.UserName = login_info.username;
                }
                else
                {
                    logins.Get_login_info(21, out login_info);
                    objAuthentication.Password = login_info.password;
                    objAuthentication.UserName = login_info.username;
                }

                objRQ.Timeout = 20000;
                objRQ.AuthenticationHeaderValue = objAuthentication;

                gcmAPI.RRTS_WebService.QuoteResponse objQResponse = new gcmAPI.RRTS_WebService.QuoteResponse();

                objQResponse = objRQ.CallRateQuote(objQR);

                objRoadRunnerResult = objQResponse;
            }
            catch (Exception e)
            {
                DB.Log("Roadrunner (Live)", e.ToString());
            }
        }

        #endregion

        #region GetShipmentOptionsForRoadRunner

        private gcmAPI.RRTS_WebService.ServiceOptions[] GetShipmentOptionsForRoadRunner()
        {
            int size = 0;

            if (quoteData.AccessorialsObj.LGPU)
                size = size + 1;
            if (quoteData.AccessorialsObj.LGDEL)
                size = size + 1;
            if (quoteData.AccessorialsObj.RESPU)
                size = size + 1;
            if (quoteData.AccessorialsObj.RESDEL)
                size = size + 1;
            //if (Convert.ToBoolean(insPick"]))
            //    size = size + 1;
            if (quoteData.AccessorialsObj.INSDEL)
                size = size + 1;
            if (quoteData.AccessorialsObj.CONPU)//School Pickup
                size = size + 1;
            if (quoteData.AccessorialsObj.CONDEL)//School Delivery
                size = size + 1;
            if (quoteData.AccessorialsObj.TRADEPU)//Exhibition Site Pickup
                size = size + 1;
            if (quoteData.AccessorialsObj.TRADEDEL)//Exhibition Site Delivery
                size = size + 1;
            if (quoteData.AccessorialsObj.APTPU || quoteData.AccessorialsObj.APTDEL)//Notification Charger
                size = size + 1;
            if (quoteData.isHazmat)
            {
                size = size + 1;
            }

            if (size > 0)
            {
                gcmAPI.RRTS_WebService.ServiceOptions[] objServiceArray = new gcmAPI.RRTS_WebService.ServiceOptions[size];
                gcmAPI.RRTS_WebService.ServiceOptions objService;

                int index = 0;

                if (quoteData.AccessorialsObj.LGPU)
                {
                    objService = new gcmAPI.RRTS_WebService.ServiceOptions();
                    objService.ServiceCode = "LGP";
                    objServiceArray[index] = objService;
                    index = index + 1;
                }
                if (quoteData.AccessorialsObj.LGDEL)
                {
                    objService = new gcmAPI.RRTS_WebService.ServiceOptions();
                    objService.ServiceCode = "LGD";
                    objServiceArray[index] = objService;
                    index = index + 1;
                }
                if (quoteData.AccessorialsObj.RESPU)
                {
                    objService = new gcmAPI.RRTS_WebService.ServiceOptions();
                    objService.ServiceCode = "RSP";
                    objServiceArray[index] = objService;
                    index = index + 1;
                }
                if (quoteData.AccessorialsObj.RESDEL)
                {
                    objService = new gcmAPI.RRTS_WebService.ServiceOptions();
                    objService.ServiceCode = "RSD";
                    objServiceArray[index] = objService;
                    index = index + 1;
                }
                //if (Convert.ToBoolean(Request.QueryString["q_InsPick"]))
                //{
                //    objService = new RRTS_WebService.ServiceOptions();
                //    objService.ServiceCode = "IP";
                //    objServiceArray[index] = objService;
                //    index = index + 1;
                //}
                if (quoteData.AccessorialsObj.INSDEL)
                {
                    objService = new gcmAPI.RRTS_WebService.ServiceOptions();
                    objService.ServiceCode = "ID";
                    objServiceArray[index] = objService;
                    index = index + 1;
                }
                if (quoteData.AccessorialsObj.CONPU)//School Pickup
                {
                    objService = new gcmAPI.RRTS_WebService.ServiceOptions();
                    objService.ServiceCode = "SHP";
                    objServiceArray[index] = objService;
                    index = index + 1;
                }

                if (quoteData.AccessorialsObj.CONDEL)//School Delivery
                {
                    objService = new gcmAPI.RRTS_WebService.ServiceOptions();
                    objService.ServiceCode = "SHD";
                    objServiceArray[index] = objService;
                    index = index + 1;
                }

                if (quoteData.AccessorialsObj.TRADEPU)//Exhibition Site Pickup
                {
                    objService = new gcmAPI.RRTS_WebService.ServiceOptions();
                    objService.ServiceCode = "ESP";
                    objServiceArray[index] = objService;
                    index = index + 1;
                }

                if (quoteData.AccessorialsObj.TRADEDEL)//Exhibition Site Delivery
                {
                    objService = new gcmAPI.RRTS_WebService.ServiceOptions();
                    objService.ServiceCode = "ESD";
                    objServiceArray[index] = objService;
                    index = index + 1;
                }

                if (quoteData.AccessorialsObj.APTPU || quoteData.AccessorialsObj.APTDEL)//Notification Charger
                {
                    objService = new gcmAPI.RRTS_WebService.ServiceOptions();
                    objService.ServiceCode = "NC";
                    objServiceArray[index] = objService;
                    index = index + 1;
                }
                if (quoteData.isHazmat)
                {
                    objService = new gcmAPI.RRTS_WebService.ServiceOptions();
                    objService.ServiceCode = "HAZ";
                    objServiceArray[index] = objService;
                    index = index + 1;
                }
                return objServiceArray;
            }
            else
                return null;
        }

        #endregion

        #region getRRTS_ShipmentDetails

        private gcmAPI.RRTS_WebService.ShipmentDetail[] getRRTS_ShipmentDetails(ref bool rateAsClass50)
        {

            //int intResult;
            gcmAPI.RRTS_WebService.ShipmentDetail[] objSDArray;

            //double dblResult;

            if (quoteData.m_lPiece.Length > 0)
            {
                //DB.Log("rrts", "size > 0", "");

                objSDArray = new gcmAPI.RRTS_WebService.ShipmentDetail[quoteData.m_lPiece.Length];

                gcmAPI.RRTS_WebService.ShipmentDetail objSD;

                for (byte i = 0; i < quoteData.m_lPiece.Length; i++)
                {
                    /////////////////////For RoadRunner///////////////////////////
                    objSD = new gcmAPI.RRTS_WebService.ShipmentDetail();

                    if (rateAsClass50.Equals(true))
                    {
                        objSD.ActualClass = 50;
                    }
                    else
                    {
                        if (quoteData.m_lPiece[i].Commodity == "HHG" || quoteData.isCommodityLkupHHG)
                        {
                            objSD.ActualClass = 150;
                        }
                        else
                        {
                            objSD.ActualClass = Convert.ToDouble(quoteData.m_lPiece[i].FreightClass);
                        }
                    }
                    objSD.Weight = Convert.ToInt32(quoteData.m_lPiece[i].Weight);
                    objSDArray[i] = objSD;
                }

                return objSDArray;
            }
            return null;

        }

        #endregion

        #region Not Used, GetResultObjectFromRoadRunnerSPC

        //private void GetResultObjectFromRoadRunnerSPC()
        //{
        //    DB.Log("GetResultObjectFromRoadRunnerSPC", "");
        //    try
        //    {
        //        if (AccessorialsObj.TRADEPU || AccessorialsObj.TRADEDEL)
        //        {
        //            throw new Exception("tradeshow");
        //            //LoadFreightArray();
        //        }
        //        //throw new Exception();
        //        //DB.Log("spc", "spc");
        //        //if (AccessorialsObj.TRADEPU || AccessorialsObj.TRADEDEL)
        //        //{
        //        //    LoadFreightArray();              
        //        //}

        //        gcmAPI.RRTS_WebService.QuoteRequest objQR = new gcmAPI.RRTS_WebService.QuoteRequest();

        //        gcmAPI.RRTS_WebService.ServiceOptions[] objSOArray;
        //        //RRTS_WebService.ShipmentDetail[] objSDArray;

        //        objQR.OriginZip = midOrigZip;
        //        objQR.DestinationZip = midDestZip; ;//"N1K1B8";
        //        objQR.OriginType = "B";//Third Party
        //        objQR.PaymentType = "P";//PrePaid

        //        //RRTS_WebService.ShipmentDetail objSD;
        //        //objSD = new RRTS_WebService.ShipmentDetail();

        //        //objSDArray = new RRTS_WebService.ShipmentDetail[1];

        //        //getRRTS_ShipmentDetails(ref objSDArray, ref objSD);

        //        bool rateAsClass50 = false;
        //        objQR.ShipmentDetails = getRRTS_ShipmentDetails(ref rateAsClass50);
        //        if (objQR.ShipmentDetails == null)
        //        {
        //            throw new Exception("error getting inputs");
        //        }

        //        //objQR.ShipmentDetails = objSDArray;

        //        objQR.ShipDate = quoteData.puDate;

        //        objSOArray = GetShipmentOptionsForRoadRunner();
        //        objQR.ServiceDeliveryOptions = objSOArray;

        //        gcmAPI.RRTS_WebService.RateQuote objRQ = new gcmAPI.RRTS_WebService.RateQuote();

        //        gcmAPI.RRTS_WebService.AuthenticationHeader objAuthentication = new gcmAPI.RRTS_WebService.AuthenticationHeader();
        //        //objAuthentication.Password = "";
        //        //objAuthentication.UserName = "";

        //        objAuthentication.Password = "";
        //        objAuthentication.UserName = "";

        //        objRQ.Timeout = intTimeOut;
        //        objRQ.AuthenticationHeaderValue = objAuthentication;

        //        gcmAPI.RRTS_WebService.QuoteResponse objQResponse = new gcmAPI.RRTS_WebService.QuoteResponse();

        //        objQResponse = objRQ.CallRateQuote(objQR);

        //        objRoadRunnerResultSPC = objQResponse;

        //    }
        //    catch (SoapException ex)
        //    {
        //        objRoadRunnerResultSPC = null;
        //        if (!ex.Message.Contains("no standard service") && !ex.Message.Contains("not in the standard"))
        //        {
        //            //DB.Log("Roadrunner (Live)", ex.ToString(), "");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        objRoadRunnerResultSPC = null;
        //        //DB.Log("Roadrunner (Live)", ex.ToString(), "");
        //    }
        //}

        #endregion

    }
}