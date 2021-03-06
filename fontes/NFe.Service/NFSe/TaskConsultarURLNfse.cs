using NFe.Certificado;
using NFe.Components;
using NFe.Components.Conam;
using NFe.Settings;
using System;
using System.IO;

namespace NFe.Service.NFSe
{
    public class TaskNFSeConsultarURL: TaskAbst
    {
        #region Objeto com os dados do XML da consulta nfse

        /// <summary>
        /// Esta herança que deve ser utilizada fora da classe para obter os valores das tag´s da consulta nfse
        /// </summary>
        private DadosPedSitNfse oDadosPedURLNfse;

        #endregion Objeto com os dados do XML da consulta nfse

        #region Execute

        public override void Execute()
        {
            var emp = Empresas.FindEmpresaByThread();

            //Definir o serviço que será executado para a classe
            Servico = Servicos.NFSeConsultarURL;

            try
            {
                Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlRetorno + "\\" +
                                         Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.PedURLNFSe).EnvioXML) + Propriedade.ExtRetorno.Urlnfse_ERR);
                Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlErro + "\\" + NomeArquivoXML);

                oDadosPedURLNfse = new DadosPedSitNfse(emp);
                var padraoNFSe = Functions.PadraoNFSe(oDadosPedURLNfse.cMunicipio);
                var cabecMsg = "";
                WebServiceProxy wsProxy = null;
                object pedURLNfse = null;

                if (IsUtilizaCompilacaoWs(padraoNFSe, Servico, oDadosPedURLNfse.cMunicipio))
                {

                    wsProxy = ConfiguracaoApp.DefinirWS(Servico, emp, oDadosPedURLNfse.cMunicipio, oDadosPedURLNfse.tpAmb, oDadosPedURLNfse.tpEmis, padraoNFSe, oDadosPedURLNfse.cMunicipio);
                    pedURLNfse = wsProxy.CriarObjeto(wsProxy.NomeClasseWS);
                }

                switch(padraoNFSe)
                {
                    case PadroesNFSe.PUBLIC_SOFT:
                        break;

                    case PadroesNFSe.CONAM:
                        var conam = new Conam((TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                        Empresas.Configuracoes[emp].PastaXmlRetorno,
                        oDadosPedURLNfse.cMunicipio,
                        Empresas.Configuracoes[emp].UsuarioWS,
                        Empresas.Configuracoes[emp].SenhaWS);

                        conam.ConsultarURLNfse(NomeArquivoXML);
                        break;
                }

                if (IsInvocar(padraoNFSe, Servico, oDadosPedURLNfse.cMunicipio))
                {

                    var securityProtocolType = WebServiceProxy.DefinirProtocoloSeguranca(oDadosPedURLNfse.cMunicipio, oDadosPedURLNfse.tpAmb, oDadosPedURLNfse.tpEmis, padraoNFSe, Servico);

                    //Assinar o XML
                    var ad = new AssinaturaDigital();
                    ad.Assinar(NomeArquivoXML, emp, oDadosPedURLNfse.cMunicipio);

                    //Invocar o método que envia o XML para o SEFAZ
                    oInvocarObj.InvocarNFSe(wsProxy, pedURLNfse, NomeMetodoWS(Servico, oDadosPedURLNfse.cMunicipio), cabecMsg, this,
                                            Propriedade.Extensao(Propriedade.TipoEnvio.PedURLNFSe).EnvioXML,    //"-ped-urlnfse",
                                            Propriedade.Extensao(Propriedade.TipoEnvio.PedURLNFSe).RetornoXML,  //"-urlnfse",
                                            padraoNFSe, Servico, securityProtocolType);

                    ///
                    /// grava o arquivo no FTP
                    var filenameFTP = Path.Combine(Empresas.Configuracoes[emp].PastaXmlRetorno,
                                                    Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.PedURLNFSe).EnvioXML) +
                                                    Propriedade.Extensao(Propriedade.TipoEnvio.PedURLNFSe).RetornoXML);
                    if (File.Exists(filenameFTP))
                    {
                        new GerarXML(emp).XmlParaFTP(emp, filenameFTP);
                    }
                }
            }
            catch(Exception ex)
            {
                try
                {
                    //Gravar o arquivo de erro de retorno para o ERP, caso ocorra
                    TFunctions.GravarArqErroServico(NomeArquivoXML,
                                        Propriedade.Extensao(Propriedade.TipoEnvio.PedURLNFSe).EnvioXML,
                                        Propriedade.ExtRetorno.Urlnfse_ERR, ex);
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
    }
}