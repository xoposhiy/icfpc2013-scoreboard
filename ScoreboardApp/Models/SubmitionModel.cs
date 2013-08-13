using System.ComponentModel.DataAnnotations;

namespace ScoreboardApp.Models
{
	public class SubmitionModel
	{
		[StringLength(500000, MinimumLength = 30, ErrorMessage = "Incorrect value")]
		[Required(ErrorMessage = "Is required")]
		public string JsonOrAuthKey { get; set; }

		[StringLength(100, ErrorMessage = "Too long")]
		[Required(ErrorMessage = "Is required")]
		public string TeamName { get; set; }

		[Url]
		public string Link { get; set; }
	}
}