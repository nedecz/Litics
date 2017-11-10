using Microsoft.VisualStudio.TestTools.UnitTesting;
using Litics.Controller.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;

namespace Litics.Controller.Controllers.Tests
{
    [TestClass()]
    public class AccountControllerTests
    {
        private AccountController _controller;
        [TestInitialize]
        public void TestInit()
        {
            _controller = new AccountController();
            _controller.Request = new HttpRequestMessage();
            _controller.Configuration = new HttpConfiguration();

        }


        [TestMethod()]
        public async Task RegisterTest()
        {
            var userAccount = new Models.RegisterBindingModel()
            {
                AccountName = "UnitDevTest",
                Email = "test@test.com",
                UserName = "UnitTest",
                Password = "#123Test",
                ConfirmPassword = "#123Test"
            };
            var result = await _controller.Register(userAccount);
            Console.WriteLine();
        }
    }
}