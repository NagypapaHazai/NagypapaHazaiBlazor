namespace NagypapaHazaiBlazor.MODELS
{
    public class Property
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Location { get; set; } = "";
        public int? Capacity { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "available";
        public string? ImageUrl { get; set; }   
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Event> Events { get; set; } = [];
        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
