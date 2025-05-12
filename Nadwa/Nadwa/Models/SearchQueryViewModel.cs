namespace Nadwa.Models;

public class SearchQueryViewModel
{
    public string? Query { get; set; }
    public decimal MinPrice { get; set; } = decimal.Zero;
    public decimal MaxPrice { get; set; } = decimal.MaxValue;
    public DateTime? FromDate { get; set; } = DateTime.Now;
    public DateTime? ToDate { get; set; } = DateTime.Now.AddDays(365);
    public int Page { get; set; } = 1;
}
