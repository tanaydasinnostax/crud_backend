using crud.Models;

namespace crud.Repositories
{
    public interface IAuthRepository
    {
        Task<Auth> GetByUsernameAsync(string username);
        Task AddUserAsync(Auth auth);
        Task<Auth?> GetByIdAsync(int userId);

        Task UpdateProfilePictureAsync(int userId, byte[] profilePicture);
    }
}
