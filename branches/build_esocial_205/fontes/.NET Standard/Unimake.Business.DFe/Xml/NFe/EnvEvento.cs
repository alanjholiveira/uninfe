﻿#pragma warning disable CS1591

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Utility;

namespace Unimake.Business.DFe.Xml.NFe
{
    [Serializable()]
    [XmlRoot("envEvento", Namespace = "http://www.portalfiscal.inf.br/nfe", IsNullable = false)]
    public class EnvEvento: XMLBase
    {
        #region Private Methods

        private void SignEvent(Evento evento, XmlElement xmlEl)
        {
            var signature = xmlEl.GetElementsByTagName("Signature")[0];
            if(signature != null)
            {
                var signatureEvento = new XmlDocument();

                signatureEvento.LoadXml(signature.OuterXml);
                evento.Signature = XMLUtility.Deserializar<Signature>(signatureEvento);
            }
        }

        #endregion Private Methods

        #region Public Properties

        [XmlElement("evento", Order = 2)]
        public List<Evento> Evento { get; set; } = new List<Evento>();

        [XmlElement("idLote", Order = 1)]
        public string IdLote { get; set; }

        [XmlAttribute(AttributeName = "versao", DataType = "token")]
        public string Versao { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void AddEvento(Evento evento)
        {
            if(Evento == null)
            {
                Evento = new List<Evento>();
            }

            Evento.Add(evento);
        }

        public override XmlDocument GerarXML()
        {
            var xmlDocument = base.GerarXML();

            #region Adicionar o atributo de namespace que falta nas tags "evento"

            var attribute = GetType().GetCustomAttribute<XmlRootAttribute>();

            for(var i = 0; i < xmlDocument.GetElementsByTagName("evento").Count; i++)
            {
                var xmlElement = (XmlElement)xmlDocument.GetElementsByTagName("evento")[i];
                xmlElement.SetAttribute("xmlns", attribute.Namespace);
            }

            #endregion Adicionar o atributo de namespace que falta nas tags "evento"

            return xmlDocument;
        }

        public override T LerXML<T>(XmlDocument doc)
        {
            if(typeof(T) != typeof(EnvEvento))
            {
                throw new InvalidCastException($"Cannot cast type '{typeof(T).Name}' into type '{typeof(EnvEvento).Name}'.");
            }

            var retornar = base.LerXML<T>(doc) as EnvEvento;

            var eventos = doc.GetElementsByTagName("evento");

            if((eventos?.Count ?? 0) > 0)
            {
                retornar.Evento = new List<Evento>();

                foreach(XmlElement xmlEl in eventos)
                {
                    var xml = new StringBuilder();
                    xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                    xml.Append($"<envEvento xmlns=\"{xmlEl.NamespaceURI}\">");
                    xml.Append($"{xmlEl.OuterXml}</envEvento>");

                    var envEvt = XMLUtility.Deserializar<EnvEvento>(xml.ToString());
                    var evt = envEvt.Evento[0];
                    SignEvent(evt, xmlEl);
                    retornar.Evento.Add(evt);
                }
            }

            return (T)(object)retornar;
        }

        #endregion Public Methods
    }

