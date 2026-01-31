using System.ComponentModel.DataAnnotations;

namespace GoWhere_Api.Model.DTOs.Request
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is required")]
        

        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}