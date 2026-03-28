# Development Plan — ASP.NET Core 9.0 Student CRUD

> **Belge Türü:** Execution Plan (Mentor → Developer)
> **Proje:** Öğrenci Yönetim Sistemi — ASP.NET Core 9.0 MVC + EF Core + SQL Server
> **Kaynak Repo:** github.com/adminManoj/Asp.NetCrudTutorial9.0-
> **Hedef:** Bu projeyi sıfırdan, bağımsız olarak inşa edebilecek seviyeye ulaşmak

---

## Nasıl Kullanılmalı?

Bu plan bir mentorluk aracıdır. Her phase bir "kapı"dır — kapıyı geçmeden sonrakine adım atılmaz. Mentor olarak geliştiriciye şunu söyleyebilirsin:

- *"Phase 2'nin tüm deliverable'larını göster, sonra Phase 3'e geçiyoruz."*
- *"Completion criteria'yı karşılamadıysan phase bitmemiş demektir."*
- *"Dikkat noktalarını okumadan koda dokunma."*

Her phase'in sonunda geliştirici somut bir çıktı (deliverable) üretmiş olmalıdır. Çıktı yoksa ilerleme yoktur.

---

## Genel Akış (Overview)

```
PHASE 0  Ortam Kurulumu & Doğrulama
   ↓
PHASE 1  Proje İskeleti Oluşturma
   ↓
PHASE 2  Veri Modeli (Entity) Tasarımı
   ↓
PHASE 3  Veritabanı Bağlantısı & DbContext
   ↓
PHASE 4  Migration & Veritabanı Oluşturma
   ↓
PHASE 5  Controller — CRUD Action'ları
   ↓
PHASE 6  View Katmanı (Razor)
   ↓
PHASE 7  Validation & Güvenlik
   ↓
PHASE 8  Hata Yönetimi & Loglama
   ↓
PHASE 9  Mimari İyileştirme (Repository Pattern)
   ↓
PHASE 10 Test Katmanı
   ↓
PHASE 11 Deploy Hazırlığı
```

---

## PHASE 0 — Ortam Kurulumu & Doğrulama

### Amaç
Geliştirme ortamını kurmak, tüm araçların doğru versiyonda çalıştığını doğrulamak. Ortam sorunları Phase 3-4'te patlar; o aşamada geri dönmek çok maliyetlidir.

