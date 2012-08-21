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

    public ActionResult Player(string name)
    {
      var player = new Player
        {
          Name = name
        };
      Session.Store(player);
      return Json(player);
    }
  }
}