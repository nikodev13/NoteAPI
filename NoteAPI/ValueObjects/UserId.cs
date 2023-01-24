﻿namespace NoteAPI.ValueObjects;

public class UserId
{
    public Guid Value { get; }

    public UserId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Invalid note id.");
        Value = value;
    }

    public static implicit operator UserId(Guid id) => new(id);
    public static implicit operator Guid(UserId id) => id.Value;
}