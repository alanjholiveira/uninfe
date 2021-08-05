﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Xml;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.NFSe;
using Unimake.Security.Platform;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.NFSe
{
    [TestClass]
    public class ConsultarNfseFaixaTest
    {
        [TestMethod]
        public void Consultar()
        {
            var path = @"D:\testenfe\Unimake PV.pfx";
            var CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "12345678");

            var xml = @"C:\projetos\uninfe\exemplos\NFSe\BETHA\2.02\ConsultarNfseFaixaEnvio.xml";

            var conteudoXML = new XmlDocument();
            conteudoXML.Load(xml);

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.NFSe,
                CertificadoDigital = CertificadoSelecionado,
                TipoAmbiente = TipoAmbiente.Producao,
                CodigoMunicipio = 4118402,
                Servico = Servico.NFSeConsultarNfseFaixa,
                SchemaVersao = "2.02"
            };

            var consultarNfseFaixa = new ConsultarNfseFaixa(conteudoXML, configuracao);
            consultarNfseFaixa.Executar();

            Debug.Assert(!string.IsNullOrWhiteSpace(consultarNfseFaixa.RetornoWSString));
        }
    }
}