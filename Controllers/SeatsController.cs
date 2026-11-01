
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _service;

        public SeatsController(ISeatService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSeatRequest request)
        {
            if (!await _service.ScreenExistsAsync(request.ScreenId))
                return NotFound("Screen not found.");

            if (await _service.SeatExistsAsync(
                request.ScreenId, request.RowName, request.SeatNumber))
                return Conflict("Seat already exists in this screen.");

            var seat = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = seat.SeatId },
                seat);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateBulk(
            List<CreateSeatRequest> requests)
        {
            if (requests == null || requests.Count == 0)
                return BadRequest("At least one seat is required.");

            // Check duplicate seats inside the request.
            var duplicateRequests = requests
                .GroupBy(r =>
                    $"{r.ScreenId}|{r.RowName}|{r.SeatNumber}",
                    StringComparer.OrdinalIgnoreCase)
                .Any(g => g.Count() > 1);

            if (duplicateRequests)
                return Conflict("Duplicate seats found in the request.");

            foreach (var request in requests)
            {
                if (!await _service.ScreenExistsAsync(request.ScreenId))
                    return NotFound($"Screen {request.ScreenId} not found.");

                if (await _service.SeatExistsAsync(
                    request.ScreenId, request.RowName, request.SeatNumber))
                    return Conflict(
                        $"Seat {request.RowName}{request.SeatNumber} already exists in screen {request.ScreenId}.");
            }

            var seats = await _service.CreateBulkAsync(requests);

            return Ok(seats);
        }

        [HttpGet("screen/{screenId:int}")]
        public async Task<IActionResult> GetByScreenId(int screenId)
        {
            if (!await _service.ScreenExistsAsync(screenId))
                return NotFound("Screen not found.");

            var seats = await _service.GetByScreenIdAsync(screenId);

            return Ok(seats);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var seat = await _service.GetByIdAsync(id);

            if (seat == null)
                return NotFound("Seat not found.");

            return Ok(seat);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id, UpdateSeatRequest request)
        {
            if (!await _service.ScreenExistsAsync(request.ScreenId))
                return NotFound("Screen not found.");

            if (await _service.SeatExistsAsync(
                request.ScreenId, request.RowName, request.SeatNumber, id))
                return Conflict("Seat already exists in this screen.");

            var seat = await _service.UpdateAsync(id, request);

            if (seat == null)
                return NotFound("Seat not found.");

            return Ok(seat);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound("Seat not found.");

            return NoContent();
        }
    }
}
