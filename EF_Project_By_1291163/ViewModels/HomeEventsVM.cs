public class HomeEventsVM
{
    public int EventId { get; set; }
    public string? EventName { get; set; }
    public decimal TicketPrice { get; set; }
    public int TotalAttendees { get; set; } 
    public int TotalTicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
}
