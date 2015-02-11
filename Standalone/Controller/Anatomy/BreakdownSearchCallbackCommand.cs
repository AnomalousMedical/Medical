using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// This is a specalized version of the callback anatomy command for searching, this way we can override
    /// when these commands are shown in the lucene search, we dont want them to show up in a restriced environment
    /// when premium features are not active.
    /// </summary>
    class BreakdownSearchCallbackCommand : CallbackAnatomyCommand
    {
        private const AnatomyCommandPermissions ShowPermissions = AnatomyCommandPermissions.Unrestricted | AnatomyCommandPermissions.PremiumActive;

        public BreakdownSearchCallbackCommand(String uiText, bool showAnatomyFinder, Action callback)
            :base(uiText, showAnatomyFinder, callback)
        {
            
        }

        public override bool allowDisplay(AnatomyCommandPermissions permissions)
        {
            return (permissions & ShowPermissions) > 0;
        }
    }
}
