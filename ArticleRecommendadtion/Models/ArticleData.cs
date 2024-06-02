using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ArticleRecommendadtion.Models
{
	public class ArticleData
	{
        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("title")]
        public string? Title { get; set; }

        [BsonElement("abstract")]
        public string? Abstract { get; set; }

        [BsonElement("fulltext")]
        public string? FullText { get; set; }

        [BsonElement("keywords")]
        public string? Keywords { get; set; }

    }
}

