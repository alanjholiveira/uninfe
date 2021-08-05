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
    public class ConsultarLoteRpsTest
    {
        #region Public Methods

        [TestMethod]
        public void Consultar()
        {
            string path = @"D:\testenfe\Unimake PV.pfx";
            X509Certificate2 CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "12345678");

            string xml = @"D:\projetos\uninfe\testes\TesteDLL_Unimake.Business.DFe\UnitTest\NFSe\SIGCORP\Recursos\ConsultarNotaPrestador-ped-sitnfse.xml";

            var conteudoXML = new XmlDocument();
            conteudoXML.Load(xml);

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.NFSe,
                CertificadoDigital = CertificadoSelecionado,
                TipoAmbiente = TipoAmbiente.Producao,
                CodigoMunicipio = 4105508, 
                Servico = Servico.NFSeConsultarNotaPrestador,
                SchemaVersao = "0.00"
            };

            var consultarNotaPrestador = new ConsultarNotaPrestador(conteudoXML, configuracao);
            consultarNotaPrestador.Executar();

            Debug.Assert(!string.IsNullOrWhiteSpace(consultarNotaPrestador.RetornoWSString));
        }

        #endregion Public Methods
    }
}