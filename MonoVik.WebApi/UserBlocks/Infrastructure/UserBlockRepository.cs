using Microsoft.EntityFrameworkCore;
using MonoVik.WebApi.Database;

namespace MonoVik.WebApi.UserBlocks.Infrastructure
{
    public class UserBlockRepository : IUserBlockRepository
    {
        public async Task AddAsync(Guid blockerId, Guid blockedId, ApplicationContext context)
        {
            UserBlock block = new UserBlock()
            {
                BlockedAt = DateTime.UtcNow,
                BlockedId = blockedId,
                BlockerId = blockerId,
            };
            await context.UserBlocks.AddAsync(block);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid blockerId, Guid blockedId, ApplicationContext context)
        {
            await context.UserBlocks
                .Where(ub => ub.BlockedId == blockedId && ub.BlockerId == blockerId)
                .ExecuteDeleteAsync();
            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid blockerId, Guid blockedId, ApplicationContext context)
        {
            return await context.UserBlocks.AnyAsync(ub => ub.BlockedId == blockedId && ub.BlockerId == blockerId);
        }

        public IQueryable<UserBlock> GetUserBlocks(Guid userId, int page, int pageSize, ApplicationContext context)
        {
            var blocks = context.UserBlocks
                .Where(ub => ub.BlockerId == userId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            return blocks;
        }
    }
}
