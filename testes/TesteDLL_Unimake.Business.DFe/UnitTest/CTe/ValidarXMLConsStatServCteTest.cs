using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Unimake.Business.DFe;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Xml.CTe;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.CTe
{
    [TestClass]
    public class ValidarXMLConsStatServCteTest
    {
        #region Public Methods

        [TestMethod]
        public void ValidarXMLConsStatServCte()
        {
            var xml = new ConsStatServCte
            {
                Versao = "3.00",
                TpAmb = TipoAmbiente.Homologacao
            };

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.CTe,
                CodigoUF = (int)UFBrasil.PR
            };

            var statusServico = new Unimake.Business.DFe.Servicos.CTe.StatusServico(xml, configuracao);
            var validar = new ValidarSchema();
            validar.Validar(statusServico.ConteudoXMLAssinado, configuracao.TipoDFe.ToString() + "." + configuracao.SchemaArquivo, configuracao.TargetNS);

            Debug.Assert(validar.Success);
        }

        #endregion Public Methods
    }
}