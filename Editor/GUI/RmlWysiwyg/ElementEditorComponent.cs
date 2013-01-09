using Engine;
using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    class ElementEditorComponent : Component
    {
        protected RmlElementEditor parentEditor;

        public ElementEditorComponent(String layoutFile, String name)
            :base(layoutFile)
        {
            this.Name = name;
        }

        public virtual void attachToParent(RmlElementEditor parentEditor, Widget parent)
        {
            this.parentEditor = parentEditor;
            widget.Visible = false;
            IntCoord clientCoord = parent.ClientCoord;
            widget.setSize(clientCoord.width, clientCoord.height);
            widget.Align = Align.Stretch;
            widget.attachToWidget(parent);
            widget.Visible = true;
        }

        public String Name { get; set; }
    }
}
