using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.Users.Infrastructure;

namespace MonoVik.WebApi.Users
{
    public class GetUser
    {
        public sealed class Endpoint : IEndpoint
        {
            public const string Tag = "Users";
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapGet("api/users/{userId:guid}", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static async Task<IResult> Handler([FromRoute] Guid userId, IUserRepository userRepository, ApplicationContext context)
        {
            return await userRepository.ExistsAsync(userId, context) ? Results.Ok(await userRepository.GetAsync(userId, context)) : Results.NotFound();
        }
    }
}
