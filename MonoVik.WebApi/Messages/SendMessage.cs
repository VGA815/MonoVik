using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.ChatMembers;
using MonoVik.WebApi.ChatMembers.Infrastructure;
using MonoVik.WebApi.Chats;
using MonoVik.WebApi.Chats.Infrastructure;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.Helpers;
using MonoVik.WebApi.Messages.Infrastructure;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonoVik.WebApi.Messages
{
    public class SendMessage
    {
        public record Request(
            [property: JsonPropertyName("content")] string Content,
            [property: JsonPropertyName("attachment_url")] string? AttachmentUrl,
            [property: JsonPropertyName("tags")] List<string>? Tags);
        public const string Tag = "Messages";
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.AttachmentUrl).Matches(Regexs.Url);
                RuleFor(x => x.Content).NotEmpty().NotNull().MinimumLength(1).MaximumLength(1000);
            }
        }
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("api/chats/{chatId}/messages", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static async Task<IResult> Handler(
            [FromBody] Request request,
            [FromRoute] Guid chatId,
            IValidator<Request> validator,
            IHttpContextAccessor httpContextAccessor,
            IMessagesRepository messagesRepository,
            IChatMembersRepository chatMembersRepository,
            ApplicationContext context)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);
            Guid currentUserId = Guid.Parse(
                new JwtSecurityTokenHandler()
                .ReadJwtToken(httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length)).Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            ChatMember? chatMember = await chatMembersRepository.GetById(chatId, currentUserId, context);
            if (chatMember is null) return Results.NotFound();
            if (chatMember.CanPost is null || chatMember.CanPost == false) return Results.Forbid();
            Message message = new Message
            {
                AttachmentUrl = request.AttachmentUrl,
                ChatId = chatId,
                Content = request.Content,
                CreatedAt = DateTime.UtcNow,
                IsEdited = false,
                SenderId = currentUserId,
                Tags = request.Tags,
                MessageId = Guid.NewGuid(),
                UpdatedAt = DateTime.UtcNow,
            };
            await messagesRepository.CreateAsync(message, context);

            var rvalue = new
            {
                method = "publish",
                @params = new
                {
                    channel = message.ChatId.ToString(),
                    data = new
                    {
                        message_id = message.MessageId,
                        sender_id = message.SenderId,
                        chat_id = message.ChatId,
                        content = message.Content,
                        attachment_url = message.AttachmentUrl,
                        tags = message.Tags,
                        created_at = message.CreatedAt,
                        updated_at = message.UpdatedAt
                    }
                }
            };

            using (var client = new HttpClient())
            {
                await client.PostAsync("http://centrifugo:8000/api", JsonContent.Create(rvalue));
            }

            return Results.Created();
        }
    }
}
