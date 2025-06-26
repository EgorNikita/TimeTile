using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            // Table and Key Configuration
            builder.ToTable("grades", t =>
            {
                t.HasCheckConstraint("CHK_Grade_Value_Positive", "\"value\" > 0");
                t.HasCheckConstraint("CHK_Grade_Weight_Positive", "\"weight\" > 0");

                string[] types = Enum.GetNames(typeof(GradeType))
                    .Select(e => '\'' + e.ToLower() + '\'')
                    .ToArray();

                string typesString = string.Join(", ", types);

                t.HasCheckConstraint(
                    "CHK_Grade_Type_Valid",
                    $"LOWER(\"type\") IN ({typesString})"
                );
            });

            builder.HasKey(e => e.Id);

            // Property Configurations
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.Value)
                .HasColumnName("value");

            builder.Property(e => e.Weight)
                .HasColumnName("weight")
                .HasDefaultValue((float)1.0);

            builder.Property(e => e.Type)
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<GradeType>(v, true)
                )
                .HasColumnName("type")
                .HasConversion<string>();
        }
    }
}
