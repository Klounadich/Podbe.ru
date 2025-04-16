using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sait.Data;
using sait.Models;

var builder = WebApplication.CreateBuilder(args);
// 1. Добавляем EF Core + PostgreSQL

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));

    

// НАСТРОЙКА IDENTITY
builder.Services.AddIdentity<User, IdentityRole>(options =>
{

    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.Password.RequireDigit = true;       // Не требовать цифры
    options.Password.RequireLowercase = true;   // Не требовать строчные буквы
    options.Password.RequireUppercase = false;   // Не требовать заглавные буквы
    options.Password.RequireNonAlphanumeric = false; // Не требовать спецсимволы
    options.Password.RequiredLength = 6;         // Минимальная длина = 6

})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddErrorDescriber<Error_Describer>()
.AddDefaultTokenProviders();

// 3. Настройка cookies
builder.Services.ConfigureApplicationCookie(options =>
{

    options.LoginPath = "/Home/Autorization1";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(15);
});

// Авта через гуглу
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

        // Опционально: указать scope для доступа к дополнительным данным
        options.Scope.Add("profile");
        options.Scope.Add("email");

        // Опционально: изменить URL перенаправления, если стандартный не подходит
        // options.CallbackPath = "/custom-signin-google";
    });

// 4. Добавляем поддержку контроллеров и представлений
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Конфигурация middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();