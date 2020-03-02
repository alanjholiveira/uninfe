﻿#pragma warning disable IDE1006 // Naming Styles

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Unimake.Business.DFe.Security;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.NFe;
using Unimake.Business.DFe.Xml.CTe;
using Unimake.Business.DFe.Xml.MDFe;
using Unimake.Business.DFe.Xml.NFe;
using Unimake.Security.Platform;

namespace TesteDLL_Unimake.Business.DFe
{
    public partial class FormTestarNFe : Form
    {
        #region Private Fields

        private readonly UFBrasil CUF = UFBrasil.PR;
        private readonly TipoAmbiente TpAmb = TipoAmbiente.Homologacao;
        private X509Certificate2 CertificadoSelecionado;

        #endregion Private Fields

        #region Private Properties

        private string UF => ((int)CUF).ToString();

        #endregion Private Properties

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

            if (string.IsNullOrWhiteSpace(path))
            {
                MessageBox.Show("Arquivo é obrigatório!", "Arquivo é requerido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var password = Microsoft.VisualBasic.Interaction.InputBox("Informe a senha do certificado.", "Certificado");

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Senha é obrigatória!", "Senha requerida", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                var bytes = CertificadoDigital.ToByteArray(path);
                CertificadoSelecionado = CertificadoDigital.CarregarCertificadoDigitalA1(bytes, password);
                MessageBox.Show("O certificado foi selecionado.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERRO!!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsStatServ
                {
                    Versao = "4.00",
                    CUF = UFBrasil.MG,
                    TpAmb = TipoAmbiente.Homologacao
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = DFE.NFe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var statusServico = new StatusServico(xml, configuracao);
                statusServico.Executar();
                MessageBox.Show(statusServico.RetornoWSString);
                MessageBox.Show(statusServico.Result.XMotivo);
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
                                DescEvento = "Confirmacao da Operacao",
                                XJust = "Justificativa para manifestação da NFe de teste"
                            })
                            {
                                COrgao = UFBrasil.AN,
                                ChNFe = "41191000563803000154550010000020901551010553",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.ManifestacaoConfirmacaoOperacao,
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
                                COrgao = CUF,
                                ChNFe = "41191006117473000150550010000579281779843610",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.CartaCorrecao,
                                NSeqEvento = 3,
                                VerEvento = "1.00",
                                TpAmb = TpAmb
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
                                COrgao = CUF,
                                ChNFe = "41191006117473000150550010000579281779843610",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.CartaCorrecao,
                                NSeqEvento = 4,
                                VerEvento = "1.00",
                                TpAmb = TpAmb
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

        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsStatServ
                {
                    Versao = "4.00",
                    CUF = CUF,
                    TpAmb = TpAmb
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = DFE.NFCe,
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
                    TipoDFe = DFE.NFCe,
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
                        CUF = CUF,
                        Mod = ModeloDFe.NFCe,
                        NNFIni = 57919,
                        NNFFin = 57919,
                        Serie = 1,
                        TpAmb = TpAmb,
                        XJust = "Justificativa da inutilizacao de teste"
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = DFE.NFCe,
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
                        UF = CUF
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = DFE.NFCe,
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
                                COrgao = CUF,
                                ChNFe = "41190806117473000150650010000579131943463890",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.Cancelamento,
                                NSeqEvento = 1,
                                VerEvento = "1.00",
                                TpAmb = TpAmb
                            }
                        }
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = DFE.NFCe,
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
                                COrgao = CUF,
                                ChNFe = "41191006117473000150650010000579281779843610",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.CartaCorrecao,
                                NSeqEvento = 3,
                                VerEvento = "1.00",
                                TpAmb = TpAmb
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
                                COrgao = CUF,
                                ChNFe = "41191006117473000150650010000579281779843610",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.CartaCorrecao,
                                NSeqEvento = 4,
                                VerEvento = "1.00",
                                TpAmb = TpAmb
                            }
                        }
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = DFE.NFCe,
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
                                        CUF = CUF,
                                        NatOp = "VENDA PRODUC.DO ESTABELEC",
                                        Mod = ModeloDFe.NFCe,
                                        Serie = 1,
                                        NNF = 57929,
                                        DhEmi = DateTime.Now,
                                        TpNF = TipoOperacao.Saida,
                                        IdDest = DestinoOperacao.OperacaoInterna,
                                        CMunFG = 4118402,
                                        TpImp = FormatoImpressaoDANFE.NFCe,
                                        TpEmis = TipoEmissao.Normal,
                                        TpAmb = TpAmb,
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
                                            UF = CUF,
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
                                        CNPJ = "01761135000132",
                                        XNome = "NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                                        EnderDest = new Unimake.Business.DFe.Xml.NFe.EnderDest
                                        {
                                            XLgr = "AV. COMENDADOR NORBERTO MARCONDES",
                                            Nro = "2156",
                                            XBairro = "CENTRO",
                                            CMun = 4104303,
                                            XMun = "CAMPO MOURAO",
                                            UF = UFBrasil.PR,
                                            CEP = "87303100",
                                            Fone = "04430171247"
                                        },
                                        IndIEDest = IndicadorIEDestinatario.NaoContribuinte,
                                        Email = "teste@teste.com.br"
                                    },
                                    Det = CriarDet(),
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
                                    Pag = new Pag
                                    {
                                        DetPag = new  List<DetPag>
                                        {
                                             new DetPag
                                             {
                                                 IndPag = IndicadorPagamento.PagamentoVista,
                                                 TPag = MeioPagamento.Outros,
                                                 VPag = 84.90
                                             }
                                        }
                                    },
                                    InfAdic = new InfAdic
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
                    TipoDFe = DFE.NFCe,
                    CertificadoDigital = CertificadoSelecionado,
                    CSC = "HCJBIRTWGCQ3HVQN7DCA0ZY0P2NYT6FVLPJG",
                    CSCIDToken = 2
                };

                var autorizacao = new Unimake.Business.DFe.Servicos.NFCe.Autorizacao(xml, configuracao);
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
                    TpAmb = TpAmb,
                    NRec = UF + "3456789012345"
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = DFE.NFCe,
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
                    TpAmb = TpAmb,
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
                        TpAmb = TpAmb,
                        XJust = "Justificativa da inutilizacao de teste"
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = DFE.CTe,
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
                        UF = CUF
                    }
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = DFE.CTe,
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
                TipoDFe = DFE.CTe,
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
                        Versao = "1.01",
                        TpAmb = TipoAmbiente.Homologacao,
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

        private void button24_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsSitMDFe
                {
                    Versao = "3.00",
                    TpAmb = TpAmb,
                    ChMDFe = ((int)CUF).ToString() + "170701761135000132570010000186931903758906"
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = DFE.MDFe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var consultaProtocolo = new Unimake.Business.DFe.Servicos.MDFe.ConsultaProtocolo(xml, configuracao);
                consultaProtocolo.Executar();

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
                    TpAmb = TpAmb
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = DFE.MDFe,
                    CodigoUF = (int)CUF,
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
                var xml = new ConsSitCTe
                {
                    Versao = "3.00",
                    TpAmb = TpAmb,
                    ChCTe = "41200210859283000185570010000005621912070343"
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = DFE.CTe,
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
                    TpAmb = TpAmb
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = DFE.CTe,
                    CodigoUF = (int)CUF,
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
                        CUF = UFBrasil.PR,
                        Mod = ModeloDFe.NFe,
                        NNFIni = 57919,
                        NNFFin = 57919,
                        Serie = 1,
                        TpAmb = TpAmb,
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
                        UF = CUF
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
                                COrgao = CUF,
                                ChNFe = "41190806117473000150550010000579131943463890",
                                CNPJ = "06117473000150",
                                DhEvento = DateTime.Now,
                                TpEvento = TipoEventoNFe.Cancelamento,
                                NSeqEvento = 1,
                                VerEvento = "1.00",
                                TpAmb = TpAmb
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

        private void Button6_Click(object sender, EventArgs e)
        {
            try
            {
                /*
                var xml = new EnviNFe
                {
                    Versao = "4.00",
                    IdLote = "000000000000001",
                    IndSinc = SimNao.Sim,
                    NFe = new List<NFe> {
                        new NFe
                        {
                            InfNFe = new List<InfNFe> {
                                new InfNFe
                                {
                                    Versao = "4.00",

                                    Ide = new Ide
                                    {
                                        CUF = CUF,
                                        NatOp = "VENDA PRODUC.DO ESTABELEC",
                                        Mod = ModeloDFe.NFe,
                                        Serie = 1,
                                        NNF = 57960,
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
                                        IndPres = IndicadorPresenca.OperacaoOutros,
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
                                            UF = CUF,
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
                                                NCM = "84714900",
                                                CFOP = "6101",
                                                UCom = "LU",
                                                QCom = 1.00,
                                                VUnCom = 84.9000000000,
                                                VProd = 84.90,
                                                CEANTrib = "SEM GTIN",
                                                UTrib = "LU",
                                                QTrib = 1.00,
                                                VUnTrib = 84.9000000000,
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
                                        ModFrete = ModalidadeFrete.ContratacaoFretePorContaRemetente_CIF,
                                        Vol = new[]
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
                                    Cobr = new Cobr()
                                    {
                                        Fat = new Fat
                                        {
                                            NFat = "057910",
                                            VOrig = 84.90,
                                            VDesc = 0,
                                            VLiq = 84.90
                                        },
                                        Dup = new[]
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
                                        DetPag = new[]
                                        {
                                             new DetPag
                                             {
                                                 IndPag = IndicadorPagamento.PagamentoVista,
                                                 TPag = MeioPagamento.Outros,
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
                */

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
                                        VerProc = "VisualNF-e 3.0.0.1"
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
                                                VUnCom = 0.46,
                                                VProd = 140.30,
                                                CEANTrib = "SEM GTIN",
                                                UTrib = "MT",
                                                QTrib = 305,
                                                VUnTrib = 0.46,
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
                                    InfAdic = new InfAdic
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

                var configuracao = new Configuracao
                {
                    CertificadoDigital = CertificadoSelecionado
                };

                var autorizacao = new Autorizacao(xml, configuracao);
                autorizacao.Executar();
                MessageBox.Show(autorizacao.RetornoWSString);
                MessageBox.Show(autorizacao.Result.XMotivo);

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

        private void Button7_Click(object sender, EventArgs e)
        {
            try
            {
                var xml = new ConsReciNFe
                {
                    Versao = "4.00",
                    TpAmb = TipoAmbiente.Homologacao,
                    NRec = "310000069231900"
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
            AssinaturaDigital.Assinar(xml, "rDE", "DE", CertificadoSelecionado, AlgorithmType.Sha256, true, "", "Id");
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
                        Versao = "1.01",
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
            var det = new List<Det>();

            for (var i = 0; i < 2; i++)
            {
                det.Add(new Det
                {
                    NItem = i + 1,
                    Prod = new Prod
                    {
                        CProd = "01042",
                        CEAN = "SEM GTIN",
                        XProd = "NOTA FISCAL EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL",
                        NCM = "84714900",
                        CFOP = "5101",
                        UCom = "LU",
                        QCom = 1.00,
                        VUnCom = 84.9000000000,
                        VProd = 84.90,
                        CEANTrib = "SEM GTIN",
                        UTrib = "LU",
                        QTrib = 1.00,
                        VUnTrib = 84.9000000000,
                        IndTot = SimNao.Sim,
                        XPed = "300474",
                        NItemPed = 1
                    },
                    Imposto = new Imposto
                    {
                        VTotTrib = 12.63,
                        ICMS = CriarICMS(),
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
                });
            }

            return det;
        }

        /// <summary>
        /// Vou adicionar as tags de ICMS, isso é para o caso de se ter mais de uma por produto, mas o mais comum é ter uma só.
        /// </summary>
        /// <returns></returns>
        private List<Unimake.Business.DFe.Xml.NFe.ICMS> CriarICMS()
        {
            var icms = new List<Unimake.Business.DFe.Xml.NFe.ICMS>();

            for (var i = 0; i < 1; i++)
            {
                icms.Add(
                    new Unimake.Business.DFe.Xml.NFe.ICMS
                    {
                        ICMSSN102 = new ICMSSN102
                        {
                            Orig = OrigemMercadoria.Nacional,
                            CSOSN = "102"
                        }
                    });
            }

            return icms;
        }

        private void FormTestarNFe_Shown(object sender, EventArgs e)
        {
            var cert = new CertificadoDigital();
            CertificadoSelecionado = cert.Selecionar();
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
                    NRec = "410000007934162"
                };

                var configuracao = new Configuracao
                {
                    TipoDFe = DFE.CTe,
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
                        COrgao = CUF,
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
                    TipoDFe = DFE.CTe,
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
                    TipoDFe = DFE.MDFe,
                    CertificadoDigital = CertificadoSelecionado
                };

                var retAutorizacao = new Unimake.Business.DFe.Servicos.MDFe.RetAutorizacao(xml, configuracao);
                retAutorizacao.Executar();
                MessageBox.Show(retAutorizacao.RetornoWSString);
                MessageBox.Show(retAutorizacao.Result.XMotivo);
            }
            catch(Exception ex)
            {
                CatchException(ex);
            }

        }
    }
}