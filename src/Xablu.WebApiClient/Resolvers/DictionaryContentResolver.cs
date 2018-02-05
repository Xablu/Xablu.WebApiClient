using System.Net.Http;
using System.Collections.Generic;

namespace Xablu.WebApiClient.Resolvers
{
    public class DictionaryContentResolver
    {
        public virtual HttpContent ResolveHttpContent(Dictionary<string, string> content)
        {
            return new FormUrlEncodedContent(content);
        }
    }
}