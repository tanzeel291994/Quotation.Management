using Quotation.Management.Contracts.Repositories;
using Quotation.Management.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quotation.Management.Repositories
{
    public class SeriesRepository : ISeriesRepository<SeriesMaster>
    {
        public SeriesRepository()
        {

        }

        public  dynamic GetAll ()
        {

            using (var context = new QMTContext())
            {
               return context.SeriesMasters.Select(x => new 
               {
                   BrandName = x.Brand!.BrandName,
                   SeriesName = x.SeriesName,
                   GroupName= x.Group!.GroupName,
                   SeriesId =  x.SeriesId
               }).ToList();
            }
        }

        public SeriesMaster? GetSeries(string seriesName)
        {

            using (var context = new QMTContext())
            {
                return context.SeriesMasters.Where(x => x.SeriesName == seriesName.ToUpper()).FirstOrDefault();
            }
        }

        public SeriesMaster InsertSeriesIfNotExist(SeriesMaster _series, QMTContext? _context = null)
        {
            //using (var context = _context ?? new QMTContext())
            //{
                var context = _context ?? new QMTContext();
                SeriesMaster? seriesMaster = context.SeriesMasters.Where(x => x.GroupId == _series.GroupId && x.BrandId == _series.BrandId && x.SeriesName == _series.SeriesName).FirstOrDefault();
                if (seriesMaster == null)
                {
                    context.SeriesMasters.Add(_series);
                    context.SaveChanges();
                    return _series;
                }
                return seriesMaster;
            //}
        }


        public SeriesMaster InsertSeries(SeriesMaster series)
        {

            using (var context = new QMTContext())
            {
                SeriesMaster _series = new SeriesMaster();
                _series.SeriesName = series.SeriesName;
                _series.GroupId = series.GroupId;
                _series.BrandId = series.BrandId;
                context.SeriesMasters.Add(_series);
                context.SaveChanges();
                return _series;
            }
        }
    }
}
