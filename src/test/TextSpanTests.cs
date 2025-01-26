using Shouldly;

namespace TextMappingUtils.Test;

public class TextSpanTests
{
    [Fact]
    public void Ctor_SetsProperties_ToProvidedValues()
    {
        var span = new TextSpan(1, 5);
        span.Start.ShouldBe(1);
        span.End.ShouldBe(5);
    }

    [Fact]
    public void Ctor_SetsProperties_ToMinAndMax()
    {
        var span = new TextSpan(9, 6);
        span.Start.ShouldBe(6);
        span.End.ShouldBe(9);
    }

    [Fact]
    public void Ctor_SetsProperties_ToSame()
    {
        var span = new TextSpan(2, 2);
        span.Start.ShouldBe(2);
        span.End.ShouldBe(2);
    }

    [Fact]
    public void Ctor_ThrowsArgumentOutOfRange_ForStartLessThan0() =>
        Should.Throw<ArgumentOutOfRangeException>(() => new TextSpan(-1, 2));

    [Fact]
    public void Ctor_ThrowsArgumentOutOfRange_ForEndLessThan0() =>
        Should.Throw<ArgumentOutOfRangeException>(() => new TextSpan(2, -1));

    [Fact]
    public void Length_IsLength()
    {
        var span = new TextSpan(1, 5);
        span.Length.ShouldBe(4);
    }

    [Fact]
    public void IsEmpty_IsTrue_For0Length()
    {
        var span = new TextSpan(1, 1);
        span.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void IsEmpty_IsTrue_ForNegativeLength()
    {
        var span = new TextSpan() { Start = 5, End = 2 };
        span.IsEmpty.ShouldBeTrue();
    }

    [Fact]
    public void FromLength_CreatesFromLength()
    {
        var span = TextSpan.FromLength(1, 3);
        span.Start.ShouldBe(1);
        span.End.ShouldBe(4);
    }

    [Fact]
    public void Empty_IsEmpty()
    {
        var span = TextSpan.Empty(2);
        span.Start.ShouldBe(2);
        span.End.ShouldBe(2);
    }

    [Fact]
    public void ToRange_ConvertsToRange()
    {
        var span = new TextSpan(1, 3);
        var range = span.ToRange();
        range.ShouldBe(1..3);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Contains_ContainsPositions(int position)
    {
        var span = new TextSpan(2, 5);
        span.Contains(position).ShouldBeTrue();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(6)]
    public void Contains_DoesNotContainPoisitions(int position)
    {
        var span = new TextSpan(2, 5);
        span.Contains(position).ShouldBeFalse();
    }

    public static TheoryData<TextSpan, TextSpan> Between_CreatesBetween_Data() => new()
    {
        { new(2, 6), new(4, 8) },
        { new(4, 8), new(2, 6) },
        { new(2, 8), new(2, 5) },
        { new(2, 5), new(2, 8) },
        { new(2, 8), new(2, 8) },
        { new(2, 4), new(6, 8) },
        { new(6, 8), new(2, 4) },
    };

    [Theory]
    [MemberData(nameof(Between_CreatesBetween_Data))]
    public void Between_CreatesBetween(TextSpan a, TextSpan b)
    {
        var span = TextSpan.Between(a,b);
        span.Start.ShouldBe(2);
        span.End.ShouldBe(8);
    }

    [Fact]
    public void BetweenNullable_WithNotNull_CreatesBetween()
    {
        var span = TextSpan.Between(
            new(2, 4),
            (TextSpan?)new(6, 8));
        span.Start.ShouldBe(2);
        span.End.ShouldBe(8);
    }

    [Fact]
    public void BetweenNullable_WithNull_ReturnsParameter()
    {
        var x = new TextSpan(2, 8);
        var span = TextSpan.Between(x, null);
        span.ShouldBe(x);
    }

    [Fact]
    public void FromLength_ThrowsArgumentOutOfRange_ForStartLessThan0() =>
        Should.Throw<ArgumentOutOfRangeException>(() => TextSpan.FromLength(-1, 3));

    [Fact]
    public void FromLength_ThrowsArgumentOutOfRange_ForLengthLessThan0() =>
        Should.Throw<ArgumentOutOfRangeException>(() => TextSpan.FromLength(2, -5));

    [Fact]
    public void Equals_ChecksEquality()
    {
        var a = new TextSpan(1, 3);
        var b = new TextSpan(1, 3);
        a.Equals(b).ShouldBeTrue();
    }

    [Fact]
    public void ToString_FormatsCorrectly()
    {
        var span = new TextSpan(1, 5);
        span.ToString().ShouldBe("1..5");
    }
}
