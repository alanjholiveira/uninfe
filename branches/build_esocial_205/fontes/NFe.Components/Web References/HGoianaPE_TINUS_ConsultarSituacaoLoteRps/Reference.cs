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

namespace NFe.Components.HGoianaPE_TINUS_ConsultarSituacaoLoteRps
{
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "ConsultarSituacaoLoteRpsSoap", Namespace = "http://www2.tinus.com.br")]
    public partial class ConsultarSituacaoLoteRps : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback CallConsultarSituacaoLoteRpsOperationCompleted;

        private System.Threading.SendOrPostCallback TestOperationCompleted;

        private bool useDefaultCredentialsSetExplicitly;

        /// <remarks/>
        public ConsultarSituacaoLoteRps()
        {
            this.Url = global::NFe.Components.Properties.Settings.Default.NFe_Components_HGoianaPE_TINUS_ConsultarSituacaoLoteRps_ConsultarSituacaoLoteRps;
            if ((this.IsLocalFileSystemWebService(this.Url) == true))
            {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else
            {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        public new string Url
        {
            get
            {
                return base.Url;
            }
            set
            {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true)
                            && (this.useDefaultCredentialsSetExplicitly == false))
                            && (this.IsLocalFileSystemWebService(value) == false)))
                {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }

        public new bool UseDefaultCredentials
        {
            get
            {
                return base.UseDefaultCredentials;
            }
            set
            {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        /// <remarks/>
        public event CallConsultarSituacaoLoteRpsCompletedEventHandler CallConsultarSituacaoLoteRpsCompleted;

        /// <remarks/>
        public event TestCompletedEventHandler TestCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www2.tinus.com.br/WSNFSE.ConsultarSituacaoLoteRps.ConsultarSituacaoLoteRps" +
            "", RequestElementName = "ConsultarSituacaoLoteRps", RequestNamespace = "http://www2.tinus.com.br", ResponseElementName = "ConsultarSituacaoLoteRpsResponse", ResponseNamespace = "http://www2.tinus.com.br", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("ConsultarSituacaoLoteRpsResult")]
        public ConsultarSituacaoLoteRpsResposta CallConsultarSituacaoLoteRps(ConsultarSituacaoLoteRpsEnvio Arg)
        {
            object[] results = this.Invoke("CallConsultarSituacaoLoteRps", new object[] {
                        Arg});
            return ((ConsultarSituacaoLoteRpsResposta)(results[0]));
        }

        /// <remarks/>
        public void CallConsultarSituacaoLoteRpsAsync(ConsultarSituacaoLoteRpsEnvio Arg)
        {
            this.CallConsultarSituacaoLoteRpsAsync(Arg, null);
        }

        /// <remarks/>
        public void CallConsultarSituacaoLoteRpsAsync(ConsultarSituacaoLoteRpsEnvio Arg, object userState)
        {
            if ((this.CallConsultarSituacaoLoteRpsOperationCompleted == null))
            {
                this.CallConsultarSituacaoLoteRpsOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCallConsultarSituacaoLoteRpsOperationCompleted);
            }
            this.InvokeAsync("CallConsultarSituacaoLoteRps", new object[] {
                        Arg}, this.CallConsultarSituacaoLoteRpsOperationCompleted, userState);
        }

        private void OnCallConsultarSituacaoLoteRpsOperationCompleted(object arg)
        {
            if ((this.CallConsultarSituacaoLoteRpsCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CallConsultarSituacaoLoteRpsCompleted(this, new CallConsultarSituacaoLoteRpsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www2.tinus.com.br/WSNFSE.ConsultarSituacaoLoteRps.Test", RequestNamespace = "http://www2.tinus.com.br", ResponseNamespace = "http://www2.tinus.com.br", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string Test()
        {
            object[] results = this.Invoke("Test", new object[0]);
            return ((string)(results[0]));
        }

        /// <remarks/>
        public void TestAsync()
        {
            this.TestAsync(null);
        }

        /// <remarks/>
        public void TestAsync(object userState)
        {
            if ((this.TestOperationCompleted == null))
            {
                this.TestOperationCompleted = new System.Threading.SendOrPostCallback(this.OnTestOperationCompleted);
            }
            this.InvokeAsync("Test", new object[0], this.TestOperationCompleted, userState);
        }

        private void OnTestOperationCompleted(object arg)
        {
            if ((this.TestCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.TestCompleted(this, new TestCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }

        private bool IsLocalFileSystemWebService(string url)
        {
            if (((url == null)
                        || (url == string.Empty)))
            {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024)
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0)))
            {
                return true;
            }
            return false;
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www2.tinus.com.br")]
    public partial class ConsultarSituacaoLoteRpsEnvio
    {

        private tcIdentificacaoPrestador prestadorField;

        private string protocoloField;

        /// <remarks/>
        public tcIdentificacaoPrestador Prestador
        {
            get
            {
                return this.prestadorField;
            }
            set
            {
                this.prestadorField = value;
            }
        }

        /// <remarks/>
        public string Protocolo
        {
            get
            {
                return this.protocoloField;
            }
            set
            {
                this.protocoloField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www2.tinus.com.br")]
    public partial class tcIdentificacaoPrestador
    {

        private string cnpjField;

        private string inscricaoMunicipalField;

        /// <remarks/>
        public string Cnpj
        {
            get
            {
                return this.cnpjField;
            }
            set
            {
                this.cnpjField = value;
            }
        }

        /// <remarks/>
        public string InscricaoMunicipal
        {
            get
            {
                return this.inscricaoMunicipalField;
            }
            set
            {
                this.inscricaoMunicipalField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www2.tinus.com.br")]
    public partial class tcMensagemRetorno
    {

        private string codigoField;

        private string mensagemField;

        private string correcaoField;

        /// <remarks/>
        public string Codigo
        {
            get
            {
                return this.codigoField;
            }
            set
            {
                this.codigoField = value;
            }
        }

        /// <remarks/>
        public string Mensagem
        {
            get
            {
                return this.mensagemField;
            }
            set
            {
                this.mensagemField = value;
            }
        }

        /// <remarks/>
        public string Correcao
        {
            get
            {
                return this.correcaoField;
            }
            set
            {
                this.correcaoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3056.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www2.tinus.com.br")]
    public partial class ConsultarSituacaoLoteRpsResposta
    {

        private string numeroLoteField;

        private sbyte situacaoField;

        private bool situacaoFieldSpecified;

        private tcMensagemRetorno[] listaMensagemRetornoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "nonNegativeInteger")]
        public string NumeroLote
        {
            get
            {
                return this.numeroLoteField;
            }
            set
            {
                this.numeroLoteField = value;
            }
        }

        /// <remarks/>
        public sbyte Situacao
        {
            get
            {
                return this.situacaoField;
            }
            set
            {
                this.situacaoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool SituacaoSpecified
        {
            get
            {
                return this.situacaoFieldSpecified;
            }
            set
            {
                this.situacaoFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Namespace = "http://www2.tinus.com.br")]
        [System.Xml.Serialization.XmlArrayItemAttribute("MensagemRetorno")]
        public tcMensagemRetorno[] ListaMensagemRetorno
        {
            get
            {
                return this.listaMensagemRetornoField;
            }
            set
            {
                this.listaMensagemRetornoField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void CallConsultarSituacaoLoteRpsCompletedEventHandler(object sender, CallConsultarSituacaoLoteRpsCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CallConsultarSituacaoLoteRpsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal CallConsultarSituacaoLoteRpsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ConsultarSituacaoLoteRpsResposta Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((ConsultarSituacaoLoteRpsResposta)(this.results[0]));
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    public delegate void TestCompletedEventHandler(object sender, TestCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.7.3056.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class TestCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal TestCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public string Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591