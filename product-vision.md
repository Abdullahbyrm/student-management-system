# Product Vision — AspNetCoreCrudDemo

## Proje Özeti

**AspNetCoreCrudDemo**, ASP.NET Core 9.0 MVC mimarisi üzerine inşa edilmiş bir **Öğrenci Yönetim Sistemi** (Student Management System) uygulamasıdır. Uygulama, öğrenci kayıtlarını oluşturma, listeleme, düzenleme ve silme (CRUD) işlemlerini gerçekleştiren temel bir web uygulamasıdır.

---

## Vizyon Beyanı

> Eğitim kurumlarının öğrenci kayıtlarını dijital ortamda hızlı, güvenli ve kolay bir şekilde yönetebilmesini sağlayan, sade ve genişletilebilir bir web uygulaması sunmak.

---

## Hedef Kullanıcılar

| Kullanıcı Profili | Açıklama |
|---|---|
| Eğitim Yöneticileri | Öğrenci kayıtlarını oluşturan, güncelleyen ve silen okul/kurum yöneticileri |
| Sekreterlik / Kayıt Ofisi | Günlük öğrenci kayıt işlemlerini yürüten personel |
| Geliştiriciler (Öğrenim Amaçlı) | ASP.NET Core MVC ile CRUD operasyonlarını öğrenmek isteyen yazılımcılar |

---

## Çözdüğü Problem

Geleneksel kağıt bazlı veya dağınık Excel tablolarına dayalı öğrenci kayıt süreçleri hataya açık, zaman alıcı ve ölçeklenemezdir. Bu uygulama, merkezi bir veritabanı üzerinden web tabanlı erişim sunarak bu sorunları çözer.

---

## Temel Değer Önerisi

- **Basitlik**: Minimal bir arayüzle temel CRUD işlemlerini kolayca gerçekleştirme
- **Hız**: Entity Framework Core ile veritabanı işlemlerinin asenkron olarak hızlıca yapılması
- **Genişletilebilirlik**: MVC mimarisi sayesinde yeni özellikler (kimlik doğrulama, raporlama, rol yönetimi) kolayca eklenebilir
- **Modern Teknoloji**: .NET 9.0 üzerinde çalışarak en güncel framework desteğinden yararlanma

---

## Ürün Kapsamı (Mevcut Durum)

### Var Olan Özellikler
- Öğrenci listesini görüntüleme (Index)
- Yeni öğrenci kaydı oluşturma (Create)
- Mevcut öğrenci bilgilerini düzenleme (Edit)
- Öğrenci detaylarını görüntüleme (Details)
- Öğrenci kaydını silme (Delete)
- Anti-forgery token ile CSRF koruması
- Client-side ve server-side validation desteği
- Bootstrap tabanlı responsive tasarım

### Mevcut Olmayan / Gelecekte Eklenebilecek Özellikler
- Kimlik doğrulama ve yetkilendirme (Authentication & Authorization)
- Sayfalama (Pagination) ve arama/filtreleme
- Loglama ve detaylı hata yönetimi
- API katmanı (REST API)
- Birim testleri (Unit Tests)
- Repository / Service katmanı (Clean Architecture)
- Daha zengin UI (DataTables, SPA framework entegrasyonu)

---

## Başarı Kriterleri

| Kriter | Ölçüt |
|---|---|
| CRUD Fonksiyonelliği | 4 temel işlem (Create, Read, Update, Delete) sorunsuz çalışmalı |
| Veritabanı Bütünlüğü | Veriler SQL Server üzerinde tutarlı bir şekilde saklanmalı |
| Kullanıcı Deneyimi | Formlar doğrulama mesajları göstermeli, işlemler anında yansımalı |
| Kod Kalitesi | MVC desenine uygun, okunabilir ve sürdürülebilir kod yapısı |
