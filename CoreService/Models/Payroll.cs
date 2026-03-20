using System;
using System.Collections.Generic;

namespace CoreService.Models;

public partial class Payroll
{
    public int PayrollId { get; set; }

    public int? EmployeeId { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public decimal? BaseSalary { get; set; }

    public decimal? Bonus { get; set; }

    public decimal? Overtime { get; set; }

    public decimal? Deduction { get; set; }

    public decimal? NetSalary { get; set; }

    public string? Status { get; set; }

    public virtual Employee? Employee { get; set; }
}
