using Microsoft.VisualStudio.TestTools.UnitTesting;
using GS.AWSAPIGatewayClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GS.AWSAPIGatewayClient.Tests
{
    [TestClass()]
    public class AmazonSigV4UtilTests
    {
        [TestMethod()]
        public void SignTest()
        {
            var requestDate = DateTime.Parse("2/26/2018 5:00:00 PM");
            var hashedRequestPayload = AmazonSigV4Util.CreateRequestPayload(string.Empty);
            var authorization = AmazonSigV4Util.Sign(hashedRequestPayload, "POST", "/master/v1/DigitalReceipt/Process", string.Empty, "gamestop.com", "foo", "bar", requestDate, null);
            Assert.AreEqual("Credential=foo/20180226/us-east-1/execute-api/aws4_request, SignedHeaders=content-type;host;x-amz-date, Signature=6f6f61fbf4420b00ef210ae7f8992a9b9a8575851b5651d4766330b15fcd5ca6", authorization);
        }
    }
}