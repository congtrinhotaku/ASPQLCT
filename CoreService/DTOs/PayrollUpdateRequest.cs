namespace CoreService.DTOs
{
    public class PayrollUpdateRequest
    {
        public decimal? Bonus { get; set; }
        public decimal? Overtime { get; set; }
        public decimal? Deduction { get; set; }
        public string? Status { get; set; }
    }
}