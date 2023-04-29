using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using SimLoad.Common.Models;
using SimLoad.Server.Common.Authorization;
using SimLoad.Server.Data.Contexts;
using SimLoad.Server.Data.Entities.Organisation;
using SimLoad.Server.Data.Entities.Test;
using SimLoad.Server.Tests.Hubs;
using SimLoad.Server.Tests.Requests;

namespace SimLoad.Server.Tests.Services;

public interface IStartTestService
{
    Task<IActionResult> CreateTest(StartTestRequest request);
}

public class StartTestService : IStartTestService
{
    private readonly IHubContext<TestsHub> _instructionsHub;

    private readonly IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions>
        _permissionEvaluator;

    private readonly TestDbContext _testDbContext;
    private readonly LoadGeneratorDbContext _loadGeneratorDbContext;

    public StartTestService(IHubContext<TestsHub> instructionsHub, IPermissionEvaluator<Organisation, OrganisationMember, OrganisationPermissions> permissionEvaluator, TestDbContext testDbContext, LoadGeneratorDbContext loadGeneratorDbContext)
    {
        _instructionsHub = instructionsHub;
        _permissionEvaluator = permissionEvaluator;
        _testDbContext = testDbContext;
        _loadGeneratorDbContext = loadGeneratorDbContext;
    }

    public async Task<IActionResult> CreateTest(StartTestRequest request)
    {
        return await _permissionEvaluator.Evaluate(request.OrganisationId, p => p.TestRun,
            async (_, _) =>
            {

                var loadGeneratorsAvailable = await LoadGeneratorsAvailable(request.LoadGeneratorIds);
                if (!loadGeneratorsAvailable)
                {
                    return new BadRequestObjectResult(new Exception("Load generators are not all available"));
                }
                
                // Create test
                var test = new Test
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    ProjectId = request.ProjectId,
                    StartTime = DateTime.UtcNow,
                    Duration = request.Duration,
                    ScenarioWeights = new List<TestScenarioWeight>(),
                    VirtualUserGraphs = new List<TestVirtualUserGraph>()
                };

                // Add scenario weights
                foreach (var scenario in request.Scenarios)
                    test.ScenarioWeights.Add(new TestScenarioWeight
                    {
                        Id = Guid.NewGuid(),
                        ScenarioId = scenario.Id,
                        Weight = scenario.Weight,
                        TestId = test.Id,
                        Test = test
                    });

                var balancedGraphs = VirtualUserGraphDistributor.DistributeGraphs(request);

                // Add virtual user graphs
                foreach (var (loadGeneratorId, graph) in balancedGraphs)
                foreach (var point in graph.Points)
                    test.VirtualUserGraphs.Add(new TestVirtualUserGraph
                    {
                        Id = Guid.NewGuid(),
                        LoadGeneratorId = loadGeneratorId,
                        X = point.X,
                        Y = point.Y,
                        TestId = test.Id,
                        Test = test
                    });

                // Save to database
                _testDbContext.Tests.Add(test);
                await _testDbContext.SaveChangesAsync();

                var instructionTasks = new List<Task>();

                // Send instructions to load generators
                foreach (var loadGeneratorId in request.LoadGeneratorIds)
                {
                    var instruction = new Instruction
                    {
                        ProjectId = test.ProjectId,
                        TestId = test.Id,
                        LoadGeneratorId = loadGeneratorId,
                        Cancel = false
                    };

                    instructionTasks.Add(
                        _instructionsHub
                            .Clients
                            .Group(request.OrganisationId.ToString())
                            .SendAsync("ReceiveInstruction", instruction)
                    );
                }

                await Task.WhenAll(instructionTasks);

                return new OkObjectResult(test.Id);
            });
    }

    private async Task<bool> LoadGeneratorsAvailable(List<Guid> loadGeneratorIds)
    {

        var loadGenerators = await _loadGeneratorDbContext.LoadGeneratorConnections
            .Where(c => loadGeneratorIds.Contains(c.Id))
            .ToListAsync();

        return loadGenerators.All(l => l.Available);

    }
    
}