namespace NoteAPI.Shared.Endpoints;

public interface IQuery { }

public interface IQueryHandler<in TQuery> 
    where TQuery : IQuery
{
    ValueTask<IResult> HandleAsync(TQuery request, CancellationToken cancellationToken);
}

public interface ICommand { }

public interface ICommandHandler<in TCommand> 
    where TCommand : ICommand
{
    ValueTask<IResult> HandleAsync(TCommand request, CancellationToken cancellationToken);
}
