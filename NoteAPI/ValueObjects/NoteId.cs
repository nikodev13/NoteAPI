namespace NoteAPI.ValueObjects;

public record NoteId
{
    public Guid Value { get; }

    public NoteId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Invalid note id.");
        Value = value;
    }

    public static implicit operator NoteId(Guid id) => new(id);
    public static implicit operator Guid(NoteId id) => id.Value;
}