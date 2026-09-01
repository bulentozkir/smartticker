using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.Media;
using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MarkdownInline = Markdig.Syntax.Inlines.Inline;

namespace SmartTicker.Desktop.Controls;

public sealed record MarkdownHelpHeading(string Title, string Anchor, int Level, Control Target);

public sealed record MarkdownHelpDocument(Control Content, IReadOnlyList<MarkdownHelpHeading> Headings);

public static partial class MarkdownHelpRenderer
{
    private static readonly IBrush BodyBrush = Brush("#D0D7DE");
    private static readonly IBrush MutedBrush = Brush("#8B949E");
    private static readonly IBrush HeadingBrush = Brush("#E6EDF3");
    private static readonly IBrush AccentBrush = Brush("#70E1A1");
    private static readonly IBrush LinkBrush = Brush("#79C0FF");
    private static readonly IBrush SurfaceBrush = Brush("#161B22");
    private static readonly IBrush CodeBrush = Brush("#A5D6FF");
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    public static MarkdownHelpDocument Render(
        string markdown,
        Action<string> navigateToAnchor,
        Action<Uri> openExternalLink)
    {
        var root = new StackPanel
        {
            Spacing = 12,
            Margin = new Thickness(22, 18, 26, 30),
            MaxWidth = 960,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        var headings = new List<MarkdownHelpHeading>();
        var usedAnchors = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var document = Markdown.Parse(markdown, Pipeline);
        RenderBlocks(document, root, headings, usedAnchors, navigateToAnchor, openExternalLink);
        return new MarkdownHelpDocument(root, headings);
    }

    private static void RenderBlocks(
        IEnumerable<Block> blocks,
        Panel target,
        List<MarkdownHelpHeading> headings,
        Dictionary<string, int> usedAnchors,
        Action<string> navigateToAnchor,
        Action<Uri> openExternalLink)
    {
        foreach (var block in blocks)
        {
            var control = RenderBlock(
                block,
                headings,
                usedAnchors,
                navigateToAnchor,
                openExternalLink);
            if (control is not null)
            {
                target.Children.Add(control);
            }
        }
    }

    private static Control? RenderBlock(
        Block block,
        List<MarkdownHelpHeading> headings,
        Dictionary<string, int> usedAnchors,
        Action<string> navigateToAnchor,
        Action<Uri> openExternalLink) => block switch
    {
        HeadingBlock heading => RenderHeading(heading, headings, usedAnchors, navigateToAnchor, openExternalLink),
        ParagraphBlock paragraph => RenderParagraph(paragraph, navigateToAnchor, openExternalLink),
        ListBlock list => RenderList(list, headings, usedAnchors, navigateToAnchor, openExternalLink),
        QuoteBlock quote => RenderQuote(quote, headings, usedAnchors, navigateToAnchor, openExternalLink),
        Table table => RenderTable(table, headings, usedAnchors, navigateToAnchor, openExternalLink),
        FencedCodeBlock code => RenderCode(code),
        CodeBlock code => RenderCode(code),
        ThematicBreakBlock => new Border
        {
            BorderBrush = Brush("#30363D"),
            BorderThickness = new Thickness(0, 1, 0, 0),
            Margin = new Thickness(0, 8),
        },
        _ => null,
    };

    private static Control RenderHeading(
        HeadingBlock heading,
        List<MarkdownHelpHeading> headings,
        Dictionary<string, int> usedAnchors,
        Action<string> navigateToAnchor,
        Action<Uri> openExternalLink)
    {
        var title = PlainText(heading.Inline).Trim();
        var anchor = UniqueAnchor(Slug(title), usedAnchors);
        var text = RenderInlineText(heading.Inline, navigateToAnchor, openExternalLink);
        text.FontSize = heading.Level switch
        {
            1 => 29,
            2 => 21,
            _ => 16,
        };
        text.FontWeight = FontWeight.SemiBold;
        text.Foreground = heading.Level == 1 ? AccentBrush : HeadingBrush;
        text.Margin = new Thickness(0, heading.Level == 1 ? 0 : 10, 0, heading.Level == 1 ? 5 : 2);
        headings.Add(new MarkdownHelpHeading(title, anchor, heading.Level, text));
        return text;
    }

    private static Control RenderParagraph(
        ParagraphBlock paragraph,
        Action<string> navigateToAnchor,
        Action<Uri> openExternalLink)
    {
        var text = RenderInlineText(paragraph.Inline, navigateToAnchor, openExternalLink);
        text.FontSize = 14;
        text.LineHeight = 22;
        text.Foreground = BodyBrush;
        return text;
    }

    private static Control RenderList(
        ListBlock list,
        List<MarkdownHelpHeading> headings,
        Dictionary<string, int> usedAnchors,
        Action<string> navigateToAnchor,
        Action<Uri> openExternalLink)
    {
        var panel = new StackPanel { Spacing = 7, Margin = new Thickness(8, 0, 0, 2) };
        var index = 1;
        foreach (var item in list.OfType<ListItemBlock>())
        {
            var row = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("34,*"),
                ColumnSpacing = 4,
            };
            row.Children.Add(new TextBlock
            {
                Text = list.IsOrdered ? $"{index++}." : "-",
                Foreground = AccentBrush,
                FontWeight = FontWeight.SemiBold,
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 2, 5, 0),
            });
            var content = new StackPanel { Spacing = 6 };
            Grid.SetColumn(content, 1);
            RenderBlocks(item, content, headings, usedAnchors, navigateToAnchor, openExternalLink);
            row.Children.Add(content);
            panel.Children.Add(row);
        }

