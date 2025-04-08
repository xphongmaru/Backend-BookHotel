using Microsoft.EntityFrameworkCore;
using BookHotel.Data;
using BookHotel.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using BookHotel.DTOs;

namespace BookHotel.Repositories.Admin
{
    public class BookingRoomRepository : IBookingRoomRepository
    {
        private readonly AppDbContext _context;
        public BookingRoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking_Room>> GetBookingRoom()
        {
            return await _context.Booking_Rooms.ToListAsync();
        }


        public async Task<List<Booking_Room>> GetBookingRoombyId(int id)
        { 
            return await _context.Booking_Rooms.Where(br => br.Booking_id == id).ToListAsync();
        }
    }
}
