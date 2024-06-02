using System;
using ArticleRecommendadtion.Models.VMs;
using Microsoft.AspNetCore.Mvc;

namespace ArticleRecommendadtion.AbstractServices.ApiServiceAbstract
{
	public interface IApiService
	{
        public Task<IEnumerable<RecommendationVM>> Get5FastTextRecommendations(string userEmail);

        public Task<IEnumerable<RecommendationVM>> Get5SciBertRecommendations(string userEmail);

    }
}

