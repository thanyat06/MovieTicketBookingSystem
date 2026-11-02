# Movie Ticket Booking System

A backend REST API application developed using **C#, ASP.NET Core Web API, Entity Framework Core, and SQL Server**.

The system manages movies, theatres, screens, physical seats, show scheduling, seat availability, and customer ticket bookings. It also supports booking cancellation and automatic seat availability updates.

## Features

- Movie Management – Create, view, update, delete, and search movies.
- Theatre Management – Manage theatre details.
- Screen Management – Add and manage screens under theatres.
- Seat Management – Create individual seats and bulk-create seats for screens.
- Show Management – Schedule shows and prevent overlapping shows on the same screen.
- Seat Availability – View all, available, and booked seats for a show.
- Booking Management – Create bookings, calculate ticket amounts, and retrieve booking details.
- Booking Cancellation – Cancel confirmed bookings and release the associated seats.

## Technologies Used

| Technology | Purpose |
|---|---|
| C# | Programming language |
| .NET 10 | Development framework |
| ASP.NET Core Web API | REST API development |
| Entity Framework Core 10 | ORM and database operations |
| SQL Server | Database |
| SQL Server Management Studio | Database management |
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
- **Services:** Implement business logic and validations.
- **Repositories:** Handle database operations.
- **ApplicationDbContext:** Manages database access using Entity Framework Core.

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
│   └── BookingsController.cs
│
├── Models/
│   ├── Movie.cs
│   ├── Theatre.cs
│   ├── Screen.cs
│   ├── Seat.cs
│   ├── Show.cs
│   ├── ShowSeat.cs
│   ├── Booking.cs
│   └── BookingSeat.cs
│
├── DTOs/
│   ├── Movies/
│   ├── Theatres/
│   ├── Screens/
│   ├── Seats/
│   ├── Shows/
│   └── Bookings/
│
├── Data/
│   └── ApplicationDbContext.cs
│
├── Repositories/
│   ├── Interfaces/
│   └── Repository Classes
│
├── Services/
│   ├── Interfaces/
│   └── Service Classes
│
├── Migrations/
│
├── Program.cs
├── appsettings.json
└── MovieTicketBooking.csproj
```

## Database

Database Name: `MovieTicketBookingDB`

The application contains eight main tables:

1. Movies
2. Theatres
3. Screens
4. Seats
5. Shows
6. ShowSeats
7. Bookings
8. BookingSeats

Entity Framework Core Code First approach is used to create and manage the database.

## Getting Started

### Prerequisites

- Visual Studio with .NET 10 SDK
- SQL Server Express
- SQL Server Management Studio
- Postman

### 1. Clone the repository

```bash
git clone <your-github-repository-url>
```

### 2. Open the project

Open `MovieTicketBooking.sln` in Visual Studio.

### 3. Configure the database connection

Update the connection string in `appsettings.json` according to your SQL Server instance.

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

### 5. Create and update the database

Open Package Manager Console in Visual Studio and execute:

```powershell
Add-Migration InitialCreate
Update-Database
```

If the repository already contains the initial migration, execute only:

```powershell
Update-Database
```

### 6. Run the application

```bash
dotnet run
```

Use the HTTPS URL displayed in the terminal or Visual Studio output.

## API Modules

| Module | Base Endpoint |
|---|---|
| Movies | `/api/movies` |
| Theatres | `/api/theatres` |
| Screens | `/api/screens` |
| Seats | `/api/seats` |
| Shows | `/api/shows` |
| Bookings | `/api/bookings` |

Show seat availability is accessed through:

```text
GET /api/shows/{showId}/seats
GET /api/shows/{showId}/seats/available
GET /api/shows/{showId}/seats/booked
```

Refer to [docs.md](docs.md) for detailed API endpoints, request examples, database relationships, and business rules.

## Booking Workflow

1. Create a movie.
2. Create a theatre.
3. Add a screen to the theatre.
4. Create physical seats for the screen.
5. Schedule a show for the movie and screen.
6. Automatically generate show-specific seat availability records.
7. View available seats.
8. Create a booking by selecting available seats.
9. Calculate the booking amount on the server.
10. Mark the selected seats as booked.
11. Cancel the booking when required.
12. Release the associated seats while preserving booking history.

## Important Business Rules

- A theatre can contain multiple screens.
- Screen names must be unique within the same theatre.
- Physical seats must be unique within a screen.
- A screen cannot have overlapping shows.
- Every show receives availability records for its screen's physical seats.
- Only available seats can be booked.
- Duplicate seat IDs in a booking request are rejected.
- Ticket prices and total booking amounts are calculated by the backend.
- Only confirmed bookings can be cancelled.
- Cancelled bookings retain their booking history.

## Scope and Limitations

This is a Version 1 backend implementation intended for learning and demonstration.

The following features are not included:

- User authentication and authorization
- Payment gateway integration
- Online refunds
- Temporary seat locking
- Advanced concurrent booking protection
- Frontend application
- Production deployment automation

## Testing

The APIs are tested using Postman.

Testing includes CRUD operations, duplicate validation, show scheduling, seat availability, booking creation, booking cancellation, and database verification.

## Author

Developed as a C# and ASP.NET Core Web API project to practice REST API development, layered architecture, Entity Framework Core, SQL Server, and backend business logic.