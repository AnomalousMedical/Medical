using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.Controller.AnomalousMvc;
using Logging;

namespace Medical.GUI.AnomalousMvc
{
    class NavigationBugComponent : LayoutComponent
    {
        private NavigationBugView view;
        private AnomalousMvcContext context;
        private NavigationModel navModel;
        private Button nextButton;
        private Button previousButton;
        private Button closeButton;

        public NavigationBugComponent(NavigationBugView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
            : base("Medical.GUI.AnomalousMvc.NavigationBug.NavigationBugComponent.layout", viewHost)
        {
            this.view = view;
            this.context = context;

            nextButton = (Button)widget.findWidget("Next");
            previousButton = (Button)widget.findWidget("Previous");
            closeButton = (Button)widget.findWidget("Close");
            closeButton.MouseButtonClick += closeButton_MouseButtonClick;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void opening()
        {
            navModel = context.getModel<NavigationModel>(view.NavigationModelName);
            if (navModel != null)
            {
                nextButton.MouseButtonClick += nextButton_MouseButtonClick;
                previousButton.MouseButtonClick += previousButton_MouseButtonClick;
                navModel.CurrentIndexChanged += navModel_CurrentIndexChanged;
                navModel_CurrentIndexChanged(navModel);
            }
            else
            {
                Log.Warning("Cannot setup navigation gui for navigation model '{0}' because it cannot be found.", view.NavigationModelName);
            }
            base.opening();
        }

        public override void closing()
        {
            base.closing();
            navModel.CurrentIndexChanged -= navModel_CurrentIndexChanged;
            nextButton.MouseButtonClick -= nextButton_MouseButtonClick;
            previousButton.MouseButtonClick -= previousButton_MouseButtonClick;
        }

        void navModel_CurrentIndexChanged(NavigationModel navModel)
        {
            previousButton.Enabled = navModel.HasPrevious;
            nextButton.Enabled = navModel.HasNext;
        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            context.runAction(view.CloseButtonAction, this.ViewHost);
        }

        void previousButton_MouseButtonClick(Widget source, EventArgs e)
        {
            context.runAction(view.PreviousButtonAction, this.ViewHost);
        }

        void nextButton_MouseButtonClick(Widget source, EventArgs e)
        {
            context.runAction(view.NextButtonAction, this.ViewHost);
        }
    }
}
