![xablu logo ](/Assets/xablu_logo.png "Xablu")

# Xablu.WebApiClient   


The Xablu WebApiClient is a C# HTTP library which aims to simplify consuming of REST / GraphQL API services in .NET projects.<br/> 

> :construction: :warning: *Already using this library*?<br/> 
> *We have been working on a 2nd version of WebApiClient which is based on Refit and will support GraphQL. This version has many new features. Beware, upgrading from version 1 to version 2 has some breaking changes since it’s not backwards compatible.* 

[![Build Status](https://xablu.visualstudio.com/WebApiClient/_apis/build/status/Xablu.Xablu.WebApiClient?branchName=develop)](https://xablu.visualstudio.com/WebApiClient/_build/latest?definitionId=1&branchName=develop)
![Github tag](https://img.shields.io/github/tag/Xablu/Xablu.WebApiClient.svg)
[![NuGet](https://img.shields.io/nuget/v/Xablu.WebApiClient.svg?label=NuGet)](https://www.nuget.org/packages/Xablu.WebApiClient/)

## Table of contents  
1. [How Xablu.WebApiClient Works](#howto)<br/>
2. [Download / Install](#downloadinstal)<br/>
3. [Key Features](#features)<br/>
4. [Example ](#examples)<br/>
5. [Contributions](#contributions)<br/>
6. [Feedback](#feedback)

## How Xablu.WebApiClient Works <a name="howto"></a>
WebApiClient is an open source library, created and maintained by [Xablu](https://www.xablu.com/). It is currently available for .NET / Xamarin. Through experience, we discovered that any .NET client app that has resilient calls to web services, uses a combination of libraries. Therefore, we built: 

* A REST client that is flexible and has no limitations. 
* A GraphQL client that is powerful and includes a query builder.

![webapiclient model](/Assets/model.png "WebApiClient Model")

## Technical detail
We have taken the time to update this package with all kind of new features including different libraries. Including these libraries come with new technologies 
that you might not know about. Because of this reason we have made a summary below about each of the technologie explaining their use-case.

### Refit: 
[Refit](https://github.com/reactiveui/refit) is the backbone for the REST client. It allows making HTTP/S requests to external services. Any app that uses Refit does not require much effort to start using our code. All the features provided by Refit are also exposed by our library without limitation. 

### Fussilade: 
[Fusillade](https://github.com/reactiveui/Fusillade) is an HttpClient implementation which allows an app to efficiently schedule / create requests with different priorities. As a user you have the ability to set these priorities when you make a request. 

### Polly: 
[Polly](https://github.com/reactiveui/Fusillade) is a very flexible resilience and transient-fault-handling library that allow apps to react to certain situations through policies like retry and timeout. This package provides a very simple way of using all common available features. Every HTTP call made has Polly implemented and the user has the option to customize this. 

### GraphQL.Client: 
[GraphQL.Client](https://github.com/graphql-dotnet/graphql-client) is the base of our GraphQL implementation, which also includes all additions  from Fussilade and Polly as well. The coolest thing? We built a query builder that translates your common response models into a query (and it also gives you the results back as C# objects).   

## Download / Install <a name="downloadinstal"></a>
The Xablu.WebApiClient is written following the multi-target library approach. Meaning you can simply add the Xablu.WebApiClient package through NuGet. Install the NuGet package into your shared .NET Standard project and ALL Client projects. 

* NuGet: Xablu.WebApiClient
* PM> Install-Package Xablu.WebApiClient
* Namespace: using Xablu.WebApiClient

## Key Features <a name="features"></a>

The WebApiClient contains new features with respect to the previous version. The list of key features is depicted below: 

REST Client:
  * Based on Refit                                          ✔
  * Implemented Retry and Timeout from Polly                ✔
  * Implemented Fusillade’s Priorities                      ✔

GraphQL Client: 
  * Based on GraphQL.Client                                 ✔
  * Implemented Retry and Timeout from Polly                ✔
  * Implemented Fusillade’s Priorities                      ✔
  * Implemented a Custom, Object-Oriented Query Builder     ✔

## Example <a name="examples"></a>

Make sure to check out the [Samples](https://github.com/Xablu/Xablu.WebApiClient/tree/develop/src/Samples) in this repository. Instructions on how to run the BooksQL sample app can be found [here](https://github.com/Xablu/Xablu.WebApiClient/wiki/3.-Mobile-Sample). Here is also an example call for connecting with a Web API service through Refit:

Abstract:
```c#
Task<TResult> Call<TResult>(Func<T, Task<TResult>> operation, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout); 
```
Example implementation:
```c#
async Task<IEnumerable<MyModel>> GetModelsItemsAsync(bool forceRefresh = false) 
{
  IWebApiClient<IRefitInterface> webApiClient = WebApiClientFactory.Get<IRefitInterface>("baseURL", defaultHeaders: true);
  var jsonresult = await webApiClient.Call(
      operation: myRefitService => myRefitService.GetData(),
      priority: Priority.UserInitiated,
      retryCount: 2,
      shouldRetry: exception => myShouldRetryCondition(exception),
      timeout: 60); 
}
```
Down here is an example call for connecting with a web API service through GraphQL:

Abstract:
```c#
public static IWebApiClient<T> Get<T>(string baseUrl, bool autoRedirectRequests = true, Func<DelegatingHandler> delegatingHandler = default, IDictionary<string, string> defaultHeaders = default) where T : class
```
Implementation:
```c#
async Task GraphqlAsync()
{
  var defaultHeaders = new Dictionary<string, string>
  {
    ["User-Agent"] = "ExampleUser",
    ["Authorization"] = "Bearer ******"
  };
  IWebApiClient<IGitHubApi> webApiClient = WebApiClientFactory.Get<IGitHubApi>("https://api.github.com", false, default, defaultHeaders);
  var requestForSingleUser = new Request<UserResponseModel>("(login: \"ExampleUser\")");
  await webApiClient.SendQueryAsync(requestForSingleUser);
}
```

# Contributions <a name="contributions"></a>
All contributions are welcome! If you have a problem, please open an issue. And PRs are also appreciated! 

# Feedback <a name="feedback"></a>
Are you using this library? We would love to hear from you!
Or do you have any questions or suggestions?
You are welcome to discuss it on:

[<img src="/Assets/github.png">](https://github.com/Xablu)
[<img src="/Assets/twitter.png">](https://twitter.com/xabluhq)
