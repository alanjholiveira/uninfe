using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class GeraNfseTest
    {
        #region Public Methods

        [TestMethod]
        public void GerarNota()
        {
            string path = @"D:\testenfe\Unimake PV.pfx";
            X509Certificate2 CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "12345678");

            string xml = @"C:\projetos\uninfe\exemplos\NFSe\AVMB_ASTEN\GerarNfseEnvio-env-loterps.xml";

            var conteudoXML = new XmlDocument();
            conteudoXML.Load(xml);

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.NFSe,
                CertificadoDigital = CertificadoSelecionado,
                TipoAmbiente = TipoAmbiente.Producao,
                CodigoMunicipio = 4314407,
                Servico = Servico.NFSeGerarNfse,
                SchemaVersao = "2.02"
            };

            var gerarNfse = new GerarNfse(conteudoXML, configuracao);
            gerarNfse.Executar();

            Debug.Assert(!string.IsNullOrWhiteSpace(gerarNfse.RetornoWSString));
        }

        #endregion Public Methods
    }
}