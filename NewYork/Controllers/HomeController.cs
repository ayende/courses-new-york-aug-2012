﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NewYork.Models;
using System.Linq;
using Raven.Abstractions.Data;
using Raven.Client.Document;
using Raven.Client.Linq;

namespace NewYork.Controllers
{
	public class HomeController : RavenController
	{
		public ActionResult Patch()
		{
			DocumentStore.DatabaseCommands.UpdateByIndex("Raven/DocumentsByEntityName",
				new IndexQuery
					{
						Query = "Tag:Teams"
					}, new ScriptedPatchRequest
						{
							Script = @"
this.Name = this.Name.replace('team #', '');
"
						});

			return Json("Okay");
		}

		public ActionResult Dynamic()
		{
			var load = Session.Load<dynamic>("users/ayende");
			return Json(load.Name);
		}

		public ActionResult Save()
		{
			for (var i = 0; i < 7*1024; i++)
			{
				Session.Store(new Team
					{
						Name = "team #" + i
					});
			}

			return Json("Done");
		}

		public ActionResult Show(int id)
		{
			var team = Session
				.Include<Team>(x => x.Players)
				.Load(id);

			var players = team.Players
				.Select(Session.Load<Player>)
				.Select(x => x == null ? "MIA" : x.Name)
				.ToArray();
			return Json(new
				{
					Players = players,
					team.Name,
					team.Id
				});
		}

		public ActionResult Stats(int id)
		{
			var playerId = "players/" + id;

			var player = Session.Advanced.Lazily.Load<Player>(id);
			var teams = Session.Query<Team>()
				.Where(x => x.Players.Any(p => p == playerId))
				.Lazily();

			Session.Advanced.Eagerly.ExecuteAllPendingLazyOperations();
			return Json(new
			{
				Teams = teams.Value.Select(x => x.Name),
				player.Value.Name,
				player.Value.Id
			});
		}
		public ActionResult Team(string name)
		{
			var team = new Team
				 {
					 Name = name
				 };
			Session.Store(team);
			return Json(team.Id);
		}

		public ActionResult Player(string name)
		{
			var player = new Player
			  {
				  Name = name
			  };
			Session.Store(player);
			return Json(player.Id);
		}

		public ActionResult Join(int id, string player)
		{
			var team = Session.Load<Team>(id);
			team.Players.Add(player);
			return Json("Added");
		}
	}


}
