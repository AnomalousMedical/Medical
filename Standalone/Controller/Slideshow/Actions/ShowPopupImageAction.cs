using Anomalous.GuiFramework;
using Engine.Attributes;
using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Medical.SlideshowActions
{
    public class ShowPopupImageAction : SlideAction
    { 
        [DoNotSave]
        private EditInterface editInterface;

        private String name;
        private String imageName;

        public ShowPopupImageAction(String name)
        {
            this.Name = name;
        }

        public override EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = new EditInterface("Show Popup Image");
            }
            return editInterface;
        }

        public override void addToController(Slide slide, MvcController controller, AnomalousMvcContext context)
        {
            String closeCommandName = String.Format("CloseImagePopup__{0}", name);

            RunCommandsAction openAction = new RunCommandsAction(name, new ShowViewCommand(name));
            controller.Actions.add(openAction);

            RunCommandsAction closeAction = new RunCommandsAction(closeCommandName, new CloseViewCommand());
            controller.Actions.add(closeAction);

            RawRmlView popupView = new RawRmlView(name);
            popupView.ElementName = new LayoutElementName(GUILocationNames.ContentAreaPopup);
            popupView.Buttons.add(new CloseButtonDefinition(String.Format("CloseButton__{0}", name), String.Format("{0}/{1}", slide.UniqueName, closeCommandName)));
            popupView.Rml = String.Format(ImageRml, imageName);
            popupView.FakePath = Path.Combine(slide.UniqueName, "ImagePopup.rml");
            context.Views.add(popupView);
        }

        public override void setupAction(Slide slide, RunCommandsAction action)
        {
            //Does not setup single action, this does nothing
        }

        public override void cleanup(Slide slide, CleanupInfo info, ResourceProvider resourceProvider)
        {
            info.claimFile(Path.Combine(slide.UniqueName, imageName));
            base.cleanup(slide, info, resourceProvider);
        }

        public override string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public String ImageName
        {
            get
            {
                return imageName;
            }
            set
            {
                imageName = value;
            }
        }

        protected ShowPopupImageAction(LoadInfo info)
            : base(info)
        {

        }

        private const String ImageRml = @"<rml>
  <head>
    <link type=""text/template"" href=""/MasterTemplate.trml"" />
    <link type = ""text/rcss"" href=""SlideStyle.rcss"" />
  </head>
  <body template = ""MasterTemplate"">
    <img src=""{0}"" style=""width:100%;""/>
  </body>
</rml>";
    }
}
