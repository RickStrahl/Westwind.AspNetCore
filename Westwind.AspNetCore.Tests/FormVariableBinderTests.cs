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
using Westwind.Web;

namespace Westwind.AspNetCore.Tests
{
    [TestClass]
    public class FormVariableBinderTests
    {

        [TestMethod]
        public void BasicUnbindingTest()
        {
            var request = GetRequest();
            var product = new TestProduct() { Sku = "Markdown_Monster", Price = 0.00M, Cost = 0.00M };

            var binder = new FormVariableBinder(request, product, prefixes: "Product.");

            Assert.IsTrue(binder.Unbind(), binder.BindingErrors.ToString());

            Assert.AreEqual(product.Price,59.99M);
            Assert.AreEqual(product.Cost, 2.22M);
            Assert.AreEqual(product.Sku, "MARKDOWN_MONSTER");
        }

        [TestMethod]
        public void PropertyExclusionTest()
        {
            var request = GetRequest();
            var product = new TestProduct() { Sku = "Markdown_Monster", Price = 49.00M, Cost = 3.00M };

            var binder = new FormVariableBinder(request, product, prefixes: "Product.");
            binder.PropertyExclusionList.Add("Cost");
            binder.PropertyExclusionList.Add("Product.Price");

            Assert.IsTrue(binder.Unbind(), binder.BindingErrors.ToString());

            // this should be left at default
            Assert.AreEqual(product.Cost, 3.00M);
            Assert.AreEqual(product.Price, 49.00M);

            // these should be updated
            Assert.AreEqual(product.Sku, "MARKDOWN_MONSTER");
        }

        [TestMethod]
        public void PropertyInclusionTest()
        {
            var request = GetRequest();
            var product = new TestProduct() { Sku = "Markdown_Monster", Price = 49.00M, Cost = 3.00M };

            var binder = new FormVariableBinder(request, product, prefixes: "Product.");
            binder.PropertyInclusionList.Add("Sku");
            binder.PropertyInclusionList.Add("Product.Price");

            Assert.IsTrue(binder.Unbind(), binder.BindingErrors.ToString());

            // this should be left at default
            Assert.AreEqual(product.Cost, 3.00M);

            // these should be updated
            Assert.AreEqual(product.Price, 59.99M);
            Assert.AreEqual(product.Sku, "MARKDOWN_MONSTER");
        }



        [TestMethod]
        public void RequestTest()
        {
            var request = GetRequest();

            Assert.AreEqual(request.Form["Product.Sku"],"MARKDOWN_MONSTER");
            Assert.AreEqual(request.Form["Product.Price"], "59.99");
            Assert.AreEqual(request.Form["Product.Cost"], "2.22");

        }


        public HttpRequest GetRequest()
        {
            var httpContext = new DefaultHttpContext();
            var request = httpContext.Request;

            var formDictionary = new Dictionary<string, StringValues>();
            formDictionary.Add("Product.Price", "59.99");
            formDictionary.Add("Product.Cost", "2.22");
            formDictionary.Add("Product.Sku", "MARKDOWN_MONSTER");

            request.Form = new FormCollection(formDictionary); 
            return request;
        }
    }

    public class TestProduct
    {
        public string Sku { get; set;  }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }

    }
}
