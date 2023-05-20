using FluentValidation;
using NoteAPI.ValueObjects;

namespace NoteAPI.Entities;

public class Note
{
    public NoteId Id { get; }

    public NoteTitle Title { get; private set; }
    public string? Content { get; private set; }

    public UserId OwnerId { get; }
    
    public DateTime CreatedAt { get; }
    public DateTime? LastUpdatedAt { get; set; }

    public Note(NoteTitle title, string? content, UserId ownerId)
    {
        Id = Guid.NewGuid();
        Title = title;
        Content = content;
        OwnerId = ownerId;
        CreatedAt = DateTime.Now;
    }

    public void UpdateTitle(NoteTitle title)
    {
        Title = title;
        LastUpdatedAt = DateTime.Now;
    }
    
    public void UpdateContent(string? content)
    {
        Content = content;
        LastUpdatedAt = DateTime.Now;
    }
}