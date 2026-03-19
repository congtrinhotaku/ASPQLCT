using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using phanquyen.Models;

namespace phanquyen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class departmentController : ControllerBase
    {
        private readonly LoginAppContext _context;

        public departmentController(LoginAppContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var departments = _context.Departments.ToList();
            return Ok(departments);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var department = _context.Departments.FirstOrDefault(d => d.DepartmentId == id);
            if (department == null)
            {
                return NotFound("Không tìm thấy phòng ban");
            }
            return Ok(department);
        }

        [Authorize(Roles = "HR,CEO")]
        [HttpPost("create")]
        public IActionResult Create([FromBody] Department department)
        {
            if (department == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }
            if (string.IsNullOrEmpty(department.Name))
            {
                return BadRequest("Tên phòng ban là bắt buộc");
            }

            _context.Departments.Add(department);
            _context.SaveChanges();

            return Ok(department);
        }

        [Authorize(Roles = "HR,CEO")]
        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] Department department)
        {
            var existingDept = _context.Departments.FirstOrDefault(d => d.DepartmentId == id);
            if (existingDept == null)
            {
                return NotFound("Không tìm thấy phòng ban");
            }

            existingDept.Name = department.Name;
            _context.SaveChanges();

            return Ok(existingDept);
        }

        [Authorize(Roles = "HR,CEO")]
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var department = _context.Departments.FirstOrDefault(d => d.DepartmentId == id);
            if (department == null)
            {
                return NotFound("Không tìm thấy phòng ban");
            }

            _context.Departments.Remove(department);
            _context.SaveChanges();

            return Ok("Xóa thành công");
        }
    }
}