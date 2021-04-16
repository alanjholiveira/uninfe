using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Unimake.Business.DFe.Utility;
using Unimake.Business.DFe.Xml.CTe;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.CTe
{
    [TestClass]
    public class DeserializaRetornoCancelamentoCTeTest
    {
        #region Public Methods

        [TestMethod]
        public void DeserializarRetornoCancelamentoCTeTest()
        {
            var xml = File.ReadAllText(@"D:\temp\retonroXmlCancelamentoCTe.xml");
            var eventoCTe = XMLUtility.Deserializar<EventoCTe>(xml);
            Assert.AreEqual("3.00", eventoCTe.InfEvento.DetEvento.VersaoEvento);
        }

        #endregion Public Methods
    }
}