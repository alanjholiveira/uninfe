using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Xml;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.NFSe;
using Unimake.Security.Platform;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.NFSe
{
    [TestClass]
    public class RecepcionarLoteRpsSincronoTest
    {
        #region Public Methods

        [TestMethod]
        public void RecepcionarLoteRps()
        {
            var path = @"D:\testenfe\CertificadoA1Projesom.pfx";
            var CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "Itajubá");

            var xml = @"C:\projetos\uninfe\exemplos\NFSe\SONNER\2.01\EnviarLoteRpsSincronoEnvio-env-loterps.xml";

            var conteudoXML = new XmlDocument();
            conteudoXML.Load(xml);

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.NFSe,
                CertificadoDigital = CertificadoSelecionado,
                TipoAmbiente = TipoAmbiente.Homologacao,
                Servico = Servico.NFSeRecepcionarLoteRpsSincrono,
                CodigoMunicipio = 3132404,
                SchemaVersao = "2.01"
            };

            var recepcionarLoteRpsSincrono = new RecepcionarLoteRpsSincrono(conteudoXML, configuracao);
            recepcionarLoteRpsSincrono.Executar();

            Debug.Assert(!string.IsNullOrWhiteSpace(recepcionarLoteRpsSincrono.RetornoWSString));
        }

        #endregion Public Methods
    }
}