using Microsoft.EntityFrameworkCore;
using MonoVik.WebApi.Database;

namespace MonoVik.WebApi.Chats.Infrastructure
{
    public sealed class ChatRepository : IChatRepository
    {
        public async Task CreateAsync(Chat chat, ApplicationContext context)
        {
            await context.Chats.AddAsync(chat);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid chatId, ApplicationContext context)
        {
            await context.Chats
                .Where(c => c.ChatId == chatId)
                .ExecuteDeleteAsync();
            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid chatId, ApplicationContext context)
        {
            return await context.Chats.AnyAsync(c => c.ChatId == chatId);
        }

        public async Task<Chat?> GetById(Guid chatId, ApplicationContext context)
        {
            return await context.Chats.FirstOrDefaultAsync(c => c.ChatId.Equals(chatId));
        }

        public IQueryable<Chat> GetChatsByMember(Guid memberId, int page, int pageSize, ApplicationContext context)
        {
            var chats = context.Chats.Include(c => c.ChatMembers)
                .Where(c => c.ChatMembers.Any(cm => cm.UserId == memberId))
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            return chats;
        }

        public async Task<Chat?> GetWithMembers(Guid chatId, ApplicationContext context)
        {
            return await context.Chats.Include(c => c.ChatMembers).SingleOrDefaultAsync(c => c.ChatId == chatId);
        }

        public async Task UpdateAsync(Chat chat, ApplicationContext context)
        {
            await context.Chats
                .Where(c => c.ChatId == chat.ChatId)
                .ExecuteUpdateAsync(c =>
                    c.SetProperty(c => c.IsGroup, chat.IsGroup)
                    .SetProperty(c => c.ChatDescription, chat.ChatDescription)
                    .SetProperty(c => c.ChatImageUrl, chat.ChatImageUrl)
                    .SetProperty(c => c.ChatType, chat.ChatType)
                    .SetProperty(c => c.UpdatedAt, DateTime.UtcNow)
                    .SetProperty(c => c.ChatName, chat.ChatName)
                    .SetProperty(c => c.IsActive, true));
        }
    }
}
