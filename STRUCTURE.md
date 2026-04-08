# ShoppingApp — Repository Structure & Architecture

## Architecture Overview

This project follows **Clean (Onion) Architecture** with four distinct layers and strict dependency rules. The Domain layer sits at the center with zero outward dependencies, the Application layer contains business logic and service contracts, the Infrastructure layer handles persistence and external integrations, and the API layer manages HTTP presentation concerns.

## High-Level Architecture Diagram

```mermaid
graph TB
    subgraph API ["ShoppingApp.API — Presentation Layer"]
        Controllers["Controllers<br/>AuthController · CartController<br/>CategoriesController · CouponsController<br/>OrdersController · ProductsController<br/>AdminOrdersController · AdminProductsController"]
        Middleware["ExceptionHandlingMiddleware"]
    end

    subgraph Application ["ShoppingApp.Application — Business Logic Layer"]
        direction TB
        Interfaces["Service Interfaces<br/>IAuthService · ICartService<br/>ICategoryService · ICouponService<br/>IOrderService · IPaymentService<br/>IProductService"]
        Services["Service Implementations<br/>CartService · CategoryService<br/>CouponService · OrderService<br/>ProductService"]
        DTOs["DTOs (18 total)<br/>Auth · Cart · Categories<br/>Coupons · Orders · Products"]
        Validators["FluentValidation<br/>AddToCartValidator<br/>CreateProductValidator<br/>RegisterValidator"]
        Common["Common<br/>ServiceResult·T· · PagedResult·T·"]
        Mappings["MappingExtensions<br/>.ToDto() extensions"]
    end

    subgraph Domain ["ShoppingApp.Domain — Core Layer"]
        Entities["Domain Entities (10)<br/>User · Product · Category<br/>CartItem · Order · OrderItem<br/>Coupon · ProductImage<br/>OrderStatus · BaseEntity"]
        RepoInterfaces["Repository Contracts<br/>IProductRepository · ICategoryRepository<br/>ICartRepository · IOrderRepository<br/>ICouponRepository · IUnitOfWork"]
    end

    subgraph Infrastructure ["ShoppingApp.Infrastructure — Data & Services Layer"]
        Repositories["Repository Implementations<br/>ProductRepository · CategoryRepository<br/>CartRepository · OrderRepository<br/>CouponRepository · UnitOfWork"]
        InfraServices["External Services<br/>AuthService — JWT tokens<br/>MockPaymentService"]
        DbContext["AppDbContext<br/>IdentityDbContext·User, IdentityRole·Guid·, Guid·"]
        Configurations["EF Configurations<br/>Category · Order<br/>Product · User"]
    end

    subgraph Tests ["ShoppingApp.Tests — Unit Tests"]
        AppTests["Application Tests<br/>CartServiceTests · CouponServiceTests<br/>OrderServiceTests · ProductServiceTests<br/>ValidatorTests"]
        DomainTests["Domain Tests<br/>OrderTests"]
    end

    Controllers -->|"uses"| Interfaces
    Controllers -->|"returns"| DTOs
    Middleware -.->|"wraps"| Controllers
    Services -->|"implements"| Interfaces
    Services -->|"queries"| RepoInterfaces
    Services -->|"operates on"| Entities
    Services -->|"returns"| Common
    Mappings -->|"maps"| Entities
    Mappings -->|"produces"| DTOs
    Repositories -->|"implements"| RepoInterfaces
    Repositories -->|"persists via"| DbContext
    InfraServices -->|"implements"| Interfaces
    DbContext -->|"maps"| Entities
    DbContext -->|"applies"| Configurations
    AppTests -.->|"tests"| Services
    AppTests -.->|"tests"| Validators
    DomainTests -.->|"tests"| Entities
```

## Layer Dependency Flow

```mermaid
graph LR
    A["API<br/>(Presentation)"] -->|"references"| B["Application<br/>(Business Logic)"]
    A -->|"bootstraps"| D["Infrastructure<br/>(Data Access)"]
    B -->|"references"| C["Domain<br/>(Core)"]
    D -->|"implements"| B
    D -->|"references"| C
    T["Tests"] -.->|"references"| B
    T -.->|"references"| C

    style C fill:#4CAF50,color:#fff,stroke:#388E3C
    style B fill:#2196F3,color:#fff,stroke:#1565C0
    style D fill:#FF9800,color:#fff,stroke:#E65100
    style A fill:#9C27B0,color:#fff,stroke:#6A1B9A
    style T fill:#607D8B,color:#fff,stroke:#37474F
```

