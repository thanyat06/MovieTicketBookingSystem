using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetAllAsync(
            int? showId, string? status)
        {
            var query = _context.Bookings
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .AsQueryable();

            if (showId.HasValue)
                query = query.Where(b => b.ShowId == showId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(b => b.Status == status);

            return await query
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<List<Booking>> GetByShowIdAsync(int showId)
        {
            return await _context.Bookings
                .Include(b => b.BookingSeats)
                    .ThenInclude(bs => bs.Seat)
                .Where(b => b.ShowId == showId)
                .ToListAsync();
        }

        public async Task<Booking> CreateAsync(
            Booking booking, List<int> seatIds)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var show = await _context.Shows
                    .FirstOrDefaultAsync(s => s.ShowId == booking.ShowId);

                if (show == null)
                    throw new InvalidOperationException("Show not found.");

                var showSeats = await _context.ShowSeats
                    .Where(ss => ss.ShowId == booking.ShowId &&
                                 seatIds.Contains(ss.SeatId))
                    .ToListAsync();

                if (showSeats.Count != seatIds.Count ||
                    showSeats.Any(ss => ss.Status != "Available"))
                {
                    throw new InvalidOperationException(
                        "One or more seats are unavailable.");
                }

                booking.TotalAmount = show.TicketPrice * seatIds.Count;

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                var bookingSeats = seatIds.Select(seatId =>
                    new BookingSeat
                    {
                        BookingId = booking.BookingId,
                        SeatId = seatId,
                        Price = show.TicketPrice
                    }).ToList();

                await _context.BookingSeats.AddRangeAsync(bookingSeats);

                foreach (var seat in showSeats)
                {
                    seat.Status = "Booked";
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return await GetByIdAsync(booking.BookingId)
                    ?? booking;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> CancelAsync(int bookingId)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.BookingSeats)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                    return false;

                if (booking.Status != "Confirmed")
                    throw new InvalidOperationException(
                        "Only confirmed bookings can be cancelled.");

                booking.Status = "Cancelled";

                var seatIds = booking.BookingSeats
                    .Select(bs => bs.SeatId)
                    .ToList();

                var showSeats = await _context.ShowSeats
                    .Where(ss => ss.ShowId == booking.ShowId &&
                                 seatIds.Contains(ss.SeatId))
                    .ToListAsync();

                foreach (var seat in showSeats)
                {
                    seat.Status = "Available";
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}