using NFe.Components.Abstract;

namespace NFe.Components.CENTI
{
    public abstract class CentiBase : EmiteNFSeBase
    {
        #region locais/ protegidos
        private readonly int CodigoMun = 0;
        string SenhaWs = "";
        string UsuarioWs = "";
       
        EmiteNFSeBase centiService;

        protected EmiteNFSeBase CentiService
        {
            get
            {
                if (centiService == null)
                {
                    switch (CodigoMun)
                    {
                        case 5207402: //Edéia-GO
                            centiService = new EdeiaGO(tpAmb, PastaRetorno, SenhaWs, UsuarioWs);
                            break;

                        case 5220603: //Silvânia-GO
                            centiService = new SilvaniaGO(tpAmb, PastaRetorno, SenhaWs, UsuarioWs);
                            break;

                        default:
                            throw new Exceptions.ServicoInexistenteException();
                    }
                }

                return centiService;
            }
        }

        #endregion locais/ protegidos

        #region Construtores

        public CentiBase(TipoAmbiente tpAmb, string pastaRetorno, int codMun, string usuario, string senha)
            : base(tpAmb, pastaRetorno)
        {
            CodigoMun = codMun;
            SenhaWs = senha;
            UsuarioWs = usuario; 
        }

        #endregion Construtores

        #region Métodos

        public override void EmiteNF(string file)
        {
            CentiService.EmiteNF(file);
        }

        public override void CancelarNfse(string file)
        {
            CentiService.CancelarNfse(file);
        }

        public override void ConsultarLoteRps(string file)
        {
            CentiService.ConsultarLoteRps(file);
        }

        public override void ConsultarSituacaoLoteRps(string file)
        {
            CentiService.ConsultarSituacaoLoteRps(file);
        }

        public override void ConsultarNfse(string file)
        {
            CentiService.ConsultarNfse(file);
        }

        public override void ConsultarNfsePorRps(string file)
        {
            CentiService.ConsultarNfsePorRps(file);
        }

        #endregion Métodos
    }
}