> **Domain** has zero outward dependencies. **Application** depends only on Domain. **Infrastructure** implements interfaces from both Application and Domain. **API** wires everything together via DI. **Tests** reference Application and Domain only (no Infrastructure or API).

## Entity Relationship Diagram

```mermaid
erDiagram
    User {
        Guid Id PK
        string FirstName
        string LastName
        string Email
        string Address
        string City
        string PostalCode
    }
    Product {
        Guid Id PK
        string Name
        decimal Price
        decimal DiscountPrice
        string SKU
        int StockQuantity
        bool IsActive
        Guid CategoryId FK
    }
    Category {
        Guid Id PK
        string Name
        string Description
        string Slug
    }
    Order {
        Guid Id PK
        string OrderNumber
        Guid UserId FK
        OrderStatus Status
        decimal SubTotal
        decimal DiscountAmount
        decimal TotalAmount
        string ShippingAddress
        string CouponCode
    }
    OrderItem {
        Guid Id PK
        Guid OrderId FK
        Guid ProductId FK
        string ProductName
        decimal UnitPrice
        int Quantity
    }
    CartItem {
        Guid Id PK
        Guid UserId FK
        Guid ProductId FK
        int Quantity
    }
    Coupon {
        Guid Id PK
        string Code
        decimal DiscountPercent
        decimal MaxDiscountAmount
        decimal MinOrderAmount
        int UsageLimit
        int TimesUsed
        DateTime ExpiresAt
    }
    ProductImage {
        Guid Id PK
        string Url
        bool IsPrimary
        Guid ProductId FK
    }

    User ||--o{ Order : "places"
    User ||--o{ CartItem : "has"
    Order ||--|{ OrderItem : "contains"
    Product ||--o{ CartItem : "added to"
    Product ||--o{ OrderItem : "ordered as"
    Product ||--o{ ProductImage : "has"
    Category ||--o{ Product : "contains"
```

## Directory Structure

```
coursepractice/
├── README.md
├── QA-REPORT.md
├── STRUCTURE.md
├── src/
│   ├── ShoppingApp.API/                    # Presentation layer (8 controllers)
│   │   ├── Program.cs                      # App bootstrap, DI, JWT, Swagger, role seeding
│   │   ├── appsettings.json                # Configuration
│   │   ├── Controllers/                    # REST API endpoints
│   │   │   ├── AuthController.cs           #   POST /auth/register, /auth/login
│   │   │   ├── CartController.cs           #   GET/POST/PUT/DELETE /cart
│   │   │   ├── CategoriesController.cs     #   GET/POST /categories
│   │   │   ├── CouponsController.cs        #   GET/POST /coupons, validate
│   │   │   ├── OrdersController.cs         #   GET/POST /orders
│   │   │   ├── ProductsController.cs       #   GET /products
│   │   │   ├── AdminOrdersController.cs    #   PUT /admin/orders (status updates)
│   │   │   └── AdminProductsController.cs  #   CRUD /admin/products
│   │   └── Middleware/
│   │       └── ExceptionHandlingMiddleware.cs  # Global error handling
│   │
│   ├── ShoppingApp.Application/            # Business logic layer
│   │   ├── Common/                         # ServiceResult<T>, PagedResult<T>
│   │   ├── DTOs/                           # 18 data transfer objects
│   │   │   ├── Auth/                       #   AuthResponseDto, LoginDto, RegisterDto
│   │   │   ├── Cart/                       #   AddToCartDto, CartItemDto, UpdateCartItemDto
│   │   │   ├── Categories/                 #   CategoryDto, CreateCategoryDto
│   │   │   ├── Coupons/                    #   CouponDto, CreateCouponDto
│   │   │   ├── Orders/                     #   CreateOrderDto, OrderDto, OrderItemDto
│   │   │   └── Products/                   #   CreateProductDto, ProductDto, UpdateProductDto
│   │   ├── Interfaces/                     # 7 service contracts
│   │   ├── Mappings/                       # Entity → DTO via .ToDto() extensions
│   │   ├── Services/                       # 5 business logic implementations
│   │   └── Validators/                     # 3 FluentValidation validators
│   │
│   ├── ShoppingApp.Domain/                 # Core domain layer (zero dependencies)
│   │   ├── Entities/                       # 10 domain models with business rules
│   │   └── Interfaces/                     # 5 repository contracts + IUnitOfWork
│   │
│   └── ShoppingApp.Infrastructure/         # Data access & external services
│       ├── DependencyInjection.cs          # EF Core, Identity, repo registration
│       ├── Data/
│       │   ├── AppDbContext.cs             # IdentityDbContext with 7 DbSets
│       │   └── Configurations/             # 4 Fluent API entity configurations
│       ├── Repositories/                   # 5 repository + UnitOfWork implementations
│       └── Services/
│           ├── AuthService.cs              # JWT token generation & Identity auth
│           └── MockPaymentService.cs       # Mock payment processor
│
└── tests/
    └── ShoppingApp.Tests/                  # Unit tests (Moq + xUnit)
        ├── Application/                    # 5 service & validator test classes
        └── Domain/                         # Entity business logic tests
```

