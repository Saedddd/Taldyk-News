using System.ComponentModel.DataAnnotations;

namespace TaldykNews.WebUI.Models.ViewModel.Account
{
    public class IndexVM
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Пароль должен быть не менее 5 символов в длину", MinimumLength = 5)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Пароль должен быть не менее 5 символов в длину", MinimumLength = 5)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
        public string Role { get; set; }
    }
}
