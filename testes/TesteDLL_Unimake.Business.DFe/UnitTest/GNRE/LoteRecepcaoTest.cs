using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Servicos.GNRE;
using Unimake.Business.DFe.Xml.GNRE;
using Unimake.Security.Platform;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.GNRE
{
    [TestClass]
    public class LoteRecepcaoTest
    {
        #region Public Methods

        [TestMethod]
        public void LoteRecepcao()
        {
            string path = @"D:\testenfe\certificadoGNRE2_12345678.pfx";
            X509Certificate2 CertificadoSelecionado = new CertificadoDigital().CarregarCertificadoDigitalA1(path, "12345678");

            var xml = new TLoteGNRE
            {
                Guias = new Guias
                {
                    TDadosGNRE = new List<TDadosGNRE>
                    {
                        new TDadosGNRE
                        {
                            Versao = "2.00",
                            UfFavorecida = UFBrasil.PR,
                            TipoGNRE = TipoGuiaGNRE.Simples,
                            ContribuinteEmitente = new ContribuinteEmitente
                            {
                                Identificacao = new Identificacao
                                {
                                    CPF = "68630452064",
                                    CNPJ = "79358552000126",
                                    IE = "054494562414"
                                },
                                RazaoSocial = "Unimake Software",
                                Endereco = "Rua teste",
                                Municipio = "18402",
                                UF = UFBrasil.PR,
                                CEP= "87704030",
                                Telefone = "31414900"
                            },

                            ItensGNRE = new ItensGNRE
                            {
                                Item = new List<Item>
                                {
                                    new Item
                                    {
                                        Receita = "123456",
                                        DetalhamentoReceita = "teste detalhamento receita",
                                        DocumentoOrigem = new DocumentoOrigem
                                        {
                                            Tipo = "01",
                                            Value = "1234"
                                        },
                                        Produto = "Produto teste",
                                        Referencia = new Referencia
                                        {
                                            Periodo = 1,
                                            Mes = Meses.Novembro,
                                            Ano = "2020",
                                            Parcela = "1"
                                        },

                                        DataVencimento = DateTime.Now,

                                        Valor = new List<Valor>
                                        {
                                            new Valor
                                            {
                                                Tipo = Valor.ItemValorTipo.Item11,
                                                ValorOriginal = 10.00
                                            }
                                        },
                                        Convenio = "teste convenio",
                                        ContribuinteDestinatario = new ContribuinteDestinatario
                                        {
                                            Identificacao = new Identificacao
                                            {
                                                CPF = "68630452064",
                                                CNPJ = "79358552000126",
                                                IE = "054494562414"
                                            },
                                            RazaoSocial = "Unimake Software",
                                            Municipio = "18402",
                                        },

                                        CamposExtras = new List<CamposExtras>
                                        {
                                            new CamposExtras
                                            {
                                                CampoExtra = new CampoExtra
                                                {
                                                    Codigo = 123,
                                                    Valor = "1500",
                                                }
                                            }
                                        },
                                    }
                                }
                            },
                         ValorGNRE = 150.33 
                        }
                    }
                }
            };

            int Ambiente = 2;

            var configuracao = new Configuracao
            {
                TipoDFe = TipoDFe.GNRE,
                TipoEmissao = TipoEmissao.Normal,
                CertificadoDigital = CertificadoSelecionado,
                TipoAmbiente = (TipoAmbiente)Ambiente,
                CodigoUF = 41 //Paraná
            };

            var loteRecepcao = new LoteRecepcao(xml, configuracao);
            
            loteRecepcao.Executar();

            Debug.Assert(loteRecepcao.Result != null);
            Debug.Assert(!string.IsNullOrWhiteSpace(loteRecepcao.Result.SituacaoRecepcao.Codigo));
        }

        #endregion Public Methods
    }
}