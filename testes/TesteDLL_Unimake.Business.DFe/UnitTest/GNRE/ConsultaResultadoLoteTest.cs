using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.GNRE;
using Unimake.Business.DFe.Xml.GNRE;
using Unimake.Security.Platform;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.GNRE
{
    [TestClass]
    public class ConsultaResultadoLoteTest
    {
        #region Public Methods

        [TestMethod]
        public void ConsultaResultadoLote()
        {
            string path = @"D:\testenfe\OestePharma_1234.pfx";
            X509Certificate2 CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "1234");

            var xml = new TConsLoteGNRE
            {
                Ambiente = TipoAmbiente.Homologacao,
                NumeroRecibo = "2100254560",
                IncluirPDFGuias = SimNaoLetra.Sim
            };

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.GNRE,
                TipoEmissao = TipoEmissao.Normal,
                CertificadoDigital = CertificadoSelecionado,
                TipoAmbiente = TipoAmbiente.Homologacao,
                CodigoUF = 41 //Paraná
            };

            var consultaResultadoLote = new ConsultaResultadoLote(xml, configuracao);
            consultaResultadoLote.Executar();

            try
            {
                consultaResultadoLote.GravarXmlRetorno(@"d:\testenfe", xml.NumeroRecibo + "-ret-gnre.xml");
                consultaResultadoLote.GravarPDFGuia(@"d:\testenfe", "GuiaGNRE.pdf");
            }
            catch
            {
            }

            Debug.Assert(consultaResultadoLote.Result != null);
            Debug.Assert(!string.IsNullOrWhiteSpace(consultaResultadoLote.Result.SituacaoProcess.Codigo));
        }

        #endregion Public Methods
    }
}
