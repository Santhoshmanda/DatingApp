using Microsoft.AspNetCore.Mvc;
using API.Data; 
using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {   
         private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
              _context=context;
              _tokenService=tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {   
            if(await UserExists(registerDto.Username)) return BadRequest("UserName is taken");
            using var hmac = new HMACSHA512();

            var user= new AppUser{
                UserName=registerDto.Username.ToLower(),
                PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt=hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto{
                Username=user.UserName,
                Token=_tokenService.CreateToken(user)

            };

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x=>x.UserName==loginDto.Username); //diff between first or defaul and single or def will throw exception if more than one record
            if(user==null) return Unauthorized();
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for(int i=0;i<computedHash.Length;i++){
                if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid response");
                
            }
            return new UserDto{
                Username=user.UserName,
                Token=_tokenService.CreateToken(user)
            };

        }

        private async Task<bool> UserExists(string username)
        {
            return await  _context.Users.AnyAsync(x=>x.UserName==username.ToLower());
        }
    }
}