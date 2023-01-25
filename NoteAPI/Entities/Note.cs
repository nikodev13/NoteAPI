using NoteAPI.ValueObjects;

namespace NoteAPI.Entities;

public class Note
{
    public required NoteId Id { get; init; }
    public required string Title { get; set; }
    public string? Content { get; set; }

    public required UserId OwnerId { get; init; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}