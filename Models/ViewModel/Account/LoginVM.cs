using System.ComponentModel.DataAnnotations;

namespace TaldykNews.WebUI.Models.ViewModel.Account
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Не введен email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Не введен пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
