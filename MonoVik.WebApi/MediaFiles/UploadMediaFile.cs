using MonoVik.WebApi.Database;
using MonoVik.WebApi.Endpoints;
using MonoVik.WebApi.MediaFiles.Infrastructure;
using MonoVik.WebApi.Users.Infrastructure;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MonoVik.WebApi.MediaFiles
{
    public class UploadMediaFile
    {
        public const string Tag = "MediaFiles";
        public sealed class Endpoint : IEndpoint
        {
            public void MapEndpoint(IEndpointRouteBuilder app)
            {
                app.MapPost("api/media", Handler)
                    .WithTags(Tag)
                    .RequireAuthorization()
                    .DisableAntiforgery();
            }
        }
        public static async Task<IResult> Handler(
            IFormFile file,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository,
            IMediaFileRepository mediaFileRepository,
            MediaFileLinkFactory mediaFileLinkFactory,
            ApplicationContext context)
        {
            Guid currentUserId = Guid.Parse(
                new JwtSecurityTokenHandler()
                .ReadJwtToken(httpContextAccessor.HttpContext!.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length)).Claims.FirstOrDefault(c => c.Type == "sub")!.Value);
            if (!await userRepository.ExistsAsync(currentUserId, context)) return Results.BadRequest();
            Guid mediaId = Guid.NewGuid();
            var dirPath = Path.Combine(configuration["actualPath"]!, currentUserId.ToString());
            var filePath = Path.Combine(dirPath, mediaId.ToString());
            string mediaFileLink = mediaFileLinkFactory.Create(currentUserId, mediaId);
            MediaFile mediaFile = new MediaFile
            {
                FileType = file.ContentType,
                FileUrl = mediaFileLink,
                MediaId = mediaId,
                FileSize = Convert.ToInt32(file.Length),
                IsPublic = false,
                UploadDate = DateTime.UtcNow,
                UploaderId = currentUserId,
            };
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            using (var filestream = File.Create(filePath, 2048, FileOptions.SequentialScan))
            {
                await file.CopyToAsync(filestream);
            }
            await mediaFileRepository.InsertAsync(mediaFile, context);
            return Results.Ok(mediaFileLink);
        }
    }
}
