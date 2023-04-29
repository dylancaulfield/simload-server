using MongoDB.Bson.Serialization.Attributes;

namespace SimLoad.Data.Entities.User;

public class UserEntity : INamedCollection
{
	public Guid Id { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
	public string Password { get; set; }
	
	[BsonIgnore]
	public string CollectionName => "users";
}