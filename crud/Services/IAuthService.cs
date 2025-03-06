namespace crud.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string username, string password);
        Task<string> SignUpAsync(string username, string email, string password);
    }
}
