using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NoteAPI.Entities;
// using NoteAPI.ReadModels;

namespace NoteAPI.Persistence;

public class NoteDbContext : DbContext
{
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<User> Users => Set<User>();

    public NoteDbContext(DbContextOptions<NoteDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(NoteDbContext))!);
    }
}