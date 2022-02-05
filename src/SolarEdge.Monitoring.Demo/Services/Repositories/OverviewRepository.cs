using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SolarEdge.Monitoring.Demo.Database;
using SolarEdge.Monitoring.Demo.Models;
using SolarEdge.Monitoring.Demo.Services.Repositories.Base;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace SolarEdge.Monitoring.Demo.Services.Repositories
{
	public class OverviewRepository : BaseRepository<Overview>, IOverviewRepository
	{
		private readonly ILogger<OverviewRepository> _logger;

		public OverviewRepository(DataContext appDbContext, ILogger<OverviewRepository> logger)
			: base(appDbContext)
		{
			_logger = logger;
		}

		public async Task<Overview> GetFirstAsync(CancellationToken cancellationToken = default, bool disableTracking = true)
		{
			_logger.LogDebug(nameof(GetFirstAsync));
			IQueryable<Overview> query = DatabaseSet;
			if (disableTracking)
			{
				query = query.AsNoTracking();
			}
			var result = await query
				.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
			return result;
		}

		public override async Task<Overview> GetSingleAsync(Expression<Func<Overview, bool>> predicate, CancellationToken cancellationToken = default, bool disableTracking = true)
		{
			_logger.LogDebug(nameof(GetSingleAsync));
			IQueryable<Overview> query = DatabaseSet;
			if (disableTracking)
			{
				query = query.AsNoTracking();
			}
			var result = await query
				.SingleOrDefaultAsync(predicate, cancellationToken).ConfigureAwait(false);
			return result;
		}

		public override async Task<IList<Overview>> GetAllAsync(CancellationToken cancellationToken = default, bool disableTracking = true)
		{
			_logger.LogDebug(nameof(GetAllAsync));
			IQueryable<Overview> query = DatabaseSet;
			if (disableTracking)
			{
				query = query.AsNoTracking();
			}
			var result = await query
				.ToListAsync(cancellationToken).ConfigureAwait(false);
			return result;
		}
	}
}
