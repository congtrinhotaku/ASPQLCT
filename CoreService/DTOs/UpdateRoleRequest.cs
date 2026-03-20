using System.ComponentModel.DataAnnotations;

namespace CoreService.DTOs
{
    public class UpdateRoleRequest
    {
        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        public string Role { get; set; }
    }
}