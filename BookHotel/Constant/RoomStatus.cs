namespace BookHotel.Constant
{
    public static class RoomStatus
    {
        public const string Available = "Available";
        public const string Hidden = "Hidden";
        public const string Unavailable = "Unavailable";

        public static List<string> GetAll()
        {
            return new List<string> { Available, Hidden, Unavailable };
        }
    }
}
