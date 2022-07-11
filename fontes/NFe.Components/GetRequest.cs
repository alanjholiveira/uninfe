using NFe.Components.Abstract;
using NFe.Components.Interface;
using NFe.Components.SIGISSWEB;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;

namespace NFe.Components
{
    public class GetRequest : RequestBase, IGetRequest
    {
        /// <summary>
        /// Faz o Get e retorna uma string como resultado
        /// </summary>
        /// <param name="url">url base para utilizar dentro do get</param>
        /// <returns></returns>
        public string GetForm(string url)
        {
            string result = string.Empty;
            string postParameter = string.Empty;

            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;

            if (Proxy != null)
            {
                request.UseDefaultCredentials = false;
                request.Proxy = Proxy;
                request.Proxy.Credentials = Proxy.Credentials;
                request.Credentials = Proxy.Credentials;
            }

            //ajustar para permitir o cabeçalho HTTP/1.0
            SetAllowUnsafeHeaderParsing20();

            //evitar o erro "The remote server returned an error: (417) Expectation Failed."
            //para cabeçalhos HTTP/1.0
            ServicePointManager.Expect100Continue = false;

            WebResponse response = default(WebResponse);
            bool success = true;

            try
            {
                response = request.GetResponse();
            }
            catch (WebException webEx)
            {
                response = webEx.Response;
                success = false;
            }

            var streamDados = response.GetResponseStream();
            StreamReader reader = new StreamReader(streamDados);
            result = reader.ReadToEnd();
            streamDados.Close();
            response.Close();
            response.Dispose();

            if (!success &&
                result.StartsWith("\n"))
                result = result.Substring(1);

            return result;
        }

        /// <summary>
        /// Faz o Get e retorna uma string como resultado
        /// </summary>
        /// <param name="url">url base para utilizar dentro do get</param>
        /// <returns></returns>
        public string Get(string url)
        {
            string result = string.Empty;
            string postParameter = string.Empty;

            var request = WebRequest.CreateHttp(url);
            request.Headers.Add("AUTHORIZATION", Variavel.login_sistema);
            request.Method = "GET";
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;

            if (Proxy != null)
            {
                request.UseDefaultCredentials = false;
                request.Proxy = Proxy;
                request.Proxy.Credentials = Proxy.Credentials;
                request.Credentials = Proxy.Credentials;
            }

            //ajustar para permitir o cabeçalho HTTP/1.0
            SetAllowUnsafeHeaderParsing20();

            //evitar o erro "The remote server returned an error: (417) Expectation Failed."
            //para cabeçalhos HTTP/1.0
            ServicePointManager.Expect100Continue = false;

            WebResponse response = default(WebResponse);
            bool success = true;

            try
            {
                response = request.GetResponse();
            }
            catch (WebException webEx)
            {
                response = webEx.Response;
                success = false;
            }

            var streamDados = response.GetResponseStream();
            StreamReader reader = new StreamReader(streamDados);
            result = reader.ReadToEnd();
            streamDados.Close();
            response.Close();
            response.Dispose();

            if (!success &&
                result.StartsWith("\n"))
                result = result.Substring(1);

            return result;
        }

        /// <summary>
        /// Faz o post e retorna uma string  com o resultado
        /// </summary>
        /// <param name="url">url base para utilizar dentro do post</param>
        /// <param name="postData">dados a serem enviados junto com o post</param>
        /// <returns></returns>
        public string GetAuto(string url, IDictionary<string, string> postData = null, IList<string> headers = null)
        {
            var result = string.Empty;
            var postParameter = string.Empty;
            var xmlFile = "";

            if (postData.Keys.Contains("f1"))
            {
                xmlFile = postData["f1"];
                var doc = new XmlDocument();
                doc.Load(xmlFile);
            }

            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;

            foreach (var header in headers)
            {
                request.Headers.Add(header);
            }

            if (Proxy != null)
            {
                request.UseDefaultCredentials = false;
                request.Proxy = Proxy;
                request.Proxy.Credentials = Proxy.Credentials;
                request.Credentials = Proxy.Credentials;
            }

            //ajustar para permitir o cabeçalho HTTP/1.0
            SetAllowUnsafeHeaderParsing20();

            //evitar o erro "The remote server returned an error: (417) Expectation Failed."
            //para cabeçalhos HTTP/1.0
            ServicePointManager.Expect100Continue = false;

            var response = default(WebResponse);
            var success = true;

            try
            {
                response = request.GetResponse();
            }
            catch (WebException webEx)
            {
                response = webEx.Response;
                success = false;
            }

            var streamDados = response.GetResponseStream();
            StreamReader reader = new StreamReader(streamDados);
            result = reader.ReadToEnd();
            streamDados.Close();
            response.Close();
            response.Dispose();

            if (!success &&
                result.StartsWith("\n"))
                result = result.Substring(1);

            return result;      
        }

    }
}