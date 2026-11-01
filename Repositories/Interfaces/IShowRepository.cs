
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IShowRepository
    {
        Task<List<Show>> GetAllAsync(
            DateTime? date, int? movieId, int? theatreId);

        Task<Show?> GetByIdAsync(int id);

        Task<List<Show>> GetByMovieIdAsync(int movieId);

        Task<List<Show>> GetByTheatreIdAsync(int theatreId);

        Task<bool> HasOverlapAsync(
            int screenId, DateTime startTime, DateTime endTime,
            int? excludeShowId = null);

        Task<bool> HasBookingsAsync(int showId);

        Task AddAsync(Show show);

        Task UpdateAsync(Show show);

        Task DeleteAsync(Show show);

        Task<List<ShowSeat>> GetShowSeatsAsync(int showId);

        Task<List<ShowSeat>> GetAvailableSeatsAsync(int showId);

        Task<List<ShowSeat>> GetBookedSeatsAsync(int showId);
    }
}
