using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    interface ElementEditorComponent
    {
        void attachToParent(RmlElementEditor parentEditor, Widget parent);

        String Name { get; }
    }
}
