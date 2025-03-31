using MonoVik.WebApi.Database;

namespace MonoVik.WebApi.Chats.Infrastructure
{
    public interface IChatRepository
    {
        Task<bool> ExistsAsync(Guid chatId, ApplicationContext context);
        Task CreateAsync(Chat chat, ApplicationContext context);
        Task DeleteAsync(Guid chatId, ApplicationContext context);
        IQueryable<Chat> GetChatsByMember(Guid memberId, int page, int pageSize, ApplicationContext context);
        Task UpdateAsync(Chat chat, ApplicationContext context);
        Task<Chat?> GetWithMembers(Guid chatId, ApplicationContext context);
        Task<Chat?> GetById(Guid chatId, ApplicationContext context);
    }
}
