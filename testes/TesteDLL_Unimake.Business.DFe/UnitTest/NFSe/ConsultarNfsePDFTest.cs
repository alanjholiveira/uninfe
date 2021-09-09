using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Xml;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.NFSe;
using Unimake.Security.Platform;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.NFSe
{
    [TestClass]
    public class ConsultarNfsePDFTest
    {
        [TestMethod]
        public void Consultar()
        {
            try
            {
                var path = @"D:\testenfe\Unimake PV.pfx";
                var CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "12345678");

                var xml = @"D:\projetos\uninfe\exemplos\NFSe\PRODATA\ConsultarNotaPdfEnvio-ped-nfsepdf.xml";

                var conteudoXML = new XmlDocument();
                conteudoXML.Load(xml);

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.NFSe,
                    CertificadoDigital = CertificadoSelecionado,
                    TipoAmbiente = TipoAmbiente.Producao,
                    CodigoMunicipio = 3513504,
                    Servico = Servico.NFSeConsultarNfsePDF,
                    SchemaVersao = "2.01"
                };

                var consultarNfsePDF = new ConsultarNfsePDF(conteudoXML, configuracao);
                consultarNfsePDF.Executar();

                Debug.Assert(!string.IsNullOrWhiteSpace(consultarNfsePDF.RetornoWSString));

                try
                {
                    consultarNfsePDF.ExtrairPDF("d:\testenfe", "Nfse.pdf", "Base64Pdf");
                }
                catch
                {
                    Debug.Assert(false);
                }
            }
            catch
            {
                Debug.Assert(false);
            }
        }
    }
}