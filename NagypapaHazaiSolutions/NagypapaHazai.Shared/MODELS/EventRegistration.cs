using NagypapaHazaiBlazor.MODELS;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NagypapaHazai.Shared.MODELS
{
    public class EventRegistration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }
        [ForeignKey("EventId")]
        public Event? Event { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        // "pending", "approved", "rejected", "cancelled"
        [Required]
        public string Status { get; set; } = "pending";
    }
}
