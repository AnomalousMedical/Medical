using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;

namespace Medical.Presentation
{
    class SlideEntry : PresentationEntry
    {
        public SlideEntry(String name)
            :base(name)
        {
            
        }

        public override void addToContext(AnomalousMvcContext mvcContex, NavigationModel navModel)
        {
            RmlView view = new RmlView(Name);
            view.RmlFile = String.Format("{0}/{0}.rml", Name);
            view.IsWindow = false;
            view.ViewLocation = ViewLocations.Left;
            view.Buttons.add(new CloseButtonDefinition("__PresentationReserved_Common/Close"));
            view.Buttons.add(new ButtonDefinition("Previous", "__PresentationReserved_Common/MoveNext"));
            view.Buttons.add(new ButtonDefinition("Next", "__PresentationReserved_Common/MovePrevious"));
            mvcContex.Views.add(view);

            mvcContex.Controllers.add(new MvcController(Name, new RunCommandsAction("Show", 
                new ShowViewCommand(view.Name))));

            navModel.addNavigationLink(new NavigationLink(name: Name, action: String.Format("{0}/Show", Name)));
        }
    }
}
