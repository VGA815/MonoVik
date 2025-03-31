using Microsoft.AspNetCore.Http;
using MonoVik.WebApi.Chats.Infrastructure;
using MonoVik.WebApi.Chats;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.Users.Infrastructure;
using MonoVik.WebApi.Users;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.ChatMembers.Infrastructure;
using MonoVik.WebApi.Database;

namespace MonoVik.WebApi.ChatMembers
{
    public class RemoveChatMember
    {
        public const string Tag = "ChatMembers";
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapDelete("api/chats/{chatId:guid}/members", Handler)
                .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static async Task<IResult> Handler(
            [FromRoute] Guid chatId,
            [FromQuery] Guid userId,
            IHttpContextAccessor httpContextAccessor,
            IChatMembersRepository chatMembersRepository,
            ApplicationContext context)
        {
            Guid currentUserId = Guid.Parse(
                new JwtSecurityTokenHandler()
                .ReadJwtToken(httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length)).Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            ChatMember? chatMember = await chatMembersRepository.GetById(chatId, currentUserId, context);
            if (chatMember == null) return Results.NotFound();
            if (currentUserId.Equals(userId))
            {
                ChatMember member = new ChatMember
                {
                    UserId = currentUserId,
                    ChatId = chatId,
                };
                await chatMembersRepository.RemoveAsync(member, context);
                return Results.Ok();
            }
            if (chatMember.CanRemoveMembers == false || chatMember.CanRemoveMembers == null) return Results.Forbid();
            ChatMember? rmMember = await chatMembersRepository.GetById(chatId, userId, context);
            if (rmMember == null) return Results.NotFound();
            await chatMembersRepository.RemoveAsync(rmMember, context);
            return Results.Ok();
        }
    }
}
