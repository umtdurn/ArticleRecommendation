using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ArticleRecommendadtion.Models
{
	public class Article
	{
		[BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

		[BsonElement("name")]
		public string Name { get; set; } = string.Empty;

		[BsonElement("data")]
		public ArticleData? Data { get; set; }
	}
}

