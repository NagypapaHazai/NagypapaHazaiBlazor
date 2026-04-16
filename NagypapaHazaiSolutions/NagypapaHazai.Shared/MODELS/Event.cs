using NagypapaHazaiBlazor.MODELS;

namespace NagypapaHazai.Shared.MODELS
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public DateOnly? StartDate { get; set; }    // ← date típus miatt DateOnly
        public DateOnly? EndDate { get; set; }      // ← date típus miatt DateOnly
        public int? MaxParticipants { get; set; }   // ← nullable, mert DB-ben null
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;
        public List<EventRegistration> Registrations { get; set; } = new();
    }
}
