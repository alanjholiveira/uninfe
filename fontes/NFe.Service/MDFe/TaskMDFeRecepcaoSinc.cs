using NFe.Components;
using NFe.Exceptions;
using NFe.Settings;
using System;
using System.IO;
using System.Xml;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Xml.MDFe;
using Unimake.Security.Exceptions;

namespace NFe.Service
{
    public class TaskMDFeRecepcaoSinc: TaskAbst
    {
        public TaskMDFeRecepcaoSinc(string arquivo)
        {
            Servico = Servicos.MDFeEnviarSinc;

            NomeArquivoXML = arquivo;
            ConteudoXML.PreserveWhitespace = false;
            ConteudoXML.Load(arquivo);
        }

        #region Execute

        public override void Execute()
        {
            var emp = Empresas.FindEmpresaByThread();

            try
            {
                var xmlMDFe = new MDFe();
                xmlMDFe = Unimake.Business.DFe.Utility.XMLUtility.Deserializar<MDFe>(ConteudoXML);

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.MDFe,
                    TipoEmissao = Unimake.Business.DFe.Servicos.TipoEmissao.Normal,
                    CertificadoDigital = Empresas.Configuracoes[emp].X509Certificado
                };

                if(ConfiguracaoApp.Proxy)
                {
                    configuracao.HasProxy = true;
                    configuracao.ProxyAutoDetect = ConfiguracaoApp.DetectarConfiguracaoProxyAuto;
                    configuracao.ProxyUser = ConfiguracaoApp.ProxyUsuario;
                    configuracao.ProxyPassword = ConfiguracaoApp.ProxySenha;
                }

                var autorizacaoSinc = new Unimake.Business.DFe.Servicos.MDFe.AutorizacaoSinc(xmlMDFe, configuracao);
                autorizacaoSinc.Executar();

                ConteudoXML = autorizacaoSinc.ConteudoXMLAssinado;

                SalvarArquivoEmProcessamento(emp);

                vStrXmlRetorno = autorizacaoSinc.RetornoWSString;

                if(autorizacaoSinc.Result.CStat == 104)
                {
                    FinalizarNFeSincrono(vStrXmlRetorno, emp, xmlMDFe.InfMDFe.Chave);

                    oGerarXML.XmlRetorno(Propriedade.Extensao(Propriedade.TipoEnvio.MDFe).EnvioXML, Propriedade.Extensao(Propriedade.TipoEnvio.PedRec).RetornoXML, vStrXmlRetorno);
                }
                else if(autorizacaoSinc.Result.CStat > 200 ||
                    autorizacaoSinc.Result.CStat == 108 || //Verifica se o servidor de processamento está paralisado momentaneamente. Wandrey 13/04/2012
                    autorizacaoSinc.Result.CStat == 109) //Verifica se o servidor de processamento está paralisado sem previsão. Wandrey 13/04/2012
                {
                    //Se o status do retorno do lote for maior que 200 ou for igual a 108 ou 109,
                    //vamos ter que excluir a nota do fluxo, porque ela foi rejeitada pelo SEFAZ
                    //Primeiro vamos mover o xml da nota da pasta EmProcessamento para pasta de XML´s com erro e depois tira ela do fluxo
                    //Wandrey 30/04/2009
                    oAux.MoveArqErro(Empresas.Configuracoes[emp].PastaXmlEnviado + "\\" + PastaEnviados.EmProcessamento.ToString() + "\\" + (new FileInfo(NomeArquivoXML).Name));
                }
            }
            catch(ValidarXMLException ex)
            {
                try
                {
                    //Gravar o arquivo de erro de retorno para o ERP, caso ocorra
                    TFunctions.GravarArqErroServico(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.MDFe).EnvioXML, Propriedade.ExtRetorno.ProRec_ERR, ex);
                }
                catch { }
            }
            catch(ExceptionSemInternet ex)
            {
                try
                {
                    //Gravar o arquivo de erro de retorno para o ERP, caso ocorra
                    TFunctions.GravarArqErroServico(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.MDFe).EnvioXML, Propriedade.ExtRetorno.ProRec_ERR, ex);
                }
                catch { }
            }
            catch(Exception ex)
            {
                try
                {
                    //Gravar o arquivo de erro de retorno para o ERP, caso ocorra
                    if(File.Exists(NomeArquivoXML))
                    {
                        TFunctions.GravarArqErroServico(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.MDFe).EnvioXML, Propriedade.ExtRetorno.ProRec_ERR, ex);
                    }
                    else
                    {
                        var nomeArqEmProcesamento = Empresas.Configuracoes[emp].PastaXmlEnviado + "\\" + PastaEnviados.EmProcessamento.ToString() + "\\" + (new FileInfo(NomeArquivoXML).Name);
                        TFunctions.GravarArqErroServico(nomeArqEmProcesamento, Propriedade.Extensao(Propriedade.TipoEnvio.MDFe).EnvioXML, Propriedade.ExtRetorno.ProRec_ERR, ex);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Salvar o arquivo do MDFe assinado na pasta EmProcessamento
        /// </summary>
        /// <param name="emp">Codigo da empresa</param>
        private void SalvarArquivoEmProcessamento(int emp)
        {
            Empresas.Configuracoes[emp].CriarSubPastaEnviado();

            var nomeArqMDFe = new FileInfo(NomeArquivoXML).Name;

            var arqEmProcessamento = Empresas.Configuracoes[emp].PastaXmlEnviado + "\\" + PastaEnviados.EmProcessamento.ToString() + "\\" + nomeArqMDFe;
            var sw = File.CreateText(arqEmProcessamento);
            sw.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + ConteudoXML.GetElementsByTagName("MDFe")[0].OuterXml);
            sw.Close();

            if(File.Exists(arqEmProcessamento))
            {
                File.Delete(NomeArquivoXML);
            }
        }

        #endregion Execute

        #region FinalizarNFeSincrono()

        /// <summary>
        /// Finalizar a NFe no processo Síncrono
        /// </summary>
        /// <param name="xmlRetorno">Conteúdo do XML retornado da SEFAZ</param>
        /// <param name="emp">Código da empresa para buscar as configurações</param>
        private void FinalizarNFeSincrono(string xmlRetorno, int emp, string chMDFe)
        {
            //#region Modelo de retorno para ser utilizando em testes, não desmarque ou se esqueça de marcar como comentário depois de testado. Não apague.
            //xmlRetorno = "<retMDFe xmlns=\"http://www.portalfiscal.inf.br/mdfe\" versao=\"3.00\"><tpAmb>1</tpAmb><cUF>41</cUF><verAplic>RS20210422122725</verAplic><cStat>104</cStat><xMotivo>Arquivo Processado</xMotivo><protMDFe versao=\"3.00\" xmlns=\"http://www.portalfiscal.inf.br/mdfe\"><infProt Id=\"MDFe941200015321345\"><tpAmb>1</tpAmb><verAplic>RS20200915181452</verAplic><chMDFe>41201280568835000181580010000010401406004659</chMDFe><dhRecbto>2020-12-04T08:36:36-03:00</dhRecbto><nProt>941200015321345</nProt><digVal>op++bKeqQeIEdBOQ5osoQvWnStQ=</digVal><cStat>100</cStat><xMotivo>Autorizado o uso do MDF-e</xMotivo></infProt></protMDFe></retMDFe>";
            //#endregion

            var xml = new XmlDocument();
            xml.Load(Functions.StringXmlToStream(xmlRetorno));

            var protMDFe = xml.GetElementsByTagName("protMDFe");

            var fluxoNFe = new FluxoNfe();

            var retRecepcao = new TaskMDFeRetRecepcao
            {
                chMDFe = chMDFe,
                NomeArquivoXML = NomeArquivoXML
            };

            retRecepcao.FinalizarMDFe(protMDFe, fluxoNFe, emp, ConteudoXML);
        }

        #endregion FinalizarNFeSincrono()
    }
}