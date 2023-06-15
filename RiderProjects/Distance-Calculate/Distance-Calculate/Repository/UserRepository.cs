using Distance_Calculate.Data;
using Distance_Calculate.Interface;
using Distance_Calculate.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Distance_Calculate.Repository;

public class UserRepository : IUserRepository
{
    private readonly WebAPIContext _context;

    public UserRepository(WebAPIContext context)
    {
        _context = context;
    }


    public async Task<ActionResult<List<User>>> GetAllUser()
    {
        var listUser = await _context.Users.ToListAsync();
        return listUser;
    }

    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (id == null) throw new Exception("userId Not found");
        return user;
    }

    public async Task<ActionResult<User>> UpdateUser(int id, UserUpdateDTO user)
    {
        var findUser = await _context.Users.FindAsync(id);

        if (findUser == null)
        {
            throw new Exception("userId Not found");
        }

        findUser.Avatar = user.Avatar;
        findUser.Email = user.Email;
        findUser.Username = user.UserName;
        findUser.PhoneNumber = user.PhoneNumber;
        findUser.DOB = user.DOB;

        _context.Entry(findUser).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        
        return findUser;
    }

    public async Task<ActionResult<User>> DeleteUser(int id, bool? saveChangesError)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            throw new Exception("userId Not found");
        }

        _context.Entry(user).State = EntityState.Deleted;
        await _context.SaveChangesAsync();
        
        return user;
    }
}