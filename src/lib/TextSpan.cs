namespace TextMappingUtils;

/// <summary>
/// Represents a span of characters within a text.
/// </summary>
/// <remarks>
/// It is recommended to construct new <see cref="TextSpan"/>s through the <see cref="TextSpan(int, int)"/> constructor.
/// While <see cref="Start"/> and <see cref="End"/> can both be initialized through the object initializer,
/// using the constructor will ensure the invariant that <see cref="Start"/> is strictly less than or equal to <see cref="End"/>
/// holds true. Otherwise, care should be taken to ensure this manually.
/// </remarks>
public readonly struct TextSpan : IEquatable<TextSpan>
{
    /// <summary>
    /// The <i>inclusive</i>, 0-indexed starting index of the span.
    /// </summary>
    /// <remarks>
    /// When using <see cref="TextSpan(int, int)"/>, this is ensured to be strictly
    /// less than or equal to <see cref="End"/> and greater than or equal to 0.
    /// When initializing this property manually, the same should be ensured by the user.
    /// </remarks>
    public int Start { get; init; }

    /// <summary>
    /// The <i>exclusive</i>, 0-indexed ending index of the span.
    /// </summary>
    /// <remarks>
    /// When using <see cref="TextSpan(int, int)"/>, this is ensured to be strictly
    /// greater than or equal to <see cref="Start"/> and greater than or equal to 0.
    /// When initializing this property manually, the same should be ensured by the user.
    /// </remarks>
    public int End { get; init; }

    /// <summary>
    /// The length of the span, in characters.
    /// </summary>
    public int Length => End - Start;

    /// <summary>
    /// Whether the span is empty (or negative) in length.
    /// </summary>
    /// <remarks>
    /// This property will be <see langword="true"/> in the case that <see cref="Start"/> and <see cref="End"/>
    /// have been improperly initialized such that the length of the span is negative,
    /// although many methods acting on <see cref="TextSpan"/>s will still not work properly if this is the case.
    /// </remarks>
    public bool IsEmpty => Start >= End;

    /// <summary>
    /// Creates a new <see cref="TextSpan"/> from a start and end position.
    /// </summary>
    /// <param name="start">The <i>inclusive</i>, 0-indexed starting index of the span.</param>
    /// <param name="end">The <i>exclusive</i>, 0-indexed ending index of the span.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Either <paramref name="start"/> or <paramref name="end"/> are less than 0.
    /// </exception>
    /// <remarks>
    /// This constructor will ensure that <see cref="Start"/> is strictly less than or equal to <see cref="End"/>.
    /// If <paramref name="start"/> is greater than <paramref name="end"/>, the two parameters will be swapped.
    /// </remarks>
    public TextSpan(int start, int end)
    {
        if (start < 0)
            throw new ArgumentOutOfRangeException(nameof(start),
                "Start cannot be less than 0.");
        if (end < 0)
            throw new ArgumentOutOfRangeException(nameof(end),
                "Start cannot be less than 0.");

        if (start <= end)
        {
            Start = start;
            End = end;
        }
        else
        {
            Start = end;
            End = start;
        }
    }

#if NETCOREAPP
    /// <summary>
    /// Returns a <see cref="Range"/> constructed from the span.
    /// </summary>
    public Range ToRange() => new(
        start: new(Start, fromEnd: false),
        end: new(End, fromEnd: false));
#endif

    /// <summary>
    /// Constructs a new <see cref="TextSpan"/> from a start position and a length.
    /// </summary>
    /// <param name="start">The <i>inclusive</i>, 0-indexed starting index of the span.</param>
    /// <param name="length">The length of the span.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Either <paramref name="start"/> or <paramref name="length"/> are less than 0.
    /// </exception>
    public static TextSpan FromLength(int start, int length)
    {
        if (start < 0)
            throw new ArgumentOutOfRangeException(nameof(start),
                "Start cannot be less than 0.");
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length),
                "Length cannot be less than 0");

        return new()
        {
            Start = start,
            End = start + length
        };
    }

    /// <summary>
    /// Constructs a new empty <see cref="TextSpan"/>.
    /// The constructed span will have a <see cref="Length"/> of 0,
    /// and <see cref="Start"/> and <see cref="End"/> will have the same value.
    /// </summary>
    /// <param name="at">The value to construct the empty span with.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="at"/> is less than 0.
    /// </exception>
    public static TextSpan Empty(int at)
    {
        if (at < 0)
            throw new ArgumentOutOfRangeException(nameof(at),
                "At cannot be less than 0.");
        
        return new()
        {
            Start = at,
            End = at
        };
    }

    /// <summary>
    /// Checks whether the current <see cref="TextSpan"/> is equal to another.
    /// </summary>
    /// <param name="other">The other <see cref="TextSpan"/> to check against.</param>
    public bool Equals(TextSpan other) =>
        Start == other.Start &&
        End == other.End;

    /// <inheritdoc/>
    public override bool Equals(object? obj) =>
        obj is TextSpan other && Equals(other);

    /// <summary>
    /// Checks a <see cref="TextSpan"/> is equal to another.
    /// </summary>
    public static bool operator ==(TextSpan a, TextSpan b) =>
        a.Equals(b);

    /// <summary>
    /// Checks a <see cref="TextSpan"/> is not equal to another.
    /// </summary>
    public static bool operator !=(TextSpan a, TextSpan b) =>
        !a.Equals(b);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        HashCode.Combine(Start, End);

    /// <inheritdoc/>
    public override string ToString() =>
        $"{Start}..{End}";
}

/// <summary>
/// Various extensions relating to <see cref="TextSpan"/>.
/// </summary>
public static class TextSpanExtensions
{
    /// <summary>
    /// Forms a slice out of the given span of characters,
    /// beginning at <see cref="TextSpan.Start"/> with the length of the <see cref="TextSpan"/>.
    /// </summary>
    /// <param name="textSpan">The source span of characters.</param>
    /// <param name="span">The <see cref="TextSpan"/> to use to slice the span of characters.</param>
    public static Span<char> Slice(this Span<char> span, TextSpan textSpan) =>
        span[textSpan.Start..textSpan.End];

    /// <summary>
    /// Forms a slice out of the given read-only span of characters,
    /// beginning at <see cref="TextSpan.Start"/> with the length of the <see cref="TextSpan"/>.
    /// </summary>
    /// <param name="textSpan">The source read-only span of characters.</param>
    /// <param name="span">The <see cref="TextSpan"/> to use to slice the span of characters.</param>
    public static ReadOnlySpan<char> Slice(this ReadOnlySpan<char> span, TextSpan textSpan) =>
        span[textSpan.Start..textSpan.End];

    /// <summary>
    /// Retrieves a substring from a string.
    /// The substring starts at a <see cref="TextSpan.Start"/> with the length of the <see cref="TextSpan"/>.
    /// </summary>
    /// <param name="str">The source string.</param>
    /// <param name="textSpan">The <see cref="TextSpan"/> to use to form a substring of the string.</param>
    public static string Substring(this string str, TextSpan textSpan) =>
        str[textSpan.Start..textSpan.End];
}
