using Microsoft.EntityFrameworkCore;
using MonoVik.WebApi.Database;
using System;
using System.Threading.Tasks;

namespace MonoVik.WebApi.Users.Infrastructure
{
    public sealed class UserRepository : IUserRepository
    {
        public async Task<bool> ExistsAsync(string email, ApplicationContext context)
        {
            return await context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetAsync(string email, ApplicationContext context)
        {
            return await context.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task InsertAsync(User user, ApplicationContext context)
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        public async Task<User?> GetAsync(Guid userId, ApplicationContext context)
        {
            return await context.Users.SingleOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<bool> ExistsAsync(Guid userId, ApplicationContext context)
        {
            return await context.Users.AnyAsync(u => u.UserId == userId);
        }

        public async Task DeleteAsync(User user, ApplicationContext context)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
        public async Task UpdateAsync(User user, ApplicationContext context)
        {
            await context.Users
                .Where(u => u.UserId == user.UserId)
                .ExecuteUpdateAsync(p =>
                    p.SetProperty(u => u.StatusMessage, u => user.StatusMessage)
                    .SetProperty(u => u.AvatarUrl, u => user.AvatarUrl)
                    .SetProperty(u => u.UpdatedAt, u => DateTime.UtcNow)
                    .SetProperty(u => u.Username, u => user.Username));
            await context.SaveChangesAsync();
        }
    }
}
