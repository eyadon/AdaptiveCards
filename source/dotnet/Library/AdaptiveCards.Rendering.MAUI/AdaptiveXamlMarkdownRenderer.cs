// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.MarkedNet;

namespace AdaptiveCards.Rendering.MAUI
{

    public class AdaptiveXamlMarkdownRenderer : MarkdownRenderer
    {
        public AdaptiveXamlMarkdownRenderer() : base()
        {
        }

        public AdaptiveXamlMarkdownRenderer(Options options) : base(options)
        {
        }

        private static readonly string listMarker = "<ListMarker/>";

        public override string Blockquote(string quote)
        {
            // not supported
            return String.Empty;
        }

        public override string Br()
        {
            return "<Span Text=\"\n\" />";
        }

        public override string Code(string code, string lang, bool escaped)
        {
            // not supported
            return string.Empty;
        }

        public override string Codespan(string text)
        {
            return $"<Span FontFamily=\"Consolas\" Text=\"{text}\" />";
        }

        public override string Del(string text)
        {
            return $"<Span TextDecorations=\"Strikethrough\">{text}</Span>";
        }

        public override string Em(string text)
        {
            return $"<Span FontAttributes=\"Italic\">{text}</Span>";
        }

        public override string Heading(string text, int level, string raw)
        {
            return $"<Span Text=\"{text}\n\" />";
        }

        public override string Hr()
        {
            // not supported
            return String.Empty;
        }

        public override string Html(string html)
        {
            // not supported
            return String.Empty;
        }

        public override string Image(string href, string title, string text)
        {
            // not supported
            return String.Empty;
        }

        public override string Link(string href, string title, string text)
        {
            return string.Empty;
            //return $"<Hyperlink Command=\"NavigationCommands.GoToPage\" CommandParameter=\"{href}\">{text}</Hyperlink>";
        }

        public override string List(string body, bool ordered, int start)
        {
            if (ordered)
            {
                int outputNum;
                int iMarker;
                for (outputNum = start, iMarker = body.IndexOf(listMarker);
                    iMarker > 0;
                    iMarker = body.IndexOf(listMarker), outputNum++)
                {
                    body = body.Remove(iMarker, listMarker.Length).Insert(iMarker, outputNum.ToString() + ".");
                }
            }
            else
            {
                body = body.Replace(listMarker, "•");
            }
            return $"{body}<LineBreak/>";
        }

        public override string ListItem(string text)
        {
            return $" {listMarker} {text}<LineBreak/>";
        }

        public override string Paragraph(string text)
        {
            return $"<Span Text=\"{text}\n\"></Span>";
        }

        public override string Strong(string text)
        {
            return $"<Span FontAttributes=\"Bold\">{text}</Span>";
        }

        public override string Table(string header, string body)
        {
            // not supported
            return string.Empty;
        }

        public override string TableRow(string content)
        {
            // not supported
            return string.Empty;
        }

        public override string TableCell(string content, TableCellFlags flags)
        {
            // not supported
            return string.Empty;
        }

        public override string Postprocess(string text)
        {
            text = text.Replace("<LineBreak/><LineBreak/>", "<LineBreak/>");
            while (text.EndsWith("<LineBreak/>"))
                text = text.Substring(0, text.LastIndexOf("<LineBreak/>"));
            return base.Postprocess(text);
        }
    }
}
