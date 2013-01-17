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
            foreach (String file in resourceProvider.listFiles("*.sld"))
            {
                using (Stream stream = resourceProvider.openFile(file))
                {
                    Slide slide = SharedXmlSaver.Load<Slide>(stream);
                    View view = slide.createView(file);
                    mvcContext.Views.add(view);

                    MvcController controller = slide.createController(file, view.Name);
                    mvcContext.Controllers.add(controller);
                    NavigationLink link = new NavigationLink(file, null, file + "/Show");
                    navModel.addNavigationLink(link);
                }
            }
            return mvcContext;
        }

        protected Slideshow(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, ReflectedSaver.DefaultScanner);
        }

        public virtual void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, ReflectedSaver.DefaultScanner);
        }
    }
}
