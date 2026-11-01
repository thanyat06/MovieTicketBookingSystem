
using MovieTicketBooking.DTOs.Screens;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IScreenService
    {
        Task<List<ScreenResponse>> GetAllAsync(int? theatreId);
        Task<ScreenResponse?> GetByIdAsync(int id);
        Task<List<ScreenResponse>> GetByTheatreIdAsync(int theatreId);
        Task<bool> TheatreExistsAsync(int theatreId);
        Task<bool> ScreenNameExistsAsync(
            int theatreId, string screenName, int? excludeScreenId = null);
        Task<ScreenResponse> CreateAsync(CreateScreenRequest request);
        Task<ScreenResponse?> UpdateAsync(int id, UpdateScreenRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
