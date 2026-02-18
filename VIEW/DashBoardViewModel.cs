using NagypapaHazaiBlazor.MODELS;

namespace NagypapaHazaiBlazor.VIEW
{
    public class DashboardViewModel
    {
        public List<Property> Properties { get; set; } = [];
        public List<Event> Events { get; set; } = [];
        public List<Booking> Bookings { get; set; } = [];
    }

}
