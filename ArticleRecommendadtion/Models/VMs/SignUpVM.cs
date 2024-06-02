using System;
namespace ArticleRecommendadtion.Models.VMs
{
	public class SignUpVM
	{
		public string FirstName { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public string Password { get; set; } = string.Empty;

		public IEnumerable<string> Interests { get; set; } = new List<string>();
	}
}

