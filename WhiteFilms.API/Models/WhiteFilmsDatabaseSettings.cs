namespace WhiteFilms.API.Models
{
    public class WhiteFilmsDatabaseSettings : IWhiteFilmsDatabaseSettings
    {
        /* 用于保存连接数据库相关设置的类*/
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string AccountCollectionName { get; set; }
        public string WhiteFilmsCollectionName { get; set; }
    }

    public interface IWhiteFilmsDatabaseSettings
    {
        /* 用于保存连接数据库相关设置的interface 类*/
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string AccountCollectionName { get; set; }
        public string WhiteFilmsCollectionName { get; set; }
    }
}