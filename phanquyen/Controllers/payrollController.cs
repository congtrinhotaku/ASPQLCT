using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using phanquyen.DTOs;
using phanquyen.Models;

namespace phanquyen.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class payrollController : ControllerBase
    {
        private readonly LoginAppContext _context;

        public payrollController(LoginAppContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Payroll,CEO")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var payrolls = _context.Payrolls
                .Include(p => p.Employee)
                .Select(p => new
                {
                    p.PayrollId,
                    p.EmployeeId,
                    EmployeeName = p.Employee.FullName,
                    p.Month,
                    p.Year,
                    p.BaseSalary,
                    p.Bonus,
                    p.Overtime,
                    p.Deduction,
                    p.NetSalary,
                    p.Status
                })
                .OrderByDescending(p => p.Year).ThenByDescending(p => p.Month)
                .ToList();
            return Ok(payrolls);
        }

   
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var payroll = _context.Payrolls
                .Include(p => p.Employee)
                .Where(p => p.PayrollId == id)
                .Select(p => new
                {
                    p.PayrollId,
                    p.EmployeeId,
                    EmployeeName = p.Employee.FullName,
                    p.Month,
                    p.Year,
                    p.BaseSalary,
                    p.Bonus,
                    p.Overtime,
                    p.Deduction,
                    p.NetSalary,
                    p.Status
                })
                .FirstOrDefault();

            if (payroll == null)
            {
                return NotFound("Không tìm thấy bảng lương.");
            }

            return Ok(payroll);
        }

    
        [HttpPost("create")]
        public IActionResult Create([FromBody] PayrollRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = _context.Employees.Find(request.EmployeeId);
            if (employee == null)
            {
                return NotFound($"Không tìm thấy nhân viên với ID {request.EmployeeId}.");
            }

            var existingPayroll = _context.Payrolls.FirstOrDefault(p =>
                p.EmployeeId == request.EmployeeId && p.Month == request.Month && p.Year == request.Year);

            if (existingPayroll != null)
            {
                return BadRequest($"Bảng lương cho nhân viên này trong tháng {request.Month}/{request.Year} đã tồn tại.");
            }

            var payroll = new Payroll
            {
                EmployeeId = request.EmployeeId,
                Month = request.Month,
                Year = request.Year,
                BaseSalary = employee.BaseSalary, 
                Bonus = request.Bonus,
                Overtime = request.Overtime,
                Deduction = request.Deduction,
                Status = request.Status ?? "Pending"
            };

            _context.Payrolls.Add(payroll);
            _context.SaveChanges();

            var result = _context.Payrolls
                .Where(p => p.PayrollId == payroll.PayrollId)
                .Include(p => p.Employee)
                .Select(p => new
                {
                    p.PayrollId,
                    p.EmployeeId,
                    EmployeeName = p.Employee.FullName,
                    p.Month,
                    p.Year,
                    p.BaseSalary,
                    p.Bonus,
                    p.Overtime,
                    p.Deduction,
                    p.NetSalary,
                    p.Status
                }).FirstOrDefault();

            return CreatedAtAction(nameof(GetById), new { id = payroll.PayrollId }, result);
        }

        [HttpPut("update/{id}")]
        public IActionResult Update(int id, [FromBody] PayrollUpdateRequest request)
        {
            var payroll = _context.Payrolls.Find(id);
            if (payroll == null)
            {
                return NotFound("Không tìm thấy bảng lương.");
            }

            payroll.Bonus = request.Bonus;
            payroll.Overtime = request.Overtime;
            payroll.Deduction = request.Deduction;
            payroll.Status = request.Status;

            _context.SaveChanges();

            var result = _context.Payrolls
                .Where(p => p.PayrollId == id)
                .Include(p => p.Employee)
                .Select(p => new
                {
                    p.PayrollId,
                    p.EmployeeId,
                    EmployeeName = p.Employee.FullName,
                    p.Month,
                    p.Year,
                    p.BaseSalary,
                    p.Bonus,
                    p.Overtime,
                    p.Deduction,
                    p.NetSalary,
                    p.Status
                }).FirstOrDefault();

            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            var payroll = _context.Payrolls.Find(id);
            if (payroll == null)
            {
                return NotFound("Không tìm thấy bảng lương.");
            }

            _context.Payrolls.Remove(payroll);
            _context.SaveChanges();

            return Ok("Xóa bảng lương thành công.");
        }
    }
}