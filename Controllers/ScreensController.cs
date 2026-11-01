
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreensController : ControllerBase
    {
        private readonly IScreenService _service;

        public ScreensController(IScreenService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateScreenRequest request)
        {
            if (!await _service.TheatreExistsAsync(request.TheatreId))
                return NotFound("Theatre not found.");

            if (await _service.ScreenNameExistsAsync(
                request.TheatreId, request.ScreenName))
                return Conflict("Screen name already exists in this theatre.");

            var screen = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = screen.ScreenId },
                screen);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int? theatreId)
        {
            if (theatreId.HasValue &&
                !await _service.TheatreExistsAsync(theatreId.Value))
                return NotFound("Theatre not found.");

            var screens = await _service.GetAllAsync(theatreId);

            return Ok(screens);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var screen = await _service.GetByIdAsync(id);

            if (screen == null)
                return NotFound("Screen not found.");

            return Ok(screen);
        }

        [HttpGet("theatre/{theatreId:int}")]
        public async Task<IActionResult> GetByTheatreId(int theatreId)
        {
            if (!await _service.TheatreExistsAsync(theatreId))
                return NotFound("Theatre not found.");

            var screens = await _service.GetByTheatreIdAsync(theatreId);

            return Ok(screens);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id, UpdateScreenRequest request)
        {
            if (!await _service.TheatreExistsAsync(request.TheatreId))
                return NotFound("Theatre not found.");

            if (await _service.ScreenNameExistsAsync(
                request.TheatreId, request.ScreenName, id))
                return Conflict("Screen name already exists in this theatre.");

            var screen = await _service.UpdateAsync(id, request);

            if (screen == null)
                return NotFound("Screen not found.");

            return Ok(screen);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound("Screen not found.");

            return NoContent();
        }
    }
}
