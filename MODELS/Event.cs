namespace NagypapaHazaiBlazor.MODELS
{
    public class Event
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MaxParticipants { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Property? Property { get; set; }
        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
