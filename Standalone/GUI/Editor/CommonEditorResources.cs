using Medical.GUI;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public static class CommonEditorResources
    {
        private static bool load = true;

        public static void initialize(StandaloneController standaloneController)
        {
            if (load)
            {
                load = false;

                ResourceManager.Instance.load("Medical.Resources.MoreEditorIcons.xml");

                standaloneController.ViewHostFactory.addFactory(new RmlWysiwygComponentFactory());
                standaloneController.ViewHostFactory.addFactory(new DragAndDropFactory());
                standaloneController.ViewHostFactory.addFactory(new GenericEditorComponentFactory());
                standaloneController.ViewHostFactory.addFactory(new PropTimelineFactory(standaloneController.Clipboard));
                standaloneController.ViewHostFactory.addFactory(new TimelineComponentFactory(standaloneController.Clipboard));
                standaloneController.ViewHostFactory.addFactory(new MovementSequenceEditorFactory(standaloneController.MovementSequenceController, standaloneController.Clipboard));

                PropertiesForm.addFormCreationMethod(typeof(ChangeHandPosition), (property, parentWidget) =>
                {
                    return new PoseableHandProperties(property, parentWidget);
                });
            }
        }
    }
}
