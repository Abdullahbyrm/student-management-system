# Requirements Document — AspNetCoreCrudDemo

## 1. Fonksiyonel Gereksinimler (Functional Requirements)

### FR-01: Öğrenci Listeleme
- Sistem, veritabanında kayıtlı tüm öğrencileri bir tablo halinde göstermelidir.
- Tablo; öğrenci adı, e-posta, ders (kurs) ve kayıt tarihi sütunlarını içermelidir.
- Her satırda Edit, Details ve Delete bağlantıları bulunmalıdır.
- Tablonun üzerinde "Create New" bağlantısı ile yeni kayıt oluşturma sayfasına yönlendirme yapılmalıdır.

### FR-02: Yeni Öğrenci Oluşturma (Create)
- Kullanıcı, bir form aracılığıyla yeni öğrenci kaydı oluşturabilmelidir.
- Form alanları: Name (zorunlu), Email, Course, EnrollmentDate
- Form gönderildiğinde server-side model doğrulaması (ModelState.IsValid) yapılmalıdır.
- Başarılı kayıt sonrası kullanıcı öğrenci listesine yönlendirilmelidir.
- CSRF koruması için AntiForgeryToken kullanılmalıdır.

### FR-03: Öğrenci Detay Görüntüleme (Details)
- Kullanıcı, belirli bir öğrencinin detay bilgilerini görüntüleyebilmelidir.
- Detay sayfasında tüm alanlar (Name, Email, Course, EnrollmentDate) salt-okunur gösterilmelidir.
- Geçersiz veya bulunamayan ID için HTTP 404 yanıtı dönmelidir.

### FR-04: Öğrenci Bilgisi Düzenleme (Edit)
- Kullanıcı, var olan bir öğrencinin bilgilerini güncelleyebilmelidir.
- Düzenleme formu mevcut verilerle dolu olarak açılmalıdır.
- StudentId alanı hidden input olarak gönderilmelidir.
- URL'deki ID ile form'daki StudentId eşleşmezse HTTP 404 dönmelidir.
- Eşzamanlılık çakışması (DbUpdateConcurrencyException) ele alınmalıdır.

### FR-05: Öğrenci Silme (Delete)
- Kullanıcıya silme işlemi öncesinde bir onay sayfası gösterilmelidir.
- Onay sayfasında silinecek öğrencinin tüm bilgileri görüntülenmelidir.
- Onaydan sonra kayıt veritabanından kalıcı olarak silinmelidir.
- Silme sonrası kullanıcı öğrenci listesine yönlendirilmelidir.

### FR-06: Ana Sayfa ve Navigasyon
- Uygulama bir ana sayfa (Home/Index) ve gizlilik sayfası (Privacy) içermelidir.
- Navbar üzerinden Home, Privacy ve "Manage Students" sayfalarına erişilebilmelidir.

---

## 2. Teknik Gereksinimler (Technical Requirements)

### TR-01: Mimari
- Uygulama, ASP.NET Core MVC (Model-View-Controller) mimari desenini kullanmalıdır.
- Minimal hosting modeli (top-level statements) ile Program.cs yapılandırılmalıdır.

### TR-02: Veritabanı
- Veritabanı olarak Microsoft SQL Server kullanılmalıdır.
- Entity Framework Core 9.0 ORM aracı ile veritabanı erişimi sağlanmalıdır.
- Code-First yaklaşımı ile migration'lar aracılığıyla veritabanı şeması oluşturulmalıdır.
- Connection string `appsettings.json` dosyasında tanımlanmalıdır.

### TR-03: Veri Modeli — Student Entity

| Alan | Tip | Kısıtlamalar |
|---|---|---|
| StudentId | int | Primary Key, Identity (auto-increment) |
| Name | string | Required |
| Email | string | — |
| Course | string | — |
| EnrollmentDate | DateTime | — |

### TR-04: Framework & Paketler

| Paket | Versiyon | Amaç |
|---|---|---|
| .NET SDK | 9.0 | Çalışma zamanı (runtime) |
| Microsoft.EntityFrameworkCore | 9.0.0 | ORM |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.0 | SQL Server provider |
| Microsoft.EntityFrameworkCore.Design | 9.0.0 | Migration ve scaffolding araçları |
| Microsoft.EntityFrameworkCore.Tools | 9.0.0 | PMC (Package Manager Console) komutları |
| Microsoft.VisualStudio.Web.CodeGeneration.Design | 9.0.0 | Scaffolding (otomatik kod üretimi) |

### TR-05: Frontend
- Bootstrap (lib klasöründe yerel kopya) ile responsive tasarım sağlanmalıdır.
- jQuery ve jQuery Validation ile client-side form doğrulaması desteklenmelidir.
- Razor View Engine (.cshtml) ile sunucu taraflı HTML üretimi yapılmalıdır.
- Layout dosyası (_Layout.cshtml) ile tutarlı sayfa yapısı sağlanmalıdır.

### TR-06: Güvenlik
- Tüm POST formlarında `[ValidateAntiForgeryToken]` attribute kullanılmalıdır.
- `[Bind]` attribute ile overposting saldırılarına karşı koruma sağlanmalıdır.
- Production ortamında HSTS (HTTP Strict Transport Security) etkinleştirilmelidir.
- HTTPS yönlendirmesi aktif olmalıdır.

---

## 3. Fonksiyonel Olmayan Gereksinimler (Non-Functional Requirements)

### NFR-01: Performans
- Tüm veritabanı sorguları asenkron (async/await) olarak çalıştırılmalıdır.
- Sayfa yüklenme süreleri geliştirme ortamında 2 saniyenin altında olmalıdır.

### NFR-02: Kullanılabilirlik
- Uygulama mobil cihazlarda da kullanılabilir (responsive) olmalıdır.
- Form alanlarında Display Name attribute ile kullanıcı dostu etiketler gösterilmelidir.
- Hata durumlarında kullanıcıya anlaşılır bir hata sayfası (Error.cshtml) sunulmalıdır.

### NFR-03: Sürdürülebilirlik
- Kod, MVC ayrımına sadık kalarak organize edilmelidir.
- Her controller yalnızca kendi sorumluluk alanıyla ilgilenmelidir.
- DbContext, Dependency Injection ile controller'lara enjekte edilmelidir.

### NFR-04: Ortam Yapılandırması
- Development ve Production ortamları için ayrı ayar dosyaları (appsettings.json, appsettings.Development.json) kullanılmalıdır.
- launchSettings.json ile geliştirme profilleri tanımlanmalıdır.

---

## 4. Proje Klasör Yapısı (Mevcut Durum)

```
AspNetCoreCrudDemo/
├── Controllers/
│   ├── HomeController.cs
│   └── StudentsController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Migrations/
│   ├── 20250526080257_initial.cs
│   ├── 20250526080257_initial.Designer.cs
│   └── ApplicationDbContextModelSnapshot.cs
├── Models/
│   ├── ErrorViewModel.cs
│   └── Student.cs
├── Properties/
│   └── launchSettings.json
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── Privacy.cshtml
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   ├── _ValidationScriptsPartial.cshtml
│   │   └── Error.cshtml
│   ├── Students/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   ├── Details.cshtml
│   │   └── Delete.cshtml
│   ├── _ViewImports.cshtml
│   └── _ViewStart.cshtml
├── wwwroot/
│   ├── css/site.css
│   ├── js/site.js
│   ├── lib/ (bootstrap, jquery, jquery-validation)
│   └── favicon.ico
├── appsettings.json
├── appsettings.Development.json
├── AspNetCoreCrudDemo.csproj
└── Program.cs
```
