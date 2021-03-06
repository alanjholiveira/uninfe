//------------------------------------------------------------------------------
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

namespace NFe.Components.PWebfiscoTecnologia_Cancelar {
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
    [System.Web.Services.WebServiceBindingAttribute(Name="webserviceBinding", Namespace="http://187.32.116.170/issqn/wservice/wsnfecancela.php")]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(CancelaNfe))]
    public partial class webservice : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback CancelaNfeOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public webservice() {
            this.Url = global::NFe.Components.Properties.Settings.Default.NFe_Components_PBarraBonitaSP_Cancelar_webservice;
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
        public event CancelaNfeCompletedEventHandler CancelaNfeCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://www.webfiscotecnologia.com.br/issqn/wservice/wsnfecancela.php/CancelaNfe", RequestNamespace="", ResponseNamespace="")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public CancelaNfe[] CancelaNfe(string usuario, string pass, string prf, string usr, string ctr, string tipo, string obs) {
            object[] results = this.Invoke("CancelaNfe", new object[] {
                        usuario,
                        pass,
                        prf,
                        usr,
                        ctr,
                        tipo,
                        obs});
            return ((CancelaNfe[])(results[0]));
        }
        
        /// <remarks/>
        public void CancelaNfeAsync(string usuario, string pass, string prf, string usr, string ctr, string tipo, string obs) {
            this.CancelaNfeAsync(usuario, pass, prf, usr, ctr, tipo, obs, null);
        }
        
        /// <remarks/>
        public void CancelaNfeAsync(string usuario, string pass, string prf, string usr, string ctr, string tipo, string obs, object userState) {
            if ((this.CancelaNfeOperationCompleted == null)) {
                this.CancelaNfeOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCancelaNfeOperationCompleted);
            }
            this.InvokeAsync("CancelaNfe", new object[] {
                        usuario,
                        pass,
                        prf,
                        usr,
                        ctr,
                        tipo,
                        obs}, this.CancelaNfeOperationCompleted, userState);
        }
        
        private void OnCancelaNfeOperationCompleted(object arg) {
            if ((this.CancelaNfeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CancelaNfeCompleted(this, new CancelaNfeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="http://187.32.116.170/issqn/wservice/wsnfecancela.php")]
    public partial class CancelaNfe {
        
        private string okkField;
        
        /// <remarks/>
        public string okk {
            get {
                return this.okkField;
            }
            set {
                this.okkField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    public delegate void CancelaNfeCompletedEventHandler(object sender, CancelaNfeCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.8.4084.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CancelaNfeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CancelaNfeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public CancelaNfe[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((CancelaNfe[])(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591