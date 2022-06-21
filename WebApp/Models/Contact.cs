using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WebApp.Models
{
    public class Contact
    {
        [Key]
        [IgnoreDataMember]
        public int Identifier { get; set; }

        //Name
        public string? id { get; set; }

        [IgnoreDataMember]
        public User? User { get; set; }

        //displayName
        public string name { get; set; }

        public string server { get; set; }

        public string? last { get; set; }

        public string? lastdate { get; set; }

        [IgnoreDataMember]
        public List<Message>? Messages { get; set; }



    }
}
