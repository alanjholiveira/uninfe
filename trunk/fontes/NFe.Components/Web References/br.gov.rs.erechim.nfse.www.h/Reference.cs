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

namespace NFe.Components.br.gov.rs.erechim.nfse.www.h {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="NfseService_HomologPortBinding", Namespace="http://NFSe.wsservices.systempro.com.br/")]
    public partial class NfseService_Homolog : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback CancelarNfseOperationCompleted;
        
        private System.Threading.SendOrPostCallback SubstituirNfseOperationCompleted;
        
        private System.Threading.SendOrPostCallback ConsultarNfseFaixaOperationCompleted;
        
        private System.Threading.SendOrPostCallback EnviarLoteRpsSincronoOperationCompleted;
        
        private System.Threading.SendOrPostCallback GerarNfseOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public NfseService_Homolog() {
            this.Url = global::NFe.Components.Properties.Settings.Default.NFe_Components_br_gov_rs_erechim_nfse_www_h_NfseService_Homolog;
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
        public event CancelarNfseCompletedEventHandler CancelarNfseCompleted;
        
        /// <remarks/>
        public event SubstituirNfseCompletedEventHandler SubstituirNfseCompleted;
        
        /// <remarks/>
        public event ConsultarNfseFaixaCompletedEventHandler ConsultarNfseFaixaCompleted;
        
        /// <remarks/>
        public event EnviarLoteRpsSincronoCompletedEventHandler EnviarLoteRpsSincronoCompleted;
        
        /// <remarks/>
        public event GerarNfseCompletedEventHandler GerarNfseCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://NFSe.wsservices.systempro.com.br/", ResponseNamespace="http://NFSe.wsservices.systempro.com.br/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string CancelarNfse([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string nfseCabecMsg, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string nfseDadosMsg) {
            object[] results = this.Invoke("CancelarNfse", new object[] {
                        nfseCabecMsg,
                        nfseDadosMsg});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void CancelarNfseAsync(string nfseCabecMsg, string nfseDadosMsg) {
            this.CancelarNfseAsync(nfseCabecMsg, nfseDadosMsg, null);
        }
        
        /// <remarks/>
        public void CancelarNfseAsync(string nfseCabecMsg, string nfseDadosMsg, object userState) {
            if ((this.CancelarNfseOperationCompleted == null)) {
                this.CancelarNfseOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCancelarNfseOperationCompleted);
            }
            this.InvokeAsync("CancelarNfse", new object[] {
                        nfseCabecMsg,
                        nfseDadosMsg}, this.CancelarNfseOperationCompleted, userState);
        }
        
        private void OnCancelarNfseOperationCompleted(object arg) {
            if ((this.CancelarNfseCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CancelarNfseCompleted(this, new CancelarNfseCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://NFSe.wsservices.systempro.com.br/", ResponseNamespace="http://NFSe.wsservices.systempro.com.br/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SubstituirNfse([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string nfseCabecMsg, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string nfseDadosMsg) {
            object[] results = this.Invoke("SubstituirNfse", new object[] {
                        nfseCabecMsg,
                        nfseDadosMsg});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void SubstituirNfseAsync(string nfseCabecMsg, string nfseDadosMsg) {
            this.SubstituirNfseAsync(nfseCabecMsg, nfseDadosMsg, null);
        }
        
        /// <remarks/>
        public void SubstituirNfseAsync(string nfseCabecMsg, string nfseDadosMsg, object userState) {
            if ((this.SubstituirNfseOperationCompleted == null)) {
                this.SubstituirNfseOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSubstituirNfseOperationCompleted);
            }
            this.InvokeAsync("SubstituirNfse", new object[] {
                        nfseCabecMsg,
                        nfseDadosMsg}, this.SubstituirNfseOperationCompleted, userState);
        }
        
        private void OnSubstituirNfseOperationCompleted(object arg) {
            if ((this.SubstituirNfseCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SubstituirNfseCompleted(this, new SubstituirNfseCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://NFSe.wsservices.systempro.com.br/", ResponseNamespace="http://NFSe.wsservices.systempro.com.br/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ConsultarNfseFaixa([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string nfseCabecMsg, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string nfseDadosMsg) {
            object[] results = this.Invoke("ConsultarNfseFaixa", new object[] {
                        nfseCabecMsg,
                        nfseDadosMsg});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ConsultarNfseFaixaAsync(string nfseCabecMsg, string nfseDadosMsg) {
            this.ConsultarNfseFaixaAsync(nfseCabecMsg, nfseDadosMsg, null);
        }
        
        /// <remarks/>
        public void ConsultarNfseFaixaAsync(string nfseCabecMsg, string nfseDadosMsg, object userState) {
            if ((this.ConsultarNfseFaixaOperationCompleted == null)) {
                this.ConsultarNfseFaixaOperationCompleted = new System.Threading.SendOrPostCallback(this.OnConsultarNfseFaixaOperationCompleted);
            }
            this.InvokeAsync("ConsultarNfseFaixa", new object[] {
                        nfseCabecMsg,
                        nfseDadosMsg}, this.ConsultarNfseFaixaOperationCompleted, userState);
        }
        
        private void OnConsultarNfseFaixaOperationCompleted(object arg) {
            if ((this.ConsultarNfseFaixaCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ConsultarNfseFaixaCompleted(this, new ConsultarNfseFaixaCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://NFSe.wsservices.systempro.com.br/", ResponseNamespace="http://NFSe.wsservices.systempro.com.br/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string EnviarLoteRpsSincrono([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string nfseCabecMsg, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string nfseDadosMsg) {
            object[] results = this.Invoke("EnviarLoteRpsSincrono", new object[] {
                        nfseCabecMsg,
                        nfseDadosMsg});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void EnviarLoteRpsSincronoAsync(string nfseCabecMsg, string nfseDadosMsg) {
            this.EnviarLoteRpsSincronoAsync(nfseCabecMsg, nfseDadosMsg, null);
        }
        
        /// <remarks/>
        public void EnviarLoteRpsSincronoAsync(string nfseCabecMsg, string nfseDadosMsg, object userState) {
            if ((this.EnviarLoteRpsSincronoOperationCompleted == null)) {
                this.EnviarLoteRpsSincronoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnEnviarLoteRpsSincronoOperationCompleted);
            }
            this.InvokeAsync("EnviarLoteRpsSincrono", new object[] {
                        nfseCabecMsg,
                        nfseDadosMsg}, this.EnviarLoteRpsSincronoOperationCompleted, userState);
        }
        
        private void OnEnviarLoteRpsSincronoOperationCompleted(object arg) {
            if ((this.EnviarLoteRpsSincronoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.EnviarLoteRpsSincronoCompleted(this, new EnviarLoteRpsSincronoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("", RequestNamespace="http://NFSe.wsservices.systempro.com.br/", ResponseNamespace="http://NFSe.wsservices.systempro.com.br/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string GerarNfse([System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string nfseCabecMsg, [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)] string nfseDadosMsg) {
            object[] results = this.Invoke("GerarNfse", new object[] {
                        nfseCabecMsg,
                        nfseDadosMsg});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GerarNfseAsync(string nfseCabecMsg, string nfseDadosMsg) {
            this.GerarNfseAsync(nfseCabecMsg, nfseDadosMsg, null);
        }
        
        /// <remarks/>
        public void GerarNfseAsync(string nfseCabecMsg, string nfseDadosMsg, object userState) {
            if ((this.GerarNfseOperationCompleted == null)) {
                this.GerarNfseOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGerarNfseOperationCompleted);
            }
            this.InvokeAsync("GerarNfse", new object[] {
                        nfseCabecMsg,
                        nfseDadosMsg}, this.GerarNfseOperationCompleted, userState);
        }
        
        private void OnGerarNfseOperationCompleted(object arg) {
            if ((this.GerarNfseCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GerarNfseCompleted(this, new GerarNfseCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void CancelarNfseCompletedEventHandler(object sender, CancelarNfseCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CancelarNfseCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CancelarNfseCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void SubstituirNfseCompletedEventHandler(object sender, SubstituirNfseCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SubstituirNfseCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal SubstituirNfseCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void ConsultarNfseFaixaCompletedEventHandler(object sender, ConsultarNfseFaixaCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ConsultarNfseFaixaCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ConsultarNfseFaixaCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void EnviarLoteRpsSincronoCompletedEventHandler(object sender, EnviarLoteRpsSincronoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class EnviarLoteRpsSincronoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal EnviarLoteRpsSincronoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void GerarNfseCompletedEventHandler(object sender, GerarNfseCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GerarNfseCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GerarNfseCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
}

#pragma warning restore 1591