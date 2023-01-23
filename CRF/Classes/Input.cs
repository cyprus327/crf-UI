using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Linq;
using CRF.Structs;
using System.Collections.Generic;
using System;
using System.Text;

namespace CRF.Classes {
    public class Input {
        [DllImport("user32.dll")] static extern short GetAsyncKeyState(int key);
        [DllImport("user32.dll")] static extern bool GetCursorPos(out POINT mousePos);

        public static Dictionary<string, char> Keys = new Dictionary<string, char>() {
            { "A", 'A' }, { "B", 'B' }, { "C", 'C' }, { "D", 'D' }, { "E", 'E' }, { "F", 'F' }, { "G", 'G' }, { "H", 'H' }, { "I", 'I' }, { "J", 'J' }, { "K", 'K' }, { "L", 'L' }, { "M", 'M' }, 
            { "N", 'N' }, { "O", 'O' }, { "P", 'P' }, { "Q", 'Q' }, { "R", 'R' }, { "S", 'S' }, { "T", 'T' }, { "U", 'U' }, { "V", 'V' }, { "W", 'W' }, { "X", 'X' }, { "Y", 'Y' }, { "Z", 'Z' }, 
            { "0", (char)48 }, { "1", (char)49 }, { "2", (char)50 }, { "3", (char)51 }, { "4", (char)52 }, { "5", (char)53 }, { "6", (char)54 }, { "7", (char)55 }, { "8", (char)56 }, { "9", (char)57 }, 
            { "-", (char)45 }
        };

        readonly Dictionary<char, bool> _prevKeyState = new Dictionary<char, bool>();
        bool prevRMBState = false, prevLMBState = false;

        public bool GetKeyDown(char key) {
            return (GetAsyncKeyState(key) & 0x8000) != 0;
        }

        public bool GetKeyUp(char key) {
            bool currentKeyState = GetKeyDown(key);
            bool keyUp = false;
            if (!_prevKeyState.ContainsKey(key)) {
                _prevKeyState[key] = currentKeyState;
            }
            else {
                if (_prevKeyState[key] && !currentKeyState) {
                    keyUp = true;
                }
                _prevKeyState[key] = currentKeyState;
            }
            return keyUp;
        }

        public bool GetCursorPos(out int x, out int y) {
            bool result = GetCursorPos(out POINT point);
            x = point.X;
            y = point.Y;
            return result;
        }

        public bool LeftMouseDown() {
            return (GetAsyncKeyState(0x01) & 0x8000) != 0;
        }

        public bool RightMouseDown() {
            return (GetAsyncKeyState(0x02) & 0x8000) != 0;
        }

        public bool LeftMouseUp() {
            bool currentRMBState = LeftMouseDown();
            bool RMBUp = prevLMBState && !currentRMBState;
            prevLMBState = currentRMBState;
            return RMBUp;
        }

        public bool RightMouseUp() {
            bool currentRMBState = RightMouseDown();
            bool RMBUp = prevRMBState && !currentRMBState;
            prevRMBState = currentRMBState;
            return RMBUp;
        }

        public string ReadKeyboardInput() {
            StringBuilder sb = new StringBuilder();
            
            if (GetKeyUp((char)13)) sb.Append("\n");
            foreach (var key in Keys) {
                if (GetKeyUp(key.Value)) {
                    sb.Append(key.Value);
                }
            }
            if (GetKeyUp(' ')) sb.Append(' ');

            return sb.ToString();
        }
    }
}

