using AutoMapper;
using HotelLstWebApi.IRepository;
using HotelLstWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelLstWebApi.Controllers
{
    [ApiVersion("2.0", Deprecated =true)]
    //[Route("api/{v:apiversion}/country")]
    [Route("api/country")]
    [ApiController]
    public class CountryV2Controller : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryV2Controller> _logger;
        private readonly IMapper _mapper;
        public CountryV2Controller(IUnitOfWork unitOfWork, ILogger<CountryV2Controller> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;

        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _unitOfWork.Countries.GetAll();
            var results = _mapper.Map<IList<CountryDTO>>(countries);
            return Ok(results);
        }
    }
}
