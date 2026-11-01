using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IBookingService
    {
        Task<List<BookingResponse>> GetAllAsync(
            int? showId, string? status);

        Task<BookingResponse?> GetByIdAsync(int id);

        Task<List<BookingResponse>> GetByShowIdAsync(int showId);

        Task<BookingResponse> CreateAsync(CreateBookingRequest request);

        Task<bool> CancelAsync(int bookingId);
    }
}