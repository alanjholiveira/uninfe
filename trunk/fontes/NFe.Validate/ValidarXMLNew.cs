using NFe.Components;
using NFe.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Unimake.Business.DFe;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Utility;
using ServicosCTe = Unimake.Business.DFe.Servicos.CTe;
using ServicosCTeOS = Unimake.Business.DFe.Servicos.CTeOS;
using ServicosMDFe = Unimake.Business.DFe.Servicos.MDFe;
using ServicosNFe = Unimake.Business.DFe.Servicos.NFe;
using XmlCTe = Unimake.Business.DFe.Xml.CTe;
using XmlCTeOS = Unimake.Business.DFe.Xml.CTeOS;
using XmlMDFe = Unimake.Business.DFe.Xml.MDFe;
using XmlNFe = Unimake.Business.DFe.Xml.NFe;

namespace NFe.Validate
{
    public class ValidarXMLNew
    {
        #region Private Fields

        public string TipoArquivoXML { get; set; }

        #endregion Private Fields

        #region Private Methods

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

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Validar XML
        /// </summary>
        /// <param name="arquivoXML">Caminho completo do arquivo XML a ser validado</param>
        /// <param name="retornoArquivo">Gerar arquivos na pasta de retorno com a resposta da validação? Se false, não vai gerar os retornos bem como não vai movimentar o arquivo validado para a subpasta validados</param>
        /// <returns>Retorna se efetuou a validação ou não (Se não detectar o tipo do arquivo ele não roda a validação)</returns>
        public bool Validar(string arquivoXML, bool retornoArquivo)
        {
            try
            {
                var emp = Empresas.FindEmpresaByThread();
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(arquivoXML);

                var xmlSalvar = new XmlDocument();

                var configuracao = new Configuracao
                {
                    TipoEmissao = Unimake.Business.DFe.Servicos.TipoEmissao.Normal,
                    CertificadoDigital = Empresas.Configuracoes[emp].X509Certificado
                };

                var validarSchema = new ValidarSchema();

                var tipoXML = XMLUtility.DetectXMLType(xmlDoc);

                bool jaValidou = false;

                switch (tipoXML)
                {
                    #region XML da NFe

                    case TipoXML.NFeStatusServico:
                        var xmlConsStatServNFe = new XmlNFe.ConsStatServ();
                        xmlConsStatServNFe = XMLUtility.Deserializar<XmlNFe.ConsStatServ>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.NFe;

                        var statusServico = new ServicosNFe.StatusServico(xmlConsStatServNFe, configuracao);
                        xmlSalvar = xmlDoc;
                        xmlDoc = statusServico.ConteudoXMLAssinado; // Depois de criar o xmlSalvar pq não posso apagar tags de uso específico do UNINFE que pode ter sido gerado pelo ERP.
                        break;

                    case TipoXML.NFeConsultaSituacao:
                        var xmlConsSitNFe = new XmlNFe.ConsSitNFe();
                        xmlConsSitNFe = XMLUtility.Deserializar<XmlNFe.ConsSitNFe>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.NFe;

                        var consultaProtocolo = new ServicosNFe.ConsultaProtocolo(xmlConsSitNFe, configuracao);
                        xmlSalvar = xmlDoc;
                        xmlDoc = consultaProtocolo.ConteudoXMLAssinado; // Depois de criar o xmlSalvar pq não posso apagar tags de uso específico do UNINFE que pode ter sido gerado pelo ERP.
                        break;

                    case TipoXML.NFeConsultaRecibo:
                        var xmlConsReciNFe = new XmlNFe.ConsReciNFe();
                        xmlConsReciNFe = XMLUtility.Deserializar<XmlNFe.ConsReciNFe>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.NFe;

                        var retAutorizacaoNFe = new ServicosNFe.RetAutorizacao(xmlConsReciNFe, configuracao);
                        xmlSalvar = xmlDoc;
                        xmlDoc = retAutorizacaoNFe.ConteudoXMLAssinado; // Depois de criar o xmlSalvar pq não posso apagar tags de uso específico do UNINFE que pode ter sido gerado pelo ERP.
                        break;

                    case TipoXML.NFeConsultaCadastro:
                        var xmlConsCad = new XmlNFe.ConsCad();
                        xmlConsCad = XMLUtility.Deserializar<XmlNFe.ConsCad>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.NFe;

                        var consultaCadastroNFe = new ServicosNFe.ConsultaCadastro(xmlConsCad, configuracao);

                        xmlDoc = consultaCadastroNFe.ConteudoXMLAssinado;
                        xmlSalvar = xmlDoc;
                        break;

                    case TipoXML.NFeEnvioEvento:
                        var xmlEnvEventoNFe = new XmlNFe.EnvEvento();
                        xmlEnvEventoNFe = XMLUtility.Deserializar<XmlNFe.EnvEvento>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.NFe;

                        var recepcaoEventoNFe = new ServicosNFe.RecepcaoEvento(xmlEnvEventoNFe, configuracao);

                        var tpEvento = ((int)xmlEnvEventoNFe.Evento[0].InfEvento.TpEvento);
                        configuracao.SchemaArquivo = configuracao.SchemasEspecificos[tpEvento.ToString()].SchemaArquivo;

                        xmlDoc = recepcaoEventoNFe.ConteudoXMLAssinado;
                        xmlSalvar = xmlDoc;
                        break;

                    case TipoXML.NFeInutilizacao:
                        var xmlInutNFe = new XmlNFe.InutNFe();
                        xmlInutNFe = XMLUtility.Deserializar<XmlNFe.InutNFe>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.NFe;

                        var inutilizacaoNFe = new ServicosNFe.Inutilizacao(xmlInutNFe, configuracao);

                        xmlDoc = inutilizacaoNFe.ConteudoXMLAssinado;
                        xmlSalvar = xmlDoc;
                        break;

                    case TipoXML.NFeDistribuicaoDFe:
                        var xmlDistibDFeNFe = new XmlNFe.DistDFeInt();
                        xmlDistibDFeNFe = XMLUtility.Deserializar<XmlNFe.DistDFeInt>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.NFe;

                        var distribuicaoDFe = new ServicosNFe.DistribuicaoDFe(xmlDistibDFeNFe, configuracao);

                        xmlDoc = distribuicaoDFe.ConteudoXMLAssinado;
                        xmlSalvar = xmlDoc;
                        break;

                    case TipoXML.NFe:
                        var xmlNFe = new XmlNFe.EnviNFe
                        {
                            Versao = "4.00",
                            IdLote = "000000000000001",
                            NFe = new List<XmlNFe.NFe>
                            {
                                XMLUtility.Deserializar<XmlNFe.NFe>(xmlDoc)
                            }
                        };

                        if (xmlNFe.NFe[0].InfNFe[0].Ide.Mod == ModeloDFe.NFCe)
                        {
                            if (string.IsNullOrWhiteSpace(Empresas.Configuracoes[emp].IdentificadorCSC.Trim()) || string.IsNullOrWhiteSpace(Empresas.Configuracoes[emp].TokenCSC))
                            {
                                throw new Exception("Para autorizar NFC-e é obrigatório informar nas configurações do UniNFe os campos CSC e IDToken do CSC.");
                            }

                            configuracao.TipoDFe = TipoDFe.NFCe;
                            configuracao.CSC = Empresas.Configuracoes[emp].IdentificadorCSC;
                            configuracao.CSCIDToken = Convert.ToInt32(Empresas.Configuracoes[emp].TokenCSC);

                            var autorizacao = new Unimake.Business.DFe.Servicos.NFCe.Autorizacao(xmlNFe, configuracao);
                            xmlDoc = autorizacao.ConteudoXMLAssinado;
                        }
                        else
                        {
                            configuracao.TipoDFe = TipoDFe.NFe;

                            var autorizacao = new ServicosNFe.Autorizacao(xmlNFe, configuracao);
                            xmlDoc = autorizacao.ConteudoXMLAssinado;
                        }

                        xmlSalvar.LoadXml(xmlDoc.GetElementsByTagName("NFe")[0].OuterXml);
                        break;

                    case TipoXML.NFeEnvioEmLote:
                        var xmlEnviNFe = new XmlNFe.EnviNFe();
                        xmlEnviNFe = XMLUtility.Deserializar<XmlNFe.EnviNFe>(xmlDoc);

                        if (xmlEnviNFe.NFe[0].InfNFe[0].Ide.Mod == ModeloDFe.NFCe)
                        {
                            if (string.IsNullOrWhiteSpace(Empresas.Configuracoes[emp].IdentificadorCSC.Trim()) || string.IsNullOrWhiteSpace(Empresas.Configuracoes[emp].TokenCSC))
                            {
                                throw new Exception("Para autorizar NFC-e é obrigatório informar nas configurações do UniNFe os campos CSC e IDToken do CSC.");
                            }

                            configuracao.TipoDFe = TipoDFe.NFCe;
                            configuracao.CSC = Empresas.Configuracoes[emp].IdentificadorCSC;
                            configuracao.CSCIDToken = Convert.ToInt32(Empresas.Configuracoes[emp].TokenCSC);

                            var autorizacao = new Unimake.Business.DFe.Servicos.NFCe.Autorizacao(xmlEnviNFe, configuracao);
                            xmlDoc = autorizacao.ConteudoXMLAssinado;
                        }
                        else
                        {
                            configuracao.TipoDFe = TipoDFe.NFe;

                            var autorizacao = new ServicosNFe.Autorizacao(xmlEnviNFe, configuracao);
                            xmlDoc = autorizacao.ConteudoXMLAssinado;
                        }

                        xmlSalvar = xmlDoc;
                        break;

                    #endregion XML da NFe

                    #region XML do CTe

                    case TipoXML.CTeConsultaRecibo:
                        var xmlConsReciCTe = new XmlCTe.ConsReciCTe();
                        xmlConsReciCTe = XMLUtility.Deserializar<XmlCTe.ConsReciCTe>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.CTe;

                        var retAutorizacaoCTe = new ServicosCTe.RetAutorizacao(xmlConsReciCTe, configuracao);
                        xmlSalvar = xmlDoc;
                        xmlDoc = retAutorizacaoCTe.ConteudoXMLAssinado; // Depois de criar o xmlSalvar pq não posso apagar tags de uso específico do UNINFE que pode ter sido gerado pelo ERP.
                        break;

                    case TipoXML.CTeConsultaSituacao:
                        var xmlConsSitCTe = new XmlCTe.ConsSitCTe();
                        xmlConsSitCTe = XMLUtility.Deserializar<XmlCTe.ConsSitCTe>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.CTe;

                        var consultaProtocoloCTe = new ServicosCTe.ConsultaProtocolo(xmlConsSitCTe, configuracao);
                        xmlSalvar = xmlDoc;
                        xmlDoc = consultaProtocoloCTe.ConteudoXMLAssinado; // Depois de criar o xmlSalvar pq não posso apagar tags de uso específico do UNINFE que pode ter sido gerado pelo ERP.
                        break;

                    case TipoXML.CTeStatusServico:
                        var xmlConsStatServCTe = new XmlCTe.ConsStatServCte();
                        xmlConsStatServCTe = XMLUtility.Deserializar<XmlCTe.ConsStatServCte>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.CTe;

                        var statusServicoCTe = new ServicosCTe.StatusServico(xmlConsStatServCTe, configuracao);
                        xmlSalvar = xmlDoc;
                        xmlDoc = statusServicoCTe.ConteudoXMLAssinado; // Depois de criar o xmlSalvar pq não posso apagar tags de uso específico do UNINFE que pode ter sido gerado pelo ERP.
                        break;

                    case TipoXML.CTeEnvioEvento:
                        var xmlEventoCTe = new XmlCTe.EventoCTe();
                        xmlEventoCTe = XMLUtility.Deserializar<XmlCTe.EventoCTe>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.CTe;

                        var recepcaoEventoCTe = new ServicosCTe.RecepcaoEvento(xmlEventoCTe, configuracao);

                        var tpEventoCTe = ((int)xmlEventoCTe.InfEvento.TpEvento);
                        configuracao.SchemaArquivo = configuracao.SchemasEspecificos[tpEventoCTe.ToString()].SchemaArquivo;

                        xmlDoc = recepcaoEventoCTe.ConteudoXMLAssinado;
                        xmlSalvar = xmlDoc;
                        break;

                    case TipoXML.CTeInutilizacao:
                        var xmlInutCTe = new XmlCTe.InutCTe();
                        xmlInutCTe = XMLUtility.Deserializar<XmlCTe.InutCTe>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.CTe;

                        var inutilizacaoCTe = new ServicosCTe.Inutilizacao(xmlInutCTe, configuracao);

                        xmlDoc = inutilizacaoCTe.ConteudoXMLAssinado;
                        xmlSalvar = xmlDoc;
                        break;

                    case TipoXML.CTeDistribuicaoDFe:
                        var xmlDistibDFeCTe = new XmlCTe.DistDFeInt();
                        xmlDistibDFeCTe = XMLUtility.Deserializar<XmlCTe.DistDFeInt>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.CTe;

                        var distribuicaoCTe = new ServicosCTe.DistribuicaoDFe(xmlDistibDFeCTe, configuracao);

                        xmlDoc = distribuicaoCTe.ConteudoXMLAssinado;
                        xmlSalvar = xmlDoc;
                        break;

                    case TipoXML.CTe:
                        var xmlCTe = new XmlCTe.EnviCTe
                        {
                            Versao = "3.00",
                            IdLote = "000000000000001",
                            CTe = new List<XmlCTe.CTe>
                            {
                                XMLUtility.Deserializar<XmlCTe.CTe>(xmlDoc)
                            }
                        };

                        configuracao.TipoDFe = TipoDFe.CTe;

                        var autorizacaoCTe = new ServicosCTe.Autorizacao(xmlCTe, configuracao);
                        xmlDoc = autorizacaoCTe.ConteudoXMLAssinado;
                        xmlSalvar.LoadXml(xmlDoc.GetElementsByTagName("CTe")[0].OuterXml);

                        XmlValidarCTe(tipoXML, configuracao, validarSchema, retornoArquivo, xmlSalvar, arquivoXML, xmlDoc, emp);
                        jaValidou = true;
                        break;

                    case TipoXML.CTeOS:
                        var xmlCTeOS = new XmlCTeOS.CTeOS();
                        xmlCTeOS = XMLUtility.Deserializar<XmlCTeOS.CTeOS>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.CTeOS;

                        var autorizacaoCTeOS = new ServicosCTeOS.Autorizacao(xmlCTeOS, configuracao);
                        xmlDoc = autorizacaoCTeOS.ConteudoXMLAssinado;
                        xmlSalvar = xmlDoc;
                        break;

                    case TipoXML.CTeEnvioEmLote:
                        var xmlEnviCTe = new XmlCTe.EnviCTe();
                        xmlEnviCTe = XMLUtility.Deserializar<XmlCTe.EnviCTe>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.CTe;

                        var autorizacaoEnviCTe = new ServicosCTe.Autorizacao(xmlEnviCTe, configuracao);
                        xmlDoc = autorizacaoEnviCTe.ConteudoXMLAssinado;
                        xmlSalvar = xmlDoc;

                        XmlValidarCTe(tipoXML, configuracao, validarSchema, retornoArquivo, xmlSalvar, arquivoXML, xmlDoc, emp);
                        jaValidou = true;
                        break;

                    #endregion XML do CTe

                    #region XML do MDFe

                    case TipoXML.MDFeEnvioEmLote:
                        var xmlEnviMDFe = new XmlMDFe.EnviMDFe();
                        xmlEnviMDFe = XMLUtility.Deserializar<XmlMDFe.EnviMDFe>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.MDFe;

                        var autorizacaoEnviMDFe = new ServicosMDFe.Autorizacao(xmlEnviMDFe, configuracao);
                        xmlDoc = autorizacaoEnviMDFe.ConteudoXMLAssinado;
                        xmlSalvar = xmlDoc;

                        XmlValidarMDFe(autorizacaoEnviMDFe.EnviMDFe, tipoXML, configuracao, validarSchema, retornoArquivo, xmlSalvar, arquivoXML, xmlDoc, emp);
                        jaValidou = true;
                        break;

                    case TipoXML.MDFeEnvioEvento:
                        var xmlEventoMDFe = new XmlMDFe.EventoMDFe();
                        xmlEventoMDFe = XMLUtility.Deserializar<XmlMDFe.EventoMDFe>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.MDFe;

                        var recepcaoEventoMDFe = new ServicosMDFe.RecepcaoEvento(xmlEventoMDFe, configuracao);

                        var tpEventoMDFe = ((int)xmlEventoMDFe.InfEvento.TpEvento);
                        configuracao.SchemaArquivo = configuracao.SchemasEspecificos[tpEventoMDFe.ToString()].SchemaArquivo;

                        xmlDoc = recepcaoEventoMDFe.ConteudoXMLAssinado;
                        xmlSalvar = xmlDoc;
                        break;

                    case TipoXML.MDFe:
                        var xmlMDFe = new XmlMDFe.EnviMDFe
                        {
                            Versao = "3.00",
                            IdLote = "000000000000001",
                            MDFe = XMLUtility.Deserializar<XmlMDFe.MDFe>(xmlDoc)
                        };

                        configuracao.TipoDFe = TipoDFe.MDFe;

                        var autorizacaoMDFe = new ServicosMDFe.Autorizacao(xmlMDFe, configuracao);
                        xmlDoc = autorizacaoMDFe.ConteudoXMLAssinado;
                        xmlSalvar.LoadXml(xmlDoc.GetElementsByTagName("MDFe")[0].OuterXml);

                        XmlValidarMDFe(autorizacaoMDFe.EnviMDFe, tipoXML, configuracao, validarSchema, retornoArquivo, xmlSalvar, arquivoXML, xmlDoc, emp);
                        jaValidou = true;
                        break;

                    case TipoXML.MDFeConsultaNaoEncerrado:
                        var xmlConsMDFeNaoEnc = new XmlMDFe.ConsMDFeNaoEnc();
                        xmlConsMDFeNaoEnc = XMLUtility.Deserializar<XmlMDFe.ConsMDFeNaoEnc>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.MDFe;

                        var consMDFeNaoEnc = new ServicosMDFe.ConsNaoEnc(xmlConsMDFeNaoEnc, configuracao);
                        xmlSalvar = xmlDoc;
                        xmlDoc = consMDFeNaoEnc.ConteudoXMLAssinado; // Depois de criar o xmlSalvar pq não posso apagar tags de uso específico do UNINFE que pode ter sido gerado pelo ERP.
                        break;

                    case TipoXML.MDFeConsultaRecibo:
                        var xmlConsReciMDFe = new XmlMDFe.ConsReciMDFe();
                        xmlConsReciMDFe = XMLUtility.Deserializar<XmlMDFe.ConsReciMDFe>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.MDFe;

                        var retAutorizacaoMDFe = new ServicosMDFe.RetAutorizacao(xmlConsReciMDFe, configuracao);
                        xmlSalvar = xmlDoc;
                        xmlDoc = retAutorizacaoMDFe.ConteudoXMLAssinado; // Depois de criar o xmlSalvar pq não posso apagar tags de uso específico do UNINFE que pode ter sido gerado pelo ERP.
                        break;

                    case TipoXML.MDFeConsultaSituacao:
                        var xmlConsSitMDFe = new XmlMDFe.ConsSitMDFe();
                        xmlConsSitMDFe = XMLUtility.Deserializar<XmlMDFe.ConsSitMDFe>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.MDFe;

                        var consultaProtocoloMDFe = new ServicosMDFe.ConsultaProtocolo(xmlConsSitMDFe, configuracao);
                        xmlSalvar = xmlDoc;
                        xmlDoc = consultaProtocoloMDFe.ConteudoXMLAssinado; // Depois de criar o xmlSalvar pq não posso apagar tags de uso específico do UNINFE que pode ter sido gerado pelo ERP.
                        break;

                    case TipoXML.MDFeStatusServico:
                        var xmlConsStatServMDFe = new XmlMDFe.ConsStatServMDFe();
                        xmlConsStatServMDFe = XMLUtility.Deserializar<XmlMDFe.ConsStatServMDFe>(xmlDoc);

                        configuracao.TipoDFe = TipoDFe.MDFe;

                        var statusServicoMDFe = new ServicosMDFe.StatusServico(xmlConsStatServMDFe, configuracao);
                        xmlSalvar = xmlDoc;
                        xmlDoc = statusServicoMDFe.ConteudoXMLAssinado; // Depois de criar o xmlSalvar pq não posso apagar tags de uso específico do UNINFE que pode ter sido gerado pelo ERP.
                        break;

                    #endregion XML do MDFe

                    default:
                        return false;
                }

                if (!jaValidou)
                {
                    Validar(tipoXML, configuracao, validarSchema, retornoArquivo, xmlSalvar, arquivoXML, xmlDoc, emp);
                }
            }
            catch (Exception ex)
            {
                var erro = "Ocorreu um erro ao validar o XML: " + ex.Message;
                if (retornoArquivo)
                {
                    GravarXMLRetornoValidacao(arquivoXML, "3", erro);
                    new Auxiliar().MoveArqErro(arquivoXML);
                }
                else
                {
                    throw new Exception(erro);
                }
            }

            return true;
        }

