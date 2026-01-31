using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoWhere_Api.Model.DTOs.Request
{
    public class UserRegisterDto
    {

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters.")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
        [StringLength(100, ErrorMessage = "Full name cannot be longer than 100 characters.")]
        public string? FullName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(15, ErrorMessage = "Phone number cannot be longer than 15 characters.")]
        public string PhoneNumber { get; set; }
        [ForeignKey("RoleId"), Range(1, 5, ErrorMessage = "RoleId must be between 1 and 5.")]
        [Required(ErrorMessage = "RoleId is required.")]
        public int RoleId { get; set; }
    }
}
