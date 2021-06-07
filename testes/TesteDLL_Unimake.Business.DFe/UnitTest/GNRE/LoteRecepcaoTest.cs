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
                                    CNPJ = "07638784000127",
                                    IE = "9035408506"
                                },
                                RazaoSocial = "OESTE PHARMA DISTRIBUIDORA DE MEDICAMENTOS EIRELI",
                                Endereco = "RUA SALGADO FILHO",
                                Municipio = "04808",
                                UF = UFBrasil.PR,
                                CEP= "85811100",
                                Telefone = "04532233033"
                            },

                            ItensGNRE = new ItensGNRE
                            {
                                Item = new List<Item>
                                {
                                    new Item
                                    {
                                        Receita = "100099",
                                        DocumentoOrigem = new DocumentoOrigem
                                        {
                                            Tipo = "10",
                                            Value = "000535770"
                                        },
                                        DataVencimento = DateTime.Now,
                                        Valor = new List<Valor>
                                        {
                                            new Valor
                                            {
                                                Tipo = Valor.ItemValorTipo.Item11,
                                                ValorOriginal = 122.97
                                            }
                                        },
                                        ContribuinteDestinatario = new ContribuinteDestinatario
                                        {
                                            Identificacao = new Identificacao
                                            {
                                                CNPJ = "24415230000694",
                                            },
                                            RazaoSocial = "NAVARRO DISTRIBUIDORA DE MEDICAMENTOS S/A",
                                            Municipio = "25506",
                                        },
                                    }
                                }
                            },
                         ValorGNRE = 122.97,
                         DataPagamento = DateTime.Now
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