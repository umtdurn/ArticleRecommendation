using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ArticleRecommendadtion.Models;
using Microsoft.AspNetCore.Authorization;
using ArticleRecommendadtion.AbstractServices.MongoDbAbstract;
using ArticleRecommendadtion.ConcreteServices.MongoDbConcrete;
using ArticleRecommendadtion.AbstractServices.BusinessServiceAbstract;
using ArticleRecommendadtion.Models.VMs;
using System.Text;
using System.Text.Json;
using ArticleRecommendadtion.AbstractServices.ApiServiceAbstract;

namespace ArticleRecommendadtion.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IBusinessService _businessService;
    private readonly IHttpClientFactory _clientFactory;
    private readonly IApiService _apiService;
    private readonly IMongoDbService _mongoService;

    public HomeController(ILogger<HomeController> logger, IBusinessService businessService, IHttpClientFactory clientFactory, IApiService apiService, IMongoDbService mongoService)
    {
        _logger = logger;
        _businessService = businessService;
        _clientFactory = clientFactory;
        _apiService = apiService;
        _mongoService = mongoService;
    }

    public IActionResult Index()
    {
        IndexVM init = new IndexVM();
        return View(init);
        //return RedirectToAction("GetRecommendations","Home");
    }

    public async Task<IActionResult> GetRecommendations()
    {
        using (var client = new HttpClient())
        {
            // Anahtar kelimeleri virgülle ayrılmış bir string olarak hazırla
            string keywords = "Machine Learning,Computer Vision,Image Processing,Deep Learning";
            string url = $"http://localhost:8000/recommendations/?keywords={keywords}";

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }

            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<IActionResult> ArticleDetails(string title)
    {
        Article article = await _mongoService.GetDocumentByTitleAsync<Article>("ArticleDataset", title);
        ArticleData articleData = new ArticleData()
        {
            Title = article.Data.Title,
            Abstract = article.Data.Abstract,
            Keywords = article.Data.Keywords,
            FullText = article.Data.FullText,
            Name = string.Empty
        };

        return Json(articleData);
    }

    [HttpPost]
    public async Task<IActionResult> Search(string searchWord)
    {
        string loggedUserMail = HttpContext.User.Claims.ElementAt(0).Value;    
        IEnumerable<Article> data = await _businessService.GetArticlesBySearchWord(searchWord);
        IEnumerable<RecommendationVM> fastTextRecomm = await _apiService.Get5FastTextRecommendations(loggedUserMail);
        IEnumerable<RecommendationVM> sciBertRecomm = await _apiService.Get5SciBertRecommendations(loggedUserMail);
        double fasttextPrecision = await _mongoService.GetPrecision("fasttext");
        double scibertPrecision = await _mongoService.GetPrecision("scibert");
        IndexVM model = new IndexVM()
        {
            SearchArticles = data,
            FastTextRecommendations = fastTextRecomm,
            SciBertRecommendations = sciBertRecomm,
            FastTextPrecision = fasttextPrecision,
            SciBertPrecision = scibertPrecision
        };
        return View("Index", model);
    }

    [HttpPost]
    public async Task<IActionResult> PrecisionFeedback(string title, string state, string type)
    {
        PrecisionVM insert = new PrecisionVM()
        {
            Title = title,
            State = state,
            RecommType = type
        };

        await _mongoService.AddDocumentAsync<PrecisionVM>("PrecisionTable", insert);

        if(state == "like")
        {
            string loggedUserMail = HttpContext.User.Claims.ElementAt(0).Value;
            var insertDoc = new LikedArticle();
            if (!string.IsNullOrEmpty(title))
            {
                insertDoc.Title = title;
                insertDoc.UserEmail = loggedUserMail;
            }
            await _mongoService.AddDocumentAsync<LikedArticle>("LikedArticles", insertDoc);
        }
        else if (state == "disslike")
        {
            string loggedUserMail = HttpContext.User.Claims.ElementAt(0).Value;
            var insertDoc = new LikedArticle();
            if (!string.IsNullOrEmpty(title))
            {
                insertDoc.Title = title;
                insertDoc.UserEmail = loggedUserMail;
            }
            await _mongoService.AddDocumentAsync<LikedArticle>("DisslikedArticles", insertDoc);
        }


        return Ok("true");
    }

    [HttpPost]
    public async Task<IActionResult> AddArticleLikedList(string title)
    {
        string loggedUserMail = HttpContext.User.Claims.ElementAt(0).Value;
        var insertDoc = new LikedArticle();
        if (!string.IsNullOrEmpty(title))
        {
            insertDoc.Title = title;
            insertDoc.UserEmail = loggedUserMail;
        }
        await _mongoService.AddDocumentAsync<LikedArticle>("LikedArticles", insertDoc);
        return Ok("true");
    }

    [HttpPost]
    public async Task<IActionResult> AddArticleDisslikedList(string title)
    {
        string loggedUserMail = HttpContext.User.Claims.ElementAt(0).Value;
        var insertDoc = new LikedArticle();
        if (!string.IsNullOrEmpty(title))
        {
            insertDoc.Title = title;
            insertDoc.UserEmail = loggedUserMail;
        }
        await _mongoService.AddDocumentAsync<LikedArticle>("DisslikedArticles", insertDoc);
        return Ok("true");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

