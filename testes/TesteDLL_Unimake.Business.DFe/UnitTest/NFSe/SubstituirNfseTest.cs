using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Xml;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.NFSe;
using Unimake.Security.Platform;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.NFSe
{
    [TestClass]
    public class SubstituirNfseTest
    {
        #region Public Methods

        [TestMethod]
        public void RecepcionarLoteRps()
        {
            var path = @"D:\testenfe\Unimake PV.pfx";
            var CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "12345678");

            var xml = @"C:\projetos\uninfe\exemplos\NFSe\AVMB_ASTEN\SubstituirNfseEnvio-ped-substnfse.xml";

            var conteudoXML = new XmlDocument();
            conteudoXML.Load(xml);

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.NFSe,
                CertificadoDigital = CertificadoSelecionado,
                TipoAmbiente = TipoAmbiente.Producao,
                CodigoMunicipio = 4314407,
                Servico = Servico.NFSeSubstituirNfse,
                SchemaVersao = "2.02"
            };

            var substituirNfse = new SubstituirNfse(conteudoXML, configuracao);
            substituirNfse.Executar();

            Debug.Assert(!string.IsNullOrWhiteSpace(substituirNfse.RetornoWSString));
        }

        #endregion Public Methods
    }
}