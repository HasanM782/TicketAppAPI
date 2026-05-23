namespace EventManagementApi.DTOs.Ticket
{
    public class CreateTicketDto
    {
        public string Type { get; set; } = null!;
        public decimal Price { get; set; }
        public int QuantityAvailable { get; set; }
    }
}
