using NFe.Certificado;
using NFe.Components;
using NFe.Components.Coplan;
using NFe.Components.Pronin;
using NFe.Components.SystemPro;
using NFe.Settings;
using System;
using System.IO;
using System.ServiceModel;
using System.Xml;
using static NFe.Components.Security.SOAPSecurity;

namespace NFe.Service.NFSe
{
    public class TaskSubstituirNfse: TaskAbst
    {
        public TaskSubstituirNfse(string arquivo)
        {
            Servico = Servicos.NFSeSubstituirNfse;
            NomeArquivoXML = arquivo;
        }

        #region Objeto com os dados do XML da consulta nfse

        /// <summary>
        /// Esta herança que deve ser utilizada fora da classe para obter os valores das tag´s da consulta nfse
        /// </summary>
        private DadosPedSitNfse dadosXML;

        #endregion Objeto com os dados do XML da consulta nfse

        #region Execute

        public override void Execute()
        {
            var emp = Empresas.FindEmpresaByThread();

            try
            {
                Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlRetorno + "\\" +
                    Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.PedSubstNfse).EnvioXML) + Propriedade.Extensao(Propriedade.TipoEnvio.PedSubstNfse).RetornoERR);
                Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlErro + "\\" + NomeArquivoXML);

                dadosXML = new DadosPedSitNfse(emp);
                var padraoNFSe = Functions.PadraoNFSe(dadosXML.cMunicipio);

                switch(padraoNFSe)
                {
                    case PadroesNFSe.NOTAINTELIGENTE:
                    case PadroesNFSe.AVMB_ASTEN:
                    case PadroesNFSe.COPLAN:
                    case PadroesNFSe.SIMPLISS:
                    case PadroesNFSe.SONNER:
                    case PadroesNFSe.SMARAPD:
                        ExecuteDLL(emp, dadosXML.cMunicipio, padraoNFSe);
                        break;

                    default:
                        switch (dadosXML.cMunicipio)
                        {
                            case 3530805: //Mogi Mirim-SP
                            case 3131307: //Ipatinga-MG
                            case 2610004: //Palmares-PE
                            case 3552205: //Sorocaba-SP
                            case 4310009: //Ibirubá-RS
                            case 3168606: //Teófilo Otoni-MG
                            case 3115300: //Cataguases-MG
                            case 3147907: //Passos-MG
                            case 5107602: //Rondonópolis-MT
                            case 3147105: //Pará de Minas-MG
                            case 3303401: //Nova Friburgo-RJ
                                ExecuteDLL(emp, dadosXML.cMunicipio, padraoNFSe);
                                break;

                            default:

                                WebServiceProxy wsProxy = null;
                                object pedSubstNfse = null;

                                if (IsUtilizaCompilacaoWs(padraoNFSe))
                                {
                                    wsProxy = ConfiguracaoApp.DefinirWS(Servico, emp, dadosXML.cMunicipio, dadosXML.tpAmb, dadosXML.tpEmis, padraoNFSe, dadosXML.cMunicipio);
                                    pedSubstNfse = wsProxy.CriarObjeto(wsProxy.NomeClasseWS);
                                }
                                var cabecMsg = "";

                                var securityProtocolType = WebServiceProxy.DefinirProtocoloSeguranca(dadosXML.cMunicipio, dadosXML.tpAmb, dadosXML.tpEmis, padraoNFSe, Servico);

                                switch (padraoNFSe)
                                {
                                    case PadroesNFSe.EMBRAS:
                                        cabecMsg = "<cabecalho versao=\"2.02\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.02</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.E_RECEITA:
                                        cabecMsg = "<cabecalho xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\" versao=\"2.02\"><versaoDados>2.02</versaoDados></cabecalho>";
                                        break;
                                    case PadroesNFSe.ADM_SISTEMAS:
                                        cabecMsg = "<cabecalho versao=\"2.01\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.01</versaoDados></cabecalho>";
                                        wsProxy = new WebServiceProxy(Empresas.Configuracoes[emp].X509Certificado);

                                        pedSubstNfse = dadosXML.tpAmb == 1 ?
                                                        new Components.PAmargosaBA.InfseClient(GetBinding(), new EndpointAddress("https://demo.saatri.com.br/servicos/nfse.svc")) :
                                                        new Components.HAmargosaBA.InfseClient(GetBinding(), new EndpointAddress("https://homologa-demo.saatri.com.br/servicos/nfse.svc")) as object;

                                        SignUsingCredentials(emp, pedSubstNfse);
                                        break;

                                    case PadroesNFSe.INDAIATUBA_SP:
                                        cabecMsg = "<cabecalho versao=\"2.03\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.03</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.SIGCORP_SIGISS_203:
                                        cabecMsg = "<cabecalho versao=\"2.03\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.03</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.SMARAPD_204:
                                        cabecMsg = "<cabecalho versao=\"2.04\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.04</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.IIBRASIL:
                                        cabecMsg = "<cabecalho xmlns=\"http://www.abrasf.org.br/nfse.xsd\" versao=\"2.04\"><versaoDados>2.04</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.SYSTEMPRO:
                                        var syspro = new SystemPro((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                Empresas.Configuracoes[emp].PastaXmlRetorno, Empresas.Configuracoes[emp].X509Certificado, dadosXML.cMunicipio);
                                        var ad = new AssinaturaDigital();
                                        ad.Assinar(NomeArquivoXML, emp, dadosXML.cMunicipio);

                                        syspro.SubstituirNfse(NomeArquivoXML);
                                        break;

                                    case PadroesNFSe.PRONIN:
                                        if (dadosXML.cMunicipio == 4323002)
                                        {
                                            var pronin = new Pronin((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                dadosXML.cMunicipio,
                                                ConfiguracaoApp.ProxyUsuario,
                                                ConfiguracaoApp.ProxySenha,
                                                ConfiguracaoApp.ProxyServidor,
                                                Empresas.Configuracoes[emp].X509Certificado);

                                            var assPronin = new AssinaturaDigital();
                                            assPronin.Assinar(NomeArquivoXML, emp, dadosXML.cMunicipio);

                                            pronin.SubstituirNfse(NomeArquivoXML);
                                        }
                                        break;

                                    case PadroesNFSe.COPLAN:

                                        #region Coplan

                                        var coplan = new Coplan((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                            Empresas.Configuracoes[emp].PastaXmlRetorno,
                                             dadosXML.cMunicipio,
                                            ConfiguracaoApp.ProxyUsuario,
                                            ConfiguracaoApp.ProxySenha,
                                            ConfiguracaoApp.ProxyServidor,
                                            Empresas.Configuracoes[emp].X509Certificado);

                                        var assCoplan = new AssinaturaDigital();
                                        assCoplan.Assinar(NomeArquivoXML, emp, dadosXML.cMunicipio);

                                        coplan.SubstituirNfse(NomeArquivoXML);
                                        break;

                                        #endregion Coplan


                                }

                                if (IsInvocar(padraoNFSe, Servico, Empresas.Configuracoes[emp].UnidadeFederativaCodigo))
                                {

                                    //Assinar o XML
                                    var ad = new AssinaturaDigital();
                                    ad.Assinar(NomeArquivoXML, emp, dadosXML.cMunicipio);

                                    //Invocar o método que envia o XML para o SEFAZ
                                    oInvocarObj.InvocarNFSe(wsProxy, pedSubstNfse, NomeMetodoWS(Servico, dadosXML.cMunicipio), cabecMsg, this,
                                        Propriedade.Extensao(Propriedade.TipoEnvio.PedSubstNfse).EnvioXML,
                                        Propriedade.Extensao(Propriedade.TipoEnvio.PedSubstNfse).RetornoXML,
                                        padraoNFSe, Servico, securityProtocolType);

                                    ///
                                    /// grava o arquivo no FTP
                                    var filenameFTP = Path.Combine(Empresas.Configuracoes[emp].PastaXmlRetorno,
                                        Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.PedSubstNfse).EnvioXML) +
                                        Propriedade.Extensao(Propriedade.TipoEnvio.PedSubstNfse).RetornoXML);

                                    if (File.Exists(filenameFTP))
                                    {
                                        new GerarXML(emp).XmlParaFTP(emp, filenameFTP);
                                    }
                                }

                                break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //Gravar o arquivo de erro de retorno para o ERP, caso ocorra
                    TFunctions.GravarArqErroServico(NomeArquivoXML,
                        Propriedade.Extensao(Propriedade.TipoEnvio.PedSubstNfse).EnvioXML,
                        Propriedade.Extensao(Propriedade.TipoEnvio.PedSubstNfse).RetornoERR, ex);
                }
                catch
                {
                    //Se falhou algo na hora de gravar o retorno .ERR (de erro) para o ERP, infelizmente não posso fazer mais nada.
                    //Wandrey 31/08/2011
                }
            }
            finally
            {
                try
                {
                    Functions.DeletarArquivo(NomeArquivoXML);
                }
                catch
                {
                    //Se falhou algo na hora de deletar o XML de cancelamento de NFe, infelizmente
                    //não posso fazer mais nada, o UniNFe vai tentar mandar o arquivo novamente para o webservice, pois ainda não foi excluido.
                    //Wandrey 31/08/2011
                }
            }
        }

        #endregion Execute

        /// <summary>
        /// Executa o serviço utilizando a DLL do UniNFe.
        /// </summary>
        /// <param name="emp">Empresa que está enviando o XML</param>
        /// <param name="municipio">Código do município para onde será enviado o XML</param>
        /// <param name="padraoNFSe">Padrão do munípio para NFSe</param>
        private void ExecuteDLL(int emp, int municipio, PadroesNFSe padraoNFSe)
        {
            var conteudoXML = new XmlDocument();
            conteudoXML.Load(NomeArquivoXML);

            var finalArqEnvio = Propriedade.Extensao(Propriedade.TipoEnvio.PedSubstNfse).EnvioXML;
            var finalArqRetorno = Propriedade.Extensao(Propriedade.TipoEnvio.PedSubstNfse).RetornoXML;
            var versaoXML = DefinirVersaoXML(municipio, conteudoXML, padraoNFSe);

            Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlRetorno + "\\" + Functions.ExtrairNomeArq(NomeArquivoXML, finalArqEnvio) + Functions.ExtractExtension(finalArqRetorno) + ".err");

            var configuracao = new Unimake.Business.DFe.Servicos.Configuracao
            {
                TipoDFe = Unimake.Business.DFe.Servicos.TipoDFe.NFSe,
                CertificadoDigital = Empresas.Configuracoes[emp].X509Certificado,
                TipoAmbiente = (Unimake.Business.DFe.Servicos.TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                CodigoMunicipio = municipio,
                Servico = Unimake.Business.DFe.Servicos.Servico.NFSeSubstituirNfse,
                SchemaVersao = versaoXML,
                MunicipioToken = Empresas.Configuracoes[emp].SenhaWS
            };

            var substituirNfse = new Unimake.Business.DFe.Servicos.NFSe.SubstituirNfse(conteudoXML, configuracao);
            substituirNfse.Executar();

            vStrXmlRetorno = substituirNfse.RetornoWSString;

            XmlRetorno(finalArqEnvio, finalArqRetorno);

            /// grava o arquivo no FTP
            var filenameFTP = Path.Combine(Empresas.Configuracoes[emp].PastaXmlRetorno,
                Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.PedSubstNfse).EnvioXML) + Propriedade.Extensao(Propriedade.TipoEnvio.PedSubstNfse).RetornoXML);

            if (File.Exists(filenameFTP))
            {
                new GerarXML(emp).XmlParaFTP(emp, filenameFTP);
            }
        }

        /// <summary>
        /// Retorna a versão do XML que está sendo enviado para o município de acordo com o Padrão/Município
        /// </summary>
        /// <param name="codMunicipio">Código do município para onde será enviado o XML</param>
        /// <param name="xmlDoc">Conteúdo do XML da NFSe</param>
        /// <param name="padraoNFSe">Padrão do munípio para NFSe</param>
        /// <returns>Retorna a versão do XML que está sendo enviado para o município de acordo com o Padrão/Município</returns>
        private string DefinirVersaoXML(int codMunicipio, XmlDocument xmlDoc, PadroesNFSe padraoNFSe)
        {
            var versaoXML = "0.00";

            switch (padraoNFSe)
            {
                case PadroesNFSe.DIGIFRED:
                    versaoXML = "2.00";
                    break;

                case PadroesNFSe.SONNER:
                case PadroesNFSe.QUASAR:
                    versaoXML = "2.01";
                    break;

                case PadroesNFSe.NOTAINTELIGENTE:
                case PadroesNFSe.AVMB_ASTEN:
                case PadroesNFSe.VERSATEC:
                case PadroesNFSe.WEBISS:
                    versaoXML = "2.02";
                    break;

                case PadroesNFSe.SIGCORP_SIGISS:
                case PadroesNFSe.SIMPLISS:
                case PadroesNFSe.SMARAPD:
                case PadroesNFSe.DSF:
                case PadroesNFSe.COPLAN:
                    versaoXML = "2.03";
                    break;

                case PadroesNFSe.EL:
                case PadroesNFSe.TRIBUTUS:
                    versaoXML = "2.04";
                    break;
            }

            return versaoXML;
        }
    }
}