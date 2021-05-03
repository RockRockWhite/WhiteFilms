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

            // 验证权限
            if (_accountsService.GetPermission(payload.Name) != Permissions.Administrator)
            {
                return BadRequest(new Response<string>(new PermissionDeniedError()) {resultBody = ""});
            }

            // 将账本写入数据库
            _filmsService.Create(film);

            return Ok(new Response<Dictionary<string, string>>(new Ok())
                {resultBody = new Dictionary<string, string>() {["id"] = film.Id}});
        }

        [HttpGet("{id}")]
        public ActionResult GetFilm(string id)
        {
            /* 由id获得电影 */

            var film = _filmsService.Get(id);

            if (film == null)
            {
                return NotFound(new Response<string>(new FilmIdError()) {resultBody = ""});
            }

            return Ok(new Response<Film>(new Ok()) {resultBody = film});
        }

        [HttpGet]
        public ActionResult GetFilm([FromQuery] int page, [FromQuery] int limit)
        {
            /* 分页获得电影 */

            var films = _filmsService.Get(page, limit);
            return Ok(new Response<IQueryable<Film>>(new Ok()) {resultBody = films});
        }

        [HttpPatch("{id}")]
        public ActionResult PatchFilm(string id, [FromQuery] string Authorization,
            [FromBody] JsonPatchDocument<Film> filmUpdates)
        {
            /* 修改记账本 此操作无法修改记账本权限 */
            var film = _filmsService.Get(id);

            if (film == null)
            {
                return NotFound(new Response<string>(new FilmIdError()) {resultBody = ""});
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

            if (_accountsService.GetPermission(payload.Name) != Permissions.Administrator)
            {
                return BadRequest(new Response<string>(new PermissionDeniedError()) {resultBody = ""});
            }

            filmUpdates.ApplyTo(film);

            _filmsService.Update(id, film);

            return Ok(new Response<string>(new Ok()) {resultBody = ""});
        }


        [HttpDelete("{id}")]
        public ActionResult DeleteFilm(string id, [FromQuery] string Authorization)
        {
            /* 由id获得记账本 */
            var film = _filmsService.Get(id);

            if (film == null)
            {
                return NotFound(new Response<string>(new FilmIdError()) {resultBody = ""});
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

            if (_accountsService.GetPermission(payload.Name) != Permissions.Administrator)
            {
                return BadRequest(new Response<string>(new PermissionDeniedError()) {resultBody = ""});
            }

            _filmsService.Delete(id);

            return Ok(new Response<string>(new Ok()) {resultBody = ""});
        }
    }
}