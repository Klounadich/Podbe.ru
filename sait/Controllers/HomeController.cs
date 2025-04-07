using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using sait.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace sait.Controllers
{
    public class HomeController : Controller

    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public HomeController(UserManager<User> userManager, SignInManager<User> signInManager , IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;


        }





        public IActionResult Index() // Главная страница (Views/Home/Index.cshtml)
        {
            return View();
        }

        public IActionResult Form() // Страница формы (Views/Home/Form.cshtml)
        {
            return View();
        }

        public IActionResult Autorization1()
        {
            return View();
        }

        public IActionResult Autorization2() // Страница авторизации (Views/Home/Autorization1.cshtml)
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Profile");
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RequestButton() {
            if (!User.Identity.IsAuthenticated)
            {
                return View("Autorization1");
            }
            return View("Form");
        }
        public IActionResult Check(Request req) // Страница авторизации (Views/Home/Autorization1.cshtml)
        {
            
            if (ModelState.IsValid)
            {
                var connectionString = _configuration.GetConnectionString("PostgreSQLConnection");
                using (var formConnection = new NpgsqlConnection(connectionString))
                {
                    formConnection.Open();

                    var sql = @"
                    INSERT INTO candidat_request (
                        ""Должность"",
                        ""Образование"",
                        ""Опыт Работы"",
                        ""Навыки"",
                        ""График Работы"",
                        ""Возраст"",
                        ""Зарплата"",
                        ""Личные качества""
                    ) VALUES (
                        @Должность,
                        @Образование,
                        @ОпытРаботы,
                        @Навыки,
                     @ГрафикРаботы,
                     @Возраст,
                     @Зарплата,
                     @ЛичныеКачества
                    )";

                    var command = new NpgsqlCommand(sql ,formConnection);
                    command.Parameters.AddWithValue("@Должность", req.Post);
                    command.Parameters.AddWithValue("@Образование", req.Education_lvl);
                    command.Parameters.AddWithValue("@ОпытРаботы", req.Work_exp);
                    command.Parameters.AddWithValue("@Навыки", req.Skills);
                    command.Parameters.AddWithValue("@ГрафикРаботы", req.schedule);
                    command.Parameters.AddWithValue("@Возраст", req.Age);
                    command.Parameters.AddWithValue("@Зарплата", req.salary);
                    command.Parameters.AddWithValue("@ЛичныеКачества", req.merit);
                    command.ExecuteNonQuery();

                    var sql1 = @"
                    INSERT INTO sender_request (
                        ""Название компании"",
                        ""Сфера деятельности"",
                        ""Юр. Адрес"",
                        ""Имя"",
                        ""Фамилия"",
                        ""Номер телефона"",
                        ""Электронная почта"",
                        ""Занимаемая Должность""
                    ) VALUES (
                        @Названиекомпании,
                        @Сферадеятельности,
                        @Юр.Адрес,
                        @Имя,
                     @Фамилия,
                     @Номертелефона,
                     @Электроннаяпочта,
                     @ЗанимаемаяДолжность
                    )";

                    var command2 = new NpgsqlCommand(sql1,formConnection);
                    command2.Parameters.AddWithValue("@НазваниеКомпании", req.Company_name);
                    command2.Parameters.AddWithValue("@Сферадеятельности" , req.Field);
                    command2.Parameters.AddWithValue("@Юр.Адрес" , req.Adress);
                    command2.Parameters.AddWithValue("@Имя" , req.Name);
                    command2.Parameters.AddWithValue("@Фамилия" , req.Surname);
                    command2.Parameters.AddWithValue("@Номертелефона", req.PhoneNumber);
                    command2.Parameters.AddWithValue("@Электроннаяпочта" , req.Email);
                    command2.Parameters.AddWithValue("@ЗанимаемаяДолжность", req.WorkerPost);
                    command2.ExecuteNonQuery();







                    TempData["SuccessMessage"] = "Заявка успешно отправлена!";
                    return RedirectToAction("Form");
                }
            }
            return View("Form", req);
        }

        [HttpPost]
        public async Task<IActionResult> Reg_Check(Registration model)
        {
            bool hasErrors = false;
            var errorDescriber = new Error_Describer();

            // 1. Проверка на пустые поля
            if (string.IsNullOrEmpty(model.UserName))
            {
                ModelState.AddModelError("UserName", "Поле не заполнено!");
                hasErrors = true;
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("Email", "Поле не заполнено!");
                hasErrors = true;
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("Password", "Поле не заполнено!");
                hasErrors = true;
            }

            // 2. Проверка уникальности (только если поля не пустые)
            if (!string.IsNullOrEmpty(model.UserName))
            {
                var userWithSameName = await _userManager.FindByNameAsync(model.UserName);
                if (userWithSameName != null)
                {
                    ModelState.AddModelError("UserName", "Пользователь с таким логином уже существует");
                    hasErrors = true;
                }
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);
                if (userWithSameEmail != null)
                {
                    ModelState.AddModelError("Email", "Пользователь с такой почтой уже зарегистрирован");
                    hasErrors = true;
                }
            }

            // 3. Предварительная проверка пароля с ВАШИМ Error_Describer
            if (!string.IsNullOrEmpty(model.Password))
            {
                // Создаем временного пользователя для валидации
                var tempUser = new User { UserName = "temp_validation_user" };

                // Получаем все валидаторы пароля
                var validators = _userManager.PasswordValidators;

                // Проверяем пароль каждым валидатором
                foreach (var validator in validators)
                {
                    var result = await validator.ValidateAsync(_userManager, tempUser, model.Password);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            // Преобразуем стандартную ошибку в кастомную
                            var customError = GetCustomError(errorDescriber, error);
                            ModelState.AddModelError("Password", customError.Description);
                        }
                        hasErrors = true;
                    }
                }
            }

            // 4. Если есть ошибки - возвращаем
            if (hasErrors)
            {
                return View("Autorization2", model);
            }

            // 5. Финальная регистрация
            User user = new User { UserName = model.UserName, Email = model.Email };
            var createResult = await _userManager.CreateAsync(user, model.Password);

            if (createResult.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Profile");
            }

            // 6. Обработка ошибок регистрации с ВАШИМ Error_Describer
            foreach (var error in createResult.Errors)
            {
                var customError = GetCustomError(errorDescriber, error);
                ModelState.AddModelError(string.Empty, customError.Description);
            }

            return View("Autorization2", model);
        }

        // Метод для преобразования стандартной ошибки в кастомную
        private IdentityError GetCustomError(Error_Describer describer, IdentityError error)
        {
            return error.Code switch
            {
                nameof(IdentityErrorDescriber.PasswordRequiresDigit) => describer.PasswordRequiresDigit(),
                nameof(IdentityErrorDescriber.PasswordTooShort) => describer.PasswordTooShort(6),
                nameof(IdentityErrorDescriber.PasswordRequiresLower) => describer.PasswordRequiresLower(),
                nameof(IdentityErrorDescriber.PasswordRequiresUpper) => describer.PasswordRequiresUpper(),
                nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric) => describer.PasswordRequiresNonAlphanumeric(),
                _ => error
            };
        }

        [HttpPost]
        public async Task<IActionResult> Authorize (Authorization aut)
        {
            if (!ModelState.IsValid)
            {
                return View("Autorization1", aut);
            }

            

            if (string.IsNullOrEmpty(aut.Aut_UserName))
            {

                ModelState.AddModelError("Aut_UserName", "Поле не заполнено!");
            }

            if (string.IsNullOrEmpty(aut.Aut_Password))
            {
                ModelState.AddModelError("Aut_Password", "Поле не заполнено!");
            }
            var Log_UserName = await _userManager.FindByNameAsync(aut.Aut_UserName);
            var Log_Password = await _userManager.CheckPasswordAsync( Log_UserName ,aut.Aut_Password);
            
            
                
            if (Log_UserName==null) {
                ModelState.AddModelError("Aut_UserName", "Неправильно введено имя или пароль.Попробуйте снова");
                return View("Autorization1", aut);
            }

            if (!Log_Password)
            {
                ModelState.AddModelError("Aut_UserName", "Неправильно введено имя или пароль.Попробуйте снова");
                return View("Autorization1", aut);
            }

            await _signInManager.SignInAsync(Log_UserName, isPersistent:true );
            return RedirectToAction("Profile");  // пароль для теста:6328123Red?
            
        }
    }
}
