using Xablu.WebApiClient.Abstractions.HttpExtensions;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Xablu.WebApiClient.Abstractions.Resolvers
{
    public class FormUrlEncodedContentResolver
        : IHttpContentResolver
    {
        public virtual HttpContent ResolveHttpContent<TContent>(TContent content)
        {
            var nameValueCollection = content as IEnumerable<KeyValuePair<string, string>>;

            if (nameValueCollection == null)
                throw new ArgumentException(
                    "Content parameter is of the wrong type. The parameter should derive from 'IEnumerable<KeyValuePair<string, string>>'.",
                    nameof(content));

            return new LargeFormUrlEncodedContent(nameValueCollection);
        }
    }
}