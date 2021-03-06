using NFe.Certificado;
using NFe.Components;
using NFe.Components.Conam;
using NFe.Components.Consist;
using NFe.Components.Coplan;
using NFe.Components.EGoverne;
using NFe.Components.EGoverneISS;
using NFe.Components.EL;
using NFe.Components.Elotech;
using NFe.Components.Fiorilli;
using NFe.Components.GeisWeb;
using NFe.Components.GIAP;
using NFe.Components.SIGISSWEB;
using NFe.Components.GovDigital;
using NFe.Components.Memory;
using NFe.Components.Metropolis;
using NFe.Components.MGM;
using NFe.Components.Pronin;
using NFe.Components.RLZ_INFORMATICA;
using NFe.Components.SigCorp;
using NFe.Components.Simple;
using NFe.Components.SimplISS;
using NFe.Components.SystemPro;
using NFe.Components.Tinus;
using NFe.Components.WEBFISCO_TECNOLOGIA;
using NFe.Components.CENTI;
using NFe.Settings;
using NFe.Validate;
using NFSe.Components;
using System;
using System.IO;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Xml;
using static NFe.Components.Security.SOAPSecurity;

namespace NFe.Service.NFSe
{
    public class TaskNFSeRecepcionarLoteRps: TaskAbst
    {
        #region Objeto com os dados do XML de lote rps

        /// <summary>
        /// Esta herança que deve ser utilizada fora da classe para obter os valores das tag´s do lote rps
        /// </summary>
        private DadosEnvLoteRps oDadosEnvLoteRps;

        #endregion Objeto com os dados do XML de lote rps

        public TaskNFSeRecepcionarLoteRps(string arquivo)
        {
            Servico = Servicos.NFSeRecepcionarLoteRps;

            NomeArquivoXML = arquivo;
            ConteudoXML.PreserveWhitespace = false;
            ConteudoXML.Load(arquivo);
        }

