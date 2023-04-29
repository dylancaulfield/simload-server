using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Project;
using SimLoad.Server.Tests.Requests;
using SimLoad.Server.Tests.Responses;

namespace SimLoad.Server.Tests.Services;

public interface IGetTestMetaDataService
{
    Task<IActionResult> GetTestMetaData(GetTestMetadataRequest request);
}

public class GetTestMetaDataService : IGetTestMetaDataService
{

    private readonly IPermissionEvaluator<Project, ProjectMember, ProjectPermissions> _permissionEvaluator;
    private readonly TestDbContext _testDbContext;

    public GetTestMetaDataService(IPermissionEvaluator<Project, ProjectMember, ProjectPermissions> permissionEvaluator, TestDbContext testDbContext)
    {
        _permissionEvaluator = permissionEvaluator;
        _testDbContext = testDbContext;
    }

    public async Task<IActionResult> GetTestMetaData(GetTestMetadataRequest request)
    {

        return await _permissionEvaluator.Evaluate(request.ProjectId, p => true, async (_, _) =>
        {

            var test = await _testDbContext.Tests
                .Where(t => t.Id == request.TestId)
                .Select(t => new GetTestMetaDataResponse
                {
                    Name = t.Name,
                    Description = t.Description ?? string.Empty,
                    StartedAt = t.StartTime
                })
                .SingleOrDefaultAsync();
            if (test is null) return new NotFoundResult();

            return new OkObjectResult(test);

        });

    }
    
}