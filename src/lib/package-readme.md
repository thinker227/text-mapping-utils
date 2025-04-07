# Text mapping utilities

A couple utilities for mapping between text- and line/character positions.

## Examples

```cs
string text = """
The quick brown fox
jumps over
the lazy dog
""";

// Construct a LineMap over the lines of the text
LineMap map = LineMap.Create(text);

// Fetch the first line of the text and print its length
Line line = map.GetLine(1);
Console.WriteLine($"Line 1 contains {line1.Span.Length} characters.");

// Fetch the 25th character in the text and prints its line and its offset in the line
CharacterPosition character = map.GetCharacterPosition(25);
Console.WriteLine($"Character 25 is character {character.Offset} in line {character.Line.LineNumber}.");
```

## Types

- `TextSpan`: A lightweight representation of a span of characters within a text. Supports slicing strings and spans through `Substring` and `Slice` extension methods.

- `LineMap`: An efficient mapping between lines and character positions of a text. Can be used to obtain the line and column numbers from an absolute index within a text.

## Changelog (1.1.1)

- Allow `TextSpanExtensions.Slice` to be used with generic spans

- Update docs ever so slightly

- Optimize `LineMap.Create`
