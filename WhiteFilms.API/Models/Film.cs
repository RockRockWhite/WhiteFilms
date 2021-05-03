using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WhiteFilms.API.Models
{
    public class Film
    {
        /* 电影的数据类型 */
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; } // 电影Id

        [Required] public string[] Directors { get; set; }
        [Required] public string[] ScriptWriter { get; set; }
        [Required] public string[] StarringRoles { get; set; }
        [Required] public string[] Tags { get; set; }
        [Required] public string Location { get; set; } // 制片地区
        [Required] public string Description { get; set; }
        [Required] public DateTime Time { get; set; } // 上映时间
        [Required] public int[] Stars { get; set; } // 电影五星评分系统 
    }
}