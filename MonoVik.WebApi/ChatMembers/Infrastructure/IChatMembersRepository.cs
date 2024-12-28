using MonoVik.WebApi.Database;

namespace MonoVik.WebApi.ChatMembers.Infrastructure
{
    public interface IChatMembersRepository
    {
        Task AddAsync(ChatMember chatMember, ApplicationContext context);
        Task RemoveAsync(ChatMember chatMember, ApplicationContext context);
        IQueryable<ChatMember> GetMembersByChat(Guid chatId, int page, int pageSize, ApplicationContext context);
        Task UpdateAsync(ChatMember chatMember, ApplicationContext context);
    }
}
