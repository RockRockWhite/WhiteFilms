using System;

namespace WhiteFilms.API.Models
{
    public enum Permissions : short
    {
        Administrator = 1, // 有权限上传编辑
        User = 0 // 无权限上传编辑
    }
}