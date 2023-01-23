using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using CRF.Classes;
using System.Net.Http;

namespace CRF {
    static class Program {
        static void Main() {
            MarkdownRenderer notes = new MarkdownRenderer() {
                Wrappers = new List<RectWrapper>() {
                    new RectWrapper("**bold**text\n- bullet point") { 
                        LineColor = Color.Red, X = 50, Y = 100 
                    },
                    new RectWrapper("plain + *italics*text\n\n\n# big header\n## medium header\n### small header") { 
                        LineColor = Color.Orange, X = 600, Y = 460 
                    }
                },
                FontSizeMultiplier = 1.5f
            };


            Engine t = new NotesWindow("test", new Size(1270, 720), notes);
            t.Run();
        }
    }
}