using NFe.Components.Abstract;
using NFSe.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace NFe.Components.SIGISSWEB
{
    public class SIGISSWEB : EmiteNFSeBase
    {
        #region Public Properties

        public override string NameSpaces
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string URLAPIBase
        {
            get
            {
                if (tpAmb.Equals(TipoAmbiente.taHomologacao))
                    return $@"https://wshml.sigissweb.com/rest/";
                else
                    return $@"https://wsleme.sigissweb.com/rest/";
            }
        }

        #endregion Public Properties

        #region Public Construstor

        public SIGISSWEB(TipoAmbiente tpAmb, string pastaRetorno, string usuario, string senha)
            : base(tpAmb, pastaRetorno)
        {
            Usuario = usuario;
            Senha = senha;
        }

        #endregion Public Construstor

        #region Public Methods

        public override void EmiteNF(string file)
        {
            string result = "";

            GeraToken();

            using (POSTRequest post = new POSTRequest
            {
                Proxy = Proxy
            })
            {
                IList<string> autorizations = new List<string>()
                {
                    $"Authorization: {Variavel.login_sistema}"
                };

                result = post.PostForm(Path.Combine(URLAPIBase, "nfes"),
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

            GeraToken();

            string numeronf = "";
            string serie = "";
            string motivo = "";

            XmlDocument xmlCancela = new XmlDocument();
            xmlCancela.Load(file);

            XmlNode root = xmlCancela?.GetElementsByTagName("CancelarNfseEnvio")[0];
            if (root.HasChildNodes)
            {
                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    if(i == 0)
                    {
                        numeronf = root.ChildNodes[i].InnerText;
                    }
                    if (i == 1)
                    {
                        serie = root.ChildNodes[i].InnerText;
                    }
                    if (i == 2)
                    {
                        motivo = root.ChildNodes[i].InnerText;
                    }
                }
            }

            using (GetRequest get = new GetRequest
            {
                Proxy = Proxy
            })
            {
                result = get.Get($"{Path.Combine(URLAPIBase, $"nfes/cancela/{numeronf}/serie/{serie}/motivo/{motivo}")}");
            }

            GerarRetorno(file, result, Propriedade.Extensao(Propriedade.TipoEnvio.PedCanNFSe).EnvioXML,
                                       Propriedade.Extensao(Propriedade.TipoEnvio.PedCanNFSe).RetornoXML);
        }

        public override void ConsultarNfsePDF(string file)
        {
            string result = "";

            GeraToken();

            string numeronf = "";
            string serie = "";

            XmlDocument xmlConsPdf = new XmlDocument();
            xmlConsPdf.Load(file);

            XmlNode root = xmlConsPdf?.GetElementsByTagName("ConsultarNotaPdfEnvio")[0];
            if (root.HasChildNodes)
            {
                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    if (i == 0)
                    {
                        numeronf = root.ChildNodes[i].InnerText;
                    }
                    if (i == 1)
                    {
                        serie = root.ChildNodes[i].InnerText;
                    }
                }
            }

            using (GetRequest get = new GetRequest
            {
                Proxy = Proxy
            })
            {
                result = get.Get($"{Path.Combine(URLAPIBase, $"nfes/nfimpressa/{numeronf}/serie/{serie}")}");
            }

            GerarRetorno(file, result, Propriedade.Extensao(Propriedade.TipoEnvio.PedNFSePDF).EnvioXML,
                                       Propriedade.Extensao(Propriedade.TipoEnvio.PedNFSePDF).RetornoXML);
        }

        public override void GerarRetorno(string file, string result, string extEnvio, string extRetorno)
        {
            FileInfo fi = new FileInfo(file);
            string nomearq = PastaRetorno + "\\" + fi.Name.Replace(extEnvio, extRetorno);

            Encoding iso = Encoding.GetEncoding("UTF-8");
            StreamWriter write = new StreamWriter(nomearq, false, iso);
            write.Write(result);
            write.Flush();
            write.Close();
            write.Dispose();
        }

        private void GeraToken()
        {
            string json = "{ \"login\": \"" + Usuario + "\",\"senha\":\"" + Senha + "\"}";

            System.Net.HttpWebRequest webrequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://wshml.sigissweb.com/rest/login");        
            webrequest.Method = "POST";
            webrequest.ContentType = "application/json; charset=utf-8";

            using (var streamWriter = new StreamWriter(webrequest.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
            }

            Stream stream = webrequest.GetRequestStream();

            using (System.Net.WebResponse response = webrequest.GetResponse())
            {

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    //VARIAVEL GLOBAL
                    Variavel.login_sistema = reader.ReadToEnd();
                }

                stream.Close();
            }
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

        public override void ConsultarNfse(string file)
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}