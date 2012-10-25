using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace Medical.Irony
{
    public class ToTerminatorTerminal : Terminal
    {
        private char terminateChar;

        public ToTerminatorTerminal(String name, char terminateChar)
            : base(name)
        {
            this.terminateChar = terminateChar;
        }

        public override IList<string> GetFirsts()
        {
            return null;
        }

        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            int stopIndex = source.Text.IndexOf(terminateChar, source.Location.Position);
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
