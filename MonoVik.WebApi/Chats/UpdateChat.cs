using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.ChatMembers;
using MonoVik.WebApi.Chats.Infrastructure;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.Helpers;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonoVik.WebApi.Chats
{
    public class UpdateChat
    {
        public record Request(
            [property: JsonPropertyName("chat_id")] Guid ChatId,
            [property: JsonPropertyName("chat_name")] string ChatName,
            [property: JsonPropertyName("chat_description")] string? ChatDescription = null,
            [property: JsonPropertyName("chat_image_url")] string? ChatImageUrl = null,
            [property: JsonPropertyName("is_group")] bool? IsGroup = null,
            [property: JsonPropertyName("chat_type")] string? ChatType = null);
        public const string Tag = "Chats";
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.ChatName).NotEmpty().NotNull().MaximumLength(100).MinimumLength(5).Matches(Regexs.Username);
                RuleFor(x => x.ChatDescription).MaximumLength(300);
                RuleFor(x => x.ChatImageUrl).Matches(Regexs.Url);
            }
        }
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPut("api/chats", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static async Task<IResult> Handler(
            [FromBody] Request request,
            IChatRepository chatRepository,
            IValidator<Request> validator,
            IHttpContextAccessor httpContextAccessor,
            ApplicationContext context)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);
            Chat? oldChat = await chatRepository.GetWithMembers(request.ChatId, context);
            if (oldChat is null) return Results.BadRequest();
            Guid userId = Guid.Parse(
                new JwtSecurityTokenHandler()
                .ReadJwtToken(httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length)).Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            ChatMember? chatMember = oldChat.ChatMembers.FirstOrDefault(cm => cm.UserId == userId);
            if (chatMember is null) return Results.Forbid();
            if (chatMember.Role == "admin" ||  chatMember.Role == "owner")
            {
                Chat newChat = new Chat
                {
                    ChatDescription = request.ChatDescription,
                    ChatId = request.ChatId,
                    ChatImageUrl = request.ChatImageUrl,
                    ChatName = request.ChatName,
                    ChatType = request.ChatType,
                    IsGroup = request.IsGroup
                };
                await chatRepository.UpdateAsync(newChat, context);
                return Results.Ok();
            }
            else {
                Results.Forbid();
            }
            return Results.InternalServerError("Error in chat update logic. Check PUT api/chats endpoint");
        }
    }
}
