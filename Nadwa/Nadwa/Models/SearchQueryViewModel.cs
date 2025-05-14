namespace Nadwa.Models;

public class SearchQueryViewModel
{
    public string? Query { get; set; }
    public decimal MinPrice { get; set; } = decimal.Zero;
    public decimal MaxPrice { get; set; } = 1000;
    public DateTime? FromDate { get; set; } = DateTime.UtcNow;
    public DateTime? ToDate { get; set; } = DateTime.UtcNow.AddDays(365);
    public int Page { get; set; } = 1;
}
