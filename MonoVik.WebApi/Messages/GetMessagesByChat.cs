using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.ChatMembers.Infrastructure;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.Messages.Infrastructure;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace MonoVik.WebApi.Messages
{
    public class GetMessagesByChat
    {
        public const string Tag = "Messages";
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapGet("api/chats/{chatId:guid}/messages", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static IResult Handler(
            [FromRoute] Guid chatId,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            ApplicationContext context,
            IMessagesRepository messagesRepository,
            IChatMembersRepository chatMembersRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            Guid currentUserId = Guid.Parse(
                new JwtSecurityTokenHandler()
                    .ReadJwtToken(httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length)).Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            if (chatMembersRepository.GetById(chatId, currentUserId, context) is null) return Results.NotFound();
            return Results.Ok(messagesRepository.GetByChatAsync(chatId, page, pageSize, context));
        }
    }
}
