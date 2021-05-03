using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using  WhiteFilms.API.Models;

namespace  WhiteFilms.API.Services
{
    public class TallybooksService
    {
        private readonly IMongoCollection<Tallybook> _tallybooks;

        public TallybooksService(WhiteFilmsDatabaseSettings settings)
        {
            /* 构造函数 获得数据库Collection */
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _tallybooks = database.GetCollection<Tallybook>(settings.WhiteFilmsCollectionName);
        }

        // CRUD
        public Tallybook Create(Tallybook tallybook)
        {
            /* 创建账本 */
            _tallybooks.InsertOne(tallybook);
            return tallybook;
        }

        public Tallybook Get(string id) => _tallybooks.Find(tallybook => tallybook.Id == id).FirstOrDefault();

        public Permissions GetPermission(string tallybookId, string username)
        {
            /* 获得用户对改记账本的权限 使用前请保证记账本id正确 */
            var tallybook = Get(tallybookId);

            if (!tallybook.Permissions.ContainsKey(username))
            {
                return Permissions.Denier;
            }
            else
            {
                return tallybook.Permissions[username];
            }
        }

        public void Update(string id, Tallybook newTallybook) =>
            /* 更新记账本信息 */
            _tallybooks.ReplaceOne(tallybook => tallybook.Id == id, newTallybook);

        public void Update(string tallybookId, string username, Permissions permissions)
        {
            /* 更新用户的权限 */
            var tallybook = Get(tallybookId);
            tallybook.Permissions[username] = permissions;
            Update(tallybookId, tallybook);
        }

        public void Delete(string id) => _tallybooks.DeleteOne(tallybook => tallybook.Id == id);
    }
}