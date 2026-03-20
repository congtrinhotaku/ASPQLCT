namespace CoreService.DTOs
{
    public class EmployeeRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Position { get; set; }
        public decimal? BaseSalary { get; set; }
        public string? Status { get; set; }
        public List<int>? DepartmentIds { get; set; }
    }
}
