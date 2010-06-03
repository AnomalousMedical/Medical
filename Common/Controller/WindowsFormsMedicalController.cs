using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCPlatform;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using Engine.Platform;
using Engine.Renderer;

namespace Medical.Controller
{
    public delegate void PumpMessage(ref Message msg);

    public class WindowsFormsMedicalController : MedicalController
    {
        public event PumpMessage PumpMessage;

        //GUI
        private DrawingWindow hiddenEmbedWindow;

        public override void Dispose()
        {
            base.Dispose();
            if (hiddenEmbedWindow != null)
            {
                hiddenEmbedWindow.Dispose();
            }
        }

        public void initialize(OSWindow mainForm)
        {
            hiddenEmbedWindow = new DrawingWindow();
            WindowsMessagePump windowsPump = new WindowsMessagePump();
            windowsPump.MessageReceived += new PumpMessageEvent(win32Timer_MessageReceived);

            initialize(mainForm, windowsPump, createWindow);
            PluginManager.RendererPlugin.PrimaryWindow.setEnabled(false);
        }

        void win32Timer_MessageReceived(ref WinMsg message)
        {
            Message msg = Message.Create(message.hwnd, message.message, message.wParam, message.lParam);
            ManualMessagePump.pumpMessage(ref msg);
            if (PumpMessage != null)
            {
                PumpMessage.Invoke(ref msg);
            }
        }

        /// <summary>
        /// Helper function to create the default window. This is the callback
        /// to the PluginManager.
        /// </summary>
        /// <param name="defaultWindow"></param>
        private void createWindow(out DefaultWindowInfo defaultWindow)
        {
            defaultWindow = new DefaultWindowInfo(hiddenEmbedWindow);
        }
    }
}
