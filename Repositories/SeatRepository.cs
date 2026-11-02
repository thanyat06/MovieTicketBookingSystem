
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Seat>> GetByScreenIdAsync(int screenId)
        {
            return await _context.Seats
                .AsNoTracking()
                .Where(s => s.ScreenId == screenId)
                .OrderBy(s => s.RowName)
                .ThenBy(s => s.SeatNumber)
                .ToListAsync();
        }

        public async Task<Seat?> GetByIdAsync(int id)
        {
            return await _context.Seats.FindAsync(id);
        }

        public async Task<bool> SeatExistsAsync(
            int screenId, string rowName, int seatNumber,
            int? excludeSeatId = null)
        {
            return await _context.Seats.AnyAsync(s =>
                s.ScreenId == screenId &&
                s.RowName == rowName &&
                s.SeatNumber == seatNumber &&
                (!excludeSeatId.HasValue ||
                 s.SeatId != excludeSeatId.Value));
        }

        public async Task<List<Seat>> GetAllAsync()
        {
            return await _context.Seats.ToListAsync();
        }

        public async Task AddAsync(Seat seat)
        {
            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(List<Seat> seats)
        {
            await _context.Seats.AddRangeAsync(seats);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Seat seat)
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Seat seat)
        {
            _context.Seats.Remove(seat);
            await _context.SaveChangesAsync();
        }
    }
}
