
using MovieTicketBooking.DTOs.Movies;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _repository;

        public MovieService(IMovieRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MovieResponse>> GetAllAsync(
            int pageNumber, int pageSize, string? title)
        {
            var movies = await _repository.GetAllAsync(
                pageNumber, pageSize, title);

            return movies.Select(m => MapToResponse(m)).ToList();
        }

        public async Task<MovieResponse?> GetByIdAsync(int id)
        {
            var movie = await _repository.GetByIdAsync(id);

            return movie == null ? null : MapToResponse(movie);
        }

        public async Task<List<MovieResponse>> SearchAsync(string title)
        {
            var movies = await _repository.SearchAsync(title);

            return movies.Select(m => MapToResponse(m)).ToList();
        }

        public async Task<MovieResponse> CreateAsync(CreateMovieRequest request)
        {
            var movie = new Movie
            {
                Title = request.Title,
                Description = request.Description,
                DurationMinutes = request.DurationMinutes,
                ReleaseDate = request.ReleaseDate,
                Language = request.Language,
                Genre = request.Genre
            };

            await _repository.AddAsync(movie);

            return MapToResponse(movie);
        }

        public async Task<MovieResponse?> UpdateAsync(
            int id, UpdateMovieRequest request)
        {
            var movie = await _repository.GetByIdAsync(id);

            if (movie == null)
                return null;

            movie.Title = request.Title;
            movie.Description = request.Description;
            movie.DurationMinutes = request.DurationMinutes;
            movie.ReleaseDate = request.ReleaseDate;
            movie.Language = request.Language;
            movie.Genre = request.Genre;

            await _repository.UpdateAsync(movie);

            return MapToResponse(movie);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var movie = await _repository.GetByIdAsync(id);

            if (movie == null)
                return false;

            await _repository.DeleteAsync(movie);

            return true;
        }

        private MovieResponse MapToResponse(Movie movie)
        {
            return new MovieResponse
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                Description = movie.Description,
                DurationMinutes = movie.DurationMinutes,
                ReleaseDate = movie.ReleaseDate,
                Language = movie.Language,
                Genre = movie.Genre
            };
        }
    }
}
