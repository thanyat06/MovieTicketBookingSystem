using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;
        private readonly IShowService _showService;

        public BookingService(
            IBookingRepository repository,
            IShowService showService)
        {
            _repository = repository;
            _showService = showService;
        }

        public async Task<List<BookingResponse>> GetAllAsync(
            int? showId, string? status)
        {
            var bookings = await _repository.GetAllAsync(showId, status);

            return bookings.Select(MapToResponse).ToList();
        }

        public async Task<BookingResponse?> GetByIdAsync(int id)
        {
            var booking = await _repository.GetByIdAsync(id);

            return booking == null ? null : MapToResponse(booking);
        }

        public async Task<List<BookingResponse>> GetByShowIdAsync(int showId)
        {
            var bookings = await _repository.GetByShowIdAsync(showId);

            return bookings.Select(MapToResponse).ToList();
        }

        public async Task<BookingResponse> CreateAsync(
            CreateBookingRequest request)
        {
            if (request.SeatIds.Distinct().Count() != request.SeatIds.Count)
                throw new InvalidOperationException(
                    "Duplicate seat IDs are not allowed.");

            var show = await _showService.GetByIdAsync(request.ShowId);

            if (show == null)
                throw new KeyNotFoundException("Show not found.");

            var availableSeats =
                await _showService.GetAvailableSeatsAsync(request.ShowId);

            if (request.SeatIds.Any(id =>
                !availableSeats.Any(s => s.SeatId == id)))
            {
                throw new InvalidOperationException(
                    "One or more seats are unavailable.");
            }

            var booking = new Booking
            {
                BookingReference = "BK" +
                    Guid.NewGuid().ToString("N")[..12].ToUpper(),

                ShowId = request.ShowId,
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                CustomerPhone = request.CustomerPhone,
                BookingDate = DateTime.UtcNow,
                Status = "Confirmed"
            };

            var created = await _repository.CreateAsync(
                booking, request.SeatIds);

            return MapToResponse(created);
        }

        public async Task<bool> CancelAsync(int bookingId)
        {
            return await _repository.CancelAsync(bookingId);
        }

        private BookingResponse MapToResponse(Booking booking)
        {
            return new BookingResponse
            {
                BookingId = booking.BookingId,
                BookingReference = booking.BookingReference,
                ShowId = booking.ShowId,
                CustomerName = booking.CustomerName,
                CustomerEmail = booking.CustomerEmail,
                CustomerPhone = booking.CustomerPhone,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.TotalAmount,
                Status = booking.Status,

                Seats = booking.BookingSeats.Select(bs =>
                    new BookingSeatResponse
                    {
                        SeatId = bs.SeatId,
                        RowName = bs.Seat.RowName,
                        SeatNumber = bs.Seat.SeatNumber,
                        SeatType = bs.Seat.SeatType,
                        Price = bs.Price
                    }).ToList()
            };
        }
    }
}