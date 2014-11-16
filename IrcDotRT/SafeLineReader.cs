using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IrcDotRT
{
    // Reads lines from text sources safely; unterminated lines are not returned.
    internal class SafeLineReader
    {
        // Current incomplete line;
        private string currentLine;

        private bool endOfLine = false;

        private char PreviousCharacter()
        {
            return currentLine[currentLine.Length - 1];
        }

        public bool Add(char character)
        {
            if (character == '\n' && PreviousCharacter() == '\r')
                endOfLine = true;

            currentLine += character;

            return endOfLine;
        }

        public string FlushLine()
        {
            string tempLine = currentLine.Substring(0, currentLine.Length - 2);

            currentLine = String.Empty;
            endOfLine = false;

            return tempLine;
        }

        public string SafeFlushLine()
        {
            if (endOfLine)
                return FlushLine();
            else
                return null;
        }
    }
}
