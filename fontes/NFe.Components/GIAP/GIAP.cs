using System;

namespace NFe.Components.GIAP
{
    public class Giap : GiapBase
    {
        public override string NameSpaces => throw new NotImplementedException();

        #region Construtures
        public Giap(TipoAmbiente tpAmb, string pastaRetorno, int codMun, string senha)
            : base(tpAmb, pastaRetorno, codMun, senha)
        { }
        #endregion
    }
}
