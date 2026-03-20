using System.ComponentModel.DataAnnotations;

namespace CoreService.DTOs
{
    public class RegisterRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Password { get; set; }

        public string role { get; set; }
    }
}
