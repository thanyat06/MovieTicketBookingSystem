
using MovieTicketBooking.DTOs.Seats;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _repository;
        private readonly IScreenRepository _screenRepository;

        public SeatService(
            ISeatRepository repository,
            IScreenRepository screenRepository)
        {
            _repository = repository;
            _screenRepository = screenRepository;
        }

        public async Task<List<SeatResponse>> GetAllAsync()
        {
            var seats = await _repository.GetAllAsync();

            return seats.Select(s => MapToResponse(s)).ToList();
        }

        public async Task<List<SeatResponse>> GetByScreenIdAsync(int screenId)
        {
            var seats = await _repository.GetByScreenIdAsync(screenId);

            return seats.Select(s => MapToResponse(s)).ToList();
        }

        public async Task<SeatResponse?> GetByIdAsync(int id)
        {
            var seat = await _repository.GetByIdAsync(id);

            return seat == null ? null : MapToResponse(seat);
        }

        public async Task<bool> ScreenExistsAsync(int screenId)
        {
            return await _screenRepository.GetByIdAsync(screenId) != null;
        }

        public async Task<bool> SeatExistsAsync(
            int screenId, string rowName, int seatNumber,
            int? excludeSeatId = null)
        {
            return await _repository.SeatExistsAsync(
                screenId, rowName, seatNumber, excludeSeatId);
        }

        public async Task<SeatResponse> CreateAsync(CreateSeatRequest request)
        {
            var seat = new Seat
            {
                ScreenId = request.ScreenId,
                RowName = request.RowName,
                SeatNumber = request.SeatNumber,
                SeatType = request.SeatType
            };

            await _repository.AddAsync(seat);

            return MapToResponse(seat);
        }

        public async Task<List<SeatResponse>> CreateBulkAsync(
            List<CreateSeatRequest> requests)
        {
            var seats = requests.Select(r => new Seat
            {
                ScreenId = r.ScreenId,
                RowName = r.RowName,
                SeatNumber = r.SeatNumber,
                SeatType = r.SeatType
            }).ToList();

            await _repository.AddRangeAsync(seats);

            return seats.Select(s => MapToResponse(s)).ToList();
        }

        public async Task<SeatResponse?> UpdateAsync(
            int id, UpdateSeatRequest request)
        {
            var seat = await _repository.GetByIdAsync(id);

            if (seat == null)
                return null;

            seat.ScreenId = request.ScreenId;
            seat.RowName = request.RowName;
            seat.SeatNumber = request.SeatNumber;
            seat.SeatType = request.SeatType;

            await _repository.UpdateAsync(seat);

            return MapToResponse(seat);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var seat = await _repository.GetByIdAsync(id);

            if (seat == null)
                return false;

            await _repository.DeleteAsync(seat);

            return true;
        }

        private SeatResponse MapToResponse(Seat seat)
        {
            return new SeatResponse
            {
                SeatId = seat.SeatId,
                ScreenId = seat.ScreenId,
                RowName = seat.RowName,
                SeatNumber = seat.SeatNumber,
                SeatType = seat.SeatType
            };
        }
    }
}
