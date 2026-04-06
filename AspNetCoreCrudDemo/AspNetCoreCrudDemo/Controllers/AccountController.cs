using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using AspNetCoreCrudDemo.Data;
using AspNetCoreCrudDemo.Models;
using AspNetCoreCrudDemo.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using AspNet.Security.OAuth.GitHub;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreCrudDemo.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public AccountController(ApplicationDbContext context, IJwtTokenService jwtTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Kullanıcı adı veya Email zaten var mı kontrol et? (Önemli!)
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username || u.Email == model.Email);
                
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Bu kullanıcı adı veya e-posta zaten kullanılıyor.");
                    return View(model);
                }

                // 2. Yeni kullanıcıyı hazırla
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Role = (model.Username.ToLower() == "admin") ? "Admin" : "User" // PRATİK TEST: Sadece ismi "admin" olan Yönetici olur
                };

                // 3. Şifreyi "Karıştırma Makinesinden" (Hash) geçir!
                user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

                // 4. Veritabanına kaydet (Üyeler defterine ekle)
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Şimdilik ana sayfaya gönderelim, ilerde Login sayfasına göndereceğiz.
                TempData["SuccessMessage"] = "Kayıt başarıyla tamamlandı! Şimdi giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Kullanıcıyı bul (Username veya Email ile)
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.UsernameOrEmail || u.Email == model.UsernameOrEmail);

                if (user == null)
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                    return View(model);
                }

                // 2. Şifreyi doğrula
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
                
                if (result == PasswordVerificationResult.Failed)
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                    return View(model);
                }

                // 3. Başarılı! Bilet basma makinesini (JWT) çalıştır.
                var tokenResponse = _jwtTokenService.GenerateTokenResponse(user);

                // 4. Yedek Bileti (Refresh Token) veritabanına, kullanıcının hanesine işle.
                user.RefreshToken = tokenResponse.RefreshToken;
                user.RefreshTokenExpiry = DateTime.Now.AddDays(30); // Profesyonel standart: 30 Gün
                _context.Update(user);
                await _context.SaveChangesAsync();

                // 5. Biletleri "Gizli Cebe" (HttpOnly Cookie) koy!
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };

                // Ana Bilet
                Response.Cookies.Append("jwt_token", tokenResponse.AccessToken, cookieOptions);
                
                // Yedek Bilet (Refresh Token) - Bu da gizli durmalı
                Response.Cookies.Append("refresh_token", tokenResponse.RefreshToken, cookieOptions);

                TempData["SuccessMessage"] = $"Hoş geldin, {user.Username}!";
                return RedirectToAction("Index", "Dashboard");
            }

            return View(model);
        }

        // GET: Account/Logout
        [HttpGet]
        public IActionResult Logout()
        {
            // Cüzdandaki tüm biletleri siliyoruz!
            Response.Cookies.Delete("jwt_token");
            Response.Cookies.Delete("refresh_token");
            
            TempData["SuccessMessage"] = "Başarıyla çıkış yapıldı.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/RefreshToken (Bileti tazeleme merkezi)
        [HttpGet]
        public async Task<IActionResult> RefreshToken()
        {
            // 1. Çerezlerden yedek bileti (refresh token) oku
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken)) return RedirectToAction("Login");

            // 2. Bu yedek bilet veritabanımızda var mı ve hala geçerli mi?
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiry > DateTime.Now);

            if (user == null)
            {
                // Yedek bilet geçersizse her şeyi temizle ve giriş sayfasına at.
                Response.Cookies.Delete("jwt_token");
                Response.Cookies.Delete("refresh_token");
                return RedirectToAction("Login");
            }

            // 3. Her şey yolunda! Yeni biletleri bas.
            var tokenResponse = _jwtTokenService.GenerateTokenResponse(user);

            // 4. Yeni yedek bileti veritabanında güncelle (Rotation - Güvenlik için şart)
            user.RefreshToken = tokenResponse.RefreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(30);
            _context.Update(user);
            await _context.SaveChangesAsync();

            // 5. Yeni biletleri tarayıcıya gönder
            var cookieOptions = new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict };
            Response.Cookies.Append("jwt_token", tokenResponse.AccessToken, cookieOptions);
            Response.Cookies.Append("refresh_token", tokenResponse.RefreshToken, cookieOptions);

            // 6. Kullanıcıyı kaldığı yere veya ana sayfaya geri gönder
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/ExternalLogin (Google/GitHub butonuna basınca buraya gelir)
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider)
        {
            // Hangi dış servise (Google/GitHub) gideceğimizi belirliyoruz.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }

        // GET: Account/ExternalLoginCallback (Google/GitHub bizi buraya geri gönderir)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            // 1. Dış servisten gelen bilgileri (Email, İsim vb.) paketinden çıkarıyoruz.
            var authenticateResult = await HttpContext.AuthenticateAsync("External");
            if (!authenticateResult.Succeeded) return RedirectToAction("Login");

            var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
            var username = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name) ?? email?.Split('@')[0];

            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login");

            // 2. Bu e-posta ile bizde zaten bir kullanıcı var mı?
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                // Yoksa, Google/GitHub'dan aldığımız bilgilerle yeni bir hesap açıyoruz (Otomatik Kayıt)
                user = new User
                {
                    Email = email,
                    Username = username ?? "User_" + Guid.NewGuid().ToString("N").Substring(0, 8),
                    PasswordHash = "EXTERNAL_LOGIN_" + Guid.NewGuid().ToString(), // Şifreye gerek yok, dış servis onayladı!
                    Role = "User"
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // 3. Kullanıcı artık sistemimizde var. Biletlerini (Access + Refresh) basalım.
            var tokenResponse = _jwtTokenService.GenerateTokenResponse(user);

            // 4. Yedek Bileti (Refresh Token) veritabanına işle (Aynı normal girişteki gibi)
            user.RefreshToken = tokenResponse.RefreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(30);
            _context.Update(user);
            await _context.SaveChangesAsync();

            // 5. Biletleri "Gizli Cebe" (Cookie) koy!
            var cookieOptions = new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict };
            Response.Cookies.Append("jwt_token", tokenResponse.AccessToken, cookieOptions);
            Response.Cookies.Append("refresh_token", tokenResponse.RefreshToken, cookieOptions);

            TempData["SuccessMessage"] = $"Hoş geldin, {user.Username}! {authenticateResult.Properties.Items[".AuthScheme"]} ile başarıyla giriş yapıldı.";
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
