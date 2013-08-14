using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ScoreboardApp.Api;

namespace ScoreboardApp.Models
{
	public class ScoreboardModel
	{
		public static ScoreboardModel ContestScore(List<TeamStatus> teams)
		{
			return new ScoreboardModel
				{
					Teams = teams,
					getScore = team => team.contestScore,
					Name = "Main Division",
					scores = new[] {1400, 1250, 1100, 904, 701, 568, 428, 291, 206, 116, 38, 0},
					places = new[] {11, 25, 50, 75, 100, 125, 150, 175, 200, 225, 250, 275, int.MaxValue}
				};
		}

		public static ScoreboardModel LightningScore(List<TeamStatus> teams)
		{
			return new ScoreboardModel
				{
					Teams = teams,
					getScore = team => team.lightningScore,
					Name = "Lightning Division",
					scores = new[] {520, 457, 345, 249, 129, 67, 20, 3, 0},
					places = new[] {5, 10, 25, 50, 75, 100, 125, 150, int.MaxValue}
				};
		}
		public List<TeamStatus> Teams;
		public Func<TeamStatus, int> getScore;
		public string Name;
		public int[] scores;
		public int[] places;

	}
}