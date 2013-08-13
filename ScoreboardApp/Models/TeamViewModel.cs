using System.Collections.Generic;
using ScoreboardApp.Api;

namespace ScoreboardApp.Models
{
	public class TeamViewModel
	{
		public string TeamId { get; set; }
		public string TeamNumber { get; set; }
		public TeamStatus Team { get; set; }
		public IList<MyProblemJson> Problems { get; set; }
	}
}