using crud.Models;
using Microsoft.EntityFrameworkCore;

namespace crud.Repositories
{
    public class AuthRepository:IAuthRepository
    {
        private readonly SpendSmartDbContext _AuthDbContext;
        public AuthRepository(SpendSmartDbContext AuthDbContext)
        {
            _AuthDbContext = AuthDbContext;
        }
        public async Task<Auth> GetByUsernameAsync(string username)
        {
            return await _AuthDbContext.Authentication.FirstOrDefaultAsync(e => e.Username == username);
        }
        public async Task AddUserAsync(Auth auth)
        {
            _AuthDbContext.Authentication.Add(auth);
            await _AuthDbContext.SaveChangesAsync();
        }
        public async Task<Auth?> GetByIdAsync(int userId)
        {
            return await _AuthDbContext.Authentication.FindAsync(userId);
        }

        public async Task UpdateProfilePictureAsync(int userId, byte[] profilePitcture)
        {
            var user = await _AuthDbContext.Authentication.FindAsync(userId);
            if(user == null)
            {
                throw new InvalidOperationException("User not found");
            }
            user.ProfilePicture = profilePitcture;
            await _AuthDbContext.SaveChangesAsync();
        }
    }
}
