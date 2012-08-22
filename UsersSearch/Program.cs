using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Client.Linq;

namespace UsersSearch
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var docStore = new DocumentStore
				{
					Url = "http://localhost.fiddler:8080",
					DefaultDatabase = "Users"
				}.Initialize())
			{

				IndexCreation.CreateIndexes(typeof(User).Assembly, docStore);


				while (true)
				{
					var search = Console.ReadLine();

					using (var session = docStore.OpenSession())
					{
						DoQueryForUsers(session, search);
					}
				}

			}
		}

		private static void DoQueryForUnits(IDocumentSession session, string search)
		{
			var result = session.Query<UnitAndPeople_Search.Result, UnitAndPeople_Search>()
				.Search(x => x.Query, search)
				.AsProjection<UnitAndPeople_Search.Result>()
				.FirstOrDefault();

			if (result == null)
			{
				Console.WriteLine("nothing found");
				return;
			}
			Console.WriteLine(result.Title);

			//var person = result as Person;
			//if (person != null)
			//{
			//    Console.WriteLine("Call " + person.Phone + " for " + person.Name);
			//    return;
			//}

			//var unit = result as Unit;
			//if(unit != null)
			//{
			//    DoQueryForUnits(session, unit.LeasedTo);
			//}
		}


		private static void DoQueryForUsers(IDocumentSession session, string search)
		{
			var q = session.Query<Users_Search.Result, Users_Search>()
				.Search(x => x.Query, search)
				.As<User>();

			var user = q.FirstOrDefault();
			if (user == null)
			{
				var suggestionQueryResult = q.Suggest(new SuggestionQuery());
				switch (suggestionQueryResult.Suggestions.Length)
				{
					case 0:
						Console.WriteLine("Nothing found, bummer");
						break;
					case 1:
						DoQueryForUsers(session, suggestionQueryResult.Suggestions[0]);
						break;
					default:
						Console.WriteLine("Did you mean?");
						foreach (var suggestion in suggestionQueryResult.Suggestions)
						{
							Console.WriteLine("\t{0}", suggestion);
						}
						break;
				}
			}
			else
			{
				Console.WriteLine("{0} {1}", user.Name, user.Email);
				var documentId = session.Advanced.GetDocumentId(user);
				session.Advanced.DocumentStore.Changes()
					.ForDocument(documentId)
					.Subscribe(notification =>
						{
							Console.WriteLine("A document we previously searched on was changed: " + notification.Name);
						});
			}
		}
	}
}
