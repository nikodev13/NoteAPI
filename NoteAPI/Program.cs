using Microsoft.EntityFrameworkCore;
using NoteAPI.Authentication;
using NoteAPI.Authorization;
using NoteAPI.Persistence;
using NoteAPI.Shared.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<NoteDbContext>(
        options => options.UseSqlServer(builder.Configuration.GetConnectionString("NoteDbConnectionString")))
        
    .ConfigureAuthentication(builder.Configuration)
    .ConfigureAuthorization()
    
    .RegisterEndpointsHandlers()
        
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger().UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.RegisterEndpoints();

app.Run();