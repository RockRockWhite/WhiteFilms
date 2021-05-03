using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Hosting.Internal;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace  WhiteFilms.API.Models
{
    public class Tallybook
    {
        /* 记账本的数据模型 */
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; init; }

        [Required] public string Name { get; set; }
        public Dictionary<string, Permissions> Permissions { get; set; } // own edit read三个等级 用户id和权限的映射
        public Tally[] Tallies { get; set; }
    }
}