﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using Medical.Controller.AnomalousMvc;
using Logging;

namespace Medical.GUI.AnomalousMvc
{
    class NavigationComponent : LayoutComponent
    {
        private ScrollView iconScrollView;
        private FlowLayoutContainer flowLayout = new FlowLayoutContainer(FlowLayoutContainer.LayoutType.Horizontal, 10.0f, new Vector2(4.0f, 10.0f));
        private List<NavigationButton> buttons = new List<NavigationButton>();
        private NavigationButton selectedButton;
        private NavigationView view;
        private AnomalousMvcContext context;
        private NavigationModel navModel;

        public NavigationComponent(NavigationView view, AnomalousMvcContext context, MyGUIViewHost viewHost)
            : base("Medical.GUI.AnomalousMvc.NavigationView.NavigationComponent.layout", viewHost)
        {
            this.view = view;
            this.context = context;

            iconScrollView = (ScrollView)widget;
            Size2 size = iconScrollView.CanvasSize;
            size.Width = 10;
            iconScrollView.CanvasSize = size;
        }

        public override void Dispose()
        {
            clearButtons();
            base.Dispose();
        }

        public override void opening()
        {
            navModel = context.getModel<NavigationModel>(view.NavigationModelName);
            if (navModel != null)
            {
                flowLayout.SuppressLayout = true;
                foreach (NavigationLink link in navModel.Links)
                {
                    addButton(link.Action, link.Text, link.Image);
                }
                flowLayout.SuppressLayout = false;
                flowLayout.invalidate();
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
        }

        void navModel_CurrentIndexChanged(NavigationModel navModel)
        {
            if (buttons.Count > 0)
            {
                if (selectedButton != null)
                {
                    selectedButton.Selected = false;
                }
                selectedButton = buttons[navModel.CurrentIndex];
                selectedButton.Selected = true;
            }
        }

        private void addButton(String action, String text, String imageKey)
        {
            Button button = iconScrollView.createWidgetT("Button", "VerticalIconTextButton", 0, 0, 78, 64, Align.Default, "") as Button;
            button.Caption = text;
            button.ForwardMouseWheelToParent = true;
            int captionWidth = (int)button.getTextSize().Width;
            button.setSize(captionWidth + 10, button.Height);
            button.ImageBox.setItemResource(imageKey);
            NavigationButton navButton = new NavigationButton(button);
            navButton.Clicked += new EventDelegate<NavigationButton>(navButton_Clicked);
            navButton.Action = action;
            navButton.Visible = true;
            flowLayout.addChild(navButton.Layout);
            buttons.Add(navButton);

            //Adjust scroll area size
            Size2 size = iconScrollView.CanvasSize;
            size.Width = flowLayout.DesiredSize.Width;
            iconScrollView.CanvasSize = size;
        }

        void navButton_Clicked(NavigationButton source)
        {
            navModel.CurrentIndex = buttons.IndexOf(source);
            context.runAction(source.Action, ViewHost);
        }

        private void clearButtons()
        {
            flowLayout.clearChildren();
            foreach (NavigationButton navButton in buttons)
            {
                navButton.Dispose();
            }
            buttons.Clear();
            selectedButton = null;
        }
    }
}
