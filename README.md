# Xablu.WebApiClient
The Xablu WebApiClient is a C# HTTP library which aims to simplify consuming of Web API services in .NET projects.

### Setup & Usage
* Available on NuGet: https://www.nuget.org/packages/Xablu.WebApiClient/
* Install into each project that utilizes the WebApiClient

### Build Status: 
[![Build status](https://ci.appveyor.com/api/projects/status/5ey0sq4fn01t9o56?svg=true
)](https://ci.appveyor.com/project/Xablu/xablu-webapiclient)
![GitHub tag](https://img.shields.io/github/tag/Xablu/Xablu.WebApiClient.svg)
[![NuGet](https://img.shields.io/nuget/v/Xablu.WebApiClient.svg?label=NuGet)](https://www.nuget.org/packages/Xablu.WebApiClient/)
[![MyGet](https://img.shields.io/myget/xabluhq/v/Xablu.WebApiClient.svg)](https://www.myget.org/F/Xablu.WebApiClient/api/v2)

# Usage

### Standard

Create a new singleton of WebApiClient and use it to do REST operations.
```c#
var restApiClient = new RestApiClient("http://baseurl.com/");
```

### MvvmCross

If you want to use the standard HTTP client handlers, just register `IRestApiClient` with a base url. You can set the type of native handler in your csproj settings.

```c#
Mvx.RegisterSingleton<IRestApiClient>(new RestApiClient(Constants.ApiBaseUrl));
```

When using custom HTTP handler, register a custom iOS Handler

```c#
var options = new RestApiClientOptions(Constants.ApiBaseUrl) 
{
    DefaultHttpMessageHandler = () => new NSUrlSessionHandler();
};
Mvx.RegisterSingleton<IRestApiClient>(new RestApiClient(options));
```

Register the custom handler in Android

```c#
var options = new RestApiClientOptions(Constants.ApiBaseUrl) 
{
    DefaultHttpMessageHandler = () => new AndroidClientHandler();
};
Mvx.RegisterSingleton<IRestApiClient>(new RestApiClient(options));
```

Create a client to handle Http traffic

```c#
public class AuthenticationClient : IAuthenticationClient
{
   public AuthenticationClient(IRestApiClient apiClient) : base(apiClient)
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

```c#
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

