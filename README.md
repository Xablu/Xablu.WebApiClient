# Xablu.WebApiClient
The Xablu WebApiClient is a C# HTTP library which aims to simplify consuming of Web API services in .NET projects.

# Usage

Register iOS Handler in MvvmCross

```
	Mvx.RegisterSingleton<IWebApiClient>(new WebApiClient(Settings.ApiBaseUrl, () => new NSUrlSessionHandler()));
```

Register Android Handler in MvvmCross
```
	Mvx.RegisterSingleton<IWebApiClient>(new WebApiClient(Settings.ApiBaseUrl, () => new AndroidClientHandler()));
```

Create a client to handle Http traffic
```
	public class AuthenticationClient : BaseClient, IAuthenticationClient
	{
		public AuthenticationClient(IWebApiClient apiClient) : base(apiClient)
		{
		}

		public Task<int> AuthenticateWithCredentials(string username, string password)
		{
			var uri = new UriTemplate("user/logon");

			var content = new MultipartFormDataContent();
			content.Add(new StringContent(username), "username");
			content.Add(new StringContent(password), "password");

			return ExecuteRemoteRequest(async () => await apiClient.PostAsync<MultipartFormDataContent, int>(Priority.UserInitiated, uri.Resolve(), content).ConfigureAwait(false));
		}
	}
  ```
  
  Add a Service to change and handle user logic
  ```
	public class AuthenticationService : IAuthenticationService
	{
		private IBlobCache cache;
		private IAuthenticationClient authClient;

		public AuthenticationService(IAuthenticationClient authClient, IBlobCache cache = null)
			: base()
		{
			this.cache = (cache ?? BlobCache.Secure);
			this.authClient = authClient;
		}

		public async Task<bool> Login(string username, string password)
		{
			var userId = await authClient.AuthenticateWithCredentials(username, password).ConfigureAwait(false);

			if (userId != default(int))
			{
				await this.cache.InsertObject(Settings.UserIdCacheKey, userId);
				return true;
			}

			return false;
		}

		public async Task<bool> IsAuthenticated()
		{
			try
			{
				var userId = await cache.GetObject<int>(Settings.UserIdCacheKey).ToTask().ConfigureAwait(false);
				return true;
			}
			catch (KeyNotFoundException)
			{
				return false;
			}
		}

		public async Task Logout()
		{
			await cache.InvalidateAll().ToTask().ConfigureAwait(false);
		}
	}
	
	```
