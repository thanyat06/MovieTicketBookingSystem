# Movie Ticket Booking System – Technical Documentation

## 1. Project Overview

The Movie Ticket Booking System is a backend REST API application developed using C#, ASP.NET Core Web API, Entity Framework Core, and SQL Server.

The application allows users to manage movies, theatres, screens, physical seats, and movie shows. Customers can view seat availability, book tickets, retrieve booking details, and cancel confirmed bookings.

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

## 3. Technology Stack

| Component | Technology |
|---|---|
| Language | C# |
| Framework | .NET 10 |
| Backend | ASP.NET Core Web API |
| ORM | Entity Framework Core 10 |
| Database | SQL Server |
| Database Tool | SQL Server Management Studio |
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

Data Transfer Objects are used to define API request and response structures instead of exposing database entities directly.

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
│   ├── MovieRepository.cs
│   ├── TheatreRepository.cs
│   ├── ScreenRepository.cs
│   ├── SeatRepository.cs
│   ├── ShowRepository.cs
│   └── BookingRepository.cs
│
├── Services/
│   ├── Interfaces/
│   ├── MovieService.cs
│   ├── TheatreService.cs
│   ├── ScreenService.cs
│   ├── SeatService.cs
│   ├── ShowService.cs
│   └── BookingService.cs
│
├── Migrations/
├── Program.cs
├── appsettings.json
└── MovieTicketBooking.csproj
```

## 6. Database Design

Database Name: `MovieTicketBookingDB`

The application contains eight entities.

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

### Important Entity Differences

**Seat:** Represents a permanent physical seat in a screen.

**ShowSeat:** Represents the availability of that physical seat for a particular show.

**BookingSeat:** Represents a physical seat included in a customer's booking history.

These three entities have different responsibilities and must not be merged.

## 8. API Modules and Endpoints

Replace `{id}`, `{showId}`, and other route parameters with actual database IDs.

### 8.1 Movie Management

Base URL: `/api/movies`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/movies` | Create movie |
| GET | `/api/movies` | Get movies with pagination |
| GET | `/api/movies/{id}` | Get movie by ID |
| PUT | `/api/movies/{id}` | Update movie |
| DELETE | `/api/movies/{id}` | Delete movie |
| GET | `/api/movies/search?title=leo` | Search movies by title |

Pagination parameters:

```text
/api/movies?pageNumber=1&pageSize=5
```

### 8.2 Theatre Management

Base URL: `/api/theatres`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/theatres` | Create theatre |
| GET | `/api/theatres` | Get all theatres |
| GET | `/api/theatres/{id}` | Get theatre by ID |
| PUT | `/api/theatres/{id}` | Update theatre |
| DELETE | `/api/theatres/{id}` | Delete theatre |

### 8.3 Screen Management

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

### 8.4 Seat Management

Base URL: `/api/seats`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/seats` | Create one physical seat |
| POST | `/api/seats/bulk` | Create multiple seats |
| GET | `/api/seats/screen/{screenId}` | Get seats by screen |
| GET | `/api/seats/{id}` | Get seat by ID |
| PUT | `/api/seats/{id}` | Update seat |
| DELETE | `/api/seats/{id}` | Delete seat |

### 8.5 Show Management

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

### 8.6 Show Seat Availability

These endpoints are implemented inside `ShowsController`.

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/shows/{showId}/seats` | Get all seats for a show |
| GET | `/api/shows/{showId}/seats/available` | Get available seats |
| GET | `/api/shows/{showId}/seats/booked` | Get booked seats |

### 8.7 Booking Management

Base URL: `/api/bookings`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/bookings` | Create booking |
| GET | `/api/bookings` | Get all bookings |
| GET | `/api/bookings?showId=1` | Filter bookings by show |
| GET | `/api/bookings?status=Confirmed` | Filter bookings by status |
| GET | `/api/bookings/{id}` | Get booking details |
| GET | `/api/bookings/show/{showId}` | Get bookings by show |
| PUT | `/api/bookings/{id}/cancel` | Cancel booking |

## 9. Business Logic and Validations

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

## 10. Transaction Handling

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

**Note:** These transactions ensure that the related database operations succeed or fail together. They do not provide advanced protection against simultaneous booking requests.

## 11. HTTP Status Codes

| Status Code | Meaning | Example |
|---|---|---|
| 200 | OK | Successful GET or update |
| 201 | Created | Resource successfully created |
| 204 | No Content | Successful deletion |
| 400 | Bad Request | Invalid request data |
| 404 | Not Found | Requested record does not exist |
| 409 | Conflict | Duplicate or unavailable resource |
| 500 | Internal Server Error | Unexpected server error |

## 12. Database Setup

### Connection String

Configure the SQL Server connection in `appsettings.json`.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=MovieTicketBookingDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Change the server name if your SQL Server instance is different.

### Entity Framework Core Migrations

Run these commands in Visual Studio Package Manager Console:

```powershell
Add-Migration InitialCreate
Update-Database
```

If the migration already exists, run:

```powershell
Update-Database
```

## 13. Testing

Postman is used to test all API modules.

The complete workflow is:

1. Create a movie.
2. Create a theatre.
3. Create a screen.
4. Create physical seats.
5. Create a show.
6. View all seats for the show.
7. View available seats.
8. Create a booking for two available seats.
9. View booked seats.
10. Retrieve the booking.
11. Cancel the booking.
12. View available seats again.

Additional validation tests include duplicate records, invalid durations, invalid show times, overlapping shows, unavailable seats, duplicate seat IDs, and repeated cancellation.

Database records are verified using SQL queries in SQL Server Management Studio.

## 14. Project Scope and Limitations

This implementation covers the Version 1 backend requirements.

The following features are outside the current scope:

- User registration and login.
- Authentication and authorization.
- JWT and role-based access.
- Payment gateway integration.
- Refund processing.
- Temporary seat holds.
- Advanced concurrency handling.
- Frontend application.
- Production deployment automation.

## 15. Conclusion

The Movie Ticket Booking System demonstrates the development of a RESTful backend application using ASP.NET Core Web API and SQL Server.

It covers CRUD operations, layered architecture, Entity Framework Core relationships, DTOs, repository and service patterns, business validations, show scheduling, seat availability, transaction-based booking, and cancellation.

The project provides practical experience in designing and implementing a structured backend application.