using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAPI.Entities;
using NoteAPI.Persistence;
using NoteAPI.ReadModels;
using NoteAPI.Services;
using NoteAPI.Shared.Endpoints;
using NoteAPI.Shared.Validation;

namespace NoteAPI.Endpoints.Notes;

public class CreateNoteRequest : IRequest
{
    [FromBody]
    public CreateNoteRequestBody Body { get; init; }
    
    public class CreateNoteRequestBody
    {
        public required string Title { get; init; }
        public string? Content { get; init; }
    }
}

public class CreateNoteEndpoint : IEndpoint
{
    public void Configure(IEndpointRouteBuilder endpoint)
    {
        endpoint.MapPost<CreateNoteRequest, CreateNoteRequestHandler>("/api/notes")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict)
            .UseValidation<CreateNoteRequest>()
            .RequireAuthorization();
    }
}

public class CreateNoteRequestHandler : IRequestHandler<CreateNoteRequest>
{
    private readonly NoteDbContext _dbContext;
    private readonly IUserContextService _userContextService;

    public CreateNoteRequestHandler(NoteDbContext dbContext, IUserContextService userContextService)
    {
        _dbContext = dbContext;
        _userContextService = userContextService;
    }
    
    public async ValueTask<IResult> HandleAsync(CreateNoteRequest request, CancellationToken cancellationToken)
    {
        var userId = _userContextService.UserId;
        
        if (await _dbContext.Notes.AnyAsync(x => x.OwnerId == userId && x.Title == request.Body.Title, cancellationToken))
        {
            return Results.Conflict($"Note with title `{request.Body.Title}` already exists.");
        }

        var note = new Note()
        {
            Id = Guid.NewGuid(),
            Title = request.Body.Title,
            Content = request.Body.Content,
            OwnerId = userId!.Value,
            CreatedAt = DateTime.Now
        };

        await _dbContext.AddAsync(note, cancellationToken);

        return Results.Created($"/notes/{note.Id}", NoteReadModel.From(note));
    }
}

public class CreateNoteRequestValidator : AbstractValidator<CreateNoteRequest>
{
    public CreateNoteRequestValidator()
    {
        RuleFor(x => x.Body.Title)
            .NotEmpty();
    }
}