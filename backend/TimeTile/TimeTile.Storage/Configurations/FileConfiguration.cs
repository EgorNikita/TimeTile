using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;
using File = TimeTile.Core.Models.File;

namespace TimeTile.Storage.Configurations
{
    internal class FileConfiguration : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> builder)
        {
            // Table Configuration
            builder.ToTable("files", t =>
            {
                t.HasCheckConstraint(
                    "CHK_File_Size_Valid",
                    "\"size\" > 0"
                );

                string[] extensions = Enum.GetNames(typeof(FileExtension))
                    .Select(e => '\'' + e.ToLower() + '\'')
                    .ToArray();

                string allExtensions = string.Join(", ", extensions);

                t.HasCheckConstraint(
                    "CHK_File_Extension_Valid",
                    $"\"extension\" IN ({allExtensions})"
                );
            });

            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.StoragePath, "files_storage_path_key")
                .IsUnique();

            // Define properties with column names
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(e => e.OriginalName)
                .HasColumnName("original_name")
                .HasMaxLength(255);

            builder.Property(e => e.Extension)
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<FileExtension>(v, true)
                )
                .HasColumnName("extension")
                .HasConversion<string>();

            builder.Property(e => e.Size)
                .HasColumnName("size");

            builder.Property(e => e.StoragePath)
                .HasColumnName("storage_path")
                .HasMaxLength(500);
        }
    }
}
