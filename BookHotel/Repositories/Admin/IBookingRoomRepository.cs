using BookHotel.DTOs;
using BookHotel.Models;

namespace BookHotel.Repositories.Admin
{
    public interface IBookingRoomRepository
    {
        Task<IEnumerable<Booking_Room>> GetBookingRoom();
        Task<List<Booking_Room>> GetBookingRoombyId(int id);
    }
}
