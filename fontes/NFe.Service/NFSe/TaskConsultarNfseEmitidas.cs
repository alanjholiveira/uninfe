using NFe.Certificado;
using NFe.Components;
using NFe.Settings;
using System;
using System.IO;
using System.Xml;

namespace NFe.Service.NFSe
{
    public class TaskConsultarNfseEmitidas : TaskAbst
    {
        public TaskConsultarNfseEmitidas(string arquivo)
        {
            Servico = Servicos.NFSeConsultarNFSeEmitidas;
            NomeArquivoXML = arquivo;
        }

        #region Objeto com os dados do XML da consulta nfse

        /// <summary>
        /// Esta herança que deve ser utilizada fora da classe para obter os valores das tag´s da consulta nfse
        /// </summary>
        private DadosPedNFSeEmit oDadosPedNFSeEmit;

        #endregion Objeto com os dados do XML da consulta nfse

        #region Execute

        public override void Execute()
        {
            var emp = Empresas.FindEmpresaByThread();

            try
            {
                Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlRetorno + "\\" +
                    Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.PedNFSeEmit).EnvioXML) + Propriedade.Extensao(Propriedade.TipoEnvio.PedNFSeEmit).RetornoERR);
                Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlErro + "\\" + NomeArquivoXML);

                oDadosPedNFSeEmit = new DadosPedNFSeEmit(emp);

                //Criar objetos das classes dos serviços dos webservices do SEFAZ
                var padraoNFSe = Functions.PadraoNFSe(oDadosPedNFSeEmit.cMunicipio);

                switch (oDadosPedNFSeEmit.cMunicipio)
                {
                    case 3550308: //São Paulo-SP
                        ExecuteDLL(emp, oDadosPedNFSeEmit.cMunicipio, padraoNFSe);
                        break;
                 
                        WebServiceProxy wsProxy = null;
                        object pedConsNfseRecebidas = null;

                        if (IsUtilizaCompilacaoWs(padraoNFSe))
                        {
                            wsProxy = ConfiguracaoApp.DefinirWS(Servico, emp, oDadosPedNFSeEmit.cMunicipio, oDadosPedNFSeEmit.tpAmb, oDadosPedNFSeEmit.tpEmis, padraoNFSe, oDadosPedNFSeEmit.cMunicipio);
                            pedConsNfseRecebidas = wsProxy.CriarObjeto(wsProxy.NomeClasseWS);
                        }
                        var cabecMsg = "";

                        var securityProtocolType = WebServiceProxy.DefinirProtocoloSeguranca(oDadosPedNFSeEmit.cMunicipio, oDadosPedNFSeEmit.tpAmb, oDadosPedNFSeEmit.tpEmis, padraoNFSe, Servico);

                        //Assinar o XML
                        var ad = new AssinaturaDigital();
                        ad.Assinar(NomeArquivoXML, emp, oDadosPedNFSeEmit.cMunicipio);

                        //Invocar o método que envia o XML para o SEFAZ
                        oInvocarObj.InvocarNFSe(wsProxy, pedConsNfseRecebidas, NomeMetodoWS(Servico, oDadosPedNFSeEmit.cMunicipio), cabecMsg, this,
                            Propriedade.Extensao(Propriedade.TipoEnvio.PedNFSeEmit).EnvioXML,
                            Propriedade.Extensao(Propriedade.TipoEnvio.PedNFSeEmit).RetornoXML,
                            padraoNFSe, Servico, securityProtocolType);

                        ///
                        /// grava o arquivo no FTP
                        var filenameFTP = Path.Combine(Empresas.Configuracoes[emp].PastaXmlRetorno,
                            Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.PedNFSeEmit).EnvioXML) +
                            Propriedade.Extensao(Propriedade.TipoEnvio.PedNFSeEmit).RetornoXML);

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
                        Propriedade.Extensao(Propriedade.TipoEnvio.PedNFSeEmit).EnvioXML,
                        Propriedade.Extensao(Propriedade.TipoEnvio.PedNFSeEmit).RetornoERR, ex);
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

            var finalArqEnvio = Propriedade.Extensao(Propriedade.TipoEnvio.PedNFSeEmit).EnvioXML;
            var finalArqRetorno = Propriedade.Extensao(Propriedade.TipoEnvio.PedNFSeEmit).RetornoXML;
            var versaoXML = DefinirVersaoXML(municipio, conteudoXML, padraoNFSe);

            Functions.DeletarArquivo(Empresas.Configuracoes[emp].PastaXmlRetorno + "\\" + Functions.ExtrairNomeArq(NomeArquivoXML, finalArqEnvio) + Functions.ExtractExtension(finalArqRetorno) + ".err");

            var configuracao = new Unimake.Business.DFe.Servicos.Configuracao
            {
                TipoDFe = Unimake.Business.DFe.Servicos.TipoDFe.NFSe,
                CertificadoDigital = Empresas.Configuracoes[emp].X509Certificado,
                TipoAmbiente = (Unimake.Business.DFe.Servicos.TipoAmbiente)Empresas.Configuracoes[emp].AmbienteCodigo,
                CodigoMunicipio = municipio,
                Servico = Unimake.Business.DFe.Servicos.Servico.NFSeConsultaNFeEmitidas,
                SchemaVersao = versaoXML
            };

            var consultaNFeEmitidas = new Unimake.Business.DFe.Servicos.NFSe.ConsultaNFeEmitidas(conteudoXML, configuracao);
            consultaNFeEmitidas.Executar();

            vStrXmlRetorno = consultaNFeEmitidas.RetornoWSString;

            XmlRetorno(finalArqEnvio, finalArqRetorno);

            /// grava o arquivo no FTP
            var filenameFTP = Path.Combine(Empresas.Configuracoes[emp].PastaXmlRetorno,
                Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.PedNFSeEmit).EnvioXML) + Propriedade.Extensao(Propriedade.TipoEnvio.PedNFSeEmit).RetornoXML);

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
                case PadroesNFSe.PAULISTANA:
                    versaoXML = "2.00";
                    break;
            }

            return versaoXML;
        }
    }
}