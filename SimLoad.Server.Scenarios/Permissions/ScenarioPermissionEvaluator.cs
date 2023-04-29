using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimLoad.Server.Common;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Scenario;

namespace SimLoad.Server.Scenarios.Permissions;

public class ScenarioPermissionEvaluator : IPermissionEvaluator<Scenario, ScenarioMember, ScenarioPermissions>
{
    private readonly ScenarioDbContext _scenarioDbContext;
    private readonly Guid? _userId;

    public ScenarioPermissionEvaluator(ScenarioDbContext scenarioDbContext, IRequestContext requestContext)
    {
        _scenarioDbContext = scenarioDbContext;
        _userId = requestContext.UserId;
    }

    public async Task<IActionResult> Evaluate(Guid entityId, Func<ScenarioPermissions, bool> hasPermission,
        Func<Scenario, ScenarioMember, Task<IActionResult>> onPermissionAllowed)
    {
        var scenario = await _scenarioDbContext.Scenarios.FindAsync(entityId);
        if (scenario is null) return new NotFoundResult();

        var scenarioPermissions = await _scenarioDbContext.ScenarioPermissions
            .Include(sp => sp.ScenarioMember)
            .ThenInclude(sm => sm.ProjectMember)
            .ThenInclude(pm => pm.OrganisationMember)
            .SingleOrDefaultAsync(sp => sp.ScenarioMember.ProjectMember.OrganisationMember.UserId == _userId
                                        && sp.ScenarioMember.ScenarioId == entityId);
        if (scenarioPermissions is null || !hasPermission(scenarioPermissions)) return new ForbidResult();

        return await onPermissionAllowed(scenario, scenarioPermissions.ScenarioMember);
    }
}