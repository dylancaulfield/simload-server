using System.Text;

namespace SimLoad.LoadGenerator.Common;

public interface IClientInformationProvider
{
    ClientInformation GetClientInformation();
}

public class ClientInformationProvider : IClientInformationProvider
{
    public ClientInformation GetClientInformation()
    {
        Guid clientId;

        if (!File.Exists("client-id.txt"))
        {
            using (var file = File.OpenWrite("client-id.txt"))
            {
                clientId = Guid.NewGuid();
                var bytes = Encoding.UTF8.GetBytes(clientId.ToString());
                file.Write(bytes);
            }

            return new ClientInformation
            {
                LoadGeneratorId = clientId
            };
        }

        var clientIdString = File.ReadAllText("client-id.txt", Encoding.UTF8);

        return new ClientInformation
        {
            LoadGeneratorId = Guid.Parse(clientIdString)
        };
    }
}

public class ClientInformation
{
    public Guid LoadGeneratorId { get; init; }
}