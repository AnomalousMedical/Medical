using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class DocumentBuilder
    {
        private StringBuilder stringBuilder;

        public DocumentBuilder()
        {
            stringBuilder = new StringBuilder();
        }

        public DocumentBuilder(int capacity)
        {
            stringBuilder = new StringBuilder(capacity);
        }

        public void writeLine(String text)
        {
            stringBuilder.AppendLine(text);
        }

        public void writeLine(String text, params Object[] args)
        {
            stringBuilder.AppendFormat(text, args);
            stringBuilder.AppendLine();
        }

        public void breakLine()
        {
            stringBuilder.AppendLine();
        }

        public void writeSentence(String text)
        {
            stringBuilder.Append(text);
            stringBuilder.Append("  ");
        }

        public void writeSentence(String text, params Object[] args)
        {
            stringBuilder.AppendFormat(text, args);
            stringBuilder.Append("  ");
        }

        public void startParagraph()
        {
            stringBuilder.Append('\t');
        }

        public void endParagraph()
        {
            stringBuilder.Append("\n\n");
        }

        public override string ToString()
        {
            return stringBuilder.ToString();
        }
    }
}
