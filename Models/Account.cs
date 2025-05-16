using System.ComponentModel.DataAnnotations;
using System.Data;
namespace demo.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public Role Role { get; set; } = Role.User;
    }

    public enum Role 
    {
        Admin,
        User
    }
}
