﻿using Microsoft.EntityFrameworkCore;
using NoteAPI.Entities;
using NoteAPI.Persistence;
using NoteAPI.ValueObjects;

namespace NoteAPI.IntegrationTests.Tests.Extensions;

public static class NotesDbExtensions
{
    public static async ValueTask<Note> AddUniqueNoteToDb(this Testing testing, UserId ownerId)
    {
        var unique = new Note($"Unique note with random number: {Random.Shared.NextInt64()}", null, ownerId);
        await testing.AddEntities(unique);
        return unique;
    }

    public static async ValueTask DeleteNoteFromDb(this Testing testing, params NoteId[] notesIds)
    {
        await testing.DeleteEntities((Note x) => notesIds.Contains(x.Id));
    }
}