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

namespace gcmAPI.RL_CarriersTransitTimes_API {
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
    [System.Web.Services.WebServiceBindingAttribute(Name="TransitTimesServiceSoap", Namespace="http://www.rlcarriers.com/")]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(BaseReply))]
    public partial class TransitTimesService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetTransitTimesOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public TransitTimesService() {
            this.Url = global::gcmAPI.Properties.Settings.Default.gcmAPI_RL_CarriersTransitTimes_API_TransitTimesService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
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
        public event GetTransitTimesCompletedEventHandler GetTransitTimesCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.rlcarriers.com/GetTransitTimes", RequestNamespace="http://www.rlcarriers.com/", ResponseNamespace="http://www.rlcarriers.com/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public TransitTimesReply GetTransitTimes(string APIKey, TransitTimesRequest input) {
            object[] results = this.Invoke("GetTransitTimes", new object[] {
                        APIKey,
                        input});
            return ((TransitTimesReply)(results[0]));
        }
        
        /// <remarks/>
        public void GetTransitTimesAsync(string APIKey, TransitTimesRequest input) {
            this.GetTransitTimesAsync(APIKey, input, null);
        }
        
        /// <remarks/>
        public void GetTransitTimesAsync(string APIKey, TransitTimesRequest input, object userState) {
            if ((this.GetTransitTimesOperationCompleted == null)) {
                this.GetTransitTimesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetTransitTimesOperationCompleted);
            }
            this.InvokeAsync("GetTransitTimes", new object[] {
                        APIKey,
                        input}, this.GetTransitTimesOperationCompleted, userState);
        }
        
        private void OnGetTransitTimesOperationCompleted(object arg) {
            if ((this.GetTransitTimesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetTransitTimesCompleted(this, new GetTransitTimesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.rlcarriers.com/")]
    public partial class TransitTimesRequest {
        
        private string customerDataField;
        
        private string dateOfPickupField;
        
        private ServicePoint originPointField;
        
        private Destinations destinationsField;
        
        /// <remarks/>
        public string CustomerData {
            get {
                return this.customerDataField;
            }
            set {
                this.customerDataField = value;
            }
        }
        
        /// <remarks/>
        public string DateOfPickup {
            get {
                return this.dateOfPickupField;
            }
            set {
                this.dateOfPickupField = value;
            }
        }
        
        /// <remarks/>
        public ServicePoint OriginPoint {
            get {
                return this.originPointField;
            }
            set {
                this.originPointField = value;
            }
        }
        
        /// <remarks/>
        public Destinations Destinations {
            get {
                return this.destinationsField;
            }
            set {
                this.destinationsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ServicePoint {
        
        private string cityField;
        
        private string stateOrProvinceField;
        
        private string zipOrPostalCodeField;
        
        private string countryCodeField;
        
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
        public string StateOrProvince {
            get {
                return this.stateOrProvinceField;
            }
            set {
                this.stateOrProvinceField = value;
            }
        }
        
        /// <remarks/>
        public string ZipOrPostalCode {
            get {
                return this.zipOrPostalCodeField;
            }
            set {
                this.zipOrPostalCodeField = value;
            }
        }
        
        /// <remarks/>
        public string CountryCode {
            get {
                return this.countryCodeField;
            }
            set {
                this.countryCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ErrorResult {
        
        private string levelField;
        
        private string messageField;
        
        /// <remarks/>
        public string Level {
            get {
                return this.levelField;
            }
            set {
                this.levelField = value;
            }
        }
        
        /// <remarks/>
        public string Message {
            get {
                return this.messageField;
            }
            set {
                this.messageField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ServiceLevel {
        
        private string serviceLevelCodeField;
        
        private string serviceLevelNameField;
        
        /// <remarks/>
        public string ServiceLevelCode {
            get {
                return this.serviceLevelCodeField;
            }
            set {
                this.serviceLevelCodeField = value;
            }
        }
        
        /// <remarks/>
        public string ServiceLevelName {
            get {
                return this.serviceLevelNameField;
            }
            set {
                this.serviceLevelNameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class Destination {
        
        private ServicePoint destinationPointField;
        
        private ServiceCenterInfo serviceCenterInfoField;
        
        private ServiceLevel[] serviceLevelsField;
        
        private int serviceDaysField;
        
        private System.DateTime deliveryDateField;
        
        private string destinationNotesField;
        
        private ErrorResult resultField;
        
        /// <remarks/>
        public ServicePoint DestinationPoint {
            get {
                return this.destinationPointField;
            }
            set {
                this.destinationPointField = value;
            }
        }
        
        /// <remarks/>
        public ServiceCenterInfo ServiceCenterInfo {
            get {
                return this.serviceCenterInfoField;
            }
            set {
                this.serviceCenterInfoField = value;
            }
        }
        
        /// <remarks/>
        public ServiceLevel[] ServiceLevels {
            get {
                return this.serviceLevelsField;
            }
            set {
                this.serviceLevelsField = value;
            }
        }
        
        /// <remarks/>
        public int ServiceDays {
            get {
                return this.serviceDaysField;
            }
            set {
                this.serviceDaysField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime DeliveryDate {
            get {
                return this.deliveryDateField;
            }
            set {
                this.deliveryDateField = value;
            }
        }
        
        /// <remarks/>
        public string DestinationNotes {
            get {
                return this.destinationNotesField;
            }
            set {
                this.destinationNotesField = value;
            }
        }
        
        /// <remarks/>
        public ErrorResult Result {
            get {
                return this.resultField;
            }
            set {
                this.resultField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class ServiceCenterInfo {
        
        private string serviceCenterCodeField;
        
        private string serviceCenterLocationField;
        
        private string serviceCenterPhoneField;
        
        /// <remarks/>
        public string ServiceCenterCode {
            get {
                return this.serviceCenterCodeField;
            }
            set {
                this.serviceCenterCodeField = value;
            }
        }
        
        /// <remarks/>
        public string ServiceCenterLocation {
            get {
                return this.serviceCenterLocationField;
            }
            set {
                this.serviceCenterLocationField = value;
            }
        }
        
        /// <remarks/>
        public string ServiceCenterPhone {
            get {
                return this.serviceCenterPhoneField;
            }
            set {
                this.serviceCenterPhoneField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class Origin {
        
        private ServicePoint originPointField;
        
        private ServiceCenterInfo serviceCenterInfoField;
        
        /// <remarks/>
        public ServicePoint OriginPoint {
            get {
                return this.originPointField;
            }
            set {
                this.originPointField = value;
            }
        }
        
        /// <remarks/>
        public ServiceCenterInfo ServiceCenterInfo {
            get {
                return this.serviceCenterInfoField;
            }
            set {
                this.serviceCenterInfoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class GetTransitTimesResult {
        
        private int transitTimesIDField;
        
        private string myRLCIDField;
        
        private System.DateTime pickupDateField;
        
        private Origin originField;
        
        private Destination[] destinationsField;
        
        /// <remarks/>
        public int TransitTimesID {
            get {
                return this.transitTimesIDField;
            }
            set {
                this.transitTimesIDField = value;
            }
        }
        
        /// <remarks/>
        public string MyRLCID {
            get {
                return this.myRLCIDField;
            }
            set {
                this.myRLCIDField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime PickupDate {
            get {
                return this.pickupDateField;
            }
            set {
                this.pickupDateField = value;
            }
        }
        
        /// <remarks/>
        public Origin Origin {
            get {
                return this.originField;
            }
            set {
                this.originField = value;
            }
        }
        
        /// <remarks/>
        public Destination[] Destinations {
            get {
                return this.destinationsField;
            }
            set {
                this.destinationsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(TransitTimesReply))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.rlcarriers.com/")]
    public partial class BaseReply {
        
        private string customerDataField;
        
        private bool wasSuccessField;
        
        private string[] messagesField;
        
        /// <remarks/>
        public string CustomerData {
            get {
                return this.customerDataField;
            }
            set {
                this.customerDataField = value;
            }
        }
        
        /// <remarks/>
        public bool WasSuccess {
            get {
                return this.wasSuccessField;
            }
            set {
                this.wasSuccessField = value;
            }
        }
        
        /// <remarks/>
        public string[] Messages {
            get {
                return this.messagesField;
            }
            set {
                this.messagesField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.rlcarriers.com/")]
    public partial class TransitTimesReply : BaseReply {
        
        private GetTransitTimesResult resultField;
        
        /// <remarks/>
        public GetTransitTimesResult Result {
            get {
                return this.resultField;
            }
            set {
                this.resultField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2634.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class Destinations {
        
        private ServicePoint[] destinationPointField;
        
        /// <remarks/>
        public ServicePoint[] DestinationPoint {
            get {
                return this.destinationPointField;
            }
            set {
                this.destinationPointField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1099.0")]
    public delegate void GetTransitTimesCompletedEventHandler(object sender, GetTransitTimesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1099.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetTransitTimesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetTransitTimesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public TransitTimesReply Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((TransitTimesReply)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591