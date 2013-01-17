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

namespace Medical
{
    public class SlideTypeController : SaveableTypeController<Slide>
    {
        public const String Icon = CommonResources.NoIcon;
        public delegate Slide CreateSlide(String filePath);

        public SlideTypeController(EditorController editorController)
            : base(".sld", editorController)
        {

        }

        public String createSlideFileSafely(String filePath, CreateSlide createSlideCb)
        {
            filePath = Path.ChangeExtension(filePath, Extension);
            if (EditorController.ResourceProvider.exists(filePath))
            {
                MessageBox.show(String.Format("Are you sure you want to override {0}?", filePath), "Override", MessageBoxStyle.IconQuest | MessageBoxStyle.Yes | MessageBoxStyle.No, delegate(MessageBoxStyle overrideResult)
                {
                    if (overrideResult == MessageBoxStyle.Yes)
                    {
                        createNewSlide(filePath, createSlideCb);
                    }
                });
            }
            else
            {
                createNewSlide(filePath, createSlideCb);
            }
            return filePath;
        }

        void createNewSlide(String filePath, CreateSlide createSlideCb)
        {
            Slide slide = createSlideCb(filePath);
            creatingNewFile(filePath);
            saveObject(filePath, slide);
            openEditor(filePath);
        }

        public void save(Slide slide, String filename)
        {
            try
            {
                saveObject(filename, slide);
                EditorController.NotificationManager.showNotification(String.Format("{0} saved.", filename), Icon, 2);
            }
            catch (Exception ex)
            {
                MessageBox.show(String.Format("There was an error saving your timeline to\n'{0}'\nPlease make sure that destination is valid.", filename), "Error", MessageBoxStyle.IconError | MessageBoxStyle.Ok);
                Log.Error("Could not save timeline. {0}", ex.Message);
            }
        }
    }
}
