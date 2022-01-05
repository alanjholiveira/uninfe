using NFe.Components;
using NFe.Settings;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Unimake.Business.DFe;
using Unimake.Business.DFe.Servicos;

namespace NFe.Validate
{
    public class ValidarXMLNew
    {
        /// <summary>
        /// Tipos de XMLs
        /// </summary>
        private enum TipoXML
        {
            /// <summary>
            /// Consulta Status NFe
            /// </summary>
            consStatServ,

            /// <summary>
            /// Consulta Situação da NFe
            /// </summary>
            consSitNFe,

            /// <summary>
            /// Consulta Recibo da NFe
            /// </summary>
            consReciNFe,

            /// <summary>
            /// Consulta Cadastro de Contribuintes ICMS
            /// </summary>
            ConsCad,

            /// <summary>
            /// Pedido de Distribuição de DF-e de interesse do ator
            /// </summary>
            distDFeInt,

            /// <summary>
            /// Registro de evento
            /// </summary>
            envEvento,

            /// <summary>
            /// Solicitação de inutilização da NF-e
            /// </summary>
            inutNFe,

            /// <summary>
            /// Consulta Recibo do CT-e
            /// </summary>
            consReciCTe,

            /// <summary>
            /// Consulta Recibo do CT-e
            /// </summary>
            consSitCTe,

            /// <summary>
            /// Consulta situação do CT-e
            /// </summary>
            consStatServCte,

            /// <summary>
            /// Recepcao de eventos
            /// </summary>
            eventoCTe,

            /// <summary>
            /// Solicitação de inutilização do CT-e
            /// </summary>
            inutCTe,
        }

        /// <summary>
        /// Detectar o tipo do XML
        /// </summary>
        /// <param name="xmlDoc">Conteúdo do XML que será validado</param>
        /// <returns>Tipo do XML</returns>
        private TipoXML DetectXMLType(XmlDocument xmlDoc)
        {
            var tipoXML = (TipoXML)Enum.Parse(typeof(TipoXML), xmlDoc.DocumentElement.Name);

            return tipoXML;
        }

