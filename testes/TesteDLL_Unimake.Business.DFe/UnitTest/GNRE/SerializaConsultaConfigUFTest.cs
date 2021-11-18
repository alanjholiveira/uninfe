using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Xml;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Utility;
using Unimake.Business.DFe.Xml.GNRE;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.GNRE
{
    [TestClass]
    public class SerializaConsultaConfigUFTest
    {
        #region Public Methods

        [TestMethod]
        public void SerializaConsultaConfigUF()
        {
            var xml = new TConsultaConfigUf
            {
                Ambiente = TipoAmbiente.Homologacao,
                UF = UFBrasil.PR,
                Receita = new Receita
                {
                    Courier = SimNaoLetra.Nao,
                    Value = 123456 
                }
            };

            var conteudoXml = xml.GerarXML();

            var ambiente = conteudoXml.GetElementsByTagName("ambiente")[0].InnerText;
            var uf = conteudoXml.GetElementsByTagName("uf")[0].InnerText;
            var receita = conteudoXml.GetElementsByTagName("receita")[0].InnerText;

            XmlDocument docResultado = new XmlDocument();
            docResultado.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\"?><TConsultaConfigUf xmlns=\"http://www.gnre.pe.gov.br\"><ambiente>2</ambiente><uf>PR</uf><receita courier=\"N\">123456</receita></TConsultaConfigUf>");

            Debug.Assert(ambiente.Equals("2"));
            Debug.Assert(uf.Equals("PR"));
            Debug.Assert(receita.Equals("123456"));
            Debug.Assert(conteudoXml.InnerText.Equals(docResultado.InnerText));
        }

        #endregion Public Methods
    }
}