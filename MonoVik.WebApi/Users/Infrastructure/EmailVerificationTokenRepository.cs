using MonoVik.WebApi.Database;

namespace MonoVik.WebApi.Users.Infrastructure
{
    public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
    {
        public async Task InsertAsync(EmailVerificationToken emailVerificationToken, ApplicationContext context)
        {
            context.EmailVerificationTokens.Add(emailVerificationToken);
            await context.SaveChangesAsync();
        }
    }
}
