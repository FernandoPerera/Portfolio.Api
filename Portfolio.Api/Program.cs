using Portfolio.Api.Profiles.Create.Infrastructure.Extensions;
using Portfolio.Api.Shared.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Shared dependencies
builder.Services.AddSharedInfrastructure();

// Actions
builder.Services.AddProfileCreate();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();