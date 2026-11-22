using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController(AppDbContext context , ITokenService tokenService): BaseApiController
    {
        [HttpPost("register")]//api/account/register
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await EmailExists(registerDTO.Email)) return BadRequest("Email already exists");
           
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            var user = new AppUser
            {
                DisplayName = registerDTO.DisplayName,
                Email = registerDTO.Email.ToLower(),
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerDTO.Password)),
                PasswordSalt = hmac.Key
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user.ToDto(tokenService);
        }
        [HttpPost("login")]//api/account/login
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await context.Users.SingleOrDefaultAsync(x => x.Email == loginDTO.Email.ToLower());
            if (user == null) return Unauthorized("Invalid email");
            using var hmac = new System.Security.Cryptography.HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginDTO.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
            }
            return user.ToDto(tokenService);
        }

        private async Task<bool> EmailExists(string email)
        {
            return await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }
    }
}