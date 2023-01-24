using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoteAPI.Entities;
using NoteAPI.ValueObjects;

namespace NoteAPI.Persistence.Configurations;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId)
            .HasConversion(x => x.Value, x => new UserId(x));

        builder.HasIndex(x => x.Email).IsUnique();
    }
}