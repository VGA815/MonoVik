using MonoVik.WebApi.Database;

namespace MonoVik.WebApi.UserBlocks.Infrastructure
{
    public interface IUserBlockRepository
    {
        Task<bool> ExistsAsync(Guid blockerId, Guid blockedId, ApplicationContext context);
        Task AddAsync(Guid blockerId, Guid blockedId, ApplicationContext context);
        Task DeleteAsync(Guid blockerId, Guid blockedId, ApplicationContext context);
       IQueryable<UserBlock> GetUserBlocks(Guid userId, int page, int pageSize, ApplicationContext context); 
    }
}
