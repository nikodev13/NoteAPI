using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NoteAPI.Entities;
using NoteAPI.Persistence;
using NoteAPI.ReadModels;
using NoteAPI.Services;
using NoteAPI.Shared.Endpoints;
using NoteAPI.Shared.Validation;

namespace NoteAPI.Endpoints.Notes;

public record CreateNoteRequest(CreateNoteRequest.CreateNoteRequestBody Body) : IRequest
{
    public record CreateNoteRequestBody(string Title, string? Content = null);
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
        var userId = _userContextService.UserId!;
        var (title, content) = request.Body;

        var noteTitleAlreadyExists = await _dbContext.Notes
            .Where(x => x.OwnerId.Equals(userId) && x.Title == title)
            .AnyAsync(cancellationToken);
        
        if (noteTitleAlreadyExists) 
            return Results.Conflict($"Note with title `{title}` already exists.");

        var note = new Note(title, content, userId);

        await _dbContext.AddAsync(note, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

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