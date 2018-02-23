# Xablu.WebApiClient
The Xablu WebApiClient is a C# HTTP library which aims to simplify consuming of Web API services in .NET projects.

## Setup
The Xablu.WebApiClient is written according to the Xamarin Plugin philosophy. Meaning you can simply add the Xablu.WebApiClient package through 
[NuGet](https://www.nuget.org/packages/Xablu.WebApiClient/Install). Install the NuGet package into your shared .NET Standard project and ALL Client projects.

* NuGet: [Xablu.WebApiClient](https://www.nuget.org/packages/Xablu.WebApiClient/Install)
* `PM> Install-Package Xablu.WebApiClient`
* Namespace: using Xablu.WebApiClient

## Build Status: 
[![Build status](https://ci.appveyor.com/api/projects/status/5ey0sq4fn01t9o56?svg=true
)](https://ci.appveyor.com/project/Xablu/xablu-webapiclient)
![GitHub tag](https://img.shields.io/github/tag/Xablu/Xablu.WebApiClient.svg)
[![NuGet](https://img.shields.io/nuget/v/Xablu.WebApiClient.svg?label=NuGet)](https://www.nuget.org/packages/Xablu.WebApiClient/)
[![MyGet](https://img.shields.io/myget/xabluhq/v/Xablu.WebApiClient.svg)](https://www.myget.org/F/Xablu.WebApiClient/api/v2)

## Usage

Before using the Xablu.WebApiClient plugin (via `CrossRestApiClient.Current`) make sure you configure the Xablu.WebApiClient plugin with at least the base address of the API you want to connect to.
Failing to do so will result in a `NotConfiguredException` when using the plugin.

> Note: it is no longer needed to supply a platform specific `HttpMessageHandler`, this is now build in into the plugin. This means that the configuration (as shown below) can all be done in 
> your shared code.

Configuration is done through the `CrossRestApiClient.Configure` method, like this:

```
CrossRestApiClient.Configure((opt) => opt.ApiBaseAddress = "https://api.mybackend.com");
```

Once you have configured the Xablu.WebApiClient plugin you can start using it through the static `CrossRestApiClient.Current` property, like this:

```
var client = CrossRestApiClient.Current;
var result = await client.GetAsync<IEnumerable<TodoItem>>(Priority.UserInitiated, "mypath").ConfigureAwait(false);

if(!result.IsSuccessStatusCode)
{
  // Handle error response...
}

return result.Content;
```