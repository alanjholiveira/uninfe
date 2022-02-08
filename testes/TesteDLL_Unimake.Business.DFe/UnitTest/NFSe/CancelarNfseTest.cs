using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.NFSe;
using Unimake.Security.Platform;
using System.Xml;
using System.Text;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.NFSe
{
    [TestClass]
    public class CancelarNfseTest
    {
        #region Public Methods

        [TestMethod]
        public void Cancelar()
        {
            var path = @"D:\testenfe\certificados\Volta_Redonda-senha123456.pfx";
            var CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "123456");

            string xml = @"C:\projetos\uninfe\exemplos\NFSe\SONNER\2.01\CancelarNfseEnvio-ped-cannfse.xml";

            var conteudoXML = new XmlDocument();
            conteudoXML.Load(xml);

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.NFSe,
                CertificadoDigital = CertificadoSelecionado,
                Servico = Servico.NFSeCancelarNfse,
                TipoAmbiente = TipoAmbiente.Homologacao,
                CodigoMunicipio = 3306305,
                SchemaVersao = "2.03"
            };

            try
            {
                var cancelarNfse = new CancelarNfse(conteudoXML, configuracao);
                cancelarNfse.Executar();

                Debug.Assert(!string.IsNullOrWhiteSpace(cancelarNfse.RetornoWSString));
            }
            catch(Exception ex)
            {
                throw (ex);
            }
        }

        #endregion Public Methods
    }
}