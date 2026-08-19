# CompanyCatalog API

Firmaların ürün katalogunu yöneten, **.NET 10** ile geliştirilmiş bir REST API. Clean Architecture, CQRS, Domain-Driven Design ve JWT tabanlı kimlik doğrulama gibi modern backend mimari pratiklerini gerçek bir domain üzerinde uygular.

> Bir e-ticaret backend'inin (Trendyol/Hepsiburada tarzı) çekirdek yapısını temsil eder: satıcı firmalar, ürün kategorileri ve bu ikisini birleştiren ürünler.

---

## İçindekiler

- [Mimari](#mimari)
- [Teknoloji Stack'i](#teknoloji-stacki)
- [Domain Modeli](#domain-modeli)
- [Kurulum](#kurulum)
- [API Endpoint'leri](#api-endpointleri)
- [Kimlik Doğrulama](#kimlik-doğrulama)
- [Uygulanan Kavramlar](#uygulanan-kavramlar)
- [Proje Yapısı](#proje-yapısı)

---

## Mimari

Proje **Clean Architecture** prensipleriyle 4 katmana ayrılmıştır. Bağımlılıklar her zaman içe doğru akar — dış katmanlar iç katmanları bilir, tersi olmaz.

```
┌─────────────────────────────────────────────┐
│                     Api                       │  ← Endpoints, Middleware, DI
│  ┌───────────────────────────────────────┐   │
│  │             Infrastructure             │   │  ← EF Core, Repositories, JWT
│  │  ┌─────────────────────────────────┐  │   │
│  │  │          Application             │  │   │  ← CQRS, Commands, Queries
│  │  │  ┌───────────────────────────┐  │  │   │
│  │  │  │         Domain             │  │  │   │  ← Entities, Business Rules
│  │  │  └───────────────────────────┘  │  │   │
│  │  └─────────────────────────────────┘  │   │
│  └───────────────────────────────────────┘   │
└─────────────────────────────────────────────┘
```

| Katman | Sorumluluk |
|--------|-----------|
| **Domain** | Aggregate root'lar, entity'ler, iş kuralları, domain exception'lar. Hiçbir dış bağımlılığı yok. |
| **Application** | CQRS command/query handler'ları, validation, iş akışı orkestrasyonu. |
| **Infrastructure** | EF Core, PostgreSQL, repository implementasyonları, JWT üretimi, BCrypt. |
| **Api** | Minimal API endpoint'leri, middleware, dependency injection, OpenAPI. |

---

## Teknoloji Stack'i

| Teknoloji | Kullanım Amacı |
|-----------|---------------|
| **.NET 10 / C# 14** | Runtime ve dil |
| **Minimal API** | Endpoint tanımları (Controller yerine, daha hafif) |
| **PostgreSQL** | İlişkisel veritabanı |
| **EF Core 10** | ORM, migration, ilişki yönetimi |
| **MediatR** | CQRS pattern, pipeline behavior'lar |
| **FluentValidation** | Deklaratif input validation |
| **BCrypt.Net** | Şifre hash'leme (work factor 12) |
| **JWT Bearer** | Stateless kimlik doğrulama |
| **Serilog** | Yapılandırılmış loglama |
| **OpenAPI + Scalar** | API dokümantasyonu ve test arayüzü |

### Neden Bu Tercihler?

- **Minimal API vs Controller:** Daha az boilerplate, endpoint'ler açık ve gruplanabilir. Modern .NET'in önerdiği yaklaşım.
- **CQRS + MediatR:** Okuma (query) ve yazma (command) yollarını ayırır. Handler'lar tek sorumluluğa sahip, test edilebilir.
- **Result Pattern:** Exception'ları akış kontrolü için kullanmak yerine, hataları açıkça `Result<T>` ile döneriz. Öngörülebilir ve performanslı.
- **PostgreSQL:** Açık kaynak, güçlü, production-grade. Local kurulum kolay.

---

## Domain Modeli

```
   ┌──────────┐         ┌──────────┐
   │  Company │         │ Category │
   └────┬─────┘         └────┬─────┘
        │ 1                  │ 1
        │                    │
        │ N                  │ N
        └────────┬───────────┘
                 │
           ┌─────▼─────┐
           │  Product  │
           └───────────┘
```

- Bir **Company**'nin birden fazla **Product**'ı olabilir.
- Bir **Category**'nin birden fazla **Product**'ı olabilir.
- Her **Product** bir Company'ye ve bir Category'ye aittir.
- **User** entity'si kimlik doğrulama için ayrı bir aggregate'tir.

Tüm entity'ler **soft delete** (IsActive flag) kullanır — veri asla fiziksel olarak silinmez. Company veya Category silinmeye çalışıldığında, ilişkili Product'lar varsa `RESTRICT` constraint devreye girer.

---

## Kurulum

### Ön Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 16+](https://www.postgresql.org/download/) (local) veya [Docker](https://www.docker.com/)

### Seçenek 1: Docker ile (Önerilen)

```bash
# Repoyu klonla
git clone https://github.com/<kullanici-adi>/CompanyCatalog.git
cd CompanyCatalog

# PostgreSQL'i Docker ile başlat
docker compose up -d

# API'yi çalıştır
dotnet run --project src/CompanyCatalog.Api
```

### Seçenek 2: Local PostgreSQL ile

```bash
# 1. PostgreSQL'de veritabanı oluştur
createdb companycatalog

# 2. Connection string'i ayarla (appsettings.Development.json)
#    veya .env dosyası oluştur (.env.example'ı kopyala)

# 3. API'yi çalıştır (migration'lar otomatik uygulanır)
dotnet run --project src/CompanyCatalog.Api
```

Uygulama başladığında **seed data** otomatik oluşturulur: bir Admin kullanıcısı ve örnek Company/Category/Product verileri.

### API'ye Erişim

- **Scalar UI:** `https://localhost:<port>/scalar/v1`
- **OpenAPI JSON:** `https://localhost:<port>/openapi/v1.json`
- **Health Check:** `https://localhost:<port>/health`

---

## API Endpoint'leri

### Auth

| Method | Endpoint | Açıklama | Yetki |
|--------|----------|----------|-------|
| POST | `/api/v1/auth/register` | Yeni kullanıcı kaydı | Herkes |
| POST | `/api/v1/auth/login` | Giriş, JWT token döner | Herkes |
| GET | `/api/v1/auth/me` | Mevcut kullanıcı bilgisi | Authenticated |

### Companies

| Method | Endpoint | Açıklama | Yetki |
|--------|----------|----------|-------|
| GET | `/api/v1/companies` | Liste (pagination, filtre, sıralama) | Authenticated |
| GET | `/api/v1/companies/{id}` | Tek firma | Authenticated |
| POST | `/api/v1/companies` | Yeni firma | Admin |
| PUT | `/api/v1/companies/{id}` | Güncelle | Admin |
| DELETE | `/api/v1/companies/{id}` | Soft delete | Admin |
| PATCH | `/api/v1/companies/{id}/toggle-status` | Aktif/pasif | Admin |

### Categories

| Method | Endpoint | Açıklama | Yetki |
|--------|----------|----------|-------|
| GET | `/api/v1/categories` | Liste (pagination, filtre) | Authenticated |
| GET | `/api/v1/categories/{id}` | Tek kategori | Authenticated |
| POST | `/api/v1/categories` | Yeni kategori | Admin |
| PUT | `/api/v1/categories/{id}` | Güncelle | Admin |
| DELETE | `/api/v1/categories/{id}` | Soft delete | Admin |
| PATCH | `/api/v1/categories/{id}/toggle-status` | Aktif/pasif | Admin |

### Products

| Method | Endpoint | Açıklama | Yetki |
|--------|----------|----------|-------|
| GET | `/api/v1/products` | Liste (zengin filtreleme) | Authenticated |
| GET | `/api/v1/products/{id}` | Tek ürün | Authenticated |
| POST | `/api/v1/products` | Yeni ürün | Admin |
| PUT | `/api/v1/products/{id}` | Detay güncelle | Admin |
| PATCH | `/api/v1/products/{id}/sku` | SKU güncelle | Admin |
| DELETE | `/api/v1/products/{id}` | Soft delete | Admin |
| PATCH | `/api/v1/products/{id}/toggle-status` | Aktif/pasif | Admin |
| PATCH | `/api/v1/products/{id}/add-stock` | Stok ekle | Admin |
| PATCH | `/api/v1/products/{id}/remove-stock` | Stok çıkar | Admin |

**Product listeleme filtreleri:** `search`, `companyId`, `categoryId`, `minPrice`, `maxPrice`, `isActive`, `inStock`, `page`, `pageSize`, `sortBy`, `sortDescending`

---

## Kimlik Doğrulama

Sistem **JWT Bearer** token kullanır ve iki rol destekler:

| Rol | Yetkiler |
|-----|----------|
| **User** | Tüm kaynakları okuyabilir (GET) |
| **Admin** | Tam CRUD — oluşturma, güncelleme, silme |

### Varsayılan Admin Hesabı

Seed data ile otomatik oluşturulur:

```
Email:    admin@companycatalog.com
Password: Admin123!
```

> **Not:** Bu kimlik bilgileri yalnızca geliştirme/demo amaçlıdır. Production ortamında environment variable veya güvenli secret yönetimi kullanılmalıdır.

### Kullanım

1. `POST /api/v1/auth/login` ile giriş yap, `accessToken`'ı al.
2. Scalar UI'da üstteki **Auth** butonuna tıkla, token'ı yapıştır (başına `Bearer ` yazmadan).
3. Artık korumalı endpoint'leri çağırabilirsin.

---

## Uygulanan Kavramlar

Bu proje aşağıdaki mimari ve tasarım pratiklerini uygular:

- **Clean Architecture** — Katmanlı, bağımlılıkları içe akan yapı
- **CQRS** — Command ve Query sorumluluklarının ayrımı
- **Domain-Driven Design** — Aggregate root'lar, rich domain model, factory method'lar, domain exception'lar
- **Result Pattern** — Exception-free hata yönetimi
- **Repository + Unit of Work** — Veri erişim soyutlaması
- **Pipeline Behaviors** — Validation, logging, exception handling için cross-cutting concern'ler
- **Vertical Slice Organization** — Feature bazlı klasör yapısı
- **RFC 9457 Problem Details** — Standart hata yanıt formatı
- **Soft Delete** — Veri kaybını önleyen pasifleştirme
- **Pagination & Filtering** — Sayfalama, filtreleme, sıralama
- **Foreign Key İlişkileri** — Navigation property'ler, projection ile join, `RESTRICT` cascade davranışı

---

## Proje Yapısı

```
CompanyCatalog/
├── src/
│   ├── CompanyCatalog.Domain/          # Entity'ler, iş kuralları
│   │   ├── Common/                     # Base sınıflar (Entity, AggregateRoot)
│   │   ├── Users/
│   │   ├── Companies/
│   │   ├── Categories/
│   │   └── Products/
│   │
│   ├── CompanyCatalog.Application/      # CQRS, use case'ler
│   │   ├── Abstractions/                # Interface'ler (Repository, Result)
│   │   ├── Auth/
│   │   ├── Companies/
│   │   ├── Categories/
│   │   └── Products/
│   │       ├── Commands/
│   │       ├── Queries/
│   │       └── Shared/
│   │
│   ├── CompanyCatalog.Infrastructure/   # EF Core, JWT, BCrypt
│   │   ├── Persistence/
│   │   │   ├── Configurations/          # EF Fluent API
│   │   │   ├── Repositories/
│   │   │   ├── Migrations/
│   │   │   └── Seed/                     # DatabaseSeeder
│   │   └── Authentication/              # JWT, PasswordHasher
│   │
│   └── CompanyCatalog.Api/              # Endpoint'ler, middleware
│       ├── Endpoints/
│       ├── Extensions/
│       └── Program.cs
│
├── docker-compose.yml
├── .env.example
└── README.md
```

---

## Lisans

Bu proje eğitim ve portfolio amaçlı geliştirilmiştir.
