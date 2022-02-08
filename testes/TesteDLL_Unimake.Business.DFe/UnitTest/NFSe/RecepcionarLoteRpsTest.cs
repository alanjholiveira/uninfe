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
            var path = @"D:\testenfe\certificados\Volta_Redonda-senha123456.pfx";
            var CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "123456");

            var xml = @"C:\projetos\uninfe\exemplos\NFSe\SIMPLISS\2.03\EnviarLoteRpsEnvio-env-loterps.xml";

            var conteudoXML = new XmlDocument();
            conteudoXML.Load(xml);

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.NFSe,
                CertificadoDigital = CertificadoSelecionado,
                Servico = Servico.NFSeRecepcionarLoteRps,
                TipoAmbiente = TipoAmbiente.Homologacao,
                CodigoMunicipio = 3306305,
                SchemaVersao = "2.03"
            };

            var recepcionarLoteRps = new RecepcionarLoteRps(conteudoXML, configuracao);
            recepcionarLoteRps.Executar();

            Debug.Assert(!string.IsNullOrWhiteSpace(recepcionarLoteRps.RetornoWSString));
        }

        #endregion Public Methods
    }
}