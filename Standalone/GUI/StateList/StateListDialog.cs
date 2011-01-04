using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class StateListPopup : Dialog
    {
        private ImageAtlas imageAtlas = new ImageAtlas("StateListAtlas", new Size2(100.0f, 100.0f), new Size2(512.0f, 512.0f));
        ButtonList stateListBox;
        private Dictionary<MedicalState, ButtonListItem> entries = new Dictionary<MedicalState, ButtonListItem>();
        private bool ignoreIndexChanges = false;

        private MedicalStateController stateController;

        public StateListPopup(MedicalStateController stateController)
            :base("Medical.GUI.StateList.StateListDialog.layout")
        {
            stateListBox = new ButtonList(window.findWidget("StateList/ScrollView") as ScrollView);
            stateListBox.SelectedValueChanged += new EventHandler(stateListBox_SelectedValueChanged);

            Button deleteButton = window.findWidget("StateList/DeleteButton") as Button;
            deleteButton.MouseButtonClick += new MyGUIEvent(deleteButton_MouseButtonClick);

            this.stateController = stateController;
            stateController.StateAdded += stateController_StateAdded;
            stateController.StateRemoved += stateController_StateRemoved;
            stateController.StatesCleared += stateController_StatesCleared;
            stateController.StateChanged += stateController_StateChanged;
            stateController.BlendingStarted += stateController_BlendingStarted;
            stateController.BlendingStopped += stateController_BlendingStopped;
            stateController.StateUpdated += stateController_StateUpdated;
        }

        public override void Dispose()
        {
            stateListBox.Dispose();

            stateController.StateAdded -= stateController_StateAdded;
            stateController.StateRemoved -= stateController_StateRemoved;
            stateController.StatesCleared -= stateController_StatesCleared;
            stateController.StateChanged -= stateController_StateChanged;
            stateController.BlendingStarted -= stateController_BlendingStarted;
            stateController.BlendingStopped -= stateController_BlendingStopped;
            stateController.StateUpdated -= stateController_StateUpdated;

            base.Dispose();
        }

        void stateListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (stateListBox.SelectedIndex != -1)
            {
                stateController.directBlend(stateListBox.SelectedIndex, 1.0f);
            }
        }

        void stateController_StateAdded(MedicalStateController controller, MedicalState state, int index)
        {
            String imageId = imageAtlas.addImage(state, state.Thumbnail);
            ButtonListItem entry = stateListBox.addItem(state.Name, imageId);
            entries.Add(state, entry);
            stateListBox.SelectedItem = entries[state];
        }

        void stateController_StateRemoved(MedicalStateController controller, MedicalState state, int index)
        {
            ButtonListItem entry = entries[state];
            stateListBox.removeItem(entry);
            entries.Remove(state);
            imageAtlas.removeImage(state);
        }

        void stateController_StateUpdated(MedicalState state)
        {
            ButtonListItem entry = entries[state];
            entry.Caption = state.Name;
            imageAtlas.replaceImage(state, state.Thumbnail);
        }

        void stateController_StatesCleared(MedicalStateController controller)
        {
            stateListBox.clear();
            entries.Clear();
        }

        void stateController_StateChanged(MedicalState state)
        {
            if (!ignoreIndexChanges)
            {
                ButtonListItem stateItem;
                entries.TryGetValue(state, out stateItem);
                stateListBox.SelectedItem = stateItem;
            }
        }

        void stateController_BlendingStopped(MedicalStateController controller)
        {
            ignoreIndexChanges = false;
        }

        void stateController_BlendingStarted(MedicalStateController controller)
        {
            ignoreIndexChanges = true;
        }

        void deleteButton_MouseButtonClick(Widget source, EventArgs e)
        {
            if (stateListBox.SelectedItem != null)
            {
                stateController.destroyState(stateListBox.SelectedIndex);
            }
        }
    }
}
