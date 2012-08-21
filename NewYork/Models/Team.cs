using System.Collections.Generic;

namespace NewYork.Models
{
	public class Team
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public List<string> Players { get; set; }

		public Team()
		{
			Players = new List<string>();
		}
	}

	public class Player
	{
		public string Id { get; set; }
		public string Name { get; set; }
	}
}