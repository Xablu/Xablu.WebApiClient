using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xablu.WebApiClient.Resolvers;

namespace Sample.Resolvers
{
    public class ContentResolver : SimpleJsonContentResolver
    {
        public ContentResolver(JsonSerializer serializer) : base(serializer)
        {
        }

        public override HttpContent ResolveHttpContent<TContent>(TContent content)
        {
            return base.ResolveHttpContent<TContent>(content);
        }
    }
}