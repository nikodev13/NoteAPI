using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoteAPI.Entities;
using NoteAPI.ValueObjects;

namespace NoteAPI.Persistence.Configurations;

public class NoteEntityTypeConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new NoteId(x));

        builder.Property(x => x.OwnerId)
            .HasConversion(x => x.Value, x => new UserId(x));
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.OwnerId);

        builder.HasIndex(x => new { x.Id, x.Title }).IsUnique();
    }
}