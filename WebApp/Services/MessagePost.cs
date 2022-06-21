using System.ComponentModel.DataAnnotations;

namespace WebApp.Services
{
    public class MessagePost
    {
        [Key]
        public int id { get; set; }
        public string content { get; set; }
        public bool sent { get; set; }
        public string? date { get; set; }
    }
}
