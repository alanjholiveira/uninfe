using NFe.Components.Abstract;
using NFSe.Components;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace NFe.Components.GIAP
{
    public class SaoCarlosSP : EmiteNFSeBase
    {
        #region Public Properties

        public override string NameSpaces
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private string versao = "";
        private string servico = "";      

        public string URLAPIBase
        {
            get
            {
                if (tpAmb.Equals(TipoAmbiente.taHomologacao))
                    return $@"http://webservice.giap.com.br/WSNfsesScarlos02/nfseresources/ws/{versao}{servico}/simula";
                else
                    return $@"http://webservice.giap.com.br/WSNfsesScarlos02/nfseresources/ws/{versao}{servico}";                        
            }
        }

        #endregion Public Properties

        #region Public Construstor

        public SaoCarlosSP(TipoAmbiente tpAmb, string pastaRetorno, string senha)
            : base(tpAmb, pastaRetorno)
        {
            Senha = senha;
        }

        #endregion Public Construstor

        #region Public Methods

        public override void EmiteNF(string file)
        {
            string result = "";
            versao = "v2";
            servico = "/emissao";

            using (POSTRequest post = new POSTRequest
            {
                Proxy = Proxy
            })
            {
                IList<string> autorizations = new List<string>()
                {
                    $"Authorization: {Senha}"
                };

                result = post.PostForm(Path.Combine(URLAPIBase),
                    new Dictionary<string, string>
                    {
                        { "f1", file}
                    },
                    autorizations);
            }

            GerarRetorno(file, result, Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).EnvioXML,
                                           Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).RetornoXML);
        }

        public override void CancelarNfse(string file)
        {
            string result = "";
            versao = "v2";
            servico = "/cancela";
            

            using (POSTRequest post = new POSTRequest
            {
                Proxy = Proxy
            })
            {
                IList<string> autorizations = new List<string>()
                {
                    $"Authorization: {Senha}"
                };

                result = post.PostForm(Path.Combine(URLAPIBase),
                    new Dictionary<string, string>
                    {
                        { "f1", file}
                    },
                    autorizations);
            }

            GerarRetorno(file, result, Propriedade.Extensao(Propriedade.TipoEnvio.PedCanNFSe).EnvioXML,
                                       Propriedade.Extensao(Propriedade.TipoEnvio.PedCanNFSe).RetornoXML);
        }

        public override void ConsultarNfse(string file)
        {
            string result = "";
            servico = "consulta";

            using (POSTRequest post = new POSTRequest
            {
                Proxy = Proxy
            })
            {
                IList<string> autorizations = new List<string>();

                result = post.PostForm(Path.Combine(URLAPIBase),
                    new Dictionary<string, string>
                    {
                        { "f1", file}
                    },
                    autorizations);
            }

            GerarRetorno(file, result, Propriedade.Extensao(Propriedade.TipoEnvio.PedSitNFSe).EnvioXML,
                                   Propriedade.Extensao(Propriedade.TipoEnvio.PedSitNFSe).RetornoXML);
        }

        public override void ConsultarLoteRps(string file)
        {
            throw new Exception();
        }

        public override void ConsultarNfsePorRps(string file)
        {
            throw new Exception();
        }

        public override void ConsultarSituacaoLoteRps(string file)
        {
            throw new Exception();
        }

        public override void GerarRetorno(string file, string result, string extEnvio, string extRetorno)
        {
            var fi = new FileInfo(file);
            var nomearq = PastaRetorno + "\\" + fi.Name.Replace(extEnvio, extRetorno);

            var iso = Encoding.GetEncoding("ISO-8859-1");
            var write = new StreamWriter(nomearq, true, iso);
            write.Write(result);
            write.Flush();
            write.Close();
            write.Dispose();
        }

        #endregion Public Methods
    }
}