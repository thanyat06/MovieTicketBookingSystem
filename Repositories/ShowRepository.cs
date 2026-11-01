
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Show>> GetAllAsync(
            DateTime? date, int? movieId, int? theatreId)
        {
            var query = _context.Shows
                .AsNoTracking()
                .AsQueryable();

            if (date.HasValue)
            {
                var start = date.Value.Date;
                var end = start.AddDays(1);

                query = query.Where(s =>
                    s.StartTime >= start && s.StartTime < end);
            }

            if (movieId.HasValue)
            {
                query = query.Where(s => s.MovieId == movieId.Value);
            }

            if (theatreId.HasValue)
            {
                query = query.Where(s =>
                    s.Screen != null &&
                    s.Screen.TheatreId == theatreId.Value);
            }

            return await query
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<Show?> GetByIdAsync(int id)
        {
            return await _context.Shows.FindAsync(id);
        }

        public async Task<List<Show>> GetByMovieIdAsync(int movieId)
        {
            return await _context.Shows
                .AsNoTracking()
                .Where(s => s.MovieId == movieId)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<List<Show>> GetByTheatreIdAsync(int theatreId)
        {
            return await _context.Shows
                .AsNoTracking()
                .Where(s => s.Screen != null &&
                            s.Screen.TheatreId == theatreId)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<bool> HasOverlapAsync(
            int screenId, DateTime startTime, DateTime endTime,
            int? excludeShowId = null)
        {
            return await _context.Shows.AnyAsync(s =>
                s.ScreenId == screenId &&
                s.StartTime < endTime &&
                s.EndTime > startTime &&
                (!excludeShowId.HasValue ||
                 s.ShowId != excludeShowId.Value));
        }

        public async Task<bool> HasBookingsAsync(int showId)
        {
            return await _context.Bookings
                .AnyAsync(b => b.ShowId == showId);
        }

        public async Task AddAsync(Show show)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Shows.Add(show);
                await _context.SaveChangesAsync();

                var seats = await _context.Seats
                    .Where(s => s.ScreenId == show.ScreenId)
                    .ToListAsync();

                var showSeats = seats.Select(seat => new ShowSeat
                {
                    ShowId = show.ShowId,
                    SeatId = seat.SeatId,
                    Status = "Available"
                }).ToList();

                await _context.ShowSeats.AddRangeAsync(showSeats);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task UpdateAsync(Show show)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var oldShowSeats = await _context.ShowSeats
                    .Where(ss => ss.ShowId == show.ShowId)
                    .ToListAsync();

                _context.ShowSeats.RemoveRange(oldShowSeats);

                await _context.SaveChangesAsync();

                var seats = await _context.Seats
                    .Where(s => s.ScreenId == show.ScreenId)
                    .ToListAsync();

                var newShowSeats = seats.Select(seat => new ShowSeat
                {
                    ShowId = show.ShowId,
                    SeatId = seat.SeatId,
                    Status = "Available"
                }).ToList();

                await _context.ShowSeats.AddRangeAsync(newShowSeats);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task DeleteAsync(Show show)
        {
            _context.Shows.Remove(show);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ShowSeat>> GetShowSeatsAsync(int showId)
        {
            return await _context.ShowSeats
                .Include(ss => ss.Seat)
                .Where(ss => ss.ShowId == showId)
                .ToListAsync();
        }

        public async Task<List<ShowSeat>> GetAvailableSeatsAsync(int showId)
        {
            return await _context.ShowSeats
                .Include(ss => ss.Seat)
                .Where(ss => ss.ShowId == showId &&
                             ss.Status == "Available")
                .ToListAsync();
        }

        public async Task<List<ShowSeat>> GetBookedSeatsAsync(int showId)
        {
            return await _context.ShowSeats
                .Include(ss => ss.Seat)
                .Where(ss => ss.ShowId == showId &&
                             ss.Status == "Booked")
                .ToListAsync();
        }
    }
}
