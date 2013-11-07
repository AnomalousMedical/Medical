﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;
using Engine.Reflection;
using Medical.Editor;
using Medical.Controller.AnomalousMvc;
using Engine.Attributes;
using Engine;

namespace Medical.GUI.AnomalousMvc
{
    public delegate RocketEventController CreateCustomEventControllerDelegate(AnomalousMvcContext mvcContext, ViewHost viewHost);

    public class RmlView : MyGUIView
    {
        private float scaleFactor = -1.0f;

        public event Action<RmlView, RmlWidgetComponent> ComponentCreated;

        [DoNotSave]
        private CreateCustomEventControllerDelegate createCustomEventController;

        public RmlView(String name)
            :base(name)
        {
            RmlFile = name + ".rml";
        }

        [EditableFile("*.rml", "Rml Files")]
        public String RmlFile { get; set; }

        [Editable]
        public float ScaleFactor
        {
            get
            {
                return scaleFactor;
            }
            set
            {
                scaleFactor = value;
            }
        }

        public float FinalScaleFactor
        {
            get
            {
                float scale = ScaleHelper.ScaleFactor;
                if (scaleFactor > 0)
                {
                    scale *= scaleFactor;
                }
                return scale;
            }
        }

        protected override void customizeEditInterface(EditInterface editInterface)
        {
            editInterface.addCommand(new EditInterfaceCommand("Edit", openFileInViewer));
            base.customizeEditInterface(editInterface);
        }

        public CreateCustomEventControllerDelegate CreateCustomEventController
        {
            get
            {
                return createCustomEventController;
            }
            set
            {
                createCustomEventController = value;
            }
        }

        public enum CustomQueries
        {
            OpenFileInRmlViewer
        }

        private void openFileInViewer(EditUICallback callback, EditInterfaceCommand command)
        {
            callback.runOneWayCustomQuery(CustomQueries.OpenFileInRmlViewer, RmlFile);
        }

        internal void _fireComponentCreated(RmlWidgetComponent component)
        {
            if (ComponentCreated != null)
            {
                ComponentCreated.Invoke(this, component);
            }
        }

        internal RocketEventController createRocketEventController(AnomalousMvcContext mvcContext, ViewHost viewHost)
        {
            if (createCustomEventController != null)
            {
                return createCustomEventController(mvcContext, viewHost);
            }
            return new RmlMvcEventController(mvcContext, viewHost);
        }

        protected RmlView(LoadInfo info)
            :base (info)
        {

        }
    }
}
