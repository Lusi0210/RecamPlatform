using System;

namespace Remp.Remp.Models;

public class ListingCase
{
    public int Id { get; set; }
    public string Address { get; set; }
    public string PropertyStatus { get; set; }
    public string PropertyType { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int Garages { get; set; }
    public double Area {get;set;}
    public decimal Price { get; set; }
    public string CurrentStatus { get; set; }
}
