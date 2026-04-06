using AspNetCoreCrudDemo.Data;
using AspNetCoreCrudDemo.Interfaces;
using AspNetCoreCrudDemo.Repositories;
using AspNetCoreCrudDemo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// ÖNEMLİ: Sunucu her yeniden başladığında tüm eski oturumları silmek (Invalidate) için 
// dinamik ve benzersiz bir şifreleme anahtarı (Secret Key) atıyoruz.
builder.Configuration["JwtSettings:SecretKey"] = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Authentication & JWT Yapılandırması
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie("External") // Sosyal girişlerin geçici olarak konaklayacağı bekleme odası
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
    };

    // Biletler Header'da değil, CEREZ (Cookie) içinde olduğu için özel ayar yapıyoruz.
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["jwt_token"];
            return System.Threading.Tasks.Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            // Eğer Yetkisiz ise (401), kullanıcıyı Login sayfasına nazikçe yönlendir.
            // Bu sayede "Siteye ulaşılamıyor" hatasından kurtulmuş oluruz.
            context.HandleResponse();
            context.Response.Redirect("/Account/Login");
            return System.Threading.Tasks.Task.CompletedTask;
        }
    };
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleSettings:ClientId"]!;
    options.ClientSecret = builder.Configuration["GoogleSettings:ClientSecret"]!;
    options.CallbackPath = "/signin-google";
    options.SignInScheme = "External";
})
.AddGitHub(options =>
{
    options.ClientId = builder.Configuration["GitHubSettings:ClientId"]!;
    options.ClientSecret = builder.Configuration["GitHubSettings:ClientSecret"]!;
    options.CallbackPath = "/signin-github";
    options.Scope.Add("user:email");
    options.SignInScheme = "External";
});

// Aşçıyı (Repository) ve Menüsünü sisteme tanıtıyoruz. (Bağımlılık Enjeksiyonu / DI)
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

// Bilet Basma Makinesini (JWT Servisi) sisteme tanıtıyoruz.
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Https yönlendirmesini (yerel/localhost ssl sertifika problemleri için) kapatıyoruz.
// app.UseHttpsRedirection();

app.UseStaticFiles(); // Statik dosyaları (CSS/JS) her yerde çalışacak şekilde aktif et!
app.UseRouting();

// Bilet kontrolünü yap (Kimin geldiğini anla) - SIRALAMA COK ONEMLI!
app.UseAuthentication();

// Yetkisi var mı diye bak (Kimin neye hakkı var)
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
