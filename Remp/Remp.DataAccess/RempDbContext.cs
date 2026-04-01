using System;
using Microsoft.EntityFrameworkCore;
using Remp.Remp.Models;

namespace Remp.Remp.DataAccess;

public class RempDbContext : DbContext
{
    public RempDbContext(DbContextOptions<RempDbContext> options) : base(options)
    {
    }

    public DbSet<Agent> Agents { get; set; }
    public DbSet<AgentListingCase> AgentListingCases { get; set; }
    public DbSet<AgentPhotographyCompany> AgentPhotographyCompanies { get; set; }
    public DbSet<CaseContact> CaseContacts { get; set; }
    public DbSet<ListingCase> ListingCases { get; set; }
    public DbSet<MediaAsset> MediaAssets { get; set; }
    public DbSet<PhotographyCompany> PhotographyCompanies { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agent>().HasKey(a => a.Id);
        modelBuilder.Entity<AgentListingCase>().HasKey(al => new { al.AgentId, al.ListingCaseId });
        modelBuilder.Entity<AgentPhotographyCompany>().HasKey(ap => new { ap.AgentId, ap.PhotographyCompanyId });
        modelBuilder.Entity<CaseContact>().HasKey(c => c.ContactId);
        modelBuilder.Entity<ListingCase>().HasKey(l => l.Id);
        modelBuilder.Entity<MediaAsset>().HasKey(m => m.Id);
        modelBuilder.Entity<PhotographyCompany>().HasKey(p => p.Id);
        modelBuilder.Entity<ListingCase>()
            .Property(x => x.Latitude)
            .HasPrecision(9, 6);
        modelBuilder.Entity<ListingCase>()
            .Property(x => x.Longitude)
            .HasPrecision(9, 6);
        base.OnModelCreating(modelBuilder);
    }
}
