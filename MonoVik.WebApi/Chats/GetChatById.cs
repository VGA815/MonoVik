using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.Chats.Infrastructure;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonoVik.WebApi.Chats
{
    public class GetChatById
    {
        public const string Tag = "Chats";
        public record Response(
            [property: JsonPropertyName("chat_id")] Guid ChatId,
            [property: JsonPropertyName("chat_name")] string ChatName,
            [property: JsonPropertyName("chat_description")] string? ChatDescription,
            [property: JsonPropertyName("chat_image_url")] string? ChatImageUrl,
            [property: JsonPropertyName("is_group")] bool? IsGroup,
            [property: JsonPropertyName("chat_type")] string? ChatType,
            [property: JsonPropertyName("is_active")] bool? IsActive,
            [property: JsonPropertyName("created_at")] DateTime? CreatedAt,
            [property: JsonPropertyName("updated_at")] DateTime? UpdatedAt);
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapGet("api/chats/{chatId:guid}", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static async Task<IResult> Handler(
            [FromRoute] Guid chatId,
            IChatRepository chatRepository,
            ApplicationContext context)
        {
            return Results.Ok(await chatRepository.GetById(chatId, context));
        }
    }
}
