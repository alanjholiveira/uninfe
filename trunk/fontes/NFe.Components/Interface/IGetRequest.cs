using System.Collections.Generic;

namespace NFe.Components.Interface
{
    public interface IGetRequest : IRequest
    {
        /// <summary>
        /// Faz o Get e retorna uma string como resultado
        /// </summary>
        /// <param name="url">url base para utilizar dentro do get</param>
        /// <returns></returns>
        string GetForm(string url);

        /// <summary>
        /// Faz o Get e retorna uma string como resultado
        /// </summary>
        /// <param name="url">url base para utilizar dentro do get</param>
        /// <returns></returns>
        string Get(string url);

        /// <summary>
        /// Faz o Get e retorna uma string como resultado
        /// </summary>
        /// <param name="url">url base para utilizar dentro do get</param>
        /// <param name="postData">dados a serem enviados junto com o post</param>
        /// <returns></returns>
        string GetAuto(string url, IDictionary<string, string> postData = null, IList<string> headers = null);
    }
}