﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GS.AWSAPIGatewayClient
{
    public interface IAWSAPIGatewayClient<T>
    {
        T Get(string canonicalPath, string queryString, Dictionary<string, string> headers = null);
        bool Put(string canonicalPath, string queryString, JObject updatedObject, Dictionary<string, string> headers = null);
        bool Post(string canonicalPath, string queryString, JObject postObject, Dictionary<string, string> headers = null);
        bool Delete(string canonicalPath, string queryString, Dictionary<string, string> headers = null);
    }
}
