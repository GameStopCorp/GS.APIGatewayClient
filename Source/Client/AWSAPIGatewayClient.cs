using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GS.AWSAPIGatewayClient
{
    public class AWSAPIGatewayClient : IAWSAPIGatewayClient<object>
    {
        private const string AMZ_DATE = "X-Amz-Date";

        private string Host;
        private string AccessKey;
        private string SecretKey;
        
        private HttpClient _client;
        private HttpClient HttpClient
        {
            get { return _client ?? (_client = new HttpClient()); }
        }

        public AWSAPIGatewayClient(string host, string accessKey, string secretKey)
        {
            Host = host;
            AccessKey = accessKey;
            SecretKey = secretKey;

            HttpClient.BaseAddress = new Uri("https://" + Host);
            HttpClient.DefaultRequestHeaders.Accept.Clear();
        }

        public void Authorize(string verb, string canonicalPath, string queryString, JObject jobject = null, Dictionary<string, string> headers = null)
        {
            var requestDate = DateTime.UtcNow;
            var hashedRequestPayload = AmazonSigV4Util.CreateRequestPayload(JsonConvert.SerializeObject(jobject) ?? string.Empty);
            var authorization = AmazonSigV4Util.Sign(hashedRequestPayload, verb, canonicalPath, queryString, Host, AccessKey, SecretKey, requestDate, headers);

            HttpClient.DefaultRequestHeaders.Remove(AMZ_DATE);
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AmazonSigV4Util.Algorithm, authorization);
            HttpClient.DefaultRequestHeaders.Add(AMZ_DATE, requestDate.ToString("yyyyMMddTHHmmss") + "Z");

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    HttpClient.DefaultRequestHeaders.Remove(header.Key);
                    HttpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
        }

        public object Get(string canonicalPath, string queryString, Dictionary<string, string> headers = null)
        {
            Authorize("GET", canonicalPath, queryString, null, headers);

            object obj = null;

            var response = HttpClient.GetAsync(canonicalPath + queryString).Result;
            if (response.IsSuccessStatusCode)
                obj = response.Content.ReadAsAsync<object>().Result;
            else throw new Exception(String.Format("GET request failed: {0} - {1}", response.StatusCode, response.Content.ReadAsStringAsync().Result));

            return obj;
        }

        public bool Put(string canonicalPath, string queryString, JObject updatedObject, Dictionary<string, string> headers = null)
        {
            Authorize("PUT", canonicalPath, queryString, updatedObject, headers);
            var response = HttpClient.PutAsJsonAsync(canonicalPath + queryString, updatedObject).Result;

            if (!response.IsSuccessStatusCode)
                throw new Exception(String.Format("PUT request failed: {0} - {1}", response.StatusCode, response.Content.ReadAsStringAsync().Result));

            return response.IsSuccessStatusCode;
        }

        public bool Post(string canonicalPath, string queryString, JObject postObject, Dictionary<string, string> headers = null)
        {
            Authorize("POST", canonicalPath, queryString, postObject, headers);

            var response = HttpClient.PostAsJsonAsync(canonicalPath + queryString, postObject).Result;

            if (!response.IsSuccessStatusCode)
                throw new Exception(String.Format("POST request failed: {0} - {1}", response.StatusCode, response.Content.ReadAsStringAsync().Result));

            return response.IsSuccessStatusCode;
        }

        public bool Delete(string canonicalPath, string queryString, Dictionary<string, string> headers = null)
        {
            Authorize("DELETE", canonicalPath, queryString, null, headers);

            var response = HttpClient.DeleteAsync(canonicalPath + queryString).Result;

            if (!response.IsSuccessStatusCode)
                throw new Exception(String.Format("POST request failed: {0} - {1}", response.StatusCode, response.Content.ReadAsStringAsync().Result));

            return response.IsSuccessStatusCode;
        }
    }
}
