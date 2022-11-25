using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quotation.Management.Contracts.Services;
using Quotation.Management.Entities.Models;

namespace QMT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly ISeriesService _seriesService;
        public SeriesController(ISeriesService seriesService)
        {
            _seriesService = seriesService ?? throw new ArgumentNullException(nameof(seriesService));
        }

        [HttpGet]
        [ProducesResponseType(typeof(SeriesMaster), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetSeries()
        {
            var _brandList = _seriesService.GetSeries();
            return Ok(_brandList);
        }

        [HttpPost]
        [ProducesResponseType(typeof(SeriesMaster), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertSeries(SeriesMaster series)
        {
            var _brand = _seriesService.InsertSeries(series);
            return Ok(_brand);
        }
    }
}
