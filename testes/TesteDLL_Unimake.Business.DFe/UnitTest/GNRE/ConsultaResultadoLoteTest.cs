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
            string path = @"D:\testenfe\certificadoGNRE2_12345678.pfx";
            X509Certificate2 CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "12345678");

            var xml = new TConsLoteGNRE
            {
                Ambiente = TipoAmbiente.Producao,
                NumeroRecibo = "1234567891",
            };

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.GNRE,
                TipoEmissao = TipoEmissao.Normal,
                CertificadoDigital = CertificadoSelecionado,
                CodigoUF = 41 //Paraná
            };

            var consultaResultadoLote = new ConsultaResultadoLote(xml, configuracao);
            consultaResultadoLote.Executar();

            Debug.Assert(consultaResultadoLote.Result != null);
            Debug.Assert(!string.IsNullOrWhiteSpace(consultaResultadoLote.Result.SituacaoProcess.Codigo));
        }

        #endregion Public Methods
    }
}
