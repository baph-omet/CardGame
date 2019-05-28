using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardGame.UI {
    public class TextBox : Sprite {
        public int Index;
        public bool Hidden;
        
        public string Title;
        public string Text;
        public string[] Choices;

        private int Width;
        private int[] Margin;

        public TextBox(string text) : this(null, text) { }
        public TextBox(string title, string text) : this(title, text, null) { }
        public TextBox(string title, string text, string[] choices) : this(title,text,choices,Program.WindowSize[0] - 1) { }
        public TextBox(string title, string text, string[] choices, int width) : this(title,text,choices,width,new int[] {0,0}) {}
        public TextBox(string title, string text, string[] choices, int width, int[] margin) {
            Title = title;
            Text = text;
            Choices = choices;
            Index = 0;
            Width = width;
            Hidden = false;
            if (margin.Length == 2) Margin = margin;
            else throw new ArgumentException("Margin array must be exactly 2 arguments {left, top}");
        }

        public string Render() {
            if (Hidden) return "";
            StringBuilder buffer = new StringBuilder();
            
            // Top margin
            for (int i = 0; i < Margin[1]; i++) buffer.AppendLine();

            // Title
            if (Title != null && Title.Length > 0) buffer.AppendLine(GetLeftMargin() + String.Format("{0," + (Program.WindowSize[0] - 20) + "}", Title));

            // Header
            buffer.Append(GetLeftMargin() + " ");
            for (int i = 0; i < Width - 2; i++) buffer.Append("_");
            buffer.AppendLine();
            buffer.Append(GetLeftMargin() + "|");
            for (int i = 0; i < Width - 2; i++) buffer.Append(" ");
            buffer.AppendLine("|");

            // Text
            if (Text != null && Text.Length > 0) {
                string[] lines = Text.Replace('\r','\0').Split('\n');
                foreach (string line in lines) {
                    if (line.Length > Width - 4) {
                        String[] words = line.Split(' ');

                        StringBuilder message = new StringBuilder();
                        for (int i = 0; i < words.Length; i++) {
                            if (message.Length > 0) message.Append(' ');
                            message.Append(words[i]);
                            if (i == words.Length - 1 || message.Length + words[i + 1].Length + 1 > Width - 4) {
                                buffer.Append(GetLeftMargin());
                                buffer.AppendLine("| " + String.Format("{0,-" + (Width - 4) + "}", message.ToString()) + " |");
                                message.Clear();
                            }
                        }
                    } else buffer.AppendLine(GetLeftMargin() + "| " + String.Format("{0,-" + (Width - 4) + "}", line) + " |");
                }
            }

            // Choices
            if (Choices != null) for (int i = 0; i < Choices.Length; i++) 
                buffer.AppendLine(GetLeftMargin() + "| " + (i == Index ? ">" : " ") + " " + String.Format("{0,-" + (Width - 6) + "}", Choices[i]) + " |");

            // Footer
            buffer.Append(GetLeftMargin() + "|");
            for (int i = 0; i < Width - 2; i++) {
                if (i == Width - 6) buffer.Append("v");
                else buffer.Append("_");
            }
            buffer.AppendLine("|");

            return buffer.ToString();
        }

        private string GetLeftMargin() {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < Margin[0]; i++) b.Append('\0');
            return b.ToString();
        }
    }
}
