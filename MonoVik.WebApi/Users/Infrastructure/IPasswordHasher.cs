namespace MonoVik.WebApi.Users.Infrastructure
{
    public interface IPasswordHasher
    {
        public string Hash(string password);
        bool Verify(string password, string passwordHash);
    }
}
