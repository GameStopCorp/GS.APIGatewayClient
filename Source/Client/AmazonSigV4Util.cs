using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GS.AWSAPIGatewayClient
{
    public static class AmazonSigV4Util
    {
        private const string RegionName = "us-east-1";
        private const string ServiceName = "execute-api";
        public const string Algorithm = "AWS4-HMAC-SHA256";

        public static string Sign(string hashedRequestPayload, string requestMethod, string canonicalUri, string canonicalQueryString, string host, string accessKey, string secretKey, DateTime requestDateTime, Dictionary<string, string> headers)
        {
            //var currentDateTime = DateTime.UtcNow;

            var dateStamp = requestDateTime.ToString("yyyyMMdd");
            var requestDate = requestDateTime.ToString("yyyyMMddTHHmmss") + "Z";
            var credentialScope = string.Format("{0}/{1}/{2}/aws4_request", dateStamp, RegionName, ServiceName);

            // Default headers
            var headersToSign = new SortedDictionary<string, string> {
                    { "content-type", requestMethod == "PUT" || requestMethod == "POST" ? "application/json; charset=utf-8" : string.Empty },
                    { "host", host  },
                    { "x-amz-date", requestDate }
                };

            // Add in custom headers
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    headersToSign.Add(header.Key, header.Value);
                }
            }

            var canonicalHeaders = string.Join("\n", headersToSign.Select(x => x.Key.ToLowerInvariant() + ":" + x.Value.Trim())) + "\n";
            var signedHeaders = string.Join(";", headersToSign.Select(x => x.Key.ToLowerInvariant()));

            // Task 1: Create a Canonical Request For Signature Version 4
            var canonicalRequest = requestMethod + "\n" + canonicalUri + "\n" + canonicalQueryString + "\n" + canonicalHeaders + "\n" + signedHeaders + "\n" + hashedRequestPayload;
            var hashedCanonicalRequest = _HexEncode(_Hash(_ToBytes(canonicalRequest)));

            // Task 2: Create a String to Sign for Signature Version 4
            var stringToSign = Algorithm + "\n" + requestDate + "\n" + credentialScope + "\n" + hashedCanonicalRequest;

            // Task 3: Calculate the AWS Signature Version 4
            var signingKey = _GetSignatureKey(secretKey, dateStamp, RegionName, ServiceName);
            var signature = _HexEncode(_HmacSha256(stringToSign, signingKey));

            // Task 4: Prepare a signed request
            // Authorization: algorithm Credential=access key ID/credential scope, SignedHeadaers=SignedHeaders, Signature=signature

            var authorization = string.Format("Credential={0}/{1}/{2}/{3}/aws4_request, SignedHeaders={4}, Signature={5}",
                accessKey, dateStamp, RegionName, ServiceName, signedHeaders, signature);

            return authorization;
        }

        public static string CreateRequestPayload(string jsonString)
        {
            var hashedRequestPayload = _HexEncode(_Hash(_ToBytes(jsonString)));

            return hashedRequestPayload;
        }

        private static byte[] _GetSignatureKey(string key, string dateStamp, string regionName, string serviceName)
        {
            var kDate = _HmacSha256(dateStamp, _ToBytes("AWS4" + key));
            var kRegion = _HmacSha256(regionName, kDate);
            var kService = _HmacSha256(serviceName, kRegion);
            return _HmacSha256("aws4_request", kService);
        }

        private static byte[] _ToBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str.ToCharArray());
        }

        private static string _HexEncode(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant();
        }

        private static byte[] _Hash(byte[] bytes)
        {
            return SHA256.Create().ComputeHash(bytes);
        }

        private static byte[] _HmacSha256(string data, byte[] key)
        {
            return new HMACSHA256(key).ComputeHash(_ToBytes(data));
        }
    }
}
