# Movie Ticket Booking System – Technical Documentation

## 1. Project Overview

The Movie Ticket Booking System is a backend REST API application developed using C#, ASP.NET Core Web API, Entity Framework Core, and SQL Server.

The application manages movies, theatres, screens, physical seats, and movie shows. Customers can view seat availability, book tickets, retrieve booking details, and cancel confirmed bookings.

The application also implements JWT authentication and role-based authorization for Admin, TheatreManager, and Customer users.

The project follows a layered architecture to separate HTTP handling, business logic, and database operations.

## 2. Project Objectives

- Develop RESTful APIs for movie ticket booking.
- Implement CRUD operations for movies, theatres, screens, and seats.
- Manage movie show schedules.
- Prevent overlapping shows on the same screen.
- Track seat availability separately for each show.
- Implement booking creation and cancellation.
- Maintain booking and seat history.
- Perform database operations using Entity Framework Core.
- Implement JWT-based authentication.
- Restrict API access using role-based authorization.

## 3. Technology Stack

| Component | Technology |
|---|---|
| Language | C# |
| Framework | .NET 10 |
| Backend | ASP.NET Core Web API |
| ORM | Entity Framework Core 10 |
| Database | SQL Server |
| Database Tool | SQL Server Management Studio |
| Authentication | JWT |
| API Testing | Postman |
| IDE | Visual Studio |
| Version Control | Git and GitHub |

## 4. Architecture

The application follows:

**Controller → Service → Repository → DbContext → SQL Server**

### Controllers

Controllers receive HTTP requests, call the corresponding service methods, and return HTTP responses.

### Services

Services contain business logic, validations, and mapping between entities and DTOs.

### Repositories

Repositories perform database operations using Entity Framework Core.

### DbContext

`ApplicationDbContext` manages entity configurations, relationships, and communication with SQL Server.

### DTOs

Data Transfer Objects define API request and response structures instead of exposing database entities directly.

### Authentication and Authorization

JWT authentication validates user identity. Role-based authorization controls access to protected API operations.

