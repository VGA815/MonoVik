using Microsoft.EntityFrameworkCore;
using MonoVik.WebApi.Database;
using FluentValidation;
using MonoVik.WebApi.Endpoints;
using Serilog;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using MonoVik.WebApi.Middleware;
using System.Net.Mail;
using System.Net;
using MonoVik.WebApi.Users.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using MonoVik.WebApi.UserPreferences.Infrastructure;
using MonoVik.WebApi.UserBlocks.Infrastructure;
using MonoVik.WebApi.Chats.Infrastructure;
using MonoVik.WebApi.ChatMembers.Infrastructure;
using MonoVik.WebApi.Messages.Infrastructure;
using MonoVik.WebApi.MediaFiles.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
# region SwaggerGen
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(t => t.FullName?.Replace('+', '.'));

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Jwt Authentication",
        Description = "Enter your JWT in this field",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT"
    };

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            []
        }
    };

    options.AddSecurityRequirement(securityRequirement);
});
# endregion
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(builder.Configuration["ConnectionString"]));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddHealthChecks();
builder.Services.AddEndpoints();
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));
# region Smtp
var smtp = new SmtpClient
{
    Host = "smtp.gmail.com",
    Port = 587,
    EnableSsl = true,
    UseDefaultCredentials = false,
    DeliveryMethod = SmtpDeliveryMethod.Network,
    Credentials = new NetworkCredential("vovan990028@gmail.com", "emkx zlpq usgg tokk")
};
builder.Services.AddFluentEmail("vovan990028@gmail.com")
    .AddSmtpSender(smtp);
# endregion
# region Repositories
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IUserPreferenceRepository, UserPreferenceRepository>();
builder.Services.AddSingleton<IUserBlockRepository, UserBlockRepository>();
builder.Services.AddSingleton<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
builder.Services.AddSingleton<IChatRepository, ChatRepository>();
builder.Services.AddSingleton<IChatMembersRepository, ChatMembersRepository>();
builder.Services.AddSingleton<IMessagesRepository, MessagesRepository>();
builder.Services.AddSingleton<IMediaFileRepository, MediaFileRepository>();
# endregion
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<EmailVerificationLinkFactory>();
builder.Services.AddScoped<MediaFileLinkFactory>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<TokenProvider>();
# region Authorization
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ClockSkew = TimeSpan.Zero
        };
    });
# endregion
var app = builder.Build();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseRequestContextLogging();
app.UseSerilogRequestLogging();

app.UseAuthentication();

app.UseAuthorization();

app.Run();