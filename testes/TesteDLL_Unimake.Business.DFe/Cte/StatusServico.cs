using Unimake.Business.DFe.Servicos;
using Unimake.Business.DFe.Xml.CTe;

namespace Cte
{
    internal class StatusServico
    {
        private ConsStatServCte xml;
        private Configuracao configuracao;

        public StatusServico(ConsStatServCte xml, Configuracao configuracao)
        {
            this.xml = xml;
            this.configuracao = configuracao;
        }
    }
}