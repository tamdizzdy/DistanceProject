using Distance_Calculate.Model;
using Microsoft.AspNetCore.Mvc;

namespace Distance_Calculate.Interface;

public interface IUserRepository
{
      Task<ActionResult<List<User>>> GetAllUser();
      Task<ActionResult<User>> GetById(int id);
      Task<ActionResult<User>> UpdateUser(int id, UserUpdateDTO user);
      Task<ActionResult<User>> DeleteUser(int id, bool? saveChangesError = false);
}