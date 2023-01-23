using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CRF.Classes {
    public class MarkdownRenderer {
        /*
        readonly Dictionary<string, Func<Match, DrawableElement>> _replacements = new Dictionary<string, Func<Match, DrawableElement>>() {
            { @"^# (.*)", m => new HeadingElement(1, m.Groups[1].Value) }, // large header
            { @"^## (.*)", m => new HeadingElement(2, m.Groups[1].Value) }, // medium header
            { @"^### (.*)", m => new HeadingElement(3, m.Groups[1].Value) }, // small header
            { @"\[(.*?)\]\((.*?)\)", m => new LinkElement(m.Groups[1].Value, m.Groups[2].Value) }, // links
            { @"\*\*(.*?)\*\*", m => new BoldElement(m.Groups[1].Value) }, // bold text
            { @"_(.*?)_", m => new ItalicElement(m.Groups[1].Value) }, // italic text
            { @"^\* (.*)", m => new BulletPointElement(m.Groups[1].Value) }, // bullet point 1
        };
        */
        
        public List<RectWrapper> Wrappers { get; set; } = new List<RectWrapper>();
        public float FontSizeMultiplier { get; set; } = 1f;

        public void Draw(Graphics g, Font font, Brush brush, Color bgCol) {
            foreach (RectWrapper wrapper in Wrappers) {
                wrapper.Draw(g, bgCol);
                wrapper.Rects = GetRectsAndDrawText(wrapper, g, font, brush);
                wrapper.LineWidthMultiplier = FontSizeMultiplier * 0.5f;
            }
        } 

        private List<RectangleF> GetRectsAndDrawText(RectWrapper wrapper, Graphics g, Font font, Brush brush) {
            List<RectangleF> boundingBoxes = new List<RectangleF>();
            string[] lines = wrapper.Text.ToString().Split('\n');
            int x = wrapper.X, y = wrapper.Y, cursorLine = 0;
            for (int i = 0; i < lines.Length; i++) {
                RectangleF boundingBox = new RectangleF();
                cursorLine += lines[i].Length;

                if (!wrapper.Selected) {
                    foreach (MarkdownToken token in Tokenize(lines[i], font, brush)) {
                        SizeF size = g.MeasureString(token.Text, token.Font);
                        boundingBox.Width += size.Width;
                        boundingBox.Height = Math.Max(boundingBox.Height, size.Height);
                        g.DrawString(token.Text, token.Font, token.Brush, x, y);
                        x += (int)size.Width;
                    }
                }
                else {
                    SizeF size = g.MeasureString(lines[i], font, new PointF(x, y), new StringFormat(StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.LineLimit));
                    boundingBox.Width += size.Width;
                    boundingBox.Height = Math.Max(boundingBox.Height, size.Height);
                    g.DrawString(lines[i], font, brush, x, y);
                }
                
                boundingBoxes.Add(boundingBox);
                y += (int)boundingBox.Height;
                x = wrapper.X;
            }

            return boundingBoxes;
        }

        private List<MarkdownToken> Tokenize(string line, Font font, Brush brush) {
            var tokens = new List<MarkdownToken>();
            int currentIndex = 0;
            while (currentIndex < line.Length) {
                switch (line[currentIndex]) {
                    case '#':
                        CreateHeaderToken(line, font, brush, ref tokens, ref currentIndex);
                        break;
                    case '*':
                        if (currentIndex + 1 < line.Length - 1 && line[currentIndex + 1] == '*')
                            CreateBoldToken(line, font, brush, ref tokens, ref currentIndex);
                        else
                            CreateItalicToken(line, font, brush, ref tokens, ref currentIndex);
                        break;
                    case '-':
                        if (line[currentIndex + 1] == ' ')
                            CreateBulletPointToken(line, font, brush, ref tokens, ref currentIndex);
                        else
                            CreatePlainTextToken(line, font, brush, ref tokens, ref currentIndex);
                        break;
                    default:
                        CreatePlainTextToken(line, font, brush, ref tokens, ref currentIndex);
                        break;
                }
            }
            return tokens;
        }

        private void CreateHeaderToken(string line, Font font, Brush brush, ref List<MarkdownToken> tokens, ref int currentIndex) {
            int startIndex = currentIndex;
            int endIndex = line.IndexOfAny(new[] { ' ', '\t', '\r', '\n' }, startIndex);
            endIndex = endIndex == -1 ? line.Length : endIndex;
            int headerLevel = endIndex - startIndex;
            tokens.Add(new MarkdownToken {
                Text = line.Substring(endIndex).TrimStart(),
                Font = new Font(font.FontFamily, (font.Size + (20 / headerLevel)) * FontSizeMultiplier, FontStyle.Bold),
                Brush = brush,
            });
            currentIndex = line.Length;
        }

        private void CreateBoldToken(string line, Font font, Brush brush, ref List<MarkdownToken> tokens, ref int currentIndex) {
            int startIndex = currentIndex + 2;
            int endIndex = line.IndexOf("**", startIndex);
            endIndex = endIndex == -1 ? line.Length : endIndex;
            tokens.Add(new MarkdownToken {
                Text = line.Substring(startIndex, endIndex - startIndex),
                Font = new Font(font.FontFamily, font.Size * FontSizeMultiplier, FontStyle.Bold),
                Brush = brush,
            });
            currentIndex = endIndex + 2;
        }

        private void CreateItalicToken(string line, Font font, Brush brush, ref List<MarkdownToken> tokens, ref int currentIndex) {
            int startIndex = currentIndex + 1;
            int endIndex = line.IndexOfAny(new[] { '*', '_' }, startIndex);
            endIndex = endIndex == -1 ? line.Length : endIndex;
            tokens.Add(new MarkdownToken {
                Text = line.Substring(startIndex, endIndex - startIndex),
                Font = new Font(font.FontFamily, font.Size * FontSizeMultiplier, FontStyle.Italic),
                Brush = brush,
            });
            currentIndex = endIndex + 1;
        }

        private void CreateBulletPointToken(string line, Font font, Brush brush, ref List<MarkdownToken> tokens, ref int currentIndex) {
            var startIndex = currentIndex + 2;
            var endIndex = line.IndexOf("-", startIndex);
            if (endIndex == -1) {
                endIndex = line.Length;
            }
            tokens.Add(new MarkdownToken {
                Text = $"\u2022 {line.Substring(startIndex, endIndex - startIndex)}",
                Font = new Font(font.FontFamily, font.Size * FontSizeMultiplier, FontStyle.Regular),
                Brush = brush,
            });
            currentIndex = endIndex;
        }

        private void CreatePlainTextToken(string line, Font font, Brush brush, ref List<MarkdownToken> tokens, ref int currentIndex) {
            int endIndex = line.IndexOfAny(new[] { '*', '_', '#', '-' }, currentIndex);
            endIndex = endIndex == -1 ? line.Length : endIndex;
            tokens.Add(new MarkdownToken {
                Text = line.Substring(currentIndex, endIndex - currentIndex),
                Font = new Font(font.FontFamily, font.Size * FontSizeMultiplier, FontStyle.Regular),
                Brush = brush,
            });
            currentIndex = endIndex;
        }
    }

    public class MarkdownToken {
        public string Text { get; set; }
        public Font Font { get; set; }
        public Brush Brush { get; set; }
    }
}