using Microsoft.AspNetCore.Mvc;

namespace Nadwa.Models;

public class SearchQueryViewModel
{
    public string? Query { get; set; } = String.Empty;
    public decimal MinPrice { get; set; } = decimal.Zero;
    public decimal MaxPrice { get; set; } = 100000;
    public DateTime? FromDate { get; set; } = DateTime.UtcNow;
    public DateTime? ToDate { get; set; } = DateTime.UtcNow.AddYears(5);
    public int Page { get; set; } = 1;

}
