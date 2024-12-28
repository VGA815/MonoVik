using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.Helpers;
using MonoVik.WebApi.Users.Infrastructure;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonoVik.WebApi.Users
{
    public class UpdateUser
    {
        public record Request([property: JsonPropertyName("user")] User User);
        public const string Tag = "Users";
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.User.UserId).NotEmpty().NotNull();
                RuleFor(r => r.User.Username).NotEmpty().NotNull().MaximumLength(50).Matches(Regexs.Username);
                RuleFor(r => r.User.AvatarUrl).NotNull();
                RuleFor(r => r.User.StatusMessage).NotNull();
            }
            public sealed class Endpoint : IEndpoint
            {
                public void MapEndpoint(IEndpointRouteBuilder app)
                {
                    app.MapPut("api/users", Handler)
                        .WithTags(Tag)
                        .RequireAuthorization();
                }
            }
            public static async Task<IResult> Handler(
                [FromBody] Request request, 
                ApplicationContext context, 
                ILogger<UpdateUser> logger, 
                IUserRepository userRepository, 
                IHttpContextAccessor httpContextAccessor,
                IValidator<Request> validator)
            {
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

                HttpContext httpContext = httpContextAccessor.HttpContext!;
                if (!await userRepository.ExistsAsync(request.User.UserId, context)) return Results.NotFound();
                var jwt = new JwtSecurityTokenHandler()
                    .ReadJwtToken(httpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length));
                Guid userId = Guid.Parse(jwt.Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
                if (!userId.Equals(request.User.UserId)) return Results.Forbid();
                await userRepository.UpdateAsync(request.User, context);
                return Results.Ok();
            }
        }
    }
}