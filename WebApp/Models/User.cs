using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;


namespace WebApp.Models
{
    public class User
    {
        [Key]
        public string userName { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string displayName { get; set; }
        
        public string? profilePic { get; set; }
        [IgnoreDataMember]
        public List<Contact>? Contacts { get; set; }

        [IgnoreDataMember]
        public List<AndroidDeviceIDModel>? DeviceIDs { get; set; }


    }
}
