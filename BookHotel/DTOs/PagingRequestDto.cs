namespace BookHotel.DTOs
{
    public class PagingRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
