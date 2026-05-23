namespace EventManagementApi.DTOs.Ticket
{
    public class TicketGetDto
    {
        public int TicketId { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; } = null!;
        public string Type { get; set; } = null!;
        public decimal Price { get; set; }
        public int QuantityAvailable { get; set; }
    }
}
