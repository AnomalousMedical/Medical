using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public class RmlSlide : Slide
    {
        private String rml;
        
        public RmlSlide()
        {

        }

        public View createView(String name)
        {
            return new RawRmlView(name)
            {
                Rml = this.Rml
            };
        }

        public MvcController createController(String name, String viewName)
        {
            MvcController controller = new MvcController(name);
            RunCommandsAction showCommand = new RunCommandsAction("Show");
            showCommand.addCommand(new ShowViewCommand(viewName));
            controller.Actions.add(showCommand);
            customizeController(controller);
            return controller;
        }

        protected virtual void customizeController(MvcController controller)
        {

        }

        [Editable]
        public String Rml
        {
            get
            {
                return rml;
            }
            set
            {
                rml = value;
            }
        }

        protected RmlSlide(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, ReflectedSaver.DefaultScanner);
        }

        public virtual void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, ReflectedSaver.DefaultScanner);
        }
    }
}
