using Microsoft.EntityFrameworkCore;

namespace NoteAPI.IntegrationTests.Helpers;

public class NoteDbContextSQLLite : Persistence.NoteDbContext
{
    public NoteDbContextSQLLite(DbContextOptions<Persistence.NoteDbContext> options) : base(options)
    {
        Database.EnsureCreated();
        Database.Migrate();
    }
}