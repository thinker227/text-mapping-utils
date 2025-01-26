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
