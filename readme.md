<div align="center">
    <a href="https://github.com/thinker227/text-mapping-utils/actions/workflows/build-test.yml"><img alt="Build and test status" src="https://img.shields.io/github/actions/workflow/status/thinker227/text-mapping-utils/build-test.yml?style=for-the-badge&label=Build%20%26%20Test"></a>
    <br/>
    <a href="https://www.nuget.org/packages/thinker227.TextMappingUtils"><img alt="Nuget" src="https://img.shields.io/nuget/vpre/thinker227.TextMappingUtils?style=for-the-badge&label=Nuget%3A%20thinker227.TextMappingUtils"></a>
</div>

<br/><br/>

This package and repository contains a couple small utilities for mapping between text- and line/character positions within text.

This is mainly an updated version of these helpers I've written for others projects which I felt were simpler to publish as a package rather than copy-pasting them between my various projects.

## Types

- `TextSpan`: a lightweight representation of a span of characters within a text. Supports slicing strings and spans through `Substring` and `Slice` extension methods.

- `LineMap`: an efficient mapping between lines and character positions of a text. Can be used to obtain the line and column numbers from an absolute index within a text.
