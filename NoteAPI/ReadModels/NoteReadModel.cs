using NoteAPI.Entities;

namespace NoteAPI.ReadModels;

public class NoteReadModel
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public string? Content { get; init; }

    public static NoteReadModel From(Note note) 
        => new()
        {
            Id = note.Id.Value,
            Title = note.Title,
            Content = note.Content
        };
}