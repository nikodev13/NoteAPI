using FluentValidation;
using NoteAPI.Authentication;
using NoteAPI.Authorization;
using NoteAPI.Middlewares;
using NoteAPI.Persistence;
using NoteAPI.Services;
using NoteAPI.Shared.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .ConfigurePersistence(builder.Configuration)

    .RegisterAppServices()

    .ConfigureAuthentication(builder.Configuration)
    .ConfigureAuthorization()

    .RegisterEndpointsHandlers()
    
    .AddScoped<ExceptionHandlingMiddleware>()
    
    .AddValidatorsFromAssembly(typeof(Program).Assembly)
    
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger().UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.RegisterEndpoints();

app.Run();

public partial class Program { }
