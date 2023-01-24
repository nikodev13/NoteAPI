namespace NoteAPI.ReadModels;

public class NoteReadModel
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
}