# GS.APIGatewayClient
A .NET (C#) AWS API Gateway client interface and signing utility.

# Installation
First, download the latest .nupkg within /nuget-deployables-gs-store-systems/GS.APIClientGateway/ from Artifactory.
Then under *Tools* -> *NuGet Package Manager* -> *Package Manager Settings* -> *Package Sources*, add a new package source for the directory you downloaded the package to.
Finally, *Browse* within the Package Manager for "GS.APIClientGateway".

Installing the package will also install:
  - Newtonsoft.Json
  - GS.StoreSystems.Logging
  - System.Net.Http
  - System.Net.Http.Formatting.Extension

# Implementation
```
var apiGatewayClient = new APIGatewayClient([string hostname], [string accessKey], [string secretKey], [ILogging logger]);

bool postSuccess = apiGatewayClient.Post([string canonicalPath], [string queryString], [JObject postObject], [Dictonary<string, string> headers = null]);
bool putSuccess = apiGatewayClient.Put([string canonicalPath], [string queryString], [JObject updateObject], [Dictonary<string, string> headers = null]);
bool deleteSuccess = apiGatewayClient.Delete([string canonicalPath], [string queryString], [Dictonary<string, string> headers = null]);
object getResult = apiGatewayClient.Get([string canonicalPath], [string queryString], [Dictonary<string, string> headers = null]);
```