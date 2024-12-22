using MonoVik.WebApi.Database;

namespace MonoVik.WebApi.Users.Infrastructure
{
    public interface IEmailVerificationTokenRepository
    {
        Task InsertAsync(EmailVerificationToken emailVerificationToken, ApplicationContext context);
    }
}
