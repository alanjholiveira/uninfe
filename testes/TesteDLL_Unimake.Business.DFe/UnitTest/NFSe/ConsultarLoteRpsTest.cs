using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Xml;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.NFSe;
using Unimake.Security.Platform;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.NFSe
{
    [TestClass]
    public class ConsultarNotaPrestadorTest
    {
        [TestMethod]
        public void Consultar()
        {
            var path = @"D:\testenfe\Unimake PV.pfx";
            var CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "12345678");

            var xml = @"C:\projetos\uninfe\exemplos\NFSe\AVMB_ASTEN\ConsultarLoteRpsEnvio-ped-loterps.xml";

            var conteudoXML = new XmlDocument();
            conteudoXML.Load(xml);

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.NFSe,
                CertificadoDigital = CertificadoSelecionado,
                TipoAmbiente = TipoAmbiente.Producao,
                CodigoMunicipio = 4314407,
                Servico = Servico.NFSeConsultarLoteRps,
                SchemaVersao = "2.02"
            };

            var consultarLoteRps = new ConsultarLoteRps(conteudoXML, configuracao);
            consultarLoteRps.Executar();

            Debug.Assert(!string.IsNullOrWhiteSpace(consultarLoteRps.RetornoWSString));
        }
    }
}