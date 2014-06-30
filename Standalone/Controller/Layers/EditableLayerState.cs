using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;
using Engine.Reflection;

namespace Medical
{
    /// <summary>
    /// This subclass adds editing functions to the LayerState. Use this
    /// subclass anywhere you need a LayerState to show up on an editing UI.
    /// </summary>
    public class EditableLayerState : LayerState, EditInterfaceOverride
    {
        public EditableLayerState()
            :base()
        {

        }

        private EditInterface editInterface;

        public EditInterface getEditInterface(string memberName, MemberScanner scanner)
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, scanner, memberName, null);
                editInterface.addCommand(new EditInterfaceCommand("Capture Layers", captureLayerState));
                editInterface.addCommand(new EditInterfaceCommand("Preview", preview));
            }
            return editInterface;
        }

        private void captureLayerState(EditUICallback callback, EditInterfaceCommand caller)
        {
            captureState();
        }

        private void preview(EditUICallback callback, EditInterfaceCommand caller)
        {
            apply();
        }

        #region Saving

        protected EditableLayerState(LoadInfo info)
            :base(info)
        {
            
        }

        #endregion
    }
}
