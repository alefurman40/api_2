#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
#endregion

namespace gcmAPI.Models.Dispatch
{
    public class Logger
    {
        DispatchData dispatch_data;
        StringBuilder sb;

        public Logger(ref DispatchData dispatch_data, ref StringBuilder sb)
        {
            this.dispatch_data = dispatch_data;
            this.sb = sb;
        }

        #region Log_all_request_info
        public void Log_all_request_info()
        {
            sb.Append(string.Concat(
                "is_response=true&intCustCompanyID=", dispatch_data.intCustCompanyID,
                "&intShipmentID=", dispatch_data.intShipmentID,
                "&intSegmentID=", dispatch_data.intSegmentID,
                "&intSegmentID2=", dispatch_data.intSegmentID2,
                "&intCarrierCompID=", dispatch_data.intCarrierCompID,
                "&intRQID=", dispatch_data.intRQID,
                "&intPUCompID=", dispatch_data.intPUCompID,

                "&glbDeliveryDate=", dispatch_data.glbDeliveryDate,
                "&txtShipmentDate=", dispatch_data.txtShipmentDate,

                "&q_ShipmentReadyDate=", dispatch_data.q_ShipmentReadyDate,

                "&deliveryDay=", dispatch_data.deliveryDay,

                "&hasInsurance=", dispatch_data.hasInsurance,
                "&insuranceCost=", dispatch_data.insuranceCost,
                "&minInsuranceCost=", dispatch_data.minInsuranceCost,
                "&isAAFES_Shipment=", dispatch_data.isAAFES_Shipment,

                "&isDUR=", dispatch_data.isDUR,
                "&isAssociationID_5=", dispatch_data.isAssociationID_5,
                "&isUSED=", dispatch_data.isUSED,
                //" intRQID:", dispatch_data.intRQID,

                

                "&oCityState=", dispatch_data.oCityState,
                "&dCityState=", dispatch_data.dCityState,
                "&oState=", dispatch_data.oState,
                "&dState=", dispatch_data.dState,

                "&q_OCity=", dispatch_data.q_OCity,
                "&q_DCity=", dispatch_data.q_DCity,
                "&rate=", dispatch_data.rate,
                "&ourRate=", dispatch_data.ourRate,

                "&txtSellRate=", dispatch_data.txtSellRate,
                
                "&shipmentValue=", dispatch_data.shipmentValue,
                

                "&username=", dispatch_data.username,
                "&repName=", dispatch_data.repName,
                "&Initials=", dispatch_data.Initials,
                "&Status=", dispatch_data.Status,

                "&carrier=", dispatch_data.carrier,
                "&carrierKey=", dispatch_data.carrierKey,
                "&DLS_PrimaryReferencePNW=", dispatch_data.DLS_PrimaryReferencePNW,
                
                "&rateType=", dispatch_data.rateType,
                "&txtComment=", dispatch_data.txtComment,
                "&txtPONumber=", dispatch_data.txtPONumber,

                "&selectedTransit=", dispatch_data.selectedTransit,
                "&selectedOnTime=", dispatch_data.selectedOnTime,
                "&selectedRate=", dispatch_data.selectedRate,
                "&selectedCarrier=", dispatch_data.selectedCarrier,

                "&topCarrier=", dispatch_data.topCarrier,
                "&topOnTime=", dispatch_data.topOnTime,
                "&topTransit=", dispatch_data.topTransit,
                "&topRate=", dispatch_data.topRate,

                "&txtDesc1=", dispatch_data.txtDesc1,
                "&txtDesc2=", dispatch_data.txtDesc2,
                "&txtDesc3=", dispatch_data.txtDesc3,
                "&txtDesc4=", dispatch_data.txtDesc4,

                "&DimsCubeDesc1=", dispatch_data.DimsCubeDesc1,
                "&DimsCubeDesc2=", dispatch_data.DimsCubeDesc2,
                "&DimsCubeDesc3=", dispatch_data.DimsCubeDesc3,
                "&DimsCubeDesc4=", dispatch_data.DimsCubeDesc4,

                Get_Log_companies_info(),

                Get_Log_accessorials_info(),

                Get_Log_line_items_info(),

                "&commodityName=", dispatch_data.commodityName,
                "&ddlDeliveryAddress=", dispatch_data.ddlDeliveryAddress,
                "&ddlPickupAddress=", dispatch_data.ddlPickupAddress,
                "&rdblClientType=", dispatch_data.rdblClientType,

                "&rdblBill=", dispatch_data.rdblBill,
                "&ddlRHour=", dispatch_data.ddlRHour,
                "&ddlRMinute=", dispatch_data.ddlRMinute,
                "&ddlRAMPM=", dispatch_data.ddlRAMPM,

                "&ddlCHour=", dispatch_data.ddlCHour,
                "&ddlCMinute=", dispatch_data.ddlCMinute,
                "&ddlCAMPM=", dispatch_data.ddlCAMPM
                
            )
            );
        }

