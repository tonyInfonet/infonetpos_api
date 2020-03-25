using System;
using System.Text;

namespace Infonet.CStoreCommander.BusinessLayer.Utilities
{
    /// <summary>
    /// Class for Token Validation
    /// </summary>
    public static class TokenValidator
    {
        private const int ExpirationHours = 24;

        /// <summary>
        /// Checks whether token is valid
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsValidToken(string token)
        {
            var result = false;
            try
            {
                // Base64 decode the string, obtaining the token:username:timeStamp.
                var key = Encoding.UTF8.GetString(Convert.FromBase64String(token));

                // Split the parts.
                var parts = key.Split(':');
                if (parts.Length == 3)
                {
                    // Get the hash message, username, and timestamp.
                    var username = parts[0];
                    var ticks = long.Parse(parts[1]);
                    var posId = int.Parse(parts[2]);
                    var timeStamp = new DateTime(ticks);

                    // Ensure the timestamp is valid.
                    var expired = Math.Abs((DateTime.UtcNow - timeStamp).TotalHours) > ExpirationHours;
                    if (!expired)
                    {
                        // Hash the message with the key to generate a token.
                        var computedToken = TokenGenerator.GenerateToken(username, ticks, posId);

                        // Compare the computed token with the one supplied and ensure they match.
                        result = token == computedToken;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return result;
        }


        public static string GetUserCode(string token)
        {
            var result = string.Empty;
            try
            {
                // Base64 decode the string, obtaining the token:username:timeStamp.
                var key = Encoding.UTF8.GetString(Convert.FromBase64String(token));

                // Split the parts.
                var parts = key.Split(':');
                if (parts.Length > 1)
                {
                    // Get the hash message, username, and timestamp.
                    var username = parts[0];
                    result = username;
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return result;
        }


        public static int GetPosId(string token)
        {
            int result = 0;
            try
            {
                // Base64 decode the string, obtaining the token:username:timeStamp.
                var key = Encoding.UTF8.GetString(Convert.FromBase64String(token));

                // Split the parts.
                var parts = key.Split(':');
                if (parts.Length > 1)
                {
                    // Get the hash message, username, and timestamp.
                    var posid = parts[2];
                    result = Convert.ToInt32(posid);
                }
            }
            catch (Exception)
            {
                // ignored
            }
            return result;
        }
    }
}