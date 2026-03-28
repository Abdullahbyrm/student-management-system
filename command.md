# SQL Server Test Komutları

Bu dosya, SQL Server'ın çalışıp çalışmadığını ve bağlantıların düzgün olup olmadığını test etmek için terminal (PowerShell veya Komut İstemcisi) üzerinden kullanabileceğiniz komutları içerir.

## 1. Servis Kontrolü (PowerShell)
Arka planda çalışan SQL Server servislerinin durumunu kontrol etmek için:
```powershell
Get-Service -DisplayName *SQL*
```
_Not: Bu komut, SQL Server (MSSQLSERVER) veya SQL Server (SQLEXPRESS) gibi servislerin listelenmesini sağlar._

## 2. LocalDB Kontrolü
LocalDB (Geliştirici ortamı için varsayılan SQL örneği) çalışıyor mu bunu kontrol etmek için:
```powershell
sqllocaldb info
```

## 3. Bağlantı Testi (sqlcmd ile)
SQL Server'a terminal üzerinden bağlanarak sorgu atabilmek için (Bu işlem veritabanınızın versiyonunu listeler):

**LocalDB için:**
```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -Q "SELECT @@VERSION"
```

**SQL Express için:**
```powershell
sqlcmd -S ".\SQLEXPRESS" -E -Q "SELECT @@VERSION"
```

**Varsayılan SQL Server (ör. localhost) için:**
```powershell
sqlcmd -S "localhost" -E -Q "SELECT @@VERSION"
```

## Parametrelerin Anlamı:
- `-S`: Sunucu adı (Server Name)
- `-E`: Windows Kimlik Doğrulamasını kullanarak güvenilir bağlantı sağla
- `-Q`: Belirtilen T-SQL sorgusunu anında çalıştır ve ardından çıkış yap

---

## 4. .NET ve EF Core Test Komutları
Geliştirme ortamınızın ve Entity Framework Core aracının sorunsuz çalıştığını doğrulamak için kullanabileceğiniz `.NET` komutları:

**.NET SDK Sürümünü Kontrol Etmek:**
```powershell
dotnet --version
```
_(Beklenen: 9.0.x gibi bir çıktı vermelidir.)_

**Entity Framework (EF Core) Aracını Yüklemek:**
```powershell
dotnet tool install --global dotnet-ef
```

**EF Core Aracının Başarıyla Kurulduğunu/Çalıştığını Doğrulamak:**
```powershell
dotnet ef --version
```

**Yüklü .NET Proje Şablonlarına Göz Atmak ve Çalıştığını Test Etmek:**
```powershell
dotnet new --list
```
_(Beklenen: Listede 'mvc', 'console', 'webapi' gibi proje kurulum şablonlarının görünmesidir.)_
