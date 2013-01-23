using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using System.IO;
using System.Xml;
using MyGUIPlugin;
using Logging;
using Medical.Controller.AnomalousMvc;
using Medical.Platform;
using Engine.Platform;
using Medical;

namespace Lecture
{
    public class ShowTypeController : SaveableTypeController<Slideshow>
    {
        public const String Icon = CommonResources.NoIcon;

        public ShowTypeController(EditorController editorController)
            : base(".show", editorController)
        {

        }

        public void save(Slideshow slide, String filename)
        {
            try
            {
                saveObject(filename, slide);
                EditorController.NotificationManager.showNotification(String.Format("{0} saved.", filename), Icon, 2);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("There was an error saving your slideshow to\n'{0}'\nPlease make sure that destination is valid.", filename), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                Log.Error("Could not save slideshow. {0}", ex.Message);
            }
        }
    }
}
