namespace crud.Models
{
    public class Auth
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string SecuredPassword { get; set; }
        public byte[]? ProfilePicture { get; set; }

    }

}
