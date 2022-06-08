#pragma warning disable IDE1006 // Naming Styles

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Unimake.Business.DFe;
using Unimake.Business.DFe.Security;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.NFe;
using Unimake.Business.DFe.Utility;
using Unimake.Business.DFe.Xml.CTe;
using Unimake.Business.DFe.Xml.CTeOS;
using Unimake.Business.DFe.Xml.MDFe;
using Unimake.Business.DFe.Xml.NFe;
using Unimake.Exceptions;
using Unimake.Security.Platform;
using Dest = Unimake.Business.DFe.Xml.NFe.Dest;
using Dup = Unimake.Business.DFe.Xml.NFe.Dup;
using Emit = Unimake.Business.DFe.Xml.NFe.Emit;
using EnderDest = Unimake.Business.DFe.Xml.NFe.EnderDest;
using EnderEmit = Unimake.Business.DFe.Xml.NFe.EnderEmit;
using Fat = Unimake.Business.DFe.Xml.NFe.Fat;
using ICMS = Unimake.Business.DFe.Xml.NFe.ICMS;
using Ide = Unimake.Business.DFe.Xml.NFe.Ide;
using InfAdic = Unimake.Business.DFe.Xml.NFe.InfAdic;
using InfRespTec = Unimake.Business.DFe.Xml.NFe.InfRespTec;

namespace TesteDLL_Unimake.Business.DFe
{
    public partial class FormTestarNFe : Form
    {
        #region Private Fields

        private X509Certificate2 CertificadoSelecionado;

        #endregion Private Fields

        #region Private Methods

        private void BtnAbrirCertificadoArquivo_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "PFX|*.pfx",
                CheckFileExists = true
            };

            ofd.ShowDialog();
            var path = ofd.FileName;
            var password = Microsoft.VisualBasic.Interaction.InputBox("Informe a senha do certificado.", "Certificado");

