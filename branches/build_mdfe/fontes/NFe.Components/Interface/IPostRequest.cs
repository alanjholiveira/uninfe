using System.Collections.Generic;

namespace NFe.Components.Interface
{
    public interface IPostRequest : IRequest
    {
        #region Public Methods

        /// <summary>
        /// Faz o post e retorna uma string  com o resultado
        /// </summary>
        /// <param name="url">url base para utilizar dentro do post</param>
        /// <param name="postData">dados a serem enviados junto com o post</param>
        /// <returns></returns>
        string PostForm(string url, IDictionary<string, string> postData);

        /// <summary>
        /// Faz o post e retorna uma string  com o resultado
        /// </summary>
        /// <param name="url">url base para utilizar dentro do post</param>
        /// <param name="postData">dados a serem enviados junto com o post</param>
        /// <param name="authorization">Informar a string de autorização. Será informada como header na requisição</param>
        /// <returns></returns>
        string PostForm(string url, IDictionary<string, string> postData, string authorization);

        /// <summary>
        /// Faz o post e retorna uma string  com o resultado
        /// </summary>
        /// <param name="url">url base para utilizar dentro do post</param>
        /// <param name="postData">dados a serem enviados junto com o post</param>
        /// <returns></returns>
        string PostForm(string url, IDictionary<string, string> postData = null, IList<string> headers = null);

        #endregion Public Methods
    }
}