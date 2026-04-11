using NagypapaHazaiBlazor.MODELS;
using System.ComponentModel.DataAnnotations.Schema;

namespace NagypapaHazai.Shared.MODELS
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PropertyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Pending"; 
        public string? AdminNote { get; set; }
        public string? ExchangeOffer { get; set; } 

        public User? User { get; set; }
        public Property? Property { get; set; }
    }
}
