using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace  WhiteFilms.API.Models
{
    public class Tally
    {
        public string Id { get; } // 账本Id

        [Required] public decimal Amount { get; set; } // 消费金额
        [Required] public string For { get; set; } // 消费内容
        public string Note { get; set; } // 备注
        public string Operator { get; set; } // 操作者
        public DateTime Time { get; } // 记账时间

        public Tally()
        {
            // 附加上时间戳
            Time = DateTime.Now;
            // 生成md5 id
            var md5 = MD5.Create();
            Id = BitConverter.ToString(
                md5.ComputeHash(
                    Encoding.UTF8.GetBytes(Time.ToString() + Amount.ToString() + new Random().Next(1, 100))));
            Id = Id.Replace("-", "");
        }
    }
}