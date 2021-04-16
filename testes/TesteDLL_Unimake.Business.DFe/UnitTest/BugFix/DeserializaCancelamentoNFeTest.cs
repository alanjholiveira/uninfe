using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using System.Xml;
using Unimake.Business.DFe.Utility;
using Unimake.Business.DFe.Xml.NFe;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.BugFix
{
    [TestClass]
    public class DeserializaCancelamentoNFeTest
    {
        #region Public Methods

        [TestMethod]
        public void DeserializarEvento()
        {
            ProcEventoNFe xmlProcEventoNFe;

            var doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(@"D:\Temp\41210106117473000150550010000661231006756360_110111_01-procEventoNFe.xml", Encoding.UTF8));

            xmlProcEventoNFe = XMLUtility.Deserializar<ProcEventoNFe>(doc);

            Assert.IsNotNull(xmlProcEventoNFe.RetEvento);
            Assert.IsNotNull(xmlProcEventoNFe.Evento.Signature);
        }

        #endregion Public Methods
    }
}