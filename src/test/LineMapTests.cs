using Shouldly;

namespace TextMappingUtils.Test;

public class LineMapTests
{
    // Using manual newlines here instead of a raw string to ensure that the newlines stay as just \n.
    private const string Text = "all\nbabel\ncs";

    public static TheoryData<int, Line> GetLine_ReturnsLine_InRange_Data()
    {
        var data = new TheoryData<int, Line>();

        Add(lineNumber: 0, lineStart: 0, lineEnd: 4);
        Add(lineNumber: 1, lineStart: 4, lineEnd: 10);
        Add(lineNumber: 2, lineStart: 10, lineEnd: 12);

        return data;

        void Add(int lineNumber, int lineStart, int lineEnd) =>
            data.Add(lineNumber, new(lineNumber, new(lineStart, lineEnd)));
    }

    [Theory]
    [MemberData(nameof(GetLine_ReturnsLine_InRange_Data))]
    public void GetLine_ReturnsLine_InRage(int lineNumber, Line expected)
    {
        var map = LineMap.Create(Text);

        var line = map.GetLine(lineNumber);

        line.ShouldBe(expected);
    }

    public static TheoryData<int> GetLine_ThrowsArgumentOutOfRangeException_OutOfRange_Data() =>
    [
        -1,
        4,
        5
    ];

    [Theory]
    [MemberData(nameof(GetLine_ThrowsArgumentOutOfRangeException_OutOfRange_Data))]
    public void GetLine_ThrowsArgumentOutOfRangeException_OutOfRange(int lineNumber)
    {
        var map = LineMap.Create(Text);

        Should.Throw<ArgumentOutOfRangeException>(() => map.GetLine(lineNumber));
    }

    public static TheoryData<int, CharacterPosition> GetCharacterPosition_ReturnsCharacterPosition_InRange_Data()
    {
        var data = new TheoryData<int, CharacterPosition>();

        Add(
            position: 0,
            lineNumber: 0,
            lineStart: 0,
            lineEnd: 4,
            characterOffset: 0);

        Add(
            position: 3,
            lineNumber: 0,
            lineStart: 0,
            lineEnd: 4,
            characterOffset: 3);

        Add(
            position: 4,
            lineNumber: 1,
            lineStart: 4,
            lineEnd: 10,
            characterOffset: 0);

        Add(
            position: 6,
            lineNumber: 1,
            lineStart: 4,
            lineEnd: 10,
            characterOffset: 2);

        Add(
            position: 11,
            lineNumber: 2,
            lineStart: 10,
            lineEnd: 12,
            characterOffset: 1);

        Add(
            position: 12,
            lineNumber: 2,
            lineStart: 10,
            lineEnd: 12,
            characterOffset: 2);

        return data;

        void Add(int position, int lineNumber, int lineStart, int lineEnd, int characterOffset) =>
            data.Add(position, new(new(lineNumber, new(lineStart, lineEnd)), characterOffset));
    }

    [Theory]
    [MemberData(nameof(GetCharacterPosition_ReturnsCharacterPosition_InRange_Data))]
    public void GetCharacterPosition_ReturnsCharacterPosition_InRange(
        int position,
        CharacterPosition expected)
    {
        var map = LineMap.Create(Text);

        var characterPosition = map.GetCharacterPosition(position);

        characterPosition.ShouldBe(expected);
    }

    public static TheoryData<int> GetCharacterPosition_ThrowsArgumentOutOfRangeException_OutOfRange_Data() =>
    [
        -1,
        -2,
        13,
        14
    ];

    [Theory]
    [MemberData(nameof(GetCharacterPosition_ThrowsArgumentOutOfRangeException_OutOfRange_Data))]
    public void GetCharacterPosition_ThrowsArgumentOutOfRangeException_OutOfRange(int position)
    {
        var map = LineMap.Create(Text);

        Should.Throw<ArgumentOutOfRangeException>(() => map.GetCharacterPosition(position));
    }

    [Fact]
    public void LineCount_ReturnsLineCount()
    {
        var map = LineMap.Create("\n\n\n\n");

        map.LineCount.ShouldBe(5);
    }

    [Fact]
    public void Size_ReturnsCharacterCount()
    {
        var map = LineMap.Create(Text);

        map.Size.ShouldBe(12);
    }

    [Fact]
    public void Enumerable_ReturnsLines()
    {
        var map = LineMap.Create(Text);

        map.ToArray().ShouldBe([
            new(0, new(0, 4)),
            new(1, new(4, 10)),
            new(2, new(10, 12))
        ]);
    }
}
