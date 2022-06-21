using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WebApp.Models
{
    public class Message
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string content { get; set; }

        
        public string? created { get; set; }

        [Required]
        public bool sent { get; set; }

        [IgnoreDataMember]
        public Contact? Contact { get; set; }

    }
}
