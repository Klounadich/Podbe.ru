using System.ComponentModel.DataAnnotations;

namespace sait.Models
{
    public class Request
    {
        
        [Required(ErrorMessage = "Заполните поле!")]
        public string Post {  get; set; } = null!;
        [Required(ErrorMessage = "Заполните поле!")]
        public string Education_lvl { get; set; } = null!;
        [Required(ErrorMessage = "Заполните поле!")]
        public string Work_exp { get; set; } = null!;
        [Required(ErrorMessage = "Заполните поле!")]
        public string Skills { get; set; } = null!;
        [Required(ErrorMessage = "Заполните поле!")]
        public string schedule { get; set; } = null!;
        [Required(ErrorMessage = "Заполните поле!")]
        public string Age { get; set; } = null!;
        [Required(ErrorMessage = "Заполните поле!")]
        public string salary { get; set; } = null!;
        [Required(ErrorMessage = "Заполните поле!")]
        public string merit { get; set; } = null!;

        // Инфа о компании

        [Required(ErrorMessage = "Заполните поле!")]
        public string Company_name { get; set; } = null!;


        [Required(ErrorMessage = "Заполните поле!")]
        public string Field { get; set; } = null!; 

        [Required(ErrorMessage = "Заполните поле!")]
        public string Adress { get; set; } = null!;

        [Required(ErrorMessage = "Заполните поле!")]
        public string Name { get; set; } = null!; 


        [Required(ErrorMessage = "Заполните поле!")]
        public string Surname { get; set; } = null!;

        [StringLength(12, ErrorMessage ="Номер телефона не может быть длиннее 12 символов")]
        [Required(ErrorMessage = "Заполните поле!")]
        public string PhoneNumber { get; set; } = null!;


        [Required(ErrorMessage = "Заполните поле!")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Заполните поле!")]
        public string WorkerPost { get; set; } = null!;

        public string ID_sender { get; set; }

        public bool Status { get; set; }

    }
}
