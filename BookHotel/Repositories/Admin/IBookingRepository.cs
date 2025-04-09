using BookHotel.DTOs;
using BookHotel.Models;

namespace BookHotel.Repositories.Admin
{
    public interface IBookingRepository
    {
        //lay toan bo danh sach booking
       Task<IEnumerable<Booking>> GetBooking();
        public decimal getTotal(int id);
    }
}
