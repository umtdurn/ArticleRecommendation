using System;
using ArticleRecommendadtion.AbstractServices.BusinessServiceAbstract;
using ArticleRecommendadtion.AbstractServices.MongoDbAbstract;
using ArticleRecommendadtion.Models;

namespace ArticleRecommendadtion.ConcreteServices.BusinessServiceConcrete
{
	public class BusinessService : IBusinessService
	{

        private readonly IMongoDbService _mongoService;

        public BusinessService(IMongoDbService service)
		{
            _mongoService = service;
        }

        public async Task<IEnumerable<Article>> GetArticlesBySearchWord(string searchWord)
        {
            var data = await _mongoService.GetArticlesBySearchWord<Article>("ArticleDataset", searchWord);
            return data;
        }

        public IEnumerable<string> GetRecommByFastText()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetRecommByScibert()
        {
            throw new NotImplementedException();
        }
    }
}

