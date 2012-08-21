using System.Collections.Generic;
using System.Linq;

namespace NewYork.Models
{
	public class Auction
	{
		public string Id { get; set; }
		public Bid CurrentWin { get; set; }

		
	}

	public class Bid
	{
		public decimal Amount { get; set; }
		public string Username { get; set; }
	}

	public class BidCollection
	{
		public string AuctionId { get; set; }
		public List<Bid> Bids { get; set; }

		public BidCollection()
		{
			Bids = new List<Bid>();
		}
	}
}