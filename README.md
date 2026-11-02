# Movie Ticket Booking System

A backend REST API application developed using **C#, ASP.NET Core Web API, Entity Framework Core, and SQL Server**.

The system manages movies, theatres, screens, physical seats, show scheduling, seat availability, and customer ticket bookings. It also supports booking cancellation, automatic seat availability updates, JWT authentication, and role-based authorization.

## Features

- **Movie Management** – Create, view, update, delete, and search movies.
- **Theatre Management** – Manage theatre details.
- **Screen Management** – Add and manage screens under theatres.
- **Seat Management** – Create individual seats and bulk-create seats for screens.
- **Show Management** – Schedule shows and prevent overlapping shows on the same screen.
- **Seat Availability** – View all, available, and booked seats for a show.
- **Booking Management** – Create bookings, calculate ticket amounts, and retrieve booking details.
- **Booking Cancellation** – Cancel confirmed bookings and release associated seats.
- **JWT Authentication** – Authenticate users and generate JWT access tokens.
- **Role-Based Authorization** – Restrict API operations based on Admin, TheatreManager, and Customer roles.
- **Role Seeding** – Initialize Admin and TheatreManager accounts.

## Technologies Used

| Technology | Purpose |
|---|---|
| C# | Programming language |
| .NET 10 | Development framework |
| ASP.NET Core Web API | REST API development |
| Entity Framework Core 10 | ORM and database operations |
| SQL Server | Database |
| SQL Server Management Studio | Database management |
| JWT | Authentication |
| Postman | API testing |
| Visual Studio | Development environment |
| Git and GitHub | Version control |

## Project Architecture

The application follows a layered architecture:

```text
Client (Postman)
      |
      v
Controllers
      |
      v
Services
      |
      v
Repositories
      |
      v
ApplicationDbContext
      |
      v
SQL Server Database
```

- **Controllers:** Handle HTTP requests and responses.
- **Services:** Implement business logic, validations, and DTO mapping.
- **Repositories:** Handle database operations.
- **ApplicationDbContext:** Manages database access using Entity Framework Core.
- **DTOs:** Define API request and response structures.
- **Authentication and Authorization:** Validate JWT tokens and restrict access based on user roles.

## Project Structure

```text
MovieTicketBooking/
│
├── Controllers/
│   ├── MoviesController.cs
│   ├── TheatresController.cs
│   ├── ScreensController.cs
│   ├── SeatsController.cs
│   ├── ShowsController.cs
│   ├── BookingsController.cs
│   └── AuthController.cs
│
├── Models/
│   ├── Movie.cs
│   ├── Theatre.cs
│   ├── Screen.cs
│   ├── Seat.cs
│   ├── Show.cs
│   ├── ShowSeat.cs
│   ├── Booking.cs
│   ├── BookingSeat.cs
│   └── User.cs
│
├── DTOs/
│   ├── Movies/
│   ├── Theatres/
│   ├── Screens/
│   ├── Seats/
│   ├── Shows/
│   ├── Bookings/
│   └── Auth/
│
├── Data/
│   ├── ApplicationDbContext.cs
│   └── Seeding/
│       └── RoleSeeder.cs
│
├── Repositories/
│   ├── Interfaces/
│   └── Repository Classes
│
├── Services/
│   ├── Interfaces/
│   └── Service Classes
│
├── Constants/
│
├── Migrations/
├── Program.cs
├── appsettings.json
└── MovieTicketBooking.csproj
```

The structure above is representative. Refer to the actual solution for the exact files and folder names.

## Database

**Database Name:** `MovieTicketBookingDB`

The application contains nine main tables:

1. Movies
2. Theatres
3. Screens
4. Seats
5. Shows
6. ShowSeats
7. Bookings
8. BookingSeats
9. Users

Entity Framework Core Code First approach is used to create and manage the database.

## Authentication and Authorization

The application uses JWT-based authentication.

After successful login, the API generates a JWT access token. The token is passed in the Authorization header when accessing protected endpoints.

**Supported roles:**

| Role | Responsibility |
|---|---|
| Admin | Access administrative operations |
| TheatreManager | Access permitted theatre management operations |
| Customer | Access customer booking operations |

Role-based authorization restricts access to endpoints according to the user's role.

### Authorization Header

```text
Authorization: Bearer <your-jwt-token>
```

### Authorization Testing

The following scenarios were tested using Postman:

- Admin token accessing Admin-restricted endpoints.
- TheatreManager token accessing permitted endpoints.
- Customer token creating bookings.
- Customer token attempting to access restricted Admin endpoints.
- Unauthorized requests without a valid token.

