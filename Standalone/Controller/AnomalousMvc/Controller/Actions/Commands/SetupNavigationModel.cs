using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    class SetupNavigationModel : ActionCommand
    {
        public SetupNavigationModel()
        {
            NavigationModel = new NavigationModel();
        }

        public override void execute(AnomalousMvcContext context)
        {
            NavigationModel.resetIndex();
            context.addModel(NavigationModel.Name, NavigationModel);
        }

        protected override void createEditInterface()
        {
            editInterface = NavigationModel.getEditInterface("Setup Navigation");
        }

        public NavigationModel NavigationModel { get; set; }

        public override string Type
        {
            get
            {
                return "Setup Navigation";
            }
        }

        protected SetupNavigationModel(LoadInfo info)
            :base(info)
        {

        }
    }
}
