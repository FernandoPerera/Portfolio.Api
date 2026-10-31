using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Api.Shared.Infrastructure.Adapters;

namespace Portfolio.Api.Tests.Shared.Infrastructure.Builders;

public abstract class ProblemDetailsMapperBuilder
{
    public static ProblemDetailsMapper Build(ILogger<ProblemDetailsMapper>? logger = null)
    {
        return new ProblemDetailsMapper(logger ?? NullLogger<ProblemDetailsMapper>.Instance);
    }
}
