using System.Diagnostics;
using System.Security.Claims;
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

        public IActionResult Support()
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
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var requests = await GetUserRequests(user.Id);

            var model = new ProfileView
            {
                Email = email,
                UserName = user, // Или UserName = user.UserName
                Requests = requests,
                
            };

            return View(model);
        }

        // Новый метод для получения данных (без возврата представления)
        private async Task<List<Request>> GetUserRequests(string userId)
        {
            var requests = new List<Request>();
            var connectionString = _configuration.GetConnectionString("PostgreSQLConnection");

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var sql = @"SELECT * FROM ""Request"" WHERE ""ID_sender"" = @userId";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@userId", userId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        Console.WriteLine("Вообще дошли до сюда");
                        while (await reader.ReadAsync())
                        {
                            requests.Add(new Request
                            {

                                // Заполните ВСЕ необходимые поля
                                Post = reader["Post"]?.ToString(),
                                Status = reader["status"] != DBNull.Value && Convert.ToBoolean(reader["status"]),

                                Req_Created = reader["Req_Created"]?.ToString().Substring(0, 10)


                            });
                        }
                    }
                }
            }

            return requests;
        }

        // Отдельный метод для отображения страницы с заявками (если он вам нужен)
        public async Task<IActionResult> Request_Checker()
        {
            var userId = _userManager.GetUserId(User);
            

            var requests = await GetUserRequests(userId);
            return View(requests);
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
        public async Task<IActionResult> Check(Request req) // Страница авторизации (Views/Home/Autorization1.cshtml)
        {
            var id_sender = _userManager.GetUserId(User);
            Console.WriteLine("Метод Check вызван.");
            ModelState.Remove("Req_Created");
            ModelState.Remove("ID_sender");
            if (ModelState.IsValid)
            {
                Console.WriteLine("Данные отправлены.");
                var connectionString = _configuration.GetConnectionString("PostgreSQLConnection");
                using (var formConnection = new NpgsqlConnection(connectionString))
                {
                    formConnection.Open();

                    var sql = @"
                    INSERT INTO ""Request"" (
                        ""Post"",
                        ""Education_lvl"" ,
                        ""Work_exp"",
                        ""Skills"",
                        ""shedule"",    
                        ""Age"",
                        ""salary"",
                        ""merit"",
                        ""Company_name"",
                        ""Field"",
                        ""Adress"",
                        ""Name"",
                        ""Surname"",
                        ""PhoneNumber"",
                        ""Email"",
                        ""WorkerPost"",
                        ""ID_sender"",
                        ""Status"",
                        ""Req_Created""

                    ) VALUES (
                       @Post,
                        @Education_lvl ,
                        @Work_exp,
                        @Skills,
                        @shedule,    
                        @Age,
                        @salary,
                        @merit,
                        @Company_name,
                        @Field,
                        @Adress,
                        @Name,
                        @Surname,
                        @PhoneNumber,
                        @Email,
                        @WorkerPost,
                         @ID_sender,
                        @Status,
                        @Req_Created
                    )";
                    
                    var command = new NpgsqlCommand(sql ,formConnection);
                    command.Parameters.AddWithValue("@Post", req.Post);
                    command.Parameters.AddWithValue("@Education_lvl", req.Education_lvl);
                    command.Parameters.AddWithValue("@Work_exp", req.Work_exp);
                    command.Parameters.AddWithValue("@Skills", req.Skills);
                    command.Parameters.AddWithValue("@shedule", req.schedule);
                    command.Parameters.AddWithValue("@Age", req.Age);
                    command.Parameters.AddWithValue("@salary", req.salary);
                    command.Parameters.AddWithValue("@merit", req.merit);
                    command.Parameters.AddWithValue("@Company_name", req.Company_name);
                    command.Parameters.AddWithValue("@Field", req.Field);
                    command.Parameters.AddWithValue("@Adress", req.Adress);
                    command.Parameters.AddWithValue("@Name", req.Name);
                    command.Parameters.AddWithValue("@Surname", req.Surname);
                    command.Parameters.AddWithValue("@PhoneNumber", req.PhoneNumber);
                    command.Parameters.AddWithValue("@Email", req.Email);
                    command.Parameters.AddWithValue("@WorkerPost", req.WorkerPost);
                    command.Parameters.AddWithValue("@Req_Created", DateTime.Now);
                    
                    command.Parameters.AddWithValue("@ID_sender", id_sender);
                    command.Parameters.AddWithValue("@Status", req.Status);

                    try
                    {
                        command.ExecuteNonQuery();
                        TempData["SuccessMessage"] = "Заявка успешно отправлена!";
                        return RedirectToAction("Form");
                    }
                    
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка: {ex.Message}");
                        TempData["Error"] = "Произошла ошибка. Попробуйте позже.";
                        return View("Form", req);
                    }

                    

                }

                

            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ Ошибки валидации ModelState:");
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Поле: {entry.Key}, Ошибка: {error.ErrorMessage}");
                        TempData["Error"] = "Произошла ошибка. Попробуйте позже.";
                        return View("Form", req);
                    }
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
        [AllowAnonymous]
        public IActionResult Google(string provider = "Google", string returnUrl = null)
        {
            // Генерируем URL для обратного вызова
            var redirectUrl = Url.Action("GoogleCallback", "Home", new { returnUrl });

            // Настраиваем свойства аутентификации
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            // Перенаправляем к Google
            return Challenge(properties, provider);
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallBack(string returnUrl = null, string remoteError = null) 
        {
            if (remoteError != null) {
                return RedirectToAction("Autorization1", new { message = "Произошла Ошибка , попробуйте позже" });

            }


            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }
                
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider , info.ProviderKey, isPersistent: true);
            if (result.Succeeded) {

                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                return RedirectToAction("Profile");
            }

            if (result.IsLockedOut || result.IsNotAllowed) {

                return RedirectToAction("--------");
            }

            else
            {
                var UserName = info.Principal.FindFirstValue(ClaimTypes.Email);

                string[] parts = UserName.Split('@');
                if (parts.Length == 2) // чистка ника от гмайла
                {
                    string username = parts[0];
                    UserName=username;
                }
                    var Email = info.Principal.FindFirstValue(ClaimTypes.Email);



                var user = new User
                {
                    UserName = UserName,
                    Email = Email,
                };
                var logresult= await _userManager.CreateAsync(user);

                if (logresult.Succeeded) 
                {
                    await _userManager.AddLoginAsync(user, info);

                    // Входим
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

                    return RedirectToAction("Profile");



                }
            }
            return View(info);
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
