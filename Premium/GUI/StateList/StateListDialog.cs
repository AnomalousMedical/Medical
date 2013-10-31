using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    public class StateListDialog : MDIDialog
    {
        private ImageAtlas imageAtlas = new ImageAtlas("StateListAtlas", new IntSize2(100, 100));
        private SingleSelectButtonGrid stateListBox;
        private Dictionary<MedicalState, ButtonGridItem> entries = new Dictionary<MedicalState, ButtonGridItem>();
        private bool ignoreIndexChanges = false;
        private int lastWidth = -1;
        private int lastHeight = -1;

        private MedicalStateController stateController;

        public StateListDialog(MedicalStateController stateController)
            :base("Medical.GUI.StateList.StateListDialog.layout")
        {
            stateListBox = new SingleSelectButtonGrid(window.findWidget("StateList/ScrollView") as ScrollView, new ButtonGridListLayout());

            foreach (var state in stateController.States)
            {
                addMedicalState(state);
            }

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

            this.Resized += new EventHandler(StateListDialog_Resized);
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

        public override void deserialize(ConfigFile configFile)
        {
            base.deserialize(configFile);
            fixListItemWidth();
        }

        void StateListDialog_Resized(object sender, EventArgs e)
        {
            fixListItemWidth();
        }

        private void fixListItemWidth()
        {
            //Layout only if size changes
            if (window.Width != lastWidth || window.Height != lastHeight)
            {
                lastWidth = window.Width;
                lastHeight = window.Height;
                stateListBox.layout();
            }
        }

        private void addMedicalState(MedicalState state)
        {
            String imageId = imageAtlas.addImage(state, state.Thumbnail);
            ButtonGridItem entry = stateListBox.addItem("", state.Name, imageId);
            entry.UserObject = state;
            entries.Add(state, entry);
            entry.ItemClicked += entry_ItemClicked;
            stateListBox.SelectedItem = entries[state];
        }

        private void removeMedicalState(MedicalState state)
        {
            ButtonGridItem entry;
            if (entries.TryGetValue(state, out entry))
            {
                stateListBox.removeItem(entry);
                entries.Remove(state);
                imageAtlas.removeImage(state);
            }
        }

        void entry_ItemClicked(object sender, EventArgs e)
        {
            changeState();
        }

        private void changeState()
        {
            if (stateListBox.SelectedItem != null && stateController != null)
            {
                stateController.directBlend((MedicalState)stateListBox.SelectedItem.UserObject, 1.0f);
            }
        }

        void stateController_StateAdded(MedicalStateController controller, MedicalState state)
        {
            addMedicalState(state);
        }

        void stateController_StateRemoved(MedicalStateController controller, MedicalState state)
        {
            removeMedicalState(state);
        }

        void stateController_StateUpdated(MedicalState state)
        {
            ButtonGridItem entry = entries[state];
            entry.Caption = state.Name;
            //imageAtlas.replaceImage(state, state.Thumbnail);
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
                ButtonGridItem stateItem;
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
                stateController.destroyState((MedicalState)stateListBox.SelectedItem.UserObject);
            }
        }
    }
}
