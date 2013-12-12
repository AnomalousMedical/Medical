using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;
using Engine;

namespace Medical.Controller.AnomalousMvc
{
    public abstract class View : SaveableEditableItem
    {
        public View(String name)
            :base(name)
        {
            ViewLocation = ViewLocations.Left;
            IsWindow = false;
            Transparent = false;
            LayoutName = GUILocationNames.ContentArea;
            LayoutHint = "Left";
        }

        [Editable]
        public String LayoutName { get; set; }

        [Editable]
        public String LayoutHint { get; set; }

        [Editable]
        private ViewLocations viewLocation;
        public ViewLocations ViewLocation
        {
            get
            {
                return viewLocation;
            }
            set
            {
                viewLocation = value;
                LayoutHint = viewLocation.ToString();
            }
        }

        [Editable]
        public bool IsWindow { get; set; }

        [Editable]
        public bool Transparent { get; set; }

        [EditableAction]
        public String OpeningAction { get; set; }

        [EditableAction]
        public String ClosingAction { get; set; }

        protected View(LoadInfo info)
            :base (info)
        {
            if (info.hasValue("<FillScreen>k__BackingField"))
            {
                LayoutName = GUILocationNames.FullscreenPopup;
            }
            else if (info.hasValue("<ViewLocation>k__BackingField"))
            {
                LayoutName = GUILocationNames.ContentArea;
                LayoutHint = info.GetValue<ViewLocations>("<ViewLocation>k__BackingField", ViewLocations.Left).ToString();
            }
        }

        public enum CustomQueries
        {
            AddControllerForView
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.IconReferenceTag = "MvcContextEditor/IndividualViewIcon";
            editInterface.addCommand(new EditInterfaceCommand("Create Controller", (uiCallback, caller) =>
            {
                uiCallback.runOneWayCustomQuery(CustomQueries.AddControllerForView, this);
            }));
            base.customizeEditInterface(editInterface);
        }
    }
}
