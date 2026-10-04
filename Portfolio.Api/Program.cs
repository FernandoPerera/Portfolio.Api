using Portfolio.Api.Auth.Login.Infrastructure.Extensions;
using Portfolio.Api.Auth.Refresh.Infrastructure.Extensions;
using Portfolio.Api.Auth.Shared.Infrastructure.Extensions;
using Portfolio.Api.Languages.Create.Infrastructure.Extensions;
using Portfolio.Api.Profiles.Create.Infrastructure.Extensions;
using Portfolio.Api.Profiles.Find.Infrastructure.Extensions;
using Portfolio.Api.Shared.Infrastructure.Extensions;
using Portfolio.Api.Timeline.Create.Infrastructure.Extensions;
using Portfolio.Api.Timeline.Find.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

// Shared dependencies
builder.Services.AddSharedInfrastructure();
builder.Services.AddAuthSharedInfrastructure();

// Actions
builder.Services.AddAuthLogin();
builder.Services.AddAuthRefresh();
builder.Services.AddLanguageCreate();
builder.Services.AddProfileCreate();
builder.Services.AddProfileFind();
builder.Services.AddTimelineCreate();
builder.Services.AddTimelineFind();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();