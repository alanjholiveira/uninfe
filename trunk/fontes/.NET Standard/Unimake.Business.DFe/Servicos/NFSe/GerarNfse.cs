﻿using System.Xml;

namespace Unimake.Business.DFe.Servicos.NFSe
{
    /// <summary>
    /// Enviar o XML de NFSe para o webservice
    /// </summary>
    public class GerarNfse: ServicoBase
    {
        /// <summary>
        /// Construtor
        /// </summary>
        public GerarNfse() : base()
        { }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="conteudoXML">Conteúdo do XML que será enviado para o WebService</param>
        /// <param name="configuracao">Objeto "Configuracoes" com as propriedade necessária para a execução do serviço</param>
        public GerarNfse(XmlDocument conteudoXML, Configuracao configuracao) : base(conteudoXML, configuracao)
        { }

        /// <summary>
        /// Definir o valor de algumas das propriedades do objeto "Configuracoes"
        /// </summary>
        protected override void DefinirConfiguracao()
        {
            if(!Configuracoes.Definida)
            {
                base.DefinirConfiguracao();
            }
        }

        /// <summary>
        /// Executar o serviço
        /// </summary>
        public override void Executar()
        {
            if(!Configuracoes.Definida)
            {
                DefinirConfiguracao();
            }

            base.Executar();
        }
    }
}