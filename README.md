# Xablu.WebApiClient   ![xablu logo ](/Assets/xablu_logo.png "Xablu")


The Xablu WebApiClient is a C# HTTP library which aims to simplify consuming of Web API services in .NET projects.<br/> 

> :construction: :warning: Already using this library? 
> We have been working on a 2nd version of WebApiClient which is based on Refit and will support GraphQL. This version has may new features. Beware, upgrading from version 1 to version 2 has some breaking changes since it’s not backwards compatible. 

## Table of contents  
1.[How Xablu.WebApiClient Works](#howto)<br/>
2.[Download / Install](#downloadinstal)<br/>
3.[Build Status](#buildstatus)<br/> 
4.[Key Features](#features)<br/>
5.[Example ](#examples)<br/>
6.[Contributions](#contributions)<br/>
7.[Feedback](#feedback)

## How Xablu.WebApiClient Works <a name="howto"></a>
WebApiClient is an open source library, created and maintained by Xablu. It is currently available for .NET / Xamarin. Through experience, we discovered that any .NET client app that has resilient calls to web services, uses a combination of libraries. Therefore, we built: 

* A REST client library that combines the most popular libraries like Refit, Fussilade and Poly into a single package. 
* A library that makes it easier to work with GraphQL APIs.

![webapiclient model](/Assets/model.png "WebApiClient Model")

## Technical detail
We have taken the time to update this package with all kind of new features including different libraries. Including these libraries come with new technologies 
that you might not know about. Because of this reason we have made a summary below about each of the technologie explaining their use-case.

### Refit: 
Refit is the backbone that allows making HTTP/S requests to external services. Any app that uses Refit does not require much effort to start using our code. All the features provided by Refit are also exposed by our library without limitation. 

### Fussilade: 
Fusillade is an HttpClient implementation which allows an app to efficiently schedule / create requests with different priorities. As a user you have the ability to set these priorities when you make a request. 

### Polly: 
Polly is a very flexible resilience and transient-fault-handling library that allow apps to react to certain situations through policies like retry and timeout. This package provides a very simple way of using all common available features. Every HTTP call made has Polly implemented and the user has the option to customize this. 

### GraphQL: 
GraphQL intergration has a dependancy towards the GraphQL.Client, and also includes all additions which come from Fussilade and Polly as well. This package comes with a query builder which translates your common response models into a query.   

## Download / Install <a name="downloadinstal"></a>
The Xablu.WebApiClient is written following the multi-target library approach. Meaning you can simply add the Xablu.WebApiClient package through NuGet. Install the NuGet package into your shared .NET Standard project and ALL Client projects. 

* NuGet: Xablu.WebApiClient
* PM> Install-Package Xablu.WebApiClient
* Namespace: using Xablu.WebApiClient

## Build Status: <a name="buildstatus"></a>
[![Build status](https://ci.appveyor.com/api/projects/status/5ey0sq4fn01t9o56?svg=true)](https://ci.appveyor.com/project/Xablu/xablu-webapiclient)
[![GitHub tag](https://img.shields.io/github/tag/Xablu/Xablu.WebApiClient.svg)
[![NuGet](https://img.shields.io/nuget/v/Xablu.WebApiClient.svg?label=NuGet)](https://www.nuget.org/packages/Xablu.WebApiClient/)
[![MyGet](https://img.shields.io/myget/xabluhq/v/Xablu.WebApiClient.svg)](https://www.myget.org/F/Xablu.WebApiClient/api/v2)

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

Make sure to check out the [Test Console App](https://github.com/Xablu/Xablu.WebApiClient/tree/develop/Samples/TestConsoleApp) inside the package. Down here is an example call for connecting with a Web API service through Refit:

Abstract:
```
Task<TResult> Call<TResult>(Func<T, Task<TResult>> operation, Priority priority, int retryCount, Func<Exception, bool> shouldRetry, int timeout); 
```
Example implementation:
```
async Task<IEnumerable<MyModel>> GetModelsItemsAsync(bool forceRefresh = false) 
{
  IWebApiClient<IRefitInterface> webApiClient = WebApiClientFactory.Get<IRefitInterface>("baseURL", bool defaultHeaders: true);
  var jsonresult = await webApiClient.Call(
      (myRefitService) => myRefitService.GetData(),
      (Polly.Priority) Priority.UserInitiated,
      (retryCount) 2,
      (shouldRetry) => true,
      (timeout) timeout: 60); 
}
```
Down here is an example call for connecting with a web API service through GraphQL:

Abstract:
```
public static IWebApiClient<T> Get<T>(string baseUrl, bool autoRedirectRequests = true, Func<DelegatingHandler> delegatingHandler = default, IDictionary<string, string> defaultHeaders = default) where T : class
```
Implementation:
```
async Task GraphqlAsync()
{
  var defaultHeaders = new Dictionary<string, string>
  {
    ["User-Agent"] = "ExampleUser",
    ["Authorization"] = "Bearer ******"
  };
  var webApiClient = WebApiClientFactory.Get<IGitHubApi>("https://api.github.com", false, default, defaultHeaders);
  var requestForSingleUser = new Request<UserResponseModel>(null, "(login: \"ExampleUser\")");
  var requestForUsersList = new Request<UsersResponseModel>();
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