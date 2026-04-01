using System;

namespace Remp.Remp.Models;

public class ListingCase
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public int PostCode { get; set; }
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }
    public double Price { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int Garages { get; set; }
    public DateTime CreatedAt { get; set; }
    public Boolean IsDeleted { get; set; }
    public Enum.PropertyType PropertyType { get; set; }
    public Enum.SaleCategory SaleCategory { get; set; }
    public Enum.ListcaseStatus ListcaseStatus { get; set; }
    public string UserId { get; set; }
}
