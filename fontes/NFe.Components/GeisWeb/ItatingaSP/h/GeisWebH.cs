using NFe.Components.Abstract;
using NFe.Components.HGeisWeb;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace NFe.Components.GeisWeb.ItatingaSP.h
{
    public class GeisWebH : EmiteNFSeBase
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

        public GeisWebH(TipoAmbiente tpAmb, string pastaRetorno, string proxyuser, string proxypass, string proxyserver, X509Certificate2 certificado)
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
                Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).RetornoXML, Encoding.UTF8);
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
                Propriedade.Extensao(Propriedade.TipoEnvio.PedCanNFSe).RetornoXML, Encoding.UTF8);
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