using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace sait.Models
{
    public class Registration 
    {
        
        [StringLength(15 , ErrorMessage = "Имя пользователя больше 15 символов!")]
        [Required(ErrorMessage = "Заполните поле!")]
        public string UserName { get; set; }
        [EmailAddress(ErrorMessage ="Ваш адрес почты не соответсвует формату")]
        [Required(ErrorMessage = "Заполните поле!")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Заполните поле!")]
        [DataType(DataType.Password)]
        public string Password { get; set; } 
    }
}