## 5. Project Folder Structure

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
├── Migrations/
├── Program.cs
├── appsettings.json
└── MovieTicketBooking.csproj
```

This is a representative structure. Refer to the actual Visual Studio solution for the exact file names and folders.

## 6. Database Design

**Database Name:** `MovieTicketBookingDB`

The application contains nine main entities.

### 6.1 Movie

Stores movie information.

| Column | Description |
|---|---|
| MovieId | Primary key |
| Title | Movie title |
| Description | Movie description |
| DurationMinutes | Movie duration |
| ReleaseDate | Movie release date |
| Language | Movie language |
| Genre | Movie genre |

### 6.2 Theatre

Stores theatre information.

| Column | Description |
|---|---|
| TheatreId | Primary key |
| Name | Theatre name |
| Location | Theatre location |

### 6.3 Screen

Stores screens belonging to theatres.

| Column | Description |
|---|---|
| ScreenId | Primary key |
| TheatreId | Foreign key referencing Theatre |
| ScreenName | Screen name |

A theatre can contain multiple screens. Screen names must be unique within the same theatre.

### 6.4 Seat

Stores permanent physical seats belonging to a screen.

| Column | Description |
|---|---|
| SeatId | Primary key |
| ScreenId | Foreign key referencing Screen |
| RowName | Seat row |
| SeatNumber | Seat number |
| SeatType | Regular or Premium |

The combination of ScreenId, RowName, and SeatNumber must be unique.

### 6.5 Show

Stores movie show scheduling information.

| Column | Description |
|---|---|
| ShowId | Primary key |
| MovieId | Foreign key referencing Movie |
| ScreenId | Foreign key referencing Screen |
| StartTime | Show start time |
| EndTime | Show end time |
| TicketPrice | Ticket price |

The end time must be later than the start time. Shows on the same screen must not overlap.

### 6.6 ShowSeat

Tracks the availability of physical seats for individual shows.

| Column | Description |
|---|---|
| ShowSeatId | Primary key |
| ShowId | Foreign key referencing Show |
| SeatId | Foreign key referencing Seat |
| Status | Available or Booked |

The combination of ShowId and SeatId must be unique.

### 6.7 Booking

Stores customer booking information.

| Column | Description |
|---|---|
| BookingId | Primary key |
| BookingReference | Unique booking reference |
| ShowId | Foreign key referencing Show |
| UserId | User reference associated with the booking |
| CustomerName | Customer name |
| CustomerEmail | Customer email |
| CustomerPhone | Customer phone |
| BookingDate | Booking date and time |
| TotalAmount | Total booking amount |
| Status | Confirmed or Cancelled |

### 6.8 BookingSeat

Stores the seats included in a booking.

| Column | Description |
|---|---|
| BookingSeatId | Primary key |
| BookingId | Foreign key referencing Booking |
| SeatId | Foreign key referencing Seat |
| Price | Price of the booked seat |

A physical seat can appear in booking history for different shows. Therefore, SeatId is not globally unique in BookingSeats.

### 6.9 User

Stores application user and authentication information.

The User entity supports login, password verification, and role-based access.

| Information | Description |
|---|---|
| User ID | Primary key |
| Full Name | User's name |
| Email | User's email address |
| Password Hash | Hashed password |
| Role | Admin, TheatreManager, or Customer |
| Created At | Account creation date |

Refer to the actual `User` model for exact property names and constraints.

## 7. Database Relationships

The system contains the following relationships:

1. Theatre → Screens: One-to-Many
2. Screen → Seats: One-to-Many
3. Movie → Shows: One-to-Many
4. Screen → Shows: One-to-Many
5. Show → ShowSeats: One-to-Many
6. Seat → ShowSeats: One-to-Many
7. Show → Bookings: One-to-Many
8. Booking → BookingSeats: One-to-Many
9. Seat → BookingSeats: One-to-Many
10. User → Bookings: One-to-Many, where configured through the booking's user reference.

### Important Entity Differences

**Seat:** Represents a permanent physical seat in a screen.

**ShowSeat:** Represents the availability of that physical seat for a particular show.

**BookingSeat:** Represents a physical seat included in a customer's booking history.

These three entities have different responsibilities and must not be merged.

## 8. API Modules and Endpoints

Replace `{id}`, `{showId}`, and other route parameters with actual database IDs.

### 8.1 Authentication

Base URL: `/api/auth`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a customer account |
| POST | `/api/auth/login` | Authenticate a user and generate a JWT |

Authentication endpoints are used to obtain access tokens for protected API operations.

### 8.2 Movie Management

Base URL: `/api/movies`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/movies` | Create movie |
| GET | `/api/movies` | Get movies with pagination |
| GET | `/api/movies/{id}` | Get movie by ID |
| PUT | `/api/movies/{id}` | Update movie |
| DELETE | `/api/movies/{id}` | Delete movie |
| GET | `/api/movies/search?title=leo` | Search movies by title |

Pagination:

```text
/api/movies?pageNumber=1&pageSize=5
```

### 8.3 Theatre Management

Base URL: `/api/theatres`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/theatres` | Create theatre |
| GET | `/api/theatres` | Get all theatres |
| GET | `/api/theatres/{id}` | Get theatre by ID |
| PUT | `/api/theatres/{id}` | Update theatre |
| DELETE | `/api/theatres/{id}` | Delete theatre |

### 8.4 Screen Management

Base URL: `/api/screens`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/screens` | Create screen |
| GET | `/api/screens` | Get all screens |
| GET | `/api/screens?theatreId=1` | Filter screens by theatre |
| GET | `/api/screens/{id}` | Get screen by ID |
| GET | `/api/screens/theatre/{theatreId}` | Get screens by theatre |
| PUT | `/api/screens/{id}` | Update screen |
| DELETE | `/api/screens/{id}` | Delete screen |

### 8.5 Seat Management

Base URL: `/api/seats`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/seats` | Create one physical seat |
| POST | `/api/seats/bulk` | Create multiple seats |
| GET | `/api/seats` | Get all physical seats |
| GET | `/api/seats/screen/{screenId}` | Get seats by screen |
| GET | `/api/seats/{id}` | Get seat by ID |
| PUT | `/api/seats/{id}` | Update seat |
| DELETE | `/api/seats/{id}` | Delete seat |

### 8.6 Show Management

Base URL: `/api/shows`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/shows` | Create show |
| GET | `/api/shows` | Get shows with optional filters |
| GET | `/api/shows/{id}` | Get show by ID |
| GET | `/api/shows/movie/{movieId}` | Get shows by movie |
| GET | `/api/shows/theatre/{theatreId}` | Get shows by theatre |
| GET | `/api/shows/search?date=2026-11-03` | Search shows by date |
| PUT | `/api/shows/{id}` | Update show |
| DELETE | `/api/shows/{id}` | Delete show |

