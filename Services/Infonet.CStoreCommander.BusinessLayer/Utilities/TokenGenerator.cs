using System;
using System.Text;

namespace Infonet.CStoreCommander.BusinessLayer.Utilities
{
    /// <summary>
    /// Class for Generating a Token
    /// </summary>
    public static class TokenGenerator
    {
        /// <summary>
        /// Generated Token
        /// </summary>
        /// <param name="username"></param>
        /// <param name="ticks"></param>
        /// <param name="posId"></param>
        /// <returns></returns>
        public static string GenerateToken(string username, long ticks, int posId)
        {
            string hash = string.Join(":", new string[] { username.ToUpper(),  ticks.ToString(), posId.ToString() });
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(hash));
        }
    }
}