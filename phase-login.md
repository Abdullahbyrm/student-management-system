# Phase: Login & JWT Authentication

## Projenin Mevcut Durumu

| Özellik | Durum |
|---|---|
| Framework | .NET 9.0 (MVC) |
| Veritabanı | SQL Server + EF Core 9.0 |
| Authentication | **Yok** — `app.UseAuthorization()` var ama servis kaydı yok |
| Controller'lar | HomeController, StudentsController (tam CRUD) |
| Model | Student (StudentId, Name, Email, Course, EnrollmentDate) |
| API Katmanı | Yok — sadece MVC (View dönen controller'lar) |

**Sonuç:** Proje şu an tamamen açık; herhangi bir koruma mekanizması bulunmuyor. Login sayfası, JWT authentication ve cookie tabanlı JWT yönetimi eklenebilir.

---

## Ön Bilgi: Bu Rehber Nasıl Kullanılmalı?

Bu doküman **adım adım uygulaman gereken bir görev listesi değil**. Her adımda:

1. Önce **konsepti araştır** — ne olduğunu, neden kullanıldığını anla
2. **Seçenekleri değerlendir** — her zaman birden fazla yol vardır
3. **Dene** — küçük bir PoC (Proof of Concept) yap, çalıştır, hatalarını gör
4. **Sonra entegre et** — çalışan çözümü projeye ekle

Her bölümde 🔍 **Araştır**, 🤔 **Düşün**, 🛠️ **Uygula** ve ✅ **Doğrula** etiketleri var. Bunlar sana hangi aşamada olduğunu gösterir.

---

## Bölüm 1: Authentication vs Authorization — Temel Farkı Anlamak

🔍 **Araştır:**
- Authentication (kimlik doğrulama) ile Authorization (yetkilendirme) arasındaki fark nedir?
- ASP.NET Core'da bu iki kavram nasıl ayrı middleware olarak çalışır?
- `app.UseAuthentication()` ve `app.UseAuthorization()` middleware sıralaması neden önemlidir?

🤔 **Düşün:**
- Projede `app.UseAuthorization()` zaten var. Peki neden tek başına bir şey yapmıyor?
- Bir controller action'ına `[Authorize]` attribute eklersen ve authentication yoksa ne olur? Dene.

🛠️ **Uygula:**
- `StudentsController`'a `[Authorize]` attribute ekle ve uygulamayı çalıştır.
- Ne tür bir hata alıyorsun? Hata mesajını oku ve anlamaya çalış.
- Bu hatayı çözmeden bir sonraki bölüme geçme — hatanın sebebini anlamak önemli.

✅ **Doğrula:**
- Authentication olmadan Authorization'ın işe yaramadığını kendi gözlerinle gördün mü?

---

## Bölüm 2: JWT Nedir ve Neden Kullanılır?

🔍 **Araştır:**
- JWT (JSON Web Token) nedir? Üç parçası nelerdir? (Header, Payload, Signature)
- [jwt.io](https://jwt.io) sitesine git, bir JWT'yi decode et, yapısını incele.
- Session-based authentication ile Token-based authentication arasındaki farklar nelerdir?
- Hangi senaryolarda JWT tercih edilir, hangilerinde session cookie daha uygundur?

🤔 **Düşün:**
- Bu proje bir MVC uygulaması (tarayıcı tabanlı). JWT genellikle API'ler için kullanılır. O zaman neden JWT tercih ediyoruz?
- Aşağıdaki argümanları değerlendir:
  - "MVC uygulamalarında ASP.NET Identity + Cookie Authentication daha doğal bir seçimdir"
  - "JWT kullanmak, ileride bir API katmanı eklemeyi kolaylaştırır"
  - "JWT öğrenmek, modern authentication akışlarını anlamak için iyi bir başlangıçtır"
- Bu projenin amacı **öğrenmek** olduğuna göre, JWT ile devam etmek mantıklı mı? Kendi cevabını oluştur.

> **Not:** Gerçek bir production MVC uygulamasında muhtemelen ASP.NET Identity + Cookie Authentication kullanırdık. Ancak JWT'yi öğrenmek, API tabanlı mimarileri ve modern auth akışlarını anlamak için çok değerli.

---

## Bölüm 3: Kullanıcı Modeli ve Veritabanı Hazırlığı

🔍 **Araştır:**
- Bir `User` modeli için minimum hangi alanlar gerekir? (Id, Username, Email, PasswordHash...)
- Şifre neden **hash** olarak saklanır? Plain text saklamanın riskleri nelerdir?
- `BCrypt`, `PBKDF2`, `Argon2` gibi hashing algoritmaları arasındaki farklar nelerdir?
- ASP.NET Core'da built-in `PasswordHasher<T>` sınıfı ne iş yapar?

🤔 **Düşün:**
- User modelin Student modeline benzer olacak. ApplicationDbContext'e nasıl ekleyeceksin?
- Şifre hash'lemek için harici bir kütüphane mi kullanacaksın (BCrypt.Net) yoksa ASP.NET Core'un built-in `PasswordHasher`'ını mı?
- Her iki yaklaşımın artıları ve eksileri neler?

🛠️ **Uygula:**
1. `Models/User.cs` oluştur:
   - Hangi property'ler olmalı? (Id, Username, Email, PasswordHash, Role?)
   - DataAnnotation'lar ekle (Required, MaxLength, vs.)
2. `ApplicationDbContext`'e `DbSet<User>` ekle
3. Migration oluştur: `dotnet ef migrations add AddUserTable`
4. Migration'ı uygula: `dotnet ef database update`

✅ **Doğrula:**
- Veritabanında Users tablosu oluştu mu?
- SQL Server'da tabloyu aç ve kolonları kontrol et.
- PasswordHash alanı yeterince uzun mu? (hash'ler genellikle 60-100+ karakter olur)

---

## Bölüm 4: Register (Kayıt) İşlemi

🔍 **Araştır:**
- Form-based registration akışı nasıl çalışır?
- ViewModel ile Model arasındaki fark nedir? Neden doğrudan Model kullanmak yerine ViewModel oluşturuyoruz?
- `ModelState.IsValid` ne kontrol eder?

🤔 **Düşün:**
- `RegisterViewModel` hangi alanları içermeli? (Username, Email, Password, ConfirmPassword?)
- Password ve ConfirmPassword eşleşme kontrolünü nerede yapacaksın? (Client-side? Server-side? İkisi birden?)
- Aynı username veya email ile tekrar kayıt olunmasını nasıl engellersin?

🛠️ **Uygula:**
1. `Models/RegisterViewModel.cs` oluştur
2. `Controllers/AccountController.cs` oluştur
   - `[HttpGet] Register()` — formu göster
   - `[HttpPost] Register(RegisterViewModel model)` — kayıt işlemi
3. `Views/Account/Register.cshtml` oluştur
4. Şifreyi hash'leyip User tablosuna kaydet

✅ **Doğrula:**
- Kayıt formunu doldur ve gönder.
- Veritabanında kullanıcı oluştu mu?
- PasswordHash alanında düz şifre mi var yoksa hash mi? Bu kritik bir kontrol.
- Aynı username ile tekrar kayıt olmayı dene — ne oluyor?

---

## Bölüm 5: JWT Token Üretimi

🔍 **Araştır:**
- `System.IdentityModel.Tokens.Jwt` NuGet paketi ne sağlar?
- `Microsoft.AspNetCore.Authentication.JwtBearer` paketi ne iş yapar?
- JWT token üretirken hangi bilgiler gerekir? (Secret key, Issuer, Audience, Claims, Expiry)
- Claim nedir? Bir token'a hangi claim'ler eklenir?

🤔 **Düşün:**
- JWT secret key'i nerede saklayacaksın? (appsettings.json? Environment variable? User secrets?)
- Token süresi ne kadar olmalı? (5 dakika? 1 saat? 1 gün?) Kısa tutmanın ve uzun tutmanın avantajları/dezavantajları neler?
- Refresh token konseptini araştır — neden sadece access token yeterli olmayabilir?

🛠️ **Uygula:**
1. Gerekli NuGet paketlerini yükle:
   ```bash
   dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
   dotnet add package System.IdentityModel.Tokens.Jwt
   ```
2. `appsettings.json`'a JWT ayarlarını ekle:
   ```json
   "JwtSettings": {
     "SecretKey": "???",
     "Issuer": "???",
     "Audience": "???",
     "ExpirationInMinutes": ???
   }
   ```
   Bu alanları doldururken her birinin ne anlama geldiğini araştır. SecretKey'in minimum uzunluğu ne olmalı?

3. Bir JWT token üreten servis oluştur: `Services/JwtTokenService.cs`
   - Bu servisin interface'i nasıl olmalı? `IJwtTokenService`
   - Token üretirken hangi claim'leri ekleyeceksin?
   - DI (Dependency Injection) ile nasıl kayıt edeceksin?

4. Üretilen token'ı [jwt.io](https://jwt.io) sitesinde decode et ve içeriğini kontrol et.

✅ **Doğrula:**
- Token başarıyla üretiliyor mu?
- Token'ı jwt.io'da decode ettiğinde claim'leri görebiliyor musun?
- Signature doğrulanıyor mu? (jwt.io'da secret key'i girerek kontrol et)

---

## Bölüm 6: Login İşlemi ve JWT'nin Cookie'ye Yazılması

🔍 **Araştır:**
- JWT'yi client'a göndermek için kaç farklı yol var? (Response body, Cookie, LocalStorage, SessionStorage)
- Her yöntemin güvenlik açısından artıları ve eksileri neler?
- `HttpOnly` cookie nedir? Neden önemlidir?
- `Secure` flag nedir?
- `SameSite` attribute nedir ve hangi değerleri alabilir? (Strict, Lax, None)
- XSS (Cross-Site Scripting) saldırısı nedir? LocalStorage'daki token XSS'e neden açıktır?
- CSRF (Cross-Site Request Forgery) saldırısı nedir? Cookie-based yaklaşım CSRF'e neden açıktır?

🤔 **Düşün:**
- Bu proje bir MVC uygulaması olduğu için JWT'yi cookie'de tutmak mantıklı. Neden?
- `HttpOnly + Secure + SameSite=Strict` cookie kullanmanın avantajları neler?
- API-first bir uygulamada farklı bir yaklaşım mı kullanırdın?

🛠️ **Uygula:**
1. `AccountController`'a Login action'ları ekle:
   - `[HttpGet] Login()` — formu göster
   - `[HttpPost] Login(LoginViewModel model)` — doğrulama + token üretimi + cookie yazma
2. `Models/LoginViewModel.cs` oluştur (Username/Email + Password)
3. Login başarılı olduğunda:
   - Kullanıcıyı veritabanında bul
   - Şifreyi doğrula (hash karşılaştırma)
   - JWT token üret
   - Token'ı HttpOnly cookie olarak yaz:
     ```csharp
     Response.Cookies.Append("jwt_token", token, new CookieOptions
     {
         HttpOnly = ???,
         Secure = ???,
         SameSite = ???,
         Expires = ???
     });
     ```
     Bu alanları doldurmadan önce her birinin ne yaptığını araştır.

✅ **Doğrula:**
- Login yaptıktan sonra tarayıcının Developer Tools > Application > Cookies bölümünü aç.
- Cookie oluştu mu? HttpOnly flag'i aktif mi?
- Cookie'nin içeriğini JavaScript ile okumaya çalış (`document.cookie`). Okuyabildin mi? Neden?

---

## Bölüm 7: JWT Validation Middleware Kurulumu

🔍 **Araştır:**
- ASP.NET Core'da JWT validation nasıl yapılandırılır?
- `TokenValidationParameters` nedir ve hangi ayarları içerir?
- Cookie'den JWT okumak için `Events.OnMessageReceived` event'ı nasıl kullanılır?
- Middleware sıralaması neden kritiktir? (`UseAuthentication` nerede olmalı?)

🤔 **Düşün:**
- Standart JwtBearer middleware, token'ı `Authorization: Bearer <token>` header'ından okur. Ama bizim token cookie'de. Bu sorunu nasıl çözersin?
- İki yaklaşım var:
  1. `OnMessageReceived` event'ında cookie'den oku
  2. Custom middleware yaz
- Hangisi daha temiz? Hangisi daha standart?

🛠️ **Uygula:**
1. `Program.cs`'de authentication servislerini yapılandır:
   ```csharp
   builder.Services.AddAuthentication(options =>
   {
       options.DefaultAuthenticateScheme = ???;
       options.DefaultChallengeScheme = ???;
   })
   .AddJwtBearer(options =>
   {
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = ???,
           ValidateAudience = ???,
           ValidateLifetime = ???,
           ValidateIssuerSigningKey = ???,
           ValidIssuer = ???,
           ValidAudience = ???,
           IssuerSigningKey = ???
       };

       options.Events = new JwtBearerEvents
       {
           OnMessageReceived = context =>
           {
               // Cookie'den token'ı oku ve context.Token'a ata
               // Bunu kendin araştırıp implementasyonunu yaz
               ???
           }
       };
   });
   ```
2. `app.UseAuthentication()` middleware'ini doğru sıraya ekle.

✅ **Doğrula:**
- Login olmadan `/Students` sayfasına gitmeyi dene. Ne oluyor?
- Login olduktan sonra aynı sayfaya git. Şimdi ne oluyor?
- Cookie'yi tarayıcıdan sil ve sayfayı yenile. Ne oluyor?

---

## Bölüm 8: Login/Logout UI Entegrasyonu

🔍 **Araştır:**
- `_Layout.cshtml` nedir ve neden tüm sayfaları etkiler?
- Razor view'da kullanıcının login olup olmadığını nasıl kontrol edersin? (`User.Identity.IsAuthenticated`)
- `User.Claims` üzerinden kullanıcı bilgilerine nasıl erişirsin?

🛠️ **Uygula:**
1. `_Layout.cshtml`'de navbar'a Login/Register/Logout linkleri ekle:
   - Kullanıcı login değilse: Login | Register
   - Kullanıcı login ise: "Hoş geldin, {username}" | Logout
2. Logout action'ı oluştur:
   - Cookie'yi sil
   - Ana sayfaya yönlendir
3. Login olmamış kullanıcıları login sayfasına yönlendirme mekanizmasını ayarla:
   - `[Authorize]` attribute redirect URL'i nasıl belirlenir?
   - `AccountController/AccessDenied` action'ı gerekir mi?

✅ **Doğrula:**
- Login/Logout akışını uçtan uca test et.
- Navbar doğru şekilde güncelleniyor mu?
- Farklı tarayıcılarda test et — cookie davranışı tutarlı mı?

---

## Bölüm 9: Güvenlik Kontrolü ve İyileştirmeler

🔍 **Araştır:**
- OWASP Top 10 nedir? Authentication ile ilgili hangi maddeler var?
- Token expiration sonrası ne olmalı?
- Brute-force login saldırılarına karşı ne yapılabilir?

🤔 **Düşün ve Değerlendir:**
Aşağıdaki kontrol listesini gözden geçir. Her maddeyi kendi uygulamanda kontrol et:

- [ ] Şifreler hash'lenmiş olarak mı saklanıyor?
- [ ] JWT secret key yeterince uzun ve güvenli mi? (minimum 256-bit)
- [ ] Token süresi makul mü?
- [ ] Cookie `HttpOnly` flag'i aktif mi?
- [ ] Cookie `Secure` flag'i aktif mi? (HTTPS zorunlu mu?)
- [ ] `SameSite` attribute doğru ayarlanmış mı?
- [ ] Login sayfasında CSRF koruması var mı? (`@Html.AntiForgeryToken()`)
- [ ] Hatalı login denemelerinde generic mesaj mı veriliyor? ("Kullanıcı adı veya şifre hatalı" — hangisinin yanlış olduğunu söyleme)
- [ ] SQL Injection'a açık bir yer var mı? (EF Core kullanıyorsan muhtemelen korumalısın, ama kontrol et)

---

## Bölüm 10: Bonus — İleride Yapılabilecekler (Opsiyonel)

Bu bölüm zorunlu değil, ama öğrenme isteğin varsa araştırabilirsin:

| Konu | Açıklama | Zorluk |
|---|---|---|
| Refresh Token | Access token süresi dolduğunda yeni token almak | ⭐⭐⭐ |
| Role-Based Authorization | Admin vs User rolleri, farklı sayfalara erişim | ⭐⭐ |
| ASP.NET Identity ile karşılaştırma | Aynı işi Identity ile yapıp farkları görmek | ⭐⭐ |
| Rate Limiting | Login endpoint'ine istek sınırı koymak | ⭐⭐ |
| Two-Factor Authentication (2FA) | İki adımlı doğrulama | ⭐⭐⭐⭐ |
| OAuth2 / OpenID Connect | Google/GitHub ile login | ⭐⭐⭐⭐ |

---

## Özet: Beklenen Çıktılar

Bu rehberi tamamladığında projenin şu yapıya sahip olmalı:

```
AspNetCoreCrudDemo/
├── Controllers/
│   ├── HomeController.cs
│   ├── StudentsController.cs      ← [Authorize] ile korunmuş
│   └── AccountController.cs       ← YENİ (Login, Register, Logout)
├── Models/
│   ├── Student.cs
│   ├── User.cs                    ← YENİ
│   ├── LoginViewModel.cs          ← YENİ
│   ├── RegisterViewModel.cs       ← YENİ
│   └── ErrorViewModel.cs
├── Services/
│   ├── IJwtTokenService.cs        ← YENİ
│   └── JwtTokenService.cs         ← YENİ
├── Views/
│   ├── Account/                   ← YENİ KLASÖR
│   │   ├── Login.cshtml
│   │   └── Register.cshtml
│   ├── Shared/
│   │   └── _Layout.cshtml         ← GÜNCELLEME (navbar)
│   └── ...
├── Program.cs                     ← GÜNCELLEME (JWT config)
├── appsettings.json               ← GÜNCELLEME (JwtSettings)
└── ...
```

> **Hatırla:** Amacın sadece kodu çalıştırmak değil, her adımda **neden** böyle yapıldığını anlamak. Bir şey çalışmıyorsa, hata mesajını oku, araştır, anla. Kopyala-yapıştır yerine anlayarak ilerle.
