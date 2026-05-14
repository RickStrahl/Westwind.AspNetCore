using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Westwind.AspNetCore.Extensions;

namespace Westwind.AspNetCore.Tests
{
    [TestClass]
    public class HttpRequestExtensionsTests
    {
        [TestMethod]
        public void GetClientIpAddressWithoutProxyCheckReturnsRemoteAddress()
        {
            var request = CreateRequest(IPAddress.Parse("203.0.113.20"));
            request.Headers["X-Forwarded-For"] = "198.51.100.10, 10.0.0.1";

            var result = request.GetClientIpAddress();
            Assert.AreEqual("203.0.113.20", result);
        }

        [TestMethod]
        public void GetClientIpAddressWithProxyCheckUsesFirstForwardedAddress()
        {
            var request = CreateRequest(IPAddress.Parse("10.0.0.5"));
            request.Headers["X-Forwarded-For"] = "198.51.100.10, 10.0.0.1";

            var result = request.GetClientIpAddress(checkForProxy: true);

            Assert.AreEqual("198.51.100.10", result);
        }

        [TestMethod]
        public void GetClientIpAddressWithProxyCheckUsesForwardedHeaderAddress()
        {
            var request = CreateRequest(IPAddress.Parse("10.0.0.5"));
            request.Headers["Forwarded"] = "for=198.51.100.21;proto=https;by=203.0.113.1";

            var result = request.GetClientIpAddress(checkForProxy: true);

            Assert.AreEqual("198.51.100.21", result);
        }

        private static HttpRequest CreateRequest(IPAddress remoteIpAddress)
        {
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = remoteIpAddress;

            return context.Request;
        }
    }
}
