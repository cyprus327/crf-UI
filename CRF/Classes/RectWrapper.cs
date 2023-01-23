using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

namespace CRF.Classes {
    public class RectWrapper {
        public RectWrapper(string input) {
            Text = new StringBuilder(input);
        }

        public List<RectangleF> Rects { get; set; } = new List<RectangleF>();
        public Rectangle Bounds { get; private set; }
        public StringBuilder Text { get; set; }
        public int X { get; set; } = 20;
        public int Y { get; set; } = 20;
        public int Margin { get; set; } = 10;
        public float LineWidthMultiplier { get; set; } = 3f;
        public Color LineColor { get; set; } = Color.White;
        public bool Selected { get; set; } = false;

        public Rectangle Draw(Graphics graphics, Color bgCol) {
            float maxRight = float.MinValue, maxBottom = 0;
            foreach (RectangleF rect in Rects) {
                maxRight = Math.Max(maxRight, rect.Width);
                maxBottom += rect.Height;
            }

            Bounds = new Rectangle(X - Margin, Y - Margin, (int)(maxRight + Margin * 2), (int)(maxBottom + Margin * 2));

            int r = bgCol.R + 22, g = bgCol.G + 22, b = bgCol.B + 22;
            Color ligherBgCol = Color.FromArgb(
                r <= 255 ? r < 0 ? 0 : r : 255,
                g <= 255 ? g < 0 ? 0 : g : 255,
                b <= 255 ? b < 0 ? 0 : b : 255);

            using (Brush bgBrush = new SolidBrush(ligherBgCol)) {
                using (Pen pen = new Pen(Selected ? Color.Green : LineColor, LineWidthMultiplier)) {
                    graphics.FillRectangle(bgBrush, Bounds);
                    graphics.DrawRectangle(pen, Bounds);
                }
            }
            
            return Bounds;
        }

        public void HandleMoveResponse(int x, int y, int dx, int dy, int gridSize = 10) {
            //gridSize = Math.Max(1, gridSize);
            X -= dx;
            Y -= dy;
            // the commented out part above works but no grid

            //X = x - dx - ((x - dx) % gridSize);
            //Y = y - dy - ((y - dy) % gridSize);
        }
    }
}
