using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.Controller.AnomalousMvc;

namespace Medical.GUI.AnomalousMvc
{
    class WizardPanel : MyGUIViewHost
    {
        private Layout layout;
        private Widget subLayoutWidget;
        private ScrollView panelScroll;
        private WizardView wizardView;
        protected AnomalousMvcContext context;

        public WizardPanel(String layoutFile, WizardView view, AnomalousMvcContext context)
            :base("Medical.GUI.AnomalousMvc.DistortionWizard.WizardPanel.layout")
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

            //int buttonAreaHeight = widget.Height;

            Button cancelButton = (Button)widget.findWidget("StateWizardButtons/Cancel");
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            Button finishButton = (Button)widget.findWidget("StateWizardButtons/Finish");
            finishButton.MouseButtonClick += new MyGUIEvent(finishButton_MouseButtonClick);
        }

        void finishButton_MouseButtonClick(Widget source, EventArgs e)
        {
            context.runAction(wizardView.FinishAction);
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            context.runAction(wizardView.CancelAction);
        }
    }
}