Available filters:

```text
/api/shows?date=2026-11-03
/api/shows?movieId=1
/api/shows?theatreId=1
/api/shows?date=2026-11-03&movieId=1&theatreId=1
```

### 8.7 Show Seat Availability

These endpoints are implemented inside `ShowsController`.

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/shows/{showId}/seats` | Get all seats for a show |
| GET | `/api/shows/{showId}/seats/available` | Get available seats |
| GET | `/api/shows/{showId}/seats/booked` | Get booked seats |

### 8.8 Booking Management

Base URL: `/api/bookings`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/bookings` | Create booking |
| GET | `/api/bookings` | Get bookings |
| GET | `/api/bookings?showId=1` | Filter bookings by show |
| GET | `/api/bookings?status=Confirmed` | Filter bookings by status |
| GET | `/api/bookings/{id}` | Get booking details |
| GET | `/api/bookings/show/{showId}` | Get bookings by show |
| PUT | `/api/bookings/{id}/cancel` | Cancel booking |

Access to booking endpoints is subject to the configured authorization rules and booking ownership checks.

## 9. Authentication and Authorization

### 9.1 JWT Authentication

The application uses JSON Web Tokens (JWT) to authenticate users.

The login process is:

1. The user sends login credentials.
2. The API verifies the user's email and password.
3. The password is checked against its stored hash.
4. A JWT access token is generated after successful authentication.
5. The token is returned to the client.
6. The client sends the token when accessing protected endpoints.

### 9.2 Authorization Header

Protected API requests use the following header:

```text
Authorization: Bearer <your-jwt-token>
```

### 9.3 User Roles

| Role | Responsibility |
|---|---|
| Admin | Access administrative operations |
| TheatreManager | Access permitted theatre management operations |
| Customer | Perform permitted customer booking operations |

### 9.4 Role-Based Access

Role-based authorization restricts API access according to the authenticated user's role.

- Admin users can access Admin-restricted operations.
- TheatreManager users can access operations permitted to their role.
- Customer users can perform permitted customer operations, including booking.
- Customers cannot access restricted Admin operations.
- Requests without valid authentication cannot access protected endpoints.

The current implementation uses role-based restrictions. Theatre-specific ownership enforcement for TheatreManager accounts is not included.

### 9.5 Role Seeding

Admin and TheatreManager accounts are initialized using the role seeding process.

The seeder checks whether the configured accounts already exist before creating them. Passwords are stored as hashes.

Seed account configuration is supplied through application configuration and user secrets rather than hardcoded passwords.

## 10. Business Logic and Validations

### Movie Validation

- Movie duration must be greater than zero.
- Required movie fields must be provided.
- Invalid request data is rejected.

### Theatre Validation

- Required theatre information must be provided.

### Screen Validation

- The referenced theatre must exist.
- Duplicate screen names are not allowed within the same theatre.

### Seat Validation

- The referenced screen must exist.
- Seat number must be positive.
- Duplicate physical seats are not allowed within the same screen.
- Bulk creation validates duplicate entries.

### Show Validation

- The referenced movie and screen must exist.
- EndTime must be later than StartTime.
- Shows on the same screen cannot overlap.
- Adjacent shows are allowed when one ends exactly as the next begins.
- Show creation automatically generates ShowSeat records for the screen's physical seats.
- Shows with booking history cannot be updated or deleted.

### Booking Validation

- The referenced show must exist.
- At least one seat must be selected.
- Duplicate SeatIds are rejected.
- All requested seats must belong to the selected show.
- All requested seats must be available.
- The backend calculates the total amount using the show's ticket price.
- Booking creation updates the selected ShowSeat records to Booked.
- Booking and seat updates are performed within a database transaction.

### Booking Cancellation

- Only Confirmed bookings can be cancelled.
- Cancelled bookings cannot be cancelled again.
- The booking status changes to Cancelled.
- The associated ShowSeat records become Available.
- Booking and BookingSeat history is preserved.

