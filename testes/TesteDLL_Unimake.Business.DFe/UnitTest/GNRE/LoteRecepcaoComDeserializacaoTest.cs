using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.GNRE;
using Unimake.Business.DFe.Xml.GNRE;
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
            var path = @"D:\testenfe\certificadoGNRE2_12345678.pfx";
            var CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "12345678");
            List<XmlGNRE> xmlGNRE = new List<GNRE.XmlGNRE>();

            xmlGNRE.Add(new XmlGNRE
            {
                FileName = @"\projetos\uninfe\testes\TesteDLL_Unimake.Business.DFe\UnitTest\GNRE\Recursos\GNRE_Teste1_Bemsimples-gnre.xml",
                CodigoUF = 43 //Rio Grande do Sul
            });
            xmlGNRE.Add(new XmlGNRE
            {
                FileName = @"\projetos\uninfe\testes\TesteDLL_Unimake.Business.DFe\UnitTest\GNRE\Recursos\GNRE_Teste2_com_tag_referencia-gnre.xml",
                CodigoUF = 52 //Goiás
            });
            xmlGNRE.Add(new XmlGNRE
            {
                FileName = @"\projetos\uninfe\testes\TesteDLL_Unimake.Business.DFe\UnitTest\GNRE\Recursos\GNRE_Teste3_com_tag_produto-gnre.xml",
                CodigoUF = 21 //Maranhão
            });
            xmlGNRE.Add(new XmlGNRE
            {
                FileName = @"\projetos\uninfe\testes\TesteDLL_Unimake.Business.DFe\UnitTest\GNRE\Recursos\GNRE_Teste4_com_tag_Detalhamento-gnre.xml",
                CodigoUF = 51 //Mato Grosso
            });
            xmlGNRE.Add(new XmlGNRE
            {
                FileName = @"\projetos\uninfe\testes\TesteDLL_Unimake.Business.DFe\UnitTest\GNRE\Recursos\GNRE_Teste5_com_tag_CamposExtras-gnre.xml",
                CodigoUF = 17 //Tocantins
            });

            var doc = new XmlDocument();
            foreach(var item in xmlGNRE)
            {
                doc.LoadXml(System.IO.File.ReadAllText(item.FileName, Encoding.UTF8));

                var xml = Unimake.Business.DFe.Utility.XMLUtility.Deserializar<TLoteGNRE>(doc);

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.GNRE,
                    TipoEmissao = TipoEmissao.Normal,
                    CertificadoDigital = CertificadoSelecionado,
                    CodigoUF = item.CodigoUF
                };

                var loteRecepcao = new LoteRecepcao(xml, configuracao);

                loteRecepcao.Executar();

                Debug.Assert(loteRecepcao.ConteudoXMLOriginal.OuterXml.Equals(doc.OuterXml));
                Debug.Assert(loteRecepcao.Result != null);
                Debug.Assert(!string.IsNullOrWhiteSpace(loteRecepcao.Result.SituacaoRecepcao.Codigo));
            } 
        }

        #endregion Public Methods
    }

    public class XmlGNRE
    {
        public string FileName { get; set; }
        public int CodigoUF { get; set; }
    }
}