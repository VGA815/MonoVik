using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;

namespace MonoVik.WebApi.Users
{
    public static class VerifyEmail
    {
        public const string Tag = "Users";
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapGet("api/users/verify-email", Handler).WithTags(Tag).WithName("VerifyEmail");
            }
        }
        public static async Task<IResult> Handler([FromQuery] Guid tokenId, ApplicationContext context)
        {
            EmailVerificationToken? token = await context.EmailVerificationTokens.Include(e => e.User).FirstOrDefaultAsync(e => e.TokenId == tokenId);

            if (token is null || token.ExpiresAt < DateTime.UtcNow || token.User.IsVerified)
            {
                return Results.BadRequest("something went wrong");
            }

            token.User.IsVerified = true;

            context.EmailVerificationTokens.Remove(token);

            await context.SaveChangesAsync();

            return Results.Ok();
        }
    }
}
