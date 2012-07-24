﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;

namespace Medical.Controller.AnomalousMvc
{
    public abstract class View : SaveableEditableItem
    {
        public View(String name)
            :base(name)
        {
            ViewLocation = ViewLocations.Left;
            IsWindow = false;
        }

        [Editable]
        public ViewLocations ViewLocation { get; set; }

        [Editable]
        public bool IsWindow { get; set; }

        [Editable]
        public String ClosingAction { get; set; }

        protected View(LoadInfo info)
            :base (info)
        {

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
