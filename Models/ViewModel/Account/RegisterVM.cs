using System.ComponentModel.DataAnnotations;

namespace TaldykNews.WebUI.Models.ViewModel.Account
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Не указан e-mail")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Не указано ФИО")]
        [Display(Name = "ФИО")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Пароль должен быть не менее 5 символов в длину", MinimumLength = 5)]

        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
