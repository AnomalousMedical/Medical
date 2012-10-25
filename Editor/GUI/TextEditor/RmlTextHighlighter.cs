using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Medical.Irony;
using Engine;

namespace Medical.GUI
{
    class RmlTextHighlighter : TextHighlighter
    {
        private static RmlTextHighlighter instance;
        public static RmlTextHighlighter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RmlTextHighlighter();
                }
                return instance;
            }
        }

        LanguageData language;
        Parser parser;

        private RmlTextHighlighter()
        {
            language = new LanguageData(new XmlGrammar());
            parser = new Parser(language);
        }

        public void colorString(StringBuilder input)
        {
            ParseTree parseTree = parser.Parse(input.ToString());

            int additionalOffset = 0;
            foreach (Token token in parseTree.Tokens)
            {
                int tokenStart = token.Location.Position;
                int tokenEnd = tokenStart + token.Length;

                input.Insert(tokenStart + additionalOffset, getColor(token));
                additionalOffset += 7;
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
        }

        private Color backgroundColor = Color.FromHexString("191919");

        private String CommentColor = "#B675C6";

        private String AttributeColor = "#99B2FF";

        private String ElementColor = "#FFC66D";

        private String StringColor = "#14EF35";

        private String TextColor = "#FFFFFF";

        private String PunctuationColor = "#CC7832";

        private String getColor(Token token)
        {
            if (token.EditorInfo != null)
            {
                switch (token.EditorInfo.Color)
                {
                    case TokenColor.Comment:
                        return CommentColor;
                    case TokenColor.Identifier:
                        if (token.Terminal.Name == XmlGrammar.AttributeIdentifier)
                        {
                            return AttributeColor;
                        }
                        return ElementColor;
                    case TokenColor.Keyword:
                        return ElementColor;
                    case TokenColor.String:
                        return StringColor;
                    case TokenColor.Text:
                        return PunctuationColor;
                    default:
                        return TextColor;
                }
            }
            else
            {
                return TextColor;
            }
        }
    }
}
