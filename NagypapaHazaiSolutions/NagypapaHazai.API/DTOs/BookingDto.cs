namespace NagypapaHazai.API.DTOs
{
    public class BookingDto
    {
        public int PropertyId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ExchangeOffer { get; set; }
    }
}