using NFe.Certificado;
using NFe.Components;
using NFe.Settings;
using System;
using System.IO;
using System.Xml;

namespace NFe.Service.NFSe
{
    public class TaskConsultarNfseRecebidas: TaskAbst
    {
        public TaskConsultarNfseRecebidas(string arquivo)
        {
            Servico = Servicos.NFSeConsultarNFSeRecebidas;
            NomeArquivoXML = arquivo;
        }

        #region Objeto com os dados do XML da consulta nfse

        /// <summary>
        /// Esta herança que deve ser utilizada fora da classe para obter os valores das tag´s da consulta nfse
        /// </summary>
        private DadosPedSitNfse oDadosPedSitNfse;

        #endregion Objeto com os dados do XML da consulta nfse

        #region Execute

        public override void Execute()
        {
            var emp = Empresas.FindEmpresaByThread();

            try
            {
                Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlRetorno + "\\" +
                    Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.PedSitNFSeRec).EnvioXML) + Propriedade.Extensao(Propriedade.TipoEnvio.PedSitNFSeRec).RetornoERR);
                Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlErro + "\\" + NomeArquivoXML);

                oDadosPedSitNfse = new DadosPedSitNfse(emp);

                //Criar objetos das classes dos serviços dos webservices do SEFAZ
                var padraoNFSe = Functions.PadraoNFSe(oDadosPedSitNfse.cMunicipio);

                switch (oDadosPedSitNfse.cMunicipio)
                {
                    case 3550308: //São Paulo-SP
                        ExecuteDLL(emp, oDadosPedSitNfse.cMunicipio, padraoNFSe);
                        break;
                 
                        WebServiceProxy wsProxy = null;
                        object pedConsNfseRecebidas = null;

                        if (IsUtilizaCompilacaoWs(padraoNFSe))
                        {
                            wsProxy = ConfiguracaoApp.DefinirWS(Servico, emp, oDadosPedSitNfse.cMunicipio, oDadosPedSitNfse.tpAmb, oDadosPedSitNfse.tpEmis, padraoNFSe, oDadosPedSitNfse.cMunicipio);
                            pedConsNfseRecebidas = wsProxy.CriarObjeto(wsProxy.NomeClasseWS);
                        }
                        var cabecMsg = "";

                        var securityProtocolType = WebServiceProxy.DefinirProtocoloSeguranca(oDadosPedSitNfse.cMunicipio, oDadosPedSitNfse.tpAmb, oDadosPedSitNfse.tpEmis, padraoNFSe, Servico);

                        //Assinar o XML
                        var ad = new AssinaturaDigital();
                        ad.Assinar(NomeArquivoXML, emp, oDadosPedSitNfse.cMunicipio);

                        //Invocar o método que envia o XML para o SEFAZ
                        oInvocarObj.InvocarNFSe(wsProxy, pedConsNfseRecebidas, NomeMetodoWS(Servico, oDadosPedSitNfse.cMunicipio), cabecMsg, this,
                            Propriedade.Extensao(Propriedade.TipoEnvio.PedSitNFSeRec).EnvioXML,
                            Propriedade.Extensao(Propriedade.TipoEnvio.PedSitNFSeRec).RetornoXML,
                            padraoNFSe, Servico, securityProtocolType);

                        ///
                        /// grava o arquivo no FTP
                        var filenameFTP = Path.Combine(Empresas.Configuracoes[emp].PastaXmlRetorno,
                            Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.PedSitNFSeRec).EnvioXML) +
                            Propriedade.Extensao(Propriedade.TipoEnvio.PedSitNFSeRec).RetornoXML);

                        if (File.Exists(filenameFTP))
                        {
                            new GerarXML(emp).XmlParaFTP(emp, filenameFTP);
                        }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    //Gravar o arquivo de erro de retorno para o ERP, caso ocorra
                    TFunctions.GravarArqErroServico(NomeArquivoXML,
                        Propriedade.Extensao(Propriedade.TipoEnvio.PedSitNFSeRec).EnvioXML,
                        Propriedade.Extensao(Propriedade.TipoEnvio.PedSitNFSeRec).RetornoERR, ex);
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

            var finalArqEnvio = Propriedade.Extensao(Propriedade.TipoEnvio.PedSitLoteRps).EnvioXML;
            var finalArqRetorno = Propriedade.Extensao(Propriedade.TipoEnvio.PedSitLoteRps).RetornoXML;
            var versaoXML = DefinirVersaoXML(municipio, conteudoXML, padraoNFSe);
            var servico = DefinirServico(municipio, conteudoXML);

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

            switch (servico)
            {
                case Unimake.Business.DFe.Servicos.Servico.NFSeConsultaNFeRecebidas:
                    var consultaNFeRecebidas = new Unimake.Business.DFe.Servicos.NFSe.ConsultaNFeRecebidas(conteudoXML, configuracao);
                    consultaNFeRecebidas.Executar();

                    vStrXmlRetorno = consultaNFeRecebidas.RetornoWSString;
                    break;

                case Unimake.Business.DFe.Servicos.Servico.NFSeConsultaNFeEmitidas:
                    var consultaNFeEmitidas = new Unimake.Business.DFe.Servicos.NFSe.ConsultaNFeEmitidas(conteudoXML, configuracao);
                    consultaNFeEmitidas.Executar();

                    vStrXmlRetorno = consultaNFeEmitidas.RetornoWSString;
                    break;
            }

            XmlRetorno(finalArqEnvio, finalArqRetorno);

            /// grava o arquivo no FTP
            var filenameFTP = Path.Combine(Empresas.Configuracoes[emp].PastaXmlRetorno,
                Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.PedSubstNfse).EnvioXML) + Propriedade.Extensao(Propriedade.TipoEnvio.PedSubstNfse).RetornoXML);

            if (File.Exists(filenameFTP))
            {
                new GerarXML(emp).XmlParaFTP(emp, filenameFTP);
            }
        }

        private Unimake.Business.DFe.Servicos.Servico DefinirServico(int municipio, XmlDocument doc)
        {
            var result = Unimake.Business.DFe.Servicos.Servico.NFSeConsultaNFeRecebidas;

            var padraoNFSe = Functions.PadraoNFSe(municipio);

            switch (padraoNFSe)
            {
                case PadroesNFSe.PAULISTANA:
                    switch (doc.DocumentElement.Name)
                    {
                        case "ConsultaNFeEmitidas":
                            result = Unimake.Business.DFe.Servicos.Servico.NFSeConsultaNFeEmitidas;
                            break;
                        case "ConsultaNFeRecebidas":
                            result = Unimake.Business.DFe.Servicos.Servico.NFSeConsultaNFeRecebidas;
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

            switch (padraoNFSe)
            {
                case PadroesNFSe.PAULISTANA:
                    versaoXML = "2.00";
                    break;
            }

            return versaoXML;
        }
    }
}