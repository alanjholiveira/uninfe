using NFe.Components.Abstract;

namespace NFe.Components.GIAP
{
    public abstract class GiapBase : EmiteNFSeBase
    {
        #region locais/ protegidos
        private readonly int CodigoMun = 0;
        string SenhaWs = "";
       
        EmiteNFSeBase giapService;

        protected EmiteNFSeBase GiapService
        {
            get
            {
                if (giapService == null)
                {
                    switch (CodigoMun)
                    {
                        case 3507605: //Bragança Paulista - SP
                            giapService = new BragancaPaulistaSP(tpAmb, PastaRetorno, SenhaWs);
                            break;

                        case 3548906: //São Carlos - SP
                            giapService = new SaoCarlosSP(tpAmb, PastaRetorno, SenhaWs);
                            break;


                        default:
                            throw new Exceptions.ServicoInexistenteException();
                    }
                }

                return giapService;
            }
        }

        #endregion locais/ protegidos

        #region Construtores

        public GiapBase(TipoAmbiente tpAmb, string pastaRetorno, int codMun, string senha)
            : base(tpAmb, pastaRetorno)
        {
            CodigoMun = codMun;
            SenhaWs = senha;
        }

        #endregion Construtores

        #region Métodos

        public override void EmiteNF(string file)
        {
            GiapService.EmiteNF(file);
        }

        public override void CancelarNfse(string file)
        {
            GiapService.CancelarNfse(file);
        }

        public override void ConsultarLoteRps(string file)
        {
            GiapService.ConsultarLoteRps(file);
        }

        public override void ConsultarSituacaoLoteRps(string file)
        {
            GiapService.ConsultarSituacaoLoteRps(file);
        }

        public override void ConsultarNfse(string file)
        {
            GiapService.ConsultarNfse(file);
        }

        public override void ConsultarNfsePorRps(string file)
        {
            GiapService.ConsultarNfsePorRps(file);
        }

        #endregion Métodos
    }
}