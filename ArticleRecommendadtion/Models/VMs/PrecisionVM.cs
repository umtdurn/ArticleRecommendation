using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ArticleRecommendadtion.Models.VMs
{
	public class PrecisionVM
	{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

		public string RecommType { get; set; } = string.Empty;

		public string Title { get; set; } = string.Empty;

		public string State { get; set; } = string.Empty;
	}
}

