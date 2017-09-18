using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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