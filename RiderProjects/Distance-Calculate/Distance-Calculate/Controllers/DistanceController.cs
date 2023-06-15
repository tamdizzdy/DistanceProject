using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Distance_Calculate.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Distance_Calculate.Controllers
{
    [Route("api/v1/distance-calculate")]
    [ApiController]
    public class DistanceController : ControllerBase
    {
        private readonly IDistanceRepository _repository;

        public DistanceController(IDistanceRepository repository)
        {
            _repository = repository;
        }

        [SwaggerOperation("Get calculate-distance-speed-time by longitude,latitude")]
        [HttpGet("calculate-distance-time/ByUserId")]
        public async Task<IActionResult> CalculateDistance2(int userId, double latitude, double longitude)
        {
            try
            {
                var calculateResult = await _repository.CalculateDistanceSpeedTime(userId, latitude, longitude);
                return Ok(calculateResult);
            }
            catch
            {
                return Conflict();
            }
        }

        [SwaggerOperation("Get user description by Id")]
        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var calculateResult = await _repository.GetUserById(userId);
                return Ok(calculateResult);
            }
            catch
            {
                return Conflict();
            }
        }
    }
}
