using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MovieTicketBooking.Constants;
using MovieTicketBooking.DTOs.Bookings;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingsController(IBookingService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            return int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        private bool IsAdmin()
        {
            return User.IsInRole(UserRoles.Admin);
        }

        [HttpPost]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Create(CreateBookingRequest request)
        {
            try
            {
                var booking = await _service.CreateAsync(
                    request,
                    GetUserId(),
                    User.FindFirstValue(ClaimTypes.Name)!,
                    User.FindFirstValue(ClaimTypes.Email)!);

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(
            int? showId, string? status)
        {
            var bookings = await _service.GetAllAsync(showId, status);

            return Ok(bookings);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await _service.GetByIdAsync(
                id, GetUserId(), IsAdmin());

            if (booking == null)
                return NotFound("Booking not found.");

            return Ok(booking);
        }

        [HttpGet("show/{showId:int}")]
        [Authorize(Roles = "Admin")]
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
                var result = await _service.CancelAsync(
                    id, GetUserId(), IsAdmin());

                if (!result)
                    return NotFound("Booking not found.");

                var booking = await _service.GetByIdAsync(
                    id, GetUserId(), IsAdmin());

                return Ok(booking);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}