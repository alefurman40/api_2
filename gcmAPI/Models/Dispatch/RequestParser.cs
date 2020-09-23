#region Using
using gcmAPI.Models.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
#endregion

namespace gcmAPI.Models.Dispatch
{
    public class RequestParser
    {
        FormDataCollection form;
        DispatchData dispatch_data;

        public RequestParser(ref FormDataCollection form, ref DispatchData dispatch_data)
        {
            this.form = form;
            this.dispatch_data = dispatch_data;
        }

        #region SetParameters

        public void SetParameters()
        {
            #region glbDeliveryDate

            DateTime glbDeliveryDate;
            if (!DateTime.TryParse(form.Get("glbDeliveryDate"), out glbDeliveryDate))
            {
                glbDeliveryDate = DateTime.Today;
            }
            dispatch_data.glbDeliveryDate = glbDeliveryDate;

            #endregion

            dispatch_data.txtShipmentDate = form.Get("txtShipmentDate");
            dispatch_data.q_ShipmentReadyDate = form.Get("q_ShipmentReadyDate");
            dispatch_data.deliveryDay = form.Get("deliveryDay");

            dispatch_data.insuranceCost = 0.0;
            double.TryParse(form.Get("insuranceCost"), out dispatch_data.insuranceCost);
            dispatch_data.minInsuranceCost = 0.0;
            double.TryParse(form.Get("minInsuranceCost"), out dispatch_data.minInsuranceCost);
            dispatch_data.hasInsurance = form.Get("hasInsurance");

            dispatch_data.intCustCompanyID = 0;
            int.TryParse(form.Get("intCustCompanyID"), out dispatch_data.intCustCompanyID);

            #region Boolean flags

            try
            {
                if (!bool.TryParse(form.Get("isAAFES_Shipment"), out dispatch_data.isAAFES_Shipment))
                {
                    dispatch_data.isAAFES_Shipment = false;
                }
            }
            catch (Exception e)
            {
                DB.Log("bool.TryParse(form.Get(isAAFES_Shipment", e.ToString());
            }

            if (!bool.TryParse(form.Get("isDUR"), out dispatch_data.isDUR))
            {
                dispatch_data.isDUR = false;
            }
            if (!bool.TryParse(form.Get("isAssociationID_5"), out dispatch_data.isAssociationID_5))
            {
                dispatch_data.isAssociationID_5 = false;
            }

            //if (!bool.TryParse(form.Get("isHHG"), out quoteData.isHHG))
            //{
            //    quoteData.isHHG = false;
            //}
            if (!bool.TryParse(form.Get("isUSED"), out dispatch_data.isUSED))
            {
                dispatch_data.isUSED = false;
            }

            #endregion


            dispatch_data.oCityState = form.Get("oCityState");
            dispatch_data.dCityState = form.Get("dCityState");
            dispatch_data.oState = form.Get("oState");
            dispatch_data.dState = form.Get("dState");
            dispatch_data.q_OCity = form.Get("q_OCity");

            dispatch_data.q_DCity = form.Get("q_DCity");
            dispatch_data.rate = form.Get("rate");
            dispatch_data.ourRate = form.Get("ourRate");
            dispatch_data.txtSellRate = form.Get("txtSellRate");
            dispatch_data.shipmentValue = form.Get("shipmentValue");

            dispatch_data.username = form.Get("username").ToLower();
            dispatch_data.repName = form.Get("repName").ToLower();

            dispatch_data.Initials = form.Get("Initials");
            dispatch_data.Status = form.Get("Status");
            dispatch_data.carrier = form.Get("carrier");
            dispatch_data.carrierKey = form.Get("carrierKey");
            dispatch_data.rateType = form.Get("rateType");

            dispatch_data.txtComment = form.Get("txtComment");

            dispatch_data.txtPONumber = form.Get("txtPONumber");
            dispatch_data.selectedTransit = form.Get("selectedTransit");
            dispatch_data.selectedOnTime = form.Get("selectedOnTime");
            dispatch_data.selectedRate = form.Get("selectedRate");
            dispatch_data.selectedCarrier = form.Get("selectedCarrier");

            dispatch_data.topCarrier = form.Get("topCarrier");
            dispatch_data.topOnTime = form.Get("topOnTime");
            dispatch_data.topTransit = form.Get("topTransit");
            dispatch_data.topRate = form.Get("topRate");
            dispatch_data.txtDesc1 = form.Get("txtDesc1");

            dispatch_data.txtDesc2 = form.Get("txtDesc2");
            dispatch_data.txtDesc3 = form.Get("txtDesc3");
            dispatch_data.txtDesc4 = form.Get("txtDesc4");
            dispatch_data.DimsCubeDesc1 = form.Get("DimsCubeDesc1");
            dispatch_data.DimsCubeDesc2 = form.Get("DimsCubeDesc2");

            dispatch_data.DimsCubeDesc3 = form.Get("DimsCubeDesc3");
            dispatch_data.DimsCubeDesc4 = form.Get("DimsCubeDesc4");

            SetParameters_companies_info();
            SetParameters_accessorials_info();
            SetParameters_line_items_info();

            dispatch_data.commodityName = form.Get("commodityName");


            dispatch_data.ddlDeliveryAddress = form.Get("ddlDeliveryAddress");
            dispatch_data.ddlPickupAddress = form.Get("ddlPickupAddress");

            dispatch_data.rdblClientType = form.Get("rdblClientType");
            dispatch_data.rdblBill = form.Get("rdblBill");
            dispatch_data.ddlRHour = form.Get("ddlRHour");
            dispatch_data.ddlRMinute = form.Get("ddlRMinute");
            dispatch_data.ddlRAMPM = form.Get("ddlRAMPM");

            dispatch_data.ddlCHour = form.Get("ddlCHour");
            dispatch_data.ddlCMinute = form.Get("ddlCMinute");
            dispatch_data.ddlCAMPM = form.Get("ddlCAMPM");
            dispatch_data.repName = form.Get("repName");
            dispatch_data.repName = form.Get("repName");

            dispatch_data.repName = form.Get("repName");
            dispatch_data.repName = form.Get("repName");
            dispatch_data.repName = form.Get("repName");
            dispatch_data.repName = form.Get("repName");
            dispatch_data.repName = form.Get("repName");

            dispatch_data.repName = form.Get("repName");
            dispatch_data.repName = form.Get("repName");
            dispatch_data.repName = form.Get("repName");
            dispatch_data.repName = form.Get("repName");
            dispatch_data.repName = form.Get("repName");

        }

