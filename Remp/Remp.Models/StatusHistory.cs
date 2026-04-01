using System;

namespace Remp.Remp.Models;

public class StatusHistory
{
    public int Id { get; set; }
    public int ListingCaseId { get; set; }
    public string OldStatus { get; set; }
    public string NewStatus { get; set; }
    public DateTime ChangedAt { get; set; }

}
