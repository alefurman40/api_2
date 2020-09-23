#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Formatting;
using gcmAPI.Models;
using gcmAPI.Models.Dispatch;
using System.Text;
using gcmAPI.Models.Utilities;

#endregion

namespace gcmAPI.Controllers
{
    public class CreateLTLBookingController : ApiController
    {
        // This function will go into Public CreateLTLBooking
        public string Post(SharedLTL.LTLBookingInfo json)
        {
            //LTLBooking booking = new LTLBooking();
            //HelperFuncs.writeToSiteErrors("test SearchObject", string.Concat(json.origName, " ", json.origEmail));

            DB.Log("LTLBookingInfo", json);

            LTLBooking booking = new LTLBooking();
            booking.makeBookingViaSOAP(ref json);

            return booking.makeBookingViaSOAP(ref json);

            #region Not used
            //FormDataCollection form = new FormDataCollection("");
            ////

            //DispatchData dispatch_data = new DispatchData();

            //RequestParser parser = new RequestParser(ref form, ref dispatch_data);

            //parser.SetParameters();
            //parser.SetParameters_companies_info();
            //parser.SetParameters_accessorials_info();
            //parser.SetParameters_line_items_info();

            //StringBuilder sb = new StringBuilder();

            //Logger logger = new Logger(ref dispatch_data, ref sb);
            //logger.Log_all_request_info();

            //Dispatch dispatch = new Dispatch(ref dispatch_data);
            //dispatch.SaveDispatchInformation();

            //return sb.ToString();
            #endregion
        }

        #region Json_to_URL_encoded
        private string Json_to_URL_encoded(ref SharedLTL.LTLBookingInfo json)
        {
            return string.Concat("intCustCompanyID=0&intShipmentID=0&intSegmentID=0&intSegmentID2=0",
                "&intCarrierCompID=0&intRQID&intPUCompID=&glbDeliveryDate=01/01/2020",
                "&insuranceCost=&minInsuranceCost=&isAAFES_Shipment=false&isDUR=false&isAssociationID_5=false",
                "&isUSED=false",
                "&chkHazMat1=false&chkHazMat2=false&chkHazMat3=false&chkHazMat4=false",
                "&q_HazMat1=false&q_HazMat2=false&q_HazMat3=false&q_HazMat4=false",

                "&oCityState=&dCityState=&oState=&dState=&q_OCity=&q_DCity=",
                "&rate=0&ourRate=0&txtSellRate=&hasInsurance=false&shipmentValue=0&deliveryDay=",
                "&username=&repName=&Initials=&Status=&carrier=&carrierKey=&DLS_PrimaryReferencePNW=",
                "&txtShipmentDate=", json.shipmentDate , "&q_ShipmentReadyDate=01/01/2020&rateType=&txtComment=",
                "&txtPONumber=&selectedTransit=&selectedOnTime=&selectedRate=&selectedCarrier=",
                "&topCarrier=&topOnTime=&topTransit=&topRate=",
                "&txtDesc1=&txtDesc2=&txtDesc3=&txtDesc4=",
                "&DimsCubeDesc1=&DimsCubeDesc2=&DimsCubeDesc3=&DimsCubeDesc4=",

                "&txtPName=&txtPCompany=&txtPAddress1=&txtPAddress2=&txtPCity=&txtPST=",
                "&txtPZip=&txtPEmail=&txtPPhone=&txtPFax",

                "&txtDName=&txtDCompany=&txtDAddress1=&txtDAddress2=&txtDCity=&txtDST=",
                "&txtDZip=&txtDEmail=&txtDPhone=&txtDFax",

                "&txtTPName=&txtTPCompany=&txtTPAddress1=&txtTPAddress2=&txtTPCity=&txtTPST",
                "&txtTPZip=&txtTPEmail=&txtTPPhone=&txtTPFax=",

                "&q_ResPick=false&q_ResDel=&q_ConstPick=&q_ConstDel=&q_TradePick=&q_TradeDel=",
                "&q_TailPick=&q_TailDel=&q_AppPick=&q_AppDel=&q_InsPick=&q_InsDel=",
                "&commodityName",

                "&q_Weight1=&q_Weight2=&q_Weight3=&q_Weight4=",
                "&q_Length1=&q_Length2=&q_Length3=&q_Length4=",
                "&=q_Width1&q_Width2=&q_Width3=&q_Width4=",
                "&q_Height1=&q_Height2=&q_Height3=&q_Height4=",
                "&txtPallet1=&txtPallet2=&txtPallet3=&txtPallet4=",
                "&lblClass1=&lblClass2=&lblClass3=&lblClass4=",
                "&q_Class1=&q_Class2=&q_Class3=&q_Class4=",
                "&lblWeight1=&lblWeight2=&lblWeight3=&lblWeight4=",
                "&lblType1=&lblType2=&lblType3=&lblType4=",
                "&lblPiece1=&lblPiece2=&lblPiece3=&lblPiece4=",
                "&q_Piece1=&q_Piece2=&q_Piece3=&q_Piece4=",
                "&txtNMFC1=&txtNMFC2=&txtNMFC3=&txtNMFC4=",
                "&q_Unit1=&q_Unit2=&q_Unit3=&q_Unit4=",
                "&Commodity1=&Commodity2=&Commodity3=&Commodity4=",

                "&ddlDeliveryAddress=&ddlPickupAddress=&rdblClientType=&rdblBill=",
                "&ddlRHour=&ddlRMinute=&ddlRAMPM=&ddlCHour=",
                "&ddlCMinute=&ddlCAMPM=",
                "&exportPath=&exportPath2=&BOLReportName2=&strReportFileName=",
                "&strReportFileName2="
                );
        }

        #endregion

        #region Controller for new booking function, not yet in use
        /*
        [HttpPost]
        public string Post(FormDataCollection form)
        {

            DispatchData dispatch_data = new DispatchData();

            RequestParser parser = new RequestParser(ref form, ref dispatch_data);

            parser.SetParameters();
            parser.SetParameters_companies_info();
            parser.SetParameters_accessorials_info();
            parser.SetParameters_line_items_info();

            StringBuilder sb = new StringBuilder();

            Logger logger = new Logger(ref dispatch_data, ref sb);
            logger.Log_all_request_info();

            Dispatch dispatch = new Dispatch(ref dispatch_data);
            dispatch.SaveDispatchInformation();

            return sb.ToString();

        }
        */
        #endregion

        #region Not used, actions

        // GET api/createltlbooking
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/createltlbooking/5
        public string Get(int id)
        {
            return "value";
        }

        // PUT api/createltlbooking/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/createltlbooking/5
        public void Delete(int id)
        {
        }

        #endregion

        #region Not used
        //[HttpPost]
        //public string Post(SharedLTL.LTLBookingInfo json)
        //{
        //    //LTLBooking booking = new LTLBooking();
        //    //HelperFuncs.writeToSiteErrors("test SearchObject", string.Concat(json.origName, " ", json.origEmail));

        //    LTLBooking booking = new LTLBooking();
        //    booking.makeBookingViaSOAP(ref json);

        //    return booking.makeBookingViaSOAP(ref json);

        //    //return "test SearchObject";
        //}
        #endregion

    }
}
