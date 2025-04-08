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
    }
}
