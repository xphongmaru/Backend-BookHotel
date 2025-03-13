namespace BookHotel.Repositories.Admin
{
    public interface IControl
    {
        IRoleRepository Roles { get; }
        IPermissionRepository Permissions { get; }
        IUserRepository User { get; }
        Task Save();
    }
}
