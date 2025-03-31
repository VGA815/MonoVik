using Microsoft.EntityFrameworkCore;
using MonoVik.WebApi.Database;

namespace MonoVik.WebApi.Messages.Infrastructure
{
    public sealed class MessagesRepository : IMessagesRepository
    {
        public async Task CreateAsync(Message message, ApplicationContext context)
        {
            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid messageId, ApplicationContext context)
        {
            await context.Messages
                .Where (x => x.MessageId == messageId)
                .ExecuteDeleteAsync();
            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid messageId, ApplicationContext context)
        {
            return await context.Messages.AnyAsync(x => x.MessageId == messageId);
        }

        public IQueryable<Message> GetByChatAsync(Guid chatId, int page, int pageSize, ApplicationContext context)
        {
            var msgs = context.Messages
                .Where(m => m.ChatId == chatId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            return msgs;
        }

        public Message GetById(Guid messageId, ApplicationContext context)
        {
            return context.Messages.FirstOrDefault(m => m.MessageId == messageId)!;
        }

        public async Task UpdateAsync(Message message, ApplicationContext context)
        {
            await context.Messages
                .Where(m => m.MessageId == message.MessageId)
                .ExecuteUpdateAsync(m => m
                    .SetProperty(msg => msg.IsEdited, true)
                    .SetProperty(msg => msg.Content, message.Content)
                    .SetProperty(msg => msg.UpdatedAt, DateTime.UtcNow)
                    .SetProperty(msg => msg.Tags, message.Tags));
        }
    }
}
