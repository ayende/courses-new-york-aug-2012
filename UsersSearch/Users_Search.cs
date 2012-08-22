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
}