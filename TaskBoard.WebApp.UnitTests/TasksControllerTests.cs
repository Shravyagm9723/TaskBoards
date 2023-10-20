using System;
using System.Net;
using System.Linq;
using TaskBoard.WebApp.Controllers;
using TaskBoard.WebApp.Models.Task;

using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using TaskBoard.Tests.Common;

namespace TaskBoard.WebApp.UnitTests
{
    public class TasksControllerTests : UnitTestsBase
    {
        private TasksController controller;

        [OneTimeSetUp]
        public void InitialSetUp()
        {
            // Instantiate the controller class with the testing database
            this.controller = new TasksController(
                this.testDb.CreateDbContext());
            // Set UserMaria as current logged user
            TestingUtils.AssignCurrentUserForController(this.controller, this.testDb.UserMaria);
        }

        [Test]
        public void Test_Create_PostValidData()
        {
        }

        [Test]
        public void Test_DeletePage_ValidId()
        {
        }

        [Test]
        public void Test_Delete_UnauthorizedUser()
        {
        }

        [Test]
        public void Test_Edit_ValidId()
        {
        }

        [Test]
        public void Test_Edit_InvalidId()
        {
        }

        [Test]
        public void Test_Edit_UnauthorizedUser()
        {
        }
    }
}
