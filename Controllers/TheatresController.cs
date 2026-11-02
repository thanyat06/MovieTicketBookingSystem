using Microsoft.AspNetCore.Authorization;
using MovieTicketBooking.Constants;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Theatres;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheatresController : ControllerBase
    {
        private readonly ITheatreService _service;

        public TheatresController(ITheatreService service)
        {
            _service = service;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateTheatreRequest request)
        {
            var theatre = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = theatre.TheatreId },
                theatre);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var theatres = await _service.GetAllAsync();

            return Ok(theatres);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var theatre = await _service.GetByIdAsync(id);

            if (theatre == null)
                return NotFound("Theatre not found.");

            return Ok(theatre);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id, UpdateTheatreRequest request)
        {
            var theatre = await _service.UpdateAsync(id, request);

            if (theatre == null)
                return NotFound("Theatre not found.");

            return Ok(theatre);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound("Theatre not found.");

            return NoContent();
        }
    }
}
