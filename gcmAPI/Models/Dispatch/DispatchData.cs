#region Using 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

#endregion

namespace gcmAPI.Models.Dispatch
{
    public class DispatchData
    {
        public int intCustCompanyID, intShipmentID, intSegmentID, intSegmentID2, intCarrierCompID, intRQID,
            intPUCompID;

        public DateTime glbDeliveryDate;
        public string txtShipmentDate, q_ShipmentReadyDate;
        public double insuranceCost, minInsuranceCost;

        public bool isAAFES_Shipment, isDUR, isAssociationID_5, isUSED;

        public bool chkHazMat1, chkHazMat2, chkHazMat3, chkHazMat4;

        public string q_HazMat1, q_HazMat2, q_HazMat3, q_HazMat4;

        public string oCityState, dCityState, oState, dState, q_OCity, q_DCity;
       
        public string rate, ourRate, txtSellRate, hasInsurance, shipmentValue, deliveryDay;
        public string username, repName, Initials, Status, carrier, carrierKey, DLS_PrimaryReferencePNW;
        
        public string rateType, txtComment, txtPONumber;
        public string selectedTransit, selectedOnTime, selectedRate, selectedCarrier, topCarrier,
            topOnTime,topTransit, topRate;
        public string txtDesc1, txtDesc2, txtDesc3, txtDesc4,
            DimsCubeDesc1, DimsCubeDesc2, DimsCubeDesc3, DimsCubeDesc4;

        public string txtPName, txtPCompany, txtPAddress1, txtPAddress2, txtPCity, txtPST, txtPZip, 
            txtPEmail, txtPPhone, txtPFax;
        public string txtDName, txtDCompany, txtDAddress1, txtDAddress2, txtDCity, txtDST, txtDZip,
            txtDEmail, txtDPhone, txtDFax;
        public string txtTPName, txtTPCompany, txtTPAddress1, txtTPAddress2, txtTPCity, txtTPST, txtTPZip,
            txtTPEmail,txtTPPhone, txtTPFax;


        public string q_ResPick, q_ResDel, q_ConstPick, q_ConstDel, q_TradePick, q_TradeDel, q_TailPick,
            q_TailDel, q_AppPick, q_AppDel, q_InsPick, q_InsDel;

        public string q_Weight1, q_Weight2, q_Weight3, q_Weight4,
            q_Length1, q_Length2, q_Length3, q_Length4,
            q_Width1, q_Width2, q_Width3, q_Width4,
            q_Height1, q_Height2, q_Height3, q_Height4,
            txtPallet1, txtPallet2, txtPallet3, txtPallet4,
            lblClass1, lblClass2, lblClass3, lblClass4,
            q_Class1, q_Class2, q_Class3, q_Class4,
            lblWeight1, lblWeight2, lblWeight3, lblWeight4,
            lblType1, lblType2, lblType3, lblType4,
            lblPiece1, lblPiece2, lblPiece3, lblPiece4,
            q_Piece1, q_Piece2, q_Piece3, q_Piece4,
            txtNMFC1, txtNMFC2, txtNMFC3, txtNMFC4,
            q_Unit1, q_Unit2, q_Unit3, q_Unit4,
            Commodity1, Commodity2, Commodity3, Commodity4, commodityName;

        public string ddlDeliveryAddress, ddlPickupAddress, rdblClientType, rdblBill;

        public string ddlRHour, ddlRMinute, ddlRAMPM, ddlCHour, ddlCMinute, ddlCAMPM;

        public string exportPath, exportPath2, BOLReportName2, strReportFileName, strReportFileName2;
        
    }
}