        #endregion

        #region Get_Log_companies_info
        private string Get_Log_companies_info()
        {
            string log = string.Concat(
                "&txtPName=", dispatch_data.txtPName,
                "&txtPCompany=", dispatch_data.txtPCompany,
                "&txtPAddress1=", dispatch_data.txtPAddress1,
                "&txtPAddress2=", dispatch_data.txtPAddress2,
                "&txtPCity=", dispatch_data.txtPCity,
                "&txtPST=", dispatch_data.txtPST,
                "&txtPZip=", dispatch_data.txtPZip,
                "&txtPEmail=", dispatch_data.txtPEmail,
                "&txtPPhone=", dispatch_data.txtPPhone,
                "&txtPFax=", dispatch_data.txtPFax,

                "&txtDName=", dispatch_data.txtDName,
                "&txtDCompany=", dispatch_data.txtDCompany,
                "&txtDAddress1=", dispatch_data.txtDAddress1,
                "&txtDAddress2=", dispatch_data.txtDAddress2,
                "&txtDCity=", dispatch_data.txtDCity,
                "&txtDST=", dispatch_data.txtDST,
                "&txtDZip=", dispatch_data.txtDZip,
                "&txtDEmail=", dispatch_data.txtDEmail,
                "&txtDPhone=", dispatch_data.txtDPhone,
                "&txtDFax=", dispatch_data.txtDFax,

                "&txtTPName=", dispatch_data.txtTPName,
                "&txtTPCompany=", dispatch_data.txtTPCompany,
                "&txtTPAddress1=", dispatch_data.txtTPAddress1,
                "&txtTPAddress2=", dispatch_data.txtTPAddress2,
                "&txtTPCity=", dispatch_data.txtTPCity,
                "&txtTPST=", dispatch_data.txtTPST,
                "&txtTPZip=", dispatch_data.txtTPZip,
                "&txtTPEmail=", dispatch_data.txtTPEmail,
                "&txtTPPhone=", dispatch_data.txtTPPhone,
                "&txtTPFax=", dispatch_data.txtTPFax
                );
            return log;
        }

        #endregion

        #region Get_Log_accessorials_info
        private string Get_Log_accessorials_info()
        {
            string log = string.Concat(
                "&q_ResPick=", dispatch_data.q_ResPick,
                "&q_ResDel=", dispatch_data.q_ResDel,
                "&q_ConstPick=", dispatch_data.q_ConstPick,
                "&q_ConstDel=", dispatch_data.q_ConstDel,
                "&q_TradePick=", dispatch_data.q_TradePick,
                "&q_TradeDel=", dispatch_data.q_TradeDel,
                "&q_TailPick=", dispatch_data.q_TailPick,
                "&q_TailDel=", dispatch_data.q_TailDel,
                "&q_AppPick=", dispatch_data.q_AppPick,
                "&q_AppDel=", dispatch_data.q_AppDel,

                "&q_InsDel=", dispatch_data.q_InsDel,

                "&chkHazMat1=", dispatch_data.chkHazMat1,
                "&chkHazMat2=", dispatch_data.chkHazMat2,
                "&chkHazMat3=", dispatch_data.chkHazMat3,
                "&chkHazMat4=", dispatch_data.chkHazMat4,

                "&q_HazMat1=", dispatch_data.q_HazMat1,
                "&q_HazMat2=", dispatch_data.q_HazMat2,
                "&q_HazMat3=", dispatch_data.q_HazMat3,
                "&q_HazMat4=", dispatch_data.q_HazMat4
                );
            return log;
        }