        /// <summary>
        /// Validar XML
        /// </summary>
        /// <param name="arquivoXML">Caminho completo do arquivo XML a ser validado</param>
        /// <returns>Retorna se efetuou a validação ou não (Se não detectar o tipo do arquivo ele não roda a validação)</returns>
        public bool Validar(string arquivoXML)
        {
            try
            {
                var emp = Empresas.FindEmpresaByThread();
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(arquivoXML);

                var validarSchema = new ValidarSchema();

                switch (DetectXMLType(xmlDoc))
                {
                    case TipoXML.consStatServ:
                        validarSchema.Validar(xmlDoc, TipoDFe.NFe.ToString() + "." + "consStatServ_v4.00.xsd", "http://www.portalfiscal.inf.br/nfe");
                        break;

                    case TipoXML.consSitNFe:
                        validarSchema.Validar(xmlDoc, TipoDFe.NFe.ToString() + "." + "consSitNFe_v4.00.xsd", "http://www.portalfiscal.inf.br/nfe");
                        break;

                    case TipoXML.consReciNFe:
                        validarSchema.Validar(xmlDoc, TipoDFe.NFe.ToString() + "." + "consReciNFe_v4.00.xsd", "http://www.portalfiscal.inf.br/nfe");
                        break;

                    case TipoXML.ConsCad:
                        validarSchema.Validar(xmlDoc, TipoDFe.NFe.ToString() + "." + "consCad_v2.00.xsd", "http://www.portalfiscal.inf.br/nfe");
                        break;

                    case TipoXML.envEvento:
                        validarSchema.Validar(xmlDoc, TipoDFe.NFe.ToString() + "." + "envEvento_v1.00.xsd", "http://www.portalfiscal.inf.br/nfe");
                        break;

                    case TipoXML.inutNFe:
                        validarSchema.Validar(xmlDoc, TipoDFe.NFe.ToString() + "." + "inutNFe_v4.00.xsd", "http://www.portalfiscal.inf.br/nfe");
                        break;

                    case TipoXML.consReciCTe:
                        validarSchema.Validar(xmlDoc, TipoDFe.CTe.ToString() + "." + "consReciCTe_v3.00.xsd", "http://www.portalfiscal.inf.br/cte");
                        break;

                    case TipoXML.consSitCTe:
                        validarSchema.Validar(xmlDoc, TipoDFe.CTe.ToString() + "." + "consSitCTe_v3.00.xsd", "http://www.portalfiscal.inf.br/cte");
                        break;

                    case TipoXML.consStatServCte:
                        validarSchema.Validar(xmlDoc, TipoDFe.CTe.ToString() + "." + "consStatServCTe_v3.00.xsd", "http://www.portalfiscal.inf.br/cte");
                        break;

                    case TipoXML.eventoCTe:
                        validarSchema.Validar(xmlDoc, TipoDFe.CTe.ToString() + "." + "eventoCTe_v3.00.xsd", "http://www.portalfiscal.inf.br/cte");
                        break;

                    case TipoXML.inutCTe:
                        validarSchema.Validar(xmlDoc, TipoDFe.CTe.ToString() + "." + "inutCte_v3.00.xsd", "http://www.portalfiscal.inf.br/cte");
                        break;

                    case TipoXML.distDFeInt:
                        if (xmlDoc.BaseURI.Contains("con-dist-dfecte"))
                        {
                            validarSchema.Validar(xmlDoc, TipoDFe.CTe.ToString() + "." + "distDFeInt_v1.00.xsd", "http://www.portalfiscal.inf.br/cte");
                        }
                        else if (xmlDoc.BaseURI.Contains("con-dist-dfe"))
                        {
                            validarSchema.Validar(xmlDoc, TipoDFe.NFe.ToString() + "." + "distDFeInt_v1.01.xsd", "http://www.portalfiscal.inf.br/nfe");
                        }
                        break;

                    default:
                        return false;
                }

                if (!validarSchema.Success)
                {
                    GravarXMLRetornoValidacao(arquivoXML, "2", "Ocorreu um erro ao validar o XML: " + validarSchema.ErrorMessage);
                    new Auxiliar().MoveArqErro(arquivoXML);
                }
                else
                {
                    if (!Directory.Exists(Empresas.Configuracoes[emp].PastaValidado))
                    {
                        Directory.CreateDirectory(Empresas.Configuracoes[emp].PastaValidado);
                    }

                    var arquivoNovo = Empresas.Configuracoes[emp].PastaValidado + "\\" + Path.GetFileName(arquivoXML);

                    Functions.Move(arquivoXML, arquivoNovo);
                    GravarXMLRetornoValidacao(arquivoXML, "1", "XML assinado e validado com sucesso.");
                }
            }
            catch (Exception ex) 
            {
                GravarXMLRetornoValidacao(arquivoXML, "3", "Ocorreu um erro ao validar o XML: " + ex.Message);
                new Auxiliar().MoveArqErro(arquivoXML);

            }
            return true;
        }

        /// <summary>
        /// Na tentativa de somente validar ou assinar o XML se encontrar um erro vai ser retornado um XML para o ERP
        /// </summary>
        /// <param name="arquivo">Nome do arquivo XML validado</param>
        /// <param name="PastaXMLRetorno">Pasta de retorno para ser gravado o XML</param>
        /// <param name="cStat">Status da validação</param>
        /// <param name="xMotivo">Status descritivo da validação</param>
        /// <by>Wandrey Mundin Ferreira</by>
        /// <date>28/05/2009</date>
        private void GravarXMLRetornoValidacao(string arquivo, string cStat, string xMotivo)
        {
            var emp = Empresas.FindEmpresaByThread();

            //Definir o nome do arquivo de retorno
            var arquivoRetorno = Functions.ExtrairNomeArq(arquivo, ".xml") + "-ret.xml";

            var xml = new XDocument(new XDeclaration("1.0", "utf-8", null),
                new XElement("Validacao",
                new XElement("cStat", cStat),
                new XElement("xMotivo", xMotivo)));
            xml.Save(Empresas.Configuracoes[emp].PastaXmlRetorno + "\\" + arquivoRetorno);
        }
    }
}