using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Authorization.Models;
using SimLoad.Server.Authorization.Requests;
using SimLoad.Server.Authorization.Responses;
using SimLoad.Server.Common.Encryption;
using SimLoad.Server.Data.Contexts;

namespace SimLoad.Server.Authorization.Services;

public interface ICreateLoginSessionService
{
    Task<IActionResult> CreateLoginSession(CreateSessionRequest request);
}

public class CreateLoginSessionService : ICreateLoginSessionService
{
    private readonly IEncryptor _encryptor;
    private readonly ISendVerificationCodeService _sendVerificationCodeService;
    private readonly UserDbContext _userDbContext;

    public CreateLoginSessionService(IEncryptor encryptor, ISendVerificationCodeService sendVerificationCodeService,
        UserDbContext userDbContext)
    {
        _encryptor = encryptor;
        _sendVerificationCodeService = sendVerificationCodeService;
        _userDbContext = userDbContext;
    }

    public async Task<IActionResult> CreateLoginSession(CreateSessionRequest request)
    {
        // Check if the session is expired
        if (request.Session is null) return await CreateNewSession(request);
        var session = _encryptor.Decrypt<LoginSession>(request.Session);
        if (session.IsExpired)
            return new BadRequestObjectResult(new ErrorResponse
            {
                Message = "Session has expired"
            });

        return await ReSendVerificationCode(session);
    }

    private async Task<IActionResult> ReSendVerificationCode(LoginSession loginSession)
    {
        await _sendVerificationCodeService.SendVerificationCode(loginSession);

        var response = new CreateSessionResponse
        {
            Session = _encryptor.Encrypt(loginSession)
        };

        return new OkObjectResult(response);
    }

    private async Task<IActionResult> CreateNewSession(CreateSessionRequest request)
    {
        var user = await _userDbContext
            .Users
            .FirstOrDefaultAsync(
                u => u.EmailAddresses.Any(
                    ue => ue.EmailAddress == request.EmailAddress));
        if (user is null) return new NotFoundResult();

        var session = new LoginSession
        {
            UserId = user.Id
        };

        await _sendVerificationCodeService.SendVerificationCode(session);

        var response = new CreateSessionResponse
        {
            Session = _encryptor.Encrypt(session)
        };

        return new OkObjectResult(response);
    }
}