using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using CRF.Structs;
using CRF.Classes;

namespace CRF {
    public class Canvas : Form {
        public Canvas() {
            DoubleBuffered = true;
        }
    }

    public abstract class Engine {
        public Engine(Size screenSize, string title) {
            ScreenSize = screenSize;
            _title = title;
            _window = new Canvas {
                Size = new Size(ScreenSize.Width, ScreenSize.Height),
                Location = new Point(300, 300),
                StartPosition = FormStartPosition.Manual,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                ShowInTaskbar = true,
                Name = _title
            };
            _window.Paint += Renderer;
            _mainThread = new Thread(MainLoop);
            _mainThread.SetApartmentState(ApartmentState.STA);
            ScreenPosition = _window.Location;
            Console.WriteLine(ScreenPosition.ToString());
        }

        public static bool ShowDebugInfo = false;
        public static Brush ScreenTextBrush = new SolidBrush(Color.White);
        public static Font ScreenTextFont = new Font(FontFamily.GenericMonospace, 16f);
        public static Size ScreenSize = new Size();
        public static Color BackgroundColor = Color.FromArgb(37, 37, 37);
        public static Point ScreenPosition = new Point(300, 300);

        float delta = 0f;
        readonly DeltaTime _deltaTime = new DeltaTime();

        readonly string _title = "";
        readonly Canvas _window = null;
        readonly Thread _mainThread = null;
        Graphics mainGraphics;

        public void Run() {
            _mainThread.Start();
            Application.Run(_window);
        }

        private void MainLoop() {
            Awake();

            while (true) {
                try {
                    _window.BeginInvoke((MethodInvoker)delegate { _window.Refresh(); });
                }
                catch { } // ignore exception that gets thrown once at the very beginning every time

                Thread.Sleep(1); // prevents jittering / unstable fps
            }
        }
        
        private void Renderer(object sender, PaintEventArgs e) {
            mainGraphics = e.Graphics;
            mainGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            mainGraphics.SmoothingMode = SmoothingMode.AntiAlias;

            mainGraphics.Clear(BackgroundColor);

            delta = _deltaTime.Get();
            Update(mainGraphics, delta);

            _window.Text = $"{_title} | FPS: {(int)(1000 / delta / 1000)}";
        }

        public abstract void Awake();

        public abstract void Update(Graphics g, float deltaTime);

    }
}
