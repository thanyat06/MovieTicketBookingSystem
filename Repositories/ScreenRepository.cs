
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class ScreenRepository : IScreenRepository
    {
        private readonly ApplicationDbContext _context;

        public ScreenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Screen>> GetAllAsync(int? theatreId)
        {
            var query = _context.Screens.AsNoTracking();

            if (theatreId.HasValue)
            {
                query = query.Where(s => s.TheatreId == theatreId.Value);
            }

            return await query
                .OrderBy(s => s.ScreenId)
                .ToListAsync();
        }

        public async Task<Screen?> GetByIdAsync(int id)
        {
            return await _context.Screens.FindAsync(id);
        }

        public async Task<List<Screen>> GetByTheatreIdAsync(int theatreId)
        {
            return await _context.Screens
                .AsNoTracking()
                .Where(s => s.TheatreId == theatreId)
                .ToListAsync();
        }

        public async Task<bool> ScreenNameExistsAsync(
            int theatreId, string screenName, int? excludeScreenId = null)
        {
            return await _context.Screens.AnyAsync(s =>
                s.TheatreId == theatreId &&
                s.ScreenName == screenName &&
                (!excludeScreenId.HasValue ||
                 s.ScreenId != excludeScreenId.Value));
        }

        public async Task AddAsync(Screen screen)
        {
            _context.Screens.Add(screen);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Screen screen)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Screen screen)
        {
            _context.Screens.Remove(screen);
            await _context.SaveChangesAsync();
        }
    }
}
