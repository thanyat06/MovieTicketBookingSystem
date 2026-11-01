using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingsController(IBookingService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingRequest request)
        {
            try
            {
                var booking = await _service.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = booking.BookingId },
                    booking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            int? showId, string? status)
        {
            var bookings = await _service.GetAllAsync(showId, status);

            return Ok(bookings);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _service.GetByIdAsync(id);

            if (booking == null)
                return NotFound("Booking not found.");

            return Ok(booking);
        }

        [HttpGet("show/{showId:int}")]
        public async Task<IActionResult> GetByShowId(int showId)
        {
            var bookings = await _service.GetByShowIdAsync(showId);

            return Ok(bookings);
        }

        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var result = await _service.CancelAsync(id);

                if (!result)
                    return NotFound("Booking not found.");

                return Ok(await _service.GetByIdAsync(id));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}