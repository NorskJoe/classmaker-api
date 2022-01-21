namespace classmaker_models.Entities
{
    public class User
    {
        public int UserId { get; init; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }  
    }
}