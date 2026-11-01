
using MovieTicketBooking.DTOs.Screens;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class ScreenService : IScreenService
    {
        private readonly IScreenRepository _repository;
        private readonly ITheatreRepository _theatreRepository;

        public ScreenService(
            IScreenRepository repository,
            ITheatreRepository theatreRepository)
        {
            _repository = repository;
            _theatreRepository = theatreRepository;
        }

        public async Task<List<ScreenResponse>> GetAllAsync(int? theatreId)
        {
            var screens = await _repository.GetAllAsync(theatreId);

            return screens.Select(s => MapToResponse(s)).ToList();
        }

        public async Task<ScreenResponse?> GetByIdAsync(int id)
        {
            var screen = await _repository.GetByIdAsync(id);

            return screen == null ? null : MapToResponse(screen);
        }

        public async Task<List<ScreenResponse>> GetByTheatreIdAsync(int theatreId)
        {
            var screens = await _repository.GetByTheatreIdAsync(theatreId);

            return screens.Select(s => MapToResponse(s)).ToList();
        }

        public async Task<bool> TheatreExistsAsync(int theatreId)
        {
            return await _theatreRepository.GetByIdAsync(theatreId) != null;
        }

        public async Task<bool> ScreenNameExistsAsync(
            int theatreId, string screenName, int? excludeScreenId = null)
        {
            return await _repository.ScreenNameExistsAsync(
                theatreId, screenName, excludeScreenId);
        }

        public async Task<ScreenResponse> CreateAsync(CreateScreenRequest request)
        {
            var screen = new Screen
            {
                TheatreId = request.TheatreId,
                ScreenName = request.ScreenName
            };

            await _repository.AddAsync(screen);

            return MapToResponse(screen);
        }

        public async Task<ScreenResponse?> UpdateAsync(
            int id, UpdateScreenRequest request)
        {
            var screen = await _repository.GetByIdAsync(id);

            if (screen == null)
                return null;

            screen.TheatreId = request.TheatreId;
            screen.ScreenName = request.ScreenName;

            await _repository.UpdateAsync(screen);

            return MapToResponse(screen);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var screen = await _repository.GetByIdAsync(id);

            if (screen == null)
                return false;

            await _repository.DeleteAsync(screen);

            return true;
        }

        private ScreenResponse MapToResponse(Screen screen)
        {
            return new ScreenResponse
            {
                ScreenId = screen.ScreenId,
                TheatreId = screen.TheatreId,
                ScreenName = screen.ScreenName
            };
        }
    }
}
