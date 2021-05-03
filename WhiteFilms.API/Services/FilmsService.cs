using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using WhiteFilms.API.Models;

namespace WhiteFilms.API.Services
{
    public class FilmsService
    {
        private readonly IMongoCollection<Film> _films;

        public FilmsService(WhiteFilmsDatabaseSettings settings)
        {
            /* 构造函数 获得数据库Collection */
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _films = database.GetCollection<Film>(settings.FilmsCollectionName);
        }

        // CRUD
        public Film Create(Film film)
        {
            /* 创建账本 */
            _films.InsertOne(film);
            return film;
        }

        public Film Get(string id) => _films.Find(film => film.Id == id).FirstOrDefault();

        public void Update(string id, Film newFilm) =>
            /* 更新记账本信息 */
            _films.ReplaceOne(film => film.Id == id, newFilm);

        public void Delete(string id) => _films.DeleteOne(film => film.Id == id);
    }
}