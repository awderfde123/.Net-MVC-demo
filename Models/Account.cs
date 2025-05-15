using System.ComponentModel.DataAnnotations;
namespace demo.Models
{
    public class Account
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
