using System.Collections;
using System.Diagnostics;

namespace TextMappingUtils;

/// <summary>
/// A mapping between lines and character positions of a text.
/// A <see cref="LineMap"/> does not contain any text, it merely contains
/// numeric representations of lines within a piece of text.
/// The principal method of constructing a <see cref="LineMap"/> is <see cref="LineMap.Create"/>.
/// </summary>
/// <seealso cref="Create"/>
/// <seealso cref="GetCharacterPosition"/>
/// <seealso cref="GetLine"/>
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
    /// Creates a new line map using <c>\n</c> as the line delimiter.
    /// </summary>
    /// <param name="str">The span of characters to create the map from.</param>
    /// <remarks>
    /// The created <see cref="LineMap"/> always contains at least one line.
    /// If <paramref name="str"/> is empty, the map will contain a single empty line.
    /// </remarks>
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
    /// Gets the line with a specified line number. Runs in <c>O(1)</c>.
    /// </summary>
    /// <param name="lineNumber">The 0-indexed line number of the line to get.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="lineNumber"/> is less than to 0 or greater or equal to than <see cref="LineCount"/>.
    /// </exception>
    public Line GetLine(int lineNumber)
    {
        if (lineNumber < 0)
            throw new ArgumentOutOfRangeException(nameof(lineNumber),
                "Line number cannot be less than 0.");
        if (lineNumber >= LineCount)
            throw new ArgumentOutOfRangeException(nameof(lineNumber),
                $"Line number cannot be greater than the amount of lines the line map was constructed from ({LineCount}).");

        return lines[lineNumber];
    }

    /// <summary>
    /// Gets the character position at a specified position from the start of the text.
    /// Runs in at most <c>O(log n)</c> where <c>n</c> is <see cref="LineCount"/>.
    /// </summary>
    /// <param name="position">The position in the text to get the character position at.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="position"/> is less than 0 or greater than <see cref="Size"/>.
    /// </exception>
    /// <remarks>
    /// If <paramref name="position"/> is equal to <see cref="Size"/>,
    /// the returned character position will be the final character of the last line in the map.
    /// </remarks>
    public CharacterPosition GetCharacterPosition(int position)
    {
        if (position < 0)
            throw new ArgumentOutOfRangeException(nameof(position),
                "Character position cannot be less than 0.");
        if (position > Size)
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
/// A single line of text within a larger text.
/// </summary>
/// <param name="LineNumber">The 0-indexed line number of the line.</param>
/// <param name="Span">The span of the line within the larger text.</param>
[DebuggerDisplay("Line {LineNumber} ({Span})")]
public readonly record struct Line(int LineNumber, TextSpan Span);

/// <summary>
/// A character position in a text.
/// </summary>
/// <param name="Line">The line of the character.</param>
/// <param name="Offset">The character's offset from the start of the line.</param>
[DebuggerDisplay("{Line.LineNumber}:{Offset}")]
public readonly record struct CharacterPosition(Line Line, int Offset)
{
    /// <summary>
    /// The absolute position of the character within the larger text.
    /// </summary>
    public int AbsolutePosition => Line.Span.Start + Offset;
}
