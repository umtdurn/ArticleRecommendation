using System;
using ArticleRecommendadtion.AbstractServices.MongoDbAbstract;
using ArticleRecommendadtion.Models;
using ArticleRecommendadtion.Models.VMs;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ArticleRecommendadtion.ConcreteServices.MongoDbConcrete
{
	public class MongoDbService : IMongoDbService
	{
		private IMongoDatabase _database;

		public MongoDbService(IConfiguration configuration)
		{
			var client = new MongoClient(configuration["MongoDbSetting:ConnectionString"]);
			_database = client.GetDatabase(configuration["MongoDbSetting:DatabaseName"]);
		}

        public async Task AddDocumentAsync<T>(string collectionName, T document)
        {
            var collection = _database.GetCollection<T>(collectionName);
            await collection.InsertOneAsync(document);
        }

        public async Task DeleteDocumentByTitleAsync<T>(string collectionName, string title)
        {
            var collection = _database.GetCollection<T>(collectionName);
            await collection.DeleteOneAsync(Builders<T>.Filter.Eq("title", title));
        }

        public async Task<List<T>> GetAllDocumentsAsync<T>(string collectionName)
        {
            var collection = _database.GetCollection<T>(collectionName);
            return await collection.Find(x => true).ToListAsync();
        }

        public async Task<T> GetDocumentByTitleAsync<T>(string collectionName, string title)
        {
            var collection = _database.GetCollection<T>(collectionName);
            return await collection.Find(Builders<T>.Filter.Eq("data.title", title)).FirstOrDefaultAsync();
        }

        public async Task UpdateDocumentByTitleAsync<T>(string collectionName, string title, T document)
        {
            var collection = _database.GetCollection<T>(collectionName);
            await collection.ReplaceOneAsync(Builders<T>.Filter.Eq("title", title), document);
        }

        public async Task<T> GetUserByEmailAsync<T>(string collectionName, string email)
        {
            var collection = _database.GetCollection<T>(collectionName);
            return await collection.Find(Builders<T>.Filter.Eq("Email", email)).FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetArticlesBySearchWord<T>(string collectionName, string searchWord)
        {
            var collection = _database.GetCollection<T>(collectionName);
            return await collection.Find(Builders<T>
                .Filter.Regex("data.keywords", new BsonRegularExpression(searchWord, "i"))).ToListAsync();
        }

        public async Task<double> GetPrecision(string type)
        {
            var collection = _database.GetCollection<PrecisionVM>("PrecisionTable");

            // Filter tanımı
            var filter = Builders<PrecisionVM>.Filter.Eq(p => p.RecommType, type) &
                         Builders<PrecisionVM>.Filter.Eq(p => p.State, "like");

            // Filtreye göre veri çekme
            var likeds = await collection.Find(filter).ToListAsync();
            double likedCount = likeds.Count();

            var filter2 = Builders<PrecisionVM>.Filter.Eq(p => p.RecommType, type);

            // Filtreye göre veri çekme
            var results = await collection.Find(filter2).ToListAsync();
            double totalCount = results.Count();

            double precisionVal = likedCount / totalCount;
            return precisionVal;
        }

        public async Task UpdateUserAsync(string collectionName, User user, string oldMail)
        {
            var collection = _database.GetCollection<User>(collectionName);
            var filter = Builders<User>.Filter.Eq("Email", oldMail);
            var x = await collection.ReplaceOneAsync(filter, user);
        }
    }
    
}