        #endregion

        #region Get_Log_accessorials_info
        private string Get_Log_line_items_info()
        {
            string log = string.Concat(
                "&q_Weight1=", dispatch_data.q_Weight1,
                "&q_Weight2=", dispatch_data.q_Weight2,
                "&q_Weight3=", dispatch_data.q_Weight3,
                "&q_Weight4=", dispatch_data.q_Weight4,

                "&q_Length1=", dispatch_data.q_Length1,
                "&q_Length2=", dispatch_data.q_Length2,
                "&q_Length3=", dispatch_data.q_Length3,
                "&q_Length4=", dispatch_data.q_Length4,

                "&q_Width1=", dispatch_data.q_Width1,
                "&q_Width2=", dispatch_data.q_Width2,
                "&q_Width3=", dispatch_data.q_Width3,
                "&q_Width4=", dispatch_data.q_Width4,

                "&q_Height1=", dispatch_data.q_Height1,
                "&q_Height2=", dispatch_data.q_Height2,
                "&q_Height3=", dispatch_data.q_Height3,
                "&q_Height4=", dispatch_data.q_Height4,

                "&txtPallet1=", dispatch_data.txtPallet1,
                "&txtPallet2=", dispatch_data.txtPallet2,
                "&txtPallet3=", dispatch_data.txtPallet3,
                "&txtPallet4=", dispatch_data.txtPallet4,

                "&lblClass1=", dispatch_data.lblClass1,
                "&lblClass2=", dispatch_data.lblClass2,
                "&lblClass3=", dispatch_data.lblClass3,
                "&lblClass4=", dispatch_data.lblClass4,

                "&q_Class1=", dispatch_data.q_Class1,
                "&q_Class2=", dispatch_data.q_Class2,
                "&q_Class3=", dispatch_data.q_Class3,
                "&q_Class4=", dispatch_data.q_Class4,

                "&lblWeight1=", dispatch_data.lblWeight1,
                "&lblWeight2=", dispatch_data.lblWeight2,
                "&lblWeight3=", dispatch_data.lblWeight3,
                "&lblWeight4=", dispatch_data.lblWeight4,

                "&lblType1=", dispatch_data.lblType1,
                "&lblType2=", dispatch_data.lblType2,
                "&lblType3=", dispatch_data.lblType3,
                "&lblType4=", dispatch_data.lblType4,

                "&lblPiece1=", dispatch_data.lblPiece1,
                "&lblPiece2=", dispatch_data.lblPiece2,
                "&lblPiece3=", dispatch_data.lblPiece3,
                "&lblPiece4=", dispatch_data.lblPiece4,

                "&q_Piece1=", dispatch_data.q_Piece1,
                "&q_Piece2=", dispatch_data.q_Piece2,
                "&q_Piece3=", dispatch_data.q_Piece3,
                "&q_Piece4=", dispatch_data.q_Piece4,

                "&txtNMFC1=", dispatch_data.txtNMFC1,
                "&txtNMFC2=", dispatch_data.txtNMFC2,
                "&txtNMFC3=", dispatch_data.txtNMFC3,
                "&txtNMFC4=", dispatch_data.txtNMFC4,

                "&q_Unit1=", dispatch_data.q_Unit1,
                "&q_Unit2=", dispatch_data.q_Unit2,
                "&q_Unit3=", dispatch_data.q_Unit3,
                "&q_Unit4=", dispatch_data.q_Unit4,

                "&Commodity1=", dispatch_data.Commodity1,
                "&Commodity2=", dispatch_data.Commodity2,
                "&Commodity3=", dispatch_data.Commodity3,
                "&Commodity4=", dispatch_data.Commodity4

                //"&q_HazMat4=", dispatch_data.q_HazMat4
                );
            return log;
        }

        #endregion

    }
}