using MongoDB.Bson;
using MongoDB.Driver;

namespace SimLoad.Data.Entities.User;

public class UserQueries
{

	private readonly IMongoCollection<UserEntity> userCollection;

	public UserQueries(SimLoadMongoCollection<UserEntity> userCollection)
	{
		this.userCollection = userCollection.Collection;
	}

	public async Task<UserEntity?> GetUserByEmailAddress(string emailAddress)
	{

		var filter = new BsonDocument
		{
			{
				"Email", emailAddress
			}
		};

		return await userCollection.Find(filter).FirstOrDefaultAsync();
	}

}