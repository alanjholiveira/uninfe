using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Xml;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.NFSe;
using Unimake.Security.Platform;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.NFSe
{
    [TestClass]
    public class RecepcionarLoteRpsTest
    {
        #region Public Methods        

        [TestMethod]
        public void RecepcionarLoteRps()
        {
            var path = @"D:\testenfe\Unimake PV.pfx";
            var CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "12345678");

            var xml = @"C:\projetos\uninfe\exemplos\NFSe\AVMB_ASTEN\EnviarLoteRpsEnvio-env-loterps.xml";

            var conteudoXML = new XmlDocument();
            conteudoXML.Load(xml);

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.NFSe,
                CertificadoDigital = CertificadoSelecionado,
                TipoAmbiente = TipoAmbiente.Homologacao,
                CodigoMunicipio = 4314407,
                Servico = Servico.NFSeRecepcionarLoteRps,
                SchemaVersao = "2.02"
            };

            var recepcionarLoteRps = new RecepcionarLoteRps(conteudoXML, configuracao);
            recepcionarLoteRps.Executar();

            Debug.Assert(!string.IsNullOrWhiteSpace(recepcionarLoteRps.RetornoWSString));
        }

        #endregion Public Methods
    }
}