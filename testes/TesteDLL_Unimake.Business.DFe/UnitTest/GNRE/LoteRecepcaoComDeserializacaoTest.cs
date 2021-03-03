using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.GNRE;
using Unimake.Business.DFe.Xml.GNRE;
using Unimake.Business.DFe.Xml.NFe;
using Unimake.Security.Platform;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.GNRE
{
    [TestClass]
    public class LoteRecepcaoComDeserializacaoTest
    {
        #region Public Methods

        [TestMethod]
        public void LoteRecepcao()
        {
            string path = @"D:\testenfe\certificadoGNRE2_12345678.pfx";
            X509Certificate2 CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "12345678");

            var doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(@"D:\projetos\uninfe\testes\TesteDLL_Unimake.Business.DFe\UnitTest\GNRE\Recursos\GNRE_NFe6152-gnre.xml", Encoding.UTF8));
            var xml = Unimake.Business.DFe.Utility.XMLUtility.Deserializar<TLoteGNRE>(doc);

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.GNRE,
                TipoEmissao = TipoEmissao.Normal,
                CertificadoDigital = CertificadoSelecionado,
                CodigoUF = 43 //Rio Grande do Sul
            };

            var loteRecepcao = new LoteRecepcao(xml, configuracao);
            
            loteRecepcao.Executar();

            Debug.Assert(loteRecepcao.Result != null);
            Debug.Assert(!string.IsNullOrWhiteSpace(loteRecepcao.Result.SituacaoRecepcao.Codigo));
        }

        #endregion Public Methods
    }
}