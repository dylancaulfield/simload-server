using SimLoad.Common.Interfaces;

namespace SimLoad.Server.Results.Requests;

public class ResultBatchRequest
{
    public List<SerializableResult> Results { get; set; }
}