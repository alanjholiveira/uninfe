using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Xml;
using Unimake.Business.DFe.Xml.NFe;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.BugFix
{
    [TestClass]
    public class Bug135664
    {
        #region Public Methods

        [TestMethod]
        public void DeserializarEvento()
        {
            var doc = new XmlDocument();
            doc.LoadXml(System.IO.File.ReadAllText(@"D:\Temp\41210106117473000150550010000661231006756360_110111_01-procEventoNFe.xml", Encoding.UTF8));

            var xmlProcEventoNFe = Unimake.Business.DFe.Utility.XMLUtility.Deserializar<ProcEventoNFe>(doc);

            Assert.IsNotNull(xmlProcEventoNFe.RetEvento);
            Assert.IsNotNull(xmlProcEventoNFe.Evento.Signature);
        }

        #endregion Public Methods
    }
}