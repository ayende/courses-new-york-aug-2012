using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace UsersSearch
{
	public class Users_Search : AbstractIndexCreationTask<User, Users_Search.Result>
	{
		public class Result
		{
			public string Query;
		}

		public Users_Search()
		{
			Map = users =>
				  from user in users
				  select new
					  {
						  Query = new object[]
						      {
							      user.Name,
							      user.Email, 
							      user.Email.Split('@')
						      }
					  };
			Index(x => x.Query, FieldIndexing.Analyzed);
		}
	}

	public class Users_CountByDomain : AbstractIndexCreationTask<User, Users_CountByDomain.Result>
	{
		public class Result
		{
			public string Domain;
			public int Count;
		}

		public Users_CountByDomain()
		{
			Map = users =>
				  from user in users
				  select new
					  {
						  Domain = user.Email.Split('@')[1],
						  Count = 1
					  };
			Reduce = results =>
					 from result in results
					 group result by result.Domain
						 into g
						 select new
							 {
								 Domain = g.Key,
								 Count = g.Sum(x => x.Count)
							 };

			StoreAllFields(FieldStorage.Yes);
		}
	}
	public class Person : IEntity
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Phone { get; set; }
	}

	public interface IEntity
	{
	}

	public class Unit : IEntity
	{
		public string Id { get; set; }
		public string Location { get; set; }
		public string LeasedTo { get; set; }
	}

	public class UnitAndPeople_Search : AbstractMultiMapIndexCreationTask<UnitAndPeople_Search.Result>
	{
		public class Result
		{
			public string Query { get; set; }
			public string Title { get; set; }
		}

		public UnitAndPeople_Search()
		{
			AddMap<Person>(people =>
						   from person in people
						   select new
							   {
								   Query = new[] { person.Name, person.Phone, person.Id },
								   Title = person.Name
							   }
			);

			AddMap<Unit>(units =>
						 from unit in units
						 select new
							 {
								 Query = new[] { unit.Location },
								 Title = unit.Location
							 }
			);

			Index(x => x.Query, FieldIndexing.Analyzed);
			Store(x => x.Title, FieldStorage.Yes);
		}
	}

}