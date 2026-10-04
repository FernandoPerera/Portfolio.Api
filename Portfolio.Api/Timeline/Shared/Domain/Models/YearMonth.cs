using System.Globalization;
using LanguageExt;
using LanguageExt.Common;
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

    public static Either<Error, YearMonth> Create(string yearMonth)
    {
        if (string.IsNullOrWhiteSpace(yearMonth))
        {
            return Error.New(new ValidationException("Date cannot be empty"));
        }

        if (!DateOnly.TryParseExact(yearMonth.Trim(), Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
        {
            return Error.New(new ValidationException($"Date must have the format {Format}"));
        }

        return new YearMonth(date);
    }

    public static Either<Error, YearMonth> CreateUnsafe(DateOnly date) =>
        new YearMonth(date);

    public DateOnly ToDateOnly() => _date;

    public override string ToString() => _date.ToString(Format, CultureInfo.InvariantCulture);
}
