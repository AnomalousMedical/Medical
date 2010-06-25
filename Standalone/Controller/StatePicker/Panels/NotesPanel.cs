using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class NotesPanel : StateWizardPanel
    {
        public NotesPanel(String notesPanelFile, StateWizardPanelController controller)
            :base(notesPanelFile, controller)
        {
            this.LayerState = "MandibleSizeLayers";
            this.NavigationState = "WizardMidlineAnterior";
            this.TextLine1 = "Notes";
        }
    }
}
