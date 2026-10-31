using System.Globalization;
using LanguageExt;
using Portfolio.Api.Shared.Domain.Errors;

namespace Portfolio.Api.Timeline.Shared.Domain.Models;

public class YearMonth
{
    private const string Format = "yyyy-MM";

    private readonly DateOnly _date;

    private YearMonth(DateOnly date)
    {
        _date = date;
    }

    public static Either<DomainError, YearMonth> Create(string yearMonth)
    {
        if (string.IsNullOrWhiteSpace(yearMonth))
        {
            return new ValidationError("Date cannot be empty");
        }

        if (!DateOnly.TryParseExact(yearMonth.Trim(), Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return new ValidationError($"Date must have the format {Format}");
        }

        return new YearMonth(date);
    }

    public static Either<DomainError, YearMonth> CreateUnsafe(DateOnly date) =>
        new YearMonth(date);

    public DateOnly ToDateOnly() => _date;

    public override string ToString() => _date.ToString(Format, CultureInfo.InvariantCulture);
}