    [Serializable]
    [XmlType(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class Evento
    {
        #region Public Properties

        [XmlElement("infEvento", Order = 0)]
        public InfEvento InfEvento { get; set; }

        [XmlElement("Signature", Namespace = "http://www.w3.org/2000/09/xmldsig#", Order = 1)]
        public Signature Signature { get; set; }

        [XmlAttribute(AttributeName = "versao", DataType = "token")]
        public string Versao { get; set; }

        #endregion Public Properties
    }

    [Serializable()]
    [XmlType(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public class InfEvento
    {
        #region Private Fields

        private EventoDetalhe _detEvento;

        #endregion Private Fields

        #region Public Properties

        [XmlElement("chNFe", Order = 4)]
        public string ChNFe { get; set; }

        [XmlElement("CNPJ", Order = 2)]
        public string CNPJ { get; set; }

        [XmlIgnore]
        public UFBrasil COrgao { get; set; }

        [XmlElement("cOrgao", Order = 0)]
        public int COrgaoField
        {
            get => (int)COrgao;
            set => COrgao = (UFBrasil)Enum.Parse(typeof(UFBrasil), value.ToString());
        }

        [XmlElement("CPF", Order = 3)]
        public string CPF { get; set; }

        [XmlElement("detEvento", Order = 9)]
        public EventoDetalhe DetEvento
        {
            get => _detEvento;
            set
            {
                switch(TpEvento)
                {
                    case 0:
                        _detEvento = value;
                        break;

                    case TipoEventoNFe.CartaCorrecao:
                        _detEvento = new DetEventoCCE();
                        break;

                    case TipoEventoNFe.Cancelamento:
                        _detEvento = new DetEventoCanc();
                        break;

                    case TipoEventoNFe.ManifestacaoConfirmacaoOperacao:
                    case TipoEventoNFe.ManifestacaoCienciaOperacao:
                    case TipoEventoNFe.ManifestacaoDesconhecimentoOperacao:
                    case TipoEventoNFe.ManifestacaoOperacaoNaoRealizada:
                        _detEvento = new DetEventoManif();
                        break;

                    case TipoEventoNFe.CancelamentoPorSubstituicao:
                        _detEvento = new DetEventoCancSubst();
                        break;

                    case TipoEventoNFe.EPEC:
                        _detEvento = new DetEventoEPEC();
                        break;

                    case TipoEventoNFe.ComprovanteEntregaNFe:
                        _detEvento = new DetEventoCompEntregaNFe();
                        break;

                    case TipoEventoNFe.CancelamentoComprovanteEntregaNFe:
                        _detEvento = new DetEventoCancCompEntregaNFe();
                        break;

                    case TipoEventoNFe.PedidoProrrogacao:
                    default:
                        throw new NotImplementedException($"O tipo de evento '{TpEvento}' não está implementado.");
                }

                _detEvento.XmlReader = value.XmlReader;
                _detEvento.ProcessReader();
            }
        }

        [XmlIgnore]
        public DateTime DhEvento { get; set; }

        [XmlElement("dhEvento", Order = 5)]
        public string DhEventoField
        {
            get => DhEvento.ToString("yyyy-MM-ddTHH:mm:sszzz");
            set => DhEvento = DateTime.Parse(value);
        }

        [XmlAttribute(DataType = "ID")]
        public string Id
        {
            get => "ID" + ((int)TpEvento).ToString() + ChNFe + NSeqEvento.ToString("00");
            set => _ = value;
        }

        [XmlElement("nSeqEvento", Order = 7)]
        public int NSeqEvento { get; set; }

        [XmlElement("tpAmb", Order = 1)]
        public TipoAmbiente TpAmb { get; set; }

        [XmlElement("tpEvento", Order = 6)]
        public TipoEventoNFe TpEvento { get; set; }

        [XmlElement("verEvento", Order = 8)]
        public string VerEvento { get; set; }

        #endregion Public Properties

        #region Public Constructors

        public InfEvento()
        {
        }

        public InfEvento(EventoDetalhe detEvento) => DetEvento = detEvento ?? throw new ArgumentNullException(nameof(detEvento));

        #endregion Public Constructors

        #region Public Methods

        public bool ShouldSerializeCNPJ() => !string.IsNullOrWhiteSpace(CNPJ);

        public bool ShouldSerializeCPF() => !string.IsNullOrWhiteSpace(CPF);

        #endregion Public Methods
    }

    [XmlInclude(typeof(DetEventoCanc))]
    [XmlInclude(typeof(DetEventoCCE))]
    [XmlInclude(typeof(DetEventoCancSubst))]
    public class EventoDetalhe: IXmlSerializable
    {
        #region Internal Properties

        internal XmlReader XmlReader { get; set; }

        private static readonly List<string> hasField = new List<string>
        {
            "COrgaoAutor",
        };

        private static readonly BindingFlags bindingFlags = BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.IgnoreCase;

        #endregion Internal Properties

        #region Internal Methods

        internal virtual void ProcessReader()
        {
            if(XmlReader == null)
            {
                return;
            }

            var type = GetType();

            if(XmlReader.HasAttributes)
            {
                if(XmlReader.GetAttribute("versao") != "")
                {
                    var pi = type.GetProperty("versao", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    pi?.SetValue(this, XmlReader.GetAttribute("versao"));
                }
            }

            while(XmlReader.Read())
            {
                if(XmlReader.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                SetValue(type);
            }
        }

        internal virtual void SetValue(Type type)
        {
            var pi = GetPropertyInfo(type);

            if(pi == null)
            {
                return;
            }

            SetValue(pi);
        }

        internal virtual void SetValue(PropertyInfo pi) =>
            pi?.SetValue(this, Converter.ToAny(XmlReader.GetValue<object>(XmlReader.Name), pi.PropertyType));

        protected internal PropertyInfo GetPropertyInfo(Type type)
        {
            var pi = hasField.Exists(w => w.ToLower() == XmlReader.Name.ToLower()) ?
                                type.GetProperty(XmlReader.Name + "Field", bindingFlags) :
                                type.GetProperty(XmlReader.Name, bindingFlags);
            return pi;
        }

        #endregion Internal Methods

        #region Public Properties

        [XmlElement("descEvento", Order = 0)]
        public virtual string DescEvento { get; set; }

        [XmlAttribute(AttributeName = "versao", DataType = "token")]
        public virtual string Versao { get; set; }

        #endregion Public Properties

        #region Public Methods

        public XmlSchema GetSchema() => default;

        public void ReadXml(XmlReader reader) => XmlReader = reader;

        public virtual void WriteXml(XmlWriter writer) => writer.WriteAttributeString("versao", Versao);

        #endregion Public Methods
    }

    [Serializable]
    [XmlRoot(ElementName = "detEvento")]
    public class DetEventoCanc: EventoDetalhe
    {
        #region Public Properties

        [XmlElement("descEvento", Order = 0)]
        public override string DescEvento { get; set; } = "Cancelamento";

        [XmlElement("nProt", Order = 1)]
        public string NProt { get; set; }

        [XmlElement("xJust", Order = 2)]
        public string XJust { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteRaw($@"
            <descEvento>{DescEvento}</descEvento>
            <nProt>{NProt}</nProt>
            <xJust>{XJust}</xJust>");
        }

        #endregion Public Methods
    }

    [Serializable]
    [XmlRoot(ElementName = "detEvento")]
    public class DetEventoCancSubst: EventoDetalhe
    {
        #region Public Properties

        [XmlElement("descEvento", Order = 0)]
        public override string DescEvento { get; set; } = "Cancelamento por substituicao";

        [XmlIgnore]
        public UFBrasil COrgaoAutor { get; set; }

        [XmlElement("cOrgaoAutor", Order = 1)]
        public string COrgaoAutorField
        {
            get => ((int)COrgaoAutor).ToString();
            set => COrgaoAutor = Converter.ToAny<UFBrasil>(value);
        }

        [XmlElement("tpAutor", Order = 2)]
        public TipoAutor TpAutor { get; set; }

        [XmlElement("verAplic", Order = 3)]
        public string VerAplic { get; set; }

        [XmlElement("nProt", Order = 4)]
        public string NProt { get; set; }

        [XmlElement("xJust", Order = 5)]
        public string XJust { get; set; }

        [XmlElement("chNFeRef", Order = 6)]
        public string ChNFeRef { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteRaw($@"<descEvento>{DescEvento}</descEvento>" +
                $@"<cOrgaoAutor>{(int)COrgaoAutor}</cOrgaoAutor>" +
                $@"<tpAutor>{(int)TpAutor}</tpAutor>" +
                $@"<verAplic>{VerAplic}</verAplic>" +
                $@"<nProt>{NProt}</nProt>" +
                $@"<xJust>{XJust}</xJust>" +
                $@"<chNFeRef>{ChNFeRef}</chNFeRef>");
        }

        #endregion Public Methods
    }

    [Serializable]
    [XmlRoot(ElementName = "detEvento")]
    public class DetEventoCCE: EventoDetalhe
    {
        #region Public Properties

        [XmlElement("descEvento", Order = 0)]
        public override string DescEvento { get; set; } = "Carta de Correcao";

        [XmlElement("xCondUso", Order = 2)]
        public string XCondUso { get; set; } = "A Carta de Correcao e disciplinada pelo paragrafo 1o-A do art. 7o do Convenio S/N, de 15 de dezembro de 1970 e pode ser utilizada para regularizacao de erro ocorrido na emissao de documento fiscal, desde que o erro nao esteja relacionado com: I - as variaveis que determinam o valor do imposto tais como: base de calculo, aliquota, diferenca de preco, quantidade, valor da operacao ou da prestacao; II - a correcao de dados cadastrais que implique mudanca do remetente ou do destinatario; III - a data de emissao ou de saida.";

        [XmlElement("xCorrecao", Order = 1)]
        public string XCorrecao { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteRaw($@"<descEvento>{DescEvento}</descEvento><xCorrecao>{XCorrecao}</xCorrecao><xCondUso>{XCondUso}</xCondUso>");
        }

        #endregion Public Methods
    }

    [Serializable]
    [XmlRoot(ElementName = "detEvento")]
    public class DetEventoManif: EventoDetalhe
    {
        #region Public Properties

        private string DescEventoField;

        /// <summary>
        /// Informe, inclusive obdecendo letras maiúscuas e minúsculas um dos seguintes textos:
        /// </summary>
        [XmlElement("descEvento", Order = 0)]
        public override string DescEvento
        {
            get => DescEventoField;
            set
            {
                if(!value.Equals("Ciencia da Operacao") &&
                    !value.Equals("Confirmacao da Operacao") &&
                    !value.Equals("Desconhecimento da Operacao") &&
                    !value.Equals("Operacao nao Realizada"))
                {
                    throw new Exception("O conteúdo da tag <descEvento> deve ser: Ciencia da Operacao, Confirmacao da Operacao, Desconhecimento da Operacao ou Operacao nao Realizada. O texto deve ficar idêntico ao descrito, inclusive letras maiúsculas e minúsculas.");
                }
                else
                {
                    DescEventoField = value;
                }
            }
        }

        [XmlElement("xJust", Order = 1)]
        public string XJust { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            if(string.IsNullOrWhiteSpace(XJust) ||
                DescEvento.Equals("Ciencia da Operacao") ||
                DescEvento.Equals("Confirmacao da Operacao"))
            {
                writer.WriteRaw($@"<descEvento>{DescEvento}</descEvento>");
            }
            else
            {
                writer.WriteRaw($@"<descEvento>{DescEvento}</descEvento><xJust>{XJust}</xJust>");
            }
        }

        #endregion Public Methods
    }

    [Serializable]
    [XmlRoot(ElementName = "detEvento")]
    public class DetEventoEPEC: EventoDetalhe
    {
        internal override void SetValue(PropertyInfo pi)
        {
            if(pi.Name == nameof(Dest))
            {
                XmlReader.Read();
                Dest = new DetEventoEPECDest();
                Dest.UF = XmlReader.GetValue<UFBrasil>(nameof(Dest.UF));
                Dest.CNPJ = XmlReader.GetValue<string>(nameof(Dest.CNPJ));
                Dest.CPF = XmlReader.GetValue<string>(nameof(Dest.CPF));
                Dest.IdEstrangeiro = XmlReader.GetValue<string>(nameof(Dest.IdEstrangeiro));
                Dest.IE = XmlReader.GetValue<string>(nameof(Dest.IE));
                Dest.VNF = XmlReader.GetValue<double>(nameof(Dest.VNF));
                Dest.VICMS = XmlReader.GetValue<double>(nameof(Dest.VICMS));
                Dest.VST = XmlReader.GetValue<double>(nameof(Dest.VST));
                return;
            }

            base.SetValue(pi);
        }

        [XmlElement("descEvento", Order = 0)]
        public override string DescEvento { get; set; } = "EPEC";

        [XmlIgnore]
        public UFBrasil COrgaoAutor { get; set; }

        [XmlElement("cOrgaoAutor", Order = 1)]
        public int COrgaoAutorField
        {
            get => (int)COrgaoAutor;
            set => COrgaoAutor = (UFBrasil)Enum.Parse(typeof(UFBrasil), value.ToString());
        }

        [XmlElement("tpAutor", Order = 2)]
        public TipoAutor TpAutor { get; set; }

        [XmlElement("verAplic", Order = 3)]
        public string VerAplic { get; set; }

        [XmlIgnore]
        public DateTime DhEmi { get; set; }

        [XmlElement("dhEmi", Order = 4)]
        public string DhEmiField
        {
            get => DhEmi.ToString("yyyy-MM-ddTHH:mm:sszzz");
            set => DhEmi = DateTime.Parse(value);
        }

        [XmlElement("tpNF", Order = 5)]
        public TipoOperacao TpNF { get; set; }

        [XmlElement("IE", Order = 6)]
        public string IE { get; set; }

        [XmlElement("dest", Order = 7)]
        public DetEventoEPECDest Dest { get; set; }

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            var linha = $@"<descEvento>{DescEvento}</descEvento>
                       <cOrgaoAutor>{COrgaoAutorField}</cOrgaoAutor>
                       <tpAutor>{(int)TpAutor}</tpAutor>
                       <verAplic>{VerAplic}</verAplic>
                       <dhEmi>{DhEmiField}</dhEmi>
                       <tpNF>{(int)TpNF}</tpNF>
                       <IE>{IE}</IE>";

            linha += $@"<dest>";

            linha += $@"<UF>{Dest.UF}</UF>";
            if(!string.IsNullOrWhiteSpace(Dest.CNPJ))
            {
                linha += $@"<CNPJ>{Dest.CNPJ}</CNPJ>";
            }
            if(!string.IsNullOrWhiteSpace(Dest.CPF))
            {
                linha += $@"<CPF>{Dest.CPF}</CPF>";
            }
            if(!string.IsNullOrWhiteSpace(Dest.IdEstrangeiro))
            {
                linha += $@"<idEstrangeiro>{Dest.IdEstrangeiro}</idEstrangeiro>";
            }
            if(!string.IsNullOrWhiteSpace(Dest.IE))
            {
                linha += $@"<IE>{Dest.IE}</IE>";
            }
            linha += $@"<vNF>{Dest.VNFField}</vNF>";
            linha += $@"<vICMS>{Dest.VICMSField}</vICMS>";
            linha += $@"<vST>{Dest.VSTField}</vST>";

            linha += $@"</dest>";

            writer.WriteRaw(linha);
        }
    }

    [Serializable]
    [XmlRoot(ElementName = "dest")]
    public class DetEventoEPECDest
    {
        [XmlElement("UF", Order = 0)]
        public UFBrasil UF { get; set; }

        [XmlElement("CNPJ", Order = 1)]
        public string CNPJ { get; set; }

        [XmlElement("CPF", Order = 1)]
        public string CPF { get; set; }

        [XmlElement("idEstrangeiro", Order = 2)]
        public string IdEstrangeiro { get; set; }

        [XmlElement("IE", Order = 3)]
        public string IE { get; set; }

        [XmlIgnore]
        public double VNF { get; set; }

        [XmlElement("vNF", Order = 4)]
        public string VNFField
        {
            get => VNF.ToString("F2", CultureInfo.InvariantCulture);
            set => VNF = Converter.ToDouble(value);
        }

        [XmlIgnore]
        public double VICMS { get; set; }

        [XmlElement("vICMS", Order = 5)]
        public string VICMSField
        {
            get => VICMS.ToString("F2", CultureInfo.InvariantCulture);
            set => VICMS = Converter.ToDouble(value);
        }

        [XmlIgnore]
        public double VST { get; set; }

        [XmlElement("vST", Order = 6)]
        public string VSTField
        {
            get => VST.ToString("F2", CultureInfo.InvariantCulture);
            set => VST = Converter.ToDouble(value);
        }

        #region ShouldSerialize

        public bool ShouldSerializeCNPJ() => !string.IsNullOrWhiteSpace(CNPJ);

        public bool ShouldSerializeCPF() => !string.IsNullOrWhiteSpace(CPF);

        public bool ShouldSerializeIdEstrangeiro() => !string.IsNullOrWhiteSpace(IdEstrangeiro);

        public bool ShouldSerializeIE() => !string.IsNullOrWhiteSpace(IE);

        #endregion ShouldSerialize
    }

    [Serializable]
    [XmlRoot(ElementName = "detEvento")]
    public class DetEventoCompEntregaNFe: EventoDetalhe
    {
        #region Public Properties

        [XmlElement("descEvento", Order = 0)]
        public override string DescEvento { get; set; } = "Comprovante de Entrega da NF-e";

        [XmlIgnore]
        public UFBrasil COrgaoAutor { get; set; }

        [XmlElement("cOrgaoAutor", Order = 1)]
        public int COrgaoAutorField
        {
            get => (int)COrgaoAutor;
            set => COrgaoAutor = (UFBrasil)Enum.Parse(typeof(UFBrasil), value.ToString());
        }

        [XmlIgnore]
        public TipoAutor TpAutor { get; set; }

        [XmlElement("tpAutor", Order = 2)]
        public int TpAutorField
        {
            get => (int)TpAutor;
            set
            {
                if(value != (int)TipoAutor.EmpresaEmitente)
                {
                    throw new Exception("Conteúdo da TAG <tpAutor> inválido. Valor aceito 1-Empresa emitente.");
                }

                TpAutor = (TipoAutor)Enum.Parse(typeof(TipoAutor), value.ToString());
            }
        }

        [XmlElement("verAplic", Order = 3)]
        public string VerAplic { get; set; }

        [XmlIgnore]
        public DateTime DhEntrega { get; set; }

        [XmlElement("dhEntrega", Order = 4)]
        public string DhEntregaField
        {
            get => DhEntrega.ToString("yyyy-MM-ddTHH:mm:sszzz");
            set => DhEntrega = DateTime.Parse(value);
        }

        [XmlElement("nDoc", Order = 5)]
        public string NDoc { get; set; }

        [XmlElement("xNome", Order = 6)]
        public string XNome { get; set; }

        [XmlElement("latGPS", Order = 7)]
        public string LatGPS { get; set; }

        [XmlElement("longGPS", Order = 8)]
        public string LongGPS { get; set; }

        [XmlElement("hashComprovante", Order = 9)]
        public string HashComprovante { get; set; }

        [XmlIgnore]
        public DateTime DhHashComprovante { get; set; }

        [XmlElement("dhHashComprovante", Order = 10)]
        public string DhHashComprovanteField
        {
            get => DhHashComprovante.ToString("yyyy-MM-ddTHH:mm:sszzz");
            set => DhHashComprovante = DateTime.Parse(value);
        }

        #endregion Public Properties

        #region Public Methods

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteRaw($@"
            <descEvento>{DescEvento}</descEvento>
            <cOrgaoAutor>{COrgaoAutorField}</cOrgaoAutor>
            <tpAutor>{TpAutorField}</tpAutor>
            <verAplic>{VerAplic}</verAplic>
            <dhEntrega>{DhEntregaField}</dhEntrega>
            <nDoc>{NDoc}</nDoc>
            <xNome>{XNome}</xNome>
            <latGPS>{LatGPS}</latGPS>
            <longGPS>{LongGPS}</longGPS>
            <hashComprovante>{HashComprovante}</hashComprovante>
            <dhHashComprovante>{DhHashComprovanteField}</dhHashComprovante>");
        }

        #endregion Public Methods
    }

    [Serializable]
    [XmlRoot(ElementName = "detEvento")]
    public class DetEventoCancCompEntregaNFe: EventoDetalhe
    {
        #region Public Properties

        [XmlElement("descEvento", Order = 0)]
        public override string DescEvento { get; set; } = "Cancelamento Comprovante de Entrega da NF-e";

        [XmlIgnore]
        public UFBrasil COrgaoAutor { get; set; }

        [XmlElement("cOrgaoAutor", Order = 1)]
        public int COrgaoAutorField
        {
            get => (int)COrgaoAutor;
            set => COrgaoAutor = (UFBrasil)Enum.Parse(typeof(UFBrasil), value.ToString());
        }

        [XmlIgnore]
        public TipoAutor TpAutor { get; set; }

        [XmlElement("tpAutor", Order = 2)]
        public int TpAutorField
        {
            get => (int)TpAutor;
            set
            {
                if(value != (int)TipoAutor.EmpresaEmitente)
                {
                    throw new Exception("Conteúdo da TAG <tpAutor> inválido. Valor aceito 1-Empresa emitente.");
                }

                TpAutor = (TipoAutor)Enum.Parse(typeof(TipoAutor), value.ToString());
            }
        }

        [XmlElement("verAplic", Order = 3)]
        public string VerAplic { get; set; }

        [XmlElement("nProtEvento", Order = 4)]
        public string NProtEvento { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override void WriteXml(XmlWriter writer)
        {
            base.WriteXml(writer);

            writer.WriteRaw($@"
            <descEvento>{DescEvento}</descEvento>
            <cOrgaoAutor>{COrgaoAutorField}</cOrgaoAutor>
            <tpAutor>{TpAutorField}</tpAutor>
            <verAplic>{VerAplic}</verAplic>
            <nProtEvento>{NProtEvento}</nProtEvento>");
        }

        #endregion Public Methods
    }
}