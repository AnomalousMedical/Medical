﻿using System;
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

        AnalysisEditorComponentParent Parent { get; }

        Widget Widget { get; }
    }
}