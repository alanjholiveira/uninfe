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
    public class ConsultaNotaValidaTest
    {
        #region Public Methods

        [TestMethod]
        public void Consultar()
        {
            string path = @"D:\testenfe\Unimake PV.pfx";
            X509Certificate2 CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "12345678");

            string xml = @"C:\projetos\uninfe\exemplos\NFSe\SIGCORP-SIGISS\Cianorte - PR\ConsultarNotaValida-ped-loterps.xml";

            var conteudoXML = new XmlDocument();
            conteudoXML.Load(xml);

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.NFSe,
                CertificadoDigital = CertificadoSelecionado,
                TipoAmbiente = TipoAmbiente.Producao,
                CodigoMunicipio = 4105508, //3303203,
                Servico = Servico.NFSeConsultarNotaValida,
                SchemaVersao = "0.00"
            };

            var consultarNotaValida = new ConsultarNotaValida(conteudoXML, configuracao);
            consultarNotaValida.Executar();

            Debug.Assert(!string.IsNullOrWhiteSpace(consultarNotaValida.RetornoWSString));
        }

        #endregion Public Methods
    }
}