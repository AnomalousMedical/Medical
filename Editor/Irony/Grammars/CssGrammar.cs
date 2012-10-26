using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;
using System.Globalization;

namespace Medical.Irony
{
    [Language("CSS", "1", "CSS Grammar")]
    public class CssGrammar : Grammar
    {
        public const String SimpleSelectorIdentifier = "SimpleSelector";
        public const String PseudoClassIdentifier = "PseudoClass";
        public const String Property = "Property";
        public const String Value = "Value";

        public CssGrammar()
            : base(false)
        {
            //Terminals
            Terminal comment = new CommentTerminal("comment", "/*", "*/");
            NonGrammarTerminals.Add(comment);
            ToTerminatorTerminal attributeSelector = new ToTerminatorTerminal("AttributeSelectorContent", ']');

            IdentifierTerminal simpleSelectorId = new IdentifierTerminal(SimpleSelectorIdentifier, ".-*#", ".-*#");
            IdentifierTerminal pseudoClassId = new IdentifierTerminal(PseudoClassIdentifier);
            IdentifierTerminal propertyId = new IdentifierTerminal(Property, "-", "-");
            ToTerminatorTerminal valueId = new ToTerminatorTerminal(Value, ';');
            valueId.EditorInfo = new TokenEditorInfo(TokenType.Identifier, TokenColor.Identifier, TokenTriggers.None);

            KeyTerm blockOpen = ToTerm("{");
            KeyTerm blockClose = ToTerm("}");
            KeyTerm semi = ToTerm(";");
            KeyTerm colin = ToTerm(":");
            KeyTerm childChain = ToTerm(">");
            KeyTerm siblingProceed = ToTerm("+");
            KeyTerm groupDelim = ToTerm(",");
            KeyTerm attributeSelectOpen = ToTerm("[");
            KeyTerm attributeSelectClose = ToTerm("]");

            //Non Terminals
            NonTerminal css = new NonTerminal("CSS");
            NonTerminal rule = new NonTerminal("Rule");
            NonTerminal rules = new NonTerminal("Rules");
            NonTerminal declaration = new NonTerminal("Declaration");
            NonTerminal declarations = new NonTerminal("Declarations");
            NonTerminal pseudoClassRule = new NonTerminal("PseudoClassRule");
            NonTerminal pseudoClasses = new NonTerminal("PseudoClasses");
            NonTerminal selectorChain = new NonTerminal("Chain");
            NonTerminal selectorChainDelim = new NonTerminal("ChainDelim");
            NonTerminal selectorGroup = new NonTerminal("SelectorGroup");
            NonTerminal selector = new NonTerminal("Selector");
            NonTerminal optAttributeSelector = new NonTerminal("OptAttributeSelector");
            NonTerminal multiAttributeSelector = new NonTerminal("MultiAttributeSelector");
            NonTerminal simpleSelectorWithAttributeSelector = new NonTerminal("simpleSelectorWithAttributeSelector");

            //Rules
            this.Root = css;

            optAttributeSelector.Rule = attributeSelectOpen + attributeSelector + attributeSelectClose;
            multiAttributeSelector.Rule = MakeStarRule(multiAttributeSelector, optAttributeSelector);
            pseudoClassRule.Rule = colin + pseudoClassId;
            pseudoClasses.Rule = MakeStarRule(pseudoClasses, pseudoClassRule);
            simpleSelectorWithAttributeSelector.Rule = simpleSelectorId + multiAttributeSelector;
            selectorChainDelim.Rule = childChain | siblingProceed | Empty;
            selectorChain.Rule = MakePlusRule(selectorChain, selectorChainDelim, simpleSelectorWithAttributeSelector);
            selector.Rule = selectorChain + pseudoClasses;
            selectorGroup.Rule = MakePlusRule(selectorGroup, groupDelim, selector);

            declaration.Rule = propertyId + colin + valueId + semi;
            declarations.Rule = MakeStarRule(declarations, declaration);

            rule.Rule = selectorGroup + blockOpen + declarations + blockClose;
            rules.Rule = MakeStarRule(rules, rule);

            css.Rule = rules;

            MarkPunctuation(blockOpen, blockClose, semi, colin, attributeSelectOpen, attributeSelectClose);
            MarkTransient(pseudoClassRule, multiAttributeSelector, pseudoClasses);
        }
    }
}
