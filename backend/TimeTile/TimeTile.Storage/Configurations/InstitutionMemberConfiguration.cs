using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class InstitutionMemberConfiguration : IEntityTypeConfiguration<InstitutionMember>
    {
        public void Configure(EntityTypeBuilder<InstitutionMember> builder)
        {
            // Table Configuration
            builder.ToTable("institution_members", t =>
                t.HasCheckConstraint(
                    "CK_InstitutionMember_WeekWorkHours_Positive",
                    "\"week_work_hours\" > 0"
                )
            );

            // Define properties with column names
            builder.Property(e => e.WeekWorkHours)
                .HasColumnName("week_work_hours");

            builder.Property(e => e.PreferredClassroomId)
                .HasColumnName("preferred_classroom_id")
                .IsRequired(false);

            // Define relationships
            builder.HasOne(d => d.Classroom)
                .WithMany(p => p.InstitutionMembers)
                .HasForeignKey(d => d.PreferredClassroomId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("institution_members_preferred_classroom_id_fkey");
        }
    }
}
