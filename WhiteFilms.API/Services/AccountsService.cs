using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Driver;
using WhiteFilms.API.Models;

namespace WhiteFilms.API.Services
{
    public class AccountsService
    {
        private readonly IMongoCollection<Account> _accounts;
        private readonly PasswordsService _passwordsService;

        public AccountsService(WhiteFilmsDatabaseSettings settings, PasswordsService passwordsService)
        {
            /* 构造函数 获得数据库Collection */
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _accounts = database.GetCollection<Account>(settings.AccountCollectionName);
            _passwordsService = passwordsService;
        }

        // CRUD

        public Account Create(Account account)
        {
            /* 创建账户 */

            // 获得盐值
            account.Salt = _passwordsService.NewSalt(account.Username);
            // 加盐加密密码
            account.Password = _passwordsService.SaltedHash(account.Salt, account.Password);
            // 加盐加密安全问题
            foreach (var key in account.SecurityQuestions.Keys)
            {
                account.SecurityQuestions[key] =
                    _passwordsService.SaltedHash(account.Salt, account.SecurityQuestions[key]);
            }

            _accounts.InsertOne(account);
            return account;
        }

        public Account Get(string username) => _accounts.Find(account => account.Username == username).FirstOrDefault();

        public bool CheckPassword(Account account, string password) =>
            _passwordsService.CheckSaltedHash(account.Salt, password, account.Password);


        public void Update(string username, Account newAccount)
        {
            _accounts.ReplaceOne(account => account.Username == username, newAccount);
        }

        public void Update(string username, Permissions permission)
        {
            /* 更新用户权限 */
            var account = Get(username);
            account.Permission = permission;
            Update(username, account);
        }

        public void Delete(Account accountIn) => _accounts.DeleteOne(account => account == accountIn);
        public void Delete(string username) => _accounts.DeleteOne(account => account.Username == username);
    }
}