using LanguageExt;
using LanguageExt.Common;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Shared.Domain.Models;

public class TimelineId
{
    private readonly Guid _id;

    private TimelineId(Guid id)
    {
        _id = id;
    }

    public static Either<Error, TimelineId> Create(Guid id)
    {
        if (id == Guid.Empty)
        {
            return Error.New(new ValidationException("Timeline id cannot be empty"));
        }

        return new TimelineId(id);
    }

    public Guid ToGuid() => _id;
}
