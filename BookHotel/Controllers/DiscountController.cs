using Microsoft.AspNetCore.Mvc;
using BookHotel.Repositories.Admin;
using BookHotel.Models;
using BookHotel.DTOs;
using System.Linq;
using System.Threading.Tasks;
using BookHotel.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        [Authorize]
        [HttpGet("admin/getall")]
        public async Task<ActionResult<IEnumerable<Discount>>> GetDiscount(int page=1,int pageSize=5)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
            if (guess.Role != 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn không có quyền truy cập!", 400)));

            var totalCount = await _context.Discounts.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var discountList = await _context.Discounts.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var respone = new
            {
                currentPage = page,
                pageSize = pageSize,
                totalPages = totalPages,
                totalItems = totalCount,
                items = discountList
            };

            return Ok(new ApiResponse(true, respone, null));
        }

        [Authorize]
        [HttpGet("admin/get-by-code")]
        public async Task<ActionResult> GetDiscountbyCode(string Code)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
            if (guess.Role != 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn không có quyền truy cập!", 400)));

            var discount = _context.Discounts.FirstOrDefault(d => d.Code == Code);

            if (discount == null) {
                return NotFound(new ApiResponse(false, null, new ErrorResponse("khong tim thAY giam gia", 400)));
            }

            return Ok(new ApiResponse(true, discount, null));
        }

        [Authorize]
        [HttpPost("admin")]
        public async Task<ActionResult> CreateDiscount([FromBody] DiscountDto request)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
            if (guess.Role != 0)
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn không có quyền truy cập!", 400)));

            try
            {
                if (_discountRepository.DiscountExist(request.Code))
                {
                    throw new Exception("Discount code already exist");
                }

                Discount newDiscount = new Discount
                {
                    Code = request.Code,
                    Discount_percentage = request.Discount_percentage,
                    Price_applies = request.Price_applies,
                    Max_discount = request.Max_discount,
                    Start_date = DateTime.Parse(request.Start_date),
                    End_date = DateTime.Parse(request.End_date),
                    Quantity = request.Quantity,
                    Status = true
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

        [Authorize]
        [HttpPut("admin")]
        public async Task<ActionResult> updateDiscount([FromBody] DiscountWithIdDto request)
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
                if (guess == null)
                    return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));
                if (guess.Role != 0)
                    return BadRequest(new ApiResponse(false, null, new ErrorResponse("Bạn không có quyền truy cập!", 400)));

                var discount = _context.Discounts.FirstOrDefault(d => d.Discount_id == request.Discount_id);
                if (discount == null)
                {
                    throw new Exception("Discount doesn't exist");
                }
                var dulikatecheck = _context.Discounts.FirstOrDefault(d => d.Discount_id != request.Discount_id && d.Code == request.Code);

                if (dulikatecheck != null)
                {
                    throw new Exception("Discount code already exist");
                }

                discount.Code = request.Code;
                discount.Discount_percentage = request.Discount_percentage;
                discount.Price_applies = request.Price_applies;
                discount.Max_discount = request.Max_discount;
                discount.Start_date = DateTime.Parse(request.Start_date);
                discount.End_date = DateTime.Parse(request.End_date);
                discount.Quantity = request.Quantity;
                discount.Status = request.Status;

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

        [Authorize]
        [HttpGet("/user/getall")]
        public async Task<ActionResult> getallDiscountUser(int page = 1, int pageSize = 5)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));

            var totalCount = await _context.Discounts.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var discountList = await _context.Discounts.Where(d => d.Status == true).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var respone = new
            {
                currentPage = page,
                pageSize = pageSize,
                totalPages = totalPages,
                totalItems = totalCount,
                items = discountList
            };

            return Ok(new ApiResponse(true, respone, null));

        }

        [HttpDelete("admin")]
        public async Task<ActionResult> DeleteDiscount(int id)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var guess = await _context.Guess.FirstOrDefaultAsync(g => g.Email == userEmail);
            if (guess == null)
                return Unauthorized(new ApiResponse(false, null, new ErrorResponse("Không xác thực được người dùng.", 401)));

            var discount= await _context.Discounts.FirstOrDefaultAsync(d=>d.Discount_id == id);

            if(discount == null)
            {
                return NotFound(new ApiResponse(false, null, new ErrorResponse("Không tìm thấy đánh giá này!", 404)));
            }

            _context.Discounts.Remove(discount);


            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok(new ApiResponse(true, "Xóa thành công!", null));
            }
            else
            {
                return BadRequest(new ApiResponse(false, null, new ErrorResponse("Xóa thất bại!", 400)));
            }
        }
        }
    }
 

  