using System;
using System.ComponentModel;
using System.Threading;
using System.Web.Mvc;
using Raven.Client;
using Raven.Client.Document;

namespace NewYork.Controllers
{
	public class RavenController : Controller
	{
		private static IDocumentStore _documentStore;
		public static IDocumentStore DocumentStore
		{
			get
			{
				if(_documentStore != null)
					return _documentStore;
				lock (typeof(RavenController))
				{
					Thread.MemoryBarrier();
					if (_documentStore != null)
						return _documentStore;

					return _documentStore = CreateDocumentStore();
				}
			}
		}

		private static IDocumentStore CreateDocumentStore()
		{
			var docStore = new DocumentStore
				{
					ConnectionStringName = "RavenDB"
				};
			docStore.Initialize();
			return docStore;
		}

		public new IDocumentSession Session { get; set; }

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			Session = DocumentStore.OpenSession();
		}

		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			base.OnActionExecuted(filterContext);
			using(Session)
			{
				if (Session == null)
					return;
				if (filterContext.Exception != null)
					return;
				Session.SaveChanges();
			}
		}

		protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
		{
			return base.Json(data, contentType, contentEncoding, JsonRequestBehavior.AllowGet);
		}
	}
}