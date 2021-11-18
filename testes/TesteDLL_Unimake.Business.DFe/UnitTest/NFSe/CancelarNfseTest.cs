﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.NFSe;
using Unimake.Security.Platform;
using System.Xml;
using System.Text;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.NFSe
{
    [TestClass]
    public class CancelarNfseTest
    {
        #region Public Methods

        [TestMethod]
        public void Cancelar()
        {
            var path = @"D:\testenfe\Unimake PV.pfx";
            X509Certificate2 CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "12345678");

            string xml = @"C:\projetos\uninfe\exemplos\NFSe\AVMB_ASTEN\CancelarNfseEnvio-ped-cannfse.xml";

            var conteudoXML = new XmlDocument();
            conteudoXML.Load(xml);

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.NFSe,
                CertificadoDigital = CertificadoSelecionado,
                TipoAmbiente = TipoAmbiente.Producao,
                CodigoMunicipio = 4314407,
                Servico = Servico.NFSeCancelarNfse,
                SchemaVersao = "2.02"
            };

            var cancelarNfse = new CancelarNfse(conteudoXML, configuracao);
            cancelarNfse.Executar();

            Debug.Assert(!string.IsNullOrWhiteSpace(cancelarNfse.RetornoWSString));
        }

        #endregion Public Methods
    }
}