using Microsoft.EntityFrameworkCore;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Users;
using System;
using System.Threading.Tasks;

namespace MonoVik.WebApi.UserPreferences.Infrastructure
{
    public class UserPreferenceRepository : IUserPreferenceRepository
    {
        public async Task CreateAsync(UserPreference userPreference, ApplicationContext context)
        {
            await context.UserPreferences.AddAsync(userPreference);
            await context.SaveChangesAsync();
        }

        public async Task CreateDefaultAsync(Guid userId, ApplicationContext context)
        {
            UserPreference userPreference = new UserPreference()
            {
                NotificationSound = true,
                PrivacyLevel = "basic",
                ReceiveNotifications = true,
                Theme = "dark",
                UserId = userId,
            };
            await context.UserPreferences.AddAsync(userPreference);
            await context.SaveChangesAsync();
        }

        public async Task<UserPreference> GetAsync(Guid userId, ApplicationContext context)
        {
            return await context.UserPreferences.SingleOrDefaultAsync(up => up.UserId == userId) ?? throw new NotImplementedException();
        }

        public async Task UpdateAsync(UserPreference userPreference, ApplicationContext context)
        {
            await context.UserPreferences
                .Where(up => up.UserId == userPreference.UserId)
                .ExecuteUpdateAsync(x =>
                    x.SetProperty(up => up.PrivacyLevel, userPreference.PrivacyLevel)
                    .SetProperty(up => up.ReceiveNotifications, userPreference.ReceiveNotifications)
                    .SetProperty(up => up.NotificationSound, userPreference.NotificationSound)
                    .SetProperty(up => up.Theme, userPreference.Theme));
            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid userId, ApplicationContext context)
        {
            return await context.UserPreferences.AnyAsync(up => up.UserId == userId);
        }
    }
}
