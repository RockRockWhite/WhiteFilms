using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using WhiteFilms.API.Models;
using WhiteFilms.API.Services;

namespace WhiteFilms.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilmsController : ControllerBase
    {
        private readonly FilmsService _filmsService;
        private readonly TokensService _tokensService;
        private readonly AccountsService _accountsService;

        public FilmsController(FilmsService filmsService, TokensService tokensService,
            AccountsService accountsService)
        {
            _filmsService = filmsService;
            _tokensService = tokensService;
            _accountsService = accountsService;
        }

        [HttpPost]
        public ActionResult CreateNewFilm([FromBody] Film film, [FromQuery] string Authorization)
        {
            // 验证token是否合法 非法将抛出异常
            Payload payload = null;
            try
            {
                payload = _tokensService.ValidateToken(Authorization.Replace("Bearer ", ""));
            }
            catch (Exception e)
            {
                return BadRequest(new Response<string>(new InvalidToken()) {resultBody = ""});
            }
            // 验证权限 TODO

            // 将账本写入数据库
            _filmsService.Create(film);

            return Ok(new Response<Dictionary<string, string>>(new Ok())
                {resultBody = new Dictionary<string, string>() {["id"] = film.Id}});
        }

        [HttpGet("{id}")]
        public ActionResult GetTallybook(string id, [FromQuery] string Authorization)
        {
            /* 由id获得记账本 */
            var tallybook = _tallybooksService.Get(id);

            if (tallybook == null)
            {
                return NotFound(new Response<string>(new TallybookIdError()) {resultBody = ""});
            }

            // 验证用户权限
            Payload payload = null;
            try
            {
                payload = _tokensService.ValidateToken(Authorization.Replace("Bearer ", ""));
            }
            catch (Exception e)
            {
                return BadRequest(new Response<string>(new InvalidToken()) {resultBody = ""});
            }

            if (_tallybooksService.GetPermission(id, payload.Name) == Permissions.Denier)
            {
                return BadRequest(new Response<string>(new PermissionDeniedError()) {resultBody = ""});
            }

            return Ok(new Response<Tallybook>(new Ok()) {resultBody = tallybook});
        }

        [HttpPatch("{id}")]
        public ActionResult PatchTallybook(string id, [FromQuery] string Authorization,
            [FromBody] JsonPatchDocument<Tallybook> tallybookUpdates)
        {
            /* 修改记账本 此操作无法修改记账本权限 */
            var tallybook = _tallybooksService.Get(id);

            if (tallybook == null)
            {
                return NotFound(new Response<string>(new TallybookIdError()) {resultBody = ""});
            }

            // 验证用户权限
            Payload payload = null;
            try
            {
                payload = _tokensService.ValidateToken(Authorization.Replace("Bearer ", ""));
            }
            catch (Exception e)
            {
                return BadRequest(new Response<string>(new InvalidToken()) {resultBody = ""});
            }

            if (_tallybooksService.GetPermission(id, payload.Name) == Permissions.Denier ||
                _tallybooksService.GetPermission(id, payload.Name) == Permissions.Reader)
            {
                return BadRequest(new Response<string>(new PermissionDeniedError()) {resultBody = ""});
            }

            // 防止Editor用户越权修改权限
            var permissions = tallybook.Permissions;
            tallybookUpdates.ApplyTo(tallybook);

            _tallybooksService.Update(id, tallybook);


            return Ok(new Response<string>(new Ok()) {resultBody = ""});
        }

        [HttpPatch("{id}/Permissions/Owner")]
        public ActionResult PatchTallybook(string id, string Authorization, string reOwnerTo)
        {
            /* 将记账本的权限移交给新的用户 */
            var tallybook = _tallybooksService.Get(id);

            if (tallybook == null)
            {
                return NotFound(new Response<string>(new TallybookIdError()) {resultBody = ""});
            }

            // 验证用户权限
            Payload payload = null;
            try
            {
                payload = _tokensService.ValidateToken(Authorization.Replace("Bearer ", ""));
            }
            catch (Exception e)
            {
                return BadRequest(new Response<string>(new InvalidToken()) {resultBody = ""});
            }

            if (_tallybooksService.GetPermission(id, payload.Name) != Permissions.Owner)
            {
                return BadRequest(new Response<string>(new PermissionDeniedError()) {resultBody = ""});
            }

            // 修改用户权限信息
            _accountsService.Update(payload.Name, id, Permissions.Editer);
            _accountsService.Update(reOwnerTo, id, Permissions.Owner);

            // 修改记账本权限信息
            _tallybooksService.Update(id, payload.Name, Permissions.Editer);
            _tallybooksService.Update(id, reOwnerTo, Permissions.Owner);

            return Ok(new Response<string>(new Ok()) {resultBody = ""});
        }

        [HttpPatch("{id}/Permissions/{username}")]
        public ActionResult PatchTallybook(string id, string Authorization, string username,
            [FromQuery] Permissions permission)
        {
            /* 改变某个用户的记账本权限 */
            var tallybook = _tallybooksService.Get(id);

            if (tallybook == null)
            {
                return NotFound(new Response<string>(new TallybookIdError()) {resultBody = ""});
            }

            // 验证用户权限
            Payload payload = null;
            try
            {
                payload = _tokensService.ValidateToken(Authorization.Replace("Bearer ", ""));
            }
            catch (Exception e)
            {
                return BadRequest(new Response<string>(new InvalidToken()) {resultBody = ""});
            }

            if (_tallybooksService.GetPermission(id, payload.Name) != Permissions.Owner)
            {
                return BadRequest(new Response<string>(new PermissionDeniedError()) {resultBody = ""});
            }

            // 不允许没有Owner 也不允许多个Owner
            if (_tallybooksService.GetPermission(id, username) == Permissions.Owner || permission == Permissions.Owner)
            {
                return BadRequest(new Response<string>(new TallybookMustHaveOnlyOwnerError()) {resultBody = ""});
            }

            // 修改用户权限信息
            _accountsService.Update(username, id, permission);

            // 修改记账本权限信息
            _tallybooksService.Update(id, username, permission);

            return Ok(new Response<string>(new Ok()) {resultBody = ""});
        }


        [HttpDelete("{id}")]
        public ActionResult DeleteTallybook(string id, [FromQuery] string Authorization)
        {
            /* 由id获得记账本 */
            var tallybook = _tallybooksService.Get(id);

            if (tallybook == null)
            {
                return NotFound(new Response<string>(new TallybookIdError()) {resultBody = ""});
            }

            // 验证用户权限
            Payload payload = null;
            try
            {
                payload = _tokensService.ValidateToken(Authorization.Replace("Bearer ", ""));
            }
            catch (Exception e)
            {
                return BadRequest(new Response<string>(new InvalidToken()) {resultBody = ""});
            }

            if (_tallybooksService.GetPermission(id, payload.Name) == Permissions.Denier ||
                _tallybooksService.GetPermission(id, payload.Name) == Permissions.Reader)
            {
                return BadRequest(new Response<string>(new PermissionDeniedError()) {resultBody = ""});
            }

            _tallybooksService.Delete(id);

            return Ok(new Response<string>(new Ok()) {resultBody = ""});
        }
    }
}