        #endregion

        #region SetParameters_companies_info

        public void SetParameters_companies_info()
        {

            dispatch_data.txtPName = form.Get("txtPName");
            dispatch_data.txtPCompany = form.Get("txtPCompany");
            dispatch_data.txtPAddress1 = form.Get("txtPAddress1");
            dispatch_data.txtPAddress2 = form.Get("txtPAddress2");
            dispatch_data.txtPCity = form.Get("txtPCity");
            dispatch_data.txtPST = form.Get("txtPST");
            dispatch_data.txtPZip = form.Get("txtPZip");
            dispatch_data.txtPEmail = form.Get("txtPEmail");
            dispatch_data.txtPPhone = form.Get("txtPPhone");
            dispatch_data.txtPFax = form.Get("txtPFax");

            dispatch_data.txtDName = form.Get("txtDName");
            dispatch_data.txtDCompany = form.Get("txtDCompany");
            dispatch_data.txtDAddress1 = form.Get("txtDAddress1");
            dispatch_data.txtDAddress2 = form.Get("txtDAddress2");
            dispatch_data.txtDCity = form.Get("txtDCity");
            dispatch_data.txtDST = form.Get("txtDST");
            dispatch_data.txtDZip = form.Get("txtDZip");
            dispatch_data.txtDEmail = form.Get("txtDEmail");
            dispatch_data.txtDPhone = form.Get("txtDPhone");
            dispatch_data.txtDFax = form.Get("txtDFax");

            dispatch_data.txtTPName = form.Get("txtTPName");
            dispatch_data.txtTPCompany = form.Get("txtTPCompany");
            dispatch_data.txtTPAddress1 = form.Get("txtTPAddress1");
            dispatch_data.txtTPAddress2 = form.Get("txtTPAddress2");
            dispatch_data.txtTPCity = form.Get("txtTPCity");
            dispatch_data.txtTPST = form.Get("txtTPST");
            dispatch_data.txtTPZip = form.Get("txtTPZip");
            dispatch_data.txtTPEmail = form.Get("txtTPEmail");
            dispatch_data.txtTPPhone = form.Get("txtTPPhone");
            dispatch_data.txtTPFax = form.Get("txtTPFax");

        }

        #endregion

        #region SetParameters_accessorials_info

        public void SetParameters_accessorials_info()
        {

            dispatch_data.q_ResPick = form.Get("q_ResPick");
            dispatch_data.q_ResDel = form.Get("q_ResDel");

            dispatch_data.q_ConstPick = form.Get("q_ConstPick");
            dispatch_data.q_ConstDel = form.Get("q_ConstDel");
            dispatch_data.q_TradePick = form.Get("q_TradePick");
            dispatch_data.q_TradeDel = form.Get("q_TradeDel");
            dispatch_data.q_TailPick = form.Get("q_TailPick");

            dispatch_data.q_TailDel = form.Get("q_TailDel");
            dispatch_data.q_AppPick = form.Get("q_AppPick");
            dispatch_data.q_AppDel = form.Get("q_AppDel");

            dispatch_data.q_InsDel = form.Get("q_InsDel");

            #region Hazmat

            if (!bool.TryParse(form.Get("chkHazMat1"), out dispatch_data.chkHazMat1))
            {
                dispatch_data.chkHazMat1 = false;
            }
            if (!bool.TryParse(form.Get("chkHazMat2"), out dispatch_data.chkHazMat2))
            {
                dispatch_data.chkHazMat2 = false;
            }
            if (!bool.TryParse(form.Get("chkHazMat3"), out dispatch_data.chkHazMat3))
            {
                dispatch_data.chkHazMat3 = false;
            }
            if (!bool.TryParse(form.Get("chkHazMat4"), out dispatch_data.chkHazMat4))
            {
                dispatch_data.chkHazMat4 = false;
            }

            dispatch_data.q_HazMat1 = form.Get("q_HazMat1");
            dispatch_data.q_HazMat2 = form.Get("q_HazMat2");
            dispatch_data.q_HazMat3 = form.Get("q_HazMat3");
            dispatch_data.q_HazMat4 = form.Get("q_HazMat4");

            #endregion

        }