### Yapılacak İşler
1. .NET 9.0 SDK'yı kur
2. IDE seç ve kur (Visual Studio 2022 veya VS Code + C# Dev Kit)
3. SQL Server kur (Express veya Developer Edition)
4. SQL Server Management Studio (SSMS) kur
5. `dotnet-ef` global tool'u kur
6. Her aracın çalıştığını terminal üzerinden doğrula

### Deliverables
- [ ] `dotnet --version` çıktısının `9.0.x` olduğunu gösteren ekran görüntüsü veya terminal çıktısı
- [ ] `dotnet ef --version` çıktısı
- [ ] SSMS üzerinden SQL Server'a başarıyla bağlantı kurulduğunu gösteren ekran görüntüsü
- [ ] Sunucu adının (Server Name) not alınmış olması (Phase 3'te lazım)

### Bağımlılıklar
Yok. Bu ilk adım.

### Dikkat Edilmesi Gerekenler
- SQL Server kurulumunda **Mixed Mode Authentication** seç; ileride connection string esnekliği sağlar.
- `dotnet-ef` tool'u global olarak kurulmalı, proje bazlı değil. Aksi halde migration komutları çalışmaz.
- VS Code kullanılacaksa `C# Dev Kit` extension'ı şart; yoksa IntelliSense çalışmaz.

### Completion Criteria
> Terminal üzerinden `dotnet new --list` komutu çalışıyor, `dotnet ef` komutu tanınıyor, SSMS ile SQL Server instance'ına bağlanılabiliyor. Üç araç da hatasız yanıt veriyor.

---

## PHASE 1 — Proje İskeleti Oluşturma

### Amaç
MVC şablonuyla boş bir proje oluşturmak, varsayılan klasör yapısını anlamak, projenin "merhaba dünya" seviyesinde çalıştığını görmek.

### Yapılacak İşler
1. `dotnet new mvc` komutuyla proje oluştur
2. Oluşan klasör yapısını incele — her klasörün ne iş yaptığını yaz
3. `dotnet run` ile projeyi ayağa kaldır
4. Tarayıcıda varsayılan ana sayfanın açıldığını doğrula
5. `Program.cs` dosyasını satır satır oku ve middleware pipeline'ı anla

### Deliverables
- [ ] Çalışan bir MVC projesi (`dotnet run` ile hatasız ayağa kalkıyor)
- [ ] Tarayıcıda açılan varsayılan sayfanın ekran görüntüsü
- [ ] Oluşan klasör yapısının kendi cümleleriyle açıklaması (1-2 cümle/klasör):
  - `Controllers/` → ?
  - `Models/` → ?
  - `Views/` → ?
  - `wwwroot/` → ?
  - `Program.cs` → ?

### Bağımlılıklar
Phase 0 tamamlanmış olmalı.

### Dikkat Edilmesi Gerekenler
- Proje adı `AspNetCoreCrudDemo` olmalı; namespace'ler ve referanslar buna göre şekillenir.
- `dotnet new mvc` komutu `HomeController`, `ErrorViewModel`, Layout ve varsayılan view'ları otomatik üretir. Bunları silme, üzerine inşa edeceksin.
- `Program.cs` dosyası .NET 6+ minimal hosting modeli kullanır (top-level statements). `Startup.cs` yok — her şey tek dosyada.

### Completion Criteria
> Proje `dotnet run` ile hatasız ayağa kalkıyor. Geliştirici, oluşan her klasörün rolünü kendi cümleleriyle açıklayabiliyor. `Program.cs`'teki `AddControllersWithViews()` ve `MapControllerRoute()` satırlarının ne yaptığını söyleyebiliyor.

---

## PHASE 2 — Veri Modeli (Entity) Tasarımı

### Amaç
Uygulamanın temel veri yapısını (Student entity) tanımlamak. Bu model, veritabanı tablosunun, formların ve view'ların temelini oluşturur.

### Yapılacak İşler
1. `Models/Student.cs` dosyasını oluştur
2. Property'leri tanımla: `StudentId`, `Name`, `Email`, `Course`, `EnrollmentDate`
3. Data Annotation attribute'larını ekle: `[Key]`, `[Required]`, `[Display]`
4. Projenin hâlâ derlenebildiğini doğrula (`dotnet build`)

### Deliverables
- [ ] `Models/Student.cs` dosyası — 5 property, uygun data annotation'lar
- [ ] `dotnet build` başarıyla tamamlanıyor (sıfır hata)
- [ ] Her attribute'un ne işe yaradığının kendi cümleleriyle açıklaması:
  - `[Key]` → ?
  - `[Required]` → ?
  - `[Display(Name = "...")]` → ?

### Bağımlılıklar
Phase 1 tamamlanmış olmalı.

### Dikkat Edilmesi Gerekenler
- `StudentId` adlandırması bir EF Core konvansiyonudur. `<ClassName>Id` veya `Id` pattern'i otomatik olarak Primary Key + Identity olarak algılanır. `[Key]` attribute'u burada zorunlu değildir ama açıklayıcılık sağlar.
- `string` property'ler nullable reference type uyarısı verebilir (.NET 9'da `<Nullable>enable</Nullable>` aktif). Bunu şimdilik görmezden gelebilir ya da `= string.Empty;` ile initialize edebilirsin.
- Modeli şimdi basit tut. Validation derinleştirmesi Phase 7'de yapılacak.

### Completion Criteria
> `Student.cs` dosyası var, doğru namespace'te, `dotnet build` hatasız. Geliştirici, "EF Core bu sınıfı nasıl bir SQL tablosuna çevirir?" sorusuna cevap verebiliyor.

---

## PHASE 3 — Veritabanı Bağlantısı & DbContext

### Amaç
Entity Framework Core'u projeye entegre etmek: NuGet paketlerini yüklemek, DbContext sınıfını yazmak, connection string'i tanımlamak ve DI container'a kaydetmek.

### Yapılacak İşler
1. EF Core NuGet paketlerini yükle (4 paket + scaffolding paketi)
2. `Data/ApplicationDbContext.cs` dosyasını oluştur
3. `appsettings.json`'a connection string ekle (kendi SQL Server adınla)
4. `Program.cs`'te `AddDbContext<ApplicationDbContext>()` ile DI kaydını yap
5. `dotnet build` ile her şeyin derlendiğini doğrula

### Deliverables
- [ ] `dotnet list package` çıktısı — 5 paketin yüklü olduğu görülmeli:
  - `Microsoft.EntityFrameworkCore`
  - `Microsoft.EntityFrameworkCore.SqlServer`
  - `Microsoft.EntityFrameworkCore.Design`
  - `Microsoft.EntityFrameworkCore.Tools`
  - `Microsoft.VisualStudio.Web.CodeGeneration.Design`
- [ ] `Data/ApplicationDbContext.cs` dosyası — `DbSet<Student> Students` property'si var
- [ ] `appsettings.json` — `ConnectionStrings:DefaultConnection` tanımlı
- [ ] `Program.cs` — `AddDbContext` satırı eklendi
- [ ] `dotnet build` başarılı

### Bağımlılıklar
Phase 2 tamamlanmış olmalı (Student modeli var olmalı, çünkü DbContext onu referans eder).

### Dikkat Edilmesi Gerekenler
- Connection string'deki `Server=` değeri Phase 0'da not aldığın sunucu adı olmalı. Yanlış sunucu adı Phase 4'te migration'ı patlatır ve hatanın kaynağını bulmak zor olabilir.
- `TrustServerCertificate=True` eklemeyi unutma; yoksa SSL hatası alırsın.
- `EntityFrameworkCore.Design` ve `EntityFrameworkCore.Tools` paketleri runtime'da kullanılmaz, yalnızca development aracıdır. `.csproj`'da `PrivateAssets=all` olduklarına dikkat et.
- `AddDbContext` çağrısı **Scoped** lifetime ile kayıt yapar. Bu, her HTTP isteği için yeni bir DbContext oluşturulup istek sonunda dispose edilmesi demektir. Bu davranışı anlamak ileride concurrency sorunlarını önler.

### Completion Criteria
> `dotnet build` hatasız. Geliştirici şu soruları cevaplayabiliyor: "DbContext ne iş yapar?", "DbSet ne demek?", "Connection string nereye yazılır ve nereden okunur?", "AddDbContext neden Program.cs'te?"

---

## PHASE 4 — Migration & Veritabanı Oluşturma

### Amaç
Code-First yaklaşımıyla veritabanı şemasını oluşturmak. C# model sınıfından SQL tablosuna giden yolu görmek ve anlamak.

### Yapılacak İşler
1. `dotnet ef migrations add initial` komutunu çalıştır
2. Oluşan migration dosyalarını incele (3 dosya)
3. `Up()` metodundaki SQL karşılıklarını anla
4. `dotnet ef database update` ile veritabanına uygula
5. SSMS'te veritabanını ve tabloyu doğrula

### Deliverables
- [ ] `Migrations/` klasöründe 3 dosya:
  - `YYYYMMDD_initial.cs` (Up/Down metodları)
  - `YYYYMMDD_initial.Designer.cs`
  - `ApplicationDbContextModelSnapshot.cs`
- [ ] SSMS'te `AspNetCoreCrudDb` veritabanı oluşmuş
- [ ] `Students` tablosu var — sütunlar: `StudentId (PK, Identity)`, `Name`, `Email`, `Course`, `EnrollmentDate`
- [ ] `__EFMigrationsHistory` tablosu var ve bir kayıt içeriyor
- [ ] Geliştirici, `Up()` metodundaki her satırın SQL karşılığını açıklayabiliyor

### Bağımlılıklar
Phase 3 tamamlanmış olmalı (DbContext + connection string + DI kaydı). Connection string'in geçerli olması şart — buradaki hata bu phase'i tamamen bloklar.

### Dikkat Edilmesi Gerekenler
- `dotnet ef` komutu bulamazsa: `dotnet tool install --global dotnet-ef` çalıştır (Phase 0'da yapılmış olmalı).
- "Cannot connect to SQL Server" hatası alırsan: SSMS'te aynı connection string ile bağlanmayı dene; sorun connection string'teyse Phase 3'e geri dön.
- Migration dosyasını **asla elle değiştirme** (bu aşamada). İleride advanced senaryolarda gerekebilir, ama şimdi değil.
- `Down()` metodu geri alma (rollback) içindir: `dotnet ef database update 0` ile veritabanını sıfırlayabilirsin. Bunu bir kere dene ve tekrar `update` yap — migration döngüsünü kavra.

### Completion Criteria
> SSMS'te `Students` tablosu görünüyor, sütun tipleri beklenen şekilde. `__EFMigrationsHistory` tablosunda `initial` migration kaydı var. Geliştirici "Code-First nedir, migration ne iş yapar?" sorusuna üç cümleyle cevap verebiliyor.

---

## PHASE 5 — Controller: CRUD Action'ları

### Amaç
Uygulamanın iş mantığını taşıyan controller'ı yazmak. 5 temel CRUD operasyonunu (List, Detail, Create, Edit, Delete) action metotları olarak implement etmek.

### Yapılacak İşler
1. `Controllers/StudentsController.cs` dosyasını oluştur
2. Constructor injection ile `ApplicationDbContext` al
3. Şu action'ları implement et:
   - `Index()` → GET — tüm öğrencileri listele
   - `Details(int? id)` → GET — tek öğrenci detayı
   - `Create()` → GET — boş form göster
   - `Create(Student)` → POST — yeni kayıt oluştur
   - `Edit(int? id)` → GET — düzenleme formu göster
   - `Edit(int id, Student)` → POST — güncelleme yap
   - `Delete(int? id)` → GET — silme onayı göster
   - `DeleteConfirmed(int id)` → POST — sil
4. Her action'da null/not-found kontrolleri yap
5. Her POST action'a `[ValidateAntiForgeryToken]` ekle

### Deliverables
- [ ] `StudentsController.cs` — 8 action metodu, tümü async
- [ ] `dotnet build` başarılı
- [ ] Her action için kısa açıklama (1-2 cümle): ne yapar, hangi HTTP method'u kabul eder, ne döner
- [ ] Şu kavramların açıklanması:
  - Constructor Injection → nasıl çalışıyor?
  - `[HttpPost]` vs `[HttpGet]` → fark ne?
  - `[Bind("...")]` → neden var?
  - `ModelState.IsValid` → ne kontrol ediyor?
  - `RedirectToAction` → neden doğrudan View dönmüyoruz?

### Bağımlılıklar
Phase 4 tamamlanmış olmalı (veritabanı var, DbContext çalışıyor).

### Dikkat Edilmesi Gerekenler
- Bu phase'de henüz View yok. Controller'ı yazıp `dotnet build` ile derlenmesini doğrula; çalıştırma (run) Phase 6'da olacak.
- `Edit` action'ında `DbUpdateConcurrencyException` yakalanmalı. Bu, iki kullanıcı aynı anda aynı kaydı düzenlediğinde oluşan race condition'dır. Şimdi tam anlamak zorunda değilsin ama kodda olmalı.
- `[HttpPost, ActionName("Delete")]` pattern'ine dikkat: C#'ta aynı isimde iki metot olamaz, bu yüzden metot adı `DeleteConfirmed` ama route adı `Delete` olarak kalır.
- POST → Redirect → GET pattern'i (PRG) çift form gönderimini engeller. Bu bir web standart pratiğidir; controller dışında da bilinmesi gerekir.

### Completion Criteria
> `StudentsController.cs` derlenebilir durumda. 8 action metodu var, null kontrolleri ve anti-forgery token koruması mevcut. Geliştirici, "bir kullanıcı /Students/Edit/5 adresine gittiğinde sırayla neler olur?" sorusunu adım adım anlatabilir.

---

## PHASE 6 — View Katmanı (Razor)

### Amaç
Kullanıcının göreceği ekranları oluşturmak. Her CRUD operasyonu için bir Razor view yazmak ve layout üzerinden navigasyonu bağlamak.

### Yapılacak İşler
1. `Views/Students/` klasörünü oluştur
2. 5 view dosyası yaz:
   - `Index.cshtml` — öğrenci listesi (tablo)
   - `Create.cshtml` — yeni kayıt formu
   - `Edit.cshtml` — düzenleme formu
   - `Details.cshtml` — salt-okunur detay
   - `Delete.cshtml` — silme onayı
3. `_Layout.cshtml`'e "Manage Students" navigasyon linki ekle
4. Uygulamayı çalıştır ve tüm CRUD akışını test et

### Deliverables
- [ ] 5 view dosyası `Views/Students/` altında
- [ ] Navigasyon çubuğunda "Manage Students" linki görünüyor
- [ ] **Tam CRUD döngüsü çalışıyor — canlı demo:**
  1. `/Students` → boş liste görünür
  2. "Create New" → form doldur → kaydet → listede görünür
  3. "Edit" → bilgiyi değiştir → kaydet → güncelleme yansır
  4. "Details" → tüm alanlar salt-okunur gösterilir
  5. "Delete" → onay sayfası → sil → listeden kaybolur
- [ ] SSMS'te verinin gerçekten oluşup silindiğini doğrula

### Bağımlılıklar
Phase 5 tamamlanmış olmalı (Controller var ve derlenebilir durumda).

### Dikkat Edilmesi Gerekenler
- View isimleri action isimleriyle eşleşmeli: `Index` action'ı → `Index.cshtml` arar. Eşleşmezse "view not found" hatası alırsın.
- `@model` direktifi her view'ın en üstünde olmalı. Yanlış model tipi binding hatalarına neden olur.
- `Edit.cshtml`'de `StudentId` mutlaka `<input type="hidden">` olarak gönderilmeli. Yoksa EF Core hangi kaydın güncelleneceğini bilemez.
- `_ValidationScriptsPartial` yalnızca form içeren view'lara eklenir (Create, Edit). Index ve Details'a ekleme.
- **Bu phase kritik bir dönüm noktasıdır.** Phase 6 sonunda çalışan, kullanılabilir bir uygulaman olacak. Sonraki phase'ler iyileştirmedir.

### Completion Criteria
> Uygulama `dotnet run` ile çalışıyor. 5 CRUD ekranı hatasız açılıyor. Bir kayıt oluşturulabiliyor, düzenlenebiliyor, detayı görülebiliyor ve silinebiliyor. Geliştirici bu döngüyü birinin karşısında canlı olarak gösterebilir.

---

## ✅ MİLESTONE — Çalışan MVP

> Phase 0-6 tamamlandığında ortada **tam çalışan bir CRUD uygulaması** vardır.
> Buradan sonraki phase'ler (7-11) uygulamayı production-ready seviyeye taşıyan kalite, güvenlik ve mimari iyileştirmelerdir.
> Geliştirici burada bir mola verip yaptığını sindirmelidir.

---

## PHASE 7 — Validation & Güvenlik Derinleştirme

### Amaç
Form doğrulamalarını güçlendirmek, güvenlik mekanizmalarını anlamak ve bilinçli hale getirmek.

### Yapılacak İşler
1. `Student` modeline ek validation attribute'ları ekle:
   - `[StringLength]` — Name için min/max karakter
   - `[EmailAddress]` — Email formatı
   - `[DataType(DataType.Date)]` — EnrollmentDate için tarih formatı
   - Tüm string alanlara `[Required]` (halihazırda sadece Name'de var)
2. Custom `ErrorMessage` parametreleri ekle (Türkçe veya İngilizce)
3. Tarayıcıda client-side validation'ın çalıştığını doğrula (form göndermeden hata mesajı çıkıyor mu?)
4. JavaScript'i devre dışı bırakıp server-side validation'ın da çalıştığını doğrula
5. Mevcut güvenlik mekanizmalarını dokümante et

### Deliverables
- [ ] Güncellenmiş `Student.cs` — zenginleştirilmiş data annotation'lar
- [ ] Client-side validation çalışıyor (ekran görüntüsü: form göndermeden hata mesajları)
- [ ] Server-side validation çalışıyor (JS kapalıyken test)
- [ ] Güvenlik mekanizmaları listesi (kendi cümleleriyle):
  - `[ValidateAntiForgeryToken]` → ne koruyor?
  - `[Bind("...")]` → ne koruyor?
  - `app.UseHsts()` → ne yapar?
  - `app.UseHttpsRedirection()` → ne yapar?

### Bağımlılıklar
Phase 6 tamamlanmış olmalı (çalışan formlar mevcut).

### Dikkat Edilmesi Gerekenler
- Client-side validation iyi bir UX sağlar ama **asla güvenlik katmanı olarak güvenilmez**. Bir saldırgan tarayıcı konsolundan JavaScript'i devre dışı bırakabilir. Bu yüzden server-side validation her zaman aktif olmalıdır.
- `[EmailAddress]` attribute'u basit bir regex kontrolü yapar; %100 doğrulama için harici kütüphane gerekir. Ama bu aşama için yeterlidir.
- Model değişiklikleri yaptıktan sonra yeni migration gerekebilir (string length gibi constraint'ler veritabanı şemasını etkiler). Phase 4'e dönerek `dotnet ef migrations add AddValidationConstraints` çalıştırmayı düşün.

### Completion Criteria
> Tüm form alanlarında hem client-side hem server-side validation çalışıyor. Geliştirici CSRF, overposting ve HSTS kavramlarını açıklayabiliyor. Geçersiz veri ile form gönderme denenmiş ve reddedilmiş.

---

## PHASE 8 — Hata Yönetimi & Loglama

### Amaç
Beklenmeyen hatalarda uygulamanın düzgün davranmasını sağlamak. Development ve Production ortamları için farklı hata davranışları tanımlamak. Loglama altyapısını kurmak.

### Yapılacak İşler
1. `Program.cs`'teki mevcut hata yönetimi middleware'ini incele ve anla
2. Development ortamında `UseDeveloperExceptionPage()` davranışını gözlemle (kasıtlı bir hata tetikle)
3. Production modda `UseExceptionHandler("/Home/Error")` davranışını test et
4. Controller'a `ILogger<StudentsController>` enjekte et
5. Kritik noktalara log satırları ekle (bilgi, uyarı, hata seviyeleri)
6. 404 durumlarını ele al: var olmayan bir ID ile istek yap, sonucu gözlemle

### Deliverables
- [ ] Development modda hata detayının göründüğü ekran görüntüsü
- [ ] Production modda genel hata sayfasının göründüğü ekran görüntüsü
- [ ] Controller'da en az 3 yere log satırı eklenmiş:
  - Bir `LogInformation` (başarılı işlem)
  - Bir `LogWarning` (kayıt bulunamadı)
  - Bir `LogError` (exception yakalandı)
- [ ] Konsol çıktısında log mesajlarının göründüğü ekran görüntüsü
- [ ] Ortam ayrımının nasıl çalıştığının açıklaması:
  - `appsettings.json` vs `appsettings.Development.json` → fark ne?
  - `ASPNETCORE_ENVIRONMENT` değişkeni → ne yapar?

### Bağımlılıklar
Phase 7 tamamlanmış olmalı.

### Dikkat Edilmesi Gerekenler
- **Asla Production'da detaylı hata mesajı gösterme.** Stack trace, connection string gibi bilgileri dışarıya sızdırırsın.
- Log seviyeleri hiyerarşiktir: `Trace < Debug < Information < Warning < Error < Critical`. `appsettings.json`'daki `LogLevel` ayarı hangi seviyenin konsola yazılacağını belirler.
- Bu phase'de henüz Serilog veya NLog gibi harici loglama kütüphaneleri kullanma. Built-in `ILogger` yeterli. Amacımız loglama konseptini öğrenmek.

### Completion Criteria
> Hata yönetimi ortama göre farklı davranıyor. Log mesajları konsola yazılıyor ve doğru seviyede. Geliştirici "Production'da neden detaylı hata göstermeyiz?" sorusuna somut bir güvenlik gerekçesiyle cevap verebiliyor.

---

## PHASE 9 — Mimari İyileştirme: Repository Pattern

### Amaç
Controller'ı veritabanı erişim detaylarından soyutlamak. Repository pattern ile sorumluluk ayrımı (Separation of Concerns) sağlamak. Bu yapı, test yazılabilirliğin de önkoşuludur.

### Yapılacak İşler
1. `Interfaces/IStudentRepository.cs` arayüzünü tanımla (6 metot)
2. `Repositories/StudentRepository.cs` implementasyonunu yaz
3. `Program.cs`'te DI kaydını yap: `AddScoped<IStudentRepository, StudentRepository>()`
4. `StudentsController`'ı refactor et: `ApplicationDbContext` yerine `IStudentRepository` alsın
5. Uygulamanın hâlâ aynı şekilde çalıştığını doğrula (regresyon testi)

### Deliverables
- [ ] `Interfaces/IStudentRepository.cs` — 6 metot imzası
- [ ] `Repositories/StudentRepository.cs` — implementasyon
- [ ] `Program.cs`'te DI kaydı
- [ ] Refactor edilmiş `StudentsController.cs` — artık interface'e bağımlı
- [ ] CRUD döngüsü hâlâ çalışıyor (regresyon testi — Phase 6'daki aynı 5 adım)
- [ ] Güncellenmiş klasör yapısı:
  ```
  AspNetCoreCrudDemo/
  ├── Controllers/
  ├── Data/
  ├── Interfaces/       ← YENİ
  ├── Repositories/     ← YENİ
  ├── Models/
  ├── Views/
  ...
  ```
- [ ] Şu soruların yanıtı:
  - "Controller neden doğrudan DbContext kullanmamalı?"
  - "Interface kullanmak ne avantaj sağlıyor?"
  - "`AddScoped` ne demek? `AddTransient` veya `AddSingleton`'dan farkı ne?"

### Bağımlılıklar
Phase 8 tamamlanmış olmalı. Bu phase bir refactoring phase'idir; mevcut çalışan kodu bozmadan dönüştürme gerektirir.

### Dikkat Edilmesi Gerekenler
- **Bu phase'de yeni özellik ekleme.** Sadece mevcut kodu yeniden yapılandır. Refactoring sırasında scope genişletmek en sık yapılan hatadır.
- Her refactoring adımından sonra `dotnet build` çalıştır. Küçük adımlarla ilerle.
- Repository içindeki her metot tek bir sorumluluk taşımalı: `GetAllAsync` sadece getirme yapar, loglama veya business logic karıştırma.
- `AddScoped` doğru seçimdir çünkü DbContext da Scoped'dır. Repository lifetime'ı DbContext lifetime'ını aşmamalı.

### Completion Criteria
> Controller artık `IStudentRepository`'ye bağımlı, `ApplicationDbContext`'e değil. Tüm CRUD operasyonları hâlâ çalışıyor. Geliştirici "Bu değişiklik kullanıcı için hiçbir şeyi değiştirmedi, peki neden yaptık?" sorusuna mimari bir gerekçeyle cevap verebiliyor.

---

## PHASE 10 — Test Katmanı

### Amaç
Repository katmanı için unit test yazmak. Test altyapısını kurmak, temel test pattern'ini (Arrange-Act-Assert) öğrenmek.

### Yapılacak İşler
1. Ayrı bir test projesi oluştur (`dotnet new xunit`)
2. Ana projeyi referans olarak ekle
3. `Microsoft.EntityFrameworkCore.InMemory` paketini test projesine ekle
4. En az 5 unit test yaz:
   - Kayıt ekleme (Add) → başarılı
   - Tüm kayıtları listeleme (GetAll) → doğru sayı
   - ID ile getirme (GetById) → bulunan
   - ID ile getirme (GetById) → bulunamayan (null)
   - Kayıt silme (Delete) → başarılı
5. Testleri çalıştır ve tümünün geçtiğini doğrula

### Deliverables
- [ ] `AspNetCoreCrudDemo.Tests/` proje klasörü
- [ ] En az 5 test metodu — tümü geçiyor (`dotnet test` çıktısı)
- [ ] Her test Arrange-Act-Assert pattern'ini takip ediyor
- [ ] InMemoryDatabase kullanılıyor (gerçek SQL Server'a bağımlılık yok)
- [ ] Şu kavramların açıklaması:
  - "Unit test neden gerçek veritabanı kullanmamalı?"
  - "InMemoryDatabase ne sağlıyor?"
  - "Arrange-Act-Assert ne demek?"

### Bağımlılıklar
Phase 9 tamamlanmış olmalı (Repository pattern mevcut — test edilecek şey budur). Eğer Phase 9 atlandıysa, controller testleri çok daha karmaşık olur.

### Dikkat Edilmesi Gerekenler
- Her test birbirinden bağımsız olmalı. Paylaşılan state testleri kırılgan yapar. Her testte yeni bir InMemoryDatabase oluştur (`Guid.NewGuid()` ile benzersiz isim ver).
- InMemoryDatabase bazı SQL Server davranışlarını simüle etmez (ör: foreign key constraint, stored procedure). Ama bu aşama için yeterlidir.
- Test projesinin namespace'i ana projeden farklı olmalı: `AspNetCoreCrudDemo.Tests`.

### Completion Criteria
> `dotnet test` komutu çalışıyor, 5 testin tümü yeşil. Geliştirici bir testi kasıtlı olarak kırabiliyor (ör: beklenen değeri değiştir) ve kırmızı görebiliyor. "Neden test yazarız?" sorusuna pratik bir gerekçe verebiliyor.

---

## PHASE 11 — Deploy Hazırlığı

### Amaç
Uygulamayı geliştirme ortamından çıkarıp yayınlanabilir (publishable) hale getirmek. Deploy seçeneklerini anlamak.

### Yapılacak İşler
1. `dotnet publish -c Release -o ./publish` komutunu çalıştır
2. `publish/` klasörünün içeriğini incele
3. `appsettings.Production.json` dosyası oluştur (production connection string)
4. Production'da environment variable ile `ASPNETCORE_ENVIRONMENT=Production` ayarla
5. Opsiyonel: basit bir Dockerfile yaz
6. Deploy seçeneklerini araştır ve karşılaştır (IIS, Azure, Docker, Linux)

### Deliverables
- [ ] `publish/` klasörü oluşmuş ve dosyalar mevcut
- [ ] `appsettings.Production.json` dosyası (hassas bilgiler placeholder olarak)
- [ ] Deploy seçeneklerinin karşılaştırma tablosu (kendi cümleleriyle, en az 3 seçenek):
  - Seçenek adı → Ne zaman kullanılır? → Avantajı/Dezavantajı
- [ ] Opsiyonel: `Dockerfile` dosyası
- [ ] Şu soruların yanıtı:
  - "`dotnet run` ile `dotnet publish` farkı ne?"
  - "Production'da connection string'i nereye koymalıyım?"
  - "Neden `appsettings.json`'a production şifresi yazmamalıyım?"

### Bağımlılıklar
Phase 10 tamamlanmış olmalı. Testler geçmiyor ise deploy anlamsızdır.

### Dikkat Edilmesi Gerekenler
- **Production connection string'ini asla source code'a veya git repo'ya koyma.** Environment variable, Azure Key Vault veya User Secrets kullan.
- `dotnet publish` komutu `Release` konfigürasyonuyla optimize edilmiş çıktı üretir; Debug sembollerini içermez.
- Gerçek bir deploy yapmak bu phase'in zorunlu parçası değil. Amaç publish sürecini anlamak ve deploy seçeneklerini bilmek.

### Completion Criteria
> `publish/` klasörü var ve çalıştırılabilir durumda. Geliştirici development ile production ortamı arasındaki farkları, güvenlik açısından hassas bilgilerin nasıl yönetilmesi gerektiğini açıklayabiliyor.

---

## Özet Kontrol Tablosu

| Phase | Çıktı | Kritik mi? | Tahmini Süre |
|---|---|---|---|
| 0 — Ortam Kurulumu | Araçlar doğrulanmış | Blocker | 1-2 saat |
| 1 — Proje İskeleti | Çalışan boş MVC projesi | Blocker | 30 dk |
| 2 — Model | `Student.cs` dosyası | Blocker | 30 dk |
| 3 — DbContext | Veritabanı bağlantı altyapısı | Blocker | 1 saat |
| 4 — Migration | Veritabanı + tablo | Blocker | 30 dk |
| 5 — Controller | 8 action metodu | Blocker | 2-3 saat |
| 6 — Views | 5 view + çalışan CRUD | **MVP** | 2-3 saat |
| 7 — Validation | Güçlendirilmiş doğrulama | Önemli | 1-2 saat |
| 8 — Error Handling | Loglama + ortam ayrımı | Önemli | 1-2 saat |
| 9 — Repository | Refactored mimari | Önemli | 2-3 saat |
| 10 — Test | 5+ unit test geçiyor | Önemli | 2-3 saat |
| 11 — Deploy | Publish + ortam bilgisi | Nice-to-have | 1-2 saat |

**Toplam tahmini süre: 15-22 saat** (tam bir öğrenme hızıyla, acelesiz).

---

## Mentor İçin Notlar

**Phase 0-6 arası sıralıdır ve atlanamaz.** Her phase bir öncekinin çıktısına bağımlıdır. Geliştirici "ben zaten biliyorum" dese bile deliverable'ları göstermeli.

**Phase 6 sonrası MVP tamamlanmıştır.** Buradan sonraki phase'ler bağımsız olarak ele alınabilir; örneğin önce Phase 10 (test) yapılıp sonra Phase 9 (repository) yapılabilir — ama Phase 9 test yazılabilirliği kolaylaştırdığı için önerilen sıra budur.

**Her phase sonunda 5 dakikalık sözlü review yapın.** Geliştirici deliverable'ları göstersin ve "Dikkat Edilmesi Gerekenler" bölümündeki kavramları kendi cümleleriyle açıklasın. Ezbere kod yazmak ile mimariyi anlamak arasındaki fark burada ortaya çıkar.

**Hata yapmasına izin verin.** Özellikle Phase 3-4'te yanlış connection string yazmak ve Phase 5-6'da view adını yanlış koymak çok öğreticidir. Hatayı görmeden önce çözümü vermeyin.
