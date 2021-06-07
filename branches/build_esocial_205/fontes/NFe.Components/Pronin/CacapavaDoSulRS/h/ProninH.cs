﻿using NFe.Components.Abstract;
using NFe.Components.PCacapavaDoSulRS;
using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace NFe.Components.Pronin.CacapavaDoSulRS.h
{
    public class ProninH : EmiteNFSeBase
    {
        private BasicHttpBinding_INFSEGeracao ServiceGeracao = new BasicHttpBinding_INFSEGeracao();
        private BasicHttpBinding_INFSEConsultas ServiceConsultas = new BasicHttpBinding_INFSEConsultas();

        public override string NameSpaces
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #region construtores

        public ProninH(TipoAmbiente tpAmb, string pastaRetorno, string usuarioProxy, string senhaProxy, string domainProxy, X509Certificate certificado)
            : base(tpAmb, pastaRetorno)
        {
            ServicePointManager.ServerCertificateValidationCallback = MyCertHandler;
            ServiceGeracao.Proxy = WebRequest.DefaultWebProxy;
            ServiceGeracao.Proxy.Credentials = new NetworkCredential(usuarioProxy, senhaProxy);
            ServiceGeracao.Credentials = new NetworkCredential(senhaProxy, senhaProxy);
            ServiceGeracao.ClientCertificates.Add(certificado);

            ServiceConsultas.Proxy = WebRequest.DefaultWebProxy;
            ServiceConsultas.Proxy.Credentials = new NetworkCredential(usuarioProxy, senhaProxy);
            ServiceConsultas.Credentials = new NetworkCredential(senhaProxy, senhaProxy);
            ServiceConsultas.ClientCertificates.Add(certificado);

            Cabecalho cabecMsg = new Cabecalho();
            cabecMsg.versao = "2.03";
            cabecMsg.versaoDados = "2.03";

            ServiceGeracao.cabecalho = cabecMsg;
            ServiceConsultas.cabecalho = cabecMsg;
        }

        /// <summary>
        /// Identificamos falha no certificado o do servidor, então temos que ignorar os erros
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private bool MyCertHandler(object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        #endregion construtores

        #region Métodos

        public override void EmiteNF(string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            string result = "";

            switch (doc.DocumentElement.Name)
            {
                case "GerarNfseEnvio":
                    result = ServiceGeracao.GerarNfse(doc.InnerXml);
                    break;
                case "EnviarLoteRpsSincronoEnvio":
                    result = ServiceGeracao.EnviarLoteRpsSincrono(doc.InnerXml);
                    break;
                case "EnviarLoteRpsEnvio":
                    result = ServiceGeracao.RecepcionarLoteRps(doc.InnerXml);
                    break;
            }

            //string result = ServiceGeracao.RecepcionarLoteRps(doc.InnerXml);

            GerarRetorno(file, result, Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).EnvioXML,
                                        Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).RetornoXML);
        }

        public override void CancelarNfse(string file)
        {
            throw new Exceptions.ServicoInexistenteException();
        }

        public override void ConsultarLoteRps(string file)
        {
            throw new Exceptions.ServicoInexistenteException();
        }

        public override void ConsultarSituacaoLoteRps(string file)
        {
            throw new Exceptions.ServicoInexistenteException();
        }

        public override void ConsultarNfse(string file)
        {
            throw new Exceptions.ServicoInexistenteException();
        }

        public override void ConsultarNfsePorRps(string file)
        {
            throw new Exceptions.ServicoInexistenteException();
        }

        #endregion Métodos
    }
}