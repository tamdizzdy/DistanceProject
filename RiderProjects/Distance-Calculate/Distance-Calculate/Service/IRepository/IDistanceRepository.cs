using Distance_Calculate.Model;
using Microsoft.AspNetCore.Mvc;

namespace Distance_Calculate.Interface;

public interface IDistanceRepository
{
    Task<Object> CalculateDistanceSpeedTime(int userId, double latitude, double longitude);
    Task<Object> GetUserById(int userId);
}