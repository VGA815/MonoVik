using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.UserBlocks.Infrastructure;
using MonoVik.WebApi.Users.Infrastructure;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonoVik.WebApi.UserBlocks
{
    public class UnblockUser
    {
        public record Request(
            [property: JsonPropertyName("blocked_id")] Guid BlockedId);
        public const string Tag = "UserBlocks";
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.BlockedId).NotEmpty().NotNull();
            }
        }
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapDelete("api/user/blocks", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static async Task<IResult> Handler(
            [FromBody] Request request,
            IUserBlockRepository userBlockRepository,
            ApplicationContext context,
            IValidator<Request> validator,
            IHttpContextAccessor httpContextAccessor)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

            HttpContext httpContext = httpContextAccessor.HttpContext!;
            var jwt = new JwtSecurityTokenHandler()
                .ReadJwtToken(httpContext.Request.Headers["Authorization"]!.ToString().Substring("Bearer ".Length));
            Guid userId = Guid.Parse(jwt.Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            if (!await userBlockRepository.ExistsAsync(userId, request.BlockedId, context)) return Results.NotFound();
            await userBlockRepository.DeleteAsync(userId, request.BlockedId, context);
            return Results.Ok();
        }
    }
}
