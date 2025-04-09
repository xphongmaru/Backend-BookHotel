using BookHotel.DTOs;
using BookHotel.Models;

namespace BookHotel.Repositories.Admin
{
    public interface IDiscountRepository
    {
        public bool DiscountExist(string code);

    }
}
