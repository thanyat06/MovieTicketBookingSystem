
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface ITheatreRepository
    {
        Task<List<Theatre>> GetAllAsync();
        Task<Theatre?> GetByIdAsync(int id);
        Task AddAsync(Theatre theatre);
        Task UpdateAsync(Theatre theatre);
        Task DeleteAsync(Theatre theatre);
    }
}
