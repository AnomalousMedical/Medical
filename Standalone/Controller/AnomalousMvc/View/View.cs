﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Engine.Editing;
using Engine.Attributes;
using Engine;
using Anomalous.GuiFramework;

namespace Medical.Controller.AnomalousMvc
{
    public abstract class View : SaveableEditableItem
    {
        public View(String name)
            :base(name)
        {
            ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left);
            Transparent = false;
        }

        [EditableElementName(ForceAsProperty=true)]
        public LayoutElementName ElementName { get; set; }

        [Editable]
        public bool Transparent { get; set; }

        [EditableAction]
        public String OpeningAction { get; set; }

        [EditableAction]
        public String ClosingAction { get; set; }

        public enum CustomQueries
        {
            AddControllerForView
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.IconReferenceTag = "MvcContextEditor/IndividualViewIcon";
            editInterface.addCommand(new EditInterfaceCommand("Create Controller", uiCallback =>
            {
                uiCallback.runOneWayCustomQuery(CustomQueries.AddControllerForView, this);
            }));
            base.customizeEditInterface(editInterface);
        }

        protected const int CurrentVersion = 1;

        protected View(LoadInfo info)
            : base(info)
        {
            if (info.Version < CurrentVersion)
            {
                if (info.Version < 1)
                {
                    if (info.hasValue("<ViewLocation>k__BackingField"))
                    {
                        switch (info.GetValue<ViewLocations>("<ViewLocation>k__BackingField", ViewLocations.Left))
                        {
                            case ViewLocations.Left:
                                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Left);
                                break;
                            case ViewLocations.Right:
                                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Right);
                                break;
                            case ViewLocations.Top:
                                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Top);
                                break;
                            case ViewLocations.Bottom:
                                ElementName = new BorderLayoutElementName(GUILocationNames.ContentArea, BorderLayoutLocations.Bottom);
                                break;
                            case ViewLocations.Floating:
                                ElementName = new LayoutElementName(GUILocationNames.FullscreenPopup);
                                break;
                        }
                    }
                }
            }
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
            info.Version = CurrentVersion;
        }
    }
}
