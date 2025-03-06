namespace BookHotel.Repositories.Admin
{
    public interface IControl
    {
        IRoleRepository Roles { get; }
        Task Save();
    }
}
