namespace MonoVik.WebApi.Helpers
{
    public static class Regexs
    {
        public const string Password = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$";
        public const string Email = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        public const string Username = "^[a-zA-Z_][a-zA-Z0-9_.]*";
        public const string Url = "https?:\\/\\/(www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b([-a-zA-Z0-9()@:%_\\+.~#?&//=]*)\r\n";
    }
}
