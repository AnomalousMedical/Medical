using Engine.ObjectManagement;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// An anatomy command that will execute a given Action when run.
    /// </summary>
    public class CallbackAnatomyCommand : AbstractExecuteAnatomyCommand
    {
        private String uiText;
        private Action callback;

        public CallbackAnatomyCommand(String uiText, Action callback)
        {
            this.uiText = uiText;
            this.callback = callback;
        }

        public override bool link(SimObject owner, AnatomyIdentifier parentAnatomy, ref string errorMessage)
        {
            return true;
        }

        public override void destroy()
        {
            
        }

        public override AnatomyCommandUIType UIType
        {
            get
            {
                return AnatomyCommandUIType.Executable;
            }
        }

        public override string UIText
        {
            get
            {
                return uiText;
            }
        }

        public override void execute()
        {
            callback();
        }

        public override void getInfo(SaveInfo info)
        {
            
        }
    }
}
