using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using  WhiteFilms.API.Models;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace  WhiteFilms.API.Services
{
    public class TokensService

    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly SymmetricSecurityKey _key;
        private readonly int _expires;

        public TokensService()
        {
            _issuer = "WhiteFilms";
            _audience = "WhiteFilms";
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("WhiteFilms is the best films website"));
            _expires = 14; // days
        }


        public string NewToken(string id, string username)
        {
            /*生成一个token*/
            if (id.Length != 24)
            {
                throw new ArgumentException("id is not correct.", nameof(id));
            }

            if (!username.Any())
            {
                throw new ArgumentException("username cannot be empty.", nameof(username));
            }

            var claim = new Claim[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.NameId, id)
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claim,
                expires: DateTime.Now.AddDays(_expires),
                signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }

        public TokenValidationParameters GetTokenTokenValidationParameters()
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                RequireExpirationTime = true
            };
            return tokenValidationParameters;
        }

        public Payload ValidateToken(string token)
        {
            /* 用于校验token 校验成功返回true*/
            SecurityToken validtoken;
            var principal =
                new JwtSecurityTokenHandler().ValidateToken(token, this.GetTokenTokenValidationParameters(),
                    out validtoken);
            var payloadJson = ((JwtSecurityToken) validtoken).Payload.SerializeToJson();
            var payload = JsonSerializer.Deserialize<Payload>(payloadJson);

            return payload;
        }
    }
}