using LanguageExt;
using Shouldly;

namespace Portfolio.Api.Tests.Shared.Extensions;

public static class ShouldlyExtensions
{
    public static void ShouldBeUnit(this Unit actual)
    {
        actual.ShouldBe(Unit.Default);
    }
}