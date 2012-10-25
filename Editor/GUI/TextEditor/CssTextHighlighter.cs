using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using Medical.Irony;
using Engine;

namespace Medical.GUI
{
    class CssTextHighlighter : TextHighlighter
    {
        private static CssTextHighlighter instance;
        public static CssTextHighlighter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CssTextHighlighter();
                }
                return instance;
            }
        }

        LanguageData language;
        Parser parser;

        private CssTextHighlighter()
        {
            language = new LanguageData(new CssGrammar());
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

        private String CommentColor = "#348000";

        private String PropertyColor = "#FF0000";

        private String ValueColor = "#0034FF";

        private String SelectorColor = "#800000";

        private String PunctuationColor = "#000000";

        private String OtherTextColor = "#000000";

        private String getColor(Token token)
        {
            if (token.EditorInfo != null)
            {
                switch (token.EditorInfo.Color)
                {
                    case TokenColor.Comment:
                        return CommentColor;
                    case TokenColor.Identifier:
                        switch (token.Terminal.Name)
                        {
                            case CssGrammar.Property:
                                return PropertyColor;
                            case CssGrammar.Value:
                                return ValueColor;
                            default:
                                return SelectorColor;
                        }
                    case TokenColor.Text:
                        return PunctuationColor;
                    default:
                        return OtherTextColor;
                }
            }
            else
            {
                return OtherTextColor;
            }
        }
    }
}
