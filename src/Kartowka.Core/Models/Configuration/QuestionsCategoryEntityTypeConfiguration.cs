using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kartowka.Core.Models.Configuration;

public class QuestionsCategoryEntityTypeConfiguration : IEntityTypeConfiguration<QuestionsCategory>
{
    public void Configure(EntityTypeBuilder<QuestionsCategory> builder)
    {
        builder.HasKey(category => category.Id);

        builder.Property<long>("PackId");

        builder.HasMany<Question>()
            .WithOne()
            .HasForeignKey(question => question.QuestionsCategoryId);
    }
}