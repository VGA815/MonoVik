using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.Users.Infrastructure;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace MonoVik.WebApi.Users
{
    public class DeleteUser
    {
        public const string Tag = "Users";
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapDelete("api/users", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static async Task<IResult> Handler(
            [FromQuery] Guid targetId, 
            IUserRepository userRepository, ApplicationContext context, IHttpContextAccessor httpContextAccessor, ILogger<DeleteUser> logger)
        {
            HttpContext httpContext = httpContextAccessor.HttpContext!;
            string authHeader = httpContext.Request.Headers["Authorization"].ToString();
            string jwt = authHeader.Substring("Bearer ".Length);
            var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
            Guid userId = Guid.Parse(token.Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            if (!userId.Equals(targetId)) return Results.Forbid();
            User? user = await userRepository.GetAsync(targetId, context);
            if (user is null) return Results.NotFound();
            await userRepository.DeleteAsync(user, context);
            return Results.Ok();
        }
    }
}
