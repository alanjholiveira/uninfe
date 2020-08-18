using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.GNRE;
using Unimake.Business.DFe.Utility;
using Unimake.Business.DFe.Xml.GNRE;
using Unimake.Security.Platform;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.GNRE
{
    [TestClass]
    public class ConsultaConfigUFTest
    {
        #region Public Methods

        [TestMethod]
        public void ConsultaConfigUF()
        {
            string path = @"D:\testenfe\certificadoGNRE2_12345678.pfx";
            X509Certificate2 CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "12345678");

            var xml = new TConsultaConfigUf
            {
                Ambiente = TipoAmbiente.Homologacao,
                UF = UFBrasil.PR,
                Receita = new Receita
                {
                    Courier = SimNaoLetra.Nao,
                    Value = 123456
                },
                Versao = "1.00"
            };

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.GNRE,
                TipoEmissao = TipoEmissao.Normal,
                CertificadoDigital = CertificadoSelecionado
            };

            var consultaConfigUF = new ConsultaConfigUF(xml, configuracao);
            //TODO: Wandrey - Propriedade Result da consultaConfigUF está com falha
            consultaConfigUF.Executar();

            //Debug.Assert(ambiente.Equals("2"));
        }

        #endregion Public Methods
    }
}