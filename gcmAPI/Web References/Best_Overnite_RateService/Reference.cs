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

namespace gcmAPI.Best_Overnite_RateService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="TQUOTEAPISoap11Binding", Namespace="http://tquoteapi.wsbeans.iseries")]
    public partial class TQUOTEAPI : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback getquote_XMLOperationCompleted;
        
        private System.Threading.SendOrPostCallback getquoteOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public TQUOTEAPI() {
            this.Url = global::gcmAPI.Properties.Settings.Default.gcmAPI_Best_Overnite_RateService_TQUOTEAPI;
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
        public event getquote_XMLCompletedEventHandler getquote_XMLCompleted;
        
        /// <remarks/>
        public event getquoteCompletedEventHandler getquoteCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:getquote_XML", RequestNamespace="http://tquoteapi.wsbeans.iseries/xsd", ResponseNamespace="http://tquoteapi.wsbeans.iseries/xsd", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public string getquote_XML(GETQUOTEInput args0) {
            object[] results = this.Invoke("getquote_XML", new object[] {
                        args0});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void getquote_XMLAsync(GETQUOTEInput args0) {
            this.getquote_XMLAsync(args0, null);
        }
        
        /// <remarks/>
        public void getquote_XMLAsync(GETQUOTEInput args0, object userState) {
            if ((this.getquote_XMLOperationCompleted == null)) {
                this.getquote_XMLOperationCompleted = new System.Threading.SendOrPostCallback(this.Ongetquote_XMLOperationCompleted);
            }
            this.InvokeAsync("getquote_XML", new object[] {
                        args0}, this.getquote_XMLOperationCompleted, userState);
        }
        
        private void Ongetquote_XMLOperationCompleted(object arg) {
            if ((this.getquote_XMLCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getquote_XMLCompleted(this, new getquote_XMLCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("urn:getquote", RequestNamespace="http://tquoteapi.wsbeans.iseries/xsd", ResponseNamespace="http://tquoteapi.wsbeans.iseries/xsd", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public GETQUOTEResult getquote(GETQUOTEInput args0) {
            object[] results = this.Invoke("getquote", new object[] {
                        args0});
            return ((GETQUOTEResult)(results[0]));
        }
        
        /// <remarks/>
        public void getquoteAsync(GETQUOTEInput args0) {
            this.getquoteAsync(args0, null);
        }
        
        /// <remarks/>
        public void getquoteAsync(GETQUOTEInput args0, object userState) {
            if ((this.getquoteOperationCompleted == null)) {
                this.getquoteOperationCompleted = new System.Threading.SendOrPostCallback(this.OngetquoteOperationCompleted);
            }
            this.InvokeAsync("getquote", new object[] {
                        args0}, this.getquoteOperationCompleted, userState);
        }
        
        private void OngetquoteOperationCompleted(object arg) {
            if ((this.getquoteCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.getquoteCompleted(this, new getquoteCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3221.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tquoteapi.wsbeans.iseries/xsd")]
    public partial class GETQUOTEInput {
        
        private QUOTEDS qUOTEField;
        
        private SECURITYINFODS sECURITYINFOField;
        
        /// <remarks/>
        public QUOTEDS QUOTE {
            get {
                return this.qUOTEField;
            }
            set {
                this.qUOTEField = value;
            }
        }
        
        /// <remarks/>
        public SECURITYINFODS SECURITYINFO {
            get {
                return this.sECURITYINFOField;
            }
            set {
                this.sECURITYINFOField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3221.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tquoteapi.wsbeans.iseries/xsd")]
    public partial class QUOTEDS {
        
        private ACCESSORIALDS[] aCCESSORIALField;
        
        private int aCCESSORIALCOUNTField;
        
        private string bOLField;
        
        private decimal cODAMTField;
        
        private string cODTERMSField;
        
        private string cODWHOField;
        
        private CUSTINFO cONSIGNEEField;
        
        private string iAMField;
        
        private ITEMDS[] iTEMField;
        
        private int iTEMCOUNTField;
        
        private string poField;
        
        private string pPDCOLField;
        
        private CUSTINFO sHIPPERField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ACCESSORIAL")]
        public ACCESSORIALDS[] ACCESSORIAL {
            get {
                return this.aCCESSORIALField;
            }
            set {
                this.aCCESSORIALField = value;
            }
        }
        
        /// <remarks/>
        public int ACCESSORIALCOUNT {
            get {
                return this.aCCESSORIALCOUNTField;
            }
            set {
                this.aCCESSORIALCOUNTField = value;
            }
        }
        
        /// <remarks/>
        public string BOL {
            get {
                return this.bOLField;
            }
            set {
                this.bOLField = value;
            }
        }
        
        /// <remarks/>
        public decimal CODAMT {
            get {
                return this.cODAMTField;
            }
            set {
                this.cODAMTField = value;
            }
        }
        
        /// <remarks/>
        public string CODTERMS {
            get {
                return this.cODTERMSField;
            }
            set {
                this.cODTERMSField = value;
            }
        }
        
        /// <remarks/>
        public string CODWHO {
            get {
                return this.cODWHOField;
            }
            set {
                this.cODWHOField = value;
            }
        }
        
        /// <remarks/>
        public CUSTINFO CONSIGNEE {
            get {
                return this.cONSIGNEEField;
            }
            set {
                this.cONSIGNEEField = value;
            }
        }
        
        /// <remarks/>
        public string IAM {
            get {
                return this.iAMField;
            }
            set {
                this.iAMField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ITEM")]
        public ITEMDS[] ITEM {
            get {
                return this.iTEMField;
            }
            set {
                this.iTEMField = value;
            }
        }
        
        /// <remarks/>
        public int ITEMCOUNT {
            get {
                return this.iTEMCOUNTField;
            }
            set {
                this.iTEMCOUNTField = value;
            }
        }
        
        /// <remarks/>
        public string PO {
            get {
                return this.poField;
            }
            set {
                this.poField = value;
            }
        }
        
        /// <remarks/>
        public string PPDCOL {
            get {
                return this.pPDCOLField;
            }
            set {
                this.pPDCOLField = value;
            }
        }
        
        /// <remarks/>
        public CUSTINFO SHIPPER {
            get {
                return this.sHIPPERField;
            }
            set {
                this.sHIPPERField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3221.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tquoteapi.wsbeans.iseries/xsd")]
    public partial class ACCESSORIALDS {
        
        private string cODEField;
        
        /// <remarks/>
        public string CODE {
            get {
                return this.cODEField;
            }
            set {
                this.cODEField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3221.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tquoteapi.wsbeans.iseries/xsd")]
    public partial class SERVICEINFO {
        
        private string dIRINDField;
        
        private string fRIField;
        
        private string mONField;
        
        private string pHONEField;
        
        private string sATField;
        
        private string sUNField;
        
        private string tERMINALField;
        
        private string tHUField;
        
        private string tUEField;
        
        private string wEDField;
        
        private string zIPField;
        
        /// <remarks/>
        public string DIRIND {
            get {
                return this.dIRINDField;
            }
            set {
                this.dIRINDField = value;
            }
        }
        
        /// <remarks/>
        public string FRI {
            get {
                return this.fRIField;
            }
            set {
                this.fRIField = value;
            }
        }
        
        /// <remarks/>
        public string MON {
            get {
                return this.mONField;
            }
            set {
                this.mONField = value;
            }
        }
        
        /// <remarks/>
        public string PHONE {
            get {
                return this.pHONEField;
            }
            set {
                this.pHONEField = value;
            }
        }
        
        /// <remarks/>
        public string SAT {
            get {
                return this.sATField;
            }
            set {
                this.sATField = value;
            }
        }
        
        /// <remarks/>
        public string SUN {
            get {
                return this.sUNField;
            }
            set {
                this.sUNField = value;
            }
        }
        
        /// <remarks/>
        public string TERMINAL {
            get {
                return this.tERMINALField;
            }
            set {
                this.tERMINALField = value;
            }
        }
        
        /// <remarks/>
        public string THU {
            get {
                return this.tHUField;
            }
            set {
                this.tHUField = value;
            }
        }
        
        /// <remarks/>
        public string TUE {
            get {
                return this.tUEField;
            }
            set {
                this.tUEField = value;
            }
        }
        
        /// <remarks/>
        public string WED {
            get {
                return this.wEDField;
            }
            set {
                this.wEDField = value;
            }
        }
        
        /// <remarks/>
        public string ZIP {
            get {
                return this.zIPField;
            }
            set {
                this.zIPField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3221.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tquoteapi.wsbeans.iseries/xsd")]
    public partial class SERVICEDS {
        
        private decimal dAYSField;
        
        private SERVICEINFO dESTINATIONField;
        
        private string eRRORCODEField;
        
        private string[] eSTDELIVERYField;
        
        private SERVICEINFO oRIGINField;
        
        private string[] sHIPDATEField;
        
        /// <remarks/>
        public decimal DAYS {
            get {
                return this.dAYSField;
            }
            set {
                this.dAYSField = value;
            }
        }
        
        /// <remarks/>
        public SERVICEINFO DESTINATION {
            get {
                return this.dESTINATIONField;
            }
            set {
                this.dESTINATIONField = value;
            }
        }
        
        /// <remarks/>
        public string ERRORCODE {
            get {
                return this.eRRORCODEField;
            }
            set {
                this.eRRORCODEField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ESTDELIVERY")]
        public string[] ESTDELIVERY {
            get {
                return this.eSTDELIVERYField;
            }
            set {
                this.eSTDELIVERYField = value;
            }
        }
        
        /// <remarks/>
        public SERVICEINFO ORIGIN {
            get {
                return this.oRIGINField;
            }
            set {
                this.oRIGINField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SHIPDATE")]
        public string[] SHIPDATE {
            get {
                return this.sHIPDATEField;
            }
            set {
                this.sHIPDATEField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3221.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tquoteapi.wsbeans.iseries/xsd")]
    public partial class RATINGDS {
        
        private decimal aMOUNTField;
        
        private string bOLField;
        
        private decimal cODAMTField;
        
        private string cODTERMSField;
        
        private string cODWHOField;
        
        private CUSTINFO cONSIGNEEField;
        
        private string dISCLAIMERField;
        
        private string eRRORCODEField;
        
        private decimal fUELField;
        
        private string poField;
        
        private string pPDCOLField;
        
        private decimal qUOTENUMBERField;
        
        private CUSTINFO sHIPPERField;
        
        private decimal tOTALACCESSORIALField;
        
        private decimal tOTALFREIGHTField;
        
        /// <remarks/>
        public decimal AMOUNT {
            get {
                return this.aMOUNTField;
            }
            set {
                this.aMOUNTField = value;
            }
        }
        
        /// <remarks/>
        public string BOL {
            get {
                return this.bOLField;
            }
            set {
                this.bOLField = value;
            }
        }
        
        /// <remarks/>
        public decimal CODAMT {
            get {
                return this.cODAMTField;
            }
            set {
                this.cODAMTField = value;
            }
        }
        
        /// <remarks/>
        public string CODTERMS {
            get {
                return this.cODTERMSField;
            }
            set {
                this.cODTERMSField = value;
            }
        }
        
        /// <remarks/>
        public string CODWHO {
            get {
                return this.cODWHOField;
            }
            set {
                this.cODWHOField = value;
            }
        }
        
        /// <remarks/>
        public CUSTINFO CONSIGNEE {
            get {
                return this.cONSIGNEEField;
            }
            set {
                this.cONSIGNEEField = value;
            }
        }
        
        /// <remarks/>
        public string DISCLAIMER {
            get {
                return this.dISCLAIMERField;
            }
            set {
                this.dISCLAIMERField = value;
            }
        }
        
        /// <remarks/>
        public string ERRORCODE {
            get {
                return this.eRRORCODEField;
            }
            set {
                this.eRRORCODEField = value;
            }
        }
        
        /// <remarks/>
        public decimal FUEL {
            get {
                return this.fUELField;
            }
            set {
                this.fUELField = value;
            }
        }
        
        /// <remarks/>
        public string PO {
            get {
                return this.poField;
            }
            set {
                this.poField = value;
            }
        }
        
        /// <remarks/>
        public string PPDCOL {
            get {
                return this.pPDCOLField;
            }
            set {
                this.pPDCOLField = value;
            }
        }
        
        /// <remarks/>
        public decimal QUOTENUMBER {
            get {
                return this.qUOTENUMBERField;
            }
            set {
                this.qUOTENUMBERField = value;
            }
        }
        
        /// <remarks/>
        public CUSTINFO SHIPPER {
            get {
                return this.sHIPPERField;
            }
            set {
                this.sHIPPERField = value;
            }
        }
        
        /// <remarks/>
        public decimal TOTALACCESSORIAL {
            get {
                return this.tOTALACCESSORIALField;
            }
            set {
                this.tOTALACCESSORIALField = value;
            }
        }
        
        /// <remarks/>
        public decimal TOTALFREIGHT {
            get {
                return this.tOTALFREIGHTField;
            }
            set {
                this.tOTALFREIGHTField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3221.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tquoteapi.wsbeans.iseries/xsd")]
    public partial class CUSTINFO {
        
        private string aDDRESS1Field;
        
        private string aDDRESS2Field;
        
        private string cITYField;
        
        private string nAMEField;
        
        private string sTATEField;
        
        private string zIPField;
        
        /// <remarks/>
        public string ADDRESS1 {
            get {
                return this.aDDRESS1Field;
            }
            set {
                this.aDDRESS1Field = value;
            }
        }
        
        /// <remarks/>
        public string ADDRESS2 {
            get {
                return this.aDDRESS2Field;
            }
            set {
                this.aDDRESS2Field = value;
            }
        }
        
        /// <remarks/>
        public string CITY {
            get {
                return this.cITYField;
            }
            set {
                this.cITYField = value;
            }
        }
        
        /// <remarks/>
        public string NAME {
            get {
                return this.nAMEField;
            }
            set {
                this.nAMEField = value;
            }
        }
        
        /// <remarks/>
        public string STATE {
            get {
                return this.sTATEField;
            }
            set {
                this.sTATEField = value;
            }
        }
        
        /// <remarks/>
        public string ZIP {
            get {
                return this.zIPField;
            }
            set {
                this.zIPField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3221.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tquoteapi.wsbeans.iseries/xsd")]
    public partial class RATEITEMDS {
        
        private string aCCCODEField;
        
        private decimal aMOUNTField;
        
        private string dESCRIPTIONField;
        
        private string hAZField;
        
        private decimal pALLETSField;
        
        private decimal pIECESField;
        
        private decimal rATEField;
        
        private decimal wEIGHTField;
        
        private decimal _CLASSField;
        
        /// <remarks/>
        public string ACCCODE {
            get {
                return this.aCCCODEField;
            }
            set {
                this.aCCCODEField = value;
            }
        }
        
        /// <remarks/>
        public decimal AMOUNT {
            get {
                return this.aMOUNTField;
            }
            set {
                this.aMOUNTField = value;
            }
        }
        
        /// <remarks/>
        public string DESCRIPTION {
            get {
                return this.dESCRIPTIONField;
            }
            set {
                this.dESCRIPTIONField = value;
            }
        }
        
        /// <remarks/>
        public string HAZ {
            get {
                return this.hAZField;
            }
            set {
                this.hAZField = value;
            }
        }
        
        /// <remarks/>
        public decimal PALLETS {
            get {
                return this.pALLETSField;
            }
            set {
                this.pALLETSField = value;
            }
        }
        
        /// <remarks/>
        public decimal PIECES {
            get {
                return this.pIECESField;
            }
            set {
                this.pIECESField = value;
            }
        }
        
        /// <remarks/>
        public decimal RATE {
            get {
                return this.rATEField;
            }
            set {
                this.rATEField = value;
            }
        }
        
        /// <remarks/>
        public decimal WEIGHT {
            get {
                return this.wEIGHTField;
            }
            set {
                this.wEIGHTField = value;
            }
        }
        
        /// <remarks/>
        public decimal _CLASS {
            get {
                return this._CLASSField;
            }
            set {
                this._CLASSField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3221.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tquoteapi.wsbeans.iseries/xsd")]
    public partial class GETQUOTEResult {
        
        private int rATECOUNTField;
        
        private RATEITEMDS[] rATEITEMField;
        
        private RATINGDS rATINGField;
        
        private SERVICEDS sERVICEField;
        
        /// <remarks/>
        public int RATECOUNT {
            get {
                return this.rATECOUNTField;
            }
            set {
                this.rATECOUNTField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("RATEITEM")]
        public RATEITEMDS[] RATEITEM {
            get {
                return this.rATEITEMField;
            }
            set {
                this.rATEITEMField = value;
            }
        }
        
        /// <remarks/>
        public RATINGDS RATING {
            get {
                return this.rATINGField;
            }
            set {
                this.rATINGField = value;
            }
        }
        
        /// <remarks/>
        public SERVICEDS SERVICE {
            get {
                return this.sERVICEField;
            }
            set {
                this.sERVICEField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3221.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tquoteapi.wsbeans.iseries/xsd")]
    public partial class SECURITYINFODS {
        
        private string pASSWORDField;
        
        private string uSERNAMEField;
        
        /// <remarks/>
        public string PASSWORD {
            get {
                return this.pASSWORDField;
            }
            set {
                this.pASSWORDField = value;
            }
        }
        
        /// <remarks/>
        public string USERNAME {
            get {
                return this.uSERNAMEField;
            }
            set {
                this.uSERNAMEField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3221.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tquoteapi.wsbeans.iseries/xsd")]
    public partial class ITEMDS {
        
        private string dESCRIPTIONField;
        
        private string hAZField;
        
        private decimal pALLETSField;
        
        private decimal pIECESField;
        
        private decimal wEIGHTField;
        
        private decimal _CLASSField;
        
        /// <remarks/>
        public string DESCRIPTION {
            get {
                return this.dESCRIPTIONField;
            }
            set {
                this.dESCRIPTIONField = value;
            }
        }
        
        /// <remarks/>
        public string HAZ {
            get {
                return this.hAZField;
            }
            set {
                this.hAZField = value;
            }
        }
        
        /// <remarks/>
        public decimal PALLETS {
            get {
                return this.pALLETSField;
            }
            set {
                this.pALLETSField = value;
            }
        }
        
        /// <remarks/>
        public decimal PIECES {
            get {
                return this.pIECESField;
            }
            set {
                this.pIECESField = value;
            }
        }
        
        /// <remarks/>
        public decimal WEIGHT {
            get {
                return this.wEIGHTField;
            }
            set {
                this.wEIGHTField = value;
            }
        }
        
        /// <remarks/>
        public decimal _CLASS {
            get {
                return this._CLASSField;
            }
            set {
                this._CLASSField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    public delegate void getquote_XMLCompletedEventHandler(object sender, getquote_XMLCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getquote_XMLCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getquote_XMLCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    public delegate void getquoteCompletedEventHandler(object sender, getquoteCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3062.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class getquoteCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal getquoteCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public GETQUOTEResult Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((GETQUOTEResult)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591