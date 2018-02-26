# GS.Pkg.AWSAPIGatewayClient
A .NET (C#) AWS API Gateway client interface and signing utility.

# Installation
Within the Visual Studio NuGet Package Manger, select GameStop Nuget as the Package Source and run the following command:
```
Install-Package GS.AWSAPIGatewayClient
```

Installing the package will also install:
  - Newtonsoft.Json
  - System.Net.Http
  - System.Net.Http.Formatting.Extension

# Implementation
```
var client = new AWSAPIGatewayClient<T>([string hostname], [string accessKey], [string secretKey]);

bool postSuccess = client.Post([string canonicalPath], [string queryString], [JObject postObject], [Dictonary<string, string> headers = null]);
bool putSuccess = client.Put([string canonicalPath], [string queryString], [JObject updateObject], [Dictonary<string, string> headers = null]);
bool deleteSuccess = client.Delete([string canonicalPath], [string queryString], [Dictonary<string, string> headers = null]);
T getResult = client.Get([string canonicalPath], [string queryString], [Dictonary<string, string> headers = null]);
```