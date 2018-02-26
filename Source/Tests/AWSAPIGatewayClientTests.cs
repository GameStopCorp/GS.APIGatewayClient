using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Moq;

namespace GS.AWSAPIGatewayClient.Tests
{
    public class Config
    {
        public string host, accessKey, secretKey;
    }

    [TestClass()]
    public class AWSAPIGatewayClientTests
    {
        private GS.AWSAPIGatewayClient.AWSAPIGatewayClient _apiGatewayClient;
        private JObject _receiptObject;

        private bool IsInitialized
        {
            get
            {
                return _apiGatewayClient != null;
            }
        }

        private bool InitializeConsumer()
        {
            if (IsInitialized) return true;

            var receiptDataPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ReceiptData.json");
            var receiptData = File.ReadAllText(receiptDataPath);
            _receiptObject = JObject.FromObject(JsonConvert.DeserializeObject(receiptData));
            if (!_receiptObject.TryGetValue("Id", out JToken idToken))
            {
                return false;
            }
            var messageId = ((((JValue)idToken) == null) ? "null" : ((JValue)idToken).Value.ToString());

            var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Config.json");
            var config = File.ReadAllText(configPath);
            var configObject = JsonConvert.DeserializeObject<Config>(config);

            _apiGatewayClient = new GS.AWSAPIGatewayClient.AWSAPIGatewayClient(configObject.host, configObject.accessKey, configObject.secretKey);

            return true;
        }

        [TestMethod()]
        public void PostTest()
        {
            if (InitializeConsumer())
            {
                var uri = "/master/v1/DigitalReceipt/Process";
                
                Assert.IsTrue(_apiGatewayClient.Post(uri, string.Empty, _receiptObject));
            }
            else Assert.Fail("Unable to initialize consumer.");
        }
    }
}