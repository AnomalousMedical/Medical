using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    interface AnalysisEditorComponentParent
    {
        void requestLayout();

        void removeChildComponent(AnalysisEditorComponent child);

        void openVariableBrowser(VariableChosenCallback variableChosenCallback);

        AnalysisEditorComponentParent Parent { get; }

        Widget Widget { get; }

        void requestSelected(AnalysisEditorComponent component);
    }
}
