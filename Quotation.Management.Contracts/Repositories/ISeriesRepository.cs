using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Contracts.Repositories
{
    public interface ISeriesRepository<T> where T :class
    {
        dynamic GetAll();

        SeriesMaster InsertSeries(SeriesMaster brand);

        SeriesMaster InsertSeriesIfNotExist(SeriesMaster _series, QMTContext? _context = null);

        SeriesMaster? GetSeries(string seriesName);
    }
}
