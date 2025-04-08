using Microsoft.AspNetCore.Mvc;
using BookHotel.Repositories.Admin;
using BookHotel.Models;
using BookHotel.DTOs;
using System.Linq;
using System.Threading.Tasks;
using BookHotel.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BookHotel.Controllers
{
    [Route("api/discount")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly DiscountRepository _discountRepository;
        private readonly AppDbContext _context;
        public DiscountController(AppDbContext context, DiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Discount>>> GetDiscount()
        {
            return await _context.Discounts.ToListAsync();
        }

        [HttpPost]
        public ActionResult CreateDiscount([FromBody] DiscountDto request) 
        { 
               var newDiscount=new Discount()
               {
                  Code = request.Code,
                  Discount_percentage = request.Discount_percentage,
                  Price_applies = request.Price_applies,
                  Max_discount=request.Max_discount,
                  Start_date = request.Start_date,
                  End_date = request.End_date,
               };
            _context.Discounts.Add(newDiscount);
            _context.SaveChanges();
            return  Ok(newDiscount);
        }
        
        [HttpPut]
        public Task<ActionResult>
        
    }

   
}

