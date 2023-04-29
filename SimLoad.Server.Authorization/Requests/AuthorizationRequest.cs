using System.ComponentModel.DataAnnotations;
using SimLoad.Common;

namespace SimLoad.Server.Authorization.Requests;

public class AuthorizationRequest : IValidatableObject
{
    /// <summary>
    ///     Correlate the session to a user
    /// </summary>
    public string? Session { get; set; }

    /// <summary>
    ///     The code that is emailed to a user
    /// </summary>
    [StringLength(8)]
    public string? Code { get; set; }

    /// <summary>
    ///     The API key of a load generator
    /// </summary>
    public Guid? ApiKey { get; set; }

    public AuthorizationType AuthorizationType =>
        ApiKey is null ? AuthorizationType.User : AuthorizationType.LoadGenerator;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Session is not null && ApiKey is not null)
            yield return new ValidationResult("Session and ApiKey cannot be used together",
                new[] { nameof(Session), nameof(ApiKey) });

        if (Session is not null && Code is null)
            yield return new ValidationResult("Session must come with a Code",
                new[] { nameof(Session), nameof(Code) });

        if (ApiKey is not null && (Session is not null || Code is not null))
            yield return new ValidationResult("ApiKey cannot be used with Session or Code",
                new[] { nameof(ApiKey), nameof(Session), nameof(Code) });
    }
}