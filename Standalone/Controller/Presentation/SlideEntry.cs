using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using Engine.Saving;
using System.IO;

namespace Medical.Presentation
{
    public class SlideEntry : PresentationEntry
    {
        public SlideEntry()
        {
            
        }

        public void createFile(String defaultRml, ResourceProvider resourceProvider)
        {
            if (!resourceProvider.exists(UniqueName))
            {
                resourceProvider.createDirectory("", UniqueName);
            }
            using (StreamWriter streamWriter = new StreamWriter(resourceProvider.openWriteStream(File)))
            {
                streamWriter.Write(defaultRml);
            }
        }

        public override void addToContext(AnomalousMvcContext mvcContex, NavigationModel navModel)
        {
            RmlView view = new RmlView(UniqueName);
            view.RmlFile = File;
            view.IsWindow = false;
            view.ViewLocation = ViewLocations.Left;
            view.Buttons.add(new CloseButtonDefinition("Close", "__PresentationReserved_Common/Close"));
            view.Buttons.add(new ButtonDefinition("Previous", "__PresentationReserved_Common/MovePrevious"));
            view.Buttons.add(new ButtonDefinition("Next", "__PresentationReserved_Common/MoveNext"));
            mvcContex.Views.add(view);

            mvcContex.Controllers.add(new MvcController(UniqueName, new RunCommandsAction("Show", 
                new ShowViewCommand(view.Name))));

            navModel.addNavigationLink(new NavigationLink(name: UniqueName, action: String.Format("{0}/Show", UniqueName)));
        }

        public override string File
        {
            get
            {
                return String.Format("{0}/{0}.rml", UniqueName);
            }
        }

        public override void getInfo(SaveInfo info)
        {
            base.getInfo(info);
        }

        protected SlideEntry(LoadInfo info)
            :base(info)
        {

        }
    }
}