        private void Validar(TipoXML tipoXML, Configuracao configuracao, ValidarSchema validarSchema, bool retornoArquivo, XmlDocument xmlSalvar, string arquivoXML, XmlDocument xmlDoc, int emp)
        {
            var schema = configuracao.TipoDFe.ToString() + "." + configuracao.SchemaArquivo;
            switch (configuracao.TipoDFe)
            {
                case TipoDFe.NFCe:
                    schema = TipoDFe.NFe.ToString() + "." + configuracao.SchemaArquivo;
                    break;

                case TipoDFe.CTeOS:
                    schema = TipoDFe.CTe.ToString() + "." + configuracao.SchemaArquivo;
                    break;
            }

            TipoArquivoXML = EnumHelper.GetEnumItemDescription(tipoXML);

            validarSchema.Validar(xmlDoc, schema, configuracao.TargetNS);

            if (!validarSchema.Success)
            {
                var erro = "Ocorreu um erro ao validar o XML: " + validarSchema.ErrorMessage;
                if (retornoArquivo)
                {
                    GravarXMLRetornoValidacao(arquivoXML, "2", erro);
                    new Auxiliar().MoveArqErro(arquivoXML);
                }
                else
                {
                    throw new Exception(erro);
                }
            }
            else
            {
                if (retornoArquivo)
                {
                    if (!Directory.Exists(Empresas.Configuracoes[emp].PastaValidado))
                    {
                        Directory.CreateDirectory(Empresas.Configuracoes[emp].PastaValidado);
                    }

                    var arquivoNovo = Empresas.Configuracoes[emp].PastaValidado + "\\" + Path.GetFileName(arquivoXML);

                    //Gravar XML assinado e validado na subpasta "Validados"
                    var SW_2 = File.CreateText(arquivoNovo);
                    SW_2.Write(xmlSalvar.OuterXml);
                    SW_2.Close();

                    GravarXMLRetornoValidacao(arquivoXML, "1", "XML assinado e validado com sucesso.");
                }
            }
        }

