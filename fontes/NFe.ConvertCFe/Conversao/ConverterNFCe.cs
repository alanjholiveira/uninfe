﻿using NFe.Settings;
using NFe.Validate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Unimake.SAT.Servico.Envio;

namespace NFe.SAT.Conversao
{
    /// <summary>
    /// Classe responsável pela conversão da NFCe em CFe
    /// </summary>
    public class ConverterNFCe
    {
        private Empresa DadosEmpresa;

        private FileStream FileStream;

        private XmlDocument Document;

        private envCFeCFe ObjEnvio = new envCFeCFe();

        private string InputFile;

        private string OutputFile;

        /// <summary>
        /// Arquivo de saída
        /// </summary>
        public string XMLOutput = "";

        /// <summary>
        /// Converter a CFe
        /// </summary>
        /// <param name="file">arquivo da NFCe</param>
        /// <param name="dadosEmpresa">dados da empresa</param>
        /// <param name="outputfile">onde será salvo o arquivo</param>
        public ConverterNFCe(string file, Empresa dadosEmpresa, string outputfile = null)
        {
            if (outputfile == null)
                outputfile = file;

            InputFile = file;
            OutputFile = outputfile;
            DadosEmpresa = dadosEmpresa;

            FileStream = new FileStream(InputFile, FileMode.Open, FileAccess.ReadWrite);

            Document = new XmlDocument();
            Document.Load(FileStream);

            FileStream.Dispose();
            FileStream.Close();
        }

        /// <summary>
        /// Executar a conversão
        /// </summary>
        public void ConverterSAT()
        {
            ObjEnvio = GerarLoteCFe();

            if (File.Exists(OutputFile))
                File.Delete(OutputFile);

            XMLOutput = ObjEnvio.Serialize();
            XmlDocument xmloutput = new XmlDocument();
            xmloutput.LoadXml(XMLOutput);

            XmlElement root = xmloutput.DocumentElement;
            root.RemoveAttribute("xmlns:xsd");
            root.RemoveAttribute("xmlns:xsi");

            xmloutput.Save(OutputFile);
        }

        /// <summary>
        /// Gerar arquivo de lote a partir da NFCe
        /// </summary>
        /// <returns>Lote da CFe</returns>
        private envCFeCFe GerarLoteCFe()
        {
            return new envCFeCFe
            {
                infCFe = new envCFeCFeInfCFe
                {
                    versaoDadosEnt = "0.07",
                    ide = new envCFeCFeInfCFeIde
                    {
                        CNPJ = DadosEmpresa.CNPJSoftwareHouse,
                        signAC = DadosEmpresa.SignACSAT,
                        numeroCaixa = "001" //TODO: SAT ver de onde vai pegar isso
                    },
                    emit = new envCFeCFeInfCFeEmit
                    {
                        CNPJ = GetValueXML("emit", "CNPJ"),
                        IE = GetValueXML("emit", "IE"),
                        IM = GetValueXML("emit", "IM"),
                        indRatISSQN = GetValueXML("emit", "indRatISSQN"),
                    },
                    dest = new envCFeCFeInfCFeDest
                    {
                        Item = GetDocumentoPessoa("dest"),
                        ItemElementName = (GetDocumentoPessoa("dest").Length == 11 ? ItemChoiceType.CPF : ItemChoiceType.CNPJ),
                        xNome = GetValueXML("dest", "xNome")
                    },
                    det = PopularProdutos(),
                    total = new envCFeCFeInfCFeTotal
                    {
                        vCFeLei12741 = GetValueXML("ICMSTot", "vNF")
                    },
                    entrega = new envCFeCFeInfCFeEntrega
                    {
                        xLgr = GetValueXML("enderDest", "xLgr"),
                        nro = GetValueXML("enderDest", "nro"),
                        xCpl = GetValueXML("enderDest", "xCpl"),
                        xBairro = GetValueXML("enderDest", "xBairro"),
                        xMun = GetValueXML("enderDest", "xMun"),
                        UF = GetValueXML("enderDest", "UF")
                    },
                    pgto = new envCFeCFeInfCFePgto
                    {
                        MP = PopularMeioPagamento()
                    }
                }
            };
        }