## Key Design Patterns

| Pattern | Implementation |
|---------|---------------|
| **Clean Architecture** | Four layers with strict dependency inversion; Domain at the center |
| **Repository** | `IXxxRepository` interfaces in Domain, EF Core implementations in Infrastructure |
| **Unit of Work** | `IUnitOfWork` aggregates all 5 repositories + `SaveChangesAsync()` for atomic transactions |
| **Result Pattern** | `ServiceResult<T>` with `Ok(data)` / `Fail(error)` for consistent success/failure responses |
| **DTO Mapping** | Manual `.ToDto()` extension methods in `MappingExtensions` — no AutoMapper dependency |
| **Dependency Injection** | Constructor injection; services registered in `Program.cs` + `DependencyInjection.cs` |
| **Middleware Pipeline** | `ExceptionHandlingMiddleware` for centralized exception-to-HTTP-response mapping |
| **FluentValidation** | Auto-registered validators for `AddToCart`, `CreateProduct`, `Register` requests |
| **Role-Based Auth** | JWT Bearer tokens with `Admin` and `Customer` roles seeded on startup |

## Service-to-Repository Mapping

| Service | Interface | Dependencies |
|---------|-----------|-------------|
| `ProductService` | `IProductService` | `IUnitOfWork.Products` |
| `CartService` | `ICartService` | `IUnitOfWork.Cart`, `IUnitOfWork.Products` |
| `CategoryService` | `ICategoryService` | `IUnitOfWork.Categories` |
| `CouponService` | `ICouponService` | `IUnitOfWork.Coupons` |
| `OrderService` | `IOrderService` | `IUnitOfWork` (all repos), `IPaymentService` |
| `AuthService` | `IAuthService` | `UserManager<User>`, `IConfiguration` (JWT settings) |
| `MockPaymentService` | `IPaymentService` | None (returns mock success) |

## Domain Entities — Key Business Logic

| Entity | Business Methods |
|--------|-----------------|
| `Product` | `EffectivePrice` — returns `DiscountPrice ?? Price`; `HasSufficientStock(qty)` — stock check; `ReduceStock(qty)` — decrements inventory |
| `Order` | `GenerateOrderNumber()` — creates unique order numbers; `Cancel()` — transitions status to Cancelled |
| `Coupon` | `IsValid()` — checks expiry, usage limits; `CalculateDiscount(orderTotal)` — applies percentage with max cap and minimum order threshold |
| `OrderItem` | `LineTotal` — computed as `UnitPrice × Quantity` |

## Infrastructure Configuration

| Component | Details |
|-----------|---------|
| **Database** | SQL Server via EF Core (`DefaultConnection` connection string) |
| **Identity** | `IdentityDbContext<User, IdentityRole<Guid>, Guid>` — passwords require 8+ chars, digit, uppercase; unique email enforced |
| **JWT Auth** | Issuer/audience validation, `SymmetricSecurityKey` from `Jwt:Key` config |
| **Swagger** | OpenAPI v1 with Bearer token security scheme |
| **Role Seeding** | `Admin` and `Customer` roles created on startup if absent |

## Tech Stack

- **.NET 8** / ASP.NET Core Web API
- **Entity Framework Core** with SQL Server
- **ASP.NET Core Identity** with Guid-based users and roles
- **JWT Bearer Authentication** with role-based authorization
- **FluentValidation** for request validation
- **Swagger / OpenAPI** for API documentation
- **Moq** + **xUnit** for unit testing
