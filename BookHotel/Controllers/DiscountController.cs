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
        private readonly IDiscountRepository _discountRepository;
        private readonly AppDbContext _context;
        public DiscountController(AppDbContext context, IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
            _context = context;
        }

        [HttpGet("getall")]
        public async Task<ActionResult<IEnumerable<Discount>>> GetDiscount()
        {
            return await _context.Discounts.ToListAsync();
        }

        [HttpGet]
        public ActionResult GetDiscountbyCode(string Code)
        {
            var discount= _context.Discounts.FirstOrDefault(d => d.Code == Code);

            if (discount == null) { 
                return NotFound(new ApiResponse(false,null,new ErrorResponse("khong tim thAY giam gia",400)));
            }

            return Ok(new ApiResponse(true,discount,null));
        }

        [HttpPost]
        public ActionResult CreateDiscount([FromBody] DiscountDto request)
        {
            try
            {
                if (_discountRepository.DiscountExist(request.Code))
                {
                    throw new Exception("Discount code already exist");
                }

                var newDiscount = new Discount()
                {
                    Code = request.Code,
                    Discount_percentage = request.Discount_percentage,
                    Price_applies = request.Price_applies,
                    Max_discount = request.Max_discount,
                    Start_date = request.Start_date,
                    End_date = request.End_date,
                };
                _context.Discounts.Add(newDiscount);
                _context.SaveChanges();
                var respone = new ApiResponse(true, newDiscount, null);
                return Ok(respone);
            }
            catch (Exception ex)
            {
                var respone = new ApiResponse(false, null, new ErrorResponse(ex.Message, 500));
                return BadRequest(respone);
            }
        }

        [HttpPut]
        public async Task<ActionResult> updateDiscount([FromBody] DiscountWithIdDto request)
        {
            try
            {
                var discount = _context.Discounts.FirstOrDefault(d => d.Discount_id == request.Discount_id);
                if (discount == null)
                {
                    throw new Exception("Discount doesn't exist");
                }
                var dulikatecheck = _context.Discounts.FirstOrDefault(d => d.Discount_id != request.Discount_id && d.Code == request.Code);
                if (dulikatecheck == null)
                {
                    throw new Exception("Discount code already exist");
                }

                discount.Code = request.Code;
                discount.Discount_percentage = request.Discount_percentage;
                discount.Price_applies = request.Price_applies;
                discount.Max_discount = request.Max_discount;
                discount.Start_date = request.Start_date;
                discount.End_date = request.End_date;

                _context.Entry(discount).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                var respone = new ApiResponse(true, discount, null);
                return Ok(respone);
            }
            catch (Exception ex)
            {
                var respone = new ApiResponse(false, null, new ErrorResponse(ex.Message, 500));
                return BadRequest(respone);
            }

        }


    }
}

