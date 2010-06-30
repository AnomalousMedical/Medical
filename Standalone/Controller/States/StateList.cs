using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;

namespace Medical.GUI
{
    class StateList : IDisposable
    {
        private MedicalStateController stateController;
        ButtonList stateListBox;
        private Dictionary<MedicalState, ButtonListItem> entries = new Dictionary<MedicalState, ButtonListItem>();
        private bool ignoreIndexChanges = false;
        private Layout layout;
        private Widget mainWidget;
        private MyGUILayoutContainer layoutContainer;

        public StateList(String stateListFile, MedicalStateController stateController)
        {
            this.stateController = stateController;
            stateController.StateAdded += new MedicalStateAdded(stateController_StateAdded);
            stateController.StateRemoved += new MedicalStateRemoved(stateController_StateRemoved);
            stateController.StatesCleared += new MedicalStateEvent(stateController_StatesCleared);
            stateController.StateChanged += new MedicalStateStatusUpdate(stateController_StateChanged);
            stateController.BlendingStarted += new MedicalStateEvent(stateController_BlendingStarted);
            stateController.BlendingStopped += new MedicalStateEvent(stateController_BlendingStopped);
            stateController.StateUpdated += new MedicalStateStatusUpdate(stateController_StateUpdated);

            layout = LayoutManager.Instance.loadLayout(stateListFile);
            mainWidget = layout.getWidget(0);
            mainWidget.Visible = false;
            layoutContainer = new MyGUILayoutContainer(mainWidget);

            stateListBox = new ButtonList(mainWidget.findWidget("StateList/ScrollView") as ScrollView);

            stateListBox.SelectedValueChanged += new EventHandler(stateListBox_SelectedValueChanged);
            //stateListBox.ListBox.MouseUp += new MouseEventHandler(ListBox_MouseUp);
            //stateListBox.ListBox.KeyUp += new KeyEventHandler(ListBox_KeyUp);

            //deleteCommand.Execute += new EventHandler(deleteCommand_Execute);
        }

        public void Dispose()
        {
            LayoutManager.Instance.unloadLayout(layout);
        }

        public LayoutContainer LayoutContainer
        {
            get
            {
                return layoutContainer;
            }
        }

        //void ListBox_MouseUp(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Right)
        //    {
        //        stateListBox.SelectedIndex = stateListBox.ListBox.IndexFromPoint(e.Location);
        //        contextMenu.Show(stateListBox.PointToScreen(e.Location));
        //    }
        //}

        void stateListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (stateListBox.SelectedIndex != -1)
            {
                //stateController.blendTo(stateListBox.SelectedIndices[0], 1.0f);
                stateController.directBlend(stateListBox.SelectedIndex, 1.0f);
            }
        }

        //void ListBox_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Delete)
        //    {
        //        deleteCommand_Execute(this, EventArgs.Empty);
        //    }
        //}

        void deleteCommand_Execute(object sender, EventArgs e)
        {
            int selectedIndex = stateListBox.SelectedIndex;
            stateController.destroyState(selectedIndex);
            if (selectedIndex < stateListBox.ItemCount)
            {
                stateListBox.SelectedIndex = selectedIndex;
            }
            else if (stateListBox.ItemCount > 0)
            {
                if (selectedIndex > 0)
                {
                    stateListBox.SelectedIndex = selectedIndex - 1;
                }
            }
        }

        void stateController_StateAdded(MedicalStateController controller, MedicalState state, int index)
        {
            ButtonListItem entry = stateListBox.addItem(state.Name);
            //entry.Image = state.Thumbnail;
            entries.Add(state, entry);
        }

        void stateController_StateRemoved(MedicalStateController controller, MedicalState state, int index)
        {
            ButtonListItem entry = entries[state];
            stateListBox.removeItem(entry);
            entries.Remove(state);
        }

        void stateController_StateUpdated(MedicalState state)
        {
            ButtonListItem entry = entries[state];
            entry.Caption = state.Name;
            //entry.Image = state.Thumbnail;
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
                stateListBox.SelectedItem = entries[state];
            }
        }

        void stateController_BlendingStopped(MedicalStateController controller)
        {
            ignoreIndexChanges = false;
            //StatusController.TaskCompleted();
        }

        void stateController_BlendingStarted(MedicalStateController controller)
        {
            ignoreIndexChanges = true;
            //StatusController.SetStatus("Blending...");
        }
    }
}
