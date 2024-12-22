namespace MonoVik.WebApi.Users.Infrastructure
{
    public class EmailVerificationLinkFactory(IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
    {
        public string Create(EmailVerificationToken emailVerificationToken)
        {
            string? verificationLink = linkGenerator.GetUriByName(
                httpContextAccessor.HttpContext!,
                "VerifyEmail",
                new { tokenId = emailVerificationToken.TokenId });
            return verificationLink ?? throw new Exception("Could not create email verification link");
        }
    }
}
