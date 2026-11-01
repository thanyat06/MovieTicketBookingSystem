
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly IShowService _service;

        public ShowsController(IShowService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateShowRequest request)
        {
            if (request.EndTime <= request.StartTime)
                return BadRequest("End time must be later than start time.");

            if (!await _service.MovieExistsAsync(request.MovieId))
                return NotFound("Movie not found.");

            if (!await _service.ScreenExistsAsync(request.ScreenId))
                return NotFound("Screen not found.");

            if (await _service.HasOverlapAsync(
                request.ScreenId, request.StartTime, request.EndTime))
                return Conflict("Another show already exists during this time on this screen.");

            var show = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = show.ShowId },
                show);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            DateTime? date, int? movieId, int? theatreId)
        {
            var shows = await _service.GetAllAsync(
                date, movieId, theatreId);

            return Ok(shows);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(DateTime date)
        {
            var shows = await _service.GetAllAsync(date, null, null);

            return Ok(shows);
        }

        [HttpGet("movie/{movieId:int}")]
        public async Task<IActionResult> GetByMovieId(int movieId)
        {
            if (!await _service.MovieExistsAsync(movieId))
                return NotFound("Movie not found.");

            var shows = await _service.GetByMovieIdAsync(movieId);

            return Ok(shows);
        }

        [HttpGet("theatre/{theatreId:int}")]
        public async Task<IActionResult> GetByTheatreId(int theatreId)
        {
            var shows = await _service.GetAllAsync(null, null, theatreId);

            return Ok(shows);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var show = await _service.GetByIdAsync(id);

            if (show == null)
                return NotFound("Show not found.");

            return Ok(show);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id, UpdateShowRequest request)
        {
            if (request.EndTime <= request.StartTime)
                return BadRequest("End time must be later than start time.");

            if (!await _service.MovieExistsAsync(request.MovieId))
                return NotFound("Movie not found.");

            if (!await _service.ScreenExistsAsync(request.ScreenId))
                return NotFound("Screen not found.");

            if (await _service.HasOverlapAsync(
                request.ScreenId, request.StartTime, request.EndTime, id))
                return Conflict("Another show already exists during this time on this screen.");

            try
            {
                var show = await _service.UpdateAsync(id, request);

                if (show == null)
                    return NotFound("Show not found.");

                return Ok(show);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);

                if (!result)
                    return NotFound("Show not found.");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("{showId:int}/seats")]
        public async Task<IActionResult> GetShowSeats(int showId)
        {
            if (await _service.GetByIdAsync(showId) == null)
                return NotFound("Show not found.");

            var seats = await _service.GetShowSeatsAsync(showId);

            return Ok(seats);
        }

        [HttpGet("{showId:int}/seats/available")]
        public async Task<IActionResult> GetAvailableSeats(int showId)
        {
            if (await _service.GetByIdAsync(showId) == null)
                return NotFound("Show not found.");

            var seats = await _service.GetAvailableSeatsAsync(showId);

            return Ok(seats);
        }

        [HttpGet("{showId:int}/seats/booked")]
        public async Task<IActionResult> GetBookedSeats(int showId)
        {
            if (await _service.GetByIdAsync(showId) == null)
                return NotFound("Show not found.");

            var seats = await _service.GetBookedSeatsAsync(showId);

            return Ok(seats);
        }
    }
}
