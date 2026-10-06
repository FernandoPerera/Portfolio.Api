using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Api.Profiles.Find.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Update.Application.UseCases;
using Portfolio.Api.Profiles.Update.Infrastructure.Adapters;
using Portfolio.Api.Profiles.Update.Infrastructure.Controllers;
using Portfolio.Api.Shared.Infrastructure.Adapters;
using Portfolio.Api.Shared.Infrastructure.Data;
using Portfolio.Api.Tests.Profiles.Shared.Infrastructure.Builders;
using Portfolio.Api.Tests.Profiles.Update.Infrastructure.Builders;
using Shouldly;

namespace Portfolio.Api.Tests.Profiles.Update.Infrastructure.Controllers;

public class ProfileUpdaterControllerShould
{
    private readonly AppDbContext _appDbContext;
    private readonly ProfileUpdaterController _controller;

    public ProfileUpdaterControllerShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _appDbContext = new AppDbContext(options);
        var updaterRepository = new EntityFrameworkProfileUpdaterAdapter(_appDbContext);
        var finderRepository = new EntityFrameworkProfileFinderAdapter(_appDbContext);
        var updater = new ProfileUpdater(finderRepository, updaterRepository);
        var errorResponseMapper = new ProblemDetailsMapper();
        _controller = new ProfileUpdaterController(updater, errorResponseMapper);
    }

    [Fact]
    public async Task UpdateProfile()
    {
        var profileId = Guid.NewGuid();
        var translation = ProfileTranslationEntityBuilder.Build(profileId);
        await _appDbContext.Profiles.AddAsync(ProfileEntityBuilder.Build(profileId, translations: [translation]));
        await _appDbContext.SaveChangesAsync();
        var request = UpdateProfileRequestBuilder.Build();

        var response = await _controller.Update(translation.LanguageCode, request);

        response.ShouldBeOfType<NoContentResult>();
    }

    [Fact]
    public async Task FailWhenProfileDoesNotExist()
    {
        var request = UpdateProfileRequestBuilder.Build();

        var response = await _controller.Update("es", request);

        var objectResult = response.ShouldBeOfType<ObjectResult>();
        var problemDetails = objectResult.Value.ShouldBeOfType<ProblemDetails>();
        problemDetails.Status.ShouldBe(StatusCodes.Status404NotFound);
    }
}
