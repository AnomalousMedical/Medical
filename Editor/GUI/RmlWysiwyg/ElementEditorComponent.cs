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

        /// <summary>
        /// By default the editors will not save their changes, you must call this function 
        /// from one of the editors when changes are made or they will not appear on the 
        /// final document when the editor is closed.
        /// </summary>
        protected void fireChangesMade()
        {
            parentEditor._changesMade();
        }
    }
}
