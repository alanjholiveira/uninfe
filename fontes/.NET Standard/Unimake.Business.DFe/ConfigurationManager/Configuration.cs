﻿using Unimake.Business.DFe.ConfigurationManager.Contract;

namespace Unimake.Business.DFe.ConfigurationManager
{
    public class Configuration : IConfiguration
    {
        #region Public Properties

        #region Configurações

        public string ArquivoConfigPadrao => "Config.xml";
        public string ArquivoConfigGeral => PastaArqConfig + ArquivoConfigPadrao;

        public virtual string PastaArqConfig => @"Servicos\Config\";

        public virtual string PastaArqConfigNFCe => PastaArqConfig + Servicos.TipoDFe.NFCe.ToString() + @"\";
        public virtual string PastaArqConfigNFe => PastaArqConfig + Servicos.TipoDFe.NFe.ToString() + @"\";
        public virtual string PastaArqConfigCTe => PastaArqConfig + Servicos.TipoDFe.CTe.ToString() + @"\";
        public virtual string PastaArqConfigCTeOS => PastaArqConfig + Servicos.TipoDFe.CTeOS.ToString() + @"\";
        public virtual string PastaArqConfigMDFe => PastaArqConfig + Servicos.TipoDFe.MDFe.ToString() + @"\";

        #endregion

        #region Schemas

        public virtual string PastaSchema => @"Xml\Schemas\";

        public virtual string PastaSchemaNFe => PastaSchema + Servicos.TipoDFe.NFe.ToString() + @"\";
        public virtual string PastaSchemaNFCe => PastaSchemaNFe;
        public virtual string PastaSchemaCTe => PastaSchema + Servicos.TipoDFe.CTe.ToString() + @"\";
        public virtual string PastaSchemaCTeOS => PastaSchemaCTe;
        public virtual string PastaSchemaMDFe => PastaSchema + Servicos.TipoDFe.MDFe.ToString() + @"\";

        #endregion

        #endregion Public Properties
    }
}