        public override void Execute()
        {
            var emp = Empresas.FindEmpresaByThread();

            //Definir o serviço que será executado para a classe
            Servico = Servicos.NFSeRecepcionarLoteRps;

            try
            {
                Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlRetorno + "\\" +
                                         Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).EnvioXML) + Propriedade.ExtRetorno.RetEnvLoteRps_ERR);
                Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlErro + "\\" + NomeArquivoXML);

                oDadosEnvLoteRps = new DadosEnvLoteRps(emp);

                EnvLoteRps(emp, NomeArquivoXML);
                var padraoNFSe = Functions.PadraoNFSe(oDadosEnvLoteRps.cMunicipio);

                switch(padraoNFSe)
                {
                    case PadroesNFSe.NOTAINTELIGENTE:
                    case PadroesNFSe.PRODATA:
                    case PadroesNFSe.BETHA:
                    case PadroesNFSe.AVMB_ASTEN:
                        ExecuteDLL(emp, oDadosEnvLoteRps.cMunicipio, padraoNFSe);
                        break;

                    default:
                        switch(oDadosEnvLoteRps.cMunicipio)
                        {
                            case 4105508: //Cianorte-PR
                            case 3303203: //Nilópolis-RJ
                            case 3305109: //São João de Meriti-RJ
                            case 3505500: //Barretos-SP
                            case 2802908: //Itabaiana-SE
                            case 4217600: //Siderópolis-SC
                            case 3127701: //Governador Valadares-MG
                            case 5107909: //Sinop-MT
                            case 4209102: //Joinville-SC
                            case 3306305: //Volta Redonda - RJ
                            case 3530706: //Mogi Guaçu - SP
                            case 5105606: //Matupá-MT
                            case 3132404: //Itajubá-MG
                            case 2933307: //Vitória da Conquista-BA
                            case 3201209: //Cachoeiro de Itapemirim
                            case 3506003: //Bauru-SP
                            case 2925303: //Porto Seguro-BA
                            case 3530805: //Mogi Mirim-SP
                            case 3131307: //Ipatinga-MG
                            case 3106200: //Belo Horizonte-MG
                            case 2610004: //Palmares-PE
                            case 3550308: //São Paulo-SP
                            case 3552205: //Sorocaba-SP
                            case 4310009: //Ibirubá-RS
                            case 3168606: //Teófilo Otoni-MG
                            case 3523107: //Itaquaquecetuba-SP
                            case 3115300: //Cataguases-MG
                            case 3147907: //Passos-MG
                            case 5107602: //Rondonópolis-MT
                            case 3147105: //Pará de Minas-MG
                            case 3303401: //Nova Friburgo-RJ
                            case 3529005: //Marília-SP
                                ExecuteDLL(emp, oDadosEnvLoteRps.cMunicipio, padraoNFSe);
                                break;

                            default:

                                WebServiceProxy wsProxy = null;
                                object envLoteRps = null;

                                if(IsUtilizaCompilacaoWs(padraoNFSe, Servico, oDadosEnvLoteRps.cMunicipio))
                                {
                                    if(padraoNFSe == PadroesNFSe.ABACO_204)
                                    {
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.ABACO_204);
                                    }

                                    wsProxy = ConfiguracaoApp.DefinirWS(Servico, emp, oDadosEnvLoteRps.cMunicipio, oDadosEnvLoteRps.tpAmb, oDadosEnvLoteRps.tpEmis, padraoNFSe, oDadosEnvLoteRps.cMunicipio);
                                    if(wsProxy != null)
                                    {
                                        envLoteRps = wsProxy.CriarObjeto(wsProxy.NomeClasseWS);
                                    }
                                }

                                var securityProtocolType = WebServiceProxy.DefinirProtocoloSeguranca(oDadosEnvLoteRps.cMunicipio, oDadosEnvLoteRps.tpAmb, oDadosEnvLoteRps.tpEmis, padraoNFSe, Servico);

                                var cabecMsg = "";
                                switch(padraoNFSe)
                                {
                                    case PadroesNFSe.IPM:

                                        //código da cidade da receita federal, este arquivo pode ser encontrado em ~\uninfe\doc\Codigos_Cidades_Receita_Federal.xls</para>
                                        //O código da cidade está hardcoded pois ainda está sendo usado apenas para campo mourão
                                        var ipm = new IPM((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                          Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                          Empresas.Configuracoes[emp].UsuarioWS,
                                                          Empresas.Configuracoes[emp].SenhaWS,
                                                          oDadosEnvLoteRps.cMunicipio);

                                        if(ConfiguracaoApp.Proxy)
                                        {
                                            ipm.Proxy = Unimake.Net.Utility.GetProxy(ConfiguracaoApp.ProxyServidor, ConfiguracaoApp.ProxyUsuario, ConfiguracaoApp.ProxySenha, ConfiguracaoApp.ProxyPorta);
                                        }

                                        if(oDadosEnvLoteRps.cMunicipio == 4303103 || oDadosEnvLoteRps.cMunicipio == 4104808 || oDadosEnvLoteRps.cMunicipio == 4215000)
                                        {
                                            var adIPM = new AssinaturaDigital();
                                            //adIPM.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio);
                                            adIPM.Assinar(NomeArquivoXML, "nfse", "nfse", Empresas.Configuracoes[emp].X509Certificado, emp);
                                        }

                                        ipm.EmiteNF(NomeArquivoXML);
                                        break;

                                    case PadroesNFSe.GINFES:
                                        switch(oDadosEnvLoteRps.cMunicipio)
                                        {
                                            case 2304400: //Fortaleza - CE
                                                cabecMsg = "<ns2:cabecalho versao=\"3\" xmlns:ns2=\"http://www.ginfes.com.br/cabecalho_v03.xsd\"><versaoDados>3</versaoDados></ns2:cabecalho>";
                                                break;

                                            case 4125506: //São José dos Pinhais - PR
                                                cabecMsg = "<ns2:cabecalho versao=\"3\" xmlns:ns2=\"http://nfe.sjp.pr.gov.br/cabecalho_v03.xsd\"><versaoDados>3</versaoDados></ns2:cabecalho>";
                                                break;

                                            default:
                                                cabecMsg = "<ns2:cabecalho versao=\"3\" xmlns:ns2=\"http://www.ginfes.com.br/cabecalho_v03.xsd\"><versaoDados>3</versaoDados></ns2:cabecalho>";
                                                break;
                                        }
                                        break;

                                    case PadroesNFSe.ABASE:
                                        cabecMsg = "<cabecalho xmlns=\"http://nfse.abase.com.br/nfse.xsd\" versao =\"1.00\"><versaoDados>1.00</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.ABACO:
                                    case PadroesNFSe.CANOAS_RS:
                                        cabecMsg = "<cabecalho versao=\"201001\"><versaoDados>V2010</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.ABACO_204:
                                        cabecMsg = "<cabecalho xmlns=\"http://www.abrasf.org.br/nfse.xsd\" versao=\"201001\"><versaoDados>2.04</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.BHISS:
                                        cabecMsg = "<cabecalho xmlns=\"http://www.abrasf.org.br/nfse.xsd\" versao=\"1.00\"><versaoDados >1.00</versaoDados ></cabecalho>";
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.BHISS);
                                        break;

                                    case PadroesNFSe.SH3:
                                        cabecMsg = "<cabecalho xmlns=\"http://www.abrasf.org.br/nfse.xsd\" versao=\"1.00\"><versaoDados >1.00</versaoDados ></cabecalho>";
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.SH3);
                                        break;

                                    case PadroesNFSe.WEBISS:
                                        cabecMsg = "<cabecalho xmlns=\"http://www.abrasf.org.br/nfse.xsd\" versao=\"1.00\"><versaoDados >1.00</versaoDados ></cabecalho>";
                                        break;

                                    case PadroesNFSe.WEBISS_202:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.WEBISS_202);

                                        cabecMsg = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><cabecalho xmlns=\"http://www.abrasf.org.br/nfse.xsd\" versao=\"2.02\"><versaoDados>2.02</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.NA_INFORMATICA:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.VVISS);

                                        wsProxy = new WebServiceProxy(Empresas.Configuracoes[emp].X509Certificado);

                                        //if (oDadosEnvLoteRps.tpAmb == 1)
                                        //    envLoteRps = new Components.PCorumbaMS.NfseWSService();
                                        //else
                                        //    envLoteRps = new Components.HCorumbaMS.NfseWSService();

                                        break;

                                    case PadroesNFSe.BSITBR:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.BSITBR);

                                        wsProxy = new WebServiceProxy(Empresas.Configuracoes[emp].X509Certificado);

                                        if(oDadosEnvLoteRps.tpAmb == 1)
                                        {
                                            switch(oDadosEnvLoteRps.cMunicipio)
                                            {
                                                case 5211800:
                                                    envLoteRps = new Components.PJaraguaGO.nfseWS();
                                                    break;

                                                case 3507506:
                                                    envLoteRps = new Components.PBotucatuSP.nfseWS();
                                                    break;

                                                case 5211909:
                                                    envLoteRps = new Components.PJataiGO.nfseWS();
                                                    break;

                                                case 5220603:
                                                    envLoteRps = new Components.PSilvaniaGO.nfseWS();
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Este município não dispõe de ambiente de homologação para envio de NFS-e em teste.");
                                        }

                                        break;

                                    case PadroesNFSe.PORTOVELHENSE:
                                        cabecMsg = "<cabecalho versao=\"2.00\" xmlns:ns2=\"http://www.w3.org/2000/09/xmldsig#\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.00</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.DSF:
                                        if(oDadosEnvLoteRps.cMunicipio == 3549904)
                                        {
                                            cabecMsg = "<cabecalho versao=\"3\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>3</versaoDados></cabecalho>";
                                        }
                                        else
                                        {
                                            EncryptAssinatura();
                                        }
                                        break;

                                    case PadroesNFSe.TECNOSISTEMAS:
                                        cabecMsg = "<?xml version=\"1.0\" encoding=\"utf-8\"?><cabecalho xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" versao=\"20.01\" xmlns=\"http://www.nfse-tecnos.com.br/nfse.xsd\"><versaoDados>20.01</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.FINTEL:
                                        cabecMsg = "<cabecalho versao=\"2.02\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.02</versaoDados></cabecalho>";
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.FINTEL);
                                        break;

                                    case PadroesNFSe.PORTALFACIL_ACTCON:
                                        cabecMsg = "<cabecalho><versaoDados>2.01</versaoDados></cabecalho>";
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.PORTALFACIL_ACTCON);
                                        break;

                                    case PadroesNFSe.PORTALFACIL_ACTCON_202:
                                        if(oDadosEnvLoteRps.cMunicipio != 3169901)
                                        {
                                            cabecMsg = "<cabecalho><versaoDados>2.02</versaoDados></cabecalho>";
                                        }

                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.PORTALFACIL_ACTCON_202);
                                        break;

                                    case PadroesNFSe.RLZ_INFORMATICA_02:
                                        cabecMsg = "<cabecalho><versaoDados>2.02</versaoDados></cabecalho>";
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.RLZ_INFORMATICA_02);
                                        break;

                                    case PadroesNFSe.SYSTEMPRO:

                                        #region SystemPro

                                        var syspro = new SystemPro((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                            Empresas.Configuracoes[emp].PastaXmlRetorno, Empresas.Configuracoes[emp].X509Certificado, oDadosEnvLoteRps.cMunicipio);
                                        var ad = new AssinaturaDigital();
                                        ad.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio);
                                        syspro.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion SystemPro

                                    case PadroesNFSe.SIGCORP_SIGISS:
                                        var sigcorp = new SigCorp((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                            Empresas.Configuracoes[emp].PastaXmlRetorno,
                                            oDadosEnvLoteRps.cMunicipio);
                                        sigcorp.EmiteNF(NomeArquivoXML);
                                        break;

                                    case PadroesNFSe.SIGCORP_SIGISS_203:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.SIGCORP_SIGISS_203);

                                        cabecMsg = "<cabecalho versao=\"2.03\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.03</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.FIORILLI:

                                        #region Fiorilli

                                        var fiorilli = new Fiorilli((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                         Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                         oDadosEnvLoteRps.cMunicipio,
                                                                         Empresas.Configuracoes[emp].UsuarioWS,
                                                                         Empresas.Configuracoes[emp].SenhaWS,
                                                                         ConfiguracaoApp.ProxyUsuario,
                                                                         ConfiguracaoApp.ProxySenha,
                                                                         ConfiguracaoApp.ProxyServidor,
                                                                         Empresas.Configuracoes[emp].X509Certificado);

                                        var ass = new AssinaturaDigital();
                                        ass.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio);

                                        // Validar o Arquivo XML
                                        var validar = new ValidarXML(NomeArquivoXML, Empresas.Configuracoes[emp].UnidadeFederativaCodigo, false);
                                        var resValidacao = validar.ValidarArqXML(NomeArquivoXML);
                                        if(resValidacao != "")
                                        {
                                            throw new Exception(resValidacao);
                                        }

                                        fiorilli.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion Fiorilli

                                    case PadroesNFSe.SIMPLISS:

                                        #region Simpliss

                                        var simpliss = new SimplISS((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                        oDadosEnvLoteRps.cMunicipio,
                                        Empresas.Configuracoes[emp].UsuarioWS,
                                        Empresas.Configuracoes[emp].SenhaWS,
                                        ConfiguracaoApp.ProxyUsuario,
                                        ConfiguracaoApp.ProxySenha,
                                        ConfiguracaoApp.ProxyServidor);

                                        var sign = new AssinaturaDigital();
                                        sign.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio);

                                        simpliss.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion Simpliss

                                    case PadroesNFSe.CONAM:

                                        #region Conam

                                        var conam = new Conam((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                        oDadosEnvLoteRps.cMunicipio,
                                        Empresas.Configuracoes[emp].UsuarioWS,
                                        Empresas.Configuracoes[emp].SenhaWS);

                                        conam.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion Conam

                                    case PadroesNFSe.RLZ_INFORMATICA:

                                        #region RLZ

                                        var rlz = new Rlz_Informatica((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                        oDadosEnvLoteRps.cMunicipio);

                                        rlz.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion RLZ

                                    case PadroesNFSe.EGOVERNE:

                                        #region E-Governe

                                        var egoverne = new EGoverne((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                            Empresas.Configuracoes[emp].PastaXmlRetorno,
                                            oDadosEnvLoteRps.cMunicipio,
                                            ConfiguracaoApp.ProxyUsuario,
                                            ConfiguracaoApp.ProxySenha,
                                            ConfiguracaoApp.ProxyServidor,
                                            Empresas.Configuracoes[emp].X509Certificado);

                                        var assEGovoverne = new AssinaturaDigital();
                                        assEGovoverne.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio);

                                        egoverne.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion E-Governe

                                    case PadroesNFSe.EL:

                                        #region E&L

                                        var el = new EL((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                        oDadosEnvLoteRps.cMunicipio,
                                                        Empresas.Configuracoes[emp].UsuarioWS,
                                                        Empresas.Configuracoes[emp].SenhaWS,
                                                        (ConfiguracaoApp.Proxy ? ConfiguracaoApp.ProxyUsuario : ""),
                                                        (ConfiguracaoApp.Proxy ? ConfiguracaoApp.ProxySenha : ""),
                                                        (ConfiguracaoApp.Proxy ? ConfiguracaoApp.ProxyServidor : ""));

                                        el.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion E&L

                                    case PadroesNFSe.GOVDIGITAL:

                                        #region GOV-Digital

                                        var govdig = new GovDigital((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                            Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                            Empresas.Configuracoes[emp].X509Certificado,
                                                                            oDadosEnvLoteRps.cMunicipio,
                                                                            ConfiguracaoApp.ProxyUsuario,
                                                                            ConfiguracaoApp.ProxySenha,
                                                                            ConfiguracaoApp.ProxyServidor);

                                        var adgovdig = new AssinaturaDigital();
                                        adgovdig.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio);

                                        govdig.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion GOV-Digital

                                    case PadroesNFSe.EQUIPLANO:
                                        cabecMsg = "1";
                                        break;

                                    case PadroesNFSe.NATALENSE:
                                        cabecMsg = "<cabecalho xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" versao=\"1\" xmlns=\"http://www.abrasf.org.br/ABRASF/arquivos/nfse.xsd\"><versaoDados>1</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.VVISS:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.VVISS);
                                        break;

                                    case PadroesNFSe.METROPOLIS:

                                        #region METROPOLIS

                                        var metropolis = new Metropolis((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                      Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                      oDadosEnvLoteRps.cMunicipio,
                                                                      ConfiguracaoApp.ProxyUsuario,
                                                                      ConfiguracaoApp.ProxySenha,
                                                                      ConfiguracaoApp.ProxyServidor,
                                                                      Empresas.Configuracoes[emp].X509Certificado);

                                        var metropolisdig = new AssinaturaDigital();
                                        metropolisdig.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio);

                                        metropolis.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion METROPOLIS

                                    case PadroesNFSe.MGM:

                                        #region MGM

                                        var mgm = new MGM((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                           Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                           oDadosEnvLoteRps.cMunicipio,
                                                           Empresas.Configuracoes[emp].UsuarioWS,
                                                           Empresas.Configuracoes[emp].SenhaWS);
                                        mgm.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion MGM

                                    case PadroesNFSe.CONSIST:

                                        #region Consist

                                        var consist = new Consist((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                        oDadosEnvLoteRps.cMunicipio,
                                        Empresas.Configuracoes[emp].UsuarioWS,
                                        Empresas.Configuracoes[emp].SenhaWS,
                                        ConfiguracaoApp.ProxyUsuario,
                                        ConfiguracaoApp.ProxySenha,
                                        ConfiguracaoApp.ProxyServidor);

                                        consist.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion Consist

                                    case PadroesNFSe.NOTAINTELIGENTE:

                                        #region Nota Inteligente

                                        wsProxy = new WebServiceProxy(Empresas.Configuracoes[emp].X509Certificado);

                                        if(oDadosEnvLoteRps.tpAmb == 1)
                                        {
                                            //envLoteRps = new NFe.Components.PClaudioMG.api_portClient();
                                        }
                                        else
                                        {
                                            throw new Exception("Município de São Paulo-SP não dispõe de ambiente de homologação para envio de NFS-e em teste.");
                                        }

                                        #endregion Nota Inteligente

                                        break;

                                    case PadroesNFSe.COPLAN:

                                        #region Coplan

                                        var coplan = new Coplan((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                            Empresas.Configuracoes[emp].PastaXmlRetorno,
                                            oDadosEnvLoteRps.cMunicipio,
                                            ConfiguracaoApp.ProxyUsuario,
                                            ConfiguracaoApp.ProxySenha,
                                            ConfiguracaoApp.ProxyServidor,
                                            Empresas.Configuracoes[emp].X509Certificado);

                                        var assCoplan = new AssinaturaDigital();
                                        assCoplan.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio);

                                        coplan.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion Coplan

                                    case PadroesNFSe.MEMORY:

                                        #region Memory

                                        var memory = new Memory((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                        oDadosEnvLoteRps.cMunicipio,
                                        Empresas.Configuracoes[emp].UsuarioWS,
                                        Empresas.Configuracoes[emp].SenhaWS,
                                        ConfiguracaoApp.ProxyUsuario,
                                        ConfiguracaoApp.ProxySenha,
                                        ConfiguracaoApp.ProxyServidor);

                                        var sigMem = new AssinaturaDigital();
                                        sigMem.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio);

                                        memory.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion Memory

                                    case PadroesNFSe.PRODEB:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.PRODEB);

                                        cabecMsg = "<cabecalho><versaoDados>2.01</versaoDados><versao>2.01</versao></cabecalho>";
                                        break;

                                    case PadroesNFSe.CARIOCA:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.CARIOCA);
                                        break;

                                    case PadroesNFSe.PRONIN:
                                        if(oDadosEnvLoteRps.cMunicipio == 3131703 ||
                                            oDadosEnvLoteRps.cMunicipio == 4303004 ||
                                            oDadosEnvLoteRps.cMunicipio == 3556602 ||
                                            oDadosEnvLoteRps.cMunicipio == 3512803 ||
                                            oDadosEnvLoteRps.cMunicipio == 4323002 ||
                                            oDadosEnvLoteRps.cMunicipio == 3505807 ||
                                            oDadosEnvLoteRps.cMunicipio == 3530300 ||
                                            oDadosEnvLoteRps.cMunicipio == 4308904 ||
                                            oDadosEnvLoteRps.cMunicipio == 4118501 ||
                                            oDadosEnvLoteRps.cMunicipio == 3554300 ||
                                            oDadosEnvLoteRps.cMunicipio == 3542404 ||
                                            oDadosEnvLoteRps.cMunicipio == 5005707 ||
                                            oDadosEnvLoteRps.cMunicipio == 4314423 ||
                                            oDadosEnvLoteRps.cMunicipio == 3535804 ||
                                            oDadosEnvLoteRps.cMunicipio == 4306932 ||
                                            oDadosEnvLoteRps.cMunicipio == 4310207 ||
                                            oDadosEnvLoteRps.cMunicipio == 4322400 ||
                                            oDadosEnvLoteRps.cMunicipio == 4302808 ||
                                            oDadosEnvLoteRps.cMunicipio == 3501301 ||
                                            oDadosEnvLoteRps.cMunicipio == 4300109 ||
                                            oDadosEnvLoteRps.cMunicipio == 4124053 ||
                                            oDadosEnvLoteRps.cMunicipio == 3550407 ||
                                            oDadosEnvLoteRps.cMunicipio == 1502400 ||
                                            oDadosEnvLoteRps.cMunicipio == 4322509 ||
                                            oDadosEnvLoteRps.cMunicipio == 4301057 ||
                                            oDadosEnvLoteRps.cMunicipio == 4115804 ||
                                            oDadosEnvLoteRps.cMunicipio == 3550803 ||
                                            oDadosEnvLoteRps.cMunicipio == 4313953)
                                        {
                                            var pronin = new Pronin((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                oDadosEnvLoteRps.cMunicipio,
                                                ConfiguracaoApp.ProxyUsuario,
                                                ConfiguracaoApp.ProxySenha,
                                                ConfiguracaoApp.ProxyServidor,
                                                Empresas.Configuracoes[emp].X509Certificado);

                                            var assPronin = new AssinaturaDigital();
                                            assPronin.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio);

                                            pronin.EmiteNF(NomeArquivoXML);
                                        }
                                        break;

                                    case PadroesNFSe.EGOVERNEISS:

                                        #region EGoverne ISS

                                        var egoverneiss = new EGoverneISS((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                        oDadosEnvLoteRps.cMunicipio,
                                        Empresas.Configuracoes[emp].UsuarioWS,
                                        Empresas.Configuracoes[emp].SenhaWS,
                                        ConfiguracaoApp.ProxyUsuario,
                                        ConfiguracaoApp.ProxySenha,
                                        ConfiguracaoApp.ProxyServidor);

                                        egoverneiss.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion EGoverne ISS

                                    case PadroesNFSe.SUPERNOVA:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.SUPERNOVA);
                                        break;

                                    case PadroesNFSe.MARINGA_PR:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.MARINGA_PR);
                                        break;

                                    #region Tinus

                                    case PadroesNFSe.TINUS:
                                        var tinus = new Tinus((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                            Empresas.Configuracoes[emp].PastaXmlRetorno,
                                            oDadosEnvLoteRps.cMunicipio,
                                            ConfiguracaoApp.ProxyUsuario,
                                            ConfiguracaoApp.ProxySenha,
                                            ConfiguracaoApp.ProxyServidor,
                                            Empresas.Configuracoes[emp].X509Certificado);

                                        var tinusAss = new AssinaturaDigital();
                                        tinusAss.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio);

                                        tinus.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion Tinus

                                    case PadroesNFSe.GEISWEB:
                                        var geisWeb = new GeisWeb((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                            Empresas.Configuracoes[emp].PastaXmlRetorno,
                                            oDadosEnvLoteRps.cMunicipio,
                                            ConfiguracaoApp.ProxyUsuario,
                                            ConfiguracaoApp.ProxySenha,
                                            ConfiguracaoApp.ProxyServidor,
                                            Empresas.Configuracoes[emp].X509Certificado);

                                        //var geisWebAss = new AssinaturaDigital();
                                        //geisWebAss.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio);

                                        geisWeb.EmiteNF(NomeArquivoXML);
                                        break;


                                    #region SOFTPLAN

                                    case PadroesNFSe.SOFTPLAN:
                                        var softplan = new Components.SOFTPLAN.SOFTPLAN((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                        Empresas.Configuracoes[emp].TokenNFse,
                                                                        Empresas.Configuracoes[emp].TokenNFSeExpire,
                                                                        Empresas.Configuracoes[emp].UsuarioWS,
                                                                        Empresas.Configuracoes[emp].SenhaWS,
                                                                        Empresas.Configuracoes[emp].ClientID,
                                                                        Empresas.Configuracoes[emp].ClientSecret);

                                        var softplanAssinatura = new AssinaturaDigital();
                                        softplanAssinatura.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio);

                                        // Validar o Arquivo XML
                                        var softplanValidar = new ValidarXML(NomeArquivoXML, Empresas.Configuracoes[emp].UnidadeFederativaCodigo, false);
                                        var validacao = softplanValidar.ValidarArqXML(NomeArquivoXML);
                                        if(validacao != "")
                                        {
                                            throw new Exception(validacao);
                                        }

                                        if(ConfiguracaoApp.Proxy)
                                        {
                                            softplan.Proxy = Unimake.Net.Utility.GetProxy(ConfiguracaoApp.ProxyServidor, ConfiguracaoApp.ProxyUsuario, ConfiguracaoApp.ProxySenha, ConfiguracaoApp.ProxyPorta);
                                        }

                                        var softplanAss = new AssinaturaDigital();
                                        softplanAss.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio, AlgorithmType.Sha256);

                                        softplan.EmiteNF(NomeArquivoXML);

                                        if(Empresas.Configuracoes[emp].TokenNFse != softplan.Token)
                                        {
                                            Empresas.Configuracoes[emp].SalvarConfiguracoesNFSeSoftplan(softplan.Usuario,
                                                                                                        softplan.Senha,
                                                                                                        softplan.ClientID,
                                                                                                        softplan.ClientSecret,
                                                                                                        softplan.Token,
                                                                                                        softplan.TokenExpire,
                                                                                                        Empresas.Configuracoes[emp].CNPJ);
                                        }

                                        break;

                                    #endregion SOFTPLAN

                                    #region CENTI

                                    case PadroesNFSe.CENTI:
                                        var centi = new CENTI((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                        oDadosEnvLoteRps.cMunicipio,
                                                                        Empresas.Configuracoes[emp].UsuarioWS,
                                                                        Empresas.Configuracoes[emp].SenhaWS);

                                        var centiAssinatura = new AssinaturaDigital();
                                        centiAssinatura.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio);

                                        // Validar o Arquivo XML
                                        var centiValidar = new ValidarXML(NomeArquivoXML, Empresas.Configuracoes[emp].UnidadeFederativaCodigo, false);
                                        var validacaoCenti = centiValidar.ValidarArqXML(NomeArquivoXML);
                                        if(validacaoCenti != "")
                                        {
                                            throw new Exception(validacaoCenti);
                                        }

                                        if(ConfiguracaoApp.Proxy)
                                        {
                                            centi.Proxy = Unimake.Net.Utility.GetProxy(ConfiguracaoApp.ProxyServidor, ConfiguracaoApp.ProxyUsuario, ConfiguracaoApp.ProxySenha, ConfiguracaoApp.ProxyPorta);
                                        }

                                        //var softplanAss = new AssinaturaDigital();
                                        //softplanAss.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio, AlgorithmType.Sha256);

                                        centi.EmiteNF(NomeArquivoXML);

                                        break;

                                    #endregion CENTI

                                    #region GIAP

                                    case PadroesNFSe.GIAP:
                                        var giap = new Giap((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                        oDadosEnvLoteRps.cMunicipio,
                                                                        Empresas.Configuracoes[emp].SenhaWS);

                                        if (ConfiguracaoApp.Proxy)
                                        {
                                            giap.Proxy = Unimake.Net.Utility.GetProxy(ConfiguracaoApp.ProxyServidor, ConfiguracaoApp.ProxyUsuario, ConfiguracaoApp.ProxySenha, ConfiguracaoApp.ProxyPorta);
                                        }

                                        giap.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion GIAP

                                    #region SIGISSWEB

                                    case PadroesNFSe.SIGISSWEB:
                                        var sigissweb = new Components.SIGISSWEB.SIGISSWEB((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                        Empresas.Configuracoes[emp].UsuarioWS,
                                                                        Empresas.Configuracoes[emp].SenhaWS);

                                        if (ConfiguracaoApp.Proxy)
                                        {
                                            sigissweb.Proxy = Unimake.Net.Utility.GetProxy(ConfiguracaoApp.ProxyServidor, ConfiguracaoApp.ProxyUsuario, ConfiguracaoApp.ProxySenha, ConfiguracaoApp.ProxyPorta);
                                        }

                                        sigissweb.EmiteNF(NomeArquivoXML);
                                        break;

                                    #endregion SIGISSWEB

                                    case PadroesNFSe.INTERSOL:
                                        cabecMsg = "<?xml version=\"1.0\" encoding=\"utf-8\"?><p:cabecalho versao=\"1\" xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\" xmlns:p=\"http://ws.speedgov.com.br/cabecalho_v1.xsd\" xmlns:p1=\"http://ws.speedgov.com.br/tipos_v1.xsd\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://ws.speedgov.com.br/cabecalho_v1.xsd cabecalho_v1.xsd \"><versaoDados>1</versaoDados></p:cabecalho>";
                                        break;

                                    case PadroesNFSe.MANAUS_AM:
                                        cabecMsg = "<cabecalho versao=\"201001\"><versaoDados>V2010</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.EMBRAS:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.EMBRAS);
                                        cabecMsg = "<cabecalho versao=\"2.02\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.02</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.DESENVOLVECIDADE:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.EMBRAS);
                                        break;

                                    case PadroesNFSe.MODERNIZACAO_PUBLICA:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.MODERNIZACAO_PUBLICA);
                                        cabecMsg = "<cabecalho xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\" versao=\"2.02\"><versaoDados>2.02</versaoDados></cabecalho>";
                                        break;



                                    case PadroesNFSe.PUBLICA:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.PUBLICA);
                                        break;

                                    case PadroesNFSe.E_RECEITA:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.E_RECEITA);
                                        cabecMsg = "<cabecalho xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\" versao=\"2.02\"><versaoDados>2.02</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.TIPLAN_203:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.TIPLAN_203);
                                        cabecMsg = "<cabecalho versao=\"2.03\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.03</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.ADM_SISTEMAS:
                                        cabecMsg = "<cabecalho versao=\"2.01\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.01</versaoDados></cabecalho>";
                                        wsProxy = new WebServiceProxy(Empresas.Configuracoes[emp].X509Certificado);

                                        envLoteRps = oDadosEnvLoteRps.tpAmb == 1 ?
                                                        new Components.PAmargosaBA.InfseClient(GetBinding(), new EndpointAddress("https://demo.saatri.com.br/servicos/nfse.svc")) :
                                                        new Components.HAmargosaBA.InfseClient(GetBinding(), new EndpointAddress("https://homologa-demo.saatri.com.br/servicos/nfse.svc")) as object;

                                        SignUsingCredentials(emp, envLoteRps);
                                        break;

                                    case PadroesNFSe.PUBLIC_SOFT:
                                        break;

                                    case PadroesNFSe.MEGASOFT:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.MEGASOFT);
                                        cabecMsg = "<cabecalho versao=\"1.00\" xmlns=\"http://megasoftarrecadanet.com.br/xsd/nfse_v01.xsd\"><versaoDados>1.00</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.SIMPLE:

                                        var simple = new Simple((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                      Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                      oDadosEnvLoteRps.cMunicipio,
                                                                      ConfiguracaoApp.ProxyUsuario,
                                                                      ConfiguracaoApp.ProxySenha,
                                                                      ConfiguracaoApp.ProxyServidor,
                                                                      Empresas.Configuracoes[emp].X509Certificado);

                                        simple.EmiteNF(NomeArquivoXML);
                                        break;

                                    case PadroesNFSe.INDAIATUBA_SP:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.INDAIATUBA_SP);
                                        cabecMsg = "<cabecalho versao=\"2.03\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.03</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.SISPMJP:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.SISPMJP);
                                        cabecMsg = "<cabecalho versao=\"2.02\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\" ><versaoDados>2.02</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.SMARAPD_203:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.SMARAPD_203);
                                        break;

                                    case PadroesNFSe.SMARAPD_204:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.SMARAPD_204);
                                        cabecMsg = "<cabecalho versao=\"2.04\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.04</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.D2TI:
                                        cabecMsg = "<cabecalhoNfseLote xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.ctaconsult.com/nfse\"><versao>1.00</versao><ambiente>" + Empresas.Configuracoes[emp].AmbienteCodigo + "</ambiente></cabecalhoNfseLote>";
                                        break;

                                    case PadroesNFSe.IIBRASIL:
                                        Servico = Servicos.NFSeGerarNfse;
                                        cabecMsg = "<cabecalho xmlns=\"http://www.abrasf.org.br/nfse.xsd\" versao=\"2.04\"><versaoDados>2.04</versaoDados></cabecalho>";
                                        GerarTagIntegracao(Empresas.Configuracoes[emp].SenhaWS);
                                        break;

                                    case PadroesNFSe.WEBFISCO_TECNOLOGIA:
                                        var webTecnologia = new WEBFISCO_TECNOLOGIA((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                           Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                           oDadosEnvLoteRps.cMunicipio,
                                                           Empresas.Configuracoes[emp].UsuarioWS,
                                                           Empresas.Configuracoes[emp].SenhaWS);
                                        webTecnologia.EmiteNF(NomeArquivoXML);
                                        break;

                                    case PadroesNFSe.ELOTECH:
                                        var elotech = new Elotech((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                            Empresas.Configuracoes[emp].PastaXmlRetorno,
                                            oDadosEnvLoteRps.cMunicipio,
                                            ConfiguracaoApp.ProxyUsuario,
                                            ConfiguracaoApp.ProxySenha,
                                            ConfiguracaoApp.ProxyServidor,
                                            Empresas.Configuracoes[emp].X509Certificado);

                                        elotech.EmiteNF(NomeArquivoXML);
                                        break;

                                    case PadroesNFSe.SYSMAR:
                                        Servico = GetTipoServicoSincrono(Servico, NomeArquivoXML, PadroesNFSe.SYSMAR);
                                        break;
                                }

                                if(IsInvocar(padraoNFSe, Servico, oDadosEnvLoteRps.cMunicipio))
                                {
                                    //Assinar o XML
                                    var ad = new AssinaturaDigital();
                                    ad.Assinar(NomeArquivoXML, emp, oDadosEnvLoteRps.cMunicipio);

                                    //Invocar o método que envia o XML para o SEFAZ
                                    oInvocarObj.InvocarNFSe(wsProxy, envLoteRps, NomeMetodoWS(Servico, oDadosEnvLoteRps.cMunicipio), cabecMsg, this,
                                                            Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).EnvioXML,
                                                            Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).RetornoXML,
                                                            padraoNFSe, Servico, securityProtocolType);

                                    ///
                                    /// grava o arquivo no FTP
                                    var filenameFTP = Path.Combine(Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                      Functions.ExtrairNomeArq(NomeArquivoXML,
                                                                      Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).EnvioXML) + "\\" + Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).RetornoXML);
                                    if(File.Exists(filenameFTP))
                                    {
                                        new GerarXML(emp).XmlParaFTP(emp, filenameFTP);
                                    }
                                }

                                break;
                        }

                        break;
                }
            }
            catch(Exception ex)
            {
                try
                {
                    //Gravar o arquivo de erro de retorno para o ERP, caso ocorra
                    TFunctions.GravarArqErroServico(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).EnvioXML, Propriedade.ExtRetorno.RetEnvLoteRps_ERR, ex);
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

        private void GerarTagIntegracao(string token)
        {
            var doc = new XmlDocument();
            doc.Load(NomeArquivoXML);
            string conteudoXML, integridade;
            conteudoXML = doc.GetElementsByTagName("Rps")[0].OuterXml;
            conteudoXML = Regex.Replace(conteudoXML, "[^\x20-\x7E]+", "");
            conteudoXML = Regex.Replace(conteudoXML, "[ ]+", "");
            integridade = Criptografia.GerarRSASHA512(conteudoXML + token, true);

            foreach(var item in ConteudoXML)
            {
                if(typeof(XmlElement) == item.GetType())
                {
                    XmlNode gerarNfseEnvio = (XmlElement)ConteudoXML.GetElementsByTagName("GerarNfseEnvio")[0];
                    XmlNode tagintegridade = ConteudoXML.CreateElement("Integridade", ConteudoXML.DocumentElement.NamespaceURI);

                    tagintegridade.InnerXml = (integridade.Trim()).Trim();

                    gerarNfseEnvio.AppendChild(tagintegridade);

                    conteudoXML = gerarNfseEnvio.OuterXml;

                    break;
                }
            }
            try
            {
                // Atualizar a string do XML já assinada
                var StringXMLAssinado = conteudoXML;

                // Gravar o XML Assinado no HD
                var SW_2 = File.CreateText(NomeArquivoXML);
                SW_2.Write(StringXMLAssinado);
                SW_2.Close();
            }
            catch
            {
                throw;
            }
        }

        #region EncryptAssinatura()

        /// <summary>
        /// Encriptar a tag Assinatura quando for município de Blumenau - SC
        /// </summary>
        private void EncryptAssinatura()
        {
            ///danasa: 12/2013
            var val = new Validate.ValidarXML(NomeArquivoXML, oDadosEnvLoteRps.cMunicipio, false);
            val.EncryptAssinatura(Empresas.FindEmpresaByThread(), NomeArquivoXML);
        }

        #endregion EncryptAssinatura()

        #region EnvLoteRps()

        /// <summary>
        /// Fazer a leitura do conteúdo do XML de lote rps e disponibiliza o conteúdo em um objeto para analise
        /// </summary>
        /// <param name="arquivoXML">Arquivo XML que é para efetuar a leitura</param>
        private void EnvLoteRps(int emp, string arquivoXML)
        {
        }

        #endregion EnvLoteRps()

        /// <summary>
        /// Executa o serviço utilizando a DLL do UniNFe.
        /// </summary>
        /// <param name="emp">Empresa que está enviando o XML</param>
        /// <param name="municipio">Código do município para onde será enviado o XML</param>
        /// <param name="padraoNFSe">Padrão do munípio para NFSe</param>
        private void ExecuteDLL(int emp, int municipio, PadroesNFSe padraoNFSe)
        {
            if(padraoNFSe == PadroesNFSe.PAULISTANA)
            {
                EncryptAssinatura();
            }
            var conteudoXML = new XmlDocument();
            conteudoXML.Load(NomeArquivoXML);

            var finalArqEnvio = Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).EnvioXML;
            var finalArqRetorno = Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).RetornoXML;
            var servico = DefinirServico(municipio, conteudoXML, padraoNFSe);
            var versaoXML = DefinirVersaoXML(municipio, conteudoXML, padraoNFSe);

            Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlRetorno + "\\" + Functions.ExtrairNomeArq(NomeArquivoXML, finalArqEnvio) + Functions.ExtractExtension(finalArqRetorno) + ".err");

            var configuracao = new Unimake.Business.DFe.Servicos.Configuracao
            {
                TipoDFe = Unimake.Business.DFe.Servicos.TipoDFe.NFSe,
                CertificadoDigital = Empresas.Configuracoes[emp].X509Certificado,
                TipoAmbiente = (Unimake.Business.DFe.Servicos.TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                CodigoMunicipio = municipio,
                Servico = servico,
                SchemaVersao = versaoXML,
                MunicipioToken = Empresas.Configuracoes[emp].SenhaWS
            };

            switch(servico)
            {
                case Unimake.Business.DFe.Servicos.Servico.NFSeGerarNfse:
                    var gerarNfse = new Unimake.Business.DFe.Servicos.NFSe.GerarNfse(conteudoXML, configuracao);
                    gerarNfse.Executar();
                    vStrXmlRetorno = gerarNfse.RetornoWSString;
                    break;

                case Unimake.Business.DFe.Servicos.Servico.NFSeRecepcionarLoteRps:
                    var recepcionarLoteRps = new Unimake.Business.DFe.Servicos.NFSe.RecepcionarLoteRps(conteudoXML, configuracao);
                    recepcionarLoteRps.Executar();
                    vStrXmlRetorno = recepcionarLoteRps.RetornoWSString;
                    break;

                case Unimake.Business.DFe.Servicos.Servico.NFSeRecepcionarLoteRpsSincrono:
                    var recepcionarLoteRpsSincrono = new Unimake.Business.DFe.Servicos.NFSe.RecepcionarLoteRpsSincrono(conteudoXML, configuracao);
                    recepcionarLoteRpsSincrono.Executar();
                    vStrXmlRetorno = recepcionarLoteRpsSincrono.RetornoWSString;
                    break;

                case Unimake.Business.DFe.Servicos.Servico.NFSeEnvioLoteRps:
                    var envioLoteRps = new Unimake.Business.DFe.Servicos.NFSe.EnvioLoteRps(conteudoXML, configuracao);
                    envioLoteRps.Executar();
                    vStrXmlRetorno = envioLoteRps.RetornoWSString;
                    break;

                case Unimake.Business.DFe.Servicos.Servico.NFSeEnvioRps:
                    var envioRps = new Unimake.Business.DFe.Servicos.NFSe.EnvioRps(conteudoXML, configuracao);
                    envioRps.Executar();
                    vStrXmlRetorno = envioRps.RetornoWSString;
                    break;

                case Unimake.Business.DFe.Servicos.Servico.NFSeTesteEnvioLoteRps:
                    var testeEnvioLoteRps = new Unimake.Business.DFe.Servicos.NFSe.TesteEnvioLoteRps(conteudoXML, configuracao);
                    testeEnvioLoteRps.Executar();
                    vStrXmlRetorno = testeEnvioLoteRps.RetornoWSString;
                    break;
            }

            XmlRetorno(finalArqEnvio, finalArqRetorno);

            /// grava o arquivo no FTP
            var filenameFTP = Path.Combine(Empresas.Configuracoes[emp].PastaXmlRetorno,
                Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).EnvioXML) + Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).RetornoXML);

            if(File.Exists(filenameFTP))
            {
                new GerarXML(emp).XmlParaFTP(emp, filenameFTP);
            }
        }

        /// <summary>
        /// Define qual o tipo de serviço de envio de NFSe será utilizado. Envio em lote sincrono, Envio em lote assincrono ou envio de uma única NFSe síncrono.
        /// </summary>
        /// <param name="municipio">Código do município para onde será enviado o XML</param>
        /// <param name="doc">Conteúdo do XML da NFSe</param>
        /// <param name="padraoNFSe">Padrão do munípio para NFSe</param>
        /// <returns>Retorna o tipo de serviço de envio da NFSe da prefeitura será utilizado</returns>
        private Unimake.Business.DFe.Servicos.Servico DefinirServico(int municipio, XmlDocument doc, PadroesNFSe padraoNFSe)
        {
            var result = Unimake.Business.DFe.Servicos.Servico.NFSeRecepcionarLoteRps;

            switch(padraoNFSe)
            {
                case PadroesNFSe.NOTAINTELIGENTE:
                case PadroesNFSe.BETHA:
                case PadroesNFSe.PRODATA:
                case PadroesNFSe.AVMB_ASTEN:
                case PadroesNFSe.WEBISS:
                case PadroesNFSe.COPLAN:
                case PadroesNFSe.PROPRIOJOINVILLESC:
                case PadroesNFSe.SIMPLISS:
                case PadroesNFSe.SONNER:
                case PadroesNFSe.EL:
                case PadroesNFSe.SMARAPD:
                case PadroesNFSe.BHISS:
                case PadroesNFSe.TRIBUTUS:
                case PadroesNFSe.DSF:
                case PadroesNFSe.DIGIFRED:
                case PadroesNFSe.VERSATEC:
                case PadroesNFSe.QUASAR:
                    switch (doc.DocumentElement.Name)
                    {
                        case "EnviarLoteRpsSincronoEnvio":
                            result = Unimake.Business.DFe.Servicos.Servico.NFSeRecepcionarLoteRpsSincrono;
                            break;
                        case "EnviarLoteRpsEnvio":
                            result = Unimake.Business.DFe.Servicos.Servico.NFSeRecepcionarLoteRps;
                            break;
                        case "GerarNfseEnvio":
                            result = Unimake.Business.DFe.Servicos.Servico.NFSeGerarNfse;
                            break;
                    }
                    break;

                case PadroesNFSe.SIGCORP_SIGISS:
                    switch (doc.DocumentElement.Name)
                    {
                        case "EnviarLoteRpsSincronoEnvio":
                            result = Unimake.Business.DFe.Servicos.Servico.NFSeRecepcionarLoteRpsSincrono;
                            break;
                        case "EnviarLoteRpsEnvio":
                            result = Unimake.Business.DFe.Servicos.Servico.NFSeRecepcionarLoteRps;
                            break;
                        case "GerarNfseEnvio":
                            result = Unimake.Business.DFe.Servicos.Servico.NFSeGerarNfse;
                            break;
                        case "GerarNota":
                            result = Unimake.Business.DFe.Servicos.Servico.NFSeGerarNfse;
                            break;
                    }
                    break;

                case PadroesNFSe.NOBESISTEMAS:
                case PadroesNFSe.GINFES:
                    result = Unimake.Business.DFe.Servicos.Servico.NFSeRecepcionarLoteRps;
                    break;

                case PadroesNFSe.PAULISTANA:
                    switch (doc.DocumentElement.Name)
                    {
                        case "PedidoEnvioLoteRPS":

                            if (Empresas.Configuracoes[Empresas.FindEmpresaByThread()].AmbienteCodigo == (int)NFe.Components.TipoAmbiente.taHomologacao)
                            {
                                result = Unimake.Business.DFe.Servicos.Servico.NFSeTesteEnvioLoteRps;
                            }
                            else
                            {
                                result = Unimake.Business.DFe.Servicos.Servico.NFSeEnvioLoteRps;
                            }
                            break;

                        case "PedidoEnvioRPS":
                            result = Unimake.Business.DFe.Servicos.Servico.NFSeEnvioRps;
                            break;          
                    }
                    break;
            }

            return result;
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

            switch(padraoNFSe)
            {
                case PadroesNFSe.PRODATA:
                    versaoXML = Functions.GetAttributeXML("LoteRps", "versao", NomeArquivoXML);
                    if(string.IsNullOrWhiteSpace(versaoXML))
                    {
                        versaoXML = "2.01";
                    }

                    break;

                case PadroesNFSe.BETHA:
                    versaoXML = Functions.GetAttributeXML("LoteRps", "versao", NomeArquivoXML);
                    if(string.IsNullOrWhiteSpace(versaoXML))
                    {
                        if(xmlDoc.GetElementsByTagName("GerarNfseEnvio").Count > 0)
                        {
                            versaoXML = "2.02";
                        }
                    }

                    if(!versaoXML.Equals("2.02"))
                    {
                        versaoXML = "1.00";
                    }
                    break;

                case PadroesNFSe.NOBESISTEMAS:
                case PadroesNFSe.BHISS:
                    versaoXML = "1.00";
                    break;

                case PadroesNFSe.PAULISTANA:
                case PadroesNFSe.DIGIFRED:
                    versaoXML = "2.00";
                    break;

                case PadroesNFSe.SONNER:
                case PadroesNFSe.QUASAR:
                    versaoXML = "2.01";
                    break;

                case PadroesNFSe.NOTAINTELIGENTE:
                case PadroesNFSe.AVMB_ASTEN:
                case PadroesNFSe.WEBISS:
                case PadroesNFSe.VERSATEC:
                    versaoXML = "2.02";
                    break;

                case PadroesNFSe.SIMPLISS:
                case PadroesNFSe.SMARAPD:
                case PadroesNFSe.DSF:
                case PadroesNFSe.COPLAN:
                    versaoXML = "2.03";
                    break;

                case PadroesNFSe.PROPRIOJOINVILLESC:
                case PadroesNFSe.EL:
                case PadroesNFSe.TRIBUTUS:
                    versaoXML = "2.04";
                    break;

                case PadroesNFSe.GINFES:
                    versaoXML = "3.00";
                    break;

                case PadroesNFSe.SIGCORP_SIGISS:
                    if (codMunicipio == 3530805 || codMunicipio == 3131307)
                    {
                        versaoXML = "2.03";
                    }
                    else
                    {
                        versaoXML = "3.00";
                    }
                    break;
            }

            return versaoXML;
        }
    }
}