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

        void openVariableBrowser(VariableChosenCallback variableChosenCallback);

        AnalysisEditorComponentParent Parent { get; }

        ActionBlockEditor OwnerActionBlockEditor { get; }

        Widget Widget { get; }

        void requestSelected(AnalysisEditorComponent component);
    }
}
