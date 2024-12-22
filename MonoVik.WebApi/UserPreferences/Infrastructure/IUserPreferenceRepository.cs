using MonoVik.WebApi.Database;
using MonoVik.WebApi.Users;

namespace MonoVik.WebApi.UserPreferences.Infrastructure
{
    public interface IUserPreferenceRepository
    {
        Task<UserPreference> GetAsync (Guid userId, ApplicationContext context);
        Task UpdateAsync (UserPreference userPreference, ApplicationContext context);
        Task CreateAsync (UserPreference userPreference, ApplicationContext context);
        Task CreateDefaultAsync (Guid userId, ApplicationContext context);
        Task<bool> ExistsAsync (Guid userId, ApplicationContext context);
    }
}
