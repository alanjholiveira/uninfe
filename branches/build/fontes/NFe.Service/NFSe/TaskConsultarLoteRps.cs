﻿using NFe.Certificado;
using NFe.Components;
using NFe.Components.Conam;
using NFe.Components.Coplan;
using NFe.Components.EGoverne;
using NFe.Components.EL;
using NFe.Components.Elotech;
using NFe.Components.Fiorilli;
using NFe.Components.FISSLEX;
using NFe.Components.GeisWeb;
using NFe.Components.GovDigital;
using NFe.Components.Memory;
using NFe.Components.Metropolis;
using NFe.Components.Pronin;
using NFe.Components.SigCorp;
using NFe.Components.Simple;
using NFe.Components.SimplISS;
using NFe.Components.Tinus;
using NFe.Settings;
using System;
using System.IO;
using System.ServiceModel;
using System.Xml;
using static NFe.Components.Security.SOAPSecurity;

namespace NFe.Service.NFSe
{
    /// <summary>
    /// Consultar uma NFS-e por RPS
    /// </summary>
    public class TaskNFSeConsultarLoteRps: TaskAbst
    {
        public override void Execute()
        {
            var emp = Empresas.FindEmpresaByThread();

            ///
            /// extensao permitida: PedLoteRps = "-ped-loterps.xml";
            ///
            /// Definir o serviço que será executado para a classe
            Servico = Servicos.NFSeConsultarLoteRps;

            try
            {
                Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlRetorno + "\\" +
                                         Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.PedLoteRps).EnvioXML) + Propriedade.ExtRetorno.LoteRps_ERR);
                Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlErro + "\\" + NomeArquivoXML);

                var ler = new LerXML();
                ler.PedSitNfseRps(NomeArquivoXML);
                var padraoNFSe = Functions.PadraoNFSe(ler.oDadosPedSitNfseRps.cMunicipio);

                switch(padraoNFSe)
                {
                    case PadroesNFSe.PRODATA:
                    case PadroesNFSe.BETHA:
                    case PadroesNFSe.NOTAINTELIGENTE:
                    case PadroesNFSe.AVMB_ASTEN:
                    case PadroesNFSe.TRIBUTUS:
                        ExecuteDLL(emp, ler.oDadosPedSitNfseRps.cMunicipio, padraoNFSe);
                        break;

                    default:

                        switch(ler.oDadosPedSitNfseRps.cMunicipio)
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
                            case 2933307: //Vitória da Conquista-BA
                            case 3201209: //Cachoeiro de Itapemirim
                            case 3506003: //Bauru-SP
                            case 2925303: //Porto Seguro-BA
                            case 3131307: //Ipatinga-MG
                            case 3106200: //Belo Horizonte-MG
                                ExecuteDLL(emp, ler.oDadosPedSitNfseRps.cMunicipio, padraoNFSe);
                                break;

                            default:
                                WebServiceProxy wsProxy = null;
                                object pedLoteRps = null;

                                if(IsUtilizaCompilacaoWs(padraoNFSe, Servico, ler.oDadosPedSitNfseRps.cMunicipio))
                                {
                                    wsProxy = ConfiguracaoApp.DefinirWS(Servico, emp, ler.oDadosPedSitNfseRps.cMunicipio, ler.oDadosPedSitNfseRps.tpAmb, ler.oDadosPedSitNfseRps.tpEmis, padraoNFSe, ler.oDadosPedSitNfseRps.cMunicipio);
                                    if(wsProxy != null)
                                    {
                                        pedLoteRps = wsProxy.CriarObjeto(wsProxy.NomeClasseWS);
                                    }
                                }

                                var securityProtocolType = WebServiceProxy.DefinirProtocoloSeguranca(ler.oDadosPedSitNfseRps.cMunicipio, ler.oDadosPedSitNfseRps.tpAmb, ler.oDadosPedSitNfseRps.tpEmis, padraoNFSe, Servico);

                                var cabecMsg = "";
                                switch(padraoNFSe)
                                {
                                    case PadroesNFSe.GINFES:
                                        switch(ler.oDadosPedSitNfseRps.cMunicipio)
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
                                        break;

                                    case PadroesNFSe.WEBISS:
                                        cabecMsg = "<cabecalho xmlns=\"http://www.abrasf.org.br/nfse.xsd\" versao=\"1.00\"><versaoDados >1.00</versaoDados ></cabecalho>";
                                        break;

                                    case PadroesNFSe.WEBISS_202:
                                        cabecMsg = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><cabecalho xmlns=\"http://www.abrasf.org.br/nfse.xsd\" versao=\"2.02\"><versaoDados>2.02</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.PORTOVELHENSE:
                                        cabecMsg = "<cabecalho versao=\"2.00\" xmlns:ns2=\"http://www.w3.org/2000/09/xmldsig#\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.00</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.TECNOSISTEMAS:
                                        cabecMsg = "<?xml version=\"1.0\" encoding=\"utf-8\"?><cabecalho xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" versao=\"20.01\" xmlns=\"http://www.nfse-tecnos.com.br/nfse.xsd\"><versaoDados>20.01</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.FINTEL:
                                        cabecMsg = "<cabecalho versao=\"2.02\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.02</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.SIGCORP_SIGISS:
                                        var sigcorp = new SigCorp((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                        Convert.ToInt32(ler.oDadosPedSitNfseRps.cMunicipio));
                                        sigcorp.ConsultarLoteRps(NomeArquivoXML);
                                        break;

                                    case PadroesNFSe.SIGCORP_SIGISS_203:
                                        cabecMsg = "<cabecalho versao=\"2.03\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.03</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.FIORILLI:
                                        var fiorilli = new Fiorilli((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                        ler.oDadosPedSitNfseRps.cMunicipio,
                                                                        Empresas.Configuracoes[emp].UsuarioWS,
                                                                        Empresas.Configuracoes[emp].SenhaWS,
                                                                        ConfiguracaoApp.ProxyUsuario,
                                                                        ConfiguracaoApp.ProxySenha,
                                                                        ConfiguracaoApp.ProxyServidor,
                                                                        Empresas.Configuracoes[emp].X509Certificado);

                                        fiorilli.ConsultarLoteRps(NomeArquivoXML);
                                        break;

                                    case PadroesNFSe.SIMPLISS:
                                        var simpliss = new SimplISS((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                ler.oDadosPedSitNfseRps.cMunicipio,
                                                                Empresas.Configuracoes[emp].UsuarioWS,
                                                                Empresas.Configuracoes[emp].SenhaWS,
                                                                ConfiguracaoApp.ProxyUsuario,
                                                                ConfiguracaoApp.ProxySenha,
                                                                ConfiguracaoApp.ProxyServidor);

                                        simpliss.ConsultarLoteRps(NomeArquivoXML);
                                        break;

                                    case PadroesNFSe.CONAM:
                                        var conam = new Conam((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                ler.oDadosPedSitNfseRps.cMunicipio,
                                                                Empresas.Configuracoes[emp].UsuarioWS,
                                                                Empresas.Configuracoes[emp].SenhaWS);

                                        conam.ConsultarLoteRps(NomeArquivoXML);
                                        break;

                                    case PadroesNFSe.EGOVERNE:

                                        #region E-Governe

                                        var egoverne = new EGoverne((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                        ler.oDadosPedSitNfseRps.cMunicipio,
                                                                        ConfiguracaoApp.ProxyUsuario,
                                                                        ConfiguracaoApp.ProxySenha,
                                                                        ConfiguracaoApp.ProxyServidor,
                                                                        Empresas.Configuracoes[emp].X509Certificado);

                                        var assegov = new AssinaturaDigital();
                                        assegov.Assinar(NomeArquivoXML, emp, ler.oDadosPedSitNfseRps.cMunicipio);

                                        egoverne.ConsultarLoteRps(NomeArquivoXML);
                                        break;

                                    #endregion E-Governe

                                    case PadroesNFSe.EL:

                                        #region E&L

                                        var el = new EL((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                        ler.oDadosPedSitNfseRps.cMunicipio,
                                                        Empresas.Configuracoes[emp].UsuarioWS,
                                                        Empresas.Configuracoes[emp].SenhaWS,
                                                        (ConfiguracaoApp.Proxy ? ConfiguracaoApp.ProxyUsuario : ""),
                                                        (ConfiguracaoApp.Proxy ? ConfiguracaoApp.ProxySenha : ""),
                                                        (ConfiguracaoApp.Proxy ? ConfiguracaoApp.ProxyServidor : ""));

                                        el.ConsultarLoteRps(NomeArquivoXML);
                                        break;

                                    #endregion E&L

                                    case PadroesNFSe.GOVDIGITAL:
                                        var govdig = new GovDigital((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                            Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                            Empresas.Configuracoes[emp].X509Certificado,
                                                                            ler.oDadosPedSitNfseRps.cMunicipio,
                                                                            ConfiguracaoApp.ProxyUsuario,
                                                                            ConfiguracaoApp.ProxySenha,
                                                                            ConfiguracaoApp.ProxyServidor);

                                        var adgovdig = new AssinaturaDigital();
                                        adgovdig.Assinar(NomeArquivoXML, emp, ler.oDadosPedSitNfseRps.cMunicipio);

                                        govdig.ConsultarLoteRps(NomeArquivoXML);
                                        break;

                                    case PadroesNFSe.EQUIPLANO:
                                        cabecMsg = "1";
                                        break;

                                    case PadroesNFSe.RLZ_INFORMATICA_02:
                                        if(ler.oDadosPedSitNfseRps.cMunicipio == 5107958)
                                        {
                                            cabecMsg = "<cabecalho><versaoDados>2.02</versaoDados></cabecalho>";
                                        }

                                        break;

                                    case PadroesNFSe.PORTALFACIL_ACTCON_202:
                                        if(ler.oDadosPedSitNfseRps.cMunicipio != 3169901)
                                        {
                                            cabecMsg = "<cabecalho><versaoDados>2.02</versaoDados></cabecalho>";
                                        }

                                        break;

                                    case PadroesNFSe.PORTALFACIL_ACTCON:
                                        cabecMsg = "<cabecalho><versaoDados>2.01</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.FISSLEX:
                                        var fisslex = new FISSLEX((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                        ler.oDadosPedSitNfseRps.cMunicipio,
                                                                        Empresas.Configuracoes[emp].UsuarioWS,
                                                                        Empresas.Configuracoes[emp].SenhaWS,
                                                                        ConfiguracaoApp.ProxyUsuario,
                                                                        ConfiguracaoApp.ProxySenha,
                                                                        ConfiguracaoApp.ProxyServidor);

                                        fisslex.ConsultarLoteRps(NomeArquivoXML);
                                        break;

                                    case PadroesNFSe.NATALENSE:
                                        cabecMsg = "<cabecalho xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" versao=\"1\" xmlns=\"http://www.abrasf.org.br/ABRASF/arquivos/nfse.xsd\"><versaoDados>1</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.PAULISTANA:
                                        wsProxy = new WebServiceProxy(Empresas.Configuracoes[emp].X509Certificado);

                                        if(ler.oDadosPedSitNfseRps.tpAmb == 1)
                                        {
                                            pedLoteRps = new NFe.Components.PSaoPauloSP.LoteNFe();
                                        }
                                        else
                                        {
                                            throw new Exception("Município de São Paulo-SP não dispõe de ambiente de homologação para envio de NFS-e em teste.");
                                        }
                                        break;

                                    case PadroesNFSe.MEMORY:

                                        #region Memory

                                        var memory = new Memory((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                        ler.oDadosPedSitNfseRps.cMunicipio,
                                        Empresas.Configuracoes[emp].UsuarioWS,
                                        Empresas.Configuracoes[emp].SenhaWS,
                                        ConfiguracaoApp.ProxyUsuario,
                                        ConfiguracaoApp.ProxySenha,
                                        ConfiguracaoApp.ProxyServidor);

                                        memory.ConsultarLoteRps(NomeArquivoXML);
                                        break;

                                    #endregion Memory

                                    case PadroesNFSe.CAMACARI_BA:
                                        cabecMsg = "<cabecalho><versaoDados>2.01</versaoDados><versao>2.01</versao></cabecalho>";
                                        break;

                                    case PadroesNFSe.NA_INFORMATICA:
                                        wsProxy = new WebServiceProxy(Empresas.Configuracoes[emp].X509Certificado);

                                        //if (ler.oDadosPedSitNfseRps.tpAmb == 1)
                                        //    pedLoteRps = new Components.PCorumbaMS.NfseWSService();
                                        //else
                                        //    pedLoteRps = new Components.HCorumbaMS.NfseWSService();

                                        break;

                                    case PadroesNFSe.METROPOLIS:

                                        #region METROPOLIS

                                        var metropolis = new Metropolis((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                      Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                      ler.oDadosPedSitNfseRps.cMunicipio,
                                                                      ConfiguracaoApp.ProxyUsuario,
                                                                      ConfiguracaoApp.ProxySenha,
                                                                      ConfiguracaoApp.ProxyServidor,
                                                                      Empresas.Configuracoes[emp].X509Certificado);

                                        var metropolisdig = new AssinaturaDigital();
                                        metropolisdig.Assinar(NomeArquivoXML, emp, ler.oDadosPedSitNfseRps.cMunicipio);

                                        metropolis.ConsultarLoteRps(NomeArquivoXML);
                                        break;

                                    #endregion METROPOLIS

                                    case PadroesNFSe.BSITBR:
                                        wsProxy = new WebServiceProxy(Empresas.Configuracoes[emp].X509Certificado);

                                        if(ler.oDadosPedSitNfseRps.tpAmb == 1)
                                        {
                                            switch(ler.oDadosPedSitNfseRps.cMunicipio)
                                            {
                                                case 5211800:
                                                    pedLoteRps = new Components.PJaraguaGO.nfseWS();
                                                    break;

                                                case 5220454:
                                                    pedLoteRps = new Components.PSenadorCanedoGO.nfseWS();
                                                    break;

                                                case 3507506:
                                                    pedLoteRps = new Components.PBotucatuSP.nfseWS();
                                                    break;

                                                case 5211909:
                                                    pedLoteRps = new Components.PJataiGO.nfseWS();
                                                    break;

                                                case 5220603:
                                                    pedLoteRps = new Components.PSilvaniaGO.nfseWS();
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Este município não dispõe de ambiente de homologação para envio de NFS-e em teste.");
                                        }

                                        break;


                                    case PadroesNFSe.PRONIN:
                                        if(ler.oDadosPedSitNfseRps.cMunicipio == 3131703 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 4303004 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 4322509 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 3556602 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 3512803 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 4323002 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 3505807 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 3530300 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 4308904 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 4118501 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 3554300 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 3542404 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 5005707 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 4314423 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 3511102 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 3535804 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 4306932 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 4322400 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 4302808 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 3501301 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 4300109 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 4124053 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 3550407 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 4310207 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 1502400 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 4301057 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 4115804 ||
                                            ler.oDadosPedSitNfseRps.cMunicipio == 3550803)

                                        {
                                            var pronin = new Pronin((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                ler.oDadosPedSitNfseRps.cMunicipio,
                                                ConfiguracaoApp.ProxyUsuario,
                                                ConfiguracaoApp.ProxySenha,
                                                ConfiguracaoApp.ProxyServidor,
                                                Empresas.Configuracoes[emp].X509Certificado);

                                            var assPronin = new AssinaturaDigital();
                                            assPronin.Assinar(NomeArquivoXML, emp, ler.oDadosPedSitNfseRps.cMunicipio);

                                            pronin.ConsultarLoteRps(NomeArquivoXML);
                                        }
                                        break;

                                    case PadroesNFSe.COPLAN:
                                        var coplan = new Coplan((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                            Empresas.Configuracoes[emp].PastaXmlRetorno,
                                            ler.oDadosPedSitNfseRps.cMunicipio,
                                            ConfiguracaoApp.ProxyUsuario,
                                            ConfiguracaoApp.ProxySenha,
                                            ConfiguracaoApp.ProxyServidor,
                                            Empresas.Configuracoes[emp].X509Certificado);

                                        var assCoplan = new AssinaturaDigital();
                                        assCoplan.Assinar(NomeArquivoXML, emp, ler.oDadosPedSitNfseRps.cMunicipio);

                                        coplan.ConsultarLoteRps(NomeArquivoXML);
                                        break;

                                    case PadroesNFSe.TINUS:
                                        var tinus = new Tinus((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                            Empresas.Configuracoes[emp].PastaXmlRetorno,
                                            ler.oDadosPedSitNfseRps.cMunicipio,
                                            ConfiguracaoApp.ProxyUsuario,
                                            ConfiguracaoApp.ProxySenha,
                                            ConfiguracaoApp.ProxyServidor,
                                            Empresas.Configuracoes[emp].X509Certificado);

                                        tinus.ConsultarLoteRps(NomeArquivoXML);
                                        break;

                                    case PadroesNFSe.GEISWEB:
                                        var geisWeb = new GeisWeb((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                            Empresas.Configuracoes[emp].PastaXmlRetorno,
                                            ler.oDadosPedSitNfseRps.cMunicipio,
                                            ConfiguracaoApp.ProxyUsuario,
                                            ConfiguracaoApp.ProxySenha,
                                            ConfiguracaoApp.ProxyServidor,
                                            Empresas.Configuracoes[emp].X509Certificado);

                                        geisWeb.ConsultarLoteRps(NomeArquivoXML);
                                        break;


                                    case PadroesNFSe.SH3:
                                        cabecMsg = "<cabecalho xmlns=\"http://www.abrasf.org.br/nfse.xsd\" versao=\"1.00\"><versaoDados >1.00</versaoDados ></cabecalho>";
                                        break;

                                    case PadroesNFSe.INTERSOL:
                                        cabecMsg = "<?xml version=\"1.0\" encoding=\"utf-8\"?><p:cabecalho versao=\"1\" xmlns:ds=\"http://www.w3.org/2000/09/xmldsig#\" xmlns:p=\"http://ws.speedgov.com.br/cabecalho_v1.xsd\" xmlns:p1=\"http://ws.speedgov.com.br/tipos_v1.xsd\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://ws.speedgov.com.br/cabecalho_v1.xsd cabecalho_v1.xsd \"><versaoDados>1</versaoDados></p:cabecalho>";
                                        break;

                                    case PadroesNFSe.MANAUS_AM:
                                        cabecMsg = "<cabecalho versao=\"201001\"><versaoDados>V2010</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.EMBRAS:
                                        cabecMsg = "<cabecalho versao=\"2.02\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.02</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.MODERNIZACAO_PUBLICA:
                                        cabecMsg = "<cabecalho xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\" versao=\"2.02\"><versaoDados>2.02</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.E_RECEITA:
                                        cabecMsg = "<cabecalho xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\" versao=\"2.02\"><versaoDados>2.02</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.TIPLAN_203:
                                    case PadroesNFSe.INDAIATUBA_SP:
                                        cabecMsg = "<cabecalho versao=\"2.03\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.03</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.ADM_SISTEMAS:
                                        cabecMsg = "<cabecalho versao=\"2.01\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.01</versaoDados></cabecalho>";
                                        wsProxy = new WebServiceProxy(Empresas.Configuracoes[emp].X509Certificado);

                                        pedLoteRps = ler.oDadosPedSitNfseRps.tpAmb == 1 ?
                                                        new Components.PAmargosaBA.InfseClient(GetBinding(), new EndpointAddress("https://demo.saatri.com.br/servicos/nfse.svc")) :
                                                        new Components.HAmargosaBA.InfseClient(GetBinding(), new EndpointAddress("https://homologa-demo.saatri.com.br/servicos/nfse.svc")) as object;

                                        SignUsingCredentials(emp, pedLoteRps);
                                        break;

                                    case PadroesNFSe.SIMPLE:

                                        var simple = new Simple((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                                                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                        ler.oDadosPedSitNfseRps.cMunicipio,
                                                                        ConfiguracaoApp.ProxyUsuario,
                                                                        ConfiguracaoApp.ProxySenha,
                                                                        ConfiguracaoApp.ProxyServidor,
                                                                        Empresas.Configuracoes[emp].X509Certificado);

                                        simple.ConsultarNfsePorRps(NomeArquivoXML);
                                        break;

                                    case PadroesNFSe.SISPMJP:
                                        cabecMsg = "<cabecalho versao=\"2.02\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\" ><versaoDados>2.02</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.SMARAPD_204:
                                        cabecMsg = "<cabecalho versao=\"2.04\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>2.04</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.DSF:
                                        if(ler.oDadosPedSitNfseRps.cMunicipio == 3549904)
                                        {
                                            cabecMsg = "<cabecalho versao=\"3\" xmlns=\"http://www.abrasf.org.br/nfse.xsd\"><versaoDados>3</versaoDados></cabecalho>";
                                        }
                                        break;

                                    case PadroesNFSe.IIBRASIL:
                                        cabecMsg = "<cabecalho xmlns=\"http://www.abrasf.org.br/nfse.xsd\" versao=\"2.04\"><versaoDados>2.04</versaoDados></cabecalho>";
                                        break;

                                    case PadroesNFSe.ELOTECH:
                                        var elotech = new Elotech((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                                            Empresas.Configuracoes[emp].PastaXmlRetorno,
                                            ler.oDadosPedSitNfseRps.cMunicipio,
                                            ConfiguracaoApp.ProxyUsuario,
                                            ConfiguracaoApp.ProxySenha,
                                            ConfiguracaoApp.ProxyServidor,
                                            Empresas.Configuracoes[emp].X509Certificado);

                                        elotech.ConsultarLoteRps(NomeArquivoXML);
                                        break;
                                }

                                if(base.IsInvocar(padraoNFSe, Servico, ler.oDadosPedSitNfseRps.cMunicipio))
                                {
                                    //Assinar o XML
                                    var ad = new AssinaturaDigital();
                                    ad.Assinar(NomeArquivoXML, emp, ler.oDadosPedSitNfseRps.cMunicipio);

                                    //Invocar o método que envia o XML para o SEFAZ
                                    oInvocarObj.InvocarNFSe(wsProxy, pedLoteRps, NomeMetodoWS(Servico, ler.oDadosPedSitNfseRps.cMunicipio),
                                                            cabecMsg, this,
                                                            Propriedade.Extensao(Propriedade.TipoEnvio.PedLoteRps).EnvioXML,   //"-ped-loterps",
                                                            Propriedade.Extensao(Propriedade.TipoEnvio.PedLoteRps).RetornoXML, //"-loterps",
                                                            padraoNFSe, Servico, securityProtocolType);

                                    ///
                                    /// grava o arquivo no FTP
                                    var filenameFTP = Path.Combine(Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                                      Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.PedLoteRps).EnvioXML) + Propriedade.Extensao(Propriedade.TipoEnvio.PedLoteRps).RetornoXML);
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
                    TFunctions.GravarArqErroServico(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.PedLoteRps).EnvioXML, Propriedade.ExtRetorno.LoteRps_ERR, ex);
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

            var finalArqEnvio = Propriedade.Extensao(Propriedade.TipoEnvio.PedLoteRps).EnvioXML;
            var finalArqRetorno = Propriedade.Extensao(Propriedade.TipoEnvio.PedLoteRps).RetornoXML;
            var servico = DefinirServico(municipio, conteudoXML);
            var versaoXML = DefinirVersaoXML(municipio, conteudoXML, padraoNFSe);

            Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlRetorno + "\\" + Functions.ExtrairNomeArq(NomeArquivoXML, finalArqEnvio) + Functions.ExtractExtension(finalArqRetorno) + ".err");

            var configuracao = new Unimake.Business.DFe.Servicos.Configuracao
            {
                TipoDFe = Unimake.Business.DFe.Servicos.TipoDFe.NFSe,
                CertificadoDigital = Empresas.Configuracoes[emp].X509Certificado,
                TipoAmbiente = (Unimake.Business.DFe.Servicos.TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                CodigoMunicipio = municipio,
                Servico = servico,
                SchemaVersao = versaoXML
            };

            switch(servico)
            {
                case Unimake.Business.DFe.Servicos.Servico.NFSeConsultarNotaValida:
                    var consultarNotaValida = new Unimake.Business.DFe.Servicos.NFSe.ConsultarNotaValida(conteudoXML, configuracao);
                    consultarNotaValida.Executar();

                    vStrXmlRetorno = consultarNotaValida.RetornoWSString;
                    break;

                case Unimake.Business.DFe.Servicos.Servico.NFSeConsultarLoteRps:
                    var consultarLoteRps = new Unimake.Business.DFe.Servicos.NFSe.ConsultarLoteRps(conteudoXML, configuracao);
                    consultarLoteRps.Executar();

                    vStrXmlRetorno = consultarLoteRps.RetornoWSString;
                    break;
            }

            XmlRetorno(finalArqEnvio, finalArqRetorno);

            /// grava o arquivo no FTP
            var filenameFTP = Path.Combine(Empresas.Configuracoes[emp].PastaXmlRetorno,
                Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.PedLoteRps).EnvioXML) + Propriedade.Extensao(Propriedade.TipoEnvio.PedLoteRps).RetornoXML);

            if(File.Exists(filenameFTP))
            {
                new GerarXML(emp).XmlParaFTP(emp, filenameFTP);
            }
        }

        private Unimake.Business.DFe.Servicos.Servico DefinirServico(int municipio, XmlDocument doc)
        {
            var result = Unimake.Business.DFe.Servicos.Servico.NFSeConsultarLoteRps;

            var padraoNFSe = Functions.PadraoNFSe(municipio);

            switch(padraoNFSe)
            {

                case PadroesNFSe.SIGCORP_SIGISS:
                    switch (doc.DocumentElement.Name)
                    {
                        case "ConsultarNotaValida":
                            result = Unimake.Business.DFe.Servicos.Servico.NFSeConsultarNotaValida;
                            break;
                        case "ConsultarLoteRps":
                            result = Unimake.Business.DFe.Servicos.Servico.NFSeConsultarLoteRps;
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
                case PadroesNFSe.BETHA:
                    versaoXML = "2.02";

                    if(xmlDoc.DocumentElement.Name.Contains("e:"))
                    {
                        versaoXML = "1.00";
                    }
                    break;

                case PadroesNFSe.NOBESISTEMAS:
                case PadroesNFSe.BHISS:
                    versaoXML = "1.00";
                    break;

                case PadroesNFSe.PRODATA:
                    versaoXML = "2.01";
                    break;

                case PadroesNFSe.NOTAINTELIGENTE:
                case PadroesNFSe.AVMB_ASTEN:
                case PadroesNFSe.WEBISS:
                case PadroesNFSe.COPLAN:
                    versaoXML = "2.02";
                    break;

                case PadroesNFSe.SIGCORP_SIGISS:
                case PadroesNFSe.SIMPLISS:
                case PadroesNFSe.SMARAPD:
                    versaoXML = "2.03";
                    break;

                case PadroesNFSe.PROPRIOJOINVILLESC:
                case PadroesNFSe.EL:
                case PadroesNFSe.TRIBUTUS:
                    versaoXML = "2.04";
                    break;
            }

            return versaoXML;
        }
    }
}