using Microsoft.AspNetCore.Authorization;
using MovieTicketBooking.Constants;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _service;

        public MoviesController(IMovieService service)
        {
            _service = service;
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateMovieRequest request)
        {
            var movie = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = movie.MovieId },
                movie);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            int pageNumber = 1,
            int pageSize = 10,
            string? title = null)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                return BadRequest("Invalid pagination values.");

            var movies = await _service.GetAllAsync(
                pageNumber, pageSize, title);

            return Ok(movies);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var movie = await _service.GetByIdAsync(id);

            if (movie == null)
                return NotFound("Movie not found.");

            return Ok(movie);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id, UpdateMovieRequest request)
        {
            var movie = await _service.UpdateAsync(id, request);

            if (movie == null)
                return NotFound("Movie not found.");

            return Ok(movie);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
                return NotFound("Movie not found.");

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("Title is required.");

            var movies = await _service.SearchAsync(title);

            return Ok(movies);
        }
    }
}
