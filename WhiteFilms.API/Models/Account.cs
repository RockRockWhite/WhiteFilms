using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Hosting.Internal;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace WhiteFilms.API.Models
{
    public class Account
    {
        public Account()
        {
            // 默认为user
            Permission = Permissions.User;
        }

        /* 用户账户的数据模型 */
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; init; }

        [Required] public string Username { get; set; }
        public string Salt { get; set; }
        [Required] public string Password { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public Dictionary<string, string> SecurityQuestions { get; set; } //问题index 答案
        public Permissions Permission { get; set; } // 用户操作 权限
    }
}