namespace API.StoreShared.Auth
{   public class AuthResult
    {
        public bool Result { get; set; }
        public string Token { get; set; }
        public List<string> Errors { get; set; }
    }
}
