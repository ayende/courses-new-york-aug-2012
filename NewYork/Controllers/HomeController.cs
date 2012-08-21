using System.Web.Mvc;
using NewYork.Models;

namespace NewYork.Controllers
{
	public class HomeController : RavenController
	{
		 public ActionResult Team(string name)
		 {
			 var entity = new Team
				 {
					 Name = name
				 };
			 Session.Store(entity);
			 return Json(entity.Id);
		 }
	}
}