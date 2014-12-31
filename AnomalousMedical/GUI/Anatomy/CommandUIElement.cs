using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using MyGUIPlugin;
using Anomalous.GuiFramework;

namespace Medical.GUI
{
    abstract class CommandUIElement : LayoutContainer, IDisposable
    {
        public abstract void Dispose();

        public override void bringToFront()
        {
            
        }

        public override void setAlpha(float alpha)
        {
            
        }

        public abstract void addCommand(AnatomyCommand command);

        public abstract void clearCommands();
    }
}
