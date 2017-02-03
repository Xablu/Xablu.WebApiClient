using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Xablu.WebApiClient.Resolvers
{
	internal class DefaultHttpResponseResolver
		: IHttpResponseResolver
	{
		public async Task<TResult> ResolveHttpResponseAsync<TResult>(HttpResponseMessage responseMessage)
		{
			if (!responseMessage.IsSuccessStatusCode)
			{
				return default(TResult);
			}

			var responseAsString = await responseMessage.Content.ReadAsStringAsync();

			var result = JObject.Parse(responseAsString);
			return JsonConvert.DeserializeObject<TResult>((string)result.SelectToken("Result"));
		}
	}
}

