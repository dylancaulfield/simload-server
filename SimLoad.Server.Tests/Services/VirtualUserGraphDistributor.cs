using SimLoad.Common.Models;
using SimLoad.Server.Tests.Requests;

namespace SimLoad.Server.Tests.Services;

/// <summary>
///     This class divides the user count evenly between a number of load generators
///     in a round robin fashion
/// </summary>
public static class VirtualUserGraphDistributor
{
    public static Dictionary<Guid, VirtualUserGraph> DistributeGraphs(StartTestRequest request)
    {
        var numberOfPoints = request.VirtualUserGraph.Points.Count;
        var loadGeneratorUserCountGraphDictionary =
            new Dictionary<Guid, VirtualUserGraph>(); // LoadGeneratorId -> VirtualUserGraph

        // For each load generator initialize a new graph
        foreach (var loadGeneratorId in request.LoadGeneratorIds)
        {
            loadGeneratorUserCountGraphDictionary[loadGeneratorId] = new VirtualUserGraph();

            // Add N points (0, 0) to be updated later
            for (var i = 0; i < numberOfPoints; i++)
                loadGeneratorUserCountGraphDictionary[loadGeneratorId].Points.Add(new Point
                {
                    X = 0,
                    Y = 0
                });
        }

        var lastLoadGeneratorIndex = request.LoadGeneratorIds.Count - 1;

        for (var i = 0; i < request.VirtualUserGraph.Points.Count; i++)
        {
            // Begin at the first load generator
            var currentLoadGeneratorIdIndex = 0;

            // For each point iterate until j equals the height of the graph
            for (var j = 0; j < request.VirtualUserGraph.Points[i].Y; j++)
            {
                var currentLoadGeneratorId = request.LoadGeneratorIds[currentLoadGeneratorIdIndex];

                // Add one to the Y value of the point for the current load generator
                loadGeneratorUserCountGraphDictionary[currentLoadGeneratorId].Points[i].Y++;
                loadGeneratorUserCountGraphDictionary[currentLoadGeneratorId].Points[i].X =
                    request.VirtualUserGraph.Points[i].X;

                // Cycle through the load generators continuously
                currentLoadGeneratorIdIndex++;
                if (currentLoadGeneratorIdIndex > lastLoadGeneratorIndex) currentLoadGeneratorIdIndex = 0;
            }
        }

        return loadGeneratorUserCountGraphDictionary;
    }
}