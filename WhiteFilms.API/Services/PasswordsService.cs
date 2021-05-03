using System;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;

namespace  WhiteFilms.API.Services
{
    public class PasswordsService
    {
        private readonly MD5 _md5;
        private readonly SHA1 _sha1;
        private readonly Random random = new Random();

        public PasswordsService()
        {
            _md5 = MD5.Create();
            ;
            _sha1 = SHA1.Create();
        }

        public string NewSalt(string username)
        {
            /* 产生一个salt值 */
            if (!username.Any())
            {
                throw new ArgumentException("username cannot be empty.", nameof(username));
            }

            var datetime = DateTime.Now;
            var salt = BitConverter.ToString(
                _md5.ComputeHash(Encoding.UTF8.GetBytes(datetime.ToString() + random.Next(1, 10))));
            salt = salt.Replace("-", "");
            return salt;
        }

        public string SaltedHash(string salt, string password)
        {
            /* 加盐加密密码 */
            if (salt.Length != 32)
            {
                throw new ArgumentException("salt is not correct.", nameof(salt));
            }

            if (!password.Any())
            {
                throw new ArgumentException("password cannot be empty.", nameof(password));
            }

            // 第一次不带盐值hash
            var _password = BitConverter.ToString(_sha1.ComputeHash(Encoding.UTF8.GetBytes(password)));
            _password = _password.Replace("-", "");
            // 带盐值hash
            _password = BitConverter.ToString(_sha1.ComputeHash(Encoding.UTF8.GetBytes(_password + salt)));
            _password = _password.Replace("-", "");

            return _password;
        }

        public bool CheckSaltedHash(string salt, string password, string saltedHash)
        {
            /* 校验加盐加密是否正确 返回True为正确 返回False为错误 */

            if (salt.Length != 32)
            {
                throw new ArgumentException("salt is not correct.", nameof(salt));
            }

            if (!password.Any())
            {
                throw new ArgumentException("password cannot be empty.", nameof(password));
            }

            return this.SaltedHash(salt, password) == saltedHash;
        }
    }
}