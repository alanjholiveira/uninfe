using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Utility;
using Unimake.Business.DFe.Xml.CTe;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.CTe
{
    [TestClass]
    public class SerializaEventoComprovanteEntregaTest
    {
        #region Public Methods

        [TestMethod]
        public void SerializarEventoComprovanteEntrega()
        {
            //TODO WANDREY: Resolver esta encrenca
            //var dataHora = DateTime.Now;

            //var xml = new EventoCTe
            //{
            //    Versao = "3.00",
            //    InfEvento = new Unimake.Business.DFe.Xml.CTe.InfEvento(new Unimake.Business.DFe.Xml.CTe.DetEventoCompEntrega
            //    {
            //        VersaoEvento = "3.00",
            //        EventoCECTe = new EventoCECTe
            //        {
            //            NProt = "141200000007987",
            //            DhEntrega = dataHora,
            //            NDoc = "91886127085",
            //            XNome = "Teste",
            //            Latitude = "00",
            //            Longitude = "000",
            //            HashEntrega = "1234564321321321321231231321",
            //            DhHashEntrega = dataHora,
            //            InfEntrega = new List<InfEntrega>
            //            {
            //                new InfEntrega
            //                {
            //                    ChNFe = "12345678901234567890123456789012345678901234"
            //                }
            //            }
            //        }
            //    })
            //    {
            //        COrgao = UFBrasil.PR,
            //        ChCTe = "41200210859283000185570010000005671227070615",
            //        CNPJ = "10859283000185",
            //        DhEvento = dataHora,
            //        TpEvento = TipoEventoCTe.ComprovanteEntrega,
            //        NSeqEvento = 1,
            //        TpAmb = TipoAmbiente.Homologacao
            //    }
            //};

            //var ConteudoXml = XMLUtility.Serializar<EventoCTe>(xml);

            //if(ConteudoXml.GetElementsByTagName("dhEntrega") != null)
            //{
            //    var dhEntrega = ConteudoXml.GetElementsByTagName("dhEntrega")[0].InnerText;

            //    Debug.Assert(DateTime.TryParse(dhEntrega, out var dateValue));

            //    var dhEntrega2 = DateTime.Parse(dhEntrega);

            //    Debug.Assert(dhEntrega2.ToString().Equals(dataHora.ToString()));
            //}
        }

        #endregion Public Methods
    }
}