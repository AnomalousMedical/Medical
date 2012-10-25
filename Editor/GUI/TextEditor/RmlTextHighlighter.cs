using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Medical.Irony;

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

        private String getColor(Token token)
        {
            if (token.EditorInfo != null)
            {
                switch (token.EditorInfo.Color)
                {
                    case TokenColor.Comment:
                        return "#348000";
                    case TokenColor.Identifier:
                        if (token.Terminal.Name == XmlGrammar.AttributeIdentifier)
                        {
                            return "#FF0000";
                        }
                        return "#800000";
                    case TokenColor.Keyword:
                        return "#800000";
                    case TokenColor.Number:
                        return "#000000";
                    case TokenColor.String:
                        return "#0034FF";
                    case TokenColor.Text:
                        return "#0034FF";
                    default:
                        return "#000000";
                }
            }
            else
            {
                return "#000000";
            }
        }
    }
}
