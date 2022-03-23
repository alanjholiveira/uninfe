using NFe.Components;
using NFe.Components.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace NFSe.Components
{
    namespace Excpetions
    {
        /// <summary>
        /// Serviço não disponível para o padrão IPM
        /// </summary>
        public class ServicoInexistenteIPMException : NFe.Components.Exceptions.ServicoInexistenteException
        {
            #region Public Properties

            public override string Message => "Serviço não disponível para padrão IPM";

            #endregion Public Properties
        }
    }

    /// <summary>
    /// Emite notas fiscais de serviço no padrão IPM
    /// </summary>
    public class IPM : EmiteNFSeBase, IEmiteNFSeIPM
    {
        #region Private Methods

        private string EnviaXML(string file)
        {
            var result = "";

            using (var post = new POSTRequest
            {
                Proxy = Proxy
            })
            {
                #region Timbo-SC
                if (Cidade == 8357 || Cidade == 4218202) //Produção
                {
                    var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Usuario}:{Senha}"));

                    // informe 1 para retorno em xml
                    result = post.PostForm("https://timbo.atende.net/atende.php?pg=rest&service=WNERestServiceNFSe&cidade=padrao", new Dictionary<string, string>
                    {
                        {"f1", file}           //Endereço físico do arquivo
                    }, $"Authorization: Basic {base64}");
                }
                #endregion Timbo-SC

                #region Cascavel-PR
                else if ((Cidade == 74934 || Cidade == 4104808) && tpAmb == TipoAmbiente.taProducao) //Produção
                {
                    var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Usuario}:{Senha}"));

                    // informe 1 para retorno em xml
                    result = post.PostForm("https://ws-cascavel.atende.net:7443/atende.php?pg=rest&service=WNERestServiceNFSe&cidade=padrao", new Dictionary<string, string>
                    {
                        {"f1", file}           //Endereço físico do arquivo
                    }, $"Authorization: Basic {base64}");
                }
                else if ((Cidade == 74934 || Cidade == 4104808) && tpAmb == TipoAmbiente.taHomologacao) //Homologação
                {
                    var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Usuario}:{Senha}"));

                    // informe 1 para retorno em xml
                    result = post.PostForm("https://treinamento.atende.net/atende.php?pg=rest&service=WNERestServiceNFSe&cidade=migra_cascavel", new Dictionary<string, string>
                    {
                        {"f1", file}           //Endereço físico do arquivo
                    }, $"Authorization: Basic {base64}");
                }
                #endregion Cascavel-PR

                #region Guarapuava-PR
                else if (Cidade == 7583 || Cidade == 4109401) //Produção
                {
                    var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Usuario}:{Senha}"));

                    // informe 1 para retorno em xml
                    result = post.PostForm("https://guarapuava.atende.net/atende.php?pg=rest&service=WNERestServiceNFSe&cidade=padrao", new Dictionary<string, string>
                    {
                        {"f1", file}           //Endereço físico do arquivo
                    }, $"Authorization: Basic {base64}");
                }
                #endregion Guarapuava-PR

                #region Pinhais-PR
                else if ((Cidade == 5453 || Cidade == 4119152) && tpAmb == TipoAmbiente.taProducao) //Produção
                {
                    var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Usuario}:{Senha}"));

                    // informe 1 para retorno em xml
                    result = post.PostForm("https://ws-pinhais.atende.net:7443/atende.php?pg=rest&service=WNERestServiceNFSe&cidade=padrao", new Dictionary<string, string>
                    {
                        {"f1", file}           //Endereço físico do arquivo
                    }, $"Authorization: Basic {base64}");
                }
                else if ((Cidade == 5453 || Cidade == 4119152) && tpAmb == TipoAmbiente.taHomologacao) //Homologação
                {
                    var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Usuario}:{Senha}"));

                    // informe 1 para retorno em xml
                    result = post.PostForm("https://treinamento.atende.net/atende.php?pg=rest&service=WNERestServiceNFSe&cidade=migra_pinhais", new Dictionary<string, string>
                    {
                        {"f1", file}           //Endereço físico do arquivo
                    }, $"Authorization: Basic {base64}");
                }
                #endregion Pinhais-PR

                #region Rio do Sul-SC
                else if (Cidade == 8291 || Cidade == 4214805) //Produção
                {
                    var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Usuario}:{Senha}"));

                    // informe 1 para retorno em xml
                    result = post.PostForm("https://riodosul.atende.net/atende.php?pg=rest&service=WNERestServiceNFSe&cidade=padrao", new Dictionary<string, string>
                    {
                        { "f1", file}           //Endereço físico do arquivo
                    }, $"Authorization: Basic {base64}");
                }
                #endregion Rio do Sul-SC

                #region Apucarana-PR
                else if ((Cidade == 7425 || Cidade == 4101408) && tpAmb == TipoAmbiente.taProducao) //Produção
                {
                    var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Usuario}:{Senha}"));

                    // informe 1 para retorno em xml
                    result = post.PostForm("https://apucarana.atende.net/atende.php?pg=rest&service=WNERestServiceNFSe&cidade=padrao", new Dictionary<string, string>
                    {
                        {"f1", file}           //Endereço físico do arquivo
                    }, $"Authorization: Basic {base64}");
                }
                #endregion Apucarana-PR

                #region Palhoça-SC

                else if ((Cidade == 8233 || Cidade == 4211900) && tpAmb == TipoAmbiente.taProducao) //Produção
                {
                    var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Usuario}:{Senha}"));

                    // informe 1 para retorno em xml
                    result = post.PostForm("https://ws-palhoca.atende.net:7443/atende.php?pg=rest&service=WNERestServiceNFSe&cidade=padrao", new Dictionary<string, string>
                    {
                        {"f1", file}           //Endereço físico do arquivo
                    }, $"Authorization: Basic {base64}");
                }

                #endregion Palhoça-SC

                else
                {
                    // informe 1 para retorno em xml
                    result = post.PostForm("http://sync.nfs-e.net/datacenter/include/nfw/importa_nfw/nfw_import_upload.php?eletron=1", PreparePostData(file));
                }
            }

            return result;
        }

        private Dictionary<string, string> PreparePostData(string file) => new Dictionary<string, string>
        {
            {"login", Usuario  },  //CPF/CNPJ, sem separadores}
            {"senha", Senha},      //Senha de acesso ao sistema: www.nfse.
            {"cidade", Cidade.ToString()},   //Código da cidade na receita federal (TOM), pesquisei o código em http://www.ekwbrasil.com.br/municipio.php3.
            {"f1", file}           //Endereço físico do arquivo
        };

        #endregion Private Methods

        #region Public Properties

        public override string NameSpaces => throw new NotImplementedException();

        #endregion Public Properties

        #region Public Constructors

        public IPM(TipoAmbiente tpAmb, string pastaRetorno, string usuario, string senha, int cidade)
            : base(tpAmb, pastaRetorno)
        {
            Usuario = usuario;
            Senha = senha;
            Cidade = CodigoTom(cidade);
            PastaRetorno = pastaRetorno;
        }

        #endregion Public Constructors

        #region Public Methods

        public override void CancelarNfse(string file)
        {
            var result = EnviaXML(file);

            GerarRetorno(file, result, Propriedade.Extensao(Propriedade.TipoEnvio.PedCanNFSe).EnvioXML,
                Propriedade.Extensao(Propriedade.TipoEnvio.PedCanNFSe).RetornoXML);
        }

        public int CodigoTom(int nCodIbge)
        {
            switch (nCodIbge)
            {
                case 4104303: // Campo mourão - PR
                    return 7483;

                case 4309209: // Gravataí - RS
                    return 8683;

                case 4104204: // Campo Largo - PR
                    return 7481;

                case 4118204: // Paranaguá - PR
                    return 7745;

                case 4217808: // Taió - SC
                    return 8351;

                case 4201307: // Araquari - SC
                    return 8025;

                case 4215802: // São Bento do Sul-SC
                    return 8311;

                case 4307807: // Estrela-RS
                    return 8653;

                case 4211900: // Palhoça-SC
                    return 8233;

                case 4317202: // Santa Rosa-RS
                    return 8847;

                case 4202909: // Brusque-SC
                    return 8055;

                case 4302105: // Bento Gonçalves-RS
                    return 8041;

                case 4207502: // Indaial-SC
                    return 8147;

                case 4211801: // Ouro-SC
                    return 8231;

                case 4119152: // Pinhais-PR
                    return 5453;

                case 4127205: // Terra Boa - PR

                case 4313508: // Osório-RS
                    return 8773;

                case 4118006: //Paraíso do Norte - PR
                    return 7741;

                case 4300604: //Alvorada-RS
                    return 8511;

                case 4104907: //Castro-PR
                    return 7495;

                case 4104808: //Cascavel-PR
                    return 74934;

                case 4303103: //Cachoeirinha-SC
                    return 85618;

                case 4114609: //Marechal Cândido Rondon-PR
                    return 7683;

                case 4213203: //Pomerode-SC
                    return 8259;

                case 4213500: //Porto Belo-SC
                    return 8265;

                case 4215000: //Rio Negrinho-SC
                    return 8295;

                case 4109401: //Guarapuava-PR
                    return 7583;

                case 4218202: //Timbó-SC
                    return 8357;

                case 4214805: //Rio do Sul-SC
                    return 8291;

                case 4101408: //Apucarana-PR
                    return 7425;
            }

            return 0;
        }

        public override void ConsultarLoteRps(string file)
        { }

        public override void ConsultarNfse(string file)
        {
            var result = EnviaXML(file);

            var doc = new XmlDocument();
            doc.LoadXml(result);

            if (!doc.InnerText.Contains("XSD Error") && !doc.InnerText.Contains("00206 - Nenhuma NFSe foi encontrada"))
            {
                doc.DocumentElement.RemoveChild(doc.GetElementsByTagName("codigo_html")[0]);
            }

            result = doc.OuterXml;

            GerarRetorno(file, result, Propriedade.Extensao(Propriedade.TipoEnvio.PedSitNFSe).EnvioXML,
                Propriedade.Extensao(Propriedade.TipoEnvio.PedSitNFSe).RetornoXML);
        }

        public override void ConsultarNfsePorRps(string file)
        {
            var result = EnviaXML(file);

            GerarRetorno(file, result, Propriedade.Extensao(Propriedade.TipoEnvio.PedSitNFSeRps).EnvioXML,
                Propriedade.Extensao(Propriedade.TipoEnvio.PedSitNFSeRps).RetornoXML);
        }

        public override void ConsultarSituacaoLoteRps(string file)
        { }

        public override void EmiteNF(string file)
        {
            var result = EnviaXML(file);

            GerarRetorno(file, result, Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).EnvioXML,
                Propriedade.Extensao(Propriedade.TipoEnvio.EnvLoteRps).RetornoXML);
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