        private void XmlValidarCTe(TipoXML tipoXML, Configuracao configuracao, ValidarSchema validarSchema, bool retornoArquivo, XmlDocument xmlSalvar, string arquivoXML, XmlDocument xmlDoc, int emp)
        {
            if (configuracao.SchemasEspecificos.Count > 0)
            {
                #region Validar o XML geral

                configuracao.SchemaArquivo = configuracao.SchemasEspecificos["1"].SchemaArquivo; //De qualquer modal o xml de validação da parte geral é o mesmo, então vou pegar do número 1, pq tanto faz.
                Validar(tipoXML, configuracao, validarSchema, retornoArquivo, xmlSalvar, arquivoXML, xmlDoc, emp);

                #endregion Validar o XML geral

                #region Validar a parte específica de modal do CTe

                foreach (XmlElement itemCTe in xmlDoc.GetElementsByTagName("CTe"))
                {
                    var modal = string.Empty;

                    foreach (XmlElement itemIde in itemCTe.GetElementsByTagName("ide"))
                    {
                        modal = itemIde.GetElementsByTagName("modal")[0].InnerText;
                    }

                    foreach (XmlElement itemInfModal in itemCTe.GetElementsByTagName("infModal"))
                    {
                        var xmlEspecifico = new XmlDocument();
                        xmlEspecifico.LoadXml(itemInfModal.InnerXml);

                        configuracao.SchemaArquivo = configuracao.SchemasEspecificos[modal.Substring(1, 1)].SchemaArquivoEspecifico;
                        Validar(tipoXML, configuracao, validarSchema, retornoArquivo, xmlSalvar, arquivoXML, xmlEspecifico, emp);
                    }
                }

                #endregion Validar a parte específica de cada evento
            }            
        }

