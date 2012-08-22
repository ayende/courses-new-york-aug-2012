using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace Tagging
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var docStore = new DocumentStore
			{
				Url = "http://172.16.1.55:8080",
				DefaultDatabase = "Tagging"
			}.Initialize())
			{
				new Social().Execute(docStore);
				//CreateData(docStore);
			}
		}

		private static void CreateData(IDocumentStore docStore)
		{
			using (var session = docStore.OpenSession())
			{
				session.Store(new Tag
					{
						Name = "Cats",
						Description = "Cats are nice"
					}, "tags/cats");
				session.Store(new Tag
					{
						Name = "Dogs",
						Description = "Dogs are the best"
					}, "tags/dogs");


				for (int i = 0; i < 50; i++)
				{
					session.Store(new Image
						{
							PostedAt = DateTime.Today.AddHours(i),
							TagIds = {"tags/cats", "tags/dogs"}
						});
				}

				for (int i = 0; i < 50; i++)
				{
					session.Store(new Post
						{
							PostedAt = DateTime.Today.AddHours(i),
							TagIds = {"tags/dogs"}
						});
				}

				for (int i = 0; i < 25; i++)
				{
					session.Store(new Post
						{
							PostedAt = DateTime.Today.AddHours(-i),
							TagIds = {"tags/cats"}
						});
				}

				session.SaveChanges();
			}
		}
	}

	public class Tag
	{
		public string Name { get; set; }
		public string Description { get; set; }
	}

	public class Image
	{
		public List<string> TagIds { get; set; }
		public string Title { get; set; }
		public DateTime PostedAt { get; set; }
		public Image()
		{
			TagIds = new List<string>();
		}
	}

	public class Post
	{
		public List<string> TagIds { get; set; }
		public string Title { get; set; }
		public DateTime PostedAt { get; set; }
		public Post()
		{
			TagIds = new List<string>();
		}
	}

	public class Social : AbstractMultiMapIndexCreationTask<Social.Result>
	{
		public class Result
		{
			public string TagId;
			public int Images;
			public int Posts;
			public DateTime Day;
		}

		public Social()
		{
			AddMap<Image>(images =>
			              from image in images 
						  from tag in image.TagIds
						  select new
							  {
								  Day = image.PostedAt.Date,
								  TagId = tag,
								  Images = 1,
								  Posts = 0
							  });

			AddMap<Post>(posts =>
						  from post in posts
						  from tag in post.TagIds
						  select new
						  {
							  Day = post.PostedAt.Date,
							  TagId = tag,
							  Images = 0,
							  Posts = 1
						  });

			Reduce = results =>
			         from result in results
			         group result by new {result.Day, result.TagId}
			         into g
			         select new
				         {
					         g.Key.Day,
					         g.Key.TagId,
					         Images = g.Sum(x => x.Images),
					         Posts = g.Sum(x => x.Posts)
				         };

			TransformResults = (database, results) =>
			                   from result in results
			                   let tag = database.Load<Tag>(result.TagId)
			                   select new
				                   {
					                   tag.Name,
					                   tag.Description,
					                   result.TagId,
					                   result.Posts,
					                   result.Images,
					                   result.Day
				                   };

		}
	}
}
