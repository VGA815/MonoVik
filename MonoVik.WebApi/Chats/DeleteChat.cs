using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.Chats.Infrastructure;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using System.IdentityModel.Tokens.Jwt;

namespace MonoVik.WebApi.Chats
{
    public class DeleteChat
    {
        public const string Tag = "Chats";
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapDelete("api/chats", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static async Task<IResult> Handler(
            [FromQuery] Guid chatId,
            IHttpContextAccessor httpContextAccessor,
            IChatRepository chatRepository,
            ApplicationContext context)
        {
            Chat? chat = await chatRepository.GetWithMembers(chatId, context);
            if (chat is null) return Results.BadRequest();
            Guid userId = Guid.Parse(
                new JwtSecurityTokenHandler()
                .ReadJwtToken(httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length)).Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            if (!chat!.ChatMembers.Any(cm => cm.UserId == userId)) return Results.Forbid();
            if (chat!.ChatMembers.FirstOrDefault(cm => cm.UserId == userId)!.Role != "owner") return Results.Forbid();
            await chatRepository.DeleteAsync(chatId, context);
            return Results.Ok();
        }
    }
}
