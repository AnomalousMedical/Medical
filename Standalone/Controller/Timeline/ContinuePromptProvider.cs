using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public delegate void ContinuePromptCallback();

    public abstract class ContinuePromptProvider
    {
        private ContinuePromptCallback callback;

        public void showPrompt(ContinuePromptCallback callback)
        {
            this.callback = callback;
            doShowPrompt();
        }

        public void hidePrompt()
        {
            doHidePrompt();
        }

        protected void executeCallback()
        {
            hidePrompt();
            if (callback != null)
            {
                callback.Invoke();
            }
            callback = null;
        }

        protected abstract void doShowPrompt();

        protected abstract void doHidePrompt();
    }
}
