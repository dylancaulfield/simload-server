using System.ComponentModel.DataAnnotations;

namespace SimLoad.Server.Users.Requests;

public class CreateUserRequest
{
    [EmailAddress] public string EmailAddress { get; set; }

    public string DisplayName { get; set; }
}