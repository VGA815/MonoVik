using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.ChatMembers.Infrastructure;
using MonoVik.WebApi.Chats;
using MonoVik.WebApi.Chats.Infrastructure;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.Users.Infrastructure;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonoVik.WebApi.ChatMembers
{
    public class AddChatMember
    {
        public const string Tag = "ChatMembers";
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("api/cahts/{chatId:guid}/members", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static async Task<IResult> Handler(
            [FromRoute] Guid chatId,
            [FromQuery] Guid userId,
            IHttpContextAccessor httpContextAccessor,
            IChatMembersRepository chatMembersRepository,
            IChatRepository chatRepository,
            IUserRepository userRepository,
            ApplicationContext context)
        {
            if (!await userRepository.ExistsAsync(userId, context)) return Results.NotFound("User doesnt exists");
            Guid currentUserId = Guid.Parse(
                new JwtSecurityTokenHandler()
                .ReadJwtToken(httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length)).Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            Chat? chat = await chatRepository.GetWithMembers(chatId, context);
            if (chat == null) return Results.NotFound("Chat doesnt exists");
            bool? canAdd = chat.ChatMembers.FirstOrDefault(cm => cm.UserId == currentUserId)!.CanAddMembers;
            if (canAdd == null || canAdd == false) return Results.Forbid();
            if (await chatMembersRepository.ExistsAsync(userId, chatId, context)) return Results.BadRequest("Chat member already exists");
            ChatMember chatMember = new ChatMember
            {
                UserId = userId,
                ChatId = chatId,
            };

            await chatMembersRepository.AddAsync(chatMember, context);
            return Results.Ok();
        }
    }
}
