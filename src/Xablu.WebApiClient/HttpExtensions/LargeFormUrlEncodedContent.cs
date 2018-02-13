using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Xablu.WebApiClient.HttpExtensions
{
    /// <summary>
    /// A replacement implementation of the <see cref="FormUrlEncodedContent"/> to handle large data requests.
    /// </summary>
    /// <seealso cref="System.Net.Http.ByteArrayContent" />
    public class LargeFormUrlEncodedContent : ByteArrayContent
    {
        const string mediaContentType = "application/x-www-form-urlencoded";

        /// <summary>
        /// Initializes a new instance of the <see cref="LargeFormUrlEncodedContent"/> class.
        /// </summary>
        /// <param name="nameValueCollection">The name value collection.</param>
        public LargeFormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
            : base(GetContentByteArray(nameValueCollection))
        {
            Headers.ContentType = new MediaTypeHeaderValue(mediaContentType);
        }

        /// <summary>
        /// Gets the content byte array in <see cref="mediaContentType"/> format.
        /// </summary>
        /// <param name="nameValueCollection">The name value collection.</param>
        /// <returns>Content data array in <see cref="mediaContentType"/> format.</returns>
        /// <exception cref="System.ArgumentNullException">nameValueCollection</exception>
        static byte[] GetContentByteArray(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            if (nameValueCollection == null)
            {
                throw new ArgumentNullException(nameof(nameValueCollection));
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> current in nameValueCollection)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append('&');
                }

                stringBuilder.Append(Encode(current.Key));
                stringBuilder.Append('=');
                stringBuilder.Append(Encode(current.Value));
            }
            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }

        /// <summary>
        /// Encodes the specified data for <see cref="mediaContentType"/>.
        /// </summary>
        /// <param name="data">The data to encode.</param>
        /// <returns>Encoded data string.</returns>
        private static string Encode(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            return System.Net.WebUtility.UrlEncode(data).Replace("%20", "+");
        }
    }
}