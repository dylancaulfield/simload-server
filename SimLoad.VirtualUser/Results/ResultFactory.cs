using SimLoad.Common.Interfaces;
using SimLoad.Common.Models;
using SimLoad.Common.Models.Scenario;
using SimLoad.LoadGenerator.Common;

namespace SimLoad.VirtualUser;

public interface IResultFactory
{
    SerializableResult CreateHttpResult(HttpRequestMessage httpRequest, Guid operationId, Guid virtualUserId,
        int responseCode,
        int duration);
}

public class ResultFactory : IResultFactory
{
    private readonly ClientInformation _clientInformation;
    private readonly Instruction _instruction;
    private readonly Scenario _scenario;

    public ResultFactory(IClientInformationProvider clientInformationProvider, Scenario scenario,
        Instruction instruction)
    {
        _clientInformation = clientInformationProvider.GetClientInformation();
        _scenario = scenario;
        _instruction = instruction;
    }

    public SerializableResult CreateHttpResult(HttpRequestMessage httpRequest, Guid operationId, Guid virtualUserId,
        int responseCode,
        int duration)
    {
        return new SerializableResult(DateTime.UtcNow, _instruction.TestId, operationId, virtualUserId,
            _scenario.ScenarioId,
            _clientInformation.LoadGeneratorId, httpRequest.Method.Method, httpRequest.RequestUri!.Host,
            httpRequest.RequestUri!.AbsolutePath,
            httpRequest.RequestUri!.Query,
            _scenario.Name, responseCode, duration);
    }
}