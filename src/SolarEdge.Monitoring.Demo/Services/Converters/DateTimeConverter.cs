using Newtonsoft.Json.Converters;

namespace SolarEdge.Monitoring.Demo.Services.Converters
{
	public class DateFormatConverter : IsoDateTimeConverter
	{
		public DateFormatConverter(string format)
		{
			DateTimeFormat = format;
		}
	}
}
