using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sait.Data;
using sait.Models;

var builder = WebApplication.CreateBuilder(args);
// 1. ��������� EF Core + PostgreSQL

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));

    

// ��������� IDENTITY
builder.Services.AddIdentity<User, IdentityRole>(options =>
{

    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.Password.RequireDigit = true;       // �� ��������� �����
    options.Password.RequireLowercase = true;   // �� ��������� �������� �����
    options.Password.RequireUppercase = false;   // �� ��������� ��������� �����
    options.Password.RequireNonAlphanumeric = false; // �� ��������� �����������
    options.Password.RequiredLength = 6;         // ����������� ����� = 6

})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddErrorDescriber<Error_Describer>()
.AddDefaultTokenProviders();

// 3. ��������� cookies
builder.Services.ConfigureApplicationCookie(options =>
{

    options.LoginPath = "/Home/Autorization1";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(15);
});

// ���� ����� �����
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

        // �����������: ������� scope ��� ������� � �������������� ������
        options.Scope.Add("profile");
        options.Scope.Add("email");

        // �����������: �������� URL ���������������, ���� ����������� �� ��������
        // options.CallbackPath = "/custom-signin-google";
    });

// 4. ��������� ��������� ������������ � �������������
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ������������ middleware pipeline
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