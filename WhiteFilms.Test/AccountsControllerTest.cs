using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32.SafeHandles;
using MongoDB.Bson;
using WhiteFilms.API;
using WhiteFilms.API.Controllers;
using WhiteFilms.API.Services;
using Xunit;
using Xunit.Abstractions;

namespace WhiteFilms.Test
{
    public class AccountsControllerTest
    {
        private readonly AccountsController _accountsController;
        private readonly ITestOutputHelper _testOutputHelper;

        public AccountsControllerTest(AccountsController accountsController, ITestOutputHelper testOutputHelper)
        {
            _accountsController = accountsController;
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void GetAccountTest()
        {
            string username = "admin";
            string password = "admin";
            
            _testOutputHelper.WriteLine(_accountsController.GetAccount(username, password).ToJson());
        }
    }
}