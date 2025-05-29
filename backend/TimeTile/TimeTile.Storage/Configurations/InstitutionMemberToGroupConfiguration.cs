using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Configurations
{
    internal class InstitutionMemberToGroupConfiguration : IEntityTypeConfiguration<InstitutionMemberToGroup>
    {
        public void Configure(EntityTypeBuilder<InstitutionMemberToGroup> builder)
        {
            // Map to table and create index
            builder.ToTable("institution_members_groups");

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => new { e.InstitutionMemberId, e.GroupId, e.DeletedAt })
                .HasDatabaseName("institution_members_groups_institution_member_group_deleted_at_key")
                .AreNullsDistinct(false)
                .IsUnique();

            // Define properties with column names
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            builder.Property(e => e.InstitutionMemberId)
                .HasColumnName("institution_member_id");
            builder.Property(e => e.GroupId)
                .HasColumnName("group_id");

            // Define relationships
            builder.HasOne(d => d.InstitutionMember)
                .WithMany(p => p.InstitutionMemberToGroups)
                .HasForeignKey(d => d.InstitutionMemberId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("institution_members_groups_institution_member_id_fkey");

            builder.HasOne(d => d.Group)
                .WithMany(p => p.InstitutionMembersToGroup)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("institution_members_groups_group_id_fkey");
        }
    }
}
