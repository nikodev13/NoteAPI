using Microsoft.EntityFrameworkCore;
using NoteAPI.Persistence;
using NoteAPI.Shared.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<NoteDbContext>(
        options => options.UseSqlServer(builder.Configuration.GetConnectionString("NoteDbConnectionString")))
        
    .RegisterEndpointsHandlers()
        
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger().UseSwaggerUI();
}

app.RegisterEndpoints();

app.Run();