using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
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
            var path = @"D:\testenfe\OestePharmaSenha-123456.pfx";
            var CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "123456");

            var xml = new TConsLoteGNRE
            {
                Ambiente = TipoAmbiente.Homologacao,
                NumeroRecibo = "2100261340",
                IncluirPDFGuias = SimNaoLetra.Sim,
                IncluirArquivoPagamento = SimNaoLetra.Nao
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

                if(consultaResultadoLote.Result.SituacaoProcess.Codigo == "402") //GNRE autorizada
                {
                    //Gravar XML de distruibuição que é o mesmo retornado
                    consultaResultadoLote.GravarXmlDistribuicao(@"d:\testenfe", xml.NumeroRecibo + "-procgnre.xml");
                }
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
