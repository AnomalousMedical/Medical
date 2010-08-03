using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCPlatform;

namespace Medical.Controller
{
    class WinformsMessagePump : OSMessagePump
    {
        private AgnosticMessagePump agnosticPump = new AgnosticMessagePump();

        public void loopCompleted()
        {
            agnosticPump.loopCompleted();
        }

        public void loopStarting()
        {
            agnosticPump.loopStarting();
        }

        public bool processMessages()
        {
            bool processedMessage = agnosticPump.processMessages();
            System.Windows.Forms.Application.DoEvents();
            return processedMessage;
        }
    }
}
