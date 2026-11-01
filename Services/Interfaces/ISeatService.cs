
using MovieTicketBooking.DTOs.Seats;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface ISeatService
    {
        Task<List<SeatResponse>> GetByScreenIdAsync(int screenId);
        Task<SeatResponse?> GetByIdAsync(int id);
        Task<bool> ScreenExistsAsync(int screenId);
        Task<bool> SeatExistsAsync(
            int screenId, string rowName, int seatNumber,
            int? excludeSeatId = null);
        Task<SeatResponse> CreateAsync(CreateSeatRequest request);
        Task<List<SeatResponse>> CreateBulkAsync(
            List<CreateSeatRequest> requests);
        Task<SeatResponse?> UpdateAsync(int id, UpdateSeatRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
