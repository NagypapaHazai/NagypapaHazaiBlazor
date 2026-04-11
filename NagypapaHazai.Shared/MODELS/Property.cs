namespace NagypapaHazai.Shared.MODELS
{
    public class Property
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Location { get; set; } = "";
        public string? Description { get; set; }
        public int? Capacity { get; set; }
        public string? Status { get; set; }
        public int PricePerNight { get; set; }
        public string? ImageUrl { get; set; }       // ← ÚJ
        public DateTime CreatedAt { get; set; } = DateTime.Now; // ← ÚJ


        // ÚJ MEZŐ – ide tudsz majd extra szöveges infót írni az ingatlanról
        public string? ExtraInfo { get; set; }


        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
