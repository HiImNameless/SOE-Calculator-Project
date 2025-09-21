namespace SOE_Calculator_Project.Models
{
    using System.Security.Cryptography;

    public class User
    {
        // Brandon Lombaard 223021599
        // Columns for the User table
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
