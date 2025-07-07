# VacationTracker

A comprehensive vacation and leave management system built with ASP.NET Core 8.0 MVC, designed to handle employee leave requests, approvals, and company-wide vacation tracking with multi-tenant architecture.

## 🚀 Features

### Core Functionality
- **Employee Management**: Complete employee lifecycle management with role-based access
- **Leave Request System**: Submit, approve, and track vacation requests
- **Multi-Company Support**: Multi-tenant architecture supporting multiple companies
- **Role-Based Authorization**: Admin, Manager, Approver, Employee, and SystemAdmin roles
- **Department & Location Management**: Organize employees by departments and locations
- **Allowance Tracking**: Manage employee leave allowances
- **Request Types**: Support for different types of leave requests

### Security & Authentication
- **ASP.NET Core Identity**: Secure user authentication and authorization
- **Role-Based Access Control**: Fine-grained permissions based on user roles
- **System Administrator**: Special SystemAdmin role with access to all companies
- **Company Isolation**: Users can only access data from their assigned company
- **Secure Password Policies**: Enforced password requirements and account lockout

### Technical Features
- **Repository Pattern**: Clean separation of data access logic
- **Service Layer**: Business logic abstraction with CompanyService
- **DTO Pattern**: Data Transfer Objects for optimized data transfer
- **Soft Delete**: Non-destructive deletion with IsDeleted flag
- **Comprehensive Logging**: Serilog integration with SQL Server logging
- **Entity Framework Core**: Modern ORM with code-first approach

## 🛠 Technology Stack

- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server (LocalDB for development)
- **ORM**: Entity Framework Core 8.0
- **Authentication**: ASP.NET Core Identity
- **Logging**: Serilog with SQL Server sink
- **UI**: Bootstrap, jQuery, Razor Views
- **Testing**: xUnit (separate test project)
- **Development Tools**: Visual Studio 2022

### Key Packages
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (8.0.0)
- `Microsoft.EntityFrameworkCore.SqlServer` (8.0.0)
- `Serilog.AspNetCore` (6.1.0)
- `Serilog.Sinks.MSSqlServer` (6.2.0)

## 📋 Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB for development)
- Visual Studio 2022 or VS Code
- Git

## 🚀 Installation & Setup

### 1. Clone the Repository
```bash
git clone https://github.com/yourusername/VacationTracker.git
cd VacationTracker
```

### 2. Database Setup
```bash
# Update connection string in appsettings.json if needed
# Run Entity Framework migrations
dotnet ef database update
```

### 3. Build and Run
```bash
dotnet restore
dotnet build
dotnet run
```

### 4. Access the Application
Navigate to `https://localhost:5001` or `http://localhost:5000`

## 🔐 Default Login

The system automatically creates a System Administrator account during first run:

- **Email**: `admin@admin.com`
- **Password**: `Admin01!`
- **Role**: SystemAdmin
- **Company ID**: -1 (Special system admin identifier)

## 📁 Project Structure

```
VacationTracker/
├── Areas/
│   └── Identity/                 # ASP.NET Core Identity
│       ├── Data/
│       ├── Pages/               # Identity UI pages
│       ├── RoleSeed.cs          # Role and user seeding
│       └── UserClaims.cs        # Custom user claims
├── Controllers/                 # MVC Controllers
│   ├── AllowanceController.cs
│   ├── DepartmentController.cs
│   ├── EmployeeController.cs
│   ├── GenderController.cs
│   ├── HomeController.cs
│   ├── LocationController.cs
│   └── RequestTypeController.cs
├── Data/
│   ├── ApplicationDbContext.cs  # EF Core DbContext
│   └── Migrations/              # Database migrations
├── Interfaces/                  # Repository and service interfaces
├── Models/
│   ├── DTO/                     # Data Transfer Objects
│   ├── Constants/               # Application constants
│   ├── Employee.cs
│   ├── Request.cs
│   ├── Company.cs
│   ├── Department.cs
│   ├── Location.cs
│   ├── Allowance.cs
│   ├── Gender.cs
│   └── RequestType.cs
├── Repositories/                # Data access layer
│   ├── CompanyRepository.cs
│   ├── LocationRepository.cs
│   ├── DepartmentRepository.cs
│   ├── AllowanceRepository.cs
│   └── GenderRepository.cs
├── Services/                    # Business logic layer
│   └── CompanyService.cs
├── Views/                       # Razor Views
├── wwwroot/                     # Static files
├── Program.cs                   # Application entry point
├── Startup.cs                   # Application configuration
└── appsettings.json            # Configuration settings

VacationTrackerTests/            # Separate test project
├── Controllers/                 # Controller tests
├── Services/                    # Service tests
├── Repositories/                # Repository tests
└── Helpers/                     # Test utilities
```

## 🔧 Configuration

