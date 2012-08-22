using System.Web.Mvc;
using NewYork.Models;
using System.Linq;
using Raven.Client.Linq;

namespace NewYork.Controllers
{
	public class ShirtsController : RavenController
	{
		public ActionResult Search()
		{
			RavenQueryStatistics stats;
			var q = Session.Query<Shirt>()
				.Statistics(out stats)
				.Where(x => x.Types.Any(t => t == new ShirtType { Color = "Red", Size = "XL" }))
				.Where(x => x.Types.Any(t => t == new ShirtType { Color = "Blue", Size = "M" }));

			return Json(new
				{
					Query = q.ToString(),
					Results = q.ToList(),
					stats.IndexName
				});
		}
	}
}