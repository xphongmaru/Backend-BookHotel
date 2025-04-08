using Microsoft.EntityFrameworkCore;
using BookHotel.Data;
using BookHotel.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using BookHotel.DTOs;

namespace BookHotel.Repositories.Admin
{
    public class DiscountRepository : IDiscountRepository
    {
        public AppDbContext _context;
       
        public DiscountRepository(AppDbContext context)
        {
            _context = context;
            
        }

        
    }
}
