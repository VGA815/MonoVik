using FluentValidation;
using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.Helpers;
using MonoVik.WebApi.Users.Infrastructure;
using System.Text.Json.Serialization;

namespace MonoVik.WebApi.Users
{
    public class LoginUser
    {
        public record Request(
            [property: JsonPropertyName("email")] string Email,
            [property: JsonPropertyName("password")] string Password);
        public record Response(
            [property: JsonPropertyName("jwt")] string Jwt);
        public const string Tag = "Users";
        public sealed class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(r => r.Email).NotEmpty().NotNull().Matches(Regexs.Email);
                RuleFor(r => r.Password).NotNull().NotEmpty().Matches(Regexs.Password);
            }
        }
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("api/users/login", Handler).WithTags(Tag);
            }
        }
        public static async Task<IResult> Handler(
            Request request, 
            IUserRepository 
            userRepository, 
            IPasswordHasher 
            passwordHasher, 
            TokenProvider tokenProvider, 
            ApplicationContext context,
            IValidator<Request> validator)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors);

            var user = await userRepository.GetAsync(request.Email, context);
            if (user == null) return Results.BadRequest("The user was not found");

            bool verified = passwordHasher.Verify(request.Password, user.PasswordHash);

            if (!verified) return Results.BadRequest("The password is incorrect");

            var token = tokenProvider.Create(user);

            return Results.Ok(new Response(token));
        }
    }
}
