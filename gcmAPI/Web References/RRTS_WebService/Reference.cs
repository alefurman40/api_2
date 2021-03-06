﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace gcmAPI.RRTS_WebService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1099.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="RateQuoteSoap", Namespace="https://webservices.rrts.com/ratequote/")]
    public partial class RateQuote : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private AuthenticationHeader authenticationHeaderValueField;
        
        private System.Threading.SendOrPostCallback CallRateQuoteOperationCompleted;
        
        private System.Threading.SendOrPostCallback RateQuoteByAccountOperationCompleted;
        
        private System.Threading.SendOrPostCallback RateQuoteByAccountByTerminalOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public RateQuote() {
            this.Url = global::gcmAPI.Properties.Settings.Default.gcmAPI_RRTS_WebService_RateQuote;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public AuthenticationHeader AuthenticationHeaderValue {
            get {
                return this.authenticationHeaderValueField;
            }
            set {
                this.authenticationHeaderValueField = value;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event CallRateQuoteCompletedEventHandler CallRateQuoteCompleted;
        
        /// <remarks/>
        public event RateQuoteByAccountCompletedEventHandler RateQuoteByAccountCompleted;
        
        /// <remarks/>
        public event RateQuoteByAccountByTerminalCompletedEventHandler RateQuoteByAccountByTerminalCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapHeaderAttribute("AuthenticationHeaderValue")]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("https://webservices.rrts.com/ratequote/RateQuote", RequestElementName="RateQuote", RequestNamespace="https://webservices.rrts.com/ratequote/", ResponseElementName="RateQuoteResponse", ResponseNamespace="https://webservices.rrts.com/ratequote/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("RateQuoteResult")]
        public QuoteResponse CallRateQuote(QuoteRequest request) {
            object[] results = this.Invoke("CallRateQuote", new object[] {
                        request});
            return ((QuoteResponse)(results[0]));
        }
        
        /// <remarks/>
        public void CallRateQuoteAsync(QuoteRequest request) {
            this.CallRateQuoteAsync(request, null);
        }
        
        /// <remarks/>
        public void CallRateQuoteAsync(QuoteRequest request, object userState) {
            if ((this.CallRateQuoteOperationCompleted == null)) {
                this.CallRateQuoteOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCallRateQuoteOperationCompleted);
            }
            this.InvokeAsync("CallRateQuote", new object[] {
                        request}, this.CallRateQuoteOperationCompleted, userState);
        }
        
        private void OnCallRateQuoteOperationCompleted(object arg) {
            if ((this.CallRateQuoteCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CallRateQuoteCompleted(this, new CallRateQuoteCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapHeaderAttribute("AuthenticationHeaderValue")]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("https://webservices.rrts.com/ratequote/RateQuoteByAccount", RequestNamespace="https://webservices.rrts.com/ratequote/", ResponseNamespace="https://webservices.rrts.com/ratequote/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public QuoteResponse RateQuoteByAccount(QuoteAccountRequest request) {
            object[] results = this.Invoke("RateQuoteByAccount", new object[] {
                        request});
            return ((QuoteResponse)(results[0]));
        }
        
        /// <remarks/>
        public void RateQuoteByAccountAsync(QuoteAccountRequest request) {
            this.RateQuoteByAccountAsync(request, null);
        }
        
        /// <remarks/>
        public void RateQuoteByAccountAsync(QuoteAccountRequest request, object userState) {
            if ((this.RateQuoteByAccountOperationCompleted == null)) {
                this.RateQuoteByAccountOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRateQuoteByAccountOperationCompleted);
            }
            this.InvokeAsync("RateQuoteByAccount", new object[] {
                        request}, this.RateQuoteByAccountOperationCompleted, userState);
        }
        
        private void OnRateQuoteByAccountOperationCompleted(object arg) {
            if ((this.RateQuoteByAccountCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RateQuoteByAccountCompleted(this, new RateQuoteByAccountCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapHeaderAttribute("AuthenticationHeaderValue")]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("https://webservices.rrts.com/ratequote/RateQuoteByAccountByTerminal", RequestNamespace="https://webservices.rrts.com/ratequote/", ResponseNamespace="https://webservices.rrts.com/ratequote/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public QuoteResponse RateQuoteByAccountByTerminal(QuoteAccountByTerminalRequest request) {
            object[] results = this.Invoke("RateQuoteByAccountByTerminal", new object[] {
                        request});
            return ((QuoteResponse)(results[0]));
        }
        
        /// <remarks/>
        public void RateQuoteByAccountByTerminalAsync(QuoteAccountByTerminalRequest request) {
            this.RateQuoteByAccountByTerminalAsync(request, null);
        }
        
        /// <remarks/>
        public void RateQuoteByAccountByTerminalAsync(QuoteAccountByTerminalRequest request, object userState) {
            if ((this.RateQuoteByAccountByTerminalOperationCompleted == null)) {
                this.RateQuoteByAccountByTerminalOperationCompleted = new System.Threading.SendOrPostCallback(this.OnRateQuoteByAccountByTerminalOperationCompleted);
            }
            this.InvokeAsync("RateQuoteByAccountByTerminal", new object[] {
                        request}, this.RateQuoteByAccountByTerminalOperationCompleted, userState);
        }
        
        private void OnRateQuoteByAccountByTerminalOperationCompleted(object arg) {
            if ((this.RateQuoteByAccountByTerminalCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.RateQuoteByAccountByTerminalCompleted(this, new RateQuoteByAccountByTerminalCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://webservices.rrts.com/ratequote/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="https://webservices.rrts.com/ratequote/", IsNullable=false)]
    public partial class AuthenticationHeader : System.Web.Services.Protocols.SoapHeader {
        
        private string userNameField;
        
        private string passwordField;
        
        private string siteField;
        
        private System.Xml.XmlAttribute[] anyAttrField;
        
        /// <remarks/>
        public string UserName {
            get {
                return this.userNameField;
            }
            set {
                this.userNameField = value;
            }
        }
        
        /// <remarks/>
        public string Password {
            get {
                return this.passwordField;
            }
            set {
                this.passwordField = value;
            }
        }
        
        /// <remarks/>
        public string Site {
            get {
                return this.siteField;
            }
            set {
                this.siteField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr {
            get {
                return this.anyAttrField;
            }
            set {
                this.anyAttrField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://webservices.rrts.com/ratequote/")]
    public partial class QuoteDetail {
        
        private string actualClassField;
        
        private string ratedClassField;
        
        private double chargeField;
        
        private string codeField;
        
        private string descriptionField;
        
        private double rateField;
        
        private string weightField;
        
        private string extraMessagesField;
        
        /// <remarks/>
        public string ActualClass {
            get {
                return this.actualClassField;
            }
            set {
                this.actualClassField = value;
            }
        }
        
        /// <remarks/>
        public string RatedClass {
            get {
                return this.ratedClassField;
            }
            set {
                this.ratedClassField = value;
            }
        }
        
        /// <remarks/>
        public double Charge {
            get {
                return this.chargeField;
            }
            set {
                this.chargeField = value;
            }
        }
        
        /// <remarks/>
        public string Code {
            get {
                return this.codeField;
            }
            set {
                this.codeField = value;
            }
        }
        
        /// <remarks/>
        public string Description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
            }
        }
        
        /// <remarks/>
        public double Rate {
            get {
                return this.rateField;
            }
            set {
                this.rateField = value;
            }
        }
        
        /// <remarks/>
        public string Weight {
            get {
                return this.weightField;
            }
            set {
                this.weightField = value;
            }
        }
        
        /// <remarks/>
        public string ExtraMessages {
            get {
                return this.extraMessagesField;
            }
            set {
                this.extraMessagesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://webservices.rrts.com/ratequote/")]
    public partial class RoutingInformation {
        
        private string destinationStateField;
        
        private string destinationZipField;
        
        private string originStateField;
        
        private string originZipField;
        
        private string estimatedTransitDaysField;
        
        private string originTerminalField;
        
        /// <remarks/>
        public string DestinationState {
            get {
                return this.destinationStateField;
            }
            set {
                this.destinationStateField = value;
            }
        }
        
        /// <remarks/>
        public string DestinationZip {
            get {
                return this.destinationZipField;
            }
            set {
                this.destinationZipField = value;
            }
        }
        
        /// <remarks/>
        public string OriginState {
            get {
                return this.originStateField;
            }
            set {
                this.originStateField = value;
            }
        }
        
        /// <remarks/>
        public string OriginZip {
            get {
                return this.originZipField;
            }
            set {
                this.originZipField = value;
            }
        }
        
        /// <remarks/>
        public string EstimatedTransitDays {
            get {
                return this.estimatedTransitDaysField;
            }
            set {
                this.estimatedTransitDaysField = value;
            }
        }
        
        /// <remarks/>
        public string OriginTerminal {
            get {
                return this.originTerminalField;
            }
            set {
                this.originTerminalField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://webservices.rrts.com/ratequote/")]
    public partial class CustomerInformation {
        
        private int accountNumberField;
        
        private string nameField;
        
        private string address1Field;
        
        private string address2Field;
        
        private string cityField;
        
        private string stateField;
        
        private string zipCodeField;
        
        /// <remarks/>
        public int AccountNumber {
            get {
                return this.accountNumberField;
            }
            set {
                this.accountNumberField = value;
            }
        }
        
        /// <remarks/>
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public string Address1 {
            get {
                return this.address1Field;
            }
            set {
                this.address1Field = value;
            }
        }
        
        /// <remarks/>
        public string Address2 {
            get {
                return this.address2Field;
            }
            set {
                this.address2Field = value;
            }
        }
        
        /// <remarks/>
        public string City {
            get {
                return this.cityField;
            }
            set {
                this.cityField = value;
            }
        }
        
        /// <remarks/>
        public string State {
            get {
                return this.stateField;
            }
            set {
                this.stateField = value;
            }
        }
        
        /// <remarks/>
        public string ZipCode {
            get {
                return this.zipCodeField;
            }
            set {
                this.zipCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://webservices.rrts.com/ratequote/")]
    public partial class QuoteResponse {
        
        private int quoteNumberField;
        
        private double netChargeField;
        
        private CustomerInformation customerField;
        
        private RoutingInformation routingInfoField;
        
        private QuoteDetail[] rateDetailsField;
        
        private string originTypeField;
        
        private string paymentTypeField;
        
        private double cODAmountField;
        
        private System.DateTime shipmentDateField;
        
        private int customerCubicFootField;
        
        private int hawaiianRatedCubicFootField;
        
        /// <remarks/>
        public int QuoteNumber {
            get {
                return this.quoteNumberField;
            }
            set {
                this.quoteNumberField = value;
            }
        }
        
        /// <remarks/>
        public double NetCharge {
            get {
                return this.netChargeField;
            }
            set {
                this.netChargeField = value;
            }
        }
        
        /// <remarks/>
        public CustomerInformation Customer {
            get {
                return this.customerField;
            }
            set {
                this.customerField = value;
            }
        }
        
        /// <remarks/>
        public RoutingInformation RoutingInfo {
            get {
                return this.routingInfoField;
            }
            set {
                this.routingInfoField = value;
            }
        }
        
        /// <remarks/>
        public QuoteDetail[] RateDetails {
            get {
                return this.rateDetailsField;
            }
            set {
                this.rateDetailsField = value;
            }
        }
        
        /// <remarks/>
        public string OriginType {
            get {
                return this.originTypeField;
            }
            set {
                this.originTypeField = value;
            }
        }
        
        /// <remarks/>
        public string PaymentType {
            get {
                return this.paymentTypeField;
            }
            set {
                this.paymentTypeField = value;
            }
        }
        
        /// <remarks/>
        public double CODAmount {
            get {
                return this.cODAmountField;
            }
            set {
                this.cODAmountField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime ShipmentDate {
            get {
                return this.shipmentDateField;
            }
            set {
                this.shipmentDateField = value;
            }
        }
        
        /// <remarks/>
        public int CustomerCubicFoot {
            get {
                return this.customerCubicFootField;
            }
            set {
                this.customerCubicFootField = value;
            }
        }
        
        /// <remarks/>
        public int HawaiianRatedCubicFoot {
            get {
                return this.hawaiianRatedCubicFootField;
            }
            set {
                this.hawaiianRatedCubicFootField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://webservices.rrts.com/ratequote/")]
    public partial class CODFee {
        
        private bool prepaidField;
        
        private double cODAmountField;
        
        /// <remarks/>
        public bool Prepaid {
            get {
                return this.prepaidField;
            }
            set {
                this.prepaidField = value;
            }
        }
        
        /// <remarks/>
        public double CODAmount {
            get {
                return this.cODAmountField;
            }
            set {
                this.cODAmountField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://webservices.rrts.com/ratequote/")]
    public partial class ServiceOptions {
        
        private string serviceCodeField;
        
        /// <remarks/>
        public string ServiceCode {
            get {
                return this.serviceCodeField;
            }
            set {
                this.serviceCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://webservices.rrts.com/ratequote/")]
    public partial class ShipmentDetail {
        
        private double actualClassField;
        
        private int weightField;
        
        /// <remarks/>
        public double ActualClass {
            get {
                return this.actualClassField;
            }
            set {
                this.actualClassField = value;
            }
        }
        
        /// <remarks/>
        public int Weight {
            get {
                return this.weightField;
            }
            set {
                this.weightField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(QuoteAccountRequest))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(QuoteAccountByTerminalRequest))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://webservices.rrts.com/ratequote/")]
    public partial class QuoteRequest {
        
        private string originZipField;
        
        private string destinationZipField;
        
        private ShipmentDetail[] shipmentDetailsField;
        
        private string originTypeField;
        
        private string paymentTypeField;
        
        private string palletCountField;
        
        private string linearFeetField;
        
        private string cubicFeetField;
        
        private int piecesField;
        
        private ServiceOptions[] serviceDeliveryOptionsField;
        
        private CODFee cODField;
        
        private string discountField;
        
        private string listedConsigneeCityField;
        
        private string internalUseField;
        
        private string palletPositionField;
        
        private System.DateTime shipDateField;
        
        /// <remarks/>
        public string OriginZip {
            get {
                return this.originZipField;
            }
            set {
                this.originZipField = value;
            }
        }
        
        /// <remarks/>
        public string DestinationZip {
            get {
                return this.destinationZipField;
            }
            set {
                this.destinationZipField = value;
            }
        }
        
        /// <remarks/>
        public ShipmentDetail[] ShipmentDetails {
            get {
                return this.shipmentDetailsField;
            }
            set {
                this.shipmentDetailsField = value;
            }
        }
        
        /// <remarks/>
        public string OriginType {
            get {
                return this.originTypeField;
            }
            set {
                this.originTypeField = value;
            }
        }
        
        /// <remarks/>
        public string PaymentType {
            get {
                return this.paymentTypeField;
            }
            set {
                this.paymentTypeField = value;
            }
        }
        
        /// <remarks/>
        public string PalletCount {
            get {
                return this.palletCountField;
            }
            set {
                this.palletCountField = value;
            }
        }
        
        /// <remarks/>
        public string LinearFeet {
            get {
                return this.linearFeetField;
            }
            set {
                this.linearFeetField = value;
            }
        }
        
        /// <remarks/>
        public string CubicFeet {
            get {
                return this.cubicFeetField;
            }
            set {
                this.cubicFeetField = value;
            }
        }
        
        /// <remarks/>
        public int Pieces {
            get {
                return this.piecesField;
            }
            set {
                this.piecesField = value;
            }
        }
        
        /// <remarks/>
        public ServiceOptions[] ServiceDeliveryOptions {
            get {
                return this.serviceDeliveryOptionsField;
            }
            set {
                this.serviceDeliveryOptionsField = value;
            }
        }
        
        /// <remarks/>
        public CODFee COD {
            get {
                return this.cODField;
            }
            set {
                this.cODField = value;
            }
        }
        
        /// <remarks/>
        public string Discount {
            get {
                return this.discountField;
            }
            set {
                this.discountField = value;
            }
        }
        
        /// <remarks/>
        public string ListedConsigneeCity {
            get {
                return this.listedConsigneeCityField;
            }
            set {
                this.listedConsigneeCityField = value;
            }
        }
        
        /// <remarks/>
        public string InternalUse {
            get {
                return this.internalUseField;
            }
            set {
                this.internalUseField = value;
            }
        }
        
        /// <remarks/>
        public string PalletPosition {
            get {
                return this.palletPositionField;
            }
            set {
                this.palletPositionField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime ShipDate {
            get {
                return this.shipDateField;
            }
            set {
                this.shipDateField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(QuoteAccountByTerminalRequest))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://webservices.rrts.com/ratequote/")]
    public partial class QuoteAccountRequest : QuoteRequest {
        
        private int accountField;
        
        /// <remarks/>
        public int Account {
            get {
                return this.accountField;
            }
            set {
                this.accountField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://webservices.rrts.com/ratequote/")]
    public partial class QuoteAccountByTerminalRequest : QuoteAccountRequest {
        
        private string terminalCodeField;
        
        /// <remarks/>
        public string TerminalCode {
            get {
                return this.terminalCodeField;
            }
            set {
                this.terminalCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1099.0")]
    public delegate void CallRateQuoteCompletedEventHandler(object sender, CallRateQuoteCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1099.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CallRateQuoteCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CallRateQuoteCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public QuoteResponse Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((QuoteResponse)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1099.0")]
    public delegate void RateQuoteByAccountCompletedEventHandler(object sender, RateQuoteByAccountCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1099.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RateQuoteByAccountCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal RateQuoteByAccountCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public QuoteResponse Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((QuoteResponse)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1099.0")]
    public delegate void RateQuoteByAccountByTerminalCompletedEventHandler(object sender, RateQuoteByAccountByTerminalCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1099.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class RateQuoteByAccountByTerminalCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal RateQuoteByAccountByTerminalCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public QuoteResponse Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((QuoteResponse)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591