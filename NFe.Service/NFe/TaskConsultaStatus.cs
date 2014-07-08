﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.IO;

using NFe.Components;
using NFe.Settings;
using NFe.Certificado;
using NFe.Exceptions;

namespace NFe.Service
{
    public class TaskConsultaStatus : TaskAbst
    {
        public TaskConsultaStatus()
        {
            Servico = Servicos.ConsultaStatusServicoNFe;
        }

        #region Classe com os dados do XML da consulta do status do serviço da NFe
        /// <summary>
        /// Esta herança que deve ser utilizada fora da classe para obter os valores das tag´s do status do serviço
        /// </summary>
        private DadosPedSta dadosPedSta;
        #endregion

        #region Execute
        public override void Execute()
        {
            int emp = Functions.FindEmpresaByThread();

            try
            {
                dadosPedSta = new DadosPedSta();
                //Ler o XML para pegar parâmetros de envio
                PedSta(emp, NomeArquivoXML);

                if (vXmlNfeDadosMsgEhXML)  //danasa 12-9-2009
                {
                    //Definir o objeto do WebService
                    WebServiceProxy wsProxy = ConfiguracaoApp.DefinirWS(Servicos.ConsultaStatusServicoNFe, emp, dadosPedSta.cUF, dadosPedSta.tpAmb, dadosPedSta.tpEmis, dadosPedSta.versao);

                    //Criar objetos das classes dos serviços dos webservices do SEFAZ
                    var oStatusServico = wsProxy.CriarObjeto(NomeClasseWS(Servico, dadosPedSta.cUF));
                    var oCabecMsg = wsProxy.CriarObjeto(NomeClasseCabecWS(dadosPedSta.cUF, Servico));

                    //Atribuir conteúdo para duas propriedades da classe nfeCabecMsg
                    wsProxy.SetProp(oCabecMsg, "cUF", dadosPedSta.cUF.ToString());
                    wsProxy.SetProp(oCabecMsg, "versaoDados", dadosPedSta.versao);

                    //Invocar o método que envia o XML para o SEFAZ
                    oInvocarObj.Invocar(wsProxy, oStatusServico, NomeMetodoWS(Servico, dadosPedSta.cUF), oCabecMsg, this, "-ped-sta", "-sta");
                }
                else
                {
                    // Gerar o XML de solicitacao de situacao do servico a partir do TXT gerado pelo ERP
                    oGerarXML.StatusServicoNFe(System.IO.Path.GetFileNameWithoutExtension(NomeArquivoXML) + ".xml", dadosPedSta.tpAmb, dadosPedSta.tpEmis, dadosPedSta.cUF, dadosPedSta.versao);
                }
            }
            catch (Exception ex)
            {
                var extRet = vXmlNfeDadosMsgEhXML ? Propriedade.ExtEnvio.PedSta_XML : Propriedade.ExtEnvio.PedSta_TXT;

                try
                {
                    //Gravar o arquivo de erro de retorno para o ERP, caso ocorra
                    TFunctions.GravarArqErroServico(NomeArquivoXML, extRet, Propriedade.ExtRetorno.Sta_ERR, ex);
                }
                catch
                {
                    //Se falhou algo na hora de gravar o retorno .ERR (de erro) para o ERP, infelizmente não posso fazer mais nada.
                    //Wandrey 09/03/2010
                }
            }
            finally
            {
                try
                {
                    //Deletar o arquivo de solicitação do serviço
                    Functions.DeletarArquivo(NomeArquivoXML);
                }
                catch
                {
                    //Se falhou algo na hora de deletar o XML de solicitação do serviço, 
                    //infelizmente não posso fazer mais nada, o UniNFe vai tentar mandar 
                    //o arquivo novamente para o webservice
                    //Wandrey 09/03/2010
                }
            }
        }
        #endregion

        #region PedSta()
        /// <summary>
        /// Faz a leitura do XML de pedido do status de serviço
        /// </summary>
        /// <param name="cArquivoXml">Nome do XML a ser lido</param>
        /// <by>Wandrey Mundin Ferreira</by>
        private void PedSta(int emp, string cArquivoXML)
        {
            dadosPedSta.tpAmb = 0;
            dadosPedSta.cUF = Empresa.Configuracoes[emp].UnidadeFederativaCodigo;
            dadosPedSta.versao = NFe.ConvertTxt.versoes.VersaoXMLStatusServico;

            ///
            /// danasa 9-2009
            /// Assume o que está na configuracao
            /// 
            dadosPedSta.tpEmis = Empresa.Configuracoes[emp].tpEmis;

            ///
            /// danasa 12-9-2009
            /// 
            if (Path.GetExtension(cArquivoXML).ToLower() == ".txt")
            {
                // tpEmis|1						<<< opcional >>>
                // tpAmb|1
                // cUF|35
                // versao|3.10
                List<string> cLinhas = Functions.LerArquivo(cArquivoXML);
                foreach (string cTexto in cLinhas)
                {
                    string[] dados = cTexto.Split('|');
                    switch (dados[0].ToLower())
                    {
                        case "tpamb":
                            dadosPedSta.tpAmb = Convert.ToInt32("0" + dados[1].Trim());
                            break;
                        case "cuf":
                            dadosPedSta.cUF = Convert.ToInt32("0" + dados[1].Trim());
                            break;
                        case "tpemis":
                            dadosPedSta.tpEmis = Convert.ToInt32("0" + dados[1].Trim());
                            break;
                        case "versao":
                            dadosPedSta.versao = dados[1].Trim();
                            break;
                    }
                }
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(cArquivoXML);

                XmlNodeList consStatServList = doc.GetElementsByTagName("consStatServ");

                foreach (XmlNode consStatServNode in consStatServList)
                {
                    XmlElement consStatServElemento = (XmlElement)consStatServNode;

                    dadosPedSta.tpAmb = Convert.ToInt32("0" + consStatServElemento.GetElementsByTagName("tpAmb")[0].InnerText);
                    dadosPedSta.versao = consStatServElemento.Attributes["versao"].InnerText;

                    if (consStatServElemento.GetElementsByTagName("cUF").Count != 0)
                    {
                        dadosPedSta.cUF = Convert.ToInt32("0" + consStatServElemento.GetElementsByTagName("cUF")[0].InnerText);
                    }

                    if (consStatServElemento.GetElementsByTagName("tpEmis").Count != 0)
                    {
                        dadosPedSta.tpEmis = Convert.ToInt16(consStatServElemento.GetElementsByTagName("tpEmis")[0].InnerText);
                        /// para que o validador não rejeite, excluo a tag <tpEmis>
                        doc.DocumentElement.RemoveChild(consStatServElemento.GetElementsByTagName("tpEmis")[0]);
                        /// salvo o arquivo modificado
                        doc.Save(cArquivoXML);
                    }
                }
            }
        }
        #endregion
    }
}
