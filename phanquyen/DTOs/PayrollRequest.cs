using System.ComponentModel.DataAnnotations;

namespace phanquyen.DTOs
{
    public class PayrollRequest
    {
        [Required(ErrorMessage = "EmployeeId là bắt buộc")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Tháng là bắt buộc")]
        [Range(1, 12, ErrorMessage = "Tháng phải từ 1 đến 12")]
        public int Month { get; set; }

        [Required(ErrorMessage = "Năm là bắt buộc")]
        [Range(2000, 2100, ErrorMessage = "Năm không hợp lệ")]
        public int Year { get; set; }

        public decimal? Bonus { get; set; } = 0;
        public decimal? Overtime { get; set; } = 0;
        public decimal? Deduction { get; set; } = 0;
        public string? Status { get; set; }
    }
}