using System;
using NFe.Components.Abstract;
using NFe.Components.PItatingaSP;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Text;

namespace NFe.Components.GeisWeb.ItatingaSP.p
{
    public class GeisWebP : EmiteNFSeBase
    {
        private System.Web.Services.Protocols.SoapHttpClientProtocol Service;
        private X509Certificate2 Certificado;

        public override string NameSpaces
        {
            get
            {
                return "";
            }
        }

        #region construtores

        public GeisWebP(TipoAmbiente tpAmb, string pastaRetorno, string proxyuser, string proxypass, string proxyserver, X509Certificate2 certificado)
            : base(tpAmb, pastaRetorno)
        {
            ServicePointManager.Expect100Continue = false;
            Certificado = certificado;
        }

        #endregion construtores

        #region Métodos

        public override void EmiteNF(string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            GeisWebService Service = new GeisWebService();
            string strResult = Service.EnviaLoteRPS(doc.OuterXml);

            GerarRetorno(file,
                strResult,
                Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).EnvioXML,
                Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).RetornoXML);
        }

        public override void CancelarNfse(string file)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            GeisWebService Service = new GeisWebService();
            string strResult = Service.CancelaNota(doc.OuterXml);

            GerarRetorno(file,
                strResult,
                Propriedade.Extensao(Propriedade.TipoEnvio.PedCanNFSe).EnvioXML,
                Propriedade.Extensao(Propriedade.TipoEnvio.PedCanNFSe).RetornoXML);
        }

        public override void ConsultarLoteRps(string file)
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(file);

            GeisWebService Service = new GeisWebService();
            string strResult = Service.ConsultaNota(doc.OuterXml);

            GerarRetorno(file,
                strResult,
                Propriedade.Extensao(Propriedade.TipoEnvio.PedLoteRps).EnvioXML,
                Propriedade.Extensao(Propriedade.TipoEnvio.PedLoteRps).RetornoXML, Encoding.UTF8);
        }

        public override void ConsultarNfse(string file)
        {
            throw new System.NotImplementedException();
        }

        public override void ConsultarSituacaoLoteRps(string file)
        {
            throw new System.NotImplementedException();
        }

        public override void ConsultarNfsePorRps(string file)
        {
            throw new System.NotImplementedException();
        }

        #endregion Métodos
    }
}