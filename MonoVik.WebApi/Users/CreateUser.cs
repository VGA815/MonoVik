using FluentEmail.Core;
using FluentValidation;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.Helpers;
using MonoVik.WebApi.UserPreferences.Infrastructure;
using MonoVik.WebApi.Users.Infrastructure;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace MonoVik.WebApi.Users
{
    public static class CreateUser
    {
        public record Request(
            [property: JsonPropertyName("username")] string UserName,
            [property: JsonPropertyName("password")] string Password,
            [property: JsonPropertyName("email")] string Email);
        public record Response(
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("jwt")] string Jwt);
        public const string Tag = "Users";
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.UserName).NotEmpty().NotNull().MaximumLength(50).Matches(Regexs.Username);
                RuleFor(r => r.Password).NotEmpty().NotNull().MaximumLength(100).Matches(Regexs.Password);
                RuleFor(r => r.Email).NotEmpty().NotNull().MaximumLength(100).Matches(Regexs.Email);
            }
        }
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("api/users", Handler).WithTags(Tag);
            }
        }
        public static async Task<IResult> Handler(
            Request request,
            IFluentEmail fluentEmail,
            TokenProvider tokenProvider,
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IValidator<Request> validator,
            IEmailVerificationTokenRepository emailVerificationTokenRepository,
            EmailVerificationLinkFactory emailVerificationLinkFactory,
            ApplicationContext context,
            IUserPreferenceRepository userPreferenceRepository)
        {
            if (await userRepository.ExistsAsync(request.Email, context)) return Results.BadRequest("The email is already in use");
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = request.UserName,
                Email = request.Email,
                PasswordHash = passwordHasher.Hash(request.Password)
            };

            await userRepository.InsertAsync(user, context);

            DateTime utcNow = DateTime.UtcNow;

            var verificationToken = new EmailVerificationToken
            {
                TokenId = Guid.NewGuid(),
                UserId = user.UserId,
                CreatedAt = utcNow,
                ExpiresAt = utcNow.AddDays(1)
            };

            await emailVerificationTokenRepository.InsertAsync(verificationToken, context);

            string verificationLink = emailVerificationLinkFactory.Create(verificationToken);

            await fluentEmail
                .To(user.Email)
                .Subject("Email verification for MicroVik")
                .Body($"To verify your email address <a href='{verificationLink}'>click here</a>", isHtml: true)
                .SendAsync();

            var token = tokenProvider.Create(user);

            await userPreferenceRepository.CreateDefaultAsync(user.UserId, context);

            return Results.Ok(new Response(user.UserId.ToString(), token));
        }

    }
}
