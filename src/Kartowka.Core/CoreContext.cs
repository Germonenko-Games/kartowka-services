using Kartowka.Core.Models;
using Kartowka.Core.Models.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Kartowka.Core;

public class CoreContext : DbContext
{
    public CoreContext(DbContextOptions<CoreContext> options) : base(options)
    {
    }

    public DbSet<Asset> Assets => Set<Asset>();

    public DbSet<Pack> Packs => Set<Pack>();

    public DbSet<Question> Questions => Set<Question>();

    public DbSet<QuestionsCategory> QuestionsCategories => Set<QuestionsCategory>();

    public DbSet<Round> Rounds => Set<Round>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AssetEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new PackEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new QuestionEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new QuestionsCategoryEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RoundEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
    }
}