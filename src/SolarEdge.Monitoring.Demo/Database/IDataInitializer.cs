using System.Threading.Tasks;

namespace SolarEdge.Monitoring.Demo.Database
{
	public interface IDataInitializer
	{
		Task Initialize();
	}
}