
using MovieTicketBooking.DTOs.Shows;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class ShowService : IShowService
    {
        private readonly IShowRepository _repository;
        private readonly IMovieRepository _movieRepository;
        private readonly IScreenRepository _screenRepository;

        public ShowService(
            IShowRepository repository,
            IMovieRepository movieRepository,
            IScreenRepository screenRepository)
        {
            _repository = repository;
            _movieRepository = movieRepository;
            _screenRepository = screenRepository;
        }

        public async Task<List<ShowResponse>> GetAllAsync(
            DateTime? date, int? movieId, int? theatreId)
        {
            var shows = await _repository.GetAllAsync(
                date, movieId, theatreId);

            return shows.Select(s => MapToResponse(s)).ToList();
        }

        public async Task<ShowResponse?> GetByIdAsync(int id)
        {
            var show = await _repository.GetByIdAsync(id);

            return show == null ? null : MapToResponse(show);
        }

        public async Task<List<ShowResponse>> GetByMovieIdAsync(int movieId)
        {
            var shows = await _repository.GetByMovieIdAsync(movieId);

            return shows.Select(s => MapToResponse(s)).ToList();
        }

        public async Task<List<ShowResponse>> GetByTheatreIdAsync(int theatreId)
        {
            var shows = await _repository.GetByTheatreIdAsync(theatreId);

            return shows.Select(s => MapToResponse(s)).ToList();
        }

        public async Task<bool> MovieExistsAsync(int movieId)
        {
            return await _movieRepository.GetByIdAsync(movieId) != null;
        }

        public async Task<bool> ScreenExistsAsync(int screenId)
        {
            return await _screenRepository.GetByIdAsync(screenId) != null;
        }

        public async Task<bool> HasOverlapAsync(
            int screenId, DateTime startTime, DateTime endTime,
            int? excludeShowId = null)
        {
            return await _repository.HasOverlapAsync(
                screenId, startTime, endTime, excludeShowId);
        }

        public async Task<ShowResponse> CreateAsync(CreateShowRequest request)
        {
            var show = new Show
            {
                MovieId = request.MovieId,
                ScreenId = request.ScreenId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TicketPrice = request.TicketPrice
            };

            await _repository.AddAsync(show);

            return MapToResponse(show);
        }

        public async Task<ShowResponse?> UpdateAsync(
            int id, UpdateShowRequest request)
        {
            var show = await _repository.GetByIdAsync(id);

            if (show == null)
                return null;

            if (await _repository.HasBookingsAsync(id))
                throw new InvalidOperationException(
                    "A show with existing bookings cannot be updated.");

            show.MovieId = request.MovieId;
            show.ScreenId = request.ScreenId;
            show.StartTime = request.StartTime;
            show.EndTime = request.EndTime;
            show.TicketPrice = request.TicketPrice;

            await _repository.UpdateAsync(show);

            return MapToResponse(show);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var show = await _repository.GetByIdAsync(id);

            if (show == null)
                return false;

            if (await _repository.HasBookingsAsync(id))
                throw new InvalidOperationException(
                    "A show with existing bookings cannot be deleted.");

            await _repository.DeleteAsync(show);

            return true;
        }

        private ShowResponse MapToResponse(Show show)
        {
            return new ShowResponse
            {
                ShowId = show.ShowId,
                MovieId = show.MovieId,
                ScreenId = show.ScreenId,
                StartTime = show.StartTime,
                EndTime = show.EndTime,
                TicketPrice = show.TicketPrice
            };
        }
    }
}
