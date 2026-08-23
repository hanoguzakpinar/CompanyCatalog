# CompanyCatalog API

Firmaların ürün katalogunu yöneten, **.NET 10** ile geliştirilmiş bir REST API. Clean Architecture, CQRS, Domain-Driven Design ve JWT tabanlı kimlik doğrulama gibi modern backend mimari pratiklerini gerçek bir domain üzerinde uygular.

> Bir e-ticaret backend'inin (Trendyol/Hepsiburada tarzı) çekirdek yapısını temsil eder: satıcı firmalar, ürün kategorileri ve bu ikisini birleştiren ürünler.

---

## İçindekiler

- [Mimari](#mimari)
- [Teknoloji Stack'i](#teknoloji-stacki)
- [Domain Modeli](#domain-modeli)
- [Veritabanı Şeması](#veritabanı-şeması)
- [Kurulum](#kurulum)
- [Yapılandırma](#yapılandırma)
- [API Endpoint'leri](#api-endpointleri)
- [Kimlik Doğrulama](#kimlik-doğrulama)
- [Hata Yönetimi](#hata-yönetimi)
- [Uygulanan Kavramlar](#uygulanan-kavramlar)
- [Proje Yapısı](#proje-yapısı)
- [Bilinen Eksikler](#bilinen-eksikler)

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

| Katman | Sorumluluk | Dış bağımlılık |
|--------|-----------|----------------|
| **Domain** | Aggregate root'lar, entity'ler, iş kuralları, domain exception'lar. | Yok — saf C# |
| **Application** | CQRS command/query handler'ları, validation, iş akışı orkestrasyonu. | MediatR, FluentValidation, EF Core (yalnızca `IQueryable` için) |
| **Infrastructure** | EF Core, PostgreSQL, repository implementasyonları, JWT üretimi, BCrypt. | Npgsql, BCrypt, JWT |
| **Api** | Minimal API endpoint'leri, middleware, dependency injection, OpenAPI. | ASP.NET Core, Scalar, Serilog |

Tüm projelerde `Nullable`, `ImplicitUsings` ve **`TreatWarningsAsErrors`** açıktır (`Directory.Build.props`) — yani derleyici uyarısı bırakan kod build'i kırar.

---

## Teknoloji Stack'i

| Teknoloji | Sürüm | Kullanım Amacı |
|-----------|-------|---------------|
| **.NET / C#** | net10.0 / `latest` | Runtime ve dil |
| **Minimal API** | 10.0.8 | Endpoint tanımları (Controller yerine, daha hafif) |
| **PostgreSQL** | 16+ | İlişkisel veritabanı |
| **EF Core + Npgsql** | 10.0.8 / 10.0.1 | ORM, migration, ilişki yönetimi |
| **MediatR** | 14.1.0 | CQRS pattern, pipeline behavior'lar |
| **FluentValidation** | 12.1.1 | Deklaratif input validation |
| **BCrypt.Net-Next** | 4.2.0 | Şifre hash'leme |
| **JWT Bearer** | 10.0.8 | Stateless kimlik doğrulama |
| **Serilog** | 10.0.0 | Yapılandırılmış loglama (Console sink) |
| **OpenAPI + Scalar** | 10.0.8 / 2.14.14 | API dokümantasyonu ve test arayüzü |
| **AspNetCore.HealthChecks.NpgSql** | 9.0.0 | Veritabanı sağlık kontrolü |

### Neden Bu Tercihler?

- **Minimal API vs Controller:** Daha az boilerplate, endpoint'ler açık ve gruplanabilir. Modern .NET'in önerdiği yaklaşım.
- **CQRS + MediatR:** Okuma (query) ve yazma (command) yollarını ayırır. Handler'lar tek sorumluluğa sahip, test edilebilir.
- **Result Pattern:** Exception'ları akış kontrolü için kullanmak yerine, hataları açıkça `Result<T>` ile döneriz. Öngörülebilir ve performanslı.
- **PostgreSQL:** Açık kaynak, güçlü, production-grade. Local kurulum kolay.
- **Guid v7 (`Guid.CreateVersion7()`):** Zaman sıralı GUID'ler — index fragmentasyonunu azaltır.

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

Tüm entity'ler `AggregateRoot<Guid>` türer; `Id`, `CreatedAt`, `UpdatedAt` ve domain event listesi base sınıftan gelir. Nesneler yalnızca statik **factory method**'larla (`Create`) üretilir — setter'lar `private`, dolayısıyla geçersiz durumda bir entity yaratılamaz.

### İş Kuralları (Domain Invariant'ları)

| Entity | Kural |
|--------|-------|
| **User** | Email `@` içermeli; full name ≥ 2 karakter |
| **Company** | İsim 2–200 karakter; vergi no tam 10 hane ve yalnızca rakam |
| **Category** | İsim 2–100 karakter; açıklama ≤ 500 karakter |
| **Product** | İsim 2–200, SKU 3–50 karakter (otomatik `UPPERCASE`), fiyat ve stok negatif olamaz |
| **Product.RemoveStock** | Mevcut stoktan fazlası düşülemez — aksi halde `DomainException` |
| **Activate/Deactivate** | Zaten aktif olanı aktifleştirmek (veya tersi) `DomainException` fırlatır |

### Soft Delete

`DELETE` endpoint'leri kaydı **fiziksel olarak silmez**; ilgili aggregate'in `Deactivate()` metodunu çağırarak `IsActive = false` yapar. Veri veritabanında kalır ve `toggle-status` ile geri açılabilir.

Product → Company/Category foreign key'leri ayrıca `DeleteBehavior.Restrict` ile yapılandırılmıştır; bu, veritabanı seviyesinde bir güvenlik ağıdır (API üzerinden hard delete yapılmadığı için normal akışta tetiklenmez).

> **Not:** Global query filter kullanılmaz. Pasif kayıtlar listeleme sonuçlarına dahildir; filtrelemek için `?isActive=true` query parametresi geçilmelidir.

---

## Veritabanı Şeması

Tablo ve kolon isimleri `snake_case`'dir (EF Fluent API ile açıkça eşlenmiştir).

| Tablo | Önemli kolonlar | Index'ler |
|-------|-----------------|-----------|
| `users` | `email`, `password_hash`, `full_name`, `role`, `is_active` | `email` **unique** |
| `companies` | `name`, `tax_number`, `is_active` | `tax_number` **unique**, `name`, `is_active` |
| `categories` | `name`, `description`, `is_active` | `name`, `is_active` |
| `products` | `name`, `sku`, `price` (18,2), `stock_quantity`, `company_id`, `category_id`, `is_active` | `sku` **unique**, `company_id`, `category_id`, `is_active`, `price` |

Migration'lar (`src/CompanyCatalog.Infrastructure/Persistence/Migrations`):

```
AddUsersTable → AddCompaniesTable → AddCategoriesTable → AddProductsTable
```

Uygulama açılışında `DatabaseSeeder.SeedAsync()` içinde `Database.MigrateAsync()` çağrılır — migration'lar **otomatik uygulanır**, elle `dotnet ef database update` gerekmez.

---

## Kurulum

### Ön Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 16+](https://www.postgresql.org/download/) (local) veya [Docker](https://www.docker.com/)

### 1. Repoyu klonla

```bash
git clone https://github.com/<kullanici-adi>/CompanyCatalog.git
```

### 2. PostgreSQL'i hazırla

Docker ile (repoda compose dosyası yok, tek komut yeterli):

```bash
docker run -d --name companycatalog-db -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=companycatalog -p 5432:5432 postgres:16
```

Local PostgreSQL kullanıyorsan veritabanını oluşturman yeterli:

```bash
createdb companycatalog
```

### 3. Connection string'i ayarla

`src/CompanyCatalog.Api/appsettings.json` içindeki `ConnectionStrings:Database` değerini kendi kurulumuna göre düzenle — veya dosyaya dokunmadan environment variable ile geç (aşağıya bakınız).

### 4. Çalıştır

```bash
dotnet run --project src/CompanyCatalog.Api
```

Uygulama başladığında migration'lar uygulanır ve **seed data** otomatik oluşturulur: bir Admin kullanıcısı, 2 Company, 2 Category ve 3 Product. Veri zaten varsa seeding atlanır.

### API'ye Erişim

Varsayılan portlar `launchSettings.json`'da tanımlıdır — HTTP `5186`, HTTPS `7186`.

| Adres | Açıklama |
|-------|----------|
| `http://localhost:5186/scalar/v1` | Scalar UI — yalnızca Development ortamında |
| `http://localhost:5186/openapi/v1.json` | OpenAPI dokümanı — yalnızca Development ortamında |
| `http://localhost:5186/health` | Sağlık kontrolü (tüm check'ler) |
| `http://localhost:5186/health/ready` | Yalnızca `ready` etiketli check'ler (PostgreSQL bağlantısı) |

> `dotnet run` varsayılan olarak `http` profilini kullanır. HTTPS için: `dotnet run --project src/CompanyCatalog.Api --launch-profile https`

---

## Yapılandırma

Ayarlar `appsettings.json` üzerinden okunur. Standart ASP.NET Core konfigürasyon zinciri geçerli olduğu için her değer environment variable ile override edilebilir (iç içe anahtarlar için `__` ayracı):

```bash
export ConnectionStrings__Database="Host=localhost;Port=5432;Database=companycatalog;Username=postgres;Password=postgres"
export Jwt__SecretKey="en-az-32-karakterlik-gizli-anahtar"
```

| Anahtar | Açıklama | Varsayılan |
|---------|----------|-----------|
| `ConnectionStrings:Database` | PostgreSQL bağlantı cümlesi | `Host=localhost;Port=5432;Database=companycatalog` |
| `Jwt:Issuer` | Token issuer | `CompanyCatalog.Api` |
| `Jwt:Audience` | Token audience | `CompanyCatalog.Client` |
| `Jwt:SecretKey` | HMAC-SHA256 imzalama anahtarı (min. 32 karakter) | — |
| `Jwt:ExpirationInMinutes` | Token geçerlilik süresi | `60` |
| `Serilog:*` | Log seviyeleri, sink'ler, enricher'lar | Console sink |

> ⚠️ **Güvenlik notu:** Bu repoda `appsettings.json` örnek bir connection string ve JWT secret ile birlikte versiyonlanmıştır. Production'da bu değerler environment variable, `dotnet user-secrets` veya bir secret manager üzerinden verilmeli; gerçek kimlik bilgileri asla repoya commit edilmemelidir.

---

## API Endpoint'leri

Tüm endpoint'ler `/api/v1` prefix'i altındadır. `auth/register` ve `auth/login` dışındaki her şey kimlik doğrulama ister; yazma işlemleri `AdminOnly` policy'sine tabidir.

### Auth

| Method | Endpoint | Açıklama | Yetki |
|--------|----------|----------|-------|
| POST | `/api/v1/auth/register` | Yeni kullanıcı kaydı (her zaman `User` rolü) | Herkes |
| POST | `/api/v1/auth/login` | Giriş, JWT token döner | Herkes |
| GET | `/api/v1/auth/me` | Token sahibinin claim bilgileri | Authenticated |

### Companies

| Method | Endpoint | Açıklama | Yetki |
|--------|----------|----------|-------|
| GET | `/api/v1/companies` | Liste (pagination, filtre, sıralama) | Authenticated |
| GET | `/api/v1/companies/{id}` | Tek firma | Authenticated |
| POST | `/api/v1/companies` | Yeni firma | Admin |
| PUT | `/api/v1/companies/{id}` | Güncelle | Admin |
| DELETE | `/api/v1/companies/{id}` | Soft delete | Admin |
| PATCH | `/api/v1/companies/{id}/toggle-status` | Aktif/pasif | Admin |

**Query parametreleri:** `search`, `isActive`, `page` (1), `pageSize` (20), `sortBy` (`name`), `sortDescending` (false)

### Categories

| Method | Endpoint | Açıklama | Yetki |
|--------|----------|----------|-------|
| GET | `/api/v1/categories` | Liste (pagination, filtre) | Authenticated |
| GET | `/api/v1/categories/{id}` | Tek kategori | Authenticated |
| POST | `/api/v1/categories` | Yeni kategori | Admin |
| PUT | `/api/v1/categories/{id}` | Güncelle | Admin |
| DELETE | `/api/v1/categories/{id}` | Soft delete | Admin |
| PATCH | `/api/v1/categories/{id}/toggle-status` | Aktif/pasif | Admin |

**Query parametreleri:** `search`, `isActive`, `page` (1), `pageSize` (20), `sortBy` (`name`), `sortDescending` (false)

### Products

| Method | Endpoint | Açıklama | Yetki |
|--------|----------|----------|-------|
| GET | `/api/v1/products` | Liste (zengin filtreleme) | Authenticated |
| GET | `/api/v1/products/{id}` | Tek ürün | Authenticated |
| POST | `/api/v1/products` | Yeni ürün | Admin |
| PUT | `/api/v1/products/{id}` | Detay güncelle (isim, açıklama, fiyat) | Admin |
| PATCH | `/api/v1/products/{id}/sku` | SKU güncelle | Admin |
| DELETE | `/api/v1/products/{id}` | Soft delete | Admin |
| PATCH | `/api/v1/products/{id}/toggle-status` | Aktif/pasif | Admin |
| PATCH | `/api/v1/products/{id}/add-stock` | Stok ekle | Admin |
| PATCH | `/api/v1/products/{id}/remove-stock` | Stok çıkar | Admin |

**Query parametreleri:** `search` (isim/SKU/açıklama), `companyId`, `categoryId`, `minPrice`, `maxPrice`, `isActive`, `inStock`, `page` (1), `pageSize` (20), `sortBy`, `sortDescending`

**Geçerli `sortBy` değerleri:** `name` (varsayılan), `price`, `stock`, `createdAt`

`PUT` isteklerinde route'taki `id` ile body'deki `Id` eşleşmelidir, aksi halde `400 Bad Request` döner.

### Örnek İstekler

```bash
# Giriş yap
curl -X POST http://localhost:5186/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"oguz@hanakpinar.com","password":"Admin123!"}'

# Elektronik kategorisindeki, stokta olan, 1000 TL altı ürünler — fiyata göre azalan
curl "http://localhost:5186/api/v1/products?maxPrice=1000&inStock=true&sortBy=price&sortDescending=true" \
  -H "Authorization: Bearer <token>"

# Stok düşme (Admin)
curl -X PATCH http://localhost:5186/api/v1/products/<id>/remove-stock \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"quantity":5}'
```

### Sayfalanmış Yanıt Formatı

```json
{
  "items": [ ... ],
  "totalCount": 42,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3,
  "hasNextPage": true,
  "hasPreviousPage": false
}
```

---

## Kimlik Doğrulama

Sistem **JWT Bearer** token kullanır ve iki rol destekler:

| Rol | Yetkiler |
|-----|----------|
| **User** | Tüm kaynakları okuyabilir (GET) |
| **Admin** | Tam CRUD — oluşturma, güncelleme, silme, stok yönetimi |

Token HMAC-SHA256 ile imzalanır, varsayılan ömrü **60 dakika**dır ve `ClockSkew = TimeSpan.Zero` ayarıyla doğrulanır (süresi dolan token anında geçersiz olur). Token içindeki claim'ler: `sub`, `email`, `role`, `fullName`, `jti`.

`/auth/register` ile açılan hesaplar **her zaman `User` rolündedir** — API üzerinden Admin hesabı oluşturulamaz.

### Şifre Politikası

Kayıt sırasında şifre en az 8 karakter olmalı ve en az bir büyük harf, bir küçük harf ve bir rakam içermelidir. Şifreler **BCrypt** ile hash'lenir; veritabanında düz metin şifre tutulmaz.

### Varsayılan Admin Hesabı

Seed data ile otomatik oluşturulur (`DatabaseSeeder.SeedAdminUserAsync`):

```
Email:    oguz@hanakpinar.com
Password: Admin123!
```

> **Not:** Bu kimlik bilgileri yalnızca geliştirme/demo amaçlıdır. Production ortamında seed hesabı devre dışı bırakılmalı veya kimlik bilgileri environment variable ile verilmelidir.

### Kullanım

1. `POST /api/v1/auth/login` ile giriş yap, `accessToken`'ı al.
2. Scalar UI'da üstteki **Auth** butonuna tıkla, token'ı yapıştır (başına `Bearer ` yazmadan).
3. Artık korumalı endpoint'leri çağırabilirsin.

---

## Hata Yönetimi

Handler'lar exception fırlatmak yerine `Result` / `Result<T>` döner. API katmanındaki `ToHttpResult()` extension'ı bu sonucu HTTP yanıtına çevirir:

| Sonuç | HTTP | Yanıt |
|-------|------|-------|
| `Result<T>` başarılı | `200 OK` | Değerin kendisi (veya `onSuccess` ile `201 Created`) |
| `Result` başarılı | `204 No Content` | Gövdesiz |
| `ErrorType.Validation` | `400 Bad Request` | Problem Details + `code` |
| `ErrorType.Unauthorized` | `401 Unauthorized` | Problem Details |
| `ErrorType.Forbidden` | `403 Forbidden` | Problem Details |
| `ErrorType.NotFound` | `404 Not Found` | Problem Details + `code` |
| `ErrorType.Conflict` | `409 Conflict` | Problem Details + `code` |
| `ErrorType.Failure` | `500 Internal Server Error` | Problem Details |

Yakalanmayan exception'lar `GlobalExceptionHandler` tarafından loglanır ve istemciye iç detay sızdırmayan genel bir `500` Problem Details yanıtı döner.

### MediatR Pipeline

Her istek üç behavior'dan sırayla geçer:

```
UnhandledExceptionBehavior  →  LoggingBehavior  →  ValidationBehavior  →  Handler
        (logla, yeniden fırlat)      (süre ölç)        (FluentValidation)
```

`ValidationBehavior` doğrulama hatasında handler'ı hiç çağırmaz; doğrudan `ErrorType.Validation` içeren bir `Result` üretir.

---

## Uygulanan Kavramlar

Bu proje aşağıdaki mimari ve tasarım pratiklerini uygular:

- **Clean Architecture** — Katmanlı, bağımlılıkları içe akan yapı
- **CQRS** — Command ve Query sorumluluklarının ayrımı
- **Domain-Driven Design** — Aggregate root'lar, rich domain model, factory method'lar, domain exception'lar
- **Result Pattern** — Exception-free hata yönetimi
- **Repository + Unit of Work** — Veri erişim soyutlaması (`DbContext` hem `IUnitOfWork` hem `IApplicationDbContext`)
- **Pipeline Behaviors** — Validation, logging, exception handling için cross-cutting concern'ler
- **Vertical Slice Organization** — Feature bazlı klasör yapısı (`Products/Commands/AddStock/...`)
- **RFC 9457 Problem Details** — Standart hata yanıt formatı
- **Soft Delete** — Veri kaybını önleyen pasifleştirme
- **Pagination & Filtering** — Sayfalama, filtreleme, sıralama
- **Projection** — Query'lerde `Select` ile doğrudan DTO'ya map, `AsNoTracking` ile takipsiz okuma
- **Foreign Key İlişkileri** — Navigation property'ler, projection ile join, `RESTRICT` cascade davranışı

---

## Proje Yapısı

```
CompanyCatalog/
├── src/
│   ├── CompanyCatalog.Domain/          # Entity'ler, iş kuralları
│   │   ├── Common/                     # Entity, AggregateRoot, DomainException, IDomainEvent
│   │   ├── Users/
│   │   ├── Companies/
│   │   ├── Categories/
│   │   └── Products/
│   │
│   ├── CompanyCatalog.Application/      # CQRS, use case'ler
│   │   ├── Abstractions/
│   │   │   ├── Authentication/          # IJwtService, IPasswordHasher
│   │   │   ├── Behaviors/               # Validation, Logging, UnhandledException
│   │   │   ├── Persistence/             # Repository ve UnitOfWork interface'leri
│   │   │   └── Results/                 # Result, Error, ErrorType
│   │   ├── Auth/
│   │   ├── Companies/
│   │   ├── Categories/
│   │   └── Products/
│   │       ├── Commands/                # CreateProduct, AddStock, RemoveStock, ...
│   │       ├── Queries/                 # GetProducts, GetProductById
│   │       └── Shared/                  # ProductResponse
│   │
│   ├── CompanyCatalog.Infrastructure/   # EF Core, JWT, BCrypt
│   │   ├── Persistence/
│   │   │   ├── Configurations/          # EF Fluent API
│   │   │   ├── Repositories/
│   │   │   ├── Migrations/
│   │   │   ├── Seed/                    # DatabaseSeeder
│   │   │   └── ApplicationDbContext.cs
│   │   └── Authentication/              # JwtService, JwtOptions, PasswordHasher
│   │
│   └── CompanyCatalog.Api/              # Endpoint'ler, middleware
│       ├── Endpoints/                   # Auth, Company, Category, Product
│       ├── Extensions/                  # ResultExtensions
│       ├── Middleware/                  # GlobalExceptionHandler
│       ├── OpenApi/                     # BearerSecuritySchemeTransformer
│       ├── appsettings.json
│       └── Program.cs
│
├── Directory.Build.props                # Ortak derleme ayarları
├── CompanyCatalog.slnx
└── README.md
```

---

## Bilinen Eksikler

Projenin şu an kapsamadığı, bilinçli olarak sonraya bırakılmış noktalar:

- **Test projesi yok** — unit/integration test katmanı henüz eklenmedi.
- **Refresh token yok** — token süresi dolduğunda yeniden login gerekir.
- **Domain event'ler yayınlanmıyor** — `Entity<TId>` altyapıyı barındırıyor (`RaiseDomainEvent`), ancak `SaveChanges` sırasında dispatch eden bir mekanizma yok.
- **Secret'lar repoda** — `appsettings.json` versiyonlanmış durumda; secret yönetimi environment variable'a taşınmalı.
- **Global query filter yok** — pasif (`IsActive = false`) kayıtlar filtrelenmediği sürece listelerde görünür.
- **Rate limiting / CORS / response caching** yapılandırılmamış.

---

## Lisans

Bu proje eğitim ve portfolio amaçlı geliştirilmiştir.
