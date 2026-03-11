namespace NagypapaHazaiBlazor.MODELS
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";


        public string Role { get; set; } = "User"; 

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public List<EventRegistration> EventRegistrations { get; set; } = new();
    }
}


