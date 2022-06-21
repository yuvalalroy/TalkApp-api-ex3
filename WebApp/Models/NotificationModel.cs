using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WebApp.Models
{
    public class NotificationModel
    {
        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
    }
}
