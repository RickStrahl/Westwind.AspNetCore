using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Westwind.AspNetCode.Security;
using Westwind.Utilities;
using Westwind.Utilities.Data;
using Westwind.Web;

#if true // disabled for now - config not set up. You can manually remove and set the connection string

namespace Westwind.AspNetCore.Tests
{
    [TestClass]
    public class UserTokenManagerTests
    {
        public const string ConnectionString ="server=.;database=WestwindWebStore;integrated security=true;encrypt=false";

        [TestMethod]
        public void CreateTokenTest()
        {
            var manager = new UserTokenManager(ConnectionString);
            var token = manager.CreateNewToken("1111", "Reference #1", "12345678");
            
            Assert.IsNotNull(token, manager.ErrorMessage);
            Console.WriteLine(token);
        }

        [TestMethod]
        public void ValidateTokenTest()
        {
            var manager = new UserTokenManager(ConnectionString);
            var token = manager.CreateNewToken("2222", "Reference #1", "1234");

            Assert.IsNotNull(token, manager.ErrorMessage);
            Console.WriteLine(token);


            Assert.IsTrue(manager.IsTokenValid(token, false), manager.ErrorMessage);
        }

        [TestMethod]
        public void GetTokenByTokenIdentifierTest()
        {
            var tokenIdentifier = DataUtils.GenerateUniqueId();

            var manager = new UserTokenManager(ConnectionString);
            var token = manager.CreateNewToken("3333", "Reference #3", tokenIdentifier);

            Assert.IsNotNull(token, manager.ErrorMessage);
            Console.WriteLine(token);

            var userToken = manager.GetTokenByTokenIdentifier(tokenIdentifier);
            Assert.IsNotNull(userToken, manager.ErrorMessage);
            Console.WriteLine(JsonSerializationUtils.Serialize(userToken));            
        }   

    }
}

#endif
