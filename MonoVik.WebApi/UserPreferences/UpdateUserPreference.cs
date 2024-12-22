using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.UserPreferences.Infrastructure;
using MonoVik.WebApi.Users.Infrastructure;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonoVik.WebApi.UserPreferences
{
    public class UpdateUserPreference
    {
        public record Request([property: JsonPropertyName("user_preference")] UserPreference UserPreference);
        public const string Tag = "UserPreferences";
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.UserPreference.Theme).NotEmpty().NotNull();
                RuleFor(x => x.UserPreference.PrivacyLevel).NotEmpty().NotNull();
                RuleFor(x => x.UserPreference.NotificationSound).NotEmpty().NotNull();
                RuleFor(x => x.UserPreference.ReceiveNotifications).NotEmpty().NotNull();
            }
        }
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPut("api/user/preferences", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization();
            }
        }
        public static async Task<IResult> Handler(
            [FromBody] Request request, 
            ApplicationContext context, 
            IUserPreferenceRepository userPreferenceRepository,
            IValidator<Request> validator,
            IHttpContextAccessor httpContextAccessor)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);
            if (!await userPreferenceRepository.ExistsAsync(request.UserPreference.UserId, context)) return Results.NotFound();

            HttpContext httpContext = httpContextAccessor.HttpContext!;
            var jwt = new JwtSecurityTokenHandler()
                .ReadJwtToken(httpContext.Request.Headers["Authorization"]!.ToString().Substring("Bearer ".Length));
            Guid userId = Guid.Parse(jwt.Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            if (!userId.Equals(request.UserPreference.UserId)) return Results.Forbid();
            await userPreferenceRepository.UpdateAsync(request.UserPreference, context);
            return Results.Ok();
        }
    }
}
