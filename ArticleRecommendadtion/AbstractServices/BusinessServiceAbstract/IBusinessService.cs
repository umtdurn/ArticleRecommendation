using System;
using ArticleRecommendadtion.Models;

namespace ArticleRecommendadtion.AbstractServices.BusinessServiceAbstract
{
	public interface IBusinessService
	{
		public Task<IEnumerable<Article>> GetArticlesBySearchWord(string searchWord);

		public IEnumerable<string> GetRecommByFastText();

        public IEnumerable<string> GetRecommByScibert();

    }
}

