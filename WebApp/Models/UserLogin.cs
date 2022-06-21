using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class UserLogin
    {
        [Required]
        public string userName { get; set; }

        [Required]
        public string password { get; set; }
    }
}
