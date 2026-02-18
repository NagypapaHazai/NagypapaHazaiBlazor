namespace NagypapaHazaiBlazor.MODELS
{
    public class Booking
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int? EventId { get; set; }
        public string UserName { get; set; } = "";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "pending";
        public string? ExchangeOffer { get; set; }

        public Property? Property { get; set; }
        public Event? Event { get; set; }
    }
}
