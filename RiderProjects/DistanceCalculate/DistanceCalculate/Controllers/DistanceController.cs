using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Distance_Calculate.Data;
using Distance_Calculate.Interface;
using Distance_Calculate.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Distance_Calculate.Controllers
{
    [Route("api/v1/distance-calculate")]
    [ApiController]
    public class DistanceController : ControllerBase
    {
        private readonly WebAPIContext _context;
        private readonly IDistanceRepository _repository;

        public DistanceController(IDistanceRepository repository, WebAPIContext context)
        {
            _repository = repository;
            _context = context;
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
            catch(Exception ex)
            {
                return BadRequest("UserId notfound or User not start");
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
            catch (Exception ex)
            {
                return BadRequest("UserId notfound or User not start");
            }
        }
    }
}
