using System.ComponentModel.DataAnnotations;

namespace phanquyen.DTOs
{
    public class UpdateRoleRequest
    {
        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        public string Role { get; set; }
    }
}