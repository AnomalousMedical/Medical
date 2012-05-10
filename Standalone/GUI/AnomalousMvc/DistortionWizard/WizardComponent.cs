using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class WizardComponent<WizardViewType> : LayoutComponent
        where WizardViewType : WizardView
    {
        private Layout layout;
        private Widget subLayoutWidget;
        private ScrollView panelScroll;
        protected WizardViewType wizardView;
        protected AnomalousMvcContext context;

        public WizardComponent(String layoutFile, WizardViewType view, AnomalousMvcContext context, MyGUIViewHost viewHost)
            :base("Medical.GUI.AnomalousMvc.DistortionWizard.WizardComponent.layout", viewHost)
        {
            this.wizardView = view;
            this.context = context;
            panelScroll = (ScrollView)widget.findWidget("PanelScroll");

            layout = LayoutManager.Instance.loadLayout(layoutFile);
            subLayoutWidget = layout.getWidget(0);

            if (view.AttachToScrollView)
            {
                subLayoutWidget.attachToWidget(panelScroll);
                panelScroll.CanvasSize = new Size2(subLayoutWidget.Width, subLayoutWidget.Height);
                subLayoutWidget.setPosition(0, 0);
            }
            else
            {
                subLayoutWidget.attachToWidget(widget);
                subLayoutWidget.setPosition(0, panelScroll.Top);
                subLayoutWidget.setSize(panelScroll.Width, panelScroll.Height);
                panelScroll.Visible = false;
            }
            subLayoutWidget.Align = Align.Stretch;

            Button cancelButton = (Button)widget.findWidget("StateWizardButtons/Cancel");
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            Button finishButton = (Button)widget.findWidget("StateWizardButtons/Finish");
            finishButton.MouseButtonClick += new MyGUIEvent(finishButton_MouseButtonClick);

            Button nextButton = (Button)widget.findWidget("StateWizardButtons/Next");
            nextButton.MouseButtonClick += new MyGUIEvent(nextButton_MouseButtonClick);

            Button previousButton = (Button)widget.findWidget("StateWizardButtons/Previous");
            previousButton.MouseButtonClick += new MyGUIEvent(previousButton_MouseButtonClick);
        }

        void previousButton_MouseButtonClick(Widget source, EventArgs e)
        {
            context.runAction(wizardView.PreviousAction, ViewHost);
        }

        void nextButton_MouseButtonClick(Widget source, EventArgs e)
        {
            context.runAction(wizardView.NextAction, ViewHost);
        }

        void finishButton_MouseButtonClick(Widget source, EventArgs e)
        {
            context.runAction(wizardView.FinishAction, ViewHost);
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            context.runAction(wizardView.CancelAction, ViewHost);
        }
    }
}
