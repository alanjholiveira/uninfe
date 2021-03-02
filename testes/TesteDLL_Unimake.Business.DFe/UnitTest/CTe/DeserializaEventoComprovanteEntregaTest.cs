using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Unimake.Business.DFe.Utility;
using Unimake.Business.DFe.Xml.CTe;

namespace TesteDLL_Unimake.Business.DFe.UnitTest.CTe
{
	[TestClass]
	public class DeserializaEventoComprovanteEntregaTest
	{
		#region Public Methods

		[TestMethod]
		public void DeserializarEventoComprovanteEntrega()
		{
			var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<eventoCTe versao=""3.00"" xmlns=""http://www.portalfiscal.inf.br/cte"">
	<infEvento Id=""ID1101804120021085928300018557001000000567122707061501"">
		<cOrgao>41</cOrgao>
		<tpAmb>2</tpAmb>
		<CNPJ>10859283000185</CNPJ>
		<chCTe>41200210859283000185570010000005671227070615</chCTe>
		<dhEvento>2020-03-18T11:33:14-03:00</dhEvento>
		<tpEvento>110180</tpEvento>
		<nSeqEvento>1</nSeqEvento>
		<detEvento versaoEvento=""3.00"">
			<evCECTe>
				<descEvento>Comprovante de Entrega</descEvento>
				<nProt>141200000007987</nProt>
				<dhEntrega>2020-03-18T11:33:14-03:00</dhEntrega>
				<nDoc>91886127085</nDoc>
				<xNome>Teste</xNome>
				<latitude>00</latitude>
				<longitude>000</longitude>
				<hashEntrega>1234564321321321321231231321</hashEntrega>
				<dhHashEntrega>2020-03-18T11:33:14-03:00</dhHashEntrega>
				<infEntrega>
					<chNFe>12345678901234567890123456789012345678901234</chNFe>
				</infEntrega>
				<infEntrega>
					<chNFe>12345678901234567890123456789012345678901234</chNFe>
				</infEntrega>
				<infEntrega>
					<chNFe>12345678901234567890123456789012345678901234</chNFe>
				</infEntrega>
			</evCECTe>
		</detEvento>
	</infEvento>
</eventoCTe>";
			//TODO WANDREY: Resolver esta encrenca
			//var eventoCTe = XMLUtility.Deserializar<EventoCTe>(xml);
			//Debug.Assert((eventoCTe.InfEvento.DetEvento as DetEventoCompEntrega)?.EventoCECTe?.InfEntrega.Count == 3);
		}

		#endregion Public Methods
	}
}