        #endregion

        #region SetParameters_line_items_info

        public void SetParameters_line_items_info()
        {

            dispatch_data.q_Weight1 = form.Get("q_Weight1");
            dispatch_data.q_Weight2 = form.Get("q_Weight2");
            dispatch_data.q_Weight3 = form.Get("q_Weight3");
            dispatch_data.q_Weight4 = form.Get("q_Weight4");

            dispatch_data.q_Length1 = form.Get("q_Length1");
            dispatch_data.q_Length2 = form.Get("q_Length2");
            dispatch_data.q_Length3 = form.Get("q_Length3");
            dispatch_data.q_Length4 = form.Get("q_Length4");

            dispatch_data.q_Width1 = form.Get("q_Width1");
            dispatch_data.q_Width2 = form.Get("q_Width2");
            dispatch_data.q_Width3 = form.Get("q_Width3");
            dispatch_data.q_Width4 = form.Get("q_Width4");

            dispatch_data.q_Height1 = form.Get("q_Height1");
            dispatch_data.q_Height2 = form.Get("q_Height2");
            dispatch_data.q_Height3 = form.Get("q_Height3");
            dispatch_data.q_Height4 = form.Get("q_Height4");

            dispatch_data.txtPallet1 = form.Get("txtPallet1");
            dispatch_data.txtPallet2 = form.Get("txtPallet2");
            dispatch_data.txtPallet3 = form.Get("txtPallet3");
            dispatch_data.txtPallet4 = form.Get("txtPallet4");

            dispatch_data.lblClass1 = form.Get("lblClass1");
            dispatch_data.lblClass2 = form.Get("lblClass2");
            dispatch_data.lblClass3 = form.Get("lblClass3");
            dispatch_data.lblClass4 = form.Get("lblClass4");

            dispatch_data.q_Class1 = form.Get("q_Class1");
            dispatch_data.q_Class2 = form.Get("q_Class2");
            dispatch_data.q_Class3 = form.Get("q_Class3");
            dispatch_data.q_Class4 = form.Get("q_Class4");

            dispatch_data.lblWeight1 = form.Get("lblWeight1");
            dispatch_data.lblWeight2 = form.Get("lblWeight2");
            dispatch_data.lblWeight3 = form.Get("lblWeight3");
            dispatch_data.lblWeight4 = form.Get("lblWeight4");

            dispatch_data.lblType1 = form.Get("lblType1");
            dispatch_data.lblType2 = form.Get("lblType2");
            dispatch_data.lblType3 = form.Get("lblType3");
            dispatch_data.lblType4 = form.Get("lblType4");

            dispatch_data.lblPiece1 = form.Get("lblPiece1");
            dispatch_data.lblPiece2 = form.Get("lblPiece2");
            dispatch_data.lblPiece3 = form.Get("lblPiece3");
            dispatch_data.lblPiece4 = form.Get("lblPiece4");

            dispatch_data.q_Piece1 = form.Get("q_Piece1");
            dispatch_data.q_Piece2 = form.Get("q_Piece2");
            dispatch_data.q_Piece3 = form.Get("q_Piece3");
            dispatch_data.q_Piece4 = form.Get("q_Piece4");

            dispatch_data.txtNMFC1 = form.Get("txtNMFC1");
            dispatch_data.txtNMFC2 = form.Get("txtNMFC2");
            dispatch_data.txtNMFC3 = form.Get("txtNMFC3");
            dispatch_data.txtNMFC4 = form.Get("txtNMFC4");

            dispatch_data.q_Unit1 = form.Get("q_Unit1");
            dispatch_data.q_Unit2 = form.Get("q_Unit2");
            dispatch_data.q_Unit3 = form.Get("q_Unit3");
            dispatch_data.q_Unit4 = form.Get("q_Unit4");

            dispatch_data.Commodity1 = form.Get("Commodity1");
            dispatch_data.Commodity2 = form.Get("Commodity2");
            dispatch_data.Commodity3 = form.Get("Commodity3");
            dispatch_data.Commodity4 = form.Get("Commodity4");

        }

        #endregion
    }
}