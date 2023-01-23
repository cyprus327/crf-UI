using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CRF.Structs;
using CRF.Classes;
using System.Drawing.Drawing2D;

namespace CRF {
    public class NotesWindow : Engine {
        public NotesWindow(string windowTitle, Size windowSize, MarkdownRenderer notes) : base(windowSize, windowTitle) {
            _renderer = notes;
        }

        readonly Input _input = new Input();
        readonly MarkdownRenderer _renderer;

        int lastX = 0, lastY = 0, dx = 0, dy = 0, totalMovedX = 0, totalMovedY = 0;
        bool aWrapperSelected = false;

        public override void Awake() {
            BackgroundColor = Color.FromArgb(37, 37, 37);
            ScreenTextFont = new Font("Consolas", 12f);
            ScreenTextBrush = Brushes.White;
        }

        public override void Update(Graphics g, float deltaTime) {
            HandleInput();
            DrawGrid(g);
            _renderer.Draw(g, ScreenTextFont, ScreenTextBrush, BackgroundColor);
        }

        private void HandleInput() {
            if (_input.GetCursorPos(out int x, out int y)) {
                x -= ScreenPosition.X;
                y -= ScreenPosition.Y + 25;
            }
            dx = lastX - x;
            dy = lastY - y;

            if (!aWrapperSelected) {
                if (_input.GetKeyDown((char)18)) {
                    _renderer.FontSizeMultiplier = Math.Max(0.5f, Math.Min(10, _renderer.FontSizeMultiplier + (float)dy / 100));
                }
             
                if (_input.GetKeyUp((char)17)) {
                    foreach (var wrapper in _renderer.Wrappers) {
                        wrapper.X -= wrapper.X % 10;
                        wrapper.Y -= wrapper.Y % 10;
                    }
                }
                
                if (_input.GetKeyDown(' ')) {
                    foreach (var wrapper in _renderer.Wrappers) {
                        wrapper.X -= dx;
                        wrapper.Y -= dy;
                    }
                    totalMovedX += dx;
                    totalMovedY += dy;
                }
            } 
            

            int selectedWrappers = 0;
            foreach (var wrapper in _renderer.Wrappers) {
                if (wrapper.Bounds.Contains(x, y)) {
                    if (_input.RightMouseDown()) {
                        wrapper.HandleMoveResponse(x, y, dx, dy);
                    }
                    if (_input.LeftMouseUp()) {
                        wrapper.Selected = !wrapper.Selected;
                    }
                }

                if (wrapper.Selected) {
                    selectedWrappers++;
                    if (_input.GetKeyDown((char)27)) wrapper.Selected = false;
                }
            }
            aWrapperSelected = selectedWrappers > 0;
            
            lastX = x;
            lastY = y;
        }

        private void DrawGrid(Graphics g, int squareSize = 70, float lineThickness = 1.5f) {
            using (Pen pen = new Pen(Color.FromArgb(BackgroundColor.R - 10, BackgroundColor.G - 10, BackgroundColor.B - 10), lineThickness)) {
                int startX = (-totalMovedX % squareSize) - squareSize;
                int startY = (-totalMovedY % squareSize) - squareSize;

                for (int y = startY; y <= ScreenSize.Height; y += squareSize) {
                    g.DrawLine(pen, new Point(startX, y), new Point(ScreenSize.Width, y));
                }

                for (int x = startX; x <= ScreenSize.Width; x += squareSize) {
                    g.DrawLine(pen, new Point(x, startY), new Point(x, ScreenSize.Height));
                }
            }
        }
    }

    public static class StrExtensions {
        public static string ShiftHeld(this string str) {
            StringBuilder output = new StringBuilder();
            output.Append(str);
            for (int i = 0; i < output.Length; i++) {
                if      (output[i] == (char)49) output[i] = '\u0021';
                else if (output[i] == (char)50) output[i] = '\u0040';
                else if (output[i] == (char)51) output[i] = '\u0023';
                else if (output[i] == (char)52) output[i] = '\u0024';
                else if (output[i] == (char)53) output[i] = (char)37;
                else if (output[i] == (char)54) output[i] = '\u005E';
                else if (output[i] == (char)55) output[i] = '\u0038';
                else if (output[i] == (char)56) output[i] = '\u002A';
                else if (output[i] == (char)57) output[i] = '\u0028';
                else if (output[i] == (char)48) output[i] = '\u0029';
            }
            return output.ToString();
        }
    }
}