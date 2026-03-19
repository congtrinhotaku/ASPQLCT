using System;
using System.Collections.Generic;

namespace phanquyen.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public string? Position { get; set; }

    public decimal? BaseSalary { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
}