## Getting Started

### Prerequisites

- Visual Studio with .NET 10 SDK
- SQL Server Express
- SQL Server Management Studio
- Postman
- Git

### 1. Clone the repository

```bash
git clone <your-github-repository-url>
```

### 2. Open the project

Open `MovieTicketBooking.sln` in Visual Studio.

### 3. Configure the database connection

Update the connection string in `appsettings.json` according to your SQL Server instance.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=MovieTicketBookingDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 4. Restore NuGet packages

```bash
dotnet restore
```

### 5. Configure application secrets

Configure the JWT signing key and other required authentication settings using the configuration keys expected by the application.

For seeded accounts, initialize user secrets and configure the following values:

```bash
dotnet user-secrets init

dotnet user-secrets set "SeedUsers:Admin:FullName" "System Admin"
dotnet user-secrets set "SeedUsers:Admin:Email" "<admin-email>"
dotnet user-secrets set "SeedUsers:Admin:Password" "<strong-admin-password>"

dotnet user-secrets set "SeedUsers:TheatreManager:FullName" "Theatre Manager"
dotnet user-secrets set "SeedUsers:TheatreManager:Email" "<manager-email>"
dotnet user-secrets set "SeedUsers:TheatreManager:Password" "<strong-manager-password>"
```

Do not commit passwords, JWT signing keys, or other secrets to GitHub.

### 6. Create and update the database

Open Package Manager Console in Visual Studio.

If the initial migration has not been created:

```powershell
Add-Migration InitialCreate
```

Update the database:

```powershell
Update-Database
```

If migrations already exist in the repository, execute only:

```powershell
Update-Database
```

### 7. Run the application

```bash
dotnet run
```

Use the HTTPS URL displayed in the terminal or Visual Studio output.

## API Modules

| Module | Base Endpoint |
|---|---|
| Authentication | `/api/auth` |
| Movies | `/api/movies` |
| Theatres | `/api/theatres` |
| Screens | `/api/screens` |
| Seats | `/api/seats` |
| Shows | `/api/shows` |
| Bookings | `/api/bookings` |

### Show Seat Availability

```text
GET /api/shows/{showId}/seats
GET /api/shows/{showId}/seats/available
GET /api/shows/{showId}/seats/booked
```

Refer to [docs.md](docs.md) for detailed API endpoints, request examples, database relationships, authentication, and business rules.

## Booking Workflow

1. Admin creates a movie.
2. Admin creates a theatre.
3. Admin or permitted TheatreManager adds a screen.
4. Physical seats are created for the screen.
5. A show is scheduled for the movie and screen.
6. Show-specific seat availability records are generated automatically.
7. Customer views available seats.
8. Customer creates a booking by selecting available seats.
9. The backend calculates the booking amount.
10. Selected seats are marked as booked.
11. Customer can cancel a confirmed booking.
12. Associated seats are released while preserving booking history.

## Important Business Rules

- A theatre can contain multiple screens.
- Screen names must be unique within the same theatre.
- Physical seats must be unique within a screen.
- A screen cannot have overlapping shows.
- Every show receives availability records for its screen's physical seats.
- Only available seats can be booked.
- Duplicate seat IDs in a booking request are rejected.
- Ticket prices and total booking amounts are calculated by the backend.
- Booking creation and cancellation use database transactions.
- Only confirmed bookings can be cancelled.
- Cancelled bookings retain their booking history.
- Protected operations require valid authentication and appropriate authorization.

## Scope and Limitations

This is a Version 1 backend implementation intended for learning and demonstration.

The following features are not included:

- Payment gateway integration.
- Online refunds.
- Temporary seat locking.
- Advanced concurrent booking protection.
- Frontend application.
- Production deployment automation.
- Theatre-specific ownership enforcement for TheatreManager accounts.

## Testing

The APIs are tested using Postman.

Testing includes:

- CRUD operations.
- Duplicate record validation.
- Invalid request validation.
- Show scheduling and overlapping show validation.
- Automatic ShowSeat generation.
- Seat availability.
- Booking creation and cancellation.
- Database transaction handling.
- JWT login and token generation.
- Admin, TheatreManager, and Customer authorization.
- Unauthorized and forbidden access scenarios.

Database records are verified using SQL queries in SQL Server Management Studio.

## Author

Developed as a C# and ASP.NET Core Web API project to practice REST API development, layered architecture, Entity Framework Core, SQL Server, JWT authentication, role-based authorization, and backend business logic.
