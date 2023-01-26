using NoteAPI.Authentication;
using NoteAPI.Authorization;
using NoteAPI.Middlewares;
using NoteAPI.Persistence;
using NoteAPI.Shared.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .ConfigurePersistence(builder.Configuration)
        
    .ConfigureAuthentication(builder.Configuration)
    .ConfigureAuthorization()
    
    .RegisterEndpointsHandlers()
    
    .AddScoped<ExceptionHandlingMiddleware>()
    
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger().UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.RegisterEndpoints();

app.Run();