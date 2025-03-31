using Microsoft.EntityFrameworkCore;
using MonoVik.WebApi.Database;

namespace MonoVik.WebApi.ChatMembers.Infrastructure
{
    public sealed class ChatMembersRepository : IChatMembersRepository
    {
        public async Task AddAsync(ChatMember chatMember, ApplicationContext context)
        {
            await context.ChatMembers.AddAsync(chatMember);
            await context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid chatId, Guid userId, ApplicationContext context)
        {
            return await context.ChatMembers.AnyAsync(cm => cm.UserId == userId && cm.ChatId == chatId);
        }

        public async Task<ChatMember?> GetById(Guid chatId, Guid userId, ApplicationContext context)
        {
            return await context.ChatMembers.FirstOrDefaultAsync(cm => cm.ChatId == chatId && cm.UserId == userId);
        }

        public IQueryable<ChatMember> GetMembersByChat(Guid chatId, int page, int pageSize, ApplicationContext context)
        {
            var members = context.ChatMembers
                .Where(cm => cm.ChatId == chatId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            return members;
        }

        public async Task RemoveAsync(ChatMember chatMember, ApplicationContext context)
        {
            await context.ChatMembers
                .Where(cm => cm.UserId == chatMember.UserId && cm.ChatId == chatMember.ChatId)
                .ExecuteDeleteAsync();
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChatMember chatMember, ApplicationContext context)
        {
            await context.ChatMembers
                .Where(cm => cm.UserId == chatMember.UserId && cm.ChatId == chatMember.ChatId)
                .ExecuteUpdateAsync(cm => cm
                    .SetProperty(cm => cm.CanPost, chatMember.CanPost)
                    .SetProperty(cm => cm.CanAddMembers, chatMember.CanAddMembers)
                    .SetProperty(cm => cm.CanRemoveMembers, chatMember.CanRemoveMembers)
                    .SetProperty(cm => cm.Role, chatMember.Role));
        }
    }
}
