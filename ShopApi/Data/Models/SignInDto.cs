using System.ComponentModel.DataAnnotations;

namespace ShopApi.Data.Models
{
    public class SignInDto
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
