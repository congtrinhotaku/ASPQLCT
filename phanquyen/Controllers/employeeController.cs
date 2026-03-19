using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using phanquyen.Models;
using phanquyen.DTOs;
namespace phanquyen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class employeeController : ControllerBase
    {
        private readonly LoginAppContext _context;
        private readonly IConfiguration _config;

        public employeeController(LoginAppContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        public IActionResult GetAll()
        {

            var employees = _context.Employees
                .Include(e => e.Departments)
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FullName,
                    e.Email,
                    e.Phone,
                    e.Address,
                    e.Position,
                    e.BaseSalary,
                    e.Status,
                    Departments = e.Departments.Select(d => new { d.DepartmentId, d.Name }).ToList()
                })
                .ToList();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var employee = _context.Employees
                .Include(e => e.Departments)
                .Where(e => e.EmployeeId == id)
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FullName,
                    e.Email,
                    e.Phone,
                    e.Address,
                    e.Position,
                    e.BaseSalary,
                    e.Status,
                    Departments = e.Departments.Select(d => new { d.DepartmentId, d.Name }).ToList()
                })
                .FirstOrDefault();

            if (employee == null)
            {
                return NotFound("Không tìm thấy nhân viên");
            }
            return Ok(employee);
        }

        [Authorize(Roles = "HR,CEO")]
        [HttpPost("create")]
        public IActionResult CreateEmployee([FromBody] EmployeeRequest request)
        {
            if (request == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }
            if (string.IsNullOrEmpty(request.FullName) || string.IsNullOrEmpty(request.Email))
            {
                return BadRequest("FullName và Email là bắt buộc");
            }
            
            var employee = new Employee
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                Position = request.Position,
                BaseSalary = request.BaseSalary,
                Status = request.Status
            };

           
            if (request.DepartmentIds != null && request.DepartmentIds.Count > 0)
            {
                var departments = _context.Departments.Where(d => request.DepartmentIds.Contains(d.DepartmentId)).ToList();
                foreach (var dept in departments)
                {
                    employee.Departments.Add(dept);
                }
            }

            _context.Employees.Add(employee);
            _context.SaveChanges();


            var result = new
            {
                employee.EmployeeId,
                employee.FullName,
                employee.Email,
                employee.Phone,
                employee.Address,
                employee.Position,
                employee.BaseSalary,
                employee.Status,
                Departments = employee.Departments.Select(d => new { d.DepartmentId, d.Name }).ToList()
            };

            return CreatedAtAction(nameof(GetById), new { id = employee.EmployeeId }, result);
        }

        [Authorize(Roles = "HR,CEO")]
        [HttpPut("update/{id}")]
        public IActionResult UpdateEmployee(int id, [FromBody] EmployeeRequest request)
        {
          
            var existingEmployee = _context.Employees
                .Include(e => e.Departments)
                .FirstOrDefault(e => e.EmployeeId == id);

            if (existingEmployee == null)
            {
                return NotFound("Không tìm thấy nhân viên");
            }

            existingEmployee.FullName = request.FullName;
            existingEmployee.Email = request.Email;
            existingEmployee.Phone = request.Phone;
            existingEmployee.Address = request.Address;
            existingEmployee.Position = request.Position;
            existingEmployee.BaseSalary = request.BaseSalary;
            existingEmployee.Status = request.Status;

           
            existingEmployee.Departments.Clear();
            if (request.DepartmentIds != null && request.DepartmentIds.Count > 0)
            {
                var departments = _context.Departments.Where(d => request.DepartmentIds.Contains(d.DepartmentId)).ToList();
                foreach (var dept in departments)
                {
                    existingEmployee.Departments.Add(dept);
                }
            }

            _context.SaveChanges();

       
            var result = new
            {
                existingEmployee.EmployeeId,
                existingEmployee.FullName,
                existingEmployee.Email,
                existingEmployee.Phone,
                existingEmployee.Address,
                existingEmployee.Position,
                existingEmployee.BaseSalary,
                existingEmployee.Status,
                Departments = existingEmployee.Departments.Select(d => new { d.DepartmentId, d.Name }).ToList()
            };

            return Ok(result);
        }

        [Authorize(Roles = "HR,CEO")]
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.EmployeeId == id);
            if (employee == null)
            {
                return NotFound("Không tìm thấy nhân viên");
            }

            _context.Employees.Remove(employee);
            _context.SaveChanges();

            return Ok("Xóa thành công");
        }
    }

    
    
}
