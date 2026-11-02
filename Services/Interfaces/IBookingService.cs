using MovieTicketBooking.DTOs.Bookings;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IBookingService
    {
        Task<List<BookingResponse>> GetAllAsync(int? showId, string? status);

        Task<BookingResponse?> GetByIdAsync(
            int id, int userId, bool isAdmin);

        Task<List<BookingResponse>> GetByShowIdAsync(int showId);

        Task<BookingResponse> CreateAsync(
            CreateBookingRequest request,
            int userId,
            string customerName,
            string customerEmail);

        Task<bool> CancelAsync(
            int bookingId, int userId, bool isAdmin);
    }
}