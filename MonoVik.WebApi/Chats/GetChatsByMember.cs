using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.Chats.Infrastructure;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace MonoVik.WebApi.Chats
{
    public class GetChatsByMember
    {
        public const string Tag = "Chats";
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapGet("api/chats", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static IResult Handler(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            IHttpContextAccessor httpContextAccessor,
            IChatRepository chatRepository,
            ApplicationContext context)
        {
            Guid userId = Guid.Parse(
                new JwtSecurityTokenHandler()
                .ReadJwtToken(httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length)).Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            var chats = chatRepository.GetChatsByMember(userId, page, pageSize, context);
            var response = chats.Select(c => c.ChatId);
            return Results.Ok(response);
        }
    }
}
