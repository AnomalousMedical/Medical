using Engine.Attributes;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller.AnomalousMvc
{
    /// <summary>
    /// This class will maintain the state for a back button. This was designed to be natural 
    /// for editing, so all you have to do is add a RecordBackAction to the show command for 
    /// the view you wish to be able to go back to.
    /// 
    /// As a result it will track a "currentAction" along with the actual stack of previous actions, it has
    /// been hardened against flowing off the front of the stack.
    /// </summary>
    public class BackStackModel : MvcModel
    {
        public const String DefaultName = "DefaultNavigationStack";

        private Stack<String> actionStack = new Stack<string>();

        [DoNotSave]
        private String currentAction;

        [DoNotSave]
        private bool allowSettingAction = true;

        public BackStackModel(String name)
            :base(name)
        {
            
        }

        public void ignoreNextCurrentAction()
        {
            allowSettingAction = false;
        }

        public void setCurrentAction(String action)
        {
            if (allowSettingAction)
            {
                if (currentAction != null)
                {
                    actionStack.Push(currentAction);
                }
                currentAction = action;
            }
            allowSettingAction = true;
        }

        public String getPreviousAction()
        {
            if (actionStack.Count == 0)
            {
                return null;
            }
            currentAction = actionStack.Pop();
            return currentAction;
        }

        protected BackStackModel(LoadInfo info)
            :base(info)
        {
            
        }
    }
}
