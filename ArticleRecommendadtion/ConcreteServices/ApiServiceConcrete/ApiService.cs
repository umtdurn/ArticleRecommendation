using System;
using ArticleRecommendadtion.AbstractServices.ApiServiceAbstract;
using ArticleRecommendadtion.AbstractServices.MongoDbAbstract;
using ArticleRecommendadtion.Models;
using ArticleRecommendadtion.Models.VMs;
using Newtonsoft.Json;

namespace ArticleRecommendadtion.ConcreteServices.ApiServiceConcrete
{
    public class ApiService : IApiService
    {
        private readonly IMongoDbService _mongoService;

        public ApiService(IMongoDbService mongoService)
        {
            _mongoService = mongoService;
        }

        public async Task<IEnumerable<RecommendationVM>> Get5FastTextRecommendations(string userEmail)
        {
            User user = await _mongoService.GetUserByEmailAsync<User>("Users",userEmail);
            string userInterests = string.Join(",", user.Interests);

            using (var client = new HttpClient())
            {
                //string url = $"http://localhost:8000/fasttext_recommendations/?keywords={userInterests}";
                //string url2 = "http://localhost:8000/fasttext_recommendations/?keywords=" + userInterests + "&mail=" + userEmail + "\"";

                string queryString = $"?keywords={userInterests}&mail={userEmail}";
                string baseUrl = "http://localhost:8000/fasttext_recommendations/";
                // Tam URL
                string requestUrl = baseUrl + queryString;

                var response = await client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    // JSON string'i List<List<object>> olarak deserialize et
                    var tempRecommendations = JsonConvert.DeserializeObject<List<List<object>>>(jsonString);

                    // List<List<object>>'ı List<Recommendation> listesine dönüştür
                    if(tempRecommendations is not null)
                    {
                        var recommendations = tempRecommendations.Select(r => new RecommendationVM
                        {
                            Title = (r[0] is not null) ? r[0].ToString()! : "",
                            Score = Convert.ToDouble(r[1])
                        }).ToList();

                        return recommendations;
                    }
                }

                return new List<RecommendationVM>();
            }
        }

        public async Task<IEnumerable<RecommendationVM>> Get5SciBertRecommendations(string userEmail)
        {
            User user = await _mongoService.GetUserByEmailAsync<User>("Users", userEmail);
            string userInterests = string.Join(",", user.Interests);

            string queryString = $"?keywords={userInterests}&mail={userEmail}";
            string baseUrl = "http://localhost:8000/scibert_recommendations/";
            // Tam URL
            string requestUrl = baseUrl + queryString;


            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    // JSON string'i List<List<object>> olarak deserialize et
                    var tempRecommendations = JsonConvert.DeserializeObject<List<List<object>>>(jsonString);

                    // List<List<object>>'ı List<Recommendation> listesine dönüştür
                    if (tempRecommendations is not null)
                    {
                        var recommendations = tempRecommendations.Select(r => new RecommendationVM
                        {
                            Title = (r[0] is not null) ? r[0].ToString()! : "",
                            Score = Convert.ToDouble(r[1])
                        }).ToList();

                        return recommendations;
                    }
                }

                return new List<RecommendationVM>();
            }
        }
    }
}

