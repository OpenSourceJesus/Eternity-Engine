using System.Collections.Generic;

namespace Frogger
{
	public static class LocalUserInfo
	{
		public static string username;
		public static Dictionary<string, uint> winsDict = new Dictionary<string, uint>();
		public static Dictionary<string, uint> winsInCurrentSeasonDict = new Dictionary<string, uint>();
		public static Dictionary<string, uint> lossesDict = new Dictionary<string, uint>();
		public static Dictionary<string, uint> lossesInCurrentSeasonDict = new Dictionary<string, uint>();
		public static Dictionary<string, uint> hitsDealtDict = new Dictionary<string, uint>();
		public static Dictionary<string, uint> hitsDealtInCurrentSeasonDict = new Dictionary<string, uint>();
		public static Dictionary<string, uint> hitsRecievedDict = new Dictionary<string, uint>();
		public static Dictionary<string, uint> hitsRecievedInCurrentSeasonDict = new Dictionary<string, uint>();
		public static Dictionary<string, float> skillDict = new Dictionary<string, float>();
		public static Dictionary<string, float> skillInCurrentSeasonDict = new Dictionary<string, float>();
		public static uint wave;
		public static uint lastMonthPlayed;
	}
}