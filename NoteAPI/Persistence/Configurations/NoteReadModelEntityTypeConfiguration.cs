using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoteAPI.ReadModels;

namespace NoteAPI.Persistence.Configurations;

// public class NoteReadModelEntityTypeConfiguration : IEntityTypeConfiguration<NoteReadModel>
// {
//     public void Configure(EntityTypeBuilder<NoteReadModel> builder)
//     {
//         builder.ToView("v_NotesReadModels");
//         builder.HasNoKey();
//     }
// }