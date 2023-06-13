using Microsoft.EntityFrameworkCore;

namespace NoteAPI.IntegrationTests.Helpers;

public class SqliteNoteDbContext : Persistence.NoteDbContext
{
    public SqliteNoteDbContext(DbContextOptions<Persistence.NoteDbContext> options) : base(options)
    {
        base.Database.EnsureCreated();
        base.Database.Migrate();
    }
}