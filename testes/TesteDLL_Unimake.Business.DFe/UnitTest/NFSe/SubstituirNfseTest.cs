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
        public void SubstituirNfse()
        {
            var path = @"D:\testenfe\CertificadoA1Projesom.pfx";
            var CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "Itajubá");

            var xml = @"C:\projetos\uninfe\exemplos\NFSe\SONNER\2.01\SubstituirNfseEnvio-ped-substnfse.xml";

            var conteudoXML = new XmlDocument();
            conteudoXML.Load(xml);

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.NFSe,
                CertificadoDigital = CertificadoSelecionado,
                Servico = Servico.NFSeSubstituirNfse,
                TipoAmbiente = TipoAmbiente.Homologacao,
                CodigoMunicipio = 3132404,
                SchemaVersao = "2.01"
            };

            var substituirNfse = new SubstituirNfse(conteudoXML, configuracao);
            substituirNfse.Executar();

            Debug.Assert(!string.IsNullOrWhiteSpace(substituirNfse.RetornoWSString));
        }

        #endregion Public Methods
    }
}