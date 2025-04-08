using Microsoft.AspNetCore.Mvc;
using BookHotel.Repositories.Admin;
using BookHotel.Models;
using BookHotel.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace BookHotel.Controllers
{
    [Route("api/booking")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        [HttpGet]
        public async Task<ActionResult> getBooking()
        {
            var bookingList = await _bookingRepository.GetBooking();
            return Ok(bookingList);
        }
            
    }
}

