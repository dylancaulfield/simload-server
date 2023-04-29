using System.ComponentModel.DataAnnotations;

namespace SimLoad.Server.Authorization.Requests;

public class CreateSessionRequest
{
    [EmailAddress] public string EmailAddress { get; init; }
    public string? Session { get; set; }
}