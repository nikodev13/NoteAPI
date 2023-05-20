namespace NoteAPI.ValueObjects;

public record NoteTitle
{
    public string Value { get; }

    public NoteTitle(string value)
    {
        if (string.IsNullOrEmpty(value)) throw new ArgumentException(); 
        Value = value;
    }
    
    public static implicit operator NoteTitle(string title) => new(title);
    public static implicit operator string(NoteTitle title) => title.Value;
}