            try
            {
                CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, password);
                MessageBox.Show("O certificado foi selecionado.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERRO!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            var testeCert = new CertificadoDigital();
            var certSel = testeCert.CarregarCertificadoDigitalA1(path, password);

            //Converte o certificado digital em array byte
            var certificadoString = testeCert.ToByteArray(path);

            //Agora você pode gravar o array byte no banco de dados (certificadoString)

            //Recuperar o certificado para uso a partir de uma array byte
            var CertSelect = testeCert.CarregarCertificadoDigitalA1(certificadoString, password);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsStatServ
                {
                    Versao = "4.00",
                    CUF = UFBrasil.SE,
                    TpAmb = TipoAmbiente.Homologacao
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.NFe,
                    TipoEmissao = TipoEmissao.Normal,
                    CertificadoDigital = CertificadoSelecionado,
                    HasProxy = false,
                    ProxyAutoDetect = false,
                    ProxyUser = "",
                    ProxyPassword = ""
                };

                var testeExceptionInterop = new Unimake.Exceptions.ThrowHelper();

                try
                {
                    testeExceptionInterop.Throw(new Unimake.Exceptions.CertificadoDigitalException());

                    var statusServico = new StatusServico(xml, configuracao);
                    statusServico.Executar();

                    MessageBox.Show(statusServico.RetornoWSString);
                    MessageBox.Show(statusServico.Result.XMotivo);
                }
                catch (Exception)
                {
                    MessageBox.Show(testeExceptionInterop.GetMessage());
                    MessageBox.Show(testeExceptionInterop.GetErrorCode().ToString());
                }

            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EnvEvento
                {
                    Versao = "1.00",
                    IdLote = "000000000000001",
                    Evento = new List<Evento> {
                        new Evento
                        {
                            Versao = "1.00",
                            InfEvento = new Unimake.Business.DFe.Xml.NFe.InfEvento(new Unimake.Business.DFe.Xml.NFe.DetEventoManif
                            {
                                Versao = "1.00",
                                DescEvento = "Operacao nao Realizada",
                                XJust = "Justificativa para manifestação da NFe de teste"
                            })
                            {
                                COrgao = UFBrasil.AN,
                                ChNFe = "41191000563803000154550010000020901551010553",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.ManifestacaoOperacaoNaoRealizada,
                                NSeqEvento = 1,
                                VerEvento = "1.00",
                                TpAmb = TipoAmbiente.Homologacao
                            }
                        }
                    }
                };



                var certDigital = new Unimake.Security.Platform.CertificadoDigital();
                var certSelecionado = certDigital.CarregarCertificadoDigitalA1(@"d:\teste\certificado.pfx", "123456");

                var thumbPrint = certDigital.GetThumbprint(certSelecionado); //Retorna o thumbprint do certificado digital
                var serialNumber = certDigital.GetSerialNumber(certSelecionado); //Retorna o serial number do certificado digital

                //Data inicial da validade do certificado digital
                var dataValidadeInicial = certDigital.GetNotBefore(certSelecionado);

                //Data final da validade do certificado digital
                var dataValidadeFinal = certDigital.GetNotAfter(certSelecionado);

                var configuracao = new Configuracao
                {
                    CertificadoDigital = CertificadoSelecionado
                };

                var recepcaoEvento = new RecepcaoEvento(xml, configuracao);
                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                if (recepcaoEvento.Result.CStat == 128) //128 = Lote de evento processado com sucesso
                {
                    switch (recepcaoEvento.Result.RetEvento[0].InfEvento.CStat)
                    {
                        case 135: //Evento homologado com vinculação da respectiva NFe
                        case 136: //Evento homologado sem vinculação com a respectiva NFe (SEFAZ não encontrou a NFe na base dela)
                        case 155: //Evento de Cancelamento homologado fora do prazo permitido para cancelamento
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;

                        default: //Evento rejeitado
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EnvEvento
                {
                    Versao = "1.00",
                    IdLote = "000000000000001",
                    Evento = new List<Evento> {
                        new Evento
                        {
                            Versao = "1.00",
                            InfEvento = new Unimake.Business.DFe.Xml.NFe.InfEvento(new Unimake.Business.DFe.Xml.NFe.DetEventoCCE
                            {
                                Versao = "1.00",
                                XCorrecao = "CFOP errada, segue CFOP correta."
                            })
                            {
                                COrgao = UFBrasil.PR,
                                ChNFe = "41191006117473000150550010000579281779843610",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.CartaCorrecao,
                                NSeqEvento = 3,
                                VerEvento = "1.00",
                                TpAmb = TipoAmbiente.Homologacao
                            }
                        },
                        new Evento
                        {
                            Versao = "1.00",
                            InfEvento = new Unimake.Business.DFe.Xml.NFe.InfEvento(new Unimake.Business.DFe.Xml.NFe.DetEventoCCE
                            {
                                Versao = "1.00",
                                XCorrecao = "Nome do transportador está errado, segue nome correto."
                            })
                            {
                                COrgao = UFBrasil.PR,
                                ChNFe = "41191006117473000150550010000579281779843610",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.CartaCorrecao,
                                NSeqEvento = 4,
                                VerEvento = "1.00",
                                TpAmb = TipoAmbiente.Homologacao
                            }
                        }
                    }
                };

                //var doc = new XmlDocument();
                //doc.Load(@"C:\projetos\uninfe\exemplos\NFe e NFCe 4.00\cce26111253420477000192550550000033071213028272_01-ped-eve.xml");
                //var xml = new EnvEvento();
                //xml = xml.LerXML<EnvEvento>(doc);

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.NFe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var recepcaoEvento = new RecepcaoEvento(xml, configuracao);
                recepcaoEvento.Executar();
                var xmlDistrib = new XmlDocument();
                xmlDistrib = recepcaoEvento.ProcEventoNFeResult[0].GerarXML();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                if (recepcaoEvento.Result.CStat == 128) //128 = Lote de evento processado com sucesso
                {
                    switch (recepcaoEvento.Result.RetEvento[0].InfEvento.CStat)
                    {
                        case 135: //Evento homologado com vinculação da respectiva NFe
                        case 136: //Evento homologado sem vinculação com a respectiva NFe (SEFAZ não encontrou a NFe na base dela)
                        case 155: //Evento de Cancelamento homologado fora do prazo permitido para cancelamento
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;

                        default: //Evento rejeitado
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsStatServ
                {
                    Versao = "4.00",
                    CUF = UFBrasil.SE,
                    TpAmb = TipoAmbiente.Homologacao
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.NFCe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var statusServico = new Unimake.Business.DFe.Servicos.NFCe.StatusServico(xml, configuracao);
                statusServico.Executar();

                MessageBox.Show(statusServico.RetornoWSString);
                MessageBox.Show(statusServico.Result.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsSitNFe
                {
                    Versao = "4.00",
                    TpAmb = TipoAmbiente.Homologacao,
                    ChNFe = "31191222204648000384650010000001881000524048"
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.NFCe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var consultaProtocolo = new Unimake.Business.DFe.Servicos.NFCe.ConsultaProtocolo(xml, configuracao);
                consultaProtocolo.Executar();
                MessageBox.Show(consultaProtocolo.RetornoWSString);
                MessageBox.Show(consultaProtocolo.Result.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new InutNFe
                {
                    Versao = "4.00",
                    InfInut = new InutNFeInfInut
                    {
                        Ano = "19",
                        CNPJ = "06117473000150",
                        CUF = UFBrasil.MS,
                        Mod = ModeloDFe.NFCe,
                        NNFIni = 57919,
                        NNFFin = 57919,
                        Serie = 1,
                        TpAmb = TipoAmbiente.Producao,
                        XJust = "Justificativa da inutilizacao de teste"
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.NFCe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var inutilizacao = new Unimake.Business.DFe.Servicos.NFCe.Inutilizacao(xml, configuracao);
                inutilizacao.Executar();
                MessageBox.Show(inutilizacao.RetornoWSString);
                MessageBox.Show(inutilizacao.Result.InfInut.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (inutilizacao.Result.InfInut.CStat)
                {
                    case 102: //Inutilização homologada
                        inutilizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                        break;

                    default: //Inutilização rejeitada
                        inutilizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new Unimake.Business.DFe.Xml.NFe.ConsCad
                {
                    Versao = "2.00",
                    InfCons = new InfCons()
                    {
                        CNPJ = "06117473000150",
                        UF = UFBrasil.PR,
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.NFCe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var consultaCad = new Unimake.Business.DFe.Servicos.NFCe.ConsultaCadastro(xml, configuracao);
                consultaCad.Executar();
                MessageBox.Show(consultaCad.RetornoWSString);
                MessageBox.Show(consultaCad.Result.InfCons.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EnvEvento
                {
                    Versao = "1.00",
                    IdLote = "000000000000001",
                    Evento = new List<Evento> {
                        new Evento
                        {
                            Versao = "1.00",
                            InfEvento = new Unimake.Business.DFe.Xml.NFe.InfEvento(new Unimake.Business.DFe.Xml.NFe.DetEventoCanc
                            {
                                NProt = "141190000660363",
                                Versao = "1.00",
                                XJust = "Justificativa para cancelamento da NFe de teste"
                            })
                            {
                                COrgao = UFBrasil.PR,
                                ChNFe = "41190806117473000150650010000579131943463890",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.Cancelamento,
                                NSeqEvento = 1,
                                VerEvento = "1.00",
                                TpAmb = TipoAmbiente.Homologacao
                            }
                        }
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.NFCe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var recepcaoEvento = new Unimake.Business.DFe.Servicos.NFCe.RecepcaoEvento(xml, configuracao);
                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                if (recepcaoEvento.Result.CStat == 128) //128 = Lote de evento processado com sucesso
                {
                    switch (recepcaoEvento.Result.RetEvento[0].InfEvento.CStat)
                    {
                        case 135: //Evento homologado com vinculação da respectiva NFe
                        case 136: //Evento homologado sem vinculação com a respectiva NFe (SEFAZ não encontrou a NFe na base dela)
                        case 155: //Evento de Cancelamento homologado fora do prazo permitido para cancelamento
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;

                        default: //Evento rejeitado
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EnvEvento
                {
                    Versao = "1.00",
                    IdLote = "000000000000001",
                    Evento = new List<Evento> {
                        new Evento
                        {
                            Versao = "1.00",
                            InfEvento = new Unimake.Business.DFe.Xml.NFe.InfEvento(new Unimake.Business.DFe.Xml.NFe.DetEventoCCE
                            {
                                Versao = "1.00",
                                XCorrecao = "CFOP errada, segue CFOP correta."
                            })
                            {
                                COrgao = UFBrasil.PR,
                                ChNFe = "41191006117473000150650010000579281779843610",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.CartaCorrecao,
                                NSeqEvento = 3,
                                VerEvento = "1.00",
                                TpAmb = TipoAmbiente.Homologacao
                            }
                        },
                        new Evento
                        {
                            Versao = "1.00",
                            InfEvento = new Unimake.Business.DFe.Xml.NFe.InfEvento(new Unimake.Business.DFe.Xml.NFe.DetEventoCCE
                            {
                                Versao = "1.00",
                                XCorrecao = "Nome do transportador esta errado, segue nome correto."
                            })
                            {
                                COrgao = UFBrasil.PR,
                                ChNFe = "41191006117473000150650010000579281779843610",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.CartaCorrecao,
                                NSeqEvento = 4,
                                VerEvento = "1.00",
                                TpAmb = TipoAmbiente.Homologacao
                            }
                        }
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.NFCe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var recepcaoEvento = new Unimake.Business.DFe.Servicos.NFCe.RecepcaoEvento(xml, configuracao);
                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                if (recepcaoEvento.Result.CStat == 128) //128 = Lote de evento processado com sucesso
                {
                    switch (recepcaoEvento.Result.RetEvento[0].InfEvento.CStat)
                    {
                        case 135: //Evento homologado com vinculação da respectiva NFe
                        case 136: //Evento homologado sem vinculação com a respectiva NFe (SEFAZ não encontrou a NFe na base dela)
                        case 155: //Evento de Cancelamento homologado fora do prazo permitido para cancelamento
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;

                        default: //Evento rejeitado
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            try
            {
                //var xml = new EnviNFe
                //{
                //    Versao = "4.00",
                //    IdLote = "000000000000001",
                //    IndSinc = SimNao.Sim,
                //    NFe = new List<NFe> {
                //        new NFe
                //        {
                //            InfNFe = new List<Unimake.Business.DFe.Xml.NFe.InfNFe> {
                //                new Unimake.Business.DFe.Xml.NFe.InfNFe
                //                {
                //                    Versao = "4.00",

                //                    Ide = new Unimake.Business.DFe.Xml.NFe.Ide
                //                    {
                //                        CUF = UFBrasil.MG,
                //                        NatOp = "VENDA PRODUC.DO ESTABELEC",
                //                        Mod = ModeloDFe.NFCe,
                //                        Serie = 1,
                //                        NNF = 57931,
                //                        DhEmi = DateTime.Now,
                //                        TpNF = TipoOperacao.Saida,
                //                        IdDest = DestinoOperacao.OperacaoInterna,
                //                        CMunFG = 4118402,
                //                        TpImp = FormatoImpressaoDANFE.NFCe,
                //                        TpEmis = TipoEmissao.ContingenciaOffLine,
                //                        TpAmb = TipoAmbiente.Homologacao,
                //                        FinNFe = FinalidadeNFe.Normal,
                //                        IndFinal = SimNao.Sim,
                //                        IndPres = IndicadorPresenca.OperacaoPresencial,
                //                        ProcEmi = ProcessoEmissao.AplicativoContribuinte,
                //                        VerProc = "TESTE 1.00"
                //                    },
                //                    Emit = new Unimake.Business.DFe.Xml.NFe.Emit
                //                    {
                //                        CNPJ = "06117473000150",
                //                        XNome = "UNIMAKE SOLUCOES CORPORATIVAS LTDA",
                //                        XFant = "UNIMAKE - PARANAVAI",
                //                        EnderEmit = new Unimake.Business.DFe.Xml.NFe.EnderEmit
                //                        {
                //                            XLgr = "RUA ANTONIO FELIPE",
                //                            Nro = "1500",
                //                            XBairro = "CENTRO",
                //                            CMun = 4118402,
                //                            XMun = "PARANAVAI",
                //                            UF = UFBrasil.MG,
                //                            CEP = "87704030",
                //                            Fone = "04431414900"
                //                        },
                //                        IE = "9032000301",
                //                        IM = "14018",
                //                        CNAE = "6202300",
                //                        CRT = CRT.SimplesNacional
                //                    },
                //                    Det = CriarDet(),
                //                    Total = new Total
                //                    {
                //                        ICMSTot = new ICMSTot
                //                        {
                //                            VBC = 0,
                //                            VICMS = 0,
                //                            VICMSDeson = 0,
                //                            VFCP = 0,
                //                            VBCST = 0,
                //                            VST = 0,
                //                            VFCPST = 0,
                //                            VFCPSTRet = 0,
                //                            VProd = 84.90,
                //                            VFrete = 0,
                //                            VSeg = 0,
                //                            VDesc = 0,
                //                            VII = 0,
                //                            VIPI = 0,
                //                            VIPIDevol = 0,
                //                            VPIS = 0,
                //                            VCOFINS = 0,
                //                            VOutro = 0,
                //                            VNF = 84.90,
                //                            VTotTrib = 12.63
                //                        }
                //                    },
                //                    Transp = new Transp
                //                    {
                //                        ModFrete = ModalidadeFrete.SemOcorrenciaTransporte
                //                    },
                //                    Cobr = new Unimake.Business.DFe.Xml.NFe.Cobr()
                //                    {
                //                        Fat = new Unimake.Business.DFe.Xml.NFe.Fat
                //                        {
                //                            NFat = "057910",
                //                            VOrig = 84.90,
                //                            VDesc = 0,
                //                            VLiq = 84.90
                //                        },
                //                        Dup = new List<Unimake.Business.DFe.Xml.NFe.Dup>
                //                        {
                //                            new Unimake.Business.DFe.Xml.NFe.Dup
                //                            {
                //                                NDup = "001",
                //                                DVenc = DateTime.Now,
                //                                VDup = 84.90
                //                            },
                //                            new Unimake.Business.DFe.Xml.NFe.Dup
                //                            {
                //                                NDup = "002",
                //                                DVenc = DateTime.Now,
                //                                VDup = 84.90
                //                            }
                //                        }
                //                    },
                //                    Pag = new Pag
                //                    {
                //                        DetPag = new  List<DetPag>
                //                        {
                //                             new DetPag
                //                             {
                //                                 IndPag = IndicadorPagamento.PagamentoVista,
                //                                 TPag = MeioPagamento.Outros,
                //                                 VPag = 84.90
                //                             },
                //                             new DetPag
                //                             {
                //                                 IndPag = IndicadorPagamento.PagamentoPrazo,
                //                                 TPag = MeioPagamento.Outros,
                //                                 VPag = 84.90,
                //                                 //Card = new Card
                //                                 //{
                //                                 //    TpIntegra = TipoIntegracaoPagamento.PagamentoIntegrado,
                //                                 //    CAut = "",
                //                                 //    CNPJ = "",
                //                                 //    TBand = BandeiraOperadoraCartao.Cabal
                //                                 //},                                                 
                //                             }
                //                        }
                //                    },
                //                    InfAdic = new Unimake.Business.DFe.Xml.NFe.InfAdic
                //                    {
                //                        InfCpl = ";CONTROLE: 0000241197;PEDIDO(S) ATENDIDO(S): 300474;Empresa optante pelo simples nacional, conforme lei compl. 128 de 19/12/2008;Permite o aproveitamento do credito de ICMS no valor de R$ 2,40, correspondente ao percentual de 2,83% . Nos termos do Art. 23 - LC 123/2006 (Resolucoes CGSN n. 10/2007 e 53/2008);Voce pagou aproximadamente: R$ 6,69 trib. federais / R$ 5,94 trib. estaduais / R$ 0,00 trib. municipais. Fonte: IBPT/empresometro.com.br 18.2.B A3S28F;",
                //                    },
                //                    InfRespTec = new Unimake.Business.DFe.Xml.NFe.InfRespTec
                //                    {
                //                        CNPJ = "06117473000150",
                //                        XContato = "Wandrey Mundin Ferreira",
                //                        Email = "wandrey@unimake.com.br",
                //                        Fone = "04431414900"
                //                    }
                //                }
                //            }
                //        }
                //    }
                //};

                var xml = new EnviNFe
                {
                    Versao = "4.00",
                    IdLote = "000000000000001",
                    IndSinc = SimNao.Sim,
                    NFe = new List<NFe> {
                        new NFe
                        {
                            InfNFe = new List<Unimake.Business.DFe.Xml.NFe.InfNFe> {
                                new Unimake.Business.DFe.Xml.NFe.InfNFe
                                {
                                    Versao = "4.00",

                                    Ide = new Ide
                                    {
                                        CUF = UFBrasil.PR,
                                        NatOp = "VENDA PRODUC.DO ESTABELEC",
                                        Mod = ModeloDFe.NFCe,
                                        Serie = 1,
                                        NNF = 57931,
                                        DhEmi = DateTime.Now,
                                        DhSaiEnt = DateTime.Now,
                                        TpNF = TipoOperacao.Saida,
                                        IdDest = DestinoOperacao.OperacaoInterna,
                                        CMunFG = 4118402,
                                        TpImp = FormatoImpressaoDANFE.NFCe,
                                        TpEmis = TipoEmissao.Normal,
                                        TpAmb = TipoAmbiente.Homologacao,
                                        FinNFe = FinalidadeNFe.Normal,
                                        IndFinal = SimNao.Sim,
                                        IndPres = IndicadorPresenca.OperacaoPresencial,
                                        ProcEmi = ProcessoEmissao.AplicativoContribuinte,
                                        VerProc = "TESTE 1.00"
                                    },
                                    Emit = new Emit
                                    {
                                        CNPJ = "06117473000150",
                                        XNome = "UNIMAKE SOLUCOES CORPORATIVAS LTDA",
                                        XFant = "UNIMAKE - PARANAVAI",
                                        EnderEmit = new EnderEmit
                                        {
                                            XLgr = "RUA ANTONIO FELIPE",
                                            Nro = "1500",
                                            XBairro = "CENTRO",
                                            CMun = 4118402,
                                            XMun = "PARANAVAI",
                                            UF = UFBrasil.PR,
                                            CEP = "87704030",
                                            Fone = "04431414900"
                                        },
                                        IE = "9032000301",
                                        IM = "14018",
                                        CNAE = "6202300",
                                        CRT = CRT.SimplesNacional
                                    },
                                    Det = new List<Det> {
                                        new Det
                                        {
                                            NItem = 1,
                                            Prod = new Prod
                                            {
                                                CProd = "01042",
                                                CEAN = "SEM GTIN",
                                                XProd = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                                                NCM = "84714900",
                                                CFOP = "5101",
                                                UCom = "LU",
                                                QCom = 1.00m,
                                                VUnCom = 84.9000000000M,
                                                VProd = 84.90,
                                                CEANTrib = "SEM GTIN",
                                                UTrib = "LU",
                                                QTrib = 1.00m,
                                                VUnTrib = 84.9M,
                                                IndTot = SimNao.Sim,
                                                XPed = "300474",
                                                NItemPed = 1
                                            },
                                            Imposto = new Imposto
                                            {
                                                VTotTrib = 12.63,
                                                ICMS = new List<ICMS> {
                                                    new ICMS
                                                    {
                                                        ICMSSN101 = new ICMSSN101
                                                        {
                                                            Orig = OrigemMercadoria.Nacional,
                                                            PCredSN = 2.8255,
                                                            VCredICMSSN = 2.40
                                                        }
                                                    }
                                                },
                                                PIS = new PIS
                                                {
                                                    PISOutr = new PISOutr
                                                    {
                                                        CST = "99",
                                                        VBC = 0.00,
                                                        PPIS = 0.00,
                                                        VPIS = 0.00
                                                    }
                                                },
                                                COFINS = new COFINS
                                                {
                                                    COFINSOutr = new COFINSOutr
                                                    {
                                                        CST = "99",
                                                        VBC = 0.00,
                                                        PCOFINS = 0.00,
                                                        VCOFINS = 0.00
                                                    }
                                                }
                                            }
                                        },
                                        new Det
                                        {
                                            NItem = 2,
                                            Prod = new Prod
                                            {
                                                CProd = "11111",
                                                CEAN = "SEM GTIN",
                                                XProd = "TESTE DO PRODUTO DO ITEM 2",
                                                NCM = "99999999", //"84714900",
                                                CFOP = "5101",
                                                UCom = "LU",
                                                QCom = 1.00m,
                                                VUnCom = 84.9000000000M,
                                                VProd = 84.90,
                                                CEANTrib = "SEM GTIN",
                                                UTrib = "LU",
                                                QTrib = 1.00m,
                                                VUnTrib = 84.9M,
                                                IndTot = SimNao.Sim,
                                                XPed = "300474",
                                                NItemPed = 1
                                            },
                                            Imposto = new Imposto
                                            {
                                                VTotTrib = 12.63,
                                                ICMS = new List<ICMS> {
                                                    new ICMS
                                                    {
                                                        ICMSSN101 = new ICMSSN101
                                                        {
                                                            Orig = OrigemMercadoria.Nacional,
                                                            PCredSN = 2.8255,
                                                            VCredICMSSN = 2.40
                                                        }
                                                    }
                                                },
                                                PIS = new PIS
                                                {
                                                    PISOutr = new PISOutr
                                                    {
                                                        CST = "99",
                                                        VBC = 0.00,
                                                        PPIS = 0.00,
                                                        VPIS = 0.00
                                                    }
                                                },
                                                COFINS = new COFINS
                                                {
                                                    COFINSOutr = new COFINSOutr
                                                    {
                                                        CST = "99",
                                                        VBC = 0.00,
                                                        PCOFINS = 0.00,
                                                        VCOFINS = 0.00
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    Total = new Total
                                    {
                                        ICMSTot = new ICMSTot
                                        {
                                            VBC = 0,
                                            VICMS = 0,
                                            VICMSDeson = 0,
                                            VFCP = 0,
                                            VBCST = 0,
                                            VST = 0,
                                            VFCPST = 0,
                                            VFCPSTRet = 0,
                                            VProd = 84.90,
                                            VFrete = 0,
                                            VSeg = 0,
                                            VDesc = 0,
                                            VII = 0,
                                            VIPI = 0,
                                            VIPIDevol = 0,
                                            VPIS = 0,
                                            VCOFINS = 0,
                                            VOutro = 0,
                                            VNF = 84.90,
                                            VTotTrib = 12.63
                                        }
                                    },
                                    Transp = new Transp
                                    {
                                        ModFrete = ModalidadeFrete.SemOcorrenciaTransporte
                                    },
                                    Cobr = new Unimake.Business.DFe.Xml.NFe.Cobr()
                                    {
                                        Fat = new Fat
                                        {
                                            NFat = "057910",
                                            VOrig = 84.90,
                                            VDesc = 0,
                                            VLiq = 84.90
                                        },
                                        Dup = new List<Dup>
                                        {
                                            new Dup
                                            {
                                                NDup = "001",
                                                DVenc = DateTime.Now,
                                                VDup = 84.90
                                            }
                                        }
                                    },
                                    Pag = new Pag
                                    {
                                        DetPag = new List<DetPag>
                                        {
                                             new DetPag
                                             {
                                                 IndPag = IndicadorPagamento.PagamentoVista,
                                                 TPag = MeioPagamento.Dinheiro,
                                                 VPag = 84.90
                                             }
                                        }
                                    },
                                    InfAdic = new InfAdic
                                    {
                                        InfCpl = ";CONTROLE: 0000241197;PEDIDO(S) ATENDIDO(S): 300474;Empresa optante pelo simples nacional, conforme lei compl. 128 de 19/12/2008;Permite o aproveitamento do credito de ICMS no valor de R$ 2,40, correspondente ao percentual de 2,83% . Nos termos do Art. 23 - LC 123/2006 (Resolucoes CGSN n. 10/2007 e 53/2008);Voce pagou aproximadamente: R$ 6,69 trib. federais / R$ 5,94 trib. estaduais / R$ 0,00 trib. municipais. Fonte: IBPT/empresometro.com.br 18.2.B A3S28F;",
                                    },
                                    InfRespTec = new InfRespTec
                                    {
                                        CNPJ = "06117473000150",
                                        XContato = "Wandrey Mundin Ferreira",
                                        Email = "wandrey@unimake.com.br",
                                        Fone = "04431414900"
                                    }
                                }
                            }
                        }
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.NFCe,
                    CertificadoDigital = CertificadoSelecionado,
                    CSC = "HCJBIRTWGCQ3HVQN7DCA0ZY0P2NYT6FVLPJG",
                    CSCIDToken = 2
                };

                var chaveNFe = xml.NFe[0].InfNFe[0].Chave;
                var NumeroNota = xml.NFe[0].InfNFe[0].Ide.NNF;

                var autorizacao = new Unimake.Business.DFe.Servicos.NFCe.Autorizacao(xml, configuracao);
                var qq = autorizacao.ConteudoXMLAssinado;
                autorizacao.Executar();

                MessageBox.Show(autorizacao.RetornoWSString);
                MessageBox.Show(autorizacao.Result.XMotivo);

                //Gravar o XML de distribuição se a nota foi autorizada ou denegada
                if (autorizacao.Result.ProtNFe != null)
                {
                    switch (autorizacao.Result.ProtNFe.InfProt.CStat)
                    {
                        case 100: //Autorizado o uso da NF-e
                        case 110: //Uso Denegado
                        case 150: //Autorizado o uso da NF-e, autorização fora de prazo
                        case 205: //NF-e está denegada na base de dados da SEFAZ [nRec:999999999999999]
                        case 301: //Uso Denegado: Irregularidade fiscal do emitente
                        case 302: //Uso Denegado: Irregularidade fiscal do destinatário
                        case 303: //Uso Denegado: Destinatário não habilitado a operar na UF
                            MessageBox.Show(autorizacao.NfeProcResult.NomeArquivoDistribuicao);

                            autorizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;

                        default: //NF Rejeitada
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsReciNFe
                {
                    Versao = "4.00",
                    TpAmb = TipoAmbiente.Homologacao,
                    NRec = "503456789012345"
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.NFCe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var retAutorizacao = new Unimake.Business.DFe.Servicos.NFCe.RetAutorizacao(xml, configuracao);
                retAutorizacao.Executar();

                MessageBox.Show(retAutorizacao.RetornoWSString);
                MessageBox.Show(retAutorizacao.Result.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsSitNFe
                {
                    Versao = "4.00",
                    TpAmb = TipoAmbiente.Homologacao,
                    ChNFe = "41200106117473000150550010000606641403753210"
                };

                var configuracao = new Configuracao
                {
                    CertificadoDigital = CertificadoSelecionado
                };

                var consultaProtocolo = new ConsultaProtocolo(xml, configuracao);
                consultaProtocolo.Executar();
                MessageBox.Show(consultaProtocolo.RetornoWSString);
                if (consultaProtocolo.Result.CStat == 100)
                {
                    if (consultaProtocolo.Result.ProtNFe != null) //Retornou o protocolo de autorização
                    {
                        MessageBox.Show(consultaProtocolo.Result.ProtNFe.InfProt.NProt); //Demonstrar o protocolo de autorização
                    }
                    else
                    {
                        MessageBox.Show(consultaProtocolo.Result.XMotivo);
                    }
                }
                else
                {
                    MessageBox.Show(consultaProtocolo.Result.XMotivo);
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new InutCTe
                {
                    Versao = "3.00",
                    InfInut = new InutCTeInfInut
                    {
                        Ano = "19",
                        CNPJ = "06117473000150",
                        CUF = UFBrasil.PR,
                        Mod = ModeloDFe.CTe,
                        NCTIni = 57919,
                        NCTFin = 57919,
                        Serie = 1,
                        TpAmb = TipoAmbiente.Homologacao,
                        XJust = "Justificativa da inutilizacao de teste"
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var inutilizacao = new Unimake.Business.DFe.Servicos.CTe.Inutilizacao(xml, configuracao);
                inutilizacao.Executar();
                MessageBox.Show(inutilizacao.RetornoWSString);
                MessageBox.Show(inutilizacao.Result.InfInut.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (inutilizacao.Result.InfInut.CStat)
                {
                    case 102: //Inutilização homologada
                        inutilizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                        break;

                    default: //Inutilização rejeitada
                        inutilizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new Unimake.Business.DFe.Xml.CTe.ConsCad
                {
                    Versao = "2.00",
                    InfCons = new InfCons()
                    {
                        CNPJ = "06117473000150",
                        UF = UFBrasil.PR
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var consultaCad = new Unimake.Business.DFe.Servicos.CTe.ConsultaCadastro(xml, configuracao);
                consultaCad.Executar();
                MessageBox.Show(consultaCad.RetornoWSString);
                MessageBox.Show(consultaCad.Result.InfCons.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            var nsu = "000000000000000";
            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.CTe,
                CertificadoDigital = CertificadoSelecionado
            };

            pbConsultaDFe.Visible = true;
            pbConsultaDFe.Minimum = 0;
            Application.DoEvents();
            pbConsultaDFe.Refresh();

            while (true)
            {
                try
                {
                    var xml = new Unimake.Business.DFe.Xml.CTe.DistDFeInt
                    {
                        Versao = "1.00",
                        TpAmb = TipoAmbiente.Producao,
                        CNPJ = "31905001000109",
                        CUFAutor = UFBrasil.PR,
                        DistNSU = new Unimake.Business.DFe.Xml.CTe.DistNSU
                        {
                            UltNSU = nsu
                        }
                    };

                    var distribuicaoDFe = new Unimake.Business.DFe.Servicos.CTe.DistribuicaoDFe(xml, configuracao);
                    distribuicaoDFe.Executar();

                    #region Atualizar ProgressBar

                    if (pbConsultaDFe.Maximum != Convert.ToInt32(distribuicaoDFe.Result.MaxNSU))
                    {
                        pbConsultaDFe.Maximum = Convert.ToInt32(distribuicaoDFe.Result.MaxNSU);
                    }

                    pbConsultaDFe.Value = Convert.ToInt32(distribuicaoDFe.Result.UltNSU);
                    pbConsultaDFe.Refresh();
                    Application.DoEvents();

                    #endregion Atualizar ProgressBar

                    if (distribuicaoDFe.Result.CStat.Equals(138)) //Documentos localizados
                    {
                        var folder = @"c:\testenfe\doczipcte";

                        if (Environment.MachineName == "MARCELO-PC")
                        {
                            folder = @"D:\temp\uninfe";
                        }

                        //Salvar os XMLs do docZIP no HD
                        distribuicaoDFe.GravarXMLDocZIP(folder);
                    }

                    nsu = distribuicaoDFe.Result.UltNSU;

                    if (Convert.ToInt64(distribuicaoDFe.Result.UltNSU) >= Convert.ToInt64(distribuicaoDFe.Result.MaxNSU))
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    CatchException(ex);
                }
            }

            pbConsultaDFe.Visible = false;
            Application.DoEvents();

            MessageBox.Show("Consulta finalizada.");
        }

        private void button24_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsSitMDFe
                {
                    Versao = "3.00",
                    TpAmb = TipoAmbiente.Homologacao,
                    ChMDFe = "31220437920833000180580010000000021000000020"
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.MDFe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var consultaProtocolo = new Unimake.Business.DFe.Servicos.MDFe.ConsultaProtocolo(xml, configuracao);
                consultaProtocolo.Executar();

                //Pra pegar se o MDFe foi autorizado ou não pela consulta protocolo
                if (consultaProtocolo.Result.ProtMDFe.InfProt.CStat == 100) //MDFe autorizado
                {
                    //Pegar o protocolo de autorização do MDFe para salvar em base de dados
                    var protocoloDeAutorizacaoMDFe = consultaProtocolo.Result.ProtMDFe.InfProt.NProt;

                    //Procurar eventos retornados na consulta protocolo
                    //for (int i = 0; i < consultaProtocolo.Result.GetProcEventoMDFeCount; i++)
                    for (var i = 0; i < consultaProtocolo.Result.ProcEventoMDFe.Count; i++)
                    {
                        var procEventoMDFe = consultaProtocolo.Result.ProcEventoMDFe[i];

                        if (procEventoMDFe.EventoMDFe.InfEvento.TpEvento == TipoEventoMDFe.Cancelamento) //Evento de cancelamento
                        {
                            //Pegar o protocolo de cancelamento do MDFe para salvar na base de dados
                            var protocoloCancelamento = procEventoMDFe.RetEventoMDFe.InfEvento.NProt;
                        }
                        else if (procEventoMDFe.EventoMDFe.InfEvento.TpEvento == TipoEventoMDFe.Encerramento) //Evento de encerramento
                        {
                            //Pegar o protocolo de cancelamento do MDFe para salvar na base de dados
                            var protocoloCancelamento = procEventoMDFe.RetEventoMDFe.InfEvento.NProt;
                        }
                    }
                }

                MessageBox.Show(consultaProtocolo.RetornoWSString);
                MessageBox.Show(consultaProtocolo.Result.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsStatServMDFe
                {
                    Versao = "3.00",
                    TpAmb = TipoAmbiente.Homologacao
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.MDFe,
                    CodigoUF = (int)UFBrasil.SE,
                    CertificadoDigital = CertificadoSelecionado
                };

                var statusServico = new Unimake.Business.DFe.Servicos.MDFe.StatusServico(xml, configuracao);
                statusServico.Executar();
                MessageBox.Show(statusServico.RetornoWSString);
                MessageBox.Show(statusServico.Result.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            try
            {
                //var xmlRetConsSitCTe = new RetConsSitCTe
                //{
                //    TpAmb = TipoAmbiente.Producao,
                //    VerAplic = "PR-v3_1_26",
                //    CStat = 101,
                //    XMotivo = "Cancelamento de CT-e homologado",
                //    CUF = UFBrasil.PR,
                //    ProtCTe = new ProtCTe
                //    {
                //        Versao = "3.00",
                //        InfProt = new Unimake.Business.DFe.Xml.CTe.InfProt
                //        {
                //            TpAmb = TipoAmbiente.Producao,
                //            VerAplic = "PR-v3_1_26",
                //            ChCTe = "41210180568835000181570010000007221820184779",
                //            DhRecbtoField = "2021-01-20T16:38:30-03:00",
                //            NProt = "141210008271578",
                //            DigVal = "6iI9oC1Ti7f4Gj/IquVfDemGwtg=",
                //            CStat = 100,
                //            XMotivo = "Autorizado o uso do CT-e"
                //        }
                //    },
                //    ProcEventoCTe = new List<Unimake.Business.DFe.Xml.XMLBase>
                //    {
                //        new ProcEventoCTe<InfEventoCanc>
                //        {
                //            Versao = "3.00",
                //            EventoCTe = new EventoCTe<InfEventoCanc>
                //            {
                //                Versao = "3.00",
                //                InfEvento = new InfEventoCanc
                //                {
                //                    COrgao = UFBrasil.PR,
                //                    TpAmb = TipoAmbiente.Producao,
                //                    CNPJ = "80568835000181",
                //                    ChCTe = "41210180568835000181570010000007221820184779",
                //                    DhEventoField = "2021-01-21T10:41:20-03:00",
                //                    TpEvento = TipoEventoCTe.Cancelamento,
                //                    NSeqEvento = 1,
                //                    DetEvento = new Unimake.Business.DFe.Xml.CTe.DetEventoCanc
                //                    {
                //                        VersaoEvento = "3.00",
                //                        EvCancCTe = new EvCancCTe
                //                        {
                //                            DescEvento = "Cancelamento",
                //                            NProt = "141210008271578",
                //                            XJust = "nao vai mais o pedido, foi cancelado"
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //};

                //var doc = XMLUtility.Serializar<RetConsSitCTe>(xmlRetConsSitCTe);

                var doc = new XmlDocument();
                doc.LoadXml(File.ReadAllText(@"C:\Users\Wandrey\Downloads\Modelos XML\CTe Situação\41210180568835000181570010000007221820184779-sit.xml", Encoding.UTF8));

                var xmlTeste = new RetConsSitCTe();
                var qq = xmlTeste.LerXML<RetConsSitCTe>(doc);

                var xml = new ConsSitCTe
                {
                    Versao = "3.00",
                    TpAmb = TipoAmbiente.Producao,
                    ChCTe = "41210180568835000181570010000007221820184779"
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var consultaProtocolo = new Unimake.Business.DFe.Servicos.CTe.ConsultaProtocolo(xml, configuracao);
                consultaProtocolo.Executar();

                MessageBox.Show(consultaProtocolo.RetornoWSString);
                MessageBox.Show(consultaProtocolo.Result.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button27_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsStatServCte
                {
                    Versao = "3.00",
                    TpAmb = TipoAmbiente.Homologacao
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTe,
                    CodigoUF = (int)UFBrasil.SE,
                    TipoEmissao = TipoEmissao.Normal,
                    CertificadoDigital = CertificadoSelecionado
                };

                var statusServico = new Unimake.Business.DFe.Servicos.CTe.StatusServico(xml, configuracao);
                statusServico.Executar();
                MessageBox.Show(statusServico.RetornoWSString);
                MessageBox.Show(statusServico.Result.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new InutNFe
                {
                    Versao = "4.00",
                    InfInut = new InutNFeInfInut
                    {
                        Ano = "19",
                        CNPJ = "06117473000150",
                        CUF = UFBrasil.MS,
                        Mod = ModeloDFe.NFe,
                        NNFIni = 57919,
                        NNFFin = 57919,
                        Serie = 1,
                        TpAmb = TipoAmbiente.Homologacao,
                        XJust = "Justificativa da inutilizacao de teste"
                    }
                };

                var configuracao = new Configuracao
                {
                    CertificadoDigital = CertificadoSelecionado
                };

                var inutilizacao = new Inutilizacao(xml, configuracao);
                inutilizacao.Executar();

                MessageBox.Show(inutilizacao.RetornoWSString);
                MessageBox.Show(inutilizacao.Result.InfInut.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (inutilizacao.Result.InfInut.CStat)
                {
                    case 102: //Inutilização homologada
                        inutilizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                        break;

                    default: //Inutilização rejeitada
                        inutilizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new Unimake.Business.DFe.Xml.NFe.ConsCad
                {
                    Versao = "2.00",
                    InfCons = new InfCons()
                    {
                        CNPJ = "06117473000150",
                        UF = UFBrasil.PR
                    }
                };

                var configuracao = new Configuracao
                {
                    CertificadoDigital = CertificadoSelecionado
                };

                var consultaCad = new ConsultaCadastro(xml, configuracao);
                consultaCad.Executar();
                MessageBox.Show(consultaCad.RetornoWSString);
                MessageBox.Show(consultaCad.Result.InfCons.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            try
            {

                var xml = new EnvEvento
                {
                    Versao = "1.00",
                    IdLote = "000000000000001",
                    Evento = new List<Evento> {
                        new Evento
                        {
                            Versao = "1.00",
                            InfEvento = new Unimake.Business.DFe.Xml.NFe.InfEvento(new Unimake.Business.DFe.Xml.NFe.DetEventoCanc
                            {
                                NProt = "141190000660363",
                                Versao = "1.00",
                                XJust = "Justificativa para cancelamento da NFe de teste"
                            })
                            {
                                COrgao = UFBrasil.PR,
                                ChNFe = "41190806117473000150550010000579131943463890",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.Cancelamento,
                                NSeqEvento = 1,
                                VerEvento = "1.00",
                                TpAmb = TipoAmbiente.Homologacao
                            }
                        }
                    }
                };

                var configuracao = new Configuracao
                {
                    CertificadoDigital = CertificadoSelecionado
                };

                var recepcaoEvento = new RecepcaoEvento(xml, configuracao);
                recepcaoEvento.Executar();

                var xmlDistrib = new XmlDocument();
                xmlDistrib = recepcaoEvento.ProcEventoNFeResult[0].GerarXML();

                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.XMotivo);

                var qq = recepcaoEvento.ProcEventoNFeResult[0].GerarXML().OuterXml;



                //Gravar o XML de distribuição se a inutilização foi homologada
                if (recepcaoEvento.Result.CStat == 128) //128 = Lote de evento processado com sucesso
                {
                    switch (recepcaoEvento.Result.RetEvento[0].InfEvento.CStat)
                    {
                        case 135: //Evento homologado com vinculação da respectiva NFe
                        case 136: //Evento homologado sem vinculação com a respectiva NFe (SEFAZ não encontrou a NFe na base dela)
                        case 155: //Evento de Cancelamento homologado fora do prazo permitido para cancelamento
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;

                        default: //Evento rejeitado
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            //var testeExceptionInterop = new Unimake.Exceptions.ThrowHelper();

            try
            {
                var xml = new EnviNFe
                {
                    Versao = "4.00",
                    IdLote = "000000000000001",
                    IndSinc = SimNao.Sim,
                    NFe = new List<NFe> {
                        new NFe
                        {
                            InfNFe = new List<Unimake.Business.DFe.Xml.NFe.InfNFe> {
                                new Unimake.Business.DFe.Xml.NFe.InfNFe
                                {
                                    Versao = "4.00",

                                    Ide = new Ide
                                    {
                                        CUF = (UFBrasil)Enum.Parse(typeof(UFBrasil),"SP"),
                                        NatOp = "VENDA PRODUC.DO ESTABELEC",
                                        Mod = ModeloDFe.NFe,
                                        Serie = 1,
                                        NNF = 57971,
                                        DhEmi = DateTime.Now,
                                        DhSaiEnt = DateTime.Now,
                                        TpNF = TipoOperacao.Saida,
                                        IdDest = DestinoOperacao.OperacaoInterestadual,
                                        CMunFG = 4118402,
                                        TpImp = FormatoImpressaoDANFE.NormalRetrato,
                                        TpEmis = TipoEmissao.Normal,
                                        TpAmb = TipoAmbiente.Homologacao,
                                        FinNFe = FinalidadeNFe.Normal,
                                        IndFinal = SimNao.Sim,
                                        IndPres = IndicadorPresenca.OperacaoPresencial,
                                        ProcEmi = ProcessoEmissao.AplicativoContribuinte,
                                        VerProc = "TESTE 1.00",
                                        NFref = new List<NFref>
                                        {
                                            new NFref
                                            {
                                                RefNFe = "00000000000000000000000chavenfedevolvida00000000000000"
                                            },
                                            new NFref
                                            {
                                                RefNFe = "00000000000000000000000chavenfedevolvida00000000000000"
                                            },
                                            new NFref
                                            {
                                                RefNFe = "00000000000000000000000chavenfedevolvida00000000000000"
                                            },
                                        }

                                    },
                                    Emit = new Emit
                                    {
                                        CNPJ = "06117473000150",
                                        XNome = "UNIMAKE SOLUCOES CORPORATIVAS LTDA",
                                        XFant = "UNIMAKE - PARANAVAI",
                                        EnderEmit = new EnderEmit
                                        {
                                            XLgr = "RUA ANTONIO FELIPE",
                                            Nro = "1500",
                                            XBairro = "CENTRO",
                                            CMun = 4118402,
                                            XMun = "PARANAVAI",
                                            UF = UFBrasil.PR,
                                            CEP = "87704030",
                                            Fone = "04431414900"
                                        },
                                        IE = "9032000301",
                                        IM = "14018",
                                        CNAE = "6202300",
                                        CRT = CRT.SimplesNacional
                                    },
                                    Dest = new Dest
                                    {
                                        CNPJ = "04218457000128",
                                        XNome = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                                        EnderDest = new EnderDest
                                        {
                                            XLgr = "AVENIDA DA SAUDADE",
                                            Nro = "1555",
                                            XBairro = "CAMPOS ELISEOS",
                                            CMun = 3543402,
                                            XMun = "RIBEIRAO PRETO",
                                            UF = UFBrasil.SP,
                                            CEP = "14080000",
                                            Fone = "01639611500"
                                        },
                                        IndIEDest = IndicadorIEDestinatario.ContribuinteICMS,
                                        IE = "582614838110",
                                        Email = "janelaorp@janelaorp.com.br"
                                    },
                                    Det = new List<Det> {
                                        new Det
                                        {
                                            NItem = 1,
                                            Prod = new Prod
                                            {
                                                CProd = "01042",
                                                CEAN = "SEM GTIN",
                                                XProd = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                                                NCM = "847149000", //Tirar um zero do final
                                                CFOP = "6101",
                                                UCom = "LU",
                                                QCom = 1.00m,
                                                VUnCom = 84.9000000000M,
                                                VProd = 84.90,
                                                CEANTrib = "SEM GTIN",
                                                UTrib = "LU",
                                                QTrib = 1.00m,
                                                VUnTrib = 12536.3653003546M,
                                                IndTot = SimNao.Sim,
                                                XPed = "300474",
                                                NItemPed = 1
                                            },
                                            Imposto = new Imposto
                                            {
                                                VTotTrib = 12.63,
                                                ICMS = new List<ICMS> {
                                                    new ICMS
                                                    {
                                                        ICMSSN101 = new ICMSSN101
                                                        {
                                                            Orig = OrigemMercadoria.Nacional,
                                                            PCredSN = 2.8255,
                                                            VCredICMSSN = 2.40
                                                        }
                                                    }
                                                },
                                                PIS = new PIS
                                                {
                                                    PISOutr = new PISOutr
                                                    {
                                                        CST = "99",
                                                        VBC = 0.00,
                                                        PPIS = 0.00,
                                                        VPIS = 0.00
                                                    }
                                                },
                                                COFINS = new COFINS
                                                {
                                                    COFINSOutr = new COFINSOutr
                                                    {
                                                        CST = "99",
                                                        VBC = 0.00,
                                                        PCOFINS = 0.00,
                                                        VCOFINS = 0.00
                                                    }
                                                },
                                            },
                                            ImpostoDevol = new ImpostoDevol
                                            {
                                                PDevol = 0,
                                                IPI = new IPIDevol
                                                {
                                                    VIPIDevol = 0,
                                                }
                                            }
                                        },
                                        new Det
                                        {
                                            NItem = 2,
                                            Prod = new Prod
                                            {
                                                CProd = "11111",
                                                CEAN = "SEM GTIN",
                                                XProd = "TESTE DO PRODUTO DO ITEM 2",
                                                NCM = "84714900",
                                                CFOP = "6101",
                                                UCom = "LU",
                                                QCom = 1.00m,
                                                VUnCom = 84.9000000000M,
                                                VProd = 84.90,
                                                CEANTrib = "SEM GTIN",
                                                UTrib = "LU",
                                                QTrib = 1.00m,
                                                VUnTrib = 84.9M,
                                                IndTot = SimNao.Sim,
                                                XPed = "300474",
                                                NItemPed = 1
                                            },
                                            Imposto = new Imposto
                                            {
                                                VTotTrib = 12.63,
                                                ICMS = new List<ICMS> {
                                                    new ICMS
                                                    {
                                                        ICMSSN101 = new ICMSSN101
                                                        {
                                                            Orig = OrigemMercadoria.Nacional,
                                                            PCredSN = 2.8255,
                                                            VCredICMSSN = 2.40
                                                        }
                                                    }
                                                },
                                                PIS = new PIS
                                                {
                                                    PISOutr = new PISOutr
                                                    {
                                                        CST = "99",
                                                        VBC = 0.00,
                                                        PPIS = 0.00,
                                                        VPIS = 0.00
                                                    }
                                                },
                                                COFINS = new COFINS
                                                {
                                                    COFINSOutr = new COFINSOutr
                                                    {
                                                        CST = "99",
                                                        VBC = 0.00,
                                                        PCOFINS = 0.00,
                                                        VCOFINS = 0.00
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    Total = new Total
                                    {
                                        ICMSTot = new ICMSTot
                                        {
                                            VBC = 0,
                                            VICMS = 0,
                                            VICMSDeson = 0,
                                            VFCP = 0,
                                            VBCST = 0,
                                            VST = 0,
                                            VFCPST = 0,
                                            VFCPSTRet = 0,
                                            VProd = 169.80,
                                            VFrete = 0,
                                            VSeg = 0,
                                            VDesc = 0,
                                            VII = 0,
                                            VIPI = 0,
                                            VIPIDevol = 0,
                                            VPIS = 0,
                                            VCOFINS = 0,
                                            VOutro = 0,
                                            VNF = 169.80,
                                            VTotTrib = 25.26
                                        }
                                    },
                                    Transp = new Transp
                                    {
                                        ModFrete = ModalidadeFrete.ContratacaoFretePorContaRemetente_CIF,
                                        Vol = new List<Vol>
                                        {
                                            new Vol
                                            {
                                                QVol = 1,
                                                Esp = "LU",
                                                Marca = "UNIMAKE",
                                                PesoL = 0.000,
                                                PesoB = 0.000
                                            }
                                        }
                                    },
                                    Cobr = new Unimake.Business.DFe.Xml.NFe.Cobr()
                                    {
                                        Fat = new Fat
                                        {
                                            NFat = "057910",
                                            VOrig = 169.80,
                                            VDesc = 0,
                                            VLiq = 169.80
                                        },
                                        Dup = new List<Dup>
                                        {
                                            new Dup
                                            {
                                                NDup = "001",
                                                DVenc = DateTime.Now,
                                                VDup = 169.80
                                            }
                                        }
                                    },
                                    Pag = new Pag
                                    {
                                        DetPag = new List<DetPag>
                                        {
                                             new DetPag
                                             {
                                                 IndPag = IndicadorPagamento.PagamentoVista,
                                                 TPag = MeioPagamento.Dinheiro,
                                                 VPag = 169.80
                                             }
                                        }
                                    },
                                    InfAdic = new InfAdic
                                    {
                                        InfCpl = ";CONTROLE: 0000241197;PEDIDO(S) ATENDIDO(S): 300474;Empresa optante pelo simples nacional, conforme lei compl. 128 de 19/12/2008;Permite o aproveitamento do credito de ICMS no valor de R$ 2,40, correspondente ao percentual de 2,83% . Nos termos do Art. 23 - LC 123/2006 (Resolucoes CGSN n. 10/2007 e 53/2008);Voce pagou aproximadamente: R$ 6,69 trib. federais / R$ 5,94 trib. estaduais / R$ 0,00 trib. municipais. Fonte: IBPT/empresometro.com.br 18.2.B A3S28F;",
                                    },
                                    InfRespTec = new InfRespTec
                                    {
                                        CNPJ = "06117473000150",
                                        XContato = "Wandrey Mundin Ferreira",
                                        Email = "wandrey@unimake.com.br",
                                        Fone = "04431414900"
                                    }
                                }
                            }
                        }
                    }
                };

                var configuracao = new Configuracao
                {
                    CertificadoDigital = CertificadoSelecionado
                };

                var autorizacao = new Autorizacao(xml, configuracao);
                autorizacao.Executar();

                MessageBox.Show(autorizacao.RetornoWSString);
                MessageBox.Show(autorizacao.Result.XMotivo);

                //Gravar o arquivo do conteúdo retornado em uma pasta qualquer para ter em segurança. Pode-se também gravar na base de dados. Fica a critério de cada um.
                File.WriteAllText(@"c:\testenfe\retorno\nomearquivoretorno.xml", autorizacao.RetornoWSString);

                if (autorizacao.Result.ProtNFe != null)
                {
                    //Gravar o XML de distribuição se a nota foi autorizada ou denegada
                    switch (autorizacao.Result.ProtNFe.InfProt.CStat)
                    {
                        case 100: //Autorizado o uso da NF-e
                        case 110: //Uso Denegado
                        case 150: //Autorizado o uso da NF-e, autorização fora de prazo
                        case 205: //NF-e está denegada na base de dados da SEFAZ [nRec:999999999999999]
                        case 301: //Uso Denegado: Irregularidade fiscal do emitente
                        case 302: //Uso Denegado: Irregularidade fiscal do destinatário
                        case 303: //Uso Denegado: Destinatário não habilitado a operar na UF
                            autorizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                            var docProcNFe = autorizacao.NfeProcResult.GerarXML();
                            MessageBox.Show(autorizacao.NfeProcResult.NomeArquivoDistribuicao);

                            break;

                        default: //NF Rejeitada
                            autorizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(testeExceptionInterop.GetMessage());

                CatchException(ex);
            }
        }

        //TODO:  Nunca usado, pode apagar?
        //private List<Comb> CriarComb()
        //{
        //    var comb = new List<Comb>
        //    {

        //        //Primeiro Comb
        //        new Comb
        //        {
        //            CODIF = "",
        //            CProdANP = "",
        //            DescANP = ""
        //            //Demais tags..

        //        },

        //        //Segundo comb, etc...
        //        new Comb
        //        {
        //            CODIF = "",
        //            CProdANP = "",
        //            DescANP = ""
        //            //Demais tags..
        //        }
        //    };

        //    return comb;
        //}

        private void Button7_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsReciNFe
                {
                    Versao = "4.00",
                    TpAmb = TipoAmbiente.Homologacao,
                    NRec = "131210140351219"
                };

                var configuracao = new Configuracao
                {
                    CertificadoDigital = CertificadoSelecionado
                };

                var retAutorizacao = new RetAutorizacao(xml, configuracao);
                retAutorizacao.Executar();
                MessageBox.Show(retAutorizacao.RetornoWSString);
                MessageBox.Show(retAutorizacao.Result.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            var xml = new XmlDocument();
            xml.Load(@"C:\Users\Wandrey\Downloads\NFe Paraguai\FE_v150.xml");
            AssinaturaDigital.Assinar(xml, "rDE", "DE", CertificadoSelecionado, AlgorithmType.Sha256, true, "Id");
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            var nsu = "000000000000000";
            var configuracao = new Configuracao
            {
                CertificadoDigital = CertificadoSelecionado
            };

            pbConsultaDFe.Visible = true;
            pbConsultaDFe.Minimum = 0;
            Application.DoEvents();
            pbConsultaDFe.Refresh();

            while (true)
            {
                try
                {
                    var xml = new Unimake.Business.DFe.Xml.NFe.DistDFeInt
                    {
                        Versao = "1.35",
                        TpAmb = TipoAmbiente.Homologacao,
                        CNPJ = "06117473000150",
                        CUFAutor = UFBrasil.PR,
                        DistNSU = new Unimake.Business.DFe.Xml.NFe.DistNSU
                        {
                            UltNSU = nsu
                        }
                    };

                    var distribuicaoDFe = new DistribuicaoDFe(xml, configuracao);
                    distribuicaoDFe.Executar();

                    #region Atualizar ProgressBar

                    if (pbConsultaDFe.Maximum != Convert.ToInt32(distribuicaoDFe.Result.MaxNSU))
                    {
                        pbConsultaDFe.Maximum = Convert.ToInt32(distribuicaoDFe.Result.MaxNSU);
                    }

                    pbConsultaDFe.Value = Convert.ToInt32(distribuicaoDFe.Result.UltNSU);
                    pbConsultaDFe.Refresh();
                    Application.DoEvents();

                    #endregion Atualizar ProgressBar

                    if (distribuicaoDFe.Result.CStat.Equals(138)) //Documentos localizados
                    {
                        var folder = @"c:\testenfe\doczip";

                        if (Environment.MachineName == "MARCELO-PC")
                        {
                            folder = @"D:\temp\uninfe";
                        }

                        //Salvar os XMLs do docZIP no HD
                        distribuicaoDFe.GravarXMLDocZIP(folder, true);
                    }

                    nsu = distribuicaoDFe.Result.UltNSU;

                    if (Convert.ToInt64(distribuicaoDFe.Result.UltNSU) >= Convert.ToInt64(distribuicaoDFe.Result.MaxNSU))
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    CatchException(ex);
                }
            }

            pbConsultaDFe.Visible = false;
            Application.DoEvents();

            MessageBox.Show("Consulta finalizada.");
        }

        private void CatchException(Exception ex)
        {
            var message = new StringBuilder();

            do
            {
                message.AppendLine($"{ex.Message}\r\n");
                ex = ex.InnerException;
            } while (ex != null);

            MessageBox.Show(message.ToString(), "ERRO!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Dentro deste método o desenvolvedor pode abrir um loop na sua tabela de produtos da nota e adicionar
        /// vários produtos na tag Det do XML.
        ///
        /// No exemplo abaixo vou adicionar 2 produtos para se ter uma ideia.
        /// </summary>
        /// <returns></returns>
        private List<Det> CriarDet()
        {
            var dets = new List<Unimake.Business.DFe.Xml.NFe.Det>();


            for (var i = 0; i < 1; i++)
            {
                dets.Add(new Unimake.Business.DFe.Xml.NFe.Det
                {
                    NItem = i + 1,
                    Prod = new Unimake.Business.DFe.Xml.NFe.Prod
                    {
                        CProd = "01042",
                        CEAN = "SEM GTIN",
                        XProd = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                        NCM = "84714900",
                        CFOP = "6101",
                        UCom = "LU",
                        QCom = 1.00m,
                        VUnCom = 84.9000000000M,
                        VProd = 84.90,
                        CEANTrib = "SEM GTIN",
                        UTrib = "LU",
                        QTrib = 1.00m,
                        VUnTrib = 84.9000000000M,
                        IndTot = SimNao.Sim,
                        XPed = "300474",
                        NItemPed = 1
                    },
                    Imposto = new Unimake.Business.DFe.Xml.NFe.Imposto
                    {
                        VTotTrib = 12.63,
                        ICMS = new List<Unimake.Business.DFe.Xml.NFe.ICMS>
                        {
                            new Unimake.Business.DFe.Xml.NFe.ICMS
                            {
                                ICMSSN101 = new Unimake.Business.DFe.Xml.NFe.ICMSSN101
                                {
                                    Orig = OrigemMercadoria.Nacional,
                                    PCredSN = 2.8255,
                                    VCredICMSSN = 2.40
                                }
                            }
                        },
                        PIS = new Unimake.Business.DFe.Xml.NFe.PIS
                        {
                            PISOutr = new Unimake.Business.DFe.Xml.NFe.PISOutr
                            {
                                CST = "99",
                                VBC = 0.00,
                                PPIS = 0.00,
                                VPIS = 0.00
                            }
                        },
                        COFINS = new Unimake.Business.DFe.Xml.NFe.COFINS
                        {
                            COFINSOutr = new Unimake.Business.DFe.Xml.NFe.COFINSOutr
                            {
                                CST = "99",
                                VBC = 0.00,
                                PCOFINS = 0.00,
                                VCOFINS = 0.00
                            }
                        }
                    }
                });
            }

            return dets;
        }

        /// <summary>
        /// Vou adicionar as tags de ICMS, isso é para o caso de se ter mais de uma por produto, mas o mais comum é ter uma só.
        /// </summary>
        /// <returns></returns>
        private List<Unimake.Business.DFe.Xml.NFe.ICMS> CriarICMS()
        {
            var icmslist = new List<Unimake.Business.DFe.Xml.NFe.ICMS>();

            var icms = new Unimake.Business.DFe.Xml.NFe.ICMS();

            var cst = "102";

            switch (cst)
            {
                case "102":
                    icms.ICMSSN102 = new ICMSSN102
                    {
                        Orig = OrigemMercadoria.Nacional,
                        CSOSN = "102"
                    };
                    break;

                case "101":
                    icms.ICMSSN101 = new ICMSSN101
                    {
                        Orig = OrigemMercadoria.Nacional,
                        CSOSN = "101"
                    };
                    break;

                case "10":
                    icms.ICMS10 = new ICMS10
                    {
                        Orig = OrigemMercadoria.Nacional,
                        CST = "10"
                    };
                    break;

                default:
                    break;
            }


            icmslist.Add(icms);

            return icmslist;
        }

        private void FormTestarNFe_Shown(object sender, EventArgs e)
        {
            var cert = new CertificadoDigital();
            CertificadoSelecionado = cert.Selecionar();

            ////Setar a senha do PIN do certificado A3
            //if (CertificadoSelecionado.IsA3())
            //{
            //    CertificadoSelecionado.SetPinPrivateKey("1234");
            //}
        }

        #endregion Private Methods

        #region Public Constructors

        public FormTestarNFe() => InitializeComponent();

        #endregion Public Constructors

        private void button23_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsReciCTe
                {
                    Versao = "3.00",
                    TpAmb = TipoAmbiente.Homologacao,
                    NRec = "501000074526711"
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var retAutorizacao = new Unimake.Business.DFe.Servicos.CTe.RetAutorizacao(xml, configuracao);
                retAutorizacao.Executar();
                MessageBox.Show(retAutorizacao.RetornoWSString);
                MessageBox.Show(retAutorizacao.Result.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button28_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EventoCTe
                {
                    Versao = "3.00",
                    InfEvento = new Unimake.Business.DFe.Xml.CTe.InfEvento(new Unimake.Business.DFe.Xml.CTe.DetEventoCanc
                    {
                        NProt = "141200000007987",
                        VersaoEvento = "3.00",
                        XJust = "Justificativa para cancelamento da CTe de teste"
                    })
                    {
                        COrgao = UFBrasil.PR,
                        ChCTe = "41200210859283000185570010000005671227070615",
                        CNPJ = "10859283000185",
                        DhEvento = DateTime.Now,
                        TpEvento = TipoEventoCTe.Cancelamento,
                        NSeqEvento = 1,
                        TpAmb = TipoAmbiente.Homologacao
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTe,
                    CertificadoDigital = CertificadoSelecionado
                };


                var recepcaoEvento = new Unimake.Business.DFe.Servicos.CTe.RecepcaoEvento(xml, configuracao);

                //xml.LerXML<EventoCTe>(recepcaoEvento.ConteudoXMLAssinado);

                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.InfEvento.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (recepcaoEvento.Result.InfEvento.CStat)
                {
                    case 134: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respectivo CT-e com situação diferente de Autorizada.
                    case 135: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respetivo CTe.
                    case 136: //Recebido pelo Sistema de Registro de Eventos – vinculação do evento ao respectivo CT-e prejudicado.
                        recepcaoEvento.GravarXmlDistribuicao(@"c:\testecte\");
                        break;

                    default:
                        //Quando o evento é rejeitado pela Sefaz.
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button29_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsReciMDFe
                {
                    Versao = "3.00",
                    TpAmb = TipoAmbiente.Homologacao,
                    NRec = "410000007934162"
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.MDFe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var retAutorizacao = new Unimake.Business.DFe.Servicos.MDFe.RetAutorizacao(xml, configuracao);
                retAutorizacao.Executar();
                MessageBox.Show(retAutorizacao.RetornoWSString);
                MessageBox.Show(retAutorizacao.Result.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }

        }

        private void button30_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EventoMDFe
                {
                    Versao = "3.00",
                    InfEvento = new Unimake.Business.DFe.Xml.MDFe.InfEvento(new Unimake.Business.DFe.Xml.MDFe.DetEventoCanc
                    {
                        NProt = "141200000007987",
                        VersaoEvento = "3.00",
                        XJust = "Justificativa para cancelamento da MDFe de teste"
                    })
                    {
                        COrgao = UFBrasil.PR,
                        ChMDFe = "41200210859283000185570010000005671227070615",
                        CNPJ = "10859283000185",
                        DhEvento = DateTime.Now,
                        TpEvento = TipoEventoMDFe.Cancelamento,
                        NSeqEvento = 1,
                        TpAmb = TipoAmbiente.Homologacao
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.MDFe,
                    CertificadoDigital = CertificadoSelecionado
                };


                var recepcaoEvento = new Unimake.Business.DFe.Servicos.MDFe.RecepcaoEvento(xml, configuracao);

                //xml.LerXML<EventoCTe>(recepcaoEvento.ConteudoXMLAssinado);

                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.InfEvento.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (recepcaoEvento.Result.InfEvento.CStat)
                {
                    case 134: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respectivo CT-e com situação diferente de Autorizada.
                    case 135: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respetivo CTe.
                    case 136: //Recebido pelo Sistema de Registro de Eventos – vinculação do evento ao respectivo CT-e prejudicado.
                        recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                        break;

                    default:
                        //Quando o evento é rejeitado pela Sefaz.
                        recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button31_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EventoCTe
                {
                    Versao = "3.00",
                    InfEvento = new Unimake.Business.DFe.Xml.CTe.InfEvento(new Unimake.Business.DFe.Xml.CTe.DetEventoCompEntrega
                    {
                        VersaoEvento = "3.00",
                        EventoCECTe = new EventoCECTe
                        {
                            NProt = "141200000007987",
                            DhEntrega = DateTime.Now,
                            NDoc = "91886127085",
                            XNome = "Teste",
                            Latitude = "00",
                            Longitude = "000",
                            HashEntrega = "1234564321321321321231231321",
                            DhHashEntrega = DateTime.Now,
                            InfEntrega = new List<InfEntrega>
                            {
                                new InfEntrega
                                {
                                    ChNFe = "12345678901234567890123456789012345678901234"
                                }
                            }
                        }
                    })
                    {
                        COrgao = UFBrasil.PR,
                        ChCTe = "41200210859283000185570010000005671227070615",
                        CNPJ = "10859283000185",
                        DhEvento = DateTime.Now,
                        TpEvento = TipoEventoCTe.ComprovanteEntrega,
                        NSeqEvento = 1,
                        TpAmb = TipoAmbiente.Homologacao
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTe,
                    CertificadoDigital = CertificadoSelecionado
                };


                var recepcaoEvento = new Unimake.Business.DFe.Servicos.CTe.RecepcaoEvento(xml, configuracao);

                //xml.LerXML<EventoCTe>(recepcaoEvento.ConteudoXMLAssinado);

                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.InfEvento.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (recepcaoEvento.Result.InfEvento.CStat)
                {
                    case 134: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respectivo CT-e com situação diferente de Autorizada.
                    case 135: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respetivo CTe.
                    case 136: //Recebido pelo Sistema de Registro de Eventos – vinculação do evento ao respectivo CT-e prejudicado.
                        recepcaoEvento.GravarXmlDistribuicao(@"c:\testecte\");
                        break;

                    default:
                        //Quando o evento é rejeitado pela Sefaz.
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button32_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsMDFeNaoEnc
                {
                    Versao = "3.00",
                    TpAmb = TipoAmbiente.Homologacao,
                    XServ = "CONSULTAR NÃO ENCERRADOS",
                    CNPJ = "10859283000185"
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.MDFe,
                    CodigoUF = (int)UFBrasil.PR,
                    CertificadoDigital = CertificadoSelecionado
                };

                var consNaoEnc = new Unimake.Business.DFe.Servicos.MDFe.ConsNaoEnc(xml, configuracao);
                consNaoEnc.Executar();
                MessageBox.Show(consNaoEnc.RetornoWSString);
                MessageBox.Show(consNaoEnc.Result.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EventoCTe
                {
                    Versao = "3.00",
                    InfEvento = new Unimake.Business.DFe.Xml.CTe.InfEvento(new Unimake.Business.DFe.Xml.CTe.DetEventoCancCompEntrega
                    {
                        VersaoEvento = "3.00",
                        NProt = "141200000007987",
                        NProtCE = "141200000001111"
                    })
                    {
                        COrgao = UFBrasil.PR,
                        ChCTe = "41200210859283000185570010000005671227070615",
                        CNPJ = "10859283000185",
                        DhEvento = DateTime.Now,
                        TpEvento = TipoEventoCTe.CancelamentoComprovanteEntrega,
                        NSeqEvento = 1,
                        TpAmb = TipoAmbiente.Homologacao
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTe,
                    CertificadoDigital = CertificadoSelecionado
                };


                var recepcaoEvento = new Unimake.Business.DFe.Servicos.CTe.RecepcaoEvento(xml, configuracao);

                //xml.LerXML<EventoCTe>(recepcaoEvento.ConteudoXMLAssinado);

                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.InfEvento.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (recepcaoEvento.Result.InfEvento.CStat)
                {
                    case 134: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respectivo CT-e com situação diferente de Autorizada.
                    case 135: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respetivo CTe.
                    case 136: //Recebido pelo Sistema de Registro de Eventos – vinculação do evento ao respectivo CT-e prejudicado.
                        recepcaoEvento.GravarXmlDistribuicao(@"c:\testecte\");
                        break;

                    default:
                        //Quando o evento é rejeitado pela Sefaz.
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button34_Click(object sender, EventArgs e)
        {
            try
            {
                #region CriarCTe

                var xml = new EnviCTe
                {
                    Versao = "3.00",
                    IdLote = "000000000000001",
                    CTe = new List<CTe> {
                        new CTe
                        {
                            InfCTe = new Unimake.Business.DFe.Xml.CTe.InfCTe
                            {
                                Versao = "3.00",

                                Ide = new Unimake.Business.DFe.Xml.CTe.Ide
                                {
                                    CUF = UFBrasil.PR,
                                    CCT = "01722067",
                                    CFOP  = "6352",
                                    NatOp = "PREST.SERV.TRANSP.INDUSTR",
                                    Mod = ModeloDFe.CTe,
                                    Serie = 1,
                                    NCT = 861 ,
                                    DhEmi = DateTime.Now,
                                    TpImp = FormatoImpressaoDACTE.NormalPaisagem,
                                    TpEmis = TipoEmissao.Normal,
                                    TpAmb = TipoAmbiente.Homologacao,
                                    TpCTe = TipoCTe.Normal,
                                    ProcEmi = ProcessoEmissao.AplicativoContribuinte,
                                    VerProc = "UNICO V8.0",
                                    CMunEnv = "4118402" ,
                                    XMunEnv = "PARANAVAI",
                                    UFEnv = UFBrasil.PR,
                                    Modal =  ModalidadeTransporteCTe.Rodoviario,
                                    TpServ = TipoServicoCTe.Normal,
                                    CMunIni = "4118402",
                                    XMunIni = "PARANAVAI",
                                    UFIni = UFBrasil.PR,
                                    CMunFim = "3305109",
                                    XMunFim = "SAO JOAO DE MERITI",
                                    UFFim =  UFBrasil.RJ,
                                    Retira = SimNao.Nao,
                                    IndIEToma = IndicadorIEDestinatario.ContribuinteICMS,
                                    Toma3 =  new Toma3
                                    {
                                        Toma= TomadorServicoCTe.Remetente,
                                    },
                                },
                                Emit = new Unimake.Business.DFe.Xml.CTe.Emit
                                {
                                    CNPJ = "31905001000109",
                                    IE = "9079649730",
                                    XNome = "EXATUS MOVEIS EIRELI",
                                    XFant = "EXATUS MOVEIS",
                                    EnderEmit = new Unimake.Business.DFe.Xml.CTe.EnderEmit
                                    {
                                        XLgr = "RUA JOAQUIM F. DE SOUZA",
                                        Nro = "01112",
                                        XBairro = "VILA TEREZINHA",
                                        CMun = 4118402,
                                        XMun = "PARANAVAI",
                                        CEP = "87706675",
                                        UF = UFBrasil.PR,
                                        Fone = "04434237530",
                                    },
                                },
                                Rem = new Unimake.Business.DFe.Xml.CTe.Rem
                                {
                                    CNPJ = "10197843000183",
                                    IE = "9044791606",
                                    XNome = "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                                    XFant = "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                                    Fone = "04434225480",
                                    EnderReme = new Unimake.Business.DFe.Xml.CTe.EnderReme
                                    {
                                        XLgr = "RUA AMAZONAS, 1140",
                                        Nro = "1140",
                                        XBairro = "JD. SAO FELICIO",
                                        CMun = 4118402,
                                        XMun = "PARANAVAI",
                                        CEP = "87702300",
                                        UF = UFBrasil.PR,
                                        CPais = 1058,
                                        XPais = "BRASIL",
                                    }
                                },
                                Dest = new Unimake.Business.DFe.Xml.CTe.Dest
                                {
                                    CNPJ = "00000000075108",
                                    IE = "ISENTO",
                                    XNome = "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                                    EnderDest = new Unimake.Business.DFe.Xml.CTe.EnderDest
                                    {
                                        XLgr = "R. GESSYR GONCALVES FONTES, 55",
                                        Nro = "55",
                                        XBairro = "CENTRO",
                                        CMun = 3305109,
                                        XMun = "SAO JOAO DE MERITI",
                                        CEP = "25520570",
                                        UF = UFBrasil.RJ,
                                        CPais = 1058,
                                        XPais = "BRASIL",
                                    },
                                },
                                VPrest = new Unimake.Business.DFe.Xml.CTe.VPrest
                                {
                                    VTPrest = 50.00,
                                    VRec = 50.00,
                                    Comp = new List<Unimake.Business.DFe.Xml.CTe.Comp>
                                    {
                                        new Unimake.Business.DFe.Xml.CTe.Comp
                                        {
                                            XNome = "FRETE VALOR",
                                            VComp = 50.00,
                                        },
                                    },
                                },
                                Imp = new Unimake.Business.DFe.Xml.CTe.Imp
                                {
                                    ICMS = new Unimake.Business.DFe.Xml.CTe.ICMS
                                    {
                                        ICMSSN = new Unimake.Business.DFe.Xml.CTe.ICMSSN
                                        {
                                            CST = "90",
                                            IndSN = SimNao.Sim,
                                        }
                                        }
                                },
                                InfCTeNorm = new Unimake.Business.DFe.Xml.CTe.InfCTeNorm
                                {
                                    InfCarga = new Unimake.Business.DFe.Xml.CTe.InfCarga
                                    {
                                        VCarga = 6252.96,
                                        ProPred = "MOVEIS",
                                        InfQ = new List<Unimake.Business.DFe.Xml.CTe.InfQ>
                                        {
                                            new Unimake.Business.DFe.Xml.CTe.InfQ
                                            {
                                                CUnid = CodigoUnidadeMedidaCTe.KG,
                                                TpMed ="PESO BRUTO",
                                                QCarga = 320.0000,
                                            },
                                            new Unimake.Business.DFe.Xml.CTe.InfQ
                                            {
                                                CUnid = CodigoUnidadeMedidaCTe.UNIDADE,
                                                TpMed ="UNIDADE",
                                                QCarga = 1.0000,
                                            },
                                        },
                                    },
                                    InfDoc = new Unimake.Business.DFe.Xml.CTe.InfDoc
                                    {
                                        InfNFe = new List<Unimake.Business.DFe.Xml.CTe.InfNFe>
                                        {
                                            new Unimake.Business.DFe.Xml.CTe.InfNFe
                                            {
                                                Chave = "41200306117473000150550030000652511417023254"
                                            },
                                        },
                                    },
                                    InfModal = new Unimake.Business.DFe.Xml.CTe.InfModal
                                    {
                                        VersaoModal="3.00",
                                        Rodo = new Unimake.Business.DFe.Xml.CTe.Rodo
                                        {
                                            RNTRC = "44957333",
                                            Occ = new List<Unimake.Business.DFe.Xml.CTe.Occ>
                                            {
                                                new Unimake.Business.DFe.Xml.CTe.Occ
                                                {
                                                    NOcc = 810,
                                                    DEmi = DateTime.Now,
                                                    EmiOcc = new Unimake.Business.DFe.Xml.CTe.EmiOcc
                                                    {
                                                        CNPJ = "31905001000109",
                                                        CInt = "0000001067",
                                                        IE = "9079649730",
                                                        UF = UFBrasil.PR,
                                                        Fone = "04434237530",
                                                    },
                                                },
                                            },
                                        },
                                    },
                                },
                                InfRespTec = new Unimake.Business.DFe.Xml.CTe.InfRespTec
                                {
                                    CNPJ = "06117473000150",
                                    XContato = "Wandrey Mundin Ferreira",
                                    Email= "wandrey@unimake.com.br",
                                    Fone = "04431414900",
                                },
                            },
                        },
                    },
                };

                #endregion CriarCTe

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var autorizacao = new Unimake.Business.DFe.Servicos.CTe.Autorizacao(xml, configuracao);
                autorizacao.Executar();
                MessageBox.Show(autorizacao.RetornoWSString);

                if (autorizacao.Result != null)
                {
                    MessageBox.Show(autorizacao.Result.XMotivo);

                    if (autorizacao.Result.CStat == 103) //103 = Lote Recebido com Sucesso
                    {
                        // Finalizar através da consulta do recibo.
                        var xmlRec = new ConsReciCTe
                        {
                            Versao = "3.00",
                            TpAmb = TipoAmbiente.Homologacao,
                            NRec = autorizacao.Result.InfRec.NRec
                        };

                        var configRec = new Configuracao
                        {
                            TipoDFe = TipoDFe.CTe,
                            CertificadoDigital = CertificadoSelecionado
                        };

                        var retAutorizacao = new Unimake.Business.DFe.Servicos.CTe.RetAutorizacao(xmlRec, configRec);
                        retAutorizacao.Executar();

                        autorizacao.RetConsReciCTe = retAutorizacao.Result;
                        autorizacao.GravarXmlDistribuicao(@"c:\testenfe\");

                        //Simulação da finalização do CTe através da consulta situação
                        autorizacao.RetConsReciCTe = null; //Zerar para conseguir testar

                        var xmlSit = new ConsSitCTe
                        {
                            Versao = "3.00",
                            TpAmb = TipoAmbiente.Homologacao,
                            ChCTe = xml.CTe[0].InfCTe.Chave
                        };

                        var configSit = new Configuracao
                        {
                            TipoDFe = TipoDFe.CTe,
                            CertificadoDigital = CertificadoSelecionado
                        };

                        var consultaProtocolo = new Unimake.Business.DFe.Servicos.CTe.ConsultaProtocolo(xmlSit, configSit);
                        consultaProtocolo.Executar();

                        autorizacao.RetConsSitCTes.Add(consultaProtocolo.Result);
                        autorizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button35_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EventoCTe
                {
                    Versao = "3.00",
                    InfEvento = new Unimake.Business.DFe.Xml.CTe.InfEvento(new Unimake.Business.DFe.Xml.CTe.DetEventoCCE
                    {
                        VersaoEvento = "3.00",
                        EventoCCeCTe = new EventoCCeCTe
                        {
                            InfCorrecao = new List<InfCorrecao>
                            {
                                new InfCorrecao
                                {
                                    GrupoAlterado = "ide",
                                    CampoAlterado = "cfop",
                                    ValorAlterado = "6353",
                                    NroItemAlterado = ""
                                }
                            }
                        }
                    })
                    {
                        COrgao = UFBrasil.PR,
                        ChCTe = "41200210859283000185570010000005671227070615",
                        CNPJ = "10859283000185",
                        DhEvento = DateTime.Now,
                        TpEvento = TipoEventoCTe.CartaCorrecao,
                        NSeqEvento = 1,
                        TpAmb = TipoAmbiente.Homologacao
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTe,
                    CertificadoDigital = CertificadoSelecionado
                };


                var recepcaoEvento = new Unimake.Business.DFe.Servicos.CTe.RecepcaoEvento(xml, configuracao);

                //xml.LerXML<EventoCTe>(recepcaoEvento.ConteudoXMLAssinado);

                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.InfEvento.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (recepcaoEvento.Result.InfEvento.CStat)
                {
                    case 134: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respectivo CT-e com situação diferente de Autorizada.
                    case 135: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respetivo CTe.
                    case 136: //Recebido pelo Sistema de Registro de Eventos – vinculação do evento ao respectivo CT-e prejudicado.
                        recepcaoEvento.GravarXmlDistribuicao(@"c:\testecte\");
                        break;

                    default:
                        //Quando o evento é rejeitado pela Sefaz.
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button36_Click(object sender, EventArgs e)
        {
            #region Montar o objeto com o XML da NFe que está guardando no banco de dados 

            var xmlNFe = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?><NFe xmlns=\"http://www.portalfiscal.inf.br/nfe\"><infNFe Id=\"NFe41210606117473000150550010000681511001751990\" versao=\"4.00\"><ide><cUF>41</cUF><cNF>00175199</cNF><natOp>VENDA PRODUC.DO ESTABELEC</natOp><mod>55</mod><serie>1</serie><nNF>68151</nNF><dhEmi>2021-06-01T10:02:55-03:00</dhEmi><dhSaiEnt>2021-06-01T10:02:55-03:00</dhSaiEnt><tpNF>1</tpNF><idDest>2</idDest><cMunFG>4118402</cMunFG><tpImp>1</tpImp><tpEmis>1</tpEmis><cDV>0</cDV><tpAmb>1</tpAmb><finNFe>1</finNFe><indFinal>0</indFinal><indPres>1</indPres><procEmi>0</procEmi><verProc>UNICO V8.0</verProc></ide><emit><CNPJ>06117473000150</CNPJ><xNome>UNIMAKE SOLUCOES CORPORATIVAS LTDA</xNome><xFant>UNIMAKE - PARANAVAI</xFant><enderEmit><xLgr>RUA PAULO ANTONIO DA COSTA</xLgr><nro>575</nro><xBairro>JARDIM SIMARA</xBairro><cMun>4118402</cMun><xMun>PARANAVAI</xMun><UF>PR</UF><CEP>87707210</CEP><cPais>1058</cPais><xPais>BRASIL</xPais><fone>04431414900</fone></enderEmit><IE>9032000301</IE><IM>14018</IM><CNAE>6202300</CNAE><CRT>1</CRT></emit><dest><CNPJ>05095657000101</CNPJ><xNome>KAMAR EMBALAGENS E DESCARTAVEIS LTDA - ME</xNome><enderDest><xLgr>RUA BARAO DO RIO BRANCO</xLgr><nro>1368</nro><xBairro>VILA GERMANO</xBairro><cMun>3506508</cMun><xMun>BIRIGUI</xMun><UF>SP</UF><CEP>16200308</CEP><cPais>1058</cPais><xPais>BRASIL</xPais><fone>01841412616</fone></enderDest><indIEDest>1</indIEDest><IE>214139959110</IE><email>elisangela@intis.com.br</email></dest><autXML><CPF>45813841920</CPF></autXML><det nItem=\"1\"><prod><cProd>01042</cProd><cEAN>SEM GTIN</cEAN><xProd>UNIDANFE V.3 PLUS</xProd><NCM>84714900</NCM><CFOP>6101</CFOP><uCom>LU</uCom><qCom>1</qCom><vUnCom>87.9000000000</vUnCom><vProd>87.90</vProd><cEANTrib>SEM GTIN</cEANTrib><uTrib>LU</uTrib><qTrib>1</qTrib><vUnTrib>87.9000000000</vUnTrib><indTot>1</indTot><xPed>344813</xPed><nItemPed>1</nItemPed></prod><imposto><ICMS><ICMSSN101><orig>0</orig><CSOSN>101</CSOSN><pCredSN>3.0703</pCredSN><vCredICMSSN>2.70</vCredICMSSN></ICMSSN101></ICMS><PIS><PISOutr><CST>99</CST><vBC>0.00</vBC><pPIS>0.0000</pPIS><vPIS>0.00</vPIS></PISOutr></PIS><COFINS><COFINSOutr><CST>99</CST><vBC>0.00</vBC><pCOFINS>0.0000</pCOFINS><vCOFINS>0.00</vCOFINS></COFINSOutr></COFINS></imposto></det><total><ICMSTot><vBC>0.00</vBC><vICMS>0.00</vICMS><vICMSDeson>0.00</vICMSDeson><vFCP>0.00</vFCP><vBCST>0.00</vBCST><vST>0.00</vST><vFCPST>0.00</vFCPST><vFCPSTRet>0.00</vFCPSTRet><vProd>87.90</vProd><vFrete>0.00</vFrete><vSeg>0.00</vSeg><vDesc>0.00</vDesc><vII>0.00</vII><vIPI>0.00</vIPI><vIPIDevol>0.00</vIPIDevol><vPIS>0.00</vPIS><vCOFINS>0.00</vCOFINS><vOutro>0.00</vOutro><vNF>87.90</vNF></ICMSTot></total><transp><modFrete>0</modFrete><transporta></transporta><vol><qVol>1</qVol><esp>LU</esp><marca>UNIMAKE</marca><pesoL>0.000</pesoL><pesoB>0.000</pesoB></vol></transp><cobr><fat><nFat>068151</nFat><vOrig>87.90</vOrig><vDesc>0.00</vDesc><vLiq>87.90</vLiq></fat><dup><nDup>001</nDup><dVenc>2021-06-01</dVenc><vDup>87.90</vDup></dup></cobr><pag><detPag><indPag>0</indPag><tPag>99</tPag><vPag>87.90</vPag></detPag></pag><infAdic><infCpl>;CONTROLE: 0000283199;PEDIDO(S) ATENDIDO(S): 0011344813;Empresa optante pelo simples nacional, conforme lei compl. 128 de 19/12/2008;Permite o aproveitamento do credito de ICMS no valor de R$ 2,70, correspondente ao percentual de 3,07% . Nos termos do Art. 23 - LC 123/2006 (Resolucoes CGSN n. 10/2007 e 53/2008);</infCpl></infAdic><infRespTec><CNPJ>06117473000150</CNPJ><xContato>Wandrey Mundin Ferreira</xContato><email>wandrey@unimake.com.br</email><fone>04431414900</fone></infRespTec></infNFe><Signature xmlns=\"http://www.w3.org/2000/09/xmldsig#\"><SignedInfo><CanonicalizationMethod Algorithm=\"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" /><SignatureMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#rsa-sha1\" /><Reference URI=\"#NFe41210606117473000150550010000681511001751990\"><Transforms><Transform Algorithm=\"http://www.w3.org/2000/09/xmldsig#enveloped-signature\" /><Transform Algorithm=\"http://www.w3.org/TR/2001/REC-xml-c14n-20010315\" /></Transforms><DigestMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#sha1\" /><DigestValue>++BfF3FDQ4e/jJLex2MMTCzM5qQ=</DigestValue></Reference></SignedInfo><SignatureValue>AqKoNs0+0UMC9iqSOZv3RYSWwoPso9UBG4VV2TPOKPGyg1534ZUkuacoeAydwFxX4UMv/rdeCsve2ca7k4rteD1cj42L+5Cupd7zLWfXi7bIi5k6BEnervPihOAe0Tdi0qWrBDRPXPhhJWQgmxfR0m6+9v1WCh6j3DBFHmGclP3CLNBlmZjmoUm1OEbKNnYKDnKj3vxvIqLo/VfRDwDHCTkYNGNW0cz3FZhTDFIBG6gllVBHWr4CwVwLiSRqsWdExqk/OiPv8w/ixf8/S7CWuEoU/fUQPY7extRf73c7ox7SkbbUZGrm9KAM9WlBnZOSkuIvnzF3wLWjYdjdGanjxQ==</SignatureValue><KeyInfo><X509Data><X509Certificate>MIIHwTCCBamgAwIBAgIIcCC6h06PrjswDQYJKoZIhvcNAQELBQAwdjELMAkGA1UEBhMCQlIxEzARBgNVBAoTCklDUC1CcmFzaWwxNjA0BgNVBAsTLVNlY3JldGFyaWEgZGEgUmVjZWl0YSBGZWRlcmFsIGRvIEJyYXNpbCAtIFJGQjEaMBgGA1UEAxMRQUMgU0FGRVdFQiBSRkIgdjUwHhcNMjEwNDI4MTM1NzA3WhcNMjIwNDI4MTM1NzA3WjCB/TELMAkGA1UEBhMCQlIxEzARBgNVBAoTCklDUC1CcmFzaWwxCzAJBgNVBAgTAlBSMRIwEAYDVQQHEwlQQVJBTkFWQT8xNjA0BgNVBAsTLVNlY3JldGFyaWEgZGEgUmVjZWl0YSBGZWRlcmFsIGRvIEJyYXNpbCAtIFJGQjEWMBQGA1UECxMNUkZCIGUtQ05QSiBBMTEXMBUGA1UECxMOMjAwODUxMDUwMDAxMDYxEzARBgNVBAsTCnByZXNlbmNpYWwxOjA4BgNVBAMTMVVOSU1BS0UgU09MVUNPRVMgQ09SUE9SQVRJVkFTIExUREE6MDYxMTc0NzMwMDAxNTAwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDhCaXqd9MW34oLkNyoCpcKEBAXusQ542nFPMce/VfnHBgJxmWBW4ZdArYb9vfKQF7jMtAysWQdFEBuMu6pERhei1BbdYwTAbzFywFpB2y3WvoSlPpCbwD0dZLF18VDtHliSuwXg77ztx5lKxEGsM+oPtCxcocZ81BPtGYtlB0RBmmMcFTACbDLTOtHpKbS9JLgwcOPj/juuDAiRqNtFdlWK5HyiOFV9hwF3MyBgTzs4cVOXaSfhvNd8bB2dDJpKT7Z4wjvHkDAnKB6eskeNJsBdKoDksxkE0h3gh+6kJNrOnZjjgZKakbwUkt0uKEBZXEKClQx9zU8kaYxrlr2AVStAgMBAAGjggLJMIICxTAfBgNVHSMEGDAWgBQpXkvVRky7/hanY8EdxCby3djzBTAOBgNVHQ8BAf8EBAMCBeAwbQYDVR0gBGYwZDBiBgZgTAECATMwWDBWBggrBgEFBQcCARZKaHR0cDovL3JlcG9zaXRvcmlvLmFjc2FmZXdlYi5jb20uYnIvYWMtc2FmZXdlYnJmYi9hYy1zYWZld2ViLXJmYi1wYy1hMS5wZGYwga4GA1UdHwSBpjCBozBPoE2gS4ZJaHR0cDovL3JlcG9zaXRvcmlvLmFjc2FmZXdlYi5jb20uYnIvYWMtc2FmZXdlYnJmYi9sY3ItYWMtc2FmZXdlYnJmYnY1LmNybDBQoE6gTIZKaHR0cDovL3JlcG9zaXRvcmlvMi5hY3NhZmV3ZWIuY29tLmJyL2FjLXNhZmV3ZWJyZmIvbGNyLWFjLXNhZmV3ZWJyZmJ2NS5jcmwwgYsGCCsGAQUFBwEBBH8wfTBRBggrBgEFBQcwAoZFaHR0cDovL3JlcG9zaXRvcmlvLmFjc2FmZXdlYi5jb20uYnIvYWMtc2FmZXdlYnJmYi9hYy1zYWZld2VicmZidjUucDdiMCgGCCsGAQUFBzABhhxodHRwOi8vb2NzcC5hY3NhZmV3ZWIuY29tLmJyMIG5BgNVHREEgbEwga6BGUZJTkFOQ0VJUk9AVU5JTUFLRS5DT00uQlKgIwYFYEwBAwKgGhMYU0VSR0lPIENBU1RFTEFPIFBJTkhFSVJPoBkGBWBMAQMDoBATDjA2MTE3NDczMDAwMTUwoDgGBWBMAQMEoC8TLTE4MDIxOTcwNzgwNzYzMDc5NTMwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMKAXBgVgTAEDB6AOEwwwMDAwMDAwMDAwMDAwHQYDVR0lBBYwFAYIKwYBBQUHAwIGCCsGAQUFBwMEMAkGA1UdEwQCMAAwDQYJKoZIhvcNAQELBQADggIBAC1X4mGtDHDFIt1qSqsb59EEzBdUOo9RgakyAmlWmv5Ipznt3X95vKHWaDL8s5/ClRPRqxW/cPoJXlgTqANI7RT/4Jg3XLQLmi7cOqxCwYkzqgvddE/Wy+Hk+cwqDWFhLqEbTdGFho3skuMKjkb4LI6pNLAPvCzAz0mVqkJ/3nnGGrFn8IgcKmgJxpL+wseCFo7Y2y+JuuXW/YPWtihxM2oT2fWoP39L3B8P8twqlXPhfUW5Mh4KH3n9ZkPP7/UtN65XjQ+CLILUw2PIULtb+4HWs8LIouq4n3tgtLAtCRQGe0/oC7QVhbTff2RHZHr3H39KlbAxsTQ7FfTtZM1dj7LhJj8S/8q89f9T/cgC08nbpMgQ+SQGL+GHEy+VjUszwLmL9P2D/eNqX0NFUqSPgUbqmxIrIoJLJoZgXv5G5XKtpC1wgqQZl3FNwTCG39/x2X/tqFL0rNE4sIjYfFCPOEL7R/Y2Mw201n6bJuQ0+2gKeUH5hO08xl1XXUJZZlo3vVkTz/dqnujY5faXW+Rn8HdLOyeNgPTlY3NvKm/TAyqjYlNQURLuWuOK3+xV/ptNck2RjoLN5Yl1wwQA3PI2CMq/pPVdlyXHchT3oCPe6AVja1qSo04v/77u7C8rDBFmir8c2FidbYQo0TiLXJ3xDLRlwSAKJFKxHb/M/fLwcVDE</X509Certificate></X509Data></KeyInfo></Signature></NFe>";

            var docXmlNFe = new XmlDocument();
            docXmlNFe.LoadXml(xmlNFe);

            var xml = new EnviNFe
            {
                Versao = "4.00",
                IdLote = "000000000000001",
                IndSinc = SimNao.Sim,
                NFe = new List<NFe>
                {
                    XMLUtility.Deserializar<NFe>(docXmlNFe)
                }
            };

            #endregion

            #region Criar as configurações para montagem do XML de distribução.

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.NFe,
                CertificadoDigital = CertificadoSelecionado
            };

            var autorizacao = new Autorizacao(xml, configuracao);

            #endregion

            #region Simulação da finalização do NFe através da consulta situação

            autorizacao.RetConsReciNFe = null; //Zerar pois é um novo envio de NFe

            var configSit = new Configuracao
            {
                TipoDFe = TipoDFe.NFe,
                CertificadoDigital = CertificadoSelecionado
            };

            foreach (var item in xml.NFe)
            {
                var xmlSit = new ConsSitNFe
                {
                    Versao = "4.00",
                    TpAmb = item.InfNFe[0].Ide.TpAmb,
                    ChNFe = item.InfNFe[0].Chave
                };

                var consultaProtocolo = new Unimake.Business.DFe.Servicos.NFe.ConsultaProtocolo(xmlSit, configSit);
                consultaProtocolo.Executar();

                autorizacao.RetConsSitNFes.Add(consultaProtocolo.Result);
            }

            autorizacao.GravarXmlDistribuicao(@"c:\testenfe\");

            #endregion
        }

        private void button37_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EventoCTe
                {
                    Versao = "3.00",
                    InfEvento = new Unimake.Business.DFe.Xml.CTe.InfEvento(new Unimake.Business.DFe.Xml.CTe.DetEventoPrestDesacordo
                    {
                        VersaoEvento = "3.00",
                        DescEvento = "Prestacao do Servico em Desacordo",
                        IndDesacordoOper = "1",
                        XObs = "Teste de manifestacao de servico em desacordo, ambiente de homolocacao"
                    })
                    {
                        COrgao = UFBrasil.PR,
                        ChCTe = "41200210859283000185570010000005691527070631",
                        CNPJ = "10859283000185",
                        DhEvento = DateTime.Now,
                        TpEvento = TipoEventoCTe.PrestDesacordo,
                        NSeqEvento = 1,
                        TpAmb = TipoAmbiente.Homologacao
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTe,
                    CertificadoDigital = CertificadoSelecionado
                };


                var recepcaoEvento = new Unimake.Business.DFe.Servicos.CTe.RecepcaoEvento(xml, configuracao);

                //xml.LerXML<EventoCTe>(recepcaoEvento.ConteudoXMLAssinado);

                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.InfEvento.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (recepcaoEvento.Result.InfEvento.CStat)
                {
                    case 134: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respectivo CT-e com situação diferente de Autorizada.
                    case 135: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respetivo CTe.
                    case 136: //Recebido pelo Sistema de Registro de Eventos – vinculação do evento ao respectivo CT-e prejudicado.
                        recepcaoEvento.GravarXmlDistribuicao(@"c:\testecte\");
                        break;

                    default:
                        //Quando o evento é rejeitado pela Sefaz.
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button38_Click(object sender, EventArgs e)
        {
            try
            {
                #region CriarCTe

                var xml = new EnviCTe
                {
                    Versao = "3.00",
                    IdLote = "000000000000001",
                    CTe = new List<CTe> {
                        new CTe
                        {
                            InfCTe = new Unimake.Business.DFe.Xml.CTe.InfCTe
                            {
                                Versao = "3.00",
                                Ide = new Unimake.Business.DFe.Xml.CTe.Ide
                                {
                                    CUF = UFBrasil.PR,
                                    CCT = "01722067",
                                    CFOP  = "6352",
                                    NatOp = "PREST.SERV.TRANSP.INDUSTR",
                                    Mod = ModeloDFe.CTe,
                                    Serie = 1,
                                    NCT = 868 ,
                                    DhEmi = DateTime.Now,
                                    TpImp = FormatoImpressaoDACTE.NormalPaisagem,
                                    TpEmis = TipoEmissao.Normal,
                                    TpAmb = TipoAmbiente.Homologacao,
                                    TpCTe = TipoCTe.Normal,
                                    ProcEmi = ProcessoEmissao.AplicativoContribuinte,
                                    VerProc = "UNICO V8.0",
                                    CMunEnv = "4118402",
                                    XMunEnv = "PARANAVAI",
                                    UFEnv = UFBrasil.PR,
                                    Modal =  ModalidadeTransporteCTe.Rodoviario,
                                    TpServ = TipoServicoCTe.Normal,
                                    CMunIni = "4118402",
                                    XMunIni = "PARANAVAI",
                                    UFIni = UFBrasil.PR,
                                    CMunFim = "3305109",
                                    XMunFim = "SAO JOAO DE MERITI",
                                    UFFim =  UFBrasil.RJ,
                                    Retira = SimNao.Nao,
                                    IndIEToma = IndicadorIEDestinatario.ContribuinteICMS,
                                    Toma3 =  new Toma3
                                    {
                                        Toma= TomadorServicoCTe.Remetente,
                                    },
                                },
                                Emit = new Unimake.Business.DFe.Xml.CTe.Emit
                                {
                                    CNPJ = "31905001000109",
                                    IE = "9079649730",
                                    XNome = "EXATUS MOVEIS EIRELI",
                                    XFant = "EXATUS MOVEIS",
                                    EnderEmit = new Unimake.Business.DFe.Xml.CTe.EnderEmit
                                    {
                                        XLgr = "RUA JOAQUIM F. DE SOUZA",
                                        Nro = "01112",
                                        XBairro = "VILA TEREZINHA",
                                        CMun = 4118402,
                                        XMun = "PARANAVAI",
                                        CEP = "87706675",
                                        UF = UFBrasil.PR,
                                        Fone = "04434237530",
                                    },
                                },
                                Rem = new Unimake.Business.DFe.Xml.CTe.Rem
                                {
                                    CNPJ = "10197843000183",
                                    IE = "9044791606",
                                    XNome = "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                                    XFant = "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                                    Fone = "04434225480",
                                    EnderReme = new Unimake.Business.DFe.Xml.CTe.EnderReme
                                    {
                                        XLgr = "RUA AMAZONAS, 1140",
                                        Nro = "1140",
                                        XBairro = "JD. SAO FELICIO",
                                        CMun = 4118402,
                                        XMun = "PARANAVAI",
                                        CEP = "87702300",
                                        UF = UFBrasil.PR,
                                        CPais = 1058,
                                        XPais = "BRASIL",
                                    }
                                },
                                Dest = new Unimake.Business.DFe.Xml.CTe.Dest
                                {
                                    CNPJ = "00000000075108",
                                    IE = "ISENTO",
                                    XNome = "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                                    EnderDest = new Unimake.Business.DFe.Xml.CTe.EnderDest
                                    {
                                        XLgr = "R. GESSYR GONCALVES FONTES, 55",
                                        Nro = "55",
                                        XBairro = "CENTRO",
                                        CMun = 3305109,
                                        XMun = "SAO JOAO DE MERITI",
                                        CEP = "25520570",
                                        UF = UFBrasil.RJ,
                                        CPais = 1058,
                                        XPais = "BRASIL",
                                    },
                                },
                                VPrest = new Unimake.Business.DFe.Xml.CTe.VPrest
                                {
                                    VTPrest = 50.00,
                                    VRec = 50.00,
                                    Comp = new List<Unimake.Business.DFe.Xml.CTe.Comp>
                                    {
                                        new Unimake.Business.DFe.Xml.CTe.Comp
                                        {
                                            XNome = "FRETE VALOR",
                                            VComp = 50.00,
                                        },
                                    },
                                },
                                Imp = new Unimake.Business.DFe.Xml.CTe.Imp
                                {
                                    ICMS = new Unimake.Business.DFe.Xml.CTe.ICMS
                                    {
                                        ICMSSN = new Unimake.Business.DFe.Xml.CTe.ICMSSN
                                        {
                                            CST = "90",
                                            IndSN = SimNao.Sim,
                                        }
                                    }
                                },
                                InfCTeNorm = new Unimake.Business.DFe.Xml.CTe.InfCTeNorm
                                {
                                    InfCarga = new Unimake.Business.DFe.Xml.CTe.InfCarga
                                    {
                                        VCarga = 6252.96,
                                        ProPred = "MOVEIS",
                                        InfQ = new List<Unimake.Business.DFe.Xml.CTe.InfQ>
                                        {
                                            new Unimake.Business.DFe.Xml.CTe.InfQ
                                            {
                                                CUnid = CodigoUnidadeMedidaCTe.KG,
                                                TpMed ="PESO BRUTO",
                                                QCarga = 320.0000,
                                            },
                                            new Unimake.Business.DFe.Xml.CTe.InfQ
                                            {
                                                CUnid = CodigoUnidadeMedidaCTe.UNIDADE,
                                                TpMed ="UNIDADE",
                                                QCarga = 1.0000,
                                            },
                                        },
                                    },
                                    InfDoc = new Unimake.Business.DFe.Xml.CTe.InfDoc
                                    {
                                        InfNFe = new List<Unimake.Business.DFe.Xml.CTe.InfNFe>
                                        {
                                            new Unimake.Business.DFe.Xml.CTe.InfNFe
                                            {
                                                Chave = "41200306117473000150550030000652511417023254"
                                            },
                                        },
                                    },
                                    InfModal = new Unimake.Business.DFe.Xml.CTe.InfModal
                                    {
                                        VersaoModal="3.00",
                                        Rodo = new Unimake.Business.DFe.Xml.CTe.Rodo
                                        {
                                            RNTRC = "44957333",
                                            Occ = new List<Unimake.Business.DFe.Xml.CTe.Occ>
                                            {
                                                new Unimake.Business.DFe.Xml.CTe.Occ
                                                {
                                                    NOcc = 810,
                                                    DEmi = DateTime.Now,
                                                    EmiOcc = new Unimake.Business.DFe.Xml.CTe.EmiOcc
                                                    {
                                                        CNPJ = "31905001000109",
                                                        CInt = "0000001067",
                                                        IE = "9079649730",
                                                        UF = UFBrasil.PR,
                                                        Fone = "04434237530",
                                                    },
                                                },
                                            },
                                        },
                                    },
                                },
                                InfRespTec = new Unimake.Business.DFe.Xml.CTe.InfRespTec
                                {
                                    CNPJ = "06117473000150",
                                    XContato = "Wandrey Mundin Ferreira",
                                    Email= "wandrey@unimake.com.br",
                                    Fone = "04431414900",
                                },
                            },
                        },
                        new CTe
                        {
                            InfCTe = new Unimake.Business.DFe.Xml.CTe.InfCTe
                            {
                                Versao = "3.00",
                                Ide = new Unimake.Business.DFe.Xml.CTe.Ide
                                {
                                    CUF = UFBrasil.PR,
                                    CCT = "01722067",
                                    CFOP  = "6352",
                                    NatOp = "PREST.SERV.TRANSP.INDUSTR",
                                    Mod = ModeloDFe.CTe,
                                    Serie = 1,
                                    NCT = 869 ,
                                    DhEmi = DateTime.Now,
                                    TpImp = FormatoImpressaoDACTE.NormalPaisagem,
                                    TpEmis = TipoEmissao.Normal,
                                    TpAmb = TipoAmbiente.Homologacao,
                                    TpCTe = TipoCTe.Normal,
                                    ProcEmi = ProcessoEmissao.AplicativoContribuinte,
                                    VerProc = "UNICO V8.0",
                                    CMunEnv = "4118402",
                                    XMunEnv = "PARANAVAI",
                                    UFEnv = UFBrasil.PR,
                                    Modal =  ModalidadeTransporteCTe.Rodoviario,
                                    TpServ = TipoServicoCTe.Normal,
                                    CMunIni = "4118402",
                                    XMunIni = "PARANAVAI",
                                    UFIni = UFBrasil.PR,
                                    CMunFim = "3305109",
                                    XMunFim = "SAO JOAO DE MERITI",
                                    UFFim =  UFBrasil.RJ,
                                    Retira = SimNao.Nao,
                                    IndIEToma = IndicadorIEDestinatario.ContribuinteICMS,
                                    Toma3 =  new Toma3
                                    {
                                        Toma= TomadorServicoCTe.Remetente,
                                    },
                                },
                                Emit = new Unimake.Business.DFe.Xml.CTe.Emit
                                {
                                    CNPJ = "31905001000109",
                                    IE = "9079649730",
                                    XNome = "EXATUS MOVEIS EIRELI",
                                    XFant = "EXATUS MOVEIS",
                                    EnderEmit = new Unimake.Business.DFe.Xml.CTe.EnderEmit
                                    {
                                        XLgr = "RUA JOAQUIM F. DE SOUZA",
                                        Nro = "01112",
                                        XBairro = "VILA TEREZINHA",
                                        CMun = 4118402,
                                        XMun = "PARANAVAI",
                                        CEP = "87706675",
                                        UF = UFBrasil.PR,
                                        Fone = "04434237530",
                                    },
                                },
                                Rem = new Unimake.Business.DFe.Xml.CTe.Rem
                                {
                                    CNPJ = "10197843000183",
                                    IE = "9044791606",
                                    XNome = "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                                    XFant = "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                                    Fone = "04434225480",
                                    EnderReme = new Unimake.Business.DFe.Xml.CTe.EnderReme
                                    {
                                        XLgr = "RUA AMAZONAS, 1140",
                                        Nro = "1140",
                                        XBairro = "JD. SAO FELICIO",
                                        CMun = 4118402,
                                        XMun = "PARANAVAI",
                                        CEP = "87702300",
                                        UF = UFBrasil.PR,
                                        CPais = 1058,
                                        XPais = "BRASIL",
                                    }
                                },
                                Dest = new Unimake.Business.DFe.Xml.CTe.Dest
                                {
                                    CNPJ = "00000000075108",
                                    IE = "ISENTO",
                                    XNome = "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                                    EnderDest = new Unimake.Business.DFe.Xml.CTe.EnderDest
                                    {
                                        XLgr = "R. GESSYR GONCALVES FONTES, 55",
                                        Nro = "55",
                                        XBairro = "CENTRO",
                                        CMun = 3305109,
                                        XMun = "SAO JOAO DE MERITI",
                                        CEP = "25520570",
                                        UF = UFBrasil.RJ,
                                        CPais = 1058,
                                        XPais = "BRASIL",
                                    },
                                },
                                VPrest = new Unimake.Business.DFe.Xml.CTe.VPrest
                                {
                                    VTPrest = 50.00,
                                    VRec = 50.00,
                                    Comp = new List<Unimake.Business.DFe.Xml.CTe.Comp>
                                    {
                                        new Unimake.Business.DFe.Xml.CTe.Comp
                                        {
                                            XNome = "FRETE VALOR",
                                            VComp = 50.00,
                                        },
                                    },
                                },
                                Imp = new Unimake.Business.DFe.Xml.CTe.Imp
                                {
                                    ICMS = new Unimake.Business.DFe.Xml.CTe.ICMS
                                    {
                                        ICMSSN = new Unimake.Business.DFe.Xml.CTe.ICMSSN
                                        {
                                            CST = "90",
                                            IndSN = SimNao.Sim,
                                        }
                                    }
                                },
                                InfCTeNorm = new Unimake.Business.DFe.Xml.CTe.InfCTeNorm
                                {
                                    InfCarga = new Unimake.Business.DFe.Xml.CTe.InfCarga
                                    {
                                        VCarga = 6252.96,
                                        ProPred = "MOVEIS",
                                        InfQ = new List<Unimake.Business.DFe.Xml.CTe.InfQ>
                                        {
                                            new Unimake.Business.DFe.Xml.CTe.InfQ
                                            {
                                                CUnid = CodigoUnidadeMedidaCTe.KG,
                                                TpMed ="PESO BRUTO",
                                                QCarga = 320.0000,
                                            },
                                            new Unimake.Business.DFe.Xml.CTe.InfQ
                                            {
                                                CUnid = CodigoUnidadeMedidaCTe.UNIDADE,
                                                TpMed ="UNIDADE",
                                                QCarga = 1.0000,
                                            },
                                        },
                                    },
                                    InfDoc = new Unimake.Business.DFe.Xml.CTe.InfDoc
                                    {
                                        InfNFe = new List<Unimake.Business.DFe.Xml.CTe.InfNFe>
                                        {
                                            new Unimake.Business.DFe.Xml.CTe.InfNFe
                                            {
                                                Chave = "41200306117473000150550030000652511417023254"
                                            },
                                        },
                                    },
                                    InfModal = new Unimake.Business.DFe.Xml.CTe.InfModal
                                    {
                                        VersaoModal="3.00",
                                        Rodo = new Unimake.Business.DFe.Xml.CTe.Rodo
                                        {
                                            RNTRC = "44957333",
                                            Occ = new List<Unimake.Business.DFe.Xml.CTe.Occ>
                                            {
                                                new Unimake.Business.DFe.Xml.CTe.Occ
                                                {
                                                    NOcc = 810,
                                                    DEmi = DateTime.Now,
                                                    EmiOcc = new Unimake.Business.DFe.Xml.CTe.EmiOcc
                                                    {
                                                        CNPJ = "31905001000109",
                                                        CInt = "0000001067",
                                                        IE = "9079649730",
                                                        UF = UFBrasil.PR,
                                                        Fone = "04434237530",
                                                    },
                                                },
                                            },
                                        },
                                    },
                                },
                                InfRespTec = new Unimake.Business.DFe.Xml.CTe.InfRespTec
                                {
                                    CNPJ = "06117473000150",
                                    XContato = "Wandrey Mundin Ferreira",
                                    Email= "wandrey@unimake.com.br",
                                    Fone = "04431414900",
                                },
                            },
                        },
                    }
                };

                #endregion CriarCTe

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var autorizacao = new Unimake.Business.DFe.Servicos.CTe.Autorizacao(xml, configuracao);
                autorizacao.Executar();
                MessageBox.Show(autorizacao.RetornoWSString);

                if (autorizacao.Result != null)
                {
                    MessageBox.Show(autorizacao.Result.XMotivo);

                    if (autorizacao.Result.CStat == 103) //103 = Lote Recebido com Sucesso
                    {
                        #region Finalizar através da consulta do recibo.

                        autorizacao.RetConsSitCTes.Clear(); //Zerar pois é um novo envio de CTe

                        var xmlRec = new ConsReciCTe
                        {
                            Versao = "3.00",
                            TpAmb = TipoAmbiente.Homologacao,
                            NRec = autorizacao.Result.InfRec.NRec
                        };

                        var configRec = new Configuracao
                        {
                            TipoDFe = TipoDFe.CTe,
                            CertificadoDigital = CertificadoSelecionado
                        };

                        var retAutorizacao = new Unimake.Business.DFe.Servicos.CTe.RetAutorizacao(xmlRec, configRec);
                        retAutorizacao.Executar();

                        autorizacao.RetConsReciCTe = retAutorizacao.Result;
                        autorizacao.GravarXmlDistribuicao(@"c:\testenfe\");

                        #endregion

                        #region Simulação da finalização do CTe através da consulta situação

                        autorizacao.RetConsReciCTe = null; //Zerar pois é um novo envio de CTe

                        var configSit = new Configuracao
                        {
                            TipoDFe = TipoDFe.CTe,
                            CertificadoDigital = CertificadoSelecionado
                        };

                        foreach (var item in xml.CTe)
                        {
                            var xmlSit = new ConsSitCTe
                            {
                                Versao = "3.00",
                                TpAmb = TipoAmbiente.Homologacao,
                                ChCTe = item.InfCTe.Chave
                            };

                            var consultaProtocolo = new Unimake.Business.DFe.Servicos.CTe.ConsultaProtocolo(xmlSit, configSit);
                            consultaProtocolo.Executar();

                            autorizacao.RetConsSitCTes.Add(consultaProtocolo.Result);
                        }

                        autorizacao.GravarXmlDistribuicao(@"c:\testenfe\");

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button39_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EnvEvento
                {
                    Versao = "1.00",
                    IdLote = "000000000000001",
                    Evento = new List<Evento> {
                        new Evento
                        {
                            Versao = "1.00",
                            InfEvento = new Unimake.Business.DFe.Xml.NFe.InfEvento(new Unimake.Business.DFe.Xml.NFe.DetEventoCancSubst
                            {
                                COrgaoAutor = UFBrasil.PR,
                                TpAutor = TipoAutor.EmpresaEmitente,
                                VerAplic = "v1.0",
                                NProt = "141190000660363",
                                Versao = "1.00",
                                XJust = "Justificativa para cancelamento da NFe de teste",
                                ChNFeRef = "41190806117473000150650010000579131943463123"
                            })
                            {
                                COrgao = UFBrasil.PR,
                                ChNFe = "41190806117473000150650010000579131943463890",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.CancelamentoPorSubstituicao,
                                NSeqEvento = 1,
                                VerEvento = "1.00",
                                TpAmb = TipoAmbiente.Homologacao
                            }
                        }
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.NFCe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var recepcaoEvento = new Unimake.Business.DFe.Servicos.NFCe.RecepcaoEvento(xml, configuracao);
                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                if (recepcaoEvento.Result.CStat == 128) //128 = Lote de evento processado com sucesso
                {
                    switch (recepcaoEvento.Result.RetEvento[0].InfEvento.CStat)
                    {
                        case 135: //Evento homologado com vinculação da respectiva NFe
                        case 136: //Evento homologado sem vinculação com a respectiva NFe (SEFAZ não encontrou a NFe na base dela)
                        case 155: //Evento de Cancelamento homologado fora do prazo permitido para cancelamento
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;

                        default: //Evento rejeitado
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }

        }

        private void button40_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EventoMDFe
                {
                    Versao = "3.00",
                    InfEvento = new Unimake.Business.DFe.Xml.MDFe.InfEvento(new Unimake.Business.DFe.Xml.MDFe.DetEventoIncCondutor
                    {
                        VersaoEvento = "3.00",
                        DescEvento = "Inclusao Condutor",
                        EventoIncCondutor = new EventoIncCondutor
                        {
                            Condutor = new List<CondutorMDFe>
                            {
                                new CondutorMDFe
                                {
                                    XNome = "JOSE ALMEIDA",
                                    CPF = "00000000191"

                                }
                            }
                        }
                    })
                    {
                        COrgao = UFBrasil.PR,
                        ChMDFe = "41200380568835000181580010000007171930099252",
                        CNPJ = "80568835000181",
                        DhEvento = DateTime.Now,
                        TpEvento = TipoEventoMDFe.InclusaoCondutor,
                        NSeqEvento = 1,
                        TpAmb = TipoAmbiente.Homologacao
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.MDFe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var recepcaoEvento = new Unimake.Business.DFe.Servicos.MDFe.RecepcaoEvento(xml, configuracao);

                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.InfEvento.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (recepcaoEvento.Result.InfEvento.CStat)
                {
                    case 134: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respectivo CT-e com situação diferente de Autorizada.
                    case 135: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respetivo CTe.
                    case 136: //Recebido pelo Sistema de Registro de Eventos – vinculação do evento ao respectivo CT-e prejudicado.
                        recepcaoEvento.GravarXmlDistribuicao(@"c:\testecte\");
                        break;

                    default:
                        //Quando o evento é rejeitado pela Sefaz.
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button41_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EventoMDFe
                {
                    Versao = "3.00",
                    InfEvento = new Unimake.Business.DFe.Xml.MDFe.InfEvento(new Unimake.Business.DFe.Xml.MDFe.DetEventoIncDFeMDFe
                    {
                        VersaoEvento = "3.00",
                        EventoIncDFeMDFe = new EventoIncDFeMDFe
                        {
                            DescEvento = "Inclusao DF-e",
                            NProt = "941190000014312",
                            CMunCarrega = "4118402",
                            XMunCarrega = "PARANAVAI",

                            InfDoc = new List<Unimake.Business.DFe.Xml.MDFe.InfDoc>
                            {
                                new Unimake.Business.DFe.Xml.MDFe.InfDoc
                                {
                                    CMunDescarga = "4117107",
                                    XMunDescarga = "NOVA LONDRINA",
                                    ChNFe = "41190606117473000150550020000025691118027981",
                                }
                            }
                        }
                    })
                    {
                        COrgao = UFBrasil.PR,
                        ChMDFe = "41200380568835000181580010000007171930099252",
                        CNPJ = "80568835000181",
                        DhEvento = DateTime.Now,
                        TpEvento = TipoEventoMDFe.InclusaoDFe,
                        NSeqEvento = 1,
                        TpAmb = TipoAmbiente.Homologacao
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.MDFe,
                    CertificadoDigital = CertificadoSelecionado
                };


                var recepcaoEvento = new Unimake.Business.DFe.Servicos.MDFe.RecepcaoEvento(xml, configuracao);

                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.InfEvento.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (recepcaoEvento.Result.InfEvento.CStat)
                {
                    case 134: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respectivo CT-e com situação diferente de Autorizada.
                    case 135: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respetivo CTe.
                    case 136: //Recebido pelo Sistema de Registro de Eventos – vinculação do evento ao respectivo CT-e prejudicado.
                        recepcaoEvento.GravarXmlDistribuicao(@"c:\testecte\");
                        break;

                    default:
                        //Quando o evento é rejeitado pela Sefaz.
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button42_Click(object sender, EventArgs e)
        {
            try
            {
                #region CriarMDFe

                var xml = new EnviMDFe
                {
                    Versao = "3.00",
                    IdLote = "000000000000001",
                    MDFe = new MDFe
                    {

                        InfMDFe = new InfMDFe
                        {
                            Versao = "3.00",

                            Ide = new Unimake.Business.DFe.Xml.MDFe.Ide
                            {
                                CUF = UFBrasil.PR,
                                TpAmb = TipoAmbiente.Homologacao,
                                TpEmit = TipoEmitenteMDFe.PrestadorServicoTransporte,
                                Mod = ModeloDFe.MDFe,
                                Serie = 1,
                                NMDF = 861,
                                CMDF = "01722067",
                                Modal = ModalidadeTransporteMDFe.Rodoviario,
                                DhEmi = DateTime.Now,
                                TpEmis = TipoEmissao.Normal,
                                ProcEmi = ProcessoEmissao.AplicativoContribuinte,
                                VerProc = "UNICO V8.0",
                                UFIni = UFBrasil.PR,
                                UFFim = UFBrasil.SP,
                                InfMunCarrega = new List<InfMunCarrega>
                                {
                                    new InfMunCarrega
                                    {
                                        CMunCarrega = 4118402,
                                        XMunCarrega = "PARANAVAI"

                                    }
                                },
                                DhIniViagem = DateTime.Now,

                            },
                            Emit = new Unimake.Business.DFe.Xml.MDFe.Emit
                            {
                                CNPJ = "31905001000109",
                                IE = "9079649730",
                                XNome = "EXATUS MOVEIS EIRELI",
                                XFant = "EXATUS MOVEIS",
                                EnderEmit = new Unimake.Business.DFe.Xml.MDFe.EnderEmit
                                {
                                    XLgr = "RUA JOAQUIM F. DE SOUZA",
                                    Nro = "01112",
                                    XBairro = "VILA TEREZINHA",
                                    CMun = 4118402,
                                    XMun = "PARANAVAI",
                                    CEP = "87706675",
                                    UF = UFBrasil.PR,
                                    Fone = "04434237530",
                                },
                            },
                            InfModal = new Unimake.Business.DFe.Xml.MDFe.InfModal
                            {
                                VersaoModal = "3.00",
                                Rodo = new Unimake.Business.DFe.Xml.MDFe.Rodo
                                {
                                    InfANTT = new InfANTT
                                    {
                                        RNTRC = "44957333",
                                        InfContratante = new List<InfContratante>
                                        {
                                            new InfContratante
                                            {
                                                CNPJ = "80568835000181"
                                            },
                                            new InfContratante
                                            {
                                                CNPJ = "10197843000183"
                                            }
                                        }
                                    },
                                    VeicTracao = new VeicTracao
                                    {
                                        CInt = "AXF8500",
                                        Placa = "AXF8500",
                                        Tara = 0,
                                        CapKG = 5000,
                                        Prop = new Unimake.Business.DFe.Xml.MDFe.Prop
                                        {
                                            CNPJ = "31905001000109",
                                            RNTRC = "44957333",
                                            XNome = "EXATUS MOVEIS EIRELI",
                                            IE = "9079649730",
                                            UF = UFBrasil.PR,
                                            TpProp = TipoProprietarioMDFe.Outros
                                        },
                                        Condutor = new List<Condutor>
                                        {
                                            new Condutor
                                            {
                                                XNome = "ADEMILSON LOPES DE SOUZA",
                                                CPF = "27056461832"
                                            }
                                        },
                                        TpRod = TipoRodado.Toco,
                                        TpCar = TipoCarroceriaMDFe.FechadaBau,
                                        UF = UFBrasil.PR
                                    },
                                }
                            },
                            InfDoc = new InfDocInfMDFe
                            {
                                InfMunDescarga = new List<InfMunDescarga>
                                {
                                    new InfMunDescarga
                                    {
                                        CMunDescarga = 3505708,
                                        XMunDescarga = "BARUERI",
                                        InfCTe = new List<InfMunDescargaInfCTe>
                                        {
                                            new InfMunDescargaInfCTe
                                            {
                                                ChCTe = "41200531905001000109570010000009551708222466"
                                            },
                                            new InfMunDescargaInfCTe
                                            {
                                                ChCTe = "41200531905001000109570010000009561308222474"
                                            }
                                        },
                                        InfNFe = new List<InfMunDescargaInfNFe>
                                        {
                                            new InfMunDescargaInfNFe
                                            {
                                                ChNFe = "12345678901234567890123456789012345678901234",
                                                InfUnidTransp = new List<Unimake.Business.DFe.Xml.MDFe.InfUnidTransp>
                                                {
                                                    new Unimake.Business.DFe.Xml.MDFe.InfUnidTransp
                                                    {
                                                        IdUnidTransp = "122",
                                                        TpUnidTransp = TipoUnidadeTransporte.RodoviarioReboque,
                                                        LacUnidTransp = new List<Unimake.Business.DFe.Xml.MDFe.LacUnidTransp>
                                                        {
                                                            new Unimake.Business.DFe.Xml.MDFe.LacUnidTransp
                                                            {
                                                                NLacre = "12334"
                                                            }
                                                        },
                                                        InfUnidCarga = new List<Unimake.Business.DFe.Xml.MDFe.InfUnidCarga>
                                                        {
                                                            new Unimake.Business.DFe.Xml.MDFe.InfUnidCarga
                                                            {
                                                                TpUnidCarga = TipoUnidadeCarga.Container,
                                                                IdUnidCarga = "123",
                                                                LacUnidCarga = new List<Unimake.Business.DFe.Xml.MDFe.LacUnidCarga>
                                                                {
                                                                    new Unimake.Business.DFe.Xml.MDFe.LacUnidCarga
                                                                    {
                                                                        NLacre = "3333333"
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    },
                                    new InfMunDescarga
                                    {
                                        CMunDescarga = 3550308,
                                        XMunDescarga = "SAO PAULO",
                                        InfCTe = new List<InfMunDescargaInfCTe>
                                        {
                                            new InfMunDescargaInfCTe
                                            {
                                                ChCTe = "41200531905001000109570010000009581608222490"
                                            }
                                        }
                                    }
                                }
                            },
                            ProdPred = new ProdPred
                            {
                                TpCarga = TipoCargaMDFe.CargaGeral,
                                XProd = "TESTE DE PRODUTO PREDOMINANTE",
                                InfLotacao = new InfLotacao
                                {
                                    InfLocalCarrega = new InfLocalCarrega
                                    {
                                        CEP = "87302080"
                                    },
                                    InfLocalDescarrega = new InfLocalDescarrega
                                    {
                                        CEP = "25650208"
                                    }
                                }
                            },
                            Seg = new List<Unimake.Business.DFe.Xml.MDFe.Seg>
                            {
                                new Unimake.Business.DFe.Xml.MDFe.Seg
                                {
                                    InfResp = new InfResp
                                    {
                                        RespSeg = ResponsavelSeguroMDFe.EmitenteMDFe,
                                        CNPJ = "31905001000109"
                                    },
                                    InfSeg = new Unimake.Business.DFe.Xml.MDFe.InfSeg
                                    {
                                        XSeg = "PORTO SEGURO",
                                        CNPJ = "61198164000160"
                                    },
                                    NApol = "053179456362",
                                    NAver = new List<string>
                                    {
                                        {
                                            "0000000000000000000000000000000000000000"
                                        },
                                        {
                                            "0000000000000000000000000000000000000001"
                                        },
                                    }
                                }
                            },
                            Tot = new Tot
                            {
                                QCTe = 3,
                                VCarga = 56599.09,
                                CUnid = CodigoUnidadeMedidaMDFe.KG,
                                QCarga = 2879.00
                            },
                            Lacres = new List<Unimake.Business.DFe.Xml.MDFe.Lacre>
                            {
                                new Unimake.Business.DFe.Xml.MDFe.Lacre
                                {
                                    NLacre = "1111111"
                                },
                                new Unimake.Business.DFe.Xml.MDFe.Lacre
                                {
                                    NLacre = "22222"
                                }

                            },
                            InfAdic = new Unimake.Business.DFe.Xml.MDFe.InfAdic
                            {
                                InfCpl = "DATA/HORA PREVISTA PARA O INICO DA VIAGEM: 10/08/2020 as 08:00"
                            },
                            InfRespTec = new Unimake.Business.DFe.Xml.MDFe.InfRespTec
                            {
                                CNPJ = "06117473000150",
                                XContato = "Wandrey Mundin Ferreira",
                                Email = "wandrey@unimake.com.br",
                                Fone = "04431414900",
                            },
                        },
                    },
                };

                #endregion CriarMDFe

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.MDFe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var autorizacao = new Unimake.Business.DFe.Servicos.MDFe.Autorizacao(xml, configuracao);
                autorizacao.Executar();

                MessageBox.Show(autorizacao.RetornoWSString);

                if (autorizacao.Result != null)
                {
                    MessageBox.Show(autorizacao.Result.XMotivo);

                    if (autorizacao.Result.CStat == 103) //103 = Lote Recebido com Sucesso
                    {
                        // Finalizar através da consulta do recibo.
                        var xmlRec = new ConsReciMDFe
                        {
                            Versao = "3.00",
                            TpAmb = TipoAmbiente.Homologacao,
                            NRec = autorizacao.Result.InfRec.NRec
                        };

                        var configRec = new Configuracao
                        {
                            TipoDFe = TipoDFe.MDFe,
                            CertificadoDigital = CertificadoSelecionado
                        };

                        var retAutorizacao = new Unimake.Business.DFe.Servicos.MDFe.RetAutorizacao(xmlRec, configRec);
                        retAutorizacao.Executar();

                        autorizacao.RetConsReciMDFe = retAutorizacao.Result;
                        autorizacao.GravarXmlDistribuicao(@"c:\testenfe\");

                        //Simulação da finalização do CTe através da consulta situação
                        autorizacao.RetConsReciMDFe = null; //Zerar para conseguir testar

                        var xmlSit = new ConsSitMDFe
                        {
                            Versao = "3.00",
                            TpAmb = TipoAmbiente.Homologacao,
                            ChMDFe = xml.MDFe.InfMDFe.Chave
                        };

                        var configSit = new Configuracao
                        {
                            TipoDFe = TipoDFe.MDFe,
                            CertificadoDigital = CertificadoSelecionado
                        };

                        var consultaProtocolo = new Unimake.Business.DFe.Servicos.MDFe.ConsultaProtocolo(xmlSit, configSit);
                        consultaProtocolo.Executar();

                        autorizacao.RetConsSitMDFe.Add(consultaProtocolo.Result);
                        autorizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        /// <summary>
        /// Converte uma cadeia de caracteres para um encoding específico
        /// </summary>
        /// <param name="value">Cadeia de caracteres a ser convertido</param>
        /// <param name="encodingSource">Encoding de origem</param>
        /// <param name="encodingDestination">Encoding de destino</param>
        /// <returns>Retorna o conteúdo convertido para o encoding de destino</returns>
        private string EncodingConvert(string value, Encoding encodingSource, Encoding encodingDestination)
        {
            var encodingOrigem = Encoding.Default;
            var encodingDestino = Encoding.UTF8;


            var origemBytes = encodingOrigem.GetBytes(value);
            var bytes = Encoding.Convert(encodingOrigem, encodingDestino, origemBytes);
            var chars = new char[encodingDestino.GetCharCount(bytes, 0, bytes.Length)];
            encodingDestino.GetChars(bytes, 0, bytes.Length, chars, 0);
            var convertido = new string(chars);

            var teste = Encoding.UTF8.GetString(Encoding.Default.GetBytes(value));

            return convertido;
        }

        private void button43_Click(object sender, EventArgs e)
        {
            try
            {
                var xNome = EncodingConvert("UNIMAKE SOLUÇÕES CORPORATIVAS LTDA", Encoding.Default, Encoding.UTF8);

                var xml = new Unimake.Business.DFe.Xml.NFe.EnviNFe
                {
                    Versao = "4.00",
                    IdLote = "000000000000001",
                    IndSinc = SimNao.Nao,
                    NFe = new List<Unimake.Business.DFe.Xml.NFe.NFe>
                {
                    new Unimake.Business.DFe.Xml.NFe.NFe
                    {
                        InfNFe = new List<Unimake.Business.DFe.Xml.NFe.InfNFe>
                        {
                            new Unimake.Business.DFe.Xml.NFe.InfNFe
                            {
                                Versao = "4.00",
                                Ide = new Unimake.Business.DFe.Xml.NFe.Ide
                                {
                                    CUF = UFBrasil.PR,
                                    NatOp = "VENDA PRODUC.DO ESTABELEC",
                                    Mod = ModeloDFe.NFe,
                                    Serie = 1,
                                    NNF = 58004,
                                    DhEmi = DateTime.Now,
                                    DhSaiEnt = DateTime.Now,
                                    TpNF = TipoOperacao.Saida,
                                    IdDest = DestinoOperacao.OperacaoInterestadual,
                                    CMunFG = 4118402,
                                    TpImp = FormatoImpressaoDANFE.NormalRetrato,
                                    TpEmis = TipoEmissao.Normal,
                                    TpAmb = TipoAmbiente.Homologacao,
                                    FinNFe = FinalidadeNFe.Normal,
                                    IndFinal = SimNao.Sim,
                                    IndPres = IndicadorPresenca.OperacaoPresencial,
                                    ProcEmi = ProcessoEmissao.AplicativoContribuinte,
                                    VerProc = "TESTE 1.00"
                                },
                                Emit = new Unimake.Business.DFe.Xml.NFe.Emit
                                {
                                    CNPJ = "06117473000150",
                                    XNome = "UNIMAKE SOLUÇÕES CORPORATIVAS LTDA",
                                    XFant = "UNIMAKE - PARANAVAI",
                                    EnderEmit = new Unimake.Business.DFe.Xml.NFe.EnderEmit
                                    {
                                        XLgr = "RUA ANTONIO FELIPE",
                                        Nro = "1500",
                                        XBairro = "CENTRO",
                                        CMun = 4118402,
                                        XMun = "PARANAVAI",
                                        UF = UFBrasil.PR,
                                        CEP = "87704030",
                                        Fone = "04431414900"
                                    },
                                    IE = "9032000301",
                                    IM = "14018",
                                    CNAE = "6202300",
                                    CRT = CRT.SimplesNacional
                                },
                                Dest = new Unimake.Business.DFe.Xml.NFe.Dest
                                {
                                    CNPJ = "04218457000128",
                                    XNome = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                                    EnderDest = new Unimake.Business.DFe.Xml.NFe.EnderDest
                                    {
                                        XLgr = "AVENIDA DA SAUDADE",
                                        Nro = "1555",
                                        XBairro = "CAMPOS ELISEOS",
                                        CMun = 3543402,
                                        XMun = "RIBEIRAO PRETO",
                                        UF = UFBrasil.SP,
                                        CEP = "14080000",
                                        Fone = "01639611500"
                                    },
                                    IndIEDest = IndicadorIEDestinatario.ContribuinteICMS,
                                    IE = "582614838110",
                                    Email = "teste@gmail.com.br"
                                },
                                Det = CriarDet(),
                                Total = new Unimake.Business.DFe.Xml.NFe.Total
                                {
                                    ICMSTot = new Unimake.Business.DFe.Xml.NFe.ICMSTot
                                    {
                                        VBC = 0,
                                        VICMS = 0,
                                        VICMSDeson = 0,
                                        VFCP = 0,
                                        VBCST = 0,
                                        VST = 0,
                                        VFCPST = 0,
                                        VFCPSTRet = 0,
                                        VProd = 84.90,
                                        VFrete = 0,
                                        VSeg = 0,
                                        VDesc = 0,
                                        VII = 0,
                                        VIPI = 0,
                                        VIPIDevol = 0,
                                        VPIS = 0,
                                        VCOFINS = 0,
                                        VOutro = 0,
                                        VNF = 84.90,
                                        VTotTrib = 12.63
                                    }
                                },
                                Transp = new Unimake.Business.DFe.Xml.NFe.Transp
                                {
                                    ModFrete = ModalidadeFrete.ContratacaoFretePorContaRemetente_CIF,
                                    Vol = new List<Unimake.Business.DFe.Xml.NFe.Vol>
                                    {
                                        new Unimake.Business.DFe.Xml.NFe.Vol
                                        {
                                            QVol = 1,
                                            Esp = "LU",
                                            Marca = "UNIMAKE",
                                            PesoL = 0.000,
                                            PesoB = 0.000
                                        }
                                    }
                                },
                                Cobr = new Unimake.Business.DFe.Xml.NFe.Cobr()
                                {
                                    Fat = new Unimake.Business.DFe.Xml.NFe.Fat
                                    {
                                        NFat = "057910",
                                        VOrig = 84.90,
                                        VDesc = 0,
                                        VLiq = 84.90
                                    },
                                    Dup = new List<Unimake.Business.DFe.Xml.NFe.Dup>
                                    {
                                        new Unimake.Business.DFe.Xml.NFe.Dup
                                        {
                                            NDup = "001",
                                            DVenc = DateTime.Now,
                                            VDup = 84.90
                                        }
                                    }
                                },
                                Pag = new Unimake.Business.DFe.Xml.NFe.Pag
                                {
                                    DetPag = new List<Unimake.Business.DFe.Xml.NFe.DetPag>
                                    {
                                        new Unimake.Business.DFe.Xml.NFe.DetPag
                                        {
                                            IndPag = IndicadorPagamento.PagamentoVista,
                                            TPag = MeioPagamento.Dinheiro,
                                            VPag = 84.90
                                        }
                                    }
                                },
                                InfAdic = new Unimake.Business.DFe.Xml.NFe.InfAdic
                                {
                                    InfCpl = ";CONTROLE: 0000241197;PEDIDO(S) ATENDIDO(S): 300474;Empresa optante pelo simples nacional, conforme lei compl. 128 de 19/12/2008;Permite o aproveitamento do credito de ICMS no valor de R$ 2,40, correspondente ao percentual de 2,83% . Nos termos do Art. 23 - LC 123/2006 (Resolucoes CGSN n. 10/2007 e 53/2008);Voce pagou aproximadamente: R$ 6,69 trib. federais / R$ 5,94 trib. estaduais / R$ 0,00 trib. municipais. Fonte: IBPT/empresometro.com.br 18.2.B A3S28F;",
                                },
                                InfRespTec = new Unimake.Business.DFe.Xml.NFe.InfRespTec
                                {
                                    CNPJ = "06117473000150",
                                    XContato = "Wandrey Mundin Ferreira",
                                    Email = "wandrey@unimake.com.br",
                                    Fone = "04431414900"
                                }
                            }
                        }
                    },
                    new Unimake.Business.DFe.Xml.NFe.NFe
                    {
                        InfNFe = new List<Unimake.Business.DFe.Xml.NFe.InfNFe>
                        {
                            new Unimake.Business.DFe.Xml.NFe.InfNFe
                            {
                                Versao = "4.00",
                                Ide = new Unimake.Business.DFe.Xml.NFe.Ide
                                {
                                    CUF = UFBrasil.PR,
                                    NatOp = "VENDA PRODUC.DO ESTABELEC",
                                    Mod = ModeloDFe.NFe,
                                    Serie = 1,
                                    NNF = 58005,
                                    DhEmi = DateTime.Now,
                                    DhSaiEnt = DateTime.Now,
                                    TpNF = TipoOperacao.Saida,
                                    IdDest = DestinoOperacao.OperacaoInterestadual,
                                    CMunFG = 4118402,
                                    TpImp = FormatoImpressaoDANFE.NormalRetrato,
                                    TpEmis = TipoEmissao.Normal,
                                    TpAmb = TipoAmbiente.Homologacao,
                                    FinNFe = FinalidadeNFe.Normal,
                                    IndFinal = SimNao.Sim,
                                    IndPres = IndicadorPresenca.OperacaoPresencial,
                                    ProcEmi = ProcessoEmissao.AplicativoContribuinte,
                                    VerProc = "TESTE 1.00"
                                },
                                Emit = new Unimake.Business.DFe.Xml.NFe.Emit
                                {
                                    CNPJ = "06117473000150",
                                    XNome = "UNIMAKE SOLUCOES CORPORATIVAS LTDA",
                                    XFant = "UNIMAKE - PARANAVAI",
                                    EnderEmit = new Unimake.Business.DFe.Xml.NFe.EnderEmit
                                    {
                                        XLgr = "RUA ANTONIO FELIPE",
                                        Nro = "1500",
                                        XBairro = "CENTRO",
                                        CMun = 4118402,
                                        XMun = "PARANAVAI",
                                        UF = UFBrasil.PR,
                                        CEP = "87704030",
                                        Fone = "04431414900"
                                    },
                                    IE = "9032000301",
                                    IM = "14018",
                                    CNAE = "6202300",
                                    CRT = CRT.SimplesNacional
                                },
                                Dest = new Unimake.Business.DFe.Xml.NFe.Dest
                                {
                                    CNPJ = "04218457000128",
                                    XNome = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                                    EnderDest = new Unimake.Business.DFe.Xml.NFe.EnderDest
                                    {
                                        XLgr = "AVENIDA DA SAUDADE",
                                        Nro = "1555",
                                        XBairro = "CAMPOS ELISEOS",
                                        CMun = 3543402,
                                        XMun = "RIBEIRAO PRETO",
                                        UF = UFBrasil.SP,
                                        CEP = "14080000",
                                        Fone = "01639611500"
                                    },
                                    IndIEDest = IndicadorIEDestinatario.ContribuinteICMS,
                                    IE = "582614838110",
                                    Email = "janelaorp@janelaorp.com.br"
                                },
                                Det = CriarDet(),
                                Total = new Unimake.Business.DFe.Xml.NFe.Total
                                {
                                    ICMSTot = new Unimake.Business.DFe.Xml.NFe.ICMSTot
                                    {
                                        VBC = 0,
                                        VICMS = 0,
                                        VICMSDeson = 0,
                                        VFCP = 0,
                                        VBCST = 0,
                                        VST = 0,
                                        VFCPST = 0,
                                        VFCPSTRet = 0,
                                        VProd = 84.90,
                                        VFrete = 0,
                                        VSeg = 0,
                                        VDesc = 0,
                                        VII = 0,
                                        VIPI = 0,
                                        VIPIDevol = 0,
                                        VPIS = 0,
                                        VCOFINS = 0,
                                        VOutro = 0,
                                        VNF = 84.90,
                                        VTotTrib = 12.63
                                    }
                                },
                                Transp = new Unimake.Business.DFe.Xml.NFe.Transp
                                {
                                    ModFrete = ModalidadeFrete.ContratacaoFretePorContaRemetente_CIF,
                                    Vol = new List<Unimake.Business.DFe.Xml.NFe.Vol>
                                    {
                                        new Unimake.Business.DFe.Xml.NFe.Vol
                                        {
                                            QVol = 1,
                                            Esp = "LU",
                                            Marca = "UNIMAKE",
                                            PesoL = 0.000,
                                            PesoB = 0.000
                                        }
                                    }
                                },
                                Cobr = new Unimake.Business.DFe.Xml.NFe.Cobr()
                                {
                                    Fat = new Unimake.Business.DFe.Xml.NFe.Fat
                                    {
                                        NFat = "057910",
                                        VOrig = 84.90,
                                        VDesc = 0,
                                        VLiq = 84.90
                                    },
                                    Dup = new List<Unimake.Business.DFe.Xml.NFe.Dup>
                                    {
                                        new Unimake.Business.DFe.Xml.NFe.Dup
                                        {
                                            NDup = "001",
                                            DVenc = DateTime.Now,
                                            VDup = 84.90
                                        }
                                    }
                                },
                                Pag = new Unimake.Business.DFe.Xml.NFe.Pag
                                {
                                    DetPag = new List<Unimake.Business.DFe.Xml.NFe.DetPag>
                                    {
                                        new Unimake.Business.DFe.Xml.NFe.DetPag
                                        {
                                            IndPag = IndicadorPagamento.PagamentoVista,
                                            TPag = MeioPagamento.Dinheiro,
                                            VPag = 84.90
                                        }
                                    }
                                },
                                InfAdic = new Unimake.Business.DFe.Xml.NFe.InfAdic
                                {
                                    InfCpl = ";CONTROLE: 0000241197;PEDIDO(S) ATENDIDO(S): 300474;Empresa optante pelo simples nacional, conforme lei compl. 128 de 19/12/2008;Permite o aproveitamento do credito de ICMS no valor de R$ 2,40, correspondente ao percentual de 2,83% . Nos termos do Art. 23 - LC 123/2006 (Resolucoes CGSN n. 10/2007 e 53/2008);Voce pagou aproximadamente: R$ 6,69 trib. federais / R$ 5,94 trib. estaduais / R$ 0,00 trib. municipais. Fonte: IBPT/empresometro.com.br 18.2.B A3S28F;",
                                },
                                InfRespTec = new Unimake.Business.DFe.Xml.NFe.InfRespTec
                                {
                                    CNPJ = "06117473000150",
                                    XContato = "Wandrey Mundin Ferreira",
                                    Email = "wandrey@unimake.com.br",
                                    Fone = "04431414900"
                                }
                            }
                        }
                    }
                }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.NFe,
                    Servico = Servico.NFeStatusServico,
                    CertificadoDigital = CertificadoSelecionado
                };

                var autorizacao = new Autorizacao(xml, configuracao);

                //var xmlNFe = autorizacao.GetConteudoNFeAssinada(0);

                //var xmlNFe2 = autorizacao.GetConteudoNFeAssinada(1);

                autorizacao.Executar();


                var nfeProc = autorizacao.NfeProcResult;

                if (autorizacao.Result != null)
                {
                    //Gravar o arquivo do conteúdo retornado em uma pasta qualquer para ter em segurança. Pode-se também gravar na base de dados. Fica a critério de cada um.
                    File.WriteAllText(@"c:\testenfe\retorno\nomearquivoretorno.xml", autorizacao.RetornoWSString);

                    MessageBox.Show(autorizacao.Result.XMotivo);

                    if (autorizacao.Result.CStat == 103) //103 = Lote Recebido com Sucesso
                    {
                        #region Finalizar através da consulta do recibo.

                        autorizacao.RetConsSitNFes.Clear(); //Zerar pois é um novo envio de NFe

                        var xmlRec = new ConsReciNFe
                        {
                            Versao = "4.00",
                            TpAmb = TipoAmbiente.Homologacao,
                            NRec = autorizacao.Result.InfRec.NRec
                        };

                        var configRec = new Configuracao
                        {
                            TipoDFe = TipoDFe.NFe,
                            CertificadoDigital = CertificadoSelecionado
                        };

                        var retAutorizacao = new Unimake.Business.DFe.Servicos.NFe.RetAutorizacao(xmlRec, configRec)
                        {
                            EnviNFe = XMLUtility.Deserializar<EnviNFe>(autorizacao.ConteudoXMLAssinado.OuterXml)
                        };
                        retAutorizacao.Executar();
                      

                        //Código para conferir o DigestValue da assinatura, considerando que pode ter mais de uma NFe no lote.
                        foreach (var nfe in autorizacao.EnviNFe.NFe)
                        {
                            foreach (var protNFe in retAutorizacao.Result.ProtNFe)
                            {
                                if (protNFe.InfProt.ChNFe == nfe.InfNFe[0].Chave)
                                {
                                    if (protNFe.InfProt.CStat == 100) //NFe autorizada. Só compara o DigestValue se a nota tiver autorizada
                                    {
                                        if (protNFe.InfProt.DigVal != nfe.Signature.SignedInfo.Reference.DigestValue)
                                        {
                                            MessageBox.Show("Valor de resumo da assinatura digital retornado na autorização difere do enviado na assinatura da NFe.");
                                        }
                                        else
                                        {
                                            MessageBox.Show("Valor de resumo da assinatura digital retornado na autorização está correto.");
                                        }
                                    }
                                }
                            }
                        }

                        //Código para conferir o DigestValue da assinatura, considerando que sempre vai ter somente 1 nota no lote.
                        if (retAutorizacao.Result.ProtNFe[0].InfProt.CStat == 100) //NFe autorizada. Só compara o DigestValue se a nota tiver autorizada
                        {
                            if (retAutorizacao.Result.ProtNFe[0].InfProt.DigVal != autorizacao.EnviNFe.NFe[0].Signature.SignedInfo.Reference.DigestValue)
                            {
                                MessageBox.Show("Valor de resumo da assinatura digital retornado na autorização difere do enviado na assinatura da NFe.");
                            }
                            else
                            {
                                MessageBox.Show("Valor de resumo da assinatura digital retornado na autorização está correto.");
                            }
                        }

                        autorizacao.RetConsReciNFe = retAutorizacao.Result;



                        var mes = xml.NFe[0].InfNFe[0].Ide.DhEmi.Month;
                        var ano = xml.NFe[0].InfNFe[0].Ide.DhEmi.Year;
                        var pastaEnviados = @"c:\testenfe\" + ano.ToString("0000") + mes.ToString("00") + "\\";

                        if (!Directory.Exists(pastaEnviados))
                        {
                            Directory.CreateDirectory(pastaEnviados);
                        }

                        autorizacao.GravarXmlDistribuicao(pastaEnviados);

                        #endregion

                        #region Simulação da finalização do NFe através da consulta situação

                        autorizacao.RetConsReciNFe = null; //Zerar pois é um novo envio de NFe

                        var configSit = new Configuracao
                        {
                            TipoDFe = TipoDFe.NFe,
                            CertificadoDigital = CertificadoSelecionado
                        };

                        foreach (var item in xml.NFe)
                        {
                            var xmlSit = new ConsSitNFe
                            {
                                Versao = "4.00",
                                TpAmb = TipoAmbiente.Homologacao,
                                ChNFe = item.InfNFe[0].Chave
                            };

                            var consultaProtocolo = new Unimake.Business.DFe.Servicos.NFe.ConsultaProtocolo(xmlSit, configSit);
                            consultaProtocolo.Executar();

                            autorizacao.RetConsSitNFes.Add(consultaProtocolo.Result);
                        }

                        autorizacao.GravarXmlDistribuicao(@"c:\testenfe\");

                        #endregion
                    }
                }

                if (autorizacao.Result.ProtNFe != null)
                {
                    //Gravar o XML de distribuição se a nota foi autorizada ou denegada
                    switch (autorizacao.Result.ProtNFe.InfProt.CStat)
                    {
                        case 100: //Autorizado o uso da NF-e
                        case 110: //Uso Denegado
                        case 150: //Autorizado o uso da NF-e, autorização fora de prazo
                        case 205: //NF-e está denegada na base de dados da SEFAZ [nRec:999999999999999]
                        case 301: //Uso Denegado: Irregularidade fiscal do emitente
                        case 302: //Uso Denegado: Irregularidade fiscal do destinatário
                        case 303: //Uso Denegado: Destinatário não habilitado a operar na UF
                            autorizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                            var docProcNFe = autorizacao.NfeProcResult.GerarXML();
                            MessageBox.Show(autorizacao.NfeProcResult.NomeArquivoDistribuicao);

                            break;

                        default: //NF Rejeitada
                            autorizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button45_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EventoMDFe
                {
                    Versao = "3.00",
                    InfEvento = new Unimake.Business.DFe.Xml.MDFe.InfEvento(new Unimake.Business.DFe.Xml.MDFe.DetEventoEncMDFe
                    {
                        NProt = "931210005423815",
                        VersaoEvento = "3.00",
                        CMun = 3106200,
                        CUF = UFBrasil.MG,
                        DtEnc = DateTime.Now
                    })
                    {
                        COrgao = UFBrasil.MG,
                        ChMDFe = "31210400023195924668589270000000111000001897",
                        //CNPJ = "10859283000185",
                        CPF = "23195924668",
                        DhEvento = DateTime.Now,
                        TpEvento = TipoEventoMDFe.Encerramento,
                        NSeqEvento = 1,
                        TpAmb = TipoAmbiente.Homologacao
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.MDFe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var recepcaoEvento = new Unimake.Business.DFe.Servicos.MDFe.RecepcaoEvento(xml, configuracao);

                //xml.LerXML<EventoCTe>(recepcaoEvento.ConteudoXMLAssinado);

                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.InfEvento.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (recepcaoEvento.Result.InfEvento.CStat)
                {
                    case 134: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respectivo CT-e com situação diferente de Autorizada.
                    case 135: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respetivo CTe.
                    case 136: //Recebido pelo Sistema de Registro de Eventos – vinculação do evento ao respectivo CT-e prejudicado.
                        recepcaoEvento.GravarXmlDistribuicao(@"c:\testecte\");
                        break;

                    default:
                        //Quando o evento é rejeitado pela Sefaz.
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button46_Click(object sender, EventArgs e)
        {
            #region Vou ler um XML para simplificar, mas neste ponto tem que montar o objeto do XML da NFe exatamente como ele foi enviado e autorizado para não dar diferença de assinatura

            var xml = CriarNFe();

            //TODO:  Nunca usado, pode apagar?
            //var xmlNFe = new NFe();
            //var xml = new EnviNFe
            //{
            //    IdLote = "000001",
            //    IndSinc = SimNao.Nao,
            //    Versao = "4.00",
            //    NFe = new List<NFe>
            //    {
            //        Unimake.Business.DFe.Xml.NFe.LoadFromFile(@"C:\Users\Wandrey\Downloads\31201037364506000190550010000000541000000568-NFe (1).xml")
            //    }
            //};

            //var doc = new XmlDocument();
            //doc.LoadXml(conteudoXML);

            //var xmlNFe = new NFe();
            //var xml = new EnviNFe
            //{
            //    IdLote = "000001",
            //    IndSinc = SimNao.Nao,
            //    Versao = "4.00",
            //    NFe = new List<NFe>
            //    {
            //        Unimake.Business.DFe.Utility.XMLUtility.Deserializar<NFe>(doc)
            //    }
            //};

            #endregion

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.NFe,
                CertificadoDigital = CertificadoSelecionado
            };

            var autorizacao = new Autorizacao(xml, configuracao);

            #region finalização do NFe através da consulta situação

            autorizacao.RetConsReciNFe = null; //Zerar pois é um novo envio de NFe

            var configSit = new Configuracao
            {
                TipoDFe = TipoDFe.NFe,
                CertificadoDigital = CertificadoSelecionado
            };


            for (var i = 0; i < autorizacao.EnviNFe.GetNFeCount; i++)
            {
                var teste = autorizacao.EnviNFe.GetNFe(i);
                var teste2 = teste.GetInfNFe(0);
                var teste3 = teste2.Chave;

            }

            foreach (var item in xml.NFe)
            {
                var xmlSit = new ConsSitNFe
                {
                    Versao = "4.00",
                    TpAmb = TipoAmbiente.Producao,
                    ChNFe = item.InfNFe[0].Chave
                };

                var consultaProtocolo = new Unimake.Business.DFe.Servicos.NFe.ConsultaProtocolo(xmlSit, configSit);
                consultaProtocolo.Executar();

                autorizacao.AddRetConsSitNFes(consultaProtocolo.Result);
                autorizacao.RetConsSitNFes.Add(consultaProtocolo.Result);
            }

            //Gravar o XML -procnfe.xml (distribuição do XML com o protocolo)
            autorizacao.GravarXmlDistribuicao(@"c:\testenfe\");

            #endregion
        }

        private EnviNFe CriarNFe()
        {
            #region CriarNFe

            var xml = new EnviNFe
            {
                Versao = "4.00",
                IdLote = "000000000000001",
                IndSinc = SimNao.Sim,
                NFe = new List<NFe> {
                        new NFe
                        {
                            InfNFe = new List<Unimake.Business.DFe.Xml.NFe.InfNFe> {
                                new Unimake.Business.DFe.Xml.NFe.InfNFe
                                {
                                    Versao = "4.00",

                                    Ide = new Unimake.Business.DFe.Xml.NFe.Ide
                                    {
                                        CUF = UFBrasil.MG,
                                        NatOp = "VENDA PRODUC.DO ESTABELEC",
                                        Mod = ModeloDFe.NFe,
                                        Serie = 2,
                                        NNF = 129,
                                        DhEmi = DateTime.Now,
                                        DhSaiEnt = DateTime.Now,
                                        TpNF = TipoOperacao.Saida,
                                        IdDest = DestinoOperacao.OperacaoInterna,
                                        CMunFG = 3151800,
                                        TpImp = FormatoImpressaoDANFE.NormalRetrato,
                                        TpEmis = TipoEmissao.Normal,
                                        TpAmb = TipoAmbiente.Homologacao,
                                        FinNFe = FinalidadeNFe.Normal,
                                        IndFinal = SimNao.Sim,
                                        IndPres = IndicadorPresenca.OperacaoPresencial,
                                        ProcEmi = ProcessoEmissao.AplicativoContribuinte,
                                        VerProc = "VisualNF-e 3.0.0.1",
                                        NFref = CriarNFref()
                                    },
                                    Emit = new Unimake.Business.DFe.Xml.NFe.Emit
                                    {
                                        CNPJ = "01618295000127",
                                        XNome = "BOLIVAR PRODUTOS PLASTICOS LTDA",
                                        XFant = "BOLIVAR PRODUTOS PLASTICOS LTDA.",
                                        EnderEmit = new Unimake.Business.DFe.Xml.NFe.EnderEmit
                                        {
                                            XLgr = "RODOVIA GERALDO MARTINS COSTA",
                                            Nro = "35",
                                            XBairro = "BORTOLAN SUL",
                                            CMun = 3151800,
                                            XMun = "POCOS DE CALDAS",
                                            UF = UFBrasil.MG,
                                            CEP = "37718000",
                                            Fone = "3537221444"
                                        },
                                        IE = "5183314120022",
                                        //IM = "14018",
                                        //CNAE = "6202300",
                                        CRT = CRT.RegimeNormal
                                    },
                                    Dest = new Unimake.Business.DFe.Xml.NFe.Dest
                                    {
                                        CNPJ = "05377777000193",
                                        XNome = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                                        EnderDest = new Unimake.Business.DFe.Xml.NFe.EnderDest
                                        {
                                            XLgr = "RUA PREFEITO CHAGAS",
                                            Nro = "605",
                                            XBairro = "CENTRO",
                                            CMun = 3151800,
                                            XMun = "POCOS DE CALDAS",
                                            UF = UFBrasil.MG,
                                            CEP = "37701010",
                                            Fone = "3530642491"
                                        },
                                        IndIEDest = IndicadorIEDestinatario.ContribuinteICMS,
                                        IE = "5182106750000",
                                        Email = "email@email.com.br"
                                    },
                                    //Det = CriarDet(),
                                    Det = new List<Det> {
                                        new Det
                                        {
                                            NItem = 1,
                                            Prod = new Prod
                                            {
                                                CProd = "0011290",
                                                CEAN = "SEM GTIN",
                                                XProd = "CABO DE REDE UTP CAT5E",
                                                NCM = "85444900",
                                                CEST = "1200700",
                                                CFOP = "5405",
                                                UCom = "MT",
                                                QCom = 305,
                                                VUnCom = 0.46M,
                                                VProd = 140.30,
                                                CEANTrib = "SEM GTIN",
                                                UTrib = "MT",
                                                QTrib = 305,
                                                VUnTrib = 0.46M,
                                                IndTot = SimNao.Sim,
                                                NItemPed = 1
                                            },
                                            Imposto = new Imposto
                                            {
                                                VTotTrib = 49.16,
                                                ICMS = new List<Unimake.Business.DFe.Xml.NFe.ICMS> {
                                                    new Unimake.Business.DFe.Xml.NFe.ICMS
                                                    {
                                                        ICMS60 = new Unimake.Business.DFe.Xml.NFe.ICMS60
                                                        {
                                                            Orig = OrigemMercadoria.Nacional,
                                                        }
                                                    }
                                                },
                                                PIS = new PIS
                                                {
                                                    PISAliq= new PISAliq
                                                    {
                                                        CST = "01",
                                                        VBC = 140.30,
                                                        PPIS = 1.65,
                                                        VPIS = 2.31
                                                    }
                                                },
                                                COFINS = new COFINS
                                                {
                                                    COFINSAliq = new COFINSAliq
                                                    {
                                                        CST = "01",
                                                        VBC = 140.30,
                                                        PCOFINS = 7.60,
                                                        VCOFINS = 10.66
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    Total = new Total
                                    {
                                        ICMSTot = new ICMSTot
                                        {
                                            VBC = 0,
                                            VICMS = 0,
                                            VICMSDeson = 0,
                                            VFCP = 0,
                                            VBCST = 0,
                                            VST = 0,
                                            VFCPST = 0,
                                            VFCPSTRet = 0,
                                            VProd = 140.30,
                                            VFrete = 0,
                                            VSeg = 0,
                                            VDesc = 0,
                                            VII = 0,
                                            VIPI = 0,
                                            VIPIDevol = 0,
                                            VPIS = 2.31,
                                            VCOFINS = 10.66,
                                            VOutro = 0,
                                            VNF = 140.30,
                                            VTotTrib = 49.16
                                        }
                                    },
                                    Transp = new Transp
                                    {
                                        ModFrete = ModalidadeFrete.ContratacaoFretePorContaDestinatário_FOB,
                                        Transporta = new Transporta
                                        {
                                            XNome = "RETIRADO PELO CLIENTE",
                                            XEnder ="RUA RIO DE JANEIRO",
                                            XMun ="POCOS DE CALDAS",
                                            UF =  UFBrasil.MG
                                        },
                                        Vol = new List<Vol>
                                        {
                                            new Vol
                                            {
                                                QVol = 2,
                                                Esp = "VOLUMES",
                                                Marca = "CAIXAS",
                                                PesoL = 0.000,
                                                PesoB = 0.000
                                            }
                                        }
                                    },
                                    Cobr = new Unimake.Business.DFe.Xml.NFe.Cobr()
                                    {
                                        Fat = new Unimake.Business.DFe.Xml.NFe.Fat
                                        {
                                            NFat = "151342",
                                            VOrig = 140.30,
                                            VDesc = 0,
                                            VLiq = 140.30
                                        },
                                        Dup = new List<Unimake.Business.DFe.Xml.NFe.Dup>
                                        {
                                            new Unimake.Business.DFe.Xml.NFe.Dup
                                            {
                                                NDup = "001",
                                                DVenc = DateTime.Now,
                                                VDup = 140.30
                                            }
                                        }
                                    },
                                    Pag = new Pag
                                    {
                                        DetPag = new List<DetPag>
                                        {
                                             new DetPag
                                             {
                                                 IndPag = IndicadorPagamento.PagamentoPrazo ,
                                                 TPag = MeioPagamento.BoletoBancario,
                                                 VPag = 140.30
                                             }
                                        }
                                    },
                                    InfAdic = new Unimake.Business.DFe.Xml.NFe.InfAdic
                                    {
                                        InfCpl = ";Trib aprox: Federal Estadual Municipal ; Trib aprox: Federal Estadual Municipal Fonte: ;",
                                    },
                                    InfRespTec = new Unimake.Business.DFe.Xml.NFe.InfRespTec
                                    {
                                        CNPJ = "05413671000106",
                                        XContato = "Oduvaldo de Oliveira",
                                        Email = "oduvaldo@visualsistemas.net",
                                        Fone = "3537215351"
                                    }
                                }
                             }
                        }
                    }
            };

            #endregion CriarNFe

            return xml;
        }

        private List<NFref> CriarNFref()
        {
            var nfref = new List<NFref>
            {

                //NFe
                new NFref { RefNFe = "999999999999999999999999999999999999" },
                new NFref { RefNFe = "999999999999999999999999999999999999" },

                //CTe
                new NFref { RefCTe = "999999999999999999999999999999999999" },
                new NFref { RefCTe = "999999999999999999999999999999999999" }
            };

            return nfref;
        }

        private void button47_Click(object sender, EventArgs e)
        {
            try
            {
                #region Criar CTeOS

                var xml = new CTeOS
                {
                    Versao = "3.00",
                    InfCTe = new Unimake.Business.DFe.Xml.CTeOS.InfCTe
                    {
                        Versao = "3.00",
                        Ide = new Unimake.Business.DFe.Xml.CTeOS.Ide
                        {
                            CUF = UFBrasil.PR,
                            CCT = "01722067",
                            CFOP = "6352",
                            NatOp = "PREST.SERV.TRANSP.INDUSTR",
                            Mod = ModeloDFe.CTeOS,
                            Serie = 1,
                            NCT = 861,
                            DhEmi = DateTime.Now,
                            TpImp = FormatoImpressaoDACTE.NormalPaisagem,
                            TpEmis = TipoEmissao.Normal,
                            TpAmb = TipoAmbiente.Homologacao,
                            TpCTe = TipoCTe.Normal,
                            ProcEmi = ProcessoEmissao.AplicativoContribuinte,
                            VerProc = "UNICO V8.0",
                            CMunEnv = "4118402",
                            XMunEnv = "PARANAVAI",
                            UFEnv = UFBrasil.PR,
                            Modal = ModalidadeTransporteCTe.Rodoviario,
                            TpServ = TipoServicoCTeOS.TransportePessoas,
                            CMunIni = "4118402",
                            XMunIni = "PARANAVAI",
                            UFIni = UFBrasil.PR,
                            CMunFim = "3305109",
                            XMunFim = "SAO JOAO DE MERITI",
                            UFFim = UFBrasil.RJ,
                            IndIEToma = IndicadorIEDestinatario.ContribuinteICMS,
                            InfPercurso = new List<Unimake.Business.DFe.Xml.CTeOS.InfPercurso>
                            {
                                new Unimake.Business.DFe.Xml.CTeOS.InfPercurso
                                {
                                    UFPer = UFBrasil.SP
                                }
                            }
                        },
                        Compl = new Unimake.Business.DFe.Xml.CTeOS.Compl
                        {
                            XObs = "Teste de observacao",
                            ObsCont = new List<Unimake.Business.DFe.Xml.CTeOS.ObsCont>
                            {
                                new Unimake.Business.DFe.Xml.CTeOS.ObsCont
                                {
                                    XCampo = "LEI DA TRANSPARENCIA",
                                    XTexto = "O valor aproximado de tributos incidentes sobre o preco deste servico e de R$ 177.33 .(0) Fonte: IBPT"
                                }
                            },
                        },
                        Emit = new Unimake.Business.DFe.Xml.CTeOS.Emit
                        {
                            CNPJ = "31905001000109",
                            IE = "9079649730",
                            XNome = "EXATUS MOVEIS EIRELI",
                            XFant = "EXATUS MOVEIS",
                            EnderEmit = new Unimake.Business.DFe.Xml.CTeOS.EnderEmit
                            {
                                XLgr = "RUA JOAQUIM F. DE SOUZA",
                                Nro = "01112",
                                XBairro = "VILA TEREZINHA",
                                CMun = 4118402,
                                XMun = "PARANAVAI",
                                CEP = "87706675",
                                UF = UFBrasil.PR,
                                Fone = "04434237530",
                            },
                        },
                        Toma = new Toma
                        {
                            CNPJ = "10197843000183",
                            IE = "9044791606",
                            XNome = "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                            XFant = "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                            Fone = "04434225480",
                            EnderToma = new Unimake.Business.DFe.Xml.CTeOS.EnderToma
                            {
                                XLgr = "RUA AMAZONAS, 1140",
                                Nro = "1140",
                                XBairro = "JD. SAO FELICIO",
                                CMun = 4118402,
                                XMun = "PARANAVAI",
                                CEP = "87702300",
                                UF = UFBrasil.PR,
                                CPais = 1058,
                                XPais = "BRASIL",
                            }
                        },
                        VPrest = new Unimake.Business.DFe.Xml.CTeOS.VPrest
                        {
                            VTPrest = 2845.15,
                            VRec = 2845.15,
                            Comp = new List<Unimake.Business.DFe.Xml.CTeOS.Comp>
                            {
                                new Unimake.Business.DFe.Xml.CTeOS.Comp
                                {
                                    XNome = "VIAGEM TURISMO",
                                    VComp = 2356.00,
                                },
                                new Unimake.Business.DFe.Xml.CTeOS.Comp
                                {
                                    XNome = "PEDAGIO",
                                    VComp = 311.82,
                                },
                            },
                        },
                        Imp = new Unimake.Business.DFe.Xml.CTeOS.Imp
                        {
                            ICMS = new Unimake.Business.DFe.Xml.CTeOS.ICMS
                            {
                                ICMS00 = new Unimake.Business.DFe.Xml.CTeOS.ICMS00
                                {
                                    CST = "00",
                                    VBC = 2533.33,
                                    PICMS = 7.00,
                                    VICMS = 177.33
                                }
                            },
                            VTotTrib = 177.33,
                            InfTribFed = new InfTribFed
                            {
                                VPIS = 30.00,
                                VCOFINS = 3.00,
                                VIR = 3.00,
                                VINSS = 3.00,
                                VCSLL = 3.00
                            }
                        },
                        InfCTeNorm = new Unimake.Business.DFe.Xml.CTeOS.InfCTeNorm
                        {
                            InfServico = new InfServico
                            {
                                XDescServ = "TRANSPORTES DE PESSOINHAS",
                                InfQ = new Unimake.Business.DFe.Xml.CTeOS.InfQ
                                {
                                    QCarga = 1
                                }
                            },
                            Seg = new List<Unimake.Business.DFe.Xml.CTeOS.Seg>
                            {
                                new Unimake.Business.DFe.Xml.CTeOS.Seg
                                {
                                    RespSeg = ResponsavelSeguroCTeOS.EmitenteCTeOS
                                }
                            },
                            InfModal = new Unimake.Business.DFe.Xml.CTeOS.InfModal
                            {
                                VersaoModal = "3.00",
                                RodoOS = new RodoOS
                                {
                                    TAF = "999999999999",
                                }
                            }
                        },
                        AutXML = new List<Unimake.Business.DFe.Xml.CTeOS.AutXML>
                        {
                            new Unimake.Business.DFe.Xml.CTeOS.AutXML
                            {
                             CNPJ = "99999999999999",
                            },
                            new Unimake.Business.DFe.Xml.CTeOS.AutXML
                            {
                             CNPJ = "99999999999998",
                            }
                        },
                        InfRespTec = new Unimake.Business.DFe.Xml.CTeOS.InfRespTec
                        {
                            CNPJ = "06117473000150",
                            XContato = "Wandrey Mundin Ferreira",
                            Email = "wandrey@unimake.com.br",
                            Fone = "04431414900",
                        },
                    },
                };

                #endregion Criar CTeOS

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTeOS,
                    CertificadoDigital = CertificadoSelecionado
                };

                var autorizacao = new Unimake.Business.DFe.Servicos.CTeOS.Autorizacao(xml, configuracao);
                autorizacao.Executar();
                MessageBox.Show(autorizacao.RetornoWSString);

                if (autorizacao.Result.ProtCTe != null)
                {

                    if (autorizacao.Result.CStat == 103) //103 = Lote Recebido com Sucesso
                    {
                        autorizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                    }
                    else
                    {
                        var xmlSit = new ConsSitCTe
                        {
                            Versao = "3.00",
                            TpAmb = TipoAmbiente.Homologacao,
                            ChCTe = xml.InfCTe.Chave
                        };

                        var configSit = new Configuracao
                        {
                            TipoDFe = TipoDFe.CTe,
                            CertificadoDigital = CertificadoSelecionado
                        };

                        var consultaProtocolo = new Unimake.Business.DFe.Servicos.CTe.ConsultaProtocolo(xmlSit, configSit);
                        consultaProtocolo.Executar();

                        autorizacao.RetConsSitCTes.Add(consultaProtocolo.Result);
                        autorizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button60_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsStatServCte
                {
                    Versao = "3.00",
                    TpAmb = TipoAmbiente.Homologacao
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTeOS,
                    CodigoUF = (int)UFBrasil.AP,
                    TipoEmissao = TipoEmissao.Normal,
                    CertificadoDigital = CertificadoSelecionado
                };

                var statusServico = new Unimake.Business.DFe.Servicos.CTeOS.StatusServico(xml, configuracao);
                statusServico.Executar();
                MessageBox.Show(statusServico.RetornoWSString);
                MessageBox.Show(statusServico.Result.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button59_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsSitCTe
                {
                    Versao = "3.00",
                    TpAmb = TipoAmbiente.Homologacao,
                    ChCTe = "50200210859283000185570010000005621912070311"
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTeOS,
                    CertificadoDigital = CertificadoSelecionado
                };

                var consultaProtocolo = new Unimake.Business.DFe.Servicos.CTeOS.ConsultaProtocolo(xml, configuracao);
                consultaProtocolo.Executar();

                MessageBox.Show(consultaProtocolo.RetornoWSString);
                MessageBox.Show(consultaProtocolo.Result.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button58_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new InutCTe
                {
                    Versao = "3.00",
                    InfInut = new InutCTeInfInut
                    {
                        Ano = "19",
                        CNPJ = "06117473000150",
                        CUF = UFBrasil.PR,
                        Mod = ModeloDFe.CTeOS,
                        NCTIni = 57919,
                        NCTFin = 57919,
                        Serie = 1,
                        TpAmb = TipoAmbiente.Homologacao,
                        XJust = "Justificativa da inutilizacao de teste"
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTeOS,
                    CertificadoDigital = CertificadoSelecionado
                };

                var inutilizacao = new Unimake.Business.DFe.Servicos.CTeOS.Inutilizacao(xml, configuracao);
                inutilizacao.Executar();
                MessageBox.Show(inutilizacao.RetornoWSString);
                MessageBox.Show(inutilizacao.Result.InfInut.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (inutilizacao.Result.InfInut.CStat)
                {
                    case 102: //Inutilização homologada
                        inutilizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                        break;

                    default: //Inutilização rejeitada
                        inutilizacao.GravarXmlDistribuicao(@"c:\testenfe\");
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button57_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new Unimake.Business.DFe.Xml.CTe.ConsCad
                {
                    Versao = "2.00",
                    InfCons = new InfCons()
                    {
                        CNPJ = "06117473000150",
                        UF = UFBrasil.PR
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTeOS,
                    CertificadoDigital = CertificadoSelecionado
                };

                var consultaCad = new Unimake.Business.DFe.Servicos.CTeOS.ConsultaCadastro(xml, configuracao);
                consultaCad.Executar();
                MessageBox.Show(consultaCad.RetornoWSString);
                MessageBox.Show(consultaCad.Result.InfCons.XMotivo);
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button54_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EventoCTe
                {
                    Versao = "3.00",
                    InfEvento = new Unimake.Business.DFe.Xml.CTe.InfEvento(new Unimake.Business.DFe.Xml.CTe.DetEventoCanc
                    {
                        NProt = "141200000007987",
                        VersaoEvento = "3.00",
                        XJust = "Justificativa para cancelamento da CTe de teste"
                    })
                    {
                        COrgao = UFBrasil.PR,
                        ChCTe = "41200210859283000185570010000005671227070615",
                        CNPJ = "10859283000185",
                        DhEvento = DateTime.Now,
                        TpEvento = TipoEventoCTe.Cancelamento,
                        NSeqEvento = 1,
                        TpAmb = TipoAmbiente.Homologacao
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTeOS,
                    CertificadoDigital = CertificadoSelecionado
                };


                var recepcaoEvento = new Unimake.Business.DFe.Servicos.CTeOS.RecepcaoEvento(xml, configuracao);

                //xml.LerXML<EventoCTe>(recepcaoEvento.ConteudoXMLAssinado);

                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.InfEvento.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (recepcaoEvento.Result.InfEvento.CStat)
                {
                    case 134: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respectivo CT-e com situação diferente de Autorizada.
                    case 135: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respetivo CTe.
                    case 136: //Recebido pelo Sistema de Registro de Eventos – vinculação do evento ao respectivo CT-e prejudicado.
                        recepcaoEvento.GravarXmlDistribuicao(@"c:\testecte\");
                        break;

                    default:
                        //Quando o evento é rejeitado pela Sefaz.
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button50_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EventoCTe
                {
                    Versao = "3.00",
                    InfEvento = new Unimake.Business.DFe.Xml.CTe.InfEvento(new Unimake.Business.DFe.Xml.CTe.DetEventoCCE
                    {
                        VersaoEvento = "3.00",
                        EventoCCeCTe = new EventoCCeCTe
                        {
                            InfCorrecao = new List<InfCorrecao>
                            {
                                new InfCorrecao
                                {
                                    GrupoAlterado = "ide",
                                    CampoAlterado = "cfop",
                                    ValorAlterado = "6353",
                                    NroItemAlterado = ""
                                }
                            }
                        }
                    })
                    {
                        COrgao = UFBrasil.PR,
                        ChCTe = "41200210859283000185570010000005671227070615",
                        CNPJ = "10859283000185",
                        DhEvento = DateTime.Now,
                        TpEvento = TipoEventoCTe.CartaCorrecao,
                        NSeqEvento = 1,
                        TpAmb = TipoAmbiente.Homologacao
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.CTeOS,
                    CertificadoDigital = CertificadoSelecionado
                };


                var recepcaoEvento = new Unimake.Business.DFe.Servicos.CTeOS.RecepcaoEvento(xml, configuracao);

                //xml.LerXML<EventoCTe>(recepcaoEvento.ConteudoXMLAssinado);

                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.InfEvento.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (recepcaoEvento.Result.InfEvento.CStat)
                {
                    case 134: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respectivo CT-e com situação diferente de Autorizada.
                    case 135: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respetivo CTe.
                    case 136: //Recebido pelo Sistema de Registro de Eventos – vinculação do evento ao respectivo CT-e prejudicado.
                        recepcaoEvento.GravarXmlDistribuicao(@"c:\testecte\");
                        break;

                    default:
                        //Quando o evento é rejeitado pela Sefaz.
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button44_Click(object sender, EventArgs e)
        {
            try
            {

                var doc = new XmlDocument();
                doc.Load(@"C:\Users\Wandrey\Downloads\51220513062376000172550010002136181624290564-nfe.xml");

                #region Evento Pagamento Operacao MDFe

                //var xmlEvento = XMLUtility.Deserializar<Unimake.Business.DFe.Xml.MDFe.EventoMDFe>(doc);

                //var config = new Configuracao
                //{
                //    TipoDFe = TipoDFe.MDFe,
                //    CertificadoDigital = CertificadoSelecionado,
                //};

                //var autorizacao = new Unimake.Business.DFe.Servicos.MDFe.RecepcaoEvento(xmlEvento, config);
                //autorizacao.Executar();

                #endregion

                #region Evento EPEC NFe

                //var xmlEvento = XMLUtility.Deserializar<Unimake.Business.DFe.Xml.NFe.EnvEvento>(doc);

                //var config = new Configuracao
                //{
                //    TipoDFe = TipoDFe.NFe,
                //    CertificadoDigital = CertificadoSelecionado,
                //};

                //var autorizacao = new Unimake.Business.DFe.Servicos.NFe.RecepcaoEvento(xmlEvento, config);
                //autorizacao.Executar();

                #endregion

                #region Consulta Distribuição DFe



                #endregion

                #region NFe / NFCe

                var xmlNFe = new EnviNFe
                {
                    IdLote = "000000000000001",
                    Versao = "4.00",
                    IndSinc = SimNao.Sim,
                    NFe = new List<NFe>()
                };
                xmlNFe.NFe.Add(XMLUtility.Deserializar<NFe>(doc));

                var config = new Configuracao
                {
                    TipoDFe = TipoDFe.NFCe,
                    CertificadoDigital = CertificadoSelecionado,
                    CSC = "HCJBIRTWGCQ3HVQN7DCA0ZY0P2NYT6FVLPJG",
                    CSCIDToken = 2
                };

                var autorizacao = new Unimake.Business.DFe.Servicos.NFCe.Autorizacao(xmlNFe, config);
                ////autorizacao.SetXMLConfiguracao(xmlNFe, config);
                ////var notaAssinada = autorizacao.GetConteudoNFeAssinada(0);
                autorizacao.Executar();
                ////var autorizacao = new Unimake.Business.DFe.Servicos.NFCe.Autorizacao(xmlNFe, config);
                ////autorizacao.Executar();

                #endregion

                #region CTe

                //var xmlCTe = new EnviCTe();
                //xmlCTe.IdLote = "000000000000001";
                //xmlCTe.Versao = "3.00";
                //xmlCTe.CTe = new List<CTe>();
                //xmlCTe.CTe.Add(XMLUtility.Deserializar<CTe>(doc));

                //var config = new Configuracao
                //{
                //    TipoDFe = TipoDFe.CTe,
                //    CertificadoDigital = CertificadoSelecionado
                //};

                //Unimake.Business.DFe.Servicos.CTe.Autorizacao autorizacao = new Unimake.Business.DFe.Servicos.CTe.Autorizacao(xmlCTe, config);
                //autorizacao.Executar();

                #endregion

                #region CTeOS

                //var xmlCTeOS = XMLUtility.Deserializar<CTeOS>(doc);

                //var config = new Configuracao
                //{
                //    TipoDFe = TipoDFe.CTeOS,
                //    CertificadoDigital = CertificadoSelecionado
                //};

                //Unimake.Business.DFe.Servicos.CTeOS.Autorizacao autorizacao = new Unimake.Business.DFe.Servicos.CTeOS.Autorizacao(xmlCTeOS, config);
                //autorizacao.Executar();

                #endregion

                #region MDFe

                //var xmlMDFe = new EnviMDFe
                //{
                //    IdLote = "000000000000001",
                //    Versao = "3.00",
                //    MDFe = new MDFe()
                //};
                //xmlMDFe.MDFe = XMLUtility.Deserializar<MDFe>(doc);

                //var config = new Configuracao
                //{
                //    TipoDFe = TipoDFe.MDFe,
                //    CertificadoDigital = CertificadoSelecionado
                //};

                //var autorizacao = new Unimake.Business.DFe.Servicos.MDFe.Autorizacao(xmlMDFe, config);
                //autorizacao.Executar();

                #endregion

                #region MDFe Síncrono

                //var xmlMDFe = XMLUtility.Deserializar<MDFe>(doc);
                //var config = new Configuracao
                //{
                //    TipoDFe = TipoDFe.MDFe,
                //    CertificadoDigital = CertificadoSelecionado
                //};

                //Unimake.Business.DFe.Servicos.MDFe.AutorizacaoSinc autorizacao = new Unimake.Business.DFe.Servicos.MDFe.AutorizacaoSinc(xmlMDFe, config);
                //autorizacao.Executar();

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetLastException().Message);
            }
        }

        private bool CertSetPin1 = false;
        private bool CertSetPin2 = false;

        private void TesteCertificadoA3(X509Certificate2 certificado)
        {
            try
            {
                if (certificado.IsA3())
                {
                    if (certificado.SerialNumber == "58219FD2EADA297B")
                    {
                        if (!CertSetPin1)
                        {
                            certificado.SetPinPrivateKey("@123456x");
                            CertSetPin1 = true;
                        }
                    }
                    else
                    {
                        if (!CertSetPin2)
                        {
                            certificado.SetPinPrivateKey("1234");
                            CertSetPin2 = true;
                        }
                    }
                }

                var xml = new ConsStatServ
                {
                    Versao = "4.00",
                    CUF = UFBrasil.SE,
                    TpAmb = TipoAmbiente.Homologacao
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.NFe,
                    TipoEmissao = TipoEmissao.Normal,
                    CertificadoDigital = certificado,
                    HasProxy = false,
                    ProxyAutoDetect = false,
                    ProxyUser = "",
                    ProxyPassword = ""
                };

                var statusServico = new StatusServico(xml, configuracao);
                statusServico.Executar();

                var qq = statusServico.RetornoWSString;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //TODO:  Nunca usado, pode apagar?
        //private EnviNFe CriarNotaX()
        //{
        //    var xml = new EnviNFe
        //    {
        //        Versao = "4.00",
        //        IdLote = "000000000000001",
        //        IndSinc = SimNao.Nao,
        //        NFe = new List<Unimake.Business.DFe.Xml.NFe.NFe> {
        //                new Unimake.Business.DFe.Xml.NFe.NFe
        //                {
        //                    InfNFe = new List<Unimake.Business.DFe.Xml.NFe.InfNFe> {
        //                        new Unimake.Business.DFe.Xml.NFe.InfNFe
        //                        {
        //                            Versao = "4.00",

        //                            Ide = new Unimake.Business.DFe.Xml.NFe.Ide
        //                            {
        //                                CUF = UFBrasil.MT,
        //                                CNF = "53355458",
        //                                NatOp = "Venda de Mercadoria",
        //                                Mod = ModeloDFe.NFe,
        //                                Serie = 1,
        //                                NNF = 36,
        //                                DhEmi = Convert.ToDateTime("2020-07-04T10:33:56-04:00"), // DateTime.Now,
        //                                DhSaiEnt = Convert.ToDateTime("2020-07-04T10:33:56-04:00"), //DateTime.Now,
        //                                TpNF = TipoOperacao.Saida,
        //                                IdDest = DestinoOperacao.OperacaoInterna,
        //                                CMunFG = 5107909,
        //                                TpImp = FormatoImpressaoDANFE.NormalRetrato,
        //                                TpEmis = TipoEmissao.Normal,
        //                                TpAmb = TipoAmbiente.Producao,
        //                                FinNFe = FinalidadeNFe.Normal,
        //                                IndFinal = SimNao.Sim,
        //                                IndPres = IndicadorPresenca.OperacaoPresencial,
        //                                ProcEmi = ProcessoEmissao.AplicativoContribuinte,
        //                                VerProc = "1.00"
        //                            },

        //                            #region B - Identificação da Nota Fiscal eletrônica
        //                            Emit = gerarEmit(),
        //                            #endregion

        //                            Dest = new Unimake.Business.DFe.Xml.NFe.Dest
        //                            {
        //                                CPF = "04747008101",
        //                                XNome = "ALANA NERVES ARANA",
        //                                EnderDest = new Unimake.Business.DFe.Xml.NFe.EnderDest
        //                                {
        //                                    XLgr = "RUA DOS CEDROS",
        //                                    Nro = "00",
        //                                    XBairro = "CENTRO",
        //                                    CMun = 5107909,
        //                                    XMun = "SINOP",
        //                                    UF = UFBrasil.MT,
        //                                    CEP = "78550000",
        //                                    CPais = 1058,
        //                                    XPais = "BRASIL",
        //                                    Fone = "66996888727"
        //                                },
        //                                IndIEDest = IndicadorIEDestinatario.NaoContribuinte
        //                                //IE = "ISENTO"
        //                                //Email = "janelaorp@janelaorp.com.br"
        //                            },
        //                            Det = new List<Det> {
        //                                new Det
        //                                {
        //                                    NItem = 1,
        //                                    Prod = new Prod
        //                                    {
        //                                        CProd = "786",
        //                                        CEAN = "7909446119051",
        //                                        XProd = "BULGET BG3231 09A -",
        //                                        NCM = "90041000",
        //                                        CFOP = "5102",
        //                                        UCom = "PC",
        //                                        QCom = 1.00,
        //                                        VUnCom = 300.0000000000,
        //                                        VProd = 300.00,
        //                                        CEANTrib = "7909446119051",
        //                                        UTrib = "PC",
        //                                        QTrib = 1.00,
        //                                        VUnTrib = 300.0000000000,
        //                                        IndTot = SimNao.Sim
        //                                        //XPed = "300474",
        //                                        //NItemPed = 1
        //                                    },
        //                                    Imposto = new Imposto
        //                                    {
        //                                        VTotTrib = 116.31,
        //                                        ICMS = new List<Unimake.Business.DFe.Xml.NFe.ICMS> {
        //                                            new Unimake.Business.DFe.Xml.NFe.ICMS
        //                                            {
        //                                                ICMSSN102 = new ICMSSN102
        //                                                {
        //                                                    Orig = OrigemMercadoria.Nacional,
        //                                                    CSOSN = "102"
        //                                                }
        //                                            }
        //                                        },
        //                                        IPI = new IPI
        //                                        {
        //                                            CEnq = "999",
        //                                            IPITrib = new IPITrib
        //                                            {
        //                                                CST = "99",
        //                                                VBC = 0.00,
        //                                                PIPI = 0.00,
        //                                                VIPI = 0.00
        //                                            }
        //                                        },
        //                                        PIS = new PIS
        //                                        {
        //                                            PISOutr = new PISOutr
        //                                            {
        //                                                CST = "49",
        //                                                VBC = 300.00,
        //                                                PPIS = 0.0000,
        //                                                VPIS = 0.00
        //                                            }
        //                                        },
        //                                        COFINS = new COFINS
        //                                        {
        //                                            COFINSOutr = new COFINSOutr
        //                                            {
        //                                                CST = "49",
        //                                                VBC = 300.00,
        //                                                PCOFINS = 0.0000,
        //                                                VCOFINS = 0.00
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            },
        //                            Total = new Total
        //{
        //                                ICMSTot = new ICMSTot
        //    {
        //                                    VBC = 0,
        //                                    VICMS = 0,
        //                                    VICMSDeson = 0,
        //                                    VFCP = 0,
        //                                    VBCST = 0,
        //                                    VST = 0,
        //                                    VFCPST = 0,
        //                                    VFCPSTRet = 0,
        //                                    VProd = 300.00,
        //                                    VFrete = 0,
        //                                    VSeg = 0,
        //        VDesc = 0,
        //                                    VII = 0,
        //                                    VIPI = 0,
        //                                    VIPIDevol = 0,
        //                                    VPIS = 0,
        //                                    VCOFINS = 0,
        //                                    VOutro = 0,
        //                                    VNF = 300.00,
        //                                    VTotTrib = 116.31
        //                                }
        //                            },
        //                            Transp = new Transp
        //                            {
        //                                ModFrete = ModalidadeFrete.SemOcorrenciaTransporte,
        //                                Vol = new List<Vol>
        //                                {
        //                                    new Vol
        //                                    {
        //                                        QVol = 1,
        //                                        NVol = "1"
        //                                        //Esp = "LU",
        //                                        //Marca = "UNIMAKE",
        //                                        //PesoL = 0.000,
        //                                        //PesoB = 0.000
        //                                    }
        //                                }
        //    },
        //                            //Cobr = new Cobr()
        //                            //{
        //                            //    Fat = new Fat
        //                            //    {
        //                            //        NFat = "057910",
        //                            //        VOrig = 84.90,
        //                            //        VDesc = 0,
        //                            //        VLiq = 84.90
        //                            //    },
        //                            //    Dup = new List<Dup>
        //                            //    {
        //                            //        new Dup
        //                            //        {
        //                            //            NDup = "001",
        //                            //            DVenc = DateTime.Now,
        //                            //            VDup = 84.90
        //                            //        }
        //                            //    }
        //                            //},
        //                            Pag = new Pag
        //    {
        //                                DetPag = new List<DetPag>
        //                                {
        //                                     new DetPag
        //        {
        //                                         IndPag = IndicadorPagamento.PagamentoVista,
        //                                         TPag = MeioPagamento.Dinheiro,
        //                                         VPag = 300.00
        //        }
        //    }
        //},
        //                            InfAdic = new Unimake.Business.DFe.Xml.NFe.InfAdic
        //                            {
        //                                InfCpl = "Valor Aprox de Tributos R$ 116.31 (38.77%) Fonte IBPT PROCON/MT AV. HISTORIADOR RUBENS DE MENDONCA N. 917 - B. ARAES, EDIFICIO EXECUTIVE CENTER CEP:78008-000 CUIABA-MT FONE:(66)3531-1512;EMPRESA OPTANTE DO SIMPLES NACIONAL NAO GERA CREDITO ICMS",
        //                            }
        //                            //InfRespTec = new InfRespTec
        //                            //{
        //                            //    CNPJ = "06117473000150",
        //                            //    XContato = "Wandrey Mundin Ferreira",
        //                            //    Email = "wandrey@unimake.com.br",
        //                            //    Fone = "04431414900"
        //                            //}
        //                        }
        //                    }
        //                }
        //            }
        //    };

        //    return xml;
        //}

        //TODO:  Nunca usado, pode apagar?
        //private Unimake.Business.DFe.Xml.NFe.Emit gerarEmit()
        //{
        //    var emit = new Unimake.Business.DFe.Xml.NFe.Emit
        //    {
        //        CNPJ = "35795490000144",
        //        XNome = "OTICAS BELO LTDA",
        //        XFant = "OTICAS BELO",
        //        EnderEmit = new Unimake.Business.DFe.Xml.NFe.EnderEmit
        //{
        //            XLgr = "AV. GOVERNADOR JULIO CAMPOS",
        //            Nro = "417",
        //            XBairro = "SETOR COMERCIAL",
        //            CMun = 5107909,
        //            XMun = "SINOP",
        //            UF = UFBrasil.MT,
        //            CEP = "78550242",
        //            CPais = 1058,
        //            XPais = "Brasil",
        //            Fone = "6635310180"
        //        },
        //        IE = "137960557",
        //        //IM = "",
        //        //CNAE = "6202300",
        //        CRT = CRT.SimplesNacional
        //    };

        //    return emit;
        //}

        private void button48_Click(object sender, EventArgs e)
        {
            var validarSchema = new ValidarSchema();

            var doc = new XmlDocument();
            doc.Load(@"D:\testenfe\41220306117473000150550010000000001111111111-procNFe.xml");

            var schema = "procNFe_v4.00.xsd";
            validarSchema.Validar(doc, schema, "http://www.portalfiscal.inf.br/nfe");

            if (!validarSchema.Success)
            {
                MessageBox.Show("Code: " + validarSchema.ErrorCode + "\r\n\r\nMessage: " + validarSchema.ErrorMessage);
            }
            else
            {
                MessageBox.Show("XML validado com sucesso.");
            }
        }

        private void button49_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EnvEvento
                {
                    Versao = "1.00",
                    IdLote = "000000000000001",
                    Evento = new List<Evento> {
                        new Evento
                        {
                            Versao = "1.00",
                            InfEvento = new Unimake.Business.DFe.Xml.NFe.InfEvento(new Unimake.Business.DFe.Xml.NFe.DetEventoEPEC
                            {
                                COrgaoAutor = UFBrasil.PR,
                                TpAutor = TipoAutor.EmpresaEmitente,
                                VerAplic = "1.00",
                                TpNF = TipoOperacao.Saida,
                                DhEmi = DateTime.Now,
                                IE = "9032000301",
                                Versao = "1.00",
                                Dest = new DetEventoEPECDest
                                {
                                    CNPJ = "06117473000150",
                                    UF = UFBrasil.PR,
                                    VNF = 86.00,
                                    VICMS = 6.02,
                                    VST = 0.00
                                }

                            })
                            {
                                COrgao = UFBrasil.AN,
                                ChNFe = "41190806117473000150550010000579131943463890",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.EPEC,
                                NSeqEvento = 1,
                                VerEvento = "1.00",
                                TpAmb = TipoAmbiente.Homologacao
                            }
                        }
                    }
                };

                var configuracao = new Configuracao
                {
                    CertificadoDigital = CertificadoSelecionado
                };

                var recepcaoEvento = new RecepcaoEvento(xml, configuracao);
                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                if (recepcaoEvento.Result.CStat == 128) //128 = Lote de evento processado com sucesso
                {
                    switch (recepcaoEvento.Result.RetEvento[0].InfEvento.CStat)
                    {
                        case 135: //Evento homologado com vinculação da respectiva NFe
                        case 136: //Evento homologado sem vinculação com a respectiva NFe (SEFAZ não encontrou a NFe na base dela)
                        case 155: //Evento de Cancelamento homologado fora do prazo permitido para cancelamento
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;

                        default: //Evento rejeitado
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button51_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EventoCTe
                {
                    Versao = "3.00",
                    InfEvento = new Unimake.Business.DFe.Xml.CTe.InfEvento(new Unimake.Business.DFe.Xml.CTe.DetEventoEPEC
                    {
                        VersaoEvento = "3.00",
                        XJust = "Teste de EPEC do CTE para ver se tudo está funcionando",
                        VICMS = 100,
                        VICMSST = 100,
                        VTPrest = 1000,
                        VCarga = 1000,
                        Toma4 = new EvEPECCTeToma4
                        {
                            UF = UFBrasil.PR,
                            CNPJ = "06117473000150",
                            IE = "1234567890"
                        },
                        Modal = ModalidadeTransporteCTe.Rodoviario,
                        UFIni = UFBrasil.PR,
                        UFFim = UFBrasil.PR,
                        TpCTe = TipoCTe.Normal,
                        DhEmi = DateTime.Now
                    })
                    {
                        COrgao = UFBrasil.RS,
                        ChCTe = "41200210859283000185570010000005671227070615",
                        CNPJ = "10859283000185",
                        DhEvento = DateTime.Now,
                        TpEvento = TipoEventoCTe.EPEC,
                        NSeqEvento = 1,
                        TpAmb = TipoAmbiente.Homologacao
                    }
                };

                var configuracao = new Configuracao
                {
                    CertificadoDigital = CertificadoSelecionado,
                    TipoDFe = TipoDFe.CTe
                };

                var recepcaoEvento = new Unimake.Business.DFe.Servicos.CTe.RecepcaoEvento(xml, configuracao);
                recepcaoEvento.Executar();

                MessageBox.Show(recepcaoEvento.RetornoWSString);

                switch (recepcaoEvento.Result.InfEvento.CStat)
                {
                    case 134: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respectivo CT-e com situação diferente de Autorizada.
                    case 135: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respetivo CTe.
                    case 136: //Recebido pelo Sistema de Registro de Eventos – vinculação do evento ao respectivo CT-e prejudicado.
                        recepcaoEvento.GravarXmlDistribuicao(@"c:\testecte\");
                        break;

                    default: //Evento rejeitado
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        /// <summary>
        /// ConvertToOEM
        /// </summary>
        /// <param name="FBuffer"></param>
        /// <returns></returns>
        public string ConvertToOEM(string FBuffer)
        {
            if (string.IsNullOrEmpty(FBuffer))
            {
                return "";
            }

            if (FBuffer.StartsWith("<![CDATA["))
            {
                return FBuffer;
            }

            FBuffer = FBuffer.Replace("&", "&amp;");
            FBuffer = FBuffer.Replace("<", "&lt;");
            FBuffer = FBuffer.Replace(">", "&gt;");
            FBuffer = FBuffer.Replace("\"", "&quot;");
            FBuffer = FBuffer.Replace("\t", " ");
            FBuffer = FBuffer.Replace("\n", ";");
            FBuffer = FBuffer.Replace("\r", "");
            FBuffer = FBuffer.Replace("'", "&#39;");
            FBuffer = FBuffer.Replace("&amp; lt;", "&lt;");
            FBuffer = FBuffer.Replace("&amp; gt;", "&gt;");
            FBuffer = FBuffer.Replace("&amp; amp;", "&amp;");
            FBuffer = FBuffer.Replace("&amp;amp;", "&amp;");
            FBuffer = FBuffer.Replace("&amp;lt;", "&lt;");
            FBuffer = FBuffer.Replace("&amp;gt;", "&gt;");

            string normalizedString = FBuffer.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    switch (Convert.ToInt16(normalizedString[i]))
                    {
                        case 94:    ///^
                        case 96:    ///`
                        case 126:   ///~
                        case 8216:  ///‘
                        case 8217:  ///’
                        case 161:   ///¡
                        case 162:   ///¢
                        case 163:   ///£
                        case 164:   ///¤
                        case 165:   ///¥
                        case 166:   ///¦
                        case 167:   ///§
                        case 168:   ///¨
                        case 171:   ///«
                        case 172:   ///¬
                        case 169:   ///©
                        case 174:   ///®
                        case 175:   ///¯
                        case 177:   ///±
                        case 180:   ///´
                        case 181:   ///µ
                        case 182:   ///¶
                        case 183:   ///·
                        case 187:   ///»
                        case 188:   ///¼
                        case 189:   ///½
                        case 190:   ///¾
                        case 191:   ///¿
                        case 198:   ///Æ
                        case 208:   ///Ð
                        case 216:   ///Ø
                        case 222:   ///Þ
                        case 223:   ///ß
                        case 230:   ///æ
                        case 240:   ///ð
                        case 247:   ///÷
                        case 248:   ///ø
                        case 254:   ///þ
                        case 184: c = ' '; break; ///¸
                        case 170: c = 'a'; break; ///ª
                        case 176: c = 'o'; break; ///°
                        case 178: c = '2'; break; ///²
                        case 179: c = '3'; break; ///³
                        case 185: c = '1'; break; ///¹
                        case 186: c = 'o'; break; ///º
                        case 732: c = 'Ø'; break; ///˜
                    }
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }

        private void button52_Click(object sender, EventArgs e)
        {
            string valor = "4999.200000000";

            //Converter o valor de string para Decimal - Resultado: 4999.200000000
            var valorDec = Convert.ToDecimal(valor, CultureInfo.InvariantCulture);

            //Converter o valor de string para double (ponto flutuante) - Resultado: 4999.2
            var valorDouble = Convert.ToDouble(valor, CultureInfo.InvariantCulture);
            
            //Converter o valor de Decimal para Double (ponto flutuante) - Resultado: 4999.2
            var valorDouble2 = (double)valorDec;

            //Converter o valor Double para Float - Resultado: 4999.2
            var valorFloat2 = (float)valorDouble2;

            //Agora você pode converter qualquer um deles (double, decimal ou float para string com somente 2 casas, caso queira uma string)
            var valorStringTwoDec = valorDouble2.ToString("F2", CultureInfo.InvariantCulture); //Resultado: 4999.20

            return;

            //Você pode fazer assim
            var nfeProc = (new NfeProc()).LoadFromFile(@"C:\Users\Wandrey\Downloads\Telegram Desktop\35220401157555001852550010001972511906066785-procNFe.xml");

            MessageBox.Show("Chave da NFe: " + nfeProc.NFe.InfNFe[0].Chave);
            MessageBox.Show("Numero da NFe: " + nfeProc.NFe.InfNFe[0].Ide.NNF);
            MessageBox.Show("CNPJ Emitente: " + nfeProc.NFe.InfNFe[0].Emit.CNPJ);
            MessageBox.Show("Protocolo de autorização: " + nfeProc.ProtNFe.InfProt.NProt);

            //Ou vc pode fazer assim
            XmlDocument docNfeProc = new XmlDocument();
            docNfeProc.Load(@"C:\Users\Wandrey\Downloads\Telegram Desktop\35220401157555001852550010001972511906066785-procNFe.xml");                
            Unimake.Business.DFe.Xml.NFe.NfeProc nfeProc2 = Unimake.Business.DFe.Utility.XMLUtility.Deserializar<Unimake.Business.DFe.Xml.NFe.NfeProc>(docNfeProc);

            MessageBox.Show("Chave da NFe: " + nfeProc2.NFe.InfNFe[0].Chave);
            MessageBox.Show("Numero da NFe: " + nfeProc2.NFe.InfNFe[0].Ide.NNF);
            MessageBox.Show("CNPJ Emitente: " + nfeProc2.NFe.InfNFe[0].Emit.CNPJ);
            MessageBox.Show("Protocolo de autorização: " + nfeProc2.ProtNFe.InfProt.NProt);


            string qq = ConvertToOEM("MARKETING ÇAÇA");
            
            var certDig = new CertificadoDigital();

            try
            {
                var certSelecionado = certDig.AbrirTelaSelecao();

                if (certSelecionado == null)
                {
                    MessageBox.Show("Nenhum certificado digital foi selecionado.");
                }
                else
                {
                    MessageBox.Show("Certificado digital selecionado: " + certSelecionado.Subject);
                }
            }
            catch (CertificadoDigitalException ex)
            {
                MessageBox.Show("Ocorreu falha na seleção do certificado digital.\r\n\r\nErro: " + ex.Message);
            }

            #region Ler retorno SAT

            var xmlRetornoSAT = new XmlDocument();
            xmlRetornoSAT.Load(@"C:\Users\Wandrey\Downloads\Telegram Desktop\1155-sat-ret.xml"); //Informar no parâmetro o arquivo que vai estar na pasta de retorno do UNINFE

            var listEnviarDados = xmlRetornoSAT.GetElementsByTagName("EnviarDadosVendaResponse");

            if (listEnviarDados.Count > 0) //Tem retorno do SAT
            {
                var elementXml = (XmlElement)listEnviarDados[0];

                //Verificar se tem a tag de CodigoMensagem
                var codigoMensagem = string.Empty;
                if (elementXml.GetElementsByTagName("CodigoMensagem").Count > 0)
                {
                    codigoMensagem = elementXml.GetElementsByTagName("CodigoMensagem")[0].InnerText;
                }

                //Verificar se tem a tag com a Mensagem de retorno
                var mensagem = string.Empty;
                if (elementXml.GetElementsByTagName("Mensagem").Count > 0)
                {
                    codigoMensagem = elementXml.GetElementsByTagName("Mensagem")[0].InnerText;
                }

                //Verificar se tem a tag com a chave do CFe autorizado, se tiver pegar este conteúdo e gravar na base de dados
                //para usar futuramente em uma consulta ou cancelamento do CFe.
                var chaveConsulta = string.Empty;
                if (elementXml.GetElementsByTagName("ChaveConsulta").Count > 0)
                {
                    codigoMensagem = elementXml.GetElementsByTagName("ChaveConsulta")[0].InnerText;
                }

                if (codigoMensagem == "06000") //CFe emitido com sucesso
                {
                    //Disparar impressão e fazer tratamentos desejados
                }
                else //CFe foi rejeitado, demonstrar mensagem para o usuário avisando
                {
                    MessageBox.Show(mensagem);
                }
            }

            #endregion

            #region Ler retorno de envio de NFSe
            //var doc = new XmlDocument();
            //doc.Load(@"C:\Users\Wandrey\Downloads\Telegram Desktop\35904220000124_0000001-env-loterps2 (2).xml");

            //var nodeListListaMensagemRetorno = doc.GetElementsByTagName("ListaMensagemRetorno");

            //foreach (var nodeListaMensagemRetorno in nodeListListaMensagemRetorno)
            //{
            //    var nodeListMensagemRetorno = ((XmlElement)nodeListaMensagemRetorno).GetElementsByTagName("MensagemRetorno");

            //    foreach (var nodeMensagemRetorno in nodeListMensagemRetorno)
            //    {
            //        string codigo = "";
            //        string mensagem = "";
            //        string correcao = "";

            //        var elementMensagemRetorno = (XmlElement)nodeMensagemRetorno;

            //        if (elementMensagemRetorno.GetElementsByTagName("Codigo") != null)
            //        {
            //            codigo = elementMensagemRetorno.GetElementsByTagName("Codigo")[0].InnerText;
            //        }
            //        if (elementMensagemRetorno.GetElementsByTagName("Mensagem") != null)
            //        {
            //            mensagem = elementMensagemRetorno.GetElementsByTagName("Mensagem")[0].InnerText;
            //        }
            //        if (elementMensagemRetorno.GetElementsByTagName("Correcao") != null)
            //        {
            //            correcao = elementMensagemRetorno.GetElementsByTagName("Correcao")[0].InnerText;
            //        }

            //        MessageBox.Show("Código: " + codigo + "\r\n" +
            //            "Mensagem: " + mensagem + "\r\n" +
            //            "Correção: " + correcao);
            //    }
            //}
            #endregion
        }

        private void button53_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EnvEvento
                {
                    Versao = "1.00",
                    IdLote = "000000000000001",
                    Evento = new List<Evento> {
                        new Evento
                        {
                            Versao = "1.00",
                            InfEvento = new Unimake.Business.DFe.Xml.NFe.InfEvento(new Unimake.Business.DFe.Xml.NFe.DetEventoCompEntregaNFe
                            {
                                Versao = "1.00",
                                COrgaoAutor = UFBrasil.PR,
                                TpAutor = TipoAutor.EmpresaEmitente,
                                VerAplic = "uninfe teste 1.0",
                                DhEntrega = DateTime.Now,
                                NDoc = "00000000000",
                                XNome = "Jose silva joao",
                                LatGPS = "53.339688",
                                LongGPS = "-6.236688",
                                HashComprovante = "noauBnfaoS02PYxVm8ufox7OKww=",
                                DhHashComprovante = DateTime.Now
                            })
                            {
                                COrgao = UFBrasil.AN,
                                ChNFe = "41190806117473000150550010000579131943463890",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.ComprovanteEntregaNFe,
                                NSeqEvento = 1,
                                VerEvento = "1.00",
                                TpAmb = TipoAmbiente.Homologacao
                            }
                        }
                    }
                };

                var configuracao = new Configuracao
                {
                    CertificadoDigital = CertificadoSelecionado
                };

                var recepcaoEvento = new RecepcaoEvento(xml, configuracao);
                recepcaoEvento.Executar();

                var xmlDistrib = new XmlDocument();
                xmlDistrib = recepcaoEvento.ProcEventoNFeResult[0].GerarXML();

                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.XMotivo);

                var qq = recepcaoEvento.ProcEventoNFeResult[0].GerarXML().OuterXml;

                //Gravar o XML de distribuição se a inutilização foi homologada
                if (recepcaoEvento.Result.CStat == 128) //128 = Lote de evento processado com sucesso
                {
                    switch (recepcaoEvento.Result.RetEvento[0].InfEvento.CStat)
                    {
                        case 135: //Evento homologado com vinculação da respectiva NFe
                        case 136: //Evento homologado sem vinculação com a respectiva NFe (SEFAZ não encontrou a NFe na base dela)
                        case 155: //Evento de Cancelamento homologado fora do prazo permitido para cancelamento
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;

                        default: //Evento rejeitado
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button55_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EnvEvento
                {
                    Versao = "1.00",
                    IdLote = "000000000000001",
                    Evento = new List<Evento> {
                        new Evento
                        {
                            Versao = "1.00",
                            InfEvento = new Unimake.Business.DFe.Xml.NFe.InfEvento(new Unimake.Business.DFe.Xml.NFe.DetEventoCancCompEntregaNFe
                            {
                                Versao = "1.00",
                                COrgaoAutor = UFBrasil.PR,
                                TpAutor = TipoAutor.EmpresaEmitente,
                                VerAplic = "uninfe teste 1.0",
                                NProtEvento = "123456789012345"
                            })
                            {
                                COrgao = UFBrasil.AN,
                                ChNFe = "41190806117473000150550010000579131943463890",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.CancelamentoComprovanteEntregaNFe,
                                NSeqEvento = 1,
                                VerEvento = "1.00",
                                TpAmb = TipoAmbiente.Homologacao
                            }
                        }
                    }
                };

                var configuracao = new Configuracao
                {
                    CertificadoDigital = CertificadoSelecionado
                };

                var recepcaoEvento = new RecepcaoEvento(xml, configuracao);
                recepcaoEvento.Executar();

                var xmlDistrib = new XmlDocument();
                xmlDistrib = recepcaoEvento.ProcEventoNFeResult[0].GerarXML();

                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.XMotivo);

                var qq = recepcaoEvento.ProcEventoNFeResult[0].GerarXML().OuterXml;

                //Gravar o XML de distribuição se a inutilização foi homologada
                if (recepcaoEvento.Result.CStat == 128) //128 = Lote de evento processado com sucesso
                {
                    switch (recepcaoEvento.Result.RetEvento[0].InfEvento.CStat)
                    {
                        case 135: //Evento homologado com vinculação da respectiva NFe
                        case 136: //Evento homologado sem vinculação com a respectiva NFe (SEFAZ não encontrou a NFe na base dela)
                        case 155: //Evento de Cancelamento homologado fora do prazo permitido para cancelamento
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;

                        default: //Evento rejeitado
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button56_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EventoMDFe
                {
                    Versao = "3.00",
                    InfEvento = new Unimake.Business.DFe.Xml.MDFe.InfEvento(new Unimake.Business.DFe.Xml.MDFe.DetEventoPagtoOperMDFe
                    {
                        VersaoEvento = "3.00",
                        EventoPagtoOperMDFe = new EventoPagtoOperMDFe
                        {
                            DescEvento = "Pagamento Operacao MDF-e",
                            NProt = "941190000014312",
                            InfViagens = new InfViagens
                            {
                                NroViagem = "00001",
                                QtdViagens = "00001"
                            },
                            InfPag = new List<PagtoOperMDFeInfPag>
                            {
                                new PagtoOperMDFeInfPag
                                {
                                    XNome = "TESTE TRANSPORTE E OPERACOES LTDA",
                                    CNPJ = "00000000000000",
                                    Comp = new List<Unimake.Business.DFe.Xml.MDFe.Comp>
                                    {
                                        new Unimake.Business.DFe.Xml.MDFe.Comp
                                        {
                                            TpComp = TipoComponenteMDFe.Outros,
                                            VComp = 2000.00,
                                            XComp = "PAGAMENTO DE FRETE"
                                        },
                                        new Unimake.Business.DFe.Xml.MDFe.Comp
                                        {
                                            TpComp = TipoComponenteMDFe.ValePedagio,
                                            VComp = 500.00
                                        },
                                        new Unimake.Business.DFe.Xml.MDFe.Comp
                                        {
                                            TpComp = TipoComponenteMDFe.Outros,
                                            VComp = 500.00,
                                            XComp = "COMPRA DE PNEUS"
                                        }
                                    },
                                    VContrato = 3000.00,
                                    IndPag = IndicadorPagamento.PagamentoPrazo,
                                    VAdiant = 500.00,
                                    InfPrazo = new List<InfPrazo>
                                    {
                                        new InfPrazo
                                        {
                                            NParcela = "001",
                                            DVenc = DateTime.Now.AddDays(20),
                                            VParcela = 2000.00
                                        },
                                        new InfPrazo
                                        {
                                            NParcela = "002",
                                            DVenc = DateTime.Now.AddDays(40),
                                            VParcela = 500.00
                                        }
                                    },
                                    InfBanc = new InfBanc
                                    {
                                        PIX = "+5544993333223"
                                    }
                                }
                            }
                        }
                    })
                    {
                        COrgao = UFBrasil.PR,
                        ChMDFe = "41200380568835000181580010000007171930099252",
                        CNPJ = "80568835000181",
                        DhEvento = DateTime.Now,
                        TpEvento = TipoEventoMDFe.PagamentoOperacao,
                        NSeqEvento = 1,
                        TpAmb = TipoAmbiente.Homologacao
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.MDFe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var recepcaoEvento = new Unimake.Business.DFe.Servicos.MDFe.RecepcaoEvento(xml, configuracao);

                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.InfEvento.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (recepcaoEvento.Result.InfEvento.CStat)
                {
                    case 134: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respectivo CT-e com situação diferente de Autorizada.
                    case 135: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respetivo CTe.
                    case 136: //Recebido pelo Sistema de Registro de Eventos – vinculação do evento ao respectivo CT-e prejudicado.
                        recepcaoEvento.GravarXmlDistribuicao(@"c:\testecte\");
                        break;

                    default:
                        //Quando o evento é rejeitado pela Sefaz.
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button61_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EnvEvento
                {
                    Versao = "1.00",
                    IdLote = "000000000000001",
                    Evento = new List<Evento> {
                        new Evento
                        {
                            Versao = "1.00",
                            InfEvento = new Unimake.Business.DFe.Xml.NFe.InfEvento(new Unimake.Business.DFe.Xml.NFe.DetEventoPedidoProrrogPrazoICMS
                            {
                                Versao = "1.00",
                                NProt = "000000000000000",
                                ItemPedido = new List<ItemPedidoProrrogPrazoICMS>
                                {
                                    new ItemPedidoProrrogPrazoICMS
                                    {
                                        NumItem = 1,
                                        QtdeItem = 10
                                    },
                                    new ItemPedidoProrrogPrazoICMS
                                    {
                                        NumItem = 2,
                                        QtdeItem = 15
                                    }
                                }
                            })
                            {
                                COrgao = UFBrasil.PR,
                                ChNFe = "41190806117473000150550010000579131943463890",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.PedidoProrrogacaoPrazo2,
                                NSeqEvento = 1,
                                VerEvento = "1.00",
                                TpAmb = TipoAmbiente.Homologacao
                            }
                        }
                    }
                };

                var configuracao = new Configuracao
                {
                    CertificadoDigital = CertificadoSelecionado
                };

                var recepcaoEvento = new RecepcaoEvento(xml, configuracao);
                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                if (recepcaoEvento.Result.CStat == 128) //128 = Lote de evento processado com sucesso
                {
                    switch (recepcaoEvento.Result.RetEvento[0].InfEvento.CStat)
                    {
                        case 135: //Evento homologado com vinculação da respectiva NFe
                        case 136: //Evento homologado sem vinculação com a respectiva NFe (SEFAZ não encontrou a NFe na base dela)
                        case 155: //Evento de Cancelamento homologado fora do prazo permitido para cancelamento
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;

                        default: //Evento rejeitado
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void button62_Click(object sender, EventArgs e)
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(@"C:\Users\Wandrey\Downloads\qq-ped-eve.xml");
                var xml = new EnvEvento();
                xml = xml.LerXML<EnvEvento>(doc);

                //var xml = new EnvEvento
                //{
                //    Versao = "1.00",
                //    IdLote = "000000000000001",
                //    Evento = new List<Evento> {
                //        new Evento
                //        {
                //            Versao = "1.00",
                //            InfEvento = new Unimake.Business.DFe.Xml.NFe.InfEvento(new Unimake.Business.DFe.Xml.NFe.DetEventoCancPedidoProrrogPrazoICMS
                //            {
                //                Versao = "1.00",
                //                IdPedidoCancelado = "123456789012345678901234567890123456789012345678901234",
                //                NProt = "000000000000000"
                //            })
                //            {
                //                COrgao = UFBrasil.PR,
                //                ChNFe = "41190806117473000150550010000579131943463890",
                //                CNPJ = "06117473000150",
                //                DhEvento = DateTime.Now,
                //                TpEvento = TipoEventoNFe.CancelamentoPedidoProrrogacaoPrazo1,
                //                NSeqEvento = 1,
                //                VerEvento = "1.00",
                //                TpAmb = TipoAmbiente.Homologacao
                //            }
                //        }
                //    }
                //};

                var configuracao = new Configuracao
                {
                    CertificadoDigital = CertificadoSelecionado
                };

                var recepcaoEvento = new RecepcaoEvento(xml, configuracao);
                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                if (recepcaoEvento.Result.CStat == 128) //128 = Lote de evento processado com sucesso
                {
                    switch (recepcaoEvento.Result.RetEvento[0].InfEvento.CStat)
                    {
                        case 135: //Evento homologado com vinculação da respectiva NFe
                        case 136: //Evento homologado sem vinculação com a respectiva NFe (SEFAZ não encontrou a NFe na base dela)
                        case 155: //Evento de Cancelamento homologado fora do prazo permitido para cancelamento
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;

                        default: //Evento rejeitado
                            recepcaoEvento.GravarXmlDistribuicao(@"c:\testenfe\");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void BtnEventoAlteracaoPgtoServMDFe_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EventoMDFe
                {
                    Versao = "3.00",
                    InfEvento = new Unimake.Business.DFe.Xml.MDFe.InfEvento(new DetEventoAlteracaoPagtoServMDFe
                    {
                        VersaoEvento = "3.00",

                        EventoAlteracaoPagtoServMDFe = new EventoAlteracaoPagtoServMDFe
                        {
                            DescEvento = "Alteracao Pagamento Servico MDFe",
                            NProt = "941190000014312",
                            InfPag = new List<AlteracaoPagtoServMDFeInfPag>
                            {
                                new AlteracaoPagtoServMDFeInfPag
                                {
                                    XNome = "TESTE TRANSPORTE E OPERACOES LTDA",
                                    CNPJ = "00000000000000",
                                    Comp = new List<Unimake.Business.DFe.Xml.MDFe.Comp>
                                    {
                                        new Unimake.Business.DFe.Xml.MDFe.Comp
                                        {
                                            TpComp = TipoComponenteMDFe.Outros,
                                            VComp = 2000.00,
                                            XComp = "PAGAMENTO DE FRETE"
                                        },
                                        new Unimake.Business.DFe.Xml.MDFe.Comp
                                        {
                                            TpComp = TipoComponenteMDFe.ValePedagio,
                                            VComp = 500.00
                                        },
                                        new Unimake.Business.DFe.Xml.MDFe.Comp
                                        {
                                            TpComp = TipoComponenteMDFe.Outros,
                                            VComp = 500.00,
                                            XComp = "COMPRA DE PNEUS"
                                        }
                                    },
                                    VContrato = 3000.00,
                                    IndPag = IndicadorPagamento.PagamentoPrazo,
                                    VAdiant = 500.00,
                                    InfPrazo = new List<InfPrazo>
                                    {
                                        new InfPrazo
                                        {
                                            NParcela = "001",
                                            DVenc = DateTime.Now.AddDays(20),
                                            VParcela = 2000.00
                                        },
                                        new InfPrazo
                                        {
                                            NParcela = "002",
                                            DVenc = DateTime.Now.AddDays(40),
                                            VParcela = 500.00
                                        }
                                    },
                                    InfBanc = new InfBanc
                                    {
                                        PIX = "+5544993333223"
                                    }
                                }
                            }
                        }
                    })
                    {
                        COrgao = UFBrasil.PR,
                        ChMDFe = "41200380568835000181580010000007171930099252",
                        CNPJ = "80568835000181",
                        DhEvento = DateTime.Now,
                        TpEvento = TipoEventoMDFe.AlteracaoPagamentoServico,
                        NSeqEvento = 1,
                        TpAmb = TipoAmbiente.Homologacao
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.MDFe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var recepcaoEvento = new Unimake.Business.DFe.Servicos.MDFe.RecepcaoEvento(xml, configuracao);

                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.InfEvento.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (recepcaoEvento.Result.InfEvento.CStat)
                {
                    case 134: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respectivo CT-e com situação diferente de Autorizada.
                    case 135: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respetivo CTe.
                    case 136: //Recebido pelo Sistema de Registro de Eventos – vinculação do evento ao respectivo CT-e prejudicado.
                        recepcaoEvento.GravarXmlDistribuicao(@"c:\testecte\");
                        break;

                    default:
                        //Quando o evento é rejeitado pela Sefaz.
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void BtnConfirmaServMDFe_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new EventoMDFe
                {
                    Versao = "3.00",
                    InfEvento = new Unimake.Business.DFe.Xml.MDFe.InfEvento(new DetEventoConfirmaServMDFe
                    {
                        VersaoEvento = "3.00",

                        EventoConfirmaServMDFe = new EventoConfirmaServMDFe
                        {
                            DescEvento = "Confirmacao Servico Transporte",
                            NProt = "941190000014312"
                        }
                    })
                    {
                        COrgao = UFBrasil.PR,
                        ChMDFe = "41200380568835000181580010000007171930099252",
                        CNPJ = "80568835000181",
                        DhEvento = DateTime.Now,
                        TpEvento = TipoEventoMDFe.ConfirmacaoServicoTransporte,
                        NSeqEvento = 1,
                        TpAmb = TipoAmbiente.Homologacao
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = TipoDFe.MDFe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var recepcaoEvento = new Unimake.Business.DFe.Servicos.MDFe.RecepcaoEvento(xml, configuracao);

                recepcaoEvento.Executar();
                MessageBox.Show(recepcaoEvento.RetornoWSString);
                MessageBox.Show(recepcaoEvento.Result.InfEvento.XMotivo);

                //Gravar o XML de distribuição se a inutilização foi homologada
                switch (recepcaoEvento.Result.InfEvento.CStat)
                {
                    case 134: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respectivo CT-e com situação diferente de Autorizada.
                    case 135: //Recebido pelo Sistema de Registro de Eventos, com vinculação do evento no respetivo CTe.
                    case 136: //Recebido pelo Sistema de Registro de Eventos – vinculação do evento ao respectivo CT-e prejudicado.
                        recepcaoEvento.GravarXmlDistribuicao(@"c:\testecte\");
                        break;

                    default:
                        //Quando o evento é rejeitado pela Sefaz.
                        break;
                }
            }
            catch (Exception ex)
            {
                CatchException(ex);
            }
        }

        private void BtnGerarXmlNFSePaulistana_Click(object sender, EventArgs e)
        {
            //Iniciar o XML da NFSe
            var xmlNFSe = new XDocument(new XDeclaration("1.0", "utf-8", null));
            XNamespace xNamespace = "http://www.prefeitura.sp.gov.br/nfe";

            #region Criar a primeira tag <PedidoEnvioRPS> do XML com o namespace

            //Criar a tag <PedidoEnvioRps>
            var tagPedidoEnvioRPS = new XElement(xNamespace + "PedidoEnvioRPS");

            #endregion

            #region Criar o grupo de tag <Cabecalho>

            //Criar tag <Cabecalho>
            var tagCabecalho = new XElement("Cabecalho");

            //Criar o atributo Versao na tag <Cabecalho>
            tagCabecalho.Add(new XAttribute("Versao", "1"));

            //Criar o grupo de tag CPFCNPJRemetente
            var tagCPFCNPJRemetente = new XElement("CPFCNPJRemetente");

            //Criar a tags filhas do grupo <CPFCNPJRemetente>
            tagCPFCNPJRemetente.Add(new XElement("CNPJ", "99999997000100"));

            //Adicionar a tag <CPFCNPJRementente> dentro da tag <Cabecalho>
            tagCabecalho.Add(tagCPFCNPJRemetente);

            //Adicionar a tag <Cabelhalho> dentro da tag <PedidoEnvioRPS>
            tagPedidoEnvioRPS.Add(tagCabecalho);

            #endregion

            #region Criar o grupo de tag <RPS>

            //Criar tag <RPS>
            var tagRPS = new XElement("RPS");

            //Criar tag <Assinatura>
            tagRPS.Add(new XElement("Assinatura", "d8Pg/jdA7t5tSaB8Il1d/CMiLGgfFAXzTL9o5stv6TNbhm9I94DIo0/ocqJpGx0KzoEeIQz4RSn99pWX4fiW/aETlNT3u5woqCAyL6U2hSyl/eQfWRYrqFu2zcdc4rsAG/wJbDjNO8y0Pz9b6rlTwkIJ+kMdLo+EWXMnB744olYE721g2O9CmUTvjtBgCfVUgvuN1MGjgzpgyussCOSkLpGbrqtM5+pYMXZsTaEVIIck1baDkoRpLmZ5Y/mcn1/Om1fMyhJVUAkgI5xBrORuotIP7e3+HLJnKgzQQPWCtLyEEyAqUk9Gq64wMayITua5FodaJsX+Eic/ie3kS5m50Q=="));

            //Criar grupo de Tag <ChaveRPS>
            var tagChaveRPS = new XElement("ChaveRPS");

            //Criar tags filhas do grupo <ChaveRPS>
            tagChaveRPS.Add(new XElement("InscricaoPrestador", "39616924"));
            tagChaveRPS.Add(new XElement("SerieRPS", "BB"));
            tagChaveRPS.Add(new XElement("NumeroRPS", "4105"));

            tagRPS.Add(tagChaveRPS);

            //Criar várias tags filhas da tag <RPS>
            tagRPS.Add(new XElement("TipoRPS", "RPS-M"));
            tagRPS.Add(new XElement("DataEmissao", "2015-01-20"));
            tagRPS.Add(new XElement("StatusRPS", "N"));
            tagRPS.Add(new XElement("TributacaoRPS", "T"));
            tagRPS.Add(new XElement("ValorServicos", "20500"));
            tagRPS.Add(new XElement("ValorDeducoes", "5000"));
            tagRPS.Add(new XElement("ValorPIS", "10"));
            tagRPS.Add(new XElement("ValorCOFINS", "10"));
            tagRPS.Add(new XElement("ValorINSS", "10"));
            tagRPS.Add(new XElement("ValorIR", "10"));
            tagRPS.Add(new XElement("ValorCSLL", "10"));
            tagRPS.Add(new XElement("CodigoServico", "7617"));
            tagRPS.Add(new XElement("AliquotaServicos", "0.05"));
            tagRPS.Add(new XElement("ISSRetido", "false"));

            //Criar grupo de tag <CPFCNPJTomador>
            var tagCPFCNPJTomador = new XElement("CPFCNPJTomador");

            //Criar tags filhas do grupo <CPFCNPJTomador>
            tagCPFCNPJTomador.Add(new XElement("CPF", "12345678909"));

            //Adicionar a tag <CPFCNPJTomador> dentro da tag <RPS>
            tagRPS.Add(tagCPFCNPJTomador);

            //Criar mais tags filhas da tag <RPS>
            tagRPS.Add(new XElement("RazaoSocialTomador", "TOMADOR PF"));

            //Criar tag <EnderecoTomador>
            var tagEnderecoTomador = new XElement("EnderecoTomador");

            //Criar tags filhas da tag <EnderecoTomador>
            tagEnderecoTomador.Add(new XElement("TipoLogradouro", "Av"));
            tagEnderecoTomador.Add(new XElement("Logradouro", "Paulista"));
            tagEnderecoTomador.Add(new XElement("NumeroEndereco", "100"));
            tagEnderecoTomador.Add(new XElement("ComplementoEndereco", "Cj 35"));
            tagEnderecoTomador.Add(new XElement("Bairro", "Bela Vista"));
            tagEnderecoTomador.Add(new XElement("Cidade", "3550308"));
            tagEnderecoTomador.Add(new XElement("UF", "SP"));
            tagEnderecoTomador.Add(new XElement("CEP", "1310100"));

            //Adicionar a tag <EnderecoTomador> dentro da tag <RPS>
            tagRPS.Add(tagEnderecoTomador);

            //Criar mais tags filhas da tag <RPS>
            tagRPS.Add(new XElement("EmailTomador", "tomador@teste.com.br"));
            tagRPS.Add(new XElement("Discriminacao", "Desenvolvimento de Web Site Pessoal."));

            //Adicionar a tag <RPS> dentro da tag <PedidoEnvioRPS>
            tagPedidoEnvioRPS.Add(tagRPS);

            #endregion

            //Adicionar a tag <PedidoEnvioRPS> no xmlNfse
            xmlNFSe.Add(tagPedidoEnvioRPS);

            //Gravar XML na pasta
            xmlNFSe.Save(@"d:\testenfe\xmlnfse.xml");

            //Recuperar o conteúdo gerado acima para passar para a DLL no formato XmlDocument, que é o tipo esperado pelo método Executar da DLL
            var xmlDocument = new XmlDocument();
            xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);

            using (var xmlReader = xmlNFSe.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }

            //Pronto... só passar a xmlDocumento para o método Executar
        }
    }
}

