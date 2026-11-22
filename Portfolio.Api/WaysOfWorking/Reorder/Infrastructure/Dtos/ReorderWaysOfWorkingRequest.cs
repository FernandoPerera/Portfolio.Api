namespace Portfolio.Api.WaysOfWorking.Reorder.Infrastructure.Dtos;

public record ReorderWaysOfWorkingRequest(
    List<Guid> Ids
);
