using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCore_Task.Data;
using NetCore_Task.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace NetCore_Task.Controllers

{
    [ApiController]
    [Route("api/[Controller]")]
    public class UserController : Controller
    {
        //the create already in Register
        WebAPIContext _context;
        public UserController(WebAPIContext context) 
        {
            _context = context;
        }
        [SwaggerOperation("Get List User")]
        [HttpGet]
        public async Task<ActionResult<List<User>>> index()
        {
            return Ok(await _context.Users.ToListAsync());
        }
        [SwaggerOperation("Get user By Id")]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            return Ok(user);
        }
        [SwaggerOperation("Update User By Id")]
        [HttpPut]
        public async Task<ActionResult<User>> UpdateUser(int id, UserUpdateDTO user) 
        {
            var findUser = await _context.Users.FindAsync(id);

            if (findUser == null) 
            {
                return NotFound();
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            findUser.Avatar = user.Avatar;
            findUser.Email = user.Email;
            findUser.Username = user.UserName;
            findUser.PhoneNumber = user.PhoneNumber;
            findUser.DOB = user.DOB;

            _context.Entry(findUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                
            }
            catch (DbUpdateConcurrencyException) {
                ModelState.AddModelError("", "Unable to save change. " +
                        "Try Again, if you have problem persists, " +
                        "Contact your system administrator");
            }

            return Ok(findUser);
        }
        [SwaggerOperation("Delete User By Id")]
        [HttpDelete]
        public async Task<ActionResult<User>> Delete(int id, bool? saveChangesError = false) 
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if(user == null) 
            {
                return NotFound();
            }

            try {
                _context.Entry(user).State = EntityState.Deleted;
                await _context.SaveChangesAsync();

                

            } catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "Unable to save change. " +
                        "Try Again, if you have problem persists, " +
                        "Contact your system administrator");
            }

            return Ok();
        }
    }
}