using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NewYork.Models;

namespace NewYork.Controllers
{
	public class AuctionController : RavenController
	{
		public ActionResult Strange()
		{
			Session.Advanced.LoadStartingWith<Bid>("customers/1/cars");
			
			return Json("In a Strange Land");
		}
		 public ActionResult Create()
		 {
			 var auction = new Auction
				 {
					 CurrentWin = new Bid
						 {
							 Amount = 0,
							 Username = "no one"
						 }
				 };

			 Session.Store(auction);
			 Session.Store(new BidCollection
				 {
					 AuctionId = auction.Id
				 }, auction.Id + "/bids");
			 return Json(auction.Id);
		 }

		public ActionResult Bid(string auctionId, string user, decimal amount)
		{
			var bidCollection = Session.Include<BidCollection>(x => x.AuctionId).Load(auctionId + "/bids");
			var auction = Session.Load<Auction>(auctionId);

			var bid = new Bid { Amount = amount, Username = user };
			auction.CurrentWin = bid;
			bidCollection.Bids.Add(bid);

			if (bidCollection.Bids.Count > 10)
			{
				var history = new BidCollection
				{
					AuctionId = auction.Id,
					Bids = new List<Bid>(bidCollection.Bids.Take(5))
				};
				Session.Store(history, auction.Id + "/bids/");

				bidCollection.Bids = new List<Bid>(bidCollection.Bids.Skip(5));
			}
			return Json("Added");
		}
	}
}