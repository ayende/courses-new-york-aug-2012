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
			using(var docStore = new DocumentStore
				{
					Url = "http://172.16.1.55:8080",
					DefaultDatabase = "Users"
				}.Initialize())
			{

				IndexCreation.CreateIndexes(typeof(User).Assembly, docStore);

				while (true)
				{
					var search = Console.ReadLine();

					using(var session = docStore.OpenSession())
					{
						DoQuery(session, search);
					}
				}
				
			}
		}



		private static void DoQuery(IDocumentSession session, string search)
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
						DoQuery(session, suggestionQueryResult.Suggestions[0]);
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
			}
		}
	}
}
