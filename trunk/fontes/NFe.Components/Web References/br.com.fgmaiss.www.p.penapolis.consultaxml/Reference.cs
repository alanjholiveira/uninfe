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

namespace NFe.Components.br.com.fgmaiss.www.p.penapolis.consultaxml {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.79.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="webserviceBinding", Namespace="http://www.fgmaiss.com.br/issqn/wservice/wsnfeconsultaxml.php")]
    [System.Xml.Serialization.SoapIncludeAttribute(typeof(ConsultaNfe))]
    public partial class webservice : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback ConsultaNfeOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public webservice() {
            this.Url = global::NFe.Components.Properties.Settings.Default.NFe_Components_br_com_fgmaiss_www_p_penapolis_consultaxml_webservice;
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
        public event ConsultaNfeCompletedEventHandler ConsultaNfeCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://www.fgmaiss.com.br/issqn/wservice/wsnfeconsultaxml.php/ConsultaNfe", RequestNamespace="", ResponseNamespace="")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public ConsultaNfe[] ConsultaNfe(string usuario, string pass, string prf, string usr, string ctr, string tipo) {
            object[] results = this.Invoke("ConsultaNfe", new object[] {
                        usuario,
                        pass,
                        prf,
                        usr,
                        ctr,
                        tipo});
            return ((ConsultaNfe[])(results[0]));
        }
        
        /// <remarks/>
        public void ConsultaNfeAsync(string usuario, string pass, string prf, string usr, string ctr, string tipo) {
            this.ConsultaNfeAsync(usuario, pass, prf, usr, ctr, tipo, null);
        }
        
        /// <remarks/>
        public void ConsultaNfeAsync(string usuario, string pass, string prf, string usr, string ctr, string tipo, object userState) {
            if ((this.ConsultaNfeOperationCompleted == null)) {
                this.ConsultaNfeOperationCompleted = new System.Threading.SendOrPostCallback(this.OnConsultaNfeOperationCompleted);
            }
            this.InvokeAsync("ConsultaNfe", new object[] {
                        usuario,
                        pass,
                        prf,
                        usr,
                        ctr,
                        tipo}, this.ConsultaNfeOperationCompleted, userState);
        }
        
        private void OnConsultaNfeOperationCompleted(object arg) {
            if ((this.ConsultaNfeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ConsultaNfeCompleted(this, new ConsultaNfeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.79.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.SoapTypeAttribute(Namespace="http://www.fgmaiss.com.br/issqn/wservice/wsnfeconsultaxml.php")]
    public partial class ConsultaNfe {
        
        private string okkField;
        
        private string nfecontroleField;
        
        private string emitentecnpjField;
        
        private string nfenumeroField;
        
        private string nfedataField;
        
        private string nfehoraField;
        
        private string nfeautenticacaoField;
        
        private string nfestatusField;
        
        private string destinatariocnpjField;
        
        private string destinatarioinscmunField;
        
        private string destinatarioinscestField;
        
        private string destinatariorsocialField;
        
        private string destinatarioenderecoField;
        
        private string destinatarionumeroField;
        
        private string destinatariocomplementoField;
        
        private string destinatariobairroField;
        
        private string destinatariocidadeField;
        
        private string destinatarioestadoField;
        
        private string destinatariocepField;
        
        private string nfenumfatura1Field;
        
        private string nfedatfatura1Field;
        
        private string nfevalfatura1Field;
        
        private string nfenumfatura2Field;
        
        private string nfedatfatura2Field;
        
        private string nfevalfatura2Field;
        
        private string nfenumfatura3Field;
        
        private string nfedatfatura3Field;
        
        private string nfevalfatura3Field;
        
        private string nfenumfatura4Field;
        
        private string nfedatfatura4Field;
        
        private string nfevalfatura4Field;
        
        private string nfenumfatura5Field;
        
        private string nfedatfatura5Field;
        
        private string nfevalfatura5Field;
        
        private string nfenumfatura6Field;
        
        private string nfedatfatura6Field;
        
        private string nfevalfatura6Field;
        
        private string nfeitemserv1Field;
        
        private string nfealiqserv1Field;
        
        private string nfevalserv1Field;
        
        private string nfeitemserv2Field;
        
        private string nfealiqserv2Field;
        
        private string nfevalserv2Field;
        
        private string nfeitemserv3Field;
        
        private string nfealiqserv3Field;
        
        private string nfevalserv3Field;
        
        private string nfeitemserv4Field;
        
        private string nfealiqserv4Field;
        
        private string nfevalserv4Field;
        
        private string nfeitemserv5Field;
        
        private string nfealiqserv5Field;
        
        private string nfevalserv5Field;
        
        private string nfeitemserv6Field;
        
        private string nfealiqserv6Field;
        
        private string nfevalserv6Field;
        
        private string nfeitemserv7Field;
        
        private string nfealiqserv7Field;
        
        private string nfevalserv7Field;
        
        private string nfeitemserv8Field;
        
        private string nfealiqserv8Field;
        
        private string nfevalserv8Field;
        
        private string nfevalorField;
        
        private string nfevaltributavelField;
        
        private string nfevalissField;
        
        private string nfevalissretidoField;
        
        private string nfevaldescincondicionalField;
        
        private string nfevaldescoutrosField;
        
        private string nfebasecalcpisField;
        
        private string nfealiqpisField;
        
        private string nfevalpisField;
        
        private string nfebasecalccofinsField;
        
        private string nfealiqcofinsField;
        
        private string nfevalcofinsField;
        
        private string nfebasecalccsllField;
        
        private string nfealiqcsllField;
        
        private string nfevalcsllField;
        
        private string nfebasecalcinssField;
        
        private string nfealiqinssField;
        
        private string nfevalinssField;
        
        private string nfebasecalcirrfField;
        
        private string nfealiqirrfField;
        
        private string nfevalirrfField;
        
        private string nfedescricaoservicosField;
        
        private string nfeobservacoesField;
        
        private string nfenumerorpsField;
        
        private string nfelinkField;
        
        /// <remarks/>
        public string okk {
            get {
                return this.okkField;
            }
            set {
                this.okkField = value;
            }
        }
        
        /// <remarks/>
        public string nfecontrole {
            get {
                return this.nfecontroleField;
            }
            set {
                this.nfecontroleField = value;
            }
        }
        
        /// <remarks/>
        public string emitentecnpj {
            get {
                return this.emitentecnpjField;
            }
            set {
                this.emitentecnpjField = value;
            }
        }
        
        /// <remarks/>
        public string nfenumero {
            get {
                return this.nfenumeroField;
            }
            set {
                this.nfenumeroField = value;
            }
        }
        
        /// <remarks/>
        public string nfedata {
            get {
                return this.nfedataField;
            }
            set {
                this.nfedataField = value;
            }
        }
        
        /// <remarks/>
        public string nfehora {
            get {
                return this.nfehoraField;
            }
            set {
                this.nfehoraField = value;
            }
        }
        
        /// <remarks/>
        public string nfeautenticacao {
            get {
                return this.nfeautenticacaoField;
            }
            set {
                this.nfeautenticacaoField = value;
            }
        }
        
        /// <remarks/>
        public string nfestatus {
            get {
                return this.nfestatusField;
            }
            set {
                this.nfestatusField = value;
            }
        }
        
        /// <remarks/>
        public string destinatariocnpj {
            get {
                return this.destinatariocnpjField;
            }
            set {
                this.destinatariocnpjField = value;
            }
        }
        
        /// <remarks/>
        public string destinatarioinscmun {
            get {
                return this.destinatarioinscmunField;
            }
            set {
                this.destinatarioinscmunField = value;
            }
        }
        
        /// <remarks/>
        public string destinatarioinscest {
            get {
                return this.destinatarioinscestField;
            }
            set {
                this.destinatarioinscestField = value;
            }
        }
        
        /// <remarks/>
        public string destinatariorsocial {
            get {
                return this.destinatariorsocialField;
            }
            set {
                this.destinatariorsocialField = value;
            }
        }
        
        /// <remarks/>
        public string destinatarioendereco {
            get {
                return this.destinatarioenderecoField;
            }
            set {
                this.destinatarioenderecoField = value;
            }
        }
        
        /// <remarks/>
        public string destinatarionumero {
            get {
                return this.destinatarionumeroField;
            }
            set {
                this.destinatarionumeroField = value;
            }
        }
        
        /// <remarks/>
        public string destinatariocomplemento {
            get {
                return this.destinatariocomplementoField;
            }
            set {
                this.destinatariocomplementoField = value;
            }
        }
        
        /// <remarks/>
        public string destinatariobairro {
            get {
                return this.destinatariobairroField;
            }
            set {
                this.destinatariobairroField = value;
            }
        }
        
        /// <remarks/>
        public string destinatariocidade {
            get {
                return this.destinatariocidadeField;
            }
            set {
                this.destinatariocidadeField = value;
            }
        }
        
        /// <remarks/>
        public string destinatarioestado {
            get {
                return this.destinatarioestadoField;
            }
            set {
                this.destinatarioestadoField = value;
            }
        }
        
        /// <remarks/>
        public string destinatariocep {
            get {
                return this.destinatariocepField;
            }
            set {
                this.destinatariocepField = value;
            }
        }
        
        /// <remarks/>
        public string nfenumfatura1 {
            get {
                return this.nfenumfatura1Field;
            }
            set {
                this.nfenumfatura1Field = value;
            }
        }
        
        /// <remarks/>
        public string nfedatfatura1 {
            get {
                return this.nfedatfatura1Field;
            }
            set {
                this.nfedatfatura1Field = value;
            }
        }
        
        /// <remarks/>
        public string nfevalfatura1 {
            get {
                return this.nfevalfatura1Field;
            }
            set {
                this.nfevalfatura1Field = value;
            }
        }
        
        /// <remarks/>
        public string nfenumfatura2 {
            get {
                return this.nfenumfatura2Field;
            }
            set {
                this.nfenumfatura2Field = value;
            }
        }
        
        /// <remarks/>
        public string nfedatfatura2 {
            get {
                return this.nfedatfatura2Field;
            }
            set {
                this.nfedatfatura2Field = value;
            }
        }
        
        /// <remarks/>
        public string nfevalfatura2 {
            get {
                return this.nfevalfatura2Field;
            }
            set {
                this.nfevalfatura2Field = value;
            }
        }
        
        /// <remarks/>
        public string nfenumfatura3 {
            get {
                return this.nfenumfatura3Field;
            }
            set {
                this.nfenumfatura3Field = value;
            }
        }
        
        /// <remarks/>
        public string nfedatfatura3 {
            get {
                return this.nfedatfatura3Field;
            }
            set {
                this.nfedatfatura3Field = value;
            }
        }
        
        /// <remarks/>
        public string nfevalfatura3 {
            get {
                return this.nfevalfatura3Field;
            }
            set {
                this.nfevalfatura3Field = value;
            }
        }
        
        /// <remarks/>
        public string nfenumfatura4 {
            get {
                return this.nfenumfatura4Field;
            }
            set {
                this.nfenumfatura4Field = value;
            }
        }
        
        /// <remarks/>
        public string nfedatfatura4 {
            get {
                return this.nfedatfatura4Field;
            }
            set {
                this.nfedatfatura4Field = value;
            }
        }
        
        /// <remarks/>
        public string nfevalfatura4 {
            get {
                return this.nfevalfatura4Field;
            }
            set {
                this.nfevalfatura4Field = value;
            }
        }
        
        /// <remarks/>
        public string nfenumfatura5 {
            get {
                return this.nfenumfatura5Field;
            }
            set {
                this.nfenumfatura5Field = value;
            }
        }
        
        /// <remarks/>
        public string nfedatfatura5 {
            get {
                return this.nfedatfatura5Field;
            }
            set {
                this.nfedatfatura5Field = value;
            }
        }
        
        /// <remarks/>
        public string nfevalfatura5 {
            get {
                return this.nfevalfatura5Field;
            }
            set {
                this.nfevalfatura5Field = value;
            }
        }
        
        /// <remarks/>
        public string nfenumfatura6 {
            get {
                return this.nfenumfatura6Field;
            }
            set {
                this.nfenumfatura6Field = value;
            }
        }
        
        /// <remarks/>
        public string nfedatfatura6 {
            get {
                return this.nfedatfatura6Field;
            }
            set {
                this.nfedatfatura6Field = value;
            }
        }
        
        /// <remarks/>
        public string nfevalfatura6 {
            get {
                return this.nfevalfatura6Field;
            }
            set {
                this.nfevalfatura6Field = value;
            }
        }
        
        /// <remarks/>
        public string nfeitemserv1 {
            get {
                return this.nfeitemserv1Field;
            }
            set {
                this.nfeitemserv1Field = value;
            }
        }
        
        /// <remarks/>
        public string nfealiqserv1 {
            get {
                return this.nfealiqserv1Field;
            }
            set {
                this.nfealiqserv1Field = value;
            }
        }
        
        /// <remarks/>
        public string nfevalserv1 {
            get {
                return this.nfevalserv1Field;
            }
            set {
                this.nfevalserv1Field = value;
            }
        }
        
        /// <remarks/>
        public string nfeitemserv2 {
            get {
                return this.nfeitemserv2Field;
            }
            set {
                this.nfeitemserv2Field = value;
            }
        }
        
        /// <remarks/>
        public string nfealiqserv2 {
            get {
                return this.nfealiqserv2Field;
            }
            set {
                this.nfealiqserv2Field = value;
            }
        }
        
        /// <remarks/>
        public string nfevalserv2 {
            get {
                return this.nfevalserv2Field;
            }
            set {
                this.nfevalserv2Field = value;
            }
        }
        
        /// <remarks/>
        public string nfeitemserv3 {
            get {
                return this.nfeitemserv3Field;
            }
            set {
                this.nfeitemserv3Field = value;
            }
        }
        
        /// <remarks/>
        public string nfealiqserv3 {
            get {
                return this.nfealiqserv3Field;
            }
            set {
                this.nfealiqserv3Field = value;
            }
        }
        
        /// <remarks/>
        public string nfevalserv3 {
            get {
                return this.nfevalserv3Field;
            }
            set {
                this.nfevalserv3Field = value;
            }
        }
        
        /// <remarks/>
        public string nfeitemserv4 {
            get {
                return this.nfeitemserv4Field;
            }
            set {
                this.nfeitemserv4Field = value;
            }
        }
        
        /// <remarks/>
        public string nfealiqserv4 {
            get {
                return this.nfealiqserv4Field;
            }
            set {
                this.nfealiqserv4Field = value;
            }
        }
        
        /// <remarks/>
        public string nfevalserv4 {
            get {
                return this.nfevalserv4Field;
            }
            set {
                this.nfevalserv4Field = value;
            }
        }
        
        /// <remarks/>
        public string nfeitemserv5 {
            get {
                return this.nfeitemserv5Field;
            }
            set {
                this.nfeitemserv5Field = value;
            }
        }
        
        /// <remarks/>
        public string nfealiqserv5 {
            get {
                return this.nfealiqserv5Field;
            }
            set {
                this.nfealiqserv5Field = value;
            }
        }
        
        /// <remarks/>
        public string nfevalserv5 {
            get {
                return this.nfevalserv5Field;
            }
            set {
                this.nfevalserv5Field = value;
            }
        }
        
        /// <remarks/>
        public string nfeitemserv6 {
            get {
                return this.nfeitemserv6Field;
            }
            set {
                this.nfeitemserv6Field = value;
            }
        }
        
        /// <remarks/>
        public string nfealiqserv6 {
            get {
                return this.nfealiqserv6Field;
            }
            set {
                this.nfealiqserv6Field = value;
            }
        }
        
        /// <remarks/>
        public string nfevalserv6 {
            get {
                return this.nfevalserv6Field;
            }
            set {
                this.nfevalserv6Field = value;
            }
        }
        
        /// <remarks/>
        public string nfeitemserv7 {
            get {
                return this.nfeitemserv7Field;
            }
            set {
                this.nfeitemserv7Field = value;
            }
        }
        
        /// <remarks/>
        public string nfealiqserv7 {
            get {
                return this.nfealiqserv7Field;
            }
            set {
                this.nfealiqserv7Field = value;
            }
        }
        
        /// <remarks/>
        public string nfevalserv7 {
            get {
                return this.nfevalserv7Field;
            }
            set {
                this.nfevalserv7Field = value;
            }
        }
        
        /// <remarks/>
        public string nfeitemserv8 {
            get {
                return this.nfeitemserv8Field;
            }
            set {
                this.nfeitemserv8Field = value;
            }
        }
        
        /// <remarks/>
        public string nfealiqserv8 {
            get {
                return this.nfealiqserv8Field;
            }
            set {
                this.nfealiqserv8Field = value;
            }
        }
        
        /// <remarks/>
        public string nfevalserv8 {
            get {
                return this.nfevalserv8Field;
            }
            set {
                this.nfevalserv8Field = value;
            }
        }
        
        /// <remarks/>
        public string nfevalor {
            get {
                return this.nfevalorField;
            }
            set {
                this.nfevalorField = value;
            }
        }
        
        /// <remarks/>
        public string nfevaltributavel {
            get {
                return this.nfevaltributavelField;
            }
            set {
                this.nfevaltributavelField = value;
            }
        }
        
        /// <remarks/>
        public string nfevaliss {
            get {
                return this.nfevalissField;
            }
            set {
                this.nfevalissField = value;
            }
        }
        
        /// <remarks/>
        public string nfevalissretido {
            get {
                return this.nfevalissretidoField;
            }
            set {
                this.nfevalissretidoField = value;
            }
        }
        
        /// <remarks/>
        public string nfevaldescincondicional {
            get {
                return this.nfevaldescincondicionalField;
            }
            set {
                this.nfevaldescincondicionalField = value;
            }
        }
        
        /// <remarks/>
        public string nfevaldescoutros {
            get {
                return this.nfevaldescoutrosField;
            }
            set {
                this.nfevaldescoutrosField = value;
            }
        }
        
        /// <remarks/>
        public string nfebasecalcpis {
            get {
                return this.nfebasecalcpisField;
            }
            set {
                this.nfebasecalcpisField = value;
            }
        }
        
        /// <remarks/>
        public string nfealiqpis {
            get {
                return this.nfealiqpisField;
            }
            set {
                this.nfealiqpisField = value;
            }
        }
        
        /// <remarks/>
        public string nfevalpis {
            get {
                return this.nfevalpisField;
            }
            set {
                this.nfevalpisField = value;
            }
        }
        
        /// <remarks/>
        public string nfebasecalccofins {
            get {
                return this.nfebasecalccofinsField;
            }
            set {
                this.nfebasecalccofinsField = value;
            }
        }
        
        /// <remarks/>
        public string nfealiqcofins {
            get {
                return this.nfealiqcofinsField;
            }
            set {
                this.nfealiqcofinsField = value;
            }
        }
        
        /// <remarks/>
        public string nfevalcofins {
            get {
                return this.nfevalcofinsField;
            }
            set {
                this.nfevalcofinsField = value;
            }
        }
        
        /// <remarks/>
        public string nfebasecalccsll {
            get {
                return this.nfebasecalccsllField;
            }
            set {
                this.nfebasecalccsllField = value;
            }
        }
        
        /// <remarks/>
        public string nfealiqcsll {
            get {
                return this.nfealiqcsllField;
            }
            set {
                this.nfealiqcsllField = value;
            }
        }
        
        /// <remarks/>
        public string nfevalcsll {
            get {
                return this.nfevalcsllField;
            }
            set {
                this.nfevalcsllField = value;
            }
        }
        
        /// <remarks/>
        public string nfebasecalcinss {
            get {
                return this.nfebasecalcinssField;
            }
            set {
                this.nfebasecalcinssField = value;
            }
        }
        
        /// <remarks/>
        public string nfealiqinss {
            get {
                return this.nfealiqinssField;
            }
            set {
                this.nfealiqinssField = value;
            }
        }
        
        /// <remarks/>
        public string nfevalinss {
            get {
                return this.nfevalinssField;
            }
            set {
                this.nfevalinssField = value;
            }
        }
        
        /// <remarks/>
        public string nfebasecalcirrf {
            get {
                return this.nfebasecalcirrfField;
            }
            set {
                this.nfebasecalcirrfField = value;
            }
        }
        
        /// <remarks/>
        public string nfealiqirrf {
            get {
                return this.nfealiqirrfField;
            }
            set {
                this.nfealiqirrfField = value;
            }
        }
        
        /// <remarks/>
        public string nfevalirrf {
            get {
                return this.nfevalirrfField;
            }
            set {
                this.nfevalirrfField = value;
            }
        }
        
        /// <remarks/>
        public string nfedescricaoservicos {
            get {
                return this.nfedescricaoservicosField;
            }
            set {
                this.nfedescricaoservicosField = value;
            }
        }
        
        /// <remarks/>
        public string nfeobservacoes {
            get {
                return this.nfeobservacoesField;
            }
            set {
                this.nfeobservacoesField = value;
            }
        }
        
        /// <remarks/>
        public string nfenumerorps {
            get {
                return this.nfenumerorpsField;
            }
            set {
                this.nfenumerorpsField = value;
            }
        }
        
        /// <remarks/>
        public string nfelink {
            get {
                return this.nfelinkField;
            }
            set {
                this.nfelinkField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.79.0")]
    public delegate void ConsultaNfeCompletedEventHandler(object sender, ConsultaNfeCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.79.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ConsultaNfeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ConsultaNfeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public ConsultaNfe[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ConsultaNfe[])(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591