### Database Connection
Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=VacationTracker;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Logging Configuration
Serilog is configured to log to both console and SQL Server:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "MSSqlServer",
        "Args": {
          "connectionString": "Server=(localdb)\\MSSQLLocalDB;Database=VacationTracker;Trusted_Connection=True;MultipleActiveResultSets=true",
          "tableName": "VacationTracker_Logs",
          "autoCreateSqlTable": true
        }
      }
    ]
  }
}
```

## 🏗 Architecture

### Design Patterns
- **Repository Pattern**: Abstracts data access logic
- **Service Layer**: Encapsulates business logic
- **DTO Pattern**: Optimizes data transfer between layers
- **Dependency Injection**: Loose coupling between components

### Multi-Tenant Architecture
- **Company Isolation**: Each company's data is isolated
- **System Admin**: Special role with access to all companies
- **User Claims**: Company ID stored in user claims for authorization

### Security Model
- **Role-Based Authorization**: Five distinct roles with specific permissions
- **Company-Scoped Access**: Users can only access their company's data
- **System Admin Override**: SystemAdmin role bypasses company restrictions

## 🔐 Security Features

### Authentication
- ASP.NET Core Identity with custom ApplicationUser
- Email confirmation (optional)
- Two-factor authentication support
- Account lockout after failed attempts

### Authorization
- Role-based access control
- Company-scoped data access
- Custom authorization policies
- User claims for company identification

### Data Protection
- HTTPS enforcement
- Secure cookie configuration
- CSRF protection
- Input validation and sanitization

## 📊 Database Schema

### Core Entities
- **ApplicationUser**: Extended Identity user with CompanyId
- **Company**: Multi-tenant company information
- **Employee**: Employee details with role flags
- **Request**: Leave request management
- **Department**: Organizational structure
- **Location**: Physical locations
- **Allowance**: Leave allowance tracking
- **Gender**: Employee gender options
- **RequestType**: Types of leave requests

### Key Relationships
- Employees belong to Companies, Departments, and Locations
- Requests are linked to Employees and RequestTypes
- All entities support soft delete with IsDeleted flag

## 🧪 Testing

The project includes a separate test project (`VacationTrackerTests`) for comprehensive testing:

### Test Project Structure
```
VacationTrackerTests/
├── Controllers/                 # Controller unit tests
├── Services/                    # Service layer tests
├── Repositories/                # Repository tests
└── Helpers/                     # Test utilities and mocks
```

### Test Coverage
- **Unit Tests**: Isolated testing with mocked dependencies
- **Integration Tests**: Database and service integration testing
- **Test Framework**: xUnit with async/await support
- **Mocking**: Moq for dependency injection
- **Coverage**: Comprehensive testing of all layers

### Running Tests
```bash
# Run all tests
dotnet test

# Run tests from specific project
dotnet test ../VacationTrackerTests/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Features
- **Unit Tests**: Isolated component testing with mocked dependencies
- **Async Testing**: Proper async/await pattern testing
- **Edge Cases**: Null checks, invalid models, unauthorized access
- **System Admin**: Special role testing scenarios
- **Company Isolation**: Multi-tenant access control testing
- **Repository Tests**: Data access layer testing
- **Service Tests**: Business logic testing

## 📝 API Documentation

### Controllers Overview
- **HomeController**: Dashboard and user-specific views
- **EmployeeController**: Employee CRUD operations
- **RequestTypeController**: Leave type management
- **LocationController**: Location management with company scoping
- **DepartmentController**: Department management
- **AllowanceController**: Leave allowance management
- **GenderController**: Gender options management

### Key Endpoints
- `GET /`: Dashboard
- `GET /Employee`: Employee listing
- `GET /Location`: Location management
- `GET /RequestType`: Request type management
- `POST /Employee/Create`: Create new employee
- `POST /Location/Create`: Create new location

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines
- Follow C# coding conventions
- Use meaningful commit messages
- Add appropriate logging
- Include error handling
- Test your changes thoroughly
- Ensure all tests pass before submitting PR

### Testing Requirements
- Write unit tests for new features
- Maintain test coverage above 80%
- Include integration tests for critical paths
- Test both positive and negative scenarios

## 🐛 Known Issues

- Some views may need UI improvements
- Documentation could be expanded for complex business logic
- Performance optimization opportunities in data queries

## 📞 Support

For support and questions:
- Create an issue in the GitHub repository
- Contact the development team
- Check the application logs for detailed error information

## 📈 Version History

### Current Version
- ASP.NET Core 8.0
- Entity Framework Core 8.0
- Serilog 6.1.0
- Multi-tenant architecture
- Role-based authorization
- Comprehensive logging
- Separated test project architecture

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

**Note**: This is a development version. For production deployment, ensure proper security configurations, database backups, and monitoring are in place.