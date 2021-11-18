using NFe.Components.Abstract;
using NFe.Components.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml;

namespace NFSe.Components
{
    /// <summary>
    /// Esta classe utiliza métodos POST para fazer requisições
    /// </summary>
    public class POSTRequest : RequestBase, IPostRequest
    {
        #region Public Methods

        public string Post(string usuario, string senha, string URLAPIBase, string file)
        {
            var result = new HttpResponseMessage();
            var cliente = new HttpClient();

            var body = new
            {
                usuario,
                senha,
                xml = File.ReadAllText(file)
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(body);

            result = cliente.PostAsync(URLAPIBase, new StringContent(json, Encoding.UTF8, "application/json")).Result;

            return result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// Faz o post e retorna uma string  com o resultado
        /// </summary>
        /// <param name="url">url base para utilizar dentro do post</param>
        /// <param name="postData">dados a serem enviados junto com o post</param>
        /// <param name="authorization">Informar a string de autorização. Será informada como header na requisição</param>
        /// <returns></returns>
        public string PostForm(string url, IDictionary<string, string> postData, string authorization)
        {
            var boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            var file = postData["f1"];

            #region Preparar a requisição

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;

            if (!string.IsNullOrWhiteSpace(authorization))
            {
                request.Headers.Add(authorization);
            }

            //ajustar para permitir o cabeçalho HTTP/1.0
            SetAllowUnsafeHeaderParsing20();

            //evitar o erro "The remote server returned an error: (417) Expectation Failed."
            //para caeçalhos HTTP/1.0
            ServicePointManager.Expect100Continue = false;

            if (Proxy != null)
            {
                request.UseDefaultCredentials = false;
                request.Proxy = Proxy;
                request.Proxy.Credentials = Proxy.Credentials;
                request.Credentials = Proxy.Credentials;
            }

            #endregion Preparar a requisição

            #region Crar o stream da solicitação

            Stream memStream = new MemoryStream();

            var boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            var formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

            foreach (var keyValue in postData)
            {
                var formitem = string.Format(formdataTemplate, keyValue.Key, keyValue.Value);
                var formitembytes = Encoding.UTF8.GetBytes(formitem);
                memStream.Write(formitembytes, 0, formitembytes.Length);
            }

            memStream.Write(boundarybytes, 0, boundarybytes.Length);

            var headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n Content-Type: application/octet-stream\r\n\r\n";

            var header = string.Format(headerTemplate, "f1", file);

            var headerbytes = Encoding.UTF8.GetBytes(header);

            memStream.Write(headerbytes, 0, headerbytes.Length);

            var fileStream = new FileStream(file, FileMode.Open,
            FileAccess.Read);
            var buffer = new byte[1024];

            var bytesRead = 0;

            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                memStream.Write(buffer, 0, bytesRead);
            }

            memStream.Write(boundarybytes, 0, boundarybytes.Length);
            fileStream.Close();

            request.ContentLength = memStream.Length;

            #endregion Crar o stream da solicitação

            #region Escrever na requisição

            var requestStream = request.GetRequestStream();

            memStream.Position = 0;
            var tempBuffer = new byte[memStream.Length];
            memStream.Read(tempBuffer, 0, tempBuffer.Length);
            memStream.Close();
            requestStream.Write(tempBuffer, 0, tempBuffer.Length);
            requestStream.Close();

            #endregion Escrever na requisição

            #region Resposta do servidor

            var response = request.GetResponse();

            var stream = response.GetResponseStream();
            var reader = response.ContentType.IndexOf("charset=") == -1 ?
                new StreamReader(stream, Encoding.UTF8) :
                new StreamReader(stream, Encoding.GetEncoding(response.ContentType.Substring(response.ContentType.IndexOf("charset=") + 8)));

            var result = reader.ReadToEnd();
            stream.Dispose();
            reader.Dispose();
            return result;

            #endregion Resposta do servidor
        }

        /// <summary>
        /// Faz o post e retorna uma string  com o resultado
        /// </summary>
        /// <param name="url">url base para utilizar dentro do post</param>
        /// <param name="postData">dados a serem enviados junto com o post</param>
        /// <returns></returns>
        public string PostForm(string url, IDictionary<string, string> postData)
        {
            return PostForm(url, postData, (string)null);
        }

        /// <summary>
        /// Faz o post e retorna uma string  com o resultado
        /// </summary>
        /// <param name="url">url base para utilizar dentro do post</param>
        /// <param name="postData">dados a serem enviados junto com o post</param>
        /// <returns></returns>
        public string PostForm(string url, IDictionary<string, string> postData = null, IList<string> headers = null)
        {
            var result = string.Empty;
            var postParameter = string.Empty;
            var xmlFile = "";

            foreach (var keyValue in postData.Where(w => w.Key != "f1"))
            {
                postParameter += $"&{keyValue.Key}={keyValue.Value}";
            }

            if (postParameter.Length > 1)
            {
                postParameter = postParameter?.Substring(1);
                url += $"?{postParameter}";
            }
            string accept = null;
            var contentType = accept;

            if (postData.Keys.Contains("f1"))
            {
                xmlFile = postData["f1"];
                var doc = new XmlDocument();
                doc.Load(xmlFile);
                xmlFile = doc.InnerXml;
                contentType = "application/xml";
                accept = "application/xml";
            }

            var encode = Encoding.UTF8.GetBytes(xmlFile);
            var request = WebRequest.CreateHttp(url);
            request.Method = "POST";
            request.ContentType = contentType;
            request.ContentLength = encode.Length;
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Accept = accept;

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

            if (encode.Length > 0)
            {
                var stream = request.GetRequestStream();
                stream.Write(encode, 0, encode.Length);
                stream.Close();
            }

            var response = default(WebResponse);
            var success = true;

            try
            {
                response = request?.GetResponse();
            }
            catch (WebException webEx)
            {
                response = webEx.Response;
                success = false;
            }

            var streamDados = response.GetResponseStream();
            var reader = new StreamReader(streamDados);
            result = reader.ReadToEnd();
            streamDados.Close();
            response.Close();
            response.Dispose();

            if (!success &&
                result.StartsWith("\n"))
            {
                result = result.Substring(1);
            }

            return result;
        }

        #endregion Public Methods
    }
}