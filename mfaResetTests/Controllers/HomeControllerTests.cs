using Microsoft.VisualStudio.TestTools.UnitTesting;
using mfaReset.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mfaResetTests.Fakes;
using System.Web.Mvc;
using mfaReset.ViewModel;
using Microsoft.Graph;

namespace mfaReset.Controllers.Tests
{
    [TestClass()]
    public class HomeControllerTests
    {
        List<User> CreateTestUsers()
        {
            List<User> users = new List<User>();
            User testUser = new User() { UserPrincipalName = "test.user@yourdomain.se" };
            users.Add(testUser);
            return users;
        }
        HomeController CreateHomeController() {
            var graphService = new FakeGraphService(CreateTestUsers());
            var functionService = new FakeFunctionService();
            var logService = new FakeLogService();
            return new HomeController(functionService, graphService, logService);
        }
        [TestMethod()]
        public void IndexShouldReturnIndexView()
        {
            //Arrange
            var controller = CreateHomeController();
            //Act
            var result = controller.Index() as ViewResult;
            //Assert
            Assert.AreEqual("Index", result.ViewName);
        }
        [TestMethod()]
        public async Task SearchShouldReturnUserForUPN()
        {
            var controller = CreateHomeController();
            var result = await controller.Search("test.user@yourdomain.se") as PartialViewResult;
            var viewmodel = result.Model as HomeViewModel;
            Assert.IsNotNull(viewmodel.User);
            Assert.AreEqual("_SearchResult", result.ViewName);
        }
        [TestMethod()]
        public async Task SearchShouldNotReturnUserForBadUPN()
        {
            var controller = CreateHomeController();
            var result = await controller.Search("test2.user@yourdomain.se") as PartialViewResult;
            var viewmodel = result.Model as HomeViewModel;
            Assert.IsNull(viewmodel.User);
            Assert.AreEqual("_SearchResult", result.ViewName);
        }
        [TestMethod()]
        public void FeedbackShouldReturnFeedbackView()
        {
            //Arrange
            var controller = CreateHomeController();
            //Act
            var result = controller.Feedback() as ViewResult;
            //Assert
            Assert.AreEqual("Feedback", result.ViewName);
        }
    }
}