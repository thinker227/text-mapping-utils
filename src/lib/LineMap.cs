using System.Collections;

namespace TextMappingUtils;

/// <summary>
/// A mapping between lines and character positions of a text.
/// </summary>
public sealed class LineMap : IReadOnlyList<Line>
{
    private readonly List<Line> lines;

    /// <summary>
    /// The total amount of lines in the map.
    /// </summary>
    public int LineCount => lines.Count;

    /// <summary>
    /// The size of the mapped text.
    /// </summary>
    public int Size => lines[^1].Span.End;

    int IReadOnlyCollection<Line>.Count => LineCount;

    Line IReadOnlyList<Line>.this[int index] =>
        GetLine(index);

    private LineMap(List<Line> lines) =>
        this.lines = lines;

    /// <summary>
    /// Creates a new line map.
    /// </summary>
    /// <param name="str">The string of characters to create the map from.</param>
    public static LineMap Create(ReadOnlySpan<char> str)
    {
        var lines = new List<Line>();

        var lineNumber = 0;
        var lineStart = 0;
        var lineEnd = 0;

        for (var i = 0; i < str.Length; i++)
        {
            lineEnd++;

            if (str[i] is not '\n') continue;

            lines.Add(new(lineNumber, new() { Start = lineStart, End = lineEnd }));

            lineNumber++;
            lineStart = i + 1;
            lineEnd = lineStart;
        }

        lines.Add(new(lineNumber, new() { Start = lineStart, End = lineEnd }));

        return new(lines);
    }

    /// <summary>
    /// Gets the line with a specified line number.
    /// </summary>
    /// <param name="lineNumber">The 0-indexed line number of the line to get.</param>
    public Line GetLine(int lineNumber)
    {
        if (lineNumber < 0)
            throw new ArgumentOutOfRangeException(nameof(lineNumber),
                "Line number cannot be less than or equal to 0.");
        if (lineNumber >= LineCount)
            throw new ArgumentOutOfRangeException(nameof(lineNumber),
                $"Line number cannot be greater than the amount of lines the line map was constructed from ({LineCount}).");

        return lines[lineNumber];
    }

    /// <summary>
    /// Gets the character position at a specified position from the start of the text.
    /// </summary>
    /// <param name="position">The position in the text to get the character position at.</param>
    public CharacterPosition GetCharacterPosition(int position)
    {
        if (position < 0)
            throw new ArgumentOutOfRangeException(nameof(position),
                "Character position cannot be negative.");
        if (position >= Size + 1)
            throw new ArgumentOutOfRangeException(nameof(position),
                $"Character position must be less than or equal to the size of the string the line map was constructed from {Size}.");

        var startLine = 0;
        var endLine = LineCount;

        while (true)
        {
            Line line;

            if (startLine >= endLine)
            {
                // If execution gets here then that means we're past the last line
                // and want to get the final character of the last line.
                line = lines[^1];
                return new(line, line.Span.Length);
            }

            var spanSize = endLine - startLine;
            var index = startLine + spanSize / 2;
            line = lines[index];

            if (position >= line.Span.Start && position < line.Span.End)
            {
                return new(line, position - line.Span.Start);
            }

            if (position < line.Span.Start) endLine = index;
            else startLine = index + 1;
        }
    }

    /// <summary>
    /// Enumerates the lines of the map.
    /// </summary>
    public IEnumerator<Line> GetEnumerator() => lines.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// A representation of a single line of text within a larger text.
/// </summary>
/// <param name="LineNumber">The 0-indexed line number of the line.</param>
/// <param name="Span">The span of the line within the larger text.</param>
public readonly record struct Line(int LineNumber, TextSpan Span);

/// <summary>
/// A character position in a text.
/// </summary>
/// <param name="Line">The line of the character.</param>
/// <param name="Offset">The character's offset from the start of the line.</param>
public readonly record struct CharacterPosition(Line Line, int Offset)
{
    /// <summary>
    /// The absolute position of the character within the larger text.
    /// </summary>
    public int AbsolutePosition => Line.Span.Start + Offset;
}
