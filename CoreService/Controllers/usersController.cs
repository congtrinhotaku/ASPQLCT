    
using BCrypt.Net;
using CoreService.DTOs;
using CoreService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CoreService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userController : ControllerBase
    {
        private readonly LoginAppContext _context;

        public userController(LoginAppContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "CEO")]
        [HttpGet]
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
        [HttpPut("{id}/role")]
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

            return Ok(new
            {
                message = "Cập nhật vai trò người dùng thành công.",
                user = new
                {
                    userToUpdate.Id,
                    userToUpdate.Username,
                    userToUpdate.FullName,
                    userToUpdate.Role
                }
            });
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

            return Ok(new
            {
                message = "Đăng ký thành công",
                user = new
                {
                    user.Id,
                    user.Username,
                    user.FullName,
                    user.Role
                }
            });
        }
    }
}