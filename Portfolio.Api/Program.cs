using Portfolio.Api.Auth.Login.Infrastructure.Extensions;
using Portfolio.Api.Auth.Logout.Infrastructure.Extensions;
using Portfolio.Api.Auth.Refresh.Infrastructure.Extensions;
using Portfolio.Api.Auth.Shared.Infrastructure.Extensions;
using Portfolio.Api.Languages.Create.Infrastructure.Extensions;
using Portfolio.Api.Languages.Delete.Infrastructure.Extensions;
using Portfolio.Api.Languages.Find.Infrastructure.Extensions;
using Portfolio.Api.Languages.Update.Infrastructure.Extensions;
using Portfolio.Api.Profiles.Create.Infrastructure.Extensions;
using Portfolio.Api.Profiles.Find.Infrastructure.Extensions;
using Portfolio.Api.Profiles.Update.Infrastructure.Extensions;
using Portfolio.Api.Shared.Infrastructure.Extensions;
using Portfolio.Api.Timeline.AddTranslation.Infrastructure.Extensions;
using Portfolio.Api.Timeline.Create.Infrastructure.Extensions;
using Portfolio.Api.Timeline.Delete.Infrastructure.Extensions;
using Portfolio.Api.Timeline.Find.Infrastructure.Extensions;
using Portfolio.Api.Timeline.Update.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApiDocumentation();
builder.Services.AddHealthChecks();

// Shared dependencies
builder.Services.AddSharedInfrastructure();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAdminAuthorization(builder.Configuration);
builder.Services.AddAuthShared(builder.Configuration);
builder.Services.AddRateLimiting(builder.Configuration);
builder.Services.AddCorsPolicy(builder.Configuration);

// Auth
builder.Services.AddGitHubLoginStart();
builder.Services.AddGitHubAdminAuthentication();
builder.Services.AddLoginCodeExchange();
builder.Services.AddTokenRefresh();
builder.Services.AddSessionClose();

// Language
builder.Services.AddLanguageCreate();
builder.Services.AddLanguageDelete();
builder.Services.AddLanguageFind();
builder.Services.AddLanguageUpdate();

// Profile
builder.Services.AddProfileCreate();
builder.Services.AddProfileFind();
builder.Services.AddProfileUpdate();

// Timeline
builder.Services.AddTimelineCreate();
builder.Services.AddTimelineTranslationAdd();
builder.Services.AddTimelineDelete();
builder.Services.AddTimelineFind();
builder.Services.AddTimelineUpdate();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapOpenApiDocumentation();
app.MapHealthChecks("/health").AllowAnonymous();

app.Run();

public partial class Program;
