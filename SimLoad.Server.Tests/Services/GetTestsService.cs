using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Project;
using SimLoad.Server.Tests.Responses;

namespace SimLoad.Server.Tests.Services;

public interface IGetTestsService
{
    Task<IActionResult> GetTests(Guid projectId);
}

public class GetTestsService : IGetTestsService
{

    private readonly IPermissionEvaluator<Project, ProjectMember, ProjectPermissions> _permissionEvaluator;
    private readonly TestDbContext _testDbContext;

    public GetTestsService(IPermissionEvaluator<Project, ProjectMember, ProjectPermissions> permissionEvaluator, TestDbContext testDbContext)
    {
        _permissionEvaluator = permissionEvaluator;
        _testDbContext = testDbContext;
    }

    public async Task<IActionResult> GetTests(Guid projectId)
    {

        return await _permissionEvaluator.Evaluate(projectId, p => true, async (project, member) =>
        {

            var tests = await _testDbContext.Tests
                .Where(t => t.ProjectId == projectId)
                .OrderByDescending(t => t.StartTime)
                .Select(t => new GetTestsResponse
                {
                    Id = t.Id,
                    Name = t.Name,
                    StartedAt = t.StartTime,
                    EndedAt = t.StartTime.Add(t.Duration),
                    Status = t.StartTime.Add(t.Duration) > DateTime.UtcNow ? TestStatus.InProgress : TestStatus.Complete
                })
                .ToListAsync();

            return new OkObjectResult(tests);

        });

    }
    
}