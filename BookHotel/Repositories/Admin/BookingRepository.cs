using Microsoft.EntityFrameworkCore;
using BookHotel.Data;
using BookHotel.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using BookHotel.DTOs;

namespace BookHotel.Repositories.Admin
{
    public class BookingRepository : IBookingRepository
    {
        public AppDbContext _context;
       
        public BookingRepository(AppDbContext context)
        {
            _context = context;
            
        }

        //get all booking
        public async Task<IEnumerable<Booking>> GetBooking()
        {
            var bookingList=await _context.Bookings.ToListAsync();
            return bookingList;
        }

        public decimal getTotal(int id)
        {
            decimal total = 0;

            var booking=_context.Bookings.FirstOrDefault(b=>b.Booking_id==id);

            foreach (var item in booking.Booking_Rooms)
            {
                total += _context.Rooms.FirstOrDefault(r => r.Room_id == item.Room_id).Price;
            }

            return total;
        }
           
    }
}
