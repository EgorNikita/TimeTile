using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }
    }
}
