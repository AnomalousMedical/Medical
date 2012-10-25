using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using System.Globalization;

namespace Medical.Irony.Grammars
{
    [Language("CSS", "1", "CSS Grammar")]
    public class CssGrammar : Grammar
    {
        public const String SimpleSelectorIdentifier = "Selector";
        public const String PseudoClassIdentifier = "PseudoClass";
        public const String Property = "Property";
        public const String Value = "Value";

        public CssGrammar()
            : base(false)
        {
            //Terminals
            Terminal comment = new CommentTerminal("comment", "/*", "*/");
            NonGrammarTerminals.Add(comment);
            var number = new NumberLiteral("number");
            var stringLiteral = new StringLiteral("string", "\"", StringOptions.None);
            var stringContent = new XmlContentText("StringContent");

            IdentifierTerminal simpleSelectorId = new IdentifierTerminal(SimpleSelectorIdentifier, ".-*", ".-*");
            IdentifierTerminal pseudoClassId = new IdentifierTerminal(PseudoClassIdentifier);
            IdentifierTerminal propertyId = new IdentifierTerminal(Property, "-", "-");
            ValueRule valueId = new ValueRule(Value);

            KeyTerm blockOpen = ToTerm("{");
            KeyTerm blockClose = ToTerm("}");
            KeyTerm semi = ToTerm(";");
            KeyTerm colin = ToTerm(":");
            KeyTerm childChain = ToTerm(">");
            KeyTerm siblingProceed = ToTerm("+");

            //Non Terminals
            NonTerminal css = new NonTerminal("CSS");
            NonTerminal rule = new NonTerminal("Rule");
            NonTerminal rules = new NonTerminal("Rules");
            NonTerminal declaration = new NonTerminal("Declaration");
            NonTerminal declarations = new NonTerminal("Declarations");
            NonTerminal optPseudoClass = new NonTerminal("PseudoClassRule");
            NonTerminal selectorChain = new NonTerminal("SelectorChain");
            NonTerminal selectorChainDelim = new NonTerminal("SelectorChainDelim");

            //Rules
            this.Root = css;

            optPseudoClass.Rule = (colin + pseudoClassId) | Empty;
            selectorChainDelim.Rule = childChain | siblingProceed | Empty;
            selectorChain.Rule = MakePlusRule(selectorChain, selectorChainDelim, simpleSelectorId);

            declaration.Rule = propertyId + colin + valueId + semi;
            declarations.Rule = MakeStarRule(declarations, declaration);

            rule.Rule = selectorChain + optPseudoClass + blockOpen + declarations + blockClose;
            rules.Rule = MakeStarRule(rules, rule);

            css.Rule = rules;

            MarkPunctuation(blockOpen, blockClose, semi, colin);
            MarkTransient(optPseudoClass);
        }
    }

    public class ValueRule : Terminal
    {
        public ValueRule(String name)
            : base(name)
        {

        }

        public override IList<string> GetFirsts()
        {
            return null;
        }

        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            int stopIndex = source.Text.IndexOf(';', source.Location.Position);
            if (stopIndex == source.Location.Position)
            {
                return null;
            }
            if (stopIndex < 0)
            {
                stopIndex = source.Text.Length;
            }
            source.PreviewPosition = stopIndex;
            return source.CreateToken(this.OutputTerminal);
        }
    }
}