        private void XmlValidarMDFe(XmlMDFe.EnviMDFe enviMDFe, TipoXML tipoXML, Configuracao configuracao, ValidarSchema validarSchema, bool retornoArquivo, XmlDocument xmlSalvar, string arquivoXML, XmlDocument xmlDoc, int emp)
        {
            var xml = enviMDFe;
            int modal = 0;

            if (configuracao.SchemasEspecificos.Count > 0)
            {
                modal = (int)xml.MDFe.InfMDFe.Ide.Modal;
            }

            #region Validar o XML geral

            configuracao.SchemaArquivo = configuracao.SchemasEspecificos[modal.ToString()].SchemaArquivo; //De qualquer modal o xml de validação da parte geral é o mesmo, então vou pegar do número 1, pq tanto faz.
            Validar(tipoXML, configuracao, validarSchema, retornoArquivo, xmlSalvar, arquivoXML, xmlDoc, emp);

            #endregion Validar o XML geral

            #region Validar a parte específica de modal do MDFe

            var xmlEspecifico = new XmlDocument();
            foreach (XmlElement item in xmlDoc.GetElementsByTagName("infModal"))
            {
                xmlEspecifico.LoadXml(item.InnerXml);
            }

            configuracao.SchemaArquivo = configuracao.SchemasEspecificos[modal.ToString()].SchemaArquivoEspecifico;
            Validar(tipoXML, configuracao, validarSchema, retornoArquivo, xmlSalvar, arquivoXML, xmlEspecifico, emp);

            #endregion Validar a parte específica de modal do MDFe
        }

        #endregion Public Methods
    }
}