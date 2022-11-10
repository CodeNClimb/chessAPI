using ChessAPI.Data;
using ChessAPI.Handler;
using ChessAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ChessAPIDBContext>(options => options.UseSqlite(builder.Configuration["ChessAPIConnection"]));
builder.Services.AddScoped<IChessAPIRepo, ChessAPIRepo>();

builder.Services
    .AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, ChessAPIAuthHandler>("ChessAPIAuthentication", null);
var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();

app.MapControllers();

app.UseAuthentication();

app.UseAuthorization();

app.Run();
