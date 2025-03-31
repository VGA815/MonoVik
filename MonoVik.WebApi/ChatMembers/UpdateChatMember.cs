using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.ChatMembers.Infrastructure;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonoVik.WebApi.ChatMembers
{
    public class UpdateChatMember
    {
        public record Request(
            [property: JsonPropertyName("user_id")] Guid UserId,
            [property: JsonPropertyName("role")] string Role,
            [property: JsonPropertyName("can_post")] bool CanPost,
            [property: JsonPropertyName("can_add_members")] bool CanAddMembers,
            [property: JsonPropertyName("can_remove_members")] bool CanRemoveMembers);
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.CanRemoveMembers).NotEmpty().NotNull();
            }
        }
        public const string Tag = "ChatMembers";
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPut("api/chats/{chatId:guid}/members", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static async Task<IResult> Handler(
            [FromBody] Request request,
            [FromRoute] Guid chatId,
            ApplicationContext context,
            IValidator<Request> validator,
            IChatMembersRepository chatMembersRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);
            Guid currentUserId = Guid.Parse(
                new JwtSecurityTokenHandler()
                    .ReadJwtToken(httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length)).Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            ChatMember? chatMember = await chatMembersRepository.GetById(chatId, currentUserId, context);
            if (chatMember == null) return Results.NotFound();
            if (chatMember.Role != "owner" || chatMember.CanRemoveMembers == null) return Results.Forbid();
            ChatMember updMember = new ChatMember
            {
                CanAddMembers = request.CanAddMembers,
                CanRemoveMembers = request.CanRemoveMembers,
                CanPost = request.CanPost,
                ChatId = chatId,
                Role = request.Role,
                UserId = request.UserId,
            };
            await chatMembersRepository.UpdateAsync(updMember, context);
            return Results.Ok();
        }
    }
}
