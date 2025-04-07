using System.ComponentModel.DataAnnotations;

namespace sait.Models
{
    public class Authorization
    {
        [Required (ErrorMessage = "Поле не заполнено!")]
        public string  Aut_UserName { get; set; } = null!;

        [Required(ErrorMessage = "Поле не заполнено!")]
        public string Aut_Password { get; set; } = null!;
    }
}
