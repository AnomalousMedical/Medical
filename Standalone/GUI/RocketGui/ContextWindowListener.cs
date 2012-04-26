using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Platform;
using libRocketPlugin;

namespace Medical.GUI
{
    class ContextWindowListener : OSWindowListener
    {
        private Context context;

        public ContextWindowListener(Context context)
        {
            this.context = context;
        }

        public void closed(OSWindow window)
        {
            
        }

        public void closing(OSWindow window)
        {
            
        }

        public void focusChanged(OSWindow window)
        {
            
        }

        public void moved(OSWindow window)
        {
            
        }

        public void resized(OSWindow window)
        {
            context.Dimensions = new Vector2i(window.WindowWidth, window.WindowHeight);
        }
    }
}
