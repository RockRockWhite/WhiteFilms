using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using WhiteFilms.API.Models;
using WhiteFilms.API.Services;


namespace WhiteFilms.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AccountsService _accountsService;
        private readonly TokensService _tokensService;

        public AccountsController(AccountsService accountsService, TokensService tokensService)
        {
            _accountsService = accountsService;
            _tokensService = tokensService;
        }

        [HttpPost]
        public ActionResult CreateNewAccount([FromBody] Account newAccount)
        {
            /* 创建账户 */
            if (_accountsService.Get(newAccount.Username) != null)
            {
                /* 用户名重复 */
                return BadRequest(new Response<Dictionary<string, string>>(new UsernameExistedError())
                {
                    resultBody = new Dictionary<string, string>() {["username"] = newAccount.Username}
                });
            }

            _accountsService.Create(newAccount);

            return Created("",
                new Response<Dictionary<string, string>>(new Ok())
                {
                    resultBody = new Dictionary<string, string>() {["id"] = newAccount.Id}
                });
        }

        [HttpGet("{username}")]
        public ActionResult GetAccount(string username, [FromQuery] string password)
        {
            /* 登录账户 若验证成功 则返回token*/
            var account = _accountsService.Get(username);

            if (account == null)
            {
                return NotFound("username or password error.");
            }

            if (!_accountsService.CheckPassword(account, password))
            {
                return NotFound("username or password error.");
            }

            // 为客户端产生token
            var token = _tokensService.NewToken(account.Id, account.Username);
            var response = new Response<Dictionary<string, string>>(new Ok())
                {resultBody = new Dictionary<string, string>() {["token"] = token}};
            return Ok(response);
        }

        [HttpGet]
        public ActionResult GetAccount([FromQuery] string Authorization)
        {
            /* 由token获得用户账户信息 */

            Payload payload = null;
            try
            {
                payload = _tokensService.ValidateToken(Authorization.Replace("Bearer ", ""));
            }
            catch (Exception e)
            {
                return BadRequest(new Response<string>(new InvalidToken()) {resultBody = ""});
            }

            var account = _accountsService.Get(payload.Name);

            // 隐藏关键信息
            account.Password = "";
            account.Salt = "";
            account.SecurityQuestions.Clear();

            var response = new Response<Account>(new Ok())
                {resultBody = account};

            return Ok(response);
        }

        [HttpPatch("{username}")]
        public ActionResult UpdateAccount(string username, [FromBody] JsonPatchDocument<Account> accountUpdates,
            [FromQuery] string password)
        {
            /* 更新用户账户信息*/
            var account = _accountsService.Get(username);
            if (account == null)
            {
                return NotFound(new Response<string>(new UsernameOrPasswordError()) {resultBody = ""});
            }

            // 防止越权修改
            var old_password = account.Password;
            var old_security_questions = account.SecurityQuestions;
            var old_permission = account.Permission;
            // 应用更新
            accountUpdates.ApplyTo(account);

            account.Permission = old_permission;
            _accountsService.Update(username, account);

            // 只有密码正确,才能修改密码
            if (!_accountsService.CheckPassword(account, password))
            {
                account.Password = old_password;
                account.SecurityQuestions = old_security_questions;
                if (password != "null")
                {
                    return NotFound(new Response<string>(new UsernameOrPasswordError()) {resultBody = ""});
                }
            }

            return Ok(new Response<string>(new Ok()) {resultBody = ""});
        }

        [HttpDelete("{username}")]
        public ActionResult DeleteAccount(string username, [FromQuery] string password)
        {
            /* 注销用户账户*/
            var account = _accountsService.Get(username);
            if (account == null)
            {
                return NotFound(new Response<string>(new UsernameOrPasswordError()) {resultBody = ""});
            }

            if (!_accountsService.CheckPassword(account, password))
            {
                return NotFound(new Response<string>(new UsernameOrPasswordError()) {resultBody = ""});
            }

            _accountsService.Delete(username);
            return Ok(new Response<string>(new Ok()) {resultBody = ""});
        }
    }
}