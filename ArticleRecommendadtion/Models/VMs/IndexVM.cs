using System;
namespace ArticleRecommendadtion.Models.VMs
{
	public class IndexVM
	{
		public IEnumerable<Article> SearchArticles { get; set; } = new List<Article>();

		public IEnumerable<RecommendationVM> FastTextRecommendations { get; set; } = new List<RecommendationVM>();

		public IEnumerable<RecommendationVM> SciBertRecommendations { get; set; } = new List<RecommendationVM>();

		public double FastTextPrecision { get; set; } = 0.0;

		public double SciBertPrecision { get; set; } = 0.0;
	}
}

