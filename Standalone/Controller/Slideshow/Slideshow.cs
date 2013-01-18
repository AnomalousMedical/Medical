using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Medical
{
    public class Slideshow : Saveable
    {
        private List<Slide> slides = new List<Slide>();

        public Slideshow()
        {

        }

        public AnomalousMvcContext createContext(ResourceProvider resourceProvider)
        {
            AnomalousMvcContext mvcContext;
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream resourceStream = assembly.GetManifestResourceStream(SlideshowProps.BaseContextProperties.File))
            {
                mvcContext = SharedXmlSaver.Load<AnomalousMvcContext>(resourceStream);
            }
            NavigationModel navModel = (NavigationModel)mvcContext.Models[SlideshowProps.BaseContextProperties.NavigationModel];
            int i = 0;
            foreach(Slide slide in slides)
            {
                String slideName = "Slide" + i++;
                View view = slide.createView(slideName);
                mvcContext.Views.add(view);

                MvcController controller = slide.createController(slideName, view.Name);
                mvcContext.Controllers.add(controller);
                NavigationLink link = new NavigationLink(slideName, null, slideName + "/Show");
                navModel.addNavigationLink(link);
            }
            return mvcContext;
        }

        protected Slideshow(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, ReflectedSaver.DefaultScanner);
            info.RebuildList("Slides", slides);
        }

        public virtual void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, ReflectedSaver.DefaultScanner);
            info.ExtractList("Slides", slides);
        }
    }
}
