using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace TestTagIntegridade
{
    internal class Program
    {
        #region Private Fields

        private const string Hash = "61aec2215401d0099d85d70a56d72949860ca07c55620c37b49f8f2da7cf9a671afac6c96d95bd74f9304b97cebc6a90cdf9f7134b2a5f41a12629f7d6111ba1";
        private const string HashPHP = "bf9f1f4014f59a6fe13bf733aab62bf860a655666ce8d7beee2502e4eb510929958891569967103924d04b5dc6fff5bd3f40abe3507dcdd60c12aa3920f98142";

        #endregion Private Fields

        #region Private Methods

        private static string GerarTagIntegridade(string token, string rps)
        {
            var tag = rps;
            tag = Regex.Replace(tag, "[^\x20-\x7E]+", "");
            tag = Regex.Replace(tag, "[ ]+", "");
            var result = GerarRSASHA512(tag + token, true);
            return result;
        }

        private static void Main(string[] args)
        {
            //Peguei este exemplo de hash em PHP para garantir que minha função está correta.
            //https://www.tools4noobs.com/online_php_functions/sha512/
            var stringPHP = "O rato roeu a roupa do rei de roma.";
            var hashPHP = GerarRSASHA512(stringPHP, true);
            Debug.Assert(hashPHP == HashPHP);

            //Agora vamos gerar a tag integridade
            var token = "TLXX4JN38KXTRNSEAJYYEA==";

            var xml = new XmlDocument();
            xml.Load("rps.xml");

            var rps = xml.GetElementsByTagName("Rps")[0].OuterXml;
            var tagIntegridade = GerarTagIntegridade(token, rps);
            Debug.Assert(tagIntegridade == Hash);

            Console.ReadKey();
        }

        #endregion Private Methods

        #region Public Methods

        public static string GerarRSASHA512(string value, bool lower = false)
        {
            var sha512 = SHA512.Create();
            var bytes = Encoding.UTF8.GetBytes(value);
            var hash = sha512.ComputeHash(bytes);

            var result = new StringBuilder();
            for(var i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString($"{(lower ? "x" : "X")}2"));
            }

            return result.ToString();
        }

        #endregion Public Methods
    }
}