using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ScoreboardApp.Api;
using ScoreboardApp.Models;

namespace ScoreboardApp.Controllers
{
	public class HomeController : Controller
	{
		private readonly JavaScriptSerializer js = new JavaScriptSerializer();
		private readonly object locker = new object();

		public ActionResult Index(string id)
		{
			if (id == null) return View();
			try
			{
				var model = new TeamViewModel();
				model.TeamNumber = id.StartsWith("____") ? "Unknown" : id;
				model.Team = LoadTeams().Values.FirstOrDefault(t => t.id.StartsWith(id)) ?? new TeamStatus{anonymous = true};
				model.Problems = LoadProblems(id);
				return View(model);
			}
			catch (Exception e)
			{
				return Content(e.Message);
			}
		}

		// GET: /Stats/

		public ActionResult Stats()
		{
			var teams = LoadTeams().OrderByDescending(kv => kv.Value.contestScore).Select(kv => kv.Value).ToList();
			return View(teams);
		}

		[HttpPost]
		public ActionResult Post(SubmitionModel submition)
		{
			if (!ModelState.IsValid) return RedirectToAction("Index");
			if (submition.JsonOrAuthKey.EndsWith("vpsH1H")) submition.JsonOrAuthKey = submition.JsonOrAuthKey.Substring(0, submition.JsonOrAuthKey.Length - 6);
			try
			{
				IList<MyProblemJson> problems;
				string shortId;
				if (submition.JsonOrAuthKey.Length == 40)
				{
					var id = submition.JsonOrAuthKey;
					var api = new IcfpcApi(id);
					problems = api.MyProblems();
					var team = api.GetStatus();
					team.id = id;
					team.link = submition.Link;
					team.teamName = submition.TeamName;
					UpdateTeams(id, team);
					shortId = StoreProblems(problems, id);
				}
				else
				{
					var json = submition.JsonOrAuthKey;
					problems = js.Deserialize<List<MyProblemJson>>(json);
					shortId = StoreProblems(problems, null);
					UpdateTeams(shortId, new TeamStatus { id = shortId, anonymous = true, contestScore = problems.Count(p => p.solved == true) });
				}
				return RedirectToAction("Index", "Home", new { id = shortId });
			}
			catch (Exception e)
			{
				return Content(e.Message);
			}
		}

		private Dictionary<string, TeamStatus> LoadTeams()
		{
			var input = Load("teams.json");
			if (input == "") return MigrateTeams();
			else return js.Deserialize<List<TeamStatus>>(input).ToDictionary(t => t.id);
		}

		private Dictionary<string, TeamStatus> MigrateTeams()
		{
			var d = LoadDic();
			var teams = d.Select(kv => CalculateTeamStatus(kv.Key, kv.Value)).ToDictionary(t => t.id);
			SaveTeams(teams);
			return teams;
		}

		private TeamStatus CalculateTeamStatus(string id, int score)
		{
			if (id.StartsWith("____")) return new TeamStatus { id = id, contestScore = score, anonymous = true };
			var team = new IcfpcApi(id).GetStatus();
			team.id = id;
			return team;
		}

		private void SaveTeams(IDictionary<string, TeamStatus> teams)
		{
			Save("teams.json", js.Serialize(teams.Values.ToList()));
		}

		private void UpdateTeams(string id, TeamStatus newTeam)
		{
			lock (locker)
			{
				var teams = LoadTeams();
				teams[id] = newTeam;
				SaveTeams(teams);
			}
		}


		private int TryParse(string s)
		{
			int v;
			if (int.TryParse(s, out v)) return v;
			return 0;
		}

		private string StoreProblems(IList<MyProblemJson> problems, string id)
		{
			var shortId = "";
			var serialized = js.Serialize(problems);
			if (id == null)
			{
				id = "____" + serialized.GetHashCode();
				shortId = id;
			}
			else
				shortId = id.Substring(0, 4);
			string filename = id + serialized.GetHashCode() + ".json";
			Save(filename, serialized);
			UpdateScoreboard(problems, id);
			return shortId;
		}

		public void Save(string filename, string text)
		{
			string path = Request.MapPath("~/App_Data/" + filename);
			System.IO.File.WriteAllText(path, text);
		}

		public string Load(string filename)
		{
			string path = Request.MapPath("~/App_Data/" + filename);
			if (System.IO.File.Exists(path))
				return System.IO.File.ReadAllText(path);
			return "";
		}

		public static string GetShortId(string id)
		{
			if (id.StartsWith("____")) return id;
			else return id.Substring(0, 4);
		}

		private IList<MyProblemJson> LoadProblems(string id)
		{
			string path = Request.MapPath("~/App_Data/");
			var file = Directory.EnumerateFiles(path, id + "*.json").FirstOrDefault();
			if (file == null) throw new Exception(id + " not found");
			return js.Deserialize<List<MyProblemJson>>(System.IO.File.ReadAllText(file));
		}

		private void UpdateScoreboard(IList<MyProblemJson> problems, string id)
		{
			lock (locker)
			{
				var dictionary = LoadDic();
				dictionary[id] = problems.Count(p => p.solved == true);
				var content = dictionary.OrderByDescending(kv => kv.Value).Select(kv => kv.Key + ' ' + kv.Value);
				Save("all.txt", string.Join("\r\n", content));
			}
		}

		private Dictionary<string, int> LoadDic()
		{
			return Load("all.txt").Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(line => line.Split(' '))
				.ToDictionary(p => p[0], p => TryParse(p[1]));
		}
	}
}