using Microsoft.Extensions.Logging;
using SolarEdge.Monitoring.Demo.Models.Dto;
using SolarEdge.Monitoring.Demo.Models;
using System.Linq;
using System;

namespace SolarEdge.Monitoring.Demo.Services.Converters
{
	/// <summary>
	/// EnergyDetailsDto to EnergyDetails converter
	/// </summary>
	public class EnergyDetailsDtoToEnergyDetailsConverter : IConverter<EnergyDetailsDto, EnergyDetails>
	{
		private readonly ILogger<EnergyDetailsDtoToEnergyDetailsConverter> _logger;

		public EnergyDetailsDtoToEnergyDetailsConverter(ILogger<EnergyDetailsDtoToEnergyDetailsConverter> logger)
		{
			_logger = logger;
		}

		public EnergyDetails Convert(EnergyDetailsDto energyDetailsDto)
		{
			_logger.LogDebug(nameof(Convert));

			double consumption = 0;
			double purchased = 0;
			double selfConsumption = 0;
			double production = 0;
			double feedIn = 0;
			DateTime time = DateTime.Today;

			var firstMeter = energyDetailsDto.EnergyDetails.Meters.FirstOrDefault();
			var date = firstMeter?.Values.FirstOrDefault();
			if (date != null)
			{
				time = date.Date;
			}

			foreach (var energyDetailsMeter in energyDetailsDto.EnergyDetails.Meters)
			{
				switch (energyDetailsMeter.Type)
				{
					case "Consumption":
						consumption = energyDetailsMeter.Values[0].Value;
						break;
					case "Purchased":
						purchased = energyDetailsMeter.Values[0].Value;
						break;
					case "SelfConsumption":
						selfConsumption = energyDetailsMeter.Values[0].Value;
						break;
					case "Production":
						production = energyDetailsMeter.Values[0].Value;
						break;
					case "FeedIn":
						feedIn = energyDetailsMeter.Values[0].Value;
						break;
				}
			}

			var result = new EnergyDetails()
			{
				Consumption = consumption,
				Purchased = purchased,
				SelfConsumption = selfConsumption,
				Production = production,
				FeedIn = feedIn,
				Time = time
			};

			return result;
		}
	}
}