using Portfolio.Api.Auth.Shared.Infrastructure.Extensions;
using Portfolio.Api.Languages.Create.Infrastructure.Extensions;
using Portfolio.Api.Languages.Find.Infrastructure.Extensions;
using Portfolio.Api.Languages.Update.Infrastructure.Extensions;
using Portfolio.Api.Profiles.Create.Infrastructure.Extensions;
using Portfolio.Api.Profiles.Find.Infrastructure.Extensions;
using Portfolio.Api.Profiles.Update.Infrastructure.Extensions;
using Portfolio.Api.Shared.Infrastructure.Extensions;
using Portfolio.Api.Timeline.Create.Infrastructure.Extensions;
using Portfolio.Api.Timeline.Find.Infrastructure.Extensions;
using Portfolio.Api.Timeline.Update.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

// Shared dependencies
builder.Services.AddSharedInfrastructure();
builder.Services.AddAuthentication(builder.Configuration);

// Actions
builder.Services.AddLanguageCreate();
builder.Services.AddLanguageFind();
builder.Services.AddLanguageUpdate();
builder.Services.AddProfileCreate();
builder.Services.AddProfileFind();
builder.Services.AddProfileUpdate();
builder.Services.AddTimelineCreate();
builder.Services.AddTimelineFind();
builder.Services.AddTimelineUpdate();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