        return panel;
    }

    private static Control RenderQuote(
        QuoteBlock quote,
        List<MarkdownHelpHeading> headings,
        Dictionary<string, int> usedAnchors,
        Action<string> navigateToAnchor,
        Action<Uri> openExternalLink)
    {
        var content = new StackPanel { Spacing = 8 };
        RenderBlocks(quote, content, headings, usedAnchors, navigateToAnchor, openExternalLink);
        return new Border
        {
            BorderBrush = AccentBrush,
            BorderThickness = new Thickness(3, 0, 0, 0),
            Background = SurfaceBrush,
            Padding = new Thickness(14, 10),
            Child = content,
        };
    }

    private static Control RenderCode(CodeBlock code) => new Border
    {
        Background = Brush("#010409"),
        BorderBrush = Brush("#30363D"),
        BorderThickness = new Thickness(1),
        CornerRadius = new CornerRadius(4),
        Padding = new Thickness(14, 11),
        Child = new SelectableTextBlock
        {
            Text = code.Lines.ToString(),
            FontFamily = new FontFamily("Cascadia Mono, Consolas"),
            FontSize = 13,
            Foreground = CodeBrush,
            TextWrapping = TextWrapping.Wrap,
        },
    };

    private static Control RenderTable(
        Table table,
        List<MarkdownHelpHeading> headings,
        Dictionary<string, int> usedAnchors,
        Action<string> navigateToAnchor,
        Action<Uri> openExternalLink)
    {
        var rows = table.OfType<TableRow>().ToArray();
        var columnCount = Math.Max(1, rows.Select(row => row.Count).DefaultIfEmpty(1).Max());
        var grid = new Grid
        {
            ColumnSpacing = 0,
            RowSpacing = 0,
            MinWidth = 520,
        };
        for (var column = 0; column < columnCount; column++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(
                column == 0 && columnCount == 2 ? GridLength.Auto : new GridLength(1, GridUnitType.Star)));
        }

        for (var rowIndex = 0; rowIndex < rows.Length; rowIndex++)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            var row = rows[rowIndex];
            for (var columnIndex = 0; columnIndex < row.Count; columnIndex++)
            {
                var cell = (TableCell)row[columnIndex];
                var content = new StackPanel { Spacing = 5 };
                RenderBlocks(cell, content, headings, usedAnchors, navigateToAnchor, openExternalLink);
                var border = new Border
                {
                    Background = row.IsHeader ? SurfaceBrush : Brushes.Transparent,
                    BorderBrush = Brush("#30363D"),
                    BorderThickness = new Thickness(1, 1, 0, 0),
                    Padding = new Thickness(10, 8),
                    Child = content,
                };
                Grid.SetRow(border, rowIndex);
                Grid.SetColumn(border, columnIndex);
                grid.Children.Add(border);
            }
        }

        return new ScrollViewer
        {
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled,
            Content = grid,
        };
    }

    private static TextBlock RenderInlineText(
        ContainerInline? container,
        Action<string> navigateToAnchor,
        Action<Uri> openExternalLink)
    {
        var text = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            Foreground = BodyBrush,
            Inlines = new InlineCollection(),
        };
        AppendInlines(container?.FirstChild, text.Inlines, navigateToAnchor, openExternalLink);
        return text;
    }

    private static void AppendInlines(
        MarkdownInline? inline,
        InlineCollection target,
        Action<string> navigateToAnchor,
        Action<Uri> openExternalLink)
    {
        while (inline is not null)
        {
            switch (inline)
            {
                case LiteralInline literal:
                    target.Add(new Run(literal.Content.ToString()));
                    break;
                case CodeInline code:
                    target.Add(new Run(code.Content)
                    {
                        FontFamily = new FontFamily("Cascadia Mono, Consolas"),
                        Foreground = CodeBrush,
                        Background = SurfaceBrush,
                    });
                    break;
                case LineBreakInline:
                    target.Add(new LineBreak());
                    break;
                case EmphasisInline emphasis:
                    Span span = emphasis.DelimiterCount >= 2 ? new Bold() : new Italic();
                    span.Inlines = new InlineCollection();
                    AppendInlines(emphasis.FirstChild, span.Inlines, navigateToAnchor, openExternalLink);
                    target.Add(span);
                    break;
                case LinkInline link when !link.IsImage:
                    target.Add(CreateLinkButton(
                        PlainText(link),
                        link.Url,
                        navigateToAnchor,
                        openExternalLink));
                    break;
                case ContainerInline nested:
                    AppendInlines(nested.FirstChild, target, navigateToAnchor, openExternalLink);
                    break;
            }

            inline = inline.NextSibling;
        }
    }

    private static Button CreateLinkButton(
        string text,
        string? destination,
        Action<string> navigateToAnchor,
        Action<Uri> openExternalLink)
    {
        var button = new Button
        {
            Content = string.IsNullOrWhiteSpace(text) ? destination : text,
            Background = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Padding = new Thickness(1, 0),
            Foreground = LinkBrush,
            Focusable = false,
            FontWeight = FontWeight.SemiBold,
        };
        button.Click += (_, _) =>
        {
            if (destination?.StartsWith('#') == true)
            {
                navigateToAnchor(destination[1..]);
            }
            else if (Uri.TryCreate(destination, UriKind.Absolute, out var uri))
            {
                openExternalLink(uri);
            }
        };
        return button;
    }

    private static string PlainText(ContainerInline? container)
    {
        var builder = new StringBuilder();
        AppendPlainText(container?.FirstChild, builder);
        return builder.ToString();
    }

    private static void AppendPlainText(MarkdownInline? inline, StringBuilder target)
    {
        while (inline is not null)
        {
            switch (inline)
            {
                case LiteralInline literal:
                    target.Append(literal.Content);
                    break;
                case CodeInline code:
                    target.Append(code.Content);
                    break;
                case LineBreakInline:
                    target.Append(' ');
                    break;
                case ContainerInline nested:
                    AppendPlainText(nested.FirstChild, target);
                    break;
            }

            inline = inline.NextSibling;
        }
    }

    private static string Slug(string title)
    {
        var cleaned = NonSlugCharacter().Replace(title.ToLowerInvariant(), string.Empty);
        return Whitespace().Replace(cleaned.Trim(), "-");
    }

    private static string UniqueAnchor(string anchor, Dictionary<string, int> used)
    {
        if (!used.TryGetValue(anchor, out var count))
        {
            used[anchor] = 0;
            return anchor;
        }

        used[anchor] = ++count;
        return $"{anchor}-{count}";
    }

    private static IBrush Brush(string color) => new SolidColorBrush(Color.Parse(color));

    [GeneratedRegex(@"[^\p{L}\p{Nd}\s-]")]
    private static partial Regex NonSlugCharacter();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();
}