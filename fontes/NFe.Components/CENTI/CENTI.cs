using System;

namespace NFe.Components.CENTI
{
    public class CENTI : CentiBase
    {
        public override string NameSpaces => throw new NotImplementedException();

        #region Construtures
        public CENTI(TipoAmbiente tpAmb, string pastaRetorno, int codMun, string usuario, string senha)
            : base(tpAmb, pastaRetorno, codMun, senha, usuario)
        { }
        #endregion
    }
}
