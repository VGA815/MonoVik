using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.ChatMembers;
using MonoVik.WebApi.ChatMembers.Infrastructure;
using MonoVik.WebApi.Chats.Infrastructure;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;

namespace MonoVik.WebApi.Chats
{
    public class CreateChat
    {
        public record Request(
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
                app.MapPost("api/chats", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static async Task<IResult> Handler(
            [FromBody] Request request, 
            IChatRepository chatRepository, 
            IValidator<Request> validator, 
            ApplicationContext context,
            IHttpContextAccessor httpContextAccessor,
            IChatMembersRepository chatMembersRepository)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);
            Chat chat = new Chat
            {
                ChatDescription = request.ChatDescription,
                ChatId = Guid.NewGuid(),
                ChatImageUrl = request.ChatImageUrl,
                ChatType = request.ChatType,
                ChatName = request.ChatName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsGroup = request.IsGroup,
                UpdatedAt = DateTime.UtcNow,
            };
            await chatRepository.CreateAsync(chat, context);
            HttpContext httpContext = httpContextAccessor.HttpContext!;
            Guid userId = Guid.Parse(new JwtSecurityTokenHandler()
                    .ReadJwtToken(httpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length)).Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            ChatMember chatMember = new ChatMember
            {
                ChatId = chat.ChatId,
                UserId = userId,
                CanAddMembers = true,
                CanPost = true,
                CanRemoveMembers = true,
                JoinedAt = DateTime.UtcNow,
                Role = "owner"
            };
            await chatMembersRepository.AddAsync(chatMember, context);
            return Results.Ok();
        }
    }
}
