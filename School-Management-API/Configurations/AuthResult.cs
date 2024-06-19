namespace School_Management_API.Configurations
{
    public class AuthResult
    {
        public Boolean Success { get; set; }

        public string Result { get; set; }

        public List<string> Errors { get; set; }

        public string Token { get; set; }
    }
}
