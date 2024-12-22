using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.UserBlocks.Infrastructure;
using MonoVik.WebApi.Users.Infrastructure;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace MonoVik.WebApi.UserBlocks
{
    public class GetBlocksByBlocker
    {
        public const string Tag = "UserBlocks";
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapGet("api/users/{blockerId:guid}/blocks", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static async Task<IResult> Handler(
            [FromRoute] Guid blockerId,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            IHttpContextAccessor httpContextAccessor,
            IUserBlockRepository userBlockRepository,
            ApplicationContext context,
            IUserRepository userRepository)
        {
            if (!await userRepository.ExistsAsync(blockerId, context)) return Results.NotFound();

            HttpContext httpContext = httpContextAccessor.HttpContext!;
            var jwt = new JwtSecurityTokenHandler()
                .ReadJwtToken(httpContext.Request.Headers["Authorization"]!.ToString().Substring("Bearer ".Length));
            Guid userId = Guid.Parse(jwt.Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            if (!userId.Equals(blockerId)) return Results.Forbid();
            return Results.Ok(userBlockRepository.GetUserBlocks(blockerId, page, pageSize, context));
        }
    }
}
