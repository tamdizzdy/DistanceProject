using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCore_Task.Data;
using NetCore_Task.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace NetCore_Task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalculateDistanceController : ControllerBase
    {
        private readonly WebAPIContext _context;

        public CalculateDistanceController(WebAPIContext context)
        {
            _context = context;
        }

        private DateTime previousUpdateTime;
        private double previousLatitude;
        private double previousLongitude;
        private bool canUpdatePosition = true;

        private double CalculateDistance(double startLatitude, double startLongitude, double endLatitude,
            double endLongitude)
        {
            const double earthRadiusKm = 6371; // Bán kính Trái Đất tính theo kilômét

            // Chuyển đổi độ sang radianasas
            double lat1 = DegreesToRadians(startLatitude);
            double lon1 = DegreesToRadians(startLongitude);
            double lat2 = DegreesToRadians(endLatitude);
            double lon2 = DegreesToRadians(endLongitude);

            // Tính toán haversine
            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = earthRadiusKm * c;

            return distance;
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        private double CalculateSpeed(double distance, double time)
        {
            // Tính toán vận tốc (kích cỡ và đơn vị vận tốc phụ thuộc vào yêu cầu của bạn)
            double speed = distance / time;

            return speed;
        }

        private double CalculateTime(double distance, double speed)
        {
            // Tính toán thời gian (đơn vị thời gian phụ thuộc vào yêu cầu của bạn)
            double time = distance / speed;

            return time;
        }

        [SwaggerOperation("Get calculate-distance-speed-time by longitude,latitude")]
        [HttpGet("calculate-distance-time/ByUserId")]
        public async Task<IActionResult> CalculateDistance2(int userId, double latitude, double longitude)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (userId != user.Id)
                {
                    return BadRequest("UserId mismatch");
                }

                if (user.Id == null)
                {
                    return NotFound("UserId does not exist");
                }

                // Kiểm tra nếu đã có vị trí trước đó và đã đủ 10 giây kể từ lần cập nhật trước đó
                if (previousUpdateTime != null && DateTime.UtcNow.Subtract(previousUpdateTime).TotalSeconds < 10)
                {
                    return BadRequest("Chưa đủ thời     gian để cập nhật vị trí mới.");
                }

                if (previousUpdateTime == null || DateTime.UtcNow.Subtract(previousUpdateTime).TotalSeconds >= 10)
                {
                    // Lấy kết quả tính toán gần nhất
                    CalculationResultDistance latestResult = await _context.CalculationResult
                        .Where(c => c.UserId == userId)
                        .OrderByDescending(r => r.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (latestResult == null || DateTime.UtcNow.Subtract(latestResult.CreatedAt).TotalSeconds >= 10)
                    {
                        // Nếu không có kết quả tính toán trước đó hoặc đã đủ thời gian để cập nhật vị trí mới, thực hiện cập nhật
                        if (canUpdatePosition)
                        {
                            double time;
                            double timeFromLastUpdate = latestResult != null
                                ? DateTime.UtcNow.Subtract(latestResult.CreatedAt).TotalSeconds
                                : 0;

                            // Tính toán khoảng cách và thời gian dựa trên vị trí mới và vị trí trước đó
                            double distance =
                                CalculateDistance(previousLatitude, previousLongitude, latitude, longitude);
                            double speed = CalculateSpeed(distance, 10); // Giả sử thời gian di chuyển là 10 giây
                            if (timeFromLastUpdate == 0)
                            {
                                time = CalculateTime(distance, speed);
                            }
                            else
                            {
                                time = timeFromLastUpdate + CalculateTime(distance, speed) - 10;
                            }

                            // Cập nhật vị trí trước đó và thời gian cập nhật trước đó
                            previousLatitude = latitude;
                            previousLongitude = longitude;
                            previousUpdateTime = DateTime.UtcNow;

                            // Không cho phép cập nhật trong khoảng thời gian 10 giây
                            canUpdatePosition = false;

                            // Lưu kết quả tính toán vào cơ sở dữ liệu
                            CalculationResultDistance result = new CalculationResultDistance
                            {
                                Distance = distance,
                                Speed = speed,
                                Time = time,
                                CreatedAt = DateTime.UtcNow,
                                UserId = userId
                            };

                            _context.CalculationResult.Add(result);
                            await _context.SaveChangesAsync();

                            // // Tính trung bình cộng của các trường distance, speed và time
                            // double averageDistance = await _context.CalculationResult.AverageAsync(r => r.Distance);
                            // double averageSpeed = await _context.CalculationResult.AverageAsync(r => r.Speed);
                            // double averageTime = await _context.CalculationResult.AverageAsync(r => r.Time);
                            double totalDistance = await _context.CalculationResult.Where(c => c.UserId == userId)
                                .SumAsync(r => r.Distance);

                            // Lấy tổng thời gian từ cơ sở dữ liệu
                            double totalTravelTime = await _context.CalculationResult.Where(c => c.UserId == userId)
                                .SumAsync(r => r.Time);

                            // Tính vận tốc trung bình
                            double averageSpeed = totalDistance / totalTravelTime;

                            return Ok(new
                            {
                                Distance = distance,
                                Speed = speed,
                                Time = time,
                                // AverageDistance = averageDistance,
                                AverageSpeed = averageSpeed,
                                // AverageTime = averageTime,
                                TotalDistanceFromStart = totalDistance,
                                TotalTimeFromStart = totalTravelTime
                            });
                        }
                        else
                        {
                            return BadRequest("Chưa đủ thời gian để cập nhật vị trí mới.");
                        }
                    }
                    else
                    {
                        // Kiểm tra thời gian để gửi vị trí tiếp theo
                        DateTime nextPositionUpdateAt = latestResult.CreatedAt.AddSeconds(10);
                        TimeSpan timeUntilNextUpdate = nextPositionUpdateAt.Subtract(DateTime.UtcNow);

                        if (timeUntilNextUpdate.TotalSeconds > 0)
                        {
                            // Trả về thời gian cần chờ để gửi vị trí tiếp theo
                            return Ok(new { TimeUntilNextUpdate = timeUntilNextUpdate.TotalSeconds });
                        }
                        else
                        {
                            // Nếu đã đủ thời gian để cập nhật vị trí
                            if (canUpdatePosition)

                            {
                                double time;
                                double timeFromLastUpdate = latestResult != null
                                    ? DateTime.UtcNow.Subtract(latestResult.CreatedAt).TotalSeconds
                                    : 0;
                                // Tính toán khoảng cách và thời gian dựa trên vị trí mới và vị trí trước đó
                                double distance =
                                    CalculateDistance(previousLatitude, previousLongitude, latitude, longitude);
                                double speed = CalculateSpeed(distance, 10); // Giả sử thời gian di chuyển là 10 giây
                                if (timeFromLastUpdate == 0)
                                {
                                    time = CalculateTime(distance, speed);
                                }
                                else
                                {
                                    time = timeFromLastUpdate + CalculateTime(distance, speed) - 10;
                                }

                                // Cập nhật vị trí trước đó và thời gian cập nhật trước đó
                                previousLatitude = latitude;
                                previousLongitude = longitude;
                                previousUpdateTime = DateTime.UtcNow;

                                // Không cho phép cập nhật trong khoảng thời gian 10 giây
                                canUpdatePosition = false;

                                // Lưu kết quả tính toán vào cơ sở dữ liệu
                                CalculationResultDistance result = new CalculationResultDistance
                                {
                                    Distance = distance,
                                    Speed = speed,
                                    Time = time,
                                    CreatedAt = DateTime.UtcNow,
                                    UserId = userId
                                };

                                _context.CalculationResult.Add(result);
                                await _context.SaveChangesAsync();

                                // Tính trung bình cộng của các trường distance, speed và time
                                double averageDistance = await _context.CalculationResult.Where(c => c.UserId == userId)
                                    .AverageAsync(r => r.Distance);
                                double averageSpeed = await _context.CalculationResult.Where(c => c.UserId == userId)
                                    .AverageAsync(r => r.Speed);
                                double averageTime = await _context.CalculationResult.Where(c => c.UserId == userId)
                                    .AverageAsync(r => r.Time);

                                // Trả về kết quả
                                return Ok(new
                                {
                                    Distance = distance,
                                    Speed = speed,
                                    Time = time,
                                    AverageDistance = averageDistance,
                                    AverageSpeed = averageSpeed,
                                    AverageTime = averageTime
                                });
                            }
                            else
                            {
                                return BadRequest("Chưa đủ thời gian để cập nhật vị trí mới.");
                            }
                        }
                    }
                }

                return BadRequest("Không thể tính toán vị trí mới.");
            }
            catch (Exception)
            {
                return Ok(new
                {
                    message = "UserId does not exist"
                });
            }
        }
        [SwaggerOperation("Get user description by Id")]
        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var uu = await _context.Users.FindAsync(userId);

                if (userId != uu.Id)
                {
                    return BadRequest("UserId mismatch");
                }

                if (uu.Id == null)
                {
                    return NotFound("UserId does not exist");
                }

                var user = await (from u in _context.Users
                        join c in _context.CalculationResult on u.Id equals c.UserId
                        where userId == u.Id
                        select new UserModel()
                        {
                            userId = u.Id,
                            Username = u.Username,
                            PhoneNumber = u.PhoneNumber,
                            DOB = u.DOB,
                        }
                    ).FirstOrDefaultAsync();

                double totalDistance = await _context.CalculationResult.Where(c => c.UserId == userId)
                    .SumAsync(r => r.Distance);

                double totalTravelTime =
                    await _context.CalculationResult.Where(c => c.UserId == userId).SumAsync(r => r.Time);

                double averageSpeed = totalDistance / totalTravelTime;

                int count = await _context.CalculationResult.CountAsync(c => c.UserId == userId);
                return Ok(new
                {
                    AverageSpeed = averageSpeed,
                    TotalDistanceFromStart = totalDistance,
                    TotalTimeFromStart = totalTravelTime,
                    CountReq = count
                });
            }
            catch (Exception)
            {
                return Ok(new
                {
                    message = "UserId does not exist"
                });
            }
        }
    }
}