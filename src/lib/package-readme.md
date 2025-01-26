# Text mapping utilities

A couple utilities for mapping between text- and line/character positions.

## Types

- `TextSpan`: a lightweight representation of a span of characters within a text. Supports slicing strings and spans through `Substring` and `Slice` extension methods.

- `LineMap`: an efficient mapping between lines and character positions of a text. Can be used to obtain the line and column numbers from an absolute index within a text.