## 11. Transaction Handling

Database transactions are used for booking creation and cancellation.

### Booking Creation

1. Begin database transaction.
2. Validate the show.
3. Validate the requested ShowSeat records.
4. Calculate the total amount using the show's ticket price.
5. Create the Booking record.
6. Create the BookingSeat records.
7. Update ShowSeat status to Booked.
8. Save changes.
9. Commit the transaction.

If an operation fails, the transaction is rolled back.

### Booking Cancellation

1. Begin database transaction.
2. Retrieve the booking.
3. Check whether the booking is Confirmed.
4. Change its status to Cancelled.
5. Retrieve the corresponding ShowSeat records.
6. Change their status to Available.
7. Save changes.
8. Commit the transaction.

If an operation fails, the transaction is rolled back.

**Note:** These transactions ensure that related database operations succeed or fail together. They do not provide advanced protection against simultaneous booking requests.

## 12. HTTP Status Codes

| Status Code | Meaning | Example |
|---|---|---|
| 200 | OK | Successful GET or update |
| 201 | Created | Resource successfully created |
| 204 | No Content | Successful deletion |
| 400 | Bad Request | Invalid request data |
| 401 | Unauthorized | Missing or invalid authentication |
| 403 | Forbidden | Authenticated user lacks the required role |
| 404 | Not Found | Requested record does not exist |
| 409 | Conflict | Duplicate or unavailable resource |
| 500 | Internal Server Error | Unexpected server error |

## 13. Database Setup

### Connection String

Configure the SQL Server connection in `appsettings.json`.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=MovieTicketBookingDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Change the server name if your SQL Server instance is different.

### Entity Framework Core Migrations

Run these commands in Visual Studio Package Manager Console.

If the initial migration has not been created:

```powershell
Add-Migration InitialCreate
```

Update the database:

```powershell
Update-Database
```

If the migration already exists, run:

```powershell
Update-Database
```

### Application Secrets

Configure JWT signing settings and seeded account credentials using user secrets or environment variables.

Do not store real passwords or JWT signing keys in source control.

## 14. Testing

Postman is used to test all API modules.

### Functional Testing

The complete booking workflow is:

1. Create a movie.
2. Create a theatre.
3. Create a screen.
4. Create physical seats.
5. Create a show.
6. View all seats for the show.
7. View available seats.
8. Create a booking for available seats.
9. View booked seats.
10. Retrieve the booking.
11. Cancel the booking.
12. View available seats again.

### Validation Testing

Additional validation tests include:

- Duplicate movie or theatre data where applicable.
- Duplicate screen names.
- Duplicate physical seats.
- Invalid movie duration.
- Invalid show times.
- Overlapping shows.
- Unavailable seats.
- Duplicate seat IDs.
- Nonexistent booking IDs.
- Repeated booking cancellation.

### Authentication and Authorization Testing

The following scenarios are tested:

| Test | Expected Result |
|---|---|
| Admin login with valid credentials | JWT token generated |
| TheatreManager login with valid credentials | JWT token generated |
| Customer login with valid credentials | JWT token generated |
| Admin accesses Admin-restricted endpoint | Access permitted |
| TheatreManager accesses permitted endpoint | Access permitted |
| Customer creates a permitted booking | Access permitted |
| Customer accesses restricted Admin endpoint | 403 Forbidden |
| Request without a valid token to a protected endpoint | 401 Unauthorized |

Database records are verified using SQL queries in SQL Server Management Studio.

## 15. Project Scope and Limitations

This implementation covers the Version 1 backend requirements along with JWT authentication and role-based authorization.

The following features are outside the current scope:

- Payment gateway integration.
- Refund processing.
- Temporary seat holds.
- Advanced concurrency handling.
- Frontend application.
- Production deployment automation.
- Theatre-specific ownership enforcement for TheatreManager accounts.

## 16. Conclusion

The Movie Ticket Booking System demonstrates the development of a RESTful backend application using ASP.NET Core Web API and SQL Server.

It covers CRUD operations, layered architecture, Entity Framework Core relationships, DTOs, repository and service patterns, business validations, show scheduling, seat availability, transaction-based booking, booking cancellation, JWT authentication, and role-based authorization.

The project provides practical experience in designing and implementing a structured backend application.
