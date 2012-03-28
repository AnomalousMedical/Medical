using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;

namespace Medical.GUI
{
    class StateListGUI : AbstractFullscreenGUIPopup
    {
        private ImageAtlas imageAtlas = new ImageAtlas("StateListAtlas", new Size2(100.0f, 100.0f), new Size2(512.0f, 512.0f));
        private ButtonGrid stateListBox;
        private Dictionary<MedicalState, ButtonGridItem> entries = new Dictionary<MedicalState, ButtonGridItem>();
        private bool ignoreIndexChanges = false;

        ImageBox trash;

        private MedicalStateController stateController;

        private ImageBox dragIconPreview;
        private IntVector2 dragMouseStartPosition;

        public StateListGUI(MedicalStateController stateController, GUIManager guiManager)
            :base("Medical.GUI.StateList.StateListGUI.layout", guiManager)
        {
            stateListBox = new ButtonGrid(widget.findWidget("StatesList") as ScrollView);
            stateListBox.HighlightSelectedButton = false;

            Button closeButton = widget.findWidget("CloseButton") as Button;
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            this.stateController = stateController;
            stateController.StateAdded += stateController_StateAdded;
            stateController.StateRemoved += stateController_StateRemoved;
            stateController.StatesCleared += stateController_StatesCleared;
            stateController.StateChanged += stateController_StateChanged;
            stateController.BlendingStarted += stateController_BlendingStarted;
            stateController.BlendingStopped += stateController_BlendingStopped;
            stateController.StateUpdated += stateController_StateUpdated;

            trash = (ImageBox)widget.findWidget("TrashPanel");
            trash.Visible = false;

            dragIconPreview = (ImageBox)Gui.Instance.createWidgetT("ImageBox", "ImageBox", 0, 0, 100, 100, Align.Default, "Info", "BookmarksDragIconPreview");
            dragIconPreview.Visible = false;
        }

        public override void Dispose()
        {
            stateListBox.Dispose();

            Gui.Instance.destroyWidget(dragIconPreview);

            stateController.StateAdded -= stateController_StateAdded;
            stateController.StateRemoved -= stateController_StateRemoved;
            stateController.StatesCleared -= stateController_StatesCleared;
            stateController.StateChanged -= stateController_StateChanged;
            stateController.BlendingStarted -= stateController_BlendingStarted;
            stateController.BlendingStopped -= stateController_BlendingStopped;
            stateController.StateUpdated -= stateController_StateUpdated;

            base.Dispose();
        }

        public override void setSize(int width, int height)
        {
            base.setSize(width, height);
            stateListBox.resizeAndLayout(width);
        }

        void stateController_StateAdded(MedicalStateController controller, MedicalState state)
        {
            String imageId = imageAtlas.addImage(state, state.Thumbnail);
            ButtonGridItem entry = stateListBox.addItem("", state.Name, imageId);
            entry.UserObject = state;
            entries.Add(state, entry);
            stateListBox.SelectedItem = entries[state];

            entry.MouseDrag += new EventDelegate<ButtonGridItem, MouseEventArgs>(item_MouseDrag);
            entry.MouseButtonReleased += new EventDelegate<ButtonGridItem, MouseEventArgs>(item_MouseButtonReleased);
            entry.MouseButtonPressed += new EventDelegate<ButtonGridItem, MouseEventArgs>(item_MouseButtonPressed);
            entry.ItemClicked += new EventHandler(entry_ItemClicked);
        }

        void entry_ItemClicked(object sender, EventArgs e)
        {
            if (stateListBox.SelectedItem != null)
            {
                stateController.directBlend((MedicalState)stateListBox.SelectedItem.UserObject, 1.0f);
            }
            this.hide();
        }

        void stateController_StateRemoved(MedicalStateController controller, MedicalState state)
        {
            ButtonGridItem entry = entries[state];
            stateListBox.removeItem(entry);
            entries.Remove(state);
            imageAtlas.removeImage(state);
        }

        void stateController_StateUpdated(MedicalState state)
        {
            ButtonGridItem entry = entries[state];
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

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }

        void item_MouseButtonPressed(ButtonGridItem source, MouseEventArgs arg)
        {
            dragMouseStartPosition = arg.Position;
        }

        void item_MouseButtonReleased(ButtonGridItem source, MouseEventArgs arg)
        {
            trash.Visible = false;
            dragIconPreview.Visible = false;
            IntVector2 mousePos = arg.Position;
            if (trash.contains(mousePos.x, mousePos.y))
            {
                stateController.destroyState((MedicalState)source.UserObject);
            }
        }

        void item_MouseDrag(ButtonGridItem source, MouseEventArgs arg)
        {
            dragIconPreview.setPosition(arg.Position.x - (dragIconPreview.Width / 2), arg.Position.y - (int)(dragIconPreview.Height * .75f));
            if (!dragIconPreview.Visible && (Math.Abs(dragMouseStartPosition.x - arg.Position.x) > 5 || Math.Abs(dragMouseStartPosition.y - arg.Position.y) > 5))
            {
                trash.Visible = true;
                dragIconPreview.Visible = true;
                dragIconPreview.setItemResource(imageAtlas.getImageId(source.UserObject));// bookmarksController.createThumbnail((Bookmark)source.UserObject));
                LayerManager.Instance.upLayerItem(dragIconPreview);
            }
            if (trash.contains(arg.Position.x, arg.Position.y))
            {
                trash.setItemName("Highlight");
            }
            else
            {
                trash.setItemName("Normal");
            }
        }
    }
}
