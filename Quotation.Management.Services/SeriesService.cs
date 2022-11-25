using Quotation.Management.Contracts.Repositories;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;
using Quotation.Management.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Services
{
    public class SeriesService : ISeriesService
    {
        private readonly ISeriesRepository<SeriesMaster> _seriesRepository;
        public SeriesService(ISeriesRepository<SeriesMaster> seriesRepository)
        {
            _seriesRepository = seriesRepository ?? throw new ArgumentNullException(nameof(seriesRepository));
        }
        public dynamic GetSeries()
        {
            return _seriesRepository.GetAll();
        }
        public SeriesMaster InsertSeries(SeriesMaster brand)
        {
            return _seriesRepository.InsertSeries(brand);
        }

    }
}
