using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SolarEdge.Monitoring.Demo.Models;

namespace SolarEdge.Monitoring.Demo.Services
{
	public interface IEnergyDetailsService
	{
		Task<IList<EnergyDetails>> GetEnergyDetailsTodayAsync(CancellationToken cancellationToken = default);

		Task<IList<EnergyDetails>> GetEnergyDetailsThisWeekAsync(CancellationToken cancellationToken = default);
		
		Task<IList<EnergyDetails>> GetEnergyDetailsThisMonthAsync(CancellationToken cancellationToken = default);
		
		Task<IList<EnergyDetails>> GetEnergyDetailsThisYearAsync(CancellationToken cancellationToken = default);

		Task UpdateEnergyDetailsThisMonthAsync(CancellationToken cancellationToken = default);

		Task UpdateEnergyDetailsThisWeekAsync(CancellationToken cancellationToken = default);

		Task UpdateEnergyDetailsThisYearAsync(CancellationToken cancellationToken = default);

		Task UpdateEnergyDetailsTodayAsync(CancellationToken cancellationToken = default);

		Task UpdateEnergyDetailsAsync(DateTime date, CancellationToken cancellationToken = default);
	}
}
