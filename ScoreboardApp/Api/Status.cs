using System;

namespace ScoreboardApp.Api
{
	public class TeamStatus
	{
		public bool anonymous;
		public string teamName;
		public string id;
		public string GetShortId()
		{
			return anonymous ? id : id.Substring(0, 4);
		}
		public string link;
		public int contestScore;
		public double cpuTotalTime;
		public int easyChairId;
		public int lightningScore;
		public int mismatches;
		public int numRequests;
		public int trainingScore;

		public override string ToString()
		{
			return string.Format("ContestScore: {0}, LightningScore: {1}, TrainingScore: {2}, Mismatches: {3}, NumRequests: {4}, CpuTotalTime: {5}, EasyChairId: {6}", contestScore, lightningScore, trainingScore, mismatches, numRequests, cpuTotalTime, easyChairId);
		}
	}


	[NUnit.Framework.TestFixture]
	public class Status_Test
	{
		[NUnit.Framework.Test]
		public void Test()
		{
			Console.WriteLine(new IcfpcApi("0071PimxQKpGJdtDE76gsjAoaOagBVX3tdGOfCQH").GetStatus());
		}
	}
}