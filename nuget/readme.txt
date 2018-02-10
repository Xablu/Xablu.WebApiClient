# Xablu.WebApiClient Readme

For latest changes: https://github.com/Xablu/Xablu.WebApiClient/blob/master/CHANGELOG.md

## EXTREMELY IMPORTANT SETUP
Before using the Xablu.WebApiClient (via `CrossRestApiClient.Current`) make sure you configure the `RestApiClient` with atleast the base address of the API you want to call.
Failing to do so will result in a `NotConfiguredException`.

Configuration is done through the `CrossRestApiClient.Configure` method, like this:

```
CrossRestApiClient.Configure((opt) => opt.ApiBaseAddress = "https://api.mybackend.com");
```

