using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Xml;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.NFSe;
using Unimake.Security.Platform;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.NFSe
{
    [TestClass]
    public class ConsultarNfseServicoTomadoTest
    {
        [TestMethod]
        public void Consultar()
        {
            var path = @"D:\testenfe\CertificadoA1Projesom.pfx";
            var CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "Itajubá");

            string xml = @"C:\projetos\uninfe\exemplos\NFSe\SONNER\2.01\ConsultarNfseServicoTomadoEnvio-ped-sitnfsetom.xml";

            var conteudoXML = new XmlDocument();
            conteudoXML.Load(xml);

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.NFSe,
                CertificadoDigital = CertificadoSelecionado,
                Servico = Servico.NFSeConsultarNfseServicoPrestado,
                TipoAmbiente = TipoAmbiente.Homologacao,
                CodigoMunicipio = 3132404,
                SchemaVersao = "2.01"
            };

            var consultarNfseServicoTomado = new ConsultarNfseServicoTomado(conteudoXML, configuracao);
            consultarNfseServicoTomado.Executar();

            Debug.Assert(!string.IsNullOrWhiteSpace(consultarNfseServicoTomado.RetornoWSString));
        }
    }
}