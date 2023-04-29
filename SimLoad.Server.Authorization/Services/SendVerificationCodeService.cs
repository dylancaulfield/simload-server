using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Authorization.Models;
using SimLoad.Server.Data.Contexts;

namespace SimLoad.Server.Authorization.Services;

public interface ISendVerificationCodeService
{
    Task SendVerificationCode(LoginSession loginSession);
}

public class SendVerificationCodeService : ISendVerificationCodeService
{
    private readonly IAmazonSimpleEmailService _simpleEmailService;
    private readonly UserDbContext _userDbContext;

    public SendVerificationCodeService(UserDbContext userDbContext, IAmazonSimpleEmailService simpleEmailService)
    {
        _userDbContext = userDbContext;
        _simpleEmailService = simpleEmailService;
    }

    public async Task SendVerificationCode(LoginSession loginSession)
    {
        var user = await _userDbContext
            .Users
            .Where(u => u.Id == loginSession.UserId)
            .Include(u => u.EmailAddresses)
            .SingleOrDefaultAsync();
        if (user is null || user.EmailAddresses.Count < 1) return;

        var message = new Message
        {
            Subject = new Content("SimLoad Verification Code"),
            Body = new Body
            {
                Text = new Content("Your verification code is: " + loginSession.ConfirmationCode)
            }
        };

        var sendEmailRequest = new SendEmailRequest
        {
            Source = "SimLoad Verification <no-reply@simload.ie>",
            Destination = new Destination
            {
                BccAddresses = user.EmailAddresses.Select(e => e.EmailAddress).ToList()
            },
            Message = message
        };

        await _simpleEmailService.SendEmailAsync(sendEmailRequest);
    }
}

internal class UserEmailAddresses
{
    public string[] EmailAddresses { get; set; }
}