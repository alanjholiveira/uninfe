﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Unimake.Business.DFe.Utility;
using Unimake.Business.DFe.Xml.CTe;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.CTe
{
    [TestClass]
    public class DeserializaEventoCartaCorrecaoTest
    {
        #region Public Methods

        [TestMethod]
        public void DeserializarEventoCartaCorrecao()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<eventoCTe xmlns=""http://www.portalfiscal.inf.br/cte"" versao=""3.00"">
  <infEvento Id=""ID1101103515010756541600010457000000001230100001230001"">
    <cOrgao>35</cOrgao>
    <tpAmb>2</tpAmb>
    <CNPJ>07565416000104</CNPJ>
    <chCTe>35150107565416000104570000000012301000012300</chCTe>
    <dhEvento>2015-01-05T16:35:06-02:00</dhEvento>
    <tpEvento>110110</tpEvento>
    <nSeqEvento>1</nSeqEvento>
    <detEvento versaoEvento=""3.00"">
      <evCCeCTe>
        <descEvento>Carta de Correcao</descEvento>
        <infCorrecao>
          <grupoAlterado>ide</grupoAlterado>
          <campoAlterado>cfop</campoAlterado>
          <valorAlterado>6353</valorAlterado>
        </infCorrecao>
        <infCorrecao>
          <grupoAlterado></grupoAlterado>
          <campoAlterado></campoAlterado>
          <valorAlterado></valorAlterado>
          <TagQQ></TagQQ>
        </infCorrecao>
        <infCorrecao>
          <grupoAlterado>grupoAlterado</grupoAlterado>
          <campoAlterado>campoAlterado</campoAlterado>
          <valorAlterado>valorAlterado</valorAlterado>
        </infCorrecao>
        <xCondUso>A Carta de Correcao e disciplinada pelo Art. 58-B do CONVENIO/SINIEF 06/89: Fica permitida a utilizacao de carta de correcao, para regularizacao de erro ocorrido na emissao de documentos fiscais relativos a prestacao de servico de transporte, desde que o erro nao esteja relacionado com: I - as variaveis que determinam o valor do imposto tais como: base de calculo, aliquota, diferenca de preco, quantidade, valor da prestacao;II - a correcao de dados cadastrais que implique mudanca do emitente, tomador, remetente ou do destinatario;III - a data de emissao ou de saida.</xCondUso>
      </evCCeCTe>
    </detEvento>
  </infEvento>
</eventoCTe>";
            //TODO WANDREY: Resolver esta encrenca
            //var eventoCTe = XMLUtility.Deserializar<EventoCTe>(xml);
            //Debug.Assert((eventoCTe.InfEvento.DetEvento as DetEventoCCE)?.InfCorrecao?.Count == 3);
        }

        #endregion Public Methods
    }
}