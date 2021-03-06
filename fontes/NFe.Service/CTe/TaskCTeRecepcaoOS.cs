using NFe.Components;
using NFe.Exceptions;
using NFe.Settings;
using System;
using System.IO;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.CTeOS;
using Unimake.Business.DFe.Xml.CTeOS;

namespace NFe.Service
{
    public class TaskCTeRecepcaoOS: TaskAbst
    {
        private int NumeroLote;

        public TaskCTeRecepcaoOS(string arquivo)
        {
            Servico = Servicos.CteRecepcaoOS;
            NomeArquivoXML = arquivo;
            ConteudoXML.PreserveWhitespace = false;
            ConteudoXML.Load(arquivo);
        }

        #region Classe com os dados do XML do retorno do envio do Lote de NFe

        /// <summary>
        /// Esta herança que deve ser utilizada fora da classe para obter os valores das tag´s do recibo do lote
        /// </summary>
        private DadosRecClass dadosRec;

        #endregion Classe com os dados do XML do retorno do envio do Lote de NFe

        #region Execute

        public override void Execute()
        {
            var emp = Empresas.FindEmpresaByThread();

            try
            {
                dadosRec = new DadosRecClass();
                var fluxoNfe = new FluxoNfe();

                var lerXml = new LerXML();
                lerXml.Cte(ConteudoXML);

                var idLote = lerXml.oDadosNfe.idLote;

                var xmlCTeOS = new CTeOS();
                xmlCTeOS = Unimake.Business.DFe.Utility.XMLUtility.Deserializar<CTeOS>(ConteudoXML);

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTeOS,
                    CertificadoDigital = Empresas.Configuracoes[emp].X509Certificado
                };

                if(ConfiguracaoApp.Proxy)
                {
                    configuracao.HasProxy = true;
                    configuracao.ProxyAutoDetect = ConfiguracaoApp.DetectarConfiguracaoProxyAuto;
                    configuracao.ProxyUser = ConfiguracaoApp.ProxyUsuario;
                    configuracao.ProxyPassword = ConfiguracaoApp.ProxySenha;
                }

                var autorizacao = new Autorizacao(xmlCTeOS, configuracao);

                NumeroLote = oGerarXML.GerarLoteCTeOS(NomeArquivoXML);

                autorizacao.Executar();

                ConteudoXML = autorizacao.ConteudoXMLAssinado;

                SalvarArquivoEmProcessamento(emp, lerXml.oDadosNfe.chavenfe);

                #region Código comentado para ser utilizado em testes, quando necessário

                ////Utilizo o código abaixo quando preciso fazer testes com CTeOS autorizado, sempre deixo ele comentado para quando eu precisar, não apague, por favor. Wandrey 09/12/2021
                //vStrXmlRetorno = "<retCTeOS versao=\"3.00\" xmlns=\"http://www.portalfiscal.inf.br/cte\"><tpAmb>2</tpAmb><cUF>35</cUF><verAplic>SP-CTe-23-06-2017</verAplic><cStat>100</cStat><xMotivo>Autorizado o uso do CT-e</xMotivo><protCTe versao=\"3.00\"><infProt><tpAmb>2</tpAmb><verAplic>SP-CTe-23-06-2017</verAplic><chCTe>35170746014122000138670000000000261309301440</chCTe><dhRecbto>2017-07-26T11:47:48-03:00</dhRecbto><nProt>135170008595733</nProt><digVal>XTkEEwjNnoYasDYz/VJ7HuZVUEo=</digVal><cStat>100</cStat><xMotivo>Autorizado o uso do CT-e</xMotivo></infProt></protCTe></retCTeOS>";
                //autorizacao.RetornoWSXML.LoadXml(vStrXmlRetorno);
                //autorizacao.RetornoWSString = vStrXmlRetorno;

                #endregion

                vStrXmlRetorno = autorizacao.RetornoWSString;

                #region Parte que trata o retorno do lote com a autorização, denegação ou rejeição

                GerarXMLDistribuicao(emp, autorizacao);

                //Gravar o XML retornado pelo WebService do SEFAZ na pasta de retorno para o ERP
                //Tem que ser feito neste ponto, pois somente aqui terminamos todo o processo
                oGerarXML.XmlRetorno(Propriedade.Extensao(Propriedade.TipoEnvio.CTeOS).EnvioXML,
                    Propriedade.Extensao(Propriedade.TipoEnvio.PedRec).RetornoXML,
                    vStrXmlRetorno,
                    Empresas.Configuracoes[emp].PastaXmlRetorno,
                    NumeroLote.ToString("000000000000000") + Propriedade.Extensao(Propriedade.TipoEnvio.CTeOS).EnvioXML);

                #endregion
            }
            catch(ExceptionEnvioXML ex)
            {
                //Ocorreu algum erro no exato momento em que tentou enviar o XML para o SEFAZ, vou ter que tratar
                //para ver se o XML chegou lá ou não, se eu consegui pegar o número do recibo de volta ou não, etc.
                //E ver se vamos tirar o XML do Fluxo ou finalizar ele com a consulta situação da NFe
                try
                {
                    //Gravar o arquivo de erro de retorno para o ERP, caso ocorra
                    TFunctions.GravarArqErroServico(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.CTeOS).EnvioXML, Propriedade.ExtRetorno.ProRec_ERR, ex, NumeroLote.ToString("000000000000000") + Propriedade.Extensao(Propriedade.TipoEnvio.CTeOS).EnvioXML);
                }
                catch
                {
                    //Se falhou algo na hora de gravar o retorno .ERR (de erro) para o ERP, infelizmente não posso fazer mais nada.
                    //Wandrey 16/03/2010
                }
            }
            catch(ExceptionSemInternet ex)
            {
                try
                {
                    //Gravar o arquivo de erro de retorno para o ERP, caso ocorra
                    TFunctions.GravarArqErroServico(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.CTeOS).EnvioXML, Propriedade.ExtRetorno.ProRec_ERR, ex, NumeroLote.ToString("000000000000000") + Propriedade.Extensao(Propriedade.TipoEnvio.CTeOS).EnvioXML);
                }
                catch
                {
                    //Se falhou algo na hora de gravar o retorno .ERR (de erro) para o ERP, infelizmente não posso fazer mais nada.
                    //Wandrey 16/03/2010
                }
            }
            catch(Exception ex)
            {
                try
                {
                    //Gravar o arquivo de erro de retorno para o ERP, caso ocorra
                    TFunctions.GravarArqErroServico(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.CTeOS).EnvioXML, Propriedade.ExtRetorno.ProRec_ERR, ex, NumeroLote.ToString("000000000000000") + Propriedade.Extensao(Propriedade.TipoEnvio.CTeOS).EnvioXML);
                }
                catch
                {
                    //Se falhou algo na hora de gravar o retorno .ERR (de erro) para o ERP, infelizmente não posso fazer mais nada.
                    //Wandrey 16/03/2010
                }
            }
        }

        #endregion Execute

        /// <summary>
        /// Salvar o arquivo do CTeOS assinado na pasta EmProcessamento
        /// </summary>
        /// <param name="emp">Codigo da empresa</param>
        /// <param name="chaveCTeOS">Chave do CTeOS</param>
        private void SalvarArquivoEmProcessamento(int emp, string chaveCTeOS)
        {
            Empresas.Configuracoes[emp].CriarSubPastaEnviado();

            var nomeArqCTeOS = Path.GetFileName(NomeArquivoXML);
            var arqEmProcessamento = Empresas.Configuracoes[emp].PastaXmlEnviado + "\\" + PastaEnviados.EmProcessamento.ToString() + "\\" + nomeArqCTeOS;

            var sw = File.CreateText(arqEmProcessamento);
            sw.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>" + ConteudoXML.GetElementsByTagName("CTeOS")[0].OuterXml);
            sw.Close();

            if(File.Exists(arqEmProcessamento))
            {
                File.Delete(NomeArquivoXML);

                NomeArquivoXML = arqEmProcessamento;
            }
        }

        /// <summary>
        /// Se o CTeOS foi autorizado, gerar o XML de distribuição
        /// </summary>
        /// <param name="emp">Empresa que está enviando o CTeOS</param>
        /// <param name="autorizacao">Objeto da classe de autorização do CTeOS para que façamos uso das informações do XML do CTeOS e protocolo de autorização</param>
        private void GerarXMLDistribuicao(int emp, Autorizacao autorizacao)
        {
            var pastaEmProcessamento = Empresas.Configuracoes[emp].PastaXmlEnviado + "\\" + PastaEnviados.EmProcessamento.ToString();

            switch(autorizacao.Result.CStat)
            {
                case 100: //CTeOS autorizado -> Sincrono, alguns estados, já retorna 100 direto. Por isso tenho que esperar o 100 e 104.
                case 104: //Lote processado
                    switch(autorizacao.Result.ProtCTe.InfProt.CStat)
                    {
                        case 100: //CTeOS autorizado.
                            if(File.Exists(NomeArquivoXML))
                            {
                                var nomeArqDistribuicao = Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.CTe).EnvioXML) + Propriedade.ExtRetorno.ProcCTe;

                                //Definir o caminho/nome do XML de distribuição na pasta EmProcessamento
                                var arqProcCTeEmProcessamento = Path.Combine(pastaEmProcessamento, nomeArqDistribuicao);

                                //Definir o nome da pasta dos XMLs autorizados
                                var nomePastaAutorizado = Empresas.Configuracoes[emp].PastaXmlEnviado + "\\" +
                                    PastaEnviados.Autorizados.ToString() + "\\" +
                                    Empresas.Configuracoes[emp].DiretorioSalvarComo.ToString(autorizacao.CTeOS.InfCTe.Ide.DhEmi);

                                //Definir o nome do arquivo de distribuição já combinado com a pasta
                                var arquivoAutorizado = Path.Combine(nomePastaAutorizado, nomeArqDistribuicao);

                                if(!File.Exists(arquivoAutorizado))
                                {
                                    //Gravar o XML de distribuição na pasta EmProcessamento
                                    autorizacao.GravarXmlDistribuicao(pastaEmProcessamento, nomeArqDistribuicao, autorizacao.CteOSProcResults[autorizacao.CTeOS.InfCTe.Chave].GerarXML().OuterXml);

                                    //Mover a cteProc da pasta de CTe em processamento para a NFe Autorizada
                                    //Para envitar falhar, tenho que mover primeiro o XML de distribuição (-procCTe.xml) para
                                    //depois mover o da nfe (-cte.xml), pois se ocorrer algum erro, tenho como reconstruir o senário,
                                    //assim sendo não inverta as posições. Wandrey 08/10/2009
                                    TFunctions.MoverArquivo(arqProcCTeEmProcessamento, PastaEnviados.Autorizados, autorizacao.CTeOS.InfCTe.Ide.DhEmi);
                                }

                                var nomeArquivoOriginal = Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.CTe).EnvioXML) + Propriedade.Extensao(Propriedade.TipoEnvio.CTe).EnvioXML;
                                var arquivoAutorizadoOriginal = Path.Combine(nomePastaAutorizado, nomeArquivoOriginal);

                                if(!File.Exists(arquivoAutorizadoOriginal))
                                {
                                    //Mover o CTe da pasta em processamento para CTe Autorizada
                                    //Para envitar falhar, tenho que mover primeiro o XML de distribuição (-procCTe.xml) para
                                    //depois mover o da nfe (-cte.xml), pois se ocorrer algum erro, tenho como reconstruir o cenário.
                                    //assim sendo não inverta as posições. Wandrey 08/10/2009
                                    TFunctions.MoverArquivo(NomeArquivoXML, PastaEnviados.Autorizados, autorizacao.CTeOS.InfCTe.Ide.DhEmi);
                                }
                                else
                                {
                                    if(File.Exists(arquivoAutorizado))
                                    {
                                        var arqXmlPastaErro = Path.Combine(Empresas.Configuracoes[emp].PastaXmlErro, nomeArquivoOriginal);
                                        File.Delete(arqXmlPastaErro);
                                        File.Move(NomeArquivoXML, arqXmlPastaErro);
                                    }
                                }

                                try
                                {

                                    //Disparar o UniDANFe
                                    TFunctions.ExecutaUniDanfe(arquivoAutorizado, autorizacao.CTeOS.InfCTe.Ide.DhEmi, Empresas.Configuracoes[emp]);
                                }
                                catch(Exception ex)
                                {
                                    Auxiliar.WriteLog("TaskCTeRecepcaoOS: Autorizado - " + ex.Message, false);
                                }
                            }
                            break;

                        case 110: //Uso denegado
                        case 301: //Uso denegado: Irregularidade fiscal do emitente
                            if(File.Exists(NomeArquivoXML))
                            {
                                //Nome do arquivo de distribuição
                                var nomeArqDistribuicao = Functions.ExtrairNomeArq(NomeArquivoXML, Propriedade.Extensao(Propriedade.TipoEnvio.CTe).EnvioXML) + Propriedade.ExtRetorno.Den;

                                //Definir o caminho/nome do XML de distribuição na pasta EmProcessamento
                                var arqProcCTeEmProcessamento = Path.Combine(pastaEmProcessamento, nomeArqDistribuicao);

                                //Definir o nome da pasta dos XMLs denegados
                                var nomePastaDenegado = Empresas.Configuracoes[emp].PastaXmlEnviado + "\\" +
                                    PastaEnviados.Denegados.ToString() + "\\" +
                                    Empresas.Configuracoes[emp].DiretorioSalvarComo.ToString(autorizacao.CTeOS.InfCTe.Ide.DhEmi);

                                //Definir o nome do arquivo de distribuição já combinado com a pasta
                                var arquivoDenegado = Path.Combine(nomePastaDenegado, nomeArqDistribuicao);

                                if(!File.Exists(arquivoDenegado))
                                {
                                    //Gravar o XML de distribuição na pasta EmProcessamento
                                    autorizacao.GravarXmlDistribuicao(pastaEmProcessamento, nomeArqDistribuicao, autorizacao.CteOSProcResults[autorizacao.CTeOS.InfCTe.Chave].GerarXML().OuterXml);

                                    //Mover a cteProc da pasta de CTe em processamento para a NFe Autorizada
                                    //Para envitar falhar, tenho que mover primeiro o XML de distribuição (-procCTe.xml) para
                                    //depois mover o da nfe (-cte.xml), pois se ocorrer algum erro, tenho como reconstruir o senário,
                                    //assim sendo não inverta as posições. Wandrey 08/10/2009
                                    TFunctions.MoverArquivo(arqProcCTeEmProcessamento, PastaEnviados.Denegados, autorizacao.CTeOS.InfCTe.Ide.DhEmi);
                                }

                                //Mover o CTe da pasta em processamento para CTe Autorizada
                                //Para envitar falhar, tenho que mover primeiro o XML de distribuição (-procCTe.xml) para
                                //depois mover o da nfe (-cte.xml), pois se ocorrer algum erro, tenho como reconstruir o cenário.
                                //assim sendo não inverta as posições. Wandrey 08/10/2009
                                if(File.Exists(arquivoDenegado))
                                {
                                    File.Delete(NomeArquivoXML);
                                }

                                try
                                {
                                    //Disparar o UniDANFe
                                    TFunctions.ExecutaUniDanfe(arquivoDenegado, autorizacao.CTeOS.InfCTe.Ide.DhEmi, Empresas.Configuracoes[emp]);
                                }
                                catch(Exception ex)
                                {
                                    Auxiliar.WriteLog("TaskCTeRecepcaoOS: Denegado - " + ex.Message, false);
                                }
                            }
                            break;

                        default:
                            oAux.MoveArqErro(NomeArquivoXML);
                            break;
                    }
                    break;

                default:
                    //Rejeição - Vamos tirar o XML da fila de processamento para que o ERP analise e corrija para posterior reenvio do XML.
                    TFunctions.MoveArqErro(NomeArquivoXML);
                    break;
            }
        }
    }
}