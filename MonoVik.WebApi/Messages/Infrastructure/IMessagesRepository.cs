using MonoVik.WebApi.Database;

namespace MonoVik.WebApi.Messages.Infrastructure
{
    public interface IMessagesRepository
    {
        Task CreateAsync(Message message, ApplicationContext context);
        Task DeleteAsync(Guid messageId, ApplicationContext context);
        Task UpdateAsync(Message message, ApplicationContext context);
        IQueryable<Message> GetByChatAsync(Guid chatId, int page, int pageSize, ApplicationContext context);
        Task<bool> ExistsAsync(Guid messageId, ApplicationContext context);
        Message GetById(Guid messageId, ApplicationContext context);
    }
}
