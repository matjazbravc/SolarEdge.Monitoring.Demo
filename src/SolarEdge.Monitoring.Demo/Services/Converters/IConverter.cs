namespace SolarEdge.Monitoring.Demo.Services.Converters
{
	/// <summary>
	/// Generic converter interface contract
	/// </summary>
	public interface IConverter<in TFrom, out TTo>
	{
		TTo Convert(TFrom source);
	}
}