using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.Messages.Infrastructure;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonoVik.WebApi.Messages
{
    public class UpdateMessage
    {
        public const string Tag = "Messages";
        public record Request(
            [property: JsonPropertyName("message_id")] Guid MessageId,
            [property: JsonPropertyName("content")] string Content,
            [property: JsonPropertyName("tags")] List<string> Tags);
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Content).NotEmpty().NotNull().MinimumLength(1).MaximumLength(1000);
            }
        }
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPut("api/chats/{chatId:guid}/messages", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static async Task<IResult> Handler(
            [FromRoute] Guid chatId,
            [FromBody] Request request,
            ApplicationContext context,
            IValidator<Request> validator,
            IMessagesRepository messagesRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

            if (!await messagesRepository.ExistsAsync(request.MessageId, context)) return Results.NotFound();
            Guid currentUserId = Guid.Parse(
                new JwtSecurityTokenHandler()
                    .ReadJwtToken(httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length)).Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            Message message = messagesRepository.GetById(request.MessageId, context);
            if (!currentUserId.Equals(message.SenderId) || DateTime.UtcNow > message.CreatedAt!.Value.AddHours(1)) return Results.Forbid();
            message.Content = request.Content;
            message.Tags = request.Tags;
            await messagesRepository.UpdateAsync(message, context);
            return Results.Ok();
        }
    }
}
