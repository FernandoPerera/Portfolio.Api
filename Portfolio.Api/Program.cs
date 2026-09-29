using Portfolio.Api.Profiles.Create.Infrastructure.Extensions;
using Portfolio.Api.Profiles.Find.Infrastructure.Extensions;
using Portfolio.Api.Shared.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Shared dependencies
builder.Services.AddSharedInfrastructure();

// Actions
builder.Services.AddProfileCreate();
builder.Services.AddProfileFind();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();