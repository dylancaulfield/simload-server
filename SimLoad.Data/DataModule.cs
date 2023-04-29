using Autofac;
using SimLoad.Data.Entities.HttpRequest;
using SimLoad.Data.Entities.Project;

namespace SimLoad.Data;

public class DataModule : Module
{
	protected override void Load(ContainerBuilder builder)
	{

		builder.RegisterType<SimLoadMongoDatabase>().SingleInstance();

		// Queries
	}
	
}