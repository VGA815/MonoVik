using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.Messages.Infrastructure;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace MonoVik.WebApi.Messages
{
    public class DeleteMessage
    {
        public const string Tag = "Messages";
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapDelete("api/chats/messages", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static async Task<IResult> Handler(
            [FromQuery] Guid messageId,
            ApplicationContext context,
            IHttpContextAccessor httpContextAccessor,
            IMessagesRepository messagesRepository)
        {
            if (!await messagesRepository.ExistsAsync(messageId, context)) return Results.NotFound();
            Guid currentUserId = Guid.Parse(
                new JwtSecurityTokenHandler()
                    .ReadJwtToken(httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length)).Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            Message message = messagesRepository.GetById(messageId, context);
            if (!currentUserId.Equals(message.SenderId) || DateTime.UtcNow > message.CreatedAt!.Value.AddHours(1)) return Results.Forbid();
            await messagesRepository.DeleteAsync(messageId, context);
            return Results.Ok();
        }
    }
}
