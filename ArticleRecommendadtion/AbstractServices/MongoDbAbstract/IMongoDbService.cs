using System;
using ArticleRecommendadtion.Models;

namespace ArticleRecommendadtion.AbstractServices.MongoDbAbstract
{
	public interface IMongoDbService
	{
        Task AddDocumentAsync<T>(string collectionName, T document);

        Task<List<T>> GetAllDocumentsAsync<T>(string collectionName);

        Task<T> GetDocumentByTitleAsync<T>(string collectionName, string title);

        Task UpdateDocumentByTitleAsync<T>(string collectionName, string title, T document);

        Task DeleteDocumentByTitleAsync<T>(string collectionName, string title);

        Task<T> GetUserByEmailAsync<T>(string collectionName, string email);

        Task<List<T>> GetArticlesBySearchWord<T>(string collectionName, string searchWord);

        Task<double> GetPrecision(string type);

        Task UpdateUserAsync(string collectionName, User user, string oldMail);
    }
}

