using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kartowka.Core.Models.Configuration;

internal class QuestionsCategoryEntityTypeConfiguration : IEntityTypeConfiguration<QuestionsCategory>
{
    public void Configure(EntityTypeBuilder<QuestionsCategory> builder)
    {
        builder.HasKey(category => category.Id);

        builder.Property<long>("RoundId");

        builder
            .HasMany(category => category.Questions)
            .WithOne()
            .HasForeignKey("QuestionCategoryId");
    }
}
