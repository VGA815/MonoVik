using MonoVik.WebApi.Database;

namespace MonoVik.WebApi.Users.Infrastructure
{
    public interface IUserRepository
    {
        Task<bool> ExistsAsync(string email, ApplicationContext context);
        Task<bool> ExistsAsync(Guid userId, ApplicationContext context);
        Task InsertAsync(User user, ApplicationContext context);
        Task<User?> GetAsync(string email, ApplicationContext context);
        Task<User?> GetAsync(Guid userId, ApplicationContext context);
        Task DeleteAsync(User user, ApplicationContext context);
        Task UpdateAsync(User user, ApplicationContext context);
    }
}
