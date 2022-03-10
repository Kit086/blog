using System.Text;
using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace MyBlog;

public class MyPrismCodeBlockRenderer : HtmlObjectRenderer<CodeBlock>
{
    private readonly CodeBlockRenderer _codeBlockRenderer;

    public MyPrismCodeBlockRenderer()
    {
        _codeBlockRenderer = new CodeBlockRenderer();
    }

    protected override void Write(HtmlRenderer renderer, CodeBlock node)
    {
        var fencedCodeBlock = node as FencedCodeBlock;
        var parser = node.Parser as FencedCodeBlockParser;
        if (fencedCodeBlock == null || parser == null)
        {
            _codeBlockRenderer.Write(renderer, node);
            return;
        }

        // var languageCode = fencedCodeBlock.Info.Replace(parser.InfoPrefix, string.Empty);
        var languageCode = fencedCodeBlock.Info;
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            _codeBlockRenderer.Write(renderer, node);
            return;
        }

        var attributes = new HtmlAttributes();
        attributes.AddClass($"language-{languageCode}");

        var code = ExtractSourceCode(node);

        renderer
            .Write("<pre")
            .WriteAttributes(attributes)
            .Write(">")
            .Write("<code")
            .WriteAttributes(attributes)
            .Write(">")
            .Write(code)
            .Write("</code>")
            .Write("</pre>");
    }

    protected string ExtractSourceCode(LeafBlock node)
    {
        var code = new StringBuilder();
        var lines = node.Lines.Lines;
        int totalLines = lines.Length;
        for (int i = 0; i < totalLines; i++)
        {
            var line = lines[i];
            var slice = line.Slice;
            if (slice.Text == null)
            {
                continue;
            }

            var lineText = slice.Text.Substring(slice.Start, slice.Length);
            if (i > 0)
            {
                code.AppendLine();
            }

            foreach (var c in lineText)
            {
                if (CharRemap.TryGetValue(c, out var s))
                    code.Append(s);
                else
                    code.Append(c);
            }
        }

        return code.ToString();
    }

    protected IDictionary<char, string> CharRemap = new Dictionary<char, string>
    {
        ['<'] = "&lt;",
        ['>'] = "&gt;",
        ['&'] = "&amp;"
    };
}


public class MyPrismExtension : IMarkdownExtension
{
    public void Setup(MarkdownPipelineBuilder pipeline)
    {
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer == null)
        {
            throw new ArgumentNullException(nameof(renderer));
        }

        if (renderer is TextRendererBase<HtmlRenderer> htmlRenderer)
        {
            var codeBlockRenderer = htmlRenderer.ObjectRenderers.FindExact<CodeBlockRenderer>();
            if (codeBlockRenderer != null)
            {
                htmlRenderer.ObjectRenderers.Remove(codeBlockRenderer);
            }

            htmlRenderer.ObjectRenderers.AddIfNotAlready(new MyPrismCodeBlockRenderer());
        }
    }
}