        /// <summary>
        /// Adiciona os itens da nota ao Lote da CFe
        /// </summary>
        /// <returns>Lista de Produtos</returns>
        private List<envCFeCFeInfCFeDet> PopularProdutos()
        {
            string ValorDoItem = "";
            List<envCFeCFeInfCFeDet> result = new List<envCFeCFeInfCFeDet>();

            XmlNodeList nodes = Document.GetElementsByTagName("det");
            foreach (XmlNode detNFCe in nodes)
            {
                envCFeCFeInfCFeDet det = new envCFeCFeInfCFeDet();
                det.nItem = detNFCe.Attributes["nItem"].Value;

                foreach (XmlNode itensDet in detNFCe.ChildNodes)
                {
                    switch (itensDet.Name)
                    {
                        case "prod":
                            det.prod = new envCFeCFeInfCFeDetProd
                            {
                                cProd = GetXML(itensDet.ChildNodes, "cProd"),
                                xProd = GetXML(itensDet.ChildNodes, "xProd"),
                                NCM = GetXML(itensDet.ChildNodes, "NCM"),
                                CFOP = GetXML(itensDet.ChildNodes, "CFOP"),
                                uCom = GetXML(itensDet.ChildNodes, "uCom"),
                                qCom = GetXML(itensDet.ChildNodes, "qCom"),
                                vUnCom = GetXML(itensDet.ChildNodes, "vUnCom"),

                                indRegra = "A",
                            };
                            ValorDoItem = GetXML(itensDet.ChildNodes, "vProd");
                            break;

                        case "imposto":
                            det.imposto = new envCFeCFeInfCFeDetImposto();
                            det.imposto.vItem12741 = ValorDoItem;
                            foreach (XmlNode n in itensDet.ChildNodes)
                            {
                                switch (n.Name)
                                {
                                    case "ICMS":
                                        det.imposto.Item = ImpostoProduto<envCFeCFeInfCFeDetImpostoICMS>(n.ChildNodes);
                                        break;

                                    case "PIS":
                                        det.imposto.PIS = ImpostoProduto<envCFeCFeInfCFeDetImpostoPIS>(n.ChildNodes);
                                        break;

                                    case "COFINS":
                                        det.imposto.COFINS = ImpostoProduto<envCFeCFeInfCFeDetImpostoCOFINS>(n.ChildNodes);
                                        break;

                                    default:
                                        break;
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }


                result.Add(det);
            }

            return result;
        }

        /// <summary>
        /// Formas de pagamento
        /// </summary>
        /// <returns>lista de formas de pagamento</returns>
        private List<envCFeCFeInfCFePgtoMP> PopularMeioPagamento()
        {
            List<envCFeCFeInfCFePgtoMP> result = new List<envCFeCFeInfCFePgtoMP>();

            XmlNodeList nodes = Document.GetElementsByTagName("pag");
            foreach (XmlNode pagNFCe in nodes)
            {
                envCFeCFeInfCFePgtoMP mp = new envCFeCFeInfCFePgtoMP
                {
                    cMP = GetXML(pagNFCe.ChildNodes, "tPag"),
                    vMP = GetXML(pagNFCe.ChildNodes, "vPag")
                };

                result.Add(mp);
            }
            return result;
        }

        /// <summary>
        /// Grava valores de impostos nos produtos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="childs"></param>
        /// <returns></returns>
        private T ImpostoProduto<T>(XmlNodeList childs)
            where T : new()
        {
            T result = new T();

            foreach (XmlNode tag in childs)
            {
                switch (tag.Name)
                {
                    #region ICMS00
                    case "ICMS00":
                        envCFeCFeInfCFeDetImpostoICMSICMS00 ICMS00 = new envCFeCFeInfCFeDetImpostoICMSICMS00
                        {
                            Orig = GetXML(tag.ChildNodes, "Orig"),
                            CST = GetXML(tag.ChildNodes, "CST"),
                            pICMS = GetXML(tag.ChildNodes, "pICMS"),
                        };

                        SetProperrty(result, "Item", ICMS00);
                        break;
                    #endregion

                    #region ICMS40
                    case "ICMS40":
                        envCFeCFeInfCFeDetImpostoICMSICMS40 ICMS40 = new envCFeCFeInfCFeDetImpostoICMSICMS40
                        {
                            CST = GetXML(tag.ChildNodes, "CST"),
                            Orig = GetXML(tag.ChildNodes, "Orig"),
                        };

                        SetProperrty(result, "Item", ICMS40);
                        break;
                    #endregion

                    #region ICMSSN102
                    case "ICMSSN102":
                        envCFeCFeInfCFeDetImpostoICMSICMSSN102 ICMSSN102 = new envCFeCFeInfCFeDetImpostoICMSICMSSN102
                        {
                            CSOSN = GetXML(tag.ChildNodes, "CSOSN"),
                            Orig = GetXML(tag.ChildNodes, "orig")
                        };

                        SetProperrty(result, "Item", ICMSSN102);
                        break;
                    #endregion

                    #region ICMSSN900
                    case "ICMSSN900":
                        envCFeCFeInfCFeDetImpostoICMSICMSSN900 ICMSSN900 = new envCFeCFeInfCFeDetImpostoICMSICMSSN900
                        {
                            CSOSN = GetXML(tag.ChildNodes, "CSOSN"),
                            Orig = GetXML(tag.ChildNodes, "orig")
                        };

                        SetProperrty(result, "Item", ICMSSN900);
                        break;
                    #endregion

                    #region PISAliq
                    case "PISAliq":
                        envCFeCFeInfCFeDetImpostoPISPISAliq PISAliq = new envCFeCFeInfCFeDetImpostoPISPISAliq
                        {
                            CST = GetXML(tag.ChildNodes, "CST"),
                            pPIS = GetXML(tag.ChildNodes, "pPIS"),
                            vBC = GetXML(tag.ChildNodes, "vBC"),
                            vPIS = GetXML(tag.ChildNodes, "vPIS")
                        };

                        SetProperrty(result, "Item", PISAliq);
                        break;
                    #endregion

                    #region PISNT
                    case "PISNT":
                        envCFeCFeInfCFeDetImpostoPISPISNT PISPISNT = new envCFeCFeInfCFeDetImpostoPISPISNT
                        {
                            CST = GetXML(tag.ChildNodes, "CST")
                        };

                        SetProperrty(result, "Item", PISPISNT);
                        break;
                    #endregion

                    #region PISOutr
                    case "PISOutr":
                        envCFeCFeInfCFeDetImpostoPISPISOutr PISOutr = new envCFeCFeInfCFeDetImpostoPISPISOutr
                        {
                            CST = GetXML(tag.ChildNodes, "CST"),
                            vPIS = GetXML(tag.ChildNodes, "vPIS")
                        };

                        SetProperrty(result, "Item", PISOutr);
                        break;
                    #endregion

                    #region PISQtde
                    case "PISQtde":
                        envCFeCFeInfCFeDetImpostoPISPISQtde PISQtde = new envCFeCFeInfCFeDetImpostoPISPISQtde
                        {
                            CST = GetXML(tag.ChildNodes, "CST"),
                            qBCProd = GetXML(tag.ChildNodes, "qBCProd"),
                            vAliqProd = GetXML(tag.ChildNodes, "vAliqProd"),
                            vPIS = GetXML(tag.ChildNodes, "vPIS")
                        };

                        SetProperrty(result, "Item", PISQtde);
                        break;
                    #endregion

                    #region PISSN
                    case "PISSN":
                        envCFeCFeInfCFeDetImpostoPISPISSN PISSN = new envCFeCFeInfCFeDetImpostoPISPISSN
                        {
                            CST = GetXML(tag.ChildNodes, "CST")
                        };
                        SetProperrty(result, "Item", PISSN);
                        break;
                    #endregion

                    #region COFINSAliq
                    case "COFINSAliq":
                        envCFeCFeInfCFeDetImpostoCOFINSCOFINSAliq COFINSAliq = new envCFeCFeInfCFeDetImpostoCOFINSCOFINSAliq
                        {
                            CST = GetXML(tag.ChildNodes, "CST"),
                            pCOFINS = GetXML(tag.ChildNodes, "pCOFINS"),
                            vCOFINS = GetXML(tag.ChildNodes, "vCOFINS"),
                            vBC = GetXML(tag.ChildNodes, "vBC")
                        };
                        SetProperrty(result, "Item", COFINSAliq);
                        break;
                    #endregion

                    #region COFINSNT
                    case "COFINSNT":
                        envCFeCFeInfCFeDetImpostoCOFINSCOFINSNT COFINSNT = new envCFeCFeInfCFeDetImpostoCOFINSCOFINSNT
                        {
                            CST = GetXML(tag.ChildNodes, "CST")
                        };
                        SetProperrty(result, "Item", COFINSNT);
                        break;
                    #endregion

                    #region COFINSNT
                    case "COFINSOutr":
                        envCFeCFeInfCFeDetImpostoCOFINSCOFINSOutr COFINSOutr = new envCFeCFeInfCFeDetImpostoCOFINSCOFINSOutr
                        {
                            CST = GetXML(tag.ChildNodes, "CST"),
                            vCOFINS = GetXML(tag.ChildNodes, "vCOFINS")
                        };
                        SetProperrty(result, "Item", COFINSOutr);
                        break;
                    #endregion

                    #region COFINSNT
                    case "COFINSQtde":
                        envCFeCFeInfCFeDetImpostoCOFINSCOFINSQtde COFINSQtde = new envCFeCFeInfCFeDetImpostoCOFINSCOFINSQtde
                        {
                            CST = GetXML(tag.ChildNodes, "CST"),
                            qBCProd = GetXML(tag.ChildNodes, "qBCProd"),
                            vCOFINS = GetXML(tag.ChildNodes, "vCOFINS"),
                            vAliqProd = GetXML(tag.ChildNodes, "vAliqProd")
                        };
                        SetProperrty(result, "Item", COFINSQtde);
                        break;
                    #endregion

                    #region COFINSSN
                    case "COFINSSN":
                        envCFeCFeInfCFeDetImpostoCOFINSCOFINSSN COFINSSN = new envCFeCFeInfCFeDetImpostoCOFINSCOFINSSN
                        {
                            CST = GetXML(tag.ChildNodes, "CST")
                        };
                        SetProperrty(result, "Item", COFINSSN);
                        break;
                    #endregion

                    default:
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Adiciona valores por referencia nos objetos
        /// </summary>
        /// <param name="result">objeto que será definido o valor</param>
        /// <param name="propertyName">nome da propriedade</param>
        /// <param name="value">valor que será definido</param>
        private void SetProperrty(object result, string propertyName, object value)
        {
            PropertyInfo pi = result.GetType().GetProperty(propertyName);

            if (pi != null && !String.IsNullOrEmpty(value.ToString()))
            {
                pi.SetValue(result, value, null);
            }
        }

        /// <summary>
        /// Busca valor de uma tag a partir de um nó
        /// </summary>
        /// <param name="nodes">nó onde vai ser procurado a tag</param>
        /// <param name="nameTag">nome da tag</param>
        /// <returns></returns>
        private string GetXML(XmlNodeList nodes, string nameTag)
        {
            string value = "";
            foreach (XmlNode n in nodes)
            {
                if (n.NodeType == XmlNodeType.Element)
                {
                    if (n.Name.Equals(nameTag))
                    {
                        value = n.InnerText;
                        break;
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Localiza o numero do documento da pessoa
        /// </summary>
        /// <param name="parentTag"></param>
        /// <returns></returns>
        private string GetDocumentoPessoa(string parentTag)
        {
            string result = GetValueXML("dest", "CPF");

            if (String.IsNullOrEmpty(result))
                result = GetValueXML("dest", "CNPJ");

            return result;
        }

        /// <summary>
        /// Retorna o valor do documento a partir da raiz
        /// </summary>
        /// <param name="groupTag">nome do nó</param>
        /// <param name="nameTag">nome da tag</param>
        /// <returns></returns>
        private string GetValueXML(string groupTag, string nameTag)
        {
            string value = "";
            XmlNodeList nodes = Document.GetElementsByTagName(groupTag);
            XmlNode node = nodes[0];

            foreach (XmlNode n in node)
            {
                if (n.NodeType == XmlNodeType.Element)
                {
                    if (n.Name.Equals(nameTag))
                    {
                        value = n.InnerText;
                        break;
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Retorna o dígito verificador do valor informado
        /// </summary>
        /// <param name="value">Valor para calcular o dígito verificador</param>
        /// <returns></returns>
        private int Modulus11Digit(string value)
        {
            int[] weigths = { 2, 3, 4, 5, 6, 7, 8, 9, 2, 3, 4, 5, 6, 7, 8, 9 };

            int sum = 0;
            int idx = 0;

            for (int i = value.Length - 1; i >= 0; i--)
            {
                sum += Convert.ToInt32(value[i].ToString()) * weigths[idx];
                if (idx == 9)
                {
                    idx = 2;
                }
                else
                {
                    idx++;
                }
            }

            int rest = (sum * 10) % 11;
            int result = rest;
            if (result >= 10)
                result = 0;

            return result;
        }

        /// <summary>
        /// Realiza validação do XML a partir dos schemas
        /// </summary>
        public void ValidarCFe()
        {
            ValidarXML validar = new ValidarXML(OutputFile, Convert.ToInt16(DadosEmpresa.UnidadeFederativaCodigo), true);
            string cResultadoValidacao = validar.ValidarArqXML(OutputFile);
            if (cResultadoValidacao != "")
            {
                throw new Exception(cResultadoValidacao);
            }
        }

    }
}