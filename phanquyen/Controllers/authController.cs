using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using phanquyen.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using phanquyen.DTOs;

namespace phanquyen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class authController : ControllerBase
    {

        private readonly LoginAppContext _context;
        private readonly IConfiguration _config;

        public authController(LoginAppContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == request.Username);

            if (user == null)
                return Unauthorized("Sai username");

            bool check = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            if (!check)
                return Unauthorized("Sai password");

            var token = GenerateToken(user);

            return Ok(new { token, user.FullName, user.Role });
        }

        [Authorize(Roles = "CEO")]
        [HttpPost("register")]
        public IActionResult register([FromBody] RegisterRequest request)
        {

            var existingUser = _context.Users.FirstOrDefault(x => x.Username == request.Username);
            if (existingUser != null)
            {
                return BadRequest("Username đã tồn tại");
            }
            var hashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User
            {
                Username = request.Username,
                FullName = request.FullName,
                Password = hashPassword,
                Role = string.IsNullOrWhiteSpace(request.role) ? "none" : request.role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { 
                message = "Đăng ký thành công",
                user = new {
                    user.Id,
                    user.Username,
                    user.FullName,
                    user.Role
                }
            });
        }

        private string GenerateToken(User user)
        {
            var jwt = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Authorize(Roles = "CEO")]
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users
                .Select(u => new {
                    u.Id,
                    u.Username,
                    u.FullName,
                    u.Role
                })
                .ToList();
            return Ok(users);
        }

        [Authorize(Roles = "CEO")]
        [HttpPut("users/{id}/role")]
        public IActionResult UpdateUserRole(int id, [FromBody] UpdateRoleRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userToUpdate = _context.Users.Find(id);
            if (userToUpdate == null)
            {
                return NotFound("Không tìm thấy người dùng.");
            }

            userToUpdate.Role = request.Role;
            _context.SaveChanges();

            return Ok(new {
                message = "Cập nhật vai trò người dùng thành công.",
                user = new {
                    userToUpdate.Id,
                    userToUpdate.Username,
                    userToUpdate.FullName,
                    userToUpdate.Role
                }
            });
        }
    }
}
