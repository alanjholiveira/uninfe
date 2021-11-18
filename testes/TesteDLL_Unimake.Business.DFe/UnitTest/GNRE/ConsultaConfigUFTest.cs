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
            var path = @"D:\testenfe\OestePharmaSenha-123456.pfx";
            var CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "123456");

            var xml = new TConsultaConfigUf
            {
                Ambiente = TipoAmbiente.Homologacao,
                UF = UFBrasil.RS,
                Receita = new Receita
                {
                    Courier = SimNaoLetra.Nao,
                    Value = 100064
                }
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

            Debug.Assert(consultaConfigUF.Result != null);
            Debug.Assert(!string.IsNullOrWhiteSpace(consultaConfigUF.Result.SituacaoConsulta.Codigo));
        }

        #endregion Public Methods
    }
}