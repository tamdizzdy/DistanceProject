using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Distance_Calculate.Interface;
using Distance_Calculate.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Distance_Calculate.Controllers
{
    [Route("api/v1/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repository;

        public UserController(IUserRepository repository)
        {
            _repository = repository;
        }

        [SwaggerOperation("GetAll List User")]
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var listUser = await _repository.GetAllUser();
            return Ok(listUser);
        }

        [SwaggerOperation("Get user By Id")]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            try
            {
                var user = await _repository.GetById(id);
                return Ok(user);
            }
            catch
            {
                return NoContent();
            }
        }

        [SwaggerOperation("Update User By Id")]
        [HttpPut("{userId}")]
        public async Task<ActionResult<User>> UpdateUser(int userId, UserUpdateDTO user)
        {
            try
            {
                if (userId != user.Id)
                {
                    return BadRequest("User ID mismatch");
                }

                var updateUser = await _repository.UpdateUser(userId,user);
                return Ok(updateUser);
            }
            catch
            {
                return Conflict();
            }
        }

        [SwaggerOperation("Delete User By Id")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> Delete(int id, bool? saveChangesError = false)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("userId Not found");
                }

                var updateUser = _repository.DeleteUser(id);
                return Ok(updateUser);
            }
            catch
            {
                return NoContent();
            }
        }
    }
}